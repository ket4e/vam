using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("3d9f4973-dd2e-48f5-b5f7-2634e09eadd9")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMDocumentStyle
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getStyleSheets([MarshalAs(UnmanagedType.Interface)] out nsIDOMStyleSheetList ret);
}
