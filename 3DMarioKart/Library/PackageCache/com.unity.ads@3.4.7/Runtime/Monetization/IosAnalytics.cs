#if UNITY_IOS
using System.Runtime.InteropServices;
using AOT;

namespace UnityEngine.Monetization
{
    sealed internal class IosAnalytics
    {
        private static IosAnalytics Instance { get; set; }

        delegate void unityAnalyticsTriggerAddExtras(string jsonExtras);

        [DllImport("__Internal")]
        static extern void UANAEngineDelegateSetTriggerAddExtras(unityAnalyticsTriggerAddExtras trigger);

        [DllImport("__Internal")] private static extern void InitializeUANAEngineWrapper();

        [MonoPInvokeCallback(typeof(unityAnalyticsTriggerAddExtras))]
        static void TriggerAddExtras(string extras)
        {
            Analytics.SetAnalyticsEventExtra(extras);
        }

        public void Initialize()
        {
            Instance = this;
            UANAEngineDelegateSetTriggerAddExtras(TriggerAddExtras);
            InitializeUANAEngineWrapper();
        }
    }
}
#endif
