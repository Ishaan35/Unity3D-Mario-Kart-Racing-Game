using System;
using System.Collections.Generic;

namespace UnityEngine.Advertisements.Utilities
{
    internal class CoroutineExecutor : MonoBehaviour
    {
        public int referenceCount;
        public readonly Queue<Action> queue = new Queue<Action>();

        private void Update()
        {
            lock (queue)
            {
                while (queue.Count > 0)
                {
                    queue.Dequeue()?.Invoke();
                }
            }
        }
    }
}
