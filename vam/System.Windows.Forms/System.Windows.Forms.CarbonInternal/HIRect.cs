namespace System.Windows.Forms.CarbonInternal;

internal struct HIRect
{
	public CGPoint origin;

	public CGSize size;

	public HIRect(int x, int y, int w, int h)
	{
		origin = new CGPoint(x, y);
		size = new CGSize(w, h);
	}
}
