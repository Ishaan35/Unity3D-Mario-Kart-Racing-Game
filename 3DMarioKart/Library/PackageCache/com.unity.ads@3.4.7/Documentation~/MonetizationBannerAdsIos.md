# Banner ads for iOS developers
## Overview
This guide covers implementation for banner ads in your iOS game.

* If you are a Unity developer using C#, [click here](MonetizationBannerAdsUnity.md). 
* If you are an Android developer using Java, [click here](MonetizationBannerAdsAndroid.md). 
* [Click here](MonetizationResourcesApiIos.md#uadsbannerview) for the Objective-C `UADSBannerView` API reference.

## Configuring your game for Unity Ads
To implement banner ads, you must integrate Unity Ads in your Project. To do so, follow the steps in the [basic ads integration guide](MonetizationBasicIntegrationIos.md) that detail the following:

* [Creating a Project in the Unity developer dashboard](MonetizationBasicIntegrationIos.md#creating-a-project-in-the-unity-developer-dashboard)
* [Importing the Unity Ads framework](MonetizationBasicIntegrationIos.md#importing-the-unity-ads-framework)

Once your Project is configured for Unity Ads, proceed to creating a banner Placement.

## Creating a banner Placement
[Placements](MonetizationPlacements.md) are triggered events within your game that display monetization content. Manage Placements from the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting your Project, then selecting **Monetization** > **Placements** from the left navigation bar.

Click the **ADD PLACEMENT** button to bring up the Placement creation prompt. Name your Placement and select the **Banner** type.

## Script implementation
Follow the steps in the basic integration guide for [initializing the SDK](MonetizationBasicIntegrationIos.md#initializing-the-sdk). You must intialize Unity Ads before displaying a banner ad.

Add your banner code in the `ViewController` implementation (`.m`). The following script sample is an example implementation for displaying two banner ads on the screen. For more information on the classes referenced, see the [`UADSBannerView`](MonetizationResourcesApiIos.md#uadsbannerview) API section.

```
@interface ViewController () <UADSBannerViewDelegate>
​
// This is the Placement that will display banner ads:
@property (strong) NSString* bannerPlacementId;
// This banner view object will be placed at the top of the screen:
@property (strong, nonatomic) UADSBannerView *topBannerView;
// This banner view object will be placed at the bottom of the screen:
@property (strong, nonatomic) UADSBannerView *bottomBannerView;
​
@end
​
@implementation ViewController
​
- (void)viewDidLoad {
    [super viewDidLoad];
    self.bannerPlacementId = @"banner";
    [UnityAds initialize: @"1234567" delegate: self testMode: YES];
}
​
// Example method for creating and loading the top banner view object: 
- (void)loadTopBanner{
    // Instantiate a banner view object with Placement ID and size:
    self.topBannerView = [[UADSBannerView alloc] initWithPlacementId: _bannerPlacementId size: CGSizeMake(320, 50)];
    // Set the banner delegate for event callbacks:
    self.topBannerView.delegate = self;
    // Add the banner view object to the view hierarchy:
    [self addBannerViewToTopView:self.topBannerView];
    // Load ad content to the banner view object:
    [_topBannerView load];
}
​
// Example method for creating and loading the bottom banner view object: 
- (void)loadBottomBanner{
    self.bottomBannerView = [[UADSBannerView alloc] initWithPlacementId: _bannerPlacementId size: CGSizeMake(320, 50)];
    self.bottomBannerView.delegate = self;
    [self addBannerViewToTopView:self.bottomBannerView];
    [_bottomBannerView load];
}
​
// Example method for discarding the top banner view object (e.g. if there's no fill):
- (void)unLoadTopBanner{
    // Remove the banner view object from the view hierarchy:
    [self.topBannerView removeFromSuperview];
    // Set it to nil:
    _topBannerView = nil;
}
​
// Example method for discarding the bottom banner view object:
- (void)unLoadBottomBanner{
    [self.bottomBannerView removeFromSuperview];
    _bottomBannerView = nil;
}
​
​// Example method for placing the top banner view object:
- (void)addBannerViewToTopView:(UIView *)bannerView {
    bannerView.translatesAutoresizingMaskIntoConstraints = NO;
    [self.view addSubview:bannerView];
    [self.view addConstraints:@[
                               [NSLayoutConstraint constraintWithItem:bannerView
                                                            attribute:NSLayoutAttributeTop
                                                            relatedBy:NSLayoutRelationEqual
                                                               toItem:self.topLayoutGuide
                                                            attribute:NSLayoutAttributeBottom
                                                           multiplier:1
                                                             constant:0],
                               [NSLayoutConstraint constraintWithItem:bannerView
                                                            attribute:NSLayoutAttributeCenterX
                                                         r   elatedBy:NSLayoutRelationEqual
                                                               toItem:self.view
                                                            attribute:NSLayoutAttributeCenterX
                                                           multiplier:1
                                                             constant:0]
                               ]];
}

​// Example method for placing the bottom banner view object:
- (void)addBannerViewToBottomView: (UIView *)bannerView {
    bannerView.translatesAutoresizingMaskIntoConstraints = NO;
    [self.view addSubview:bannerView];
    [self.view addConstraints:@[
                               [NSLayoutConstraint constraintWithItem:bannerView
                                                            attribute:NSLayoutAttributeBottom
                                                            relatedBy:NSLayoutRelationEqual
                                                               toItem:self.bottomLayoutGuide
                                                            attribute:NSLayoutAttributeTop
                                                           multiplier:1
                                                             constant:0],
                               [NSLayoutConstraint constraintWithItem:bannerView
                                                            attribute:NSLayoutAttributeCenterX
                                                            relatedBy:NSLayoutRelationEqual
                                                               toItem:self.view
                                                            attribute:NSLayoutAttributeCenterX
                                                           multiplier:1
                                                             constant:0]
                               ]];
}

​// Implement the delegate methods:
#pragma mark : UADSBannerViewDelegate

- (void)bannerViewDidLoad:(UADSBannerView *)bannerView {
    // Called when the banner view object finishes loading an ad.
    NSLog(@"Banner loaded for Placement: %@", bannerView.placementId);
}

- (void)bannerViewDidClick:(UADSBannerView *)bannerView {
    // Called when the banner is clicked.
    NSLog(@"Banner was clicked for Placement: %@", bannerView.placementId);
}

- (void)bannerViewDidLeaveApplication:(UADSBannerView *)bannerView {
    // Called when the banner links out of the application.
}


- (void)bannerViewDidError:(UADSBannerView *)bannerView error:(UADSBannerError *)error{
    // Called when an error occurs showing the banner view object.
    NSLog(@"Banner encountered an error for Placement: %@ with error message %@", bannerView.placementId, [error localizedDescription]);
    // Note that the UADSBannerError can indicate no fill (see API documentation).
}
@end
```

### Banner position
You can place the banner view object into your view hierarchy, just like you would any other view. This allows you to customize the position of each banner instance, or display multiple banners.

## What's next? 
View documentation for [AR ads integration](MonetizationArAdsIos.md) to offer players a fully immersive and interactive experience by incorporating digital content directly into their physical world, or [return](Monetization.md) to the monetization hub.
