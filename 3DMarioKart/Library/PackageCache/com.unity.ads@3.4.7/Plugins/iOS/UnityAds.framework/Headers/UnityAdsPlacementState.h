#import <Foundation/Foundation.h>

/**
 *  An enumerate that describes the state of `UnityAds` placements.
 *  @note All placement states, other than `kUnityAdsPlacementStateReady`, indicate that the placement is not currently ready to show ads.
 */
typedef NS_ENUM(NSInteger, UnityAdsPlacementState) {
    /**
     *  A state that indicates that the placement is ready to show an ad. The `show:` selector can be called.
     */
            kUnityAdsPlacementStateReady,
    /**
     *  A state that indicates that no state is information is available.
     *  @warning This state can that UnityAds is not initialized or that the placement is not correctly configured in the Unity Ads admin tool.
     */
            kUnityAdsPlacementStateNotAvailable,
    /**
     *  A state that indicates that the placement is currently disabled. The placement can be enabled in the Unity Ads admin tools.
     */
            kUnityAdsPlacementStateDisabled,
    /**
     *  A state that indicates that the placement is not currently ready, but will be in the future.
     *  @note This state most likely indicates that the ad content is currently caching.
     */
            kUnityAdsPlacementStateWaiting,
    /**
     *  A state that indicates that the placement is properly configured, but there are currently no ads available for the placement.
     */
            kUnityAdsPlacementStateNoFill
};
