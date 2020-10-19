
NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, UPURTransactionError) {
    kUPURTransactionErrorUnknownError,
    kUPURTransactionErrorNotSupported,
    kUPURTransactionErrorItemUnavailable,
    kUPURTransactionErrorUserCancelled,
    kUPURTransactionErrorNetworkError,
    kUPURTransactionErrorServerError
};

NSString *NSStringFromUPURTransactionError(UPURTransactionError);

NS_ASSUME_NONNULL_END
