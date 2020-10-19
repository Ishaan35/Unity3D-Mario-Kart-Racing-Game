#import "USRVUnityPurchasingDelegate.h"

NS_ASSUME_NONNULL_BEGIN

@interface USRVUnityPurchasing : NSObject
+(void)setDelegate:(id<USRVUnityPurchasingDelegate>)delegate;
+(nullable id<USRVUnityPurchasingDelegate>)getDelegate;
@end

NS_ASSUME_NONNULL_END
