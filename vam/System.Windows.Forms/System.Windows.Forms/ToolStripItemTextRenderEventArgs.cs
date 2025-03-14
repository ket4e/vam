using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripItemTextRenderEventArgs : ToolStripItemRenderEventArgs
{
	private string text;

	private Color text_color;

	private ToolStripTextDirection text_direction;

	private Font text_font;

	private TextFormatFlags text_format;

	private Rectangle text_rectangle;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public Color TextColor
	{
		get
		{
			return text_color;
		}
		set
		{
			text_color = value;
		}
	}

	public ToolStripTextDirection TextDirection
	{
		get
		{
			return text_direction;
		}
		set
		{
			text_direction = value;
		}
	}

	public Font TextFont
	{
		get
		{
			return text_font;
		}
		set
		{
			text_font = value;
		}
	}

	public TextFormatFlags TextFormat
	{
		get
		{
			return text_format;
		}
		set
		{
			text_format = value;
		}
	}

	public Rectangle TextRectangle
	{
		get
		{
			return text_rectangle;
		}
		set
		{
			text_rectangle = value;
		}
	}

	public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, ContentAlignment textAlign)
		: base(g, item)
	{
		this.text = text;
		text_rectangle = textRectangle;
		text_color = textColor;
		text_font = textFont;
		text_direction = item.TextDirection;
		switch (textAlign)
		{
		case ContentAlignment.BottomCenter:
			text_format = TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom;
			break;
		case ContentAlignment.BottomLeft:
			text_format = TextFormatFlags.Bottom;
			break;
		case ContentAlignment.BottomRight:
			text_format = TextFormatFlags.Right | TextFormatFlags.Bottom;
			break;
		case ContentAlignment.MiddleCenter:
			text_format = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
			break;
		default:
			text_format = TextFormatFlags.VerticalCenter;
			break;
		case ContentAlignment.MiddleRight:
			text_format = TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
			break;
		case ContentAlignment.TopCenter:
			text_format = TextFormatFlags.HorizontalCenter;
			break;
		case ContentAlignment.TopLeft:
			text_format = TextFormatFlags.Left;
			break;
		case ContentAlignment.TopRight:
			text_format = TextFormatFlags.Right;
			break;
		}
		if ((Application.KeyboardCapture == null || !ToolStripManager.ActivatedByKeyboard) && !SystemInformation.MenuAccessKeysUnderlined)
		{
			text_format |= TextFormatFlags.HidePrefix;
		}
	}

	public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format)
		: base(g, item)
	{
		this.text = text;
		text_rectangle = textRectangle;
		text_color = textColor;
		text_font = textFont;
		text_format = format;
		text_direction = ToolStripTextDirection.Horizontal;
	}
}
