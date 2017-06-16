using System.Collections.Generic;

#if !UNITY3D && !UNITY_3D
using System.Threading.Tasks;
#endif

namespace TMS.Common.Modularity
{
	/// <summary>
	///     Interface for Bootstrapper
	/// </summary>
	public interface IBootstrapper
	{
		/// <summary>
		///     Gets the modules list.
		/// </summary>
		IDictionary<ModuleInfo, IModule> Modules { get; }

#if UNITY3D || UNITY_3D
		/// <summary>
		///     Loads the module catalog.
		/// </summary>
		/// <param name="catalogPath">The catalog path.</param>
		/// <param name="args">The args.</param>
		IDictionary<ModuleInfo, IModule> LoadModuleCatalog(string catalogPath, params object[] args);
#else
		Task<IDictionary<ModuleInfo, IModule>> LoadModuleCatalog(string catalogPath, params object[] args);
#endif

		/// <summary>
		/// Initializes the module.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		IModule InitializeModule(ModuleInfo info, params object[] args);
	}

	/// <summary>
	///     Interface for Bootstrapper
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IBootstrapper<T> : IBootstrapper
	{
		/// <summary>
		///     Gets the modules list.
		/// </summary>
		new IDictionary<ModuleInfo, IModule<T>> Modules { get; }

#if UNITY3D || UNITY_3D
		/// <summary>
		///     Loads the module catalog.
		/// </summary>
		/// <param name="catalogPath">The catalog path.</param>
		/// <param name="api">The API.</param>
		IDictionary<ModuleInfo, IModule<T>> LoadModuleCatalog(string catalogPath, T api);
#else
		Task<IDictionary<ModuleInfo, IModule<T>>> LoadModuleCatalog(string catalogPath, T api);
#endif

		/// <summary>
		/// Loads the modules.
		/// </summary>
		/// <param name="modules">The modules.</param>
		/// <param name="api">The API.</param>
		/// <returns></returns>
		IDictionary<ModuleInfo, IModule<T>> LoadModules(IEnumerable<ModuleInfo> modules, T api);

		/// <summary>
		/// Initializes the module.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="api">The API.</param>
		/// <returns></returns>
		IModule<T> InitializeModule(ModuleInfo info, T api);
	}
}