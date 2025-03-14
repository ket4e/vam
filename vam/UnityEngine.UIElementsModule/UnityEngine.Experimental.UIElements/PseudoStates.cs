using System;

namespace UnityEngine.Experimental.UIElements;

[Flags]
internal enum PseudoStates
{
	Active = 1,
	Hover = 2,
	Checked = 8,
	Selected = 0x10,
	Disabled = 0x20,
	Focus = 0x40,
	Invisible = int.MinValue
}
