using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("0d0acd2a-61b4-11d4-9877-00c04fa0cf4a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIOutputStream
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int close();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int flush();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int write([MarshalAs(UnmanagedType.LPStr)] string aBuf, uint aCount, out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int writeFrom([MarshalAs(UnmanagedType.Interface)] nsIInputStream aFromStream, uint aCount, out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int writeSegments(nsIReadSegmentFunDelegate aReader, IntPtr aClosure, uint aCount, out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int isNonBlocking(out bool ret);
}
