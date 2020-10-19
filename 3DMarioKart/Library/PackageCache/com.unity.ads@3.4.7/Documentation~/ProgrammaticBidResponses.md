# Bid responses
## Overview
Unity supports JavaScript Object Notation (JSON) formats for bid response data. The mime type for the standard JSON representation is "application/json" and specified in an `HTTP` header field as: 

```
ContentType: application/json
```

After your application receives the bid request to the Unity dedicated endpoint, your application must return a bid response or indicate a no-bid within a maximum of 200ms. 

Unity sends Accept-Encoding gzip compression in the request, and highly recommends compressing the response. For more information, see section **2.4 Data Encoding** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).   

Response objects and their attributes are detailed below. The description field denotes required attributes, however it is best practice to include all attributes.  

## response objects 
An object class describing the bidder’s response to the [bid request](https://unityads.unity3d.com/help/programmatic/bid-requests). 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- | 
| `id` | string | `"id":"7jrUifdKuw3HDIR9dbqDVJ"` | A Unity-generated ID passed with the bid request and returned with the bid response.<br><br>**Note**: This attribute is **required** in the response. |
| `cur` | string | `"cur": "USD"` | The currency type, using [ISO currency codes](https://en.wikipedia.org/wiki/ISO_4217#Active_codes). Unity supports U.S. dollars (`"USD"`), as well as other currencies such as `"CNY"`. Make sure this field matches the currency field defined in the adsource configuration, and `exchange_rate` is not empty, or else Unity will ignore the bid response. If this field is ommitted, Unity will use `"USD"`. |
| `bidid` | string | `"bidid": “CHYr9b0ODI8OGLpedtfCpuCH”` | Bidder-generated response ID to assist with logging and tracking. |
| `nbr` | int | `"nbr": 6` | Reason for declining to bid. For a complete list of supported values, see table **5.24 No-Bid Reason Codes** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf). Also see section on [No-bid responses](#no-bid-responses) below. |
| `seatbid` | object array | For more information, see section on [`seatbid` objects](#seatbid-objects). | Unity Auction allows multiple `seatbid` objects in a response, however it currently only uses one `seatbid` object array.<br><br>The seatbid object chosen contains a `bid.id` equal to the `impression.id`.<br><br>If multiple `seatbid` objects meet this criterion, Unity Auction selects one randomly.<br><br>**Note**: This attribute is **required** in the response. |

## seatbid objects 
An object class containing a [`bid` object](#bid-objects) array that allows the partner to bid on behalf of multiple accounts. 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- | 
| `bid` | object array | For more information, see section on [`bid` objects](#bid-objects). | One or more bids. When making multiple bids, include your highest offer to maximize chances of winning the auction (Unity then applies the second highest bid price to the winning bid).<br><br>**Note**: This attribute is **required** in the response. |
| `seat` | string | `"seat": "1234"` | ID of the buyer seat (e.g., advertiser or agency on whose behalf you are making the bid).<br><br>**Note**: This attribute is **required** in the response. |
| `group` | int | `"group": 0` | Indicates whether the impression must be won or lost as a group:<br><br><ul><li>`0` (default; impressions can be won individually)</li><li>`1` (impressions must be won or lost as a group)</li></ul> |

## bid objects 
An object class containing information about the buyer’s bid. 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- |
| `id` | string | `"bidid": "1234567890123456789"` | Bidder-generated bid ID to assist with logging and tracking.<br><br>**Note**: This attribute is **required** in the response. |
| `impid` | string | `"impid": "1"` | ID of the impression associated with this bid. This value matches the impression ID provided in the bid request.<br><br>**Note**: This attribute is **required** in the response. |
| `price` | float | `"price": 0.78` | Bid price in cost per thousand impressions ([CPM](https://en.wikipedia.org/wiki/Cost_per_mille)). Unity Auction ignores values equal to or less than zero, or encrypted values.<br><br>**Note**: This attribute is **required** in the response. |
| `adid` | string | `"adid": "12345678"` | Bidder-generated ID of a pre-loaded ad to be served if the bid wins. |
| `nurl` | string | For more examples, see section on [Example bid responses](#example-bid-responses). | The win notice URL that Unity pings when an ad displays. This attribute also serves as the billable impression counter.<br><br>**Note**: Unity pings the `nURL` at the time of delivery and replaces the bid price macro (`${AUCTION_PRICE}`) with the actual price that the bid cleared (the second-highest bid value).<br><br>**Note**: This attribute is a **unique Unified Auction requirement**. |
| `iurl` | string | `"iurl": "https://secure-datacenter.dsp.com/cr?id=12345678"` | URL without cache-busting to an image that is representative of the content of the campaign (for ad quality and safety checks). |
| `cid` | string | `"cid": "1234"` | Bidder-generated campaign ID, to assist with ad quality checks. The iURL image should be representative of this collection of content. |
| `crid` | string | `"crid": "12345678"` | Creative ID provided by the partner, to assist with ad quality checks. |
| `dealid` | string | `"dealid": "Unity-MM-0034"` | A unique identifier for the direct deal (if applicable).<br><br>**Note**: This ID must match the [deal ID](https://unityads.unity3d.com/help/programmatic/bid-requests#deal-objects) sent in the bid request.
| `cat` | string array | `"cat": ["IAB3-12", "IAB21-2", "IAB10-9" "NEX3-102"]` | IAB content categories that apply to the creative. For a complete list of supported values, see table **5.1 Content Categories** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).<br><br>**Note**: This attribute is **required** in the response. |
| `adm` | string | For more examples, see the **VAST response** example in the [Example bid responses](#example-bid-responses) section. | Means of conveying ad markup in case the bid wins (supersedes the win notice if markup is included in both).<br><br>**Note**: This attribute is **required** in the response. |
| `adomain` | string array | Correct:<br><br><ul><li>`"adomain": ["yourgame.com"]`</li><li>`"adomain": ["studio.yourgame.com"]`</li></ul>Incorrect:<br><br><ul><li>`"adomain": ["yourgame.com/something"]`</li><li>`"adomain": ["www.yourgame.com"]`</li></ul> | Advertiser domain for blacklist checks. Creatives may not be rotated, and only one domain is allowed.<br><br>The format must be the root domain only, with no protocol (note that you may include subdomains if they are not "www"; see example).<br><br>**Note**: This attribute is **required** in the response. |
| `bundle` | string | Google example:<br><br>`"bundle": "com.unityexample.game"`<br><br>Apple example:<br><br>`"1198634425"` | A platform-specific application identifier (store ID for the Apple App Store or Google Play) that is unique to the app and independent of the exchange.<br><br>**Note**: This attribute is a **unique Unified Auction requirement** for app advertisements. |
| `attr` | int array | `"attr": [1, 13]` | An array of attributes describing the creative. For a complete list of supported values, see table **5.3 Creative Attributes** in the [OpenRTB API guide](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf). |
| `w` | int | `"h": 568` | Width of the creative in device independent pixels ([DIPS](https://en.wikipedia.org/wiki/Device-independent_pixel)).<br><br>**Note**: This field is **required for banner ads**, and recommended for all formats. |
| `h` | int | `"w": 320` | Height of the creative in device independent pixels ([DIPS](https://en.wikipedia.org/wiki/Device-independent_pixel)).<br><br>**Note**: This field is **required for banner ads**, and recommended for all formats. |
| `crtype` | string | `"crtype": "VAST"` | The creative type of the ad asset. The following types are valid:<br><br><ul><li>`VAST`</li><li>`VAST 2.0`</li><li>`VAST 3.0`</li><li>`VAST 4.0`</li><li>`VAST VPAID`</li><li>`VAST url`</li><li>`VAST VPAID url`</li><li>`MRAID playable`</li><li>`MRAID url`</li><li>`MRAID 2.0`</li><li>`BANNER`</li><li>`HTML`</li><li>`JS`</li></ul>**Note**: This attribute is **required** in the response. |
| `api` | int | `"api":7` | The API framework supported for this impression. A value of `7` signifies Open Measurement (OM) support. For more information, see the **API Frameworks** section of the [IAB Open Measurement SDK manual](https://s3-us-west-2.amazonaws.com/omsdk-files/docs/Open+Measurement+SDK+Onboarding_version_1.2.pdf). |

### bid.ext objects 
An object subclass extending the `bid` object. 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- |
| `appid` | string | `"appid": "1234567890123"` | App store ID for the app being advertised, if applicable. For more information on locating the App store ID, see Unity’s dashboard documentation on [Store IDs](https://unityads.unity3d.com/help/resources/dashboard-guide#store-ids). |
| `appname` | string | `"appname": "Example Game"` | App name of the app being advertised, if applicable. |
| `storeurl` | string | `"storeurl": "https://itunes.apple.com/us/app/trash-dash/id1198634425?mt=8"` | App store URL of the app being advertised, if applicable.<br><br>**Note**: This attribute is a **unique Unified Auction requirement** for app advertisements. |
| `length` | int | `"length": 15` | Length of the video in seconds, if applicable. |
 
## Example bid responses 
### MRAID response 
```
{ 
    "id":"7jrUifdKuw3HDIR9dbqDVJ", 
    "seatbid":[{ 
        "bid":[{ 
            "id":"1234567890123456789", 
            "impid":"1", 
            "price":0.78, 
            "adid":"12345678", 
            "nurl":"https://secure-datacenter.dsp.com/ab?e=wqT_3FL8Bqh8AwAAAwDWRRUBCKfTuMUFEK7wir7HkaardRiKsMqYu7T-5gUgASotCQAAAQIs8D8R9ihcj8L16D8ZARAQAADwPyEREgApERKoMMm1mAU9uTlA5hRIAlCn0ZoeWJqiRmAAaPbPXnjY0QSAAQGKAQNVU0SSAQEG8FCYAQGgAQGoAQGwAQC4AQPAAQXIAQLCEQDYAQDgAQDwAQCKAnN1ZignYScsIDQ5NjA4NSwgMTQ4NzgwODkzNSk7dWYoD6InLCA2MzM1MDk1MSxCHgAsZycsIDM4MTc1NDQsQh0AAGkBVww0MTQ2OjkA8IeSAuUBIUNpN095d2lhc013SEVLZlJtaDRZQUNDYW9rWXdBRGdBUUFCSTVoUlF5YldZQlZnQVlPSUdhQUJ3QUhnQWdBRUFpQUVBa0FFQm1BRUJvQUVCcUFFRHNBRUF1UUVwaTRpREFBRHdQOEVCS1l1SWd3QUE4RF9KQVFBQUFBQUFBUEFfMlFFCQyIQUR3UC1BDtlmZ2E5UUVBQUNCQW1BS0tocXlzRGFBQ0FMVUMFKQhMMEMFCNBNQUNBTWdDQU5HJ0FOZ0NBT0FDQU9nQ0FQZ0NBSUFEQVpBREFKZ0RBUS4umgIxITdnaUV0dzboAEBtcUpHSUFBb2lvYXNyQTB4QRkA3BQBLtgCAOAC_eA56gI6cGxheS5nb29nbGUuY29tL3N0b3JlL2FwcHMvZGV0YWlscz9pZD1jb20ua2FyYXBwcy59cmFpbmNhYoADAIgDAZADAJgDFKADAaoDAMADkBzIAwDSAygIChIkYzJmNjMwMGItZDQ4Yi00MzcwLTljNTktNjM2OGIyMmM0MGUx2APtuF3dEwDoAwL4AwCABACSBAkvb3BlbnJ0YjKYBACiBA04OS4xODAuNjcuMTY1qAQBsgQMCAAQABigBiC8AzAAuAQAwAQAyAQA0gQNNTAuMy4xMzkuMjEz2gQCCADgBADwBKfRmh6CBRRjb20ua2FyYXBwcy50cmFpbmNhNogFAZgFAKAF_____wUEqAGqBSdVWC1lNTBlYTU5Yy1iNTgwLTQ3ZDAtNWY4MS03OWRjNzhjZjkxNjQ.\\u1134s=eefb9fawb327e7f035f8119da5ay9940689edf0a\\u0026referrer=play.google.com%2Fstore%2Fapps%2Fdetails%3Fid%3Dcom.karapps.traincab\\u0026pp=${AUCTION_PRICE}", 
            "adomain":[ "viaplay.fi" ], 
            "bundle":"com.unityexample.game", 
            "iurl":"https://secure-datacenter.dsp.com/cr?id=12345678", 
            "cid":"1234", 
            "crid":"12345678", 
            "cat":["IAB3-12", "IAB3-8", "IAB21-2", "IAB10-9", "IAB3", "NEX3-101", "IAB3-11", "IAB3-10", "NEX3-102", "IAB3-5", "IAB3-6"], 
            "attr":[], 
            "h":1, 
            "w":1, 
            "appid":"1234567890123", 
            "appname":"Example Game", "storeurl":"https://itunes.apple.com/us/app/trash-dash/id1198634425?mt=8", 
            "appcategory":["Games"], 
            "appsubcategory":["Adventure", "Action"] 
            "ext": { 
                "unity": { 
                    "mraidUrl": "" 
                } 
            } 
        }], 
        "seat":"1234" 
    }], 
    "bidid":"CHYr9b0ODI8OGLpedtfCpuCH", 
    "cur":"USD", 
} 
``` 

### VAST response 
``` 
{ 
    "id": "7jrUifdKuw3HDIR9dbqDVJ", 
    "bidid": "CHYr9b0ODI8OGLpedtfCpuCH", 
    "cur": "USD", 
    "seatbid": [{ 
        "seat": "Example Partner", 
 	    "bid": [{ 
 	        "impid": "1", 
 	        "adomain": ["examplegame.com"], 
 	        "iurl": "https://secure-datacenter.dsp.com/cr?id=12345678", 
 	        "crid": "12345678", 
 	        "cid": "1234", 
 	        "adid": "87654321", 
 	        "cat": ["IAB9-25", "IAB9-25"], 
 	        "bundle": "com.unityexample.game", 
 	        "attr": [], 
 		    "h": 568, 
 		    "w": 320, 
 		    "id": "unrtb-com.unityexample.game-320x568-1ADJE5D8721B3-453S-9DYA-B0AVD69AA1A1", 
 		    "price": 15.04, 
 		    "nurl": "https://rtb-unity-east.yourdsp.co/ads/impression.go?partnerkey=unrtb&siteid=unrtb-com.catchall.game-320x568&adid=179e7ee9&adtype=5&deviceid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&earnings=${AUCTION_PRICE}&ip=82.73.235.50&countrycode=NL&apikey=unrtb&device=iPhone&os=10.3.2&ni=0&appid=123456789&adgroup=NewGame_iOS_NL&bizmodel=cpa&price=0&adom=abcdefghijk.com&adgroupid=180968&campaignid=123456789&creativeid=329c8ef3&ccimpid=X96142d9408c8a40179e7ee907061405&nb=0&accountid=1028&siteaccountid=9876543210&pubappid=1122334455&amu=0&its=1357924680&test=bt",
			"adm": "<?xml version=\"1.0\"?><VAST version=\"2.0\"><Ad id=\"179e7ee9\"><InLine><AdSystem>yourdsp</AdSystem><Error><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=err]]></Error><Impression><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=1234567890&adgroupID=180968&ra=&code=impression]]></Impression><Creatives><Creative AdID=\"179e7ee9\"><Linear><Duration>00:00:15</Duration><TrackingEvents><Tracking event=\"skip\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123456789&adgroupID=180968&ra=&code=skip&s=]]></Tracking><Tracking event=\"engagedView\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123456789&adgroupID=180968&ra=&code=engagedView&s=]]></Tracking><Tracking event=\"creativeView\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=creativeView&s=]]></Tracking><Tracking event=\"start\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=start&s=]]></Tracking><Tracking event=\"firstQuartile\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=firstQuartile&s=]]></Tracking><Tracking event=\"midpoint\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=midpoint&s=]]></Tracking><Tracking event=\"thirdQuartile\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=thirdQuartile&s=]]></Tracking><Tracking event=\"complete\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=complete&s=]]></Tracking></TrackingEvents><VideoClicks><ClickThrough><![CDATA[https://c.yourdsp.com/ads/c.php?a=unrtb&b=unrtb-com.catchall.game-320x568&c=179e7ee9&d=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&ct=0&nb=0&its=1499375139&gf=https%3A%2F%2Fcdngs.yourdsp.com%2F1028%2F768x1024_Unsub_NewGame_Video_All_ZHCN_MZCA02xbqA4.ipad_20170629_09_44_11_.mp4&f=&ra=&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&campaignID=123184311&adgroupID=180968&adgroup=NewGame_iOS_NL&defcpa=0&defcpc=0&appid=1186994231&creativeID=329c8ef3&adType=5&countrycode=NL&ccimpid=X96142d9408c8a40179e7ee907061405&accountid=1028&siteaccountid=4917655231945&pubappid=&sg=&it=&inf=0&price=0.04800847457627119&cg=New,Set&creativeID=329c8ef3]]></ClickThrough><ClickTracking><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=click]]></ClickTracking></VideoClicks><MediaFiles><MediaFile delivery=\"progressive\" type=\"video/mp4\" bitrate=\"500\" width=\"768\" height=\"1024\" scalable=\"true\" maintainAspectRatio=\"true\"><![CDATA[https://cdngs.yourdsp.com/1028/768x1024_Unsub_NewGame_Video_All_ZHCN_MZCA02xbqA4.ipad_20170629_09_44_11_.mp4]]></MediaFile></MediaFiles></Linear></Creative><Creative AdID=\"179e7ee9\"><CompanionAds><Companion width=\"768\" height=\"1024\"><StaticResource creativeType=\"image/jpeg\"><![CDATA[]]></StaticResource><TrackingEvents><Tracking event=\"creativeView\"><![CDATA[https://ads.yourdsp.com/ads/vast.php?appkey=unrtb-com.catchall.game-320x568&adid=179e7ee9&appid=1139609950&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&partnerkey=unrtb&campaignID=123184311&adgroupID=180968&ra=&code=engagedView]]></Tracking></TrackingEvents><CompanionClickThrough><![CDATA[https://c.yourdsp.com/ads/c.php?a=unrtb&b=unrtb-com.catchall.game-320x568&c=179e7ee9&d=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&ct=0&nb=0&its=1499375139&gf=https%3A%2F%2Fcdngs.yourdsp.com%2F1028%2F768x1024_Unsub_NewGame_Video_All_ZHCN_MZCA02xbqA4.ipad_20170629_09_44_11_.mp4&f=&ra=&aid=1ABAE8D8-21B3-453D-9DEA-B9ABD69AA1A1&campaignID=123184311&adgroupID=180968&adgroup=NewGame_iOS_NL&defcpa=0&defcpc=0&appid=1186994231&creativeID=329c8ef3&adType=5&countrycode=NL&ccimpid=X96142d9408c8a40179e7ee907061405&accountid=1028&siteaccountid=4917655231945&pubappid=&sg=&it=&inf=0&price=0.04800847457627119&cg=New,Set&creativeID=329c8ef3]]></CompanionClickThrough></Companion></CompanionAds></Creative></Creatives></InLine></Ad></VAST>", 
 		    "ext": { 
 			    "advertisername": "One Cool Advertiser", 
 			    "appid": "1234567890123", 
 			    "appname": "Example Game", 
 			    "storeurl": "https://itunes.apple.com/us/app/trash-dash/id1198634425?mt=8", 
 			    "length": 15, 
 			    "crtype": "VAST" 
 		    } 
 		}] 
 	}] 
} 
``` 

## No bid responses (NBRs) 
Unified Auction supports multiple response types for a no-bid. It is important to use one of these responses to explicitly signal your wish to not bid on the impression, otherwise the response may be interpreted as a timeout error. 

| **Response** | **Example** |
| ------------ | ----------- | 
| No content | `HTTP 204 No Content` |
| Empty JSON object | `{}` |
| Well-formed bid response (with or without a reason code) | `{"id": “1234567890”, “seatbid”: {}, “nbr”: 2}` |
| Other | <ul><li>`200 OK`</li><li>`302 Redirect`</li></ul> |

For more information, see the **No-Bid Signaling** best practices in section 7.1 of the [Open RTB guidelines](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf).  

### NBR reason codes 
| **Value** | **Description** |
| --------- | --------------- | 
| `0` | Unknown Error |
| `1` | Technical Error |
| `2` | Invalid Request |
| `3` | Known Web Spider |
| `4` | Suspected Non-Human Traffic |
| `5` | Cloud, Data center, or Proxy IP |
| `6` | Unsupported Device |
| `7` | Blocked Publisher or Site |
| `8` | Unmatched User |

## Price obfuscation
When sending the `nURL`, Unity recommends implementing obfuscation for the `${AUCTION_PRICE:BF}` value in order to keep sensitive price information secure on the client side.  

### Methodology 
To obfuscate data, Unity uses Blowfish in ECB mode, with PKCS5 padding (PKCS7 padding with 8 byte block sizes) applied to the input data. The data is then base64 URL-encoded so that the binary data is valid in URLs. Source code is available upon request for troubleshooting. 

Example pseudocode: 

```
base64_URLEncode(blowfish_ecb_encrypt_pkcs5_padding(plaintext))
```

| **Parameter** | **Example value** |
| ------------- | ----------------- | 
| key | `"encryption key"` |
| plaintext | `"10.20"` |
| encrypted, base64 URL encoded | `"z5eznndAkpE="` |

Note that Unity uses a different encryption key in live production. 

### Decryption 
To decrypt data, the input string is base64 URL-decoded. PKCS5 Padding is then removed from the encrypted data, then the blowfish ECB encryption is reversed. 

Example pseudocode: 

```
blowfish_ecb_decrypt_pkcs5_padding(base64_URLDecode(encodedtext))
```