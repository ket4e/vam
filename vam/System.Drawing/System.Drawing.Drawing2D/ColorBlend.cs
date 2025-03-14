namespace System.Drawing.Drawing2D;

public sealed class ColorBlend
{
	private float[] positions;

	private Color[] colors;

	public Color[] Colors
	{
		get
		{
			return colors;
		}
		set
		{
			colors = value;
		}
	}

	public float[] Positions
	{
		get
		{
			return positions;
		}
		set
		{
			positions = value;
		}
	}

	public ColorBlend()
	{
		positions = new float[1];
		colors = new Color[1];
	}

	public ColorBlend(int count)
	{
		positions = new float[count];
		colors = new Color[count];
	}
}
