#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Monetization
{
    public class IosNativePromoAdapter : INativePromoAdapter
    {
        [DllImport("__Internal")]
        static extern IntPtr UnityMonetizationCreateNativePromoAdapter(IntPtr pPlacementContent);
        [DllImport("__Internal")]
        static extern void UnityMonetizationReleaseNativePromoAdapter(IntPtr pNativePromoAdapter);
        [DllImport("__Internal")]
        static extern void UnityMonetizationNativePromoAdapterOnShown(IntPtr pNativePromoAdapter, int showType);
        [DllImport("__Internal")]
        static extern void UnityMonetizationNativePromoAdapterOnClicked(IntPtr pNativePromoAdapter);
        [DllImport("__Internal")]
        static extern void UnityMonetizationNativePromoAdapterOnClosed(IntPtr pNativePromoAdapter);

        private PromoAdPlacementContent placementContent { get; }
        private IntPtr nativePromoAdapter { get; }

        public IosNativePromoAdapter(PromoAdPlacementContent placementContent)
        {
            metadata = placementContent.metadata;
            var operations = placementContent.placementContentOperations as IosPlacementContentOperations;
            if (operations != null)
            {
                var ptr = operations.placementContentPtr;
                this.placementContent = placementContent;
                nativePromoAdapter = UnityMonetizationCreateNativePromoAdapter(ptr);
            }
        }

        ~IosNativePromoAdapter()
        {
            UnityMonetizationReleaseNativePromoAdapter(nativePromoAdapter);
        }

        public PromoMetadata metadata { get; }
        public void OnShown()
        {
            OnShown(PromoShowType.Full);
        }

        public void OnShown(PromoShowType type)
        {
            UnityMonetizationNativePromoAdapterOnShown(nativePromoAdapter, (int)type);
        }

        public void OnClosed()
        {
            UnityMonetizationNativePromoAdapterOnClosed(nativePromoAdapter);
        }

        public void OnClicked()
        {
            UnityMonetizationNativePromoAdapterOnClicked(nativePromoAdapter);
        }
    }
}
#endif
