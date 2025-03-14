using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
[Flags]
[Editor("System.Windows.Forms.Design.BorderSidesEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
public enum ToolStripStatusLabelBorderSides
{
	None = 0,
	Left = 1,
	Top = 2,
	Right = 4,
	Bottom = 8,
	All = 0xF
}
