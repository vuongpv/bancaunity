//
//  FacebookHandle.m
//  IOSPlugin
//
//  Created by An Ngo on 3/26/13.
//  Copyright (c) 2013 Smilee. All rights reserved.
//

#import "FlurryHandle.h"
#import "Defines.h"
#import "Flurry.h"


@implementation FlurryHandle


extern "C" {
    
    const void flurry_startsession(const char *apiKey){
        [Flurry startSession:[NSString stringWithUTF8String:apiKey]];
    }
    
    
    const void flurry_stopsession(){
        //    [Flurry stopSession];
    }
    
    const void flurry_logevent(const char *msg){
        [Flurry logEvent:[NSString stringWithUTF8String:msg]];
    }
    
    const void flurry_logevent1(const char *msg, const char *paramKey1, const char *paramValue1){
        NSDictionary *params =
            [NSDictionary dictionaryWithObjectsAndKeys:
                [NSString stringWithUTF8String:paramValue1], [NSString stringWithUTF8String:paramKey1],
                nil];

        [Flurry logEvent:[NSString stringWithUTF8String:msg] withParameters:params];
    }
    
    const void flurry_logevent2(const char *msg, const char *paramKey1, const char *paramValue1, const char *paramKey2, const char *paramValue2){
        NSDictionary *params =
            [NSDictionary dictionaryWithObjectsAndKeys:
                [NSString stringWithUTF8String:paramValue1], [NSString stringWithUTF8String:paramKey1],
                [NSString stringWithUTF8String:paramValue2], [NSString stringWithUTF8String:paramKey2],
                nil];
        
        [Flurry logEvent:[NSString stringWithUTF8String:msg] withParameters:params];
    }
    
    const void flurry_logevent3(const char *msg, const char *paramKey1, const char *paramValue1, const char *paramKey2, const char *paramValue2, const char *paramKey3, const char *paramValue3){
        
        NSDictionary *params =
            [NSDictionary dictionaryWithObjectsAndKeys:
                [NSString stringWithUTF8String:paramValue1], [NSString stringWithUTF8String:paramKey1],
                [NSString stringWithUTF8String:paramValue2], [NSString stringWithUTF8String:paramKey2],
                [NSString stringWithUTF8String:paramValue3], [NSString stringWithUTF8String:paramKey3],
                nil];
        
        [Flurry logEvent:[NSString stringWithUTF8String:msg] withParameters:params];
    }
    
    
    
}


@end