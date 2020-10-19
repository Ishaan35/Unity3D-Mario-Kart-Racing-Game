#import "UnityAds/UADSPurchasing.h"
#import "UnityAdsPurchasingWrapper.h"

#import "UnityAdsUtilities.h"

static UnityAdsPurchasingDidInitiatePurchasingCommandCallback iapCommandCallback = NULL;
static UnityAdsPurchasingGetProductCatalogCallback iapCatalogCallback = NULL;
static UnityAdsPurchasingGetPurchasingVersionCallback iapVersionCallback = NULL;
static UnityAdsPurchasingInitializeCallback iapInitializeCallback = NULL;

@interface UnityAdsPurchasingWrapperDelegate : NSObject <UADSPurchasingDelegate>
@end

@implementation UnityAdsPurchasingWrapperDelegate
- (void)unityAdsPurchasingGetProductCatalog {
    if(iapCatalogCallback != NULL) {
        iapCatalogCallback();
    }
}

- (void)unityAdsPurchasingGetPurchasingVersion {
    if(iapVersionCallback != NULL) {
        iapVersionCallback();
    }
}

- (void)unityAdsPurchasingInitialize {
    if(iapInitializeCallback != NULL) {
        iapInitializeCallback();
    }
}

- (void)unityAdsPurchasingDidInitiatePurchasingCommand:(NSString *)eventString {
    if(iapCommandCallback != NULL) {
        const char * rawEventString = UnityAdsCopyString([eventString UTF8String]);
        iapCommandCallback(rawEventString);
        free((void *)rawEventString);
    }
}
@end

void InitializeUnityAdsPurchasingWrapper() {
    static id<UADSPurchasingDelegate> delegate = nil;
    if (delegate == nil) {
        delegate = [[UnityAdsPurchasingWrapperDelegate alloc] init];
        [UADSPurchasing initialize:delegate];
    }
}

void UnityAdsSetDidInitiatePurchasingCommandCallback(UnityAdsPurchasingDidInitiatePurchasingCommandCallback callback) {
    iapCommandCallback = callback;
}

void UnityAdsSetGetProductCatalogCallback(UnityAdsPurchasingGetProductCatalogCallback callback) {
    iapCatalogCallback = callback;
}

void UnityAdsSetGetVersionCallback(UnityAdsPurchasingGetPurchasingVersionCallback callback) {
    iapVersionCallback = callback;
}

void UnityAdsSetInitializePurchasingCallback(UnityAdsPurchasingInitializeCallback callback) {
    iapInitializeCallback = callback;
}

void UnityAdsPurchasingDispatchReturnEvent(UnityAdsPurchasingEvent event, const char * payload) {
    if (payload == NULL) {
        payload = "";
    }
    [UADSPurchasing dispatchReturnEvent:event withPayload:[NSString stringWithUTF8String:payload]];
}
