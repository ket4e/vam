using System;

namespace ZenFulcrum.EmbeddedBrowser.VR;

[Flags]
public enum JoyPadType
{
	None = 0,
	Unknown = 2,
	Joystick = 4,
	TouchPad = 8
}
