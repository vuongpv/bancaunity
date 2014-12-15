//
//  GoogleAnalytics.h
//  Plugin
//

#import <Foundation/Foundation.h>
#import "GAI.h"
#import "GAITracker.h"

@interface GoogleAnalytics : NSObject {
    id<GAITracker> tracker;
}

- (id) initWithDispatchPeriod:(int) dispatchPeriod debug:(BOOL) debug;
- (void) startTrackerWithAccountID:(NSString *) acountID;
- (void) sendView:(NSString *) pageName;
- (void) sendEvent:(NSString *) category action:(NSString *) action label:(NSString *)label value:(long) value;
- (void) sendTransaction:(NSString *)orderID totalPrice:(long)totalPrice withAffiliation:(NSString *)affiliation totalTax:(long)totalTax shippingCost:(long)shippingCost;
- (void) dispatch;
- (void) stopTracker;

@end