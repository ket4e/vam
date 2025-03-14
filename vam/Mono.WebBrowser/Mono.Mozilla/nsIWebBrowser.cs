using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("69E5DF00-7B8B-11d3-AF61-00A024FFC08C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIWebBrowser
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int addWebBrowserListener([MarshalAs(UnmanagedType.Interface)] nsIWeakReference aListener, [MarshalAs(UnmanagedType.LPStruct)] Guid aIID);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int removeWebBrowserListener([MarshalAs(UnmanagedType.Interface)] nsIWeakReference aListener, [MarshalAs(UnmanagedType.LPStruct)] Guid aIID);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getContainerWindow([MarshalAs(UnmanagedType.Interface)] out nsIWebBrowserChrome ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setContainerWindow([MarshalAs(UnmanagedType.Interface)] nsIWebBrowserChrome value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getParentURIContentListener([MarshalAs(UnmanagedType.Interface)] out nsIURIContentListener ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setParentURIContentListener([MarshalAs(UnmanagedType.Interface)] nsIURIContentListener value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getContentDOMWindow([MarshalAs(UnmanagedType.Interface)] out nsIDOMWindow ret);
}
