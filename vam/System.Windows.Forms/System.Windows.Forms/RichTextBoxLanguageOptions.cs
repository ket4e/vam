namespace System.Windows.Forms;

[Flags]
public enum RichTextBoxLanguageOptions
{
	AutoKeyboard = 1,
	AutoFont = 2,
	ImeCancelComplete = 4,
	ImeAlwaysSendNotify = 8,
	AutoFontSizeAdjust = 0x10,
	UIFonts = 0x20,
	DualFont = 0x80
}
