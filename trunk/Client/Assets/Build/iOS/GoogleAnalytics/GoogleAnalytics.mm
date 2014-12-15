//
//  GoogleAnalytics.mm
//  Plugin

#import "GoogleAnalytics.h"
#import "GAI.h"
#import "GAIFields.h"
#import "GAITracker.h"
#import "GAIDictionaryBuilder.h"


@implementation GoogleAnalytics


- (id) initWithDispatchPeriod:(int) dispatchPeriod debug:(BOOL) debug {
	
    self = [super init]; //call to default super constructor
    
    if (self) { //check that that construction did not return a nil object.
        // Optional: automatically send uncaught exceptions to Google Analytics.
        [GAI sharedInstance].trackUncaughtExceptions = YES;
        // Optional: set Google Analytics dispatch interval to e.g. 20 seconds.
        [GAI sharedInstance].dispatchInterval = dispatchPeriod;
        // Optional: set debug to YES for extra debugging information.
        if( debug )
             [[[GAI sharedInstance] logger] setLogLevel:kGAILogLevelVerbose];
    }
    return self;
}

- (void) startTrackerWithAccountID:(NSString *)accountID {
	
    // Create tracker instance.
    tracker = [[GAI sharedInstance] trackerWithTrackingId:accountID];
}

- (void) sendView:(NSString *) viewName {
    
    [tracker set:kGAIScreenName value:viewName];

    [tracker send:[[GAIDictionaryBuilder createAppView] build]];
}

- (void) sendEvent:(NSString *) category action:(NSString *) action label:(NSString *)label value:(long) value{
    
    [tracker send:[[GAIDictionaryBuilder createEventWithCategory:category action:action label:label value:[NSNumber numberWithLong:value]] build]];
}

- (void) dispatch{
    
    [[GAI sharedInstance] dispatch];
}

- (void) stopTracker{
    

}

@end


extern "C"{
    
	static GoogleAnalytics *googleAnalytics = nil;
    
    void ga_init(int dispatchPeriod, bool debug){
		
        NSLog(@"Init ga");
        
        googleAnalytics = [[GoogleAnalytics alloc] init];
        [googleAnalytics initWithDispatchPeriod:dispatchPeriod debug:debug];
	}
    
    void ga_startTracker(const char *accountID){
		
        NSLog(@"Start tracker");
        
        NSString * account = [NSString stringWithUTF8String:accountID];
        [googleAnalytics startTrackerWithAccountID:account];
	}
    
    void ga_sendView(const char *viewName){
		
		NSString *view = [NSString stringWithUTF8String:viewName];
        if( ![view hasPrefix:@"/"] )
            view = [@"/" stringByAppendingString:view];
        
        [googleAnalytics sendView:view];
	}
    
    void ga_sendEvent(const char *category, const char *action, const char *label, int value){
        
        NSString *_category = [NSString stringWithUTF8String:category];
        NSString *_action = [NSString stringWithUTF8String:action];
        NSString *_label = [NSString stringWithUTF8String:label];
        [googleAnalytics sendEvent:_category action:_action label:_label value:value];
    }
    
    
    void ga_dispatch(){
        
        [googleAnalytics dispatch];
    }
    
    void ga_stopTracker(){
        
        [googleAnalytics stopTracker];
    }
}
