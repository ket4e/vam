using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class HScrollBar : ScrollBar
{
	protected override Size DefaultSize => ThemeEngine.Current.HScrollBarDefaultSize;

	protected override CreateParams CreateParams => base.CreateParams;

	public HScrollBar()
	{
		vert = false;
	}
}
