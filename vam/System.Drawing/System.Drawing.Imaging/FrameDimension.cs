namespace System.Drawing.Imaging;

public sealed class FrameDimension
{
	private Guid guid;

	private string name;

	private static FrameDimension page;

	private static FrameDimension resolution;

	private static FrameDimension time;

	public Guid Guid => guid;

	public static FrameDimension Page
	{
		get
		{
			if (page == null)
			{
				page = new FrameDimension(new Guid("7462dc86-6180-4c7e-8e3f-ee7333a7a483"), "Page");
			}
			return page;
		}
	}

	public static FrameDimension Resolution
	{
		get
		{
			if (resolution == null)
			{
				resolution = new FrameDimension(new Guid("84236f7b-3bd3-428f-8dab-4ea1439ca315"), "Resolution");
			}
			return resolution;
		}
	}

	public static FrameDimension Time
	{
		get
		{
			if (time == null)
			{
				time = new FrameDimension(new Guid("6aedbd6d-3fb5-418a-83a6-7f45229dc872"), "Time");
			}
			return time;
		}
	}

	public FrameDimension(Guid guid)
	{
		this.guid = guid;
	}

	internal FrameDimension(Guid guid, string name)
	{
		this.guid = guid;
		this.name = name;
	}

	public override bool Equals(object o)
	{
		if (!(o is FrameDimension frameDimension))
		{
			return false;
		}
		return guid == frameDimension.guid;
	}

	public override int GetHashCode()
	{
		return guid.GetHashCode();
	}

	public override string ToString()
	{
		if (name == null)
		{
			name = $"[FrameDimension: {guid}]";
		}
		return name;
	}
}
