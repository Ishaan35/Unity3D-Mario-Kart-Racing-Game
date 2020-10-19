# Supported creative formats
Unified Auction supports mobile static [display ad](#static-display-ads) (HTML) and [video ad](#video-ads) (VAST) formats. This article outlines specifications and best practices for Demand Side Platform (DSP) partners. 

## Static display ads 
Static display, display interstitial, and display ads refer to the same ad format in Unity, as these are commonly used for interstitial [Placements](MonetizationPlacements.md) with static graphic assets. This format typically uses an image with a hyperlink or click event to a destination URL from the advertiser. For example: 

```
<a href="https://click-destination.com"><img src="https://ad-image.jpg"></a>
``` 

### Specifications 
The following are requirements for display ad formats:

* Content must be formatted as a snippet of HTML (or JavaScript) code, not as a full HTML document or web page. 
* If the content was created with JavaScript only, it must be wrapped with `<script>` tags. 
* The Unity SDK renders ad markup within the webview as full-screen on mobile devices. However, the advertising partner is responsible for scaling and centering content. Unity loads content to the top-left corner by default. To avoid this, use CSS to center your content in various mobile screen sizes. 

Unity highly recommends taking advantage of information available in the Unified Auction [bid request](ProgrammaticBidRequests.md) to send assets that are optimized for the device, platform, and network connection represented in the request. 

### Example Display Ad 
Example of HTML with hyperlink: 

```
<style>html, body { margin: 0; padding: 0; width: 100%; height: 100%; vertical-align: middle; } html { display: table; } body { display: table-cell; vertical-align: middle; text-align: center; -webkit-text-size-adjust: none; } </style><a href="https://click-link-with-tracking.com/click?with-click-information"><img src="https://host-url.com/display/ad_image.fig" width="320" height="480"></a><img height="1" width="1" src="https://impression-tracking.com/pixel?id=1234">
```

Example of HTML with JS script: 

```
<script src="https://display-ad.com/ad-location-js-file"><script><style>some css style</style><img src="https://impression-pixel-url">
```

## Video ads 
Video Ad Serving Template ([VAST](https://www.iab.com/guidelines/digital-video-ad-serving-template-vast-3-0/)) is a universal XML schema for serving ads to digital video players. Unity supports VAST 3.0 (standards developed and maintained by the Internet Advertising Bureau ([IAB](https://www.iab.com/))), and is backwards compatible with 2.0, with the exception of some features. 

### How VAST works in Unity 
A typical VAST XML template contains:

* Impression and error URLs to track each event. 
* Information on the video length, and URLs for hosted assets. 
* Video-tracking URLs for video start, first quartile, midpoint, third quartile, and completion events. 
* Click-tracking URLs and destination URLs for video click events. 
* Optional companion ad information for the video asset, tracking, and click-through URLs. 

**Important**: The Unity SDK parses VAST XML to retrieve video media files and fire relevant tracking URLs. Properly formatted XML with the correct media assets is critical, otherwise the SDK will fail to render the ad despite receiving the VAST bid response. 

### Specifications
The following are requirements for video ads:

* Unity supports VAST 2.0 and 3.0 for linear video with an optional companion ad. 
* VAST InLine contains all the required elements to render a video ad. 
* VAST Wrapper points to another VAST XML, with a max wrapper depth of 5. 
* Supported video media files include: 
    * MIME type only `'video/mp4'` for both iOS and Android. 
    * Video duration under 40 seconds. The video duration tag is required, in `HH:MM:SS` format. 
    * Video file sizes less than 20 MB. 
* Unity supports companion ads that appear after the initial video ad completes:  
    * `<StaticResource>` as an image/jpeg, image/jpg, image/gif, or image/png. 
* For click events, the click-through URL should be a single item, while the click-tracking URL may be multiple items.  
    * `<CompanionClickTracking>` is not currently supported, but may be in the future.
    * Landscape dimensions are 480 x 320 pixels. 
    * Portrait dimensions are 320 x 480 pixels.
*  For error reporting, Unity fires an `<Error>` tracking URL with an associated error code: 
    * `100`: XML parsing error 
    * `20x`: Duration or size unsupported  
    * `30x`: Wrapper-related errors 
    * `40x`: Media file-related errors 
    * `60x`: Companion ad-related errors 
    * `90x`: Any other undefined error 
*  Unity does **not** support ad pods, non-linear ads, or industry icons from 3.0. 

### Best Practices 
* For an example VAST XML schema, please see the [example XML format](#example-xml-formats) below. 
* Limit VAST wrapper redirection to 2-3. Check the final URL points for the appropriate VAST XML file.  
* Include `'bitrate'`, `'delivery'`, `'type'`, `'width'`, and `'height'` as tag attributes. 
* Recommended video file size is under 5 MB for users with WiFi connections, and 2 MB for users with cellular connections, with bitrates of 512 kbps or lower. The Unity SDK calculates video file size from bitrate and duration.  

### Common Issues 
If your bid response wins an auction but your creatives don’t show an impression or send tracking events, here are some possible reasons to investigate:

* Blank XML: The returned XML from Wrapper `VASTTagURI` is blank or malformed. 
* XML parsing error: Missing required tag elements like `<VAST>`, `<Duration>`, or `<MediaFile>`. 
* Video file size too large: The available `<MediaFile>` in video/mp4 is over 20 MB. 
* Video duration too long: Duration exceeds 40 seconds.  

### Example XML formats
Example VAST Linear ad with Companion in `<InLine>`: 

```
<VAST version="3.0">
    <Error id="to-track-error"><![CDATA[http://error-tracking-url]]></Error>
    <Ad id="ad_id">
        <InLine>
            <AdSystem>2.0</AdSystem>
            <AdTitle>ad title</AdTitle>
            <Impression id="to-track-impression"><![CDATA[http://impression-url]]></Impression>
            <Error id="to-track-error"><![CDATA[http://error-tracking-url]]></Error>
            <Creatives>
                <Creative>
                    <Linear>
                        <Duration>00:00:15</Duration>
                        <TrackingEvents>    
                        // start, firstQuartile, midepoint, thirdQuartile, complete, mute, unmute
                            <Tracking event='start'><![CDATA[http://tracking-url]]></Tracking>
                            ....
                        </TrackingEvents>
                        <VideoClicks>
                            <ClickThrough><![CDATA[https://video_click.com]]></ClickThrough>
                            <ClickTracking><![CDATA[https://video_click_tracking.com]]></ClickTracking>
                        </VideoClicks>
                        <MediaFiles>
                            <MediaFile delivery='progressive' width='16' height='9' type='video/mp4' bitrate='600' apiFramework='NONE'><![CDATA[https://media_file_site.your_file.mp4]]>
                            </MediaFile>
                            //  ... multiple media files per quality
                        </MediaFiles>
                    </Linear>
                </Creative>
            <Creative>
                <CompanionAds>  // For end card
                    <Companion>   // Static resource asset only usually image file
                        <StaticResource creativeType="image/jpeg">
                            <![CDATA[end card url]]>
                        </StaticResource>
                        <TrackingEvents>
                            <Tracking event='creativeView'><![CDATA[https://endcard_tracking]]></Tracking>
                        </TrackingEvents>
                        <CompanionClickThrough><![CDATA[[https://click_url](https://click_url)]]></CompanionClickThrough>
                        <CompanionClickTracking><![CDATA[https://tracking_url]]></CompanionClickTracking>
                    </Companion>
                </CompanionAds>
            </Creatives>
        </InLine>
    </Ad>
</VAST>
```

Example VAST in `<Wrapper>`: 

```
<VAST version="2.0">
    <Ad id="1234">
        <Wrapper>
            <Error><![CDATA[https://vast_wrapper_error.tracking.com]]></Error>
            <Impression>...</Impression>
            <VASTAdTagURI><![CDATA[http://vast-ad--url]]></VASTAdTagURI>  // point to inline xml
        </Wrapper>
    </Ad>
</VAST>
```

### Unity VAST error codes
| **Error code** | **Description** |
| -------------- | --------------- |
| `100` | XML parsing error. |
| `202` | The video duration is too long or is not formatted properly (HH:MM:SS). |
| `302` | The wrapper limit was reached. Unity tries a maximum of 5 routes to fetch VAST content through `VASTAdTagURI`. |
| `401` | The supported video media file was not found in the media link. |
| `403` | The received media files are unsupported. Make sure they are `'video/mp4'` type with a maximum file size of 20 MB. |
| `404` | The media file url is not supported in iOS. Make sure it uses HTTPS protocol. |
| `499` | The media file is missing a valid ClickThrough URL under `<VideoClicks>`. |
| `601` | The Companion Ad (end card) size is unsupported. The minimum image size is 320 x 480 pixels (portrait) or 480 x 320 pixels (landscape). |
| `604` | A valid Companion Ad resource URL was not found. |
| `699` | The Companion Ad does not contain a valid ClickThrough URL. |
| `998` | The VAST XML content contains an invalid URL. |
| `999` | Unknown error. |