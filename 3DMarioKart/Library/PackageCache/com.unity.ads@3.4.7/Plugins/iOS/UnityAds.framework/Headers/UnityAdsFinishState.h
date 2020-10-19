/**
 *  An enumeration for the completion state of an ad.
 */
typedef NS_ENUM(NSInteger, UnityAdsFinishState) {
    /**
     *  A state that indicates that the ad did not successfully display.
     */
    kUnityAdsFinishStateError,
    /**
     *  A state that indicates that the user skipped the ad.
     */
    kUnityAdsFinishStateSkipped,
    /**
     *  A state that indicates that the ad was played entirely.
     */
    kUnityAdsFinishStateCompleted
};
