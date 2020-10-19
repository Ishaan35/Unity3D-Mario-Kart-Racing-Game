#if UNITY_ANDROID
using System;

namespace UnityEngine.Advertisements.Purchasing
{
    internal interface IPurchase
    {
        void Initialize(IPurchasingEventSender platform);
        void SendEvent(string payload);
        void onPurchasingCommand(String eventString);
        void onGetPurchasingVersion();
        void onGetProductCatalog();
        void onInitializePurchasing();
    }
}
#endif
