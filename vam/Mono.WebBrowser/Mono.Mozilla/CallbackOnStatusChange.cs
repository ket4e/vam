using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void CallbackOnStatusChange([MarshalAs(UnmanagedType.Interface)] nsIWebProgress progress, [MarshalAs(UnmanagedType.Interface)] nsIRequest request, [MarshalAs(UnmanagedType.LPWStr)] string message, int status);
