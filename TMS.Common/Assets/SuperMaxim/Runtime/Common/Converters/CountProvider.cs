namespace TMS.Common.Converters
{
	/// <summary>
	/// 
	/// </summary>
	public class CountProvider
	{
		private int _count;

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int Count
		{
			get { return ++_count; }
		}

		/// <summary>
		/// Gets the items.
		/// </summary>
		/// <value>
		/// The items.
		/// </value>
		public int[] Items
		{
			get
			{
				return new int[Count];
			}
		}
	}
}