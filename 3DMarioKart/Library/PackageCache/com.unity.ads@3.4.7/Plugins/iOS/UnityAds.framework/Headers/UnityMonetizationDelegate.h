#import "UMONPlacementContent.h"
#import "UnityMonetizationPlacementContentState.h"
#import "UnityServices.h"

NS_ASSUME_NONNULL_BEGIN

@protocol UnityMonetizationDelegate <UnityServicesDelegate>
-(void)placementContentReady:(NSString *)placementId placementContent:(UMONPlacementContent *)decision;
-(void)placementContentStateDidChange:(NSString *)placementId placementContent:(UMONPlacementContent *)placementContent previousState:(UnityMonetizationPlacementContentState)previousState newState:(UnityMonetizationPlacementContentState)newState;
@end

NS_ASSUME_NONNULL_END
