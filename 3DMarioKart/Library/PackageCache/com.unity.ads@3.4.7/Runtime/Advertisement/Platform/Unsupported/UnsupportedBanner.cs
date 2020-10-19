using System;

namespace UnityEngine.Advertisements.Platform.Unsupported
{
    internal class UnsupportedBanner : INativeBanner
    {
        public bool IsLoaded => false;

        public void SetupBanner(IBanner banner) {}

        public void Load(string placementId, BannerLoadOptions loadOptions) {}

        public void Show(string placementId, BannerOptions showOptions) {}

        public void Hide(bool destroy = false) {}

        public void SetPosition(BannerPosition position) {}
    }
}
