#region Usings

using System;
using System.Diagnostics;

#endregion

namespace Unity.IO.Compression
{
	internal class OutputBuffer
	{
		private uint bitBuf; // store uncomplete bits 
		private int bitCount; // number of bits in bitBuffer 

		private byte[] byteBuffer; // buffer for storing bytes

		internal int BytesWritten { get; private set; }

		internal int FreeBytes
		{
			get { return byteBuffer.Length - BytesWritten; }
		}

		internal int BitsInBuffer
		{
			get { return bitCount / 8 + 1; }
		}

		// set the output buffer we will be using
		internal void UpdateBuffer(byte[] output)
		{
			byteBuffer = output;
			BytesWritten = 0;
		}

		internal void WriteUInt16(ushort value)
		{
			Debug.Assert(FreeBytes >= 2, "No enough space in output buffer!");

			byteBuffer[BytesWritten++] = (byte) value;
			byteBuffer[BytesWritten++] = (byte) (value >> 8);
		}

		internal void WriteBits(int n, uint bits)
		{
			Debug.Assert(n <= 16, "length must be larger than 16!");
			bitBuf |= bits << bitCount;
			bitCount += n;
			if (bitCount >= 16)
			{
				Debug.Assert(byteBuffer.Length - BytesWritten >= 2, "No enough space in output buffer!");
				byteBuffer[BytesWritten++] = unchecked((byte) bitBuf);
				byteBuffer[BytesWritten++] = unchecked((byte) (bitBuf >> 8));
				bitCount -= 16;
				bitBuf >>= 16;
			}
		}

		// write the bits left in the output as bytes. 
		internal void FlushBits()
		{
			// flush bits from bit buffer to output buffer
			while (bitCount >= 8)
			{
				byteBuffer[BytesWritten++] = unchecked((byte) bitBuf);
				bitCount -= 8;
				bitBuf >>= 8;
			}

			if (bitCount > 0)
			{
				byteBuffer[BytesWritten++] = unchecked((byte) bitBuf);
				bitBuf = 0;
				bitCount = 0;
			}
		}

		internal void WriteBytes(byte[] byteArray, int offset, int count)
		{
			Debug.Assert(FreeBytes >= count, "Not enough space in output buffer!");
			// faster 
			if (bitCount == 0)
			{
				Array.Copy(byteArray, offset, byteBuffer, BytesWritten, count);
				BytesWritten += count;
			}
			else
			{
				WriteBytesUnaligned(byteArray, offset, count);
			}
		}

		private void WriteBytesUnaligned(byte[] byteArray, int offset, int count)
		{
			for (var i = 0; i < count; i++)
			{
				var b = byteArray[offset + i];
				WriteByteUnaligned(b);
			}
		}

		private void WriteByteUnaligned(byte b)
		{
			WriteBits(8, b);
		}

		internal BufferState DumpState()
		{
			BufferState savedState;
			savedState.pos = BytesWritten;
			savedState.bitBuf = bitBuf;
			savedState.bitCount = bitCount;
			return savedState;
		}

		internal void RestoreState(BufferState state)
		{
			BytesWritten = state.pos;
			bitBuf = state.bitBuf;
			bitCount = state.bitCount;
		}

		internal struct BufferState
		{
			internal int pos; // position
			internal uint bitBuf; // store uncomplete bits 
			internal int bitCount; // number of bits in bitBuffer 
		}
	}
}