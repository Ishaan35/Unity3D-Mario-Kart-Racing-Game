namespace UnityEngine.Advertisements
{
    internal interface INativeBanner
    {
        bool IsLoaded { get; }

        void SetupBanner(IBanner banner);
        void Load(string placementId, BannerLoadOptions loadOptions);
        void Show(string placementId, BannerOptions showOptions);
        void Hide(bool destroy = false);
        void SetPosition(BannerPosition position);
    }
}
