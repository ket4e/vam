using System;

namespace Mono.Cecil;

[Flags]
public enum EventAttributes : ushort
{
	None = 0,
	SpecialName = 0x200,
	RTSpecialName = 0x400
}
