using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[Flags]
[ComVisible(true)]
public enum Border3DSide
{
	Left = 1,
	Top = 2,
	Right = 4,
	Bottom = 8,
	Middle = 0x800,
	All = 0x80F
}
