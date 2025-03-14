using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("8bb35ed9-e332-462d-9155-4a002ab5c958")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIServiceManager
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	IntPtr getService([MarshalAs(UnmanagedType.LPStruct)] Guid aClass, [MarshalAs(UnmanagedType.LPStruct)] Guid aIID);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getServiceByContractID([MarshalAs(UnmanagedType.LPStr)] string aContractID, [MarshalAs(UnmanagedType.LPStruct)] Guid aIID, out IntPtr ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	bool isServiceInstantiated([MarshalAs(UnmanagedType.LPStruct)] Guid aClass, [MarshalAs(UnmanagedType.LPStruct)] Guid aIID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	bool isServiceInstantiatedByContractID([MarshalAs(UnmanagedType.LPStr)] string aContractID, [MarshalAs(UnmanagedType.LPStruct)] Guid aIID);
}
