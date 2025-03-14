namespace System.Drawing.Printing;

[Serializable]
public class PaperSource
{
	private PaperSourceKind kind;

	private string source_name;

	internal bool is_default;

	public PaperSourceKind Kind
	{
		get
		{
			if (kind >= (PaperSourceKind)256)
			{
				return PaperSourceKind.Custom;
			}
			return kind;
		}
	}

	public string SourceName
	{
		get
		{
			return source_name;
		}
		set
		{
			source_name = value;
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
			kind = (PaperSourceKind)value;
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

	public PaperSource()
	{
	}

	internal PaperSource(string sourceName, PaperSourceKind kind)
	{
		source_name = sourceName;
		this.kind = kind;
	}

	internal PaperSource(string sourceName, PaperSourceKind kind, bool isDefault)
	{
		source_name = sourceName;
		this.kind = kind;
		is_default = IsDefault;
	}

	public override string ToString()
	{
		string format = "[PaperSource {0} Kind={1}]";
		return string.Format(format, SourceName, Kind);
	}
}
