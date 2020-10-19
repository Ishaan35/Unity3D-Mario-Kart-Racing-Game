#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Monetization
{
    // Callbacks for Purchasing Adapter functionality
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PurchasingAdapterCallbacks
    {
        delegate void OnRetrieveProductsDelegate(IntPtr listener);
        delegate void OnPurchaseDelegate(string productId, IntPtr listener);

        [MarshalAs(UnmanagedType.FunctionPtr)]
        OnRetrieveProductsDelegate OnRetrieveProducts;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        OnPurchaseDelegate OnPurchase;
    }
}
#endif
