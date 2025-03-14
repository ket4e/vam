using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripPanelRenderEventArgs : EventArgs
{
	private Graphics graphics;

	private bool handled;

	private ToolStripPanel tool_strip_panel;

	public Graphics Graphics => graphics;

	public bool Handled
	{
		get
		{
			return handled;
		}
		set
		{
			handled = value;
		}
	}

	public ToolStripPanel ToolStripPanel => tool_strip_panel;

	public ToolStripPanelRenderEventArgs(Graphics g, ToolStripPanel toolStripPanel)
	{
		graphics = g;
		tool_strip_panel = toolStripPanel;
		handled = false;
	}
}
