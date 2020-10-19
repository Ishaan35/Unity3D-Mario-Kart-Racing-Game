using System;

namespace UnityEngine.Monetization
{
    interface IMonetizationPlatform
    {
        event EventHandler<PlacementContentReadyEventArgs> OnPlacementContentReady;
        event EventHandler<PlacementContentStateChangeEventArgs> OnPlacementContentStateChange;
        event EventHandler<UnityServicesErrorEventArgs> onError;

        bool isSupported { get; }
        string version { get; }

        void Initialize(string gameId, bool testMode);
        void SetPurchasingAdapter(IPurchasingAdapter adapter);
        bool IsReady(string placementID);

        PlacementContent GetPlacementContent(string placementID);

        INativePromoAdapter CreateNativePromoAdapter(PromoAdPlacementContent placementContent);
        void SetMetaData(MetaData metaData);
    }
}
