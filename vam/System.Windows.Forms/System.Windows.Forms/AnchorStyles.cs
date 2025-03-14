using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms;

[Flags]
[Editor("System.Windows.Forms.Design.AnchorEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public enum AnchorStyles
{
	None = 0,
	Top = 1,
	Bottom = 2,
	Left = 4,
	Right = 8
}
