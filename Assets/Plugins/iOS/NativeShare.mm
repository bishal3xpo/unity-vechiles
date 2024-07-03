#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern "C" {
    void _NativeShare(const char* filePath) {
        NSString *path = [NSString stringWithUTF8String:filePath];
        NSURL *url = [NSURL fileURLWithPath:path];
        
        UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:@[url] applicationActivities:nil];
        
        UIViewController *rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
        
        dispatch_async(dispatch_get_main_queue(), ^{
            if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone) {
                [rootViewController presentViewController:activityViewController animated:YES completion:nil];
            } else {
                // For iPad
                UIPopoverPresentationController *popover = activityViewController.popoverPresentationController;
                if (popover) {
                    popover.sourceView = rootViewController.view;
                    popover.sourceRect = CGRectMake(rootViewController.view.frame.size.width/2, rootViewController.view.frame.size.height/4, 0, 0);
                    popover.permittedArrowDirections = UIPopoverArrowDirectionAny;
                }
                [rootViewController presentViewController:activityViewController animated:YES completion:nil];
            }
        });
    }
}