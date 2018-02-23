using System;

namespace TMS.Common.Logging.Api
{
	public class FileLoggerCallbackArgs : EventArgs
	{
		public virtual string FilePath { get; protected internal set; }

		public virtual FileLoggerAction Action { get; protected internal set; }

		public virtual FileLoggerResult Status { get; protected internal set; }

		public virtual Exception Error { get; protected internal set; }

		public virtual string Text { get; protected internal set; }

		public virtual byte[] Data { get; protected internal set; }

		public override string ToString()
		{
			return string.Format("FilePath: '{0}', Action: '{1}', Status: '{2}', Error: '{3}', Text (Length): {4}, Data (Length): {5}", 
				FilePath, Action, Status, Error, Text != null ? Text.Length : 0, Data != null ? Data.Length : 0);
		}

		protected internal FileLoggerCallbackArgs()
		{
				
		}

		protected internal FileLoggerCallbackArgs(string filePath, FileLoggerAction action, FileLoggerResult status, Exception error = null)
		{
			FilePath = filePath;
			Action = action;
			Status = status;
			Error = error;
		}

		protected internal FileLoggerCallbackArgs(string text, string filePath, 
			FileLoggerAction action, FileLoggerResult status, Exception error = null) : 
			this(filePath, action, status, error)
		{
			Text = text;
		}

		protected internal FileLoggerCallbackArgs(byte[] data, string filePath, 
			FileLoggerAction action, FileLoggerResult status, Exception error = null) : this(filePath, action, status, error)
		{
			Data = data;
		}
	}
}