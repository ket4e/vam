using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("F51EBADE-8B1A-11D3-AAE7-0010830123B4")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMAbstractView
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getDocument([MarshalAs(UnmanagedType.Interface)] out nsIDOMDocumentView ret);
}
