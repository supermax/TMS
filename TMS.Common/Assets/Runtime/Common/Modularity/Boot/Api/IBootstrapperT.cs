#define UNITY3D

using System.Collections.Generic;
using TMS.Common.Core;
using TMS.Common.Modularity.Boot;
using TMS.Common.Modularity.Boot.Api;

namespace TMS.Common.Modularity
{
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

		/// <summary>
		///     Loads the module catalog.
		/// </summary>
		/// <param name="catalogPath">The catalog path.</param>
		/// <param name="api">The API.</param>
#if UNITY3D || UNITY_3D
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