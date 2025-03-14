using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Drawing;

[Serializable]
[ComVisible(true)]
[TypeConverter(typeof(RectangleConverter))]
public struct Rectangle
{
	private int x;

	private int y;

	private int width;

	private int height;

	public static readonly Rectangle Empty;

	[Browsable(false)]
	public int Bottom => y + height;

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			height = value;
		}
	}

	[Browsable(false)]
	public bool IsEmpty => x == 0 && y == 0 && width == 0 && height == 0;

	[Browsable(false)]
	public int Left => X;

	[Browsable(false)]
	public Point Location
	{
		get
		{
			return new Point(x, y);
		}
		set
		{
			x = value.X;
			y = value.Y;
		}
	}

	[Browsable(false)]
	public int Right => X + Width;

	[Browsable(false)]
	public Size Size
	{
		get
		{
			return new Size(Width, Height);
		}
		set
		{
			Width = value.Width;
			Height = value.Height;
		}
	}

	[Browsable(false)]
	public int Top => y;

	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			width = value;
		}
	}

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public Rectangle(Point location, Size size)
	{
		x = location.X;
		y = location.Y;
		width = size.Width;
		height = size.Height;
	}

	public Rectangle(int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public static Rectangle Ceiling(RectangleF value)
	{
		checked
		{
			int num = (int)Math.Ceiling(value.X);
			int num2 = (int)Math.Ceiling(value.Y);
			int num3 = (int)Math.Ceiling(value.Width);
			int num4 = (int)Math.Ceiling(value.Height);
			return new Rectangle(num, num2, num3, num4);
		}
	}

	public static Rectangle FromLTRB(int left, int top, int right, int bottom)
	{
		return new Rectangle(left, top, right - left, bottom - top);
	}

	public static Rectangle Inflate(Rectangle rect, int x, int y)
	{
		Rectangle result = new Rectangle(rect.Location, rect.Size);
		result.Inflate(x, y);
		return result;
	}

	public void Inflate(int width, int height)
	{
		Inflate(new Size(width, height));
	}

	public void Inflate(Size size)
	{
		x -= size.Width;
		y -= size.Height;
		Width += size.Width * 2;
		Height += size.Height * 2;
	}

	public static Rectangle Intersect(Rectangle a, Rectangle b)
	{
		if (!a.IntersectsWithInclusive(b))
		{
			return Empty;
		}
		return FromLTRB(Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));
	}

	public void Intersect(Rectangle rect)
	{
		this = Intersect(this, rect);
	}

	public static Rectangle Round(RectangleF value)
	{
		checked
		{
			int num = (int)Math.Round(value.X);
			int num2 = (int)Math.Round(value.Y);
			int num3 = (int)Math.Round(value.Width);
			int num4 = (int)Math.Round(value.Height);
			return new Rectangle(num, num2, num3, num4);
		}
	}

	public static Rectangle Truncate(RectangleF value)
	{
		checked
		{
			int num = (int)value.X;
			int num2 = (int)value.Y;
			int num3 = (int)value.Width;
			int num4 = (int)value.Height;
			return new Rectangle(num, num2, num3, num4);
		}
	}

	public static Rectangle Union(Rectangle a, Rectangle b)
	{
		return FromLTRB(Math.Min(a.Left, b.Left), Math.Min(a.Top, b.Top), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
	}

	public bool Contains(int x, int y)
	{
		return x >= Left && x < Right && y >= Top && y < Bottom;
	}

	public bool Contains(Point pt)
	{
		return Contains(pt.X, pt.Y);
	}

	public bool Contains(Rectangle rect)
	{
		return rect == Intersect(this, rect);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Rectangle))
		{
			return false;
		}
		return this == (Rectangle)obj;
	}

	public override int GetHashCode()
	{
		return (height + width) ^ (x + y);
	}

	public bool IntersectsWith(Rectangle rect)
	{
		return Left < rect.Right && Right > rect.Left && Top < rect.Bottom && Bottom > rect.Top;
	}

	private bool IntersectsWithInclusive(Rectangle r)
	{
		return Left <= r.Right && Right >= r.Left && Top <= r.Bottom && Bottom >= r.Top;
	}

	public void Offset(int x, int y)
	{
		this.x += x;
		this.y += y;
	}

	public void Offset(Point pos)
	{
		x += pos.X;
		y += pos.Y;
	}

	public override string ToString()
	{
		return $"{{X={x},Y={y},Width={width},Height={height}}}";
	}

	public static bool operator ==(Rectangle left, Rectangle right)
	{
		return left.Location == right.Location && left.Size == right.Size;
	}

	public static bool operator !=(Rectangle left, Rectangle right)
	{
		return left.Location != right.Location || left.Size != right.Size;
	}
}
