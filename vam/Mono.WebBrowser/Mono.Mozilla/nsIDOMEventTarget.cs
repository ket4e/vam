using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("1c773b30-d1cf-11d2-bd95-00805f8ae3f4")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMEventTarget
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int addEventListener(HandleRef type, [MarshalAs(UnmanagedType.Interface)] nsIDOMEventListener listener, bool useCapture);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int removeEventListener(HandleRef type, [MarshalAs(UnmanagedType.Interface)] nsIDOMEventListener listener, bool useCapture);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int dispatchEvent([MarshalAs(UnmanagedType.Interface)] nsIDOMEvent evt, out bool ret);
}
