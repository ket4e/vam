using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("7b9badc6-c9bc-447a-8670-dbd195aed24b")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMDocumentRange
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int createRange([MarshalAs(UnmanagedType.Interface)] out nsIDOMRange ret);
}
