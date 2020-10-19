using System;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements
{
    internal class Banner : IBanner
    {
        public IUnityLifecycleManager UnityLifecycleManager { get; }

        public bool IsLoaded => m_NativeBanner.IsLoaded;

        public bool ShowAfterLoad { get; set; }

        private INativeBanner m_NativeBanner;

        public Banner(INativeBanner nativeBanner, IUnityLifecycleManager unityLifecycleManager)
        {
            m_NativeBanner = nativeBanner;
            UnityLifecycleManager = unityLifecycleManager;
            m_NativeBanner.SetupBanner(this);
        }

        public void Load(string placementId, BannerLoadOptions loadOptions)
        {
            m_NativeBanner.Load(placementId, loadOptions);
        }

        public void Show(string placementId, BannerOptions showOptions)
        {
            m_NativeBanner.Show(placementId, showOptions);
        }

        public void Hide(bool destroy = false)
        {
            m_NativeBanner.Hide(destroy);
        }

        public void SetPosition(BannerPosition position)
        {
            m_NativeBanner.SetPosition(position);
        }

        public void UnityAdsBannerDidShow(string placementId, BannerOptions bannerOptions)
        {
            var showCallback = bannerOptions?.showCallback;
            if (showCallback != null)
            {
                UnityLifecycleManager?.Post(() => {
                    bannerOptions.showCallback();
                });
            }
        }

        public void UnityAdsBannerDidHide(string placementId, BannerOptions bannerOptions)
        {
            var hideCallback = bannerOptions?.hideCallback;
            if (hideCallback != null)
            {
                UnityLifecycleManager?.Post(() => {
                    bannerOptions.hideCallback();
                });
            }
        }

        public void UnityAdsBannerClick(string placementId, BannerOptions bannerOptions)
        {
            var clickCallback = bannerOptions?.clickCallback;
            if (clickCallback != null)
            {
                UnityLifecycleManager?.Post(() => {
                    bannerOptions.clickCallback();
                });
            }
        }

        public void UnityAdsBannerDidLoad(string placementId, BannerLoadOptions bannerOptions)
        {
            var loadCallback = bannerOptions?.loadCallback;
            if (loadCallback != null)
            {
                UnityLifecycleManager?.Post(() => {
                    bannerOptions.loadCallback();
                });
            }
        }

        public void UnityAdsBannerDidError(string message, BannerLoadOptions bannerOptions)
        {
            var errorCallback = bannerOptions?.errorCallback;
            if (errorCallback != null)
            {
                UnityLifecycleManager?.Post(() => {
                    bannerOptions.errorCallback(message ?? "");
                });
            }
        }
    }
}
