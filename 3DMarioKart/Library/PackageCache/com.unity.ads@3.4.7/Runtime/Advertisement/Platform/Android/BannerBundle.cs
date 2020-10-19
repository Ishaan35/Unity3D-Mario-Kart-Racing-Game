namespace UnityEngine.Advertisements.Platform.Android
{
    internal class BannerBundle
    {
        public AndroidJavaObject bannerView { get; }
        public string bannerPlacementId { get; }

        public BannerBundle(string bannerPlacementId, AndroidJavaObject bannerView)
        {
            this.bannerPlacementId = bannerPlacementId;
            this.bannerView = bannerView;
        }
    }
}
