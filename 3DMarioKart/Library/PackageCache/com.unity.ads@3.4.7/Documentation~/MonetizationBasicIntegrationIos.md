# Basic ads integration for iOS developers
## Overview
This guide covers basic integration for implementing Unity Ads in your iOS game.

* If you are a Unity developer using C#, [click here](MonetizationBasicIntegrationUnity.md). 
* If you are an Android developer using Java, [click here](MonetizationBasicIntegrationAndroid.md). 
* [Click here](MonetizationResourcesApiIos.md#unityads) for the Objective-C `UnityAds` API reference.

**Note**: If you only intend to implement video, interstitial, and banner ads for your monetization strategy, Unity recommends using the [`UnityAds`](MonetizationResourcesApiIos.md#unityads) API for a simpler integration experience. However, if you plan to implement [Personalized Placements](MonetizationPersonalizedPlacementsIos.md), you must integrate Unity Ads with the [`UnityMonetization`](MonetizationResourcesApiIos.md#unitymonetization) API. For more information, please see the [**Integration for Personalized Placements**](#integration-for-personalized-placements) section. 

### Guide contents
* [Creating a Unity Project](#creating-a-project-in-the-unity-developer-dashboard)
* [Creating a Placement](#creating-a-placement)
* [Importing the Unity Ads framework](#importing-the-unity-ads-framework)
* [Script implementation](#script-implementation)
    * [Initializing the SDK](#initializing-the-sdk)
    * [Interstitial ads](#interstitial-display-ads)
    * [Rewarded video ads](#rewarded-video-ads)
* [Integration for Personalized Placements](#integration-for-personalized-placements)
* [Testing your implementation](#testing)

## Creating a Project in the Unity developer dashboard
If you don't have a Unity ID yet, create one [here](https://id.unity.com/). When you have an account, follow these steps to create a Unity Project on the developer dashboard:

1. Log in to the [Developer Dashboard](https://operate.dashboard.unity3d.com/), and navigate to the **Operate** tab. 
2. Select **Projects** from the left navigation bar.
3. Click the **NEW PROJECT** button in the top right corner.

Locate your Project’s __Game ID__ by selecting your Project, then selecting **Monetization** > **Platforms** from the left navigation bar. Copy the **Apple App Store Game ID**, as you'll need it to activate the Unity Ads service in your game.

## Creating a Placement
[Placements](MonetizationPlacements.md) are triggered events within your game that display monetization content. Manage Placements from the **Operate** tab of the [developer dashboard](https://operate.dashboard.unity3d.com/) by selecting your Project, then selecting **Monetization** > **Placements** from the left navigation bar.

Click the **ADD PLACEMENT** button to bring up the Placement creation prompt. Name your Placement and select its type:

* Select **Non-rewarded** to show basic interstitial ads or promotional content. Non-rewarded Placements allow players to skip the ad after a specified period of time.
* Select **Rewarded** to allow players to opt-in to viewing ads in exchange for incentives. Rewarded Placements do not allow the player to skip the ad.
* Select **Banner** to create a dedicated Banner ad Placement. 

Every Unity Ads-enabled project has a (non-rewarded) `'video'` and (rewarded) `'rewardedVideo'` Placement by default. Feel free to use one of these for your first implementation if they suit your needs, or create your own.

## Importing the Unity Ads framework
Download the Unity Ads framework [here](https://github.com/Unity-Technologies/unity-ads-ios/releases). 

1. Drag-and-drop the framework into your Unity Project folder, and copy it. 
2. In your **ViewController** header (**.h**), import Unity Ads and set the Unity Ads delegate:

```
#import <UIKit/UIKit.h>
#import <UnityAds/UnityAds.h>

@interface ViewController : UIViewController<UnityAdsDelegate>
@end
```

## Script implementation
### Initializing the SDK
To initialize the SDK, you must reference your Project’s __Game ID__ for the appropriate platform. You can locate the ID on the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting the Project, then selecting **Settings** > **Project Settings** from the left navigation bar (see the [Dashboard guide](MonetizationResourcesDashboardGuide.md#project-settings) section on **Project Settings** for details).

In your `ViewController` implementation (`.m`), you’ll need to implement the [`UnityAdsDelegate`](MonetizationResourcesApiIos.md#unityadsdelegate) interface that handles ad callbacks, and reference it as a parameter in the `initialize` method. Initialize the SDK early in your game’s run-time life cycle, before you need to show ads. For example:

```
#import "ViewController.h"

@implementation ViewController

// Initialize the SDK:
- (void) viewDidLoad {
    [super viewDidLoad];
    [UnityAds initialize : @"1234567" delegate : self testMode : true];
}

// Implement the ads listener callback methods:
- (void)unityAdsReady:(NSString *)placementId {
    // Perform logic for ads being available to show.
}

- (void)unityAdsDidStart:(NSString *)placementId {
    // Perform logic for a user starting an ad.
}

- (void)unityAdsDidFinish:(NSString *)placementId
withFinishState:(UnityAdsFinishState)state {
    // Perform logic for a user finishing an ad.    
}

- (void)unityAdsDidError:(UnityAdsError)error withMessage:(NSString *)message {
    // Perform logic for a Unity Ads service error.   
}
@end
```

**Note**: You must implement each of the callback methods in the listener interface, even if they are empty functions for now. You will populate them with the appropriate logic where needed in the following sections. For more information on each listener callback method, see documentation on the [`UnityAdsDelegate`](MonetizationResourcesApiIos.md#unityadsdelegate) interface API. 

### Interstitial display ads
To display a full-screen interstitial ad using the [`UnityAds`](MonetizationResourcesApiIos.md#unityads) API, initialize the SDK and use the `show` function. For example:

```
#import "ViewController.h"

@implementation ViewController

- (instancetype)init {
    self = [super init];
    if (self) {
        [UnityAds initialize:@"1234567" delegate:self testMode:YES];
    }
    return self;
}

// Implement a function to display an ad for the specified Placement if available:
- (void)showAd:(NSString *)placementId {
    if ([UnityAds isReady:placementId]) {
        [UnityAds show:self placementId:placementId];
    }
}

// Implement the UnityAdsDelegate methods:

- (void)unityAdsReady:(NSString *)placementId {
    // Implement functionality for an ad being ready to show.
}

- (void)unityAdsDidStart:(NSString *)placementId {
    // Implement functionality for a user starting to watch an ad.
}

- (void)unityAdsDidFinish:(NSString *)placementId
withFinishState:(UnityAdsFinishState)state {
    // Implement functionality for a user finishing an ad.    
}

- (void)unityAdsDidError:(UnityAdsError)error withMessage:(NSString *)message {
    // Implement functionality for a Unity Ads service error occurring.   
}
@end
```

In this example, you can invoke `showAd` from anywhere in your game you wish to show an interstitial ad.

### Rewarded video ads
Rewarding players for watching ads increases user engagement, resulting in higher revenue. For example, games may reward players with in-game currency, consumables, additional lives, or experience-multipliers. For more information on how to effectively design your rewarded ads, see documentation on [Ads best practices](MonetizationResourcesBestPracticesAds.md).

To reward players for completing a video ad, add logic for the `unityAdsDidFinish` callback method to check if the user finished the ad and should be rewarded. For example:

```
#import "ViewController.h"

@implementation ViewController

- (instancetype)init {
    self = [super init];
    if (self) {
        [UnityAds initialize:@"1234567" delegate:self testMode:YES];
    }
    return self;
}

// Implement a function to display an ad for the specified Placement:
- (void)showRewardedAd:(NSString *)placementId {
    // If the Placement is rewarded:
    if ([self.placementId isEqualToString:placementId]) {
        // Show an ad if the Placement has content available:
        if ([UnityAds isReady:placementId]) {
            [UnityAds show:self placementId:placementId];
        }
    }
}

// Implement the UnityAdsDelegate methods, filling out unityAdsDidFinish:

- (void)unityAdsReady:(NSString *)placementId {
    // Implement functionality for an ad being ready to show.
}

- (void)unityAdsDidStart:(NSString *)placementId {
    // Implement functionality for a user starting to watch an ad.
}

- (void)unityAdsDidFinish:(NSString *)placementId
withFinishState:(UnityAdsFinishState)state {
    // Conditional logic dependent on whether the player finished the ad:
    if ([self.placementId isEqualToString:placementId]) {
        if (state == kUnityAdsFinishStateCompleted) {
            // Reward the user for watching the ad to completion:
            [self.delegate giveReward:placementId finishState:state];
        } else if (state == kUnityAdsFinishStateSkipped) {
            // Do not reward the user for skipping the ad.
        } else if (state == kUnityAdsFinishStateError) {
            Debug.LogWarning (“The ad did not finish due to an error.);
        }
    }    
}

- (void)unityAdsDidError:(UnityAdsError)error withMessage:(NSString *)message {
    // Implement functionality for a Unity Ads service error occurring.   
}
@end
```

## Integration for Personalized Placements
Unity’s monetization platform provides you with powerful revenue tools. If your game uses in-app purchases as well as ads, Unity’s machine learning data model can seamlessly blend content types for an optimized monetization strategy. To learn more about how Unity helps you optimize revenue, see documentation on [**Personalized Placements**](MonetizationPersonalizedPlacementsIos.md). 

### Importing the Monetization framework
Download the Unity Ads framework [here](https://github.com/Unity-Technologies/unity-ads-ios/releases). The [`UnityMonetization`](MonetizationResourcesApiIos.md#unitymonetization) API requires SDK 3.0 or later. 

1. Drag-and-drop the framework into your Unity Project folder, and copy it. 
2. In your **ViewController** header (**.h**), import Unity Ads and set the Unity Ads delegate:

```
#import <UIKit/UIKit.h>
#import <UnityAds/UnityMonetization.h>

@interface ViewController : UIViewController<UnityMonetizationDelegate, UMONShowAdDelegate>
@end
```

### Initialization
To initialize the SDK, you must reference your Project’s Game ID for the appropriate platform. You can locate the ID on the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting the Project, then selecting **Settings** > **Project Settings** from the left navigation bar (see the [Dashboard guide](MonetizationResourcesDashboardGuide.md#project-settings) section on **Project Settings** for details).

In your `ViewController` implementation (`.m`), use the [`setDelegate`](MonetizationResourcesApiIos.md#setdelegate) method to set the monetization delegate. Next, call the [`initialize`](MonetizationResourcesApiIos.md#initialize) method to initialize the SDK early in your game’s runtime lifecycle, before you need to show ads. For example:

```
#import "ViewController.h"

@implementation ViewController

- (void) viewDidLoad {
    [super viewDidLoad];
    [UnityMonetization initialize : @"1234567" delegate : self testMode : true];
}
@end
```

### Implementing basic (non-rewarded) ads
PlacementContent is an object representing monetization content that your Placement can display. For more information, see documentation on Content types and Personalized Placements. 

`UMONPlacementContent` is an object representing monetization content that your Placement can display (for more information, see documentation on [content types](MonetizationContentTypes.md) and [Personalized Placements](MonetizationPersonalizedPlacementsIos.md)). Use the [`UnityMonetizationDelegate`](MonetizationResourcesApiIos.md#unitymonetizationdelegate) delegate’s `placementContentReady` function to retrieve content when it’s ready to display, and the show function to display it. For example:

```
@interface ViewController : UIViewController <UnityMonetizationDelegate, UMONShowAdDelegate>

@property (strong) NSString* activePlacementId;

@end

@implementation ViewController

-(void) viewDidLoad {
    [super viewDidLoad];
    self.activePlacementId = @"video”;
    [UnityMonetization initialize: @"1234567" delegate: self testMode: YES];
}

-(void) showInterstitial {
    // Check if PlacementContent is ready:
    if ([self.interstitialVideo ready]) {
        // Show PlacementContent:
        [self.interstitialVideo show: self]
    }
}
@end
```

### Implementing rewarded ads
Rewarding players for watching ads increases user engagement, resulting in higher revenue. For example, games may reward players with in-game currency, consumables, additional lives, or experience-multipliers. For more information on how to effectively design your rewarded ads, see documentation on [ads best practices](MonetizationResourcesBestPracticesAds.md).

To reward players for watching ads, follow the same steps as detailed in the basic implementation section, but show the ad using a reward callback method with custom logic for players completing the ad.

#### Selecting a Placement
You must display rewarded ads through [Rewarded Placements](MonetizationPlacements.md#placement-types). Every Unity Ads-enabled Project also has a `'rewardedVideo'` Placement by default. Feel free to use this for your implementation, or [create your own](MonetizationPlacements.md#creating-new-placements) (but make sure your Placement is configured as Rewarded).

#### Adding a callback method to your script
The `show` function uses a delegate to return the ad’s [`UnityAdsFinishState`](MonetizationResourcesApiIos.md#unityadsfinishstate). This result indicates whether the player finished or skipped the ad. Use this information to write a custom function for how to handle each scenario. For example: 

```
@interface ViewController : UIViewController <UnityMonetizationDelegate, UMONShowAdDelegate>

@property (strong) NSString* interstitialPlacementId;
@property (strong) NSString* rewardedPlacementId;
@property (strong) UMONPlacementContent* interstitialVideo;
@property (strong) UMONPlacementContent* rewardedVideo;
@end

@implementation ViewController

-(void) viewDidLoad {
    [super viewDidLoad];
    self.interstitialPlacementId = @"video”;
    self.rewardedPlacementId = @"rewardedVideo”;
    [UnityMonetization initialize: @"1234567" delegate: self testMode: YES];
}

-(void) showInterstitialVideo {
    if ([self.interstitialVideo ready]) {
        [self.interstitialVideo show: self withDelegate: self]
    }
}

-(void) showRewardedVideo {
    if ([self.rewardedVideo ready]) {
        [self.rewardedVideo show: self withDelegate: self];
    }
}

// Implement the delegate for retrieving PlacementContent:
#pragma mark: UnityMonetizationDelegate

-(void) placementContentReady: (NSString *) placementId placementContent: (UMONPlacementContent *) placementContent {
    // Check and set the available PlacementContent:
    if ([placementId isEqualToString: self.interstitialPlacementId]) {
        self.interstitialVideo = placementContent;
    } else if ([placementId isEqualToString: self.rewardedPlacementId]) {
        self.rewardedVideo = placementContent;
    }
}

-(void) placementContentStateDidChange: (NSString *) placementId placementContent: (UMONPlacementContent *) decision previousState: (UnityMonetizationPlacementContentState) previousState newState: (UnityMonetizationPlacementContentState) newState {
    if (newState != kPlacementContentStateReady) {
        // Disable showing ads because content isn’t ready anymore
    }
}

-(void) unityServicesDidError: (UnityServicesError) error withMessage: (NSString *) message {
    NSLog (@"UnityMonetization ERROR: %ld - %@", (long) error, message);
}

// Implement the delegate for handling the ad’s finishState
#pragma mark: UMONShowAdDelegate

-(void) unityAdsDidStart: (NSString *) placementId {
    // (Optional) Log or perform some action when the ad starts

    NSLog (@"Unity ad started for: %@", placementId);
}

-(void) unityAdsDidFinish: (NSString *) placementId withFinishState: (UnityAdsFinishState) finishState {
    // If the ad played in its entirety, and the Placement is rewarded, perform reward logic:
    if (finishState == kUnityAdsFinishStateCompleted
        && [placementId isEqualToString:self.rewardedPlacementId]) {
        // Reward player for watching the entire video
    }
}
@end
```

#### Rewarded ads button code example 
Rewarded ads usually use a button that prompts players to opt in to watching the ad. You could modify the above example script to include a button, simply by declaring an extra variable and writing a function that calls `showRewardedVideo` when pressing the button:

```
@property (strong) UIButton* rewardedVideoButton;

-(void) onRewardedVideoButtonTap {
    [self.showRewardedVideo];
}
```

## Testing
Prior to publishing your game, enable test mode by following these steps: 

1. From the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/), select your Project.
2. Select **Monetization** > **Platforms** from the left navigation bar.
3. Select the desired platform, then select the **SETTINGS** tab.
4. Scroll down to the **TEST MODE** section and toggle **override client test mode**, then select the **Force test mode ON** radio button.

Run your project and test your ads implementation.

**Note**: You must enable test mode before testing ads integration, to avoid getting flagged for fraud.

## What's next?
Take your implementation to the next level by using Unity's additional monetization features to optimize your revenue. Here are some next steps to explore:

* Incorporate other ad content types.
    * View documentation for [banner ads integration](MonetizationBannerAdsIos.md).
    * View documentation for [AR ads integration](MonetizationArAdsIos.md).
* Incorporate in-app purchases (IAP), then promote them.
    * Use Unity's [purchasing integration](MonetizationPurchasingIntegrationIos.md) feature to integrate your purchasing solution for [IAP Promos](https://docs.unity3d.com/Manual/IAPPromo.html).
* Let machine learning power your monetization strategy.
    * When you have Ads and IAP Promo set up, use [Personalized Placements](MonetizationPersonalizedPlacementsIos.md) to provide revenue lift for your entire game.
* Review our [best practices](MonetizationResourcesBestPracticesAds.md) guide for insight on how to design effective ad mechanics.