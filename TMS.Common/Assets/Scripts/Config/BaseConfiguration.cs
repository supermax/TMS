#define UNITY3D

using TMS.Common.Logging;
using System;
using System.Collections.Generic;
using TMS.Common.Extensions;
using TMS.Common.Messaging;
using TMS.Common.Modularity;
using TMS.Common.Serialization.Json;
#if UNITY3D || UNITY_3D
using UnityEngine;
#endif

namespace TMS.Common.Config
{
	[JsonDataContract]
	[Serializable]
	public abstract class BaseConfiguration : IBaseConfiguration
	{
		[JsonDataMember("enabled")]
		public virtual bool IsEnabled { get; set; }

		[JsonDataMember("params")]
		public virtual Dictionary<string, object> CommonParams { get; set; }

		public virtual T GetCommonParamsValueOrDefault<T>(string key, T defaultValue = default(T))
		{
			if (CommonParams.IsNullOrEmpty() || !CommonParams.ContainsKey(key))
			{
				return defaultValue;
			}
			try
			{
				var value = (T)Convert.ChangeType(CommonParams[key], typeof(T));
				return value;
			}
			catch (Exception ex)
			{
				Loggers.Default.ConsoleLogger.Write(ex);
				return defaultValue;
			}			
		}

		public virtual T GetCommonParamsValue<T>(string key)
		{
			var value = (T)Convert.ChangeType(CommonParams[key], typeof(T));
			return value;
		}

		public virtual IBaseConfiguration LoadConfigFromFile(string filePath, bool updateConsumers = true)
		{
			throw new NotImplementedException();
		}

		public virtual void LoadConfigFromFileAsync(string filePath, Action<IBaseConfiguration> callback, bool updateConsumers = true)
		{
			throw new NotImplementedException();
		}

		public virtual IBaseConfiguration Update(IBaseConfiguration newConfig, bool updateConsumers = true)
		{
			throw new NotImplementedException();
		}

		public virtual void UpdateConsumers()
		{
			Messenger.Default.Publish(this);
		}

		public virtual string ToJson()
		{
			// TODO think how to remove dependency on json mapper here
			var json = JsonMapper.Default.ToJson(this);
			return json;
		}

		protected BaseConfiguration()
		{
			IsEnabled = true;
		}
	}
}