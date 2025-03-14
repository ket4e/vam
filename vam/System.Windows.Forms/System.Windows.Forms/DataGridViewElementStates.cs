using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
[Flags]
public enum DataGridViewElementStates
{
	None = 0,
	Displayed = 1,
	Frozen = 2,
	ReadOnly = 4,
	Resizable = 8,
	ResizableSet = 0x10,
	Selected = 0x20,
	Visible = 0x40
}
