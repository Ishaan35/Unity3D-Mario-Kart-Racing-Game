# Viewability data extensions 
Unified Auction provides specific viewability metrics within the [bid request](ProgrammaticBidRequests.md). Through select viewability partnerships ([MOAT Analytics](https://moat.com/analytics)), Unity can pass historical viewability measurements based on the Media Rating Council ([MRC](http://mediaratingcouncil.org/)) and [GroupM](https://www.groupm.com/about-groupm) definition within each bid request.  

## Viewability standard definition 
The [MRC viewability requirement](http://www.mediaratingcouncil.org/063014%20Viewable%20Ad%20Impression%20Guideline_Final.pdf) states that at least 50% of a video ad must be in view on-screen for 2 seconds before it counts as viewable. 

## metrics object 
Based on [OpenRTB specification 2.5](https://www.iab.com/wp-content/uploads/2016/03/OpenRTB-API-Specification-Version-2-5-FINAL.pdf) section 3.2.5, Unity sends viewability data through the `metrics` object in the bid request. 

| **Attribute** | **Data Type** | **Example** | **Description** |
| ------------- | ------------- | ----------- | --------------- | 
| `type` | string | `valid_and_viewable_perc` | The specific metric measured by viewability partner (in this case, the MOAT Valid and Viewable Rate, defined as the number of valid impressions that were viewable under the MRC standard). |
| `value` | float | `0.995` | Value of the metric in numeric format (precision-3). |
| `vendor` | string | `MOAT` | Viewabilty measurement partner. |

### Example bid request 
```
{ 
    "id": "abCdEFgOdJ1Y9abcdeIkZB", 
    "imp": [{ 
        "id": "1", 
        "video": { 
            "mimes": [ 
                "video/mp4" 
            ], 
            "minduration": 15, 
            "maxduration": 30, 
            "protocols": [2, 3, 5, 6], 
            "w": 375, 
            "h": 667, 
            "linearity": 1, 
            "sequence": 1, 
            "battr": [1, 3, 5, 6, 8, 9, 13], 
            "pos": 7 
        }, 
        "instl": 1, 
        "tagid": "com.unity.example-vast", 
        "secure": 1, 
        "metrics": [{ 
            "type": "valid_and_viewable_perc", 
            "value": 0.994, 
            "vendor": "MOAT" 
        }], 
        "app": { 
            "id": "123abc4d5e6fg7890a80e9e1f43528d8", 
            "name": "Example Game", 
            "bundle": "123456789", 
            "storeurl": "https://itunes.apple.com/us/app/ExampleGame/id123456789?mt=8" 
        }, 
        "device": { 
            "ua": "Mozilla/5.0 (iPhone; CPU iPhone OS 11_2_5 like Mac OS X) AppleWebKit/604.5.6 (KHTML, like Gecko) Mobile/15D60", 
            "geo": { 
                "lat": 50.0144, 
                "lon": -50.2772, 
                "type": 2, 
                "country": "USA", 
                "city": "San Francisco", 
                "utcoffset": -480 
            }, 
            "ip": "123.123.12.123", 
            "devicetype": 4, 
            "model": "iPhone7,2", 
            "os": "ios", 
            "osv": "11.2.5", 
            "hwv": "iPhone 6", 
            "h": 667, 
            "w": 375, 
            "language": "en", 
            "carrier": "ATT", 
            "connectiontype": 2, 
            "ifa": "12345AB-C6789-DEFG-1234" 
        }, 
        "at": 2, 
        "tmax": 200, 
        "regs": { 
            "ext": {} 
        } 
    }] 
}
```