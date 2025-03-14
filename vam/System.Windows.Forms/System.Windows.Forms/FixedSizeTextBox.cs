namespace System.Windows.Forms;

internal class FixedSizeTextBox : TextBox
{
	public FixedSizeTextBox()
	{
		SetStyle(ControlStyles.FixedWidth, value: true);
		SetStyle(ControlStyles.FixedHeight, value: true);
	}

	public FixedSizeTextBox(bool fixed_horz, bool fixed_vert)
	{
		SetStyle(ControlStyles.FixedWidth, fixed_horz);
		SetStyle(ControlStyles.FixedHeight, fixed_vert);
	}
}
