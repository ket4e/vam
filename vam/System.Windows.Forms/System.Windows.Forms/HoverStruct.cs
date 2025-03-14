using System.Drawing;

namespace System.Windows.Forms;

internal struct HoverStruct
{
	internal Timer Timer;

	internal IntPtr Window;

	internal int X;

	internal int Y;

	internal Size Size;

	internal int Interval;

	internal IntPtr Atom;
}
