#region

using TMS.Common.Core;

#endregion


namespace TMS.Common.Helpers
{
	public class NullableProvider : Observable
	{
		public NullableProvider()
		{
			
		}

		private object _nullableValue;

		public object NullableValue
		{
			get { return _nullableValue; }
			set { SetValue(ref _nullableValue, value); }
		}
	}
}