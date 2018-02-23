namespace TMS.Common.Properties
{
	public static class Resources
	{
		public static string ErrMsg_CommandDelegatesCantBeNull
		{
			get;
			private set;
		}

		public static string ErrMsg_CantCalculateHash
		{
			get; 
			private set;
		}

		public static string ErrMsg_CantCalculateHash_DefaultEncodingRequired
		{
			get;
			private set;
		}

		public static string ErrMsg_ModuleLoading_Aborted
		{
			get; 
			private set; 
		}

		static Resources()
		{
			ErrMsg_CommandDelegatesCantBeNull = "Command delegates can't be Null.";
			ErrMsg_CantCalculateHash_DefaultEncodingRequired = "Unable to calculate hash over a string without a default encoding. Consider using the GetHash(string) overload to use UTF8 Encoding";
			ErrMsg_CantCalculateHash = "Unable to calculate hash over null input data";
			ErrMsg_ModuleLoading_Aborted = "Module loading process aborted.\r\n" +
			                               "Cannot load \"{0}\" that defined as '{1}'.\r\n" +
			                               "For more details, see inner exception.";
		}
	}
}