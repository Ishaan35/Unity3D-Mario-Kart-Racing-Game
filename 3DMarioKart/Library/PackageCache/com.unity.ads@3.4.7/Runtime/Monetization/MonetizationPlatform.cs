#if UNITY_EDITOR
using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
#elif UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine.Advertisements.Purchasing;
#elif UNITY_IOS
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Advertisements.Purchasing;
#endif

using UnityEngine.Advertisements;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Monetization
{
#if UNITY_EDITOR
    sealed internal class Platform : IMonetizationPlatform
    {
        public static Placeholder m_Placeholder;
        bool isInitialized;
        Configuration m_Configuration;

        static string s_BaseUrl = "http://editor-support.unityads.unity3d.com/games";
        static string s_Version = "3.4.6";
        private UnityLifecycleManager _callbackExecutor;

        public event EventHandler<PlacementContentReadyEventArgs> OnPlacementContentReady;
        public event EventHandler<PlacementContentStateChangeEventArgs> OnPlacementContentStateChange
        {
            add {}
            remove {}
        }
        public event EventHandler<UnityServicesErrorEventArgs> onError
        {
            add {}
            remove {}
        }

        public string version { get; } = "3.4.6";

        public Platform()
        {
            var callbackExecutorGameObject = new GameObject("UnityAdsCallbackExecutorObject")
            {
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector
            };
            _callbackExecutor = new UnityLifecycleManager();
            Object.DontDestroyOnLoad(callbackExecutorGameObject);
        }

        public void Initialize(string gameId, bool testMode)
        {
            var placeHolderGameObject = new GameObject("UnityMonetizationEditorPlaceHolderObject")
            {
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector
            };
            m_Placeholder = placeHolderGameObject.AddComponent<Placeholder>();

            string configurationUrl = string.Join("/", new string[]
            {
                s_BaseUrl,
                gameId,
                string.Join("&", new string[]
                {
                    "configuration?platform=editor",
                    "unityVersion=" + Uri.EscapeDataString(Application.unityVersion),
                    "sdkVersionName=" + Uri.EscapeDataString(s_Version)
                })
            });

            WebRequest request = WebRequest.Create(configurationUrl);
            request.BeginGetResponse(result =>
            {
                WebResponse response = request.EndGetResponse(result);
                var reader = new StreamReader(response.GetResponseStream());
                string responseBody = reader.ReadToEnd();
                try
                {
                    m_Configuration = new Configuration(responseBody);
                    if (!m_Configuration.enabled)
                    {
                        Debug.LogWarning("gameId " + gameId + " is not enabled");
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError("Failed to parse configuration for gameId: " + gameId);
                    Debug.Log(responseBody);
                    Debug.LogError(exception.Message);
                }
                reader.Close();
                response.Close();
                if (m_Configuration != null)
                {
                    foreach (KeyValuePair<string, PlacementContent> entry in m_Configuration.placementContents)
                    {
                        _callbackExecutor.Post(() =>
                        {
                            OnPlacementContentReady?.Invoke(this, new PlacementContentReadyEventArgs(entry.Key, entry.Value));
                        });
                    }
                }
            }, null);
            isInitialized = true;
            UnityEngine.Advertisements.Advertisement.Initialize(gameId, testMode);
            Debug.Log("UnityMonetizationEditor: Initialize(" + gameId + ", " + testMode + ");");
        }

        public PlacementContent GetPlacementContent(string placementID)
        {
            if (m_Configuration != null && m_Configuration.placementContents.ContainsKey(placementID))
            {
                return m_Configuration.placementContents[placementID];
            }
            else
            {
                return new ShowAdPlacementContent("fakeId", new EditorShowAdOperations());
            }
        }

        public INativePromoAdapter CreateNativePromoAdapter(PromoAdPlacementContent placementContent)
        {
            return new EditorNativePromoAdapter(placementContent);
        }

        public void SetMetaData(MetaData metaData)
        {
        }

        public void SetPurchasingAdapter(IPurchasingAdapter adapter)
        {
        }

        public bool isSupported
        {
            get
            {
                return Application.isEditor;
            }
        }

        public bool IsReady(string placementID)
        {
            return isInitialized && m_Configuration != null;
        }
    }
#elif UNITY_ANDROID
    sealed internal class Platform : AndroidJavaProxy, IMonetizationPlatform, IPurchasingEventSender
    {
        private static readonly IDictionary<string, PlacementContentType> PlacementContentTypesMap = new Dictionary<string, PlacementContentType>
        {
            {"SHOW_AD", PlacementContentType.ShowAd},
            {"PROMO_AD", PlacementContentType.PromoAd},
            {"SINK_PROMO", PlacementContentType.SinkPromo},
            {"CUSTOM", PlacementContentType.Custom}
        };

        public event EventHandler<PlacementContentReadyEventArgs> OnPlacementContentReady;
        public event EventHandler<PlacementContentStateChangeEventArgs> OnPlacementContentStateChange;
        public event EventHandler<UnityServicesErrorEventArgs> onError;

        readonly AndroidJavaObject m_CurrentActivity;
        readonly IUnityLifecycleManager m_CallbackExecutor;
        readonly Purchase m_UnityMonetizationPurchase;
        readonly AndroidAnalytics m_UnityMonetizationAnalytics;

        private IDictionary<string, PlacementContent> m_PlacementContents = new Dictionary<string, PlacementContent>();
        private AndroidJavaClass m_unityMonetizationClass = new AndroidJavaClass("com.unity3d.services.monetization.UnityMonetization");
        private AndroidJavaClass m_unityServicesClass = new AndroidJavaClass("com.unity3d.services.UnityServices");
        private AndroidJavaClass m_unityPurchasingClass = new AndroidJavaClass("com.unity3d.services.purchasing.UnityPurchasing");

        public Platform() : base("com.unity3d.services.monetization.IUnityMonetizationListener")
        {
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            this.m_CurrentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            this.m_UnityMonetizationPurchase = new Purchase();
            this.m_UnityMonetizationAnalytics = new AndroidAnalytics();

            m_CallbackExecutor = new UnityLifecycleManager();
        }

        void onPlacementContentReady(string placementId, AndroidJavaObject placementcontent)
        {
            PlacementContentType type = this.getPlacementContentTypeForAndroidObject(placementcontent);
            IPlacementContentOperations operations = this.getPlacementContentOperationsForType(type, placementcontent);
            PlacementContent m_placementcontent = this.getPlacementContentForType(type, placementId, operations);
            m_placementcontent.extras = JavaMapUtilities.GetDictionaryForJavaMap(placementcontent.Call<AndroidJavaObject>("getExtras"));

            if (m_PlacementContents.ContainsKey(placementId))
            {
                m_PlacementContents.Remove(placementId);
            }
            m_PlacementContents.Add(placementId, m_placementcontent);

            m_CallbackExecutor.Post(() =>
            {
                OnPlacementContentReady?.Invoke(this, new PlacementContentReadyEventArgs(placementId, m_placementcontent));
            });
        }

        private IPlacementContentOperations getPlacementContentOperationsForType(PlacementContentType type, AndroidJavaObject placementcontent)
        {
            switch (type)
            {
                case PlacementContentType.ShowAd:
                    return new AndroidShowAdOperations(m_CallbackExecutor, placementcontent);
                case PlacementContentType.PromoAd:
                    return new AndroidPromoAdOperations(m_CallbackExecutor, placementcontent);
                default:
                    return new AndroidPlacementContentOperations(placementcontent);
            }
        }

        private PlacementContent getPlacementContentForType(PlacementContentType type, string placementId, IPlacementContentOperations operations)
        {
            switch (type)
            {
                case PlacementContentType.ShowAd:
                    return new ShowAdPlacementContent(placementId, operations as IShowAdOperations);
                case PlacementContentType.PromoAd:
                    return new PromoAdPlacementContent(placementId, operations as IPromoAdOperations);
                default:
                    return new PlacementContent(placementId, operations);
            }
        }

        private PlacementContentState getPlacementContentStateForJavaPlacementContentState(AndroidJavaObject javaObject)
        {
            return (PlacementContentState)JavaEnumUtilities.GetEnumOrdinal(javaObject);
        }

        void onPlacementContentStateChange(string placementId, AndroidJavaObject placementcontent, AndroidJavaObject previousState, AndroidJavaObject newState)
        {
            if (m_PlacementContents.ContainsKey(placementId))
            {
                PlacementContent m_placementContent = m_PlacementContents[placementId];
                PlacementContentState m_previousState = this.getPlacementContentStateForJavaPlacementContentState(previousState);
                PlacementContentState m_newState = this.getPlacementContentStateForJavaPlacementContentState(newState);

                m_CallbackExecutor.Post(() =>
                {
                    OnPlacementContentStateChange?.Invoke(this,
                        new PlacementContentStateChangeEventArgs(placementId, m_placementContent, m_previousState, m_newState));
                });
            }
        }

        void onUnityServicesError(AndroidJavaObject error, string message)
        {
            var errOrdinal = error?.Call<int>("ordinal") ?? -1;
            m_CallbackExecutor.Post(() =>
            {
                onError?.Invoke(this, new UnityServicesErrorEventArgs(errOrdinal, message));
            });
        }

        public void Initialize(string gameId, bool testMode)
        {
            m_UnityMonetizationPurchase.Initialize(this);
            m_UnityMonetizationAnalytics.Initialize();
            m_unityMonetizationClass.CallStatic("initialize", this.m_CurrentActivity, gameId, this, testMode);
        }

        public void SetPurchasingAdapter(IPurchasingAdapter adapter)
        {
            m_unityPurchasingClass.CallStatic("setAdapter", new AndroidPurchasingAdapter(adapter));
        }

        public string version => m_unityServicesClass.CallStatic<string>("getVersion");
        public bool isSupported => m_unityServicesClass.CallStatic<bool>("isSupported");

        private PlacementContentType getPlacementContentTypeForAndroidObject(AndroidJavaObject javaObject)
        {
            var placementcontentType = javaObject.SafeStringCall("getType");
            if (PlacementContentTypesMap.ContainsKey(placementcontentType))
            {
                return PlacementContentTypesMap[placementcontentType];
            }
            return PlacementContentType.Custom;
        }

        public bool IsReady(string placementID)
        {
            return m_unityMonetizationClass.CallStatic<bool>("isReady", placementID);
        }

        public PlacementContent GetPlacementContent(string placementID)
        {
            if (m_PlacementContents.ContainsKey(placementID))
            {
                return m_PlacementContents[placementID];
            }

            return null;
        }

        public INativePromoAdapter CreateNativePromoAdapter(PromoAdPlacementContent placementContent)
        {
            return new AndroidNativePromoAdapter(placementContent);
        }

        public void SetMetaData(MetaData metaData)
        {
            var metaDataObject = new AndroidJavaObject("com.unity3d.ads.metadata.MetaData", m_CurrentActivity);
            metaDataObject.Call("setCategory", metaData.category);
            foreach (var entry in metaData)
            {
                metaDataObject.Call<bool>("set", entry.Key, entry.Value);
            }
            metaDataObject.Call("commit");
        }

        void IPurchasingEventSender.SendPurchasingEvent(string payload)
        {
            m_UnityMonetizationPurchase.SendEvent(payload);
        }

        private class AndroidPurchasingAdapter : AndroidJavaProxy
        {
            private IPurchasingAdapter adapter;

            public AndroidPurchasingAdapter(IPurchasingAdapter adapter) : base("com.unity3d.services.purchasing.core.IPurchasingAdapter")
            {
                this.adapter = adapter;
            }

            public void retrieveProducts(AndroidJavaObject listener)
            {
                this.adapter.RetrieveProducts(new AndroidRetrieveProductsListener(listener));
            }

            public void onPurchase(string productID, AndroidJavaObject javaListener, AndroidJavaObject javaExtras)
            {
                IDictionary<string, object> extras = JavaMapUtilities.GetDictionaryForJavaMap(javaExtras);
                this.adapter.Purchase(productID, new AndroidTransactionListener(javaListener), extras);
            }

            private class AndroidRetrieveProductsListener : IRetrieveProductsListener
            {
                private AndroidJavaObject listener;

                public AndroidRetrieveProductsListener(AndroidJavaObject listener)
                {
                    this.listener = listener;
                }

                public void OnProductsRetrieved(ICollection<Product> products)
                {
                    var productsList = this.getJavaProductList(products);
                    listener.Call("onProductsRetrieved", productsList);
                }

                private AndroidJavaObject getJavaProductList(ICollection<Product> products)
                {
                    AndroidJavaObject javaProductList = new AndroidJavaObject("java.util.ArrayList");
                    foreach (var product in products)
                    {
                        javaProductList.Call<bool>("add", this.getJavaProduct(product));
                    }
                    return javaProductList;
                }

                private AndroidJavaObject getJavaProduct(Product product)
                {
                    AndroidJavaObject builder = new AndroidJavaClass("com.unity3d.services.purchasing.core.Product")
                        .CallStatic<AndroidJavaObject>("newBuilder");
                    builder.Call<AndroidJavaObject>("withProductId", product.productId);
                    builder.Call<AndroidJavaObject>("withLocalizedPriceString", product.localizedPriceString);
                    builder.Call<AndroidJavaObject>("withLocalizedTitle", product.localizedTitle);
                    builder.Call<AndroidJavaObject>("withIsoCurrencyCode", product.isoCurrencyCode);
                    builder.Call<AndroidJavaObject>("withLocalizedPrice", (double)product.localizedPrice);
                    builder.Call<AndroidJavaObject>("withLocalizedDescription", product.localizedDescription);
                    builder.Call<AndroidJavaObject>("withProductType", product.productType);
                    return builder.Call<AndroidJavaObject>("build");
                }
            }

            private class AndroidTransactionListener : ITransactionListener
            {
                private AndroidJavaObject listener;

                public AndroidTransactionListener(AndroidJavaObject listener)
                {
                    this.listener = listener;
                }

                public void OnTransactionComplete(TransactionDetails details)
                {
                    listener.Call("onTransactionComplete", AndroidNativePromoAdapter.getJavaTransactionDetails(details));
                }

                public void OnTransactionError(TransactionErrorDetails details)
                {
                    listener.Call("onTransactionError", AndroidNativePromoAdapter.getJavaTransactionErrorDetails(details));
                }
            }
        }
    }
#elif UNITY_IOS
    public class Platform : IMonetizationPlatform
    {
        private static readonly IDictionary<string, PlacementContentType> PlacementContentTypesMap = new Dictionary<string, PlacementContentType>
        {
            {"SHOW_AD", PlacementContentType.ShowAd},
            {"PROMO_AD", PlacementContentType.PromoAd},
            {"SINK_PROMO", PlacementContentType.SinkPromo},
            {"CUSTOM", PlacementContentType.Custom}
        };

        [DllImport("__Internal")]
        static extern bool UnityMonetizationIsReady(string placementId);
        [DllImport("__Internal")]
        static extern void UnityMonetizationInitialize(string gameId, bool isTestMode);
        [DllImport("__Internal")]
        static extern bool UnityMonetizationIsSupported();
        [DllImport("__Internal")]
        static extern string UnityMonetizationGetPlacementContentType(IntPtr pPlacementContent);

        [DllImport("__Internal")]
        [return : MarshalAs(UnmanagedType.LPWStr)]
        static extern string UnityMonetizationGetPlacementContentExtras(IntPtr pPlacementContent);
        [DllImport("__Internal")]
        static extern void UnityAdsSetMetaData(string category, string data);
        [DllImport("__Internal")]
        static extern string UnityAdsGetVersion();

        private readonly IUnityLifecycleManager _callbackExecutor;
        private readonly IDictionary<string, PlacementContent> _placementContents = new Dictionary<string, PlacementContent>();

        public event EventHandler<PlacementContentReadyEventArgs> OnPlacementContentReady;
        public event EventHandler<PlacementContentStateChangeEventArgs> OnPlacementContentStateChange;
        public event EventHandler<UnityServicesErrorEventArgs> onError;

        public Platform()
        {
            _callbackExecutor = new UnityLifecycleManager();
        }

        public void Initialize(string gameId, bool testMode)
        {
            PlatformCallbacksWrapper.Platform = this;
            new IosAnalytics().Initialize();
            new PurchasingPlatform().Initialize();
            UnityMonetizationInitialize(gameId, testMode);
        }

        public bool IsReady(string placementId)
        {
            return UnityMonetizationIsReady(placementId);
        }

        public void SetPurchasingAdapter(IPurchasingAdapter adapter)
        {
            PurchasingAdapter.Adapter = adapter;
        }

        public PlacementContent GetPlacementContent(string placementId)
        {
            if (_placementContents.ContainsKey(placementId))
            {
                return _placementContents[placementId];
            }

            return null;
        }

        public INativePromoAdapter CreateNativePromoAdapter(PromoAdPlacementContent placementContent)
        {
            return new IosNativePromoAdapter(placementContent);
        }

        public void SetMetaData(MetaData metaData)
        {
            UnityAdsSetMetaData(metaData.category, metaData.ToJSON());
        }

        public bool isSupported => UnityMonetizationIsSupported();
        public string version => UnityAdsGetVersion();

        private void OnNativePlacementContentReady(string placementId, IntPtr pPlacementContent)
        {
            var type = GetPlacementContentTypeForPlacementContentPtr(pPlacementContent);
            var operations = GetPlacementContentOperationsForType(type, pPlacementContent);
            var placementContent = GetPlacementContentForType(type, placementId, operations);
            var extras = GetPlacementContentExtras(pPlacementContent);
            placementContent.extras = extras;

            if (_placementContents.ContainsKey(placementId))
            {
                _placementContents.Remove(placementId);
            }
            _placementContents.Add(placementId, placementContent);

            _callbackExecutor.Post(() =>
            {
                OnPlacementContentReady?.Invoke(this, new PlacementContentReadyEventArgs(placementId, placementContent));
            });
        }

        private IDictionary<string, object> GetPlacementContentExtras(IntPtr pPlacementContent)
        {
            var json = UnityMonetizationGetPlacementContentExtras(pPlacementContent);
            if (json == null)
            {
                return new Dictionary<string, object>();
            }

            var deserialized = MiniJSON.Json.Deserialize(json);
            if (deserialized is IDictionary<string, object> objects)
            {
                return objects;
            }

            return new Dictionary<string, object>();
        }

        private static PlacementContentType GetPlacementContentTypeForPlacementContentPtr(IntPtr pPlacementContent)
        {
            string type = UnityMonetizationGetPlacementContentType(pPlacementContent);
            return PlacementContentTypesMap[type];
        }

        private IPlacementContentOperations GetPlacementContentOperationsForType(PlacementContentType type, IntPtr pPlacementContent)
        {
            switch (type)
            {
                case PlacementContentType.ShowAd:
                    return new IosShowAdOperations(pPlacementContent, _callbackExecutor);
                case PlacementContentType.PromoAd:
                    return new IosPromoAdOperations(pPlacementContent, _callbackExecutor);
                default:
                    return new IosPlacementContentOperations(pPlacementContent);
            }
        }

        private PlacementContent GetPlacementContentForType(PlacementContentType type, string placementId, IPlacementContentOperations operations)
        {
            switch (type)
            {
                case PlacementContentType.ShowAd:
                    return new ShowAdPlacementContent(placementId, operations as IShowAdOperations);
                case PlacementContentType.PromoAd:
                    return new PromoAdPlacementContent(placementId, operations as IPromoAdOperations);
                default:
                    return new PlacementContent(placementId, operations);
            }
        }

        internal void OnNativePlacementContentStateChanged(string placementId, IntPtr pPlacementContent, int previousState, int newState)
        {
            if (_placementContents.ContainsKey(placementId))
            {
                var placementContent = _placementContents[placementId];
                _callbackExecutor.Post(() =>
                {
                    OnPlacementContentStateChange?.Invoke(this, new PlacementContentStateChangeEventArgs(placementId, placementContent, (PlacementContentState)previousState, (PlacementContentState)newState));
                });
            }
        }

        private void OnNativeError(long error, string message)
        {
            onError?.Invoke(this, new UnityServicesErrorEventArgs(error, message));
        }

        private static class PlatformCallbacksWrapper
        {
            internal static Platform Platform { get; set; }

            delegate void OnPlacementContentReadyCallback(string placementId, IntPtr placementContent);
            delegate void OnPlacementContentStateChangedCallback(string placementId, IntPtr placementContent, int previousState, int newState);
            delegate void OnErrorCallback(long error, string message);

            [StructLayout(LayoutKind.Sequential)]
            struct UnityMonetiztionCallbacks
            {
                public OnPlacementContentReadyCallback onPlacementContentReadyCallback;
                public OnPlacementContentStateChangedCallback onPlacementContentStateChangedCallback;
                public OnErrorCallback onErrorCallback;
            }

            [DllImport("__Internal")]
            private static extern void UnityMonetizationSetMonetizationCallbacks(ref UnityMonetiztionCallbacks callback);

            static PlatformCallbacksWrapper()
            {
                var callbacks = new UnityMonetiztionCallbacks
                {
                    onPlacementContentReadyCallback = OnPlacementContentReady,
                    onPlacementContentStateChangedCallback = OnPlacementContentChanged,
                    onErrorCallback = OnError
                };
                UnityMonetizationSetMonetizationCallbacks(ref callbacks);
            }

            [MonoPInvokeCallback(typeof(OnPlacementContentReadyCallback))]
            private static void OnPlacementContentReady(string placementId, IntPtr placementContent)
            {
                Platform?.OnNativePlacementContentReady(placementId, placementContent);
            }

            [MonoPInvokeCallback(typeof(OnPlacementContentStateChangedCallback))]
            private static void OnPlacementContentChanged(string placementId, IntPtr placementContent, int previousState, int newState)
            {
                Platform?.OnNativePlacementContentStateChanged(placementId, placementContent, previousState, newState);
            }

            [MonoPInvokeCallback(typeof(OnPlacementContentStateChangedCallback))]
            private static void OnError(long error, string message)
            {
                Platform?.OnNativeError(error, message);
            }
        }

        private static class PurchasingAdapter
        {
            internal static IPurchasingAdapter Adapter { private get; set; }

            delegate void OnRetrieveProductsCallback(IntPtr listener);
            delegate void OnPurchaseProductCallback(string productId, IntPtr callbacks);

            [StructLayout(LayoutKind.Sequential)]
            struct UnityPurchasingCallbacks
            {
                public OnRetrieveProductsCallback onRetrieveProductsCallback;
                public OnPurchaseProductCallback onPurchaseProductCallback;
            }

            [DllImport("__Internal")]
            private static extern void UnityPurchasingSetPurchasingAdapterCallbacks(ref UnityPurchasingCallbacks callback);

            static PurchasingAdapter()
            {
                var callbacks = new UnityPurchasingCallbacks
                {
                    onRetrieveProductsCallback = OnRetrieveProducts,
                    onPurchaseProductCallback = OnPurchaseProduct
                };
                UnityPurchasingSetPurchasingAdapterCallbacks(ref callbacks);
            }

            [MonoPInvokeCallback(typeof(OnRetrieveProductsCallback))]
            private static void OnRetrieveProducts(IntPtr pCallback)
            {
                Adapter?.RetrieveProducts(new IosRetrieveProductsListener(pCallback));
            }

            [MonoPInvokeCallback(typeof(OnPurchaseProductCallback))]
            private static void OnPurchaseProduct(string productId, IntPtr pCallbacks)
            {
                Adapter?.Purchase(productId, new IosTransactionListener(pCallbacks), new Dictionary<string, object>());
            }
        }

        private class IosRetrieveProductsListener : IRetrieveProductsListener
        {
            // Note, this struct layout matches UnityMonetizationPurchasingAdapter.mm
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct NativeProduct
            {
                [MarshalAs(UnmanagedType.LPWStr)]
                public string productId;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string localizedTitle;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string localizedDescription;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string localizedPriceString;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string isoCurrencyCode;
                [MarshalAs(UnmanagedType.LPWStr)]
                public string productType;
                [MarshalAs(UnmanagedType.R8)]
                public double localizedPrice;
            }

            [DllImport("__Internal")]
            private static extern IntPtr UnityPurchasingAdapterAllocateProductsArray(int num);
            [DllImport("__Internal")]
            private static extern void UnityPurchasingAddItemToProductsArray(IntPtr pArray, ref NativeProduct product);
            [DllImport("__Internal")]
            private static extern void UnityPurchasingInvokeRetrieveProductsCallback(IntPtr callback, IntPtr pArray);

            private readonly IntPtr _pCallback;
            public IosRetrieveProductsListener(IntPtr callback)
            {
                _pCallback = callback;
            }

            public void OnProductsRetrieved(ICollection<Product> products)
            {
                var pProducts = UnityPurchasingAdapterAllocateProductsArray(products.Count);
                foreach (var product in products)
                {
                    var nativeProduct = new NativeProduct
                    {
                        productId = product.productId,
                        localizedTitle = product.localizedTitle,
                        localizedDescription = product.localizedDescription,
                        localizedPriceString = product.localizedPriceString,
                        isoCurrencyCode = product.isoCurrencyCode,
                        productType = product.productType,
                        localizedPrice = (double)product.localizedPrice
                    };
                    UnityPurchasingAddItemToProductsArray(pProducts, ref nativeProduct);
                }
                UnityPurchasingInvokeRetrieveProductsCallback(_pCallback, pProducts);
            }
        }

        private class IosTransactionListener : ITransactionListener
        {
            [DllImport("__Internal")]
            private static extern void UnityPurchasingInvokeTransactionCompleteCallback(IntPtr pCallbacks, [MarshalAs(UnmanagedType.LPWStr)] string transactionDetails);

            [DllImport("__Internal")]
            private static extern void UnityPurchasingInvokeTransactionErrorCallback(IntPtr pCallbacks, [MarshalAs(UnmanagedType.LPWStr)] string transactionErrorDetails);

            private readonly IntPtr _pCallbacks;

            public IosTransactionListener(IntPtr pCallbacks)
            {
                _pCallbacks = pCallbacks;
            }

            public void OnTransactionComplete(TransactionDetails details)
            {
                UnityPurchasingInvokeTransactionCompleteCallback(_pCallbacks, MiniJSON.Json.Serialize(details.ToJsonDictionary()));
            }

            public void OnTransactionError(TransactionErrorDetails details)
            {
                UnityPurchasingInvokeTransactionErrorCallback(_pCallbacks, MiniJSON.Json.Serialize(details.ToJsonDictionary()));
            }
        }
    }
#endif
}
