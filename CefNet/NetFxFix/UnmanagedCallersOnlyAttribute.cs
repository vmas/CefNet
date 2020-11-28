#if NET_LESS_5_0
namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class UnmanagedCallersOnlyAttribute : Attribute
	{
		public UnmanagedCallersOnlyAttribute() { }
		public Type[] CallConvs;
		public string EntryPoint;
	}
}
#endif
