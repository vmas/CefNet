using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CefNet.CApi;

namespace CefNet
{
	public unsafe partial class CefRequestContext
	{

#if USESAFECACHE

		private static readonly HashSet<WeakReference<CefRequestContext>> WeakRefs = new HashSet<WeakReference<CefRequestContext>>();

		public unsafe static CefRequestContext Wrap(Func<IntPtr, CefRequestContext> create, cef_request_context_t* instance)
		{
			if (instance == null)
				return null;

			IntPtr key = new IntPtr(instance);
			lock (WeakRefs)
			{
				CefRequestContext wrapper;
				foreach (WeakReference<CefRequestContext> weakRef in WeakRefs)
				{
					if (weakRef.TryGetTarget(out wrapper))
					{
						if (wrapper._instance == key
							|| instance->IsSame(wrapper.GetNativeInstance()) != 0)
						{
							instance->@base.Release();
							return wrapper;
						}
					}
				}
				wrapper = CefBaseRefCounted<cef_request_context_t>.Wrap(create, instance);
				WeakRefs.Add(wrapper.WeakRef);
				return wrapper;
			}
		}

#endif // USESAFECACHE

		/// <summary>
		/// Returns the global context object.
		/// </summary>
		public static CefRequestContext GetGlobalContext()
		{
			return CefRequestContext.Wrap(CefRequestContext.Create, CefNativeApi.cef_request_context_get_global_context());
		}

		/// <summary>
		/// Creates a new context object with the specified |settings|.
		/// </summary>
		public CefRequestContext(CefRequestContextSettings settings)
			: this(settings, null)
		{

		}

		/// <summary>
		/// Creates a new context object with the specified |settings| and optional |handler|.
		/// </summary>
		public CefRequestContext(CefRequestContextSettings settings, CefRequestContextHandler handler)
			: this(CefNativeApi.cef_request_context_create_context(
				(settings ?? throw new ArgumentNullException(nameof(settings))).GetNativeInstance(),
				handler != null ? handler.GetNativeInstance() : null))
		{
#if USESAFECACHE
			lock (WeakRefs)
			{
				WeakRefs.Add(this.WeakRef);
			}
#endif
		}

		/// <summary>
		/// Creates a new context object that shares storage with |other|.
		/// </summary>
		public CefRequestContext(CefRequestContext other)
			: this(other, null)
		{

		}

		/// <summary>
		/// Creates a new context object that shares storage with |other| and uses an optional |handler|.
		/// </summary>
		public CefRequestContext(CefRequestContext other, CefRequestContextHandler handler)
			: this(CefNativeApi.cef_create_context_shared(
				(other ?? throw new ArgumentNullException(nameof(other))).GetNativeInstance(),
				handler != null ? handler.GetNativeInstance() : null))
		{
#if USESAFECACHE
			lock (WeakRefs)
			{
				WeakRefs.Add(this.WeakRef);
			}
#endif
		}

#if USESAFECACHE

		protected override void Dispose(bool disposing)
		{
			lock (WeakRefs)
			{
				WeakRefs.Remove(WeakRef);
			}
			base.Dispose(disposing);
		}

		private WeakReference<CefRequestContext> _weakRef;

		public WeakReference<CefRequestContext> WeakRef
		{
			get
			{
				if (_weakRef == null)
				{
					lock (WeakRefs)
					{
						if (_weakRef == null)
							_weakRef = new WeakReference<CefRequestContext>(this);
					}
				}
				return _weakRef;
			}
		}

#endif // USESAFECACHE

		/// <summary>
		/// Sets the <paramref name="value"/> associated with preference <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name of the preference.</param>
		/// <param name="value">
		/// If <paramref name="value"/> is NULL the preference will be restored to its default value.
		/// </param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="InvalidOperationException">Setting the preference fails.</exception>
		public Task SetPreferenceAsync(string name, CefValue value)
		{
			if (CefApi.CurrentlyOn(CefThreadId.UI))
			{
				SetPreferenceInternal(name, value, null);
#if NET45
				return Task.FromResult(true);
#else
				return Task.CompletedTask;
#endif
			}

			var tcs = new TaskCompletionSource<bool>();
			CefNetApi.Post(CefThreadId.UI, () => SetPreferenceInternal(name, value, tcs));
			return tcs.Task;
		}

		/// <summary>
		/// Sets the <paramref name="value"/> associated with preference <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name of the preference.</param>
		/// <param name="value">
		/// If <paramref name="value"/> is NULL the preference will be restored to its default value.
		/// </param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="InvalidOperationException">Setting the preference fails.</exception>
		/// <remarks>This function must be called on the browser process UI thread.</remarks>
		public void SetPreference(string name, CefValue value)
		{
			if (!CefApi.CurrentlyOn(CefThreadId.UI))
				throw new InvalidOperationException("This function must be called on the browser process UI thread.");

			SetPreferenceInternal(name, value, null);
		}

		private unsafe void SetPreferenceInternal(string name, CefValue value, TaskCompletionSource<bool> taskCompletion)
		{
			int retval;
			string errorMsg;
			fixed (char* s0 = name)
			{
				cef_string_t error;
				cef_string_t cstr0 = new cef_string_t { Str = s0, Length = name.Length };
				retval = NativeInstance->SetPreference(&cstr0, value is null ? null : value.GetNativeInstance(), &error);
				GC.KeepAlive(this);
				errorMsg = CefString.ReadAndFree(&error);
			}
			if (retval != 0)
			{
				taskCompletion?.TrySetResult(true);
				return;
			}
			Exception exception = errorMsg is null ? new InvalidOperationException() : new InvalidOperationException(errorMsg);
			if (taskCompletion is null)
				throw exception;
			taskCompletion.TrySetException(exception);
		}

	}
}
