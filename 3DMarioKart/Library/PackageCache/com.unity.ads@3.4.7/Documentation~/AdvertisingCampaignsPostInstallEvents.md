# Passing post-install events to Unity
## Secondary conversion endpoint integration
Event parameters help refine Unity’s machine learning algorithm and improve campaign performance. [Audience Pinpointer campaigns](AdvertisingOptimizationAudiencePinpointer.md) require session event data for retention optimization campaigns, and purchase event data for revenue optimization campaigns (for more information, see the **Required Audience Pinpointer event parameters** section below). Implement these post-install event callbacks through your third-party attribution partner.

### Required Audience Pinpointer event parameters
You must include the following components with your event query for Audience Pinpointer campaigns.

| **Component** | **Parameter** |
| ------------- | ------------- |
| Base URL | `https://ads-secondary-conversion.unityads.unity3d.com/v1/events` |
| User identification | One of the following: <br><br> <ul><li>`ifa={iOS Identifier for advertising in iOS}` (required for ios; also accepts idfa)</li> <li>`aid={Google Play Advertising Identifier}` (required for android)</li> <li>`android_id_md5={md5 hash of lowercase android id}`</li> <li>`gamer_id={Unity Ads gamer id}` (if available)</li></ul> **Note**: Both iOS and Android accept advertising IDs in uppercase or lowercase, but iOS converts to uppercase, while Android converts to lowercase. |
| Ad tracking setting | <code>tracking_enabled={1&#124;0}</code> or <code>tracking_limited={1&#124;0}</code> |
| Game identification | `game_id={Unity Ads Game ID}` (required; locate this on the main page of the [advertising dashboard](https://acquire.dashboard.unity3d.com), listed under your game’s title) <br><br> Plus one of the following: <br><br> <ul><li><code>platform={ios&#124;android}&store_id={itunes id&#124;googleplay id}</code></li> <li><code>platform={ios&#124;android}&bundle_id={bundle id}</code></li> <li><code>platform={ios&#124;android}&project_id={Unity Project ID}</code></li></ul> Accepted values for iOS include `ios`, `itunes`, `iOS`, and `iTunes`. <br><br> Accepted values for Android include `android`, `google`, `Android`, `Google`, `gplay`, and `GooglePlay`. |
| Original install timestamp | `install_ts={unix time in seconds or milliseconds}` |
| Event identification | Identify which event was triggered by the user: <br><br> `event={event name}`, where the event name is one of the following: <ul><li>`session` (user started a gameplay session; **required** for retention optimization campaigns)</li> <li>`purchase` (user purchased something via IAP) and `value={amount}&currency={currency}` (both **required** for revenue optimization campaigns. Amount must be in decimal point, for example `1.23`)</li></ul> |
| Original conversion attributed? | Indicate whether the original conversion of this user has been attributed to this network or not: <br><br> `was_conversion_attributed=1` |

### Optional event parameters
Include the following optional components with your event query to help improve campaign performance.

| **Component** | **Parameter** |
| ------------- | ------------- |
| Timestamp | `ts={unix time in seconds or milliseconds}` <br><br> When the event happened. By default, the event happened at (or near) the time it was fired to the endpoint. |
| Event identification | If the event is not a session or purchase event: <br><br> `event={event name}`, where the event name is one of the following: <ul><li>`registration` (user registered to the app)</li> <li>`spent_credits` (user spent virtual currency to buy in-game items), along with `value={how many credits}` (optional) and `type={what type credits}` (optional)</li> <li>`invite` (user invited friends to the game)</li> <li>`share` (user shared the game with friends)</li> <li>`level_complete` (user completed a level), along with `value={level name}` (optional)</li> <li>`tutorial_complete` (user completed the tutorial)</li> <li>`achievement_unlocked` (user unlocked an achievement)</li> <li>`custom` (a custom event), along with `custom_event={custom_event_name}` and `value={value}&type={type}` (optional)</li></ul> |
| Test mode | `test=1` <br><br> This is a test event being fired to the service. It does not count as a real event from end users. |
| Tracking partner | `tracking_partner={tracking service name or other string to identify attribution provider}` |
| App name | `app_name=Game+Name` |
| Advertiser name | `advertiser_name=Advertiser+Name+In+Here` |
| Original impression timestamp | `impression_ts=1234567890` <br><br> Formatted as a Unix timestamp. |
| Original click timestamp | `click_ts=1234567890` <br><br> Formatted as a Unix timestamp. |
| IP address | `ip=123.456.78.90` |
| Ad network | `ad_network=UnityAds` |
| Country | `country=GB` |
| App version | `app_version=1.2.3` |
| Device type | Device model string: <br><br> `device_type=iPhone6,2` <br><br> or <br><br> `device_type=samsung+GT-S7582` | 
| OS version | `os_version=9.3.4` |
| Language | `language=en_US` or `language=en` |

#### Session event example
`https://ads-secondary-conversion.unityads.unity3d.com/v1/events?ifa=AE06DF78-CA5A-46C3-BD44-1D3B6AA4D6E9&tracking_enabled=1&game_id=UNITY_ADS_GAME_ID&platform=ios&store_id=123456789&event=session&ts=1466637860812&install_ts=1466637800175&was_conversion_attributed=1`

#### Purchase event example
`https://ads-secondary-conversion.unityads.unity3d.com/v1/events?ifa=AE06DF78-CA5A-46C3-BD44-1D3B6AA4D6E9&tracking_enabled=1&game_id=UNITY_ADS_GAME_ID&platform=ios&store_id=123456789&event=purchase&value=1.0&currency=USD&ts=1466637860812&install_ts=1466637800175&was_conversion_attributed=1`