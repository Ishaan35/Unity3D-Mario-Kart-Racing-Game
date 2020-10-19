#if UNITY_ANDROID
using System;

namespace UnityEngine.Advertisements.Platform.Android
{
    internal class AndroidBanner : AndroidJavaProxy, INativeBanner
    {
        private IBanner m_Banner;

        private AndroidJavaClass m_BannersClass;
        private AndroidJavaObject m_CurrentActivity;

        private BannerBundle m_BannerBundle;

        private BannerLoadOptions m_BannerLoadOptions;
        private BannerOptions m_BannerShowOptions;

        public bool IsLoaded => m_BannerBundle != null;

        private bool m_ListenerIsSet;

        public AndroidBanner() : base("com.unity3d.services.banners.IUnityBannerListener") {}

        public void SetupBanner(IBanner banner)
        {
            m_Banner = banner;
            m_BannersClass = new AndroidJavaClass("com.unity3d.services.banners.UnityBanners");
            m_CurrentActivity = AndroidPlatform.GetCurrentAndroidActivity();
            m_BannerBundle = null;
        }

        public void Load(string placementId, BannerLoadOptions loadOptions)
        {
            if (!m_ListenerIsSet) {
                m_ListenerIsSet = true;
                m_BannersClass.CallStatic("setBannerListener", this);
            }

            m_BannerLoadOptions = loadOptions;
            if (m_BannerBundle != null && m_BannerBundle.bannerPlacementId.Equals(placementId))
            {
                m_Banner.UnityLifecycleManager.Post(() =>
                {
                    loadOptions?.loadCallback();
                });
            }
            else
            {
                if (m_BannerBundle != null)
                {
                    Hide(true);
                    m_BannerBundle = null;
                }
                if (placementId != null)
                {
                    m_BannersClass.CallStatic("loadBanner", m_CurrentActivity, placementId);
                }
                else
                {
                    m_BannersClass.CallStatic("loadBanner", m_CurrentActivity);
                }
            }
        }

        public void Show(string placementId, BannerOptions showOptions)
        {
            m_BannerShowOptions = showOptions;
            if (m_BannerBundle != null && (string.IsNullOrEmpty(placementId) || m_BannerBundle.bannerPlacementId.Equals(placementId)))
            {
                m_CurrentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    var parent = m_BannerBundle.bannerView.Call<AndroidJavaObject>("getParent");
                    if (parent == null)
                    {
                        var layoutParams = m_BannerBundle.bannerView.Call<AndroidJavaObject>("getLayoutParams");
                        m_CurrentActivity.Call("addContentView", m_BannerBundle.bannerView, layoutParams);
                    }
                }));
                m_Banner.UnityLifecycleManager.Post(() =>
                {
                    showOptions?.showCallback();
                });
            }
            else
            {
                if (m_BannerBundle != null)
                {
                    Hide(true);
                    m_BannerBundle = null;
                }
                m_Banner.ShowAfterLoad = true;
                Load(placementId, null);
            }
        }

        public void Hide(bool destroy = false)
        {
            if (m_BannerBundle != null)
            {
                if (destroy)
                {
                    m_BannerBundle = null;
                    m_BannersClass.CallStatic("destroy");
                }
                else
                {
                    m_CurrentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                        var parent = m_BannerBundle.bannerView.Call<AndroidJavaObject>("getParent");
                        parent?.Call("removeView", m_BannerBundle.bannerView);
                    }));
                    if (m_BannerShowOptions?.hideCallback != null)
                    {
                        m_Banner.UnityLifecycleManager.Post(() =>
                        {
                            m_BannerShowOptions?.hideCallback();
                        });
                    }
                }
            }
        }

        public void SetPosition(BannerPosition position)
        {
            var index = (int)position;
            var enumClass = new AndroidJavaClass("com.unity3d.services.banners.view.BannerPosition");
            var values = enumClass.CallStatic<AndroidJavaObject>("values");
            var bannerPosition = new AndroidJavaClass("java.lang.reflect.Array").CallStatic<AndroidJavaObject>("get", values, index);

            m_BannersClass.CallStatic("setBannerPosition", bannerPosition);
        }

        private void onUnityBannerShow(string placementId)
        {
        }

        private void onUnityBannerHide(string placementId)
        {
        }

        private void onUnityBannerLoaded(String placementId, AndroidJavaObject view)
        {
            m_BannerBundle = new BannerBundle(placementId, view);
            view.Call("setBackgroundColor", UnityEngine.Advertisements.Utilities.Color.Transparent);
            if (m_Banner.ShowAfterLoad)
            {
                m_Banner.ShowAfterLoad = false;
                var layoutParams = view.Call<AndroidJavaObject>("getLayoutParams");
                m_CurrentActivity.Call("addContentView", view, layoutParams);
                m_Banner.UnityLifecycleManager.Post(() =>
                {
                    m_BannerShowOptions?.showCallback();
                });
            }

            m_Banner.UnityLifecycleManager.Post(() =>
            {
                m_BannerLoadOptions?.loadCallback();
            });
        }

        private void onUnityBannerUnloaded(string placementId)
        {
        }

        private void onUnityBannerClick(string placementId)
        {
            m_Banner.UnityLifecycleManager.Post(() =>
            {
                m_BannerShowOptions?.clickCallback();
            });
        }

        private void onUnityBannerError(string message)
        {
            m_Banner.UnityLifecycleManager.Post(() =>
            {
                m_BannerLoadOptions?.errorCallback(message);
            });
        }
    }
}
#endif
