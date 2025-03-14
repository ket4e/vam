using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("1A180F60-93B2-11d2-9B8B-00805F8A16D9")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIPersistentProperties : nsIProperties
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int get([MarshalAs(UnmanagedType.LPStr)] string prop, [MarshalAs(UnmanagedType.LPStruct)] Guid iid, out IntPtr result);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int set([MarshalAs(UnmanagedType.LPStr)] string prop, [MarshalAs(UnmanagedType.Interface)] IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int has([MarshalAs(UnmanagedType.LPStr)] string prop, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int undefine([MarshalAs(UnmanagedType.LPStr)] string prop);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int getKeys(out uint count, [MarshalAs(UnmanagedType.LPStr)] out string[] keys);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int load([MarshalAs(UnmanagedType.Interface)] nsIInputStream input);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int save([MarshalAs(UnmanagedType.Interface)] nsIOutputStream output, HandleRef header);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int subclass([MarshalAs(UnmanagedType.Interface)] nsIPersistentProperties superclass);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int enumerate([MarshalAs(UnmanagedType.Interface)] out nsISimpleEnumerator ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getStringProperty(HandleRef key, HandleRef ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setStringProperty(HandleRef key, HandleRef value, HandleRef ret);
}
