using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("e770c650-b3d3-11da-a94d-0800200c9a66")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDocumentEncoderNodeFixup
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.Interface)]
	nsIDOMNode fixupNode([MarshalAs(UnmanagedType.Interface)] nsIDOMNode aNode);
}
