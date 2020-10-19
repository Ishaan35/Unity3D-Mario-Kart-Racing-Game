NS_ASSUME_NONNULL_BEGIN

@interface UPURTransactionDetailsBuilder : NSObject
@property(strong, nonatomic) NSString *productId;
@property(strong, nonatomic) NSString *transactionId;
@property(strong, nonatomic) NSString *receipt;
@property(strong, nonatomic) NSDecimalNumber *price;
@property(strong, nonatomic) NSString *currency;
@property(strong, nonatomic) NSMutableDictionary *extras;

-(UPURTransactionDetailsBuilder *)putExtra:(NSString *)key value:(NSObject *)value;
@end

@interface UPURTransactionDetails : NSObject
@property(strong, nonatomic, readonly) NSString *productId;
@property(strong, nonatomic, readonly) NSString *transactionId;
@property(strong, nonatomic, readonly) NSString *receipt;
@property(strong, nonatomic, readonly) NSDecimalNumber *price;
@property(strong, nonatomic, readonly) NSString *currency;
@property(strong, nonatomic, readonly) NSDictionary *extras;

+(instancetype)build:(void (^)(UPURTransactionDetailsBuilder *))buildBlock;
@end

NS_ASSUME_NONNULL_END
