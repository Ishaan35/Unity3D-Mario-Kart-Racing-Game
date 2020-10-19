#if UNITY_EDITOR
namespace UnityEngine.Monetization
{
    public class EditorNativePromoAdapter : INativePromoAdapter
    {
        public EditorNativePromoAdapter(PromoAdPlacementContent placementContent)
        {
            metadata = placementContent.metadata;
        }

        public PromoMetadata metadata { get; }

        public void OnShown()
        {
            OnShown(PromoShowType.Full);
        }

        public void OnShown(PromoShowType type)
        {
            Debug.LogFormat("Native promo was shown: {0}", type);
        }

        public void OnClosed()
        {
            Debug.LogFormat("Native promo was closed.");
        }

        public void OnClicked()
        {
            Debug.LogFormat("Native promo was clicked");
        }
    }
}
#endif
