#if UNITY_ANDROID
using System;

namespace UnityEngine.Monetization
{
    public class JavaEnumUtilities
    {
        public static int GetEnumOrdinal(AndroidJavaObject javaEnumObject)
        {
            return javaEnumObject.Call<int>("ordinal");
        }
    }
}
#endif
