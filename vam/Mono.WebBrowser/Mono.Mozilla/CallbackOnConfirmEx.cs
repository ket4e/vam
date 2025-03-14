using System;
using Mono.WebBrowser;

namespace Mono.Mozilla;

internal delegate bool CallbackOnConfirmEx(IntPtr title, IntPtr text, DialogButtonFlags flags, IntPtr title0, IntPtr title1, IntPtr title2, IntPtr chkMsg, ref bool chkState, out int retVal);
