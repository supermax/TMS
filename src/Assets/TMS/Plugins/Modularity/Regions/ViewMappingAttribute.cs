using System;
using UnityEngine;

namespace TMS.Common.Modularity.Regions
{
	/// <summary>
	/// View Mapping Attribute
	/// <remarks>Can be applied to any <see cref="MonoBehaviour"/> script\class</remarks>
	/// </summary>
	public class ViewMappingAttribute : IocTypeMapAttribute
	{
		/// <summary>
		/// Gets or sets the type of the view.
		/// </summary>
		/// <value>
		/// The type of the view.
		/// </value>
		public Type ViewType { get; set; }

		/// <summary>
		/// Gets or sets the name of the region.
		/// </summary>
		/// <value>
		/// The name of the region.
		/// </value>
		public string RegionName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is delayed region.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is delayed region; otherwise, <c>false</c>.
		/// </value>
		public bool IsDelayedRegion { get; set; }

		/// <summary>
		/// Gets or sets the region mapping.
		/// </summary>
		/// <value>
		/// The region mapping.
		/// </value>
		public RegionMappingType RegionMapping { get; set; }

		/// <summary>
		/// Is mapping processing now
		/// </summary>
		private bool _isProcessing;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewMappingAttribute"/> class.
		/// </summary>
		/// <param name="viewType">Type of the view.</param>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="isDelayedRegion">if set to <c>true</c> [is delayed region].</param>
		public ViewMappingAttribute(Type viewType, string regionName = null, bool isDelayedRegion = false)
		{
			ViewType = viewType;
			RegionName = regionName;
			IsDelayedRegion = isDelayedRegion;
		}

		/// <summary>
		/// Called on owner's instantiation.
		/// </summary>
		/// <param name="instance">The instance.</param>
		protected internal override void OnInstantiation(object instance)
		{
			if (!_isProcessing && RegionMapping == RegionMappingType.OnInstantiation)
			{
				_isProcessing = true;
				try
				{
					if (RegionName != null)
					{
						var manager = IocManager.Default.Resolve<IRegionManager>();
						manager.MapView(instance as GameObject, RegionName, IsDelayedRegion);
					}
				}
				finally
				{
					_isProcessing = false;
				}
			}
			base.OnInstantiation(instance);
		}

		/// <summary>
		/// Called on attribute's registration.
		/// </summary>
		/// <param name="ownerType">Type of the owner.</param>
		protected internal override void OnRegistration(Type ownerType)
		{
			if (!_isProcessing && RegionMapping == RegionMappingType.OnRegistration)
			{
				_isProcessing = true;
				try
				{
					if (RegionName != null)
					{
						var manager = IocManager.Default.Resolve<IRegionManager>();
						manager.MapView(ownerType, RegionName, IsDelayedRegion);
					}
				}
				finally
				{
					_isProcessing = false;
				}
			}
			base.OnRegistration(ownerType);
		}
	}
}