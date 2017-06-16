#region Usings

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TMS.Common.Extensions;

#if !UNITY3D
using System.Diagnostics;
using System.Xml.Serialization;
using System.Linq.Expressions;
#else
using UnityEngine;
#endif

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
#else

#if NETFX_CORE
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
#endif

#if SILVERLIGHT
using System.Runtime.CompilerServices;
#endif

#endif

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	/// 
	/// </summary>
	public interface IDataErrorInfo
	{
		/// <summary>
		/// Gets the error.
		/// </summary>
		/// <value>
		/// The error.
		/// </value>
		string Error { get; }

		/// <summary>
		/// Gets the <see cref="System.String"/> with the specified column name.
		/// </summary>
		/// <value>
		/// The <see cref="System.String"/>.
		/// </value>
		/// <param name="columnName">Name of the column.</param>
		/// <returns></returns>
		string this[string columnName] { get; }

		/// <summary>
		/// Validates the specified column name.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <returns></returns>
		string Validate(string columnName);
	}

	/// <summary>
	///     Observable Base Class
	/// </summary>
#if !SILVERLIGHT
#if !NETFX_CORE
	[Serializable]	
#else
#if !UNITY3D
	[DataContract]
	[XmlType(Namespace = "tms.com/common")]
	[XmlRoot("Observable", Namespace = "tms.com/common", IsNullable = false)]
#endif
#endif
#endif
	public abstract class Observable :
#if UNITY3D
 MonoBehaviourBase,
#endif
 IObservable, IDataErrorInfo
	{
		/// <summary>
		///     The _id
		/// </summary>
		private string _id;

#if !UNITY3D
		/// <summary>
		///     Initializes a new instance of the <see cref="Observable" /> class.
		/// </summary>
		protected Observable()
		{
			Debug.WriteLine(string.Format("Init {0}", GetType()));
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="Observable" /> class.
		/// </summary>
		/// <param name="id">The id.</param>
		protected Observable(string id)
			: this()
		{
			_id = id;
		}
#endif

		/// <summary>
		///     Gets or sets a value indicating whether this instance is disposed.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is disposed; otherwise, <c>false</c> .
		/// </value>
		protected virtual bool IsDisposed { get; set; }

		/// <summary>
		///     Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
#if !UNITY3D
		[XmlElement("ID")]
#endif
		public virtual string Id
		{
			get { return _id; }
			set { SetValue(ref _id, value, "Id"); }
		}

		#region IDisposable Members

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

#if UNITY3D
		protected override void OnDestroy()
		{
			Dispose(true);
			base.OnDestroy();
		}
#endif

		#endregion

		#region INotifyPropertyChanged Members

		/// <summary>
		///     Occurs when a property value changes.
		/// </summary>
		public virtual event PropertyChangedEventHandler PropertyChanged
		{
			add { GetPropertyChangedHandlers().AddListener(value, keepAlive: true); }
			remove
			{
				if (_propertyChangedHandlers == null) return;
				_propertyChangedHandlers.RemoveListener(value.CreateDelegateUniqueId());
			}
		}

		#endregion

		#region INotifyPropertyChanging Members

#if !NETFX_CORE && !UNITY3D
		/// <summary>
		///     Occurs when a property value is changing.
		/// </summary>
		public virtual event PropertyChangingEventHandler PropertyChanging
		{
			add { GetPropertyChangingHandlers().AddListener(value); }
			remove
			{
				if (_propertyChangingHandlers == null) return;
				_propertyChangingHandlers.RemoveListener(value);
			}
		}
#endif

		#endregion

#if !SILVERLIGHT && !NETFX_CORE
		[field: NonSerialized]
#endif
		private WeakDelegatesManager _propertyChangedHandlers;

		/// <summary>
		///     Gets the property changed handlers.
		/// </summary>
		/// <returns></returns>
		private WeakDelegatesManager GetPropertyChangedHandlers()
		{
			return _propertyChangedHandlers ?? (_propertyChangedHandlers = new WeakDelegatesManager());
		}

#if !SILVERLIGHT && !NETFX_CORE
		[field: NonSerialized]
#endif
		private WeakDelegatesManager _propertyChangingHandlers;

		/// <summary>
		///     Gets the property changing handlers.
		/// </summary>
		/// <returns></returns>
		private WeakDelegatesManager GetPropertyChangingHandlers()
		{
			return _propertyChangingHandlers ?? (_propertyChangingHandlers = new WeakDelegatesManager());
		}

		/// <summary>
		///     Sets the field.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field">The field.</param>
		/// <param name="value">The value.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="firePropChangingEvent">
		///     if set to <c>true</c> [fire prop changing event].
		/// </param>
		/// <param name="firePropChangedEvent">
		///     if set to <c>true</c> [fire prop changed event].
		/// </param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool SetValue<T>(ref T field, T value,
#if !SILVERLIGHT && !UNITY3D
 [CallerMemberName] 
#endif
 string propertyName = null,
bool firePropChangingEvent = true,
bool firePropChangedEvent = true)
		{
			if (Equals(field, value)) return false;
			if (firePropChangingEvent) OnPropertyChanging(propertyName);
			field = value;
			if (firePropChangedEvent) OnPropertyChanged(propertyName);
			return true;
		}

		/// <summary>
		///     Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing">
		///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		/// </param>
		/// <returns>
		///     <c>true</c> if operation is allowed
		/// </returns>
		protected virtual bool Dispose(bool disposing)
		{
			if (IsDisposed || !disposing) return false;
			IsDisposed = true;

			if (_propertyChangedHandlers != null)
			{
				_propertyChangedHandlers.Dispose();
				_propertyChangedHandlers = null;
			}

			if (_propertyChangingHandlers != null)
			{
				_propertyChangingHandlers.Dispose();
				_propertyChangingHandlers = null;
			}

			_id = null;

#if !UNITY3D
			Debug.WriteLine(string.Format("Dispose {0}", GetType()));
#else
			Debug.Log(string.Format("Dispose {0}", GetType()));
#endif

			return true;
		}

#if !UNITY3D
		/// <summary>
		///     Sets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="field">The field.</param>
		/// <param name="value">The value.</param>
		/// <param name="propertySelector">The property selector.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool SetValue<T>(ref T field, T value, Expression<Func<T>> propertySelector)
		{
			var propertyName = this.GetMemberName(propertySelector);
			return SetValue(ref field, value, propertyName);
		}

		/// <summary>
		///     Called when [property changed].
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertySelector">The property selector.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool OnPropertyChanged<T>(Expression<Func<T>> propertySelector)
		{
			var propertyName = this.GetMemberName(propertySelector);
			return OnPropertyChanged(propertyName);
		}

		/// <summary>
		///     Raises the property changed event.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertySelector">The property selector.</param>
		/// <returns></returns>
		public virtual bool RaisePropertyChanged<T>(Expression<Func<T>> propertySelector)
		{
			return OnPropertyChanged(propertySelector);
		}

		/// <summary>
		///     Called when [property changing].
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertySelector">The property selector.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool OnPropertyChanging<T>(Expression<Func<T>> propertySelector)
		{
			var propertyName = this.GetMemberName(propertySelector);
			return OnPropertyChanging(propertyName);
		}
#endif

		/// <summary>
		///     Called when [property changed].
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool OnPropertyChanged(
#if !SILVERLIGHT && !UNITY3D
[CallerMemberName] 
#endif
string propertyName = null)
		{
			if (_propertyChangedHandlers == null) return false;
#if DEBUG
			var validationRes = ValidatePropertyName(propertyName);
			if (!validationRes) return false;
#endif
			GetPropertyChangedHandlers().Raise(false, this, new PropertyChangedEventArgs(propertyName));
			return true;
		}

		/// <summary>
		///     Called when [property changing].
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool OnPropertyChanging(
#if !SILVERLIGHT && !UNITY3D
[CallerMemberName] 
#endif
string propertyName = null)
		{
			if (_propertyChangingHandlers == null) return false;
#if DEBUG
			//var validationRes = ValidatePropertyName(propertyName);
			//if (!validationRes) return false;
#endif
			GetPropertyChangingHandlers().Raise(false, this, new PropertyChangedEventArgs(propertyName));
			return true;
		}

		/// <summary>
		///     Invokes the property changed for all properties.
		/// </summary>
		/// <param name="propNames">The prop names.</param>
		/// <param name="ignoreProps">The ignore props.</param>
		/// <returns>
		///     <c>true</c> on successful operation
		/// </returns>
		protected bool InvokePropertyChangedForAllProperties(string[] propNames = null,
															 params string[] ignoreProps)
		{
			var type = GetType();
			PropertyInfo[] props;
			if (propNames != null)
			{
				props = new PropertyInfo[propNames.Length];
#if !NETFX_CORE
				propNames.ForEach((i, n) => props[i] = type.GetProperty(n));
#else
				propNames.ForEach((i, name) => props[i] = type.GetRuntimeProperty(name));
#endif
			}
			else
			{
#if !NETFX_CORE
				props = type.GetProperties();
#else
				props = type.GetRuntimeProperties().ToArray();
#endif
			}
			if (props == null) return false;

			props.Where(prop => Array.IndexOf(ignoreProps, prop.Name) < 0).
				  ForEach(prop => OnPropertyChanged(prop.Name));

			return true;
		}

#if DEBUG
		/// <summary>
		///     Validates the name of the property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected bool ValidatePropertyName(string propertyName)
		{
			try
			{
#if !NETFX_CORE
				return GetType().GetProperty(propertyName) != null;
#else
				return GetType().GetRuntimeProperty(propertyName) != null;
#endif
			}
			catch
			{
				return false;
			}
		}
#endif

		/// <summary>
		///     Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is valid; otherwise, <c>false</c> .
		/// </value>
		public bool IsValid
		{
			get { return Error == null; }
		}

		/// <summary>
		///     The _error
		/// </summary>
		private string _error;

		/// <summary>
		///     Gets an error message indicating what is wrong with this object.
		/// </summary>
		/// <value>The error.</value>
		/// <returns> An error message indicating what is wrong with this object. The default is an empty string (""). </returns>
		public virtual string Error
		{
			get { return _error; }
			set
			{
				if (!SetValue(ref _error, value, "Error")) return;
				OnPropertyChanged("IsValid");
			}
		}

		/// <summary>
		///     Gets the error message for the property with the given name.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>The error message for the property. The default is an empty string ("").</returns>
		public virtual string this[string columnName]
		{
			get
			{
				Error = Validate(columnName);
				return Error;
			}
		}

		/// <summary>
		///     Validates the specified column name.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <returns>System.String.</returns>
		public virtual string Validate(string columnName)
		{
			return null;
		}
	}
}