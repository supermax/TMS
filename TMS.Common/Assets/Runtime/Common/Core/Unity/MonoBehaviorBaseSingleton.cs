using TMS.Common.Modularity;
using UnityEngine;

namespace TMS.Common.Core
{
	/// <summary>
	/// Mono Behavior Singleton
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <seealso cref="TMS.Common.Core.MonoBehaviourBase" />
	public abstract class MonoBehaviorBaseSingleton<T> : MonoBehaviourBase
		where T : MonoBehaviour
	{
		protected static volatile T _default;
		protected static bool _isInitializing;
		protected static readonly Tuple<T> Locker = new Tuple<T>();

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
							IocManager.Default.Register<T>(typeof(T), true);
							_default = IocManager.Default.Resolve<T>();
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
			IocManager.Default.Unregister(this);
			base.OnDestroy();
		}
	}
}