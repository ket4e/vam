namespace System.Windows.Forms.RTF;

internal class Font
{
	private string name;

	private string alt_name;

	private int num;

	private int family;

	private CharsetType charset;

	private int pitch;

	private int type;

	private int codepage;

	private Font next;

	private RTF rtf;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public string AltName
	{
		get
		{
			return alt_name;
		}
		set
		{
			alt_name = value;
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
			DeleteFont(rtf, value);
			num = value;
		}
	}

	public int Family
	{
		get
		{
			return family;
		}
		set
		{
			family = value;
		}
	}

	public CharsetType Charset
	{
		get
		{
			return charset;
		}
		set
		{
			charset = value;
		}
	}

	public int Pitch
	{
		get
		{
			return pitch;
		}
		set
		{
			pitch = value;
		}
	}

	public int Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public int Codepage
	{
		get
		{
			return codepage;
		}
		set
		{
			codepage = value;
		}
	}

	public Font(RTF rtf)
	{
		this.rtf = rtf;
		num = -1;
		name = string.Empty;
		lock (rtf)
		{
			if (rtf.Fonts == null)
			{
				rtf.Fonts = this;
				return;
			}
			Font fonts = rtf.Fonts;
			while (fonts.next != null)
			{
				fonts = fonts.next;
			}
			fonts.next = this;
		}
	}

	public static bool DeleteFont(RTF rtf, int font_number)
	{
		lock (rtf)
		{
			Font fonts = rtf.Fonts;
			Font font = null;
			while (fonts != null && fonts.num != font_number)
			{
				font = fonts;
				fonts = fonts.next;
			}
			if (fonts != null)
			{
				if (fonts == rtf.Fonts)
				{
					rtf.Fonts = fonts.next;
				}
				else if (font != null)
				{
					font.next = fonts.next;
				}
				else
				{
					rtf.Fonts = fonts.next;
				}
				return true;
			}
		}
		return false;
	}

	public static Font GetFont(RTF rtf, int font_number)
	{
		lock (rtf)
		{
			return GetFont(rtf.Fonts, font_number);
		}
	}

	public static Font GetFont(Font start, int font_number)
	{
		if (font_number == -1)
		{
			return start;
		}
		Font font = start;
		while (font != null && font.num != font_number)
		{
			font = font.next;
		}
		return font;
	}
}
