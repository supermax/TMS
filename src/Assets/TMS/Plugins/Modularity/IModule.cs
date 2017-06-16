namespace TMS.Common.Modularity
{
	/// <summary>
	/// </summary>
	public interface IModule
	{
	}

	/// <summary>
	///     Interface for Module Manager
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IModule<in T> : IModule
	{
		/// <summary>
		///     Initializes the module.
		/// </summary>
		/// <param name="api">The Exposed API.</param>
		void Initialize(T api);
	}
}