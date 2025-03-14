namespace System.Windows.Forms;

[Flags]
public enum ListViewHitTestLocations
{
	None = 1,
	Image = 2,
	Label = 4,
	BelowClientArea = 0x10,
	RightOfClientArea = 0x20,
	LeftOfClientArea = 0x40,
	AboveClientArea = 0x100,
	StateImage = 0x200
}
