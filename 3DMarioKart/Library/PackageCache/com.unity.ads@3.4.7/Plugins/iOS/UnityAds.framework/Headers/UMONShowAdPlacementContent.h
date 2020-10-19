#import <UIKit/UIViewController.h>
#import "UMONRewardablePlacementContent.h"
#import "UnityAdsFinishState.h"

@protocol UMONShowAdDelegate <NSObject>
-(void)unityAdsDidStart:(NSString*)placementId;
-(void)unityAdsDidFinish:(NSString*)placementId withFinishState:(UnityAdsFinishState)finishState;
@end

@interface UMONShowAdPlacementContent : UMONRewardablePlacementContent
@property (strong, nonatomic) id<UMONShowAdDelegate> delegate;
-(void)show:(UIViewController *)viewController;
-(void)show:(UIViewController *)viewController withDelegate:(id<UMONShowAdDelegate>)delegate;
@end
