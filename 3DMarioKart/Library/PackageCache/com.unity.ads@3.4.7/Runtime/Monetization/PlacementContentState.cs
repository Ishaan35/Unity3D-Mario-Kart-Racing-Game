using System;

namespace UnityEngine.Monetization
{
    public enum PlacementContentState
    {
        /// <summary>
        /// Placement is ready to show ads.
        /// </summary>
        Ready,
        /// <summary>
        /// Placement is not available.
        /// </summary>
        NotAvailable,
        /// <summary>
        /// Placement has been disabled.
        /// </summary>
        Disabled,
        /// <summary>
        /// Placement is waiting to be ready.
        /// </summary>
        Waiting,
        /// <summary>
        /// Placement has no advertisements to show.
        /// </summary>
        NoFill
    }
}
