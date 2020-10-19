# Basic ads integration for Unity developers
## Overview
This guide covers integration for implementing Unity Ads in your made-with-Unity game.

* If you are an iOS developer using Objective-C, [click here](MonetizationBasicIntegrationIos.md).
* If you are an Android developer using Java, [click here](MonetizationBasicIntegrationAndroid.md). 
* [Click here](../api/UnityEngine.Advertisements.html) for the C# `Advertisements` API reference.

### Guide contents
* [Configuring your Project](#configuring-your-project)
    * [Setting your build targets](#setting-build-targets)
    * [Installing the SDK](#installing-unity-ads)
* [Creating a Placement](#creating-a-placement)
* [Script implementation](#script-implementation)
    * [Initializing the SDK](#initializing-the-sdk)
    * [Interstitial ads](#interstitial-display-ads)
    * [Rewarded video ads](#rewarded-video-ads)
* [Integration for Personalized Placements](#integration-for-personalized-placements)
* [Testing your implementation](#testing)

## Configuring your Project
### Setting build targets
Configure your Project for a supported platform using the [Build Settings window](https://docs.unity3d.com/Manual/BuildSettings.html). Set the platform to **iOS** or **Android**, then click **Switch Platform**.

![Setting build targets in the Unity Editor](images/BuildTargets.png)

### Installing Unity Ads
To ensure the latest version of Unity Ads, download it through the Asset store, or through the Unity Package Manager in the Editor.

**Important**: You must choose either the Asset or the package. Installing both may lead to build errors.

#### Using the Asset package
[Download](https://assetstore.unity.com/packages/add-ons/services/unity-ads-66123) the latest version of Unity Ads from the Asset store. For information on downloading and installing Asset packages, see [Asset packages documentation](https://docs.unity3d.com/Manual/AssetPackages.html). 

#### Using Package Manager
Install the latest version of Unity Ads through the [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui.html), by following these steps:

1. In the Unity Editor, select **Window** > **Package Manager** to open the Package Manager.
2. Select the **Advertisements** package from the list, then select the most recent verified version.
3. Click the **Install** or **Update** button.

## Creating a Placement
[Placements](MonetizationPlacements.md) are triggered events within your game that display monetization content. Manage Placements from the **Operate** tab of the [developer dashboard](https://operate.dashboard.unity3d.com/) by selecting your Project, then selecting **Monetization** > **Placements** from the left navigation bar.

Click the **ADD PLACEMENT** button to bring up the Placement creation prompt. Name your Placement and select its type:

* Select **Non-rewarded** to show basic interstitial ads or promotional content. Non-rewarded Placements allow players to skip the ad after a specified period of time.
* Select **Rewarded** to allow players to opt-in to viewing ads in exchange for incentives. Rewarded Placements do not allow the player to skip the ad.
* Select **Banner** to create a dedicated Banner ad Placement. 

Every Unity Ads-enabled project has a (non-rewarded) ‘`video`’ and (rewarded) ‘`rewardedVideo`’ Placement by default. Feel free to use one of these for your first implementation if they suit your needs, or create your own.

## Script implementation
### Initializing the SDK
To initialize the SDK, you must reference your Project’s Game ID for the appropriate platform. You can locate the ID on the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting the Project, then selecting **Settings** > **Project Settings** from the left navigation bar (see the [Dashboard guide](MonetizationResourcesDashboardGuide.md#project-settings) section on **Project Settings** for details). 

In your game script header, include the [`UnityEngine.Advertisements`](../api/UnityEngine.Advertisements.html) namespace. Initialize the SDK early in the game’s run-time life cycle, preferably at launch, using the [`Advertisement.Initialize`](../api/UnityEngine.Advertisements.Advertisement.html) function. For example:

```
using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAdsScript : MonoBehaviour { 

    string gameId = "1234567";
    bool testMode = true;

    void Start () {
        Advertisement.Initialize (gameId, testMode);
    }
}
```

### Interstitial display ads
To display a full-screen interstitial ad using the [`Advertisements`](../api/UnityEngine.Advertisements.html) API, simply initialize the SDK and use the [`Advertisement.Show`](../api/UnityEngine.Advertisements.Advertisement.html) function. For example:

```
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAdsScript : MonoBehaviour { 

    string gameId = "1234567";
    bool testMode = true;

    void Start () {
        // Initialize the Ads service:
        Advertisement.Initialize (gameId, testMode);
        // Show an ad:
        Advertisement.Show ();
    }
}
```

### Rewarded video ads
Rewarding players for watching ads increases user engagement, resulting in higher revenue. For example, games may reward players with in-game currency, consumables, additional lives, or experience-multipliers. For more information on how to effectively design your rewarded ads, see documentation on [Ads best practices](MonetizationResourcesBestPracticesAds.md).

To reward players for completing a video ad, implement a callback method using the [`ShowResult`](../api/UnityEngine.Advertisements.ShowResult.html) result to check if the user finished the ad and should be rewarded. For example:

```
using UnityEngine;
using UnityEngine.Advertisements;

public class RewardedAdsScript : MonoBehaviour, IUnityAdsListener { 

    string gameId = "1234567";
    string myPlacementId = "rewardedVideo";
    bool testMode = true;

    // Initialize the Ads listener and service:
    void Start () {
        Advertisement.AddListener (this);
        Advertisement.Initialize (gameId, testMode);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished) {
            // Reward the user for watching the ad to completion.
        } else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
        } else if (showResult == ShowResult.Failed) {
            Debug.LogWarning ("The ad did not finish due to an error.");
        }
    }
    
    public void OnUnityAdsReady (string placementId) {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId) {
            Advertisement.Show (myPlacementId);
        }
    }

    public void OnUnityAdsDidError (string message) {
        // Log the error.
    }

    public void OnUnityAdsDidStart (string placementId) {
        // Optional actions to take when the end-users triggers an ad.
    } 
}
```

#### Rewarded video ad buttons
Using a button to allow the player to opt in to watching an ad is a common implementation for rewarded video ads. Use the example code below to create a rewarded ads button. The ads button displays an ad when pressed, as long as ads are available. To configure the button in the Unity Editor:

1. Select **Game Object** > **UI** > **Button** to add a button to your Scene.
2. Select the button you added to your Scene, then add a script component to it using the Inspector (**Add Component** > **New Script**). Name the script `RewardedAdsButton` to match the class name.
3. Open the script and add the following code:

``` 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

[RequireComponent (typeof (Button))]
public class RewardedAdsButton : MonoBehaviour, IUnityAdsListener {
    
    #if UNITY_IOS
    private string gameId = "1486551";
    #elif UNITY_ANDROID
    private string gameId = "1486550";
    #endif
    
    Button myButton;
    public string myPlacementId = "rewardedVideo";

    void Start () {   
        myButton = GetComponent <Button> ();

        // Set interactivity to be dependent on the Placement’s status:
        myButton.interactable = Advertisement.IsReady (myPlacementId); 
        
        // Map the ShowRewardedVideo function to the button’s click listener:
        if (myButton) myButton.onClick.AddListener (ShowRewardedVideo);
        
        // Initialize the Ads listener and service:
        Advertisement.AddListener (this);
        Advertisement.Initialize (gameId, true);
    }

    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo () {
        Advertisement.Show (myPlacementId);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady (string placementId) {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId) {        
            myButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished) {
            // Reward the user for watching the ad to completion.
        } else if (showResult == ShowResult.Skipped) {
            // Do not reward the user for skipping the ad.
        } else if (showResult == ShowResult.Failed) {
            Debug.LogWarning (“The ad did not finish due to an error.”);
        }
    }

    public void OnUnityAdsDidError (string message) {
        // Log the error.
    }

    public void OnUnityAdsDidStart (string placementId) {
        // Optional actions to take when the end-users triggers an ad.
    } 
}
```

## Testing
Prior to publishing your game, enable test mode by following these steps: 

1. From the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/), select your Project.
2. Select **Settings** > **Project Settings** from the left navigation bar.
3. Scroll down to the **Test mode** section and click the edit button on a platform(*Apple App Store* or *Google Play Store*), check **Override client test mode** box, then select the **Force test mode ON** radio button.

In the Unity Editor, click the play button to run your Project and test your ads implementation.

**Note**: You must enable test mode before testing ads integration, to avoid getting flagged for fraud.