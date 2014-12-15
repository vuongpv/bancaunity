//
//  FacebookHandle.m
//  IOSPlugin
//
//  Created by An Ngo on 3/26/13.
//  Copyright (c) 2013 Smilee. All rights reserved.
//

#import "FacebookHandle.h"
#import "Defines.h"


@implementation FacebookHandle

- (id) initWithAppId:(NSString*) appId permissions:(NSString*) permissions; {
    if(self = [super init])
    {
        self.appID = appId;
        self.permissions = permissions;
    }
    return self;
}

- (void)dealloc {
    
    self.appID = nil;
    self.permissions = nil;
    self.accessToken = nil;
    
    [super dealloc];
}

- (void) createNewSession
{
    FBSession* session = [[FBSession alloc] init];
    [FBSession setActiveSession:session];
}

- (void) login
{
    NSArray *permissions = [[NSArray alloc] initWithObjects:@"email", nil];
    [FBSession openActiveSessionWithReadPermissions:permissions allowLoginUI:true completionHandler:^(FBSession *session, FBSessionState status, NSError *error)
    {
         if( status == FBSessionStateClosedLoginFailed || status == FBSessionStateCreatedOpening)
         {
            [[FBSession activeSession] closeAndClearTokenInformation];
            [FBSession setActiveSession:nil];
             [self createNewSession];
        }
        else
        {
           
        }                  
         
    }];
}

- (void) logout
{
    
}

- (void) postNewFeedWithName:(NSString*)name caption:(NSString*)caption description:(NSString*)desc imgurl:(NSString*)imgurl link:(NSString*)link;
{
    // This function will invoke the Feed Dialog to post to a user's Timeline and News Feed
    // It will attemnt to use the Facebook Native Share dialog
    // If that's not supported we'll fall back to the web based dialog.

    
    // Prepare the native share dialog parameters
    FBShareDialogParams *shareParams = [[FBShareDialogParams alloc] init];
    shareParams.link = [NSURL URLWithString:link];
    shareParams.name = name;
    shareParams.caption= caption;
    shareParams.picture= [NSURL URLWithString:imgurl];
    shareParams.description = desc;
    
    if ([FBDialogs canPresentShareDialogWithParams:shareParams]){
        
        [FBDialogs presentShareDialogWithParams:shareParams
                                    clientState:nil
                                        handler:^(FBAppCall *call, NSDictionary *results, NSError *error) {
                                            if(error) {
                                                NSLog(@"Error publishing story.");
                                                UnitySendMessage("FacebookBinding", "facebook_shareresult", UNITY_EVENT_STATUS_ERROR);
                                            } else if (results[@"completionGesture"] && [results[@"completionGesture"] isEqualToString:@"cancel"]) {
                                                NSLog(@"User canceled story publishing.");
                                                UnitySendMessage("FacebookBinding", "facebook_shareresult", UNITY_EVENT_STATUS_CANCELLED);
                                            } else {
                                                NSLog(@"Story published.");
                                                UnitySendMessage("FacebookBinding", "facebook_shareresult", UNITY_EVENT_STATUS_OK);
                                            }
                                        }];
        
    }else {
        
        UnitySendMessage("FacebookBinding", "facebook_webdialog", UNITY_EVENT_STATUS_OK);
        
        // Prepare the web dialog parameters
        // stackoverflow.com/questions/15849661/ios-facebook-sdk-3-2-1-feed-post-error
        NSDictionary *params = @{
                                 @"app_id": self.appID,
                                 @"name" : name,
                                 @"caption" : caption,
                                 @"description" : desc,
                                 @"picture" : imgurl,
                                 @"link" : link
                                 };
        
        // Invoke the dialog
        [FBWebDialogs presentFeedDialogModallyWithSession:nil
                                               parameters:params
                                                  handler:
         ^(FBWebDialogResult result, NSURL *resultURL, NSError *error) {
             if (error) {
                 NSLog(@"Error publishing story.");
                 UnitySendMessage("FacebookBinding", "facebook_shareresult", UNITY_EVENT_STATUS_ERROR);
             } else {
                 if (result == FBWebDialogResultDialogNotCompleted) {
                     NSLog(@"User canceled story publishing.");
                     UnitySendMessage("FacebookBinding", "facebook_shareresult", UNITY_EVENT_STATUS_CANCELLED);
                 } else {
                     NSLog(@"Story published.");
                     UnitySendMessage("FacebookBinding", "facebook_shareresult", UNITY_EVENT_STATUS_OK);
                 }
             }
             UnitySendMessage("FacebookBinding", "facebook_webdialog", UNITY_EVENT_STATUS_CANCELLED);
         }];
    }
}


@end


extern "C"{
    
    static FacebookHandle * facebook  = nil;
    
    void facebook_init( const char *  appID, const char *  permissions )
    {
        if(!facebook) {
	        NSString *_appID = [NSString stringWithUTF8String:appID];
            facebook = [[FacebookHandle alloc] initWithAppId:_appID permissions:[NSString stringWithUTF8String:permissions]];
            [FBSettings setDefaultAppID:_appID];
            [FBAppEvents activateApp];
		}
    }
    
    const char * facebook_accessToken()
    {
        return [facebook.accessToken UTF8String];
    }
    
    void facebook_login()
    {
        [facebook login];
    }
    
    void facebook_logout()
    {
        [facebook logout];
    }
    
    void facebook_postNewFeed(const char *name, const char *caption, const char *description, const char *imgurl, const char *link)
    {
        NSString *_name = [NSString stringWithUTF8String:name];
        NSString *_caption = [NSString stringWithUTF8String:caption];
        NSString *_description = [NSString stringWithUTF8String:description];
        NSString *_imgurl = [NSString stringWithUTF8String:imgurl];
        NSString *_link = [NSString stringWithUTF8String:link];
        
        [facebook postNewFeedWithName:_name caption:_caption description:_description imgurl:_imgurl link:_link];
    }
    
    void facebook_publishAppInstall()
    {
    }
    
    void facebook_destroy() {
        
        [facebook release];
        facebook = nil;
    }

    
}