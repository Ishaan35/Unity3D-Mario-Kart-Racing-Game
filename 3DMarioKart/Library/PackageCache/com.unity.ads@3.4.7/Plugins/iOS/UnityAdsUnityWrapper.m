#import "UnityAppController.h"
#import "Unity/UnityInterface.h"

#import "UnityAds/UnityAds.h"
#import <UnityAds/UADSBanner.h>
#import "UnityAds/UADSMetaData.h"

#import "UnityAdsUtilities.h"
#import "UnityAdsPurchasingWrapper.h"
#import <UnityAds/UnityAdsFinishState.h>

typedef void (*UnityAdsReadyCallback)(const char * placementId);
typedef void (*UnityAdsDidErrorCallback)(long rawError, const char * message);
typedef void (*UnityAdsDidStartCallback)(const char * placementId);
typedef void (*UnityAdsDidFinishCallback)(const char * placementId, long rawFinishState);

static UnityAdsReadyCallback readyCallback = NULL;
static UnityAdsDidErrorCallback errorCallback = NULL;
static UnityAdsDidStartCallback startCallback = NULL;
static UnityAdsDidFinishCallback finishCallback = NULL;

@interface UnityAdsUnityWrapperDelegate : NSObject <UnityAdsDelegate>
@end

@implementation UnityAdsUnityWrapperDelegate

- (void)unityAdsReady:(NSString *)placementId {
    if(readyCallback != NULL) {
        const char * rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
        readyCallback(rawPlacementId);
        free((void *)rawPlacementId);
    }
}

- (void)unityAdsDidError:(UnityAdsError)error withMessage:(NSString *)message {
    if(errorCallback != NULL) {
        const char * rawMessage = UnityAdsCopyString([message UTF8String]);
        errorCallback(error, rawMessage);
        free((void *)rawMessage);
    }
}

- (void)unityAdsDidStart:(NSString *)placementId {
    UnityPause(1);
    if(startCallback != NULL) {
        const char * rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
        startCallback(rawPlacementId);
        free((void *)rawPlacementId);
    }
}

- (void)unityAdsDidFinish:(NSString *)placementId withFinishState:(UnityAdsFinishState)state {
    UnityPause(0);
    if(finishCallback != NULL) {
        const char * rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
        finishCallback(rawPlacementId, state);
        free((void *)rawPlacementId);
    }
}

@end

void UnityAdsInitialize(const char * gameId, bool testMode, bool enablePerPlacementLoad) {
    static UnityAdsUnityWrapperDelegate * unityAdsUnityWrapperDelegate = NULL;
    if(unityAdsUnityWrapperDelegate == NULL) {
        unityAdsUnityWrapperDelegate = [[UnityAdsUnityWrapperDelegate alloc] init];
    }
    [UnityAds initialize:[NSString stringWithUTF8String:gameId] delegate:unityAdsUnityWrapperDelegate testMode:testMode enablePerPlacementLoad:enablePerPlacementLoad];
    InitializeUnityAdsPurchasingWrapper();
}

void UnityAdsLoad(const char * placementId) {
    [UnityAds load:[NSString stringWithUTF8String:placementId]];
}

void UnityAdsShow(const char * placementId) {
    if(placementId == NULL) {
        [UnityAds show:UnityGetGLViewController()];
    } else {
        [UnityAds show:UnityGetGLViewController() placementId:[NSString stringWithUTF8String:placementId]];
    }
}

bool UnityAdsGetDebugMode() {
    return [UnityAds getDebugMode];
}

void UnityAdsSetDebugMode(bool debugMode) {
    [UnityAds setDebugMode:debugMode];
}

bool UnityAdsIsSupported() {
    return [UnityAds isSupported];
}

bool UnityAdsIsReady(const char * placementId) {
    if(placementId == NULL) {
        return [UnityAds isReady];
    } else {
        return [UnityAds isReady:[NSString stringWithUTF8String:placementId]];
    }
}

long UnityAdsGetPlacementState(const char * placementId) {
    if(placementId == NULL) {
        return [UnityAds getPlacementState];
    } else {
        return [UnityAds getPlacementState:[NSString stringWithUTF8String:placementId]];
    }
}

const char * UnityAdsGetVersion() {
    return UnityAdsCopyString([[UnityAds getVersion] UTF8String]);
}

bool UnityAdsIsInitialized() {
    return [UnityAds isInitialized];
}

void UnityAdsSetMetaData(const char * category, const char * data) {
    if(category != NULL && data != NULL) {
        UADSMetaData* metaData = [[UADSMetaData alloc] initWithCategory:[NSString stringWithUTF8String:category]];
        NSDictionary* json = [NSJSONSerialization JSONObjectWithData:[[NSString stringWithUTF8String:data] dataUsingEncoding:NSUTF8StringEncoding] options:0 error:nil];
        for(id key in json) {
            [metaData set:key value:[json objectForKey:key]];
        }
        [metaData commit];
    }
}

void UnityAdsSetReadyCallback(UnityAdsReadyCallback callback) {
    readyCallback = callback;
}

void UnityAdsSetDidErrorCallback(UnityAdsDidErrorCallback callback) {
    errorCallback = callback;
}

void UnityAdsSetDidStartCallback(UnityAdsDidStartCallback callback) {
    startCallback = callback;
}

void UnityAdsSetDidFinishCallback(UnityAdsDidFinishCallback callback) {
    finishCallback = callback;
}
