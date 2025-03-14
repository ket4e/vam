using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("114744d9-c369-456e-b55a-52fe52880d2d")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIArray
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getLength(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int queryElementAt(uint index, [MarshalAs(UnmanagedType.LPStruct)] Guid uuid, out IntPtr result);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int indexOf(uint startIndex, [MarshalAs(UnmanagedType.Interface)] IntPtr element, out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int enumerate([MarshalAs(UnmanagedType.Interface)] out nsISimpleEnumerator ret);
}
