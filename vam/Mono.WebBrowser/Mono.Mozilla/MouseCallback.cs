using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate bool MouseCallback(MouseInfo mouseInfo, ModifierKeys modifiers, [MarshalAs(UnmanagedType.Interface)] nsIDOMNode target);
