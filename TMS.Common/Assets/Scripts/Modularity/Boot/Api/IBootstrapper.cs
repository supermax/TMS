#define UNITY3D

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

		/// <summary>
		///     Loads the module catalog.
		/// </summary>
		/// <param name="catalogPath">The catalog path.</param>
		/// <param name="args">The args.</param>
#if UNITY3D || UNITY_3D
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
}