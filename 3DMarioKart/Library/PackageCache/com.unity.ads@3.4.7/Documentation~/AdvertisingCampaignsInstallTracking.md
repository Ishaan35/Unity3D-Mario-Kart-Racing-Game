# Server to server install tracking
## Overview
To help launch CPI campaigns in Unity Ads, Unity allow developers to send install tracking information via a server-to-server integration, so the advertised applications do not need to be updated to include install tracking specifically for Unity Ads. This document instructs you to integrate a third party install tracking service or your own application servers to notify Unity Ads of new installs in your iOS and Android games.

## Third part attribution providers
A third party or in-house attribution service provider is needed to correctly run campaigns in Unity Ads. The following links refer to some common providers:

[**Kochava**](https://www.kochava.com/) (preferred Unity [Asset store partner](https://assetstore.unity.com/packages/add-ons/services/analytics/kochava-attribution-analytics-130522))
* [Create an install campaign](https://support.kochava.com/campaign-management/create-an-install-campaign)
* [Kochava support](https://support.kochava.com)

[**AppsFlyer**](https://www.appsflyer.com/)
* [Unity Ads integration](https://support.appsflyer.com/hc/en-us/sections/201695093-AppsFlyer-Ad-Network-Integration)
* [AppsFlyer support](https://support.appsflyer.com) 

[**Adjust**](https://www.adjust.com/)
* [Unity Ads integration](https://docs.adjust.com/en/special-partners/unity-ads/)
* [Adjust tracker generation](https://docs.adjust.com/en/tracker-generation/)

[**Branch**](https://branch.io) (formerly Tune)
* [Unity install tracking guide](https://docs.branch.io/deep-linked-ads/unity-mobile-tracking/)

## Winner-only transmission install tracking
If you already use an external mobile install tracking service, such as [Kochava](https://www.kochava.com/unity/) (a preferred Unity [Asset store partner](https://assetstore.unity.com/packages/add-ons/services/analytics/kochava-attribution-analytics-130522)), [AppsFlyer](https://www.appsflyer.com/), [Adjust](https://www.adjust.com/), or [Mobile App Tracking by Tune](https://www.tune.com/), you can easily configure an [attribution tracking link](AdvertisingCampaignsConfiguration.md#attribution-links) in Unity Ads to notify your install tracking service of the views (or clicks), then configure a Unity Ads postback URL in your install tracking service to notify Unity Ads about the install conversions.

## Tracking URLs
The [Acquire dashboard](https://acquire.dashboard.unity3d.com/) lets you define a custom tracking URL for your campaigns. This URL reports the user’s identification at the time of impressions (or clicks) from the campaign to your install tracking service. The service then uses this information to attribute any subsequent installs to the correct ad network and make a callback to that network with the proper user details.

### URL requirements
Make sure all your tracking URLs comply with the following requirements, otherwise the attribution will not work correctly.

* The URL and any redirection uses [HTTPS](https://en.wikipedia.org/wiki/HTTPS).
* The URL contains the `{ifa}` dynamic custom token (see below).
* HTTP Redirections are executed via [HTTP 3XX codes](https://www.w3.org/Protocols/HTTP/HTRESP.html), not HTML or Javascript.
* The URL does not redirect to the Apple Store or Google Play. 
 
If you use a tracking provider that does not support HTTPS, please [contact us](mailto:adops-support@unity3d.com) for assistance. 

### Dynamic custom tracking URL tokens
The following dynamically replaced tokens are available in the Tracking URL:

| **Token** | **Description** | **Example** |
| --------- | --------------- | ----------- |
| `{ifa}` (on iOS) | The iOS [Identifier for Advertising (IDFA)](https://developer.apple.com/library/ios/#documentation/AdSupport/Reference/ASIdentifierManager_Ref/ASIdentifierManager.html) in plain text in its original uppercase form. |`1234ABCD-1234-5678-ABCD-1A2B3C4D5E6F` |
| `{ifa}` (on Android) | The [Google Advertising ID](https://developer.android.com/google/play-services/id.html) in its original lowercase form. This will replace the Android ID as the primary identification method on Android. During the transition phase, both identifiers need to run in parallel. | `1234ABCD-1234-5678-ABCD-1A2B3C4D5E6F` |
| `{ifa_md5}` (on iOS) | The iOS [Identifier for Advertising (IDFA)](https://developer.apple.com/library/ios/#documentation/AdSupport/Reference/ASIdentifierManager_Ref/ASIdentifierManager.html) MD5-hashed from its original uppercase form. | `1234567890ABCDEFGHIJK123456ABCDEF` |
| `{ifa_md5}` (on Android) | The [Google Advertising ID](https://developer.android.com/google/play-services/id.html) MD5-hashed from its original lowercase form. | `1234567890ABCDEFGHIJK123456ABCDEF` |
| `{android_id_md5}` | The MD5 hashed [Android ID](https://developer.android.com/reference/android/provider/Settings.Secure.html#ANDROID_ID) of the Android devices, in MD5 hashed form. This should only be used when the Google Advertising ID `{ifa}` is not available, such as instances when Google Play Services is not installed on the Android device. | MD5 `12345678ABCDEFGH` = `123ABC456DEF789GHI012JKL345MNO67890` |
| `{ip}` | The IP address of the user. This is provided for informational purposes only, and is not suitable for user identification in install tracking. | `123.123.123.123` |
| `{country_code}` | The user’s ISO 3166-1 alpha-2 country code in lower case.<br><br>**Note**: `"GB"` indicates the United Kingdom of Great Britain and Northern Ireland. | `GB` |
| `{campaign_id}` | The Unity Ads campaign ID. | `12345678abcdefgh9012ijkl` |
| `{campaign_name}` | The Unity Ads campaign name. | `my_ad_campaign` |
| `{game_id}` | The Unity Ads Game ID of the advertised game. | `1234567` |
| `{source_game_id}` | The Unity Ads Game ID of the game showing the ad. | `7654321` |
| `{os}` | The operating system of the device. | <ul><li>`9.2.1` (iOS)</li><li>`4.4.0` (Android)</li></ul> |
| `{device_type}` | The device type. | <ul><li>`iPad4,1`</li><li>`motorola XT1254`</li><li>`samsung SM-G900F`</li></ul> |
| `{creative_pack}` | The name of the creative pack used in the ad. | `Video Creatives Pack - EN - 15s` |
| `{creative_pack_id}` | The unique identifier for the creative pack used in the ad. | `5beafbce74ed83001acb258c` |
| `{language}` | The device language. | `en-GB` |
| `{user_agent}` | The device user agent. | <ul><li>`Mozilla/5.0` (iPhone; CPU iPhone OS 9_3_5 like Mac OS X)</li><li>`AppleWebKit/601.1.46` (KHTML, like Gecko)</li><li>`Mobile/13G36` (iOS)</li><li> `Mozilla/5.0` (Linux; Android 6.0.1; SM-G920V Build/MMB29K)</li><li>`AppleWebKit/537.36` (KHTML, like Gecko)</li><li>`Chrome/52.0.2743.98 Mobile Safari/537.36` (Android)</li></ul> |
| `{device_make}` | The device maker. | <ul><li>`Apple` (iOS)</li><li>`samsung` (Android)</li></ul> |
| `{device_model}` | The device model. | <ul><li>`iPhone7,2` (iOS)</li><li>`SM-G900F` (Android)</li></ul> |
| `{cpi}` | The cost per install in U.S. dollars. | `2.85` |
| `{video_orientation}` | The orientation of the creative shown in the ad. | <ul><li>`portrait`</li><li>`landscape`</li></ul> |
| `{screen_size}` | The screen size based on the targeting available in the [Acquire dashboard](https://acquire.dashboard.unity3d.com/).<br><br>**Note**: Screen size is only available for Android devices. | <ul><li>`small`</li><li>`normal`</li><li>`large`</li><li>`xlarge`</li></ul> |
| `{screen_density}` | Only available on Android devices. The screen density based on the targeting available in the [Acquire dashboard](https://acquire.dashboard.unity3d.com/).<br><br>**Note**: Screen size is only available for Android devices. | <ul><li>`ldpi`</li><li>`mdpi`</li><li>`hdpi`</li><li>`xhdpi`</li><li>`xxhdpi`</li><li>`xxxhdpi`</li></ul> |

As an example, the following AppsFlyer tracking URL:

```
https://app.appsflyer.com/id1234567890?idfa={ifa}&c={campaign_name}&af_sub2={source_game_id}&redirect=false
```

might appear as:

```
https://app.appsflyer.com/id1234567890?idfa=1234ABCD-1234-5678-ABCD-1A2B3C4D5E6F&c=Unity_Android_USA_Target&af_sub2=33333&redirect=false
```

## Server responses to Impressions or Clicks
The tracking URL can fire from an impression (when the user watches an ad) or a click (when the user clicks the download link from the ad). In both cases, the URL should not redirect to the Apple App Store or Google play, and the server should respond with an `HTTP 200 OK` message. Unity Ads will load the respective store page in an app sheet to avoid directing the player outside of the game.

## Postback URL request
Converted users who install the advertised game as a result of the campaign are reported via a Postback URL. You can retrieve this reporting with HTTP GET requests. 

For iOS, use the following URL:

```
https://postback.unityads.unity3d.com/games/[GAME_ID]/install?advertisingTrackingId=[YOUR_MACRO_FOR_IDFA]
```

For Android, use the following URL:

```
https://postback.unityads.unity3d.com/games/[GAME_ID]/install?advertisingTrackingId=[YOUR_MACRO_FOR_GOOGLE_AD_ID]
```

The `GAME_ID` parameter is your Unity Ads [Game ID](MonetizationResourcesDashboardGuide.md#game-ids). You can find this ID by logging into the [Acquire dashboard](https://acquire.dashboard.unity3d.com/) and selecting **Campaigns**. 

**Note**: For existing users, the legacy [impact.applifier.com](impact.applifier.com) domain will continue working in parallel with the new [postback.unityads.unity3d.com](postback.unityads.unity3d.com) domain. However, Unity encourages you to migrate to the new one. 

#### Postback URL parameters
The following identification parameters must be relayed in the Postback URL request:

| **Parameter** | **Description** | **Example** |
| ------------- | --------------- | ----------- |
| `advertisingTrackingId` (iOS) | The [Identifier for Advertising](https://developer.apple.com/library/ios/#documentation/AdSupport/Reference/ASIdentifierManager_Ref/ASIdentifierManager.html) (IDFA) in uppercase form. This is compulsory for all installs (either raw form or MD5 hashed form). | `XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX` |
| `advertisingTrackingIdMD5` (iOS) | The [Identifier for Advertising](https://developer.apple.com/library/ios/#documentation/AdSupport/Reference/ASIdentifierManager_Ref/ASIdentifierManager.html) (IDFA) in MD5 hashed, lowercase form. This is compulsory for all installs (either raw form or MD5 hashed form). | |
| `advertisingTrackingId` (Android) | The [Google Advertising ID](https://developer.android.com/google/play-services/id.html) in lowercase form.  This is compulsory for all installs (either raw form or MD5 hashed form). | `XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX` |
| `advertisingTrackingIdMD5` (Android)| The [Google Advertising ID](https://developer.android.com/google/play-services/id.html) in MD5 hashed, lowercase form. This is compulsory for all installs (either raw form or MD5 hashed form). | |
| `gamerId` | The unique Unity Ads identifier used for attributing users that have Limited Ad Tracking (LAT) on, where the value of the advertising identifier is `00000000-0000-0000-0000-000000000000`. This is mandatory when attributing iOS fingerprinted users. | `58c116080dbe250047a2a398` |
| `rawAndroidId` | The [Android ID](https://developer.android.com/reference/android/provider/Settings.Secure.html#ANDROID_ID) in its original lowercase form.<br><br>**Note**: This is not recommended, as it is not required if the Android device has correctly integrated Google Play Services and has Google Play installed. It is, however, compulsory for all Android installs which don't have a Google Advertising ID. | |
| `androidId` | The [Android ID](https://developer.android.com/reference/android/provider/Settings.Secure.html#ANDROID_ID) in MD5 hashed form.<br><br>**Note**: This is not recommended, as it is not required if the Android device has correctly integrated Google Play Services and has Google Play installed. It is, however, compulsory for all Android installs which don't have a Google Advertising ID. | |
| `attributed` | A flag denoting whether this install is attributed to Unity Ads and can be charged. The default value (`attributed=1`) indicates that the condition is true. If the condition is false (`attributed=0`), the install will only be marked to the player, and will not be charged.<br><br>**Note**: this parameter should only used if using Blanket Transmission, when all installs are sent instead of just attributed. | <ul><li>`attributed=1`</li><li>`attributed=0`</li></ul> |

#### Postback URL response format
Responses from Unity Ads' install tracking server are output as JSON files. If the message was received successfully, the server responds with a `status` field, with a value of `ok`. The server also always outputs its HTTP response code.

The response includes a parameter `"install":true` if the postback initiated the successful conversion of a chargeable install. If this was not a valid conversion (for example, if the user had already installed the game or the lookback window has passed), the response reads `"install":false`.

In addition, successful conversions include the source game ID in a `sourceGame` parameter. You can use this parameter to block certain source games or publishers, based on the quality of traffic they produce. If the conversion was not successful, the `sourceGame` parameter is omitted.

For example:

```
{"install":true,"sourceGame":"11007","responseCode":200,"status":"ok"}
```

If the `status` field is not present, any available errors are available in the `error` field.

**Note**: The status code does not indicate whether this postback call was actually recorded as a chargeable install. It onlys states that the message was successfully received and processed. For the chargeable install, use the `"install":true` field instead.