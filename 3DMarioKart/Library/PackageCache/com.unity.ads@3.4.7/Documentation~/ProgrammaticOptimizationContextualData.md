# Contextual data extensions
Unity offers additional metric signals on the app or user providing the impression, to help fine-tune your bidding strategies. Contact your account manager to enable the following data sets: 

| **Attribute** | **Data Type** | **Example** | **Description** |
| ------------- | ------------- | ----------- | --------------- | 
| `mediationOrdinal` | int | `"mediationOrdinal": 1` | The developer implements Mediation Ordinal. It signifies how many ads have been in the game session across other ad networks. |
| `mute` | bool | `"mute": true` | The current state of mute on the device when the request is made. |
| `gameCategory` | string | `"gameCategory": "Games"` | Google Play and Apple App Store category definitions |
| `subGameCategory` | string | `"subGameCategory": "Simulation"` | Google Play and Apple App Store Sub-game category definitions. |
| `sessionDepth` | int | `"sessionDepth": 1` | The number of times an ad has been delivered for the current session. |
| `bAge` | string | `"bAge": "17+"` | Content age ratings that are blocked by the publisher. If your app's age rating (as defined on its store page) exceeds this value, your bid will be filtered out. |

**Note**: Unity will not pass any of these data signals if they are not present in the request. 

### Bid request example with contextual data 
```
{ 
    "id": "ueBgFDxOdJ1Y9uairdIkVB", 
        "imp": [{ 
            "id": "1", 
                "banner": { 
                    "w": 320, 
                    "h": 480, 
                    "battr": [1, 3, 5, 6, 8, 9], 
                    "pos": 7 
                }, 
            "instl": 1, 
            "tagid": "com.unity.example-vast", 
            "secure": 1 
        }], 
        "app": { 
            "id": "123abc4d5e0aa0473a80e9e1f43528d8", 
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
            "ifa": "12345AB-C6789-DEFG-0123" 
        }, 
    "at": 2, 
    "tmax": 200, 
    "regs": {}, 
    "ext": { 
        "mediationOrdinal": 1, 
        "mute": true, 
        "gameCategory": "Games", 
        "subGameCategory": "Simulation" 
        "sessionDepth": 1, 
        "bAge":"17+"
    } 
} 
```