using System;
using System.Collections;
using UnityEngine.Events;

namespace UnityEngine.Advertisements.Utilities
{
    /// <summary>
    /// A helper class for running coroutines from non <see cref="T:UnityEngine.MonoBehaviour" /> classes.
    /// </summary>
    internal class UnityLifecycleManager : IUnityLifecycleManager
    {
        internal const string gameObjectName = "UnityEngine_UnityAds_CoroutineExecutor";

        private GameObject m_GameObject;
        private CoroutineExecutor m_CoroutineExecutor;
        private ApplicationQuit m_ApplicationQuit;
        private bool m_Disposed;

        public UnityLifecycleManager() {
            Initialize();
        }

        private void Initialize() {
            var existingCoroutineExecutorGameObject = GameObject.Find(gameObjectName);
            if (existingCoroutineExecutorGameObject != null)
            {
                m_GameObject = existingCoroutineExecutorGameObject;
                m_CoroutineExecutor = m_GameObject.GetComponent<CoroutineExecutor>();

                if (m_CoroutineExecutor != null)
                {
                    m_CoroutineExecutor.referenceCount++;
                    return;
                }
                else
                {
                    GameObject.DestroyImmediate(m_GameObject);
                    m_GameObject = null;
                }
            }

            m_GameObject = new GameObject(gameObjectName) { hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector};
            m_CoroutineExecutor = m_GameObject.AddComponent<CoroutineExecutor>();
            m_ApplicationQuit = m_GameObject.AddComponent<ApplicationQuit>();
            m_CoroutineExecutor.referenceCount++;

            GameObject.DontDestroyOnLoad(m_GameObject);
        }

        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            if (!m_CoroutineExecutor) {
                Initialize();
            }
            return m_CoroutineExecutor?.StartCoroutine(enumerator);
        }

        public void Post(Action action)
        {
            if (!m_CoroutineExecutor) {
                Initialize();
            }
            lock (m_CoroutineExecutor.queue)
            {
                m_CoroutineExecutor?.queue?.Enqueue(action);
            }
        }

        public void Dispose()
        {
            if (!m_Disposed)
            {
                m_Disposed = true;

                m_CoroutineExecutor.referenceCount--;
                if (m_CoroutineExecutor.referenceCount == 0) {
                    Object.DestroyImmediate(m_GameObject);
                }

                m_GameObject = null;
                m_CoroutineExecutor = null;
                m_ApplicationQuit = null;
            }
        }

        public void SetOnApplicationQuitCallback(UnityAction callback)
        {
            if (m_ApplicationQuit != null)
            {
                m_ApplicationQuit.OnApplicationQuitEventHandler += callback;
            }
        }
    }
}
