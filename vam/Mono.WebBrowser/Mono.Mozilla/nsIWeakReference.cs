using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("9188bc85-f92e-11d2-81ef-0060083a0bcf")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIWeakReference
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int QueryReferent([MarshalAs(UnmanagedType.LPStruct)] Guid uuid, out IntPtr result);
}
