using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("986c11d0-f340-11d4-9075-0010a4e73d9a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIClassInfo
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getInterfaces(out uint count, out IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getHelperForLanguage(uint language, [MarshalAs(UnmanagedType.Interface)] out IntPtr ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getContractID(ref IntPtr ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getClassDescription(ref IntPtr ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getClassID([MarshalAs(UnmanagedType.LPStruct)] out Guid ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getImplementationLanguage(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getFlags(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getClassIDNoAlloc([MarshalAs(UnmanagedType.LPStruct)] out Guid ret);
}
