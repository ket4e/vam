using System.Drawing;

namespace System.Windows.Forms;

public sealed class AmbientProperties
{
	private Color fore_color;

	private Color back_color;

	private Font font;

	private Cursor cursor;

	public Color BackColor
	{
		get
		{
			return back_color;
		}
		set
		{
			back_color = value;
		}
	}

	public Cursor Cursor
	{
		get
		{
			return cursor;
		}
		set
		{
			cursor = value;
		}
	}

	public Font Font
	{
		get
		{
			return font;
		}
		set
		{
			font = value;
		}
	}

	public Color ForeColor
	{
		get
		{
			return fore_color;
		}
		set
		{
			fore_color = value;
		}
	}
}
