namespace TMS.Common.Modularity.Regions
{
	/// <summary>
	/// Region Mapping Type
	/// </summary>
	public enum RegionMappingType
	{
		/// <summary>
		/// Map view on registration
		/// </summary>
		OnRegistration = 0,

		/// <summary>
		/// Map view on instantiation
		/// </summary>
		OnInstantiation,

		/// <summary>
		/// Do nothing (manual mapping)
		/// </summary>
		None,
	}
}