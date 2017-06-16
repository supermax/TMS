#region Usings

using TMS.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMS.Common.Extensions;

#if UNITY3D || UNITY_3D
using UnityEngine;
using Object = UnityEngine.Object;
#endif

#endregion

namespace TMS.Common.Modularity
{
	/// <summary>
	///     Custom and Simple IOC Manager
	/// </summary>
	public sealed class IocManager : IIocManager, IDisposable
	{
		private readonly IDictionary<Type, IEnumerable<ConstructorInfo>> _typeCtors =
			new Dictionary<Type, IEnumerable<ConstructorInfo>>();

		private readonly IDictionary<Type, TypeMappingInfo> _mappings = new Dictionary<Type, TypeMappingInfo>();

		private readonly IList<Assembly> _configuredAssemblies = new List<Assembly>();

		private static readonly IocManager Instance = new IocManager();

		private static readonly object Locker = new object();

		private static bool _isConfigured;

		/// <summary>
		/// Gets the default.
		/// </summary>
		/// <value>
		/// The default.
		/// </value>
		public static IocManager Default
		{
			get
			{
				if (!_isConfigured)
				{
					lock (Locker)
					{
						if (!_isConfigured)
						{
							_isConfigured = true;
							Instance.Configure(typeof(IocManager));
						}
					}
				}
				return Instance;
			}
		}

		/// <summary>
		///     Gets the mappings.
		/// </summary>
		private IDictionary<Type, TypeMappingInfo> Mappings
		{
			get { return _mappings; }
		}

		/// <summary>
		///     Registers the specified implementation type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="implementationType">Type of the implementation.</param>
		/// <param name="singleton">
		///     if set to <c>true</c> [singleton].
		/// </param>
		/// <param name="isWeakRef">
		///     if set to <c>true</c> [is weak ref].
		/// </param>
		public void Register<T>(Type implementationType, bool singleton = false, bool isWeakRef = false)
		{
			Register(typeof(T), implementationType, singleton, isWeakRef);
		}

		/// <summary>
		///     Registers the specified interface type.
		/// </summary>
		/// <param name="interfaceType">Type of the interface.</param>
		/// <param name="implementationType">Type of the implementation.</param>
		/// <param name="singleton">
		///     if set to <c>true</c> [singleton].
		/// </param>
		/// <param name="isWeakRef">
		///     if set to <c>true</c> [is weak ref].
		/// </param>
		public void Register(Type interfaceType, Type implementationType, bool singleton = false, bool isWeakRef = false)
		{
			Register(interfaceType, implementationType, singleton, null, isWeakRef, null);
		}

		public void Register<T>(T implementationInstance, bool isWeakRef = false) where T : class
		{
			Register<T, T>(implementationInstance, isWeakRef);
		}

		//public void Register<T>(object implementationInstance, bool isWeakRef = false) where T: class
		//{
		//	ArgumentValidator.AssertNotNullAndOfType<T>(implementationInstance, "implementationInstance");
		//	var success = ConfigureType(implementationInstance.GetType(), implementationInstance);
		//	if (success) return;

		//	Register(typeof(T), implementationInstance.GetType(), true, null, isWeakRef, implementationInstance);
		//}

		/// <summary>
		///     Registers the specified implementation instance.
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <param name="implementationInstance">The implementation instance.</param>
		/// <param name="isWeakRef">
		///     if set to <c>true</c> [is weak ref].
		/// </param>
		public void Register<TInterface, TImplementation>(TImplementation implementationInstance, bool isWeakRef = false) 
			where TImplementation : TInterface
		{
			ArgumentValidator.AssertNotDefault(implementationInstance, "implementationInstance");
			var success = ConfigureType(implementationInstance.GetType(), implementationInstance);
			if (success) return;

			Register(typeof(TInterface), typeof(TImplementation), true, null, isWeakRef, implementationInstance);
		}

		/// <summary>
		/// Unregisters the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>
		/// false in case did not find registered mapping for this type
		/// </returns>
		public bool Unregister<T>()
		{
			var key = typeof(T);
			if (!Mappings.ContainsKey(key)) return false;

			var map = Mappings[key];
			Mappings.Remove(key);
			map.Dispose();
			return true;
		}

		/// <summary>
		/// Unregisters the specified implementation instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="implementationInstance">The implementation instance.</param>
		/// <returns>
		/// false in case did not find registered mapping for this instance
		/// </returns>
		public bool Unregister<T>(object implementationInstance)
		{
			ArgumentValidator.AssertNotNull(implementationInstance, "implementationInstance");

			var key = typeof(T);
			if (!Mappings.ContainsKey(key)) return false;
			
			var map = Mappings[key];
			var typeKey = implementationInstance.GetType();
			if (map.ContainsKey(typeKey))
			{
				var typeMapping = map[typeKey];
				map.Remove(typeKey);
				typeMapping.Dispose();
			}
			if (map.Count > 0) return true;

			Mappings.Remove(key);
			map.Dispose();
			return true;
		}

		/// <summary>
		/// Unregisters all implementation instance(s).
		/// </summary>
		/// <param name="implementationInstance">The implementation instance.</param>
		/// <returns>
		/// false in case did not find registered mapping(s) for this instance
		/// </returns>
		public bool Unregister(object implementationInstance)
		{
			ArgumentValidator.AssertNotNull(implementationInstance, "implementationInstance");

			var typeKey = implementationInstance.GetType();
			var res = Mappings.Where(map => map.Value.ContainsKey(typeKey));
			if (res.IsNullOrEmpty()) return false;

			var ary = res.ToArray();
			foreach (var map in ary)
			{
				var typeMap = map.Value[typeKey];
				map.Value.Remove(typeKey);
				typeMap.Dispose();

				if(map.Value.Count > 0) continue;
				Mappings.Remove(map.Key);
			}

			return true;
		}

		//public T Resolve<T>(string implementationTypeName)
		//{
		//	// TODO
		//	//Mappings.GetFirstOrDefault()
		//	throw new NotImplementedException();
		//}

		/// <summary>
		///     Resolves the instance of type by the specified parameters.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="parameters"> The parameters. </param>
		/// <returns> </returns>
		public T Resolve<T>(params object[] parameters)
		{
			var res = Resolve(typeof(T), parameters);
			if (res == null) return default(T);
			return (T)res;
		}

		private object Resolve(Type key, params object[] parameters)
		{
			object instance;
			if (!Mappings.ContainsKey(key))
			{
#if (UNITY3D || UNITY_3D) && IOC_DEBUG
				Debug.LogFormat("Mapping does not contain the key {0}", key);
#endif
				instance = GetImplementationInstance(key, parameters);
#if (UNITY3D || UNITY_3D) && IOC_DEBUG
				Debug.LogFormat("GetImplementationInstance => {0}", instance);
#endif
				if (instance != null)
				{
					return instance;
				}

				instance = CreateInstance(key, false, parameters);
				return instance;
			}

			var map = Mappings[key].First().Value;
			if (map.IsSingleton && map.Instance != null)
			{
				instance = map.Instance;
			}
			else
			{
				var type = map.ImplementationType;
				instance = CreateInstance(type, map.IsSingleton, parameters);
				if (map.IsSingleton)
				{
					map.Instance = instance;
				}
				else if (map.TypeMapAttribute != null)
				{
					map.TypeMapAttribute.OnInstantiation(instance);
				}
			}
			return instance;
		}

		/// <summary>
		///     Resolves the specified registered key type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="registeredKeyType">Type registered type for the key.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns> </returns>
		public T Resolve<T>(Type registeredKeyType, params object[] parameters)
		{
			var res = Resolve(typeof(T), registeredKeyType, parameters);
			if (res == null) return default(T);
			return (T) res;
		}

		private object Resolve(Type key, Type registeredKeyType, params object[] parameters)
		{
			object instance;
			if (!Mappings.ContainsKey(key) && !Mappings[key].ContainsKey(registeredKeyType))
			{
				instance = GetImplementationInstance(registeredKeyType, parameters);
				if (instance != null) return instance;

				instance = CreateInstance(registeredKeyType, false, parameters);
				return instance;
			}

			var map = Mappings[key][registeredKeyType];
			if (map.IsSingleton && map.Instance != null)
			{
				instance = map.Instance;
			}
			else
			{
				var type = map.ImplementationType;
				instance = CreateInstance(type, map.IsSingleton, parameters);
				if (map.IsSingleton)
				{
					map.Instance = instance;
				}
				else if (map.TypeMapAttribute != null)
				{
					map.TypeMapAttribute.OnInstantiation(instance);
				}
			}
			return instance;
		}

		/// <summary>
		/// Configures the manager for the assembly be the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		public void Configure(Type type)
		{
#if !NETFX_CORE
			Configure(type.Assembly);
#else
			Configure(type.GetTypeInfo().Assembly);
#endif
		}

		/// <summary>
		/// Configures the manager for the assembly.
		/// </summary>
		/// <param name="srcAssembly">The assembly.</param>
		public void Configure(Assembly srcAssembly)
		{
			if (_configuredAssemblies.Contains(srcAssembly))
			{
#if UNITY_3D && DEBUG && IOC_DEBUG
				Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, string.Format("*** Configure -> _configuredAssemblies.Contains({0}) = TRUE", srcAssembly));
#endif
				return;
			}
			_configuredAssemblies.Add(srcAssembly);
#if UNITY_3D && DEBUG && IOC_DEBUG
			Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, string.Format("*** Configure -> _configuredAssemblies.Contains({0}) = FALSE", srcAssembly));
#endif

#if !NETFX_CORE
			var types = srcAssembly.GetTypes();
#else
			var types = srcAssembly.DefinedTypes;
#endif
			foreach (var type in types)
			{
				ConfigureType(type);
			}
		}

		private bool ConfigureType(Type type, object instance = null)
		{
			//#if UNITY_3D && DEBUG && IOC_DEBUG
			//				Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, "*** Configure -> " + type);
			//#endif
#if !NETFX_CORE
			var attribs = type.GetCustomAttributes(typeof(IocTypeMapAttribute), true) as IocTypeMapAttribute[];
#else
				var attribs = type.GetCustomAttributes<IocTypeMapAttribute>(true);
#endif
			if (attribs.IsNullOrEmpty())
			{
				//#if UNITY_3D && DEBUG && IOC_DEBUG
				//					var tmp = type.GetCustomAttributes(true);
				//					Loggers.ConsoleLogger.Write(LogSourceType.Trace, 
				//						"*** Configure -> CustomAttributes<IocTypeMapAttribute> = NULL | TMP = " + tmp.IsNullOrEmpty());
				//#endif
				return false;
			}

			foreach (var attr in attribs)
			{
				if (attr.MapAllInterfaces)
				{
#if !NETFX_CORE
					var interfaces = type.GetInterfaces();
#else
						var interfaces = type.ImplementedInterfaces;
#endif
					foreach (var @interface in interfaces)
					{
#if !NETFX_CORE
						Register(@interface, type, attr.IsSingleton, attr, attr.IsWeakReference, instance);
#else
						Register(@interface.GetTypeInfo().AsType(), type.AsType(), attr.IsSingleton, attr, attr.IsWeakReference, instance);
#endif
					}
					//continue;
				}
				if (attr.MappingTypes == null) continue;

				foreach (var mapType in attr.MappingTypes)
				{
#if !NETFX_CORE
					Register(mapType, type, attr.IsSingleton, attr, attr.IsWeakReference, instance);
#else
					Register(mapType, type.AsType(), attr.IsSingleton, attr, attr.IsWeakReference, instance);
#endif
#if UNITY_3D && DEBUG && IOC_DEBUG
					Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, 
						string.Format("*** Configure -> Register {0}, isSingleton={1}, instance={2}", mapType, attr.IsSingleton, instance));
#endif
				}
			}

			return true;
		}

		/// <summary>
		/// Registers the specified interface type.
		/// </summary>
		/// <param name="interfaceType">Type of the interface.</param>
		/// <param name="implementationType">Type of the implementation.</param>
		/// <param name="isSingleton">if set to <c>true</c> [singleton].</param>
		/// <param name="attribute">The attribute.</param>
		/// <param name="isWeakRef">if set to <c>true</c> [is weak ref].</param>
		/// <param name="instance">The instance of the type.</param>
		private void Register(Type interfaceType, Type implementationType, bool isSingleton,
			IocTypeMapAttribute attribute, bool isWeakRef, object instance)
		{
			TypeMappingInfo typeMapping;
			if (Mappings.ContainsKey(interfaceType))
			{
				// get existing mapping to update it
				typeMapping = Mappings[interfaceType];
			}
			else
			{
				// init new mapping and add to mappings collection
				typeMapping = new TypeMappingInfo();
				Mappings.Add(interfaceType, typeMapping);	
			}

			MappingInfo map;
			if (typeMapping.ContainsKey(implementationType))
			{
				map = typeMapping[implementationType];
			}
			else
			{
				map = new MappingInfo();
				typeMapping.Add(implementationType, map);
			}

			map.ImplementationType = implementationType;
			map.IsSingleton = isSingleton;
			map.IsWeakReference = isWeakRef;
			map.TypeMapAttribute = attribute;
			map.Instance = instance;

			if (attribute == null) return;

			attribute.OnRegistration(implementationType);
			if (!attribute.InstantiateOnRegistration) return;

			Resolve(interfaceType, implementationType);
		}

		/// <summary>
		///     Gets the implementation instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="registeredKeyType">Type of the registered key.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		private object GetImplementationInstance(Type registeredKeyType, params object[] parameters)
		{
			MappingInfo map = null;
			foreach (var item in Mappings)
			{
				if (!item.Value.ContainsKey(registeredKeyType)) continue;
				map = item.Value[registeredKeyType];
			}
			if (map == null) return null;

			if (map.Instance != null)
			{
				return map.Instance;
			}

			var instance = CreateInstance(map.ImplementationType, map.IsSingleton, parameters);
			if (map.IsSingleton)
			{
				map.Instance = instance;
			}
			return instance;
		}

		private IEnumerable<ConstructorInfo> GetTypeConstructors(Type type)
		{
			if (_typeCtors.ContainsKey(type))
			{
				return _typeCtors[type];
			}

#if !NETFX_CORE
			var ctors = type.GetConstructors();
#else
			var ctors = type.GetTypeInfo().DeclaredConstructors;
#endif
			_typeCtors[type] = ctors;
			return ctors;
		}

#if UNITY3D || UNITY_3D
		private const string RootObjName = "[SINGLETONS]";

		private static Object InstanciateMonoBehaviourObject(Type implementationType, bool isSingleton)
		{
			var instance = isSingleton ? FindObjectOfType(implementationType) : null;
			if (instance == null)
			{
				GameObject rootObj;
				if (isSingleton)
				{
					rootObj = GameObject.Find(RootObjName);
					if (rootObj == null)
					{
						rootObj = new GameObject(RootObjName);
						Debug.LogFormat("Created {0}", rootObj.name);
						if (Application.isPlaying)
						{
							Object.DontDestroyOnLoad(rootObj);
						}
					}
				}
				else
				{
					rootObj = new GameObject(implementationType.Name);
					Debug.LogFormat("Created {0}", rootObj.name);
				}

				instance = rootObj.AddComponent(implementationType);
				if (instance == null)
				{
					Debug.LogErrorFormat(
						"{1} Cannot instantiate: {0}",
						implementationType.Name, rootObj.name);
					return null;
				}

				Debug.LogFormat(
					"{2} Instance of '{0}' added to '{1}'", 
					implementationType.Name, rootObj, rootObj.name);
			}
			else
			{
				Debug.LogWarningFormat(
					"{0} Using instance already created '{1}' for {2}",
					RootObjName, instance, implementationType);
			}
			return instance;
		}

		private static Object FindObjectOfType(Type implementationType)
		{
			var res = Object.FindObjectsOfType(implementationType);
			if (res.IsNullOrEmpty()) return null;

			if (res.Length > 1)
			{
				Debug.LogWarning(RootObjName +
						" ERROR! There should never be more than 1 singleton of " +
						implementationType.Name +
						" Reopening the scene might fix it.");
			}
			return res[0];
		}
#endif

		private object CreateInstance(Type implementationType, bool isSingleton, params object[] parameters)
		{
#if UNITY3D || UNITY_3D
			if (typeof(Component).IsAssignableFrom(implementationType))
			{
				//Debug.LogFormat("{0} trying to create instance for {1}.", RootObjName, implementationType);
				var obj = InstanciateMonoBehaviourObject(implementationType, isSingleton);
				var res = Convert.ChangeType(obj, implementationType);
				return res;
			}
			//Debug.LogFormat("trying to create instance for {0}.", implementationType);
#endif
			var ctors = GetTypeConstructors(implementationType);
			var ctor = GetMatchingConstructor(ctors, parameters);
			var instance = CreateInstance(ctor, isSingleton, parameters);
			if(instance != null) return instance;

			foreach (var ctor1 in ctors)
			{
				instance = CreateInstance(ctor1, isSingleton, parameters);
				if (instance != null) return instance;
			}
			return null;
		}

		private object CreateInstance(ConstructorInfo ctor, bool isSingleton, params object[] parameters)
		{
			if (ctor == null || ctor.IsStatic || !ctor.IsPublic) return null;

			var paramsInfo = ctor.GetParameters();
			if (parameters.IsNullOrEmpty())
			{
				if (paramsInfo.IsNullOrEmpty())
				{
					var instance = ctor.Invoke(null);
					return instance;
				}

				var args = new object[paramsInfo.Length];
				for (var i = 0; i < paramsInfo.Length; i++)
				{
					if (!Mappings.ContainsKey(paramsInfo[i].ParameterType)) continue;
					var mapping = Mappings[paramsInfo[i].ParameterType];
					args[i] = CreateInstance(mapping.First().Value.ImplementationType, isSingleton);
				}

				var instance1 = ctor.Invoke(args);
				return instance1;
			}

			var args1 = new object[paramsInfo.Length];
			for (var i = 0; i < paramsInfo.Length; i++)
			{
				if (i < parameters.Length)
				{
					args1[i] = parameters[i];
				}
				else
				{
					if (!Mappings.ContainsKey(paramsInfo[i].ParameterType)) continue;
					var mapping = Mappings[paramsInfo[i].ParameterType];
					args1[i] = CreateInstance(mapping.First().Value.ImplementationType, isSingleton);
				}
			}
			var instance2 = ctor.Invoke(args1);
			return instance2;
		}

		private static ConstructorInfo GetMatchingConstructor(IEnumerable<ConstructorInfo> ctors, object[] args)
		{
			if (ctors.IsNullOrEmpty()) return null;

			// no arguments were passed, will use default constructor
			if (args.IsNullOrEmpty())
			{
				foreach (var ctor in ctors)
				{ 
					if(!ctor.IsPublic || ctor.IsStatic) continue;

					var paramsInfo = ctor.GetParameters();
					if(!paramsInfo.IsNullOrEmpty()) continue;

					return ctor;
				}
				return null;
			}

			// arguments were passed, will try to find matching constructor
			foreach (var ctor in ctors)
			{
				if (!ctor.IsPublic || ctor.IsStatic) continue;

				var paramsInfo = ctor.GetParameters();
				if (paramsInfo.IsNullOrEmpty() || paramsInfo.Length != args.Length) continue;

				var match = true;
				for (var i = 0; i < paramsInfo.Length; i++)
				{
					var paramInfo = paramsInfo[i];
					if(paramInfo == null) break;

					var arg = args[i];
					var argType = arg != null ? arg.GetType() : paramInfo.ParameterType;
					var isAssignable = paramInfo.ParameterType.IsAssignableFrom(argType);
					if (isAssignable) continue;

					match = false;
					break;
				}
				if(!match) continue;

				return ctor;
			}
			return null;
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		public void Dispose()
		{
			foreach (var mapping in _mappings)
			{
				mapping.Value.Dispose();
			}
			_mappings.Clear();

			_typeCtors.Clear();
			_configuredAssemblies.Clear();
			_isConfigured = false;
		}
	}
}