using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CefNet.Internal;

namespace CefNet
{
	/// <summary>
	/// Provides methods for interaction over the DevTools protocol.
	/// </summary>
	public static class DevToolsExtensions
	{
		private static readonly Dictionary<long, DevToolsProtocolClient> _Clients = new Dictionary<long, DevToolsProtocolClient>();

		/// <summary>
		/// Returns a number that uniquely identifies the DevTools Protocol message. 
		/// </summary>
		/// <param name="browserHost">The browser instance.</param>
		/// <returns>A number that uniquely identifies the protocol message.</returns>
		public static int GetNextDevToolsMessageId(this CefBrowserHost browserHost)
		{
			return GetProtocolClient(browserHost).IncrementMessageId();
		}

		private static DevToolsProtocolClient GetProtocolClient(CefBrowserHost browserHost)
		{
			if (browserHost is null)
				throw new ArgumentNullException(nameof(browserHost));

			DevToolsProtocolClient client;
			long browserId = browserHost.Browser.Identifier;
			lock (_Clients)
			{
				if (!_Clients.TryGetValue(browserId, out client))
				{
					var webview = browserHost.Client.GetWebView() as IChromiumWebViewPrivate;
					if (webview is null)
						throw new InvalidOperationException("This browser is not associated with a WebView control.");
					client = new DevToolsProtocolClient(webview);
					_Clients.Add(browserId, client);
				}
			}
			return client;
		}

		internal static void ReleaseProtocolClient(long browserId)
		{
			DevToolsProtocolClient protocolClient;
			lock (_Clients)
			{
				_Clients.Remove(browserId, out protocolClient);
			}
			if (protocolClient is null)
				return;
			protocolClient.Close();
		}

		/// <summary>
		/// Executes a method call over the DevTools protocol.
		/// </summary>
		/// <param name="webview">The WebView control.</param>
		/// <param name="method">The method name.</param>
		/// <param name="parameters">
		/// The dictionaly with method parameters. May be null.
		/// See the <see href="https://chromedevtools.github.io/devtools-protocol/">
		/// DevTools Protocol documentation</see> for details of supported methods
		/// and the expected parameters.
		/// </param>
		/// <returns>
		/// The JSON string with the response. Structure of the response varies depending
		/// on the method name and is defined by the &apos;RETURN OBJECT&apos; section of
		/// the Chrome DevTools Protocol command description.
		/// </returns>
		/// <remarks>
		/// Usage of the ExecuteDevToolsMethodAsync function does not require an active
		/// DevTools front-end or remote-debugging session. Other active DevTools sessions
		/// will continue to function independently. However, any modification of global
		/// browser state by one session may not be reflected in the UI of other sessions.
		/// <para/>
		/// Communication with the DevTools front-end (when displayed) can be logged
		/// for development purposes by passing the `--devtools-protocol-log-
		/// file=&lt;path&gt;` command-line flag.
		/// </remarks>
		public static async Task<string> ExecuteDevToolsMethodAsync(this IChromiumWebView webview, string method, CefDictionaryValue parameters)
		{
			if (webview is null)
				throw new ArgumentNullException(nameof(webview));

			if (method is null)
				throw new ArgumentNullException(nameof(method));

			method = method.Trim();
			if (method.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(method));

			CefBrowser browser = webview.BrowserObject;
			if (browser is null)
				throw new InvalidOperationException();

			CefBrowserHost browserHost = browser.Host;
			DevToolsProtocolClient protocolClient = GetProtocolClient(browserHost);

			await CefNetSynchronizationContextAwaiter.GetForThread(CefThreadId.UI);
			int messageId = browserHost.ExecuteDevToolsMethod(protocolClient.IncrementMessageId(), method, parameters);
			protocolClient.UpdateLastMessageId(messageId);
			DevToolsMethodResult r = await protocolClient.WaitForMessageAsync(messageId).ConfigureAwait(false);
			if (r.Success)
			{
				if (r.Result is null)
					return null;
				return Encoding.UTF8.GetString(r.Result);
			}
			CefValue errorValue = CefApi.CefParseJSONBuffer(r.Result, CefJsonParserOptions.AllowTrailingCommas);
			if (errorValue is null)
				throw new DevToolsProtocolException($"An unknown error occurred while trying to execute the '{method}' method.");
			throw new DevToolsProtocolException(errorValue.GetDictionary().GetString("message"));
		}

		/// <summary>
		/// Executes a method call over the DevTools protocol.
		/// </summary>
		/// <param name="webview">The WebView control.</param>
		/// <param name="method">The method name.</param>
		/// <param name="parameters">
		/// The JSON string with method parameters. May be null.
		/// See the <see href="https://chromedevtools.github.io/devtools-protocol/">
		/// DevTools Protocol documentation</see> for details of supported methods
		/// and the expected parameters.
		/// </param>
		/// <returns>
		/// The JSON string with the response. Structure of the response varies depending
		/// on the method name and is defined by the &apos;RETURN OBJECT&apos; section of
		/// the Chrome DevTools Protocol command description.
		/// </returns>
		/// <remarks>
		/// Usage of the ExecuteDevToolsMethodAsync function does not require an active
		/// DevTools front-end or remote-debugging session. Other active DevTools sessions
		/// will continue to function independently. However, any modification of global
		/// browser state by one session may not be reflected in the UI of other sessions.
		/// <para/>
		/// Communication with the DevTools front-end (when displayed) can be logged
		/// for development purposes by passing the `--devtools-protocol-log-
		/// file=&lt;path&gt;` command-line flag.
		/// </remarks>
		public static Task<string> ExecuteDevToolsMethodAsync(this IChromiumWebView webview, string method, string parameters)
		{
			CefValue args = null;
			if (parameters != null)
			{
				args = CefApi.CefParseJSON(parameters, CefJsonParserOptions.AllowTrailingCommas, out CefJsonParserError errorCode, out string errorMessage);
				if (args is null)
					throw new ArgumentOutOfRangeException(nameof(parameters), errorMessage is null ? $"An error occurred during JSON parsing: {errorCode}." : errorMessage);
			}
			return ExecuteDevToolsMethodAsync(webview, method, args is null ? default(CefDictionaryValue) : args.GetDictionary());
		}

		/// <summary>
		/// Executes a method call over the DevTools protocol without any optional parameters.
		/// </summary>
		/// <param name="webview">The WebView control.</param>
		/// <param name="method">The method name.</param>
		/// <returns>
		/// The JSON string with the response. Structure of the response varies depending
		/// on the method name and is defined by the &apos;RETURN OBJECT&apos; section of
		/// the Chrome DevTools Protocol command description.
		/// </returns>
		/// <remarks>
		/// Usage of the ExecuteDevToolsMethodAsync function does not require an active
		/// DevTools front-end or remote-debugging session. Other active DevTools sessions
		/// will continue to function independently. However, any modification of global
		/// browser state by one session may not be reflected in the UI of other sessions.
		/// <para/>
		/// Communication with the DevTools front-end (when displayed) can be logged
		/// for development purposes by passing the `--devtools-protocol-log-
		/// file=&lt;path&gt;` command-line flag.
		/// </remarks>
		public static Task<string> ExecuteDevToolsMethodAsync(this IChromiumWebView webview, string method)
		{
			return ExecuteDevToolsMethodAsync(webview, method, default(CefDictionaryValue));
		}

	}
}
