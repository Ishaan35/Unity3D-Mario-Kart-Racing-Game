using System;
using System.Collections.Generic;
using UnityEngine.Advertisements.Events;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements.Platform
{
    internal class Platform : IPlatform
    {
        public event EventHandler<StartEventArgs> OnStart;
        public event EventHandler<FinishEventArgs> OnFinish;

        public IBanner Banner { get; }
        public IUnityLifecycleManager UnityLifecycleManager { get; }
        public INativePlatform NativePlatform { get; }

        public bool IsInitialized => NativePlatform?.IsInitialized() ?? false;
        public bool IsShowing { get; private set; }
        public string Version => NativePlatform?.GetVersion() ?? "UnknownVersion";
        public bool DebugMode
        {
            get => NativePlatform?.GetDebugMode() ?? false;
            set => NativePlatform?.SetDebugMode(value);
        }

        public HashSet<IUnityAdsListener> Listeners { get; }

        public Platform(INativePlatform nativePlatform, IBanner banner, IUnityLifecycleManager unityLifecycleManager)
        {
            NativePlatform = nativePlatform;
            Banner = banner;
            UnityLifecycleManager = unityLifecycleManager;
            Listeners = new HashSet<IUnityAdsListener>();
            NativePlatform.SetupPlatform(this);
        }

        public void Initialize(string gameId, bool testMode, bool enablePerPlacementLoad)
        {
            if (!IsInitialized)
            {
                OnStart += (sender, e) =>
                {
                    IsShowing = true;
                };
                OnFinish += (sender, e) =>
                {
                    IsShowing = false;
                };

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
                adapter.Set("version", Version);
                SetMetaData(adapter);

                NativePlatform.Initialize(gameId, testMode, enablePerPlacementLoad);
            }
        }

        public void Load(string placementId)
        {
            if (string.IsNullOrEmpty(placementId))
            {
                Debug.LogError("placementId cannot be nil or empty");
                return;
            }

            NativePlatform.Load(placementId);
        }

        public void Show(string placementId, ShowOptions showOptions)
        {
            if (IsShowing) return;

            if (showOptions != null)
            {
#pragma warning disable 618
                if (showOptions.resultCallback != null)
                {
                    EventHandler<FinishEventArgs> finishHandler = null;
                    finishHandler = (object sender, FinishEventArgs e) =>
                    {
                        showOptions.resultCallback(e.showResult);
#pragma warning restore 618
                        OnFinish -= finishHandler;
                    };
                    OnFinish += finishHandler;
                }
                if (!string.IsNullOrEmpty(showOptions.gamerSid))
                {
                    var player = new MetaData("player");
                    player.Set("server_id", showOptions.gamerSid);
                    SetMetaData(player);
                }
            }
            NativePlatform.Show(string.IsNullOrEmpty(placementId) ? null : placementId);
        }

        public void AddListener(IUnityAdsListener listener)
        {
            Listeners?.Add(listener);
        }

        public void RemoveListener(IUnityAdsListener listener)
        {
            Listeners?.Remove(listener);
        }

        public bool IsReady(string placementId)
        {
            return NativePlatform.IsReady(placementId);
        }

        public PlacementState GetPlacementState(string placementId)
        {
            return NativePlatform.GetPlacementState(placementId);
        }

        public void SetMetaData(MetaData metaData)
        {
            NativePlatform.SetMetaData(metaData);
        }

        public void UnityAdsReady(string placementId)
        {
            UnityLifecycleManager.Post(() => {
                foreach (var listener in GetClonedHashSet(Listeners))
                {
                    listener?.OnUnityAdsReady(placementId);
                }
            });
        }

        public void UnityAdsDidError(string message)
        {
            UnityLifecycleManager.Post(() => {
                foreach (var listener in GetClonedHashSet(Listeners))
                {
                    listener?.OnUnityAdsDidError(message);
                }
            });
        }

        public void UnityAdsDidStart(string placementId)
        {
            UnityLifecycleManager.Post(() => {
                OnStart?.Invoke(this, new StartEventArgs(placementId));

                foreach (var listener in GetClonedHashSet(Listeners))
                {
                    listener?.OnUnityAdsDidStart(placementId);
                }
            });
        }

        public void UnityAdsDidFinish(string placementId, ShowResult rawShowResult)
        {
            UnityLifecycleManager.Post(() => {
                OnFinish?.Invoke(this, new FinishEventArgs(placementId, rawShowResult));

                foreach (var listener in GetClonedHashSet(Listeners))
                {
                    listener?.OnUnityAdsDidFinish(placementId, rawShowResult);
                }
            });
        }

        internal static HashSet<IUnityAdsListener> GetClonedHashSet(HashSet<IUnityAdsListener> hashSet)
        {
            return new HashSet<IUnityAdsListener>(hashSet);
        }
    }
}
