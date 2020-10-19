#import <UnityAds/UnityMonetization.h>
#import "UnityJsonAdditions.h"

const void* UnityMonetizationCreateNativePromoAdapter(const void* pPlacementContent) {
    if (pPlacementContent) {
        UMONPromoAdPlacementContent* placementContent = (__bridge UMONPromoAdPlacementContent*)pPlacementContent;
        UMONNativePromoAdapter* adapter = [[UMONNativePromoAdapter alloc] initWithPromo:placementContent];
        return CFBridgingRetain(adapter);
    }
    return NULL;
}

void UnityMonetizationReleaseNativePromoAdapter(const void* pPlacementContent) {
    CFBridgingRelease(pPlacementContent);
}

void UnityMonetizationNativePromoAdapterOnShown(const void* pNativePromoAdapter, int showType) {
    if (pNativePromoAdapter) {
        UMONNativePromoAdapter* adapter = (__bridge UMONNativePromoAdapter*)pNativePromoAdapter;
        [adapter promoDidShow:(UMONNativePromoShowType)showType];
    }
}

void UnityMonetizationNativePromoAdapterOnClicked(const void* pNativePromoAdapter) {
    if (pNativePromoAdapter) {
        UMONNativePromoAdapter* adapter = (__bridge UMONNativePromoAdapter*)pNativePromoAdapter;
        [adapter promoDidClick];
    }
}

void UnityMonetizationNativePromoAdapterOnClosed(const void* pNativePromoAdapter) {
    if (pNativePromoAdapter) {
        UMONNativePromoAdapter* adapter = (__bridge UMONNativePromoAdapter*)pNativePromoAdapter;
        [adapter promoDidClose];
    }
}
