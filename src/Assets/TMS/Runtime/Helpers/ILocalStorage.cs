namespace TMS.Common.Helpers
{
	/// <summary>
	/// Unified interface for Local\Native Storage (by Platform)
	/// </summary>
	public interface ILocalStorage
	{
		/// <summary>
		/// Gets the local app data folder.
		/// </summary>
		/// <value>
		/// The local folder.
		/// </value>
		string LocalDataFolderPath { get; }

		/// <summary>
		/// Gets the available free space.
		/// </summary>
		/// <returns>Number of bytes</returns>
		long GetAvailableFreeSpace();

		/// <summary>
		/// Deletes the cache folder.
		/// </summary>
		void DeleteCacheFolder();

		/// <summary>
		/// Gets the files.
		/// </summary>
		/// <param name="folderPath">The folder path.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="includeSubFolders">if set to <c>true</c> [include sub folders].</param>
		/// <returns>
		/// list of files
		/// </returns>
		string[] GetFiles(string folderPath, string searchPattern, bool includeSubFolders);
	}
}