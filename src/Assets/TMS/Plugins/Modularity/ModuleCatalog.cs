#region Usings

using TMS.Common.Serialization.Json;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if !UNITY3D && !UNITY_3D
#if !NETFX_CORE
using System.Windows.Markup;
#else
using Windows.UI.Xaml.Markup;
#endif
#endif

#endregion

namespace TMS.Common.Modularity
{
	/// <summary>
	/// Module Catalog
	/// </summary>
#if !UNITY3D && !UNITY_3D
#if !NETFX_CORE
	[ContentProperty("Modules")]
#else
	[ContentProperty(Name = "Modules")]
#endif
#endif
	[JsonDataContract]
	public class ModuleCatalog : IEnumerable<ModuleInfo>
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="ModuleCatalog" /> class.
		/// </summary>
		public ModuleCatalog()
		{
			Modules = new List<ModuleInfo>();
		}

		/// <summary>
		///     Gets or sets the modules.
		/// </summary>
		/// <value>
		///     The modules.
		/// </value>
		[JsonDataMember("modules")]
		public List<ModuleInfo> Modules { get; set; }

		/// <summary>
		///     Gets the count.
		/// </summary>
		[JsonDataMemberIgnore]
		public int Count
		{
			get { return Modules.Count; }
		}

		#region IEnumerable<ModuleInfo> Members

		/// <summary>
		///     Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<ModuleInfo> GetEnumerator()
		{
			return Modules.GetEnumerator();
		}

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Modules.GetEnumerator();
		}

		#endregion
	}
}