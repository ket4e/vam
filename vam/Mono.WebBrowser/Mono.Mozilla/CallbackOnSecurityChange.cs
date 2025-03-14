using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void CallbackOnSecurityChange([MarshalAs(UnmanagedType.Interface)] nsIWebProgress progress, [MarshalAs(UnmanagedType.Interface)] nsIRequest request, uint status);
