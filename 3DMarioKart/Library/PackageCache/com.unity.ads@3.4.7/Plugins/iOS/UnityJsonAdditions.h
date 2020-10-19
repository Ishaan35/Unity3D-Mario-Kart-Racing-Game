#import <UnityAds/UPURTransactionDetails.h>
#import <UnityAds/UPURTransactionErrorDetails.h>
#import <UnityAds/UMONCustomEvent.h>

NS_ASSUME_NONNULL_BEGIN

@interface UPURTransactionDetails (UnityJsonAdditions)
+(nullable instancetype)buildWithJson:(NSString *)json error:(NSError **)error;
@end

@interface UPURTransactionErrorDetails (UnityJsonAdditions)
+(nullable instancetype)buildWithJson:(NSString *)json error:(NSError **)error; 
@end

@interface UMONCustomEvent (UnityJsonAdditions)
+(nullable instancetype)buildWithJson:(NSString *)json error: (NSError **)error;
@end

NS_ASSUME_NONNULL_END
