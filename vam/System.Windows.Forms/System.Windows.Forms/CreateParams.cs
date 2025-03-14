namespace System.Windows.Forms;

public class CreateParams
{
	private string caption;

	private string class_name;

	private int class_style;

	private int ex_style;

	private int x;

	private int y;

	private int height;

	private int width;

	private int style;

	private object param;

	private IntPtr parent;

	internal Menu menu;

	internal Control control;

	public string Caption
	{
		get
		{
			return caption;
		}
		set
		{
			caption = value;
		}
	}

	public string ClassName
	{
		get
		{
			return class_name;
		}
		set
		{
			class_name = value;
		}
	}

	public int ClassStyle
	{
		get
		{
			return class_style;
		}
		set
		{
			class_style = value;
		}
	}

	public int ExStyle
	{
		get
		{
			return ex_style;
		}
		set
		{
			ex_style = value;
		}
	}

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
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

	public int Style
	{
		get
		{
			return style;
		}
		set
		{
			style = value;
		}
	}

	public object Param
	{
		get
		{
			return param;
		}
		set
		{
			param = value;
		}
	}

	public IntPtr Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
		}
	}

	internal bool HasWindowManager
	{
		get
		{
			if (control == null)
			{
				return false;
			}
			if (!(control is Form form))
			{
				return false;
			}
			return form.window_manager != null;
		}
	}

	internal WindowExStyles WindowExStyle
	{
		get
		{
			return (WindowExStyles)ex_style;
		}
		set
		{
			ex_style = (int)value;
		}
	}

	internal WindowStyles WindowStyle
	{
		get
		{
			return (WindowStyles)style;
		}
		set
		{
			style = (int)value;
		}
	}

	internal bool IsSet(WindowStyles Style)
	{
		return ((uint)style & (uint)Style) == (uint)Style;
	}

	internal bool IsSet(WindowExStyles ExStyle)
	{
		return ((uint)ex_style & (uint)ExStyle) == (uint)ExStyle;
	}

	internal static bool IsSet(WindowExStyles ExStyle, WindowExStyles Option)
	{
		return (Option & ExStyle) == Option;
	}

	internal static bool IsSet(WindowStyles Style, WindowStyles Option)
	{
		return (Option & Style) == Option;
	}

	public override string ToString()
	{
		return $"CreateParams {{'{class_name}', '{caption}', 0x{class_style:X}, 0x{ex_style:X}, {{{x}, {y}, {width}, {height}}}}}";
	}
}
