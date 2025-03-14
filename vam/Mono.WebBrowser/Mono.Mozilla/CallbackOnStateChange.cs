using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void CallbackOnStateChange([MarshalAs(UnmanagedType.Interface)] nsIWebProgress progress, [MarshalAs(UnmanagedType.Interface)] nsIRequest request, int arg2, uint arg3);
