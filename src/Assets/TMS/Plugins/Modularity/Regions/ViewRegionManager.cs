
#region

using System;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Core;
using TMS.Common.Extensions;
using TMS.Common.Messaging;
using UnityEngine;

#endregion

namespace TMS.Common.Modularity.Regions
{
	/// <summary>
	///     Region Manager
	/// </summary>
	[MessengerConsumer(typeof(IRegionManager), true, InstantiateOnRegistration = true)] 
	public class ViewRegionManager : ViewModelSingleton<IRegionManager, ViewRegionManager>, IRegionManager
	{
		private IDictionary<string, IList<RegionViewMappingPayload>> _delayedRegions;

		private IList<string> _registeredRegions;

		/// <summary>
		/// Gets the registered regions.
		/// </summary>
		/// <value>
		/// The registered regions.
		/// </value>
		protected virtual IList<string> RegisteredRegions
		{
			get { return Locker.InitWithLock(ref _registeredRegions, () => new List<string>()); }
		}

		/// <summary>
		/// Gets the delayed regions.
		/// </summary>
		/// <value>
		/// The delayed regions.
		/// </value>
		protected IDictionary<string, IList<RegionViewMappingPayload>> DelayedRegions
		{
			get
			{
				return Locker.InitWithLock(ref _delayedRegions,
					() => new Dictionary<string, IList<RegionViewMappingPayload>>());
			}
		}

		/// <summary>
		/// Called when [destroy].
		/// </summary>
		protected override void OnDestroy()
		{
			Unsubscribe<RegionViewMappingPayload>(ProcessPayload);

			if (!_registeredRegions.IsNullOrEmpty())
			{
				_registeredRegions.Clear();
				_registeredRegions = null;
			}

			if (!_delayedRegions.IsNullOrEmpty())
			{
				_delayedRegions.Clear();
				_delayedRegions = null;
			}

			base.OnDestroy();
		}

		protected override void Start()
		{
			base.Start();

			Subscribe<RegionViewMappingPayload>(ProcessPayload, CanProcessPayload);
		}

		/// <summary>
		/// Determines whether this instance [can process payload].
		/// </summary>
		/// <param name="vmp">The payload.</param>
		/// <returns>
		///   <c>true</c> if this instance [can process payload]; otherwise, <c>false</c>.
		/// </returns>
		protected virtual bool CanProcessPayload(RegionViewMappingPayload vmp)
		{
			if (vmp.RegionName.IsNullOrEmpty() || RegisteredRegions.Contains(vmp.RegionName)) return false;

			// if not registered as delayed region yet, ignore on unmap and on destroy actions
			if (!DelayedRegions.ContainsKey(vmp.RegionName))
			{
				switch (vmp.ViewMappingType)
				{
					case RegionViewMappingType.UnmapView:
					case RegionViewMappingType.DestroyRegion:
						return false;
				}
			}

			switch (vmp.ViewMappingType)
			{
				case RegionViewMappingType.MapView:
				case RegionViewMappingType.UnmapView:
				case RegionViewMappingType.DestroyRegion:
					return true;
			}
			return false;
		}


		/// <summary>
		/// Processes the payload.
		/// </summary>
		/// <param name="vmp">The payload.</param>
		protected virtual void ProcessPayload(RegionViewMappingPayload vmp)
		{
			if(RegisteredRegions.Contains(vmp.RegionName)) return;

			IList<RegionViewMappingPayload> payloads;
			if (!DelayedRegions.ContainsKey(vmp.RegionName))
			{
				payloads = new List<RegionViewMappingPayload>();
				DelayedRegions.Add(vmp.RegionName, payloads);
			}
			else
			{
				payloads = DelayedRegions[vmp.RegionName];

				switch (vmp.ViewMappingType)
				{
					case RegionViewMappingType.UnmapView:
						var mapped =
							payloads.Where(
								mapping => mapping.ViewMappingType == RegionViewMappingType.MapView && 
											(mapping.View == vmp.View || mapping.PrefabName == vmp.PrefabName));
						foreach (var mapping in mapped.ToArray())
						{
							payloads.Remove(mapping);
						}
						return;

					case RegionViewMappingType.DestroyRegion:
						DelayedRegions.Remove(vmp.RegionName);
						payloads.Clear();
						return;
				}
			}
			if(payloads.Contains(vmp)) return;

			payloads.Add(vmp);
		}

		/// <summary>
		///     Maps the view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="delayedRegion">if set to <c>true</c> [delayed region].</param>
		public void MapView(GameObject view, string regionName, bool delayedRegion = false)
		{
			//MapViewToRegion(view, regionName, delayedRegion);
		}

		/// <summary>
		///     Unmaps the view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="delayedRegion">if set to <c>true</c> [delayed region].</param>
		public void UnmapView(GameObject view, string regionName, bool delayedRegion = false)
		{
			//UnmapViewFromRegion(view, regionName, delayedRegion);
		}

		/// <summary>
		///     Unmaps all views.
		/// </summary>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="delayedRegion">if set to <c>true</c> [delayed region].</param>
		/// <exception cref="System.OperationCanceledException"></exception>
		public void UnmapAllViews(string regionName, bool delayedRegion = false)
		{
			//CleanupRegions();
			//ArgumentValidator.AssertNotNull(regionName, "regionName");
			//Debug.LogFormat("{0}.UnmapAllViews({1})", typeof(ViewRegionManager), regionName);

			//if (!_regionMappings.ContainsKey(regionName))
			//{
			//	if (!delayedRegion)
			//	{
			//		throw new OperationCanceledException(string.Format("Cannot find delayed region with name \"{0}\".", regionName));
			//	}

			//	var regionViews = _delayedRegionViews[regionName];
			//	if (regionViews.IsNullOrEmpty()) return;
			//	regionViews.ForEach(view => view.Dispose());
			//	regionViews.Clear();
			//	return;
			//}

			//var regionInfo = _regionMappings[regionName];
			//regionInfo.UnmapAllViews();
		}

		/// <summary>
		///     Unmaps the view from region.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="delayedRegion">if set to <c>true</c> [delayed region].</param>
		/// <exception cref="System.OperationCanceledException"></exception>
		public void UnmapViewFromRegion(GameObject view, string regionName, bool delayedRegion = false)
		{
			//CleanupRegions();
			//ArgumentValidator.AssertNotNull(view, "view");
			//ArgumentValidator.AssertNotNull(regionName, "regionName");

			//if (!_regionMappings.ContainsKey(regionName))
			//{
			//	if (!delayedRegion)
			//	{
			//		throw new OperationCanceledException(string.Format("Cannot find delayed region with name \"{0}\".", regionName));
			//	}

			//	RemoveDelayedRegionView(view, regionName);
			//	return;
			//}

			//UnmapViewFromExistingRegion(view, regionName);
		}

		/// <summary>
		///     Maps the view.
		/// </summary>
		/// <param name="viewType">Type of the view.</param>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="delayedRegion">if set to <c>true</c> [delayed region].</param>
		public void MapView(Type viewType, string regionName, bool delayedRegion = false)
		{
			//Func<GameObject> viewCreator = () => IocManager.Default.Resolve<GameObject>(viewType); // TODO
			//MapView(viewCreator, regionName, delayedRegion);
		}

		/// <summary>
		///     Maps the view automatic region.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="delayedRegion">if set to <c>true</c> [delayed region].</param>
		/// <exception cref="System.OperationCanceledException"></exception>
		public void MapViewToRegion(GameObject view, string regionName, bool delayedRegion = false)
		{
			//CleanupRegions();
			//ArgumentValidator.AssertNotNull(view, "view");
			//ArgumentValidator.AssertNotNull(regionName, "regionName");

			//if (!_regionMappings.ContainsKey(regionName))
			//{
			//	if (!delayedRegion)
			//	{
			//		throw new OperationCanceledException(string.Format("Cannot find delayed region with name \"{0}\".", regionName));
			//	}

			//	AddDelayedRegionView(view, regionName);
			//	return;
			//}

			//MapViewToExistingRegion(view, regionName);
		}

		/// <summary>
		///     Removes the delayed region view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="regionName">Name of the region.</param>
		/// <exception cref="System.OperationCanceledException"></exception>
		private void RemoveDelayedRegionView(GameObject view, string regionName)
		{
			//if (!_delayedRegionViews.ContainsKey(regionName))
			//{
			//	throw new OperationCanceledException(string.Format("Cannot find delayed region with name \"{0}\".", regionName));
			//}

			//var regionViews = _delayedRegionViews[regionName];
			//DelegateReference wrapper = null;

			//foreach (var delRef in regionViews)
			//{
			//	if (!delRef.IsAlive || !Equals(delRef.WeakReference.Target, view)) continue;
			//	wrapper = delRef;
			//	break;
			//}

			//regionViews.Remove(wrapper);
			//Debug.LogFormat("{0}.RemoveDelayedRegionView({1}, {2})", typeof(ViewRegionManager), view, regionName);
		}

		/// <summary>
		///     Adds the delayed region view.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="regionName">Name of the region.</param>
		private void AddDelayedRegionView(GameObject view, string regionName)
		{
			//IList<DelegateReference> regionViews;
			//if (!_delayedRegionViews.ContainsKey(regionName))
			//{
			//	regionViews = new List<DelegateReference>();
			//	_delayedRegionViews[regionName] = regionViews;
			//}
			//else
			//{
			//	regionViews = _delayedRegionViews[regionName];
			//}

			////regionViews.Add(new DelegateReference(view)); // TODO
			//Debug.LogFormat("{0}.AddDelayedRegionView({1}, {2})", typeof(ViewRegionManager), view, regionName);
		}

		/// <summary>
		///     Adds the delayed region view.
		/// </summary>
		/// <param name="viewCreator">The view creator.</param>
		/// <param name="regionName">Name of the region.</param>
		private void AddDelayedRegionView(Func<GameObject> viewCreator, string regionName)
		{
			//IList<DelegateReference> regionViews;
			//if (!_delayedRegionViews.ContainsKey(regionName))
			//{
			//	regionViews = new List<DelegateReference>();
			//	_delayedRegionViews[regionName] = regionViews;
			//}
			//else
			//{
			//	regionViews = _delayedRegionViews[regionName];
			//}

			//regionViews.Add(new DelegateReference(viewCreator, true));
			//Debug.LogFormat("{0}.AddDelayedRegionView({1}, {2})", typeof(ViewRegionManager), viewCreator, regionName);
		}

		/// <summary>
		///     Maps the view automatic existing region.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="regionName">Name of the region.</param>
		private void MapViewToExistingRegion(GameObject view, string regionName)
		{
			//var regionInfo = _regionMappings[regionName];
			//regionInfo.MapView(view);

			//Debug.LogFormat("{0}.MapViewToExistingRegion({1}, {2}), ", typeof(ViewRegionManager), view, regionName);
		}

		/// <summary>
		///     Unmaps the view from existing region.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <param name="regionName">Name of the region.</param>
		private void UnmapViewFromExistingRegion(GameObject view, string regionName)
		{
			//var regionInfo = _regionMappings[regionName];
			//regionInfo.UnmapView(view);
		}

		/// <summary>
		///     Maps the view.
		/// </summary>
		/// <param name="viewCreator">The view creator.</param>
		/// <param name="regionName">Name of the region.</param>
		/// <param name="delayedRegion">if set to <c>true</c> [delayed region].</param>
		/// <exception cref="System.OperationCanceledException"></exception>
		public void MapView(Func<GameObject> viewCreator, string regionName, bool delayedRegion = false)
		{
			//CleanupRegions();

			//ArgumentValidator.AssertNotNull(viewCreator, "view");
			//ArgumentValidator.AssertNotNull(regionName, "regionName");

			//if (!_regionMappings.ContainsKey(regionName))
			//{
			//	if (!delayedRegion)
			//	{
			//		throw new OperationCanceledException(string.Format("Cannot find delayed region with name \"{0}\".", regionName));
			//	}

			//	AddDelayedRegionView(viewCreator, regionName);
			//	return;
			//}

			//var view = viewCreator();
			//MapViewToExistingRegion(view, regionName);
		}

		/// <summary>
		///     Registers the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="regionName">Name of the region.</param>
		public void Register(GameObject target, string regionName)
		{
			//RegisterRegion(target, regionName);
		}

		/// <summary>
		///     Unregisters the specified target.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="regionName">Name of the region.</param>
		public void Unregister(GameObject target, string regionName)
		{
			//UnregisterRegion(target, regionName);
		}

		/// <summary>
		///     Gets the name of the region.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		public string GetRegionName(GameObject obj)
		{
			return obj.tag;
		}

		/// <summary>
		///     Sets the name of the region.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="value">The value.</param>
		public void SetRegionName(GameObject obj, string value)
		{
			var oldName = obj.name;
			obj.name = value;
			OnRegionNamePropertyChanged(obj, oldName, value);
		}

		private void OnRegionNamePropertyChanged(GameObject d, string oldName, string newName)
		{
			if (oldName != null)
			{
				UnregisterRegion(d, oldName);
				return;
			}
			if (newName == null) return;

			RegisterRegion(d, newName);
		}

		/// <summary>
		///     Cleanups the regions.
		/// </summary>
		private void CleanupRegions()
		{
			//TODO
			//var res = (from item in _regionMappings
			//		   where !item.Value.IsAlive
			//		   select item).ToArray();
			//foreach (var pair in res)
			//{
			//	pair.Value.Dispose();
			//	_regionMappings.Remove(pair.Key);
			//}

			//foreach (var pair in _delayedRegionViews.ToArray())
			//{
			//	foreach (var item in pair.Value.ToArray())
			//	{
			//		if (item.IsAlive) continue;
			//		pair.Value.Remove(item);
			//	}

			//	if (pair.Value.Count > 0) continue;
			//	_delayedRegionViews.Remove(pair.Key);
			//}
		}

		/// <summary>
		///     Registers the region.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="regionName">Name of the region.</param>
		public void RegisterRegion(GameObject target, string regionName)
		{
			ArgumentValidator.AssertNotNull(target, "target");
			ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");

			target.name = regionName;
			var mapping = target.AddComponent<RegionMapping>();
			mapping.name = regionName;
		}

		/// <summary>
		///     Unregisters the region.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="regionName">Name of the region.</param>
		public void UnregisterRegion(GameObject target, string regionName)
		{
			ArgumentValidator.AssertNotNull(target, "target");
			ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");

			var regionMapping = target.GetComponentInChildren<RegionMapping>();
			if(regionMapping == null) return;

			Destroy(regionMapping);
		}

		public void RegisterRegion(IRegion region)
		{
			ArgumentValidator.AssertNotNull(region, "region");
			ArgumentValidator.AssertNotNullOrEmpty(region.Name, "region.Name");
			if(RegisteredRegions.Contains(region.Name)) return;

			RegisteredRegions.Add(region.Name);
			if(!DelayedRegions.ContainsKey(region.Name)) return;

			var payloads = DelayedRegions[region.Name];
			if(payloads.IsNullOrEmpty()) return;

			var ary = payloads.ToArray();
			payloads.Clear();

			foreach (var payload in ary)
			{
				region.ProcessPayload(payload);
			}
		}

		public void UnregisterRegion(IRegion region)
		{
			ArgumentValidator.AssertNotNull(region, "region");
			ArgumentValidator.AssertNotNullOrEmpty(region.Name, "region.Name");
			if (!RegisteredRegions.Contains(region.Name)) return;

			RegisteredRegions.Remove(region.Name);
		}

		public void ShowView(GameObject view, string regionName, bool delayedRegion = false)
		{
			throw new NotImplementedException();
		}

		public void ShowView(Type viewType, string regionName, bool delayedRegion = false)
		{
			throw new NotImplementedException();
		}

		public void DestroyRegion(IRegion region)
		{
			region.DestroyRegion();
		}

		public void DestroyRegion(string regionName)
		{
			Publish(new RegionViewMappingPayload(regionName, RegionViewMappingType.DestroyRegion));
		}
	}
}