namespace System.Windows.Forms.VisualStyles;

public struct TextMetrics
{
	private int ascent;

	private int average_char_width;

	private char break_char;

	private TextMetricsCharacterSet char_set;

	private char default_char;

	private int descent;

	private int digitized_aspect_x;

	private int digitized_aspect_y;

	private int external_leading;

	private char first_char;

	private int height;

	private int internal_leading;

	private bool italic;

	private char last_char;

	private int max_char_width;

	private int overhang;

	private TextMetricsPitchAndFamilyValues pitch_and_family;

	private bool struck_out;

	private bool underlined;

	private int weight;

	public int Ascent
	{
		get
		{
			return ascent;
		}
		set
		{
			ascent = value;
		}
	}

	public int AverageCharWidth
	{
		get
		{
			return average_char_width;
		}
		set
		{
			average_char_width = value;
		}
	}

	public char BreakChar
	{
		get
		{
			return break_char;
		}
		set
		{
			break_char = value;
		}
	}

	public TextMetricsCharacterSet CharSet
	{
		get
		{
			return char_set;
		}
		set
		{
			char_set = value;
		}
	}

	public char DefaultChar
	{
		get
		{
			return default_char;
		}
		set
		{
			default_char = value;
		}
	}

	public int Descent
	{
		get
		{
			return descent;
		}
		set
		{
			descent = value;
		}
	}

	public int DigitizedAspectX
	{
		get
		{
			return digitized_aspect_x;
		}
		set
		{
			digitized_aspect_x = value;
		}
	}

	public int DigitizedAspectY
	{
		get
		{
			return digitized_aspect_y;
		}
		set
		{
			digitized_aspect_y = value;
		}
	}

	public int ExternalLeading
	{
		get
		{
			return external_leading;
		}
		set
		{
			external_leading = value;
		}
	}

	public char FirstChar
	{
		get
		{
			return first_char;
		}
		set
		{
			first_char = value;
		}
	}

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	public int InternalLeading
	{
		get
		{
			return internal_leading;
		}
		set
		{
			internal_leading = value;
		}
	}

	public bool Italic
	{
		get
		{
			return italic;
		}
		set
		{
			italic = value;
		}
	}

	public char LastChar
	{
		get
		{
			return last_char;
		}
		set
		{
			last_char = value;
		}
	}

	public int MaxCharWidth
	{
		get
		{
			return max_char_width;
		}
		set
		{
			max_char_width = value;
		}
	}

	public int Overhang
	{
		get
		{
			return overhang;
		}
		set
		{
			overhang = value;
		}
	}

	public TextMetricsPitchAndFamilyValues PitchAndFamily
	{
		get
		{
			return pitch_and_family;
		}
		set
		{
			pitch_and_family = value;
		}
	}

	public bool StruckOut
	{
		get
		{
			return struck_out;
		}
		set
		{
			struck_out = value;
		}
	}

	public bool Underlined
	{
		get
		{
			return underlined;
		}
		set
		{
			underlined = value;
		}
	}

	public int Weight
	{
		get
		{
			return weight;
		}
		set
		{
			weight = value;
		}
	}
}
