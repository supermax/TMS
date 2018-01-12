using System.Collections.Generic;
using UnityEngine;

namespace TMS.Common.Modularity.Regions
{
	public interface IRegionAdapter
	{
		ViewMapping MapView(RegionMapping target, GameObject view);

		ViewMapping ShowView(RegionMapping target, GameObject view);

		ViewMapping MapView(RegionMapping target, string prefabName);

		ViewMapping ShowView(RegionMapping target, string prefabName);

		ViewMapping UnmapView(RegionMapping target, GameObject view);

		ViewMapping HideView(RegionMapping target, GameObject view);

		ViewMapping UnmapView(RegionMapping target, string prefabName);

		ViewMapping HideView(RegionMapping target, string prefabName);

		IEnumerable<ViewMapping> UnmapAllViews(RegionMapping target);

		IEnumerable<ViewMapping> HideAllViews(RegionMapping target);

		IEnumerable<ViewMapping> ShowAllViews(RegionMapping target);

		void LockTarget(RegionMapping target);

		ViewMapping LockView(RegionMapping target, GameObject view);

		ViewMapping UnlockView(RegionMapping target, GameObject view);

		ViewMapping LockView(RegionMapping target, string prefabName);

		ViewMapping UnlockView(RegionMapping target, string prefabName);

		void UnlockTarget(RegionMapping target);
	}
}