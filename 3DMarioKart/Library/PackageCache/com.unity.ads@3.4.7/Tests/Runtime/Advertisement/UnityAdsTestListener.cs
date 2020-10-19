using UnityEngine.Events;

namespace UnityEngine.Advertisements.Tests
{
    public class UnityAdsTestListener : IUnityAdsListener
    {
        public bool HasBeenCalled;
        private UnityAction<string> m_UnityAdsReadyCallback;
        private UnityAction<string> m_UnityAdsDidErrorCallback;
        private UnityAction<string> m_UnityAdsDidStartCallback;
        private UnityAction<string, ShowResult> m_UnityAdsDidFinishCallback;

        public UnityAdsTestListener(UnityAction<string> readyCallback, UnityAction<string> errorCallback, UnityAction<string> startCallback, UnityAction<string, ShowResult> finishCallback)
        {
            HasBeenCalled = false;
            m_UnityAdsReadyCallback = readyCallback;
            m_UnityAdsDidErrorCallback = errorCallback;
            m_UnityAdsDidStartCallback = startCallback;
            m_UnityAdsDidFinishCallback = finishCallback;
        }

        public void OnUnityAdsReady(string placementId)
        {
            HasBeenCalled = true;
            m_UnityAdsReadyCallback?.Invoke(placementId);
        }

        public void OnUnityAdsDidError(string message)
        {
            HasBeenCalled = true;
            m_UnityAdsDidErrorCallback?.Invoke(message);
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            HasBeenCalled = true;
            m_UnityAdsDidStartCallback?.Invoke(placementId);
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            HasBeenCalled = true;
            m_UnityAdsDidFinishCallback?.Invoke(placementId, showResult);
        }
    }
}
