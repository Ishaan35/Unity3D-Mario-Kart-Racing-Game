#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Monetization
{
    internal class IosPlacementContentOperations : IPlacementContentOperations
    {
        [DllImport("__Internal")]
        static extern bool UnityMonetizationPlacementContentIsReady(IntPtr pPlacementContent);
        [DllImport("__Internal")]
        static extern bool UnityMonetizationPlacementContentSendCustomEvent(IntPtr pPlacementContent, [MarshalAs(UnmanagedType.LPWStr)] string customEventJson);
        [DllImport("__Internal")]
        static extern void UnityMonetizationPlacementContentReleaseReference(IntPtr pPlacementContentPtr);
        [DllImport("__Internal")]
        static extern int UnityMonetizationGetPlacementContentState(IntPtr pPlacementContentPtr);

        internal IntPtr placementContentPtr { get; }

        public IosPlacementContentOperations(IntPtr placementContentPtr)
        {
            this.placementContentPtr = placementContentPtr;
        }

        ~IosPlacementContentOperations()
        {
            UnityMonetizationPlacementContentReleaseReference(placementContentPtr);
        }

        public void SendCustomEvent(CustomEvent customEvent)
        {
            var eventJson = MiniJSON.Json.Serialize(customEvent.dictionaryValue);
            UnityMonetizationPlacementContentSendCustomEvent(placementContentPtr, eventJson);
        }

        public bool ready => UnityMonetizationPlacementContentIsReady(placementContentPtr);
        public PlacementContentState state => (PlacementContentState)UnityMonetizationGetPlacementContentState(placementContentPtr);
    }

    internal class IosRewardedOperations : IosPlacementContentOperations, IRewardedOperations
    {
        [DllImport("__Internal")]
        static extern bool UnityMonetizationPlacementContentIsRewarded(IntPtr pPlacementContent);
        [DllImport("__Internal")]
        static extern string UnityMonetizationPlacementContentGetRewardId(IntPtr pPlacementContent);

        public IosRewardedOperations(IntPtr placementContentPtr) : base(placementContentPtr)
        {
        }

        public bool IsRewarded()
        {
            return UnityMonetizationPlacementContentIsRewarded(placementContentPtr);
        }

        public string rewardId => UnityMonetizationPlacementContentGetRewardId(placementContentPtr);
    }

    internal class IosShowAdOperations : IosRewardedOperations, IShowAdOperations
    {
        [DllImport("__Internal")]
        static extern void UnityMonetizationPlacementContentShowAd(IntPtr pPlacementContent, ShowAdStartCallback startCallback, ShowAdFinishCallback finishCallback);

        class AdFinishedEventArgs : EventArgs
        {
            public AdFinishedEventArgs(ShowResult result)
            {
                this.result = result;
            }

            public ShowResult result { get; }
        }
        private static event EventHandler<AdFinishedEventArgs> OnAdFinished;
        private static event EventHandler<EventArgs> OnAdStarted;
        private readonly IUnityLifecycleManager _executor;
        private ShowAdCallbacks? _showAdCallbacks;

        public IosShowAdOperations(IntPtr placementContentPtr, IUnityLifecycleManager executor) : base(placementContentPtr)
        {
            _executor = executor;
        }

        void StartHandler(object sender, EventArgs e)
        {
            OnAdStarted -= StartHandler;
            _showAdCallbacks?.startCallback();
        }

        void FinishHandler(object sender, AdFinishedEventArgs e)
        {
            OnAdFinished -= FinishHandler;
            _executor.Post(() => { _showAdCallbacks?.finishCallback(e.result); });
        }

        public void Show(ShowAdCallbacks? callbacks)
        {
            // It might be weird to see such an indirect way to call this handler.
            // However, we do this to prevent the need for developers to use MonoPInvokeCallback with their
            // delegate. This provides a cleaner abstraction for developers at the cost of our own.
            if (callbacks?.startCallback != null)
            {
                _showAdCallbacks = callbacks;
                OnAdStarted += StartHandler;
            }

            if (callbacks?.finishCallback != null)
            {
                _showAdCallbacks = callbacks;
                OnAdFinished += FinishHandler;
            }
            UnityMonetizationPlacementContentShowAd(placementContentPtr, OnAdStartedCallback, OnAdFinishedCallback);
        }

        [MonoPInvokeCallback(typeof(ShowAdFinishCallback))]
        public static void OnAdFinishedCallback(ShowResult result)
        {
            OnAdFinished?.Invoke(null, new AdFinishedEventArgs(result));
        }

        [MonoPInvokeCallback(typeof(ShowAdStartCallback))]
        public static void OnAdStartedCallback()
        {
            OnAdStarted?.Invoke(null, new EventArgs());
        }
    }

    internal class IosPromoAdOperations : IosShowAdOperations, IPromoAdOperations
    {
        [DllImport("__Internal")]
        [return : MarshalAs(UnmanagedType.LPWStr)]
        static extern string UnityMonetizationGetPromoAdMetadata(IntPtr pPlacementContent);

        public IosPromoAdOperations(IntPtr placementContentPtr, IUnityLifecycleManager executor) : base(placementContentPtr, executor)
        {
            metadata = GetMetadataForObjCObject(placementContentPtr);
        }

        public PromoMetadata metadata { get; }

        private static PromoMetadata GetMetadataForObjCObject(IntPtr intPtr)
        {
            var metadataJson = UnityMonetizationGetPromoAdMetadata(intPtr);
            return PromoMetadataJsonDeserializer.Deserialize(metadataJson);
        }
    }

    internal static class PromoMetadataJsonDeserializer
    {
        public static PromoMetadata Deserialize(string jsonString)
        {
            var json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonString);
            return new PromoMetadata
            {
                impressionDate = GetImpressionDateTime(json["impressionDate"]),
                offerDuration = GetOfferDuration(json["offerDuration"]),
                costs = GetItemArray(json["costs"]),
                payouts = GetItemArray(json["payouts"]),
                premiumProduct = GetProductFromDictionary(json["premiumProduct"])
            };
        }

        private static TimeSpan GetOfferDuration(object rawOfferDuration)
        {
            if (rawOfferDuration is long offerDuration)
            {
                return TimeSpan.FromSeconds(offerDuration);
            }

            return default(TimeSpan);
        }

        private static DateTime GetImpressionDateTime(object rawDateTime)
        {
            if (rawDateTime is long millisecs)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddMilliseconds(millisecs);
            }

            return default(DateTime);
        }

        private static PromoItem[] GetItemArray(object rawArray)
        {
            if (rawArray is List<object> rawItems)
            {
                var items = new PromoItem[rawItems.Count];
                for (var i = 0; i < rawItems.Count; i++)
                {
                    items[i] = GetItemFromDictionary(rawItems[i]);
                }

                return items;
            }
            return new PromoItem[] {};
        }

        private static PromoItem GetItemFromDictionary(object rawItem)
        {
            if (rawItem is IDictionary<string, object> itemProps)
            {
                return new PromoItem
                {
                    itemType = itemProps["itemType"] as string,
                    productId = itemProps["productId"] as string,
                    quantity = ParseDecimal(itemProps["quantity"])
                };
            }

            return default(PromoItem);
        }

        private static decimal ParseDecimal(object rawDecimal)
        {
            if (rawDecimal is long)
            {
                return new decimal((long)rawDecimal);
            }
            else if (rawDecimal is double)
            {
                return new decimal((double)rawDecimal);
            }
            else
            {
                return default(decimal);
            }
        }

        private static Product GetProductFromDictionary(object rawProduct)
        {
            if (rawProduct is IDictionary<string, object> productParams)
            {
                return new Product
                {
                    productId = productParams["productId"] as string,
                    localizedPrice = ParseDecimal(productParams["localizedPrice"]),
                    isoCurrencyCode = productParams["isoCurrencyCode"] as string,
                    localizedPriceString = productParams["localizedPriceString"] as string,
                    localizedTitle = productParams["localizedTitle"] as string,
                    localizedDescription = productParams["localizedDescription"] as string,
                    productType = productParams["productType"] as string
                };
            }

            return default(Product);
        }
    }
}
#endif
