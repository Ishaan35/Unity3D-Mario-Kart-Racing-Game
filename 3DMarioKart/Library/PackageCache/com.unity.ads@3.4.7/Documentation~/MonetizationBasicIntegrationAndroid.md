# Basic ads integration for Android developers
## Overview
This guide covers basic integration for implementing Unity Ads in your Android game.

* If you are a Unity developer using C#, [click here](MonetizationBasicIntegrationUnity.md). 
* If you are an iOS developer using Objective-C, [click here](MonetizationBasicIntegrationIos.md). 
* [Click here](MonetizationResourcesApiAndroid.md#unityads) for the Java `UnityAds` API reference.

**Note**: If you only intend to implement video, interstitial, and banner ads for your monetization strategy, Unity recommends using the [`UnityAds`](MonetizationResourcesApiAndroid.md#unityads) API for a simpler integration experience. However, if you plan to implement [Personalized Placements](MonetizationPersonalizedPlacementsAndroid.md), you must integrate Unity Ads with the [`UnityMonetization`](MonetizationResourcesApiAndroid.md#unitymonetization) API. For more information, please see the [**Integration for Personalized Placements**](#integration-for-personalized-placements) section. 

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
Download the Unity Ads framework [here](https://github.com/Unity-Technologies/unity-ads-android/releases), specifically *unity-ads.aar*.  

### Using Android Studio
1. Create or open your existing Android project in Android Studio.
2. Add a new module and import the *unity-ads.aar* file. Name the module "unity-ads", for example.
3. Right-click on the module in the project view, then select **Open Module Settings** > **app**, and add "unity-ads" module as a dependency.
4. Add the following imports to your java Activity file:

```
import com.unity3d.ads.IUnityAdsListener;
import com.unity3d.ads.UnityAds;
```

### Without Android Studio
If you can't use the *.aar* packages with your build system, Unity also provides the same resources in a ZIP file (*unity-ads.zip* in GitHub releases). Follow these steps to use Unity Ads:

1. Include *classes.jar* in your build.
2. Manually merge the manifest from *AndroidManifest.xml*. Make sure you include both `AdUnitActivity` and `AdUnitSoftwareActivity` activities. You also need to add the `INTERNET` and `ACCESS_NETWORK_STATE` permissions.
3. If you are using ProGuard, add all lines from *proguard.txt* to your ProGuard configuration.

## Script implementation
### Initializing the SDK
To initialize the SDK, you must reference your Project’s __Game ID__ for the appropriate platform. You can locate the ID on the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting the Project, then selecting **Settings** > **Project Settings** from the left navigation bar (see the [Dashboard guide](MonetizationResourcesDashboardGuide.md#project-settings) section on **Project Settings** for details).

In your game script, you’ll need to implement an [`IUnityAdsListener`](MonetizationResourcesApiAndroid.md#iunityadslistener) interface that handles ad callbacks. The `initialize` method to initialize the SDK requires this listener as a parameter. Initialize the SDK early in your game’s run-time life cycle, before you need to show ads. For example:

```
import com.unity3d.ads.IUnityAdsListener;
import com.unity3d.ads.UnityAds;

public class InitializeAdsScript extends AppCompatActivity implements View.OnClickListener, IUnityAdsListener {

    private String unityGameID = "1234567";
    private Boolean testMode = true;

    @Override
    protected void onCreate (Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_main);
        // Declare a new listener:
        final UnityAdsListener myAdsListener = new UnityAdsListener ();
        // Initialize the SDK:
        UnityAds.initialize (this, unityGameID, myAdsListener, testMode);
    }

    // Implement the IUnityAdsListener interface methods:
    private class UnityAdsListener implements IUnityAdsListener {

        @Override
        public void onUnityAdsReady (String placementId) {
            // Implement functionality for an ad being ready to show.
        }

        @Override
        public void onUnityAdsStart (String placementId) {
            // Implement functionality for a user starting to watch an ad.
        }

        @Override
        public void onUnityAdsFinish (String placementId, UnityAds.FinishState finishState) {
            // Implement functionality for a user finishing an ad.
        }

        @Override
        public void onUnityAdsError (UnityAds.UnityAdsError error, String message) {
            // Implement functionality for a Unity Ads service error occurring.
        }
    }
}
```

For the `initialize` function, the `myActivity` parameter is the current Android Activity. The `unityGameID` variable is the [Unity Game ID](MonetizationResourcesDashboardGuide.md#project-settings) for you Project, found in the [developer dashboard](https://operate.dashboard.unity3d.com/). The `myAdsListener` variable is the listener for `IUnityMonetizationListener` callbacks. The `true` boolean indicates that the game is in test mode, and will only show test ads.

**Note**: You must implement each of the callback methods in the listener interface, even if they are empty functions for now. You will populate them with the appropriate logic where needed in the following sections. For more information on each listener callback method, see documentation on [`IUnityAdsListener`](MonetizationResourcesApiAndroid.md#iunityadslistener) API. 

### Interstitial display ads
To display a full-screen interstitial ad using the Advertisements API, simply initialize the SDK and use the Show function with the desired [Placement ID](MonetizationPlacements.md#placement-settings). For example:

```
import com.unity3d.ads.IUnityAdsListener;
import com.unity3d.ads.UnityAds;

public class ShowInterstitialAds extends AppCompatActivity implements View.OnClickListener, IUnityAdsListener {

    private String unityGameID = "1234567";
    private Boolean testMode = true;
    private String placementId = “interstitial”;

    @Override
    protected void onCreate (Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_main);
        // Declare a new listener:
        final UnityAdsListener myAdsListener = new UnityAdsListener ();
        // Initialize the SDK:
        UnityAds.initialize (this, unityGameID, myAdsListener, testMode);
    }

    // Implement a function to display an ad if the Placement is ready:
    @Override
    public void DisplayInterstitialAd () {
        if (UnityAds.isReady (placementId)) {
            UnityAds.show (placementId);
        }
    }

    // Implement the IUnityAdsListener interface methods:
    private class UnityAdsListener implements IUnityAdsListener {

        @Override
        public void onUnityAdsReady (String placementId) {
            // Implement functionality for an ad being ready to show.
        }

        @Override
        public void onUnityAdsStart (String placementId) {
            // Implement functionality for a user starting to watch an ad.
        }

        @Override
        public void onUnityAdsFinish (String placementId, UnityAds.FinishState finishState) {
            // Implement functionality for a user finishing an ad.
        }

        @Override
        public void onUnityAdsError (UnityAds.UnityAdsError error, String message) {
            // Implement functionality for a Unity Ads service error occurring.
        }
    }
}
```

In this example, you can invoke `DisplayInterstitialAd` from anywhere in your game you wish to show an interstitial ad.

### Rewarded video ads
Rewarding players for watching ads increases user engagement, resulting in higher revenue. For example, games may reward players with in-game currency, consumables, additional lives, or experience-multipliers. For more information on how to effectively design your rewarded ads, see documentation on [Ads best practices](MonetizationResourcesBestPracticesAds.md).

To reward players for completing a video ad, use the `onUnityAdsFinish` listener callback method’s `FinishState` result to check if the user finished watching the ad and should be rewarded. For example:

```
import com.unity3d.ads.IUnityAdsListener;
import com.unity3d.ads.UnityAds;

public class ShowRewardedAds extends AppCompatActivity implements View.OnClickListener, IUnityAdsListener {

    private String unityGameID = "1234567";
    private Boolean testMode = true;
    private String placementId = “rewarded”;

    @Override
    protected void onCreate (Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_main);
        // Declare a new listener:
        final UnityAdsListener myAdsListener = new UnityAdsListener ();
        // Initialize the SDK:
        UnityAds.initialize (this, unityGameID, myAdsListener, testMode);
    }

    // Implement a function to display an ad if the Placement is ready:
    @Override
    public void DisplayRewardedVideoAd () {
        if (UnityAds.isReady (placementId)) {
            UnityAds.show (placementId);
        }
    }

    // Implement the IUnityAdsListener interface methods:
    private class UnityAdsListener implements IUnityAdsListener {

        public void onUnityAdsReady (String placementId) {
            // Implement functionality for an ad being ready to show.
        }

        @Override
        public void onUnityAdsStart (String placementId) {
            // Implement functionality for a user starting to watch an ad.
        }

        @Override
        public void onUnityAdsFinish (String placementId, UnityAds.FinishState finishState) {
            // Implement conditional logic for each ad completion status:
           if (result == FinishState.COMPLETED) {
              // Reward the user for watching the ad to completion.
           } else if (result == FinishState.SKIPPED) {
              // Do not reward the user for skipping the ad.
           } else if (result == FinishState.ERROR) {
              // Log an error.
           }
        }

        @Override
        public void onUnityAdsError (UnityAds.UnityAdsError error, String message) {
            // Implement functionality for a Unity Ads service error occurring.
        }
    }
}
```

#### Rewarded video ad buttons
Using a button to allow the player to opt in to watching an ad is a common implementation for rewarded video ads. Use the example code below to create a rewarded ads button. The ads button displays an ad when pressed, as long as ads are available. To configure the button in the Unity Editor:

1. Select **Game Object** > **UI** > **Button** to add a button to your Scene.
2. Select the button you added to your Scene, then add a script component to it using the Inspector (**Add Component** > **New Script**). Name the script `RewardedAdsButton` to match the class name.
3. Open the script and add the following code: 

```
import com.unity3d.ads.IUnityAdsListener;
import com.unity3d.ads.UnityAds;
import android.view.View;
import android.widget.Button;

public class RewardedAdsButton extends AppCompatActivity implements View.OnClickListener, IUnityAdsListener {

    private String unityGameID = "1234567";
    private Boolean testMode = true;
    private String placementId = “rewardedButton”;
    private Button rewardedAdsButton;

    @Override
    protected void onCreate (Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_main);
        // Declare a new listener:
        final UnityAdsListener myAdsListener = new UnityAdsListener ();
        // Initialize the SDK:
        UnityAds.initialize (this, unityGameID, myAdsListener, testMode);
    }

    // Find the button in the view hierarchy and set its click function to show ads:
    final Button interstitialButton = (Button) findViewById (R.id.unityads_example_interstitial_button);
    interstitialButton.setOnClickListener (new View.OnClickListener () {
        @Override
        public void onClick (View v) {
            UnityAds.show (self, rewardedButton);
        }
    });

    // Implement a function to display an ad if the Placement is ready: 
    @Override
    public void DisplayRewardedAd () {
        if (UnityAds.isReady (placementId)) {
            UnityAds.show (placementId);
        }
    }

    // Implement the IUnityAdsListener interface methods:
    private class UnityAdsListener implements IUnityAdsListener {

        public void onUnityAdsReady (String placementId) {
            // Implement functionality for an ad being ready to show.
        }

        @Override
        public void onUnityAdsStart (String placementId) {
            // Implement functionality for a user starting to watch an ad.
        }

        @Override
        public void onUnityAdsFinish (String placementId, UnityAds.FinishState finishState) {
            // Implement conditional logic for each ad completion status:
           if (result == FinishState.COMPLETED) {
              // Reward the user for watching the ad to completion.
           } else if (result == FinishState.SKIPPED) {
              // Do not reward the user for skipping the ad.
           } else if (result == FinishState.ERROR) {
              // Log an error.
           }
        }

        @Override
        public void onUnityAdsError (UnityAds.UnityAdsError error, String message) {
            // Implement functionality for a Unity Ads service error occurring.
        }
    }
}
```

## Integration for Personalized Placements
Unity’s monetization platform provides you with powerful revenue tools. If your game uses in-app purchases as well as ads, Unity’s machine learning data model can seamlessly blend content types for an optimized monetization strategy. To learn more about how Unity helps you optimize revenue, see documentation on [Personalized Placements](MonetizationPersonalizedPlacementsAndroid.md).

The Unity Ads integration for Personalized Placements is slightly different, as it requires the `UnityMonetization` API instead of the `UnityAds` API.

### Importing the Monetization framework
Download the Unity Ads framework [here](https://github.com/Unity-Technologies/unity-ads-android/releases), specifically _unity-ads.aar_. The [`UnityMonetization`](MonetizationResourcesApiAndroid.md#unitymonetization) API requires SDK 3.0 or later. 

#### Using Android Studio
1. Create or open your existing Android project in Android Studio.
2. Add a new module and import the *unity-ads.aar* file. Name the module "unity-ads", for example.
3. Right-click on the module in the project view, then select **Open Module Settings** > **app**, and add "unity-ads" module as a dependency.
4. Add the following imports to your java Activity file:

```
import com.unity3d.services.IUnityServicesListener;
import com.unity3d.services.monetization.UnityMonetization;
```

#### Without Android Studio
If you can't use the *.aar* packages with your build system, Unity also provides the same resources in a ZIP file (*unity-ads.zip* in GitHub releases). Follow these steps to use Unity Ads:

1. Include *classes.jar* in your build.
2. Manually merge the manifest from *AndroidManifest.xml*. Make sure you include both `AdUnitActivity` and `AdUnitSoftwareActivity` activities. You also need to add the `INTERNET` and `ACCESS_NETWORK_STATE` permissions.
3. If you are using ProGuard, add all lines from *proguard.txt* to your ProGuard configuration.

### Initialization
To initialize the SDK, you must reference your Project’s Game ID for the appropriate platform. You can locate the ID on the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting the Project, then selecting **Settings** > **Project Settings** from the left navigation bar (see the [Dashboard guide](MonetizationResourcesDashboardGuide.md#project-settings) section on **Project Settings** for details).

In your game script, call the [`initialize`](MonetizationResourcesApiAndroid.md#initialize) method to initialize the SDK early in your game’s run-time life cycle, before you need to show ads. For example:

```
@Override
protected void onCreate (Bundle savedInstanceState) {
    super.onCreate (savedInstanceState);
    setContentView (R.layout.activity_main);

    UnityMonetization.initialize (myActivity, unityGameID, myListener, true);
}
```

In this example, the `myActivity` variable is the current Android Activity. The `unityGameID` variable is the [Unity Game ID](MonetizationResourcesDashboardGuide.md#project-settings) for your Project, located in the [developer dashboard](https://operate.dashboard.unity3d.com/). The `myListener` variable is the listener for `IUnityMonetizationListener` callbacks. You must implement this listener in order to initialize the SDK. For more information on how to do so, see the [API](MonetizationResourcesApiAndroid.md#iunitymonetizationlistener). The `true` boolean indicates that the game is in test mode, and will only show test ads.   

### Implementing basic (non-rewarded) ads
`PlacementContent` is an object representing monetization content that your Placement can display (for more information, see documentation on [Content types](MonetizationContentTypes.md) and [Personalized Placements](MonetizationPersonalizedPlacementsAndroid.md)). Use the `getPlacementContent` function to retrieve content when it’s ready to display, and the [`show`](MonetizationResourcesApiAndroid.md#show) function to display it. For example:

```
@Override
    public void showAds (View view) {

        // Check if the Placement is ready:
        if (UnityMonetization.isReady (placementId)) {
            // Retrieve the PlacementContent that is ready:
            PlacementContent pc = UnityMonetization.getPlacementContent (placementId);
            // Check that the PlacementContent is the desired type:
            if (pc.getType ().equalsIgnoreCase ("SHOW_AD")) {
                // Cast the PlacementContent as the desired type:
                ShowAdPlacementContent p = (ShowAdPlacementContent) pc;
                // Show the PlacementContent:
                p.show (this, this);
            }
        } else {
            Log.e ("This Placement is not ready!");
        }
    }
```

### Implementing rewarded ads
Rewarding players for watching ads increases user engagement, resulting in higher revenue. For example, games may reward players with in-game currency, consumables, additional lives, or experience-multipliers. For more information on how to effectively design your rewarded ads, see documentation on [ads best practices](MonetizationResourcesBestPracticesAds.md).

To reward players for watching ads, follow the same steps as detailed in the basic implementation section, but show the ad using a reward callback method with custom logic for players completing the ad.

#### Selecting a Placement
You must display rewarded ads through [Rewarded Placements](MonetizationPlacements.md#placement-types). Every Unity Ads-enabled Project also has a ‘`rewardedVideo`’ Placement by default. Feel free to use this for your implementation, or [create your own](MonetizationPlacements.md#creating-new-placements) (but make sure your Placement is configured as Rewarded).

#### Adding a callback method to your script
The `show` function takes an `onAdsFinished` callback method that the SDK uses to return a `FinishState` enum. This result indicates whether the player finished or skipped the ad. Use this information to write a custom function for how to handle each scenario. For example:

```
@Override
public void onAdFinished (String s, UnityAds.FinishState finishState) {
    if (finishState == UnityAds.FinishState.COMPLETED) {
        if (s.equals (rewardedPlacementId)) {
            // Reward the player here.
        }
    }
}
```

### Rewarded ads button code example
The following example checks at a specific point in the game whether `PlacementContent` is ready to display. As an alternate method, you can implement a listener to notify you when content is available.

```
public class MainActivity extends AppCompatActivity implements View.OnClickListener, IUnityServicesListener, IShowAdListener {
    private Button rewardedAdsButton;
    private String unityGameID = "1234567";
    private String interstitialPlacementId = "video";
    private String rewardedPlacementId = "rewardedVideo";

    @Override
    protected void onCreate (Bundle savedInstanceState) {
        super.onCreate (savedInstanceState);
        setContentView (R.layout.activity_main);

        UnityMonetization.initialize (myActivity, unityGameID, myListener, true);

        rewardedAdsButton = findViewById (R.id.rewardedAdsButton);
        rewardedAdsButton.setOnClickListener (this);
    }

    @Override
    public void onClick (View view) {
        if (UnityMonetization.isReady (rewardedPlacementId)) {
            PlacementContent pc = UnityMonetization.getPlacementContent (rewardedPlacementId);
            if (pc.getType ().equalsIgnoreCase ("SHOW_AD")) {
                ShowAdPlacementContent p = (ShowAdPlacementContent) pc;
                p.show(this, this);
            }
        } else {
            Log.e ("This Placement is not ready!");
        }
    }

    @Override
    public void onAdFinished (String s, UnityAds.FinishState finishState) {
        if (finishState == UnityAds.FinishState.COMPLETED) {
            if (s.equals (rewardedPlacementId)) {
                // Reward the player here.
            }
        }
    }
}
```

## Testing
Prior to publishing your game, enable test mode by following these steps: 

1. From the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/), select your Project.
2. Select **Monetization** > **Platforms** from the left navigation bar.
3. Select the desired platform, then select the **SETTINGS** tab.
4. Scroll down to the **TEST MODE** section and toggle override client test mode, then select the **Force test mode ON** radio button.

Run your project and test your ads implementation.

**Note**: You must enable test mode before testing ads integration, to avoid getting flagged for fraud.

## What's next?
Take your implementation to the next level by using Unity's additional monetization features to optimize your revenue. Here are some next steps to explore:

* Incorporate other ad content types.
    * View documentation for [banner ads integration](MonetizationBannerAdsAndroid.md).
    * View documentation for [AR ads integration](MonetizationArAdsAndroid.md).
* Incorporate in-app purchases (IAP), then promote them.
    * Use Unity's [purchasing integration](MonetizationPurchasingIntegrationAndroid.md) feature to integrate your purchasing solution for [IAP Promos](https://docs.unity3d.com/Manual/IAPPromo.html).
* Let machine learning power your monetization strategy.
    * When you have Ads and IAP Promo set up, use [Personalized Placements](MonetizationPersonalizedPlacementsAndroid.md) to provide revenue lift for your entire game.
* Review our [best practices](MonetizationResourcesBestPracticesAds.md) guide for insight on how to design effective ad mechanics.