using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public enum ImeMode
{
	NoControl = 0,
	On = 1,
	Off = 2,
	Disable = 3,
	Hiragana = 4,
	Katakana = 5,
	KatakanaHalf = 6,
	AlphaFull = 7,
	Alpha = 8,
	HangulFull = 9,
	Hangul = 10,
	Inherit = -1,
	Close = 11,
	OnHalf = 12
}
