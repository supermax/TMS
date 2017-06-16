#region

using System;
using System.Collections.Generic;
using TMS.Common.Extensions;

#endregion

namespace TMS.Common.Modularity
{
	/// <summary>
	///     Type Mapping Info
	/// </summary>
	internal class MappingInfo : IDisposable
	{
		private object _instance;

		private WeakReference _weakRef;

		/// <summary>
		///     Gets or sets the type of the implementation.
		/// </summary>
		/// <value>The type of the implementation.</value>
		public Type ImplementationType { get; internal set; }

		/// <summary>
		///     Gets or sets the type map attribute.
		/// </summary>
		/// <value>
		///     The type map attribute.
		/// </value>
		public IocTypeMapAttribute TypeMapAttribute { get; internal set; }

		/// <summary>
		///     Gets or sets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public object Instance
		{
			get
			{
				if (IsWeakReference)
				{
					return _weakRef != null ? _weakRef.Target : null;
				}
				return _instance;
			}
			internal set
			{
				if (Equals(_instance, value)) return; 
				_instance = null;

				if (value == null)
				{
					if (_weakRef == null) return;
					_weakRef.Target = null;
					_weakRef = null;
					return;
				}

				if (IsWeakReference)
				{
					_weakRef = new WeakReference(value);
				}
				else
				{
					_instance = value;
				}

				if (TypeMapAttribute == null) return;
				TypeMapAttribute.OnInstantiation(value);
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether this instance is singleton.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is singleton; otherwise, <c>false</c>.
		/// </value>
		public bool IsSingleton { get; internal set; }

		/// <summary>
		///     Gets or sets a value indicating whether this instance is weak reference.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is weak reference; otherwise, <c>false</c>.
		/// </value>
		public bool IsWeakReference { get; internal set; }

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			var disposable = Instance as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			Instance = null;
			ImplementationType = null;
			TypeMapAttribute = null;
			IsWeakReference = false;
		}
		#endregion
	}

	internal class TypeMappingInfo : Dictionary<Type, MappingInfo>, IDisposable
	{
		public void Dispose()
		{
			Values.ForEach(map => map.Dispose());
			Clear();
		}
	}
}