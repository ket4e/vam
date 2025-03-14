using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("46b91d66-28e2-11d4-ab1e-0010830123b4")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMDocumentEvent
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int createEvent(HandleRef eventType, [MarshalAs(UnmanagedType.Interface)] out nsIDOMEvent ret);
}
