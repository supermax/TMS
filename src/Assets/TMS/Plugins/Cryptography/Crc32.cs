using System;
using System.Collections.Generic;

namespace TMS.Common.Cryptography
{
	/// <summary>
	/// Implements a 32-bit CRC hash algorithm compatible with Zip etc.
	/// </summary>
	/// <remarks>
	/// Crc32 should only be used for backward compatibility with older file formats
	/// and algorithms. It is not secure enough for new applications.
	/// If you need to call multiple times for the same data either use the HashAlgorithm
	/// interface or remember that the result of one Compute call needs to be ~ (XOR) before
	/// being passed in as the seed for the next Compute call.
	/// </remarks>
	public sealed class Crc32 
	{
		public const UInt32 DefaultPolynomial = 0x04c11db7;
		public static UInt32 DefaultSeed = 0xffffffff;

		private static UInt32[] _defaultTable;

		private readonly UInt32 _seed;
		private readonly UInt32[] _table;
		private UInt32 _hash;
		 
		public void ChangeSeed(UInt32 seed)
		{
			DefaultSeed = seed;
		}

		public Crc32()
			: this(DefaultPolynomial, DefaultSeed)
		{
		}

		public Crc32(UInt32 polynomial, UInt32 seed)
		{
			_table = InitializeTable(polynomial);
			_seed = _hash = seed;
		}

		public void Initialize()
		{
			_hash = _seed;
		}

		public byte[] ComputeHash(IList<byte> buffer, int start, int length)
		{
			_hash = CalculateHash(_table, _hash, buffer, start, length);
			return ComputeHash();
		}

		private byte[] ComputeHash()
		{
			var hashBuffer = UInt32ToBigEndianBytes(~_hash);
			HashValue = hashBuffer;
			return hashBuffer;
		}

		public byte[] HashValue { get; set; }

		public int HashSize { get { return 32; } }

		public static UInt32 Compute(byte[] buffer)
		{
			return Compute(DefaultSeed, buffer);
		}

		public static UInt32 Compute(UInt32 seed, byte[] buffer)
		{
			return Compute(DefaultPolynomial, seed, buffer);
		}

		public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
		{
			return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
		}

		private static UInt32[] InitializeTable(UInt32 polynomial)
		{
			if (polynomial == DefaultPolynomial && _defaultTable != null)
				return _defaultTable;

			var createTable = new UInt32[256];
			for (var i = 0; i < 256; i++)
			{
				var entry = (UInt32)i;
				for (var j = 0; j < 8; j++)
					if ((entry & 1) == 1)
						entry = (entry >> 1) ^ polynomial;
					else
						entry = entry >> 1;
				createTable[i] = entry;
			}

			if (polynomial == DefaultPolynomial)
				_defaultTable = createTable;

			return createTable;
		}

		private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size)
		{
			var crc = seed;
			for (var i = start; i < size - start; i++)
				crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
			return crc;
		}

		private static byte[] UInt32ToBigEndianBytes(UInt32 uint32)
		{
			var result = BitConverter.GetBytes(uint32);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(result);

			return result;
		}
	}
}