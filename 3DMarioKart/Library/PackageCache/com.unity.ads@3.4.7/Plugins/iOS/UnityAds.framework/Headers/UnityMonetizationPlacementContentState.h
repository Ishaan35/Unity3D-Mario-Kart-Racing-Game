NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, UnityMonetizationPlacementContentState) {
    kPlacementContentStateReady,
    kPlacementContentStateNotAvailable,
    kPlacementContentStateDisabled,
    kPlacementContentStateWaiting,
    kPlacementContentStateNoFill
};

NSString *NSStringFromPlacementContentState(UnityMonetizationPlacementContentState);

UnityMonetizationPlacementContentState PlacementContentStateFromNSString(NSString *);

NS_ASSUME_NONNULL_END
