using System.IO;
using UnityEngine;

namespace TMS.Common.Tests.Serialization.Json.TestResources
{
	internal static class FileResources
	{
		#region Slots club login

		// public const string JsonTestFilePath =
		// 	@"E:\ws\dev\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_JsonTest.json";

		public static TextAsset GetJsonTextAsset()
		{
			var jTextAsset = Resources.Load<TextAsset>(@"_JsonTest");
			return jTextAsset;
		}

		#endregion
	}
}
