namespace System.Windows.Forms;

[Flags]
internal enum MotifFunctions
{
	All = 1,
	Resize = 2,
	Move = 4,
	Minimize = 8,
	Maximize = 0x10,
	Close = 0x20
}
