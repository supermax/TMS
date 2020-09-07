#define UNITY3D

#region

using TMS.Common.Logging;
using TMS.Common.Config;
using TMS.Common.Extensions;
using TMS.Common.Messaging;
using TMS.Common.Modularity;
using TMS.Common.Modularity.Ioc;
using TMS.Common.Network.Request.Api;
using TMS.Common.Serialization.Json;

#if UNITY3D
using UnityEngine;
#endif

#endregion

namespace TMS.Common.Network.Config.Api
{
	[IocTypeMap(typeof (INetworkConfiguration), true)]
	public class NetworkConfiguration : BaseConfiguration, INetworkConfiguration
	{
		protected const string ConfigFilePath = @"Config/NetworkConfig";

		public virtual NetworkRequestConfigurationData RequestsConfig { get; set; }

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
			}
			else
			{
				if (newConfig.Default != null)
				{
					RequestsConfig.Default = newConfig.Default;
				}
				if (!newConfig.Services.IsNullOrEmpty())
				{
					foreach (var svc in newConfig.Services)
					{
						RequestsConfig.Services[svc.Key] = svc.Value;
					}

					Loggers.Default.NetworkLogger.Write(string.Format(
						"{0}->Update(network config updated with {1} services)",
						GetType(), newConfig.Services.Count));
				}
			}
			if (updateConsumers)
			{
				Messenger.Default.Publish<INetworkConfiguration>(this);
			}
			return RequestsConfig;
		}
	}
}