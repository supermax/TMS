namespace TMS.Common.Core
{
	/// <summary>
	///     Singleton Base Class
	/// </summary>
	/// <typeparam name="T">Type of Singleton</typeparam>
	public class ObservableSingleton<T> : Observable
		where T : class, IObservable, new()
	{
		protected static volatile T _default;
		protected static readonly object Locker = new object();

		/// <summary>
		///     Gets the default instance.
		/// </summary>
		public static T Default
		{
			get
			{
				if (_default == null)
				{
					lock (Locker)
					{
						if (_default == null)
						{
							_default = new T();
						}
					}
				}
				return _default;
			}
		}
	}
}