namespace System.Windows.Forms;

public class StatusBarPanelClickEventArgs : MouseEventArgs
{
	private StatusBarPanel panel;

	public StatusBarPanel StatusBarPanel => panel;

	public StatusBarPanelClickEventArgs(StatusBarPanel statusBarPanel, MouseButtons button, int clicks, int x, int y)
		: base(button, clicks, x, y, 0)
	{
		panel = statusBarPanel;
	}
}
