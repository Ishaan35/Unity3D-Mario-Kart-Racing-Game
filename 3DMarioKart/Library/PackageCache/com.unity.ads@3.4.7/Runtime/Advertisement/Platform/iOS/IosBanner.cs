#if UNITY_IOS
using System.Runtime.InteropServices;
using AOT;

namespace UnityEngine.Advertisements.Platform.iOS
{
    internal class IosBanner : INativeBanner
    {
        private static IBanner s_Banner;

        public bool IsLoaded
        {
            get => UnityAdsBannerIsLoaded();
            set {}
        }

        private static BannerLoadOptions s_BannerLoadOptions;
        private static BannerOptions s_BannerOptions;
        private static string s_PlacementId;

        private delegate void UnityAdsBannerShowDelegate(string placementId);
        private delegate void UnityAdsBannerHideDelegate(string placementId);
        private delegate void UnityAdsBannerClickDelegate(string placementId);
        private delegate void UnityAdsBannerUnloadDelegate(string placementId);
        private delegate void UnityAdsBannerLoadDelegate(string placementId);
        private delegate void UnityAdsBannerErrorDelegate(string message);

        [DllImport("__Internal")]
        private static extern void UnityAdsBannerShow(string placementId, bool showAfterLoad);
        [DllImport("__Internal")]
        private static extern void UnityAdsBannerHide(bool shouldDestroy);
        [DllImport("__Internal")]
        private static extern bool UnityAdsBannerIsLoaded();
        [DllImport("__Internal")]
        private static extern void UnityAdsBannerSetPosition(int position);
        [DllImport("__Internal")]
        private static extern void UnityAdsSetBannerShowCallback(UnityAdsBannerShowDelegate callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsSetBannerHideCallback(UnityAdsBannerHideDelegate callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsSetBannerClickCallback(UnityAdsBannerClickDelegate callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsSetBannerErrorCallback(UnityAdsBannerErrorDelegate callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsSetBannerUnloadCallback(UnityAdsBannerUnloadDelegate callback);
        [DllImport("__Internal")]
        private static extern void UnityAdsSetBannerLoadCallback(UnityAdsBannerLoadDelegate callback);
        [DllImport("__Internal")]
        private static extern void UnityBannerInitialize();

        [MonoPInvokeCallback(typeof(UnityAdsBannerShowDelegate))]
        private static void UnityAdsBannerDidShow(string placementId)
        {
            s_Banner.UnityAdsBannerDidShow(placementId, s_BannerOptions);
        }

        [MonoPInvokeCallback(typeof(UnityAdsBannerHideDelegate))]
        private static void UnityAdsBannerDidHide(string placementId)
        {
            s_Banner.UnityAdsBannerDidHide(placementId, s_BannerOptions);
        }

        [MonoPInvokeCallback(typeof(UnityAdsBannerClickDelegate))]
        private static void UnityAdsBannerClick(string placementId)
        {
            s_Banner.UnityAdsBannerClick(placementId, s_BannerOptions);
        }

        [MonoPInvokeCallback(typeof(UnityAdsBannerErrorDelegate))]
        private static void UnityAdsBannerDidError(string message)
        {
            s_Banner.UnityAdsBannerDidError(message, s_BannerLoadOptions);
        }

        [MonoPInvokeCallback(typeof(UnityAdsBannerUnloadDelegate))]
        private static void UnityAdsBannerDidUnload(string placementId)
        {
        }

        [MonoPInvokeCallback(typeof(UnityAdsBannerUnloadDelegate))]
        private static void UnityAdsBannerDidLoad(string placementId)
        {
            s_Banner.UnityAdsBannerDidLoad(placementId, s_BannerLoadOptions);
            s_PlacementId = placementId;
        }

        public void SetupBanner(IBanner banner)
        {
            s_Banner = banner;

            UnityAdsSetBannerShowCallback(UnityAdsBannerDidShow);
            UnityAdsSetBannerHideCallback(UnityAdsBannerDidHide);
            UnityAdsSetBannerClickCallback(UnityAdsBannerClick);
            UnityAdsSetBannerErrorCallback(UnityAdsBannerDidError);
            UnityAdsSetBannerUnloadCallback(UnityAdsBannerDidUnload);
            UnityAdsSetBannerLoadCallback(UnityAdsBannerDidLoad);
            UnityBannerInitialize();
        }

        public void Load(string placementId, BannerLoadOptions loadOptions)
        {
            s_BannerLoadOptions = loadOptions;
            if (!string.IsNullOrEmpty(s_PlacementId) && !s_PlacementId.Equals(placementId))
            {
                Hide(true);
            }
            UnityAdsBannerShow(placementId, false);
        }

        public void Show(string placementId, BannerOptions showOptions)
        {
            s_BannerOptions = showOptions;
            s_BannerLoadOptions = null;
            if (!string.IsNullOrEmpty(s_PlacementId) && !s_PlacementId.Equals(placementId))
            {
                Hide(true);
            }
            UnityAdsBannerShow(placementId, true);
        }

        public void Hide(bool destroy = false)
        {
            UnityAdsBannerHide(destroy);
            if (!destroy)
            {
                UnityAdsBannerDidHide(string.Empty);
            }
        }

        public void SetPosition(BannerPosition position)
        {
            UnityAdsBannerSetPosition((int)position);
        }
    }
}
#endif
