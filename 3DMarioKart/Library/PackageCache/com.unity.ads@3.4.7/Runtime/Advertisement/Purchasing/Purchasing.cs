using System;
using System.Reflection;

namespace UnityEngine.Advertisements.Purchasing
{
    /// <summary>
    /// Enumerated events related to in-app purchasing (IAP).
    /// </summary>
    public enum PurchasingEvent
    {
        COMMAND,
        VERSION,
        CATALOG,
        INITIALIZATION,
        EVENT
    }
    static class Purchasing
    {
        static Type s_PurchasingManagerType;
        static Boolean s_Initialized;
        static MethodInfo s_PurchasingInitiatePurchaseMethodInfo,
                          s_PurchasingGetPromoVersionMethodInfo,
                          s_PurchasingGetPromoCatalogMethodInfo;
        static string s_PurchasingManagerClassName = "UnityEngine.Purchasing.Promo,Stores";
        static string s_PurchasingInitiatePurchaseMethodName = "InitiatePurchasingCommand",
                      s_PurchasingGetPromoVersionMethodName = "Version",
                      s_PurchasingGetPromoCatalogMethodName = "QueryPromoProducts";
        static IPurchasingEventSender s_Platform;

        public static Boolean Initialize(IPurchasingEventSender platform)
        {
            if (!s_Initialized)
            {
                try
                {
                    s_PurchasingManagerType = Type.GetType(s_PurchasingManagerClassName, true);
                    s_PurchasingInitiatePurchaseMethodInfo = s_PurchasingManagerType.GetMethod(s_PurchasingInitiatePurchaseMethodName, new Type[] { typeof(string) });
                    s_PurchasingGetPromoVersionMethodInfo = s_PurchasingManagerType.GetMethod(s_PurchasingGetPromoVersionMethodName);
                    s_PurchasingGetPromoCatalogMethodInfo = s_PurchasingManagerType.GetMethod(s_PurchasingGetPromoCatalogMethodName);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(exception.Message + "It is likely that a promo has been enabled on a placement, but IAP Promo has not been enabled in the project.");
                    return false;
                }
                s_Initialized = true;
                s_Platform = platform;
            }
            return s_Initialized;
        }

        public static Boolean InitiatePurchasingCommand(string eventString)
        {
            Boolean isCommandSuccessful = false;
            if (s_PurchasingInitiatePurchaseMethodInfo != null)
            {
                try
                {
                    isCommandSuccessful = (Boolean)s_PurchasingInitiatePurchaseMethodInfo.Invoke(s_PurchasingManagerType, new[] { eventString });
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(exception.Message);
                    return false;
                }
            }
            return isCommandSuccessful;
        }

        public static String GetPurchasingCatalog()
        {
            String purchasingCatalog = "";
            if (s_PurchasingGetPromoCatalogMethodInfo != null)
            {
                try
                {
                    purchasingCatalog = (String)s_PurchasingGetPromoCatalogMethodInfo.Invoke(s_PurchasingManagerType, null);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(exception.Message);
                }
            }
            return purchasingCatalog ?? "NULL";
        }

        public static String GetPromoVersion()
        {
            String promoVersion = "";
            if (s_PurchasingGetPromoVersionMethodInfo != null)
            {
                try
                {
                    promoVersion = (String)s_PurchasingGetPromoVersionMethodInfo.Invoke(s_PurchasingManagerType, null);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(exception.Message);
                }
            }
            return promoVersion ?? "NULL";
        }

        public static Boolean SendEvent(string payload)
        {
            if (s_Platform == null)
            {
                return false;
            }
            else
            {
                s_Platform.SendPurchasingEvent(payload);
                return true;
            }
        }
    }
}
