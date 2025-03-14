using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("B2C7ED59-8634-4352-9E37-5484C8B6E4E1")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsISelection
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getAnchorNode([MarshalAs(UnmanagedType.Interface)] out nsIDOMNode ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getAnchorOffset(out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getFocusNode([MarshalAs(UnmanagedType.Interface)] out nsIDOMNode ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getFocusOffset(out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getIsCollapsed(out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getRangeCount(out int ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getRangeAt(int index, [MarshalAs(UnmanagedType.Interface)] out nsIDOMRange ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int collapse([MarshalAs(UnmanagedType.Interface)] nsIDOMNode parentNode, int offset);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int extend([MarshalAs(UnmanagedType.Interface)] nsIDOMNode parentNode, int offset);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int collapseToStart();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int collapseToEnd();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int containsNode([MarshalAs(UnmanagedType.Interface)] nsIDOMNode node, bool entirelyContained, out bool ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int selectAllChildren([MarshalAs(UnmanagedType.Interface)] nsIDOMNode parentNode);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int addRange([MarshalAs(UnmanagedType.Interface)] nsIDOMRange range);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int removeRange([MarshalAs(UnmanagedType.Interface)] nsIDOMRange range);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int removeAllRanges();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int deleteFromDocument();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int selectionLanguageChange(bool langRTL);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int toString([MarshalAs(UnmanagedType.LPWStr)] string ret);
}
