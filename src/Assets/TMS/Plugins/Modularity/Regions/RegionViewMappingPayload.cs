using System;
using System.ComponentModel;
using TMS.Common.Extensions;
using UnityEngine;

namespace TMS.Common.Modularity.Regions
{
	/// <summary>
	/// Region View Mapping Payload
	/// </summary>
	public class RegionViewMappingPayload 
	{
		/// <summary>
		/// Gets or sets the type of the view mapping.
		/// </summary>
		/// <remarks>*** MANDATORY PROPERTY ***</remarks>
		/// <value>
		/// The type of the view mapping.
		/// </value>
		public virtual RegionViewMappingType ViewMappingType { get; protected internal set; }

		/// <summary>
		/// Gets or sets the name of the region.
		/// </summary>
		/// <remarks>*** MANDATORY PROPERTY ***</remarks>
		/// <value>
		/// The name of the region.
		/// </value>
		public virtual string RegionName { get; protected internal set; }

		/// <summary>
		/// Gets or sets a value indicating whether this payload can be used in delayed region.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is for delayed region; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsDelayedRegion { get; set; }

		/// <summary>
		/// Gets or sets the index of view that will be mapped\unmapped.
		/// </summary>
		/// <value>
		/// The index.
		/// </value>
		public virtual int Index { get; protected internal set; }

		/// <summary>
		/// Gets or sets the name of the assign to new mapped view.
		/// </summary>
		/// <value>
		/// The name of the assign view.
		/// </value>
		public virtual string AssignViewName { get; protected internal set; }

		/// <summary>
		/// Gets or sets the view to be cloned and mapped into given region.
		/// </summary>
		/// <value>
		/// The view.
		/// </value>
		public virtual GameObject View { get; protected internal set; }

		/// <summary>
		/// Gets or sets the name of the prefab to be resolved from resources and mapped into given region.
		/// </summary>
		/// <value>
		/// The name of the prefab.
		/// </value>
		public virtual string PrefabName { get; protected internal set; }

		/// <summary>
		/// Gets or sets the data context.
		/// </summary>
		/// <value>
		/// The data context.
		/// </value>
		public virtual object DataContext { get; protected internal set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RegionViewMappingPayload"/> class.
		/// </summary>
		protected RegionViewMappingPayload()
		{
			Index = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegionViewMappingPayload" /> class.
		/// </summary>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="viewMappingType">Type of the view mapping.</param>
		/// <exception cref="System.ArgumentNullException">If "regionName" is null\empty</exception>
		/// <exception cref="System.InvalidOperationException">If "viewMappingType" is not one of values: 
		/// UnmapAllViews, HideAllViews, ShowAllViews, LockAllViews, UnlockAllViews, DestroyRegion</exception>
		public RegionViewMappingPayload(string regionName, RegionViewMappingType viewMappingType) : this()
		{
			ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");

			if (viewMappingType != RegionViewMappingType.UnmapAllViews &&
				viewMappingType != RegionViewMappingType.HideAllViews &&
				viewMappingType != RegionViewMappingType.ShowAllViews &&
				viewMappingType != RegionViewMappingType.LockAllViews &&
				viewMappingType != RegionViewMappingType.UnlockAllViews &&
				viewMappingType != RegionViewMappingType.DestroyRegion)
			{
				var allowedMappingTypes = string.Format(
					"This constructor can be used for these operations only: {0}, {1}, {2}, {3}, {4}, {5}",
					RegionViewMappingType.UnmapAllViews,
					RegionViewMappingType.HideAllViews,
					RegionViewMappingType.ShowAllViews,
					RegionViewMappingType.LockAllViews,
					RegionViewMappingType.UnlockAllViews,
					RegionViewMappingType.DestroyRegion);

				var innerExc = new InvalidEnumArgumentException("viewMappingType", (int)viewMappingType, typeof(RegionViewMappingType));
				throw new InvalidOperationException(allowedMappingTypes, innerExc);
			}

			RegionName = regionName;
			ViewMappingType = viewMappingType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegionViewMappingPayload" /> class.
		/// </summary>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="viewMappingType">Type of the view mapping.</param>
		/// <param name="view">The view.</param>
		/// <param name="index">The index.</param>
		/// <param name="assignViewName">Name of the assign view.</param>
		/// <param name="dataContext">The data context.</param>
		/// <exception cref="System.ArgumentNullException">If "regionName" is null\empty</exception>
		/// <exception cref="System.ArgumentNullException">If "view" is null</exception>
		/// <exception cref="System.InvalidOperationException">If "viewMappingType" is not MapView</exception>
		public RegionViewMappingPayload(string regionName, RegionViewMappingType viewMappingType, 
			GameObject view, int? index = null, string assignViewName = null, object dataContext = null) : this()
			//: this(regionName, viewMappingType)
		{
			ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");
			ArgumentValidator.AssertNotNull(view, "view");

			RegionName = regionName;
			ViewMappingType = viewMappingType;

			if (viewMappingType != RegionViewMappingType.MapView)
			{
				var allowedMappingTypes = string.Format(
					"This constructor can be used for this operation: {0}", RegionViewMappingType.MapView);

				var innerExc = new InvalidEnumArgumentException("viewMappingType", (int)viewMappingType, typeof(RegionViewMappingType));
				throw new InvalidOperationException(allowedMappingTypes, innerExc);
			}

			View = view;
			AssignViewName = assignViewName;
			DataContext = dataContext;
			if (index.HasValue) Index = index.Value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegionViewMappingPayload" /> class.
		/// </summary>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="viewMappingType">Type of the view mapping.</param>
		/// <param name="view">The view.</param>
		/// <param name="dataContext">The data context.</param>
		/// <exception cref="InvalidOperationException">If "viewMappingType" is not
		/// MapView, UnmapView, ShowView, HideView, LockView, UnlockView, UpdateDataContext</exception>
		/// <exception cref="ArgumentNullException">If "reagionName" is null or empty</exception>
		/// <exception cref="ArgumentNullException">If "view" is null</exception>
		public RegionViewMappingPayload(string regionName, RegionViewMappingType viewMappingType, 
			GameObject view, object dataContext = null) : this()
		{
			ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");
			ArgumentValidator.AssertNotNull(view, "view");

			if (viewMappingType != RegionViewMappingType.MapView &&
				viewMappingType != RegionViewMappingType.UnmapView &&
				viewMappingType != RegionViewMappingType.ShowView &&
				viewMappingType != RegionViewMappingType.HideView &&
				viewMappingType != RegionViewMappingType.LockView &&
				viewMappingType != RegionViewMappingType.UnlockView &&
				viewMappingType != RegionViewMappingType.UpdateDataContext)
			{
				var allowedMappingTypes = string.Format(
					"This constructor can be used for these operations only: {0}, {1}, {2}, {3}, {4}, {5}, {6}.", 
					RegionViewMappingType.MapView,
					RegionViewMappingType.UnmapView,
					RegionViewMappingType.ShowView,
					RegionViewMappingType.HideView,
					RegionViewMappingType.LockView,
					RegionViewMappingType.UnlockView,
					RegionViewMappingType.UpdateDataContext);

				var innerExc = new InvalidEnumArgumentException("viewMappingType", (int)viewMappingType, typeof(RegionViewMappingType));
				throw new InvalidOperationException(allowedMappingTypes, innerExc);
			}

			View = view;
			DataContext = dataContext;
			RegionName = regionName;
			ViewMappingType = viewMappingType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegionViewMappingPayload"/> class.
		/// </summary>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="viewMappingType">Type of the view mapping.</param>
		/// <param name="prefabName">Name of the prefab.</param>
		/// <param name="index">The index.</param>
		/// <param name="assignViewName">Name of the assign view.</param>
		/// <param name="dataContext">The data context.</param>
		/// <exception cref="System.ArgumentNullException">If "regionName" is null\empty</exception>
		/// <exception cref="System.ArgumentNullException">If "prefabName" is null\empty</exception>
		/// <exception cref="System.InvalidOperationException">If "viewMappingType" is not MapView</exception>
		public RegionViewMappingPayload(string regionName, RegionViewMappingType viewMappingType,
			string prefabName, int? index = null, string assignViewName = null, object dataContext = null) : this()
			//: this(regionName, viewMappingType)
		{
			ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");
			ArgumentValidator.AssertNotNullOrEmpty(prefabName, "prefabName");

			RegionName = regionName;
			ViewMappingType = viewMappingType;

			if (viewMappingType != RegionViewMappingType.MapView)
			{
				var allowedMappingTypes = string.Format(
					"This constructor can be used for this operation: {0}", RegionViewMappingType.MapView);

				var innerExc = new InvalidEnumArgumentException("viewMappingType", (int)viewMappingType, typeof(RegionViewMappingType));
				throw new InvalidOperationException(allowedMappingTypes, innerExc);
			}

			PrefabName = prefabName;
			AssignViewName = assignViewName;
			DataContext = dataContext;
			if (index.HasValue) Index = index.Value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegionViewMappingPayload" /> class.
		/// </summary>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="viewMappingType">Type of the view mapping.</param>
		/// <param name="prefabName">Name of the prefab.</param>
		/// <param name="dataContext">The data context.</param>
		/// <exception cref="InvalidOperationException">If "viewMappingType" is not
		/// MapView, UnmapView, ShowView, HideView, LockView, UnlockView, UpdateDataContext</exception>
		/// <exception cref="ArgumentNullException">If "regionName" is null\empty</exception>
		/// <exception cref="ArgumentNullException">If "prefabName" is null\empty</exception>
		public RegionViewMappingPayload(string regionName, RegionViewMappingType viewMappingType, 
			string prefabName, object dataContext = null) : this()
		{
			ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");
			ArgumentValidator.AssertNotNullOrEmpty(prefabName, "prefabName");

			if (viewMappingType != RegionViewMappingType.MapView &&
				viewMappingType != RegionViewMappingType.UnmapView &&
				viewMappingType != RegionViewMappingType.ShowView &&
				viewMappingType != RegionViewMappingType.HideView &&
				viewMappingType != RegionViewMappingType.LockView &&
				viewMappingType != RegionViewMappingType.UnlockView &&
				viewMappingType != RegionViewMappingType.UpdateDataContext)
			{
				var allowedMappingTypes = string.Format(
					"This constructor can be used for these operations only: {0}, {1}, {2}, {3}, {4}, {5}, {6}.",
					RegionViewMappingType.MapView,
					RegionViewMappingType.UnmapView,
					RegionViewMappingType.ShowView,
					RegionViewMappingType.HideView,
					RegionViewMappingType.LockView,
					RegionViewMappingType.UnlockView,
					RegionViewMappingType.UpdateDataContext);

				var innerExc = new InvalidEnumArgumentException("viewMappingType", (int)viewMappingType, typeof(RegionViewMappingType));
				throw new InvalidOperationException(allowedMappingTypes, innerExc);
			}

			RegionName = regionName;
			ViewMappingType = viewMappingType;
			PrefabName = prefabName;
			DataContext = dataContext;
		}
	}
}
