namespace System.Windows.Forms.RTF;

internal class Charset
{
	private CharsetType id;

	private CharsetFlags flags;

	private Charcode code;

	private string file;

	public Charcode Code
	{
		get
		{
			return code;
		}
		set
		{
			code = value;
		}
	}

	public CharsetFlags Flags
	{
		get
		{
			return flags;
		}
		set
		{
			flags = value;
		}
	}

	public CharsetType ID
	{
		get
		{
			return id;
		}
		set
		{
			if (value != 0 && value == CharsetType.Symbol)
			{
				id = CharsetType.Symbol;
			}
			else
			{
				id = CharsetType.General;
			}
		}
	}

	public string File
	{
		get
		{
			return file;
		}
		set
		{
			if (file != value)
			{
				file = value;
			}
		}
	}

	public StandardCharCode this[int c] => code[c];

	public Charset()
	{
		flags = CharsetFlags.Read | CharsetFlags.Switch;
		id = CharsetType.General;
		file = string.Empty;
		ReadMap();
	}

	public bool ReadMap()
	{
		switch (id)
		{
		case CharsetType.General:
			if (file == string.Empty)
			{
				code = Charcode.AnsiGeneric;
				return true;
			}
			return true;
		case CharsetType.Symbol:
			if (file == string.Empty)
			{
				code = Charcode.AnsiSymbol;
				return true;
			}
			return true;
		default:
			return false;
		}
	}

	public char StdCharCode(string name)
	{
		return ' ';
	}

	public string StdCharName(char code)
	{
		return string.Empty;
	}
}
