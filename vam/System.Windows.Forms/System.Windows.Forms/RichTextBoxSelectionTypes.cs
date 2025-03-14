namespace System.Windows.Forms;

[Flags]
public enum RichTextBoxSelectionTypes
{
	Empty = 0,
	Text = 1,
	Object = 2,
	MultiChar = 4,
	MultiObject = 8
}
