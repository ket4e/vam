using System;

namespace Mono.Mozilla;

internal delegate bool CallbackOnAlertCheck(IntPtr title, IntPtr text, IntPtr chkMsg, ref bool chkState);
