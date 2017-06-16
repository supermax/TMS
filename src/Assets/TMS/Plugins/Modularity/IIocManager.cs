#region

using System;
using System.Reflection;

#endregion

namespace TMS.Common.Modularity
{
	/// <summary>
	///     Interface for IOC Manager
	/// </summary>
	public interface IIocManager
	{
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
		void Register<T>(Type implementationType, bool singleton = false, bool isWeakRef = false);

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
		void Register(Type interfaceType, Type implementationType, bool singleton = false, bool isWeakRef = false);

		void Register<T>(T implementationInstance, bool isWeakRef = false) where T : class;

		/// <summary>
		/// Registers the specified implementation instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="implementationInstance">The implementation instance.</param>
		/// <param name="isWeakRef">if set to <c>true</c> [is weak ref].</param>
		void Register<TInterface, TImplementation>(TImplementation implementationInstance, bool isWeakRef = false)
			where TImplementation : TInterface;

		//void Register<T>(object implementationInstance, bool isWeakRef = false) where T : class;

		/// <summary>
		/// Unregisters the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>
		/// false in case did not find registered mapping for this type
		/// </returns>
		bool Unregister<T>();

		/// <summary>
		/// Unregisters the specified implementation instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="implementationInstance">The implementation instance.</param>
		/// <returns>false in case did not find registered mapping for this instance</returns>
		bool Unregister<T>(object implementationInstance);

		/// <summary>
		/// Unregisters all implementation instance(s).
		/// </summary>
		/// <param name="implementationInstance">The implementation instance.</param>
		/// <returns>
		/// false in case did not find registered mapping(s) for this instance
		/// </returns>
		bool Unregister(object implementationInstance);

		/// <summary>
		///     Resolves the instance of type by the specified parameters.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="parameters"> The parameters. </param>
		/// <returns> </returns>
		T Resolve<T>(params object[] parameters);

		//T Resolve<T>(string implementationTypeName);

		/// <summary>
		///     Resolves the instance by registered key type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="registeredKeyType">Type registered type for the key.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns> </returns>
		T Resolve<T>(Type registeredKeyType, params object[] parameters);

		/// <summary>
		///     Configures the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		void Configure(Type type);

		/// <summary>
		///     Configures the specified SRC assembly.
		/// </summary>
		/// <param name="srcAssembly">The SRC assembly.</param>
		void Configure(Assembly srcAssembly);
	}
}