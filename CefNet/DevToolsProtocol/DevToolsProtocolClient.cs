using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefNet.Internal;

namespace CefNet
{
	internal sealed class DevToolsProtocolClient : CefDevToolsMessageObserver
	{
		private readonly IChromiumWebViewPrivate _webview;
		private readonly CefRegistration _registration;
		private Dictionary<int, TaskCompletionSource<DevToolsMethodResult>> _waitTasks = new Dictionary<int, TaskCompletionSource<DevToolsMethodResult>>();
		private int _lastMessageId;

		public DevToolsProtocolClient(IChromiumWebViewPrivate webview)
		{
			_webview = webview;
			_registration = ((IChromiumWebView)webview).BrowserObject.Host.AddDevToolsMessageObserver(this);
		}

		private object SyncRoot
		{
			get { return _waitTasks; }
		}

		public int IncrementMessageId()
		{
			int id;
			lock (SyncRoot)
			{
				id = ++_lastMessageId;
			}
			return id;
		}

		public void UpdateLastMessageId(int messageId)
		{
			if (messageId == 0)
				throw new InvalidOperationException();

			lock (SyncRoot)
			{
				if (_lastMessageId < messageId)
					_lastMessageId = messageId;
			}
		}

		public Task<DevToolsMethodResult> WaitForMessageAsync(int id)
		{
			return RemoveOrAddTaskSource(id).Task;
		}

		private TaskCompletionSource<DevToolsMethodResult> RemoveOrAddTaskSource(int messageId)
		{
			TaskCompletionSource<DevToolsMethodResult> m;
			lock (SyncRoot)
			{
				if (!_waitTasks.Remove(messageId, out m))
				{
#if NET45
					m = new TaskCompletionSource<DevToolsMethodResult>();
#else
					m = new TaskCompletionSource<DevToolsMethodResult>(TaskCreationOptions.RunContinuationsAsynchronously);
#endif
					_waitTasks.Add(messageId, m);
				}
			}
			return m;
		}

		protected internal unsafe override void OnDevToolsEvent(CefBrowser browser, string method, IntPtr @params, long paramsSize)
		{
			string args;
			if (@params == IntPtr.Zero)
			{
				args = null;
			}
			else
			{
#if NET45
				byte[] buffer = new byte[paramsSize];
				Marshal.Copy(@params, buffer, 0, (int)paramsSize);
				args = Encoding.UTF8.GetString(buffer);
#else
				args = Encoding.UTF8.GetString((byte*)@params, (int)paramsSize);
#endif
			}
			_webview.RaiseDevToolsEventAvailable(new DevToolsProtocolEventAvailableEventArgs(method, args));
		}

		protected internal override bool OnDevToolsMessage(CefBrowser browser, IntPtr message, long messageSize)
		{
			return false;
		}

		/// <summary>
		/// Method that will be called after attempted execution of a DevTools protocol
		/// function.
		/// </summary>
		/// <param name="browser">The originating browser instance.</param>
		/// <param name="messageId">The &quot;id&quot; value that identifies the originating function call message.</param>
		/// <param name="success">true, if the function succeeded; otherwise false.</param>
		/// <param name="result">
		/// The UTF8-encoded JSON dictionary value (which may be NULL). <paramref name="result"/>
		/// is only valid for the scope of this callback and should be copied if necessary.See the
		/// <see cref="OnDevToolsMessage"/> documentation for additional details on
		/// <paramref name="result"/> contents.
		/// </param>
		/// <param name="resultSize">The size of the <paramref name="result"/> buffer.</param>
		protected internal override void OnDevToolsMethodResult(CefBrowser browser, int messageId, bool success, IntPtr result, long resultSize)
		{
			byte[] buffer;
			if (result == IntPtr.Zero)
			{
				buffer = null;
			}
			else
			{
				buffer = new byte[resultSize];
				Marshal.Copy(result, buffer, 0, (int)resultSize);
			}
#if NET45
			ThreadPool.QueueUserWorkItem(CompleteWaitTask, new DevToolsMethodResult(messageId, buffer, success));
#else
			RemoveOrAddTaskSource(messageId).SetResult(new DevToolsMethodResult(messageId, buffer, success));
#endif
		}

#if NET45
		private void CompleteWaitTask(object state)
		{
			DevToolsMethodResult r = (DevToolsMethodResult)state;
			RemoveOrAddTaskSource(r.MessageID).SetResult(r);
		}
#endif

	}
}
