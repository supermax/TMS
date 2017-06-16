#region Usings

using TMS.Common.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows.Input;
using TMS.Common.Extensions;

#if !NETFX_CORE && !UNITY3D && !SILVERLIGHT
using TMS.Common.Properties;
#elif SILVERLIGHT && !WINDOWS_PHONE && !UNITY3D
using TMS.Common.Properties;
#endif

using TMS.Common.Helpers;
using TMS.Common.Modularity;

#if UNITY3D
using UnityEngine;
#endif

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	/// Interface for Delegate Command
	/// </summary>
	public interface IDelegateCommand : ICommand
	{
		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </value>
		bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
		/// </value>
		bool IsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the command parameter.
		/// </summary>
		/// <value>
		/// The command parameter.
		/// </value>
		object CommandParameter { get; set; }

		/// <summary>
		/// Determines whether [is same execution method] [the specified method].
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>
		///   <c>true</c> if [is same execution method] [the specified method]; otherwise, <c>false</c>.
		/// </returns>
		bool IsSameExecutionMethod(Delegate method);

		/// <summary>
		/// Locks this instance.
		/// </summary>
		void Lock();

		/// <summary>
		/// Unlocks this instance.
		/// </summary>
		void Unlock();

		/// <summary>
		/// Gets a value indicating whether this instance [is locked].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is locked]; otherwise, unlocked<c>false</c>.
		/// </value>
		bool IsLocked { get; }
	}

	/// <summary>
	///     An <see cref="ICommand" /> whose delegates can be attached for <see cref="Execute" /> and
	///     <see cref="CanExecuteCommand" /> .
	///     It also implements the <see cref="IActiveAware" /> interface, which is
	///     useful when registering this command in aCompositeCommand/>
	///     that monitors command's activity.
	/// </summary>
	public class DelegateCommand : Observable, IDelegateCommand, IActiveAware
#if UNITY3D
, IInputListener
#endif
	{
		private DelegateReference _executeMethod;
		private DelegateReference _canExecuteMethod;
		private bool _isActive = true;
		private bool _isEnabled = true;

		private string _text;

		/// <summary>
		///     Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text
		{
			get { return _text; }
			set
			{
				//SetValue(ref _text, value);
				// TODO workaround for Fuc#ing iOS
				if (_text == value) return;

				OnPropertyChanging("Text");
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether this instance is enabled.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				//if (!SetValue(ref _isEnabled, value)) return;
				// TODO workaround for Fuc#ing iOS
				if (_isEnabled == value) return;

				OnPropertyChanging("IsEnabled");
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
				OnCanExecuteChanged();
			}
		}

#if !SILVERLIGHT && !NETFX_CORE
		[field: NonSerialized]
#endif
		private WeakDelegatesManager _canExecuteChangedHandlers;

		/// <summary>
		///     Gets the can execute changed handlers.
		/// </summary>
		/// <returns></returns>
		private WeakDelegatesManager GetCanExecuteChangedHandlers()
		{
			return _canExecuteChangedHandlers ?? (_canExecuteChangedHandlers = new WeakDelegatesManager());
		}

#if !SILVERLIGHT && !NETFX_CORE
		[field: NonSerialized]
#endif
		private WeakDelegatesManager _isActiveChangedHandlers;

		/// <summary>
		///     Gets the is active changed handlers.
		/// </summary>
		/// <returns></returns>
		private WeakDelegatesManager GetIsActiveChangedHandlers()
		{
			return _isActiveChangedHandlers ?? (_isActiveChangedHandlers = new WeakDelegatesManager());
		}

		/// <summary>
		///     Creates a new instance of a DelegateCommand, specifying both the execute action and the can execute function.
		/// </summary>
		/// <param name="executeMethod">
		///     The method to execute when <see cref="ICommand.Execute" /> is invoked.
		/// </param>
		/// <param name="canExecuteMethod">
		///     The <see cref="Func{T,TResult}" /> to invoked when
		///     <see cref="ICommand.CanExecute" /> is invoked.
		/// </param>
		public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
#if UNITY3D
			: this()
#endif
		{
			Init(executeMethod, canExecuteMethod);
		}

		/// <summary>
		/// Initialize with specified execute methods.
		/// </summary>
		/// <param name="executeMethod">The execute method.</param>
		/// <param name="canExecuteMethod">The can execute method.</param>
		/// <exception cref="System.ArgumentNullException">executeMethod</exception>
		internal protected virtual void Init(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
		{
			if (executeMethod == null || canExecuteMethod == null)
			{
				throw new ArgumentNullException("executeMethod",  Properties.Resources.ErrMsg_CommandDelegatesCantBeNull);
			}

			_executeMethod = new DelegateReference(executeMethod, true);
			_canExecuteMethod = new DelegateReference(canExecuteMethod, true);
		}

		/// <summary>
		///     Initializes a new instance of the DelegateCommand /> class.
		/// </summary>
		/// <param name="executeMethod">The execute method.</param>
		public DelegateCommand(Action<object> executeMethod)
#if UNITY3D
			: this()
#endif
		{
			Init(executeMethod);
		}

		/// <summary>
		/// Initialize with specified execute method.
		/// </summary>
		/// <param name="executeMethod">The execute method.</param>
		/// <exception cref="System.ArgumentNullException">executeMethod</exception>
		internal protected virtual void Init(Action<object> executeMethod)
		{
			if (executeMethod == null)
			{
				//System.Diagnostics.Debug.WriteLine(string.Format("[{0}] executeMethod == null !!!", typeof(DelegateCommand)));
				throw new ArgumentNullException("executeMethod", Properties.Resources.ErrMsg_CommandDelegatesCantBeNull);
			}

			_executeMethod = new DelegateReference(executeMethod, true);
			//System.Diagnostics.Debug.WriteLine(string.Format("[{0}] _executeMethod == {1} !!!", typeof(DelegateCommand), _executeMethod));

			Func<object, bool> func = o => CanExecuteCommand(this);
			_canExecuteMethod = new DelegateReference(func, true);
			//System.Diagnostics.Debug.WriteLine(string.Format("[{0}] _canExecuteMethod == {1} !!!", typeof(DelegateCommand), _canExecuteMethod));
		}

		/// <summary>
		///     Determines whether this instance can execute the specified CMD.
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <returns>
		///     <c>true</c> if this instance can execute the specified CMD; otherwise, <c>false</c>.
		/// </returns>
		protected static bool CanExecuteCommand(DelegateCommand cmd)
		{
			var res = cmd._executeMethod != null && cmd.IsEnabled;
			return res;
		}

		/// <summary>
		///     Gets or sets a value indicating whether the object is active.
		/// </summary>
		/// <value>
		///     <see langword="true" /> if the object is active; otherwise <see langword="false" />.
		/// </value>
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive == value) return;
				_isActive = value;
				OnIsActiveChanged();
			}
		}

		/// <summary>
		///     Raises <see cref="ICommand.CanExecuteChanged" /> on the UI thread so every
		///     command invoker can require <see cref="ICommand.CanExecute" /> to check if the
		///     CompositeCommand can execute.
		/// </summary>
		protected virtual void OnCanExecuteChanged()
		{
#if UNITY3D
			if (_executeMethod != null && _executeMethod.IsAlive && _executeMethod.WeakReference.Target != null)
			{
				//       gameObject.collider.enabled = IsEnabled;
			}
#endif
			if (_canExecuteChangedHandlers == null) return;
			_canExecuteChangedHandlers.Raise(false, this, EventArgs.Empty);
		}

		/// <summary>
		///     Raises DelegateCommand.CanExecuteChanged on the UI thread so every command invoker
		///     can require to check if the command can execute.
		///     <remarks>
		///         Note that this will trigger the execution of DelegateCommand.CanExecute once for each invoker.
		///     </remarks>
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged();
		}

		/// <summary>
		///     Fired if the <see cref="IsActive" /> property changes.
		/// </summary>
		public virtual event EventHandler IsActiveChanged
		{
			add { GetIsActiveChangedHandlers().AddListener(value); }
			remove
			{
				if (_isActiveChangedHandlers == null) return;
				_isActiveChangedHandlers.RemoveListener(value.CreateDelegateUniqueId());
			}
		}

		/// <summary>
		///     This raises the DelegateCommand.IsActiveChanged event.
		/// </summary>
		protected virtual void OnIsActiveChanged()
		{
#if UNITY3D
			//Debug.Log("OnIsActiveChanged");
			if (_executeMethod != null && _executeMethod.IsAlive && _executeMethod.WeakReference.Target != null)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(IsActive);
					//Debug.Log(string.Format("OnIsActiveChanged, IsActive = {0}", IsActive));
				}
			}
#endif
			if (_isActiveChangedHandlers == null) return;
			_isActiveChangedHandlers.Raise(false, this, EventArgs.Empty);
		}

		/// <summary>
		///     Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		void ICommand.Execute(object parameter)
		{
			Execute(parameter);
		}

		/// <summary>
		///     Defines the method that determines whether the command can execute in its current state.
		/// </summary>
		/// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		/// <returns>
		///     true if this command can be executed; otherwise, false.
		/// </returns>
		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute(parameter);
		}

		/// <summary>
		///     Executes the command with the provided parameter by invoking the <see cref="Action{Object}" /> supplied during construction.
		/// </summary>
		/// <param name="parameter"></param>
		public virtual void Execute(object parameter)
		{
			if (IsAsync)
			{
				_executeMethod.BeginInvoke(parameter);
			}
			else
			{
				_executeMethod.Invoke(parameter);
			}
		}

		/// <summary>
		///     Safes the execute.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		public virtual void SafeExecute(object parameter)
		{
			if (!CanExecute(parameter)) return;
			Execute(parameter);
		}

		/// <summary>
		///     Gets or sets a value indicating whether this instance is async.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is async; otherwise, <c>false</c>.
		/// </value>
		public bool IsAsync { get; set; }

		/// <summary>
		///     Determines if the command can execute with the provided parameter by invoking the <see cref="Func{Object,Bool}" /> supplied during construction.
		/// </summary>
		/// <param name="parameter">The parameter to use when determining if this command can execute.</param>
		/// <returns>
		///     Returns <see langword="true" /> if the command can execute.  <see langword="False" /> otherwise.
		/// </returns>
		public virtual bool CanExecute(object parameter)
		{
			var isOk = _canExecuteMethod == null || (bool)_canExecuteMethod.Invoke(parameter);
			return isOk && IsEnabled;
		}

		/// <summary>
		///     Occurs when changes occur that affect whether or not the command should execute. You must keep a hard
		///     reference to the handler to avoid garbage collection and unexpected results. See remarks for more information.
		/// </summary>
		/// <remarks>
		///     When subscribing to the <see cref="ICommand.CanExecuteChanged" /> event using
		///     code (not when binding using XAML) will need to keep a hard reference to the event handler. This is to prevent
		///     garbage collection of the event handler because the command implements the Weak Event pattern so it does not have
		///     a hard reference to this handler. An example implementation can be seen in the CompositeCommand and CommandBehaviorBase
		///     classes. In most scenarios, there is no reason to sign up to the CanExecuteChanged event directly, but if you do, you
		///     are responsible for maintaining the reference.
		/// </remarks>
		/// <example>
		///     The following code holds a reference to the event handler. The myEventHandlerReference value should be stored
		///     in an instance member to avoid it from being garbage collected.
		///     <code>
		/// 		EventHandler myEventHandlerReference = new EventHandler(this.OnCanExecuteChanged);
		/// 		command.CanExecuteChanged += myEventHandlerReference;
		/// 	</code>
		/// </example>
		public event EventHandler CanExecuteChanged
		{
			add { GetCanExecuteChangedHandlers().AddListener(value, keepAlive: true); }
			remove
			{
				if (_canExecuteChangedHandlers == null) return;
				_canExecuteChangedHandlers.RemoveListener(value.CreateDelegateUniqueId());
			}
		}

		/// <summary>
		///     Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing">
		///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		/// </param>
		/// <returns></returns>
		protected override bool Dispose(bool disposing)
		{
			if (!disposing) return false;

			_isActive = false;

			if (_canExecuteMethod != null)
			{
				_canExecuteMethod.Dispose();
				_canExecuteMethod = null;
			}

			if (_executeMethod != null)
			{
				_executeMethod.Dispose();
				_executeMethod = null;
			}

			if (_isActiveChangedHandlers != null)
			{
				_isActiveChangedHandlers.Dispose();
				_isActiveChangedHandlers = null;
			}

			if (_canExecuteChangedHandlers != null)
			{
				_canExecuteChangedHandlers.Dispose();
				_canExecuteChangedHandlers = null;
			}
			return base.Dispose(true);
		}

#if UNITY3D
		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateCommand"/> class.
		/// </summary>
		public DelegateCommand()
		{
		}

		/// <summary>
		/// Called when [input event occurred].
		/// </summary>
		/// <param name="type">The type.</param>
		protected internal virtual void OnInput(InputState type)
		{

		}

		private Collider _thisCollider;
		protected bool IsDownInputEnabled;

		/// <summary>
		/// Awakes this instance.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			var col = GetComponent<Collider>();
			ArgumentValidator.AssertNotNull(col, "col");

			_thisCollider = col;
			_thisCollider.enabled = false;
		}

		#region IInputListener Implementation
		protected override void OnDestroy()
		{
			var dispatcher = IocManager.Default.Resolve<IInputDispatcher>();
			dispatcher.RemoveListener(this);
			base.OnDestroy();
		}

		protected override void OnDisable()
		{
			var dispatcher = IocManager.Default.Resolve<IInputDispatcher>();
		    if (dispatcher != null)
		    {
		        dispatcher.RemoveListener(this);
		    }
			base.OnDisable();
		}

		protected override void OnEnable()
		{
			var dispatcher = IocManager.Default.Resolve<IInputDispatcher>();
		    if (dispatcher != null)
		    {
		        dispatcher.AddListener(this);
		    }
			base.OnEnable();
		}

		virtual public bool OnInputBegan(Vector2 position)
		{
			IsDownInputEnabled = true;
			OnInput(InputState.InputDown);
			return true;
		}

		virtual public bool OnInputMoved(Vector2 position)
		{
			OnInput(InputState.InputContinues);
			return false;
		}

		virtual public void OnInputEnded(Vector2 position)
		{
			var canExecute = CanExecute(CommandParameter);
			if (!canExecute) return;

			Execute(CommandParameter);
			OnInput(InputState.InputUp);
		}

		virtual public void OnInputCanceled(Vector2 position)
		{
			IsDownInputEnabled = false;
			OnInput(InputState.None);
		}

		virtual public void OnInputStationary(Vector2 position)
		{
		}

		/*
		 * collider.bounds returns an empty bounding box when the collider isn't enabled
		 */
		Bounds IInputListener.GetBounds()
		{
			if (_thisCollider == null)
			{
				_thisCollider = GetComponent<Collider>();
			}
			bool isEnabled = _thisCollider.enabled;
			_thisCollider.enabled = true;
			Bounds bounds = _thisCollider.bounds;
			_thisCollider.enabled = isEnabled;
			return bounds;
		}

		#endregion
#endif

		private object _commandParameter;

		/// <summary>
		/// The command parameter
		/// </summary>
		public object CommandParameter
		{
			get { return _commandParameter; }
			set { SetValue(ref _commandParameter, value); }
		}

		/// <summary>
		/// Determines whether [is same execution method] [the specified method].
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns>
		///   <c>true</c> if [is same execution method] [the specified method]; otherwise, <c>false</c>.
		/// </returns>
		public bool IsSameExecutionMethod(Delegate method)
		{
			var id = method.CreateDelegateUniqueId();
			var isSame = _executeMethod.Id == id;
			return isSame;
		}

		private int? _lockCount;

		/// <summary>
		/// Fixes the lock counter overflow.
		/// </summary>
		/// <param name="diff">The differential.</param>
		private void FixLockCounterOverflow(int diff)
		{
			if (_lockCount == null || _lockCount + diff <= int.MaxValue || _lockCount + diff >= int.MinValue) return;
			Loggers.Default.ConsoleLogger.Write(LogSourceType.Warning, "{0}.Lock() -> LockCounter overflow!", GetType());
			_lockCount = 0;
		}

		private bool _prevIsEnabled;

		/// <summary>
		/// Locks this instance.
		/// </summary>
		public void Lock()
		{
			FixLockCounterOverflow(1);
			_lockCount ++;

			if (_lockCount == null || _lockCount < 1) return;
			_prevIsEnabled = IsEnabled;

			IsEnabled = false;
		}

		/// <summary>
		/// Unlocks this instance.
		/// </summary>
		public void Unlock()
		{
			FixLockCounterOverflow(-1);
			_lockCount--;

			if (_lockCount == null || _lockCount > 0) return;
			IsEnabled = _prevIsEnabled;
		}

		/// <summary>
		/// Gets a value indicating whether this instance [is locked].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is locked]; otherwise, unlocked<c>false</c>.
		/// </value>
		public bool IsLocked { get { return _lockCount > 0; } }

#if UNITY3D
		[SerializeField]
		[HideInInspector]
		public GameObject CommandTarget;

		[SerializeField]
		[HideInInspector]
		public MonoBehaviour CommandComponent;

		[SerializeField]
		[HideInInspector]
		public string CommandMethod;

		[SerializeField]
		[HideInInspector]
		public UnityEngine.Object CommandParameter1;

		[SerializeField]
		[HideInInspector]
		public string CommandParameter2;

		/// <summary>
		/// Invoked after <see cref="Awake" />
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (CommandComponent == null || CommandMethod.IsNullOrEmpty()) return;

			var type = CommandComponent.GetType();
			var methods = type.GetMethods();
			var methodName = CommandMethod.ToLowerInvariant();

			foreach (var m in methods)
			{
				var ok = InitCommandDelegate(m, methodName);
				if (!ok) continue;
				break;
			}
		}

		/// <summary>
		/// Initializes the command delegate.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <returns></returns>
		protected virtual bool InitCommandDelegate(MethodInfo method, string methodName)
		{
			if (method == null || !string.Equals(method.ToString().ToLowerInvariant(), methodName)) return false;

			var parameters = method.GetParameters();
			if (parameters.IsNullOrEmpty())
			{
				Action<object> act = o => method.Invoke(CommandComponent, null);
				Init(act);
				return true;
			}

			var parameter = parameters.GetFirstOrDefault();
			if (!CommandParameter2.IsNullOrEmpty())
			{
				var obj = Convert.ChangeType(CommandParameter2, parameter.ParameterType);
				Action<object> act1 = o => method.Invoke(CommandComponent, new[] { obj });
				Init(act1);
				return true;
			}

			Action<object> act2 = o => method.Invoke(CommandComponent, new object[] { CommandParameter1 });
			Init(act2);
			return true;
		}		
#endif
	}
}