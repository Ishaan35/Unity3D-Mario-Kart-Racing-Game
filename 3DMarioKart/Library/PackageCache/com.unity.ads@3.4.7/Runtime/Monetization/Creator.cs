using System;

namespace UnityEngine.Monetization
{
    static class Creator
    {
        static internal IMonetizationPlatform CreatePlatform()
        {
            try
            {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
                return new Platform();
#else
                return new UnsupportedPlatform();
#endif
            }
            catch (Exception exception)
            {
                try
                {
                    Debug.LogError("Error Initializing Unity Monetization.");
                    Debug.LogError(exception.Message);
                }
                catch (MissingMethodException)
                {
                }
                return new UnsupportedPlatform();
            }
        }
    }
}
