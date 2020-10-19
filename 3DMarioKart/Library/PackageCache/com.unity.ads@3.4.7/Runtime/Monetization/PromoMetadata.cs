using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnityEngine.Monetization
{
    public struct PromoItem
    {
        public string productId;
        public decimal quantity;
        public string itemType;
    }

    public struct PromoMetadata
    {
        public DateTime impressionDate;
        public TimeSpan offerDuration;
        public Product premiumProduct;
        public PromoItem[] costs;
        public PromoItem[] payouts;
        public IDictionary<string, object> customInfo;

        public bool isPremium => premiumProduct.productId != null;

        public TimeSpan timeRemaining
        {
            get
            {
                if (impressionDate == default(DateTime))
                {
                    return offerDuration;
                }

                return offerDuration - (DateTime.Now - impressionDate);
            }
        }

        public bool isExpired
        {
            get
            {
                if (impressionDate == default(DateTime))
                {
                    return false;
                }

                return timeRemaining.CompareTo(TimeSpan.FromSeconds(0)) <= 0;
            }
        }

        public PromoItem cost => costs != null && costs.Length > 0 ? costs[0] : default(PromoItem);
        public PromoItem payout => payouts != null && payouts.Length > 0 ? payouts[0] : default(PromoItem);
    }
}
