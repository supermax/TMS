#define UNITY3D

#region

using TMS.Common.Logging;
using TMS.Common.Config;
using System;
using System.Collections.Generic;
using TMS.Common.Extensions;
using TMS.Common.Logging.Api;
using TMS.Common.Messaging;
using TMS.Common.Modularity;
using TMS.Common.Network.Request.Api;
using TMS.Common.Serialization.Json;
#if UNITY3D
using UnityEngine;

#endif

#endregion

namespace TMS.Common.Network.Dispatcher.Api
{
	[IocTypeMap(typeof (INetworkDispatcherConfiguration), true)]
	public class NetworkDispatcherConfiguration : BaseConfiguration, INetworkDispatcherConfiguration
	{
		protected const string ConfigFilePath = @"Config/NetworkConfig";

		public virtual NetworkRequestConfigurationData RequestsConfig { get; set; }

		public NetworkDispatcherConfiguration()
		{
			LoadConfigFromFile(ConfigFilePath); // TODO call from another class
		}

		public override IBaseConfiguration LoadConfigFromFile(string filePath, bool updateConsumers = true)
		{
			var txt = Resources.Load<TextAsset>(filePath);
			var res = JsonMapper.Default.ToObject<NetworkRequestConfigurationData>(txt.text);
			return Update(res, updateConsumers);
		}

		public override IBaseConfiguration Update(IBaseConfiguration config, bool updateConsumers = true)
		{
			var newConfig = (NetworkRequestConfigurationData) config;

			if (RequestsConfig == null)
			{
				RequestsConfig = newConfig;

				Loggers.Default.NetworkLogger.Write(string.Format(
					"{0}->Update(network config assigned)", GetType()));

				return RequestsConfig;
			}

			if (newConfig.Default != null)
			{
				RequestsConfig.Default = newConfig.Default;
			}
			else
			{
				Loggers.Default.NetworkLogger.Write(LogSourceType.Error,
					string.Format("{0}->Update(newConfig.Default == NULL)",
						GetType()));
			}

			if (newConfig.Services.IsNullOrEmpty())
			{
				Loggers.Default.NetworkLogger.Write(LogSourceType.Error,
					string.Format("{0}->Update(newConfig.Services == NULL\\Empty)",
						GetType()));
				return RequestsConfig;
			}

			foreach (var svc in newConfig.Services)
			{
				RequestsConfig.Services[svc.Key] = svc.Value;
			}

			Loggers.Default.NetworkLogger.Write(string.Format(
				"{0}->Update(network config updated with {1} services)",
				GetType(), newConfig.Services.Count));

			if (updateConsumers)
			{
				Messenger.Default.Publish<INetworkDispatcherConfiguration>(this);
			}
			return RequestsConfig;
		}
	}
}