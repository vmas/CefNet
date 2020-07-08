using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CefNet.CApi;
using CefNet.Internal;
using CefNet.Net;

namespace CefNet
{
	public unsafe partial class CefCookieManager
	{
		/// <summary>
		/// Returns the global cookie manager. By default data will be stored at CefSettings.CachePath
		/// if specified or in memory otherwise. Using this function is equivalent to calling
		/// CefRequestContext.GetGlobalContext().GetDefaultCookieManager().
		/// </summary>
		/// <param name="callback">
		/// If |callback| is non-NULL it will be executed asnychronously on the UI thread after the
		/// manager&apos;s storage has been initialized.
		/// </param>
		public static CefCookieManager GetGlobalManager(CefCompletionCallback callback)
		{
			return CefCookieManager.Wrap(CefCookieManager.Create, CefNativeApi.cef_cookie_manager_get_global_manager(callback != null ? callback.GetNativeInstance() : null));
		}

		/// <summary>
		/// Gets an array that contains the <see cref="CefNetCookie"/> instances.
		/// </summary>
		/// <param name="filter">
		/// If the <paramref name="filter"/> function returns true, the cookie will be included in the result.
		/// </param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <remarks>The result of the task can be null if cookies cannot be accessed.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Task<CefNetCookie[]> GetCookiesAsync(CancellationToken cancellationToken)
		{
			return GetCookiesAsync(null, cancellationToken);
		}

		/// <summary>
		/// Gets an array that contains the <see cref="CefNetCookie"/> instances.
		/// </summary>
		/// <param name="filter">
		/// If the <paramref name="filter"/> function returns true, the cookie will be included in the result.
		/// </param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <remarks>The result of the task can be null if cookies cannot be accessed.</remarks>
		public Task<CefNetCookie[]> GetCookiesAsync(Func<CefCookie, bool> filter, CancellationToken cancellationToken)
		{
			var cookieVisitor = new GetCookieVisitor(filter, cancellationToken);
			if (!VisitAllCookies(cookieVisitor))
				return Task.FromResult<CefNetCookie[]>(null);
			return cookieVisitor.Task;
		}

		/// <summary>
		/// Gets an array that contains the <see cref="CefNetCookie"/> instances that are associated with a specific URI.
		/// </summary>
		/// <param name="url">The URI of the <see cref="CefNetCookie"/> instances desired.</param>
		/// <param name="includeHttpOnly">A value indicating whether HTTP-only cookies should be included in the result.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <remarks>The result of the task can be null if cookies cannot be accessed.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Task<CefNetCookie[]> GetCookiesAsync(string url, bool includeHttpOnly, CancellationToken cancellationToken)
		{
			return GetCookiesAsync(url, includeHttpOnly, null, cancellationToken);
		}

		/// <summary>
		/// Gets an array that contains the <see cref="CefNetCookie"/> instances that are associated with a specific URI.
		/// </summary>
		/// <param name="url">The URI of the <see cref="CefNetCookie"/> instances desired.</param>
		/// <param name="includeHttpOnly">
		/// A value indicating whether HTTP-only cookies should be included in the result.
		/// </param>
		/// <param name="filter">
		/// If the <paramref name="filter"/> function returns true, the cookie will be included in the result.
		/// </param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <remarks>The result of the task can be null if cookies cannot be accessed.</remarks>
		public Task<CefNetCookie[]> GetCookiesAsync(string url, bool includeHttpOnly, Func<CefCookie, bool> filter, CancellationToken cancellationToken)
		{
			if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri)
				&& (Uri.UriSchemeHttp.Equals(uri.Scheme, StringComparison.Ordinal) || Uri.UriSchemeHttps.Equals(uri.Scheme, StringComparison.Ordinal)))
			{
				var cookieVisitor = new GetCookieVisitor(filter, cancellationToken);
				if (!VisitUrlCookies(url, includeHttpOnly, cookieVisitor))
					return Task.FromResult<CefNetCookie[]>(null);
				return cookieVisitor.Task;
			}
			throw new ArgumentOutOfRangeException(nameof(url));
		}

	}
}
