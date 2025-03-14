using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("a796816d-7d47-4348-9ab8-c7aeb3216a7d")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsITimerCallback
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int notify([MarshalAs(UnmanagedType.Interface)] nsITimer timer);
}
