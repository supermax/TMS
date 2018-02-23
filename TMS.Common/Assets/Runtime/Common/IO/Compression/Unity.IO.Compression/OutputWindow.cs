#region Usings

using System;
using System.Diagnostics;

#endregion

namespace Unity.IO.Compression
{
	// This class maintains a window for decompressed output.
	// We need to keep this because the decompressed information can be 
	// a literal or a length/distance pair. For length/distance pair,
	// we need to look back in the output window and copy bytes from there.
	// We use a byte array of WindowSize circularly.
	//
	internal class OutputWindow
	{
		private const int WindowSize = 32768;
		private const int WindowMask = 32767;
		private int end; // this is the position to where we should write next byte 

		private readonly byte[] window = new byte[WindowSize]; //The window is 2^15 bytes

		// Free space in output window
		public int FreeBytes
		{
			get { return WindowSize - AvailableBytes; }
		}

		// bytes not consumed in output window
		public int AvailableBytes { get; private set; }

		// Add a byte to output window
		public void Write(byte b)
		{
			Debug.Assert(AvailableBytes < WindowSize, "Can't add byte when window is full!");
			window[end++] = b;
			end &= WindowMask;
			++AvailableBytes;
		}

		public void WriteLengthDistance(int length, int distance)
		{
			Debug.Assert(AvailableBytes + length <= WindowSize, "Not enough space");

			// move backwards distance bytes in the output stream, 
			// and copy length bytes from this position to the output stream.
			AvailableBytes += length;
			var copyStart = (end - distance) & WindowMask; // start position for coping.

			var border = WindowSize - length;
			if (copyStart <= border && end < border)
				if (length <= distance)
				{
					Array.Copy(window, copyStart, window, end, length);
					end += length;
				}
				else
				{
					// The referenced string may overlap the current
					// position; for example, if the last 2 bytes decoded have values
					// X and Y, a string reference with <length = 5, distance = 2>
					// adds X,Y,X,Y,X to the output stream.
					while (length-- > 0) window[end++] = window[copyStart++];
				}
			else
				while (length-- > 0)
				{
					window[end++] = window[copyStart++];
					end &= WindowMask;
					copyStart &= WindowMask;
				}
		}

		// Copy up to length of bytes from input directly.
		// This is used for uncompressed block.
		public int CopyFrom(InputBuffer input, int length)
		{
			length = Math.Min(Math.Min(length, WindowSize - AvailableBytes), input.AvailableBytes);
			int copied;

			// We might need wrap around to copy all bytes.
			var tailLen = WindowSize - end;
			if (length > tailLen)
			{
				// copy the first part     
				copied = input.CopyTo(window, end, tailLen);
				if (copied == tailLen) copied += input.CopyTo(window, 0, length - tailLen);
			}
			else
			{
				// only one copy is needed if there is no wrap around.
				copied = input.CopyTo(window, end, length);
			}

			end = (end + copied) & WindowMask;
			AvailableBytes += copied;
			return copied;
		}

		// copy the decompressed bytes to output array.        
		public int CopyTo(byte[] output, int offset, int length)
		{
			int copy_end;

			if (length > AvailableBytes)
			{
				// we can copy all the decompressed bytes out
				copy_end = end;
				length = AvailableBytes;
			}
			else
			{
				copy_end = (end - AvailableBytes + length) & WindowMask; // copy length of bytes
			}

			var copied = length;

			var tailLen = length - copy_end;
			if (tailLen > 0)
			{
				// this means we need to copy two parts seperately
				// copy tailLen bytes from the end of output window
				Array.Copy(window, WindowSize - tailLen,
					output, offset, tailLen);
				offset += tailLen;
				length = copy_end;
			}
			Array.Copy(window, copy_end - length, output, offset, length);
			AvailableBytes -= copied;
			Debug.Assert(AvailableBytes >= 0, "check this function and find why we copied more bytes than we have");
			return copied;
		}
	}
}