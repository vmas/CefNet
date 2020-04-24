using System;
using System.Collections.Generic;
using CefNet.CApi;

namespace CefNet
{
	public unsafe partial class CefUrlRequest
	{
		/// <summary>
		/// Creates a new URL request that is not associated with a specific browser or
		/// frame.<para/>
		/// For requests originating from the browser process:
		/// <list type="bullet">
		/// <item>
		/// <description>
		/// It may be intercepted by the client via CefResourceRequestHandler or
		/// CefSchemeHandlerFactory;
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// POST data may only contain only a single element of type
		/// <see cref="CefPostDataElementType.File"/> or <see cref="CefPostDataElementType.Bytes"/>.
		/// </description>
		/// </item>
		/// </list>
		/// For requests originating from the render process:
		/// <list type="bullet">
		/// <item>
		/// <description>
		/// It cannot be intercepted by the client so only http(s) and blob schemes
		/// are supported.
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// POST data may only contain a single element of type <see cref="CefPostDataElementType.Bytes"/>.
		/// </description>
		/// </item>
		/// </list>
		/// </summary>
		/// <remarks>
		/// Use <see cref="CefFrame.CreateUrlRequest"/> instead if you want the request to
		/// have this association, in which case it may be handled differently (see
		/// documentation on that function). Requests may originate from the both browser
		/// process and the render process.
		/// </remarks>
		/// <param name="request">
		/// The <see cref="CefRequest"/> object. It will be marked as read-only after calling
		/// this function.
		/// </param>
		/// <param name="client">
		/// The <see cref="CefUrlRequestClient"/> object to handle the resulting response.
		/// </param>
		/// <param name="context">
		/// A request context or null, if <paramref name="context"/> is empty the global
		/// request context will be used. For requests originating from the render process
		/// this parameter must be null.
		/// </param>
		public CefUrlRequest(CefRequest request, CefUrlRequestClient client, CefRequestContext context)
			: this(CefNativeApi.cef_urlrequest_create(
				(request ?? throw new ArgumentNullException(nameof(request))).GetNativeInstance(),
				client != null ? client.GetNativeInstance() : null,
				context != null ? context.GetNativeInstance() : null
				))
		{

		}

	}
}

