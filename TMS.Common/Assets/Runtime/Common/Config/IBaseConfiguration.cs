using System;
using System.Collections.Generic;
using TMS.Common.Serialization.Json;

namespace TMS.Common.Config
{
	public interface IBaseConfiguration : IJsonSerializable
	{
		bool IsEnabled { get; set; }

		Dictionary<string, object> CommonParams { get; set; }

		T GetCommonParamsValueOrDefault<T>(string key, T defaultValue = default(T));

		T GetCommonParamsValue<T>(string key);

		IBaseConfiguration LoadConfigFromFile(string filePath, bool updateConsumers = true) ;

		void LoadConfigFromFileAsync(string filePath, Action<IBaseConfiguration> callback, bool updateConsumers = true);

		IBaseConfiguration Update(IBaseConfiguration newConfig, bool updateConsumers = true);
	}
}