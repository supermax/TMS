#region

using TMS.Common.Logging;
using TMS.Common.Config;
using System;
using System.Collections.Generic;
using TMS.Common.Modularity;
using TMS.Common.Serialization.Json;
#if UNITY3D
using UnityEngine;

#endif

#endregion

namespace TMS.Common.Network
{
	[IocTypeMap(typeof (INetworkDispatcherConfiguration), true)]
	public class NetworkDispatcherConfiguration : BaseConfiguration, INetworkDispatcherConfiguration
	{
		public virtual NetworkRequestConfigurationData RequestsConfig { get; set; }
		
		public override void LoadConfigFromFile(string filePath)
		{			
#if UNITY3D
			var txt = Resources.Load<TextAsset>(filePath);
			var json = txt.text;			
#else
			var json = System.IO.File.ReadAllText(filePath);
#endif
			RequestsConfig = JsonMapper.Default.ToObject<NetworkRequestConfigurationData>(json);

			Loggers.Default.NetworkLogger.Write(LogSourceType.Trace, "Config JSON: " + json);
		}
	}
}