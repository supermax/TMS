public class PlatformLoginProviderButtonAction :
#if UNITY_ANDROID
	LoginProviderButtonAction<GoogleLoginProvider>
#elif UNITY_IOS
	LoginProviderButtonAction<AppleLoginProvider>
#else
	//TODO
#endif
{

}