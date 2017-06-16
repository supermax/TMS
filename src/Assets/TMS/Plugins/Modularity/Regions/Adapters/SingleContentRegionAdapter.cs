using System.Collections.Generic;
using UnityEngine;

namespace TMS.Common.Modularity.Regions.Adapters
{
	public class SingleContentRegionAdapter : BaseRegionAdapter
	{
		#region Implementation of IRegionAdapter

		public override ViewMapping MapView(RegionMapping target, GameObject view)
		{
			var prevElement = target.GetComponentInChildren<ViewMapping>(true);
			if (prevElement != null)
			{
				UnmapView(target, prevElement.gameObject); 
			}

			var viewMapping = MapViewInternal(target, view);
			return viewMapping;
		}

		public override ViewMapping MapView(RegionMapping target, string prefabName)
		{
			var go = Resources.Load<GameObject>(prefabName); // TODO add support for asset bundles
			var vm = MapView(target, go);
			return vm;
		}

		public override ViewMapping UnmapView(RegionMapping target, GameObject view)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = UnmapViewInternal(target, view, viewMapping);
			return viewMapping;
		}

		public override ViewMapping HideView(RegionMapping target, GameObject view)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = HideViewInternal(target, view, viewMapping);
			return viewMapping;
		}

		public override ViewMapping UnmapView(RegionMapping target, string prefabName)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if(viewMapping == null) return null; 

			var vm = UnmapView(target, viewMapping.gameObject);
			return vm;
		}

		public override IEnumerable<ViewMapping> UnmapAllViews(RegionMapping target)
		{
			var viewMapping = target.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null;

			UnmapView(target, viewMapping.gameObject);
			return new[] { viewMapping };
		}

		public override void LockTarget(RegionMapping target)
		{
			LockTargetInternal(target);
		}

		public override void UnlockTarget(RegionMapping target)
		{
			UnlockTargetInternal(target);
		}

		public override ViewMapping ShowView(RegionMapping target, GameObject view)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = ShowViewInternal(target, view, viewMapping);
			return viewMapping;
		}

		public override ViewMapping LockView(RegionMapping target, GameObject view)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = LockViewInternal(target, view, viewMapping);
			return viewMapping;
		}

		public override ViewMapping ShowView(RegionMapping target, string prefabName)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = ShowViewInternal(target, viewMapping.gameObject, viewMapping);
			return viewMapping;
		}

		public override ViewMapping LockView(RegionMapping target, string prefabName)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = LockViewInternal(target, viewMapping.gameObject, viewMapping);
			return viewMapping;
		}

		public override ViewMapping UnlockView(RegionMapping target, GameObject view)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = UnlockViewInternal(target, view, viewMapping);
			return viewMapping;
		}

		public override ViewMapping UnlockView(RegionMapping target, string prefabName)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = UnlockViewInternal(target, viewMapping.gameObject, viewMapping);
			return viewMapping;
		}

		public override ViewMapping HideView(RegionMapping target, string prefabName)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = HideViewInternal(target, viewMapping.gameObject, viewMapping);
			return viewMapping;
		}

		public override IEnumerable<ViewMapping> HideAllViews(RegionMapping target)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = HideViewInternal(target, viewMapping.gameObject, viewMapping);
			return new []{ viewMapping };
		}

		public override IEnumerable<ViewMapping> ShowAllViews(RegionMapping target)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = ShowViewInternal(target, viewMapping.gameObject, viewMapping);
			return new[] { viewMapping };
		}

		public override void UpdateDataContext(RegionMapping target, object dataContext)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return; 

			UpdateDataContextInternal(target, viewMapping.gameObject, viewMapping, dataContext);
		}

		public override ViewMapping UpdateDataContext(RegionMapping target, GameObject view, object dataContext)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = UpdateDataContextInternal(target, view, viewMapping, dataContext);
			return viewMapping;
		}

		public override ViewMapping UpdateDataContext(RegionMapping target, string prefabName, object dataContext)
		{
			var viewMapping = target.gameObject.GetComponentInChildren<ViewMapping>(true);
			if (viewMapping == null) return null; 

			viewMapping = UpdateDataContextInternal(target, viewMapping.gameObject, viewMapping, dataContext);
			return viewMapping;
		}

		#endregion
	}
}