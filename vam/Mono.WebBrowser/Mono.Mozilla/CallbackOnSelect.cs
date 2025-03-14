using System;

namespace Mono.Mozilla;

internal delegate bool CallbackOnSelect(IntPtr title, IntPtr text, uint count, IntPtr list, out int retVal);
