namespace UnityEngine.Advertisements
{
    /// <summary>
    /// The enumerated states of a Unity Ads <a href="../manual/MonetizationPlacements.html">Placement</a>.
    /// </summary>
    public enum PlacementState
    {
        /// <summary>
        /// The Placement is ready to show ads.
        /// </summary>
        Ready,
        /// <summary>
        /// The Placement is not available.
        /// </summary>
        NotAvailable,
        /// <summary>
        /// The Placement is disabled.
        /// </summary>
        Disabled,
        /// <summary>
        /// The Placement is waiting to be ready.
        /// </summary>
        Waiting,
        /// <summary>
        /// The Placement has no ads available to show.
        /// </summary>
        NoFill
    }
}
