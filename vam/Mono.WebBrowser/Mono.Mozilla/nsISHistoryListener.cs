using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("3b07f591-e8e1-11d4-9882-00c04fa02f40")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsISHistoryListener
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int OnHistoryNewEntry([MarshalAs(UnmanagedType.Interface)] nsIURI aNewURI);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int OnHistoryGoBack([MarshalAs(UnmanagedType.Interface)] nsIURI aBackURI, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int OnHistoryGoForward([MarshalAs(UnmanagedType.Interface)] nsIURI aForwardURI, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int OnHistoryReload([MarshalAs(UnmanagedType.Interface)] nsIURI aReloadURI, uint aReloadFlags, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int OnHistoryGotoIndex(int aIndex, [MarshalAs(UnmanagedType.Interface)] nsIURI aGotoURI, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int OnHistoryPurge(int aNumEntries, out bool ret);
}
