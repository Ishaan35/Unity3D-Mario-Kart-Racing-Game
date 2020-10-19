using System;

namespace UnityEngine.Advertisements.Events
{
    class FinishEventArgs : EventArgs
    {
        public string placementId { get; }
        public ShowResult showResult { get; }

        public FinishEventArgs(string placementId, ShowResult showResult)
        {
            this.placementId = placementId;
            this.showResult = showResult;
        }
    }
}
