namespace System.Drawing.Imaging;

public sealed class PropertyItem
{
	private int id;

	private int len;

	private short type;

	private byte[] value;

	public int Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public int Len
	{
		get
		{
			return len;
		}
		set
		{
			len = value;
		}
	}

	public short Type
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

	public byte[] Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	internal PropertyItem()
	{
	}
}
