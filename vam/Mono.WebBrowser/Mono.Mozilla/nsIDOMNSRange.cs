using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("a6cf90f2-15b3-11d2-932e-00805f8add32")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMNSRange
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int createContextualFragment(HandleRef fragment, [MarshalAs(UnmanagedType.Interface)] out nsIDOMDocumentFragment ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int isPointInRange([MarshalAs(UnmanagedType.Interface)] nsIDOMNode parent, int offset, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int comparePoint([MarshalAs(UnmanagedType.Interface)] nsIDOMNode parent, int offset, out short ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int intersectsNode([MarshalAs(UnmanagedType.Interface)] nsIDOMNode n, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int compareNode([MarshalAs(UnmanagedType.Interface)] nsIDOMNode n, out ushort ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int nSDetach();
}
