#if UNITY_ANDROID
using System;
using System.Collections.Generic;

namespace UnityEngine.Monetization
{
    public class AndroidNativePromoAdapter : INativePromoAdapter
    {
        public PromoMetadata metadata { get; }
        private AndroidJavaObject nativeAdapter;

        public AndroidNativePromoAdapter(PromoAdPlacementContent placementContent)
        {
            var operations = placementContent.placementContentOperations as AndroidPromoAdOperations;
            if (operations != null)
            {
                this.nativeAdapter = operations.nativeAdapter;
            }
            this.metadata = placementContent.metadata;
        }

        public void OnShown()
        {
            OnShown(PromoShowType.Full);
        }

        public void OnShown(PromoShowType type)
        {
            AndroidJavaObject nativeShowType = createNativeShowType(type);
            nativeAdapter.Call("onShown", nativeShowType);
        }

        public void OnClosed()
        {
            nativeAdapter.Call("onClosed");
        }

        public void OnClicked()
        {
            nativeAdapter.Call("onClicked");
        }

        internal static AndroidJavaObject getJavaTransactionDetails(TransactionDetails details)
        {
            AndroidJavaObject builder = new AndroidJavaClass("com.unity3d.services.purchasing.core.TransactionDetails").CallStatic<AndroidJavaObject>("newBuilder");
            builder.Call<AndroidJavaObject>("withProductId", details.productId);
            builder.Call<AndroidJavaObject>("withTransactionId", details.transactionId);
            builder.Call<AndroidJavaObject>("withPrice", (double)details.price);
            builder.Call<AndroidJavaObject>("withReceipt", details.receipt);
            builder.Call<AndroidJavaObject>("withCurrency", details.currency);
            if (details.extras != null)
            {
                foreach (KeyValuePair<string, object> entry in details.extras)
                {
                    builder.Call<AndroidJavaObject>("putExtra", entry.Key, entry.Value);
                }
            }
            return builder.Call<AndroidJavaObject>("build");
        }

        internal static AndroidJavaObject getJavaTransactionErrorDetails(TransactionErrorDetails details)
        {
            var transactionErrorIndex = (int)details.transactionError;
            var transactionErrorClass = new AndroidJavaClass("com.unity3d.services.purchasing.core.TransactionError");
            var transactionErrorValues = transactionErrorClass.CallStatic<AndroidJavaObject>("values");
            var transactionError = new AndroidJavaClass("java.lang.reflect.Array").CallStatic<AndroidJavaObject>("get", transactionErrorValues, transactionErrorIndex);

            var storeIndex = (int)details.store;
            var storeClass = new AndroidJavaClass("com.unity3d.services.purchasing.core.Store");
            var storeValues = storeClass.CallStatic<AndroidJavaObject>("values");
            var store = new AndroidJavaClass("java.lang.reflect.Array").CallStatic<AndroidJavaObject>("get", storeValues, storeIndex);

            var builder = new AndroidJavaClass("com.unity3d.services.purchasing.core.TransactionErrorDetails").CallStatic<AndroidJavaObject>("newBuilder");
            builder.Call<AndroidJavaObject>("withTransactionError", transactionError);
            builder.Call<AndroidJavaObject>("withExceptionMessage", details.exceptionMessage);
            builder.Call<AndroidJavaObject>("withStore", store);
            builder.Call<AndroidJavaObject>("withStoreSpecificErrorCode", details.storeSpecificErrorCode);
            if (details.extras != null)
            {
                foreach (KeyValuePair<string, object> entry in details.extras)
                {
                    builder.Call<AndroidJavaObject>("putExtra", entry.Key, entry.Value);
                }
            }

            return builder.Call<AndroidJavaObject>("build");
        }

        private AndroidJavaObject createNativeShowType(PromoShowType type)
        {
            var index = (int)type;
            var enumClass = new AndroidJavaClass("com.unity3d.services.monetization.placementcontent.purchasing.NativePromoShowType");
            var values = enumClass.CallStatic<AndroidJavaObject>("values");
            return new AndroidJavaClass("java.lang.reflect.Array").CallStatic<AndroidJavaObject>("get", values, index);
        }
    }
}
#endif
