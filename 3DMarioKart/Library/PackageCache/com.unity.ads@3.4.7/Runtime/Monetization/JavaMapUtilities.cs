using System.Collections.Generic;

namespace UnityEngine.Monetization
{
    internal class JavaMapUtilities
    {
        public static IDictionary<string, object> GetDictionaryForJavaMap(AndroidJavaObject javaMap)
        {
            if (javaMap == null)
            {
                return new Dictionary<string, object>();
            }

            var jsonJavaObject = new AndroidJavaObject("org.json.JSONObject", javaMap);
            var json = jsonJavaObject.Call<string>("toString");
            var jsonObject = MiniJSON.Json.Deserialize(json);
            if (jsonObject is IDictionary<string, object> objects)
            {
                return objects;
            }

            return new Dictionary<string, object>();
        }
    }
}
