using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

internal class Hwnd : IDisposable
{
	internal struct Borders
	{
		public int top;

		public int bottom;

		public int left;

		public int right;

		public void Inflate(Size size)
		{
			left += size.Width;
			right += size.Width;
			top += size.Height;
			bottom += size.Height;
		}

		public override string ToString()
		{
			return $"{{top={top}, bottom={bottom}, left={left}, right={right}}}";
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(Borders a, Borders b)
		{
			return a.left == b.left && a.right == b.right && a.top == b.top && a.bottom == b.bottom;
		}

		public static bool operator !=(Borders a, Borders b)
		{
			return !(a == b);
		}
	}

	private static Hashtable windows = new Hashtable(100, 0.5f);

	private IntPtr handle;

	internal IntPtr client_window;

	internal IntPtr whole_window;

	internal IntPtr cursor;

	internal Menu menu;

	internal TitleStyle title_style;

	internal FormBorderStyle border_style;

	internal bool border_static;

	internal int x;

	internal int y;

	internal int width;

	internal int height;

	internal bool allow_drop;

	internal Hwnd parent;

	internal bool visible;

	internal bool mapped;

	internal uint opacity;

	internal bool enabled;

	internal bool zero_sized;

	internal ArrayList invalid_list;

	internal Rectangle nc_invalid;

	internal bool expose_pending;

	internal bool nc_expose_pending;

	internal bool configure_pending;

	internal bool resizing_or_moving;

	internal bool reparented;

	internal Stack drawing_stack;

	internal object user_data;

	internal Rectangle client_rectangle;

	internal ArrayList marshal_free_list;

	internal int caption_height;

	internal int tool_caption_height;

	internal bool whacky_wm;

	internal bool fixed_size;

	internal bool zombie;

	internal Region user_clip;

	internal XEventQueue queue;

	internal WindowExStyles initial_ex_style;

	internal WindowStyles initial_style;

	internal FormWindowState cached_window_state = (FormWindowState)(-1);

	internal Point previous_child_startup_location = new Point(int.MinValue, int.MinValue);

	internal static Point previous_main_startup_location = new Point(int.MinValue, int.MinValue);

	internal ArrayList children;

	[ThreadStatic]
	private static Bitmap bmp;

	[ThreadStatic]
	private static Graphics bmp_g;

	internal object configure_lock = new object();

	internal object expose_lock = new object();

	public static Graphics GraphicsContext
	{
		get
		{
			if (bmp_g == null)
			{
				bmp = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
				bmp_g = Graphics.FromImage(bmp);
			}
			return bmp_g;
		}
	}

	public FormBorderStyle BorderStyle
	{
		get
		{
			return border_style;
		}
		set
		{
			border_style = value;
		}
	}

	public Rectangle ClientRect
	{
		get
		{
			if (client_rectangle == Rectangle.Empty)
			{
				return DefaultClientRect;
			}
			return client_rectangle;
		}
		set
		{
			client_rectangle = value;
		}
	}

	public IntPtr Cursor
	{
		get
		{
			return cursor;
		}
		set
		{
			cursor = value;
		}
	}

	public IntPtr ClientWindow
	{
		get
		{
			return client_window;
		}
		set
		{
			client_window = value;
			handle = value;
			zombie = false;
			if (!(client_window != IntPtr.Zero))
			{
				return;
			}
			lock (windows)
			{
				if (windows[client_window] == null)
				{
					windows[client_window] = this;
				}
			}
		}
	}

	public Region UserClip
	{
		get
		{
			return user_clip;
		}
		set
		{
			user_clip = value;
		}
	}

	public Rectangle DefaultClientRect
	{
		get
		{
			CreateParams createParams = new CreateParams();
			createParams.WindowStyle = initial_style;
			createParams.WindowExStyle = initial_ex_style;
			return GetClientRectangle(createParams, null, width, height);
		}
	}

	public bool ExposePending => expose_pending;

	public IntPtr Handle
	{
		get
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentNullException("Handle", "Handle is not yet assigned, need a ClientWindow");
			}
			return handle;
		}
	}

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

	public Menu Menu
	{
		get
		{
			return menu;
		}
		set
		{
			menu = value;
		}
	}

	public bool Reparented
	{
		get
		{
			return reparented;
		}
		set
		{
			reparented = value;
		}
	}

	public uint Opacity
	{
		get
		{
			return opacity;
		}
		set
		{
			opacity = value;
		}
	}

	public XEventQueue Queue
	{
		get
		{
			return queue;
		}
		set
		{
			queue = value;
		}
	}

	public bool Enabled
	{
		get
		{
			if (!enabled)
			{
				return false;
			}
			if (parent != null)
			{
				return parent.Enabled;
			}
			return true;
		}
		set
		{
			enabled = value;
		}
	}

	public IntPtr EnabledHwnd
	{
		get
		{
			if (Enabled || parent == null)
			{
				return Handle;
			}
			return parent.EnabledHwnd;
		}
	}

	public Point MenuOrigin
	{
		get
		{
			if (Control.FromHandle(handle) is Form form && form.window_manager != null)
			{
				return form.window_manager.GetMenuOrigin();
			}
			Size border3DSize = ThemeEngine.Current.Border3DSize;
			Point result = new Point(0, 0);
			if (border_style == FormBorderStyle.Fixed3D)
			{
				result.X += border3DSize.Width;
				result.Y += border3DSize.Height;
			}
			else if (border_style == FormBorderStyle.FixedSingle)
			{
				result.X++;
				result.Y++;
			}
			if (title_style == TitleStyle.Normal)
			{
				result.Y += caption_height;
			}
			else if (title_style == TitleStyle.Normal)
			{
				result.Y += tool_caption_height;
			}
			return result;
		}
	}

	public Rectangle Invalid
	{
		get
		{
			if (invalid_list.Count == 0)
			{
				return Rectangle.Empty;
			}
			Rectangle rectangle = (Rectangle)invalid_list[0];
			for (int i = 1; i < invalid_list.Count; i++)
			{
				rectangle = Rectangle.Union(rectangle, (Rectangle)invalid_list[i]);
			}
			return rectangle;
		}
	}

	public Rectangle[] ClipRectangles => (Rectangle[])invalid_list.ToArray(typeof(Rectangle));

	public Rectangle NCInvalid
	{
		get
		{
			return nc_invalid;
		}
		set
		{
			nc_invalid = value;
		}
	}

	public bool NCExposePending => nc_expose_pending;

	public Hwnd Parent
	{
		get
		{
			return parent;
		}
		set
		{
			if (parent != null)
			{
				parent.children.Remove(this);
			}
			parent = value;
			if (parent != null)
			{
				parent.children.Add(this);
			}
		}
	}

	public bool Mapped
	{
		get
		{
			if (!mapped)
			{
				return false;
			}
			if (parent != null)
			{
				return parent.Mapped;
			}
			return true;
		}
		set
		{
			mapped = value;
		}
	}

	public int CaptionHeight
	{
		get
		{
			return caption_height;
		}
		set
		{
			caption_height = value;
		}
	}

	public int ToolCaptionHeight
	{
		get
		{
			return tool_caption_height;
		}
		set
		{
			tool_caption_height = value;
		}
	}

	public TitleStyle TitleStyle
	{
		get
		{
			return title_style;
		}
		set
		{
			title_style = value;
		}
	}

	public object UserData
	{
		get
		{
			return user_data;
		}
		set
		{
			user_data = value;
		}
	}

	public IntPtr WholeWindow
	{
		get
		{
			return whole_window;
		}
		set
		{
			whole_window = value;
			zombie = false;
			if (!(whole_window != IntPtr.Zero))
			{
				return;
			}
			lock (windows)
			{
				if (windows[whole_window] == null)
				{
					windows[whole_window] = this;
				}
			}
		}
	}

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

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
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

	public Hwnd()
	{
		x = 0;
		y = 0;
		width = 0;
		height = 0;
		visible = false;
		menu = null;
		border_style = FormBorderStyle.None;
		client_window = IntPtr.Zero;
		whole_window = IntPtr.Zero;
		cursor = IntPtr.Zero;
		handle = IntPtr.Zero;
		parent = null;
		invalid_list = new ArrayList();
		expose_pending = false;
		nc_expose_pending = false;
		enabled = true;
		reparented = false;
		client_rectangle = Rectangle.Empty;
		marshal_free_list = new ArrayList(2);
		opacity = uint.MaxValue;
		fixed_size = false;
		drawing_stack = new Stack();
		children = new ArrayList();
		resizing_or_moving = false;
		whacky_wm = false;
	}

	public void Dispose()
	{
		expose_pending = false;
		nc_expose_pending = false;
		Parent = null;
		lock (windows)
		{
			windows.Remove(client_window);
			windows.Remove(whole_window);
		}
		client_window = IntPtr.Zero;
		whole_window = IntPtr.Zero;
		zombie = false;
		for (int i = 0; i < marshal_free_list.Count; i++)
		{
			Marshal.FreeHGlobal((IntPtr)marshal_free_list[i]);
		}
		marshal_free_list.Clear();
	}

	public static Hwnd ObjectFromWindow(IntPtr window)
	{
		lock (windows)
		{
			return (Hwnd)windows[window];
		}
	}

	public static Hwnd ObjectFromHandle(IntPtr handle)
	{
		lock (windows)
		{
			return (Hwnd)windows[handle];
		}
	}

	public static IntPtr HandleFromObject(Hwnd obj)
	{
		return obj.handle;
	}

	public static Hwnd GetObjectFromWindow(IntPtr window)
	{
		lock (windows)
		{
			return (Hwnd)windows[window];
		}
	}

	public static IntPtr GetHandleFromWindow(IntPtr window)
	{
		Hwnd hwnd;
		lock (windows)
		{
			hwnd = (Hwnd)windows[window];
		}
		return hwnd?.handle ?? IntPtr.Zero;
	}

	public static Borders GetBorderWidth(CreateParams cp)
	{
		Borders result = default(Borders);
		Size borderSize = ThemeEngine.Current.BorderSize;
		Size borderStaticSize = ThemeEngine.Current.BorderStaticSize;
		Size border3DSize = ThemeEngine.Current.Border3DSize;
		Size size = new Size(2 + borderSize.Width, 2 + borderSize.Height);
		Size borderSizableSize = ThemeEngine.Current.BorderSizableSize;
		if (cp.IsSet(WindowStyles.WS_CAPTION))
		{
			result.Inflate(borderSizableSize);
		}
		else if (cp.IsSet(WindowStyles.WS_BORDER))
		{
			if (cp.IsSet(WindowExStyles.WS_EX_DLGMODALFRAME))
			{
				if (cp.IsSet(WindowStyles.WS_THICKFRAME) && (cp.IsSet(WindowExStyles.WS_EX_STATICEDGE) || cp.IsSet(WindowExStyles.WS_EX_CLIENTEDGE)))
				{
					result.Inflate(borderStaticSize);
				}
			}
			else
			{
				result.Inflate(borderStaticSize);
			}
		}
		else if (cp.IsSet(WindowStyles.WS_DLGFRAME))
		{
			result.Inflate(borderSizableSize);
		}
		if (cp.IsSet(WindowStyles.WS_THICKFRAME))
		{
			if (cp.IsSet(WindowStyles.WS_DLGFRAME))
			{
				result.Inflate(borderStaticSize);
			}
			else
			{
				result.Inflate(size);
			}
		}
		Size size2 = Size.Empty;
		bool flag = cp.IsSet(WindowStyles.WS_THICKFRAME) || cp.IsSet(WindowStyles.WS_DLGFRAME);
		if (flag && cp.IsSet(WindowStyles.WS_THICKFRAME) && !cp.IsSet(WindowStyles.WS_BORDER) && !cp.IsSet(WindowStyles.WS_DLGFRAME))
		{
			size2 = borderStaticSize;
		}
		if (cp.IsSet(WindowExStyles.WS_EX_DLGMODALFRAME | WindowExStyles.WS_EX_CLIENTEDGE))
		{
			result.Inflate(border3DSize + ((!flag) ? borderSizableSize : size2));
		}
		else if (cp.IsSet(WindowExStyles.WS_EX_DLGMODALFRAME | WindowExStyles.WS_EX_STATICEDGE))
		{
			result.Inflate((!flag) ? borderSizableSize : size2);
		}
		else if (cp.IsSet(WindowExStyles.WS_EX_CLIENTEDGE | WindowExStyles.WS_EX_STATICEDGE))
		{
			result.Inflate(borderStaticSize + ((!flag) ? border3DSize : Size.Empty));
		}
		else
		{
			if (cp.IsSet(WindowExStyles.WS_EX_CLIENTEDGE))
			{
				result.Inflate(border3DSize);
			}
			if (cp.IsSet(WindowExStyles.WS_EX_DLGMODALFRAME) && !cp.IsSet(WindowStyles.WS_DLGFRAME))
			{
				result.Inflate((!cp.IsSet(WindowStyles.WS_THICKFRAME)) ? borderSizableSize : borderStaticSize);
			}
			if (cp.IsSet(WindowExStyles.WS_EX_STATICEDGE))
			{
				if (cp.IsSet(WindowStyles.WS_THICKFRAME) || cp.IsSet(WindowStyles.WS_DLGFRAME))
				{
					result.Inflate(new Size(-borderStaticSize.Width, -borderStaticSize.Height));
				}
				else
				{
					result.Inflate(borderStaticSize);
				}
			}
		}
		return result;
	}

	public static Rectangle GetWindowRectangle(CreateParams cp, Menu menu)
	{
		return GetWindowRectangle(cp, menu, Rectangle.Empty);
	}

	public static Rectangle GetWindowRectangle(CreateParams cp, Menu menu, Rectangle client_rect)
	{
		Borders borders = GetBorders(cp, menu);
		Rectangle result = new Rectangle(Point.Empty, client_rect.Size);
		result.Y -= borders.top;
		result.Height += borders.top + borders.bottom;
		result.X -= borders.left;
		result.Width += borders.left + borders.right;
		return result;
	}

	public Rectangle GetClientRectangle(int width, int height)
	{
		CreateParams createParams = new CreateParams();
		createParams.WindowStyle = initial_style;
		createParams.WindowExStyle = initial_ex_style;
		return GetClientRectangle(createParams, menu, width, height);
	}

	public ArrayList GetClippingRectangles()
	{
		ArrayList arrayList = new ArrayList();
		if (x < 0)
		{
			arrayList.Add(new Rectangle(0, 0, x * -1, Height));
			if (y < 0)
			{
				arrayList.Add(new Rectangle(x * -1, 0, Width, y * -1));
			}
		}
		else if (y < 0)
		{
			arrayList.Add(new Rectangle(0, 0, Width, y * -1));
		}
		foreach (Hwnd child in children)
		{
			if (child.visible)
			{
				arrayList.Add(new Rectangle(child.X, child.Y, child.Width, child.Height));
			}
		}
		if (parent == null)
		{
			return arrayList;
		}
		ArrayList arrayList2 = parent.children;
		foreach (Hwnd item in arrayList2)
		{
			IntPtr previousWindow = whole_window;
			if (item == this)
			{
				continue;
			}
			do
			{
				previousWindow = XplatUI.GetPreviousWindow(previousWindow);
				if (previousWindow == item.WholeWindow && item.visible)
				{
					Rectangle rectangle = Rectangle.Intersect(new Rectangle(X, Y, Width, Height), new Rectangle(item.X, item.Y, item.Width, item.Height));
					if (!(rectangle == Rectangle.Empty))
					{
						rectangle.X -= X;
						rectangle.Y -= Y;
						arrayList.Add(rectangle);
					}
				}
			}
			while (previousWindow != IntPtr.Zero);
		}
		return arrayList;
	}

	public static Borders GetBorders(CreateParams cp, Menu menu)
	{
		Borders result = default(Borders);
		if (menu != null)
		{
			int num = menu.Rect.Height;
			if (num == 0)
			{
				num = ThemeEngine.Current.CalcMenuBarSize(GraphicsContext, menu, cp.Width);
			}
			result.top += num;
		}
		if (cp.IsSet(WindowStyles.WS_CAPTION))
		{
			int num2 = ((!cp.IsSet(WindowExStyles.WS_EX_TOOLWINDOW)) ? ThemeEngine.Current.CaptionHeight : ThemeEngine.Current.ToolWindowCaptionHeight);
			result.top += num2;
		}
		Borders borderWidth = GetBorderWidth(cp);
		result.left += borderWidth.left;
		result.right += borderWidth.right;
		result.top += borderWidth.top;
		result.bottom += borderWidth.bottom;
		return result;
	}

	public static Rectangle GetClientRectangle(CreateParams cp, Menu menu, int width, int height)
	{
		Borders borders = GetBorders(cp, menu);
		Rectangle result = new Rectangle(0, 0, width, height);
		result.Y += borders.top;
		result.Height -= borders.top + borders.bottom;
		result.X += borders.left;
		result.Width -= borders.left + borders.right;
		return result;
	}

	public void AddInvalidArea(int x, int y, int width, int height)
	{
		AddInvalidArea(new Rectangle(x, y, width, height));
	}

	public void AddInvalidArea(Rectangle rect)
	{
		ArrayList arrayList = new ArrayList();
		foreach (Rectangle item in invalid_list)
		{
			if (!rect.Contains(item))
			{
				arrayList.Add(item);
			}
		}
		arrayList.Add(rect);
		invalid_list = arrayList;
	}

	public void ClearInvalidArea()
	{
		invalid_list.Clear();
		expose_pending = false;
	}

	public void AddNcInvalidArea(int x, int y, int width, int height)
	{
		if (nc_invalid == Rectangle.Empty)
		{
			nc_invalid = new Rectangle(x, y, width, height);
			return;
		}
		int num = Math.Max(nc_invalid.Right, x + width);
		int num2 = Math.Max(nc_invalid.Bottom, y + height);
		nc_invalid.X = Math.Min(nc_invalid.X, x);
		nc_invalid.Y = Math.Min(nc_invalid.Y, y);
		nc_invalid.Width = num - nc_invalid.X;
		nc_invalid.Height = num2 - nc_invalid.Y;
	}

	public void AddNcInvalidArea(Rectangle rect)
	{
		if (nc_invalid == Rectangle.Empty)
		{
			nc_invalid = rect;
		}
		else
		{
			nc_invalid = Rectangle.Union(nc_invalid, rect);
		}
	}

	public void ClearNcInvalidArea()
	{
		nc_invalid = Rectangle.Empty;
		nc_expose_pending = false;
	}

	public override string ToString()
	{
		return string.Format("Hwnd, Mapped:{3} ClientWindow:0x{0:X}, WholeWindow:0x{1:X}, Zombie={4}, Parent:[{2:X}]", client_window.ToInt32(), whole_window.ToInt32(), (parent == null) ? "<null>" : parent.ToString(), Mapped, zombie);
	}

	public static Point GetNextStackedFormLocation(CreateParams cp, Hwnd parent_hwnd)
	{
		if (cp.control == null)
		{
			return Point.Empty;
		}
		int num = cp.X;
		int num2 = cp.Y;
		Point point;
		Rectangle rectangle;
		if (parent_hwnd != null)
		{
			Control control = cp.control.Parent;
			point = parent_hwnd.previous_child_startup_location;
			rectangle = ((!(parent_hwnd.client_rectangle == Rectangle.Empty) || control == null) ? parent_hwnd.client_rectangle : control.ClientRectangle);
		}
		else
		{
			point = previous_main_startup_location;
			rectangle = Screen.PrimaryScreen.WorkingArea;
		}
		Point point2 = ((point.X != int.MinValue && point.Y != int.MinValue) ? new Point(point.X + 22, point.Y + 22) : Point.Empty);
		if (!rectangle.Contains(point2.X * 3, point2.Y * 3))
		{
			point2 = Point.Empty;
		}
		if (point2 == Point.Empty && cp.Parent == IntPtr.Zero)
		{
			point2 = new Point(22, 22);
		}
		if (parent_hwnd != null)
		{
			parent_hwnd.previous_child_startup_location = point2;
		}
		else
		{
			previous_main_startup_location = point2;
		}
		if (num == int.MinValue && num2 == int.MinValue)
		{
			num = point2.X;
			num2 = point2.Y;
		}
		return new Point(num, num2);
	}
}
