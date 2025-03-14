namespace System.Windows.Forms;

internal class MouseWheelListBox : ListBox
{
	public void SendMouseWheelEvent(MouseEventArgs e)
	{
		OnMouseWheel(e);
	}
}
