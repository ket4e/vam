namespace System.Windows.Forms;

public class VScrollProperties : ScrollProperties
{
	public VScrollProperties(ScrollableControl container)
		: base(container)
	{
		scroll_bar = container.vscrollbar;
	}
}
