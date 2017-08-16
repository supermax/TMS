using TMS.Common.Modularity;

namespace TMS.Common.Core
{
	/// <summary>
	///     Singleton Base Class
	/// </summary>
	/// <typeparam name="TInterface">The type of the interface.</typeparam>
	/// <typeparam name="TImplementation">The type of the implementation.</typeparam>
	public class Singleton<TInterface, TImplementation> 
		where TImplementation : TInterface, new()
	{
		protected static TInterface _default;
		protected static readonly Tuple<TInterface, TImplementation> Locker = new Tuple<TInterface, TImplementation>();

		/// <summary>
		///     Gets the default instance.
		/// </summary>
		public static TInterface Default
		{
			get
			{
				if (Equals(_default, default(TImplementation)))
				{
					lock (Locker)
					{
						if (Equals(_default, default(TImplementation)))
						{
							IocManager.Default.Register<TInterface>(typeof(TImplementation), true);
							_default = IocManager.Default.Resolve<TInterface>(typeof(TImplementation));
						}
					}
				}
				return _default;
			}
		}
	}

	/// <summary>
	///     Singleton Base Class
	/// </summary>
	/// <typeparam name="T">Type of Singleton</typeparam>
	public class Singleton<T> : Singleton<T, T>
		where T : class, new()
	{
		
	}
}