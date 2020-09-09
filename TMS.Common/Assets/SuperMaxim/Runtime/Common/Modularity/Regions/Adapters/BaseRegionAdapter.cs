using System.Collections.Generic;
using TMS.Common.Core;
using TMS.Common.Extensions;
using UnityEngine;

namespace TMS.Common.Modularity.Regions.Adapters
{
	public abstract class BaseRegionAdapter : ViewModel, IRegionAdapter
	{
		public abstract ViewMapping MapView(RegionMapping target, GameObject view);

		public abstract ViewMapping ShowView(RegionMapping target, GameObject view);

		public abstract ViewMapping LockView(RegionMapping target, GameObject view);

		public abstract ViewMapping MapView(RegionMapping target, string prefabName);

		public abstract ViewMapping ShowView(RegionMapping target, string prefabName);

		public abstract ViewMapping LockView(RegionMapping target, string prefabName);

		public abstract ViewMapping UnmapView(RegionMapping target, GameObject view);

		public abstract ViewMapping UnlockView(RegionMapping target, GameObject view);

		public abstract ViewMapping HideView(RegionMapping target, GameObject view);

		public abstract ViewMapping UnmapView(RegionMapping target, string prefabName);

		public abstract ViewMapping UnlockView(RegionMapping target, string prefabName);

		public abstract ViewMapping HideView(RegionMapping target, string prefabName);

		public abstract IEnumerable<ViewMapping> UnmapAllViews(RegionMapping target);

		public abstract IEnumerable<ViewMapping> HideAllViews(RegionMapping target);

		public abstract IEnumerable<ViewMapping> ShowAllViews(RegionMapping target);

		public abstract void LockTarget(RegionMapping target);

		public abstract void UnlockTarget(RegionMapping target);

		public abstract void UpdateDataContext(RegionMapping target, object dataContext);

		public abstract ViewMapping UpdateDataContext(RegionMapping target, GameObject view, object dataContext);

		public abstract ViewMapping UpdateDataContext(RegionMapping target, string prefabName, object dataContext);

		[SerializeField]
		protected List<DelegateCommand> _mapViewUpdateCommands = new List<DelegateCommand>();

		[SerializeField]
		protected List<DelegateCommand> _unmapViewUpdateCommands = new List<DelegateCommand>();

		[SerializeField]
		protected List<DelegateCommand> _showViewUpdateCommands = new List<DelegateCommand>();

		[SerializeField]
		protected List<DelegateCommand> _hideViewUpdateCommands = new List<DelegateCommand>();

		[SerializeField] protected bool _disposeCommands;

		public virtual bool DisposeCommands { get { return _disposeCommands; } set { _disposeCommands = value; } }

		public virtual List<DelegateCommand> MapViewUpdateCommands { get { return _mapViewUpdateCommands; } }

		public virtual List<DelegateCommand> UnmapViewUpdateCommands { get { return _unmapViewUpdateCommands; } }

		public virtual List<DelegateCommand> ShowViewUpdateCommands { get { return _showViewUpdateCommands; } }

		public virtual List<DelegateCommand> HideViewUpdateCommands { get { return _hideViewUpdateCommands; } }

		protected virtual void Dispose(IList<DelegateCommand> commands)
		{
			if(commands.IsNullOrEmpty()) return;
			commands.ForEach(cmd => cmd.Dispose());
			commands.Clear();
		}

		protected override bool Dispose(bool disposing)
		{
			if (disposing && DisposeCommands)
			{
				Dispose(MapViewUpdateCommands);
				Dispose(UnmapViewUpdateCommands);
				Dispose(ShowViewUpdateCommands);
				Dispose(HideViewUpdateCommands);
			}
			return base.Dispose(disposing);
		}

		protected virtual void ExecuteUnmapViewUpdateCommands()
		{
			if (UnmapViewUpdateCommands.IsNullOrEmpty()) return;
			UnmapViewUpdateCommands.ForEach(del => del.Execute(del.CommandParameter));
		}

		protected virtual void ExecuteHideViewUpdateCommands()
		{
			if (HideViewUpdateCommands.IsNullOrEmpty()) return;
			HideViewUpdateCommands.ForEach(del => del.Execute(del.CommandParameter));
		}

		protected virtual void ExecuteShowViewUpdateCommands()
		{
			if (ShowViewUpdateCommands.IsNullOrEmpty()) return;
			ShowViewUpdateCommands.ForEach(del => del.Execute(del.CommandParameter));
		}

		protected virtual void ExecuteMapViewUpdateCommands()
		{
			if(MapViewUpdateCommands.IsNullOrEmpty()) return;
			MapViewUpdateCommands.ForEach(del => del.Execute(del.CommandParameter));
		}

		protected virtual ViewMapping MapViewInternal(RegionMapping target, GameObject view)
		{
			var element = target.gameObject.AddChild(view, false);
			var viewMapping = element.AddComponent<ViewMapping>();
			viewMapping.OnViewMapped(target, view);
			ExecuteMapViewUpdateCommands();
			return viewMapping;
		}

		protected virtual ViewMapping UnmapViewInternal(RegionMapping target, GameObject view, ViewMapping viewMapping)
		{
			view.SetActive(false);
			Destroy(view);
			viewMapping.OnViewUnmapped(target, view);
			ExecuteUnmapViewUpdateCommands();
			return viewMapping;
		}

		protected virtual ViewMapping HideViewInternal(RegionMapping target, GameObject view, ViewMapping viewMapping)
		{
			view.SetActive(false);
			viewMapping.OnViewHidden(target, view);
			ExecuteHideViewUpdateCommands();
			return viewMapping;
		}

		protected virtual ViewMapping ShowViewInternal(RegionMapping target, GameObject view, ViewMapping viewMapping)
		{
			view.SetActive(true);
			viewMapping.OnViewShown(target, view);
			ExecuteShowViewUpdateCommands();
			return viewMapping;
		}

		protected virtual ViewMapping LockViewInternal(RegionMapping target, GameObject view, ViewMapping viewMapping)
		{
			view.SetActive(false); // TODO find a way to lock view and not to hide
			viewMapping.OnViewLocked(target, view);
			return viewMapping;
		}

		protected virtual ViewMapping UnlockViewInternal(RegionMapping target, GameObject view, ViewMapping viewMapping)
		{
			view.SetActive(false); // TODO find a way to lock view and not to hide
			viewMapping.OnViewUnlocked(target, view);
			return viewMapping;
		}

		protected virtual ViewMapping UpdateDataContextInternal(RegionMapping target, GameObject view, ViewMapping viewMapping, object dataContext)
		{
			viewMapping.OnDataContextChange(target, view, viewMapping.DataContext, dataContext);
			viewMapping.DataContext = dataContext;
			return viewMapping;
		}

		protected virtual void LockTargetInternal(RegionMapping target)
		{
			target.gameObject.SetActive(false);
		}

		protected virtual void UnlockTargetInternal(RegionMapping target)
		{
			target.gameObject.SetActive(true);
		}
	}
}