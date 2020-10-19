# Advertisement optimization
This section provides advice and tools for running higher performing advertising campaigns.

## Tips for creating a strong campaign
When launching a user acquisition campaign, take advantage of every lever at your disposal in order to get the best possible performance. Raising your bids to competitive levels remains the most obvious and effective way to increase volume. However, there are several other ways to optimize without touching your campaign spend. Here we will go over some of the best practices you can implement in order to achieve better results.

### Create a strong trailer
A trailer that resonates with your target audience is the key factor in boosting your conversion rate. This is especially important because conversion rate is one of the most influential metrics that Unity's algorithm uses to determine your ad ranking, and thus your visibility and performance on its network. 

General characteristics of a strong video trailer include:

* Meaningful, action-packed gameplay footage
* Clear calls to action
* References to game dynamics that make your game unique 
* High production quality
* Voiceovers, speaking characters, or sound effects
* Localized for geo-specific languages (where applicable)

### Rotate your trailers
Users eventually experience ad fatigue when exposed to the same video over and over again. This contributes to a decline in conversion rate, and thus a decline in overall volume. Combat this effect by rotating in a new trailer with fresh content approximately once a quarter, if resources permit.

### Fine-tune your campaign targeting
Segmenting your campaigns is a great way to add precision to your user acquisition efforts. Some factors to consider include:

* Choose your geo targets carefully. You might already know what countries perform best for you, and you can [assign each a different bid](AdvertisingCampaignsConfiguration.md#bids) accordingly. 
    * If you're just starting out with user acquisition and haven't gathered enough data to ascertain what performs best, Unity recommends first launching in the US, United Kingdom, Canada, and Australia. As you gather data and want to explore more opportunities, you might consider expanding to other English-speaking countries, countries in western Europe, and the Nordic countries. Conduct periodic reviews of per-country performance and adjust bids as needed.
* Separate your iOS campaigns to target different bids for iPad and iPhone.
    * iPad users generally tend to be higher quality users than iPhone users, so Unity recommends higher bids for that user segment.
* If you're targeting non-English-speaking countries, it's best to run localized versions of your trailer with static [end cards](AdvertisingCampaignsConfiguration.md#end-cards).

### Remove Underperforming Publishers
You have the ability to track which publishers deliver the best- and worst-quality installs, and target accordingly. By adding a parameter to your [tracking URL](AdvertisingCampaignsConfiguration.md#tracking-links), Unity will pass you a 5-digit publisher ID. The macro for this parameter is `"{source\_game\_id}"`. Please [consult your tracking service](AdvertisingCampaignsInstallTracking.md) for the corresponding parameter name. 

Once this parameter is in place, wait until you've collected a statistically significant amount of internal data regarding publisher performance. Then, analyze the user quality data per publisher according to your key metrics (for example, LTV or ARPU), and send Client Services a list of the low-quality publishers you'd prefer to block.

## Additional reading
### Audience Pinpointer
Audience Pinpointer is a powerful user acquisition service that uses machine learning to help you find the players most likely to have value beyond the app install. Using dynamic pricing, Audience Pinpointer allows you to bid more for predicted high-value users and less for easy to find users.

[Get started here](AdvertisingOptimizationAudiencePinpointer.md)

### Video ad best practices
These best practices are compiled from the Unity Ads network, based on proven success for rewarded video. We encourage you to review them and run multiple content types simultaneously to drive the best engagement and reach the widest possible audience. Additionally, once your videos are live, Unity Ads’ machine-learning algorithm evaluates which videos are most appropriate and engaging for each user and optimizes for them to drive the best results.

[Read them here](AdvertisingOptimizationVideoAdsBestPractices.md)