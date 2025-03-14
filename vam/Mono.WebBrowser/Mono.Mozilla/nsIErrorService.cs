using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Mono.Mozilla;

[ComImport]
[Guid("e72f94b2-5f85-11d4-9877-00c04fa0cf4a")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIErrorService
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int registerErrorStringBundle(short errorModule, [MarshalAs(UnmanagedType.LPStr)] string stringBundleURL);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int unregisterErrorStringBundle(short errorModule);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getErrorStringBundle(short errorModule, [MarshalAs(UnmanagedType.LPStr)] ref string ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int registerErrorStringBundleKey(int error, [MarshalAs(UnmanagedType.LPStr)] string stringBundleKey);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int unregisterErrorStringBundleKey(int error);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getErrorStringBundleKey(int error, StringBuilder ret);
}
