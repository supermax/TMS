#region Usings

using System.IO;
using TMS.Common.Tasks.Threading;

#endregion

namespace TMS.Common.Helpers
{
	public static class IOHelper
	{
		public static DirectoryCopyAction CopyDirectory(string sourceDirName, string destDirName, bool recursive)
		{
			var act = new DirectoryCopyAction(sourceDirName, destDirName, recursive);
			act.Copy();
			return act;
		}

		public static DirectoryCopyAction CopyDirectoryAsync(string sourceDirName, string destDirName, bool recursive)
		{
			var act = new DirectoryCopyAction(sourceDirName, destDirName, recursive);
			act.CopyAsync();
			return act;
		}
	}

	public class DirectoryCopyAction
	{
		public DirectoryCopyAction(string sourceDirName, string destDirName, bool recursive)
		{
			_sourceDirName = sourceDirName;
			_destDirName = destDirName;
			_recursive = recursive;
		}

		private readonly string _sourceDirName;

		private readonly string _destDirName;

		private readonly bool _recursive;

		public bool IsAborted { get; private set; }

		public void AbortCopy()
		{
			IsAborted = true;
			if(_task == null) return;
			_task.Abort();
			_task = null;
		}

		public DirectoryCopyAction Copy()
		{
			Copy(_sourceDirName, _destDirName, _recursive);
			return this;
		}

		private CustomTask<DirectoryCopyAction> _task;

		public DirectoryCopyAction CopyAsync()
		{
			_task = CustomTask.Create<DirectoryCopyAction>(Copy);
			_task.Run();
			return this;
		}

		private void Copy(string sourceDirName, string destDirName, bool recursive)
		{
			if (IsAborted) return;

			// Get the subdirectories for the specified directory.
			var dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			var dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			if(IsAborted) return;

			// Get the files in the directory and copy them to the new location.
			var files = dir.GetFiles();
			foreach (var file in files)
			{
				var tempPath = Path.Combine(destDirName, file.Name);
				file.CopyTo(tempPath, true);
				if (IsAborted) return;
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (!recursive || IsAborted) return;
			
			foreach (var subdir in dirs)
			{
				var temppath = Path.Combine(destDirName, subdir.Name);
				Copy(subdir.FullName, temppath, true);
			}
		}
	}
}