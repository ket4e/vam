using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void CallbackOnProgress([MarshalAs(UnmanagedType.Interface)] nsIWebProgress progress, [MarshalAs(UnmanagedType.Interface)] nsIRequest request, int arg2, int arg3);
