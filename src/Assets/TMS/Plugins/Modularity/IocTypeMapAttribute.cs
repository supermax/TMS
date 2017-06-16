#region

using System;

#endregion

namespace TMS.Common.Modularity
{
	/// <summary>
	///     IOC Type Mapping Attribute
	/// </summary>
	public class IocTypeMapAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="IocTypeMapAttribute" /> class.
		/// </summary>
		/// <param name="mapAllInterfaces">
		///     if set to <c>true</c> [map all interfaces].
		/// </param>
		/// <param name="isSingleton">
		///     if set to <c>true</c> [is singleton].
		/// </param>
		/// <param name="isWeakRef">
		///     if set to <c>true</c> [is weak ref].
		/// </param>
		public IocTypeMapAttribute(bool mapAllInterfaces = false, bool isSingleton = false, bool isWeakRef = false)
		{
			MapAllInterfaces = mapAllInterfaces;
			IsSingleton = isSingleton;
			IsWeakReference = isWeakRef;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="IocTypeMapAttribute" /> class.
		/// </summary>
		/// <param name="interfaceType">Type of the interface.</param>
		/// <param name="isSingleton">
		///     if set to <c>true</c> [is singleton].
		/// </param>
		/// <param name="isWeakRef">
		///     if set to <c>true</c> [is weak ref].
		/// </param>
		public IocTypeMapAttribute(Type interfaceType, bool isSingleton = false, bool isWeakRef = false)
		{
			MappingTypes = new[] { interfaceType };
			IsSingleton = isSingleton;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="IocTypeMapAttribute" /> class.
		/// </summary>
		/// <param name="interfaceTypes">The interface types.</param>
		/// <param name="isSingleton">
		///     if set to <c>true</c> [is singleton].
		/// </param>
		/// <param name="isWeakRef">
		///     if set to <c>true</c> [is weak ref].
		/// </param>
		public IocTypeMapAttribute(Type[] interfaceTypes, bool isSingleton = false, bool isWeakRef = false)
		{
			MappingTypes = interfaceTypes;
			IsSingleton = isSingleton;
			IsWeakReference = isWeakRef;
		}

		/// <summary>
		///     Gets or sets the mapping types.
		/// </summary>
		/// <value>
		///     The mapping types.
		/// </value>
		public Type[] MappingTypes { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether [map all interfaces].
		/// </summary>
		/// <value>
		///     <c>true</c> if [map all interfaces]; otherwise, <c>false</c>.
		/// </value>
		public bool MapAllInterfaces { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether this instance is singleton.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is singleton; otherwise, <c>false</c>.
		/// </value>
		public bool IsSingleton { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether this instance is weak reference.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is weak reference; otherwise, <c>false</c>.
		/// </value>
		public bool IsWeakReference { get; set; }

		/// <summary>
		/// Instantiate New Instance On Registration <para/>
		/// Gets or sets a value indicating whether this instance is immediate instantiation.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is immediate instantiation; otherwise, <c>false</c>.
		/// </value>
		public bool InstantiateOnRegistration { get; set; }

		/// <summary>
		///     Called on owner's instantiation.
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected internal virtual void OnInstantiation(object instance)
		{
		}

		/// <summary>
		///     Called on attribute's registration.
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		protected internal virtual void OnRegistration(Type ownerType)
		{
		}
	}
}