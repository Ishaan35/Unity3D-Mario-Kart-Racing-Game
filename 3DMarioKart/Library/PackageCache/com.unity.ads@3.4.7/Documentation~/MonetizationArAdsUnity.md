# AR ads for Unity developers
## Overview
This guide covers implementation for AR ads in your made-with-Unity game.

* If you are an iOS developer using Objective-C, [click here](MonetizationArAdsIos.md). 
* If you are an Android developer using Java, [click here](MonetizationArAdsAndroid.md). 
* [Click here](../api/UnityEngine.Advertisements.html) for the Unity (C#) `Advertisements` API reference.

### Guide contents
Implementation consists of four major steps:

1. [Configure your Project for Unity Ads](#configuring-your-project-for-unity-ads).
2. [Configure your Project to access the device's camera permission settings](#setting-camera-permissions).
3. [Create a Placement and associated script to receive and display AR content](#implementation).
4. [Contact the Unity Ads team to enable AR ads for your Project](#enabling-ar-content-through-unity).

## Configuring your Project for Unity Ads
To implement AR ads, you must integrate Unity Ads in your Project. To do so, follow the steps in the [basic ads integration guide](MonetizationBasicIntegrationUnity.md) that detail the following:

* [Setting build targets](MonetizationBasicIntegrationUnity.md#setting-build-targets)
* [Installing Unity Ads](MonetizationBasicIntegrationUnity.md#installing-unity-ads)
* [Initializing the SDK](MonetizationBasicIntegrationUnity.md#initializing-the-sdk)

Once your Project is configured for Unity Ads, proceed to configuring the camera permission settings.

## Setting camera permissions
AR ads require access to the device’s camera. You can configure the Unity Ads SDK to handle this permission query.

### iOS build targets
1. In the Unity Editor, select **Edit** > **Project Settings** > **Player**. 
2. Select the iOS icon in the platform tab menu, and scroll to the **Other Settings** section.
3. Find the **Camera Usage Description** field and enter a description of the game's reason for accessing the device’s camera (for example, _"For AR ads display"_). For more information on this setting, see the [iOS Player Settings](https://docs.unity3d.com/Manual/class-PlayerSettingsiOS.html) documentation. 

### Android build targets
First, download the Google AR core library: 

1. Access the Maven repository [here](https://mvnrepository.com/artifact/com.google.ar/core). 
2. Click the most current version
3. Select the **.aar** button listed under **Files**.

Next, configure your Android Manifest:

1. Place the downloaded _.aar_ file in your Project’s _Assets/Plugins/Android_ directory.
2. Contact Unity requesting a custom [Android Manifest](https://developer.android.com/guide/topics/manifest/manifest-intro) (_AndroidManifest.xml_) with the modifications required for AR ads to work in your Project, and a _project.properties_ file that enables loading the Android Manifest.
3. Unzip the provided _UnityAdsARConfigManifest.zip_ file, then place the resulting folder in your Project’s _Assets/Plugins/Android_ directory. 

**Important**: Google requires end-users to explicitly grant camera permissions. By default, Unity AR ads-enabled games on Android request that permission when the game starts. If you choose to postpone the permissions query, you may do so by changing the `unityplayer.SkipPermissionsDialog` value in the Android Manifest to `true`. However, this setting controls all of your game’s permission queries, not just the camera. While Unity Ads will handle the camera query, you must ensure that any other permission requests (for example, accessing the microphone) occur prior to accessing the relevant component. If one of your Project’s plugins does not support this, you may experience crashes. Unity recommends thoroughly testing your implementation if you enable this option.

## Implementation
### Creating a dedicated AR Placement
[Placements](MonetizationPlacements.md) are triggered events within your game that display monetization content. Manage Placements from the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting your Project, then selecting **Monetization** > **Placements** from the left navigation bar. The dashboard does not currently support enabling AR content. Instead, configure a Placement that you will not use for other content, using an easily identifiable Placement ID (for example, `‘arPlacement’`). In the final setup step, you will [contact Unity to enable AR content](#enabling-ar-content-through-unity) for that Placement.

Click the **ADD PLACEMENT** button to bring up the Placement creation prompt. Name your Placement and select the desired type.

### Modifying your Placement script
Follow the steps in the basic integration guide for [initializing the SDK](MonetizationBasicIntegrationUnity.md#initializing-the-sdk). You must intialize Unity Ads before displaying a banner ad.

In the script for your dedicated AR Placement, implement the following logic:

* Check for available AR content to fill your dedicated AR Placement.
    * If yes, show content from the AR Placement.
    * If no, show ad or Promo content from another Placement.

#### Script examples
The following example uses the [`Advertisements`](../api/UnityEngine.Advertisements.html) API to show an AR ad:

```
if (Advertisement.IsReady ("arPlacement")) {
   ShowOptions so = new ShowOptions ();
   so.resultCallback = MyARAdCallbackHandler;
   Advertisement.Show ("arPlacement", so);
} else if (Advertisement.IsReady ()) {
   ShowOptions so = new ShowOptions ();
   so.resultCallback = MyAdCallbackHandler; 
   // Replace MyAdCallbackHandler with the one you are already using
   Advertisement.Show (so);
}
```

The following example uses the [`Monetization`](../api/UnityEngine.Monetization.html) API to retrieve a `PlacementContent` object and show an AR ad:

```
using UnityEngine;
using UnityEngine.Monetization;

public class ARTest : MonoBehaviour {
    private PlacementContent MyPlacementContent;

    void Start () {
        Monetization.Initialize ("1234567", false);
        Monetization.onPlacementContentReady += PlacementContentReady;
    }

    public void ShowAd () {
        if (MyPlacementContent != null) {
            ShowAdPlacementContent ad = MyPlacementContent as ShowAdPlacementContent;
            ad.Show (myAdCallbackHandler); // See note on callback handlers, below
        }
    }

    public void PlacementContentReady (object sender, PlacementContentReadyEventArgs e) {
        Debug.LogFormat ("PlacementID: {0}, Object: {1}", e.placementId, e.placementContent.GetType ());
        if (e.placementId == "arPlacement") {
            MyPlacementContent = e.placementContent;
        }
    }
}
```

Unity recommends reviewing the complete documentation for [basic ads integration](MonetizationBasicIntegrationUnity.md) to better understand the methods used in these examples, including callback handlers.

**Note**: If your script already decides between multiple Placements, ensure that the dedicated AR Placement handling executes first.

### (Optional) Adjust surface tracking
Depending on the user's behavior while engaged with AR ad content, your game's surface tracking may be off. This can result in objects floating in mid-air or inside surfaces. As such, we recommend adding a custom handler for AR ads. For example:

```
void MyARAdCallbackHandler (ShowResult result) {
   Debug.Log ("AR ad successfully finished with result " + result.ToString ());
   // Implement any custom logic related to returning from an AR ad.
}
```

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
View documentation for [Personalized Placements](MonetizationPersonalizedPlacementsUnity.md) to let machine learning power your monetization strategy.