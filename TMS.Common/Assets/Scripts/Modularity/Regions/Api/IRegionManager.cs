using System;
using System.Collections.Generic;
using TMS.Common.Core;
using UnityEngine;

namespace TMS.Common.Modularity.Regions
{
	public interface IRegionManager : IViewModel
	{
		void MapView(Func<GameObject> viewCreator, string regionName, bool delayedRegion = false);

		void MapView(GameObject view, string regionName, bool delayedRegion = false);

		void ShowView(GameObject view, string regionName, bool delayedRegion = false);

		void UnmapView(GameObject view, string regionName, bool delayedRegion = false);

		void UnmapAllViews(string regionName, bool delayedRegion = false);

		void MapView(Type viewType, string regionName, bool delayedRegion = false);

		void ShowView(Type viewType, string regionName, bool delayedRegion = false);

		void RegisterRegion(GameObject target, string regionName);

		void RegisterRegion(IRegion region);

		void UnregisterRegion(GameObject target, string regionName);

		void UnregisterRegion(IRegion region);

		void DestroyRegion(IRegion region);

		void DestroyRegion(string regionName);
	}
}