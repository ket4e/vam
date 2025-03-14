namespace System.Windows.Forms;

[Flags]
public enum DataGridViewPaintParts
{
	None = 0,
	Background = 1,
	Border = 2,
	ContentBackground = 4,
	ContentForeground = 8,
	ErrorIcon = 0x10,
	Focus = 0x20,
	SelectionBackground = 0x40,
	All = 0x7F
}
