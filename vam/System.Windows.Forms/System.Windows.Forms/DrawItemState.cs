namespace System.Windows.Forms;

[Flags]
public enum DrawItemState
{
	None = 0,
	Selected = 1,
	Grayed = 2,
	Disabled = 4,
	Checked = 8,
	Focus = 0x10,
	Default = 0x20,
	HotLight = 0x40,
	Inactive = 0x80,
	NoAccelerator = 0x100,
	NoFocusRect = 0x200,
	ComboBoxEdit = 0x1000
}
