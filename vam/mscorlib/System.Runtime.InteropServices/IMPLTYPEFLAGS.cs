namespace System.Runtime.InteropServices;

[Serializable]
[Obsolete]
[Flags]
public enum IMPLTYPEFLAGS
{
	IMPLTYPEFLAG_FDEFAULT = 1,
	IMPLTYPEFLAG_FSOURCE = 2,
	IMPLTYPEFLAG_FRESTRICTED = 4,
	IMPLTYPEFLAG_FDEFAULTVTABLE = 8
}
