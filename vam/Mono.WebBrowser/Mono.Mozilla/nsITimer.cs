using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

[ComImport]
[Guid("436a83fa-b396-11d9-bcfa-00112478d626")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface nsITimer
{
	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int init([MarshalAs(UnmanagedType.Interface)] nsIObserver aObserver, uint aDelay, uint aType);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int initWithFuncCallback(nsITimerCallbackDelegate aCallback, IntPtr aClosure, uint aDelay, uint aType);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int initWithCallback([MarshalAs(UnmanagedType.Interface)] nsITimerCallback aCallback, uint aDelay, uint aType);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int cancel();

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getDelay(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setDelay(uint value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getType(out uint ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int setType(uint value);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getClosure(IntPtr ret);

	[MethodImpl(MethodImplOptions.InternalCall | MethodImplOptions.PreserveSig)]
	int getCallback([MarshalAs(UnmanagedType.Interface)] out nsITimerCallback ret);
}
