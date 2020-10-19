/**
 *  An enumeration for the various errors that can be emitted through the `UnityAdsDelegate` `unityAdsDidError:withMessage:` method.
 */
typedef NS_ENUM(NSInteger, UnityAdsError) {
    /**
     *  An error that indicates failure due to `UnityAds` currently being uninitialized.
     */
            kUnityAdsErrorNotInitialized = 0,
    /**
     *  An error that indicates failure due to a failure in the initialization process.
     */
            kUnityAdsErrorInitializedFailed,
    /**
     *  An error that indicates failure due to attempting to initialize `UnityAds` with invalid parameters.
     */
            kUnityAdsErrorInvalidArgument,
    /**
     *  An error that indicates failure of the video player.
     */
            kUnityAdsErrorVideoPlayerError,
    /**
     *  An error that indicates failure due to having attempted to initialize the `UnityAds` class in an invalid environment.
     */
            kUnityAdsErrorInitSanityCheckFail,
    /**
     *  An error that indicates failure due to the presence of an ad blocker.
     */
            kUnityAdsErrorAdBlockerDetected,
    /**
     *  An error that indicates failure due to inability to read or write a file.
     */
            kUnityAdsErrorFileIoError,
    /**
     *  An error that indicates failure due to a bad device identifier.
     */
            kUnityAdsErrorDeviceIdError,
    /**
     *  An error that indicates a failure when attempting to show an ad.
     */
            kUnityAdsErrorShowError,
    /**
     *  An error that indicates an internal failure in `UnityAds`.
     */
            kUnityAdsErrorInternalError,
};
