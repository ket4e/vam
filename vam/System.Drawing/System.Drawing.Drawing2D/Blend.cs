namespace System.Drawing.Drawing2D;

public sealed class Blend
{
	private float[] positions;

	private float[] factors;

	public float[] Factors
	{
		get
		{
			return factors;
		}
		set
		{
			factors = value;
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

	public Blend()
	{
		positions = new float[1];
		factors = new float[1];
	}

	public Blend(int count)
	{
		positions = new float[count];
		factors = new float[count];
	}
}
