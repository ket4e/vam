namespace System.Windows.Forms;

[Flags]
public enum RichTextBoxFinds
{
	None = 0,
	WholeWord = 2,
	MatchCase = 4,
	NoHighlight = 8,
	Reverse = 0x10
}
