using TMS.Common.Core;
using UnityEngine;

namespace TMS.Common.Modularity.Regions.Adapters
{
	internal class RegionViewWrapper : ViewModel
	{
		public object DataContext
		{
			get { return GetValue<object>(); }
			set { SetValue(value); }
		}

		public RegionViewWrapper(GameObject view, object dataContext)
		{
			DataContext = dataContext;
			//view.SetBinding(DataContextProperty, new Binding { Path = new PropertyPath("ViewModel"), Source = this });
			//SetValue(ContentProperty, view);
		}
	}
}