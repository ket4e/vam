using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms.CarbonInternal;

namespace System.Windows.Forms;

internal class XplatUICarbon : XplatUIDriver
{
	private static XplatUICarbon Instance;

	private static int RefCount;

	private static bool themes_enabled;

	internal static IntPtr FocusWindow;

	internal static IntPtr ActiveWindow;

	internal static IntPtr ReverseWindow;

	internal static IntPtr CaretWindow;

	internal static Hwnd MouseHwnd;

	internal static MouseButtons MouseState;

	internal static Hover Hover;

	internal static HwndDelegate HwndDelegate = GetClippingRectangles;

	internal Point mouse_position;

	internal ApplicationHandler ApplicationHandler;

	internal ControlHandler ControlHandler;

	internal HIObjectHandler HIObjectHandler;

	internal KeyboardHandler KeyboardHandler;

	internal MouseHandler MouseHandler;

	internal WindowHandler WindowHandler;

	internal static GrabStruct Grab;

	internal static Caret Caret;

	private static Dnd Dnd;

	private static Hashtable WindowMapping;

	private static Hashtable HandleMapping;

	private static IntPtr FosterParent;

	private static IntPtr Subclass;

	private static int MenuBarHeight;

	internal static ArrayList UtilityWindows;

	private static Queue MessageQueue;

	private static bool GetMessageResult;

	private static bool ReverseWindowMapped;

	private ArrayList TimerList;

	private static bool in_doevents;

	private static readonly object instancelock = new object();

	private static readonly object queuelock = new object();

	public int Reference => RefCount;

	internal override Point MousePosition => mouse_position;

	internal override int KeyboardSpeed
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override int KeyboardDelay
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override int CaptionHeight => 19;

	internal override Size CursorSize
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override bool DragFullWindows
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override Size DragSize => new Size(4, 4);

	internal override Size FrameBorderSize => new Size(2, 2);

	internal override Size IconSize
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override Size MaxWindowTrackSize
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override bool MenuAccessKeysUnderlined => false;

	internal override Size MinimizedWindowSpacingSize
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override Size MinimumWindowSize => new Size(110, 22);

	internal override Keys ModifierKeys => KeyboardHandler.ModifierKeys;

	internal override Size SmallIconSize
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override int MouseButtonCount
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override bool MouseButtonsSwapped
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override bool MouseWheelPresent
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal override MouseButtons MouseButtons => MouseState;

	internal override Rectangle VirtualScreen => WorkingArea;

	internal override Rectangle WorkingArea
	{
		get
		{
			HIRect hIRect = CGDisplayBounds(CGMainDisplayID());
			return new Rectangle((int)hIRect.origin.x, (int)hIRect.origin.y, (int)hIRect.size.width, (int)hIRect.size.height);
		}
	}

	internal override bool ThemesEnabled => themes_enabled;

	internal override event EventHandler Idle;

	private XplatUICarbon()
	{
		RefCount = 0;
		TimerList = new ArrayList();
		in_doevents = false;
		MessageQueue = new Queue();
		Initialize();
	}

	~XplatUICarbon()
	{
	}

	public static XplatUICarbon GetInstance()
	{
		lock (instancelock)
		{
			if (Instance == null)
			{
				Instance = new XplatUICarbon();
			}
			RefCount++;
		}
		return Instance;
	}

	internal void AddExpose(Hwnd hwnd, bool client, HIRect rect)
	{
		AddExpose(hwnd, client, (int)rect.origin.x, (int)rect.origin.y, (int)rect.size.width, (int)rect.size.height);
	}

	internal void AddExpose(Hwnd hwnd, bool client, Rectangle rect)
	{
		AddExpose(hwnd, client, rect.X, rect.Y, rect.Width, rect.Height);
	}

	internal void FlushQueue()
	{
		CheckTimers(DateTime.UtcNow);
		lock (queuelock)
		{
			while (MessageQueue.Count > 0)
			{
				object obj = MessageQueue.Dequeue();
				if (obj is GCHandle)
				{
					XplatUIDriverSupport.ExecuteClientMessage((GCHandle)obj);
					continue;
				}
				MSG mSG = (MSG)obj;
				NativeWindow.WndProc(mSG.hwnd, mSG.message, mSG.wParam, mSG.lParam);
			}
		}
	}

	internal static Rectangle[] GetClippingRectangles(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null)
		{
			return null;
		}
		if (hwnd.Handle != handle)
		{
			return new Rectangle[1] { hwnd.ClientRect };
		}
		return (Rectangle[])hwnd.GetClippingRectangles().ToArray(typeof(Rectangle));
	}

	internal IntPtr GetMousewParam(int Delta)
	{
		int num = 0;
		if ((MouseState & MouseButtons.Left) != 0)
		{
			num |= 1;
		}
		if ((MouseState & MouseButtons.Middle) != 0)
		{
			num |= 0x10;
		}
		if ((MouseState & MouseButtons.Right) != 0)
		{
			num |= 2;
		}
		Keys modifierKeys = ModifierKeys;
		if ((modifierKeys & Keys.Control) != 0)
		{
			num |= 8;
		}
		if ((modifierKeys & Keys.Shift) != 0)
		{
			num |= 4;
		}
		num |= Delta << 16;
		return (IntPtr)num;
	}

	internal IntPtr HandleToWindow(IntPtr handle)
	{
		if (HandleMapping[handle] != null)
		{
			return (IntPtr)HandleMapping[handle];
		}
		return IntPtr.Zero;
	}

	internal void Initialize()
	{
		System.Windows.Forms.CarbonInternal.EventHandler.Driver = this;
		ApplicationHandler = new ApplicationHandler(this);
		ControlHandler = new ControlHandler(this);
		HIObjectHandler = new HIObjectHandler(this);
		KeyboardHandler = new KeyboardHandler(this);
		MouseHandler = new MouseHandler(this);
		WindowHandler = new WindowHandler(this);
		Hover.Interval = 500;
		Hover.Timer = new Timer();
		Hover.Timer.Enabled = false;
		Hover.Timer.Interval = Hover.Interval;
		Hover.Timer.Tick += HoverCallback;
		Hover.X = -1;
		Hover.Y = -1;
		MouseState = MouseButtons.None;
		mouse_position = Point.Empty;
		Caret.Timer = new Timer();
		Caret.Timer.Interval = 500;
		Caret.Timer.Tick += CaretCallback;
		Dnd = new Dnd();
		WindowMapping = new Hashtable();
		HandleMapping = new Hashtable();
		UtilityWindows = new ArrayList();
		Rect r = default(Rect);
		SetRect(ref r, 0, 0, 0, 0);
		ProcessSerialNumber psn = default(ProcessSerialNumber);
		GetCurrentProcess(ref psn);
		TransformProcessType(ref psn, 1u);
		SetFrontProcess(ref psn);
		HIObjectRegisterSubclass(__CFStringMakeConstantString("com.novell.mwfview"), __CFStringMakeConstantString("com.apple.hiview"), 0u, System.Windows.Forms.CarbonInternal.EventHandler.EventHandlerDelegate, (uint)System.Windows.Forms.CarbonInternal.EventHandler.HIObjectEvents.Length, System.Windows.Forms.CarbonInternal.EventHandler.HIObjectEvents, IntPtr.Zero, ref Subclass);
		System.Windows.Forms.CarbonInternal.EventHandler.InstallApplicationHandler();
		CreateNewWindow(WindowClass.kDocumentWindowClass, (WindowAttributes)34078751u, ref r, ref FosterParent);
		CreateNewWindow(WindowClass.kOverlayWindowClass, (WindowAttributes)196608u, ref r, ref ReverseWindow);
		CreateNewWindow(WindowClass.kOverlayWindowClass, (WindowAttributes)196608u, ref r, ref CaretWindow);
		Rect rect = default(Rect);
		Rect rect2 = default(Rect);
		GetWindowBounds(FosterParent, 32u, ref rect);
		GetWindowBounds(FosterParent, 33u, ref rect2);
		MenuBarHeight = GetMBarHeight();
		FocusWindow = IntPtr.Zero;
		GetMessageResult = true;
		ReverseWindowMapped = false;
	}

	internal void PerformNCCalc(Hwnd hwnd)
	{
		Rectangle rectangle = new Rectangle(0, 0, hwnd.Width, hwnd.Height);
		XplatUIWin32.NCCALCSIZE_PARAMS nCCALCSIZE_PARAMS = default(XplatUIWin32.NCCALCSIZE_PARAMS);
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(nCCALCSIZE_PARAMS));
		nCCALCSIZE_PARAMS.rgrc1.left = rectangle.Left;
		nCCALCSIZE_PARAMS.rgrc1.top = rectangle.Top;
		nCCALCSIZE_PARAMS.rgrc1.right = rectangle.Right;
		nCCALCSIZE_PARAMS.rgrc1.bottom = rectangle.Bottom;
		Marshal.StructureToPtr(nCCALCSIZE_PARAMS, intPtr, fDeleteOld: true);
		NativeWindow.WndProc(hwnd.client_window, Msg.WM_NCCALCSIZE, (IntPtr)1, intPtr);
		nCCALCSIZE_PARAMS = (XplatUIWin32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(intPtr, typeof(XplatUIWin32.NCCALCSIZE_PARAMS));
		Marshal.FreeHGlobal(intPtr);
		rectangle = new Rectangle(nCCALCSIZE_PARAMS.rgrc1.left, nCCALCSIZE_PARAMS.rgrc1.top, nCCALCSIZE_PARAMS.rgrc1.right - nCCALCSIZE_PARAMS.rgrc1.left, nCCALCSIZE_PARAMS.rgrc1.bottom - nCCALCSIZE_PARAMS.rgrc1.top);
		hwnd.ClientRect = rectangle;
		rectangle = TranslateClientRectangleToQuartzClientRectangle(hwnd);
		if (hwnd.visible)
		{
			HIRect bounds = new HIRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			HIViewSetFrame(hwnd.client_window, ref bounds);
		}
		AddExpose(hwnd, client: false, 0, 0, hwnd.Width, hwnd.Height);
	}

	internal void ScreenToClient(IntPtr handle, ref QDPoint point)
	{
		int x = point.x;
		int y = point.y;
		ScreenToClient(handle, ref x, ref y);
		point.x = (short)x;
		point.y = (short)y;
	}

	internal static Rectangle TranslateClientRectangleToQuartzClientRectangle(Hwnd hwnd)
	{
		return TranslateClientRectangleToQuartzClientRectangle(hwnd, Control.FromHandle(hwnd.Handle));
	}

	internal static Rectangle TranslateClientRectangleToQuartzClientRectangle(Hwnd hwnd, Control ctrl)
	{
		Rectangle rectangle = hwnd.ClientRect;
		Form form = ctrl as Form;
		CreateParams createParams = null;
		if (form != null)
		{
			createParams = form.GetCreateParams();
		}
		if (form != null && (form.window_manager == null || createParams.IsSet(WindowExStyles.WS_EX_TOOLWINDOW)))
		{
			Hwnd.Borders borders = Hwnd.GetBorders(createParams, null);
			Rectangle rectangle2 = rectangle;
			rectangle2.Y -= borders.top;
			rectangle2.X -= borders.left;
			rectangle2.Width += borders.left + borders.right;
			rectangle2.Height += borders.top + borders.bottom;
			rectangle = rectangle2;
		}
		if (rectangle.Width < 1 || rectangle.Height < 1)
		{
			rectangle.Width = 1;
			rectangle.Height = 1;
			rectangle.X = -5;
			rectangle.Y = -5;
		}
		return rectangle;
	}

	internal static Size TranslateWindowSizeToQuartzWindowSize(CreateParams cp)
	{
		return TranslateWindowSizeToQuartzWindowSize(cp, new Size(cp.Width, cp.Height));
	}

	internal static Size TranslateWindowSizeToQuartzWindowSize(CreateParams cp, Size size)
	{
		if (cp.control is Form form && (form.window_manager == null || cp.IsSet(WindowExStyles.WS_EX_TOOLWINDOW)))
		{
			Hwnd.Borders borders = Hwnd.GetBorders(cp, null);
			Size size2 = size;
			size2.Width -= borders.left + borders.right;
			size2.Height -= borders.top + borders.bottom;
			size = size2;
		}
		if (size.Height == 0)
		{
			size.Height = 1;
		}
		if (size.Width == 0)
		{
			size.Width = 1;
		}
		return size;
	}

	internal static Size TranslateQuartzWindowSizeToWindowSize(CreateParams cp, int width, int height)
	{
		Size size = new Size(width, height);
		if (cp.control is Form form && (form.window_manager == null || cp.IsSet(WindowExStyles.WS_EX_TOOLWINDOW)))
		{
			Hwnd.Borders borders = Hwnd.GetBorders(cp, null);
			Size result = size;
			result.Width += borders.left + borders.right;
			result.Height += borders.top + borders.bottom;
			return result;
		}
		return size;
	}

	private void CaretCallback(object sender, EventArgs e)
	{
		if (!Caret.Paused)
		{
			if (!Caret.On)
			{
				ShowCaret();
			}
			else
			{
				HideCaret();
			}
		}
	}

	private void HoverCallback(object sender, EventArgs e)
	{
		if (Hover.X == mouse_position.X && Hover.Y == mouse_position.Y)
		{
			MSG msg = default(MSG);
			msg.hwnd = Hover.Hwnd;
			msg.message = Msg.WM_MOUSEHOVER;
			msg.wParam = GetMousewParam(0);
			msg.lParam = (IntPtr)(((ushort)Hover.X << 16) | (ushort)Hover.X);
			EnqueueMessage(msg);
		}
	}

	private Point ConvertScreenPointToClient(IntPtr handle, Point point)
	{
		Point result = default(Point);
		Rect rect = default(Rect);
		CGPoint point2 = default(CGPoint);
		GetWindowBounds(HIViewGetWindow(handle), 32u, ref rect);
		point2.x = point.X - rect.left;
		point2.y = point.Y - rect.top;
		HIViewConvertPoint(ref point2, IntPtr.Zero, handle);
		result.X = (int)point2.x;
		result.Y = (int)point2.y;
		return result;
	}

	private Point ConvertClientPointToScreen(IntPtr handle, Point point)
	{
		Point result = default(Point);
		Rect rect = default(Rect);
		CGPoint point2 = default(CGPoint);
		GetWindowBounds(HIViewGetWindow(handle), 32u, ref rect);
		point2.x = point.X;
		point2.y = point.Y;
		HIViewConvertPoint(ref point2, handle, IntPtr.Zero);
		result.X = (int)(point2.x + (float)rect.left);
		result.Y = (int)(point2.y + (float)rect.top);
		return result;
	}

	private double NextTimeout()
	{
		DateTime utcNow = DateTime.UtcNow;
		int num = 134217727;
		lock (TimerList)
		{
			foreach (Timer timer in TimerList)
			{
				int num2 = (int)(timer.Expires - utcNow).TotalMilliseconds;
				if (num2 < 0)
				{
					return 0.0;
				}
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		if (num < Timer.Minimum)
		{
			num = Timer.Minimum;
		}
		return (double)num / 1000.0;
	}

	private void CheckTimers(DateTime now)
	{
		lock (TimerList)
		{
			if (TimerList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < TimerList.Count; i++)
			{
				Timer timer = (Timer)TimerList[i];
				if (timer.Enabled && timer.Expires <= now && (in_doevents || (Application.MWFThread.Current.Context != null && Application.MWFThread.Current.Context.MainForm != null && Application.MWFThread.Current.Context.MainForm.IsLoaded)))
				{
					timer.FireTick();
					timer.Update(now);
				}
			}
		}
	}

	private void WaitForHwndMessage(Hwnd hwnd, Msg message)
	{
		MSG msg = default(MSG);
		bool flag = false;
		do
		{
			if (!GetMessage(null, ref msg, IntPtr.Zero, 0, 0))
			{
				continue;
			}
			if (msg.message == Msg.WM_QUIT)
			{
				PostQuitMessage(0);
				flag = true;
				continue;
			}
			if (msg.hwnd == hwnd.Handle)
			{
				if (msg.message == message)
				{
					break;
				}
				if (msg.message == Msg.WM_DESTROY)
				{
					flag = true;
				}
			}
			TranslateMessage(ref msg);
			DispatchMessage(ref msg);
		}
		while (!flag);
	}

	private void SendParentNotify(IntPtr child, Msg cause, int x, int y)
	{
		if (child == IntPtr.Zero)
		{
			return;
		}
		Hwnd objectFromWindow = Hwnd.GetObjectFromWindow(child);
		if (objectFromWindow != null && !(objectFromWindow.Handle == IntPtr.Zero) && !ExStyleSet((int)objectFromWindow.initial_ex_style, WindowExStyles.WS_EX_NOPARENTNOTIFY) && objectFromWindow.Parent != null && !(objectFromWindow.Parent.Handle == IntPtr.Zero))
		{
			if (cause == Msg.WM_CREATE || cause == Msg.WM_DESTROY)
			{
				SendMessage(objectFromWindow.Parent.Handle, Msg.WM_PARENTNOTIFY, Control.MakeParam((int)cause, 0), child);
			}
			else
			{
				SendMessage(objectFromWindow.Parent.Handle, Msg.WM_PARENTNOTIFY, Control.MakeParam((int)cause, 0), Control.MakeParam(x, y));
			}
			SendParentNotify(objectFromWindow.Parent.Handle, cause, x, y);
		}
	}

	private bool StyleSet(int s, WindowStyles ws)
	{
		return ((uint)s & (uint)ws) == (uint)ws;
	}

	private bool ExStyleSet(int ex, WindowExStyles exws)
	{
		return ((uint)ex & (uint)exws) == (uint)exws;
	}

	private void DeriveStyles(int Style, int ExStyle, out FormBorderStyle border_style, out bool border_static, out TitleStyle title_style, out int caption_height, out int tool_caption_height)
	{
		caption_height = 0;
		tool_caption_height = 0;
		border_static = false;
		if (StyleSet(Style, WindowStyles.WS_CHILD))
		{
			if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_CLIENTEDGE))
			{
				border_style = FormBorderStyle.Fixed3D;
			}
			else if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_STATICEDGE))
			{
				border_style = FormBorderStyle.Fixed3D;
				border_static = true;
			}
			else if (!StyleSet(Style, WindowStyles.WS_BORDER))
			{
				border_style = FormBorderStyle.None;
			}
			else
			{
				border_style = FormBorderStyle.FixedSingle;
			}
			title_style = TitleStyle.None;
			if (StyleSet(Style, WindowStyles.WS_CAPTION))
			{
				caption_height = 0;
				if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_TOOLWINDOW))
				{
					title_style = TitleStyle.Tool;
				}
				else
				{
					title_style = TitleStyle.Normal;
				}
			}
			if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_MDICHILD))
			{
				caption_height = 0;
				if (StyleSet(Style, WindowStyles.WS_OVERLAPPEDWINDOW) || ExStyleSet(ExStyle, WindowExStyles.WS_EX_TOOLWINDOW))
				{
					border_style = (FormBorderStyle)65535;
				}
				else
				{
					border_style = FormBorderStyle.None;
				}
			}
			return;
		}
		title_style = TitleStyle.None;
		if (StyleSet(Style, WindowStyles.WS_CAPTION))
		{
			if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_TOOLWINDOW))
			{
				title_style = TitleStyle.Tool;
			}
			else
			{
				title_style = TitleStyle.Normal;
			}
		}
		border_style = FormBorderStyle.None;
		if (StyleSet(Style, WindowStyles.WS_THICKFRAME))
		{
			if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_TOOLWINDOW))
			{
				border_style = FormBorderStyle.SizableToolWindow;
			}
			else
			{
				border_style = FormBorderStyle.Sizable;
			}
		}
		else if (StyleSet(Style, WindowStyles.WS_CAPTION))
		{
			if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_CLIENTEDGE))
			{
				border_style = FormBorderStyle.Fixed3D;
			}
			else if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_STATICEDGE))
			{
				border_style = FormBorderStyle.Fixed3D;
				border_static = true;
			}
			else if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_DLGMODALFRAME))
			{
				border_style = FormBorderStyle.FixedDialog;
			}
			else if (ExStyleSet(ExStyle, WindowExStyles.WS_EX_TOOLWINDOW))
			{
				border_style = FormBorderStyle.FixedToolWindow;
			}
			else if (StyleSet(Style, WindowStyles.WS_BORDER))
			{
				border_style = FormBorderStyle.FixedSingle;
			}
		}
		else if (StyleSet(Style, WindowStyles.WS_BORDER))
		{
			border_style = FormBorderStyle.FixedSingle;
		}
	}

	private void SetHwndStyles(Hwnd hwnd, CreateParams cp)
	{
		DeriveStyles(cp.Style, cp.ExStyle, out hwnd.border_style, out hwnd.border_static, out hwnd.title_style, out hwnd.caption_height, out hwnd.tool_caption_height);
	}

	private void ShowCaret()
	{
		if (!Caret.On)
		{
			Caret.On = true;
			ShowWindow(CaretWindow);
			Graphics graphics = Graphics.FromHwnd(HIViewGetRoot(CaretWindow));
			graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, Caret.Width, Caret.Height));
			graphics.Dispose();
		}
	}

	private void HideCaret()
	{
		if (Caret.On)
		{
			Caret.On = false;
			HideWindow(CaretWindow);
		}
	}

	private void AccumulateDestroyedHandles(Control c, ArrayList list)
	{
		if (c != null)
		{
			Control[] allControls = c.Controls.GetAllControls();
			if (c.IsHandleCreated && !c.IsDisposed)
			{
				Hwnd hwnd = Hwnd.ObjectFromHandle(c.Handle);
				list.Add(hwnd);
				CleanupCachedWindows(hwnd);
			}
			for (int i = 0; i < allControls.Length; i++)
			{
				AccumulateDestroyedHandles(allControls[i], list);
			}
		}
	}

	private void CleanupCachedWindows(Hwnd hwnd)
	{
		if (ActiveWindow == hwnd.Handle)
		{
			SendMessage(hwnd.client_window, Msg.WM_ACTIVATE, (IntPtr)0, IntPtr.Zero);
			ActiveWindow = IntPtr.Zero;
		}
		if (FocusWindow == hwnd.Handle)
		{
			SendMessage(hwnd.client_window, Msg.WM_KILLFOCUS, IntPtr.Zero, IntPtr.Zero);
			FocusWindow = IntPtr.Zero;
		}
		if (Grab.Hwnd == hwnd.Handle)
		{
			Grab.Hwnd = IntPtr.Zero;
			Grab.Confined = false;
		}
		DestroyCaret(hwnd.Handle);
	}

	private void AddExpose(Hwnd hwnd, bool client, int x, int y, int width, int height)
	{
		if (hwnd == null || x > hwnd.Width || y > hwnd.Height || x + width < 0 || y + height < 0)
		{
			return;
		}
		if (x + width > hwnd.width)
		{
			width = hwnd.width - x;
		}
		if (y + height > hwnd.height)
		{
			height = hwnd.height - y;
		}
		if (client)
		{
			hwnd.AddInvalidArea(x, y, width, height);
			if (!hwnd.expose_pending && hwnd.visible)
			{
				MSG msg = default(MSG);
				msg.message = Msg.WM_PAINT;
				msg.hwnd = hwnd.Handle;
				EnqueueMessage(msg);
				hwnd.expose_pending = true;
			}
			return;
		}
		hwnd.AddNcInvalidArea(x, y, width, height);
		if (!hwnd.nc_expose_pending && hwnd.visible)
		{
			MSG msg2 = default(MSG);
			Region region = new Region(hwnd.Invalid);
			IntPtr hrgn = region.GetHrgn(null);
			msg2.message = Msg.WM_NCPAINT;
			msg2.wParam = ((!(hrgn == IntPtr.Zero)) ? hrgn : ((IntPtr)1));
			msg2.refobject = region;
			msg2.hwnd = hwnd.Handle;
			EnqueueMessage(msg2);
			hwnd.nc_expose_pending = true;
		}
	}

	internal void EnqueueMessage(MSG msg)
	{
		lock (queuelock)
		{
			MessageQueue.Enqueue(msg);
		}
	}

	internal override void RaiseIdle(EventArgs e)
	{
		if (Idle != null)
		{
			Idle(this, e);
		}
	}

	internal override IntPtr InitializeDriver()
	{
		return IntPtr.Zero;
	}

	internal override void ShutdownDriver(IntPtr token)
	{
	}

	internal override void EnableThemes()
	{
		themes_enabled = true;
	}

	internal override void Activate(IntPtr handle)
	{
		if (ActiveWindow != IntPtr.Zero)
		{
			ActivateWindow(HIViewGetWindow(ActiveWindow), inActivate: false);
		}
		ActivateWindow(HIViewGetWindow(handle), inActivate: true);
		ActiveWindow = handle;
	}

	internal override void AudibleAlert(AlertType alert)
	{
		AlertSoundPlay();
	}

	internal override void CaretVisible(IntPtr hwnd, bool visible)
	{
		if (!(Caret.Hwnd == hwnd))
		{
			return;
		}
		if (visible)
		{
			if (Caret.Visible < 1)
			{
				Caret.Visible++;
				Caret.On = false;
				if (Caret.Visible == 1)
				{
					ShowCaret();
					Caret.Timer.Start();
				}
			}
		}
		else
		{
			Caret.Visible--;
			if (Caret.Visible == 0)
			{
				Caret.Timer.Stop();
				HideCaret();
			}
		}
	}

	internal override bool CalculateWindowRect(ref Rectangle ClientRect, CreateParams cp, Menu menu, out Rectangle WindowRect)
	{
		WindowRect = Hwnd.GetWindowRectangle(cp, menu, ClientRect);
		return true;
	}

	internal override void ClientToScreen(IntPtr handle, ref int x, ref int y)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Point point = ConvertClientPointToScreen(hwnd.ClientWindow, new Point(x, y));
		x = point.X;
		y = point.Y;
	}

	internal override void MenuToScreen(IntPtr handle, ref int x, ref int y)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Point point = ConvertClientPointToScreen(hwnd.ClientWindow, new Point(x, y));
		x = point.X;
		y = point.Y;
	}

	internal override int[] ClipboardAvailableFormats(IntPtr handle)
	{
		ArrayList arrayList = new ArrayList();
		for (DataFormats.Format format = DataFormats.Format.List; format != null; format = format.Next)
		{
			arrayList.Add(format.Id);
		}
		return (int[])arrayList.ToArray(typeof(int));
	}

	internal override void ClipboardClose(IntPtr handle)
	{
	}

	internal override int ClipboardGetID(IntPtr handle, string format)
	{
		return (int)__CFStringMakeConstantString(format);
	}

	internal override IntPtr ClipboardOpen(bool primary_selection)
	{
		if (primary_selection)
		{
			return Pasteboard.Primary;
		}
		return Pasteboard.Application;
	}

	internal override object ClipboardRetrieve(IntPtr handle, int type, XplatUI.ClipboardToObject converter)
	{
		return Pasteboard.Retrieve(handle, type);
	}

	internal override void ClipboardStore(IntPtr handle, object obj, int type, XplatUI.ObjectToClipboard converter)
	{
		Pasteboard.Store(handle, obj, type);
	}

	internal override void CreateCaret(IntPtr hwnd, int width, int height)
	{
		if (Caret.Hwnd != IntPtr.Zero)
		{
			DestroyCaret(Caret.Hwnd);
		}
		Caret.Hwnd = hwnd;
		Caret.Width = width;
		Caret.Height = height;
		Caret.Visible = 0;
		Caret.On = false;
	}

	internal override IntPtr CreateWindow(CreateParams cp)
	{
		Hwnd hwnd = null;
		Hwnd hwnd2 = new Hwnd();
		int x = cp.X;
		int y = cp.Y;
		int num = cp.Width;
		int num2 = cp.Height;
		IntPtr outPtr = IntPtr.Zero;
		IntPtr window = IntPtr.Zero;
		IntPtr hwnd3 = IntPtr.Zero;
		IntPtr zero = IntPtr.Zero;
		IntPtr outRef = IntPtr.Zero;
		IntPtr outRef2 = IntPtr.Zero;
		if (num < 1)
		{
			num = 1;
		}
		if (num2 < 1)
		{
			num2 = 1;
		}
		if (cp.Parent != IntPtr.Zero)
		{
			hwnd = Hwnd.ObjectFromHandle(cp.Parent);
			outPtr = hwnd.client_window;
		}
		else if (StyleSet(cp.Style, WindowStyles.WS_CHILD))
		{
			HIViewFindByID(HIViewGetRoot(FosterParent), new HIViewID(2003398244u, 1u), ref outPtr);
		}
		if (cp.control is Form)
		{
			Point nextStackedFormLocation = Hwnd.GetNextStackedFormLocation(cp, hwnd);
			x = nextStackedFormLocation.X;
			y = nextStackedFormLocation.Y;
		}
		hwnd2.x = x;
		hwnd2.y = y;
		hwnd2.width = num;
		hwnd2.height = num2;
		hwnd2.Parent = Hwnd.ObjectFromHandle(cp.Parent);
		hwnd2.initial_style = cp.WindowStyle;
		hwnd2.initial_ex_style = cp.WindowExStyle;
		hwnd2.visible = false;
		if (StyleSet(cp.Style, WindowStyles.WS_DISABLED))
		{
			hwnd2.enabled = false;
		}
		zero = IntPtr.Zero;
		Size size = TranslateWindowSizeToQuartzWindowSize(cp);
		Rectangle rectangle = TranslateClientRectangleToQuartzClientRectangle(hwnd2, cp.control);
		SetHwndStyles(hwnd2, cp);
		if (outPtr == IntPtr.Zero)
		{
			IntPtr outPtr2 = IntPtr.Zero;
			IntPtr outPtr3 = IntPtr.Zero;
			WindowClass windowClass = WindowClass.kOverlayWindowClass;
			WindowAttributes windowAttributes = (WindowAttributes)34078720u;
			if (StyleSet(cp.Style, WindowStyles.WS_GROUP))
			{
				windowAttributes |= WindowAttributes.kWindowCollapseBoxAttribute;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_TABSTOP))
			{
				windowAttributes |= (WindowAttributes)22u;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_SYSMENU))
			{
				windowAttributes |= WindowAttributes.kWindowCloseBoxAttribute;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_CAPTION))
			{
				windowClass = WindowClass.kDocumentWindowClass;
			}
			if (hwnd2.border_style == FormBorderStyle.FixedToolWindow)
			{
				windowClass = WindowClass.kUtilityWindowClass;
			}
			else if (hwnd2.border_style == FormBorderStyle.SizableToolWindow)
			{
				windowAttributes |= WindowAttributes.kWindowResizableAttribute;
				windowClass = WindowClass.kUtilityWindowClass;
			}
			if (windowClass == WindowClass.kOverlayWindowClass)
			{
				windowAttributes = (WindowAttributes)34078720u;
			}
			windowAttributes |= WindowAttributes.kWindowLiveResizeAttribute;
			Rect r = default(Rect);
			if (StyleSet(cp.Style, WindowStyles.WS_POPUP))
			{
				SetRect(ref r, (short)x, (short)y, (short)(x + size.Width), (short)(y + size.Height));
			}
			else
			{
				SetRect(ref r, (short)x, (short)(y + MenuBarHeight), (short)(x + size.Width), (short)(y + MenuBarHeight + size.Height));
			}
			CreateNewWindow(windowClass, windowAttributes, ref r, ref window);
			System.Windows.Forms.CarbonInternal.EventHandler.InstallWindowHandler(window);
			HIViewFindByID(HIViewGetRoot(window), new HIViewID(2003398244u, 1u), ref outPtr2);
			HIViewFindByID(HIViewGetRoot(window), new HIViewID(2003398244u, 7u), ref outPtr3);
			HIGrowBoxViewSetTransparent(outPtr3, transparency: true);
			SetAutomaticControlDragTrackingEnabledForWindow(window, enabled: true);
			outPtr = outPtr2;
		}
		HIObjectCreate(__CFStringMakeConstantString("com.novell.mwfview"), 0u, ref hwnd3);
		HIObjectCreate(__CFStringMakeConstantString("com.novell.mwfview"), 0u, ref zero);
		System.Windows.Forms.CarbonInternal.EventHandler.InstallControlHandler(hwnd3);
		System.Windows.Forms.CarbonInternal.EventHandler.InstallControlHandler(zero);
		HIViewChangeFeatures(hwnd3, 2uL, 0uL);
		HIViewChangeFeatures(zero, 2uL, 0uL);
		HIViewNewTrackingArea(hwnd3, IntPtr.Zero, (ulong)(long)hwnd3, ref outRef);
		HIViewNewTrackingArea(zero, IntPtr.Zero, (ulong)(long)zero, ref outRef2);
		HIRect bounds = ((!(window != IntPtr.Zero)) ? new HIRect(x, y, size.Width, size.Height) : new HIRect(0, 0, size.Width, size.Height));
		HIRect bounds2 = new HIRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		HIViewSetFrame(hwnd3, ref bounds);
		HIViewSetFrame(zero, ref bounds2);
		HIViewAddSubview(outPtr, hwnd3);
		HIViewAddSubview(hwnd3, zero);
		hwnd2.WholeWindow = hwnd3;
		hwnd2.ClientWindow = zero;
		if (window != IntPtr.Zero)
		{
			WindowMapping[hwnd2.Handle] = window;
			HandleMapping[window] = hwnd2.Handle;
			if (hwnd2.border_style == FormBorderStyle.FixedToolWindow || hwnd2.border_style == FormBorderStyle.SizableToolWindow)
			{
				UtilityWindows.Add(window);
			}
		}
		Dnd.SetAllowDrop(hwnd2, allow: true);
		Text(hwnd2.Handle, cp.Caption);
		SendMessage(hwnd2.Handle, Msg.WM_CREATE, (IntPtr)1, IntPtr.Zero);
		SendParentNotify(hwnd2.Handle, Msg.WM_CREATE, int.MaxValue, int.MaxValue);
		if (StyleSet(cp.Style, WindowStyles.WS_VISIBLE))
		{
			if (window != IntPtr.Zero)
			{
				if (Control.FromHandle(hwnd2.Handle) is Form)
				{
					Form form = Control.FromHandle(hwnd2.Handle) as Form;
					if (form.WindowState == FormWindowState.Normal)
					{
						SendMessage(hwnd2.Handle, Msg.WM_SHOWWINDOW, (IntPtr)1, IntPtr.Zero);
					}
				}
				ShowWindow(window);
				WaitForHwndMessage(hwnd2, Msg.WM_SHOWWINDOW);
			}
			HIViewSetVisible(hwnd3, visible: true);
			HIViewSetVisible(zero, visible: true);
			hwnd2.visible = true;
			if (!(Control.FromHandle(hwnd2.Handle) is Form))
			{
				SendMessage(hwnd2.Handle, Msg.WM_SHOWWINDOW, (IntPtr)1, IntPtr.Zero);
			}
		}
		if (StyleSet(cp.Style, WindowStyles.WS_MINIMIZE))
		{
			SetWindowState(hwnd2.Handle, FormWindowState.Minimized);
		}
		else if (StyleSet(cp.Style, WindowStyles.WS_MAXIMIZE))
		{
			SetWindowState(hwnd2.Handle, FormWindowState.Maximized);
		}
		return hwnd2.Handle;
	}

	internal override IntPtr CreateWindow(IntPtr Parent, int X, int Y, int Width, int Height)
	{
		CreateParams createParams = new CreateParams();
		createParams.Caption = string.Empty;
		createParams.X = X;
		createParams.Y = Y;
		createParams.Width = Width;
		createParams.Height = Height;
		createParams.ClassName = XplatUI.DefaultClassName;
		createParams.ClassStyle = 0;
		createParams.ExStyle = 0;
		createParams.Parent = IntPtr.Zero;
		createParams.Param = 0;
		return CreateWindow(createParams);
	}

	internal override Bitmap DefineStdCursorBitmap(StdCursor id)
	{
		return System.Windows.Forms.CarbonInternal.Cursor.DefineStdCursorBitmap(id);
	}

	internal override IntPtr DefineCursor(Bitmap bitmap, Bitmap mask, Color cursor_pixel, Color mask_pixel, int xHotSpot, int yHotSpot)
	{
		return System.Windows.Forms.CarbonInternal.Cursor.DefineCursor(bitmap, mask, cursor_pixel, mask_pixel, xHotSpot, yHotSpot);
	}

	internal override IntPtr DefineStdCursor(StdCursor id)
	{
		return System.Windows.Forms.CarbonInternal.Cursor.DefineStdCursor(id);
	}

	internal override IntPtr DefWndProc(ref Message msg)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(msg.HWnd);
		switch ((Msg)msg.Msg)
		{
		case Msg.WM_IME_COMPOSITION:
		{
			string composedString = KeyboardHandler.ComposedString;
			string text = composedString;
			foreach (char c in text)
			{
				SendMessage(msg.HWnd, Msg.WM_IME_CHAR, (IntPtr)c, msg.LParam);
			}
			break;
		}
		case Msg.WM_IME_CHAR:
			SendMessage(msg.HWnd, Msg.WM_CHAR, msg.WParam, msg.LParam);
			return IntPtr.Zero;
		case Msg.WM_QUIT:
			if (WindowMapping[hwnd.Handle] != null)
			{
				Exit();
			}
			break;
		case Msg.WM_PAINT:
			hwnd.expose_pending = false;
			break;
		case Msg.WM_NCPAINT:
			hwnd.nc_expose_pending = false;
			break;
		case Msg.WM_NCCALCSIZE:
			if (msg.WParam == (IntPtr)1)
			{
				XplatUIWin32.NCCALCSIZE_PARAMS nCCALCSIZE_PARAMS = (XplatUIWin32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(msg.LParam, typeof(XplatUIWin32.NCCALCSIZE_PARAMS));
				Control control = Control.FromHandle(hwnd.Handle);
				if (control != null)
				{
					Hwnd.Borders borders = Hwnd.GetBorders(control.GetCreateParams(), null);
					nCCALCSIZE_PARAMS.rgrc1.top += borders.top;
					nCCALCSIZE_PARAMS.rgrc1.bottom -= borders.bottom;
					nCCALCSIZE_PARAMS.rgrc1.left += borders.left;
					nCCALCSIZE_PARAMS.rgrc1.right -= borders.right;
					Marshal.StructureToPtr(nCCALCSIZE_PARAMS, msg.LParam, fDeleteOld: true);
				}
			}
			break;
		case Msg.WM_SETCURSOR:
			while (hwnd.parent != null && msg.Result == IntPtr.Zero)
			{
				hwnd = hwnd.parent;
				msg.Result = NativeWindow.WndProc(hwnd.Handle, Msg.WM_SETCURSOR, msg.HWnd, msg.LParam);
			}
			if (msg.Result == IntPtr.Zero)
			{
				IntPtr handle;
				switch ((HitTest)(msg.LParam.ToInt32() & 0xFFFF))
				{
				case HitTest.HTBOTTOM:
					handle = Cursors.SizeNS.handle;
					break;
				case HitTest.HTBORDER:
					handle = Cursors.SizeNS.handle;
					break;
				case HitTest.HTBOTTOMLEFT:
					handle = Cursors.SizeNESW.handle;
					break;
				case HitTest.HTBOTTOMRIGHT:
					handle = Cursors.SizeNWSE.handle;
					break;
				case HitTest.HTERROR:
					if (msg.LParam.ToInt32() >> 16 == 513)
					{
					}
					handle = Cursors.Default.handle;
					break;
				case HitTest.HTHELP:
					handle = Cursors.Help.handle;
					break;
				case HitTest.HTLEFT:
					handle = Cursors.SizeWE.handle;
					break;
				case HitTest.HTRIGHT:
					handle = Cursors.SizeWE.handle;
					break;
				case HitTest.HTTOP:
					handle = Cursors.SizeNS.handle;
					break;
				case HitTest.HTTOPLEFT:
					handle = Cursors.SizeNWSE.handle;
					break;
				case HitTest.HTTOPRIGHT:
					handle = Cursors.SizeNESW.handle;
					break;
				default:
					handle = Cursors.Default.handle;
					break;
				}
				SetCursor(msg.HWnd, handle);
			}
			return (IntPtr)1;
		}
		return IntPtr.Zero;
	}

	internal override void DestroyCaret(IntPtr hwnd)
	{
		if (Caret.Hwnd == hwnd)
		{
			if (Caret.Visible == 1)
			{
				Caret.Timer.Stop();
				HideCaret();
			}
			Caret.Hwnd = IntPtr.Zero;
			Caret.Visible = 0;
			Caret.On = false;
		}
	}

	[System.MonoTODO]
	internal override void DestroyCursor(IntPtr cursor)
	{
		throw new NotImplementedException();
	}

	internal override void DestroyWindow(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null)
		{
			return;
		}
		SendParentNotify(hwnd.Handle, Msg.WM_DESTROY, int.MaxValue, int.MaxValue);
		CleanupCachedWindows(hwnd);
		ArrayList arrayList = new ArrayList();
		AccumulateDestroyedHandles(Control.ControlNativeWindow.ControlFromHandle(hwnd.Handle), arrayList);
		foreach (Hwnd item in arrayList)
		{
			SendMessage(item.Handle, Msg.WM_DESTROY, IntPtr.Zero, IntPtr.Zero);
			item.zombie = true;
		}
		if (WindowMapping[hwnd.Handle] != null)
		{
			DisposeWindow((IntPtr)WindowMapping[hwnd.Handle]);
			WindowMapping.Remove(hwnd.Handle);
		}
	}

	internal override IntPtr DispatchMessage(ref MSG msg)
	{
		return NativeWindow.WndProc(msg.hwnd, msg.message, msg.wParam, msg.lParam);
	}

	internal override void DoEvents()
	{
		MSG msg = default(MSG);
		in_doevents = true;
		while (PeekMessage(null, ref msg, IntPtr.Zero, 0, 0, 1u))
		{
			TranslateMessage(ref msg);
			DispatchMessage(ref msg);
		}
		in_doevents = false;
	}

	internal override void EnableWindow(IntPtr handle, bool Enable)
	{
	}

	internal override void EndLoop(Thread thread)
	{
	}

	internal void Exit()
	{
		GetMessageResult = false;
	}

	internal override IntPtr GetActive()
	{
		return ActiveWindow;
	}

	internal override Region GetClipRegion(IntPtr hwnd)
	{
		return null;
	}

	[System.MonoTODO]
	internal override void GetCursorInfo(IntPtr cursor, out int width, out int height, out int hotspot_x, out int hotspot_y)
	{
		width = 12;
		height = 12;
		hotspot_x = 0;
		hotspot_y = 0;
	}

	internal override void GetDisplaySize(out Size size)
	{
		HIRect hIRect = CGDisplayBounds(CGMainDisplayID());
		size = new Size((int)hIRect.size.width, (int)hIRect.size.height);
	}

	internal override IntPtr GetParent(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null && hwnd.Parent != null)
		{
			return hwnd.Parent.Handle;
		}
		return IntPtr.Zero;
	}

	internal override IntPtr GetPreviousWindow(IntPtr handle)
	{
		return HIViewGetPreviousView(handle);
	}

	internal override void GetCursorPos(IntPtr handle, out int x, out int y)
	{
		QDPoint outData = default(QDPoint);
		GetGlobalMouse(ref outData);
		x = outData.x;
		y = outData.y;
	}

	internal override IntPtr GetFocus()
	{
		return FocusWindow;
	}

	internal override bool GetFontMetrics(Graphics g, Font font, out int ascent, out int descent)
	{
		FontFamily fontFamily = font.FontFamily;
		ascent = fontFamily.GetCellAscent(font.Style);
		descent = fontFamily.GetCellDescent(font.Style);
		return true;
	}

	internal override Point GetMenuOrigin(IntPtr handle)
	{
		return Hwnd.ObjectFromHandle(handle)?.MenuOrigin ?? Point.Empty;
	}

	internal override bool GetMessage(object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax)
	{
		IntPtr evt = IntPtr.Zero;
		IntPtr eventDispatcherTarget = GetEventDispatcherTarget();
		CheckTimers(DateTime.UtcNow);
		ReceiveNextEvent(0u, IntPtr.Zero, 0.0, processEvt: true, ref evt);
		if (evt != IntPtr.Zero && eventDispatcherTarget != IntPtr.Zero)
		{
			SendEventToEventTarget(evt, eventDispatcherTarget);
			ReleaseEvent(evt);
		}
		lock (queuelock)
		{
			object obj;
			while (true)
			{
				if (MessageQueue.Count <= 0)
				{
					if (Idle != null)
					{
						Idle(this, EventArgs.Empty);
					}
					else if (TimerList.Count == 0)
					{
						ReceiveNextEvent(0u, IntPtr.Zero, 0.15, processEvt: true, ref evt);
						if (evt != IntPtr.Zero && eventDispatcherTarget != IntPtr.Zero)
						{
							SendEventToEventTarget(evt, eventDispatcherTarget);
							ReleaseEvent(evt);
						}
					}
					else
					{
						ReceiveNextEvent(0u, IntPtr.Zero, NextTimeout(), processEvt: true, ref evt);
						if (evt != IntPtr.Zero && eventDispatcherTarget != IntPtr.Zero)
						{
							SendEventToEventTarget(evt, eventDispatcherTarget);
							ReleaseEvent(evt);
						}
					}
					msg.hwnd = IntPtr.Zero;
					msg.message = Msg.WM_ENTERIDLE;
					return GetMessageResult;
				}
				obj = MessageQueue.Dequeue();
				if (!(obj is GCHandle))
				{
					break;
				}
				XplatUIDriverSupport.ExecuteClientMessage((GCHandle)obj);
			}
			msg = (MSG)obj;
		}
		return GetMessageResult;
	}

	[System.MonoTODO]
	internal override bool GetText(IntPtr handle, out string text)
	{
		throw new NotImplementedException();
	}

	internal override void GetWindowPos(IntPtr handle, bool is_toplevel, out int x, out int y, out int width, out int height, out int client_width, out int client_height)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null)
		{
			x = hwnd.x;
			y = hwnd.y;
			width = hwnd.width;
			height = hwnd.height;
			PerformNCCalc(hwnd);
			client_width = hwnd.ClientRect.Width;
			client_height = hwnd.ClientRect.Height;
		}
		else
		{
			x = 0;
			y = 0;
			width = 0;
			height = 0;
			client_width = 0;
			client_height = 0;
		}
	}

	internal override FormWindowState GetWindowState(IntPtr hwnd)
	{
		IntPtr hWnd = HIViewGetWindow(hwnd);
		if (IsWindowCollapsed(hWnd))
		{
			return FormWindowState.Minimized;
		}
		if (IsWindowInStandardState(hWnd, IntPtr.Zero, IntPtr.Zero))
		{
			return FormWindowState.Maximized;
		}
		return FormWindowState.Normal;
	}

	internal override void GrabInfo(out IntPtr handle, out bool GrabConfined, out Rectangle GrabArea)
	{
		handle = Grab.Hwnd;
		GrabConfined = Grab.Confined;
		GrabArea = Grab.Area;
	}

	internal override void GrabWindow(IntPtr handle, IntPtr confine_to_handle)
	{
		Grab.Hwnd = handle;
		Grab.Confined = confine_to_handle != IntPtr.Zero;
	}

	internal override void UngrabWindow(IntPtr hwnd)
	{
		bool flag = Grab.Hwnd != IntPtr.Zero;
		Grab.Hwnd = IntPtr.Zero;
		Grab.Confined = false;
		if (flag)
		{
			SendMessage(hwnd, Msg.WM_CAPTURECHANGED, IntPtr.Zero, IntPtr.Zero);
		}
	}

	internal override void HandleException(Exception e)
	{
		StackTrace stackTrace = new StackTrace(e);
		Console.WriteLine("Exception '{0}'", e.Message + stackTrace.ToString());
		Console.WriteLine("{0}{1}", e.Message, stackTrace.ToString());
	}

	internal override void Invalidate(IntPtr handle, Rectangle rc, bool clear)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (clear)
		{
			AddExpose(hwnd, client: true, hwnd.X, hwnd.Y, hwnd.Width, hwnd.Height);
		}
		else
		{
			AddExpose(hwnd, client: true, rc.X, rc.Y, rc.Width, rc.Height);
		}
	}

	internal override void InvalidateNC(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		AddExpose(hwnd, client: false, 0, 0, hwnd.Width, hwnd.Height);
	}

	internal override bool IsEnabled(IntPtr handle)
	{
		return Hwnd.ObjectFromHandle(handle).Enabled;
	}

	internal override bool IsVisible(IntPtr handle)
	{
		return Hwnd.ObjectFromHandle(handle).visible;
	}

	internal override void KillTimer(Timer timer)
	{
		lock (TimerList)
		{
			TimerList.Remove(timer);
		}
	}

	internal override void OverrideCursor(IntPtr cursor)
	{
	}

	internal override PaintEventArgs PaintEventStart(ref Message msg, IntPtr handle, bool client)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(msg.HWnd);
		Hwnd hwnd2 = ((!(msg.HWnd == handle)) ? Hwnd.ObjectFromHandle(handle) : hwnd);
		if (Caret.Visible == 1)
		{
			Caret.Paused = true;
			HideCaret();
		}
		PaintEventArgs paintEventArgs;
		if (client)
		{
			Graphics graphics = Graphics.FromHwnd(hwnd2.client_window);
			Region region = new Region();
			region.MakeEmpty();
			Rectangle[] clipRectangles = hwnd.ClipRectangles;
			foreach (Rectangle rect in clipRectangles)
			{
				region.Union(rect);
			}
			if (hwnd.UserClip != null)
			{
				region.Intersect(hwnd.UserClip);
			}
			graphics.Clip = region;
			paintEventArgs = new PaintEventArgs(graphics, hwnd.Invalid);
			hwnd.expose_pending = false;
			hwnd.ClearInvalidArea();
			hwnd.drawing_stack.Push(paintEventArgs);
			hwnd.drawing_stack.Push(graphics);
		}
		else
		{
			Graphics graphics = Graphics.FromHwnd(hwnd2.whole_window);
			if (!hwnd.nc_invalid.IsEmpty)
			{
				graphics.SetClip(hwnd.nc_invalid);
				paintEventArgs = new PaintEventArgs(graphics, hwnd.nc_invalid);
			}
			else
			{
				paintEventArgs = new PaintEventArgs(graphics, new Rectangle(0, 0, hwnd.width, hwnd.height));
			}
			hwnd.nc_expose_pending = false;
			hwnd.ClearNcInvalidArea();
			hwnd.drawing_stack.Push(paintEventArgs);
			hwnd.drawing_stack.Push(graphics);
		}
		return paintEventArgs;
	}

	internal override void PaintEventEnd(ref Message msg, IntPtr handle, bool client)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		try
		{
			Graphics graphics = (Graphics)hwnd.drawing_stack.Pop();
			graphics.Flush();
			graphics.Dispose();
			PaintEventArgs paintEventArgs = (PaintEventArgs)hwnd.drawing_stack.Pop();
			paintEventArgs.SetGraphics(null);
			paintEventArgs.Dispose();
		}
		catch
		{
		}
		if (Caret.Visible == 1)
		{
			ShowCaret();
			Caret.Paused = false;
		}
	}

	internal override bool PeekMessage(object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax, uint flags)
	{
		IntPtr evt = IntPtr.Zero;
		IntPtr eventDispatcherTarget = GetEventDispatcherTarget();
		CheckTimers(DateTime.UtcNow);
		ReceiveNextEvent(0u, IntPtr.Zero, 0.0, processEvt: true, ref evt);
		if (evt != IntPtr.Zero && eventDispatcherTarget != IntPtr.Zero)
		{
			SendEventToEventTarget(evt, eventDispatcherTarget);
			ReleaseEvent(evt);
		}
		lock (queuelock)
		{
			if (MessageQueue.Count <= 0)
			{
				return false;
			}
			object obj = ((flags != 1) ? MessageQueue.Peek() : MessageQueue.Dequeue());
			if (obj is GCHandle)
			{
				XplatUIDriverSupport.ExecuteClientMessage((GCHandle)obj);
				return false;
			}
			msg = (MSG)obj;
			return true;
		}
	}

	internal override bool PostMessage(IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam)
	{
		MSG msg = default(MSG);
		msg.hwnd = hwnd;
		msg.message = message;
		msg.wParam = wParam;
		msg.lParam = lParam;
		EnqueueMessage(msg);
		return true;
	}

	internal override void PostQuitMessage(int exitCode)
	{
		PostMessage(FosterParent, Msg.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
	}

	internal override void RequestAdditionalWM_NCMessages(IntPtr hwnd, bool hover, bool leave)
	{
	}

	internal override void RequestNCRecalc(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null)
		{
			PerformNCCalc(hwnd);
			SendMessage(handle, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
			InvalidateNC(handle);
		}
	}

	[System.MonoTODO]
	internal override void ResetMouseHover(IntPtr handle)
	{
		throw new NotImplementedException();
	}

	internal override void ScreenToClient(IntPtr handle, ref int x, ref int y)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Point point = ConvertScreenPointToClient(hwnd.ClientWindow, new Point(x, y));
		x = point.X;
		y = point.Y;
	}

	internal override void ScreenToMenu(IntPtr handle, ref int x, ref int y)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Point point = ConvertScreenPointToClient(hwnd.WholeWindow, new Point(x, y));
		x = point.X;
		y = point.Y;
	}

	internal override void ScrollWindow(IntPtr handle, Rectangle area, int XAmount, int YAmount, bool clear)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Invalidate(handle, new Rectangle(0, 0, hwnd.Width, hwnd.Height), clear: false);
	}

	internal override void ScrollWindow(IntPtr handle, int XAmount, int YAmount, bool clear)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Invalidate(handle, new Rectangle(0, 0, hwnd.Width, hwnd.Height), clear: false);
	}

	[System.MonoTODO]
	internal override void SendAsyncMethod(AsyncMethodData method)
	{
		lock (queuelock)
		{
			MessageQueue.Enqueue(GCHandle.Alloc(method));
		}
	}

	[System.MonoTODO]
	internal override IntPtr SendMessage(IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam)
	{
		return NativeWindow.WndProc(hwnd, message, wParam, lParam);
	}

	internal override int SendInput(IntPtr hwnd, Queue keys)
	{
		return 0;
	}

	internal override void SetCaretPos(IntPtr hwnd, int x, int y)
	{
		if (hwnd != IntPtr.Zero && hwnd == Caret.Hwnd)
		{
			Caret.X = x;
			Caret.Y = y;
			ClientToScreen(hwnd, ref x, ref y);
			SizeWindow(new Rectangle(x, y, Caret.Width, Caret.Height), CaretWindow);
			Caret.Timer.Stop();
			HideCaret();
			if (Caret.Visible == 1)
			{
				ShowCaret();
				Caret.Timer.Start();
			}
		}
	}

	internal override void SetClipRegion(IntPtr hwnd, Region region)
	{
		throw new NotImplementedException();
	}

	internal override void SetCursor(IntPtr window, IntPtr cursor)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(window);
		hwnd.Cursor = cursor;
	}

	internal override void SetCursorPos(IntPtr handle, int x, int y)
	{
		CGDisplayMoveCursorToPoint(CGMainDisplayID(), new CGPoint(x, y));
	}

	internal override void SetFocus(IntPtr handle)
	{
		if (FocusWindow != IntPtr.Zero)
		{
			PostMessage(FocusWindow, Msg.WM_KILLFOCUS, handle, IntPtr.Zero);
		}
		PostMessage(handle, Msg.WM_SETFOCUS, FocusWindow, IntPtr.Zero);
		FocusWindow = handle;
	}

	internal override void SetIcon(IntPtr handle, Icon icon)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (WindowMapping[hwnd.Handle] == null)
		{
			return;
		}
		if (icon == null)
		{
			RestoreApplicationDockTileImage();
			return;
		}
		Bitmap bitmap = new Bitmap(128, 128);
		using (Graphics graphics = Graphics.FromImage(bitmap))
		{
			graphics.DrawImage(icon.ToBitmap(), 0, 0, 128, 128);
		}
		int num = 0;
		int num2 = bitmap.Width * bitmap.Height;
		IntPtr[] array = new IntPtr[num2];
		for (int i = 0; i < bitmap.Height; i++)
		{
			for (int j = 0; j < bitmap.Width; j++)
			{
				int num3 = bitmap.GetPixel(j, i).ToArgb();
				if (BitConverter.IsLittleEndian)
				{
					byte b = (byte)((uint)(num3 >> 24) & 0xFFu);
					byte b2 = (byte)((uint)(num3 >> 16) & 0xFFu);
					byte b3 = (byte)((uint)(num3 >> 8) & 0xFFu);
					byte b4 = (byte)((uint)num3 & 0xFFu);
					ref IntPtr reference = ref array[num++];
					reference = (IntPtr)(b + (b2 << 8) + (b3 << 16) + (b4 << 24));
				}
				else
				{
					ref IntPtr reference2 = ref array[num++];
					reference2 = (IntPtr)num3;
				}
			}
		}
		IntPtr provider = CGDataProviderCreateWithData(IntPtr.Zero, array, num2 * 4, IntPtr.Zero);
		IntPtr applicationDockTileImage = CGImageCreate(128, 128, 8, 32, 512, CGColorSpaceCreateDeviceRGB(), 4u, provider, IntPtr.Zero, 0, 0);
		SetApplicationDockTileImage(applicationDockTileImage);
	}

	internal override void SetModal(IntPtr handle, bool Modal)
	{
		IntPtr window = HIViewGetWindow(Hwnd.ObjectFromHandle(handle).WholeWindow);
		if (Modal)
		{
			BeginAppModalStateForWindow(window);
		}
		else
		{
			EndAppModalStateForWindow(window);
		}
	}

	internal override IntPtr SetParent(IntPtr handle, IntPtr parent)
	{
		IntPtr outPtr = IntPtr.Zero;
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		hwnd.Parent = Hwnd.ObjectFromHandle(parent);
		if (HIViewGetSuperview(hwnd.whole_window) != IntPtr.Zero)
		{
			HIViewRemoveFromSuperview(hwnd.whole_window);
		}
		if (hwnd.parent == null)
		{
			HIViewFindByID(HIViewGetRoot(FosterParent), new HIViewID(2003398244u, 1u), ref outPtr);
		}
		HIViewAddSubview((hwnd.parent != null) ? hwnd.Parent.client_window : outPtr, hwnd.whole_window);
		HIViewPlaceInSuperviewAt(hwnd.whole_window, hwnd.X, hwnd.Y);
		HIViewAddSubview(hwnd.whole_window, hwnd.client_window);
		HIViewPlaceInSuperviewAt(hwnd.client_window, hwnd.ClientRect.X, hwnd.ClientRect.Y);
		return IntPtr.Zero;
	}

	internal override void SetTimer(Timer timer)
	{
		lock (TimerList)
		{
			TimerList.Add(timer);
		}
	}

	internal override bool SetTopmost(IntPtr hWnd, bool Enabled)
	{
		HIViewSetZOrder(hWnd, 1, IntPtr.Zero);
		return true;
	}

	internal override bool SetOwner(IntPtr hWnd, IntPtr hWndOwner)
	{
		return true;
	}

	internal override bool SetVisible(IntPtr handle, bool visible, bool activate)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		object obj = WindowMapping[hwnd.Handle];
		if (obj != null)
		{
			if (visible)
			{
				ShowWindow((IntPtr)obj);
			}
			else
			{
				HideWindow((IntPtr)obj);
			}
		}
		if (visible)
		{
			SendMessage(handle, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
		}
		HIViewSetVisible(hwnd.whole_window, visible);
		HIViewSetVisible(hwnd.client_window, visible);
		hwnd.visible = visible;
		hwnd.Mapped = true;
		return true;
	}

	internal override void SetAllowDrop(IntPtr handle, bool value)
	{
	}

	internal override DragDropEffects StartDrag(IntPtr handle, object data, DragDropEffects allowed_effects)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null)
		{
			throw new ArgumentException("Attempt to begin drag from invalid window handle (" + handle.ToInt32() + ").");
		}
		return Dnd.StartDrag(hwnd.client_window, data, allowed_effects);
	}

	internal override void SetBorderStyle(IntPtr handle, FormBorderStyle border_style)
	{
		if (Control.FromHandle(handle) is Form form && form.window_manager == null && (border_style == FormBorderStyle.FixedToolWindow || border_style == FormBorderStyle.SizableToolWindow))
		{
			form.window_manager = new ToolWindowManager(form);
		}
		RequestNCRecalc(handle);
	}

	internal override void SetMenu(IntPtr handle, Menu menu)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		hwnd.menu = menu;
		RequestNCRecalc(handle);
	}

	internal override void SetWindowMinMax(IntPtr handle, Rectangle maximized, Size min, Size max)
	{
	}

	internal override void SetWindowPos(IntPtr handle, int x, int y, int width, int height)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null)
		{
			return;
		}
		if (width < 0)
		{
			width = 0;
		}
		if (height < 0)
		{
			height = 0;
		}
		if (hwnd.zero_sized && width > 0 && height > 0)
		{
			if (hwnd.visible)
			{
				HIViewSetVisible(hwnd.WholeWindow, visible: true);
			}
			hwnd.zero_sized = false;
		}
		if (width < 1 || height < 1)
		{
			hwnd.zero_sized = true;
			HIViewSetVisible(hwnd.WholeWindow, visible: false);
		}
		if (hwnd.x == x && hwnd.y == y && hwnd.width == width && hwnd.height == height)
		{
			return;
		}
		if (!hwnd.zero_sized)
		{
			hwnd.x = x;
			hwnd.y = y;
			hwnd.width = width;
			hwnd.height = height;
			SendMessage(hwnd.client_window, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
			Control control = Control.FromHandle(handle);
			CreateParams createParams = control.GetCreateParams();
			Size size = TranslateWindowSizeToQuartzWindowSize(createParams, new Size(width, height));
			Rect r = default(Rect);
			if (WindowMapping[hwnd.Handle] != null)
			{
				if (StyleSet(createParams.Style, WindowStyles.WS_POPUP))
				{
					SetRect(ref r, (short)x, (short)y, (short)(x + size.Width), (short)(y + size.Height));
				}
				else
				{
					SetRect(ref r, (short)x, (short)(y + MenuBarHeight), (short)(x + size.Width), (short)(y + MenuBarHeight + size.Height));
				}
				SetWindowBounds((IntPtr)WindowMapping[hwnd.Handle], 33u, ref r);
				HIRect bounds = new HIRect(0, 0, size.Width, size.Height);
				HIViewSetFrame(hwnd.whole_window, ref bounds);
				SetCaretPos(Caret.Hwnd, Caret.X, Caret.Y);
			}
			else
			{
				HIRect bounds2 = new HIRect(x, y, size.Width, size.Height);
				HIViewSetFrame(hwnd.whole_window, ref bounds2);
			}
			PerformNCCalc(hwnd);
		}
		hwnd.x = x;
		hwnd.y = y;
		hwnd.width = width;
		hwnd.height = height;
	}

	internal override void SetWindowState(IntPtr handle, FormWindowState state)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		IntPtr hWnd = HIViewGetWindow(handle);
		switch (state)
		{
		case FormWindowState.Minimized:
			CollapseWindow(hWnd, collapse: true);
			break;
		case FormWindowState.Normal:
			ZoomWindow(hWnd, 7, front: false);
			break;
		case FormWindowState.Maximized:
			if (Control.FromHandle(hwnd.Handle) is Form form && form.FormBorderStyle == FormBorderStyle.None)
			{
				Rect r = default(Rect);
				HIRect bounds = CGDisplayBounds(CGMainDisplayID());
				SetRect(ref r, 0, 0, (short)bounds.size.width, (short)bounds.size.height);
				SetWindowBounds((IntPtr)WindowMapping[hwnd.Handle], 33u, ref r);
				HIViewSetFrame(hwnd.whole_window, ref bounds);
			}
			else
			{
				ZoomWindow(hWnd, 8, front: false);
			}
			break;
		}
	}

	internal override void SetWindowStyle(IntPtr handle, CreateParams cp)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		SetHwndStyles(hwnd, cp);
		if (WindowMapping[hwnd.Handle] != null)
		{
			WindowAttributes windowAttributes = (WindowAttributes)34078720u;
			if (((uint)cp.Style & 0x20000u) != 0)
			{
				windowAttributes |= WindowAttributes.kWindowCollapseBoxAttribute;
			}
			if (((uint)cp.Style & 0x10000u) != 0)
			{
				windowAttributes |= (WindowAttributes)22u;
			}
			if (((uint)cp.Style & 0x80000u) != 0)
			{
				windowAttributes |= WindowAttributes.kWindowCloseBoxAttribute;
			}
			if (((uint)cp.ExStyle & 0x80u) != 0)
			{
				windowAttributes = (WindowAttributes)34078720u;
			}
			windowAttributes |= WindowAttributes.kWindowLiveResizeAttribute;
			WindowAttributes outAttributes = WindowAttributes.kWindowNoAttributes;
			GetWindowAttributes((IntPtr)WindowMapping[hwnd.Handle], ref outAttributes);
			ChangeWindowAttributes((IntPtr)WindowMapping[hwnd.Handle], windowAttributes, outAttributes);
		}
	}

	internal override void SetWindowTransparency(IntPtr handle, double transparency, Color key)
	{
	}

	internal override double GetWindowTransparency(IntPtr handle)
	{
		return 1.0;
	}

	internal override TransparencySupport SupportsTransparency()
	{
		return TransparencySupport.None;
	}

	internal override bool SetZOrder(IntPtr handle, IntPtr after_handle, bool Top, bool Bottom)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (Top)
		{
			HIViewSetZOrder(hwnd.whole_window, 2, IntPtr.Zero);
			return true;
		}
		if (!Bottom)
		{
			Hwnd hwnd2 = Hwnd.ObjectFromHandle(after_handle);
			HIViewSetZOrder(hwnd.whole_window, 2, (!(after_handle == IntPtr.Zero)) ? hwnd2.whole_window : IntPtr.Zero);
			return false;
		}
		HIViewSetZOrder(hwnd.whole_window, 1, IntPtr.Zero);
		return true;
	}

	internal override void ShowCursor(bool show)
	{
		if (show)
		{
			CGDisplayShowCursor(CGMainDisplayID());
		}
		else
		{
			CGDisplayHideCursor(CGMainDisplayID());
		}
	}

	internal override object StartLoop(Thread thread)
	{
		return new object();
	}

	[System.MonoTODO]
	internal override bool SystrayAdd(IntPtr hwnd, string tip, Icon icon, out ToolTip tt)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	internal override bool SystrayChange(IntPtr hwnd, string tip, Icon icon, ref ToolTip tt)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	internal override void SystrayRemove(IntPtr hwnd, ref ToolTip tt)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	internal override void SystrayBalloon(IntPtr hwnd, int timeout, string title, string text, ToolTipIcon icon)
	{
		throw new NotImplementedException();
	}

	internal override bool Text(IntPtr handle, string text)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (WindowMapping[hwnd.Handle] != null)
		{
			SetWindowTitleWithCFString((IntPtr)WindowMapping[hwnd.Handle], __CFStringMakeConstantString(text));
		}
		SetControlTitleWithCFString(hwnd.whole_window, __CFStringMakeConstantString(text));
		SetControlTitleWithCFString(hwnd.client_window, __CFStringMakeConstantString(text));
		return true;
	}

	internal override void UpdateWindow(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd.visible && HIViewIsVisible(handle))
		{
			SendMessage(handle, Msg.WM_PAINT, IntPtr.Zero, IntPtr.Zero);
		}
	}

	internal override bool TranslateMessage(ref MSG msg)
	{
		return System.Windows.Forms.CarbonInternal.EventHandler.TranslateMessage(ref msg);
	}

	internal void SizeWindow(Rectangle rect, IntPtr window)
	{
		Rect r = default(Rect);
		SetRect(ref r, (short)rect.X, (short)rect.Y, (short)(rect.X + rect.Width), (short)(rect.Y + rect.Height));
		SetWindowBounds(window, 33u, ref r);
	}

	internal override void DrawReversibleLine(Point start, Point end, Color backColor)
	{
	}

	internal override void FillReversibleRectangle(Rectangle rectangle, Color backColor)
	{
	}

	internal override void DrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style)
	{
	}

	internal override void DrawReversibleRectangle(IntPtr handle, Rectangle rect, int line_width)
	{
		Rectangle rect2 = rect;
		int x = 0;
		int y = 0;
		if (ReverseWindowMapped)
		{
			HideWindow(ReverseWindow);
			ReverseWindowMapped = false;
			return;
		}
		ClientToScreen(handle, ref x, ref y);
		rect2.X += x;
		rect2.Y += y;
		SizeWindow(rect2, ReverseWindow);
		ShowWindow(ReverseWindow);
		rect.X = 0;
		rect.Y = 0;
		rect.Width--;
		rect.Height--;
		Graphics graphics = Graphics.FromHwnd(HIViewGetRoot(ReverseWindow));
		for (int i = 0; i < line_width; i++)
		{
			graphics.DrawRectangle(ThemeEngine.Current.ResPool.GetPen(Color.Black), rect);
			rect.X++;
			rect.Y++;
			rect.Width--;
			rect.Height--;
		}
		graphics.Flush();
		graphics.Dispose();
		ReverseWindowMapped = true;
	}

	internal override SizeF GetAutoScaleSize(Font font)
	{
		string text = "The quick brown fox jumped over the lazy dog.";
		double num = 44.54999694824219;
		Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
		float width = (float)((double)graphics.MeasureString(text, font).Width / num);
		return new SizeF(width, font.Height);
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewConvertPoint(ref CGPoint point, IntPtr pView, IntPtr cView);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewChangeFeatures(IntPtr aView, ulong bitsin, ulong bitsout);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewFindByID(IntPtr rootWnd, HIViewID id, ref IntPtr outPtr);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIGrowBoxViewSetTransparent(IntPtr GrowBox, bool transparency);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr HIViewGetRoot(IntPtr hWnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIObjectCreate(IntPtr cfStr, uint what, ref IntPtr hwnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIObjectRegisterSubclass(IntPtr classid, IntPtr superclassid, uint options, EventDelegate upp, uint count, EventTypeSpec[] list, IntPtr state, ref IntPtr cls);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewPlaceInSuperviewAt(IntPtr view, float x, float y);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewAddSubview(IntPtr parentHnd, IntPtr childHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr HIViewGetPreviousView(IntPtr aView);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr HIViewGetSuperview(IntPtr aView);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewRemoveFromSuperview(IntPtr aView);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewSetVisible(IntPtr vHnd, bool visible);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern bool HIViewIsVisible(IntPtr vHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewGetBounds(IntPtr vHnd, ref HIRect r);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewScrollRect(IntPtr vHnd, ref HIRect rect, float x, float y);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewSetZOrder(IntPtr hWnd, int cmd, IntPtr oHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewNewTrackingArea(IntPtr inView, IntPtr inShape, ulong inID, ref IntPtr outRef);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr HIViewGetWindow(IntPtr aView);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewSetFrame(IntPtr view_handle, ref HIRect bounds);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int HIViewSetNeedsDisplayInRect(IntPtr view_handle, ref HIRect rect, bool needs_display);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void SetRect(ref Rect r, short left, short top, short right, short bottom);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int ActivateWindow(IntPtr windowHnd, bool inActivate);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern bool IsWindowActive(IntPtr windowHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetAutomaticControlDragTrackingEnabledForWindow(IntPtr window, bool enabled);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr GetEventDispatcherTarget();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SendEventToEventTarget(IntPtr evt, IntPtr target);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int ReleaseEvent(IntPtr evt);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int ReceiveNextEvent(uint evtCount, IntPtr evtTypes, double timeout, bool processEvt, ref IntPtr evt);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern bool IsWindowCollapsed(IntPtr hWnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern bool IsWindowInStandardState(IntPtr hWnd, IntPtr a, IntPtr b);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void CollapseWindow(IntPtr hWnd, bool collapse);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void ZoomWindow(IntPtr hWnd, short partCode, bool front);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetWindowAttributes(IntPtr hWnd, ref WindowAttributes outAttributes);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int ChangeWindowAttributes(IntPtr hWnd, WindowAttributes inAttributes, WindowAttributes outAttributes);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int GetGlobalMouse(ref QDPoint outData);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int BeginAppModalStateForWindow(IntPtr window);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int EndAppModalStateForWindow(IntPtr window);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int CreateNewWindow(WindowClass klass, WindowAttributes attributes, ref Rect r, ref IntPtr window);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int DisposeWindow(IntPtr wHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int ShowWindow(IntPtr wHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int HideWindow(IntPtr wHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern bool IsWindowVisible(IntPtr wHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetWindowBounds(IntPtr wHnd, uint reg, ref Rect rect);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetWindowBounds(IntPtr wHnd, uint reg, ref Rect rect);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetControlTitleWithCFString(IntPtr hWnd, IntPtr titleCFStr);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetWindowTitleWithCFString(IntPtr hWnd, IntPtr titleCFStr);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr __CFStringMakeConstantString(string cString);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int CFRelease(IntPtr wHnd);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern short GetMBarHeight();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void AlertSoundPlay();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern HIRect CGDisplayBounds(IntPtr displayID);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr CGMainDisplayID();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void CGDisplayShowCursor(IntPtr display);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void CGDisplayHideCursor(IntPtr display);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void CGDisplayMoveCursorToPoint(IntPtr display, CGPoint point);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetCurrentProcess(ref ProcessSerialNumber psn);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int TransformProcessType(ref ProcessSerialNumber psn, uint type);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetFrontProcess(ref ProcessSerialNumber psn);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr CGColorSpaceCreateDeviceRGB();

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr CGDataProviderCreateWithData(IntPtr info, IntPtr[] data, int size, IntPtr releasefunc);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern IntPtr CGImageCreate(int width, int height, int bitsPerComponent, int bitsPerPixel, int bytesPerRow, IntPtr colorspace, uint bitmapInfo, IntPtr provider, IntPtr decode, int shouldInterpolate, int intent);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void SetApplicationDockTileImage(IntPtr imageRef);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern void RestoreApplicationDockTileImage();
}
