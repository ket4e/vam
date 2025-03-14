using System;

namespace Mono.Mozilla;

internal delegate bool CallbackOnPromptUsernameAndPassword(IntPtr title, IntPtr text, IntPtr chkMsg, ref bool chkState, out IntPtr username, out IntPtr password);
