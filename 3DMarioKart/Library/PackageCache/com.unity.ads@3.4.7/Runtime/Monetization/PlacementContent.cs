using System;
using System.Collections.Generic;

namespace UnityEngine.Monetization
{
    public struct CustomEvent
    {
        public string category;
        public string type;
        public IDictionary<string, object> data;

        public CustomEvent(string type, IDictionary<string, object> data = null) : this(null, type, data)
        {
        }

        public CustomEvent(string category, string type, IDictionary<string, object> data)
        {
            this.category = category;
            this.type = type;
            this.data = data;
        }

        internal IDictionary<string, object> dictionaryValue
        {
            get => new Dictionary<string, object>
            {
                {"category", category},
                {"type", type},
                {"data", data}
            };
        }
    }

    public class PlacementContent
    {
        public PlacementContent(string placementId, IPlacementContentOperations operations)
        {
            this.placementId = placementId;
            this.placementContentOperations = operations;
        }

        public string placementId { get; set; }

        public IDictionary<string, object> extras { get; internal set; }

        internal IPlacementContentOperations placementContentOperations { get; set; }

        public bool ready => placementContentOperations.ready;
        public PlacementContentState state => placementContentOperations.state;

        public virtual void SendCustomEvent(CustomEvent customEvent)
        {
            placementContentOperations.SendCustomEvent(customEvent);
        }
    }

    public class RewardablePlacementContent : PlacementContent
    {
        public RewardablePlacementContent(string placementId, IRewardedOperations operations) : base(placementId, operations)
        {
            rewardedOperations = operations;
        }

        private IRewardedOperations rewardedOperations { get; }

        public bool rewarded => rewardedOperations.IsRewarded();
        public string rewardId => rewardedOperations.rewardId;
    }

    public class ShowAdYield : CustomYieldInstruction
    {
        public ShowResult result { get; internal set; }
        internal bool showing { get; set; }
        public override bool keepWaiting => showing;
    }

    public class ShowAdPlacementContent : RewardablePlacementContent
    {
        public ShowAdPlacementContent(string placementId, IShowAdOperations operations) : base(placementId, operations)
        {
            this.showAdOperations = operations;
        }

        public string gamerSid { get; set; }
        public bool showing { get; private set; }

        private IShowAdOperations showAdOperations { get; }
        private ShowAdYield adYield;

        public ShowAdYield Show(ShowAdCallbacks? callbacks = null)
        {
            if (!string.IsNullOrEmpty(gamerSid))
            {
                var player = new MetaData("player");
                player.Set("server_id", gamerSid);
#pragma warning disable 0618
                Monetization.SetMetaData(player);
#pragma warning restore 0618
            }
            adYield = new ShowAdYield();
            adYield.showing = true;
            var adCallbacks = new ShowAdCallbacks
            {
                finishCallback = finishState =>
                {
                    adYield.result = finishState;
                    adYield.showing = showing = false;
                    callbacks?.finishCallback?.Invoke(finishState);
                },
                startCallback = () =>
                {
                    showing = true;
                    callbacks?.startCallback?.Invoke();
                }
            };

            showAdOperations.Show(adCallbacks);
            return adYield;
        }

        public ShowAdYield Show(ShowAdFinishCallback finishCallback)
        {
            return Show(new ShowAdCallbacks
            {
                finishCallback = finishCallback
            });
        }
    }

    public class PromoAdPlacementContent : ShowAdPlacementContent
    {
        public PromoAdPlacementContent(string placementId, IPromoAdOperations operations) : base(placementId, operations)
        {
            this.promoAdOperations = operations;
        }

        private IPromoAdOperations promoAdOperations { get; set; }

        public PromoMetadata metadata => promoAdOperations.metadata;
    }
}
