#if UNITY_EDITOR
namespace UnityEngine.Advertisements.Platform.Editor
{
    internal class EditorBanner : INativeBanner
    {
        private IBanner m_Banner;
        private BannerPlaceholder m_BannerPlaceholder;

        private BannerPosition m_CurrentBannerPosition;
        private BannerPosition m_TargetBannerPosition;

        private BannerOptions m_BannerShowOptions;

        public bool IsLoaded { get; private set; }

        public void SetupBanner(IBanner banner)
        {
            m_Banner = banner;
            m_BannerPlaceholder = CreateBannerPlaceholder();
        }

        public void Load(string placementId, BannerLoadOptions loadOptions)
        {
            IsLoaded = true;
            m_CurrentBannerPosition = m_TargetBannerPosition;

            m_Banner.UnityLifecycleManager.Post(() => {
                loadOptions?.loadCallback();
            });
        }

        public void Show(string placementId, BannerOptions showOptions)
        {
            m_BannerShowOptions = showOptions;
            if (!m_Banner.IsLoaded)
            {
                Load(placementId, null);
            }

            m_BannerPlaceholder.ShowBanner(m_CurrentBannerPosition, m_BannerShowOptions);
            m_Banner.UnityLifecycleManager.Post(() => {
                showOptions?.showCallback();
            });
        }

        public void Hide(bool destroy = false)
        {
            if (destroy)
            {
                IsLoaded = false;
                return;
            }

            m_BannerPlaceholder.HideBanner();

            m_Banner.UnityLifecycleManager.Post(() => {
                m_BannerShowOptions?.hideCallback();
            });
        }

        public void SetPosition(BannerPosition position)
        {
            m_TargetBannerPosition = position;
        }

        private BannerPlaceholder CreateBannerPlaceholder()
        {
            var gameObject = new GameObject("UnityAdsBanner(Placeholder)")
            {
                hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector
            };
            GameObject.DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<BannerPlaceholder>();
        }
    }
}
#endif
