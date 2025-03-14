namespace System.Drawing.Imaging;

public sealed class ColorMap
{
	private Color newColor;

	private Color oldColor;

	public Color NewColor
	{
		get
		{
			return newColor;
		}
		set
		{
			newColor = value;
		}
	}

	public Color OldColor
	{
		get
		{
			return oldColor;
		}
		set
		{
			oldColor = value;
		}
	}
}
