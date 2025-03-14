using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("94928AB3-8B63-11d3-989D-001083010E9B")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIURIContentListener
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	bool onStartURIOpen([MarshalAs(UnmanagedType.Interface)] nsIURI aURI);

	[MethodImpl(MethodImplOptions.InternalCall)]
	bool doContent([MarshalAs(UnmanagedType.LPStr)] string aContentType, bool aIsContentPreferred, [MarshalAs(UnmanagedType.Interface)] nsIRequest aRequest, [MarshalAs(UnmanagedType.Interface)] out nsIStreamListener aContentHandler);

	[MethodImpl(MethodImplOptions.InternalCall)]
	bool isPreferred([MarshalAs(UnmanagedType.LPStr)] string aContentType, [MarshalAs(UnmanagedType.LPStr)] ref string aDesiredContentType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	bool canHandleContent([MarshalAs(UnmanagedType.LPStr)] string aContentType, bool aIsContentPreferred, [MarshalAs(UnmanagedType.LPStr)] ref string aDesiredContentType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.Interface)]
	IntPtr getLoadCookie();

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setLoadCookie([MarshalAs(UnmanagedType.Interface)] IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.Interface)]
	nsIURIContentListener getParentContentListener();

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setParentContentListener([MarshalAs(UnmanagedType.Interface)] nsIURIContentListener value);
}
