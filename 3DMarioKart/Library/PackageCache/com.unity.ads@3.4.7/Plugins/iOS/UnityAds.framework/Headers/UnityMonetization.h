#import "UMONPlacementContent.h"
#import "UMONRewardablePlacementContent.h"
#import "UMONShowAdPlacementContent.h"
#import "UMONPromoAdPlacementContent.h"
#import "UMONNativePromoAdapter.h"
#import "UnityMonetizationDelegate.h"
#import "UnityMonetizationPlacementContentState.h"

NS_ASSUME_NONNULL_BEGIN

__attribute__((deprecated("Please use the UnityAds interface")))
@interface UnityMonetization : NSObject
+(void)setDelegate:(id <UnityMonetizationDelegate>)delegate;
+(nullable id <UnityMonetizationDelegate>)getDelegate;
+(BOOL)isReady:(NSString *)placementId;
+(nullable UMONPlacementContent *)getPlacementContent:(NSString *)placementId;

+ (void)initialize:(NSString *)gameId
          delegate:(nullable id<UnityMonetizationDelegate>)delegate;

+ (void)initialize:(NSString *)gameId
          delegate:(nullable id<UnityMonetizationDelegate>)delegate
          testMode:(BOOL)testMode;
@end

NS_ASSUME_NONNULL_END
