using System.Drawing;

namespace System.Windows.Forms;

public class InvalidateEventArgs : EventArgs
{
	private Rectangle invalidated_rectangle;

	public Rectangle InvalidRect => invalidated_rectangle;

	public InvalidateEventArgs(Rectangle invalidRect)
	{
		invalidated_rectangle = invalidRect;
	}
}
