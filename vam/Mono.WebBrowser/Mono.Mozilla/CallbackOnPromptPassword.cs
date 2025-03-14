using System;

namespace Mono.Mozilla;

internal delegate bool CallbackOnPromptPassword(IntPtr title, IntPtr text, IntPtr chkMsg, ref bool chkState, out IntPtr password);
