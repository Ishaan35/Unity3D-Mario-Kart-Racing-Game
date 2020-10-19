#import "UPURProduct.h"
#import "UPURTransactionDetails.h"
#import "UPURTransactionErrorDetails.h"

NS_ASSUME_NONNULL_BEGIN

typedef void (^UnityPurchasingLoadProductsCompletionHandler)(NSArray<UPURProduct*>*);
typedef void (^UnityPurchasingTransactionCompletionHandler)(UPURTransactionDetails*);
typedef void (^UnityPurchasingTransactionErrorHandler)(UPURTransactionErrorDetails *);

@protocol USRVUnityPurchasingDelegate <NSObject>
-(void)loadProducts:(UnityPurchasingLoadProductsCompletionHandler)completionHandler;
-(void)purchaseProduct:(NSString *)productId
     completionHandler:(UnityPurchasingTransactionCompletionHandler)completionHandler
          errorHandler:(UnityPurchasingTransactionErrorHandler)errorHandler
              userInfo:(nullable NSDictionary *)extras;
@end

NS_ASSUME_NONNULL_END
