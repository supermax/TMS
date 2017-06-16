using System.IO;
using System.Text;
using TMS.Common.Extensions;
using TMS.Common.Properties;

namespace TMS.Common.IO
{
	public class TextFileHandler
	{
		public virtual TextFileHandlerMode Mode { get; protected internal set; }

		public virtual string FilePath { get; protected internal set; }

		public TextFileHandler(string filePath, TextFileHandlerMode mode)
		{
			Mode = mode;
			FilePath = filePath;
		}

		public virtual void SaveToFile(string text)
		{
			//ArgumentValidator.AssertNotNullOrEmpty(text, "text");

			//var dir = Path.GetDirectoryName(FilePath);
			//if (!Directory.Exists(dir))
			//{
			//	Directory.CreateDirectory(dir);
			//}

			//if (!_lastFilePath.IsNullOrEmpty())
			//{
			//	var fileInfo = new FileInfo(_lastFilePath);
			//	if (fileInfo.Exists && fileInfo.Length > Settings.MaxFileSize)
			//	{
			//		_lastFilePath = GetNewFilePath();
			//	}
			//}
			//else
			//{
			//	_lastFilePath = GetNewFilePath();
			//}

			//File.AppendAllText(_lastFilePath, text, Encoding.UTF8);
		}

	}

	public enum TextFileHandlerMode
	{
		CreateNew,
		Append
	}

	public class TextFileHandlerEventArgs
	{
		public virtual TextFileHandlerActionStatus Status { get; protected internal set; }

		public virtual TextFileHandler Writer { get; protected internal set; }
	}

	public enum TextFileHandlerActionStatus
	{
		Success,
		Fail,
		Aborted
	}
}