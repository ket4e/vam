using System.Drawing;

namespace System.Windows.Forms;

internal struct POINT
{
	internal int x;

	internal int y;

	internal POINT(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	internal Point ToPoint()
	{
		return new Point(x, y);
	}

	public override string ToString()
	{
		return "Point {" + x + ", " + y + "}";
	}
}
