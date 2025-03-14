using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class MouseEventArgs : EventArgs
{
	private MouseButtons buttons;

	private int clicks;

	private int delta;

	private int x;

	private int y;

	public MouseButtons Button => buttons;

	public int Clicks => clicks;

	public int Delta => delta;

	public int X => x;

	public int Y => y;

	public Point Location => new Point(x, y);

	public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
	{
		buttons = button;
		this.clicks = clicks;
		this.delta = delta;
		this.x = x;
		this.y = y;
	}
}
