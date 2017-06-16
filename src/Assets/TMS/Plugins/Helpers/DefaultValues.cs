#region

using TMS.Common.Core;

#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	///     Default Values (for XAML purposes most)
	/// </summary>
	public class DefaultValues : Singleton<DefaultValues>
	{
		/// <summary>
		///     Gets a TRUE value.
		/// </summary>
		public bool BoolTrue
		{
			get { return true; }
		}

		/// <summary>
		///     Gets a FALSE value.
		/// </summary>
		public bool BoolFalse
		{
			get { return false; }
		}

		/// <summary>
		///     Gets the max value of int.
		/// </summary>
		public int IntMax
		{
			get { return int.MaxValue; }
		}

		/// <summary>
		///     Gets the min value of int.
		/// </summary>
		public int IntMin
		{
			get { return int.MinValue; }
		}

		/// <summary>
		///     Gets the default value of int.
		/// </summary>
		public int IntDefault
		{
			get { return default(int); }
		}

		/// <summary>
		///     Gets the max value of double.
		/// </summary>
		public double DoubleMax
		{
			get { return double.MaxValue; }
		}

		/// <summary>
		///     Gets the min value of double.
		/// </summary>
		public double DoubleMin
		{
			get { return double.MinValue; }
		}

		/// <summary>
		///     Gets the default value of double.
		/// </summary>
		public double DoubleDefault
		{
			get { return default(double); }
		}

		/// <summary>
		///     Gets the NaN value of double.
		/// </summary>
		public double DoubleNaN
		{
			get { return double.NaN; }
		}
	}
}