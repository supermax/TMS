namespace TMS.Common.Network.Api
{
	public interface IServicePayload
	{
		object Tag { get; set; }

		string ServiceName { get; set; }

		bool IsCustomService { get; set; }

		bool IsHandled { get; set; }
	}
}