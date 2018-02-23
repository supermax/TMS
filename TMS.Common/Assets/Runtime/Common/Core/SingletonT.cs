namespace TMS.Common.Core
{
	/// <summary>
	///     Singleton Base Class
	/// </summary>
	/// <typeparam name="T">Type of Singleton</typeparam>
	public class Singleton<T> : Singleton<T, T>
		where T : class
	{

	}
}