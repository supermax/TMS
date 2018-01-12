#if !UNITY_EDITOR && UNITY_WSA

#region

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TMS.Common.Modularity;

#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	///     Web View
	/// </summary>
	[IocTypeMap(true)]
	internal class WebViewWrapper : IWebView
	{
		private WebView _webView;

#region Implementation of IWebView

		public double MarginDelta { get; set; }

		/// <summary>
		///     Initializes the WebView.
		/// </summary>
		/// <param name="name">The name.</param>
		public void Init(string name)
		{
			// TODO need to throw exc?
			if (_webView != null) return;

			if (Window.Current == null || Window.Current.Content == null)
			{
				throw new OperationCanceledException("Cannot get main visual root.");
			}

			var panel = Window.Current.Content as Panel;
			if (panel == null)
			{
				throw new OperationCanceledException("Cannot get main panel from main visual root.");
			}

			_webView = new WebView
							{
								Visibility = Visibility.Collapsed
							};
			panel.Children.Add(_webView);
		}

		/// <summary>
		///     Terminates this instance.
		/// </summary>
		public void Terminate()
		{
			var panel = _webView.Parent as Panel;
			if (panel == null)
			{
				throw new OperationCanceledException("Cannot get main panel from main visual root.");
			}

			panel.Children.Remove(_webView);
		}

		/// <summary>
		///     Sets the margins.
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="top">The top.</param>
		/// <param name="right">The right.</param>
		/// <param name="bottom">The bottom.</param>
		public void SetMargins(int left, int top, int right, int bottom)
		{
			_webView.Margin = new Thickness(left, top, right, bottom);
		}

		/// <summary>
		///     Sets the visibility.
		/// </summary>
		/// <param name="state">if set to <c>true</c> windows will be hidden.</param>
		public void SetVisibility(bool state)
		{
			_webView.Visibility = state ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		///     Loads the URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		public void LoadURL(string url)
		{
			_webView.Source = new Uri(url);
		}

		/// <summary>
		///     Evaluates the JavaScript.
		/// </summary>
		/// <param name="js">The JavaScript code.</param>
		public void EvaluateJS(string js)
		{
			// TODO check if works
			_webView.InvokeScript(js, null);
		}

		public void SetCallback(IWebViewCallback callback)
		{
			throw new NotImplementedException();
		}

#endregion
	}
}
#endif