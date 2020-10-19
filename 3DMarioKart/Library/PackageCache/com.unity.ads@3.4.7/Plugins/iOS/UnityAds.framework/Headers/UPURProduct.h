
NS_ASSUME_NONNULL_BEGIN

@interface UPURProductBuilder : NSObject
@property(strong, nonatomic) NSString *productId;
@property(strong, nonatomic) NSString *localizedPriceString;
@property(strong, nonatomic) NSString *localizedTitle;
@property(strong, nonatomic) NSString *isoCurrencyCode;
@property(strong, nonatomic) NSDecimalNumber *localizedPrice;
@property(strong, nonatomic) NSString *localizedDescription;
@property(strong, nonatomic) NSString *productType;
@end

@interface UPURProduct : NSObject
@property(strong, nonatomic, readonly) NSString *productId;
@property(strong, nonatomic, readonly) NSString *localizedPriceString;
@property(strong, nonatomic, readonly) NSString *localizedTitle;
@property(strong, nonatomic, readonly) NSString *isoCurrencyCode;
@property(strong, nonatomic, readonly) NSDecimalNumber *localizedPrice;
@property(strong, nonatomic, readonly) NSString *localizedDescription;
@property(strong, nonatomic, readonly) NSString *productType;
+(instancetype)build:(void (^)(UPURProductBuilder *))buildBlock;
@end

NS_ASSUME_NONNULL_END
