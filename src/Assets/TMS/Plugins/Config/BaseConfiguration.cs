using TMS.Common.Logging;
using System;
using System.Collections.Generic;

#if UNITY3D || UNITY_3D
using UnityEngine;
#endif

namespace TMS.Common.Config
{
	public abstract class BaseConfiguration : IBaseConfiguration
	{
		public virtual IDictionary<string, object> CommonParams { get; set; }

		protected BaseConfiguration()
		{
			CommonParams = new Dictionary<string, object>();
		}

		public virtual T GetCommonParamsValueOrDefault<T>(string key)
		{
			var value = default(T);
			if (!CommonParams.ContainsKey(key)) return value;

			try
			{
				value = (T)Convert.ChangeType(CommonParams[key], typeof(T));
			}
			catch (Exception ex)
			{
				Loggers.Default.ConsoleLogger.Write(ex);
			}
			return value;
		}
		
		public abstract void LoadConfigFromFile(string filePath);
	}
}