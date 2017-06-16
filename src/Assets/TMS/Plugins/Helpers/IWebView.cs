#region Code Editor
// Maxim
#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	/// Interface for WebView
	/// </summary>
	public interface IWebView
	{
		/// <summary>
		/// Gets or sets the margin delta.
		/// </summary>
		/// <value>
		/// The margin delta.
		/// </value>
		double MarginDelta { get; set; }

		/// <summary>
		/// Initializes the WebView.
		/// </summary>
		/// <param name="name">The name.</param>
		void Init(string name);

		/// <summary>
		/// Terminates this instance.
		/// </summary>
		void Terminate();

		/// <summary>
		/// Sets the margins.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="top">The top.</param>
		/// <param name="right">The right.</param>
		/// <param name="bottom">The bottom.</param>
		void SetMargins(int left, int top, int right, int bottom);

		/// <summary>
		/// Sets the visibility.
		/// </summary>
		/// <param name="state">if set to <c>true</c> windows will be hidden.</param>
		void SetVisibility(bool state);

		/// <summary>
		/// Loads the URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		void LoadURL(string url);

		/// <summary>
		/// Evaluates the JavaScript.
		/// </summary>
		/// <param name="js">The JavaScript code.</param>
		void EvaluateJS(string js);

		/// <summary>
		/// Sets the callback.
		/// </summary>
		/// <param name="callback">The callback.</param>
		void SetCallback(IWebViewCallback callback);
	}

	/// <summary>
	/// Interface for WebViewCallback
	/// </summary>
	public interface IWebViewCallback
	{
		/// <summary>
		/// Called on load start.
		/// </summary>
		/// <param name="url">The URL.</param>
		void onLoadStart(string url);

		/// <summary>
		/// Called on load finish.
		/// </summary>
		/// <param name="url">The URL.</param>
		void onLoadFinish(string url);

		/// <summary>
		/// v load fail.
		/// </summary>
		/// <param name="url">The URL.</param>
		void onLoadFail(string url);
	}
}
