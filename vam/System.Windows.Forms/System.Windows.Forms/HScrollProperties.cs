namespace System.Windows.Forms;

public class HScrollProperties : ScrollProperties
{
	public HScrollProperties(ScrollableControl container)
		: base(container)
	{
		scroll_bar = container.hscrollbar;
	}
}
