namespace TMS.Common.Network.Response.Api
{
	public enum ServiceResponseState
	{
		Unknown = 0,

		Success,

		ServerError,

		ClientError,

		AbortedByClient,

		AbortedByServer,

		ConnectionTimeout, // TODO

		InternetReachabilityError, // TODO,
		
		Processing
	}
}