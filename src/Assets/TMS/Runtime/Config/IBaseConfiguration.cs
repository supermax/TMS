using System.Collections.Generic;

namespace TMS.Common.Config
{
	public interface IBaseConfiguration
	{
		IDictionary<string, object> CommonParams { get; set; }

		T GetCommonParamsValueOrDefault<T>(string key);

		void LoadConfigFromFile(string filePath);
	}
}