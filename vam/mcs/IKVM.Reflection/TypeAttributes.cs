using System;

namespace IKVM.Reflection;

[Flags]
public enum TypeAttributes
{
	AnsiClass = 0,
	Class = 0,
	AutoLayout = 0,
	NotPublic = 0,
	Public = 1,
	NestedPublic = 2,
	NestedPrivate = 3,
	NestedFamily = 4,
	NestedAssembly = 5,
	NestedFamANDAssem = 6,
	VisibilityMask = 7,
	NestedFamORAssem = 7,
	SequentialLayout = 8,
	ExplicitLayout = 0x10,
	LayoutMask = 0x18,
	ClassSemanticsMask = 0x20,
	Interface = 0x20,
	Abstract = 0x80,
	Sealed = 0x100,
	SpecialName = 0x400,
	RTSpecialName = 0x800,
	Import = 0x1000,
	Serializable = 0x2000,
	WindowsRuntime = 0x4000,
	UnicodeClass = 0x10000,
	AutoClass = 0x20000,
	CustomFormatClass = 0x30000,
	StringFormatMask = 0x30000,
	HasSecurity = 0x40000,
	ReservedMask = 0x40800,
	BeforeFieldInit = 0x100000,
	CustomFormatMask = 0xC00000
}
