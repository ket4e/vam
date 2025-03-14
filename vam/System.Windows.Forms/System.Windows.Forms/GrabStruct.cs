using System.Drawing;

namespace System.Windows.Forms;

internal struct GrabStruct
{
	internal bool Confined;

	internal IntPtr Hwnd;

	internal Rectangle Area;
}
