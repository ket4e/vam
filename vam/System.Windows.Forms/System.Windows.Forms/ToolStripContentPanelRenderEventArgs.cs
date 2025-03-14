using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripContentPanelRenderEventArgs : EventArgs
{
	private Graphics graphics;

	private bool handled;

	private ToolStripContentPanel tool_strip_content_panel;

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

	public ToolStripContentPanel ToolStripContentPanel => tool_strip_content_panel;

	public ToolStripContentPanelRenderEventArgs(Graphics g, ToolStripContentPanel contentPanel)
	{
		graphics = g;
		tool_strip_content_panel = contentPanel;
		handled = false;
	}
}
