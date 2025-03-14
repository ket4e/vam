using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal;

internal class PGTextBox : TextBox
{
	private bool _focusing;

	public void FocusAt(Point location)
	{
		_focusing = true;
		Point point = PointToClient(location);
		XplatUI.SendMessage(Handle, Msg.WM_LBUTTONDOWN, new IntPtr(1), Control.MakeParam(point.X, point.Y));
	}

	protected override bool IsInputKey(Keys keyData)
	{
		if ((keyData & Keys.Alt) != 0 && (keyData & Keys.KeyCode) == Keys.Down)
		{
			return true;
		}
		return base.IsInputKey(keyData);
	}

	protected override void WndProc(ref Message m)
	{
		if (_focusing && m.Msg == 512)
		{
			_focusing = false;
		}
		else
		{
			base.WndProc(ref m);
		}
	}
}
