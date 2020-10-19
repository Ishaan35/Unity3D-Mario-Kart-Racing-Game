using System;
using System.Collections;
using UnityEngine.Events;

namespace UnityEngine.Advertisements.Utilities
{
    internal interface IUnityLifecycleManager : IDisposable
    {
        Coroutine StartCoroutine(IEnumerator enumerator);
        void Post(Action action);
        void SetOnApplicationQuitCallback(UnityAction action);
    }
}
