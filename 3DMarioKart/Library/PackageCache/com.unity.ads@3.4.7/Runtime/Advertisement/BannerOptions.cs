using System;

namespace UnityEngine.Advertisements
{
    /// <summary>
    /// Pass these options back to the SDK to notify it of events when displaying the banner.
    /// </summary>
    public class BannerOptions
    {
        public delegate void BannerCallback();

        /// <summary>
        /// This callback fires when the banner is visible to the user.
        /// </summary>
        public BannerCallback showCallback { get; set; }

        /// <summary>
        /// This callback fires when the banner is hidden from the user.
        /// </summary>
        public BannerCallback hideCallback { get; set; }

        /// <summary>
        /// This callback fires when the banner is clicked by the user.
        /// </summary>
        public BannerCallback clickCallback { get; set; }
    }
}
