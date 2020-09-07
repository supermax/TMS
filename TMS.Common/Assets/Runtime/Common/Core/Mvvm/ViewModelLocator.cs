#region Usings

using TMS.Common.Modularity;
using TMS.Common.Modularity.Ioc;

#endregion

namespace TMS.Common.Core
{
	public class ViewModelLocator<T>
		where T : IViewModel
	{
		public virtual T MainViewModel
		{
			get { return IocManager.Default.Resolve<T>(); }
		}
	}
}