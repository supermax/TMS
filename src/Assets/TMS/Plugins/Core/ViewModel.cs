#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Xml.Serialization;
using TMS.Common.Extensions;
using TMS.Common.Helpers;
using TMS.Common.Messaging;
using TMS.Common.Modularity;

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Linq.Expressions;
#else
#if NETFX_CORE
using TMS.Common.Log;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
#else
#if UNITY || UNITY3D || UNITY_3D
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
using UnityEngine;
#endif
#endif
#if SILVERLIGHT
using System.Runtime.CompilerServices;
#endif
#endif

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	///     View Model Base Class
	/// </summary>
#if !SILVERLIGHT
#if !NETFX_CORE
	[Serializable]
#else
#if !UNITY3D
	[DataContract]
#endif
#endif
#endif
	[XmlType(Namespace = "tms.com/common")]
	[XmlRoot("ViewModel", Namespace = "tms.com/common", IsNullable = false)]
	public class ViewModel : Observable, IViewModel, IMessengerConsumer
	{
		//void OnGUI()
		//{
		//	var myStyle = new GUIStyle();
		//	myStyle.fontSize = 50;
		//	myStyle.normal.textColor = Color.yellow;

		//	GUI.Label(new Rect(Screen.width * 0.5f, 20 , 100 , 50), Messenger.ToString(), myStyle);
		//}
		/// <summary>
		/// Gets or sets the default dispatcher.
		/// </summary>
		/// <value>
		/// The default dispatcher.
		/// </value>
		public IDispatcherProxy DefaultDispatcher { get; protected set; }

		/// <summary>
		///     The locker
		/// </summary>
#if !SILVERLIGHT && !NETFX_CORE
		[NonSerialized]
#endif
		protected readonly object Locker = new object();

		/// <summary>
		///     The _commands
		/// </summary>
		[XmlIgnore]
		private volatile CommandsContainer _commands;

		/// <summary>
		///     The _data items
		/// </summary>
		private volatile DataItemsContainer _dataItems;

		/// <summary>
		///     The _is suspended
		/// </summary>
#if !SILVERLIGHT && !NETFX_CORE
		[NonSerialized]
#endif
		private bool _isSuspended;

		/// <summary>
		///     The _properties
		/// </summary>
#if !SILVERLIGHT && !NETFX_CORE
		[NonSerialized]
#endif
		private ICollection<string> _properties;

		/// <summary>
		/// Gets a value indicating whether [is design mode].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is design mode]; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsDesignMode
		{
			get
			{
#if UNITY3D || UNITY_3D
				return Application.isEditor && !Application.isPlaying;
#else
				return DesignHelper.IsDesignMode;
#endif
			}
		}

#if !UNITY && !UNITY3D && !UNITY_3D
        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewModel" /> class.
        /// </summary>
        public ViewModel()
		{
		    DefaultDispatcher = DispatcherProxy.Default;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="ViewModel" /> class.
		/// </summary>
		/// <param name="commands">The commands.</param>
		/// <param name="dataItems">The data items.</param>
		public ViewModel(CommandsContainer commands,
						 DataItemsContainer dataItems)
			: this()
		{
			_commands = commands;
			_dataItems = dataItems;
		}
#endif

#region IViewModel Members

		/// <summary>
		///     Gets the commands.
		/// </summary>
		/// <value>The commands.</value>
		[XmlIgnore]
		public CommandsContainer Commands
		{
			get
			{
				if (_commands == null)
				{
					lock (Locker)
					{
						if (_commands == null)
						{
							_commands = new CommandsContainer();
						}
					}
				}
				return _commands;
			}
			//set { SetValue(ref _commands, value, () => Commands); }
		}

		/// <summary>
		///     Gets the data items.
		/// </summary>
		/// <value>The data items.</value>
		public DataItemsContainer DataItems
		{
			get
			{
				if (_dataItems == null)
				{
					lock (Locker)
					{
						if (_dataItems == null)
						{
							_dataItems = new DataItemsContainer();
						}
					}
				}
				return _dataItems;
			}
			//set { SetValue(ref _dataItems, value, () => DataItems); }
		}

		/// <summary>
		///     Suspends property observation and begins the initialization session
		///     <para>Ensure calling EndInit() after update</para>
		/// </summary>
		public virtual void BeginInit()
		{
			if (_properties == null)
			{
				_properties = new List<string>();
			}
			_isSuspended = true;
		}

		/// <summary>
		///     Ends the initialization session and resolves property observation
		/// </summary>
		/// <param name="isSilentUpdate">
		///     if set to <c>true</c> [the properties' change event will not be raised].
		/// </param>
		public virtual void EndInit(bool isSilentUpdate = false)
		{
			try
			{
				if (isSilentUpdate || _properties == null) return;

				foreach (var propName in _properties)
				{
					OnPropertyChanged(propName);
				}
			}
			finally
			{
				_isSuspended = false;
				if (_properties != null)
				{
					_properties.Clear();
				}
			}
		}

		/// <summary>
		///     Resets value in some properties (back to defaults).
		/// </summary>
		public virtual void Reset()
		{
			DataItems.Reset();
		}

#endregion

		/// <summary>
		/// Determines whether [is child of] [the specified data].
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		protected virtual bool IsChildOf(object data)
		{
			var contains = DataItems.Values.Contains(data);
			return contains;
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
		protected override bool Dispose(bool disposing)
		{
			if (!disposing) return false;

			Unsubscribe();

			if (_properties != null)
			{
				_properties.Clear();
				_properties = null;
			}

			if (_commands != null)
			{
				_commands.ForEach(pair =>
					{
						if (pair.Value is IDisposable)
							(pair.Value as IDisposable).Dispose();
					});
				_commands.Clear();
				_commands = null;
			}

			if (_dataItems != null)
			{
				_dataItems.Values.ForEach(item =>
                {
#if UNITY3D
					if (item is MonoBehaviourBase)
                    {
                        var mono = (item as MonoBehaviourBase);
						// destroy in case me isn't a child of item
	                    if (!transform.IsChildOf(mono.transform))
	                    {
		                    Destroy(mono);
	                    }
                    }
                    else if (item is IDisposable)
                    {
                        var disposable = item as IDisposable;
	                    var vm = disposable as ViewModel;
						// destroy in case me isn't a child of item
						if (vm == null || !vm.IsChildOf(this))
	                    {
							disposable.Dispose();
	                    }
                    }
#else
					if (item is IDisposable)
					{
						var disposable = item as IDisposable;
						var vm = disposable as ViewModel;
						// destroy in case me isn't a child of item
						if (vm == null || !vm.IsChildOf(this))
						{
							disposable.Dispose();
						}
					}
#endif
						if (item is IList)
							(item as IList).Clear();
					});
				_dataItems.Clear();
				_dataItems = null;
			}

			DefaultDispatcher = null;

			return base.Dispose(true);
		}

#if !UNITY3D
		/// <summary>
		///     Sets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		/// <param name="propertySelector">The property selector.</param>
		/// <param name="firePropChangingEvent">
		///     if set to <c>true</c> [fire prop changing event].
		/// </param>
		/// <param name="firePropChangedEvent">
		///     if set to <c>true</c> [fire prop changed event].
		/// </param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool SetValue<T>(T value, Expression<Func<T>> propertySelector,
										   bool firePropChangingEvent = true, bool firePropChangedEvent = true)
		{
			var propertyName = this.GetMemberName(propertySelector);
			return SetValue(value, propertyName, firePropChangingEvent, firePropChangedEvent);
		}

#if !UNITY3D
		/// <summary>
		///     Sets the command.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		/// <param name="commandSelector">The command selector.</param>
		/// <param name="firePropChangingEvent">
		///     if set to <c>true</c> [fire prop changing event].
		/// </param>
		/// <param name="firePropChangedEvent">
		///     if set to <c>true</c> [fire prop changed event].
		/// </param>
		/// <returns></returns>
		protected virtual bool SetCommand<T>(T value, Expression<Func<T>> commandSelector,
											 bool firePropChangingEvent = true, bool firePropChangedEvent = true)
			where T : ICommand
		{
			var commandName = this.GetMemberName(commandSelector);
			return SetCommand(value, commandName, firePropChangingEvent, firePropChangedEvent);
		}
#endif
#endif

		/// <summary>
		///     Sets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
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
		protected virtual bool SetValue<T>(T value,
#if !UNITY3D
 [CallerMemberName]
#endif
 string propertyName = null,
 bool firePropChangingEvent = true,
 bool firePropChangedEvent = true)
		{
			var field = default(T);
			if (DataItems.ContainsKey(propertyName))
			{
				field = (T)DataItems[propertyName];
			}
			if (Equals(field, value)) return false;

			if (firePropChangingEvent) OnPropertyChanging(propertyName);

			DataItems[propertyName] = value;

			if (firePropChangedEvent) OnPropertyChanged(propertyName);
			return true;
		}

		/// <summary>
		///     Sets the command.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">The value.</param>
		/// <param name="commandName">Name of the command.</param>
		/// <param name="firePropChangingEvent">
		///     if set to <c>true</c> [fire prop changing event].
		/// </param>
		/// <param name="firePropChangedEvent">
		///     if set to <c>true</c> [fire prop changed event].
		/// </param>
		/// <returns></returns>
		protected virtual bool SetCommand<T>(T value,
#if !UNITY3D
 [CallerMemberName]
#endif
 string commandName = null,
 bool firePropChangingEvent = true,
 bool firePropChangedEvent = true)
where T : ICommand
		{
			ICommand command = null;
			if (Commands.ContainsKey(commandName))
			{
				command = (T)Commands[commandName];
			}
			if (Equals(command, value)) return false;

			if (firePropChangingEvent) OnPropertyChanging(commandName);

			Commands[commandName] = value;

			if (firePropChangedEvent) OnPropertyChanged(commandName);
			return true;
		}

#if !UNITY3D
		/// <summary>
		///     Gets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertySelector">The property selector.</param>
		/// <returns>``0.</returns>
		protected virtual T GetValue<T>(Expression<Func<T>> propertySelector)
		{
			var propertyName = this.GetMemberName(propertySelector);
			return GetValue<T>(propertyName);
		}

		/// <summary>
		///     Gets the command.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="commandSelector">The command selector.</param>
		/// <returns></returns>
		protected virtual T GetCommand<T>(Expression<Func<T>> commandSelector)
			where T : ICommand
		{
			var commandName = this.GetMemberName(commandSelector);
			return GetCommand<T>(commandName);
		}
#else
		protected override void Awake()
		{
			base.Awake();
            DefaultDispatcher = DispatcherProxy.Default;
            Subscribe();
		}
#endif

		/// <summary>
		///     Gets the value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>``0.</returns>
		protected virtual T GetValue<T>(
#if !UNITY3D
[CallerMemberName]
#endif
string propertyName = null)
		{
			var value = default(T);
			if (!DataItems.ContainsKey(propertyName)) return value;
			value = (T)DataItems[propertyName];
			return value;
		}

		/// <summary>
		///     Gets the command.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="commandName">Name of the command.</param>
		/// <returns></returns>
		protected virtual T GetCommand<T>(
#if !UNITY3D
[CallerMemberName]
#endif
string commandName = null)
where T : ICommand
		{
			var value = default(T);
			if (!Commands.ContainsKey(commandName)) return value;
			value = (T)Commands[commandName];
			return value;
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
		protected override bool SetValue<T>(ref T field, T value,
#if !UNITY3D
 [CallerMemberName]
#endif
 string propertyName = null,
 bool firePropChangingEvent = true,
 bool firePropChangedEvent = true)
		{
			if (_isSuspended)
			{
				firePropChangingEvent = false;
				firePropChangedEvent = false;

				if (!_properties.Contains(propertyName))
				{
					_properties.Add(propertyName);
				}
			}

			var res = base.SetValue(ref field, value, propertyName, firePropChangingEvent, firePropChangedEvent);
			return res;
		}

		/// <summary>
		/// Subscribes this instance.
		/// </summary>
		public virtual void Subscribe()
		{

		}

		/// <summary>
		/// Unsubscribe all registered callbacks on this instance
		/// </summary>
		public virtual void Unsubscribe()
		{
			var ary = _messengerCallbacks.CreateArray();
			foreach (var id in ary)
			{
				Unsubscribe(id);
			}
			_messengerCallbacks.Clear();
		}

		/// <summary>
		/// Gets the messenger.
		/// </summary>
		/// <value>
		/// The messenger.
		/// </value>
		protected virtual IMessenger Messenger
		{
			get
			{
				var messenger = Messaging.Messenger.Default;//IocManager.Resolve<IMessenger>();
				return messenger;
			}
		}

		/// <summary>
		/// Publishes the specified payload.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="payload">The payload.</param>
		protected virtual void Publish<T>(T payload)
		{
			Messenger.Publish<T>(payload);
		}

		/// <summary>
		/// Subscribes the specified callback.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="callback">The callback.</param>
		/// <param name="filter">The filter.</param>
		/// <param name="autoUnsubscribe">if set to <c>true</c> [auto unsubscribe] (default value = TRUE).</param>
		protected virtual void Subscribe<T>(Action<T> callback, Predicate<T> filter = null, bool autoUnsubscribe = true)
		{
			//Debug.Log("111" + Messenger);
			Messenger.Subscribe<T>(callback, filter);
			//Debug.Log("222");

			// host callback for future unsubscribe
			if (!autoUnsubscribe) return;
			var id = callback.CreateDelegateUniqueId();
			if (_messengerCallbacks.Contains(id)) return;
			_messengerCallbacks.Add(id);
		}

		/// <summary>
		/// The messenger callbacks
		/// </summary>
		private readonly IList<string> _messengerCallbacks = new List<string>();

		/// <summary>
		/// Unsubscribe the specified unique callback id.
		/// </summary>
		/// <param name="uniqueCallbackId">The unique callback id.</param>
		protected virtual void Unsubscribe(string uniqueCallbackId)
		{
			Messenger.Unsubscribe(uniqueCallbackId);

			// remove hosted callback
			if (!_messengerCallbacks.Contains(uniqueCallbackId)) return;
			_messengerCallbacks.Remove(uniqueCallbackId);
		}

		/// <summary>
		/// Unsubscribe the specified callback.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="callback">The callback.</param>
		protected virtual void Unsubscribe<T>(Action<T> callback)
		{
			Messenger.Unsubscribe<T>(callback);

			// remove hosted callback
			var id = callback.CreateDelegateUniqueId();
			if (!_messengerCallbacks.Contains(id)) return;
			_messengerCallbacks.Remove(id);
		}

		/// <summary>
		/// Gets the IOC manager.
		/// </summary>
		/// <value>
		/// The IOC manager.
		/// </value>
		protected virtual IIocManager IocManager
		{
			get { return Modularity.IocManager.Default; }
		}

		/// <summary>
		/// Resolves the specified instance with given parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		protected virtual T Resolve<T>(params object[] parameters)
		{
			var res = IocManager.Resolve<T>(parameters);
			return res;
		}

		/// <summary>
		/// Resolves the specified instance by registered key type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="registeredKeyType">Type of the registered key.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		protected virtual T Resolve<T>(Type registeredKeyType, params object[] parameters)
		{
			var res = IocManager.Resolve<T>(registeredKeyType, parameters);
			return res;
		}

		/// <summary>
		/// Registers the specified implementation type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="implementationType">Type of the implementation.</param>
		/// <param name="singleton">if set to <c>true</c> [singleton].</param>
		protected virtual void Register<T>(Type implementationType, bool singleton = false)
		{
			IocManager.Register<T>(implementationType, singleton, singleton);
		}

		/// <summary>
		/// Registers the specified instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance">The instance.</param>
		protected virtual void Register<TInterface, TImplementation>(TImplementation instance)
			where TImplementation : TInterface
		{
			IocManager.Register<TInterface, TImplementation>(instance);
		}

		protected virtual void Register<T>(T implementationInstance, bool isWeakRef = false) where T : class
		{
			IocManager.Register(implementationInstance, isWeakRef);
		}

		//protected virtual void Register<T>(object implementationInstance, bool isWeakRef = false) where T : class
		//{
		//	IocManager.Register<T>(implementationInstance);
		//}

		/// <summary>
		/// Writes log
		/// </summary>
		/// <param name="msgFormat">The MSG format.</param>
		/// <param name="args">The args.</param>
		protected virtual void Log(string msgFormat, params object[] args)
		{
#if UNITY3D
			Debug.Log(string.Format(msgFormat, args));
#else
			Debug.WriteLine(msgFormat, args);
#if NETFX_CORE
			LoggerManager.Default.LogMessage(LogOperationType.Trace, msgFormat, args);
#endif
#endif
		}

		/// <summary>
		/// Logs the error.
		/// </summary>
		/// <param name="msgFormat">The MSG format.</param>
		/// <param name="args">The args.</param>
		protected virtual void LogError(string msgFormat, params object[] args)
		{
#if UNITY3D
			Debug.LogError(string.Format(msgFormat, args));
#else
			Debug.WriteLine(msgFormat, args);
#if NETFX_CORE
			LoggerManager.Default.LogError(LogOperationType.Trace, msgFormat, args);
#endif
#endif
		}

		/// <summary>
		/// Logs the exception.
		/// </summary>
		/// <param name="exc">The exc.</param>
		/// <param name="ctxt">The CTXT.</param>
		protected virtual void LogException(Exception exc, Object ctxt)
		{
#if UNITY3D
			Debug.LogException(exc, ctxt);
#else
			Debug.WriteLine(exc);
#if NETFX_CORE
			LoggerManager.Default.LogError(LogOperationType.Trace, exc);
#endif
#endif
		}
	}

	/// <summary>
	/// Commands Container
	/// </summary>
	public class CommandsContainer : ObservableDictionary<object, ICommand>, ICommand
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CommandsContainer"/> class.
		/// </summary>
		public CommandsContainer()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandsContainer"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		public CommandsContainer(IEnumerable<KeyValuePair<object, ICommand>> dictionary)
			: base(dictionary)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandsContainer"/> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		public CommandsContainer(IEqualityComparer<object> comparer)
			: base(comparer)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandsContainer"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="comparer">The comparer.</param>
		public CommandsContainer(IEnumerable<KeyValuePair<object, ICommand>> dictionary, IEqualityComparer<object> comparer)
			: base(dictionary, comparer)
		{

		}

		/// <summary>
		/// Occurs when changes occur that affect whether or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>
		/// true if this command can be executed; otherwise, false.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public bool CanExecute(object parameter)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void Execute(object parameter)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Data Items Container
	/// </summary>
	public class DataItemsContainer : ObservableDictionary<object, object>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataItemsContainer"/> class.
		/// </summary>
		public DataItemsContainer()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataItemsContainer"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		public DataItemsContainer(IEnumerable<KeyValuePair<object, object>> dictionary)
			: base(dictionary)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataItemsContainer"/> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		public DataItemsContainer(IEqualityComparer<object> comparer)
			: base(comparer)
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataItemsContainer"/> class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="comparer">The comparer.</param>
		public DataItemsContainer(IEnumerable<KeyValuePair<object, object>> dictionary, IEqualityComparer<object> comparer)
			: base(dictionary, comparer)
		{

		}

		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset()
		{
			foreach (KeyValuePair<object, object> dataItem in this)
			{
				if (dataItem.Value == null) continue;
				var valueType = dataItem.Value.GetType();
				this[dataItem.Key] = Activator.CreateInstance(valueType);
			}
		}
	}
}