# iOS (Objective-C) API
## Overview
This article documents API classes for the following namespaces:

* [`UnityAds`](#unityads)
* [`UADSBannerView`](#unityadsbanner)
* [`UnityMonetization`](#unitymonetization)
* [`USRVUnityPurchasing`](#usrvunitypurchasing)

For a comprehensive integration guide, click [here](MonetizationBasicIntegrationIos.md).

## UnityAds
Use this namespace to implement basic ad content, such as rewarded or non-rewarded video, interstitial, or banner ads.  

```
#import <UnityAds/UnityAds.h>
```

### Methods
#### initialize
Initializes the Unity Ads service, with a specified Game ID and test mode status. 

```
+ (void)initialize:(NSString *)gameId
    delegate:(nullable id<UnityAdsDelegate>)delegate
    testMode:(BOOL)testMode
    enablePerPlacementLoad:(BOOL)enablePerPlacementLoad;
}  
```

| **Parameter** | **Data type** | **Description** |
| ------------- | ------------- | --------------- |
| `gameId` | NSString | The [Unity Game ID](MonetizationResourcesDashboardGuide.md#project-settings) for your Project, located in the [developer dashboard](https://operate.dashboard.unity3d.com/). |
| `delegate` | `UnityAdsDelegate` | The delegate for [`UnityAdsDelegate`](#unityadsdelegate) callbacks. |
| `testMode` | BOOL | Set to `YES` to enable test mode, and `NO` to disable it. When in test mode, your game will not receive live ads. |
| `enablePerPlacementLoad` | boolean | When set to `true`, this parameter allows you to load content for a specific Placement prior to displaying it. Please review the [load](#load) method documentation before enabling this setting. |

You can check the initialization status with the following function:

```
+ (BOOL)isInitialized;
```

You can check whether the current platform is supported by the SDK with the following function:

```
+ (BOOL)isSupported;
```

#### load
Loads ad content for a specified Placement, allowing for more accurate fill request reporting that is consistent with the standards of most mediation providers. If you initialized the SDK with `enablePerPlacementLoad` enabled, you must call `load` before calling `show`.

**Note**: The `load` API is in closed beta and available upon invite only. If you would like to be considered for the beta, please [contact Unity Ads](mailto:ads-support@unity3d.com).

```
+ (void)load:(NSString *)placementId;
```

#### show
Shows content in the specified Placement, if it is ready.

```
+ (void)show:(UIViewController *)viewController placementId: (NSString *)placementId;
```


#### isReady
Returns whether an ad is ready to be shown for the specified [Placement](MonetizationPlacements.md). 

```
+ (BOOL)isReady:(NSString *)placementId;
```

| **Parameter** | **Data type** | **Description** |
| ------------- | ------------- | --------------- |
| `placementId` | NSString | The [Placement ID](MonetizationPlacements.md#placement-settings), located on the [developer dashboard](https://operate.dashboard.unity3d.com/). |

You can check whether an ad is ready to be shown for the specified [Placement](MonetizationPlacements.md) by calling: 

```
+ (BOOL)isReady: (NSString *)placementId;
```

#### addDelegate
Adds an active listener for [`UnityAdsDelegate`](#unityadsdelegate) callbacks.

```
+ (void)addDelegate:(id<UnityAdsDelegate>)delegate;
```

#### removeDelegate
Allows you to remove an active listener delegate.

```
+ (void)removeDelegate:(id<UnityAdsDelegate>)delegate;
```

#### setDebugMode
Controls the amount of logging output from the SDK. Set to `true` for more robust logging. 

```
+ (void)setDebugMode:(BOOL)enableDebugMode;
```

You can check the current status for debug logging from the SDK by calling: 

```
+ (BOOL)getDebugMode;
```

### Interfaces
#### UnityAdsDelegate
An interface for handling various states of an ad. Implement this listener delegate in your script to define logic for rewarded ads. The interface has the following methods: 

```
@protocol UnityAdsDelegate <NSObject>
- (void)unityAdsReady:(NSString *)placementId;
- (void)unityAdsDidStart:(NSString *)placementId;
- (void)unityAdsDidFinish:(NSString *)placementId
    withFinishState:(UnityAdsFinishState)state;
- (void)unityAdsDidError:(UnityAdsError)error withMessage: (NSString *)message;
@end
```

**Note**: Unity recommends implementing all of these methods in your code, even if you don’t use them all.

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `unityAdsReady` | Handles logic for ad content being ready to display through a specified [Placement](MonetizationPlacements.md). |
| `unityAdsDidStart` | Handles logic for the player triggering an ad to play. |
| `unityAdsDidFinish` | Handles logic for an ad finishing. Define conditional behavior for different finish states by accessing the `FinishState` result from the listener delegate (documented below). For example:<br><br><pre>- (void)unityAdsDidFinish: (NSString *)placementId<br>withFinishState: (UnityAdsFinishState)state {<br>&nbsp;&nbsp;if (state == kUnityAdsFinishStateCompleted) {<br>&nbsp;&nbsp;&nbsp;&nbsp;// Reward the user for watching the ad to completion.<br>&nbsp;&nbsp;} else if (state == kUnityAdsFinishStateSkipped) {<br>&nbsp;&nbsp;&nbsp;&nbsp;// Do not reward the user for skipping the ad.<br>&nbsp;&nbsp;} else if (state == kUnityAdsFinishStateError) {<br>&nbsp;&nbsp;&nbsp;&nbsp;// Log an error message.<br>&nbsp;&nbsp;}<br>}<br></pre> |
| `unityAdsDidError` | Handles logic for ad content failing to display because of an [error](#unityadserror). |

### Enums
#### UnityAdsFinishState
The enumerated states of the end-user’s interaction with the ad. The SDK passes this value to the `unityAdsDidFinish` callback after the ad completes.

```
typedef NS_ENUM(NSInteger, UnityAdsFinishState)
```

| **Value** | **Description** |
| --------- | --------------- |
| `kUnityAdsFinishStateError` | Indicates that the ad failed to complete due to a Unity error. |
| `kUnityAdsFinishStateSkipped` | Indicates that the user skipped the ad. |
| `kUnityAdsFinishStateCompleted` | Indicates that the user successfully finished watching the ad. |

#### UnityAdsPlacementState
The enumerated states of a Unity Ads [Placement](MonetizationPlacements.md).

```
typedef NS_ENUM(NSInteger, UnityAdsPlacementState)
```

| **Value** | **Description** |
| --------- | --------------- |
| `kUnityAdsPlacementStateReady` | The Placement is ready to show ads. |
| `kUnityAdsPlacementStateNotAvailable` | The Placement is not available. |
| `kUnityAdsPlacementStateDisabled` | The Placement was disabled. |
| `kUnityAdsPlacementStateWaiting` | The Placement is waiting to be ready. |
| `kUnityAdsPlacementStateNoFill` | The Placement has no advertisements to show. |

Retrieve the `UnityAdsPlacementState` value with the following function: 

```
+ (UnityAdsPlacementState)getPlacementState:(NSString *)placementId;
```

#### UnityAdsError
The enumerated causes of a Unity Ads error. 

| **Value** | **Description** |
| --------- | --------------- |
| `kUnityAdsErrorNotInitialized = 0` | The Unity Ads service is currently uninitialized. | 
| `kUnityAdsErrorInitializedFailed` | An error occurred in the initialization process. |
| `kUnityAdsErrorInvalidArgument` | Unity Ads initialization failed due to invalid parameters. |
| `kUnityAdsErrorVideoPlayerError` | An error occurred due to the video player failing. |
| `kUnityAdsErrorInitSanityCheckFail` | Initialization of the Unity Ads service failed due to an invalid environment. |
| `kUnityAdsErrorAdBlockerDetected` | An error occurred due to an ad blocker. |
| `kUnityAdsErrorFileIoError` | An error occurred due to inability to read or write a file. |
| `kUnityAdsErrorDeviceIdError` |  An error occurred due to a bad device identifier. |
| `kUnityAdsErrorShowError` | An error occurred when attempting to show an ad. |
| `kUnityAdsErrorInternalError` | An internal Unity Ads service error occurred. |

## UADSBannerView
Use this namespace to [implement banner ads](MonetizationBannerAdsIos.md). Unity Ads version 3.3 and above supports multiple banner instances through a single Placement.

```
@interface UADSBannerView : UIView
```

### Methods
#### initWithPlacementId 
Implements a banner view using a Placement ID and banner size. Call `initWithPlacementId` to initialize the object, then `load` to load ad content. Note that the banner object accesses lifecycle events through its [`UADSBannerViewDelegate` listener](#uadsbannerviewdelegate). 

```
-(instancetype)initWithPlacementId:(NSString *)placementId size:(CGSize)size;
```

| **Parameter** | **Type** | **Description** |
| ------------ | -------- | --------------- |
| `placementId` | NSString | The banner's [Placement ID](MonetizationPlacements.md#placement-settings) (located on the [developer dashboard](https://operate.dashboard.unity3d.com/)). |
| `size` | [`CGSize`](https://developer.apple.com/documentation/coregraphics/cgsize?language=objc) | The size of the banner object. The minimum supported size is 320 pixels by 50 pixels. |

**Note**: Using `UADSBannerView` to call a banner ad attempts to load content once, with no refreshing. If the banner returns no fill, you may destroy it and create a new one to try again. Unity recommends this method for mediated partners.

#### load
The basic method for requesting an ad for the banner.

```
-(void)load;
```

### Delegates
#### UADSBannerViewDelegate
A `UADSBannerView` delegate that grants access to banner lifecycle events.

```
@protocol UADSBannerViewDelegate <NSObject>

@optional
-(void)bannerViewDidLoad:(UADSBannerView *)bannerView; 
-(void)bannerViewDidClick:(UADSBannerView *)bannerView;
- (void)bannerViewDidLeaveApplication:(UADSBannerView *)bannerView;
- (void)bannerViewDidError:(UADSBannerView *)bannerView error:(UADSBannerError *)error;
@end
```

| **Delegate method** | **Description** | 
| -------------------- | --------------- |
| `bannerViewDidLoad` | Called when the Unity banner finishes loading an ad. The view parameter references the banner that should be inserted into the view hierarchy. |
| `bannerViewDidClick` | Called when the Unity banner is clicked. |
| `bannerViewDidLeaveApplication` | Called when the banner links outside the application. |
| `bannerViewDidError` | Called when an error occurs showing the banner. |

### Classes
#### UADSBannerError
An object extending NSError containing an error code and message.

```
@interface UADSBannerError : NSError
- (instancetype)initWithCode:(UADSBannerErrorCode)code userInfo:(nullable NSDictionary<NSErrorUserInfoKey, id> *)dict;
@end
```

#### UADSBannerErrorCode
An enum representing an error encountered during the banner lifecycle.

```
typedef NS_ENUM(NSInteger, UADSBannerErrorCode) {
    UADSBannerErrorCodeUnknown = 0,
    UADSBannerErrorCodeNativeError = 1,
    UADSBannerErrorCodeWebViewError = 2,
    UADSBannerErrorCodeNoFillError = 3
};
```

## UnityAdsBanner (deprecated)
Use this namespace to [implement banner ads](MonetizationBannerAdsIos.md).

**Note**: This API, along with all its methods and classes, has been deprecated in favor of [`BannerView`](#bannerview).

### Methods
#### loadBanner (deprecated)
The basic method for loading banner ad content. You can adjust this function with several parameters, depending on your needs.

| **Method** | **Description** |
| ------ | ----------- |
| `+ (void)loadBanner;` | Loads the banner ad with the default Placement ID. |
| `+ (void)loadBanner:(NSString *)placementId;` | Loads the banner ad with a specific [Placement ID](MonetizationPlacements.md#placement-settings) (located on the [developer dashboard](https://operate.dashboard.unity3d.com/)). |

For example:

```
- (void)loadBanner {
    if ([UnityAds isReady:self.bannerPlacementId]) {
        [UnityAdsBanner loadBanner:self.bannerPlacementId];
    }
}
```

#### setBannerPosition (deprecated)
Sets the position of the banner ad, using the [`UnityAdsBannerPosition`](#unityadsbannerposition) enum.

```
+ (void)setBannerPosition:(UnityAdsBannerPosition)bannerPosition;
```

#### destroy (deprecated)
Destroys the banner ad, removing it from the view hierarchy and hiding it from the player.

```
+ (void)destroy;
```

### Interfaces
#### UnityAdsBannerDelegate (deprecated)
**Note**: This delegate has been deprecated in favor of [`UADSBannerAdViewDelegate`](#uadsbanneradviewdelegate).

Implement this interface to handle various banner states. The listener delegate includes the following methods:

```
@protocol UnityAdsBannerDelegate <NSObject>
- (void)unityAdsBannerDidLoad:(NSString *)placementId view: (UIView *)view;
- (void)unityAdsBannerDidUnload:(NSString *)placementId;
- (void)unityAdsBannerDidShow:(NSString *)placementId;
- (void)unityAdsBannerDidClick:(NSString *)placementId;
- (void)unityAdsBannerDidHide:(NSString *)placementId;
- (void)unityAdsBannerDidError:(NSString *)message;
@end
```

**Note**: Unity recommends implementing all of these methods in your code, even if you don’t use them all.

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `unityAdsBannerDidLoad` | Called when the Unity banner finishes loading an ad. The view parameter references the banner that should be inserted into the view hierarchy. For example:<br><br><pre>- (void)unityAdsBannerDidLoad:(NSString * )placementId view:(UIView *view {<br>&nbsp;&nbsp;// Store the bannerView for later:<br>&nbsp;&nbsp;self.bannerView = view;<br>&nbsp;&nbsp;// Add the banner into your view hierarchy:<br>&nbsp;&nbsp;[self.view addSubview:self.bannerView];<br>}</pre>|
| `unityAdsBannerDidUnload` | Called when ad content is unloaded from the banner, and references to its view should be removed. |
| `unityAdsBannerDidShow` | Called when the Unity banner is shown for the first time and visible to the user. |
| `unityAdsBannerDidClick` | Called when the Unity banner is clicked. |
| `unityAdsBannerDidHide` | Called when the Unity banner is hidden. |
| `unityAdsBannerDideError` | Called when an error occurs showing the banner. |

You can set or retrieves the assigned `UnityAdsBannerDelegate` for Unity Ads to send banner callbacks to, using the following methods:

```
+ (void)setDelegate:(id <UnityAdsBannerDelegate>)delegate;
```

```
+ (nullable id <UnityAdsBannerDelegate>)getDelegate;
```

### Enums
#### UnityAdsBannerPosition (deprecated)
The enumerated positions you can set as an anchor for banner ads.

```
typedef NS_ENUM(NSInteger, UnityAdsBannerPosition) {
    kUnityAdsBannerPositionTopLeft,
    kUnityAdsBannerPositionTopCenter,
    kUnityAdsBannerPositionTopRight,
    kUnityAdsBannerPositionBottomLeft,
    kUnityAdsBannerPositionBottomCenter,
    kUnityAdsBannerPositionBottomRight,
    kUnityAdsBannerPositionCenter,
    kUnityAdsBannerPositionNone
};
```

## UnityMonetization
Use this namespace to [implement ads](MonetizationBasicIntegrationIos.md#integration-for-personalized-placements) for use with [Personalized Placements](MonetizationPersonalizedPlacementsIos.md).

```
#import <UnityAds/UnityMonetization.h>
```

### Methods
#### initialize
Initializes Unity Ads in your game at run time. To avoid errors, initialization should occur early in the game’s run-time lifecycle, preferably at boot. Initialization must occur prior to showing ads.

```
+ (void)initialize:(NSString *)gameId delegate:(nullable id <UnityMonetizationDelegate>)delegate testMode:(BOOL)testMode;
```

| **Parameter** | **Description** |
| ------------- | --------------- |
| `gameID` | The Game ID for your Project [found on the developer dashboard](MonetizationResourcesDashboardGuide.md#game-ids). |
| `delegate` | The listener delegate for [`UnityMonetizationDelegate`](#unitymonetizationdelegate) callbacks. |
| `testMode` | Indicates whether the game is in test mode. When `testMode` is `true`, you will only see test ads. When `testMode` is `false`, you will see live ads. It is important to use test mode prior to launching your game, to avoid being flagged for fraud. |

#### isReady
Checks whether a [`UMONPlacementContent`](#umonplacementcontent) object is ready for the given Placement. Returns `YES` if content is ready, and `NO` if it isn’t.

```
+ (BOOL)isReady:(NSString *)placementId;
```

### Interfaces
#### UnityMonetizationDelegate
An interface for handling various states of [`UMONPlacementContent`](#umonplacementcontent) objects. The interface has the following methods:

```
@protocol UnityMonetizationDelegate <UnityServicesDelegate>
- (void)placementContentReady:(NSString *)placementId
    placementContent:(UMONPlacementContent *)decision;

- (void)placementContentStateDidChange:(NSString *)placementId
    placementContent:(UMONPlacementContent *)placementContent
        previousState:(UnityMonetizationPlacementContentState)previousState
            newState:(UnityMonetizationPlacementContentState)newState;
@end
```


| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `placementContentReady` | Notifies you that a `UMONPlacementContent` object is available for a given Placement and is ready to show. |
| `placementContentStateDidChange` | Notifies you when the readied `UMONPlacementContent` object’s status has changed due to a refresh. | 

Example `UnityMonetizationDelegate` implementation:

```
@interface MyMonetizationDelegate <UnityMonetizationDelegate>
@end

@implementation MyMonetizationDelegate
- (void)placementContentReady:(NSString *)placementId
    placementContent:(UMONPlacementContent *)placementContent {
    if ([placementId isEqualToString:@"video"]) {
        self.interstitial = placementContent;
    } else if ([placementId isEqualToString:@"rewardedVideo"]) {
        self.rewardedVideo = placementContent;
    }
}

- (void)placementContentStateDidChange:(NSString *)placementId
    placementContent:(UMONPlacementContent *)decision
        previousState:(UnityMonetizationPlacementContentState)previousState
            newState:(UnityMonetizationPlacementContentState)newState {
    if (newState != kPlacementContentStateReady) {
        // Disable showing ads because content isn't ready anymore
    }
}
@end
```

#### UMONShowAdDelegate
Implement this delegate to provide the `unityAdsDidFinish` callback method for a video ad’s [`UnityAdsFinishState`](#unityadsfinishstate). The interface has the following methods:  

```
@protocol UMONShowAdDelegate <NSObject>
- (void)unityAdsDidStart:(NSString*)placementId;
- (void)unityAdsDidFinish:(NSString*)placementId withFinishState:(UnityAdsFinishState)finishState;
@end
```

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `unityAdsDidStart` | Handles logic for the player triggering an ad to play. |
| `unityAdsDidFinish` | Implement this method to provide logic for handling whether the ad was completed, skipped, or errored out, depending on the [finish state](#unityadsfinishstate). |

### Classes
#### UMONPlacementContent
A class representing monetization content that your [Placement](MonetizationPlacements.md) can display. 

Associated methods:

| **Method** | **Description** |
| ---------- | --------------- |
| `- (BOOL)isReady` | Returns `YES` if the `UMONPlacementContent` object is ready to display, and `NO` if it isn’t. |
| `- (NSString)getType;` | Returns the type of `UMONPlacementContent` available to display. The following string values are valid:<br><br><ul><li>Refers to video ad content using the [`UMONShowAdPlacementContent`](#umonshowadplacementcontent) class extension.</li><li>Refers to IAP Promo content using the [`UMONPromoAdPlacementContent`](#umonpromoadplacementcontent) class extension.</li><li>`NO_FILL` refers to a lack of `UMONPlacementContent` available for the specified Placement.</li></ul> |

#### UMONRewardablePlacementContent
Extends the `UMONPlacementContent` class with functionality to support rewarded content.

Associated methods:

| **Method** | **Description** |
| ---------- | --------------- |
| `- (BOOL)isRewarded` | Returns `YES` if the `UMONPlacementContent` is rewarded, and `NO` if it isn’t. |

#### UMONShowAdPlacementContent
Extends the `UMONRewardablePlacementContent` class with functionality to support video ad content.

| **Method** | **Description** |
| ---------- | --------------- |
| `- (void)show:(UIViewController *)viewController withDelegate:(id <UMONShowAdDelegate>)delegate;` | Displays the available `UMONShowAdPlacementContent`. Implement a [`UMONShowAdDelegate`](#umonshowaddelegate) delegate to define how this function responds to each ad [finish state](#unityadsfinishstate).

#### UMONPromoAdPlacementContent
Extends the `UMONShowAdPlacementContent` class, providing functionality for [IAP Promo](https://docs.unity3d.com/Manual/IAPPromo.html) content. For more information, see documentation on [Native Promo](MonetizationNativePromoIos.md).

### Enums
#### UnityAdsFinishState
An enum representing the final state of an ad when finished. Use it to handle reward cases.

| **Value** | **Description** |
| ----- | ----------- |
| `kUnityAdsFinishStateCompleted` | The player viewed the ad in its entirety. |
| `kUnityAdsFinishStateSkipped` | The player skipped the ad before it played in its entirety. |
| `kUnityAdsFinishStateError` | The ad failed to load. |

Example `unityAdsDidFinish` implementation:

```
- (void)unityAdsDidFinish:(NSString *)placementId withFinishState:(UnityAdsFinishState)finishState {

    NSLog (@"UnityAds FINISH: %@", placementId);

    if (finishState == kUnityAdsFinishStateCompleted && [placementId isEqualToString: @"rewardedVideo"]) {
        // Reward the player
        ((UMONShowAdPlacementContent *) self.rewardedContent).rewarded = YES;
        NSLog (@"Reward the player");
    }
}
```

## USRVUnityPurchasing
This class manages [purchasing adapters](MonetizationPurchasingIntegrationIos.md), which are interfaces for the SDK to retrieve the information it needs from your custom IAP implementation.

```
@interface USRVUnityPurchasing: NSObject
+ (void)setDelegate:(id<USRVUnityPurchasingDelegate>)delegate;
+ (nullable id<USRVUnityPurchasingDelegate>)getDelegate;
@end
```

### Methods
#### UnityPurchasingTransactionCompletionHandler
Your custom game logic for handling a successful transaction, used in a `USVRUnityPurchasingDelegate`'s `purchaseProduct` method.

#### UnityPurchasingTransactionErrorHandler
Your custom game logic for handling a failed transaction. The function takes a [`UPURTransactionDetails`](#upurtransactiondetails) object. In the example below, a failed transaction calls `UnityPurchasingTransactionErrorHandler`, which returns a [`UPURTransactionError`](#upurtransactionerror) enum.

### Interfaces
#### USVRUnityPurchasingDelegate
An interface for implementing a custom purchasing solution that's compatible with [IAP Promo](https://docs.unity3d.com/Manual/IAPPromo.html). The interface has the following methods:

```
typedef void (^UnityPurchasingLoadProductsCompletionHandler) (NSArray<UPURProduct*>*);
typedef void (^UnityPurchasingTransactionCompletionHandler) (UPURTransactionDetails*);
typedef void (^UnityPurchasingTransactionErrorHandler) (UPURTransactionError, NSException*);

@protocol USRVUnityPurchasingDelegate
- (void)loadProducts:(UnityPurchasingLoadProductsCompletionHandler)completionHandler;
- (void)purchaseProduct:(NSString *)productId 

    completionHandler:(UnityPurchasingTransactionCompletionHandler)completionHandler

        errorHandler:(UnityPurchasingTransactionErrorHandler)errorHandler
            userInfo:(nullable NSDictionary *)extras;
@end
```

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `loadProducts` | The SDK calls this to retrieve the list of available Products. It uses the `UnityPurchasingProductsCompletionHandler` interface, which takes a [`UPURProduct`](#upurproduct) object to register your Products (example below). |
| `purchaseProduct` | The SDK calls this when a user clicks the buy button for a promotional asset. Unity passes the purchased Product’s ID to your in-app purchasing system. The function uses the [`UnityPurchasingTransactionCompletionHandler`](#unitypurchasingtransactioncompletionhandler) and [`UnityPurchasingTransactionErrorHandler`](#unitypurchasingtransactionerrorhandler) functions to handle the purchase’s success or failure according to your implementation. | 

Example of an implemented `loadProducts` function:  

```
- (void)loadProducts:(UnityPurchasingLoadProductsCompletionHandler)completionHandler {

    completionHandler (@[[UPURProduct build:^(UPURProductBuilder *builder){
        builder.productId = @"100BronzeCoins";
        builder.localizedTitle = @"100 Bronze Coins";
        builder.localizedPriceString = @"$1.99";
        builder.localizedPrice = [NSDecimalNumber decimalNumberWithString:@"1.99"];
        builder.isoCurrencyCode = @"USD";
        builder.localizedDescription = @"Awesome Bronze Coins available for a low price!";
        builder.productType = @"Consumable";
    }]]);
}
```

Example `purchaseProduct` implementation:
```
- (void)purchaseProduct:(NSString *)productId completionHandler:(UnityPurchasingTransactionCompletionHandler)completionHandler errorHandler:(UnityPurchasingTransactionErrorHandler)errorHandler userInfo:(nullable NSDictionary *)extras {
    thirdPartyPurchasing.purchase (productId); // Generic developer purchasing function
   
    // If purchase succeeds:
    completionHandler ([UPURTransactionDetails build: ^(UPURTransactionDetailsBuilder *builder) {
        builder.productId = productId;
        builder.transactionId = thirdPartyPurchasing.transactionId;
        builder.currency = @"USD";
        builder.price = [NSDecimalNumber decimalNumberWithString: @"1.99"];
        builder.receipt = @"{\n\"data\": \"{\\\"Store\\\":\\\"fake\\\",\\\"TransactionID\\\":\\\"ce7bb1ca-bd34-4ffb-bdee-83d2784336d8\\\",\\\"Payload\\\":\\\"{ \\\\\\\"this\\\\\\\": \\\\\\\"is a fake receipt\\\\\\\" }\\\"}\"\n}";
    }]);
    
    // If purchase fails:
    errorHandler (kUPURTransactionErrorNetworkError, nil);
}
@end
```

#### UMONNativePromoAdapter
This interface provides access methods for handling user interaction with [promotional assets](MonetizationNativePromoIos.md). Use these methods to pass in your custom assets and define expected behavior. Pass a [`UMONPromoAdPlacementContent`](#umonpromoadplacementcontent) object through the `initWithPromo` function to create a new adapter. For example:

```
`self.nativePromoAdapter = [[UMONNativePromoAdapter alloc] initWithPromo: placementContent];`
```

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `- (void)promoDidShow: (UMONNativePromoShowType)showType;` | The SDK calls this function when the Promo displays. It should include your custom method for displaying promotional assets. You can pass a [`UMONNativePromoShowType`](#umonnativepromoshowtype) enum value to reference the preview type of your Promo asset. | 
| `- (void)promoDidClose;` | The SDK calls this function when the player dismisses the Promo offer. | 
| `- (void)promoDidClick;` | The SDK calls this function when the player clicks the button to purchase the Product. It should initiate the purchase flow. |

Example `UMONNativePromoAdapter` implementation:

```
@interface ViewController: UIViewController <USRVUnityPurchasingDelegate>

- (void)showPromo:(UMONPromoAdPlacementContent *)placementContent {
    self.nativePromoAdapter = [[UMONNativePromoAdapter alloc] initWithPromo:placementContent];
    UMONPromoMetaData *metaData = placementContent.metadata;
    UPURProduct *product = metaData.premiumProduct;
    NSString *price = (product == nil || product.localizedPriceString == nil) ? @"$0.99": product.localizedPriceString;
    
    self.nativePromoView.hidden = NO;
    NSString *title = [NSString stringWithFormat: @"Buy for only %@", price];
    [self.purchaseButton setTitle: title forState: UIControlStateNormal];
    [self.nativePromoAdapter promoDidShow];    
}

// If the player clicked the purchase button:
(IBAction)purchaseButtonTapped:(id)sender {
    [self.nativePromoAdapter promoDidClick];
    [self.nativePromoAdapter promoDidClose];
    self.nativePromoView.hidden = YES;
}

// If the player closed the promotional asset:
- (IBAction)promoCloseButtonTapped:(id)sender {
    self.nativePromoView.hidden = YES;
    [self.nativePromoAdapter promoDidClose];
}
```

### Classes
#### UPURProduct
An IAP product converted into an object that the SDK can use for optimizing monetization.

Set the following fields on the `@interface UPURProductBuilder:NSObject builder`, then call `+ (instancetype)build:(void (^) (UPURProductBuilder *))buildBlock` to construct the final object:

| **Builder method** | **Param type** | **Description** |
| ------------------ | -------------- | --------------- |
| `productId` | `NSString` | An internal reference ID for the Product. |
| `localizedTitle` | `NSString` | A consumer-facing name for the Product, for store UI purposes. |
| `localizedPriceString` | `NSString` | A consumer-facing price string, including the currency sign, for store UI purposes. |
| `localizedPrice` | `NSDecimalNumber` | The internal system value for the Product’s price. |
| `isoCurrencyCode` | `NSString` | The [ISO code](https://en.wikipedia.org/wiki/ISO_4217) for the Product’s localized currency. |
| `localizedDescription` | `NSString` | A consumer-facing Product description, for store UI purposes. |
| `productType` | `NSString` | Unity supports _"Consumable"_, _"Non-consumable"_, and _"Subscription"_ Product types. |

For more details on Product properties, see documentation on [defining Products](https://docs.unity3d.com/Manual/UnityIAPDefiningProducts.html).

#### UPURTransactionDetails
An IAP transaction receipt converted into an object that the SDK can use for optimizing monetization. 

| **Property** | **Type** | **Description** |
| ------------ | -------- | --------------- |
| `productId` | NSString | An internal reference ID for the Product. |
| `transactionId` | NSString | An internal reference ID for the transaction. |
| `receipt` | NSString | The JSON fields from the [`appStoreReceiptURL`](https://developer.apple.com/library/archive/releasenotes/General/ValidateAppStoreReceipt/Chapters/ValidateRemotely.html) detailing the transaction. Encode the receipt as a JSON object containing [Store](#upurstore), TransactionID, and Payload. |
| `price` | NSDecimalNumber | The internal system value for the Product’s price. |
| `currency` | NSString | The [ISO code](https://en.wikipedia.org/wiki/ISO_4217) for the Product’s localized currency. |

For more details, see Apple’s documentation on [In-App Purchase receipt fields](https://developer.apple.com/library/archive/releasenotes/General/ValidateAppStoreReceipt/Chapters/ReceiptFields.html#//apple_ref/doc/uid/TP40010573-CH106-SW1).

### Enums
#### UPURTransactionError
An enum indicating the reason a transaction failed.

```
typedef NS_ENUM (NSInteger, UPURTransactionError) {
    kUPURTransactionErrorNotSupported,
    kUPURTransactionErrorItemUnavailable,
    kUPURTransactionErrorUserCancelled,
    kUPURTransactionErrorNetworkError,
    kUPURTransactionErrorServerError,
    kUPURTransactionErrorUnknownError
};
NSString *NSStringFromUPURTransactionError (UPURTransactionError);
```

#### UPURStore
An enum indicating which store the transaction came from.

```
typedef NS_ENUM (NSInteger, UPURStore) {
    kUPURStoreNotSpecified,
    kUPURStoreGooglePlay,
    kUPURStoreAmazonAppStore,
    kUPURStoreCloudMoolah,
    kUPURStoreSamsungApps,
    kUPURStoreXiaomiMiPay,
    kUPURStoreMacAppStore,
    kUPURStoreAppleAppStore,
    kUPURStoreWinRT,
    kUPURStoreTizenStore,
    kUPURStoreFacebookStore
};
NSString *NSStringFromUPURAppStore(UPURStore);
```

#### UMONNativePromoShowType
The enumerated display types of a [Native Promo](MonetizationNativePromoIos.md) asset.

| **Value** | **Description** |
| --------- | --------------- |
| `"FULL"` | Indicates a full promotional view. | 
| `"PREVIEW"` | Indicates a minimized view that can expand to display the full Promo. |
