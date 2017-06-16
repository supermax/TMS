using System.Collections.Generic;
using TMS.Common.Extensions;
using UnityEngine;

namespace TMS.Common.Modularity.Regions.Adapters
{
	public class MultiContentRegionAdapter : BaseRegionAdapter
	{
		#region Implementation of IRegionAdapter

		public override ViewMapping MapView(RegionMapping target, GameObject view)
		{
			var viewMapping = MapViewInternal(target, view);
			return viewMapping;
		}

		public override ViewMapping MapView(RegionMapping target, string prefabName)
		{
			var go = Resources.Load<GameObject>(prefabName);
			var vm = MapView(target, go);
			return vm;
		}

		public override ViewMapping UnmapView(RegionMapping target, GameObject view)
		{
			var viewMappings = target.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if(viewMapping.gameObject != view) continue;
				var viewMappingResult = UnmapViewInternal(target, view, viewMapping);
				return viewMappingResult;
			}
			return null;
		}

		public override ViewMapping UnmapView(RegionMapping target, string prefabName)
		{
			var viewMappings = target.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject.name != prefabName) continue;
				var viewMappingResult = UnmapViewInternal(target, viewMapping.gameObject, viewMapping);
				return viewMappingResult;
			}
			return null;
		}

		public override IEnumerable<ViewMapping> UnmapAllViews(RegionMapping target)
		{
			var viewMappings = target.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				UnmapViewInternal(target, viewMapping.gameObject, viewMapping);
			}
			return viewMappings;
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
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject != view) continue;
				ShowViewInternal(target, view, viewMapping);
				return viewMapping;
			}
			return null;
		}

		public override ViewMapping LockView(RegionMapping target, GameObject view)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject != view) continue;
				LockViewInternal(target, view, viewMapping);
				return viewMapping;
			}
			return null;
		}

		public override ViewMapping ShowView(RegionMapping target, string prefabName)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject.name != prefabName) continue;
				ShowViewInternal(target, viewMapping.gameObject, viewMapping);
				return viewMapping;
			}
			return null;
		}

		public override ViewMapping LockView(RegionMapping target, string prefabName)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject.name != prefabName) continue;
				LockViewInternal(target, viewMapping.gameObject, viewMapping);
				return viewMapping;
			}
			return null;
		}

		public override ViewMapping UnlockView(RegionMapping target, GameObject view)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject != view) continue;
				UnlockViewInternal(target, view, viewMapping);
				return viewMapping;
			}
			return null;
		}

		public override ViewMapping HideView(RegionMapping target, GameObject view)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings.IsNullOrEmpty()) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject != view) continue;
				HideViewInternal(target, view, viewMapping);
				return viewMapping;
			}
			return null;
		}

		public override ViewMapping UnlockView(RegionMapping target, string prefabName)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject.name != prefabName) continue;
				UnlockViewInternal(target, viewMapping.gameObject, viewMapping);
				return viewMapping;
			}
			return null;
		}

		public override ViewMapping HideView(RegionMapping target, string prefabName)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings.IsNullOrEmpty()) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject.name != prefabName) continue;
				HideViewInternal(target, viewMapping.gameObject, viewMapping);
				return viewMapping;
			}
			return null;
		}

		public override IEnumerable<ViewMapping> HideAllViews(RegionMapping target)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings.IsNullOrEmpty()) return null;

			foreach (var viewMapping in viewMappings)
			{
				HideViewInternal(target, viewMapping.gameObject, viewMapping);
			}
			return viewMappings;
		}

		public override IEnumerable<ViewMapping> ShowAllViews(RegionMapping target)
		{
			var viewMappings = target.gameObject.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings.IsNullOrEmpty()) return null;

			foreach (var viewMapping in viewMappings)
			{
				ShowViewInternal(target, viewMapping.gameObject, viewMapping);
			}
			return viewMappings;
		}

		public override void UpdateDataContext(RegionMapping target, object dataContext)
		{
			var viewMappings = target.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return;

			foreach (var viewMapping in viewMappings)
			{
				viewMapping.OnDataContextChange(target, viewMapping.gameObject, viewMapping.DataContext, dataContext);
				viewMapping.DataContext = dataContext;
			}
		}

		public override ViewMapping UpdateDataContext(RegionMapping target, GameObject view, object dataContext)
		{
			var viewMappings = target.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject != view) continue;
				viewMapping.OnDataContextChange(target, view, viewMapping.DataContext, dataContext);
				viewMapping.DataContext = dataContext;
				return viewMapping;
			}
			return null;
		}

		public override ViewMapping UpdateDataContext(RegionMapping target, string prefabName, object dataContext)
		{
			var viewMappings = target.GetComponentsInChildren<ViewMapping>(true);
			if (viewMappings == null) return null;

			foreach (var viewMapping in viewMappings)
			{
				if (viewMapping.gameObject.name != prefabName) continue;
				viewMapping.OnDataContextChange(target, viewMapping.gameObject, viewMapping.DataContext, dataContext);
				viewMapping.DataContext = dataContext;
				return viewMapping;
			}
			return null;
		}

		#endregion
	}
}