# Data privacy
The following object classes contain the requesting app’s [COPPA](https://en.wikipedia.org/wiki/Children%27s_Online_Privacy_Protection_Act) compliance flag, [GDPR](https://en.wikipedia.org/wiki/General_Data_Protection_Regulation) compliance flag, and user consent flag. 
 
* For more information on Unity’s approach to COPPA, see the Unity Analytics [COPPA documentation](https://docs.unity3d.com/2019.1/Documentation/Manual/UnityAnalyticsCOPPA.html).
* For more information on Unity’s approach to GDPR, see the [GDPR legal documentation](https://unity3d.com/legal/gdpr). 

In order to ingest and act on bid requests based on the user’s GDPR, COPPA, and user consent status, Unified Auction provides updated OpenRTB specs to reflect the related changes to the bid request fields: 

## Privacy classes
### regs objects and extensions 
An object class containing the requesting app’s COPPA and GDPR compliance flags. 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- | 
| `regs.coppa` | int | <pre>"regs": {<br>&nbsp;&nbsp;"coppa": 1<br>}</pre> | Flag indicating if the bid request is subject to the COPPA regulations established by the USA FTC. <br><br><ul><li>`0` indicates no.</li><li>`1` indicates yes. This attribute is always passed when set to `1`. |
| `regs.ext.gdpr` | int | <pre>"regs": {<br>&nbsp;&nbsp;"ext": {<br>&nbsp;&nbsp;&nbsp;&nbsp;"gdpr": 1<br>&nbsp;&nbsp;}<br>}</pre> | Upon confirming opt-out from a GDPR-affected user, the request structure for that user’s device sends a flag in the `regs.ext` object with the integer value `1`.<br><br>Unity also strips all personally identifiable information (IDFA, AAID,  IP address, etc.) from the request.<br><br>This attribute is always passed when available. For more information on the spec, please reach out to your Unity account manager. |
| `regs.ext.us_privacy` | string | <pre>"regs": {<br>&nbsp;&nbsp;"ext": {<br>&nbsp;&nbsp;&nbsp;&nbsp;"us_privacy":"1YN-"<br>&nbsp;&nbsp;}<br>}</pre> | Flag indicating consent for requests subject to [CCPA regulations](https://oag.ca.gov/privacy/ccpa). The string format must adhere to the IAB's [U.S. privacy string spec](https://iabtechlab.com/wp-content/uploads/2019/11/U.S.-Privacy-String-v1.0-IAB-Tech-Lab.pdf).<br><br>This attribute is always passed when applicable to affected regions. |

### user object extension 
An object class containing the requesting app’s user consent flag for data collection. 

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- | 
| `user.ext.consent` | string | <pre>"user": {<br>&nbsp;&nbsp;"ext": {<br>&nbsp;&nbsp;&nbsp;&nbsp;"consent": "1"<br>&nbsp;&nbsp;}<br>}</pre> | Flag indicating if the end user has consented to data collection.<br><br><ul><li>`"0"` indicates no.</li><li>`"1"` indicates yes.</li></ul> |

## Endpoint deletion for user opt-out 
In order to continue receiving requests for users subject to GDPR, Unity requires readiness compliance from partners to handle opt-out/deletion requests via an endpoint. Unity also must confirm receipt through a 200 OK message (see section on **Protocol**, below).  

Upon receiving an opt-out/deletion request, the partner must complete deletion of said user’s data that was received from Unity Ads. This removal should affect all systems and subsystems within the partner’s ecosystem. Please note that your Data Processing Addendum will require this compliance measure. 

### Protocol 
The opt-out deletion request protocol is described as follows: 

* Unity and the partner communicate through HTTPs, and therefore the partner must set up an accessible HTTPs endpoint. 
* Unity sends opt-out/deletion requests in HTTP `POST` requests. 
* The `POST` body follows this structure:<br><br><pre>{<br>&nbsp;&nbsp;"id": "xxx", // version 1 uuid identifies the request<br>&nbsp;&nbsp;"idfa": "xxxxxx", // IDFA of the user<br>&nbsp;&nbsp;"opt-out": true, // boolean flag indicating the user opt out<br>&nbsp;&nbsp;"ts": "1524785925" // unix timestamp<br>}</pre>
* A `200` HTTP status code response indicates that the partner has received the request and will delete the user data with the provided IDFA. 
* If the partner has not yet seen the IDFA, they should still respond with a `200` HTTP status code to acknowledge receipt of the message and intent to delete data.  
* If Unity receives a non-`200` HTTP status code, it will retry the request until it receives acknowledgement.  
* Any long-term failure to return a `200` HTTP status code will result in the partner’s designation as non-compliant, at which point Unity will discontinue transmitting any data subject to GDPR. 

Any non-compliance with this requirement will result in the partner not receiving any traffic subject to GDPR. Exceptions from partners using the endpoint solution will only be made on a case-by-case basis. 

### Example of expressed consent 
```
{ 
    "user": { 
        "ext": { 
            "consent": "1" 
        } 
    }, 
    "regs": { 
        "ext": { 
            "gdpr": 1 
        } 
    } 
} 
```

### Example of user opt-out 
```
{ 
    "user": { 
        "ext": { 
            "consent": "0" 
        } 
    },
    "regs": { 
        "ext": { 
            "gdpr": 1,
            "us_privacy":"1YN-"  
        } 
    } 
}
```