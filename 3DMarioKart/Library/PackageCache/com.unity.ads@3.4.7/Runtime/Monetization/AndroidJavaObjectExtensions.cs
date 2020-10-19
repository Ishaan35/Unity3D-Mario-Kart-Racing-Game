#if UNITY_ANDROID
namespace UnityEngine.Monetization
{
    public static class AndroidJavaObjectExtensions
    {
        private static int _sdkVersion = -1;
        private static readonly int BuildVersionKitKat = 19;

        public static string SafeStringCall(this AndroidJavaObject javaObject, string methodName)
        {
            if (_sdkVersion == -1)
            {
                _sdkVersion = GetSDKLevel();
            }
            if (_sdkVersion <= BuildVersionKitKat)
            {
                var stringJavaObject = javaObject.Call<AndroidJavaObject>(methodName);
                return stringJavaObject?.Call<string>("toString");
            }

            return javaObject.Call<string>(methodName);
        }

        public static int GetSDKLevel()
        {
            using (var clazz = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return clazz.GetStatic<int>("SDK_INT");
            }
        }
    }
}
#endif
