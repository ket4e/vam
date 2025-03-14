using System.Drawing;

namespace System.Windows.Forms.Theming.Default;

internal class LabelPainter
{
	public virtual Size DefaultSize => new Size(100, 23);

	public virtual void Draw(Graphics dc, Rectangle client_rectangle, Label label)
	{
		Rectangle paddingClientRectangle = label.PaddingClientRectangle;
		label.DrawImage(dc, label.Image, paddingClientRectangle, label.ImageAlign);
		if (label.Enabled)
		{
			dc.DrawString(label.Text, label.Font, ThemeEngine.Current.ResPool.GetSolidBrush(label.ForeColor), paddingClientRectangle, label.string_format);
		}
		else
		{
			ControlPaint.DrawStringDisabled(dc, label.Text, label.Font, label.BackColor, paddingClientRectangle, label.string_format);
		}
	}
}
