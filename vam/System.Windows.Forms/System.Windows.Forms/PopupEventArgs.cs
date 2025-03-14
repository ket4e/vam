using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class PopupEventArgs : CancelEventArgs
{
	private Control associated_control;

	private IWin32Window associated_window;

	private bool is_balloon;

	private Size tool_tip_size;

	public Control AssociatedControl => associated_control;

	public IWin32Window AssociatedWindow => associated_window;

	public bool IsBalloon => is_balloon;

	public Size ToolTipSize
	{
		get
		{
			return tool_tip_size;
		}
		set
		{
			tool_tip_size = value;
		}
	}

	public PopupEventArgs(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size)
	{
		associated_window = associatedWindow;
		associated_control = associatedControl;
		is_balloon = isBalloon;
		tool_tip_size = size;
	}
}
