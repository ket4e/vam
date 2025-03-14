using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void CallbackWStringInt([MarshalAs(UnmanagedType.LPWStr)] string arg1, int arg2);
