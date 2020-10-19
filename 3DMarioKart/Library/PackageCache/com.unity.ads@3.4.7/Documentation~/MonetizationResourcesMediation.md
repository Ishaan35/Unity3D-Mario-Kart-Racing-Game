# Mediation guide
## Integrating Unity Ads in a mediation stack
While Unity Ads delivers the most value through [Unified Auction](https://unity.com/solutions/unity-ads), it also seamlessly integrates with mediation solutions to fit your workflow.

Your mediation partner dictates the integration process for Unity Ads. You can find integration guides and SDK adapters for the following widely used mediation partners below:

* [MoPub](https://developers.mopub.com/docs/mediation/networks/unityads/)
* [AdMob](https://developers.google.com/admob/unity/mediation/unity)
* [IronSource](https://developers.ironsrc.com/ironsource-mobile/unity/unityads-mediation-guide/#step-1)
* [Fyber](https://dev-unity.fyber.com/docs)

## Frequently asked questions
#### Does Unity Ads integrate with all mediation vendors?
Unity Ads integrates with most trusted mediation partners. Unity highly recommends using one of the partners listed above to ensure success, as they offer full integration guides specific to Unity. If you choose a different partner, Unity recommends confirming that they have integration resources to ensure compatibility with Unity Ads. 

#### Are there any performance issues to consider when selecting a mediation vendor?
Unity recommends integrating with an open source mediation partner that offers customizable SDK adapters. This ensures the mediator and the ad source (Unity) have minimal data discrepancies and drive better results. 

#### Do I need the Unity Ads SDK in order to run Unity Ads in mediation?
Yes. For your mediation partner to call Unity’s network, you need to install the Unity Monetization SDK. For more information on how to do this, see documentation on [Getting started](MonetizationGettingStarted.md) with Unity Monetization. You can download and import the latest SDK for your platform here:

* [Unity (C#)](https://assetstore.unity.com/packages/add-ons/services/unity-ads-66123)
* [iOS (Objective-C)](https://github.com/Unity-Technologies/unity-ads-ios)
* [Android (Java)](https://github.com/Unity-Technologies/unity-ads-android/releases)

#### Can I run Unity Ads in parallel with my mediation stack?
You can run Unity ads separately from your mediation stack. If your game is made with Unity, you can download the Asset package from the [Unity Asset Store](https://assetstore.unity.com/packages/add-ons/services/unity-monetization-3-0-66123). If your game is not made with Unity, you can download the SDK for [iOS](https://github.com/Unity-Technologies/unity-ads-ios) or [Android](https://github.com/Unity-Technologies/unity-ads-android/releases) and access all the same features. To discover what’s included with the Unity Monetization SDK 3.0, read this [Unity Blog post](https://blogs.unity3d.com/2018/10/19/revolutionizing-how-game-developers-monetize-with-unity-monetization-sdk-3-0/). 

#### If I run Unity Ads through a mediation partner, can I still leverage all of the Unity Monetization SDK features?
Nearly all of the Monetization SDK features are compatible with mediation. However, its most advanced monetization feature, [Personalized Placements](https://unity.com/solutions/mobile-business/monetize-your-game/personalized-placements), performs best when all advertising touchpoints run through Unity. This is because Personalized Placements combine your ads and in-app purchasing revenue systems by making automated decisions about which to serve, based on Unity’s machine learning algorithm’s highest predictive player LTV.    

#### Can I place Unity Ads in multiple positions of my mediation stack?
Yes. You can integrate Unity Ads into multiple price tiers of your waterfall.
Are my earnings based on numbers reported by Unity or my mediation partner?
Any earnings generated through Unity Ads are based on Unity’s reported billing numbers. While there should not be significant differences between Unity’s numbers and a mediator’s numbers, using an open source mediation partner will help ensure the two sources match as closely as possible.
