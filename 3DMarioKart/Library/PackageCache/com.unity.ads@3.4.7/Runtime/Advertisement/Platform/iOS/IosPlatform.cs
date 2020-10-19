#if UNITY_IOS

using UnityEngine.Advertisements.Purchasing;
using System.Runtime.InteropServices;
using AOT;

namespace UnityEngine.Advertisements.Platform.iOS
{
    internal class IosPlatform : INativePlatform
    {
        private static IPlatform s_Platform;

        private delegate void UnityAdsReadyDelegate(string placementId);
        private delegate void UnityAdsDidErrorDelegate(long rawError, string message);
        private delegate void UnityAdsDidStartDelegate(string placementId);
        private delegate void UnityAdsDidFinishDelegate(string placementId, long rawShowResult);

        [DllImport("__Internal")]
        private static extern void UnityAdsInitialize(string gameId, bool testMode, bool enablePerPlacementLoad);

        [DllImport("__Internal")]
        private static extern void UnityAdsLoad(string placementId);

        [DllImport("__Internal")]
        private static extern void UnityAdsShow(string placementId);

        [DllImport("__Internal")]
        private static extern bool UnityAdsGetDebugMode();

        [DllImport("__Internal")]
        private static extern void UnityAdsSetDebugMode(bool debugMode);

        [DllImport("__Internal")]
        private static extern bool UnityAdsIsReady(string placementId);

        [DllImport("__Internal")]
        private static extern long UnityAdsGetPlacementState(string placementId);

        [DllImport("__Internal")]
        private static extern string UnityAdsGetVersion();

        [DllImport("__Internal")]
        private static extern bool UnityAdsIsInitialized();

        [DllImport("__Internal")]
        private static extern void UnityAdsSetMetaData(string category, string data);

        [DllImport("__Internal")]
        private static extern void UnityAdsSetReadyCallback(UnityAdsReadyDelegate callback);

        [DllImport("__Internal")]
        private static extern void UnityAdsSetDidErrorCallback(UnityAdsDidErrorDelegate callback);

        [DllImport("__Internal")]
        private static extern void UnityAdsSetDidStartCallback(UnityAdsDidStartDelegate callback);

        [DllImport("__Internal")]
        private static extern void UnityAdsSetDidFinishCallback(UnityAdsDidFinishDelegate callback);

        [MonoPInvokeCallback(typeof(UnityAdsReadyDelegate))]
        private static void UnityAdsReady(string placementId)
        {
            s_Platform?.UnityAdsReady(placementId);
        }

        [MonoPInvokeCallback(typeof(UnityAdsDidErrorDelegate))]
        private static void UnityAdsDidError(long rawError, string message)
        {
            s_Platform?.UnityAdsDidError(message);
        }

        [MonoPInvokeCallback(typeof(UnityAdsDidStartDelegate))]
        private static void UnityAdsDidStart(string placementId)
        {
            s_Platform?.UnityAdsDidStart(placementId);
        }

        [MonoPInvokeCallback(typeof(UnityAdsDidFinishDelegate))]
        private static void UnityAdsDidFinish(string placementId, long rawShowResult)
        {
            s_Platform?.UnityAdsDidFinish(placementId, (ShowResult)rawShowResult);
        }

        public void SetupPlatform(IPlatform iosPlatform)
        {
            s_Platform = iosPlatform;

            UnityAdsSetReadyCallback(UnityAdsReady);
            UnityAdsSetDidErrorCallback(UnityAdsDidError);
            UnityAdsSetDidStartCallback(UnityAdsDidStart);
            UnityAdsSetDidFinishCallback(UnityAdsDidFinish);
        }

        public void Initialize(string gameId, bool testMode, bool enablePerPlacementLoad)
        {
            new PurchasingPlatform().Initialize();
            UnityAdsInitialize(gameId, testMode, enablePerPlacementLoad);
        }

        public void Load(string placementId)
        {
            UnityAdsLoad(placementId);
        }

        public void Show(string placementId)
        {
            UnityAdsShow(placementId);
        }

        public void SetMetaData(MetaData metaData)
        {
            UnityAdsSetMetaData(metaData.category, metaData.ToJSON());
        }

        public bool GetDebugMode()
        {
            return UnityAdsGetDebugMode();
        }

        public void SetDebugMode(bool debugMode)
        {
            UnityAdsSetDebugMode(debugMode);
        }

        public string GetVersion()
        {
            return UnityAdsGetVersion();
        }

        public bool IsInitialized()
        {
            return UnityAdsIsInitialized();
        }

        public bool IsReady(string placementId)
        {
            return UnityAdsIsReady(placementId);
        }

        public PlacementState GetPlacementState(string placementId)
        {
            return (PlacementState)UnityAdsGetPlacementState(placementId);
        }
    }
}
#endif
