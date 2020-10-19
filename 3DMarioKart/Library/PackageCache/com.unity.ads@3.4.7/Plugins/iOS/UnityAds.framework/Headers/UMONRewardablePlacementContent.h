#import "UMONPlacementContent.h"

NS_ASSUME_NONNULL_BEGIN

@interface UMONRewardablePlacementContent : UMONPlacementContent
@property(nonatomic) BOOL rewarded;
@property(strong, nonatomic) NSString *rewardId;
@end

NS_ASSUME_NONNULL_END
