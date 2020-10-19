#import "UnityAdsFinishState.h"
#import "UnityAdsError.h"

NS_ASSUME_NONNULL_BEGIN

/**
 *  The `UnityAdsDelegate` protocol defines the required methods for receiving messages from UnityAds.
 *  Must be implemented by the hosting app.
 *  The unityAdsReady: method is called when it's possible to show an ad.
 *  All other methods are used to provide notifications of events of the ad lifecycle.
 *  @note On initialization, there are ready (or error) callbacks for each placement attached to the game identifier.
 */
@protocol UnityAdsDelegate <NSObject>
/**
 *  Called when `UnityAds` is ready to show an ad. After this callback you can call the `UnityAds` `show:` method for this placement.
 *  Note that sometimes placement might no longer be ready due to exceptional reasons. These situations will give no new callbacks.
 *
 *  @warning To avoid error situations, it is always best to check `isReady` method status before calling show.
 *  @param placementId The ID of the placement that is ready to show, as defined in Unity Ads admin tools.
 */
- (void)unityAdsReady:(NSString *)placementId;
/**
 *  Called when `UnityAds` encounters an error. All errors will be logged but this method can be used as an additional debugging aid. This callback can also be used for collecting statistics from different error scenarios.
 *
 *  @param error   A `UnityAdsError` error enum value indicating the type of error encountered.
 *  @param message A human readable string indicating the type of error encountered.
 */
- (void)unityAdsDidError:(UnityAdsError)error withMessage:(NSString *)message;
/**
 *  Called on a successful start of advertisement after calling the `UnityAds` `show:` method.
 *
 * @warning If there are errors in starting the advertisement, this method may never be called. Unity Ads will directly call `unityAdsDidFinish:withFinishState:` with error status.
 *
 *  @param placementId The ID of the placement that has started, as defined in Unity Ads admin tools.
 */
- (void)unityAdsDidStart:(NSString *)placementId;
/**
 *  Called after the ad has closed.
 *
 *  @param placementId The ID of the placement that has finished, as defined in Unity Ads admin tools.
 *  @param state       An enum value indicating the finish state of the ad. Possible values are `Completed`, `Skipped`, and `Error`.
 */
- (void)unityAdsDidFinish:(NSString *)placementId
          withFinishState:(UnityAdsFinishState)state;
@end

NS_ASSUME_NONNULL_END
