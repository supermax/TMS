extern "C"
{
	uint64_t GetIosDiskSpace() {
		uint64_t totalSpace = 0;
		uint64_t totalFreeSpace = 0;
		NSError *error = nil;  
		NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);  
		NSDictionary *dictionary = [[NSFileManager defaultManager] attributesOfFileSystemForPath:[paths lastObject] error: &error];  
	
		if (dictionary) {  
			NSNumber *fileSystemSizeInBytes = [dictionary objectForKey: NSFileSystemSize];  
			NSNumber *freeFileSystemSizeInBytes = [dictionary objectForKey:NSFileSystemFreeSize];
			totalSpace = [fileSystemSizeInBytes unsignedLongLongValue];
			totalFreeSpace = [freeFileSystemSizeInBytes unsignedLongLongValue];
			NSLog(@"Memory Capacity of %llu MiB with %llu MiB Free memory available.", ((totalSpace/1024ll)/1024ll), ((totalFreeSpace/1024ll)/1024ll));
		} else {  
			NSLog(@"Error Obtaining System Memory Info: Domain = %@, Code = %ld", [error domain], (long)[error code]);
		}  
	
		return totalFreeSpace;
	}
}
