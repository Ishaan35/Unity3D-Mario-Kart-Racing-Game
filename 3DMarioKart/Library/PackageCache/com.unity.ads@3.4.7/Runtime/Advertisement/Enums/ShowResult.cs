namespace UnityEngine.Advertisements
{
    /// <summary>
    /// An enum passed to <a href="../api/Advertisements.ShowResult.html">ShowOptions.resultCallback</a> after the ad has finished, indicating the result.
    /// </summary>
    public enum ShowResult
    {
        /// <summary>
        /// Indicates that the ad failed to display.
        /// </summary>
        Failed,
        /// <summary>
        ///     Indicates that the player did not allow the ad to complete.
        /// </summary>
        Skipped,
        /// <summary>
        /// Indicates that the player watched the ad to completion.
        /// </summary>
        Finished
    }
}
