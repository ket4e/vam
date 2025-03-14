using System.Drawing;

namespace System.Windows.Forms;

public class ContentsResizedEventArgs : EventArgs
{
	private Rectangle rect;

	public Rectangle NewRectangle => rect;

	public ContentsResizedEventArgs(Rectangle newRectangle)
	{
		rect = newRectangle;
	}
}
