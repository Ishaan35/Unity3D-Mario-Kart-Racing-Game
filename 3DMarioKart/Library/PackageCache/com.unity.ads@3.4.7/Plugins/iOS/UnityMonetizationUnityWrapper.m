#import <UnityAds/UnityMonetization.h>
#import "UnityAdsUtilities.h"
#import <UnityAds/UnityServices.h>
#import "UnityAdsPurchasingWrapper.h"

/**
 * Callback invoked into C# when a decision state has changed.
 */
typedef void (*UnityMonetizationPlacementContentStateChangedCallback)(const char *placementId, const void *pDecision, int newState, int oldState);
static UnityMonetizationPlacementContentStateChangedCallback unityMonetizationPlacementContentStateChangedCallback = NULL;

/**
 * Callback invoked into C# when a decision is ready.
 */
typedef void (*UnityMonetizationPlacementContentReadyCallback)(const char *placementId, const void *pDecision);
static UnityMonetizationPlacementContentReadyCallback unityMonetizationPlacementContentReadyCallback = NULL;

/**
 * Callback invoked into C# when an error occurred
 */
typedef void (*UnityMonetizationErrorCallback)(long err, const char* message);
static UnityMonetizationErrorCallback unityMonetizationErrorCallback = NULL;

struct UnityMonetizationMonetizationCallbacks {
    UnityMonetizationPlacementContentReadyCallback unityMonetizationDecisionReadyCallback;
    UnityMonetizationPlacementContentStateChangedCallback unityMonetizationDecisionStateChangedCallback;
    UnityMonetizationErrorCallback unityMonetizationErrorCallback;
};

static id<UnityMonetizationDelegate> monetizationDelegate = nil;

@interface UnityMonetizationUnityDecisionDelegate : NSObject<UnityMonetizationDelegate>
@end

@implementation UnityMonetizationUnityDecisionDelegate
-(void)placementContentReady:(NSString *)placementId placementContent:(UMONPlacementContent *)decision {
    if (unityMonetizationPlacementContentReadyCallback != NULL) {
        const char* rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
        // Note, we are bridging instead of retaining as we don't need to track the lifecycle of this
        // object in C#. When the decision is replaced, ready will be called and C# will be notified.
        unityMonetizationPlacementContentReadyCallback(rawPlacementId, CFBridgingRetain(decision));
        free((void*)rawPlacementId);
    }
}
-(void)placementContentStateDidChange:(NSString *)placementId placementContent:(UMONPlacementContent *)placementContent previousState:(UnityMonetizationPlacementContentState)previousState newState:(UnityMonetizationPlacementContentState)newState {
    if (unityMonetizationPlacementContentStateChangedCallback != NULL) {
        const char* rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
        unityMonetizationPlacementContentStateChangedCallback(rawPlacementId, (__bridge void*)placementContent, previousState, newState);
        free((void*)rawPlacementId);
    }
}
-(void)unityServicesDidError:(UnityServicesError)error withMessage:(NSString *)message {
    if (unityMonetizationErrorCallback != NULL) {
        const char* rawError = UnityAdsCopyString([message UTF8String]);
        unityMonetizationErrorCallback(error, rawError);
        free((void*)rawError);
    }
}

@end

void UnityMonetizationSetMonetizationCallbacks(struct UnityMonetizationMonetizationCallbacks *callbacks) {
    unityMonetizationPlacementContentStateChangedCallback = callbacks->unityMonetizationDecisionStateChangedCallback;
    unityMonetizationPlacementContentReadyCallback = callbacks->unityMonetizationDecisionReadyCallback;
    unityMonetizationErrorCallback = callbacks->unityMonetizationErrorCallback;
    if (monetizationDelegate == nil) {
        monetizationDelegate = [[UnityMonetizationUnityDecisionDelegate alloc] init];
    }
}

void UnityMonetizationInitialize(const char *gameId, bool isTestMode) {
    InitializeUnityAdsPurchasingWrapper();
    [UnityMonetization initialize:[NSString stringWithUTF8String:gameId] delegate:monetizationDelegate testMode:isTestMode];
}

bool UnityMonetizationIsReady(const char* placementId) {
    return [UnityMonetization isReady:[NSString stringWithUTF8String:placementId]];
}

bool UnityMonetizationIsSupported() {
    return [UnityServices isSupported];
}
