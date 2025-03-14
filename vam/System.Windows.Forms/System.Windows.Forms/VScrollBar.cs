using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class VScrollBar : ScrollBar
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override RightToLeft RightToLeft
	{
		get
		{
			return base.RightToLeft;
		}
		set
		{
			if (RightToLeft != value)
			{
				base.RightToLeft = value;
				OnRightToLeftChanged(EventArgs.Empty);
			}
		}
	}

	protected override Size DefaultSize => ThemeEngine.Current.VScrollBarDefaultSize;

	protected override CreateParams CreateParams => base.CreateParams;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler RightToLeftChanged
	{
		add
		{
			base.RightToLeftChanged += value;
		}
		remove
		{
			base.RightToLeftChanged -= value;
		}
	}

	public VScrollBar()
	{
		vert = true;
	}
}
