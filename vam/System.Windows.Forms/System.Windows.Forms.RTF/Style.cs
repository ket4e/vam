namespace System.Windows.Forms.RTF;

internal class Style
{
	public const int NoStyleNum = 222;

	public const int NormalStyleNum = 0;

	private string name;

	private StyleType type;

	private bool additive;

	private int num;

	private int based_on;

	private int next_par;

	private bool expanding;

	private StyleElement elements;

	private Style next;

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

	public StyleType Type
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

	public bool Additive
	{
		get
		{
			return additive;
		}
		set
		{
			additive = value;
		}
	}

	public int BasedOn
	{
		get
		{
			return based_on;
		}
		set
		{
			based_on = value;
		}
	}

	public StyleElement Elements
	{
		get
		{
			return elements;
		}
		set
		{
			elements = value;
		}
	}

	public bool Expanding
	{
		get
		{
			return expanding;
		}
		set
		{
			expanding = value;
		}
	}

	public int NextPar
	{
		get
		{
			return next_par;
		}
		set
		{
			next_par = value;
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

	public Style(RTF rtf)
	{
		num = -1;
		type = StyleType.Paragraph;
		based_on = 222;
		next_par = -1;
		lock (rtf)
		{
			if (rtf.Styles == null)
			{
				rtf.Styles = this;
				return;
			}
			Style styles = rtf.Styles;
			while (styles.next != null)
			{
				styles = styles.next;
			}
			styles.next = this;
		}
	}

	public void Expand(RTF rtf)
	{
		if (num != -1)
		{
			if (expanding)
			{
				throw new Exception("Recursive style expansion");
			}
			expanding = true;
			if (num != based_on)
			{
				rtf.SetToken(TokenClass.Control, Major.ParAttr, Minor.StyleNum, based_on, "\\s");
				rtf.RouteToken();
			}
			StyleElement styleElement = elements;
			while (styleElement != null)
			{
				rtf.TokenClass = styleElement.TokenClass;
				rtf.Major = styleElement.Major;
				rtf.Minor = styleElement.Minor;
				rtf.Param = styleElement.Param;
				rtf.Text = styleElement.Text;
				rtf.RouteToken();
			}
			expanding = false;
		}
	}

	public static Style GetStyle(RTF rtf, int style_number)
	{
		lock (rtf)
		{
			return GetStyle(rtf.Styles, style_number);
		}
	}

	public static Style GetStyle(Style start, int style_number)
	{
		if (style_number == -1)
		{
			return start;
		}
		Style style = start;
		while (style != null && style.num != style_number)
		{
			style = style.next;
		}
		return style;
	}
}
