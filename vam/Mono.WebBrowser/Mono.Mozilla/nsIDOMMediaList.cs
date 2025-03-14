using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("9b0c2ed7-111c-4824-adf9-ef0da6dad371")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIDOMMediaList
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getMediaText(HandleRef ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setMediaText(HandleRef value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getLength(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int item(uint index, HandleRef ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int deleteMedium(HandleRef oldMedium);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int appendMedium(HandleRef newMedium);
}
