#region Usings

using System;
using System.IO.IsolatedStorage;
using System.Reflection;
using TMS.Common.Core;

#endregion

namespace TMS.Common.Serialization
{
	/// <summary>
	///     Class IsolatedStorageHelper
	/// </summary>
	public class IsolatedStorageHelper : Singleton<IsolatedStorageHelper>
	{
		/////// <summary>
		///////     The _store name
		/////// </summary>
		////private string _storeName;

		/// <summary>
		///     Initializes a new instance of the <see cref="IsolatedStorageHelper" /> class.
		/// </summary>
		public IsolatedStorageHelper()
		{
			var name = Assembly.GetExecutingAssembly().GetName().Name;
			InitAppStore(name);
		}

		/// <summary>
		///     Initializes the app store.
		/// </summary>
		/// <param name="path">The path.</param>
		public void InitAppStore(string path)
		{
			//var storage = GetStore();
			//if (storage.DirectoryExists(path))
			//{
			//	_storeName = path;
			//	return;
			//}
			//storage.CreateDirectory(path);
			//_storeName = path;

			throw new NotImplementedException();
		}

		/// <summary>
		///     Gets the store.
		/// </summary>
		/// <returns>IsolatedStorageFile.</returns>
		private IsolatedStorageFile GetStore()
		{
			var storage = IsolatedStorageFile.GetStore(
				IsolatedStorageScope.User |
				IsolatedStorageScope.Domain |
				IsolatedStorageScope.Assembly,
				null, null);
			return storage;
		}

		/// <summary>
		///     Files the exists.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		public bool FileExists(string filePath)
		{
			//var storage = GetStore();
			//filePath = Path.Combine(_storeName, filePath);
			//var exists = storage.FileExists(filePath);
			//return exists;

			throw new NotImplementedException();
		}

		/// <summary>
		///     Deserializes from file.
		/// </summary>
		/// <typeparam name="T">object type</typeparam>
		/// <param name="filePath">The file path.</param>
		/// <returns>deserialized object</returns>
		public T DeserializeFromFile<T>(string filePath)
		{
			//var storage = GetStore();
			//filePath = Path.Combine(_storeName, filePath);
			//using (var reader = storage.OpenFile(filePath, FileMode.Open, FileAccess.Read))
			//{
			//	var serializer = new NetDataContractSerializer();
			//	var result = (T) serializer.Deserialize(reader);
			//	reader.Close();
			//	return result;
			//}

			throw new NotImplementedException();
		}

		/// <summary>
		///     Serializes object to file.
		/// </summary>
		/// <typeparam name="T">object type</typeparam>
		/// <param name="graph">The graph.</param>
		/// <param name="filePath">The file path.</param>
		public void SerializeToFile<T>(T graph, string filePath)
		{
			//var storage = GetStore();
			//filePath = Path.Combine(_storeName, filePath);
			//using (var writer = storage.CreateFile(filePath))
			//{
			//	var serializer = new NetDataContractSerializer();
			//	serializer.Serialize(writer, graph);
			//	writer.Close();
			//}

			throw new NotImplementedException();
		}
	}
}