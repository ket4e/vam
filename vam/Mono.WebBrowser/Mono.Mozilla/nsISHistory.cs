using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("7294FE9B-14D8-11D5-9882-00C04FA02F40")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsISHistory
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getCount(out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getIndex(out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getMaxLength(out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setMaxLength(int value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getEntryAtIndex(int index, bool modifyIndex, [MarshalAs(UnmanagedType.Interface)] out nsIHistoryEntry ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int PurgeHistory(int numEntries);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int addSHistoryListener([MarshalAs(UnmanagedType.Interface)] nsISHistoryListener aListener);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int removeSHistoryListener([MarshalAs(UnmanagedType.Interface)] nsISHistoryListener aListener);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getSHistoryEnumerator([MarshalAs(UnmanagedType.Interface)] out nsISimpleEnumerator ret);
}
