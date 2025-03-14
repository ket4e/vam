using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("f85c5a20-258d-11db-a98b-0800200c9a66")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDocumentEncoder
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	void init([MarshalAs(UnmanagedType.Interface)] nsIDOMDocument aDocument, HandleRef aMimeType, uint aFlags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setSelection([MarshalAs(UnmanagedType.Interface)] nsISelection aSelection);

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setRange([MarshalAs(UnmanagedType.Interface)] nsIDOMRange aRange);

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setNode([MarshalAs(UnmanagedType.Interface)] nsIDOMNode aNode);

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setContainerNode([MarshalAs(UnmanagedType.Interface)] nsIDOMNode aContainer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setCharset(HandleRef aCharset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setWrapColumn(uint aWrapColumn);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getMimeType(HandleRef ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	void encodeToStream([MarshalAs(UnmanagedType.Interface)] nsIOutputStream aStream);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int encodeToString(HandleRef ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int encodeToStringWithContext(HandleRef aContextString, HandleRef aInfoString, HandleRef ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	void setNodeFixup([MarshalAs(UnmanagedType.Interface)] nsIDocumentEncoderNodeFixup aFixup);
}
