# Advertising Statistics API

**Note**: The following documentation applies to the current API version; aspects of the domain and functionality are subject to change for general release.

The Statistics API is an HTTP interface that allows advertisers to retrieve acquisition statistics data in CSV format.

## Authentication
Using the Unity Ads Statistics API requires an API key token. You can generate an API key token from the [Acquire dashboard](https://acquire.dashboard.unity3d.com/) by selecting **Settings** from the left navigation bar.

**Note**: The API key is generated for a specific username. If that username is removed from the organisation, that API key will be automatically revoked. Please be aware of this if you use the API key as part of an integration with a 3rd party or any other business intelligence solution.

You can use the key token in two ways:

1. Place the key token in the GET request URL `apikey` parameter. For example:

```
https://stats.unityads.unity3d.com/organizations/:organization_id/reports/acquisitions?apikey=<token>
```

2. Place the key token in the `Authorization` header of the GET request, prefixed with `Bearer`. For example:

```
curl
https://stats.unityads.unity3d.com/organizations/:organization_id/reports/acquisitions -H "Authorization: Bearer <token>"
```

## GET request
Make a GET request to the following example URL:

```
https://stats.unityads.unity3d.com/organizations/:organizationId/reports/acquisitions?splitBy=campaignSet,country&fields=timestamp,campaignSet,country,clicks,installs,spend&apikey=<token>
```

### Parameters
The following parameters define the period and scope of requested data:

| **Parameter** | **Description** | **Example** | 
| ------------- | --------------- | ----------- |
| `start` | The start time of the data query (required). | `start={ISO 8601 timestamp}` |
| `end` | The end time of the data query (required) | `end={ISO 8601 timestamp}` |
| `scale` | The time resolution of the data query. The default value is `all`. | <ul><li>`scale={all}`</li><li>`scale={hour}`</li><li>`scale={day}`</li><li>`scale={week}`</li><li>`scale={month}`</li><li>`scale={quarter}`</li><li>`scale={year}`</li></ul> |

Example time format: `2017-06-01T00:00:00.000Z`

### Filters
You can apply the following list of filters to your request:

| **Filter** | **Description** | **Example** |
| ---------- | --------------- | ----------- |
| `campaignSets` | A comma-separated list of campaign set IDs to filter. | `campaignSets=[:campaign_set_id]` |
| `campaigns` | A comma-separated list of campaign IDs to filter. | `campaigns=[:campaign_id]` |
| `targets` | A comma-separated list of target game IDs to filter. | `targets=[:target_id]` |
| `sources` | A comma-separated list of source game IDs to filter. | `sources=[:source_id]` |
| `adTypes` | A comma-separated list of ad type IDs to filter. | <ul><li>`adTypes=[(video)]`</li><li>`adTypes=[(playable)]`</li><li>`adTypes=[(graphic)]`</li></ul> |
| `countries` | A comma-separated list of country codes to filter. | `countries=[:country_code]` |
| `stores` | A comma-separated list of stores to filter. | <ul><li>`stores=[(apple)]`</li><li>`stores=[(google)]`</li><li>`stores=[(xiaomi)]`</li></ul> |
| `platforms` | A comma-separated list of platforms to filter. | <ul><li>`platforms=[(ios)]`</li><li>`platforms=[(android)]`</li></ul> |
| `osVersions` | A comma-separated list of operating system versions to filter. | `osVersions=[:os_version]` |
| `creativePacks` | A comma-separated list of creative packs to filter. | `creativePack=[:creativePack_id]` |
| `sourceAppId` | A comma-separated list of source app IDs to filter. The app ID is an identifier derived from the game's app store page. | `sourceAppIds=[:source_app_id]` |
| `exchange` | A comma-seperated list of ad exchanges to filter. | `exchange=[unity]` |

In addition, you can specify a comma-separated list of dimensions by which to split data:

```
splitBy=[(campaignSet|creativePack|adType|campaign|target|source|sourceAppId|store|country|
platform|osVersion|exchange)]
```

The following allows you to specify comma-separated fields to display in your report:

```
fields=[(timestamp|campaignSet|creativePack|adType|campaign|target|source|store|country|platform|osVersion|starts|views|clicks|installs|spend)] (default:all)
```

**Note**: Omitting the `fields` parameter returns all fields.

## Response
The data returns in CSV format, with the following delimiters:

* Commas (`,`) separate fields
* Periods (`.`) indicate decimals
* Double quotes (`" " `) indicate text fields
* The newline character separates lines within a field

### Split data
Split `campaignSet` data results in two fields:

* `campaign set id`
* `campaign set name`

Split `campaign` data results in two fields:

* `campaign id`
* `campaign name`

Split `creativePack` data results in two fields:

* `creative pack id`
* `creative pack name`

Split `target` data results in three fields:

* `target id`
* `target store id`
* `target name`

**Note**: Splitting data by too many dimensions may impact response times for querying the API. To avoid lagging or timeouts, Unity recommends querying 1 day of data at a time when splitting by `source` and `country`.

#### Example response
```
$ curl -L
"https://stats.unityads.unity3d.com/organizations/:organizationId/reports/acquisitions?split
By=campaignSet,country&fields=timestamp,campaignSet,country,clicks,installs,spend&apik
ey=<token>"
timestamp,campaign set id,campaign set name,country,clicks,installs,spend
2013-03-01T00:00:00.000Z,50ed569d57fe1f324a15fbf7,"Campaign set #5",AU,71,30,45
2013-03-01T00:00:00.000Z,50ed569d57fe1f324a15fbf7,"Campaign set #5",CA,129,88,132
CONFIDENTIAL, Unity Technologies 2017
Advertiser Stats API
2013-03-01T00:00:00.000Z,50ed569d57fe1f324a15fbf7,"Campaign set
#5",US,1745,855,1282.5
2013-03-01T00:00:00.000Z,50eeb7339e10c9d21c0225cb,"Campaign set
#6",AT,39,19,28.5
2013-03-01T00:00:00.000Z,50eeb7339e10c9d21c0225cb,"Campaign set #6",AU,16,10,15
2013-03-01T00:00:00.000Z,50eeb7339e10c9d21c0225cb,"Campaign set
#6",BE,209,120,180
```