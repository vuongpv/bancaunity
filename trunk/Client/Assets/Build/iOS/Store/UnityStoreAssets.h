#import <Foundation/Foundation.h>
#import "IStoreAsssets.h"

@interface UnityStoreAssets : NSObject <IStoreAsssets> {
	int version;
	NSMutableArray* virtualCurrenciesArray;
	NSMutableArray* virtualGoodsArray;
	NSMutableArray* virtualCurrencyPacksArray;
	NSMutableArray* virtualCategoriesArray;
	NSMutableArray* nonConsumablesArray;
}

@property (nonatomic, retain) NSMutableArray* virtualCurrenciesArray;
@property (nonatomic, retain) NSMutableArray* virtualGoodsArray;
@property (nonatomic, retain) NSMutableArray* virtualCurrencyPacksArray;
@property (nonatomic, retain) NSMutableArray* virtualCategoriesArray;
@property (nonatomic, retain) NSMutableArray* nonConsumablesArray;

- (id)initWithStoreAssetsJSON:(NSString*)storeAssetsJSON andVersion:(int)oVersion;

@end