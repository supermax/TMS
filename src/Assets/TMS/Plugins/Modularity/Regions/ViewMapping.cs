using System;
using TMS.Common.Core;
using UnityEngine;

namespace TMS.Common.Modularity.Regions
{
	[Serializable]
	public class ViewMapping : ViewModel, IViewMapping
	{
		[SerializeField] private object _dataContext;

		public object DataContext
		{
			get { return _dataContext; }
			set { SetValue(ref _dataContext, value, "DataContext"); }
		}

		public virtual void OnViewMapped(RegionMapping regionHost, GameObject view)
		{
			// TODO
		}

		public virtual void OnViewUnmapped(RegionMapping regionHost, GameObject view)
		{
			// TODO
		}

		public virtual void OnViewHidden(RegionMapping regionHost, GameObject view)
		{
			// TODO
		}

		public virtual void OnViewShown(RegionMapping regionHost, GameObject view)
		{
			// TODO
		}

		public virtual void OnDataContextChange(RegionMapping regionHost, GameObject view, object oldValue, object newValue)
		{
			// TODO
		}

		public void OnViewLocked(RegionMapping target, GameObject view)
		{
			// TODO
		}

		public void OnViewUnlocked(RegionMapping target, GameObject view)
		{
			// TODO
		}
	}
}