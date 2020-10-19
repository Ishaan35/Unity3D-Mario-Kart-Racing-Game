namespace UnityEngine.Monetization
{
    public enum PromoShowType
    {
        Preview,
        Full
    }

    public interface INativePromoAdapter
    {
        PromoMetadata metadata { get; }
        void OnShown();
        void OnShown(PromoShowType type);
        void OnClosed();
        void OnClicked();
    }
}
