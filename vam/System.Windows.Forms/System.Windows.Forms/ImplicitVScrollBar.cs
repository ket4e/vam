namespace System.Windows.Forms;

internal class ImplicitVScrollBar : VScrollBar
{
	public ImplicitVScrollBar()
	{
		implicit_control = true;
		SetStyle(ControlStyles.Selectable, value: false);
	}
}
