#import <UIKit/UIKit.h>

#import <UnityAds/UADSMediationMetaData.h>
#import <UnityAds/UADSPlayerMetaData.h>
#import <UnityAds/UADSInAppPurchaseMetaData.h>
#import <UnityAds/UnityServices.h>
#import "UADSBanner.h"
#import "UADSPurchasing.h"
#import "UANAApiAnalytics.h"
#import "UMONPlacementContent.h"
#import "UMONPromoMetaData.h"
#import "UMONRewardablePlacementContent.h"
#import "UPURProduct.h"
#import "UPURTransactionDetails.h"
#import "UPURTransactionError.h"
#import "UPURTransactionErrorDetails.h"
#import "UPURStore.h"
#import "USRVUnityPurchasing.h"
#import "UnityAnalytics.h"
#import "UnityAnalyticsAcquisitionType.h"
#import "UnityMonetizationDelegate.h"
#import "UnityAdsFinishState.h"
#import "UMONShowAdPlacementContent.h"
#import "UMONPromoAdPlacementContent.h"
#import "UMONNativePromoAdapter.h"
#import "UnityMonetization.h"
#import "UnityAdsDelegate.h"
#import "UnityAdsPlacementState.h"
#import "NSString+UnityAdsError.h"
#import "UnityAdsExtendedDelegate.h"
#import "UADSBannerView.h"
#import "UADSBannerViewDelegate.h"
#import "UADSBannerError.h"

NS_ASSUME_NONNULL_BEGIN

/**
 * `UnityAds` is a static class with methods for preparing and showing ads.
 *
 * @warning In order to ensure expected behaviour, the delegate must always be set.
 */

@interface UnityAds : NSObject

- (instancetype)init NS_UNAVAILABLE;
+ (instancetype)initialize NS_UNAVAILABLE;

/**
 *  Initializes UnityAds. UnityAds should be initialized when app starts.
 *
 *  @param gameId   Unique identifier for a game, given by Unity Ads admin tools or Unity editor.
 *  @param delegate delegate for UnityAdsDelegate callbacks
 */
+ (void)initialize:(NSString *)gameId
          delegate:(nullable id<UnityAdsDelegate>)delegate __attribute__((deprecated("Please migrate to using initialize without a delegate and add the delegate with the addDelegate method")));

/**
 *  Initializes UnityAds. UnityAds should be initialized when app starts.
 *
 *  @param gameId   Unique identifier for a game, given by Unity Ads admin tools or Unity editor.
 */
+ (void)initialize:(NSString *)gameId;

/**
 *  Initializes UnityAds. UnityAds should be initialized when app starts.
 *
 *  @param gameId        Unique identifier for a game, given by Unity Ads admin tools or Unity editor.
 *  @param delegate      delegate for UnityAdsDelegate callbacks
 *  @param testMode      Set this flag to `YES` to indicate test mode and show only test ads.
 */
+ (void)initialize:(NSString *)gameId
          delegate:(nullable id<UnityAdsDelegate>)delegate
          testMode:(BOOL)testMode __attribute__((deprecated("Please migrate to using initialize without a delegate and add the delegate with the addDelegate method")));

/**
*  Initializes UnityAds. UnityAds should be initialized when app starts.
*
*  @param gameId        Unique identifier for a game, given by Unity Ads admin tools or Unity editor.
*  @param testMode      Set this flag to `YES` to indicate test mode and show only test ads.
*/
+ (void)initialize:(NSString *)gameId
          testMode:(BOOL)testMode;

/**
 *  Initializes UnityAds. UnityAds should be initialized when app starts.
 *  Note: The `load` API is in closed beta and available upon invite only. If you would like to be considered for the beta, please contact Unity Ads Support.
 *
 *  @param gameId        Unique identifier for a game, given by Unity Ads admin tools or Unity editor.
 *  @param delegate      delegate for UnityAdsDelegate callbacks
 *  @param testMode      Set this flag to `YES` to indicate test mode and show only test ads.
 *  @param enablePerPlacementLoad Set this flag to `YES` to disable automatic placement caching. When this is enabled, developer must call `load` on placements before calling show
 */
+ (void)initialize:(NSString *)gameId
                  delegate:(nullable id<UnityAdsDelegate>)delegate
                  testMode:(BOOL)testMode
    enablePerPlacementLoad:(BOOL)enablePerPlacementLoad __attribute__((deprecated("Please migrate to using initialize without a delegate and add the delegate with the addDelegate method")));

/**
 *  Initializes UnityAds. UnityAds should be initialized when app starts.
 *  Note: The `load` API is in closed beta and available upon invite only. If you would like to be considered for the beta, please contact Unity Ads Support.
 *
 *  @param gameId        Unique identifier for a game, given by Unity Ads admin tools or Unity editor.
 *  @param testMode      Set this flag to `YES` to indicate test mode and show only test ads.
 *  @param enablePerPlacementLoad Set this flag to `YES` to disable automatic placement caching. When this is enabled, developer must call `load` on placements before calling show
 */
+ (void)initialize:(NSString *)gameId
                  testMode:(BOOL)testMode
    enablePerPlacementLoad:(BOOL)enablePerPlacementLoad;

/**
 *  Load a placement to make it available to show. Ads generally take a few seconds to finish loading before they can be shown.
 *  Note: The `load` API is in closed beta and available upon invite only. If you would like to be considered for the beta, please contact Unity Ads Support.
 *
 *  @param placementId The placement ID, as defined in Unity Ads admin tools.
 */
+ (void)load:(NSString *)placementId;
/**
 *  Show an ad using the defaul placement.
 *
 *  @param viewController The `UIViewController` that is to present the ad view controller.
 */
+ (void)show:(UIViewController *)viewController;
/**
 *  Show an ad using the provided placement ID.
 *
 *  @param viewController The `UIViewController` that is to present the ad view controller.
 *  @param placementId    The placement ID, as defined in Unity Ads admin tools.
 */
+ (void)show:(UIViewController *)viewController placementId:(NSString *)placementId;
/**
 *  Provides the currently assigned `UnityAdsDelegate`. Meant to support use of single delegate
 *
 *  @return The current `UnityAdsDelegate`.
 *  @deprecated this method is deprecated in favor of addDelegate
 */
+ (id<UnityAdsDelegate>)getDelegate __attribute__((deprecated("Please migrate to using addDelegate and removeDelegate")));
/**
 *  Allows the delegate to be reassigned after UnityAds has already been initialized. Meant to support use of a single delegate.
 *  Use `addDelegate` if you wish to have multiple `UnityAdsDelegate`
 *  Replaces any delegate set through `setDelegate` and through initialize
 *
 *  @param delegate The new `UnityAdsDelegate' for UnityAds to send callbacks to.
 *  @deprecated this method is deprecated in favor of addDelegate
 */
+ (void)setDelegate:(id<UnityAdsDelegate>)delegate __attribute__((deprecated("Please migrate to using addDelegate and removeDelegate")));

/**
 *  Allows a delegate to be registered after UnityAds has already been initialized.
 *
 *  @param delegate The new `UnityAdsDelegate' for UnityAds to send callbacks to.
 */
+ (void)addDelegate:(__nullable id<UnityAdsDelegate>)delegate;
/**
 *  Allows a delegate to be removed after UnityAds has already been initialized.
 *  This only removes delegates that have been added through `addDelegate`
 *
 *  @param delegate The already added `UnityAdsDelegate' for UnityAds to send callbacks to.
 */
+ (void)removeDelegate:(id<UnityAdsDelegate>)delegate;

/**
 *  Get the current debug status of `UnityAds`.
 *
 *  @return If `YES`, `UnityAds` will provide verbose logs.
 */
+ (BOOL)getDebugMode;
/**
 *  Set the logging verbosity of `UnityAds`. Debug mode indicates verbose logging.
 *  @warning Does not relate to test mode for ad content.
 *  @param enableDebugMode `YES` for verbose logging.
 */
+ (void)setDebugMode:(BOOL)enableDebugMode;
/**
 *  Check to see if the current device supports using Unity Ads.
 *
 *  @return If `NO`, the current device cannot initialize `UnityAds` or show ads.
 */
+ (BOOL)isSupported;
/**
 *  Check if the default placement is ready to show an ad.
 *
 *  @return If `YES`, the default placement is ready to show an ad.
 */
+ (BOOL)isReady;
/**
 *  Check if a particular placement is ready to show an ad.
 *
 *  @param placementId The placement ID being checked.
 *
 *  @return If `YES`, the placement is ready to show an ad.
 */
+ (BOOL)isReady:(NSString *)placementId;
/**
 *  Check the current state of the default placement.
 *
 *  @return If this is `kUnityAdsPlacementStateReady`, the placement is ready to show ads. Other states represent errors.
 */
+ (UnityAdsPlacementState)getPlacementState;
/**
 *  Check the current state of a placement.
 *
 *  @param placementId The placement ID, as defined in Unity Ads admin tools.
 *
 *  @return If this is `kUnityAdsPlacementStateReady`, the placement is ready to show ads. Other states represent errors.
 */
+ (UnityAdsPlacementState)getPlacementState:(NSString *)placementId;
/**
 *  Check the version of this `UnityAds` SDK
 *
 *  @return String representing the current version name.
 */
+ (NSString *)getVersion;
/**
 *  Check that `UnityAds` has been initialized. This might be useful for debugging initialization problems.
 *
 *  @return If `YES`, Unity Ads has been successfully initialized.
 */
+ (BOOL)isInitialized;

@end

@interface UnityServicesListener : NSObject <UnityServicesDelegate>
@end

NS_ASSUME_NONNULL_END
