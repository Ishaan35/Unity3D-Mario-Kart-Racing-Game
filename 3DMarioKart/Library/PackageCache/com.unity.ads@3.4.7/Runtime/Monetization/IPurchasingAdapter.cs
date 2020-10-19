using System.Collections.Generic;

namespace UnityEngine.Monetization
{
    public interface IPurchasingAdapter
    {
        void RetrieveProducts(IRetrieveProductsListener listener);
        void Purchase(string productID, ITransactionListener listener, IDictionary<string, object> extras);
    }

    public interface IRetrieveProductsListener
    {
        void OnProductsRetrieved(ICollection<Product> products);
    }

    public interface ITransactionListener
    {
        void OnTransactionComplete(TransactionDetails details);
        void OnTransactionError(TransactionErrorDetails details);
    }

    // https://docs.unity3d.com/Manual/UnityAnalyticsMonetization.html
    public struct TransactionDetails
    {
        public string productId;
        public string transactionId;
        public decimal price;
        public string currency;
        public string receipt;
        public IDictionary<string, object> extras;

        internal IDictionary<string, object> ToJsonDictionary()
        {
            return new Dictionary<string, object>
            {
                {"productId", productId},
                {"transactionId", transactionId},
                {"receipt", receipt},
                {"price", price},
                {"currency", currency},
                {"extras", extras}
            };
        }
    }

    public struct TransactionErrorDetails
    {
        public TransactionError transactionError;
        public string exceptionMessage;
        public Store store;
        public string storeSpecificErrorCode;
        public IDictionary<string, object> extras;

        internal IDictionary<string, object> ToJsonDictionary()
        {
            return new Dictionary<string, object>
            {
                {"transactionError", TransactionErrorToString()},
                {"exceptionMessage", exceptionMessage},
                {"store", StoreToString()},
                {"storeSpecificErrorCode", storeSpecificErrorCode},
                {"extras", extras}
            };
        }

        private string StoreToString()
        {
            switch (store)
            {
                case Store.NotSpecified:
                    return "NotSpecified";
                case Store.GooglePlay:
                    return "GooglePlay";
                case Store.AmazonAppStore:
                    return "AmazonAppStore";
                case Store.CloudMoolah:
                    return "CloudMoolah";
                case Store.SamsungApps:
                    return "SamsungApps";
                case Store.XiaomiMiPay:
                    return "XiaomiMiPay";
                case Store.MacAppStore:
                    return "MacAppStore";
                case Store.AppleAppStore:
                    return "AppleAppStore";
                case Store.WinRT:
                    return "WinRT";
                case Store.TizenStore:
                    return "TizenStore";
                case Store.FacebookStore:
                    return "FacebookStore";
                default:
                    return "NotSpecified";
            }
        }

        private string TransactionErrorToString()
        {
            switch (transactionError)
            {
                case TransactionError.NotSupported:
                    return "NotSupported";
                case TransactionError.ItemUnavailable:
                    return "ItemUnavailable";
                case TransactionError.UserCancelled:
                    return "UserCancelled";
                case TransactionError.NetworkError:
                    return "NetworkError";
                case TransactionError.ServerError:
                    return "ServerError";
                case TransactionError.UnknownError:
                    return "UnknownError";
                default:
                    return "UnknownError";
            }
        }
    }
}
