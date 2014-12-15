#import "UnityStoreAssets.h"
#import "JSONConsts.h"
#import "VirtualCurrencyPack.h"
#import "VirtualCurrency.h"
#import "VirtualGood.h"
#import "VirtualCategory.h"
#import "NonConsumableItem.h"
#import "SingleUseVG.h"
#import "LifetimeVG.h"
#import "EquippableVG.h"
#import "SingleUsePackVG.h"
#import "UpgradeVG.h"
#import "StoreUtils.h"


@implementation UnityStoreAssets

@synthesize virtualCategoriesArray, virtualCurrenciesArray, virtualCurrencyPacksArray, virtualGoodsArray, nonConsumablesArray;

static NSString* TAG = @"SOOMLA UnityStoreAssets";

- (id)initWithStoreAssetsJSON:(NSString*)storeAssetsJSON andVersion:(int)oVersion{
	self = [super init];
	if(self){
		version = oVersion;
		[self createFromJSON:storeAssetsJSON];
	}
	return self;
}

- (BOOL)createFromJSON:(NSString*)storeAssetsJSON {
    LogDebug(TAG, ([NSString stringWithFormat:@"the storeAssets json is %@", storeAssetsJSON]));
   
    @try {

        NSDictionary* storeInfo = [StoreUtils jsonStringToDict:storeAssetsJSON];
        
        NSMutableArray* currencies = [[[NSMutableArray alloc] init] autorelease];
        NSArray* currenciesDicts = [storeInfo objectForKey:JSON_STORE_CURRENCIES];
        for(NSDictionary* currencyDict in currenciesDicts){
            VirtualCurrency* o = [[VirtualCurrency alloc] initWithDictionary: currencyDict];
            [currencies addObject:o];
            [o release];
        }
        virtualCurrenciesArray = currencies;
        
        NSMutableArray* currencyPacks = [[[NSMutableArray alloc] init] autorelease];
        NSArray* currencyPacksDicts = [storeInfo objectForKey:JSON_STORE_CURRENCYPACKS];
        for(NSDictionary* currencyPackDict in currencyPacksDicts){
            VirtualCurrencyPack* o = [[VirtualCurrencyPack alloc] initWithDictionary: currencyPackDict];
            [currencyPacks addObject:o];
            [o release];
        }
        virtualCurrencyPacksArray = currencyPacks;
        
        
        NSDictionary* goodsDict = [storeInfo objectForKey:JSON_STORE_GOODS];
        NSArray* suGoods = [goodsDict objectForKey:JSON_STORE_GOODS_SU];
        NSArray* ltGoods = [goodsDict objectForKey:JSON_STORE_GOODS_LT];
        NSArray* eqGoods = [goodsDict objectForKey:JSON_STORE_GOODS_EQ];
        NSArray* upGoods = [goodsDict objectForKey:JSON_STORE_GOODS_UP];
        NSArray* paGoods = [goodsDict objectForKey:JSON_STORE_GOODS_PA];
        NSMutableArray* goods = [[[NSMutableArray alloc] init] autorelease];
        for(NSDictionary* gDict in suGoods){
            SingleUseVG* g = [[SingleUseVG alloc] initWithDictionary: gDict];
			[goods addObject:g];
            [g release];
        }
        for(NSDictionary* gDict in ltGoods){
            LifetimeVG* g = [[LifetimeVG alloc] initWithDictionary: gDict];
			[goods addObject:g];
            [g release];
        }
        for(NSDictionary* gDict in eqGoods){
            EquippableVG* g = [[EquippableVG alloc] initWithDictionary: gDict];
			[goods addObject:g];
        }
        for(NSDictionary* gDict in upGoods){
            UpgradeVG* g = [[UpgradeVG alloc] initWithDictionary: gDict];
			[goods addObject:g];
            [g release];
        }
        for(NSDictionary* gDict in paGoods){
            SingleUsePackVG* g = [[SingleUsePackVG alloc] initWithDictionary: gDict];
			[goods addObject:g];
            [g release];
        }
        virtualGoodsArray = goods;
        
        NSMutableArray* categories = [[[NSMutableArray alloc] init] autorelease];
        NSArray* categoriesDicts = [storeInfo objectForKey:JSON_STORE_CATEGORIES];
        for(NSDictionary* categoryDict in categoriesDicts){
            VirtualCategory* c = [[VirtualCategory alloc] initWithDictionary: categoryDict];
            [categories addObject:c];
            [c release];
        }
        virtualCategoriesArray = categories;
        
        NSMutableArray* nonConsumables = [[[NSMutableArray alloc] init] autorelease];
        NSArray* nonConsumableItemsDict = [storeInfo objectForKey:JSON_STORE_NONCONSUMABLES];
        for(NSDictionary* nonConsumableItemDict in nonConsumableItemsDict){
            NonConsumableItem* non = [[NonConsumableItem alloc] initWithDictionary:nonConsumableItemDict];
            [nonConsumables addObject:non];
            [non release];
        }
        nonConsumablesArray = nonConsumables;
        
        return YES;
    } @catch (NSException* ex) {
        LogError(TAG, @"An error occured while trying to parse store assets JSON.");
    }
    
    return NO;
}

- (int)getVersion{
	return version;
}

- (NSArray*)virtualCurrencies{
    return virtualCurrenciesArray;
}

- (NSArray*)virtualGoods{
    return virtualGoodsArray;
}

- (NSArray*)virtualCurrencyPacks{
    return virtualCurrencyPacksArray;
}

- (NSArray*)virtualCategories{
    return virtualCategoriesArray;
}

- (NSArray*)nonConsumableItems {
    return nonConsumablesArray;
}

- (void)dealloc {
    [virtualCurrenciesArray release];
    [virtualGoodsArray release];
    [virtualCurrencyPacksArray release];
    [virtualCategoriesArray release];
    [nonConsumablesArray release];
    [super dealloc];
}

@end
