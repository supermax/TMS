using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if NETFX_CORE	
using System.Linq;
#endif

namespace TMS.Common.Extensions
{
	public static class TypeExtensions
	{
		public static ITypeWrapper GetTypeWrapper(this object obj)
		{
			var type = obj.GetType();
			var wrapper = type.GetTypeWrapper();
			return wrapper;
		}

		public static ITypeWrapper GetTypeWrapper(this Type type)
		{
#if !NETFX_CORE	
			var wrapper = new TypeWrapper(type);
			return wrapper;
#else
			var wrapper = new TypeInfoWrapper(type);
			return wrapper;
#endif
		}
	}

#region interface ITypeWrapper
	/// <summary>
	/// Interface for TypeWrapper
	/// </summary>
	public interface ITypeWrapper
	{
		/// <summary>
		/// Gets the declaring type.
		/// </summary>
		/// <value>
		/// The declaring type.
		/// </value>
		Type DeclaringType { get; }

		/// <summary>
		/// Gets a value indicating whether this instance is enum.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is enum; otherwise, <c>false</c>.
		/// </value>
		bool IsEnum { get; }

		/// <summary>
		/// Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		bool IsArray { get; }

		/// <summary>
		/// Gets a value indicating whether this instance is class.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is class; otherwise, <c>false</c>.
		/// </value>
		bool IsClass { get; }

		/// <summary>
		/// Determines whether [is assignable from] [the specified type].
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if [is assignable from] [the specified type]; otherwise, <c>false</c>.
		/// </returns>
		bool IsAssignableFrom(Type type);

		/// <summary>
		/// Gets the properties.
		/// </summary>
		/// <returns></returns>
		IEnumerable<PropertyInfo> GetProperties();

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		PropertyInfo GetProperty(string name);

		/// <summary>
		/// Gets the fields.
		/// </summary>
		/// <returns></returns>
		IEnumerable<FieldInfo> GetFields();

		/// <summary>
		/// Gets the implemented interfaces.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Type> GetImplementedInterfaces();

		/// <summary>
		/// Gets the interface.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
		/// <returns></returns>
		Type GetInterface(string name, bool ignoreCase);

		/// <summary>
		/// Gets the method.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="types">The types.</param>
		/// <returns></returns>
		MethodInfo GetMethod(string name, Type[] types);

		/// <summary>
		/// Gets the event.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		EventInfo GetEvent(string name);

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		T[] GetCustomAttributes<T>(bool inherit) where T : Attribute;
	}
#endregion

#if !NETFX_CORE	
#region class TypeWrapper
	internal class TypeWrapper : ITypeWrapper
	{
		private readonly Type _type;

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeWrapper"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public TypeWrapper(Type type)
		{
			_type = type;
		}

		public Type DeclaringType
		{
			get { return _type; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is enum.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is enum; otherwise, <c>false</c>.
		/// </value>
		public bool IsEnum
		{
			get { return _type.IsEnum; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool IsArray
		{
			get { return _type.IsArray; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is class.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is class; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool IsClass
		{
			get { return _type.IsClass; }
		}

		/// <summary>
		/// Determines whether [is assignable from] [the specified type].
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if [is assignable from] [the specified type]; otherwise, <c>false</c>.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool IsAssignableFrom(Type type)
		{
			return _type.IsAssignableFrom(type);
		}

		/// <summary>
		/// Gets the properties.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<PropertyInfo> GetProperties()
		{
			return _type.GetProperties();
		}

		/// <summary>
		/// Gets the fields.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IEnumerable<FieldInfo> GetFields()
		{
			return _type.GetFields();
		}

		/// <summary>
		/// Gets the implemented interfaces.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Type> GetImplementedInterfaces()
		{
			return _type.GetInterfaces();
		}

		/// <summary>
		/// Gets the interface.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
		/// <returns></returns>
		public Type GetInterface(string name, bool ignoreCase)
		{
			return _type.GetInterface(name, ignoreCase);
		}

		/// <summary>
		/// Gets the method.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="types">The types.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public MethodInfo GetMethod(string name, Type[] types)
		{
			return _type.GetMethod(name, types);
		}

		/// <summary>
		/// Gets the event.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public EventInfo GetEvent(string name)
		{
			return _type.GetEvent(name);
		}

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public T[] GetCustomAttributes<T>(bool inherit) where T : Attribute
		{
			var attribs = _type.GetCustomAttributes(typeof(T), inherit);
			if (attribs.IsNullOrEmpty()) return null;

			var res = attribs.Cast<T>();
			return res.ToArray();

		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public PropertyInfo GetProperty(string name)
		{
			return _type.GetProperty(name);
		}
	}
	#endregion
#else
	#region class TypeInfoWrapper
	internal class TypeInfoWrapper : ITypeWrapper
	{
		private readonly TypeInfo _info;

		public TypeInfoWrapper(Type type)
		{
			_info = type.GetTypeInfo();
			DeclaringType = type;
		}

		public Type DeclaringType { get; }

		public bool IsEnum
		{
			get { return _info.IsEnum; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		public bool IsArray
		{
			get { return _info.IsArray; }
		}

		/// <summary>
		/// Gets the properties.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PropertyInfo> GetProperties()
		{
			var lst = new List<PropertyInfo>();
			var props = _info.DeclaredProperties.ToArray();
			if (!props.IsNullOrEmpty())
			{
				lst.AddRange(props);
			}
			if (_info.BaseType == null) return lst;

			var parent = _info.BaseType.GetTypeInfo();
			while (parent != null)
			{
				props = parent.DeclaredProperties.ToArray();
				if (!props.IsNullOrEmpty())
				{
					lst.AddRange(props);
				}
				if (parent.BaseType == null) break;
				parent = parent.BaseType.GetTypeInfo();
			}
			return lst;

			//return _type.DeclaredProperties;
		}

		/// <summary>
		/// Gets the implemented interfaces.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Type> GetImplementedInterfaces()
		{
			return _info.ImplementedInterfaces;
		}

		/// <summary>
		/// Gets the interface.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="ignoreCase">if set to <c>true</c> [ignoreCase].</param>
		/// <returns></returns>
		public Type GetInterface(string name, bool ignoreCase)
		{
			if (_info.ImplementedInterfaces.IsNullOrEmpty())
			{
				return null;
			}

			var res = _info.ImplementedInterfaces.FirstOrDefault(item => item.FullName.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
			return res;
		}

		/// <summary>
		/// Gets the method.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public MethodInfo GetMethod(string name, Type[] args)
		{
			var methods = _info.GetDeclaredMethods(name);
			if (methods.IsNullOrEmpty()) return null;

			foreach (var method in methods)
			{
				if(method == null || method.Name != name)
				{
					continue;
				}

				var parameters = method.GetParameters();
				var isSame = Compare(parameters, args);
				if(!isSame)
				{
					continue;
				}

				return method;				
			}
			return null;	
		}

		private static bool Compare(ParameterInfo[] parameters, Type[] args)
		{
			if (parameters.IsNullOrEmpty() && args.IsNullOrEmpty())
			{
				return true;
			}

			if (parameters.Length != args.Length)
			{
				return false;
			}

			for (var i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].ParameterType != args[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets the event.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public EventInfo GetEvent(string name)
		{
			return _info.GetDeclaredEvent(name);
		}

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public T[] GetCustomAttributes<T>(bool inherit) where T : Attribute
		{
			var attribs = _info.GetCustomAttributes<T>(inherit);
			if (attribs.IsNullOrEmpty()) return null;

			return attribs.ToArray();
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public PropertyInfo GetProperty(string name)
		{
			return _info.GetDeclaredProperty(name);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is class.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is class; otherwise, <c>false</c>.
		/// </value>
		public bool IsClass
		{
			get { return _info.IsClass; }
		}

		/// <summary>
		/// Determines whether [is assignable from] [the specified type].
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if [is assignable from] [the specified type]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsAssignableFrom(Type type)
		{
			return _info.IsAssignableFrom(type.GetTypeInfo());
		}

		/// <summary>
		/// Gets the fields.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<FieldInfo> GetFields()
		{
			return _info.DeclaredFields;
		}
	}
	#endregion
#endif
}
