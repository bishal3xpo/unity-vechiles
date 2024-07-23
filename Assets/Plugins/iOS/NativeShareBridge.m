#import <Foundation/Foundation.h>
#import "NativeShare.h"

#ifdef __cplusplus
extern "C" {
#endif

void _NativeShareFile(const char* filePath) {
    NSString *path = [NSString stringWithUTF8String:filePath];
    [NativeShare shareFile:path];
}

#ifdef __cplusplus
}
#endif
