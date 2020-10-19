using System;

namespace UnityEngine.Monetization
{
    sealed class UnsupportedPlatform : IMonetizationPlatform
    {
        public event EventHandler<PlacementContentReadyEventArgs> OnPlacementContentReady { add {} remove {} }
        public event EventHandler<PlacementContentStateChangeEventArgs> OnPlacementContentStateChange { add {} remove {} }
        public event EventHandler<UnityServicesErrorEventArgs> onError { add {} remove {} }

        public string version { get; } = null;

        public void Initialize(string gameId, bool testMode)
        {
        }

        public void SetPurchasingAdapter(IPurchasingAdapter adapter)
        {
        }

        public bool isSupported
        {
            get
            {
                return false;
            }
        }

        public bool IsReady(string placementId)
        {
            return false;
        }

        public PlacementContent GetPlacementContent(string placementID)
        {
            return null;
        }

        public INativePromoAdapter CreateNativePromoAdapter(PromoAdPlacementContent placementContent)
        {
            return null;
        }

        public void SetMetaData(MetaData metaData)
        {
        }
    }
}
