namespace System.Windows.Forms;

public class UpDownEventArgs : EventArgs
{
	private int button_id;

	public int ButtonID => button_id;

	public UpDownEventArgs(int buttonPushed)
	{
		button_id = buttonPushed;
	}
}
