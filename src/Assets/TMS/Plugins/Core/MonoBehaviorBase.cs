#region

using System;
using TMS.Common.Logging;
using UnityEngine;

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	///     Mono Behavior Base Class
	/// </summary>
	public abstract class MonoBehaviourBase : MonoBehaviour
	{
		/// <summary>
		///     Gets or sets a value indicating whether this instance is destroyed.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is destroyed; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsDestroyed { get; protected set; }

	    public LogSeverityType LogSeverity;

	    protected virtual void LogMethod(Delegate method)
	    {
	        if (LogSeverity == LogSeverityType.Normal)
	        {
	            Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, method);
	        }
        }

        /// <summary>
        ///     Invoked after <see cref="Awake" />
        /// </summary>
        protected virtual void Start()
        {
            LogMethod((Action)Start);
        }

		/// <summary>
		///     Invoked on initialization (after Deserialization or on adding as component to some <see cref="GameObject" />)
		/// </summary>
		protected virtual void Awake()
		{
            LogMethod((Action)Awake);
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
		}

		/// <summary>
		/// Called when application quit
		/// </summary>
		protected virtual void OnApplicationQuit()
		{
            LogMethod((Action)OnApplicationQuit);
        }

        /// <summary>
        /// Called when [application pause].
        /// </summary>
        /// <param name="pauseStatus">if set to <c>true</c> [pause status].</param>
        protected virtual void OnApplicationPause(bool pauseStatus)
		{
            LogMethod((Action<bool>)OnApplicationPause);
        }
	}
}