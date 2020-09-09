using System;
using System.Collections;
using TMS.Common.Extensions;
using TMS.Common.Tasks.Threading;
using UnityEngine;

namespace TMS.Common.Imaging
{
	/// <summary>
	/// 
	/// </summary>
	public static class ScreenshotHelper
	{
		/// <summary>
		/// Snap screen to texture 2d.
		/// </summary>
		/// <returns></returns>
		public static Texture2D Snap2Texture2D()
		{
			// Create a texture the size of the screen, RGB24 format
			var width = Screen.width;
			var height = Screen.height;
			var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

			// Read screen contents into the texture
			tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			tex.Apply();

			return tex;
		}

		/// <summary>
		/// Snap screen to texture 2d (supposed to be executed in coroutine)
		/// </summary>
		/// <param name="callback">The callback.</param>
		/// <returns></returns>
		private static IEnumerator Snap2Texture2DCoroutine(Action<Texture2D> callback)
		{
			// We should only read the screen buffer after rendering is complete
			yield return new WaitForEndOfFrame();

			var tex = Snap2Texture2D();
			callback(tex);
		}

		/// <summary>
		/// Snap screen to texture 2d asynchronously.
		/// </summary>
		/// <param name="callback">The callback.</param>
		public static void Snap2Texture2DAsync(Action<Texture2D> callback)
		{
			ArgumentValidator.AssertNotNull(callback, "callback");

			ThreadHelper.Default.StartCoroutine(Snap2Texture2DCoroutine(callback));
		}
	}
}