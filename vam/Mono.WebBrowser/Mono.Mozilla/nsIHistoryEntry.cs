using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("A41661D4-1417-11D5-9882-00C04FA02F40")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIHistoryEntry
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getURI([MarshalAs(UnmanagedType.Interface)] out nsIURI ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getTitle([MarshalAs(UnmanagedType.LPWStr)] string ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getIsSubFrame(out bool ret);
}
