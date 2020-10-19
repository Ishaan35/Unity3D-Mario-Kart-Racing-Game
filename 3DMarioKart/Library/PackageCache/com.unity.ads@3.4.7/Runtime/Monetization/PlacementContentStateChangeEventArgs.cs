using System;

namespace UnityEngine.Monetization
{
    public class PlacementContentStateChangeEventArgs : EventArgs
    {
        public string placementId;
        public PlacementContent placementContent;
        public PlacementContentState previousState;
        public PlacementContentState newState;

        public PlacementContentStateChangeEventArgs(string id, PlacementContent placementContent, PlacementContentState preState, PlacementContentState newState)
        {
            this.placementId = id;
            this.placementContent = placementContent;
            this.previousState = preState;
            this.newState = newState;
        }
    }
}
