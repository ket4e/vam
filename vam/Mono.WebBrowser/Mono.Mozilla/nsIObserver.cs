using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("DB242E01-E4D9-11d2-9DDE-000064657374")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIObserver
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int observe([MarshalAs(UnmanagedType.Interface)] IntPtr aSubject, [MarshalAs(UnmanagedType.LPStr)] string aTopic, [MarshalAs(UnmanagedType.LPWStr)] string aData);
}
