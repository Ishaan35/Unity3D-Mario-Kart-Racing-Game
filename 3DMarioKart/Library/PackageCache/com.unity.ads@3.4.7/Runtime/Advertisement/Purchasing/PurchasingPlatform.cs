#if UNITY_IOS
using System.Runtime.InteropServices;
using AOT;

namespace UnityEngine.Advertisements.Purchasing
{
    public class PurchasingPlatform : IPurchasingEventSender
    {
        private static PurchasingPlatform Instance { get; set; }

        delegate void unityAdsPurchasingDidInitiatePurchasingCommand(string eventString);
        delegate void unityAdsPurchasingGetProductCatalog();
        delegate void unityAdsPurchasingGetPurchasingVersion();
        delegate void unityAdsPurchasingInitialize();

        [DllImport("__Internal")]
        static extern void UnityAdsPurchasingDispatchReturnEvent(long eventType, string payload);

        [DllImport("__Internal")]
        static extern void UnityAdsSetDidInitiatePurchasingCommandCallback(unityAdsPurchasingDidInitiatePurchasingCommand callback);

        [DllImport("__Internal")]
        static extern void UnityAdsSetGetProductCatalogCallback(unityAdsPurchasingGetProductCatalog callback);

        [DllImport("__Internal")]
        static extern void UnityAdsSetGetVersionCallback(unityAdsPurchasingGetPurchasingVersion callback);

        [DllImport("__Internal")]
        static extern void UnityAdsSetInitializePurchasingCallback(unityAdsPurchasingInitialize callback);

        [MonoPInvokeCallback(typeof(unityAdsPurchasingDidInitiatePurchasingCommand))]
        static void UnityAdsDidInitiatePurchasingCommand(string eventString)
        {
            string result = Purchasing.InitiatePurchasingCommand(eventString).ToString();
            UnityAdsPurchasingDispatchReturnEvent((long)PurchasingEvent.COMMAND, result);
        }

        [MonoPInvokeCallback(typeof(unityAdsPurchasingGetProductCatalog))]
        static void UnityAdsPurchasingGetProductCatalog()
        {
            string result = Purchasing.GetPurchasingCatalog();
            UnityAdsPurchasingDispatchReturnEvent((long)PurchasingEvent.CATALOG, result);
        }

        [MonoPInvokeCallback(typeof(unityAdsPurchasingGetPurchasingVersion))]
        static void UnityAdsPurchasingGetPurchasingVersion()
        {
            string result = Purchasing.GetPromoVersion();
            UnityAdsPurchasingDispatchReturnEvent((long)PurchasingEvent.VERSION, result);
        }

        [MonoPInvokeCallback(typeof(unityAdsPurchasingInitialize))]
        static void UnityAdsPurchasingInitialize()
        {
            string result = Purchasing.Initialize(Instance).ToString();
            UnityAdsPurchasingDispatchReturnEvent((long)PurchasingEvent.INITIALIZATION, result);
        }

        public void Initialize()
        {
            Instance = this;
            UnityAdsSetDidInitiatePurchasingCommandCallback(UnityAdsDidInitiatePurchasingCommand);
            UnityAdsSetGetProductCatalogCallback(UnityAdsPurchasingGetProductCatalog);
            UnityAdsSetGetVersionCallback(UnityAdsPurchasingGetPurchasingVersion);
            UnityAdsSetInitializePurchasingCallback(UnityAdsPurchasingInitialize);
        }

        public void SendPurchasingEvent(string payload)
        {
            UnityAdsPurchasingDispatchReturnEvent((long)PurchasingEvent.EVENT, payload);
        }
    }
}
#endif
