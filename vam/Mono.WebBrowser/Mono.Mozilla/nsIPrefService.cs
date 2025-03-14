using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("decb9cc7-c08f-4ea5-be91-a8fc637ce2d2")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsIPrefService
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int readUserPrefs([MarshalAs(UnmanagedType.Interface)] nsIFile aFile);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int resetPrefs();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int resetUserPrefs();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int savePrefFile([MarshalAs(UnmanagedType.Interface)] nsIFile aFile);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getBranch([MarshalAs(UnmanagedType.LPStr)] string aPrefRoot, [MarshalAs(UnmanagedType.Interface)] out nsIPrefBranch ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getDefaultBranch([MarshalAs(UnmanagedType.LPStr)] string aPrefRoot, [MarshalAs(UnmanagedType.Interface)] out nsIPrefBranch ret);
}
