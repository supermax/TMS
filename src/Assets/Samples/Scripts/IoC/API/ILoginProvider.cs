public interface ILoginProvider
{
	IConfiguration Configuration { get; }

	void Login();

	void Logout();
}