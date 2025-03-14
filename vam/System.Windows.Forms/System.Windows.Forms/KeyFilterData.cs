using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct KeyFilterData
{
	internal bool Down;

	internal int keycode;

	internal int keysym;

	internal Keys ModifierKeys;

	internal string str;
}
