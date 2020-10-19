# Banner ads for Android developers
## Overview
This guide covers implementation for banner ads in your Android game.

* If you are a Unity developer using C#, [click here](MonetizationBannerAdsUnity.md). 
* If you are an iOS developer using Objective-C, [click here](MonetizationBannerAdsIos.md). 
* [Click here](MonetizationResourcesApiAndroid.md#bannerview) for the Java `BannerView` API reference.

## Configuring your game for Unity Ads
To implement banner ads, you must integrate Unity Ads in your Project. To do so, follow the steps in the [basic ads integration guide](MonetizationBasicIntegrationAndroid.md) that detail the following:

* [Creating a Project in the Unity developer dashboard](MonetizationBasicIntegrationAndroid.md#creating-a-project-in-the-unity-developer-dashboard)
* [Importing the Unity Ads framework](MonetizationBasicIntegrationAndroid.md#importing-the-unity-ads-framework)

Once your Project is configured for Unity Ads, proceed to creating a banner Placement.

## Creating a banner Placement
[Placements](MonetizationPlacements.md) are triggered events within your game that display monetization content. Manage Placements from the **Operate** tab of the [Developer Dashboard](https://operate.dashboard.unity3d.com/) by selecting your Project, then selecting **Monetization** > **Placements** from the left navigation bar.

Click the **ADD PLACEMENT** button to bring up the Placement creation prompt. Name your Placement and select the **Banner** type.

## Script implementation
Follow the steps in the basic integration guide for [Initializing the SDK](MonetizationBasicIntegrationAndroid.md#initializing-the-sdk). You must intialize Unity Ads before displaying a banner ad.

In your game script, import the `BannerView` API, then implement an [`IListener`](MonetizationResourcesApiAndroid.md#ilistener) interface to provide callbacks to the SDK. The following script sample is an example implementation for displaying two banner ads on the screen, and buttons for testing them. For more information on the classes referenced, see the [`BannerView`](MonetizationResourcesApiAndroid.md#bannerview) API section.

```
package com.example.test_app;
import androidx.appcompat.app.AppCompatActivity;
import android.os.Bundle;
import com.unity3d.ads.UnityAds;
import com.unity3d.services.banners.BannerView;
import com.unity3d.services.banners.UnityBannerSize;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.RelativeLayout;
public class MainActivity extends AppCompatActivity {

    String unityGameID = "3054608";
    Boolean testMode = true;
    Boolean enableLoad = true;
    String bannerPlacement = "banner";
    // Listener for banner events:
    UnityBannerListener bannerListener = new UnityBannerListener();
    // This banner view object will be placed at the top of the screen:
    BannerView topBanner;
    // This banner view object will be placed at the bottom of the screen:
    BannerView bottomBanner;
    // View objects to display banners:
    RelativeLayout topBannerView;
    RelativeLayout bottomBannerView;
    // Buttons to show the banners:
    Button showTopBannerButton;
    Button showBottomBannerButton;
    // Buttons to destroy the banners:
    Button hideTopBannerButton;
    Button hideBottomBannerButton;
    
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        // Initialize Unity Ads:
        UnityAds.initialize (this, unityGameID, null, testMode, enableLoad);
        
        // Set up a button to load the top banner when clicked:
        showTopBannerButton = findViewById(R.id.loadTopBanner);
        showTopBannerButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                showTopBannerButton.setEnabled(false);
                // Create the top banner view object:
                topBanner = new BannerView(view.getContext(), bannerPlacement, new UnityBannerSize(320, 50));
                // Set the listener for banner lifcycle events:
                topBanner.setListener(bannerListener);
                // Request a banner ad:
                topBanner.load();
                // Get the banner view:
                topBannerView = findViewById(R.id.topBanner);
                // Associate the banner view object with the banner view:
                topBannerView.addView(topBanner);
            }
        });

        // Set up a button to load the bottom banner when clicked:
        showBottomBannerButton = findViewById(R.id.loadBottomBanner);
        showBottomBannerButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                showBottomBannerButton.setEnabled(false);
                bottomBanner = new BannerView(view.getContext(), bannerPlacement, new UnityBannerSize(320, 50));
                bottomBanner.setListener(bannerListener);
                bottomBannerView = findViewById(R.id.bottomBanner);
                bottomBannerView.addView(bottomBanner);
                bottomBanner.load();
            }
        });

        // Set up a button to destroy the top banner when clicked:
        hideTopBannerButton = findViewById(R.id.hideTopBanner);
        hideTopBannerButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Remove content from the banner view:
                topBannerView.removeAllViews();
                // Remove the banner variables:
                topBannerView = null;
                topBanner = null;
                showTopBannerButton.setEnabled(true);
            }
        });

        // Set up a button to destroy the bottom banner when clicked:
        hideBottomBannerButton = findViewById(R.id.hideBottomBanner);
        hideBottomBannerButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                bottomBannerView.removeAllViews();
                bottomBannerView = null;
                bottomBanner = null;
                showBottomBannerButton.setEnabled(true);
            }
        });
    }

    BannerView SetUpBanner(){
        return new BannerView(this, bannerPlacement, new UnityBannerSize(320, 50));
    }

    // Implement listener methods:
    private class UnityBannerListener implements BannerView.IListener {
		@Override
		public void onBannerLoaded(BannerView bannerAdView) {
		    // Called when the banner is loaded.
        }

		@Override
		public void onBannerFailedToLoad(BannerView bannerAdView, BannerErrorInfo errorInfo) {
		    Log.d("SupportTest", "Banner Error" + s);
            // Note that the BannerErrorInfo object can indicate a no fill (see API documentation).
        }

		@Override
		public void onBannerClick(BannerView bannerAdView) {
            // Called when a banner is clicked.
		}

		@Override
		public void onBannerLeftApplication(BannerView bannerAdView) {
		    // Called when the banner links out of the application.
        }
    }
}
```

**Note**: This example uses a single listener for multiple banner view objects. You can also have a different listener for each banner view object.

### Banner position
You can place the banner view object into your view hierarchy, just like you would any other view. This allows you to customize the position of each banner instance, or display multiple banners.

## What's next? 
View documentation for [AR ads integration](MonetizationArAdsAndroid.md) to offer players a fully immersive and interactive experience by incorporating digital content directly into their physical world, or [return](Monetization.md) to the monetization hub.
