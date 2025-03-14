namespace System.Windows.Forms.RTF;

internal class Color
{
	private int red;

	private int green;

	private int blue;

	private int num;

	private Color next;

	public int Red
	{
		get
		{
			return red;
		}
		set
		{
			red = value;
		}
	}

	public int Green
	{
		get
		{
			return green;
		}
		set
		{
			green = value;
		}
	}

	public int Blue
	{
		get
		{
			return blue;
		}
		set
		{
			blue = value;
		}
	}

	public int Num
	{
		get
		{
			return num;
		}
		set
		{
			num = value;
		}
	}

	public Color(RTF rtf)
	{
		red = -1;
		green = -1;
		blue = -1;
		num = -1;
		lock (rtf)
		{
			if (rtf.Colors == null)
			{
				rtf.Colors = this;
				return;
			}
			Color colors = rtf.Colors;
			while (colors.next != null)
			{
				colors = colors.next;
			}
			colors.next = this;
		}
	}

	public static Color GetColor(RTF rtf, int color_number)
	{
		lock (rtf)
		{
			return GetColor(rtf.Colors, color_number);
		}
	}

	private static Color GetColor(Color start, int color_number)
	{
		if (color_number == -1)
		{
			return start;
		}
		Color color = start;
		while (color != null && color.num != color_number)
		{
			color = color.next;
		}
		return color;
	}
}
