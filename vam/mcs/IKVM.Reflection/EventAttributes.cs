using System;

namespace IKVM.Reflection;

[Flags]
public enum EventAttributes
{
	None = 0,
	SpecialName = 0x200,
	RTSpecialName = 0x400,
	ReservedMask = 0x400
}
