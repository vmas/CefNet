using CefNet.Internal;
using System;
using System.Threading;

namespace CefNet
{
	/// <summary>
	/// Provide global methods.
	/// </summary>
	public static class CefNetApi
	{
		/// <summary>
		/// Compares two instances of the specified reference type <typeparamref name="T"/>
		/// for reference equality and, if they are equal, replaces the first one.
		/// </summary>
		/// <typeparam name="T">
		/// The type to be used for <paramref name="location"/>, <paramref name="value"/>,
		/// and <paramref name="comparand"/>. This type must be a reference type.
		/// </typeparam>
		/// <param name="location">
		/// The destination, whose value is compared by reference with <paramref name="comparand"/>
		/// and possibly replaced.
		/// </param>
		/// <param name="value">
		/// The value that replaces the destination value if the comparison by reference
		/// results in equality.
		/// </param>
		/// <param name="comparand">
		/// The value that is compared by reference to the value at <paramref name="location"/>.
		/// </param>
		/// <returns></returns>
		public static T CompareExchange<T>(in T location, T value, T comparand)
			where T : class
		{
			return UtilsExtensions.As<T>(Interlocked.CompareExchange(ref UtilsExtensions.AsRef<T, object>(in location), value, comparand));
		}

		/// <summary>
		/// Post an action for execution on the specified thread. This function may be
		/// called on any thread. It is an error to request a thread from the wrong
		/// process.
		/// </summary>
		public static bool Post(CefThreadId threadId, Action action)
		{
			return CefApi.PostTask(threadId, new CefActionTask(action));
		}

		/// <summary>
		/// Post an action for execution on the specified thread. This function may be
		/// called on any thread. It is an error to request a thread from the wrong
		/// process.
		/// </summary>
		public static bool Post(CefThreadId threadId, Action action, long delay)
		{
			return CefApi.PostTask(threadId, new CefActionTask(action), delay);
		}

	}
}
