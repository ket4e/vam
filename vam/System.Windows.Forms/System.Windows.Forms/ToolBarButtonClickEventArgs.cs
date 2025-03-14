namespace System.Windows.Forms;

public class ToolBarButtonClickEventArgs : EventArgs
{
	private ToolBarButton button;

	public ToolBarButton Button
	{
		get
		{
			return button;
		}
		set
		{
			button = value;
		}
	}

	public ToolBarButtonClickEventArgs(ToolBarButton button)
	{
		this.button = button;
	}
}
