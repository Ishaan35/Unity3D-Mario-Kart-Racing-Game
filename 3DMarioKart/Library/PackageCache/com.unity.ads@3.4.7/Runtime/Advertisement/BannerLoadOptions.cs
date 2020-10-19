namespace UnityEngine.Advertisements
{
    /// <summary>
    /// Pass these options back to the SDK to notify it of events when loading the banner.
    /// </summary>
    public class BannerLoadOptions
    {
        public delegate void LoadCallback();
        public delegate void ErrorCallback(string message);

        /// <summary>
        /// This callback fires when the banner loads successfully and is available to show.
        /// </summary>
        public LoadCallback loadCallback { get; set; }

        /// <summary>
        /// This callback fires when an error occurs while loading the banner. If invoked, assume that the banner did not load.
        /// </summary>
        public ErrorCallback errorCallback { get; set; }
    }
}
