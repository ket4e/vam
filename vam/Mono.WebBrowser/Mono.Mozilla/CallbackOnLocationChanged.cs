using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void CallbackOnLocationChanged([MarshalAs(UnmanagedType.Interface)] nsIWebProgress progress, [MarshalAs(UnmanagedType.Interface)] nsIRequest request, [MarshalAs(UnmanagedType.Interface)] nsIURI uri);
