using System;

namespace UnityEngine.Advertisements.Platform.Unsupported
{
    internal sealed class UnsupportedPlatform : INativePlatform
    {
        public void SetupPlatform(IPlatform platform) {}

        public void Initialize(string gameId, bool testMode, bool enablePerPlacementLoad) {}

        public void Load(string placementId) {}

        public void Show(string placementId) {}

        public void SetMetaData(MetaData metaData) {}

        public bool GetDebugMode()
        {
            return false;
        }

        public void SetDebugMode(bool debugMode) {}

        public string GetVersion()
        {
            return "UnsupportedPlatformVersion";
        }

        public bool IsInitialized()
        {
            return false;
        }

        public bool IsReady(string placementId)
        {
            return false;
        }

        public PlacementState GetPlacementState(string placementId)
        {
            return PlacementState.NotAvailable;
        }
    }
}
