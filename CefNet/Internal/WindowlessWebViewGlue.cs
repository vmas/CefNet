using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet.Internal
{
	public class WindowlessWebViewGlue : WebViewGlue
	{
		public WindowlessWebViewGlue(IChromiumWebViewPrivate view) 
			: base(view)
		{

		}

		/// <inheritdoc />
		internal protected override bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, CefStringList acceptFilters, int selectedAcceptFilter, CefFileDialogCallback callback)
		{
			callback.Cancel();
			return true;
		}

		protected internal override bool OnJSDialog(CefBrowser browser, string originUrl, CefJSDialogType dialogType, string messageText, string defaultPromptText, CefJSDialogCallback callback, ref int suppressMessage)
		{
			return base.OnJSDialog(browser, originUrl, dialogType, messageText, defaultPromptText, callback, ref suppressMessage);
		}

		protected internal override bool OnBeforeUnloadDialog(CefBrowser browser, string messageText, bool isReload, CefJSDialogCallback callback)
		{
			return base.OnBeforeUnloadDialog(browser, messageText, isReload, callback);
		}

		protected internal override void OnResetDialogState(CefBrowser browser)
		{
			base.OnResetDialogState(browser);
		}

	}
}
