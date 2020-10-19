#if UNITY_ANDROID
using System;

namespace UnityEngine.Monetization
{
    sealed class AndroidAnalytics : AndroidJavaProxy
    {
        readonly AndroidJavaClass m_UnityAnalytics;
        void onAddExtras(String extras)
        {
            Analytics.SetAnalyticsEventExtra(extras);
        }

        public void Initialize()
        {
            m_UnityAnalytics.CallStatic("initialize", this);
        }

        public AndroidAnalytics() : base("com.unity3d.services.analytics.interfaces.IAnalytics")
        {
            m_UnityAnalytics = new AndroidJavaClass("com.unity3d.services.analytics.interfaces.Analytics");
        }
    }
}
#endif
