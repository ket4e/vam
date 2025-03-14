namespace System.Windows.Forms.RTF;

internal enum TokenClass
{
	None = -1,
	Unknown,
	Group,
	Text,
	Control,
	EOF,
	MaxClass
}
