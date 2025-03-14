namespace System.Windows.Forms.RTF;

internal class StyleElement
{
	private TokenClass token_class;

	private Major major;

	private Minor minor;

	private int param;

	private string text;

	private StyleElement next;

	public TokenClass TokenClass
	{
		get
		{
			return token_class;
		}
		set
		{
			token_class = value;
		}
	}

	public Major Major
	{
		get
		{
			return major;
		}
		set
		{
			major = value;
		}
	}

	public Minor Minor
	{
		get
		{
			return minor;
		}
		set
		{
			minor = value;
		}
	}

	public int Param
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

	public StyleElement(Style s, TokenClass token_class, Major major, Minor minor, int param, string text)
	{
		this.token_class = token_class;
		this.major = major;
		this.minor = minor;
		this.param = param;
		this.text = text;
		lock (s)
		{
			if (s.Elements == null)
			{
				s.Elements = this;
				return;
			}
			StyleElement elements = s.Elements;
			while (elements.next != null)
			{
				elements = elements.next;
			}
			elements.next = this;
		}
	}
}
