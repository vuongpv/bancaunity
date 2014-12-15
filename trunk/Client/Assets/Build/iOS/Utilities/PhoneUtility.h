//
//  PhoneUtility.h
//  IOSPlugin
//
//  Created by An Ngo on 3/26/13.
//

#import <Foundation/Foundation.h>
#import <MessageUI/MessageUI.h>
#import <MessageUI/MFMessageComposeViewController.h>

@interface PhoneUtility : NSObject<MFMessageComposeViewControllerDelegate> {

}

@property (nonatomic, retain) UIViewController * viewController;

- (void) sendSMSWithNumber:(NSString*)number message:(NSString*)message;

@end
