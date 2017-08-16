#region

using System;
using System.Collections;
using System.Text;
using TMS.Common.Extensions;
using TMS.Common.Logging;
using TMS.Common.Messaging;
using TMS.Common.Modularity;
using UnityEngine;

#endregion

namespace TMS.Common.Network
{
	// TODO add IsDebug and print all debug info in debug mode only
	/// <summary>
	/// Service Request Task
	/// </summary>
	public class ServiceRequestTask : IServiceRequestTask
	{
		protected bool IsAborted;
		protected bool IsRunning;
		protected int RetriesCount;
		protected WWW Www;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceRequestTask"/> class.
		/// </summary>
		/// <param name="payload">The payload.</param>
		/// <param name="config">The configuration.</param>
		public ServiceRequestTask(BaseServiceRequestPayload payload)
		{
			RequestPayload = payload;
		}

		protected virtual bool IsAbortRequestRequired(BaseServiceRequestPayload p)
		{
			var handle = p.AbortAll || (p.Abort && Equals(RequestPayload, p));
			return handle;
		}

		protected virtual void OnAbortServiceRequest(BaseServiceRequestPayload payload)
		{
			Dispose();

			var responsePayload = IocManager.Default.Resolve<IServiceFactory>()
				.CreateResponsePayload(payload, string.Empty, ServiceResponseState.AbortedByClient);
			if (responsePayload == null)
			{
				Loggers.Default.NetworkLogger.Write(LogSourceType.Error, "OnAbortServiceRequest(Cannot create response payload)");
				return;
			}

			Messenger.Default.PublishAndUnsubscribe(responsePayload, payload.ResponseCallbackId);
		}

		#region Implementation of IServiceRequestTask

		public virtual BaseServiceRequestPayload RequestPayload { get; protected internal set; }

		public virtual void Dispose()
		{
			Stop();

			// TODO: check if need to reset
			//Configuration = null;
			//RequestPayload = null;
		}

		public virtual void Start()
		{
			if (Www != null || IsRunning) return;

			IsRunning = true;

			Loggers.Default.NetworkLogger.Write(string.Format("--- START (SVC: {0})", RequestPayload.Config.ServiceName));

			Messenger.Default.Subscribe<BaseServiceRequestPayload>(OnAbortServiceRequest, IsAbortRequestRequired, true);

			InitWww();

			ServiceRequestTaskDispatcher.Default.StartCoroutine(_runTimeoutTimerEnumerator = RunTimeoutTimer());
			ServiceRequestTaskDispatcher.Default.StartCoroutine(_doRequestEnumerator = DoRequest());
		}

		private IEnumerator _runTimeoutTimerEnumerator;

		private IEnumerator _doRequestEnumerator;

		protected virtual string GetServiceUrl()
		{
			var svcUrl = RequestPayload.Config.ServiceUrl;
			if (RequestPayload.UrlParams.IsNullOrEmpty()) return svcUrl;

			if (svcUrl.EndsWith("/"))
			{
				svcUrl = svcUrl.TrimEnd("/".ToCharArray());
			}
			if (!svcUrl.Contains("?"))
			{
				svcUrl += "?";
			}
			else if (!svcUrl.EndsWith("&"))
			{
				svcUrl += "&";
			}

			var svcUrlBuilder = new StringBuilder(svcUrl);
			foreach (var arg in RequestPayload.UrlParams)
			{
				svcUrlBuilder.AppendFormat("{0}={1}&", arg.Key, arg.Value);
			}
			svcUrl = svcUrlBuilder.ToString();

			if (svcUrl.EndsWith("&"))
			{
				svcUrl = svcUrl.TrimEnd("&".ToCharArray());
			}
			return svcUrl;
		}

		protected virtual void InitWww()
		{
			var svcUrl = GetServiceUrl();
			Loggers.Default.NetworkLogger.Write(string.Format(">> Sending ({0}) request to: {1}", RequestPayload.RequestType, svcUrl));

			switch (RequestPayload.RequestType)
			{
				case ServiceRequestType.Post:
					InitPostRequest(svcUrl);
					break;

				case ServiceRequestType.Get:
					InitGetRequest(svcUrl);
					break;
			}
		}

		protected virtual void InitGetRequest(string svcUrl)
		{
			Www = new WWW(svcUrl);
		}

		protected virtual void InitPostRequest(string svcUrl)
		{
			var data = RequestPayload.RawRequestData;
			if (!data.IsNullOrEmpty())
			{
				Www = !RequestPayload.HeaderParams.IsNullOrEmpty() ? new WWW(svcUrl, data, RequestPayload.HeaderParams) : new WWW(svcUrl, data);
				return;
			}

			var form = new WWWForm();
			if (!RequestPayload.PostParams.IsNullOrEmpty())
			{
				foreach (var arg in RequestPayload.PostParams)
				{
					form.AddField(arg.Key, arg.Value);
				}
			}
			Www = !RequestPayload.HeaderParams.IsNullOrEmpty() ? new WWW(svcUrl, form.data, RequestPayload.HeaderParams) : new WWW(svcUrl, form);			
		}

		public virtual void Stop()
		{
			if (Www == null) return;

			Messenger.Default.Unsubscribe<BaseServiceRequestPayload>(OnAbortServiceRequest);

			IsAborted = true;
			IsRunning = false;

			if (_runTimeoutTimerEnumerator != null)
			{
				ServiceRequestTaskDispatcher.Default.StopCoroutine(_runTimeoutTimerEnumerator);
				_runTimeoutTimerEnumerator = null;
			}
			if (_doRequestEnumerator != null)
			{
				ServiceRequestTaskDispatcher.Default.StopCoroutine(_doRequestEnumerator);
				_doRequestEnumerator = null;
			}

			Www.Dispose();
			Www = null;

			Loggers.Default.NetworkLogger.Write(string.Format("--- STOP (RetriesCount: {0}, MaxRetriesNum: {1})", RetriesCount, RequestPayload.Config.RequestMaxRetriesNum));
		}

		protected virtual void Restart()
		{
			Stop();
			IsAborted = false;
			Start();
		}

		protected virtual IEnumerator RunTimeoutTimer()
		{
			Loggers.Default.NetworkLogger.Write(string.Format("TimeoutTimer start: {0}", DateTime.Now));

			for (var i = 0; i < RequestPayload.Config.RequestTimeout * 2; i++)
			{
				yield return new WaitForSeconds(0.5f);

				if (IsAborted) break;
			}

			if (!IsAborted)
			{
				Loggers.Default.NetworkLogger.Write(string.Format("TimeoutTimer end: {0}", DateTime.Now));

				if (!IsAborted && RetriesCount < RequestPayload.Config.RequestMaxRetriesNum)
				{
					RetriesCount++;
					Restart();
				}
				else
				{
					Stop();

					if (RetriesCount >= RequestPayload.Config.RequestMaxRetriesNum)
					{
						OnRequestTimeout();
					}
				}
			}
		}

		protected virtual void OnRequestTimeout()
		{
			Stop(); // TODO check if any problem with this call

			var state = Application.internetReachability == NetworkReachability.NotReachable ?
							ServiceResponseState.InternetReachabilityError : ServiceResponseState.ConnectionTimeout;

			Loggers.Default.NetworkLogger.Write(LogSourceType.Error, 
				string.Format("OnRequestTimeout(state: {0}, url: {1})", state, RequestPayload.Config.ServiceUrl));

			var responsePayload = IocManager.Default.Resolve<IServiceFactory>()
				.CreateResponsePayload(RequestPayload, string.Empty, state,
				"The connection request has been aborted after timeout.");
			if (responsePayload == null)
			{
				Loggers.Default.NetworkLogger.Write(LogSourceType.Error, "OnRequestTimeout(Cannot create response payload)");
				return;
			}

			if (!RequestPayload.ResponseCallbackId.IsNullOrEmpty())
			{
				Messenger.Default.PublishAndUnsubscribe(responsePayload, RequestPayload.ResponseCallbackId);
			}
			else
			{
				Messenger.Default.Publish(responsePayload);
			}
		}

		protected virtual IEnumerator DoRequest()
		{
			yield return Www;

			if (IsAborted || !IsRunning)
			{
				Stop();
			}
			else if (!Www.error.IsNullOrEmpty())
			{
				OnServiceError(Www.error);
			}
			else if (Www.isDone)
			{
				switch (RequestPayload.ResponseDataType)
				{
					case ResponsePayloadDataType.Image:
						OnServiceResponse(Www.texture);
						break;

					default: // TODO handle specific types
						OnServiceResponse(Www.text);
						break;
				}				
			}
		}

		protected virtual void OnServiceResponse(Texture2D image)
		{
			Stop();

			Loggers.Default.NetworkLogger.Write(string.Format("OnServiceResponse(image: {0}, url: {1})", image, RequestPayload.Config.ServiceUrl));

			var responsePayload = IocManager.Default.Resolve<IServiceFactory>()
				.CreateResponsePayload(RequestPayload, image, ServiceResponseState.Success);
			InvokeResponseCallback(responsePayload);
		}

		protected virtual void OnServiceResponse(string text)
		{
			Stop();

			var txtLog = !text.IsNullOrEmpty() && text.Length > 1000 ? string.Format("{0}...", text.Substring(0, 1000)) : text;
			Loggers.Default.NetworkLogger.Write(string.Format("OnServiceResponse(text: {0}, url: {1})", txtLog, RequestPayload.Config.ServiceUrl));

			var responsePayload = IocManager.Default.Resolve<IServiceFactory>()
				.CreateResponsePayload(RequestPayload, text, ServiceResponseState.Success);
			InvokeResponseCallback(responsePayload);
		}

		protected virtual void InvokeResponseCallback(BaseServiceResponsePayload responsePayload)
		{
			if (responsePayload == null)
			{
				Loggers.Default.NetworkLogger.Write(LogSourceType.Error, "OnServiceResponse(Cannot create response payload)");
				return;
			}
			if (!RequestPayload.ResponseCallbackId.IsNullOrEmpty())
			{
				Messenger.Default.PublishAndUnsubscribe(responsePayload, RequestPayload.ResponseCallbackId);
			}
			else
			{
				Messenger.Default.Publish(responsePayload);
			}
		}

		protected virtual void OnServiceError(string error)
		{
			Stop();

			var errorTxt = !error.IsNullOrEmpty() && error.Length > 1000 ? string.Format("{0}...", error.Substring(0, 1000)) : error;
			Loggers.Default.NetworkLogger.Write(LogSourceType.Error,
				string.Format("OnServiceError(error: {0}, url: {1})", errorTxt, RequestPayload.Config.ServiceUrl));

			var responsePayload = IocManager.Default.Resolve<IServiceFactory>()
				.CreateResponsePayload(RequestPayload, string.Empty, ServiceResponseState.ServerError, error);
			InvokeResponseCallback(responsePayload);
		}
		#endregion
	}
}