using System;
using TMS.Common.Logging;
using TMS.Common.Logging.Api;
using UnityEngine;

namespace TMS.Common.Core
{
	/// <inheritdoc />
	/// <summary>
	/// Scriptable Object Base
	/// </summary>
	/// <seealso cref="T:UnityEngine.ScriptableObject" />
	public abstract class ScriptableObjectBase : ScriptableObject
	{
		/// <summary>
		///     Gets or sets a value indicating whether this instance is destroyed.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is destroyed; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsDestroyed { get; protected set; }

		[SerializeField] private LogSeverityType _logSeverity;

		/// <summary>
		/// Gets or sets the log severity.
		/// </summary>
		/// <value>
		/// The log severity.
		/// </value>
		public virtual LogSeverityType LogSeverity
		{
			get { return _logSeverity; }
			set { _logSeverity = value; }
		}

		/// <summary>
		/// Logs the method.
		/// </summary>
		/// <param name="method">The method.</param>
		protected virtual void LogMethod(Delegate method)
		{
			if (LogSeverity == LogSeverityType.Normal)
			{
				Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, method);
			}
		}

		/// <summary>
		///     Invoked on initialization (after Deserialization or on adding as component to some <see cref="GameObject" />)
		/// </summary>
		protected virtual void Awake()
		{
			LogMethod((Action)Awake);

			//Debug.LogFormat(">>> {0} -> Awake() [ID: {1}]", GetType().Name, GetHashCode());
		}

		/// <summary>
		///     Called when this instance is enabled.
		/// </summary>
		protected virtual void OnEnable()
		{
			LogMethod((Action)OnEnable);
		}

		/// <summary>
		///     Called when this instance is enabled
		/// </summary>
		protected virtual void OnDisable()
		{
			LogMethod((Action)OnDisable);
		}

		/// <summary>
		///     Called when this instance is destroyed
		/// </summary>
		protected virtual void OnDestroy()
		{
			LogMethod((Action)OnDestroy);
			IsDestroyed = true;

			//Debug.LogFormat("<<< {0} -> OnDestroy() [ID: {1}]", GetType().Name, GetHashCode());
		}

#if UNITY_EDITOR
		/// <summary>
		/// Called when [editor validates].
		/// </summary>
		protected virtual void OnValidate()
		{
			
		}
#endif
	}
}