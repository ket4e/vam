using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate bool KeyCallback(KeyInfo keyInfo, ModifierKeys modifiers, [MarshalAs(UnmanagedType.Interface)] nsIDOMNode target);
