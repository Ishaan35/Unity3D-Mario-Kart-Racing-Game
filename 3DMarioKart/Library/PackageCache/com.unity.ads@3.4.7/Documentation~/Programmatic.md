# Introduction to Unified Auction
## Overview
Welcome to the Unified Auction documentation hub. Here you’ll find information about the platform, and integration documents that allow you to participate in Unity’s real-time bidding exchange. Unified Auction provides demand-side partners (DSPs) access to exclusive inventory, a comprehensive range of advertising formats, and 1.5 billion Unity users across 80,000+ apps globally. 

Unified Auction allows DSPs and advertisers to bid on each ad impression. All Unified Auctions are second-price auctions, meaning the winning bidder pays 1 cent higher than the next highest bid. Unity’s implementation is based off the OpenRTB 2.3 specification, and may require parameters that are optional for the Interactive Advertising Bureau ([IAB](https://www.iab.com/our-story/)). Please read the integration guidelines and [contact](mailto:ads-programmatic@unity3d.com) Unity if you have any questions. 

## Requirements 
In order to participate in Unified Auction, please review the following criteria: 

* **Support for OpenRTB 2.3+**: Unity supports OpenRTB 2.3, and some components of OpenRTB 2.5. Partners should support similar specs as described [here](https://www.iab.com/wp-content/uploads/2015/06/OpenRTB-API-Specification-Version-2-3.pdf). 
* **Implement nURL impression tracking**: `nURL` is a field in the bid response’s [`bid` object](ProgrammaticBidResponses.md#bid-objects), or the win notice URL called by the real-time bidding (RTB) exchange. `nURL` is one of several unique bid response requirements necessary for Unity integration, which are identified in the attribute description columns. 
* **Development resources**: Partners implementing Unity’s version of OpenRTB should have sufficient development resources to complete the following: 
    * A [pre-Integration questionnaire](https://goo.gl/forms/LOEA8HYrOeweVCbD3) that helps Unity understand how best to support your integration. 
    * Creative asset validation. 
    * Discrepancy check and ramp-up. 

## How it works
1. Unity receives an ad request from a mobile device. 
2. Unity makes an HTTP POST request to all bidding partner endpoints (each bidder must respond within 200ms, total roundtrip). Unity passes this value through the bid request [`tmax` field](ProgrammaticBidRequests.md#request-objects). 
3. Unified Auction runs a second-price auction based on valid bidder responses. 
4. Upon winning the auction, Unity retrieves the winning bid’s HTML and impression trackers to the client for pre-caching. 
5. When the user surfaces the ad, Unity pings the winning bidder’s [`nURL`](ProgrammaticBidResponses.md#bid-objects) (required field) to notify the DSP of the impression.
6. The creative renders to the end user, and Unity fires other event trackers along with the creative payload.

## Documentation resources 
* OpenRTB (Real-Time Bidding) specs 
  * [Bid requests](ProgrammaticBidRequests.md) 
  * [Bid responses](ProgrammaticBidResponses.md) 
* [Data privacy](ProgrammaticResourcesDataPrivacy.md)
  * [Endpoint deletion for user opt-out](ProgrammaticResourcesDataPrivacy.md#endpoint-deletion-for-user-opt-out)
* [Optimization features](ProgrammaticOptimization.md) 
	* [Contextual data extensions](ProgrammaticOptimizationContextualData.md)
	* [Viewability data extensions](ProgrammaticOptimizationViewabilityData.md)
	* [Loss notifications](ProgrammaticOptimizationLossNotifications.md) 
* [Creative format specs](ProgrammaticResourcesCreativeFormats.md) 
* [FAQs](ProgrammaticResourcesFaq.md)