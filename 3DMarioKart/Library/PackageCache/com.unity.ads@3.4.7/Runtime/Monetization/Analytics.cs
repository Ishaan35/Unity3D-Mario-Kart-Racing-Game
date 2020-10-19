using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Monetization
{
    /// <summary>
    /// Decorates Unity Analytics "Standard Events" with extras for correlating events to our game and advertising IDs.
    /// </summary>
    static class Analytics
    {
        // See also https://gitlab.internal.unity3d.com/upm-packages/analytics/com.unity.analytics
        static string s_StandardEventsClassName = "UnityEngine.Analytics.AnalyticsEvent,Unity.Analytics.StandardEvents";
        static Type s_StandardEventsType;
        // NOTICE: Update the UnityAds/link.xml file when you change these method names
        static string s_StandardEventsRegisterMethodName = "Register";
        static string s_StandardEventsUnregisterMethodName = "Unregister";
        static MethodInfo s_StandardEventsRegisterMethodInfo;
        static MethodInfo s_StandardEventsUnregisterMethodInfo;

        /// <summary>
        /// Collects extras before passing to Standard Events
        /// </summary>
        static IDictionary<string, object> s_StandardEventsExtras = new Dictionary<string, object>();

        /// <summary>
        /// Used by the event-sending pipeline of Standard Events
        /// </summary>
        static Action<IDictionary<string, object>> s_StandardEventsAction = eventData =>
        {
            foreach (var extra in s_StandardEventsExtras)
            {
                if (eventData.ContainsKey(extra.Key))
                {
                    eventData.Remove(extra.Key);
                }

                eventData.Add(extra.Key, extra.Value);
            }
        };

        static bool InitializeStandardEvents()
        {
            try
            {
                if (s_StandardEventsType == null)
                {
                    // Unity Analytics Standard Events class is named "AnalyticsEvent"
                    // Is an optional component
                    s_StandardEventsType = Type.GetType(s_StandardEventsClassName, true);

                    if (s_StandardEventsType != null)
                    {
                        s_StandardEventsRegisterMethodInfo = s_StandardEventsType.GetMethod(s_StandardEventsRegisterMethodName, new Type[] { typeof(Action<IDictionary<string, object>>) });
                        s_StandardEventsUnregisterMethodInfo = s_StandardEventsType.GetMethod(s_StandardEventsUnregisterMethodName, new Type[] { typeof(Action<IDictionary<string, object>>) });
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            return s_StandardEventsRegisterMethodInfo != null && s_StandardEventsUnregisterMethodInfo != null;
        }

        public static bool SetAnalyticsEventExtra(string jsonExtras)
        {
            bool finalResult = true;
            Dictionary<string, object> test = (Dictionary<string, object>)MiniJSON.Json.Deserialize(jsonExtras);
            foreach (KeyValuePair<string, object> entry in test)
            {
                Boolean result = SetAnalyticsEventExtra(entry.Key, entry.Value);
                if (!result)
                {
                    finalResult = false;
                }
            }

            return finalResult;
        }

        /// <summary>
        /// Set a key/value pair as additional outgoing data from the Unity Analytics Standard Events
        /// AnalyticsEvent class.
        /// Supports multiple pairs if called repeatedly with unique keys.
        /// Supports updating when called repeatedly with non-unique key.
        /// Supports removal when value is null.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>false if setting or updating was not possible, potentially due to missing depen.</returns>
        public static Boolean SetAnalyticsEventExtra(string key, object value)
        {
            if (!InitializeStandardEvents())
            {
                return false;
            }

            if (s_StandardEventsExtras.ContainsKey(key))
            {
                s_StandardEventsExtras.Remove(key);
            }

            if (value != null)
            {
                s_StandardEventsExtras.Add(key, value);
            }

            try
            {
                // Avoid over-subscribing to this delegate
                s_StandardEventsUnregisterMethodInfo.Invoke(s_StandardEventsType, new[] { s_StandardEventsAction });

                // Subscribe to the delegate
                s_StandardEventsRegisterMethodInfo.Invoke(s_StandardEventsType, new[] { s_StandardEventsAction });
            }
            catch (Exception exception)
            {
                Debug.Log(exception.Message);
            }

            return true;
        }
    }
}
