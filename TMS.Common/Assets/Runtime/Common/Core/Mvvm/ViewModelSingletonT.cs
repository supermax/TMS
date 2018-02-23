namespace TMS.Common.Core
{
	/// <summary>
	/// View Model Singleton
	/// </summary>
	/// <typeparam name="TInterface">The type of the interface.</typeparam>
	/// <typeparam name="TImplementation">The type of the implementation.</typeparam>
	public class ViewModelSingleton<TInterface, TImplementation> : ViewModel
		where TInterface : IViewModel
		where TImplementation : ViewModel, TInterface
	{
		protected static TInterface _default;
		protected static bool _isInitializing;
		protected new static readonly Tuple<TInterface, TImplementation> Locker = new Tuple<TInterface, TImplementation>();

		/// <summary>
		/// Gets the default instance
		/// </summary>
		/// <value>
		/// The default.
		/// </value>
		public static TInterface Default
		{
			get { return InitStaticDefaultInstance(); }
		}

		private static TInterface InitStaticDefaultInstance()
		{
			if (_default == null && !_isInitializing)
			{
				lock (Locker)
				{
					if (_default == null && !_isInitializing)
					{
						try
						{
							_isInitializing = true;
							Modularity.IocManager.Default.Register<TInterface>(typeof(TImplementation), true);
							_default = Modularity.IocManager.Default.Resolve<TInterface>();
						}
						finally
						{
							_isInitializing = false;
						}
					}
				}
			}
			return _default;
		}

		protected override void Awake()
		{
			InitStaticDefaultInstance();
			base.Awake();
		}

		protected override void OnDestroy()
		{
			_default = default(TInterface);
			Modularity.IocManager.Default.Unregister(this);
			base.OnDestroy();
		}
	}
}