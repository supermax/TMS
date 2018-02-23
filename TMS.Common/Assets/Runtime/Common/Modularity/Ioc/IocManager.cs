#define UNITY3D

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

#if UNITY_EDITOR
using TMS.Common.Modularity.Unity;
#endif
#endif

#if UNITY_WSA && !UNITY_EDITOR
using TypeExtensions = NUnit.Compatibility.TypeExtensions;
#endif

#endregion

namespace TMS.Common.Modularity
{
	/*
	 TODO

		>> in case we are running in unity editor:
			a) print list of none mono-beavior instrances in some component
			B) ...

		>> in case we are not in running in unity editor:
			a) do not show list of singletons, hide game object

	 */

	/// <summary>
	///     Custom and Simple IOC Manager
	/// </summary>
	public sealed class IocManager : IIocManager, IDisposable
	{
		private readonly IDictionary<Type, IEnumerable<ConstructorInfo>> _typeCtors =
			new Dictionary<Type, IEnumerable<ConstructorInfo>>();

		private readonly IDictionary<Type, IEnumerable<PropertyInfo>> _typeProps =
			new Dictionary<Type, IEnumerable<PropertyInfo>>();

		private readonly IDictionary<Type, TypeMappingInfo> _mappings = new Dictionary<Type, TypeMappingInfo>();

		private readonly IList<Assembly> _configuredAssemblies = new List<Assembly>();

		private static readonly IocManager Instance = new IocManager();

		private static readonly object Locker = new object();

		private static bool _isConfigured;

		private bool _autoConfig = true;

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
		/// Prevents a default instance of the <see cref="IocManager"/> class from being created.
		/// </summary>
		private IocManager()
		{
		}

		/// <summary>
		///     Gets the mappings.
		/// </summary>
		private IDictionary<Type, TypeMappingInfo> Mappings
		{
			get { return _mappings; }
		}

		/// <inheritdoc />
		/// <summary>
		/// Gets or sets a value indicating whether [automatically configure assembly of resolved type].
		/// </summary>
		/// <value>
		///   <c>true</c> if [automatic configure]; otherwise, <c>false</c>.
		/// </value>
		public bool AutoConfigure
		{
			get { return _autoConfig; }
			set { _autoConfig = value; }
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

		/// <summary>
		/// Registers the specified implementation instance.
		/// </summary>
		/// <typeparam name="TInterface">The type of the interface.</typeparam>
		/// <typeparam name="TImplementation">The type of the implementation.</typeparam>
		/// <param name="implementationInstance">The implementation instance.</param>
		/// <param name="isWeakRef">if set to <c>true</c> [is weak ref].</param>
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

			// TODO TEMP
#if UNITY_EDITOR && UNITY3D
			RemoveFromSingletonsList(key.Name);
#endif
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

				// TODO TEMP
#if UNITY_EDITOR && UNITY3D
				RemoveFromSingletonsList(key.Name);
#endif
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

				// TODO TEMP
#if UNITY_EDITOR && UNITY3D
				RemoveFromSingletonsList(typeKey.Name);
#endif

				if (map.Value.Count > 0) continue;
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
			if (AutoConfigure)
			{
				Configure(key);
			}

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
			if (AutoConfigure)
			{
				Configure(registeredKeyType);
			}

			object instance;
			if (!Mappings.ContainsKey(key) || !Mappings[key].ContainsKey(registeredKeyType))
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
			var types = srcAssembly.ExportedTypes;
#endif
			foreach (var type in types)
			{
				ConfigureType(type);
			}
		}

		private bool ConfigureType(Type type, object instance = null)
		{
			//#if UNITY_3D && DEBUG && IOC_DEBUG
			//System.Diagnostics.Debug.WriteLine(string.Format("*** Configure -> type: {0}", type.FullName));
			//#endif

			if (Mappings.ContainsKey(type)) return false; // TODO should be TRUE ?

			//#if UNITY_3D && DEBUG && IOC_DEBUG
			//				Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, "*** Configure -> " + type);
			//#endif

#if !NETFX_CORE
			var attribs = type.GetTypeWrapper().GetCustomAttributes<IocTypeMapAttribute>(true);
#else
			var attribs = type.GetTypeInfo().GetCustomAttributes<IocTypeMapAttribute>(true);
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
				// in case "type" has dependecies, init them and continue
				if (!attr.Dependencies.IsNullOrEmpty())
				{
					foreach (var depType in attr.Dependencies)
					{
						if (type == depType) continue;

						ConfigureType(depType); // TODO think how to pass "instance"
					}
				}

				if (attr.MapAllInterfaces)
				{
#if !NETFX_CORE
					var interfaces = type.GetInterfaces();
#else
					var interfaces = TypeExtensions.GetInterfaces(type);
#endif
					foreach (var @interface in interfaces)
					{
						Register(@interface, type, attr.IsSingleton, attr, attr.IsWeakReference, instance);
					}
					//continue;
				}
				if (attr.MappingTypes == null) continue;

				foreach (var mapType in attr.MappingTypes)
				{
					Register(mapType, type, attr.IsSingleton, attr, attr.IsWeakReference, instance);
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
#if UNITY_WSA && !UNITY_EDITOR
			var isAbstract = implementationType.GetTypeInfo().IsAbstract;
#else
			var isAbstract = implementationType.IsAbstract;
#endif
			if (isAbstract)
			{
#if UNITY_3D && DEBUG && IOC_DEBUG
				Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, string.Format("*** Register -> implementationType ({0}) is Abstract Class!", implementationType));
#endif
				return;
			}

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

//#if !NETFX_CORE
			var ctors = type.GetConstructors();
//#else
			//var ctors = type.GetTypeInfo().DeclaredConstructors;
//#endif
			_typeCtors[type] = ctors;
			return ctors;
		}

		private IEnumerable<PropertyInfo> GetTypeProperties(Type type)
		{
			if (_typeProps.ContainsKey(type))
			{
				return _typeProps[type];
			}

//#if !NETFX_CORE
			var props = type.GetProperties();
//#else
//			var props = type.GetTypeInfo().DeclaredProperties;
//#endif
			_typeProps[type] = props;
			return props;
		}

#if UNITY3D || UNITY_3D
		private const string RootObjName = "[SINGLETONS]";

		private const HideFlags DefaultHideFlags = HideFlags.None;
		//HideFlags.DontSave | HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

#if UNITY_EDITOR
		private SingletonsListComponent _singletonsList;
#endif

		private Object InstanciateMonoBehaviourObject(Type implementationType, bool isSingleton)
		{
			var instance = isSingleton ? FindObjectOfType(implementationType) : null;
			if (instance != null)
			{
				Debug.LogWarningFormat(
					"{0} Using instance already created '{1}' for {2}",
					RootObjName, instance, implementationType);

				if (!Application.isPlaying)
				{
					return instance;
				}

				var component = instance as Component;
				if (component != null)
				{
					Object.DontDestroyOnLoad(component.gameObject);
				}
				Object.DontDestroyOnLoad(instance);
				return instance;
			}

			GameObject rootObj;
			if (isSingleton)
			{
				rootObj = GameObject.Find(RootObjName);
				if (rootObj == null)
				{
					rootObj = new GameObject(RootObjName)
					{
						hideFlags = DefaultHideFlags
					};
					Debug.LogFormat("Created {0}", rootObj.name);
					if (Application.isPlaying)
					{
						Object.DontDestroyOnLoad(rootObj);
					}
#if UNITY_EDITOR
					_singletonsList = rootObj.AddComponent<SingletonsListComponent>();
#endif
				}
			}
			else
			{
				rootObj = new GameObject(implementationType.Name)
				{
					hideFlags = DefaultHideFlags
				};
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
				InitProperties(res);
				return res;
			}
			//Debug.LogFormat("trying to create instance for {0}.", implementationType);
#endif
			var ctors = GetTypeConstructors(implementationType);
			var ctor = IocHelper.GetMatchingConstructor(ctors, parameters);
			var instance = CreateInstance(ctor, isSingleton, parameters);
			if (instance != null)
			{
				InitProperties(instance);
				// TODO temp
#if UNITY_EDITOR && UNITY3D
				if (isSingleton)
				{
					AddToSingletonsList(implementationType.Name);
				}
#endif
				return instance;
			}

			foreach (var ctor1 in ctors)
			{
				instance = CreateInstance(ctor1, isSingleton, parameters);
				if (instance == null) continue;

				InitProperties(instance);
				// TODO temp
#if UNITY_EDITOR && UNITY3D
				if (isSingleton)
				{
					AddToSingletonsList(implementationType.Name);
				}
#endif
				return instance;
			}
			return null;
		}

		// TODO temp
#if UNITY_EDITOR && UNITY3D
		private void AddToSingletonsList(string singletonName)
		{
			if (_singletonsList == null) return;
			_singletonsList.Add(singletonName);
		}

		private void RemoveFromSingletonsList(string singletonName)
		{
			if (_singletonsList == null) return;
			_singletonsList.Remove(singletonName);
		}
#endif

		private void InitProperties(object instance)
		{
			if(instance == null) return; // TODO need log?

			var props = GetTypeProperties(instance.GetType());
			if(props.IsNullOrEmpty()) return;

			foreach (var prop in props)
			{
				if(prop == null || !prop.CanWrite) continue;

				var attribs = prop.GetCustomAttributes(typeof(IocDependencyMapAttribute), true);
				if(attribs.IsNullOrEmpty()) continue;

				var attrib = attribs.GetFirstOrDefault();
				if (attrib == null) continue;

				var propValue = GetImplementationInstance(prop.PropertyType);
				prop.SetValue(instance, propValue, null);
			}
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