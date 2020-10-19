# Banner ads for Unity developers
## Overview
This guide covers implementation for banner ads in your made-with-Unity game.

* If you are an iOS developer using Objective-C, [click here](MonetizationBannerAdsIos.md). 
* If you are an Android developer using Java, [click here](MonetizationBannerAdsAndroid.md). 
* [Click here](../api/UnityEngine.Advertisements.html) for the Unity (C#) `Advertisements` API reference.

## Configuring your game for Unity Ads
To implement banner ads, you must integrate Unity Ads in your Project. To do so, follow the steps in the [basic ads integration guide](MonetizationBasicIntegrationUnity.md) that detail the following:

* [Setting build targets](MonetizationBasicIntegrationUnity.md#setting-build-targets)
* [Installing Unity Ads](MonetizationBasicIntegrationUnity.md#installing-unity-ads)
* [Initializing the SDK](MonetizationBasicIntegrationUnity.md#initializing-the-sdk)

Once your Project is configured for Unity Ads, proceed to creating a banner Placement.

## Creating a banner Placement
[Placements](MonetizationPlacements.md) are triggered events within your game that display monetization content. Manage Placements from the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting your Project, then selecting **Monetization** > **Placements** from the left navigation bar.

Click the **ADD PLACEMENT** button to bring up the Placement creation prompt. Name your Placement and select the **Banner** type.

## Script implementation
Follow the steps in the basic integration guide for [initializing the SDK](MonetizationBasicIntegrationUnity.md#initializing-the-sdk). You must intialize Unity Ads before displaying a banner ad.

Include the [`UnityEngine.Advertisements`](../api/UnityEngine.Advertisements.html) namespace in your Placement script header, as it contains the `Banner` class. Use [`Banner.Show`](../api/UnityEngine.Advertisements.Banner.Show.html) to display a banner ad. For example:

```
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAdScript : MonoBehaviour {

    public string gameId = "1234567";
    public string placementId = "bannerPlacement";
    public bool testMode = true;

    void Start () {
        // Initialize the SDK if you haven't already done so:
        Advertisement.Initialize (gameId, testMode);
        StartCoroutine (ShowBannerWhenReady ());
    }

    IEnumerator ShowBannerWhenReady () {
        while (!Advertisement.IsReady (placementId)) {
            yield return new WaitForSeconds (0.5f);
        }
        Advertisement.Banner.Show (placementId);
    }
}
```

**Important**: The `Banner` class is part of the [`UnityEngine.Advertisements`](../api/UnityEngine.Advertisements.html) API. When using banner ads in conjunction with the [`Monetization` API](../api/UnityEngine.Monetization), you must initialize the `Monetization` API before accessing any classes or members of the `Advertisements` API. Accessing the `Advertisements` API prior to initializing the `Monetization` API will cause content retrieval to fail.

### Specifying banner position
By default, banner ads display anchored on the bottom-center of the screen, supporting 320 x 50 or 728 x 90 pixel resolution. To specify the banner achor, use the [`Banner.SetPosition`](../api/UnityEngine.Advertisements.Banner.SetPosition.html) method. For example:

```
Advertisement.Banner.SetPosition (BannerPosition.TOP_CENTER);
```

## What's next? 
View documentation for [AR ads integration](MonetizationArAdsUnity.md) to offer players a fully immersive and interactive experience by incorporating digital content directly into their physical world, or [return](Monetization.md) to the monetization hub.