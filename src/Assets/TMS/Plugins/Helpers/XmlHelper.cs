#region Usings

using System;
using System.IO;
using System.Xml.Serialization;
using TMS.Common.Extensions;
using TMS.Common.Logging;

#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	///     XML Helper
	/// </summary>
	public static class XmlHelper
	{
		/// <summary>
		///     Writes data (
		///     <typeparam name="T"></typeparam>
		///     ) to file.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filePath">The file path.</param>
		/// <param name="data">The data.</param>
		public static void WriteToFile<T>(string filePath, T data)
		{
			Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, string.Format("writing XML file: \"{0}\"...", filePath));

			using (var file = new FileStream(filePath, FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof (T));
				serializer.Serialize(file, data);
			}

			Loggers.Default.ConsoleLogger.Write(LogSourceType.Trace, "writing XML file done.");
		}

		/// <summary>
		///     Deserializes the specified string to data (
		///     <typeparam name="T"></typeparam>
		///     ) structure.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stringData">The string data.</param>
		/// <returns></returns>
		public static T Deserialize<T>(string stringData)
		{
			if (stringData.IsNullOrEmpty())
			{
				Loggers.Default.ConsoleLogger.Write(LogSourceType.Error, "XML Deserialization failed: data is null or empty");
				return default(T);
			}

			var data = default(T);

			try
			{
				var serializer = new XmlSerializer(typeof (T));

				using (TextReader reader = new StringReader(stringData))
				{
					data = (T) serializer.Deserialize(reader);
				}

				return data;
			}
			catch (Exception e)
			{
				Loggers.Default.ConsoleLogger.Write(LogSourceType.Exception, "Error during deserialization", e);
			}

			return data;
		}
	}
}