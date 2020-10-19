# Changelog
## [3.4.7] - 2020-06-09
### Unity (Editor, Asset Store, & Packman)
* No visible changes from 3.4.6

## [3.4.6] - 2020-06-04
### Unity (Editor, Asset Store, & Packman)
#### Bug Fixes
[Android] InitializationState Out of Memory Crash
[Android] GooglePlayStore rejection due to unsafe SSL
[Android] Android readFileBytes crash
[iOS] ios ads not respecting mute
[iOS] Crash when calling addDelegate
[Unity] Error building on unsupported platform in 2020.1+
[Unity] Remove UnityEditor.Advertisement.dll

## [3.4.5] - 2020-04-29
### Unity (Editor, Asset Store, & Packman)
* No visible changes from 3.4.4

## [3.4.4] - 2020-03-02
### Unity (Editor, Asset Store, & Packman)
#### Bug Fixes
[Editor] Fix missing reference to UnityEngine.UI

## [3.4.2] - 2020-01-15
### Unity (Editor, Asset Store, & Packman)
#### Bug Fixes
[Editor] No error callback is called when an invalid game id is used in playmode
[Android] Fix onUnityAdsError Exception: No such proxy method
[Android] Fix FatalException from BufferredInputStream.Read() that occurs on some Android devices
[iOS] Fix UnityAdsCopyString and NSStringFromIl2CppString errors when building for debug
[iOS] Fix Banner is unexpectedly scaled when force landscape mode

## [3.4.1] - 2019-12-13
### Unity (Editor, Asset Store, & Packman)
#### Bug Fixes
- [Android] Fixed an issue where callbacks would not dispatch on android devices
- [Editor Only] Fix an issue where callbacks would not fire in the editor after running playmode more than 1 time
- [Editor Only] Fix an issue where the editor canvas would not display on top of all other objects in the scene
- [Editor Only] Fix an issue where the placeholder gameobject was visible in the users scene

## [3.4.0] - 2019-12-09
### Unity (Editor, Asset Store, & Packman)
#### Deprecated Monetization class.
#### Features
UADSDK-231 - Print warning message in Asset Store package to upgrade to packman
UADSDK-232 - Restore TestMode flag from services window in Unity 2020.1+
#### Bug Fixes
ABT-951 - Test ads in editor has buttons that don't stop click event propagation
UADSDK-236 - OnUnityAdsReady only called once per placement when running in the Unity Editor.
ABT-1057 - Google Play crash reports for SDK 3.3.1
### iOS
#### Deprecated Monetization class.
#### Removed example app for Monetization.
#### Deprecation of initialize method with listener as a param in favor of initialize method 
#### Bug Fixes
UADSDK-219 - Fix iOS isWiredHeadsetOn Memory Leak
ABT-1032 - iOS callback unityAdsDidError is not triggered when initialize with invalid gameId 
ABT-1052 - [iOS] App Crash Rate increased after upgraded to SDK 3.3.0
ABT-1061 - IronSource: count duplicated impressions caused by third party
### Android
#### Deprecated Monetization class.
#### Removed example app for Monetization.
#### Deprecation of initialize method with listener as a param in favor of initialize method without a listener.
#### Bug Fixes
ABT-933 - Google / Admob App Crashing in 3.1.0
UADSDK-238 - Listener.sendErrorEvent is broken on Android
UADSDK-244 - Re-Init on Android always blocks for at least 10s
ABT-1057 - Google Play crash reports for SDK 3.3.1
ABT-1061 - IronSource: count duplicated impressions caused by third party

### Documentation updates
#### Monetization
* Updated the [Monetization Stats API](MonetizationResourcesStatistics.md):
    * Documented new API.
    * Added a migration guide from Applifier API.
    * New 408 error code.

#### Advertising

* [Audience Pinpointer](AdvertisingOptimizationAudiencePinpointer.md) is now self-serve on the Acquire dashboard.
* Updated the [server-to-server install tracking](AdvertisingCampaignsInstallTracking.md) guide.

#### Programmatic
* Added the `bAge` field to [contextual data](ProgrammaticOptimizationContextualData.md).

## [3.3.0] - 2019-09-26
### Unity (Editor, Asset Store, & Packman)
#### Fixed
* Fixed an issue where callbacks would not be executed on the main thread
* Fixed an issue where calling RemoveListener in a callback would cause a crash

### iOS
#### Added
* OS 13 update: 
    * Deprecated UI webview. Due to Apple's changes, Unity Ads no longer supports iOS 7 and 8. 
* Banner optimization:
	* New banner API, featuring the [`UADSBannerView`](../manual/MonetizationResourcesApiIos.html#uadsbannerview) class.
	* The new API supports multiple banners in a single Placement, with flexible positioning.

#### Fixed
* iOS 13 AppSheet crash fix

### Android
#### Added
* Banner optimization:
	* New banner API, featuring the [`BannerView`](../manual/MonetizationResourcesApiAndroid.html#bannerview) class.
	* The new API supports multiple banners in a single Placement, with flexible positioning.

#### Fixed
* WebView onRenderProcessGone crash fix

### Documentation updates
#### Monetization
* Added a FAQ section for [Authorized Sellers for Apps](../Manual/MonetizationResourcesFaq.html#authorized-sellers-for-apps-faqs) (`app-ads.txt`), which is now supported.

#### Advertising
* Added a section on [source bidding](../Manual/AdvertisingCampaignsConfiguration.html#source-bidding).
* Added a section on [app targeting](../Manual/AdvertisingCampaignsConfiguration.html#app-targeting).	
* Removed legacy dashboard and API guides, which are no longer supported.

#### Programmatic
* Added Open Measurement (OM) support fields, including:
    * [`source.omidpn`](../manual/ProgrammaticBidRequests.html#source-objects)
    * [`source.omidpv`](../manual/ProgrammaticBidRequests.html#source-objects)
    * [`imp.video.api`](../manual/ProgrammaticBidRequests.html#video-objects)
    * [`bid.api`](../manual/ProgrammaticBidResponses.html#bid-objects)
* Added [`app.publisher`](../manual/ProgrammaticBidRequests.html#app-objects) field.
* Added the [`bAge`](../manual/ProgrammaticOptimizationContextualData.html) (blocked age rating) field.

#### Legal
* Updated [GDPR compliance](../manual/LegalGdpr.html) to reflect Unity's opt-in approach to consent.

## [3.2.0] - 2019-07-22
### Unity (Editor, Asset Store, & Packman)
#### Added
* Added OMID viewability integration. Unity is now [IAB certified with VAST viewability](https://iabtechlab.com/blog/vast-4-1-open-measurement-the-long-awaited-video-verification-solution/).

#### Fixed
* In cases where you've installed both the package manager and Asset store versions of Unity Ads, the SDK now surfaces an error notifying you to remove one instance.
* Fixed an Android java proxy usage issue for Unity versions below 2017. This fixes a multiple listeners crash. 

### iOS
#### Added
* Added OMID viewability integration. Unity is now [IAB certified with VAST viewability](https://iabtechlab.com/blog/vast-4-1-open-measurement-the-long-awaited-video-verification-solution/). 

### Android
#### Added
* Added OMID viewability integration. Unity is now [IAB certified with VAST viewability](https://iabtechlab.com/blog/vast-4-1-open-measurement-the-long-awaited-video-verification-solution/). 

## [3.1.1] - 2019-05-16
#### Added
* Updated the Android and iOS binaries to 3.1.0.
* Support for multiple listeners.
* `ASWebAuthenticationSession` support.

#### Fixed
* Banner memory leak.
* `GetDeviceId` on Android SDK versions below 23.
* Volume change event not properly captured on iOS.
* `USRVStorage` JSON exception caught and handled.
* Analytics `onLevelUp` taking a string instead of an integer.
* Crash prevented in the `AdUnitActivity.onPause` event.
* Playstation and Xbox no longer throw errors attempting to access `UnityAdsSettings` when building a Project that includes ads on other platforms.
* Test mode resources folder moved to Editor-only scope.

## [3.0.3] - 2019-03-15
#### Added
* Updated the Android and iOS binaries.

#### Fixed
* https://fogbugz.unity3d.com/f/cases/1115398/
* Uncaught exception for purchasing integration on iOS.

## [3.0.2] - 2019-02-26
#### Added
* Updated the Android and iOS binaries.

#### Fixed
* https://fogbugz.unity3d.com/f/cases/1127423/
* https://fogbugz.unity3d.com/f/cases/1127770/

## [3.0.1] - 2019-01-25
#### Added
* Integrated the Ads 3.0.1 SDK.

## [2.3.2] - 2018-11-21
#### Added
* Integrated the Ads 2.3.0 SDK with Unity 2019.X.

#### Fixed
* https://fogbugz.unity3d.com/f/cases/1107128/
* https://fogbugz.unity3d.com/f/cases/1108663/

## [2.3.1] - 2018-11-15
#### Added
* Updated to Ads 2.3.0 SDK.
* Multithreaded Request API.
* `SendEvent` API for Ads and IAP SDK communication.
* New Unity integration.

## [2.2.1] - 2017-04-23
#### Fixed
* Fixed issues for iOS and Android.

## [2.2.0] - 2017-03-22
#### Added
* IAP Promotion support (iOS, Android).

#### Fixed
* Several rare crashes (iOS).

#### Changed
* Improved cache handling (iOS, Android).
* Increased flexibility showing different ad formats (iOS, Android).
