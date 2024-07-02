extern "C" {
    void _ShowShareSheet(const char* filePath) {
        NSString *path = [NSString stringWithUTF8String:filePath];
        NSURL *url = [NSURL fileURLWithPath:path];
        
        UIViewController *rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;

        // Create and present the UIActivityViewController with a sourceView or barButtonItem
        NSArray *activityItems = @[url];
        UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:activityItems applicationActivities:nil];
        
        // Set a sourceView or barButtonItem to avoid the exception
        activityViewController.popoverPresentationController.sourceView = rootViewController.view; // Replace with an appropriate view
        // Or
        // activityViewController.popoverPresentationController.barButtonItem = // Replace with an appropriate barButtonItem
        
        dispatch_async(dispatch_get_main_queue(), ^{
            [rootViewController presentViewController:activityViewController animated:YES completion:nil];
        });
    }
}
