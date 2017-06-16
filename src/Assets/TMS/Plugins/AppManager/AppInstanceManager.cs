#region Usings

using System;
using System.Diagnostics;
//using System.Runtime.Remoting;
//using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Permissions;
using System.Threading;

#endregion

namespace TMS.Common.AppManager
{
	/// <summary>
	///     Application Instance Manager
	/// </summary>
	[Serializable]
	//[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public sealed class AppInstanceManager : MarshalByRefObject //: IDisposable
	{
		private const string AppExtStr = "app";

		private static bool _isFirstInstance;

		private static string[] _commandLineArgs;

		private readonly string _appName;

		private readonly AppInstanceScope _appScope;

		/// <summary>
		///     Initializes a new instance of the <see cref="AppInstanceManager" /> class.
		/// </summary>
		internal AppInstanceManager()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="AppInstanceManager" /> class.
		/// </summary>
		/// <param name="appName">Name of the app.</param>
		/// <param name="scope">The scope.</param>
		/// <param name="newInstanceHandler">The new instance handler.</param>
		public AppInstanceManager(string appName, AppInstanceScope scope,
		                          EventHandler<AppInstanceCallbackEventArgs> newInstanceHandler)
		{
			_appName = appName.Trim().ToLowerInvariant();
			_appScope = scope;
			_commandLineArgs = Environment.GetCommandLineArgs();

			var eventName = GetMachineChannelName();
			var eventWaitHandle = GetEventWaitHandle(eventName);

			if (IsFirstInstance || eventWaitHandle == null)
			{
				eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventName);
				ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, WaitOrTimerCallback, newInstanceHandler, Timeout.Infinite,
				                                       false);

				eventWaitHandle.Close();
				RegisterRemoteType(eventName);
			}
			else
			{
				_isFirstInstance = false;
				UpdateRemoteObject(eventName);
				eventWaitHandle.Set();
			}
		}

		/// <summary>
		///     Gets a value indicating whether this instance is first instance.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is first instance; otherwise, <c>false</c>.
		/// </value>
		public bool IsFirstInstance
		{
			get { return _isFirstInstance; }
		}

		/// <summary>
		///     Gets the command line args.
		/// </summary>
		public string[] CommandLineArgs
		{
			get { return _commandLineArgs; }
		}

		/// <summary>
		///     Gets the event wait handle.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <returns></returns>
		[DebuggerNonUserCode]
		private static EventWaitHandle GetEventWaitHandle(string eventName)
		{
			try
			{
				var eventWaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset, eventName);
				return eventWaitHandle;
			}
			catch (WaitHandleCannotBeOpenedException)
			{
				_isFirstInstance = true;
				return null;
			}
		}

		/// <summary>
		///     Gets the name of the machine channel.
		/// </summary>
		/// <returns></returns>
		private string GetMachineChannelName()
		{
			string eventName;
			switch (_appScope)
			{
				case AppInstanceScope.User:
					eventName = string.Format("{0}_{1}_{2}_{3}",
					                          Environment.MachineName,
					                          Environment.UserDomainName,
					                          Environment.UserName,
					                          _appName).Trim();
					break;

				case AppInstanceScope.Domain:
					eventName = string.Format("{0}_{1}_{2}",
					                          Environment.MachineName,
					                          Environment.UserDomainName,
					                          _appName).Trim();
					break;

				default:
					eventName = string.Format("{0}_{1}",
					                          Environment.MachineName,
					                          _appName).Trim();
					break;
			}

			eventName = eventName.Replace(" ", string.Empty);
			eventName = eventName.Replace("-", "_");
			eventName = eventName.Replace(".", "_").ToLowerInvariant();

			return eventName;
		}

		private string GetChannelAddress(string name)
		{
			var res = string.Format("ipc://{0}/{1}{2}", name, _appName, AppExtStr);
			return res;
		}

		/// <summary>
		///     Updates the remote object.
		/// </summary>
		/// <param name="uri">The URI.</param>
		private void UpdateRemoteObject(string uri)
		{
			//var clientChannel = new IpcClientChannel();
			//try
			//{
			//	ChannelServices.RegisterChannel(clientChannel, true);

			//	var addr = GetChannelAddress(uri);
			//	var callback = Activator.GetObject(typeof (AppInstanceManager), addr) as AppInstanceManager;
			//	if (callback != null)
			//	{
			//		callback.SetCommandLineArgs(IsFirstInstance, CommandLineArgs);
			//	}
			//}
			//finally
			//{
			//	ChannelServices.UnregisterChannel(clientChannel);
			//}

			throw new NotImplementedException();
		}

		/// <summary>
		///     Sets the command line args.
		/// </summary>
		/// <param name="isFirstInstance">
		///     if set to <c>true</c> [is first instance].
		/// </param>
		/// <param name="commandLineArgs">The command line args.</param>
		public void SetCommandLineArgs(bool isFirstInstance, string[] commandLineArgs)
		{
			_isFirstInstance = isFirstInstance;
			_commandLineArgs = commandLineArgs;
		}

		/// <summary>
		///     Registers the type of the remote.
		/// </summary>
		/// <param name="uri">The URI.</param>
		private void RegisterRemoteType(string uri)
		{
			//var serverChannel = new IpcServerChannel(uri);
			//ChannelServices.RegisterChannel(serverChannel, true);

			//RemotingConfiguration.RegisterWellKnownServiceType(
			//	typeof (AppInstanceManager),
			//	string.Format("{0}{1}", _appName, AppExtStr),
			//	WellKnownObjectMode.Singleton);

			//var process = Process.GetCurrentProcess();
			//process.Exited += delegate { ChannelServices.UnregisterChannel(serverChannel); };

			throw new NotImplementedException();
		}

		/// <summary>
		///     Waits the or timer callback.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <param name="timedOut">
		///     if set to <c>true</c> [timed out].
		/// </param>
		private void WaitOrTimerCallback(object state, bool timedOut)
		{
			var callback = state as EventHandler<AppInstanceCallbackEventArgs>;
			if (callback == null) return;

			callback(state, new AppInstanceCallbackEventArgs(IsFirstInstance, CommandLineArgs));
		}
	}
}