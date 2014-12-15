
#import "PhoneUtility.h"
#import "Defines.h"


@implementation PhoneUtility

- (void) sendSMSWithNumber:(NSString*) number message:(NSString*) textMessage {
    
    MFMessageComposeViewController *controller = [[MFMessageComposeViewController alloc] init];
    if([MFMessageComposeViewController canSendText]){
        
        UIWindow* window = [UIApplication sharedApplication].keyWindow;
        if(!window) 
            window = [[UIApplication sharedApplication].windows objectAtIndex:0];
        
        UIView *rootView = [[window subviews] objectAtIndex:0];  
        id nextResponder = [rootView nextResponder];
        
        if([nextResponder isKindOfClass:[UIViewController class]]){
            
            [[UIApplication sharedApplication] setStatusBarHidden:FALSE withAnimation:UIStatusBarAnimationSlide];
            self.viewController = nextResponder;
            controller.body = textMessage;
            controller.recipients = [NSArray arrayWithObjects:number , nil];
            controller.messageComposeDelegate = self;
            controller.wantsFullScreenLayout = YES;
            [self.viewController presentModalViewController:controller animated:YES];
        }
    }
    else
    {
        UnitySendMessage("PhoneUtilityBinding", "phone_sms", UNITY_EVENT_STATUS_UNSUPPORTED);
    }
    [controller release];
}

- (void)messageComposeViewController:(MFMessageComposeViewController *)controller didFinishWithResult:(MessageComposeResult)result {
    
    [_viewController dismissModalViewControllerAnimated:YES];
    _viewController=nil;
    
    [[UIApplication sharedApplication] setStatusBarHidden:TRUE withAnimation:UIStatusBarAnimationSlide];
    
    if (result == MessageComposeResultCancelled){
        
        UnitySendMessage("PhoneUtilityBinding", "phone_sms", UNITY_EVENT_STATUS_CANCELLED);
        
    }else if (result == MessageComposeResultSent){
        
        UnitySendMessage("PhoneUtilityBinding", "phone_sms", UNITY_EVENT_STATUS_OK);
    }
    else {
        
        UnitySendMessage("PhoneUtilityBinding", "phone_sms", UNITY_EVENT_STATUS_ERROR);
    }
}

@end

extern "C"{
    
    static PhoneUtility * phoneUitlity = nil;
   
    void phone_init() {
        
        if(!phoneUitlity)
            phoneUitlity = [[PhoneUtility alloc]init];
    }
    
    void phone_sendsms(const char * number, const char * message){     
        [phoneUitlity sendSMSWithNumber:[NSString stringWithUTF8String:number] message:[NSString stringWithUTF8String:message]];
    }

}