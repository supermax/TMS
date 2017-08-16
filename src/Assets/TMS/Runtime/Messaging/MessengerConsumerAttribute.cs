#region

using System;
using TMS.Common.Modularity;

#endregion

namespace TMS.Common.Messaging
{
	/// <summary>
	///     Messenger's Consumer Attribute
	/// </summary>
	public class MessengerConsumerAttribute : IocTypeMapAttribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="MessengerConsumerAttribute" /> class.
		/// </summary>
		protected internal MessengerConsumerAttribute()
		{
			AutoSubscribe = true;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MessengerConsumerAttribute" /> class.
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
		/// <param name="autoSubscribe">
		///     if set to <c>true</c> [auto subscribe].
		/// </param>
		public MessengerConsumerAttribute(bool mapAllInterfaces = false, bool isSingleton = false, bool isWeakRef = false,
			bool autoSubscribe = true)
			: base(mapAllInterfaces, isSingleton, isWeakRef)
		{
			AutoSubscribe = autoSubscribe;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MessengerConsumerAttribute" /> class.
		/// </summary>
		/// <param name="interfaceType">Type of the interface.</param>
		/// <param name="isSingleton">
		///     if set to <c>true</c> [is singleton].
		/// </param>
		/// <param name="isWeakRef">
		///     if set to <c>true</c> [is weak ref].
		/// </param>
		/// <param name="autoSubscribe">
		///     if set to <c>true</c> [auto subscribe].
		/// </param>
		public MessengerConsumerAttribute(Type interfaceType, bool isSingleton = false, bool isWeakRef = false,
			bool autoSubscribe = true)
			: base(interfaceType, isSingleton, isWeakRef)
		{
			AutoSubscribe = autoSubscribe;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="MessengerConsumerAttribute" /> class.
		/// </summary>
		/// <param name="interfaceTypes">The interface types.</param>
		/// <param name="isSingleton">
		///     if set to <c>true</c> [is singleton].
		/// </param>
		/// <param name="isWeakRef">
		///     if set to <c>true</c> [is weak ref].
		/// </param>
		/// <param name="autoSubscribe">
		///     if set to <c>true</c> [auto subscribe].
		/// </param>
		public MessengerConsumerAttribute(Type[] interfaceTypes, bool isSingleton = false, bool isWeakRef = false,
			bool autoSubscribe = true)
			: base(interfaceTypes, isSingleton, isWeakRef)
		{
			AutoSubscribe = autoSubscribe;
		}

		/// <summary>
		///     Gets or sets a value indicating whether [auto subscribe].
		/// </summary>
		/// <value>
		///     <c>true</c> if [auto subscribe]; otherwise, <c>false</c>.
		/// </value>
		public bool AutoSubscribe { get; set; }

		/// <summary>
		///     Called on owner's instantiation.
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected internal override void OnInstantiation(object instance)
		{
			base.OnInstantiation(instance);

			var consumer = instance as IMessengerConsumer;
			if (consumer == null) return;
			consumer.Subscribe();
		}
	}
}