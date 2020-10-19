# Programmatic ads FAQs

#### Do I need to use Price Encryption? 
Unity recommends but does not require obfuscation for integration with the exchange. [Price obfuscation](ProgrammaticBidResponses.md#price-obfuscation) ensures a secure method of passing sensitive price information back to the partner, but omitting it will not impede the integration process. 

#### Where are Unity’s data centers located?
All global traffic is routed to a US-EAST data center located in Virginia. 

#### How does Unity count impressions?
Unity counts impressions when the ad is rendered. Unity pre-caches, then fires the [`nURL`](ProgrammaticBidResponses.md#bid-objects) and impression pixel at the same time the ad renders.  

#### What is your TTL?
Unity’s time to live ([TTL](https://en.wikipedia.org/wiki/Time_to_live)) is 120 minutes for all formats. 

#### Do you support loss notifications?
Yes. For more information, see the **Loss notifications** section of the [Optimization features](ProgrammaticOptimizationLossNotifications.md) documentation. 

#### What is your timeout threshold? 
The default threshold is 200 ms, though Unity is able to extend that in certain circumstances (for more information, contact your account manager). 

#### Do you require secure URLs?
Yes. All URLs that Unity is expected to fire must be [TLS](https://en.wikipedia.org/wiki/Transport_Layer_Security)-compliant and have an `https://` prefix. 

#### Do you support GZIP Requests and Responses? 
Yes, Unity is able to send GZIP requests, and by default requires partners to send GZIP responses back. This helps with bandwidth and optimizing costs.