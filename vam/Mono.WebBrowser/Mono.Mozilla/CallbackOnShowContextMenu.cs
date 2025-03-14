using System.Runtime.InteropServices;

namespace Mono.Mozilla;

internal delegate void CallbackOnShowContextMenu(uint contextFlags, [MarshalAs(UnmanagedType.Interface)] nsIDOMEvent eve, [MarshalAs(UnmanagedType.Interface)] nsIDOMNode node);
