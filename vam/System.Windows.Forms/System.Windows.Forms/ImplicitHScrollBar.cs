namespace System.Windows.Forms;

internal class ImplicitHScrollBar : HScrollBar
{
	public ImplicitHScrollBar()
	{
		implicit_control = true;
		SetStyle(ControlStyles.Selectable, value: false);
	}
}
