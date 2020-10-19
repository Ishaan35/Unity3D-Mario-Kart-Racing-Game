using System;

namespace UnityEngine.Monetization
{
    [Obsolete("Deprecated.  Please use Advertisements", false)]
    public static class Monetization
    {
        public static event EventHandler<PlacementContentReadyEventArgs> onPlacementContentReady
        {
            add
            {
                s_Platform.OnPlacementContentReady += value;
            }
            remove
            {
                s_Platform.OnPlacementContentReady -= value;
            }
        }
        public static event EventHandler<PlacementContentStateChangeEventArgs> onPlacementContentStateChange
        {
            add
            {
                s_Platform.OnPlacementContentStateChange += value;
            }
            remove
            {
                s_Platform.OnPlacementContentStateChange -= value;
            }
        }

        static IMonetizationPlatform s_Platform;
        static bool s_Initialized;

        static internal IMonetizationPlatform platform
        {
            get { return s_Platform; }
            set { s_Platform = value; }
        }

        static Monetization()
        {
            s_Platform = Creator.CreatePlatform();
        }

        /// <summary>
        /// Returns the current Unity Monetization version.
        /// </summary>
        public static string version
        {
            get
            {
                return s_Platform.version;
            }
        }

        /// <summary>
        /// Returns whether the monetization system is initialized successfully.
        /// </summary>
        public static bool isInitialized
        {
            get
            {
                return s_Initialized;
            }
            internal set
            {
                s_Initialized = value;
            }
        }

        /// <summary>
        /// Returns if the current platform is supported by the monetization system.
        /// </summary>
        public static bool isSupported
        {
            get
            {
                bool supported = Application.isEditor ||
                    (Application.platform == RuntimePlatform.Android && s_Platform.isSupported) ||
                    (Application.platform == RuntimePlatform.IPhonePlayer && s_Platform.isSupported);


                return supported;
            }
        }

        public static bool IsReady(string placementId)
        {
            return s_Platform.IsReady(string.IsNullOrEmpty(placementId) ? null : placementId);
        }

        /// <summary>
        /// Initialize the monetization system with specified gameId, testMode, IPurchasingAdapter
        /// </summary>
        /// <param name="gameId">Game identifier.</param>
        /// <param name="testMode">Test mode.</param>
        public static void Initialize(string gameId, bool testMode)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                var framework = new MetaData("framework");
                framework.Set("name", "Unity");
                framework.Set("version", Application.unityVersion);
                SetMetaData(framework);

                var adapter = new MetaData("adapter");
#if ASSET_STORE
                adapter.Set("name", "AssetStore");
#else
                adapter.Set("name", "Packman");
#endif
                adapter.Set("version", version);
                adapter.Set("flavor", "monetization");
                SetMetaData(adapter);

                s_Platform.onError += OnError;
                s_Platform.Initialize(gameId, testMode);
            }
        }

        public static void SetPurchasingAdapter(IPurchasingAdapter adapter)
        {
            s_Platform.SetPurchasingAdapter(adapter);
        }

        public static PlacementContent GetPlacementContent(string placementId)
        {
            return s_Platform.GetPlacementContent(placementId);
        }

        public static INativePromoAdapter CreateNativePromoAdapter(PromoAdPlacementContent placementContent)
        {
            return s_Platform.CreateNativePromoAdapter(placementContent);
        }

        public static void SetMetaData(MetaData metaData)
        {
            s_Platform.SetMetaData(metaData);
        }

        private static void OnError(object sender, UnityServicesErrorEventArgs e)
        {
            Debug.LogErrorFormat("Unity Services recieved error {0} - {1}", e?.error, e?.message);
        }
    }
}
