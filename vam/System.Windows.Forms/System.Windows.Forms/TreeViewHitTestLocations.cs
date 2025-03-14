using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[Flags]
[ComVisible(true)]
public enum TreeViewHitTestLocations
{
	None = 1,
	Image = 2,
	Label = 4,
	Indent = 8,
	PlusMinus = 0x10,
	RightOfLabel = 0x20,
	StateImage = 0x40,
	AboveClientArea = 0x100,
	BelowClientArea = 0x200,
	RightOfClientArea = 0x400,
	LeftOfClientArea = 0x800
}
