using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("df31c120-ded6-11d1-bd85-00805f8ae3f4")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMEventListener
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int handleEvent([MarshalAs(UnmanagedType.Interface)] nsIDOMEvent _event);
}
