using System;

namespace Mono.Mozilla;

internal delegate bool CallbackOnConfirmCheck(IntPtr title, IntPtr text, IntPtr chkMsg, ref bool chkState);
