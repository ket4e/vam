namespace System.Drawing.Printing;

[Serializable]
public class PaperSize
{
	private string name;

	private int width;

	private int height;

	private PaperKind kind;

	internal bool is_default;

	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			if (kind != 0)
			{
				throw new ArgumentException();
			}
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
			if (kind != 0)
			{
				throw new ArgumentException();
			}
			height = value;
		}
	}

	public string PaperName
	{
		get
		{
			return name;
		}
		set
		{
			if (kind != 0)
			{
				throw new ArgumentException();
			}
			name = value;
		}
	}

	public PaperKind Kind
	{
		get
		{
			if (kind > PaperKind.PrcEnvelopeNumber10Rotated)
			{
				return PaperKind.Custom;
			}
			return kind;
		}
	}

	public int RawKind
	{
		get
		{
			return (int)kind;
		}
		set
		{
			kind = (PaperKind)value;
		}
	}

	internal bool IsDefault
	{
		get
		{
			return is_default;
		}
		set
		{
			is_default = value;
		}
	}

	public PaperSize()
	{
	}

	public PaperSize(string name, int width, int height)
	{
		this.width = width;
		this.height = height;
		this.name = name;
	}

	internal PaperSize(string name, int width, int height, PaperKind kind, bool isDefault)
	{
		this.width = width;
		this.height = height;
		this.name = name;
		is_default = isDefault;
	}

	internal void SetKind(PaperKind k)
	{
		kind = k;
	}

	public override string ToString()
	{
		string format = "[PaperSize {0} Kind={1} Height={2} Width={3}]";
		return string.Format(format, PaperName, Kind, Height, Width);
	}
}
