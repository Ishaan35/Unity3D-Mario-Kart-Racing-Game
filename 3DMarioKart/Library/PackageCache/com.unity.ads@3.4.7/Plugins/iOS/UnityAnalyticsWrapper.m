#import <UnityAds/UANAApiAnalytics.h>
#import "UnityAdsUtilities.h"

typedef void (*UANAEngineTriggerAddExtras)(const char *payload);

static UANAEngineTriggerAddExtras triggerAddExtras = NULL;

void UANAEngineDelegateSetTriggerAddExtras(UANAEngineTriggerAddExtras trigger) {
    triggerAddExtras = trigger;
}

@interface UANAEngineWrapper : NSObject <UANAEngineDelegate>
@end

@implementation UANAEngineWrapper
- (void)addExtras:(NSString *)extras {
    if (triggerAddExtras) {
        const char * rawExtrasString = UnityAdsCopyString([extras UTF8String]);
        triggerAddExtras(rawExtrasString);
        free((void *)rawExtrasString);
    }
}
@end

void InitializeUANAEngineWrapper() {
    static id<UANAEngineDelegate> delegate = nil;
    if (delegate == nil) {
        delegate = [[UANAEngineWrapper alloc] init];
        [UANAApiAnalytics setAnalyticsDelegate:delegate];
    }
}
