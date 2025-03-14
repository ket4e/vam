using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[Serializable]
[TypeConverter(typeof(PaddingConverter))]
public struct Padding
{
	private int _bottom;

	private int _left;

	private int _right;

	private int _top;

	private bool _all;

	public static readonly Padding Empty = new Padding(0);

	[RefreshProperties(RefreshProperties.All)]
	public int All
	{
		get
		{
			if (!_all)
			{
				return -1;
			}
			return _top;
		}
		set
		{
			_all = true;
			_left = (_top = (_right = (_bottom = value)));
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	public int Bottom
	{
		get
		{
			return _bottom;
		}
		set
		{
			_bottom = value;
			_all = false;
		}
	}

	[Browsable(false)]
	public int Horizontal => _left + _right;

	[RefreshProperties(RefreshProperties.All)]
	public int Left
	{
		get
		{
			return _left;
		}
		set
		{
			_left = value;
			_all = false;
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	public int Right
	{
		get
		{
			return _right;
		}
		set
		{
			_right = value;
			_all = false;
		}
	}

	[Browsable(false)]
	public Size Size => new Size(Horizontal, Vertical);

	[RefreshProperties(RefreshProperties.All)]
	public int Top
	{
		get
		{
			return _top;
		}
		set
		{
			_top = value;
			_all = false;
		}
	}

	[Browsable(false)]
	public int Vertical => _top + _bottom;

	public Padding(int all)
	{
		_left = all;
		_right = all;
		_top = all;
		_bottom = all;
		_all = true;
	}

	public Padding(int left, int top, int right, int bottom)
	{
		_left = left;
		_right = right;
		_top = top;
		_bottom = bottom;
		_all = _left == _top && _left == _right && _left == _bottom;
	}

	public static Padding Add(Padding p1, Padding p2)
	{
		return p1 + p2;
	}

	public override bool Equals(object other)
	{
		if (other is Padding padding)
		{
			return _left == padding.Left && _top == padding.Top && _right == padding.Right && _bottom == padding.Bottom;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _top ^ _bottom ^ _left ^ _right;
	}

	public static Padding Subtract(Padding p1, Padding p2)
	{
		return p1 - p2;
	}

	public override string ToString()
	{
		return "{Left=" + Left + ",Top=" + Top + ",Right=" + Right + ",Bottom=" + Bottom + "}";
	}

	public static Padding operator +(Padding p1, Padding p2)
	{
		return new Padding(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);
	}

	public static bool operator ==(Padding p1, Padding p2)
	{
		return p1.Equals(p2);
	}

	public static bool operator !=(Padding p1, Padding p2)
	{
		return !p1.Equals(p2);
	}

	public static Padding operator -(Padding p1, Padding p2)
	{
		return new Padding(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);
	}
}
