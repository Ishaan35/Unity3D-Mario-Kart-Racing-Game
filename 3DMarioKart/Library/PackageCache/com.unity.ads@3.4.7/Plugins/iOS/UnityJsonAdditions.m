#import "UnityJsonAdditions.h"

NSString *const NSUnityPurchasingTransactionDetailErrorDomain = @"NSUPURTransactionDetailErrorDomain";
NSString *const NSUnityPurchasingTransactionErrorDetailErrorDomain = @"NSUPURTransactionErrorDetailErrorDomain";

UPURTransactionError UPURTransactionErrorFromNSString(NSString *error) {
    if (error) {
        if ([error isEqualToString:@"NotSupported"]) {
            return kUPURTransactionErrorNotSupported;
        } else if ([error isEqualToString:@"Item_Unavailable"]) {
            return kUPURTransactionErrorItemUnavailable;
        } else if ([error isEqualToString:@"UserCancelled"]) {
            return kUPURTransactionErrorUserCancelled;
        } else if ([error isEqualToString:@"NetworkError"]) {
            return kUPURTransactionErrorNetworkError;
        } else if ([error isEqualToString:@"ServerError"]) {
            return kUPURTransactionErrorServerError;
        } else if ([error isEqualToString:@"UnknownError"]) {
            return kUPURTransactionErrorUnknownError;
        } else {
            return kUPURTransactionErrorUnknownError;
        }
    } else {
        return kUPURTransactionErrorUnknownError;
    }
}

UPURStore UPURStoreFromNSString(NSString *store) {
    if (store) {
        if ([store isEqualToString:@"GooglePlay"]) {
            return kUPURStoreGooglePlay;
        } else if ([store isEqualToString:@"AmazonAppStore"]) {
            return kUPURStoreAmazonAppStore;
        } else if ([store isEqualToString:@"CloudMoolah"]) {
            return kUPURStoreCloudMoolah;
        } else if ([store isEqualToString:@"SamsungApps"]) {
            return kUPURStoreSamsungApps;
        } else if ([store isEqualToString:@"XiaomiMiPay"]) {
            return kUPURStoreXiaomiMiPay;
        } else if ([store isEqualToString:@"MacAppStore"]) {
            return kUPURStoreMacAppStore;
        } else if ([store isEqualToString:@"AppleAppStore"]) {
            return kUPURStoreAppleAppStore;
        } else if ([store isEqualToString:@"WinRT"]) {
            return kUPURStoreWinRT;
        } else if ([store isEqualToString:@"TizenStore"]) {
            return kUPURStoreTizenStore;
        } else if ([store isEqualToString:@"FacebookStore"]) {
            return kUPURStoreFacebookStore;
        } else if ([store isEqualToString:@"NotSpecified"]) {
            return kUPURStoreNotSpecified;
        } else {
            return kUPURStoreNotSpecified;
        }
    } else {
        return kUPURStoreNotSpecified;
    }
}

@implementation UPURTransactionDetails (UnityJsonAdditions)
// must check error before using object
+(instancetype)buildWithJson:(NSString *)json error:(NSError **)error {
    id object = [NSJSONSerialization JSONObjectWithData:[json dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingAllowFragments error:error];
    if (*error) {
        NSLog(@"UPURTransactionDetails Unable to serialize from json: %@", [*error description]);
        return nil;
    } else if ([object isKindOfClass:[NSDictionary class]]) {
        return [UPURTransactionDetails build:^(UPURTransactionDetailsBuilder *builder) {
            NSDictionary *dictionary = (NSDictionary *) object;
            builder.productId = [dictionary valueForKey:@"productId"];
            builder.transactionId = [dictionary valueForKey:@"transactionId"];
            builder.receipt = [dictionary valueForKey:@"receipt"];
            builder.price = [dictionary valueForKey:@"price"];
            builder.currency = [dictionary valueForKey:@"currency"];
            id extras = [dictionary valueForKey:@"extras"];
            if (![extras isKindOfClass:[NSNull class]]) {
                builder.extras = extras;
            }
        }];
    } else {
        NSMutableDictionary *info = [NSMutableDictionary dictionary];
        [info setValue:@"UPURTransactionDetails Expected json object to be a NSDictionary but it was not" forKey:@"Reason"];
        *error = [NSError errorWithDomain:NSUnityPurchasingTransactionDetailErrorDomain code:1 userInfo:info];
        return nil;
    }
}
@end

@implementation UPURTransactionErrorDetails (UnityJsonAdditions)
// must check error before using object
+(instancetype)buildWithJson:(NSString *)json error:(NSError **)error {
    id object = [NSJSONSerialization JSONObjectWithData:[json dataUsingEncoding:NSUTF8StringEncoding] options: NSJSONReadingAllowFragments error:error];
    if (*error) {
        NSLog(@"UPURTransactionErrorDetails Unable to serialize from json: %@", [*error description]);
        return nil;
    } else if ([object isKindOfClass:[NSDictionary class]]) {
        return [UPURTransactionErrorDetails build:^(UPURTransactionErrorDetailsBuilder *builder) {
            NSDictionary *dictionary = (NSDictionary *) object;
            builder.transactionError = UPURTransactionErrorFromNSString([dictionary valueForKey:@"transactionError"]);
            builder.exceptionMessage = [dictionary valueForKey:@"exceptionMessage"];
            builder.store = UPURStoreFromNSString([dictionary valueForKey:@"store"]);
            builder.storeSpecificErrorCode = [dictionary valueForKey:@"storeSpecificErrorCode"];
            id extras = [dictionary valueForKey:@"extras"];
            if (![extras isKindOfClass:[NSNull class]]) {
                builder.extras = extras;
            }
        }];
    } else {
        NSMutableDictionary *info = [NSMutableDictionary dictionary];
        [info setValue:@"UPURTransactionErrorDetails Expected json object to be a NSDictionary but it was not" forKey:@"Reason"];
        *error = [NSError errorWithDomain:NSUnityPurchasingTransactionErrorDetailErrorDomain code:1 userInfo:info];
        return nil;
    }
}
@end

@implementation UMONCustomEvent (UnityJsonAdditions)
// must check error before using object
+(instancetype)buildWithJson:(NSString *)json error: (NSError **)error {
    id object = [NSJSONSerialization JSONObjectWithData:[json dataUsingEncoding:NSUTF8StringEncoding] options:NSJSONReadingAllowFragments error:error];
    if (*error) {
        NSLog(@"UMONCustomEvent Unable to serialize from json: %@", [*error description]);
        return nil;
    } else if ([object isKindOfClass:[NSDictionary class]]) {
        return [UMONCustomEvent build:^(UMONCustomEventBuilder *builder) {
            NSDictionary *dictionary = (NSDictionary *) object;
            builder.category = [dictionary valueForKey:@"category"];
            builder.type = [dictionary valueForKey:@"type"];
            builder.userInfo = [dictionary valueForKey:@"userInfo"];
        }];
    } else {
        NSMutableDictionary *info = [NSMutableDictionary dictionary];
        [info setValue:@"UMONCustomEvent Expected json object to be a NSDictionary but it was not" forKey:@"Reason"];
        *error = [NSError errorWithDomain:NSUnityPurchasingTransactionDetailErrorDomain code:1 userInfo:info];
        return nil;
    }
}

@end
