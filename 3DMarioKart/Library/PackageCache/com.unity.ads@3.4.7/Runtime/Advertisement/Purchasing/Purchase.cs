#if UNITY_ANDROID
using System;

namespace UnityEngine.Advertisements.Purchasing
{
    internal sealed class Purchase : AndroidJavaProxy, IPurchase
    {
        readonly AndroidJavaClass m_UnityPurchasing;
        IPurchasingEventSender m_Platform;

        public void onPurchasingCommand(String eventString)
        {
            String result = Purchasing.InitiatePurchasingCommand(eventString).ToString();
            int eventType = (int)PurchasingEvent.COMMAND;
            m_UnityPurchasing.CallStatic("dispatchReturnEvent", eventType, result);
        }

        public void onGetPurchasingVersion()
        {
            String promoVersion = Purchasing.GetPromoVersion();
            int eventType = (int)PurchasingEvent.VERSION;
            m_UnityPurchasing.CallStatic("dispatchReturnEvent", eventType, promoVersion);
        }

        public void onGetProductCatalog()
        {
            String purchaseCatalog = Purchasing.GetPurchasingCatalog();
            int eventType = (int)PurchasingEvent.CATALOG;
            m_UnityPurchasing.CallStatic("dispatchReturnEvent", eventType, purchaseCatalog);
        }

        public void onInitializePurchasing()
        {
            String result = Purchasing.Initialize(m_Platform).ToString();
            int eventType = (int)PurchasingEvent.INITIALIZATION;
            m_UnityPurchasing.CallStatic("dispatchReturnEvent", eventType, result);
        }

        public void SendEvent(string payload)
        {
            int eventType = (int)PurchasingEvent.EVENT;
            m_UnityPurchasing.CallStatic("dispatchReturnEvent", eventType, payload);
        }

        public void Initialize(IPurchasingEventSender platform)
        {
            m_Platform = platform;
            m_UnityPurchasing.CallStatic("initialize", this);
        }

        public Purchase() : base("com.unity3d.ads.purchasing.IPurchasing")
        {
            m_UnityPurchasing = new AndroidJavaClass("com.unity3d.ads.purchasing.Purchasing");
        }
    }
}
#endif
