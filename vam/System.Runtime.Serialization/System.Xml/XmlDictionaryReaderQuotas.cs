namespace System.Xml;

public sealed class XmlDictionaryReaderQuotas
{
	private static XmlDictionaryReaderQuotas max;

	private readonly bool is_readonly;

	private int array_len;

	private int bytes;

	private int depth;

	private int nt_chars;

	private int text_len;

	public static XmlDictionaryReaderQuotas Max => max;

	public int MaxArrayLength
	{
		get
		{
			return array_len;
		}
		set
		{
			array_len = Check(value);
		}
	}

	public int MaxBytesPerRead
	{
		get
		{
			return bytes;
		}
		set
		{
			bytes = Check(value);
		}
	}

	public int MaxDepth
	{
		get
		{
			return depth;
		}
		set
		{
			depth = Check(value);
		}
	}

	public int MaxNameTableCharCount
	{
		get
		{
			return nt_chars;
		}
		set
		{
			nt_chars = Check(value);
		}
	}

	public int MaxStringContentLength
	{
		get
		{
			return text_len;
		}
		set
		{
			text_len = Check(value);
		}
	}

	public XmlDictionaryReaderQuotas()
		: this(max: false)
	{
	}

	private XmlDictionaryReaderQuotas(bool max)
	{
		is_readonly = max;
		array_len = ((!max) ? 16384 : int.MaxValue);
		bytes = ((!max) ? 4096 : int.MaxValue);
		depth = ((!max) ? 32 : int.MaxValue);
		nt_chars = ((!max) ? 16384 : int.MaxValue);
		text_len = ((!max) ? 8192 : int.MaxValue);
	}

	static XmlDictionaryReaderQuotas()
	{
		max = new XmlDictionaryReaderQuotas(max: true);
	}

	private int Check(int value)
	{
		if (is_readonly)
		{
			throw new InvalidOperationException("This quota is read-only.");
		}
		if (value <= 0)
		{
			throw new ArgumentException("Value must be positive integer.");
		}
		return value;
	}

	public void CopyTo(XmlDictionaryReaderQuotas quota)
	{
		quota.array_len = array_len;
		quota.bytes = bytes;
		quota.depth = depth;
		quota.nt_chars = nt_chars;
		quota.text_len = text_len;
	}
}
