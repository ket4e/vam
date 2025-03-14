using System;
using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void nsITimerCallbackDelegate([MarshalAs(UnmanagedType.Interface)] nsITimer timer, IntPtr closure);
