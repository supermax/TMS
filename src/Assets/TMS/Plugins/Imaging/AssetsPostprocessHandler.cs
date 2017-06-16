#if UNITY_EDITOR

#region

using System;
using UnityEditor;

#endregion

namespace TMS.Common.Imaging
{
	public class AssetsPostprocessHandler : AssetPostprocessor
	{
		public static event EventHandler PostprocessAllAssets = delegate { };

		public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
													string[] movedAssets, string[] movedFromAssetPaths)
		{
			//foreach (var str in importedAssets)
			//{
			//	Debug.Log("Reimported Asset: " + str);
			//}

			//foreach (var str in deletedAssets)
			//{
			//	Debug.Log("Deleted Asset: " + str);
			//}

			//for (var i = 0; i < movedAssets.Length; i++)
			//{
			//	Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
			//}

			PostprocessAllAssets(null, EventArgs.Empty);
		}
	}
}

#endif