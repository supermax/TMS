using System;
using System.Collections.Generic;

namespace TMS.Common.Imaging
{
	public class ScaleImage
	{
		private readonly int _height;
		private readonly SColor[] _originalColors;
		private readonly int _width;

		private SColor[] _buffer;
		private SColor[] _outArray;
		private int _scaleSizeX;
		private int _scaleSizeY;
		private float _scaleX;
		private float _scaleY;
		
		public ScaleImage(SColor[] colors, int width)
		{
			_originalColors = colors;

			_width = width;
			_height = _originalColors.Length/width;
		}

		#region linear	

		public SColor[] ScaleLinear(int targetWidth, int targetHeight)
		{
			_scaleX = _width/(float) targetWidth;
			_scaleY = _height/(float) targetHeight;

			// 500 / 250 = 2
			_scaleSizeX = (int) Math.Round(_scaleX/2.0f);
			_scaleSizeY = (int) Math.Round(_scaleY/2.0f);

			_outArray = new SColor[targetWidth*targetHeight];

			for (var y = 0; y < targetHeight; y++)
			{
				for (var x = 0; x < targetWidth; x++)
				{
					_buffer = GetPixelsForTargetPixel(x, y);
					var result = MergePixels();
					_outArray[y*targetWidth + x] = result;
				}
			}
			return _outArray;
		}

		private SColor MergePixels()
		{
			var outColor = new SColor(0, 0, 0, 0);
			foreach (var c in _buffer)
			{
				outColor.Add(c);
			}
			outColor.Divide(_buffer.Length);
			return outColor;
		}

		private SColor[] GetPixelsForTargetPixel(int px, int py)
		{
			var outArray = new Queue<SColor>(_scaleSizeX*_scaleSizeY);
			for (var y = -_scaleSizeY; y < (_scaleSizeY + 1); y++)
			{
				for (var x = -_scaleSizeX; x < (_scaleSizeX + 1); x++)
				{
					var pX = (int) Math.Round(px*_scaleX + x);
					var pY = (int) Math.Round(py*_scaleY + y);
					//image bounds check
					if (pX < 0 || pY < 0 || pX >= _width || pY >= _height) continue;

					var idx = pX + pY*_width;
					outArray.Enqueue(_originalColors[idx]);
				}
			}
			return outArray.ToArray();
		}

		#endregion

		#region lanczos

		public SColor[] ScaleLanczos(int targetWidth, int targetHeight)
		{
			var l = new Lanczos(_originalColors, _width);
			return l.Filter(targetWidth, targetHeight);
		}

		#endregion

		#region point 

		public SColor[] ScalePoint(int targetWidth, int targetHeight)
		{
			_scaleX = _width/(float) targetWidth;
			_scaleY = _height/(float) targetHeight;

			// 500 / 250 = 2
			_outArray = new SColor[targetWidth*targetHeight];

			for (var y = 0; y < targetHeight; y++)
			{
				for (var x = 0; x < targetWidth; x++)
				{
					var tIdx = GetOrgIndex((int) (x*_scaleX), (int) (y*_scaleY), targetWidth);
					_outArray[x + y*targetWidth] = _originalColors[tIdx];
				}
			}
			return _outArray;
		}

		private int GetOrgIndex(int x, int y, float targetWidth)
		{
			return x + y*_width;

			// TODO what is about this code ??? no need to round up ???
			////return (int)(_x * m_scaleX + Math.Floor(_y * m_scaleY * _targetWidth));
			//return Math.Min((int) Math.Round((float) _y*_scaleY*_targetWidth
			//								 + Math.Min(
			//									 (float) _x*_scaleX
			//									 , (float) _width)), _originalColors.Length);
		}

		#endregion
	}
}