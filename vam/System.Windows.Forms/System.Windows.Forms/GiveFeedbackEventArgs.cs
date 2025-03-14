using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class GiveFeedbackEventArgs : EventArgs
{
	internal DragDropEffects effect;

	internal bool use_default_cursors;

	public DragDropEffects Effect => effect;

	public bool UseDefaultCursors
	{
		get
		{
			return use_default_cursors;
		}
		set
		{
			use_default_cursors = value;
		}
	}

	public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors)
	{
		this.effect = effect;
		use_default_cursors = useDefaultCursors;
	}
}
