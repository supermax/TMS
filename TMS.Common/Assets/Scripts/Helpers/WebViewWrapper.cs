#region Code Editor
// Maxim
#endregion

#if !SILVERLIGHT && !WINDOWS_PHONE && !UNITY_WP8
namespace diwip.Infr.Client.Common.Slots.Helpers
{
	public class WebViewWrapper
	{
#region Implementation of IWebView

		/// <summary>
		/// Initializes the WebView.
		/// </summary>
		/// <param name="name">The name.</param>
		public void Init(string name)
		{
		}

		/// <summary>
		/// Terminates this instance.
		/// </summary>
		public void Terminate()
		{
		}

		/// <summary>
		/// Sets the margins.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="top">The top.</param>
		/// <param name="right">The right.</param>
		/// <param name="bottom">The bottom.</param>
		public void SetMargins(int left, int top, int right, int bottom)
		{
		}

		/// <summary>
		/// Sets the visibility.
		/// </summary>
		/// <param name="state">if set to <c>true</c> windows will be hidden.</param>
		public void SetVisibility(bool state)
		{
		}

		/// <summary>
		/// Loads the URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		public void LoadURL(string url)
		{
		}

		/// <summary>
		/// Evaluates the JavaScript.
		/// </summary>
		/// <param name="js">The JavaScript code.</param>
		public void EvaluateJS(string js)
		{
		}

		#endregion
	}
}
#endif