using System.Drawing;

namespace System.Windows.Forms;

public class StatusBarDrawItemEventArgs : DrawItemEventArgs
{
	private StatusBarPanel panel;

	public StatusBarPanel Panel => panel;

	public StatusBarDrawItemEventArgs(Graphics g, Font font, Rectangle r, int itemId, DrawItemState itemState, StatusBarPanel panel)
		: this(g, font, r, itemId, itemState, panel, Control.DefaultForeColor, Control.DefaultBackColor)
	{
	}

	public StatusBarDrawItemEventArgs(Graphics g, Font font, Rectangle r, int itemId, DrawItemState itemState, StatusBarPanel panel, Color foreColor, Color backColor)
		: base(g, font, r, itemId, itemState)
	{
		this.panel = panel;
	}
}
