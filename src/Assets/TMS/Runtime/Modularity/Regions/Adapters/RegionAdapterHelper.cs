using System;
using System.Linq;
using TMS.Common.Core;
using TMS.Common.Extensions;
using UnityEngine;

namespace TMS.Common.Modularity.Regions.Adapters
{
	internal static class RegionAdapterHelper
	{
		public static Type GetViewType(Type srcType)
		{
			var attrib = srcType.GetCustomAttributes(typeof(ViewMappingAttribute), true).FirstOrDefault() as ViewMappingAttribute;
			return attrib != null ? attrib.ViewType : null;
		}

		public static RegionViewWrapper CreateView(Type viewType, object dataContext)
		{
			var element = IocManager.Default.Resolve<GameObject>(viewType);
			var view = new RegionViewWrapper(element, dataContext);
			if (view.Id.IsNullOrEmpty())
			{
				view.Id = CreateViewId(null, dataContext);
			}
			return view;
		}

		public static RegionViewWrapper CreateView(object view)
		{
			var viewType = GetViewType(view.GetType());
			var element = CreateView(viewType, view);
			return element;
		}

		public static string CreateViewId(this GameObject view, object dataContext)
		{
			return string.Format("{0}_{1}", 
				view != null ? view.GetType().Name : "NULL", 
				dataContext != null ? dataContext.GetHashCode() : 0);
		}
	}
}