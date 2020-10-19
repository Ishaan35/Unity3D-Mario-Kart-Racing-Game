#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace UnityEngine.Monetization
{
    public class EditorPlacementContentOperations : IPlacementContentOperations
    {
        public EditorPlacementContentOperations()
        {
        }

        public void SendCustomEvent(CustomEvent customEvent)
        {
            Debug.LogFormat("Sent custom event in editor: {0}", customEvent.ToString());
        }

        public bool ready => true;
        public PlacementContentState state => PlacementContentState.Ready;
    }

    public class EditorRewardedOperations : EditorPlacementContentOperations, IRewardedOperations
    {
        public EditorRewardedOperations() : base()
        {
        }

        public bool IsRewarded()
        {
            return true;
        }

        public string rewardId => "rewardId";
    }

    public class EditorShowAdOperations : EditorRewardedOperations, IShowAdOperations
    {
        public EditorShowAdOperations() : base()
        {
        }

        public bool allowSkip { get; set; }
        public string placementId {get; set; }
        private ShowAdCallbacks? _showOptions;

        public virtual void Show(ShowAdCallbacks? showOptions)
        {
            ShowWithPlacement("ShowAdPlacement", showOptions);
        }

        void StartHandler()
        {
            Platform.m_Placeholder.onStart -= StartHandler;
            _showOptions?.startCallback();
        }

        void FinishHandler(ShowResult result)
        {
            Platform.m_Placeholder.onFinish -= FinishHandler;
            _showOptions?.finishCallback(result);
        }

        protected void ShowWithPlacement(string placementId, ShowAdCallbacks? showOptions)
        {
            if (Platform.m_Placeholder != null)
            {
                _showOptions = showOptions;
                Platform.m_Placeholder.onStart += StartHandler;

                Platform.m_Placeholder.onFinish += FinishHandler;

                Platform.m_Placeholder.Show(placementId, allowSkip);
            }
        }
    }

    public class EditorPromoAdOperations : EditorShowAdOperations, IPromoAdOperations
    {
        public EditorPromoAdOperations() : base()
        {
            metadata = new PromoMetadata
            {
                impressionDate = DateTime.Now,
                offerDuration = TimeSpan.FromHours(3),
                premiumProduct = new Product
                {
                    productId = "FakeProductId",
                    localizedTitle = "Fake localized title",
                    localizedDescription =  "Fake localized description",
                    localizedPrice = new decimal(1.99),
                    localizedPriceString = "$1.99",
                    isoCurrencyCode = "USD",
                    productType = "FakeProductType"
                },
                costs = new PromoItem[] {},
                payouts = new PromoItem[] {},
                customInfo = new Dictionary<string, object>()
            };
        }

        public override void Show(ShowAdCallbacks? showOptions)
        {
            ShowWithPlacement("PromoAdPlacement", showOptions);
        }

        public PromoMetadata metadata { get; }
    }
}
#endif
