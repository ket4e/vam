using System;

namespace Mono.Mozilla;

internal delegate bool CallbackOnPrompt(IntPtr title, IntPtr text, ref IntPtr retVal);
