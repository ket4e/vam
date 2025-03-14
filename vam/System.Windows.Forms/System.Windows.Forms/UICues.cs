namespace System.Windows.Forms;

[Flags]
public enum UICues
{
	None = 0,
	ShowFocus = 1,
	ShowKeyboard = 2,
	Shown = 3,
	ChangeFocus = 4,
	ChangeKeyboard = 8,
	Changed = 0xC
}
