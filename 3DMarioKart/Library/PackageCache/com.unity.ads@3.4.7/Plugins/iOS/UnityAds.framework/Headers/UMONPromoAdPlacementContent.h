#import "UMONShowAdPlacementContent.h"
#import "UMONPromoMetaData.h"

NS_ASSUME_NONNULL_BEGIN

@interface UMONPromoAdPlacementContent : UMONShowAdPlacementContent
-(instancetype)initWithPlacementId:(NSString *)placementId withParams:(NSDictionary *)params;

@property(nonatomic, strong, readonly) UMONPromoMetaData *metadata;
@end

NS_ASSUME_NONNULL_END
