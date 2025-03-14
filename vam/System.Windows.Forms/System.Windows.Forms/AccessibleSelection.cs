namespace System.Windows.Forms;

[Flags]
public enum AccessibleSelection
{
	None = 0,
	TakeFocus = 1,
	TakeSelection = 2,
	ExtendSelection = 4,
	AddSelection = 8,
	RemoveSelection = 0x10
}
