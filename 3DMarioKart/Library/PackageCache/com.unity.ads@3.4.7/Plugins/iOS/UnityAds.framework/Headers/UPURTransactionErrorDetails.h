
#import "UPURTransactionError.h"
#import "UPURStore.h"

NS_ASSUME_NONNULL_BEGIN

@interface UPURTransactionErrorDetailsBuilder: NSObject
@property (nonatomic) UPURTransactionError transactionError;
@property (strong, nonatomic) NSString *exceptionMessage;
@property (nonatomic) UPURStore store;
@property (strong, nonatomic) NSString *storeSpecificErrorCode;
@property (strong, nonatomic) NSMutableDictionary *extras;

-(void)putExtra:(NSString *)key value:(NSObject *)value;

@end

@interface UPURTransactionErrorDetails : NSObject

@property (nonatomic, readonly) UPURTransactionError transactionError;
@property (strong, nonatomic, readonly) NSString *exceptionMessage;
@property (nonatomic, readonly) UPURStore store;
@property (strong, nonatomic, readonly) NSString *storeSpecificErrorCode;
@property (strong, nonatomic, readonly) NSDictionary *extras;

+(instancetype)build:(void (^)(UPURTransactionErrorDetailsBuilder *))buildBlock;

@end

NS_ASSUME_NONNULL_END
