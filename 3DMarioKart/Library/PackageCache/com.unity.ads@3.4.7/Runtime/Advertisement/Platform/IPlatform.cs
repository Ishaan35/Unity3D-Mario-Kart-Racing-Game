using System;
using System.Collections.Generic;
using UnityEngine.Advertisements.Events;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements.Platform
{
    internal interface IPlatform
    {
        event EventHandler<StartEventArgs> OnStart;
        event EventHandler<FinishEventArgs> OnFinish;

        IBanner Banner { get; }
        IUnityLifecycleManager UnityLifecycleManager { get; }
        INativePlatform NativePlatform { get; }

        bool IsInitialized { get; }
        bool IsShowing { get; }
        string Version { get; }
        bool DebugMode { get; set; }

        HashSet<IUnityAdsListener> Listeners { get; }

        void Initialize(string gameId, bool testMode, bool enablePerPlacementLoad);
        void Load(string placementId);
        void Show(string placementId, ShowOptions showOptions);

        void AddListener(IUnityAdsListener listener);
        void RemoveListener(IUnityAdsListener listener);

        bool IsReady(string placementId);
        PlacementState GetPlacementState(string placementId);
        void SetMetaData(MetaData metaData);

        void UnityAdsReady(string placementId);
        void UnityAdsDidError(string message);
        void UnityAdsDidStart(string placementId);
        void UnityAdsDidFinish(string placementId, ShowResult rawShowResult);
    }
}
