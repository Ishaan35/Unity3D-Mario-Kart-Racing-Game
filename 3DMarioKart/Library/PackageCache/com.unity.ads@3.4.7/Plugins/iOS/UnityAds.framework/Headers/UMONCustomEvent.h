NS_ASSUME_NONNULL_BEGIN

@interface UMONCustomEventBuilder : NSObject
@property (nonatomic) NSString* category;
@property (nonatomic) NSString* type;
@property (nonatomic) NSDictionary* userInfo;
@end

@interface UMONCustomEvent : NSObject
@property (strong) NSString* category;
@property (strong) NSString* type;
@property (strong) NSDictionary* userInfo;
-(instancetype)initWithBuilder:(UMONCustomEventBuilder*)builder;
+(instancetype)build:(void (^)(UMONCustomEventBuilder *))buildBlock;
@end

NS_ASSUME_NONNULL_END
