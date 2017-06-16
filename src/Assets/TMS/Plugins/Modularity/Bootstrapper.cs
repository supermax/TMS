#region Usings

using TMS.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY3D || UNITY_3D
using UnityEngine;
using TMS.Common.Serialization.Json;
#else
using System.Threading.Tasks;
using System.Diagnostics;
#if !NETFX_CORE
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
#else
using TMS.Common.Core;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Markup;
#endif
#endif

#endregion

namespace TMS.Common.Modularity
{
	/// <summary>
	///     Bootstrapper
	/// </summary>
	public class Bootstrapper<T> : IBootstrapper<T>
	{
		/// <summary>
		///     Gets the modules list.
		/// </summary>
		public IDictionary<ModuleInfo, IModule<T>> Modules { get; private set; }

#if UNITY3D || UNITY_3D
		/// <summary>
		///     Loads the module catalog.
		/// </summary>
		/// <param name="catalogPath">The catalog path.</param>
		/// <param name="args">The args.</param>
		public IDictionary<ModuleInfo, IModule> LoadModuleCatalog(string catalogPath, params object[] args)
		{
			var api = args != null && args.Length > 0 && args[0] is T ? (T)args[0] : default(T);
			var res = LoadModuleCatalog(catalogPath, api);
			var conv = res.ToDictionary<KeyValuePair<ModuleInfo, IModule<T>>, ModuleInfo, IModule>(module => module.Key, module => module.Value);
			return conv;
		}
#else
		/// <summary>
		///     Loads the module catalog.
		/// </summary>
		/// <param name="catalogPath">The catalog path.</param>
		/// <param name="args">The args.</param>
		public async Task<IDictionary<ModuleInfo, IModule>> LoadModuleCatalog(string catalogPath, params object[] args)
		{
			var api = args != null && args.Length > 0 && args[0] is T ? (T) args[0] : default(T);
			var res = await LoadModuleCatalog(catalogPath, api);
			var conv = res.ToDictionary<KeyValuePair<ModuleInfo, IModule<T>>, ModuleInfo, IModule>(module => module.Key, module => module.Value);
			return conv;
		}
#endif

#if UNITY3D || UNITY_3D
		/// <summary>
		///     Loads the module catalog.
		/// </summary>
		/// <param name="catalogPath">The catalog path.</param>
		/// <param name="api">The API.</param>
		public IDictionary<ModuleInfo, IModule<T>> LoadModuleCatalog(string catalogPath, T api)
		{
			Modules = new Dictionary<ModuleInfo, IModule<T>>();
			var catalog = LoadModuleCatalogFromFile(catalogPath);

			foreach (var info in catalog)
			{
				try
				{
					var module = LoadModuleManager(info, api);
					Modules.Add(info, module);
				}
				catch (Exception ex)
				{
					Loggers.Default.ConsoleLogger.Write(ex);

					switch (info.PriorityType)
					{
						case ModulePriorityType.Critical:
						{
							throw new OperationCanceledException(
								string.Format(
								Properties.Resources.ErrMsg_ModuleLoading_Aborted, 
								info.ModuleName, info.PriorityType), ex);
						}
					}
				}
			}

			return Modules;
		}
#else
		/// <summary>
		///     Loads the module catalog.
		/// </summary>
		/// <param name="catalogPath">The catalog path.</param>
		/// <param name="api">The API.</param>
		public async Task<IDictionary<ModuleInfo, IModule<T>>> LoadModuleCatalog(string catalogPath, T api)
		{
			Modules = new Dictionary<ModuleInfo, IModule<T>>();
			var catalog = await LoadModuleCatalogFromFile(catalogPath);
			foreach (var info in catalog)
			{
				var module = LoadModuleManager(info, api);
				Modules.Add(info, module);
			}
			return Modules;
		}
#endif

		/// <summary>
		/// Loads the modules.
		/// </summary>
		/// <param name="modules">The modules.</param>
		/// <param name="api">The API.</param>
		/// <returns></returns>
		public IDictionary<ModuleInfo, IModule<T>> LoadModules(IEnumerable<ModuleInfo> modules, T api)
		{
			Modules = new Dictionary<ModuleInfo, IModule<T>>();
			foreach (var info in modules)
			{
				var module = LoadModuleManager(info, api);
				Modules.Add(info, module);
			}
			return Modules;
		}

		/// <summary>
		///     Loads the module manager.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="api">The API.</param>
		/// <returns></returns>
		private static IModule<T> LoadModuleManager(ModuleInfo info, T api)
		{
			IModule<T> res = null;
			switch (info.InitMethod)
			{
				case ModuleInitMethodType.None:
					if (info.ConfigureIoc)
					{
						LoadModuleAssembly(info);
					}
					break;

				case ModuleInitMethodType.Instance:
					res = InstantiateModuleManager(info);
					break;

				case ModuleInitMethodType.Init:
					res = InitializeModuleManager(info, api);
					break;
			}
			return res;
		}

		/// <summary>
		/// Initializes the module.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		IModule IBootstrapper.InitializeModule(ModuleInfo info, params object[] args)
		{
			var api = args != null && args.Length > 0 && args[0] is T ? (T)args[0] : default(T);
			var res = InitializeModule(info, api);
			return res;
		}

		public IModule<T> InitializeModule(ModuleInfo info, T api)
		{
			var moduleManager = Modules[info];
			switch (info.InitMethod)
			{
				case ModuleInitMethodType.None:
					if (info.ConfigureIoc)
					{
						LoadModuleAssembly(info);
					}
					break;

				case ModuleInitMethodType.Instance:
					if (moduleManager != null)
					{
						moduleManager = InstantiateModuleManager(info);
					}
					break;

				case ModuleInitMethodType.Init:
					if (moduleManager == null)
					{
						moduleManager = InitializeModuleManager(info, api);
					}
					else
					{
						moduleManager.Initialize(api);
					}
					break;
			}
			Modules[info] = moduleManager;
			return moduleManager;
		}

		/// <summary>
		///     Initializes the module manager.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="api">The API.</param>
		/// <returns></returns>
		private static IModule<T> InitializeModuleManager(ModuleInfo info, T api)
		{
			var obj = InstantiateModuleManager(info);
			obj.Initialize(api);
			return obj;
		}

		/// <summary>
		///     Instantiates the module manager.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns></returns>
		private static IModule<T> InstantiateModuleManager(ModuleInfo info)
		{
			Loggers.Default.ConsoleLogger.Write("Loading assembly: " + info.AssemblyPath);
			var ass = LoadModuleAssembly(info);
			var type = ass.GetType(info.ModuleClassName);
			var obj = IocManager.Default.Resolve<IModule<T>>(type, null);
			return obj;
		}

		/// <summary>
		///     Loads the module assembly.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns></returns>
		private static Assembly LoadModuleAssembly(ModuleInfo info)
		{
#if UNITY3D || UNITY_3D
			var ass = AssemblyLoader.LoadAssembly(info.AssemblyPath);
#elif !NETFX_CORE
			var ass = Assembly.Load(info.AssemblyPath);
#else
			var ass = Assembly.Load(new AssemblyName(info.AssemblyPath));
#endif
			if (info.ConfigureIoc)
			{
				IocManager.Default.Configure(ass);
			}
			return ass;
		}

#if UNITY3D || UNITY_3D
		/// <summary>
		///     Loads the module catalog from file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		private static IEnumerable<ModuleInfo> LoadModuleCatalogFromFile(string filePath)
		{
			var txt = Resources.Load<TextAsset>(filePath);
			var cat = JsonMapper.Default.ToObject<ModuleCatalog>(txt.text);
			return cat;
		}
#elif !NETFX_CORE
		/// <summary>
		///     Loads the module catalog from file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		private async static Task<IEnumerable<ModuleInfo>> LoadModuleCatalogFromFile(string filePath)
		{
			using (var reader = new StreamReader(filePath))
			{
#if !SILVERLIGHT
				var catalog = (ModuleCatalog)XamlReader.Load(reader.BaseStream);
#else
				var str = reader.ReadToEnd();
				var catalog = (ModuleCatalog) XamlReader.Load(str);
#endif
				return catalog;
			}
		}
#else
		/// <summary>
		///     Loads the module catalog from file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		private static async Task<IEnumerable<ModuleInfo>> LoadModuleCatalogFromFile(string filePath)
		{
			var installFolder = Package.Current.InstalledLocation;
			var file = await installFolder.GetFileAsync(filePath);
			var str = await FileIO.ReadTextAsync(file);

			ModuleCatalog catalog = null;
			var disp = DispatcherProxy.CreateDispatcher();
			if (disp != null)
			{
				await
					disp.Invoke((Action) (() => { catalog = (ModuleCatalog) XamlReader.Load(str); }), CoreDispatcherPriority.Normal);
			}
			return catalog;
		}
#endif

		/// <summary>
		///     Gets the modules.
		/// </summary>
		/// <value>
		///     The modules.
		/// </value>
		IDictionary<ModuleInfo, IModule> IBootstrapper.Modules
		{
			get
			{
				var b = (IBootstrapper<T>) this;
				var modules = b.Modules.ToDictionary<KeyValuePair<ModuleInfo, IModule<T>>, ModuleInfo, IModule>(
					module => module.Key, module => module.Value);
				return modules;
			}
		}
	}

#if UNITY3D || UNITY_3D
	internal static class AssemblyLoader
	{
#if !UNITY_WEBPLAYER
		private static Assembly[] _assemblyCache;

		internal static Assembly LoadAssembly(string name)
		{
			if (_assemblyCache == null)
			{
				_assemblyCache = AppDomain.CurrentDomain.GetAssemblies();
			}
			var ass = _assemblyCache.FirstOrDefault(assembly => assembly.GetName().Name == name);
			return ass;
		}
#else
		private static Assembly[] _assemblyCache;

		internal static Assembly LoadAssembly(string name)
		{
			if (_assemblyCache == null)
			{
				_assemblyCache = AppDomain.CurrentDomain.GetAssemblies();
			}
			var ass = _assemblyCache.FirstOrDefault(assembly => assembly.GetName().Name == name);
			return ass;
		}
#endif
	}
#endif
}