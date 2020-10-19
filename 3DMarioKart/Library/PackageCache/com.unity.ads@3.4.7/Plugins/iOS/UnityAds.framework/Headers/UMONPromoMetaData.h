
#import "UMONItem.h"
#import "UPURProduct.h"

NS_ASSUME_NONNULL_BEGIN

@interface UMONPromoMetaDataBuilder : NSObject
@property(strong, nonatomic) NSDate *impressionDate;
@property(nonatomic) NSTimeInterval offerDuration;
@property(strong) UPURProduct *premiumProduct;
@property(strong) NSArray<UMONItem *> *costs;
@property(strong) NSArray<UMONItem *> *payouts;
@property(strong) NSDictionary<NSString *, NSObject *> *userInfo;
@end

@interface UMONPromoMetaData : NSObject
-(instancetype)initWithBuilder:(UMONPromoMetaDataBuilder *)builder;

@property(strong, nonatomic, nullable) NSDate *impressionDate;
@property(nonatomic, readonly) NSTimeInterval offerDuration;
@property(strong, readonly, nullable) UPURProduct *premiumProduct;
@property(strong, readonly) NSArray<UMONItem *> *costs;
@property(strong, readonly) NSArray<UMONItem *> *payouts;
@property(strong, readonly) NSDictionary<NSString *, NSObject *> *userInfo;
-(BOOL)isExpired;
-(BOOL)isPremium;
-(NSTimeInterval)timeRemaining;
-(UMONItem *__nullable)cost;
-(UMONItem *__nullable)payout;
@end

NS_ASSUME_NONNULL_END
