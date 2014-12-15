//
//  FacebookHandle.h
//  IOSPlugin
//
//  Created by An Ngo on 3/26/13.
//  Copyright (c) 2013 Smilee. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <FacebookSDK/FacebookSDK.h>

@interface FacebookHandle : NSObject

@property (nonatomic, retain) NSString * appID;
@property (nonatomic, retain) NSString * permissions;

@property (nonatomic, retain) NSString * accessToken;

- (id) initWithAppId:(NSString*) appId permissions:(NSString*) permissions;

- (void) createNewSession;
- (void) login;
- (void) logout;
- (void) postNewFeedWithName:(NSString*)name caption:(NSString*)caption description:(NSString*)desc imgurl:(NSString*)imgurl link:(NSString*)link;

@end
