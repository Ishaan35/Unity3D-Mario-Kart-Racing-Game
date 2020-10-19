#if UNITY_ANDROID
using System;
using UnityEngine;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Monetization
{
    public class AndroidPlacementContentOperations : IPlacementContentOperations
    {
        protected readonly AndroidJavaObject javaObject;

        public AndroidPlacementContentOperations(AndroidJavaObject javaObject)
        {
            this.javaObject = javaObject;
        }

        public bool ready => javaObject.Call<bool>("isReady");
        public PlacementContentState state => (PlacementContentState)JavaEnumUtilities.GetEnumOrdinal(javaObject.Call<AndroidJavaObject>("getState"));

        public void SendCustomEvent(CustomEvent customEvent)
        {
            using (var javaCustomEvent = CustomEventAndroidConverter.Convert(customEvent))
            {
                javaObject.Call("sendCustomEvent", javaCustomEvent);
            }
        }
    }

    class CustomEventAndroidConverter
    {
        private static readonly string CustomEventClass = "com.unity3d.services.monetization.placementcontent.core.CustomEvent";
        internal static AndroidJavaObject Convert(CustomEvent customEvent)
        {
            return new AndroidJavaObject(CustomEventClass, customEvent.category, customEvent.type, customEvent.data);
        }
    }

    public class AndroidRewardedOperations : AndroidPlacementContentOperations, IRewardedOperations
    {
        public AndroidRewardedOperations(AndroidJavaObject javaObject) : base(javaObject)
        {
        }

        public bool IsRewarded()
        {
            return javaObject.Call<bool>("isRewarded");
        }

        public string rewardId => javaObject.SafeStringCall("getRewardId");
    }

    internal class AndroidShowAdOperations : AndroidRewardedOperations, IShowAdOperations
    {
        private IUnityLifecycleManager callbackExecutor;
        public AndroidShowAdOperations(IUnityLifecycleManager callbackExecutor, AndroidJavaObject javaObject) : base(javaObject)
        {
            this.callbackExecutor = callbackExecutor;
        }

        class AndroidShowAdCallback : AndroidJavaProxy
        {
            private ShowAdCallbacks? callbacks;
            private IUnityLifecycleManager callbackExecutor;
            public AndroidShowAdCallback(IUnityLifecycleManager callbackExecutor, ShowAdCallbacks? callbacks) : base("com.unity3d.services.monetization.placementcontent.ads.IShowAdListener")
            {
                this.callbackExecutor = callbackExecutor;
                this.callbacks = callbacks;
            }

            void onAdFinished(string placementId, AndroidJavaObject withState)
            {
                callbackExecutor.Post(() =>
                {
                    if (callbacks?.finishCallback != null)
                    {
                        var showResult = (ShowResult)withState.Call<int>("ordinal");
                        callbacks?.finishCallback?.Invoke(showResult);
                    }
                });
            }

            void onAdStarted(string placementId)
            {
                if (callbacks?.startCallback != null)
                {
                    callbacks?.startCallback?.Invoke();
                }
            }
        }

        public void Show(ShowAdCallbacks? callbacks)
        {
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject m_CurrentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            javaObject.Call("show", m_CurrentActivity, new AndroidShowAdCallback(callbackExecutor, callbacks));
        }
    }

    internal class AndroidPromoAdOperations : AndroidShowAdOperations, IPromoAdOperations
    {
        public AndroidPromoAdOperations(IUnityLifecycleManager callbackExecutor, AndroidJavaObject javaObject) : base(callbackExecutor, javaObject)
        {
            metadata = GetMetadataForJavaObject(javaObject.Call<AndroidJavaObject>("getMetadata"));
            nativeAdapter = new AndroidJavaObject("com.unity3d.services.monetization.placementcontent.purchasing.NativePromoAdapter", javaObject);
        }

        public PromoMetadata metadata { get; }

        internal AndroidJavaObject nativeAdapter { get; }

        private PromoMetadata GetMetadataForJavaObject(AndroidJavaObject metadataJavaObject)
        {
            var promoMetadata = new PromoMetadata
            {
                impressionDate = GetDateTimeFromJavaDate(metadataJavaObject.Call<AndroidJavaObject>("getImpressionDate")),
                offerDuration = TimeSpan.FromSeconds(metadataJavaObject.Call<long>("getOfferDuration")),
                costs = GetItemArrayFromJavaArray(metadataJavaObject.Call<AndroidJavaObject>("getCosts")),
                payouts = GetItemArrayFromJavaArray(metadataJavaObject.Call<AndroidJavaObject>("getPayouts")),
                premiumProduct = GetPremiumProductFromJavaObject(metadataJavaObject.Call<AndroidJavaObject>("getPremiumProduct"))
            };
            return promoMetadata;
        }

        private Product GetPremiumProductFromJavaObject(AndroidJavaObject productJavaObject)
        {
            if (productJavaObject == null)
            {
                return default(Product);
            }

            return new Product
            {
                productId = productJavaObject.SafeStringCall("getProductId"),
                localizedPrice = (decimal)productJavaObject.Call<double>("getLocalizedPrice"),
                isoCurrencyCode = productJavaObject.SafeStringCall("getIsoCurrencyCode"),
                localizedPriceString = productJavaObject.SafeStringCall("getLocalizedPriceString"),
                localizedTitle = productJavaObject.SafeStringCall("getLocalizedTitle"),
                localizedDescription = productJavaObject.SafeStringCall("getLocalizedDescription"),
                productType = productJavaObject.SafeStringCall("getProductType")
            };
        }

        private PromoItem[] GetItemArrayFromJavaArray(AndroidJavaObject javaArray)
        {
            if (javaArray == null)
            {
                return default(PromoItem[]);
            }

            var size = javaArray.Call<int>("size");
            var items = new PromoItem[size];
            for (var i = 0; i < size; i++)
            {
                items[i] = GetItemFromJavaObject(javaArray.Call<AndroidJavaObject>("get", i));
            }

            return items;
        }

        private PromoItem GetItemFromJavaObject(AndroidJavaObject itemJavaObject)
        {
            return new PromoItem
            {
                productId = itemJavaObject.SafeStringCall("getItemId"),
                itemType = itemJavaObject.SafeStringCall("getType"),
                quantity = itemJavaObject.Call<long>("getQuantity")
            };
        }

        private DateTime GetDateTimeFromJavaDate(AndroidJavaObject dateJavaObject)
        {
            if (dateJavaObject == null)
            {
                return default(DateTime);
            }

            var millisecs = dateJavaObject.Call<long>("getTime");
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(millisecs);
        }
    }
}
#endif
