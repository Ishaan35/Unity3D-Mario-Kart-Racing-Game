using System;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements
{
    internal interface IBanner
    {
        IUnityLifecycleManager UnityLifecycleManager { get; }

        bool IsLoaded { get; }
        bool ShowAfterLoad { get; set; }

        void Load(string placementId, BannerLoadOptions loadOptions);
        void Show(string placementId, BannerOptions showOptions);
        void Hide(bool destroy = false);
        void SetPosition(BannerPosition position);

        void UnityAdsBannerDidShow(string placementId, BannerOptions bannerOptions);
        void UnityAdsBannerDidHide(string placementId, BannerOptions bannerOptions);
        void UnityAdsBannerClick(string placementId, BannerOptions bannerOptions);
        void UnityAdsBannerDidLoad(string placementId, BannerLoadOptions bannerOptions);
        void UnityAdsBannerDidError(string message, BannerLoadOptions bannerOptions);
    }
}
