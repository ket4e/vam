namespace System.Drawing.Drawing2D;

public sealed class RegionData
{
	private byte[] data;

	public byte[] Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}

	internal RegionData()
	{
	}
}
