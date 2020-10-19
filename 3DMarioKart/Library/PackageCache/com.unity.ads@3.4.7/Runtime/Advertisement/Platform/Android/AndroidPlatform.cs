#if UNITY_ANDROID
using UnityEngine.Advertisements.Purchasing;

namespace UnityEngine.Advertisements.Platform.Android
{
    internal class AndroidPlatform : AndroidJavaProxy, INativePlatform, IPurchasingEventSender
    {
        private IPlatform m_Platform;
        private AndroidJavaObject m_CurrentActivity;
        private AndroidJavaClass m_UnityAds;
        private IPurchase m_UnityAdsPurchase;

        public AndroidPlatform() : base("com.unity3d.ads.IUnityAdsListener") {}

        public void SetupPlatform(IPlatform platform)
        {
            m_Platform = platform;
            m_CurrentActivity = GetCurrentAndroidActivity();
            m_UnityAds = new AndroidJavaClass("com.unity3d.ads.UnityAds");
        }

        public void Initialize(string gameId, bool testMode, bool enablePerPlacementLoad)
        {
            m_UnityAdsPurchase = new Purchase();
            m_UnityAdsPurchase?.Initialize(this);
            m_UnityAds?.CallStatic("addListener", this);
            m_UnityAds?.CallStatic("initialize", m_CurrentActivity, gameId, testMode, enablePerPlacementLoad);
        }

        public void Load(string placementId)
        {
            m_UnityAds?.CallStatic("load", placementId);
        }

        public void Show(string placementId)
        {
            if (placementId == null)
            {
                m_UnityAds?.CallStatic("show", m_CurrentActivity);
            }
            else
            {
                m_UnityAds?.CallStatic("show", m_CurrentActivity, placementId);
            }
        }

        public void SetMetaData(MetaData metaData)
        {
            var metaDataObject = new AndroidJavaObject("com.unity3d.ads.metadata.MetaData", m_CurrentActivity);
            metaDataObject.Call("setCategory", metaData.category);
            foreach (var entry in metaData.Values())
            {
                metaDataObject.Call<bool>("set", entry.Key, entry.Value);
            }
            metaDataObject.Call("commit");
        }

        public bool GetDebugMode()
        {
            return m_UnityAds?.CallStatic<bool>("getDebugMode") ?? false;
        }

        public void SetDebugMode(bool debugMode)
        {
            m_UnityAds?.CallStatic("setDebugMode", debugMode);
        }

        public string GetVersion()
        {
            return m_UnityAds?.CallStatic<string>("getVersion") ?? "UnknownVersion";
        }

        public bool IsInitialized()
        {
            return m_UnityAds?.CallStatic<bool>("isInitialized") ?? false;
        }

        public bool IsReady(string placementId)
        {
            return placementId == null ? m_UnityAds?.CallStatic<bool>("isReady") ?? false : m_UnityAds?.CallStatic<bool>("isReady", placementId) ?? false;
        }

        internal void RemoveListener()
        {
            m_UnityAds?.CallStatic("removeListener", this);
        }

        public PlacementState GetPlacementState(string placementId)
        {
            var rawPlacementState = placementId == null ? m_UnityAds.CallStatic<AndroidJavaObject>("getPlacementState") : m_UnityAds.CallStatic<AndroidJavaObject>("getPlacementState", placementId);
            return (PlacementState)rawPlacementState.Call<int>("ordinal");
        }

        public static AndroidJavaObject GetCurrentAndroidActivity()
        {
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        void IPurchasingEventSender.SendPurchasingEvent(string payload)
        {
            m_UnityAdsPurchase?.SendEvent(payload);
        }

        private void onUnityAdsReady(string placementId)
        {
            m_Platform?.UnityAdsReady(placementId);
        }

        private void onUnityAdsStart(string placementId)
        {
            m_Platform?.UnityAdsDidStart(placementId);
        }

        private void onUnityAdsFinish(string placementId, AndroidJavaObject rawShowResult)
        {
            var showResult = (ShowResult)rawShowResult.Call<int>("ordinal");
            m_Platform?.UnityAdsDidFinish(placementId, showResult);
        }

        private void onUnityAdsError(AndroidJavaObject rawError, string message)
        {
            m_Platform?.UnityAdsDidError(message);
        }

#if (!UNITY_2017_1_OR_NEWER)
        public int hashCode()
        {
            return GetHashCode();
        }

        public bool equals(UnityEngine.AndroidJavaObject o)
        {
            return Equals(o);
        }

#endif
    }
}
#endif
