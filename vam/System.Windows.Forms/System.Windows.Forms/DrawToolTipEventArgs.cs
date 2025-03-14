using System.Drawing;

namespace System.Windows.Forms;

public class DrawToolTipEventArgs : EventArgs
{
	private Control associated_control;

	private IWin32Window associated_window;

	private Color back_color;

	private Font font;

	private Rectangle bounds;

	private Color fore_color;

	private Graphics graphics;

	private string tooltip_text;

	public Control AssociatedControl => associated_control;

	public IWin32Window AssociatedWindow => associated_window;

	public Rectangle Bounds => bounds;

	public Font Font => font;

	public Graphics Graphics => graphics;

	public string ToolTipText => tooltip_text;

	public DrawToolTipEventArgs(Graphics graphics, IWin32Window associatedWindow, Control associatedControl, Rectangle bounds, string toolTipText, Color backColor, Color foreColor, Font font)
	{
		this.graphics = graphics;
		associated_window = associatedWindow;
		associated_control = associatedControl;
		this.bounds = bounds;
		tooltip_text = toolTipText;
		back_color = backColor;
		fore_color = foreColor;
		this.font = font;
	}

	public void DrawBackground()
	{
		graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(back_color), bounds);
	}

	public void DrawBorder()
	{
		ControlPaint.DrawBorder(graphics, bounds, SystemColors.WindowFrame, ButtonBorderStyle.Solid);
	}

	public void DrawText()
	{
		DrawText(TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.HidePrefix);
	}

	public void DrawText(TextFormatFlags flags)
	{
		TextRenderer.DrawTextInternal(graphics, tooltip_text, font, bounds, fore_color, flags, useDrawString: false);
	}
}
