# Data privacy and consent  
Unity Ads provides built-in solutions for acquiring user consent to data collection, as well as developer APIs for passing a flag should you wish to implement your own consent flow. Passing an affirmative consent flag (`consent == "true"`) to the SDK means that Unity and its partners can show personalized content for that user, while `consent == "false"` means that user cannot receive personalized content. 

Some regions may require consent to collect personal data by law, most notably for [GDPR](#gdpr-compliance) or [CCPA](#ccpa-compliance) regulations. However, consent extends beyond these use cases and should be applied in any region that requires it.

## GDPR Compliance
On May 25, 2018, the General Data Protection Regulation ([GDPR](https://en.wikipedia.org/wiki/General_Data_Protection_Regulation)) took effect in the European Economic Area (EEA), and all versions of the Unity Ads SDK are compliant.

### Unity's built-in solution
Unity recommends that you update to the latest version of the SDK, but it is not required for GDPR compliance. Legacy versions (below version 2.0) of the SDK now only serve contextual ads to users, strictly based on geographic location and current gameplay. No historical or personal data is used for ad targeting, including user behavior within the app and across other apps, or installs.

Versions 2.0 and higher automatically present affected users with an opportunity to opt in to targeted advertising, with no implementation needed from the publisher. On a per-app basis, the first time a Unity ad appears, the user sees a banner with the option to opt in to behaviorally targeted advertising. Thereafter, the user can click an information button to manage their privacy choices.

### Implementing a custom solution
If a publisher or mediator manually requests a user opt-in, the Unity opt-in will not appear. Please note that users can still request opt-out or data deletion, and access their data at any time by tapping the Unity Data Privacy icon when or after an ad appears.

Use the following API to pass a consent flag to the Unity Ads SDK:

#### Unity (C#)
```
// If the user opts in to targeted advertising:
MetaData gdprMetaData = new MetaData("gdpr");
gdprMetaData.Set("consent", "true");
Advertisement.SetMetaData(gdprMetaData);

// If the user opts out of targeted advertising:
MetaData gdprMetaData = new MetaData("gdpr");
gdprMetaData.Set("consent", "false");
Advertisement.SetMetaData(gdprMetaData);
```

#### iOS (Objective-C)
```
// If the user opts in to targeted advertising:
UADSMetaData *gdprConsentMetaData = [[UADSMetaData alloc] init];
[gdprConsentMetaData set:@"gdpr.consent" value:@YES];
[gdprConsentMetaData commit];

// If the user opts out of targeted advertising:
UADSMetaData *gdprConsentMetaData = [[UADSMetaData alloc] init];
[gdprConsentMetaData set:@"gdpr.consent" value:@NO];
[gdprConsentMetaData commit];
```

#### Android (Java)
```
// If the user opts in to targeted advertising:
MetaData gdprMetaData = new MetaData(this);
gdprMetaData.set("gdpr.consent", true);
gdprMetaData.commit();

// If the user opts out of targeted advertising:
MetaData gdprMetaData = new MetaData(this);
gdprMetaData.set("gdpr.consent", false);
gdprMetaData.commit();
```

If the user takes no action to agree or disagree to targeted advertising (for example, closing the prompt), Unity recommends re-prompting them at a later time.

Please visit our legal site for more information on [Unity's approach to GDPR](https://unity3d.com/legal/gdpr).

## CCPA compliance
In January of 2019, the California Consumer Privacy Act ([CCPA](https://oag.ca.gov/privacy/ccpa)) takes effect in California, and all versions of the Unity Ads SDK are compliant.

### Unity's built-in solution
Unity recommends that you update to the latest version of the SDK, but it is not required for CCPA compliance. Versions 2.0 and higher automatically present affected users with an age-gated consent flow for targeted advertising, with no implementation needed from the publisher.

### Implementing a custom solution
If a publisher or mediator manually requests a user opt-in, the Unity opt-in will not appear. 

Use the following API to pass a consent flag to the Unity Ads SDK:

#### Unity (C#)
```
// If the user opts in to targeted advertising:
MetaData privacyMetaData = new MetaData("privacy");
privacyMetaData.Set("consent", "true");
Advertisement.SetMetaData(privacyMetaData);

// If the user opts out of targeted advertising:
MetaData privacyMetaData = new MetaData("privacy");
privacyMetaData.Set("consent", "false");
Advertisement.SetMetaData(privacyMetaData);
```

#### iOS (Objective-C)
```
// If the user opts in to targeted advertising:
UADSMetaData *privacyConsentMetaData = [[UADSMetaData alloc] init];
[privacyConsentMetaData set:@"privacy.consent" value:@YES];
[privacyConsentMetaData commit];

// If the user opts out of targeted advertising:
UADSMetaData *privacyConsentMetaData = [[UADSMetaData alloc] init];
[privacyConsentMetaData set:@"privacy.consent" value:@NO];
[privacyConsentMetaData commit];
```

#### Android (Java)
```
// If the user opts in to targeted advertising:
MetaData privacyMetaData = new MetaData(this);
privacyMetaData.set("privacy.consent", true);
privacyMetaData.commit();

// If the user opts out of targeted advertising:
MetaData privacyMetaData = new MetaData(this);
privacyMetaData.set("privacy.consent", false);
privacyMetaData.commit();
```

**Note**: If you've already implemented the `gdpr` API to solicit consent, you can also use it for CCPA compliance by extending your implementation to CCPA-affected users. Similarly, the `privacy` API can apply to GDPR when extended to affected users. 

Please visit our legal site for more information on [Unity's approach to CCPA](https://unity3d.com/legal/ccpa).