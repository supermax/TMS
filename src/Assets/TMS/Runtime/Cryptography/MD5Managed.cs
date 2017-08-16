#if SILVERLIGHT || UNITY_WP8

#region

using System;
using System.Security.Cryptography;

#endregion

namespace TMS.Common.Cryptography
{
	/// <summary>
	///     Raw implementation of the MD5 hash algorithm from RFC 1321.
	/// </summary>
	internal class MD5Managed : HashAlgorithm
	{
		private ABCDStruct _abcd;
		private byte[] _data;
		private int _dataSize;
		private Int64 _totalLength;

		public MD5Managed()
		{
			HashSizeValue = 0x80;
			Initialize();
		}

		/// <summary>
		///     Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> class.
		/// </summary>
		public override void Initialize()
		{
			_data = new byte[64];
			_dataSize = 0;
			_totalLength = 0;
			//Initial values as defined in RFC 1321
			_abcd = new ABCDStruct
			{
				A = 0x67452301,
				B = 0xefcdab89,
				C = 0x98badcfe,
				D = 0x10325476
			};
		}

		protected override void HashCore(byte[] array, int ibStart, int cbSize)
		{
			var startIndex = ibStart;
			var totalArrayLength = _dataSize + cbSize;
			if (totalArrayLength >= 64)
			{
				Array.Copy(array, startIndex, _data, _dataSize, 64 - _dataSize);
				// Process message of 64 bytes (512 bits)
				MD5Core.GetHashBlock(_data, ref _abcd, 0);
				startIndex += 64 - _dataSize;
				totalArrayLength -= 64;
				while (totalArrayLength >= 64)
				{
					Array.Copy(array, startIndex, _data, 0, 64);
					MD5Core.GetHashBlock(array, ref _abcd, startIndex);
					totalArrayLength -= 64;
					startIndex += 64;
				}
				_dataSize = totalArrayLength;
				Array.Copy(array, startIndex, _data, 0, totalArrayLength);
			}
			else
			{
				Array.Copy(array, startIndex, _data, _dataSize, cbSize);
				_dataSize = totalArrayLength;
			}
			_totalLength += cbSize;
		}

		protected override byte[] HashFinal()
		{
			HashValue = MD5Core.GetHashFinalBlock(_data, 0, _dataSize, _abcd, _totalLength*8);
			return HashValue;
		}
	}
}

#endif