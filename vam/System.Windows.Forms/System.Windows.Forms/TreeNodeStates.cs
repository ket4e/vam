namespace System.Windows.Forms;

[Flags]
public enum TreeNodeStates
{
	Selected = 1,
	Grayed = 2,
	Checked = 8,
	Focused = 0x10,
	Default = 0x20,
	Hot = 0x40,
	Marked = 0x80,
	Indeterminate = 0x100,
	ShowKeyboardCues = 0x200
}
