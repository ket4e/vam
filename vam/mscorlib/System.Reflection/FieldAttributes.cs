using System.Runtime.InteropServices;

namespace System.Reflection;

[Serializable]
[Flags]
[ComVisible(true)]
public enum FieldAttributes
{
	FieldAccessMask = 7,
	PrivateScope = 0,
	Private = 1,
	FamANDAssem = 2,
	Assembly = 3,
	Family = 4,
	FamORAssem = 5,
	Public = 6,
	Static = 0x10,
	InitOnly = 0x20,
	Literal = 0x40,
	NotSerialized = 0x80,
	HasFieldRVA = 0x100,
	SpecialName = 0x200,
	RTSpecialName = 0x400,
	HasFieldMarshal = 0x1000,
	PinvokeImpl = 0x2000,
	HasDefault = 0x8000,
	ReservedMask = 0x9500
}
