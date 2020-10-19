using System;

namespace UnityEngine.Advertisements.Events
{
    class StartEventArgs : EventArgs
    {
        public string placementId { get; }

        public StartEventArgs(string placementId)
        {
            this.placementId = placementId;
        }
    }
}
