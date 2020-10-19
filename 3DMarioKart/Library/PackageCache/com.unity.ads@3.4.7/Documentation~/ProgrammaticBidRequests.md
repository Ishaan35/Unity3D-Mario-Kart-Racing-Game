# Bid requests
## Overview
When Unity receives an ad request, Unified Auction sends a bid request to all potential bidders, including data utilized for personalized ads in cases where the user does not restrict its use (see documentation on [data privacy](ProgrammaticResourcesDataPrivacy.md)). 

**Note**: Your endpoint must support HTTPS protocol in order to receive bid requests. 

Unity supports JavaScript Object Notation (JSON) formats for bid request data. The mime type for the standard JSON representation is `application/json` and specified in an HTTP header field as: 

```
contentType: application/json
``` 
 
Unity sends Accept-Encoding gzip compression in the request, and highly recommends compressing the response. For more information, see section **2.4 Data Encoding** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).  

**Note**: Unity passes each of the following bid response attributes when available, unless otherwise noted in the description column.  

## request objects 
An object class that describes a bid request. 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- |
| `id` | string | `"id": "bcaIDT1Nbv0zU8mn9tXQ6j"` | A Unity-generated ID. This ID must be returned in the [bid Response object](#bid-responses). |
| `pmp` | object | For more information, see section on [`pmp` objects](#pmp-objects). | Private marketplace container for direct deals between buyers and sellers that may pertain to the impression. The actual deals are represented as a collection of [`deal` objects](#deal-objects). |
| `imp` | object | For more information, see section on [`impression` objects](#impression-objects). | Unity supports one `impression` object per bid request. |
| `at` | int | `"at": 2` | The auction type used; all Unified Auctions are second-price auctions, so the value is always `2`. |
| `app` | object | For more information, see section on [`app` objects](#app-objects). | The `app` object describing the game. |
| `device` | object | For more information, see section on [`device` objects](#device-objects). | The `device` object describing the user's device. |
| `tmax` | int | `"tmax": 200` | Timeout value in milliseconds.<br><br>**Note**: We suggest partners respond within 200 ms to avoid the network connection dropping. |
| `bapp` | string array | A list of blocked applications by their platform-specific exchange independent application identifiers. On Android, these should be bundle or package names (for example, `com.foo.mygame`). On iOS, these are numeric IDs.<br><br>**Note**: Please contact Unity if you would like this attribute passed. |
| `bcat` | string array | `"bcat": ["IAB9-9", "IAB8"]` | Blocked advertiser categories, using [IAB taxonomy IDs](ProgrammaticResourcesCategoryMapping.md).<br><br>**Note**: This attribute is supported but optional. |
| `badv` | string array | `"badv": ["blockedAdvertiser.com"]` | Blocked list of advertisers, using their domains.<br><br>**Note**: This attribute is supported but optional. |
| `source` | object | For more information, see section on [`source` objects](#source-objects). | A source object that provides data about the inventory source and which entity makes the final decision.|
| `regs` | object | For more information, see documentation on [data privacy classes](ProgrammaticResourcesDataPrivacy.md#regs-objects-and-extensions). | An object that specifies any industry, legal, or governmental regulations enforced for this request. |
| `user` | object | For more information, see documentation on [data privacy classes](ProgrammaticResourcesDataPrivacy.md#user-object-extension). | An object representing the human user of the device, or audience of the ad. |

## source objects 
An object class that describes the nature and behavior of the entity that is the source of the bid request upstream from the exchange. The primary purpose of this object is to define post-auction or upstream decision-making when the exchange itself does not control the final decision. For more information, see the **Object: source** section of the [OpenRTB guidelines](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).

**Note**: Unity supports Open Measurement-capable creatives. [IAB’s Open Measurement SDK](https://s3-us-west-2.amazonaws.com/omsdk-files/docs/Open+Measurement+SDK+Onboarding_version_1.2.pdf) (OM SDK) offers common code and libraries for facilitating third-party access to measurement data. Sites and apps that integrate the OM SDK can send measurement signals to an Open Measurement Interface Definition (OMID) API. Measurement providers can place tags that collect these signals. In order to ingest and act on bid requests that support OM creatives, Unity has updated its OpenRTB specs to reflect the related changes to the bid request fields (see also the [`imp.video.api`](#video-objects) field).

| **Attribute** | **Type** | **Example** | **Description** |
|:---|:---|:---|:---| 
| `pchain ` | string | `"pchain": "12abcd3456789:12345"` | A payment ID chain string containing embedded syntax described in the TAG Payment ID Protocol v1.0.|
| `omidpn` | string | `"omidpn": "Unity3d"` | An identifier for the Open Measurement SDK integration. This is the same as the `name` parameter of the OMID Partner object. |
| `omidpv` | string | `"omidpv": "1.2.10"` | The version of the Open Measurement SDK integration. This is the same as the `versionString` parameter of the OMID Partner object. |

## pmp objects 
An object class that describes private marketplace attributes for direct deals between buyers and sellers that may pertain to the impression. For more information, see section **3.2.11 Object: Pmp** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).  

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- | 
| `private_auction` | int | `"private_auction": 0` | Indicator of auction eligibility to seats named in the [`deal` objects](#deal-objects):<br><br><ul><li>`0` (default; all bids are accepted)</li><li>`1` (bids are restricted to the deals specified and the terms therein)</li></ul> |
| `deals` | object array | For more information, see section on [`deal` objects](#deal-objects). | An array of `deal` objects that convey the specific deals applicable to this impression. |

## deal objects 
An object class that describes a specific deal that was established a priori between a buyer and seller. Its presence indicates that the impression is available under the terms of that deal. For more information, see section **3.2.12 Object: Deal** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).  

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- | 
| `id` | string | `"id": "abcdef12345"` | A unique identifier for the direct deal. |
| `at` | int | `"at": 3` | Optional override of the auction type of the bid request:<br><br><ul><li>`1` (first price)</li><li>`2` (second price plus)</li><li>`3` (the value passed in `bidfloor` is the agreed upon deal price).</li></ul> |
| `wadomain` | string array | `"wadomain": ["advertiser1.com", "advertiser2.com"]` | Array of advertiser domains allowed to bid on this deal. Omission implies no advertiser restrictions. |

## impression objects 
An object class that describes an ad impression. 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- | 
| `id` | string | `"id": "1"` | Unity supports one `impression` object per bid request; the `id` value is always `1`. |
| `video` | object | For more information, see section on [`video` objects](#video-objects). | The `video` object describes a video impression.<br><br>**Note**: This attribute is always passed for video ads only. |
| `banner` | object | For more information, see section on [`banner` objects](#banner-objects). | The `banner` object describes a banner display impression.<br><br>**Note**: This attribute is only passed for banner and display ads. |
| `instl` | int | `"instl": 1` | Banner ad requests have a value of `0`; all other Placements are interstitials, with a  value of `1`. For more information, see documentation on [Placements](MonetizationPlacements.md). |
| `secure` | int | `"secure": 1` | Unity requires impressions to have secure `HTTPS URL` creative assets and markup; the value is always `1`. |
| `tagid` | string | `"tagid": "15939-video"` | Unity passes `<appID>-<adType>` as the identifier. |
| `displaymanager` | string | `"displaymanager": "unity sdk"` | The name of the ad mediation partner, SDK technology, or player responsible for rendering the ad. |
| `displaymanagerver` | string | `3001` | The version of the ad mediation partner, SDK technology, or player responsible for rendering the ad. |


### video objects 
A subset of the `impression` object class that describes a video ad impression. For more information, see section **3.2.7 Object: Video** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).  

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- |
| `api` | int array | `"api": [7]` | Supported API frameworks for this impression. A value of `7` signifies Open Measurement support. | 
| `mimes` | string array | `"Mimes": ["video/mp4"]` | Content mime types supported; this value is currently always `["video/mp4"]`. |
| `minduration` | int | `"minduration": 5` | Minimum video ad duration in seconds; this value is always `5`. |
| `maxduration` | int | `"maxduration": 30` | Maximum video ad duration in seconds; this value is always `30`. |
| `w` | int | `"w": 1024` | Width of the video player in device independent pixels ([DIPS](https://en.wikipedia.org/wiki/Device-independent_pixel)). |
| `h` | int | `"h": 600` | Height of the video player in device independent pixels ([DIPS](https://en.wikipedia.org/wiki/Device-independent_pixel)). |
| `pos` | int | `"pos": 7` | Video position; Unity Ads inventory is always set to full screen (`7`). For a complete list of supported values, see table **5.4 Video Position** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf). |
| `protocols` | int array | `"protocols": [2, 3, 5, 6]` | Array of supported video protocols. For a complete list of supported values, see table **5.8 Protocols** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).<br><br>**Note**: Unity fully supports VAST 2.0 (`2`) and VAST 2.0 Wrapper (`5`), and may support some features of VAST 3.0 (`3`) and VAST 3.0 Wrapper (`6`). Please check with your account manager for details. |
| `battr` | int array | `"battr": [ 1, 3, 5, 6, 8, 9, 13]` | Blocked creative attributes. For a complete list of supported values, see table **5.3 Creative Attributes** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).<br><br>**Note**: Unity’s policy is to block the following by default:<br><br><ul><li>`[1]` Audio Ad (auto-play)</li><li>`[3]` Expandable (automatic)</li><li>`[5]` Expandable (user initiated - rollover)</li><li>`[6]` In-Banner video ad (auto-play)</li><li>`[8]` Pop (e.g., Over, Under, or Upon Exit)</li><li>`[9]` Provocative or Suggestive Imagery</li><li>`[13]` User Interactive (e.g. embedded games)</li></ul> |

### banner objects 
A subset of the `impression` object class that describes a banner ad impression. For more information, see section **3.2.6 Object: Banner** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).  

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- |
| `w` | int | `"w": 480` | Width of the banner display in device independent pixels ([DIPS](https://en.wikipedia.org/wiki/Device-independent_pixel)). |
| `h` | int | `"h": 320`  | Height of the banner display in device independent pixels ([DIPS](https://en.wikipedia.org/wiki/Device-independent_pixel)). |
| `format` | object array | `"format": [{"w": 768, "h": 1024}]` | Array of `format` objects representing the banner display sizes permitted. Supported sizes:<br><br><ul><li>250x300</li><li>300x250</li><li>320x480</li><li>480x320</li><li>768x1024</li><li>1024x768</li></ul>**Note**: If no banner format is specified, the dimensions default to the `h` and `w` values. If you have questions about supporting different dimensions, please contact Unity. |
| `pos` | int | `"pos": 7` | Ad position. For a complete list of supported values, see table **5.4 Ad Position** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf). |
| `api` | int array | `"api": [5]` | API frameworks supported by the publisher; Unity supports MRAID 2.0 (`5`). For a complete list of supported values, see table **5.6 API Frameworks** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf). |
| `battr` | int array | `"battr": [ 1, 3, 5, 6, 8, 9]` | Blocked creative attributes. For a complete list of supported values, see table **5.3 Creative Attributes** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).<br><br>**Note**: Unity’s policy is to block the following by default:<br><br><ul><li>`[1]` Audio Ad (auto-play)</li><li>`[3]` Expandable (automatic)</li><li>`[5]` Expandable (user initiated - rollover)</li><li>`[6]` In-Banner video ad (auto-play)</li><li>`[8]` Pop (e.g., Over, Under, or Upon Exit)</li><li>`[9]` Provocative or Suggestive Imagery</li></ul> |

## app objects 
An object class that describes the app making the ad request. For more information, see section **3.2.14 Object: App** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).  

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- |
| `id` | string | `"id": "a1b2c3d4e5f6g7h8i9j0123456789abc"` | Exchange-specific application ID. |
| `bundle` | string | Google example:<br><br>`"bundle": "com.unityexample.game"`<br><br>Apple example:<br><br>`"1198634425"` | Application bundle or package name, intended to be unique across exchanges.<br><br><ul><li>**Android** apps pass the Bundle ID.</li><li>**iOS** apps pass the Bundle ID or Store ID.</li></ul>**Note**: This attribute may not be passed in circumstances where contractual agreements with the publisher prohibit it. |
| `name` | string | `"name": "Trash Dash"` | App name.<br><br>**Note**: This attribute may not be passed in circumstances where contractual agreements with the publisher prohibit it. |
| `publisher` | string | `"publisher": "ExamplePublisher"` | An exchange-specific publisher ID. |
| `storeurl` | string | `"storeurl": "https://itunes.apple.com/us/app/trash-dash/id1198634425?mt=8"` | App store URL for an installed app (for [QAG](https://archive.iab.com/iab.atlasworks.com/QAGInitiative/overview.html) 1.5 compliance).<br><br>**Note**: This attribute is supported but optional. |

## device objects 
An object class that describes the end user’s device. For more information, see section **3.2.18 Object: Device** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).  

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- |
| `ifa` | string | `"ifa": "ab12c456-78de-90f1-ghi2-j3kl4567890m"` | The Android Advertising ID (AID) or iOS Identifier for Advertisers (IFA/IDFA) sanctioned for advertiser use in the clear (not hashed).<br><br>**Note**: This field is cleared where required by [COPPA or GDPR compliance](ProgrammaticResourcesDataPrivacy.md). |
| `make` | string | `"make": "Apple"` | Device make. |
| `model` | string | `"model": "iPhone7,2"` | Device model. |
| `ua` | string | `"ua": "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Mobile/14A345"` | Web browser user agent string. |
| `lmt` | int | `"lmt": 0` | "Limit Ad Tracking" signal commercially endorsed (e.g., iOS or Android).<br><br><ul><li>`0` indicates tracking is unrestricted.</li><li>`1` indicates tracking must be limited per commercial guidelines. This attribute is always passed when set to `1`. |
| `ip` | string | `"ip": "12.34.5.6"` | Device IP address.<br><br>**Note**: This field is masked where required by [COPPA or GDPR compliance](ProgrammaticResourcesDataPrivacy.md). |
| `language` | string | `"language": "en"` | Browser language, using [ISO-639-1-alpha-2](https://en.wikipedia.org/wiki/ISO_639-1) codes. |
| `connectiontype` | int | `"connectiontype": 1` | Network connection type. For a complete list of supported values, see table **5.22 Connection Type** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).<br><br>**Note**: This attribute is supported but optional. |
| `geo` | object | For more information, see section on [`geo` objects](#geo-objects). | Location of the device (assumed to be the user’s current location). |
| `devicetype` | int | `"devicetype": 4` | The general type of device. For a complete list of supported values, see table **5.21 Device Type** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).<br><br>Please note that separation between phone and tablet was introduced in in OpenRTB version 2.2:<br><br><ul><li>`1` indicates a Mobile or Tablet device.</li><li>`4` specifies a phone.</li><li>`5` specifies a tablet.</li></ul>To enforce precision, Unity uses `4` and `5` whenever possible. When mixed device types do occur, please choose one or the other. |
| `os` | string | `"os": "ios"` | Device operating system. |
| `osv` | string | `"osv": "3.1.2"` | Device operating system version. |
| `hwv` | string | `"hwv": "samsung SM-J200H"` | Hardware version of the device. |

## geo objects 
An object class that describes the device’s geographic location. For more information, see section **3.2.19 Object: Geo** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf). 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- |
| `lat` | float | `"lat": 50.2981` | Latitude from `-90.0` (south) to `90.0` (north).<br><br>**Note**: This field is cleared where required by [COPPA or GDPR compliance](ProgrammaticResourcesDataPrivacy.md). |
| `lon` | float | `"lon": 57.1814` | Longitude from `-90.0` (west) to `90.0` (east).<br><br>**Note**: This field is cleared where required by [COPPA or GDPR compliance](ProgrammaticResourcesDataPrivacy.md). |
| `type` | int | `"type": 2` | Source of location data. Unity derives location information from the device’s IP Address (`2`).<br><br>**Note**: This field is cleared where required by [COPPA or GDPR compliance](ProgrammaticResourcesDataPrivacy.md). |
| `country` | string | `"country": "USA"` | Country, using [ISO-3166-1-alpha-3](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-3) codes. |
| `city` | string | `"city": "San Francisco"` | City, using United Nations Code for Trade & Transport Locations. See **Appendix A** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf) for a link to the codes.<br><br>**Note**: This field is cleared where required by [COPPA or GDPR compliance](ProgrammaticResourcesDataPrivacy.md).|
| `utcoffset` | int | `"utcoffset": -480` | Local time, expressed as the number of +/- minutes from UTC.<br><br>**Note**: This field is cleared where required by [COPPA or GDPR compliance](ProgrammaticResourcesDataPrivacy.md). |

## Example bid requests 
### VAST bid request 
```
{ 
    "id": "bcaIDT1Nbv0zU8mn9tXQ6j", 
    "at": 2, 
    "tmax": 200, 
    "source": {
        "pchain": "12abcd3456789:12345"
        "omidpn": "Unity3d",
        "omidpv": "1.2.10"
    },
    "imp": [{ 
        "id": "1", 
        "secure": 1, 
        "instl": 1, 
        "tagid": "15939-video", 
        "video": { 
            "mimes": [
                "video/mp4" 
            ], 
            "minduration": 15, 
            "maxduration": 30, 
            "protocols": [2, 3, 5, 6], 
            "pos": 7, 
            "battr": [1, 3, 5, 6, 8, 9, 13], 
            "w": 1024, 
            "h": 600,
            "api": [7] 
        } 
    }], 
    "app": { 
        "id": "1", 
        "bundle": "com.unity3d.ads.example", 
        "name": "Unity Mobile Game", 
        "storeurl": "https://itunes.apple.com/us/app/unitygame/id12345678?mt=8" 
    }, 
    "device": { 
        "ifa": "ab12c456-78de-90f1-ghi2-j3kl4567890m", 
        "make": "Apple", 
        "model": "iPhone", 
        "lmt": 0, 
        "ua": "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Mobile/14A345", 
        "os": "ios", 
        "ip": "12.34.5.6", 
        "devicetype": 4, 
        "carrier": "Apple", 
        "osv": "10.1", 
        "hwv": "iphone6", 
        "geo": { 
            "lat": 55.5492, 
            "lon": 59.0456, 
            "country": "USA", 
            "city": "San Francisco", 
            "type": 2, 
            "utcoffset": 180 
        }, 
        "connectiontype": 3, 
        "language": "en", 
        "h": 1184, 
        "w": 768 
    }, 
    "regs": { 
        "coppa": 0 
    }, 
    "ext": {} 
}
``` 

### MRAID (playable) bid request 
```
{ 
    "id": "abcIDT1Def0zU8mn9gHI6j", 
    "source": {
        "pchain": "12abcd3456789:12345"
    },
    "imp": [{ 
        "id": "1", 
     	"banner": { 
    	    "w": 1024, 
       	    "h": 768, 
       	    "battr": [1, 3, 5, 6, 8, 9], 
       	    "pos": 7, 
       	    "api": [5] 
    	}, 
    	"instl": 1, 
    	"tagid": "com.unity-game.trashdash-m3-ios-display", 
    	"secure": 1 
   }], 
    "app": { 
        "id": "a1b2c3d4e5f6g7h8i9j0123456789abc", 
        "name": "Example Game", 
        "bundle": "com.unity.examplegame", 
        "storeurl": "https://itunes.apple.com/us/app/unitygame/id12345678?mt=8" 
    }, 
    "device": { 
        "ifa": "ab12c456-78de-90f1-ghi2-j3kl4567890m", 
   	"ua": "Mozilla/5.0 (iPad; CPU OS 11_2_2 like Mac OS X) AppleWebKit/604.4.7 (KHTML, like Gecko) Mobile/15C202", 
   	"geo": { 
     	    "lat": 55.5492, 
     	    "lon": 59.0456, 
     	    "country": "USA", 
     	    "city": "San Francisco", 
     	    "type": 2, 
     	    "utcoffset": 180 
        }, 
   	"lmt": 1, 
   	"ip": "12.34.5.6", 
   	"devicetype": 5, 
   	"model": "iPad5,3", 
   	"os": "ios", 
   	"osv": "11.2.2", 
   	"hwv": "iPad Air 2 (Wi-Fi)", 
   	"h": 768, 
   	"w": 1024, 
   	"language": "en", 
   	"connectiontype": 2 
    }, 
    "at": 2, 
    "tmax": 200, 
    "regs": {}, 
    "ext": {} 
} 
```
 
### MRAID (non-playable) bid request 
```
{ 
    "id": "ApJv9QAmN3FJjEcTtFDtW3", 
    "imp": [{ 
	"id": "1", 
    	"banner": { 
       	    "w": 480, 
       	    "h": 320, 
       	    "battr": [1, 3, 5, 6, 8, 9, 13], 
       	    "pos": 7 
     	}, 
     	"instl": 1, 
     	"tagid": "com.pixel.gun3d-display", 
     	"secure": 1 
    }], 
    "app": { 
    	"id": "1", 
    	"bundle": "com.unity3d.ads.example", 
    	"name": "Unity Mobile Game", 
    	"storeurl": "https://itunes.apple.com/us/app/unitygame/id12345678?mt=8" 
    }, 
    "device": { 
        "ua": "Mozilla/5.0 (Linux; Android 5.1.1; SM-J200H Build/LMY48B; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/64.0.3282.137 Mobile Safari/537.36", 
   	"geo": { 
     	    "lat": 55.5492, 
     	    "lon": 59.0456, 
     	    "country": "USA", 
     	    "city": "San Francisco", 
     	    "type": 2, 
     	    "utcoffset": 180 
   	}, 
	"source": {
     	    "pchain": "12abcd3456789:12345"
   	},
	"ip": "12.34.5.6", 
  	"devicetype": 4, 
   	"make": "samsung", 
   	"model": "SM-J200H", 
   	"os": "android", 
   	"osv": "5.1", 
   	"hwv": "samsung SM-J200H", 
   	"h": 540, 
   	"w": 960, 
   	"language": "ru", 
   	"carrier": "Tele2", 
   	"connectiontype": 2, 
   	"ifa": "dd92e219-68ae-43b9-bcb7-e6fb7557806e" 
    }, 
    "at": 2, 
    "tmax": 200, 
    "regs": {}, 
    "ext": {} 
} 
```