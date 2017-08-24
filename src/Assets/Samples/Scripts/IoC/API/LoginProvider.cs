public abstract class LoginProvider : ILoginProvider
{
	public virtual IConfiguration Configuration { get; set; }

	public abstract void Login();

	public abstract void Logout();

	protected LoginProvider(IAppContext context)
	{
		
	}
}