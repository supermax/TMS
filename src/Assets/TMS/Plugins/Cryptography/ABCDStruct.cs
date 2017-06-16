#region Code Editor

// Maxim

#endregion

namespace TMS.Common.Cryptography
{
	/// <summary>
	///     Simple struct for the (a,b,c,d) which is used to compute the message digest.
	/// </summary>
	internal struct ABCDStruct
	{
		public uint A;
		public uint B;
		public uint C;
		public uint D;
	}
}