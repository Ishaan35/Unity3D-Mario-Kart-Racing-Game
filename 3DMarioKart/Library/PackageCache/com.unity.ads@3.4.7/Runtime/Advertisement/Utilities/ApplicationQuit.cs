using UnityEngine.Events;

namespace UnityEngine.Advertisements.Utilities
{
    internal class ApplicationQuit : MonoBehaviour
    {
        public event UnityAction OnApplicationQuitEventHandler;

        private void OnApplicationQuit()
        {
            OnApplicationQuitEventHandler?.Invoke();
        }
    }
}
