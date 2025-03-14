using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class NavigateEventArgs : EventArgs
{
	private bool forward;

	public bool Forward => forward;

	public NavigateEventArgs(bool isForward)
	{
		forward = isForward;
	}
}
