using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void CallbackWString([MarshalAs(UnmanagedType.LPWStr)] string arg1);
