# Android (Java) API reference
## Overview
This article documents API classes for the following namespaces:

* [`UnityAds`](#unityads)
* [`BannerView`](#bannerview)
* [`UnityMonetization`](#unitymonetization)
* [`UnityPurchasing`](#unitypurchasing)

For a comprehensive integration guide, click [here](MonetizationBasicIntegrationAndroid.md).

## UnityAds
Use this namespace to [implement basic ad content](MonetizationBasicIntegrationAndroid.md), such as rewarded or non-rewarded video, interstitial, or banner ads.  

```
import com.unity3d.ads.UnityAds;
```

### Methods
#### initialize
Initializes the Unity Ads service with a specified Game ID and test mode status.

```
public static void initialize (final Activity activity,
                               final String gameId, 
                               final IUnityAdsListener listener, 
                               final boolean testMode, 
                               final boolean enablePerPlacementLoad)
```

| **Parameter** | **Data type** | **Description** |
| ------------- | ------------- | --------------- |
| `activity` | Activity | The current activity of the Android device calling the app. |
| `gameId` | String | The [Unity Game ID](MonetizationResourcesDashboardGuide.md#project-settings) for your Project, located in the [developer dashboard](https://operate.dashboard.unity3d.com/). |
| `listener` | `IUnityAdsListener` | A listener for [`IUnityAdsListener`](#iunityadslistener) callbacks. |
| `testMode` | boolean | When set to `true`, only test ads display. |
| `enablePerPlacementLoad` | boolean | When set to `true`, this parameter allows you to load content for a specific Placement prior to displaying it. Please review the [load](#load) method documentation before enabling this setting. |

You can check the initialization status with the following function:

```
public static boolean isInitialized ()
```

You can check whether the current platform is supported by the SDK with the following function:

```
public static boolean isSupported ()
```

#### load
Loads ad content for a specified Placement, allowing for more accurate fill request reporting that is consistent with the standards of most mediation providers. If you initialized the SDK with `enablePerPlacementLoad` enabled, you must call `load` before calling `show`.

**Note** The `load` API is in closed beta and available upon invite only. If you would like to be considered for the beta, please [contact Unity Ads](mailto:ads-support@unity3d.com).

```
public static void load (final String placementId) {
    UnityAdsImplementation.load (placementId);
}
```

#### show
Shows content in the specified Placement, if it is ready.

```
public static void show (final Activity activity, final String placementId)
```

| **Parameter** | **Data type** | **Description** |
| ------------- | ------------- | --------------- |
| `activity` | Activity | The current activity of the Android device calling the app. |
| `placementId` | String | The [Placement ID](MonetizationPlacements.md#placement-settings), located on the [developer dashboard](https://operate.dashboard.unity3d.com/). |

You can check whether an ad is ready to be shown for the specified [Placement](MonetizationPlacements.md) by calling: 

```
public static boolean isReady (String placementId)
```

#### addListener
Adds an active listener for [`IUnityAdsListener`](#iunityadslistener) callbacks. SDK versions 3.1+ allow you to register multiple listeners. This is especially helpful for [mediation](MonetizationResourcesMediation.md) customers.

```
public static void addListener (IUnityAdsListener listener) 
```

#### removeListener
Allows you to remove an active listener.

```
public static void removeListener (IUnityAdsListener listener) 
```

#### setListener
Sets the current listener for `IUnityAdsListener` callbacks. Use this if you only ever use one listener.

```
public static void getListener ()
```

**Note**: `setListener` is a deprecated method. Unity recommends using [`addListener`](#addlistener) instead.

#### getListener
Retrieves the current listener for `IUnityAdsListener` callbacks. Returns the first listener added.

```
public static void getListener () 
```

**Note**: `getListener` is a deprecated method. 

#### setDebugMode
Controls the amount of logging output from the SDK. Set to `true` for more robust logging. 

```
public static void setDebugMode (boolean debugMode)
```

You can check the current status for debug logging from the SDK by calling: 

```
public static boolean getDebugMode ()
```

### Interfaces
#### IUnityAdsListener
An interface for handling various states of an ad. Implement this listener in your script to define logic for rewarded ads. The interface has the following methods: 

```
public interface IUnityAdsListener {
    void onUnityAdsReady (String placementId);
    void onUnityAdsError (UnityAds.UnityAdsError error, String message);
    void onUnityAdsStart (String placementId);
    void onUnityAdsFinish (String placementId, UnityAds.FinishState result);
}
```

**Note**: Unity recommends implementing all of these methods in your code, even if you don’t use them all.

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `onUnityAdsReady` | Handles logic for ad content being ready to display through a specified [Placement](MonetizationPlacements.md). |
| `onUnityAdsStart` | Handles logic for the player triggering an ad to play. |
| `OnUnityAdsFinish` | Handles logic for an ad finishing. Define conditional behavior for different finish states by accessing the `FinishState` result from the listener (documented below). For example:<br><br><pre>void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {<br>&nbsp;&nbsp;if (showResult == ShowResult.Finished) {<br>&nbsp;&nbsp;&nbsp;&nbsp;// Reward the user for watching the ad to completion.<br>&nbsp;&nbsp;} else if (showResult == ShowResult.Skipped) {<br>&nbsp;&nbsp;&nbsp;&nbsp;// Do not reward the user for skipping the ad.<br>&nbsp;&nbsp;} else if (showResult == ShowResult.Failed) {<br>&nbsp;&nbsp;&nbsp;&nbsp;Debug.LogWarning (“The ad did not finish due to an error.);<br>&nbsp;&nbsp;}<br>}<br></pre> |
| `onUnityAdsError` | Handles logic for ad content failing to display because of an [error](#unityadserror). |

### Enums
#### FinishState
The enumerated states of the end-user’s interaction with the ad. The SDK passes this value to the `onUnityAdsFinish` callback after the ad completes.

```
enum UnityAds.FinishState
```

| **Value** | **Description** |
| --------- | --------------- |
| `ERROR` | Indicates that the ad failed to complete due to a Unity error. |
| `SKIPPED` | Indicates that the user skipped the ad. |
| `COMPLETED` | Indicates that the user successfully finished watching the ad. |

#### PlacementState
The enumerated states of a Unity Ads [Placement](MonetizationPlacements.md).

```
enum UnityEngine.Advertisements.PlacementState
```

| **Value** | **Description** |
| --------- | --------------- |
| `READY` | The Placement is ready to show ads. |
| `NOT_AVAILABLE` | The Placement is not available. |
| `DISABLED` | The Placement was disabled. |
| `WAITING` | The Placement is waiting to be ready. |
| `NO_FILL` | The Placement has no advertisements to show. |

Retrieve the `PlacementState` value with the following function: 

```
public static PlacementState getPlacementState (String placementId)
```

#### UnityAdsError
The enumerated causes of a Unity Ads error. 

| **Value** | **Description** |
| --------- | --------------- |
| `NOT_INITIALIZED` | The Unity Ads service is currently uninitialized. | 
| `kINITIALIZE_FAILED` | An error occurred in the initialization process. |
| `INVALID_ARGUMENT` | Unity Ads initialization failed due to invalid parameters. |
| `VIDEO_PLAYER_ERROR` | An error occurred due to the video player failing. |
| `INIT_SANITY_CHECK_FAIL` | Initialization of the Unity Ads service failed due to an invalid environment. |
| `AD_BLOCKER_DETECTED` | An error occurred due to an ad blocker. |
| `FILE_IO_ERROR` | An error occurred due to inability to read or write a file. |
| `DEVICE_ID_ERROR` |  An error occurred due to a bad device identifier. |
| `SHOW_ERROR` | An error occurred when attempting to show an ad. |
| `INTERNAL_ERROR` | An internal Unity Ads service error occurred. |

## BannerView
Use this namespace to [implement banner ads](MonetizationBannerAdsAndroid.md). Unity Ads version 3.3 and above supports multiple banner instances through a single Placement. 

### Methods
#### BannerView
Constructor used to instantiate a banner object using a Placement ID and banner size. Note that the banner object accesses lifecycle events through its [`IListener` interface](#ilistener).

```
public BannerView(Context context, String placementId, BannerSize size)
```

| **Parameter** | **Type** | **Description** |
| ------------ | -------- | --------------- |
| `placementId` | String | The banner's [Placement ID](MonetizationPlacements.md#placement-settings) (located on the [developer dashboard](https://operate.dashboard.unity3d.com/)). |
| `size` | [`BannerSize`](#bannersize) | The size of the banner object. The minimum support size is 320 pixels by 50 pixels. |

**Note**: Using `BannerView` to call a banner ad attempts to load content once, with no refreshing. If the banner returns no fill, you may [destroy](#destroy) it and create a new one to try again. Unity recommends this method for mediated partners.

#### getPlacementId
Returns the Placement ID called in the banner instance.

```
public String getPlacementId();
```

#### getSize
Returns a banner's [size](#bannersize).

```
public BannerSize getSize();
```

#### setListener
Sets the active listener for the banner.

```
public void setListener(IListener listener);
```

#### getListener
Retrieves the active listener for the banner.

```
public IListener getListener();
```

#### load
The basic method for requesting an ad for the banner.

```
public void load();
```

#### destroy
Call this method to remove the banner from the view hierarchy when you’re no longer using it.

```
public void destroy();
```

### Interfaces
#### IListener
A `BannerView` interface that grants access to banner lifecycle events.

```
public interface IListener {
    void onBannerLoaded(BannerView bannerAdView);
    void onBannerClick(BannerView bannerAdView);
    void onBannerFailedToLoad(BannerView bannerAdView, BannerErrorInfo errorInfo);
    void onBannerLeftApplication(BannerView bannerView);
}
```

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `onBannerLoaded` | Called when the  banner finishes loading an ad. The view parameter references the banner that should be inserted into the view hierarchy. |
| `onBannerClick` | Called when the  banner is clicked. |
| `onBannerError` | Called when an error occurs showing the banner. |
| `onBannerLeftApplication` | Called when the banner links outside the application. |

In addition, you can implement the following override methods:

```
public static abstract class Listener implements IListener {
    @Override
    public void onBannerLoaded(BannerView bannerAdView) {
    }

    @Override
    public void onBannerFailedToLoad(BannerView bannerAdView, BannerErrorInfo errorInfo) {
    }

    @Override
    public void onBannerClick(BannerView bannerAdView) {
    }

    @Override
    public void onBannerLeftApplication(BannerView bannerAdView) {
    }
}
```

### Classes
#### BannerSize
Specifies the height and width of a banner object. Unity delivers content that best fits the supplied dimensions. The minimum supported size is 320 pixels by 50 pixels.

```
public class UnityBannerSize {

    private int width;
    private int height;

    public BannerSize(int width, int height) {
        this.width = width;
        this.height = height;
    }

    public int getWidth() {
        return width;
    }

    public int getHeight() {
        return height;
    }
}
```

#### BannerErrorInfo
An object for returning an error code and message.

```
public class BannerErrorInfo {
	public BannerErrorCode errorCode;
	public String errorMessage;
	public BannerErrorInfo (String errorMessage, BannerErrorCode errorCode) {
        this.errorCode = errorCode;
        this.errorMessage = errorMessage;
    }
}
```

#### BannerErrorCode
An enum for the type of error encountered during the banner lifecycle.

```
public enum  BannerErrorCode {
    UNKNOWN,
    NATIVE_ERROR,
    WEBVIEW_ERROR,
    NO_FILL
}
```

## UnityBanners (deprecated)
Use this namespace to [implement banner ads](MonetizationBannerAdsAndroid.md).

**Note**: This API, along with all its methods and classes, has been deprecated in favor of [`BannerView`](#bannerview).

```
import com.unity3d.services.banners.UnityBanners;
```

### Methods
#### loadBanner (deprecated)
The basic method for requesting an ad for the banner. 

```
public static void loadBanner (final Activity activity, final String placementId)
```

#### destroy (deprecated)
Call this method to remove the banner from the view hierarchy when you’re no longer using it.

```
public static void destroy ()
```

#### setBannerPosition (deprecated)
Sets the position of the banner ad, using the [`BannerPosition`](#bannerposition) enum.

```
public static void setBannerPosition (BannerPosition position)
```

### Interfaces
#### IUnityBannerListener (deprecated)
Implement this interface to handle various banner states. The listener includes the following methods:

```
public interface IUnityBannerListener {
    void onUnityBannerLoaded (String placementId, View view);
    void onUnityBannerUnloaded (String placementId);
    void onUnityBannerShow (String placementId);
    void onUnityBannerClick (String placementId);
    void onUnityBannerHide (String placementId);
    void onUnityBannerError (String message);
}
```

**Note**: Unity recommends implementing all of these methods in your code, even if you don’t use them all.

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `onUnityBannerLoaded` | Called when the Unity banner finishes loading an ad. The view parameter references the banner that should be inserted into the view hierarchy. |
| `onUnityBannerUnloaded` | Called when ad content is unloaded from the banner, and references to its view should be removed. |
| `onUnityBannerShow` | Called when the Unity banner is shown for the first time and visible to the user. |
| `onUnityBannerClick` | Called when the Unity banner is clicked. |
| `onUnityBannerHide` | Called when the Unity banner is hidden. |
| `onUnityBannerError` | Called when an error occurs showing the banner. |

### Enums
#### BannerPosition (deprecated)
The enumerated positions you can set as an anchor for banner ads.

```
public enum BannerPosition {
    TOP_LEFT,
    TOP_CENTER,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_CENTER,
    BOTTOM_RIGHT,
    CENTER
}
```

## UnityMonetization
Use this namespace to [implement ads](MonetizationBasicIntegrationAndroid.md#integration-for-personalized-placements) for use with [Personalized Placements](MonetizationPersonalizedPlacementsAndroid.md).

```
import com.unity3d.services.monetization.UnityMonetization;
```

### Methods
#### initialize
Initializes Unity Ads in your game at run time. To avoid errors, initialization should occur early in the game’s run-time lifecycle, preferably at boot. Initialization must occur prior to showing ads.

```
public static void initialize (Activity activity, 
                               String gameId, 
                               IUnityMonetizationListener listener, 
                               boolean testMode);
```

The `activity` parameter is the current Android Activity. The `gameID` parameter is the Game ID for your Project [found on the developer dashboard](MonetizationResourcesDashboardGuide.md#game-ids). The `listener` parameter is the listener for `IUnityMonetizationListener` callbacks (documented below). The `testMode` parameter indicates whether the game is in test mode. When `testMode` is `true`, you will only see test ads. When `testMode` is `false`, you will see live ads. It is important to use test mode prior to launching your game, to avoid being flagged for fraud.

You can check whether the service is initialized by calling:

Checks whether a `PlacementContent` object is ready for the given Placement. Returns `true` if content is ready, and `false` if it isn’t.

```
public static boolean isInitialized ();
```

#### isReady
Checks whether a [`PlacementContent`](#placementcontent) object is ready for the given Placement. Returns `true` if content is ready, and `false` if it isn’t.

```
public static boolean isReady (String placementId);
```

#### `setListener`
Sets the `IUnityMonetizationListener` listener for `PlacementContent` callback events.

```
public static void setListener (IUnityMonetizationListener listener);
```

#### getListener
Returns the current `IUnityMonetizationListener` listener for `PlacementContent` callback events.

```
public static IUnityMonetizationListener getListener ();
```

### Interfaces
#### IUnityMonetizationListener
An interface for handling various states of a [`PlacementContent`](#placementcontent) object. The interface has the following methods: 

```
public interface IUnityMonetizationListener extends IUnityServicesListener {
    void onPlacementContentReady (String placementId, 
                                  PlacementContent placementcontent);
    void onPlacementContentStateChange (String placementId, 
                                        PlacementContent placementcontent, 
                                        UnityMonetization.PlacementContentState previousState, 
                                        UnityMonetization.PlacementContentState newState);
}
```

**Note**: Unity recommends implementing all of these methods in your code, even if you don’t use them all.

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `onPlacementContentReady` | This function notifies you that a `PlacementContent` object is ready for a given Placement and ready to show. |
| `onPlacementContentChanged` | This function notifies you when the readied `PlacementContent` object’s status has changed due to a refresh. |

Example `IUnityMonetizationListener` implementation:

```
private class UnityMonetizationListener implements IUnityMonetizationListener {
    
    @Override
    public void onPlacementContentReady (String placementId, PlacementContent placementContent) {
        // Check the Placement ID to determine behavior
        switch (placementId) {
            case "mixedPlacement":
                if (placementContent instanceof PromoAdPlacementContent) {
                    PromoAdPlacementContent promoPlacementContent = ((PromoAdPlacementContent) placementContent)
                    // Promo content is ready, prepare Promo display
                } else if (placementContent instanceof ShowAdPlacementContent) {
                    ShowAdPlacementContent adPlacementContent = ((ShowAdPlacementContent) placementContent);
                    // Ad content is ready, prepare video ad display
                }
                break;
            case "rewardedVideo":
                if (placementContent instanceof ShowAdPlacementContent) {
                    ShowAdPlacementContent adPlacementContent = ((ShowAdPlacementContent) placementContent);
                    if (adPlacementContent.isRewarded ()) {
                        // Rewarded content is ready, prepare content for display and implement reward handlers 
                    }
                }
                break;
        }
    }

    @Override
    public void onPlacementContentStateChange (String placementId, PlacementContent placementContent, UnityMonetization.PlacementContentState previousState, UnityMonetization.PlacementContentState newState) {
    }
}
```

#### IShowAdListener
Implement this interface for [`ShowAdPlacementContent`](#showadplacementcontent) objects. The interface has the following methods:

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `void onAdStarted (String placementId);` | Called when the video ad starts. |
| `void onAdFinished(String placementId, UnityAds.FinishState withState);` | Called when the video ad finishes. Use this callback method to evaluate the ad's [`FinishState`](#finishstate) and act accordingly. |

### Classes
#### PlacementContent
A class representing monetization content that your [Placement](MonetizationPlacements.md) can display. 

Associated methods:

| **Method** | **Description** |
| ---------- | --------------- |
| `public static PlacementContent getPlacementContent (String placementId);` | Returns the `PlacementContent` object that is ready for the given Placement. Returns `null` if no content is ready. 
| `public static <T extends PlacementContent> T getPlacementContent (String placementId, Class<T> asClass);` | Extends the `getPlacementContent` function to cast the returned `PlacementContent` as a specific type. The `asClass` parameter represents the type you wish to cast to. If the returned content is of a different type, it returns `null`. |
| `public boolean isReady ();` | This function returns `true` if the `PlacementContent` object is ready to display, and `false` if it isn’t. |
| `public String getType ();` | This function returns the type of `PlacementContent` available to display. The following string values are valid:<br><br><ul><li>`SHOW_AD` refers to video ad content using the [`ShowAdPlacementContent`](#showadplacementcontent) class extension.</li><li>`PROMO_AD` refers to IAP Promo content using the [`PromoAdPlacementContent`](#promoadplacementcontent) class extension.</li><li>`NO_FILL` refers to a lack of `PlacementContent` available for the specified Placement.</li></ul> |

#### RewardablePlacementContent
Extends the `PlacementContent` class with functionality to support rewarded content.

Associated methods:

| **Method** | **Description** |
| ---------- | --------------- |
| `public boolean isRewarded ();` | This function returns `true` if the `PlacementContent` is rewarded, and `false` if it isn’t. |

#### ShowAdPlacementContent
Extends the `RewardablePlacementContent` class with functionality to support video ad content.

Associated methods:

| **Method** | **Description** |
| ---------- | --------------- |
| `public void show (Activity activity, IShowAdListener listener);` | This function displays the available `ShowAdPlacementContent`. Implement an `IShowAdListener` interface to define how this function responds to each ad [`FinishState`](#finishstate). |

#### PromoAdPlacementContent
Extends the `ShowAdPlacementContent` class, providing functionality for [IAP Promo](https://docs.unity3d.com/Manual/IAPPromo.html) content. For more information, see documentation on [Native Promos](MonetizationNativePromoAndroid.md).

### Enums
#### PlacementContentState 
The enumerated states of a [Placement](MonetizationPlacements.md).

| **Value** | **Description** |
| --------- | --------------- |
| `READY` | `PlacementContent` is available to `show`. |
| `NOT_AVAILABLE` | `PlacementContentState` is unavailable. Either the SDK is not properly initialized, or the respective Placement is not configured to display monetization content. |
| `DISABLED` | The specified Placement is disabled. You can enable the Placement via the [developer dashboard](https://operate.dashboard.unity3d.com/). |
| `WAITING` | The Placement is not yet ready, but it will be in the future. This is likely due to caching. |
| `NO_FILL` | The Placement is properly configured, but there are currently no ads available to show. |

#### FinishState
An enum representing the final state of an ad when finished. Use it to handle reward cases.

| Value | Description |
| ----- | ----------- |
| `COMPLETED` | The player viewed the ad in its entirety. |
| `SKIPPED` | The player skipped the ad before it played in its entirety. |
| `ERROR` | The ad failed to load. |

## UnityPurchasing
This class manages [purchasing adapters](MonetizationPurchasingIntegrationAndroid.md), which are interfaces for the SDK to retrieve the information it needs from your custom IAP implementation. 

```
public class UnityPurchasing {    
    public static void setAdapter (IPurchasingAdapter adapter);
    public static IPurchasingAdapter getAdapter ();
}
```

### Interfaces
#### IPurchasingAdapter
An interface for implementing a custom purchasing solution that's compatible with [IAP Promo](https://docs.unity3d.com/Manual/IAPPromo.html).  

```
public interface IPurchasingAdapter {
    void retrieveProducts (IRetrieveProductsListener listener);
    void onPurchase (String productID, ITransactionListener listener, Map<String, Object> extras);
}
```

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `void retrieveProducts (IRetrieveProductsListener listener);` | The SDK calls this to retrieve the list of available Products. Use the [`IRetrieveProductsListener`](#iretrieveproductslistener) interface to register your Product list. |
| `void onPurchase (String productID, ITransactionListener listener, Map<String, Object> extras);` | The SDK calls this when a user clicks the buy button for a promotional asset. Use the [`ITransactionListener`](#itransactionlistener) interface to listen for the transaction result. Unity passes the purchased Product’s ID to your in-app purchasing system. The purchase’s success or failure handling depends on your in-app purchasing implementation. | 

Example of an implemented `retrieveProducts` function:  

```
@Override
public void retrieveProducts (IRetrieveProductsListener listener) {
    listener.onProductsRetrieved (Arrays.asList (Product.newBuilder ()
        .withProductId ("100BronzeCoins")     
        .withLocalizedTitle ("100 Bronze Coins")
        .withLocalizedPriceString ("$1.99")
        .withLocalizedPrice (1.99)
        .withIsoCurrencyCode ("USD")
        .withLocalizedDescription ("100 Bronze Coins at a new low price!")
        .withProductType ("Consumable")
        .build ()));
}
```

Example of an implemented `onPurchase` function:
```
@Override
public void onPurchase (String productID, ITransactionListener listener, Map<String, Object> extras) {
    thirdPartyPurchasing.purchase (productId); // Generic developer purchasing function
    
    // If purchase succeeds:
    listener.onTransactionComplete (TransactionDetails.newBuilder ()
        .withProductId ("100BronzeCoins")

        .withTransactionId ("ABCDEFG")
        .withReceipt ("receipt":"{\"data\": \"{\"Store\": \"fake\", \"TransactionID\": \"ce7bb1ca-bd34-4ffb-bdee-83d2784336d8\", \"Payload\": \"{ \\\"this\\\": \\\"is a fake receipt\\\"}\"}\"}")
        .withPrice (1.99)

        .withIsoCurrency ("USD")

        .build());

    // If purchase fails:
    listener.onTransactionError (TransactionError.NETWORK_ERROR, "No Network Connection");
}
```

#### IRetrieveProductsListener
Registers your IAP products with the SDK. Your implementation must convert your IAP products into [`UnityMonetization.Product`](#product) objects. This requires a minimum of the following properties for each Product:<br><br><ul><li>`productID`</li><li>`localizedPriceString`</li><li>`productType`</li><li>`isoCurrencyCode`</li><li>`localizedTitle`. </li></ul>

```
public interface IRetrieveProductsListener {
    void onProductsRetrieved (List<Product> availableProducts);
}
```

#### ITransactionListener
An interface that passes a transaction's result back to your purchasing system, in the form of a [`TransactionDetails`](#transactiondetails) or `TransactionErrorDetails` object.

| **Interface method** | **Description** | 
| -------------------- | --------------- |
| `void onTransactionComplete (TransactionDetails details);` | Your custom game logic for handling a successful transaction. The function takes a [```TransactionDetails```](#transactiondetails) object, which details the specifics of a transaction. |
| `void onTransactionError (TransactionErrorDetails details);` | Your custom game logic for handling a failed transaction. The function takes a [```TransactionError```](#transactionerror) object, which identifies the source of transaction failure. |

#### NativePromoAdapter
This interface provides access methods for handling user interaction with [promotional assets](MonetizationNativePromoAndroid.md). Use these methods to pass in your custom assets and define expected behavior. Pass a [`PromoAdPlacementContent`](#promoadplacementcontent) object through the `NativePromoAdapter` function to create a new adapter. For example:

```
final NativePromoAdapter nativePromoAdapter = new NativePromoAdapter (placementContent);
```

Implement this delegate with the following methods:

| **Delegate method** | **Description** | 
| -------------------- | --------------- |
| `public void onShown (NativePromoShowType type);` | Call this function when the Promo is shown. It should include your custom method for displaying promotional assets. You can pass a [`NativePromoShowType`](#nativepromoshowtype) enum value to reference the preview type of your Promo asset. |
| `public void onClosed ();` | Call this function when the player dismisses the Promo offer. |
| `public void onClicked ()` | Call this function when the player clicks the button to purchase the Product. It should initiate the purchase flow. |

### Classes
#### PromoAdPlacementContent
Extends the [`ShowAdPlacementContent`](#showadplacementcontent) class, providing functionality for IAP Promo content. This class has the following properties:

| **Property** | **Description** |
| ------------ | --------------- |
| `PromoMetaData` | Contains information for a `PromoAdPlacementContent` object passed through the adapter. |

#### Product
An IAP product converted into an object that the SDK can use for optimizing monetization.

Set the following fields on the builder using the `public static Builder newBuilder()` method, then the `public Product build()` method to construct the final Product object.

| **Builder method** | **Param type** | **Description** |
| ------------------ | -------------- | --------------- |
| `withProductId` | `String` | An internal reference ID for the Product. |
| `withLocalizedTitle` | `String` | A consumer-facing name for the Product, for store UI purposes. |
| `withLocalizedPriceString` | `String` | A consumer-facing price string, including the currency sign, for store UI purposes. |
| `withLocalizedPrice` | `Double` | The internal system value for the Product’s price. |
| `withIsoCurrencyCode` | `String` | The [ISO code](https://en.wikipedia.org/wiki/ISO_4217) for the Product’s localized currency. |
| `withLocalizedDescription` | `String` | A consumer-facing Product description, for store UI purposes. |
| `withProductType` | `String` | Unity supports _"Consumable"_, _"Non-consumable"_, and _"Subscription"_ Product Types. |

Use the following functions to retrieve Product properties:

| **Method** | **Description** |
| ---------- | --------------- |
| `public String getProductId ();` | Retrive the Product ID. |
| `public String getLocalizedTitle ();` | Retrive the localized Product title. |
| `public String getLocalizedPriceString ();` | Retrive the localized Product price string. |
| `public double getLocalizedPrice ();` | Retrive the localized Product price value. |
| `public String getIsoCurrencyCode ();` | Retrive the Product price's currency code. |
| `public String getLocalizedDescription ();` | Retrive the localized Product description. |
| `public String getProductType ();` | Retrive the Product type. |

For more details on Product properties, see documentation on [Defining Products](https://docs.unity3d.com/Manual/UnityIAPDefiningProducts.html).

#### TransactionDetails
An IAP transaction receipt converted into an object that the SDK can use for optimizing monetization. 

Set the following fields on the builder using the `public static Builder newBuilder ()` method, then the `public TransactionDetails build ()` method to construct the final Product object.

| **Builder method** | **Param type** | **Description** |
| ------------------ | -------------- | --------------- |
| `withProductId` | `String` | An internal reference ID for the Product. |
| `withTransactionId` | `String` | An internal reference ID for the transaction. |
| `withReceipt` | `String` | The JSON fields from the [`INAPP_PURCHASE_DATA`](https://developer.android.com/google/play/billing/billing_reference) detailing the transaction. Encode the receipt as a JSON object containing Store, TransactionID, and Payload. |
| `withPrice` | `BigDecimal` | The internal system value for the Product’s price. |
| `withIsoCurrency` | `String` | The [ISO code](https://en.wikipedia.org/wiki/ISO_4217) for the Product’s localized currency. |

Use the following functions to retrieve Product properties:

`public String getProductId ();`
`public String getTransactionId ();`
`public String getReceipt ();`
`public BigDecimal getPrice ();`
`public String getCurrency ();`

### Enums
#### TransactionError
The enumerated reasons for a transaction failure.

```
public enum TransactionError {
    ITEM_UNAVAILABLE
    NETWORK_ERROR
    NOT_SUPPORTED
    SERVER_ERROR
    UNKNOWN_ERROR
    USER_CANCELLED
}
```

#### NativePromoShowType
The enumerated display types of a [Native Promo](MonetizationNativePromoAndroid.md) asset.

| **Value** | **Description** |
| --------- | --------------- |
| `FULL` | Indicates a full promotional view. | 
| `PREVIEW` | Indicates a minimized view that can expand to display the full Promo. |
