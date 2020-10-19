# AR ads for Android developers
## Overview
This guide covers implementation for AR ads in your Android game.

* If you are a Unity developer using C#, [click here](MonetizationArAdsUnity.md). 
* If you are an iOS developer using Objective-C, [click here](MonetizationArAdsIos.md). 
* [Click here](MonetizationResourcesApiAndroid.md#unityads) for the Java `UnityAds` API reference.

### Guide contents
Implementation consists of four major steps:

1. [Configure your Project for Unity Ads](#configuring-your-game-for-unity-ads).
2. [Configure your Project to access the device's camera permission settings](#setting-camera-permissions).
3. [Create a Placement and associated script to receive and display AR content](#implementation).
4. [Contact the Unity Ads team to enable AR ads for your Project](#enabling-ar-content-through-unity).

## Configuring your Project for Unity Ads
To implement AR ads, you must integrate Unity Ads in your game. To do so, follow the steps in the [basic ads integration guide](MonetizationBasicIntegrationAndroid.md) that detail the following:

* [Creating a Project in the Unity developer dashboard](MonetizationBasicIntegrationAndroid.md#creating-a-project-in-the-unity-developer-dashboard)
* [Importing the Unity Ads framework](MonetizationBasicIntegrationAndroid.md#importing-the-unity-ads-framework)
* [Initializing the SDK](MonetizationBasicIntegrationAndroid.md#initializing-the-sdk)

Once your game is configured for Unity Ads, proceed to configuring the camera permission settings.

## Setting camera permissions
AR ads require access to the device’s camera. You can configure the Unity Ads SDK to handle this permission query.

1. Add the following to your game’s [Android Manifest](https://developer.android.com/guide/topics/manifest/manifest-intro) (see the *AndroidManifest.xml* file included in *UnityAdsARConfigManifest.zip* for reference):

```
// Manifest level
<uses-permission android:name="android.permission.CAMERA" />
// Application level
<meta-data android:name="com.google.ar.core" android:value="optional" />
```

2. Add the ARCore library as a dependency in your game’s [build configuration file](https://developer.android.com/studio/build/) (*build.gradle*):

```
dependencies {
    ...
    implementation 'com.google.ar:core:1.4.0'
}
```

If you get an error when trying to build, ensure that your Project's build configuration file references Google's Maven repository:

```
allprojects {
    repositories {
        google ()
        …
    }
}
```

## Implementation
### Creating a dedicated AR Placement
[Placements](MonetizationPlacements.md) are triggered events within your game that display monetization content. Manage Placements from the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting your Project, then selecting **Monetization** > **Placements** from the left navigation bar. The dashboard does not currently support enabling AR content. Instead, configure a Placement that you will not use for other content, using an easily identifiable Placement ID (for example, `‘arPlacement’`). In the final setup step, you will [contact Unity to enable AR content](#enabling-ar-content-through-unity) for that Placement.

Click the **ADD PLACEMENT** button to bring up the Placement creation prompt. Name your Placement and select the desired type.

### Modifying your Placement script
Follow the steps in the basic integration guide for [initializing the SDK](MonetizationBasicIntegrationAndroid.md#initializing-the-sdk). You must intialize Unity Ads before displaying a banner ad.

In the script for the dedicated AR Placement, implement the following logic:

* Check for available AR content to fill your dedicated AR Placement.
    * If yes, show content from the AR Placement.
    * If no, show ad or Promo content from another Placement.

#### Script examples
The following example uses the [`UnityAds`](MonetizationResourcesApiAndroid.md#unityads) API (recommended) to show an AR ad:

```
String arPlacementId = "arPlacement";
String interstitialPlacementId = "video";

if (UnityAds.isReady (arPlacementId)) {
    UnityAds.show (self, arPlacementId);
} else if (UnityAds.isReady (interstitialPlacementId)) { 
    UnityAds.show (self, interstitialPlacementId);
}
```

Alternatively, the following example uses the [`UnityMonetization`](MonetizationResourcesApiAndroid.md#unitymonetization) API to retrieve a `PlacementContent` object and show an AR ad:

```
protected void showAdIfReady() {
    PlacementContent arPlacement = UnityMonetization.getPlacementContent ("arPlacement");

    if (arPlacement instanceof ShowAdPlacementContent && arPlacement.isReady ()) {
        ((ShowAdPlacementContent) arPlacement).show (thisActivity, new ShowAdListenerAdapter () {
            @Override
            public void onAdStarted (String placementId) {
                Log.d ("UnityAds", "Starting AR ad!");
            }

            @Override
            public void onAdFinished (String placementId, UnityAds.FinishState withState) {
                Log.d ("UnityAds", "Finished an AR ad with state - " + withState);
            }
        });
    } else {
        PlacementContent interstitialPlacement = UnityMonetization.getPlacementContent ("video");


        if (interstitialPlacement instanceof ShowAdPlacementContent && interstitialPlacement.isReady ()) {
            ((ShowAdPlacementContent) interstitialPlacement).show (thisActivity, new ShowAdListenerAdapter () {
                @Override
                public void onAdStarted (String placementId) {
                    Log.d ("UnityAds", "Starting an interstitial ad!");
                }

                @Override
                public void onAdFinished (String placementId, UnityAds.FinishState withState) {
                    Log.d ("UnityAds", "Finished an interstitial ad with state - " + withState);
                }
            });
        }
    }
}
```

Unity recommends reviewing the complete documentation for [basic ads integration](MonetizationBasicIntegrationAndroid.md) to better understand the methods used in these examples, including callback handlers.

**Note**: If your script already decides between multiple Placements, ensure that the dedicated AR Placement handling executes first.

### (Optional) Adjust surface tracking
Depending on the user's behavior while engaged with AR ad content, your game's surface tracking may be off. This can result in objects floating in mid-air or inside surfaces. As such, we recommend adding a custom handler for AR ads.

### Rebuild your game
Your game should now be ready to receive AR content upon enabling it (see below).

## Enabling AR content through Unity
[Contact](mailto:ads-ar-support@unity3d.com) Unity to enable AR content, providing the following information:

* Your __Project ID__
* The __Placement ID__ for your dedicated AR Placement 

## Testing
Please note that because AR content is loaded directly from our servers, AR ads only work in production mode, not test mode. The AR campaigns for integration testing may not reflect the final quality of the AR content. 

For testing purposes, use the following Game ID and Placement ID combinations:

| Platform  | Game ID | Placement ID |
| --------- | ------- | ------------ |
| **iOS**  | `1234567`  | `arPlacement` |
| **Android**  | `7654321`  | `arPlacement` |

Remember to revert the test __Game ID__ and __Placement IDs__ to real production IDs before publishing your game.

## What's next? 
View documentation for [Personalized Placements](MonetizationPersonalizedPlacementsAndroid.md) to let machine learning power your monetization strategy.