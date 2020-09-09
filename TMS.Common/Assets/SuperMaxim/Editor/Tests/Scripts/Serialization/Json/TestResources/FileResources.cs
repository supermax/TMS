using System.IO;
using TMS.Common.Extensions;
using UnityEngine;

namespace TMS.Common.Tests.Serialization.Json.TestResources
{
	internal static class FileResources
	{
		#region Slots club login

		//@"E:\ws\dev\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_JsonTest.json";
		//@"Z:\SuperMax\Git\swar\skywarsarchonrises\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_Config.json"

		public static TextAsset GetJsonTextAsset(string fileName)
		{
			ArgumentValidator.AssertNotNullOrEmpty(fileName, "fileName");
			
			var jTextAsset = Resources.Load<TextAsset>(fileName);
			return jTextAsset;
		}

		#endregion
	}
}
