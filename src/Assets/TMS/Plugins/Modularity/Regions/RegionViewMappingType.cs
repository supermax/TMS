namespace TMS.Common.Modularity.Regions
{
	/// <summary>
	/// Region View Mapping Type<para />(used in <see cref="RegionViewMappingPayload" />)
	/// </summary>
	public enum RegionViewMappingType
	{
		/// <summary>
		/// Map specific view in region<para/>(MUST provide RegionName + View and\or PrefavName)
		/// </summary>
		MapView,

		/// <summary>
		/// Unmap specific view in region<para/>(MUST provide RegionName + View and\or PrefavName)
		/// </summary>
		UnmapView,

		/// <summary>
		/// Unmap all views in region<para/>(MUST provide RegionName)
		/// </summary>
		UnmapAllViews,

		/// <summary>
		/// Lock specific view in region<para/>(MUST provide RegionName + View and\or PrefavName)
		/// </summary>
		LockView,

		/// <summary>
		/// Lock all views in region<para/>(MUST provide RegionName)
		/// </summary>
		LockAllViews,

		/// <summary>
		/// Unlock specific view in region<para/>(MUST provide RegionName + View and\or PrefavName)
		/// </summary>
		UnlockView,

		/// <summary>
		/// Unlock all views in region<para/>(MUST provide RegionName)
		/// </summary>
		UnlockAllViews,
		
		/// <summary>
		/// Show specific hidden view in region<para/>(MUST provide RegionName + View and\or PrefavName)
		/// </summary>
		ShowView,

		/// <summary>
		/// Show all hidden views in region<para/>(MUST provide RegionName)
		/// </summary>
		ShowAllViews,

		/// <summary>
		/// Hide specific view in region<para/>(MUST provide RegionName + View and\or PrefavName)
		/// </summary>
		HideView,

		/// <summary>
		/// Hide all views in region<para/>(MUST provide RegionName)
		/// </summary>
		HideAllViews,

		/// <summary>
		/// Updates data context for specific view<para/>(MUST provide RegionName + View and\or PrefavName)
		/// </summary>
		UpdateDataContext,

		/// <summary>
		/// Destroy the region<para/>(MUST provide RegionName)
		/// </summary>
		DestroyRegion
	}
}