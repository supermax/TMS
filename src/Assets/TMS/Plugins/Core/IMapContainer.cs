using System.Collections;

namespace TMS.Common.Core
{
	/// <summary>
	/// Map container
	/// </summary>
	public interface IMapContainer
	{
		#region Properties

		/// <summary>
		/// Gets the map.
		/// </summary>
		/// <value>
		/// The map.
		/// </value>
		IDictionary Map { get; }

		#endregion
	}
}
