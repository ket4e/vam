using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("fa9c7f6c-61b3-11d4-9877-00c04fa0cf4a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIInputStream
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int close();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int available(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int read(HandleRef aBuf, uint aCount, out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int readSegments(nsIWriteSegmentFunDelegate aWriter, IntPtr aClosure, uint aCount, out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int isNonBlocking(out bool ret);
}
