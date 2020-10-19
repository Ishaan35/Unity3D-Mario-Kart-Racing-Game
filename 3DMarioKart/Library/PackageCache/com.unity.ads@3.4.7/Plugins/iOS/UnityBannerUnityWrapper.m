#import <UnityAds/UADSBanner.h>
#import "UnityAdsUtilities.h"

typedef void (*UnityAdsBannerShowCallback)(const char* placementId);
typedef void (*UnityAdsBannerHideCallback)(const char* placementId);
typedef void (*UnityAdsBannerClickCallback)(const char* placementId);
typedef void (*UnityAdsBannerUnloadCallback)(const char* placementId);
typedef void (*UnityAdsBannerLoadCallback)(const char* placementId);
typedef void (*UnityAdsBannerErrorCallback)(const char* message);

static UnityAdsBannerShowCallback bannerShowCallback = NULL;
static UnityAdsBannerHideCallback bannerHideCallback = NULL;
static UnityAdsBannerClickCallback bannerClickCallback = NULL;
static UnityAdsBannerErrorCallback bannerErrorCallback = NULL;
static UnityAdsBannerLoadCallback bannerLoadCallback = NULL;
static UnityAdsBannerUnloadCallback bannerUnloadCallback = NULL;

static UIView* s_banner;
static bool s_showAfterLoad;

@interface UnityBannersUnityWrapper : NSObject<UnityAdsBannerDelegate>
@end

@implementation UnityBannersUnityWrapper
- (void)unityAdsBannerDidError:(NSString *)message {
    if (bannerErrorCallback != NULL) {
        const char * rawMessage = UnityAdsCopyString([message UTF8String]);
        bannerErrorCallback(rawMessage);
        free((void *)rawMessage);
    }
}

- (void)unityAdsBannerDidHide:(NSString *)placementId {
    if (bannerHideCallback != NULL) {
        const char * rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
        bannerHideCallback(rawPlacementId);
        free((void *)rawPlacementId);
    }
}

-(void)unityAdsBannerDidClick:(NSString *)placementId {
    if (bannerClickCallback != NULL) {
        const char * rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
        bannerClickCallback(rawPlacementId);
        free((void *)rawPlacementId);
    }
}

- (void)unityAdsBannerDidShow:(NSString *)placementId {
    if (bannerShowCallback != NULL) {
        const char * rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
        bannerShowCallback(rawPlacementId);
        free((void *)rawPlacementId);
    }
}

- (void)unityAdsBannerDidLoad:(NSString *)placementId view:(UIView*)view {
    s_banner = view;
    const char * rawPlacementId = UnityAdsCopyString([placementId UTF8String]);
    if (bannerLoadCallback != NULL) {
        bannerLoadCallback(rawPlacementId);
        free((void *)rawPlacementId);
    }
    if (s_showAfterLoad) {
        s_showAfterLoad = false;
        UIView *container = UnityGetGLViewController().view;
        [container addSubview:s_banner];
        bannerShowCallback(rawPlacementId);
    }
}

- (void)unityAdsBannerDidUnload:(NSString *)placementId {
}
@end

void UnityAdsBannerShow(const char * placementId, bool showAfterLoad) {
    if (s_banner == nil) {
        s_showAfterLoad = showAfterLoad;
        if(placementId == NULL) {
            [UnityAdsBanner loadBanner];
        } else {
            [UnityAdsBanner loadBanner:[NSString stringWithUTF8String:placementId]];
        }
    } else {
        if (s_banner.superview == nil) {
            UIView *container = UnityGetGLViewController().view;
            [container addSubview:s_banner];
            bannerShowCallback(placementId);
        }
    }
}

void UnityAdsBannerHide(bool shouldDestroy) {
    if (shouldDestroy) {
        [UnityAdsBanner destroy];
        s_banner = nil;
    } else {
        if (s_banner != nil && s_banner.superview != nil) {
            [s_banner removeFromSuperview];
        }
    }
}

bool UnityAdsBannerIsLoaded() {
    return s_banner != nil;
}

void UnityAdsBannerSetPosition(int position) {
    [UnityAdsBanner setBannerPosition:(UnityAdsBannerPosition)position];
}

void UnityAdsSetBannerShowCallback(UnityAdsBannerShowCallback callback) {
    bannerShowCallback = callback;
}

void UnityAdsSetBannerHideCallback(UnityAdsBannerHideCallback callback) {
    bannerHideCallback = callback;
}

void UnityAdsSetBannerClickCallback(UnityAdsBannerClickCallback callback) {
    bannerClickCallback = callback;
}

void UnityAdsSetBannerErrorCallback(UnityAdsBannerErrorCallback callback) {
    bannerErrorCallback = callback;
}
void UnityAdsSetBannerUnloadCallback(UnityAdsBannerUnloadCallback callback) {
    bannerUnloadCallback = callback;
}
void UnityAdsSetBannerLoadCallback(UnityAdsBannerLoadCallback callback) {
    bannerLoadCallback = callback;
}

void UnityBannerInitialize() {
    static UnityBannersUnityWrapper* delegate = nil;
    if (delegate == nil) {
        delegate = [[UnityBannersUnityWrapper alloc] init];
    }
    [UnityAdsBanner setDelegate:delegate];
}
