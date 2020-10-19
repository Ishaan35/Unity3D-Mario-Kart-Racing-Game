using System.Runtime.InteropServices;

namespace UnityEngine.Monetization
{
    public struct Product
    {
        public string productId;
        public string localizedTitle;
        public string localizedDescription;
        public string localizedPriceString;
        public string isoCurrencyCode;
        public string productType;
        public decimal localizedPrice;
    }
}
