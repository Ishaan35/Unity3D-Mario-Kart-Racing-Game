using System.Collections.Generic;

namespace UnityEngine.Monetization
{
    public interface IPlacementContentOperations
    {
        void SendCustomEvent(CustomEvent customEvent);
        bool ready { get; }
        PlacementContentState state { get; }
    }

    public interface IRewardedOperations : IPlacementContentOperations
    {
        bool IsRewarded();
        string rewardId { get; }
    }

    /// <summary>
    /// ShowResult is passed to [[ShowOptions.resultCallback]] after the advertisement has completed.
    /// </summary>
    public enum ShowResult
    {
        /// <summary>
        /// Indicates that the advertisement failed to complete.
        /// </summary>
        Failed,
        /// <summary>
        /// Indicates that the advertisement was skipped.
        /// </summary>
        Skipped,
        /// <summary>
        /// Indicates that the advertisement completed successfully.
        /// </summary>
        Finished
    }
    public delegate void ShowAdFinishCallback(ShowResult finishState);
    public delegate void ShowAdStartCallback();

    public struct ShowAdCallbacks
    {
        public ShowAdFinishCallback finishCallback;
        public ShowAdStartCallback startCallback;
    }

    public interface IShowAdOperations : IRewardedOperations
    {
        void Show(ShowAdCallbacks? callbacks);
    }

    public interface IPromoAdOperations : IShowAdOperations
    {
        PromoMetadata metadata { get; }
    }
}
