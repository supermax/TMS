#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TMS.Common.Helpers
{
    [CreateAssetMenu(fileName = "PluginsUpdaterConfig", menuName = "TMS/Plugins/Update", order = 1)]
    public class PluginsUpdater : ScriptableObject
    {
        public static List<PluginsFolderMapping> Folders = new List<PluginsFolderMapping>();
		
		[MenuItem("Plugins/Refresh")]
        public static void RefreshPlugins()
        {
            //print("applicationPath: " + EditorApplication.applicationPath);

            //print("EditorApplication.applicationContentsPath:" + EditorApplication.applicationContentsPath);

            //var paths = AssetDatabase.GetSubFolders("Assets/Plugins");
            //foreach (var path in paths) 
            //{
            //    //print(path);
            //}
			AssetDatabase.Refresh();
        }
    }

	public struct PluginsFolderMapping
	{
		public string SourceFolder;

		public string DestinationFolder;
	}
}
#endif