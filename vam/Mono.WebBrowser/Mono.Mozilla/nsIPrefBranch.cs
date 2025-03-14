using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("56c35506-f14b-11d3-99d3-ddbfac2ccf65")]
internal interface nsIPrefBranch
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getRoot(ref IntPtr ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getPrefType([MarshalAs(UnmanagedType.LPStr)] string aPrefName, out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getBoolPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setBoolPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName, int aValue);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getCharPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName, [MarshalAs(UnmanagedType.LPStr)] ref string ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setCharPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName, [MarshalAs(UnmanagedType.LPStr)] string aValue);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getIntPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName, out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setIntPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName, int aValue);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getComplexValue([MarshalAs(UnmanagedType.LPStr)] string aPrefName, [MarshalAs(UnmanagedType.LPStruct)] Guid aType, out IntPtr aValue);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setComplexValue([MarshalAs(UnmanagedType.LPStr)] string aPrefName, [MarshalAs(UnmanagedType.LPStruct)] Guid aType, IntPtr aValue);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int clearUserPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int lockPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int prefHasUserValue([MarshalAs(UnmanagedType.LPStr)] string aPrefName, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int prefIsLocked([MarshalAs(UnmanagedType.LPStr)] string aPrefName, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int unlockPref([MarshalAs(UnmanagedType.LPStr)] string aPrefName);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int deleteBranch([MarshalAs(UnmanagedType.LPStr)] string aStartingAt);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getChildList([MarshalAs(UnmanagedType.LPStr)] string aStartingAt, out uint aCount, [MarshalAs(UnmanagedType.LPStr)] out string[] aChildArray);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int resetBranch([MarshalAs(UnmanagedType.LPStr)] string aStartingAt);
}
