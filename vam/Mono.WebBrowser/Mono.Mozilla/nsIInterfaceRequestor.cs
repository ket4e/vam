using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("033A1470-8B2A-11d3-AF88-00A024FFC08C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIInterfaceRequestor
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getInterface([MarshalAs(UnmanagedType.LPStruct)] Guid uuid, out IntPtr result);
}
