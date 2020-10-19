using System;

namespace UnityEngine.Monetization
{
    public class PlacementContentReadyEventArgs : EventArgs
    {
        public string placementId;
        public PlacementContent placementContent;
        public PlacementContentReadyEventArgs(string id, PlacementContent placementContent)
        {
            this.placementId = id;
            this.placementContent = placementContent;
        }
    }
}
