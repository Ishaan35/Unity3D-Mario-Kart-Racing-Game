#import "UnityMonetizationPlacementContentState.h"
#import "UMONCustomEvent.h"

NS_ASSUME_NONNULL_BEGIN

@interface UMONPlacementContent : NSObject
-(instancetype)initWithPlacementId:(NSString *)placementId withParams:(NSDictionary *)params;

@property(nonatomic, readonly, getter=isReady) BOOL ready;
@property(nonatomic, readonly) NSString *type;
@property(retain, nonatomic, readonly) NSString *placementId;
@property(nonatomic) UnityMonetizationPlacementContentState state;
@property(nonatomic) NSDictionary *userInfo;
-(void)sendCustomEvent:(UMONCustomEvent*)customEvent;
-(void)sendCustomEvent:(NSString*)type withUserInfo:(NSDictionary<NSString*, NSObject*>* __nullable)userInfo;
-(void)sendCustomEventWithType:(NSString*)type;
-(NSString*)defaultEventCategory;
@end

NS_ASSUME_NONNULL_END
