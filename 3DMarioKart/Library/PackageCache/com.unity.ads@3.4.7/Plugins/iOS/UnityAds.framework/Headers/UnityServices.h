
NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, UnityServicesError) {
    kUnityServicesErrorInvalidArgument,
    kUnityServicesErrorInitSanityCheckFail
};

@protocol UnityServicesDelegate <NSObject>
- (void)unityServicesDidError:(UnityServicesError)error withMessage:(NSString *)message;
@end

@interface UnityServices : NSObject

- (instancetype)init NS_UNAVAILABLE;
+ (instancetype)initialize NS_UNAVAILABLE;
/**
 *  Initializes UnityAds. UnityAds should be initialized when app starts.
 *
 *  @param gameId   Unique identifier for a game, given by Unity Ads admin tools or Unity editor.
 *  @param delegate delegate for UnityAdsDelegate callbacks
 *  @param testMode Set this flag to `YES` to indicate test mode and show only test ads.
 */
+ (void)initialize:(NSString *)gameId
          delegate:(nullable id<UnityServicesDelegate>)delegate
          testMode:(BOOL)testMode
          usePerPlacementLoad:(BOOL)usePerPlacementLoad;

/**
 *  Get the current debug status of `UnityAds`.
 *
 *  @return If `YES`, `UnityAds` will provide verbose logs.
 */
+ (BOOL)getDebugMode;

/**
 *  Set the logging verbosity of `UnityAds`. Debug mode indicates verbose logging.
 *  @warning Does not relate to test mode for ad content.
 *  @param enableDebugMode `YES` for verbose logging.
 */
+ (void)setDebugMode:(BOOL)enableDebugMode;

/**
 *  Check to see if the current device supports using Unity Ads.
 *
 *  @return If `NO`, the current device cannot initialize `UnityAds` or show ads.
 */
+ (BOOL)isSupported;

/**
 *  Check the version of this `UnityAds` SDK
 *
 *  @return String representing the current version name.
 */
+ (NSString *)getVersion;

/**
 *  Check that `UnityAds` has been initialized. This might be useful for debugging initialization problems.
 *
 *  @return If `YES`, Unity Ads has been successfully initialized.
 */
+ (BOOL)isInitialized;

@end

NS_ASSUME_NONNULL_END
