# Loss notifications 
Unity strives to enable partner success on its exchange by providing real-time loss notifications. These notifications communicate the reason that the participating bid lost in the auction. Advertising partners can leverage real-time information to inform their bidding strategies effectively across Unity’s inventory. 

## Integration  
To receive loss notifications, include an optional `lurl` (loss URL) attribute in your response's [bid object](ProgrammaticBidResponses.md#bid-objects) (for a complete guide on generating bid responses, see Unity's OpenRTB [bid responses](ProgrammaticBidResponses.md) specs). This allows Unity to populate the [loss reason code](#loss-reason-codes) within the bid response.  

| **Attribute** | **Type** | **Example** | **Description** |
| ------------- | -------- | ----------- | --------------- | 
| `lurl` | string | `"lurl": "http://example.com/?bid=123456&loss=${AUCTION_LOSS}"` | Loss notice URL called by the exchange when a bid loses in auction.  |

Unity will return the loss reason code within the `${AUCTION_LOSS}` macro. 

### Loss reason codes 
The following are loss reason codes Unity may return. Note that codes below `1001` are custom loss reasons that are not defined in the OpenRTB 2.5 spec. 

| **Code** | **Reason** | **Description** |
| -------- | ---------- | --------------- | 
| `1` | Internal error | A Unity internal error. |
| `2` | Impression opportunity expired | The impression took too long to show, such that the impression tracker was no longer valid. |
| `3` | Invalid bid response | The partner responded with no fill, did not respond at all. |
| `4` | Invalid Deal ID | The [Deal ID](ProgrammaticBidRequests.md#deal-objects) in the bid response does not match the deal(s) in the bid request. |
| `5` | Invalid auction ID | The [ID](ProgrammaticBidRequests.md#request-objects) in the bid request is not same as the ID in the bid response. |
| `6` | Invalid (malformed) advertiser domain | The `adomain` (ad domain) [attribute field](ProgrammaticBidResponses.md#bid-objects) in the bid response is an invalid format. |
| `7` | Missing ad markup | The `adm` (ad markup) [attribute field](ProgrammaticBidResponses.md#bid-objects) is empty. |
| `8` | Missing creative ID | The `crid` (creative ID) [attribute field](ProgrammaticBidResponses.md#bid-objects) is empty. |
| `9` | Missing bid price | The `price` (bid price) [attribute field](ProgrammaticBidResponses.md#bid-objects) is empty. |
| `101` | Bid below deal floor | The bid price was below the [bid floor](ProgrammaticBidRequests.md#deal-objects) set in the request.  |
| `102` | Lost to higher bid | The bid lost the auction because to a higher bid price. |
| `104` | Buyer seat blocked | The publisher has blocked the buyer or partner. |
| `200` | Creative filtered (reason unknown) | A catch-all for creatives that were filtered when the reason is unknown. |
| `201` | Creative filtered (pending approval) | The creative’s status is pending approval for the exchange. |
| `202` | Creative filtered (approval denied) | The creative was disapproved by Unity’s exchange review. |
| `203` | Creative filtered (invalid size) | The [width and height](ProgrammaticBidResponses.md#bid-objects) of the creative does not match the dimensions defined in the request. |
| `205` | Creative filtered (advertiser exclusions) | The publisher has excluded the advertiser. |
| `206` | Creative filtered (app bundle exclusions) | The publisher has blocked the app bundle. |
| `209` | Creative filtered (category exclusions) | The publisher has blocked the advertising content category. |
| `212` | Creative filtered (video length) | The [video length](ProgrammaticBidResponses.md#bid-ext-objects) exceeds 30 seconds. |
| `1000` | Failed Unity validator | The bid failed validation due to compliance with Unity’s OpenRTB standards described in the request-response integration guides. |
| `1001` | Invalid JSON format | The bid response is a malformed JSON. |
| `1002` | General marketplace rule block | A catch-all error for marketplace blocks that occur in the Unity Auction. |
| `1003` | Max age rating exceeded | The publisher blocked the bid due to the advertised app's age rating. |