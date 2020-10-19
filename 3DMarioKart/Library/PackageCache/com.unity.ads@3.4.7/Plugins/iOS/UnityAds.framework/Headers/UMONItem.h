@interface UMONItemBuilder : NSObject
@property(strong, nonatomic) NSString *productId;
@property(nonatomic) double quantity;
@property(strong, nonatomic) NSString *type;
@end

@interface UMONItem : NSObject
+(instancetype)build:(void (^)(UMONItemBuilder *))buildBlock;

-(instancetype)initWithBuilder:(UMONItemBuilder *)builder;

@property(nonatomic, strong, readonly) NSString *productId;
@property(nonatomic, readonly) double quantity;
@property(nonatomic, strong, readonly) NSString *type;
@end
