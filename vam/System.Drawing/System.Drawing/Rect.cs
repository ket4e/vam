namespace System.Drawing;

internal struct Rect
{
	public CGPoint origin;

	public CGSize size;

	public Rect(float x, float y, float width, float height)
	{
		origin.x = x;
		origin.y = y;
		size.width = width;
		size.height = height;
	}
}
