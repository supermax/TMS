public class AppContext : IAppContext
{
	public int AppId { get; private set; }

	public string UserName { get; private set; }

	public AppContext()
	{
		AppId = GetHashCode();
		UserName = "TMS";
	}
}