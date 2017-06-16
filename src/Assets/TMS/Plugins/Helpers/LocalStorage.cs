using System;
using System.IO;
using TMS.Common.Core;
using TMS.Common.Modularity;

#if UNITY || UNITY3D || UNITY_3D
using UnityEngine;
#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif
#endif

namespace TMS.Common.Helpers
{
#if UNITY_EDITOR
	/// <summary>
	/// Local Folders
	/// </summary>
	[IocTypeMap(typeof(ILocalStorage), true)]
	public class LocalStorage : Singleton<ILocalStorage, LocalStorage>, ILocalStorage
	{
		/// <summary>
		/// Gets the local app data folder.
		/// </summary>
		/// <value>
		/// The local folder.
		/// </value>
		public string LocalDataFolderPath
		{
			get { return Application.persistentDataPath; }
		}

		/// <summary>
		/// Gets the available free space.
		/// </summary>
		/// <returns>
		/// Number of bytes
		/// </returns>
		public long GetAvailableFreeSpace()
		{
			//var driveLetter = Path.GetPathRoot(Application.persistentDataPath);
			//var drive = new System.IO.DriveInfo(driveLetter);
			
			//var config = IocManager.Default.Resolve<IConfigurationManager>();
			//var minSpace = config.ConfigurationData.AppConfigParameters.MinimumSpaceNeeded;

			const long minSpace = 10000000000;
			return minSpace; // TODO
		}

		/// <summary>
		/// Deletes the cache folder.
		/// </summary>
		public void DeleteCacheFolder()
		{
			// TODO
		}

		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="folderPath">The folder path.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="includeSubFolders">if set to <c>true</c> [include sub folders].</param>
		/// <returns>
		/// list of files
		/// </returns>
		public string[] GetFiles(string folderPath, string searchPattern, bool includeSubFolders)
		{
			var files = Directory.GetFiles(folderPath, searchPattern, includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			return files;
		}
	}
#elif UNITY_ANDROID
	/// <summary>
	/// Local Storage Helper
	/// </summary>
	[IocTypeMap(typeof (ILocalStorage), true, InstantiateOnRegistration = true)]
	public class LocalStorage : Singleton<ILocalStorage, LocalStorage>, ILocalStorage
	{
		private const string AndroidStorageClassNamespace = "storageExtensions.Storage";

		public string LocalDataFolderPath
		{
			get { return Application.persistentDataPath; }
		}

		/// <summary>
		/// Gets the available free space.
		/// </summary>
		/// <returns>
		/// Number of bytes
		/// </returns>
		public long GetAvailableFreeSpace()
		{
			long freeMemory = 0;
			using (var androidStorageClass = new AndroidJavaClass(AndroidStorageClassNamespace))
			{
				freeMemory = androidStorageClass.CallStatic<long>("FreeMemory");
			}
			return freeMemory;
		}

		/// <summary>
		/// Deletes the cache folder.
		/// </summary>
		public void DeleteCacheFolder()
		{
			try
			{
				var finalPath = Path.Combine(Application.persistentDataPath, "UnityCache");
				Directory.Delete(finalPath, true);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="folderPath">The folder path.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="includeSubFolders">if set to <c>true</c> [include sub folders].</param>
		/// <returns>
		/// list of files
		/// </returns>
		public string[] GetFiles(string folderPath, string searchPattern, bool includeSubFolders)
		{
			var files = Directory.GetFiles(folderPath, searchPattern,
				includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			return files;
		}
	}

#elif UNITY_IPHONE
	[IocTypeMap(typeof (ILocalStorage), true, InstantiateOnRegistration = true)]
	public class LocalStorage : Singleton<ILocalStorage, LocalStorage>, ILocalStorage
	{
		public string LocalDataFolderPath
		{
			get
			{
				return GetCachesPathNative();		
			}
		}

		public long GetAvailableFreeSpace ()
		{
			return GetAvailableFreeSpaceNative ();
		}

		public void DeleteCacheFolder ()
		{
			DeleteCacheFolderNative ();
		}

	#region ILocalFolders implementation

		public string[] GetFiles (string folderPath, string searchPattern, bool includeSubFolders)
		{
			var files = Directory.GetFiles(folderPath, searchPattern,
				includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			return files;
		}

	#endregion

		[DllImport("__Internal")]
		static extern public string GetCachesPathNative();

		[DllImport("__Internal")]
		static extern public long GetAvailableFreeSpaceNative ();

		[DllImport("__Internal")]
		static extern public void DeleteCacheFolderNative ();
	}
#else
	// TODO
	/// <summary>
	/// Local Folders
	/// </summary>
	[IocTypeMap(typeof(ILocalStorage), true)]
	public class LocalStorage : Singleton<ILocalStorage, LocalStorage>, ILocalStorage
	{
		/// <summary>
		/// Gets the local app data folder.
		/// </summary>
		/// <value>
		/// The local folder.
		/// </value>
		public string LocalDataFolderPath
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
		}

		/// <summary>
		/// Gets the available free space.
		/// </summary>
		/// <returns>
		/// Number of bytes
		/// </returns>
		public long GetAvailableFreeSpace()
		{
			//var driveLetter = Path.GetPathRoot(Application.persistentDataPath);
			//var drive = new System.IO.DriveInfo(driveLetter);

			//var config = IocManager.Default.Resolve<IConfigurationManager>();
			//var minSpace = config.ConfigurationData.AppConfigParameters.MinimumSpaceNeeded;

			const long minSpace = 10000000000;
			return minSpace; // TODO
		}

		/// <summary>
		/// Deletes the cache folder.
		/// </summary>
		public void DeleteCacheFolder()
		{
			// TODO
		}

		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="folderPath">The folder path.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="includeSubFolders">if set to <c>true</c> [include sub folders].</param>
		/// <returns>
		/// list of files
		/// </returns>
		public string[] GetFiles(string folderPath, string searchPattern, bool includeSubFolders)
		{
			var files = Directory.GetFiles(folderPath, searchPattern, includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			return files;
		}
	}
#endif
}