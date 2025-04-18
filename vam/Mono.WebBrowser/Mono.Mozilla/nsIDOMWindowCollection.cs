using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("a6cf906f-15b3-11d2-932e-00805f8add32")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMWindowCollection
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getLength(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int item(uint index, [MarshalAs(UnmanagedType.Interface)] out nsIDOMWindow ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int namedItem(HandleRef name, [MarshalAs(UnmanagedType.Interface)] out nsIDOMWindow ret);
}
