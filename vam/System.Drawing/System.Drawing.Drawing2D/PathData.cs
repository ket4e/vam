namespace System.Drawing.Drawing2D;

public sealed class PathData
{
	private PointF[] points;

	private byte[] types;

	public PointF[] Points
	{
		get
		{
			return points;
		}
		set
		{
			points = value;
		}
	}

	public byte[] Types
	{
		get
		{
			return types;
		}
		set
		{
			types = value;
		}
	}
}
