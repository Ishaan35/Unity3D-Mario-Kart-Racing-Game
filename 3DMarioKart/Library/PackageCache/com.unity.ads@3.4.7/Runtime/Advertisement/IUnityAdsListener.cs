using System;

namespace UnityEngine.Advertisements
{
    /// <summary>
    /// <para>An interface for handling various states of an ad. Implement this listener in your script to define logic for <a href="../manual/MonetizationBasicIntegrationUnity.html#rewarded-video-ads">rewarded ads</a>.</para>
    /// <para>The <c>OnUnityAdsReady</c> method handles logic for ad content being ready to display through a specified <a href="../manual/MonetizationPlacements.html">Placement</a>.</para>
    /// <para>The <c>OnUnityAdsDidError</c> method handles logic for ad content failing to display because of an error.</para>
    /// <para>The <c>OnUnityAdsDidStart</c> method handles logic for the player triggering an ad to play.</para>
    /// <para>The <c>OnUnityAdsDidFinish</c> method handles logic for an ad finishing. Define conditional behavior for different finish states by accessing the <a href="../api/UnityEngine.Advertisements.ShowResult.html"><c>ShowResult</c></a> result from the listener. For more information, view documentation on implementing <a href="../manual/MonetizationBasicIntegrationUnity.html#rewarded-video-ads">rewarded ads</a>.</para>
    /// </summary>
    public interface IUnityAdsListener
    {
        void OnUnityAdsReady(string placementId);
        void OnUnityAdsDidError(string message);
        void OnUnityAdsDidStart(string placementId);
        void OnUnityAdsDidFinish(string placementId, ShowResult showResult);
    }
}
