namespace TMS.Common.Core
{
	/// <summary>
	///     View Model Singleton
	/// </summary>
	/// <typeparam name="T">ViewModelBase</typeparam>
	public class ViewModelSingleton<T> : ViewModel
		where T : ViewModel, new()
	{
		protected static volatile T _default;
		protected static bool _isInitializing;
		protected new static readonly Tuple<T> Locker = new Tuple<T>();

		/// <summary>
		/// Gets the default instance.
		/// </summary>
		/// <value>
		/// The default.
		/// </value>
		public static T Default
		{
			get { return InitStaticDefaultInstance(); }
		}

		private static T InitStaticDefaultInstance()
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
							Modularity.IocManager.Default.Register<T>(typeof(T), true);
							_default = Modularity.IocManager.Default.Resolve<T>();
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
			_default = null;
			Modularity.IocManager.Default.Unregister(this);
			base.OnDestroy();
		}
	}
}