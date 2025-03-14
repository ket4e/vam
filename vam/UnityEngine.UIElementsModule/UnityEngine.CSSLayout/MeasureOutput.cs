namespace UnityEngine.CSSLayout;

internal class MeasureOutput
{
	public static long Make(double width, double height)
	{
		return Make((int)width, (int)height);
	}

	public static long Make(int width, int height)
	{
		return ((long)width << 32) | (uint)height;
	}

	public static int GetWidth(long measureOutput)
	{
		return (int)(0xFFFFFFFFu & (measureOutput >> 32));
	}

	public static int GetHeight(long measureOutput)
	{
		return (int)(0xFFFFFFFFu & measureOutput);
	}
}
