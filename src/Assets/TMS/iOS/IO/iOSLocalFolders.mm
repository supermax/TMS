#include <stdint.h>


char *copyString(const char *string){
    char *copy = (char *)malloc(strlen(string) + 1);
    strcpy(copy, string);
    return copy;
}

#if __cplusplus
extern "C" {
#endif
    NSString *cachesPath(){
        NSString *cachePath = [NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES) firstObject];
        cachePath = [cachePath stringByAppendingPathComponent:@"slotsClubCaches"];
        if (![[NSFileManager defaultManager] fileExistsAtPath:cachePath]){
            [[NSFileManager defaultManager] createDirectoryAtPath:cachePath withIntermediateDirectories:YES attributes:nil error:NULL];
        }
        return cachePath;
    }
    
    char *GetCachesPathNative(){
        const char *cStr = [cachesPath() cStringUsingEncoding:NSUTF8StringEncoding];
        char *returnString = copyString(cStr);
        return returnString;
    }
    
    UInt64 GetAvailableFreeSpaceNative(){
        NSFileManager *fm = [NSFileManager defaultManager];
        NSDictionary *fileAttributes = [fm attributesOfFileSystemForPath:cachesPath() error:NULL];
        NSNumber *freeSpaceObj = fileAttributes[NSFileSystemFreeSize];
        return [freeSpaceObj unsignedLongLongValue];
    }
    
    void DeleteCacheFolderNative(){
        NSString *cachePath = cachesPath();
        NSArray *contents = [[NSFileManager defaultManager] contentsOfDirectoryAtPath:cachePath error:nil];
        for (NSString *file in contents){
            [[NSFileManager defaultManager] removeItemAtPath:[cachePath stringByAppendingPathComponent:file] error:NULL];
        }
    }
#if __cplusplus
}
#endif
