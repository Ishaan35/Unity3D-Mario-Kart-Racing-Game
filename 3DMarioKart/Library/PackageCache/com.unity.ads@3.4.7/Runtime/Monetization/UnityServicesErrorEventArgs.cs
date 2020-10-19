using System;

namespace UnityEngine.Monetization
{
    public class UnityServicesErrorEventArgs : EventArgs
    {
        // error is not mapped to an enum as it is not exposed to public API, can be changed later
        public long error { get; private set; }
        public string message { get; private set; }

        public UnityServicesErrorEventArgs(string message)
        {
            error = -1;
            this.message = message;
        }

        public UnityServicesErrorEventArgs(long error, string message)
        {
            this.error = error;
            this.message = message;
        }
    }
}
