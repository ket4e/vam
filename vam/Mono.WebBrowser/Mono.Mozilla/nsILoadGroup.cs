using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("3de0a31c-feaf-400f-9f1e-4ef71f8b20cc")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsILoadGroup : nsIRequest
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int getName(HandleRef ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int isPending(out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getStatus(out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int cancel(int aStatus);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int suspend();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int resume();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int getLoadGroup([MarshalAs(UnmanagedType.Interface)] out nsILoadGroup ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int setLoadGroup([MarshalAs(UnmanagedType.Interface)] nsILoadGroup value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int getLoadFlags(out ulong ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int setLoadFlags(ulong value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getGroupObserver([MarshalAs(UnmanagedType.Interface)] out nsIRequestObserver ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setGroupObserver([MarshalAs(UnmanagedType.Interface)] nsIRequestObserver value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getDefaultLoadRequest([MarshalAs(UnmanagedType.Interface)] out nsIRequest ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setDefaultLoadRequest([MarshalAs(UnmanagedType.Interface)] nsIRequest value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int addRequest([MarshalAs(UnmanagedType.Interface)] nsIRequest aRequest, IntPtr aContext);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int removeRequest([MarshalAs(UnmanagedType.Interface)] nsIRequest aRequest, IntPtr aContext, int aStatus);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getRequests([MarshalAs(UnmanagedType.Interface)] out nsISimpleEnumerator ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getActiveCount(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getNotificationCallbacks([MarshalAs(UnmanagedType.Interface)] out nsIInterfaceRequestor ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setNotificationCallbacks([MarshalAs(UnmanagedType.Interface)] nsIInterfaceRequestor value);
}
