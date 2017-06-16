using System;
using System.Collections.Generic;
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
		/// Gets the fields.
		/// </summary>
		/// <returns></returns>
		IEnumerable<FieldInfo> GetFields();

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
	}
#endregion
#else
#region class TypeInfoWrapper
	internal class TypeInfoWrapper : ITypeWrapper
	{
		private readonly TypeInfo _type;

		public TypeInfoWrapper(Type type)
		{
			_type = type.GetTypeInfo();
		}

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
		public bool IsArray
		{
			get { return _type.IsArray; }
		}

		/// <summary>
		/// Gets the properties.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PropertyInfo> GetProperties()
		{
			var lst = new List<PropertyInfo>();
			var props = _type.DeclaredProperties.ToArray();
			if (!props.IsNullOrEmpty())
			{
				lst.AddRange(props);
			}
			if (_type.BaseType == null) return lst;

			var parent = _type.BaseType.GetTypeInfo();
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
		/// Gets the interface.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="ignoreCase">if set to <c>true</c> [ignoreCase].</param>
		/// <returns></returns>
		public Type GetInterface(string name, bool ignoreCase)
		{
			var res = _type.ImplementedInterfaces.FirstOrDefault(item => item.Name == name);
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
			return _type.GetDeclaredMethod(name);
		}

		/// <summary>
		/// Gets the event.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public EventInfo GetEvent(string name)
		{
			return _type.GetDeclaredEvent(name);
		}

		/// <summary>
		/// Gets a value indicating whether this instance is class.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is class; otherwise, <c>false</c>.
		/// </value>
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
		public bool IsAssignableFrom(Type type)
		{
			return _type.IsAssignableFrom(type.GetTypeInfo());
		}

		/// <summary>
		/// Gets the fields.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<FieldInfo> GetFields()
		{
			return _type.DeclaredFields;
		}
	}
#endregion
#endif
}
