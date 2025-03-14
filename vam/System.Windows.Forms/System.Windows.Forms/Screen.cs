using System.Drawing;

namespace System.Windows.Forms;

public class Screen
{
	private static Screen[] all_screens = new Screen[1]
	{
		new Screen(primary: true, "Mono MWF Primary Display", SystemInformation.VirtualScreen, SystemInformation.WorkingArea)
	};

	private bool primary;

	private Rectangle bounds;

	private Rectangle workarea;

	private string name;

	private int bits_per_pixel;

	public static Screen[] AllScreens => all_screens;

	public static Screen PrimaryScreen => all_screens[0];

	[System.MonoTODO("Stub, always returns 32")]
	public int BitsPerPixel => bits_per_pixel;

	public Rectangle Bounds => bounds;

	public string DeviceName => name;

	public bool Primary => primary;

	public Rectangle WorkingArea => workarea;

	private Screen()
	{
		primary = true;
		bounds = SystemInformation.WorkingArea;
	}

	private Screen(bool primary, string name, Rectangle bounds, Rectangle workarea)
	{
		this.primary = primary;
		this.name = name;
		this.bounds = bounds;
		this.workarea = workarea;
		bits_per_pixel = 32;
	}

	public static Screen FromControl(Control control)
	{
		return FromPoint(control.Location);
	}

	public static Screen FromHandle(IntPtr hwnd)
	{
		Control control = Control.FromHandle(hwnd);
		if (control != null)
		{
			return FromPoint(control.Location);
		}
		return PrimaryScreen;
	}

	public static Screen FromPoint(Point point)
	{
		for (int i = 0; i < all_screens.Length; i++)
		{
			if (all_screens[i].Bounds.Contains(point))
			{
				return all_screens[i];
			}
		}
		return PrimaryScreen;
	}

	public static Screen FromRectangle(Rectangle rect)
	{
		return FromPoint(new Point(rect.Left, rect.Top));
	}

	public static Rectangle GetBounds(Control ctl)
	{
		return FromControl(ctl).Bounds;
	}

	public static Rectangle GetBounds(Point pt)
	{
		return FromPoint(pt).Bounds;
	}

	public static Rectangle GetBounds(Rectangle rect)
	{
		return FromRectangle(rect).Bounds;
	}

	public static Rectangle GetWorkingArea(Control ctl)
	{
		return FromControl(ctl).WorkingArea;
	}

	public static Rectangle GetWorkingArea(Point pt)
	{
		return FromPoint(pt).WorkingArea;
	}

	public static Rectangle GetWorkingArea(Rectangle rect)
	{
		return FromRectangle(rect).WorkingArea;
	}

	public override bool Equals(object obj)
	{
		if (obj is Screen)
		{
			Screen screen = (Screen)obj;
			if (name.Equals(screen.name) && primary == screen.primary && bounds.Equals(screen.Bounds) && workarea.Equals(screen.workarea))
			{
				return true;
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override string ToString()
	{
		return string.Concat("Screen[Bounds={", Bounds, "} WorkingArea={", WorkingArea, "} Primary={", Primary, "} DeviceName=", DeviceName);
	}
}
