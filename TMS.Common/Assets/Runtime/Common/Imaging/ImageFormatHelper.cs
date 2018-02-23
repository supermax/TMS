#region

using System;
using System.Collections.Generic;
using System.Text;
using TMS.Common.Extensions;
using UnityEngine;

#endregion

namespace TMS.Common.Imaging
{
	/// <summary>
	///     Image Format Helper
	/// </summary>
	public static class ImageFormatHelper
    {
        private static readonly byte[] Bmp = Encoding.ASCII.GetBytes("BM"); // BMP
        private static readonly byte[] Gif = Encoding.ASCII.GetBytes("GIF"); // GIF
        private static readonly byte[] Png = {137, 80, 78, 71}; // PNG
        private static readonly byte[] Tiff = {73, 73, 42}; // TIFF
        private static readonly byte[] Tiff2 = {77, 77, 42}; // TIFF
        private static readonly byte[] Jpeg = {255, 216, 255, 224}; // jpeg
        private static readonly byte[] Jpeg2 = {255, 216, 255, 225}; // jpeg canon

        private static readonly IDictionary<ImageFormat, Predicate<byte[]>> Formats;

        static ImageFormatHelper()
        {
            Formats = new Dictionary<ImageFormat, Predicate<byte[]>>
            {
                {ImageFormat.Bmp, input => Bmp.Equals(input, 0, Bmp.Length)},
                {ImageFormat.Gif, input => Gif.Equals(input, 0, Gif.Length)},
                {ImageFormat.Png, input => Png.Equals(input, 0, Png.Length)},
                {ImageFormat.Tiff, input => Tiff.Equals(input, 0, Tiff.Length) || Tiff2.Equals(input, 0, Tiff2.Length)},
                {ImageFormat.Jpeg, input => Jpeg.Equals(input, 0, Jpeg.Length) || Jpeg2.Equals(input, 0, Jpeg2.Length)}
            };
        }

	    /// <summary>
	    ///     from http://www.mikekunz.com/image_file_header.html
	    /// </summary>
	    /// <param name="bytes"></param>
	    /// <returns></returns>
	    private static ImageFormat GetImageFormat(byte[] bytes)
        {
            foreach (var format in Formats)
            {
                if (format.Value(bytes))
                {
                    return format.Key;
                }
            }
            return ImageFormat.Unknown;
        }

	    /// <summary>
	    /// </summary>
	    /// <remarks>
	    ///     from https://stackoverflow.com/questions/1397512/find-image-format-using-bitmap-object-in-c-sharp
	    /// </remarks>
	    /// <param name="raw"></param>
	    /// <returns></returns>
	    public static Texture2D CreateTextureByFileType(byte[] raw)
        {
            Texture2D texture;
            var format = GetImageFormat(raw);
            switch (format)
            {
                case ImageFormat.Bmp:
                case ImageFormat.Jpeg:
                    texture = new Texture2D(0, 0, TextureFormat.RGB24, false);
                    break;

                case ImageFormat.Gif:
                case ImageFormat.Png:
                    texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                    break;

                default:
                    Debug.LogWarning("Unhandled image format!");
                    texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                    break;
            }

            texture.LoadImage(raw);
            texture.Apply();
            return texture;
        }

        private enum ImageFormat
        {
            Bmp,
            Jpeg,
            Gif,
            Tiff,
            Png,
            Unknown
        }
    }
}