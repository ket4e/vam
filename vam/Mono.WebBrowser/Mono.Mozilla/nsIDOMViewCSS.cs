using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("0b9341f3-95d4-4fa4-adcd-e119e0db2889")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMViewCSS : nsIDOMAbstractView
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	new int getDocument([MarshalAs(UnmanagedType.Interface)] out nsIDOMDocumentView ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getComputedStyle([MarshalAs(UnmanagedType.Interface)] nsIDOMElement elt, HandleRef pseudoElt, [MarshalAs(UnmanagedType.Interface)] out nsIDOMCSSStyleDeclaration ret);
}
