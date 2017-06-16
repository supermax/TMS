#region Usings

using System;
using System.IO;
using TMS.Common.Core;

#if !NETFX_CORE
using System.Runtime.Serialization;
#else
using Windows.ApplicationModel;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Markup;
#endif

#endregion

namespace TMS.Common.Serialization
{
	/// <summary>
	///     Object Serializer
	/// </summary>
	public class ObjectSerializer : Singleton<ObjectSerializer>
	{
		/// <summary>
		///     Deserializes from file.
		/// </summary>
		/// <typeparam name="T"> object type </typeparam>
		/// <param name="filePath"> The file path. </param>
		/// <returns> deserialized object </returns>
		public T DeserializeFromFile<T>(string filePath)
		{
            var txt = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            var result = Json.JsonMapper.Default.ToObject<T>(txt);              
            return result;
		}

		/// <summary>
		///     Serializes object to file.
		/// </summary>
		/// <typeparam name="T"> object type </typeparam>
		/// <param name="graph"> The graph. </param>
		/// <param name="filePath"> The file path. </param>
		public void SerializeToFile<T>(T graph, string filePath)
		{
            var text = Json.JsonMapper.Default.ToJson(graph);
            File.WriteAllText(filePath, text);
		}

		/// <summary>
		///     Clones the specified graph.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="graph"> The graph. </param>
		/// <returns> </returns>
		public T Clone<T>(T graph)
		{
            var text = Json.JsonMapper.Default.ToJson(graph);
            var result = Json.JsonMapper.Default.ToObject<T>(text);
            return result;
		}
	}
}