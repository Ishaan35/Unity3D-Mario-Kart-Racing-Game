
NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, UPURStore) {
    kUPURStoreNotSpecified,
    kUPURStoreGooglePlay,
    kUPURStoreAmazonAppStore,
    kUPURStoreCloudMoolah,
    kUPURStoreSamsungApps,
    kUPURStoreXiaomiMiPay,
    kUPURStoreMacAppStore,
    kUPURStoreAppleAppStore,
    kUPURStoreWinRT,
    kUPURStoreTizenStore,
    kUPURStoreFacebookStore
};

NSString *NSStringFromUPURAppStore(UPURStore);

NS_ASSUME_NONNULL_END
