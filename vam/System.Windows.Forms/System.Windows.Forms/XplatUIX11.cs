using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Mono.Unix.Native;

namespace System.Windows.Forms;

internal class XplatUIX11 : XplatUIDriver
{
	internal class XException : ApplicationException
	{
		private IntPtr Display;

		private IntPtr ResourceID;

		private IntPtr Serial;

		private XRequest RequestCode;

		private byte ErrorCode;

		private byte MinorCode;

		public override string Message => GetMessage(Display, ResourceID, Serial, ErrorCode, RequestCode, MinorCode);

		public XException(IntPtr Display, IntPtr ResourceID, IntPtr Serial, byte ErrorCode, XRequest RequestCode, byte MinorCode)
		{
			this.Display = Display;
			this.ResourceID = ResourceID;
			this.Serial = Serial;
			this.RequestCode = RequestCode;
			this.ErrorCode = ErrorCode;
			this.MinorCode = MinorCode;
		}

		public static string GetMessage(IntPtr Display, IntPtr ResourceID, IntPtr Serial, byte ErrorCode, XRequest RequestCode, byte MinorCode)
		{
			StringBuilder stringBuilder = new StringBuilder(160);
			XGetErrorText(Display, ErrorCode, stringBuilder, stringBuilder.Capacity);
			string text = stringBuilder.ToString();
			Hwnd hwnd = Hwnd.ObjectFromHandle(ResourceID);
			string text2;
			string text3;
			if (hwnd != null)
			{
				text2 = hwnd.ToString();
				Control control = Control.FromHandle(hwnd.Handle);
				text3 = ((control == null) ? $"<handle {hwnd.Handle.ToInt32():X} non-existant>" : control.ToString());
			}
			else
			{
				text2 = "<null>";
				text3 = "<null>";
			}
			return $"\n  Error: {text}\n  Request:     {RequestCode:D} ({MinorCode})\n  Resource ID: 0x{ResourceID.ToInt32():X}\n  Serial:      {Serial}\n  Hwnd:        {text2}\n  Control:     {text3}";
		}
	}

	private delegate bool EventPredicate(IntPtr display, ref XEvent xevent, IntPtr arg);

	private delegate IntPtr WndProcDelegate(IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam);

	private const EventMask SelectInputMask = EventMask.KeyPressMask | EventMask.KeyReleaseMask | EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.EnterWindowMask | EventMask.LeaveWindowMask | EventMask.PointerMotionMask | EventMask.PointerMotionHintMask | EventMask.ExposureMask | EventMask.SubstructureNotifyMask | EventMask.FocusChangeMask;

	private static volatile XplatUIX11 Instance;

	private static int RefCount;

	private static object XlibLock;

	private static bool themes_enabled;

	private static IntPtr DisplayHandle;

	private static int ScreenNo;

	private static IntPtr DefaultColormap;

	private static IntPtr CustomVisual;

	private static IntPtr CustomColormap;

	private static IntPtr RootWindow;

	private static IntPtr FosterParent;

	private static XErrorHandler ErrorHandler;

	private static bool ErrorExceptions;

	private int render_major_opcode;

	private int render_first_event;

	private int render_first_error;

	private static IntPtr ClipMagic;

	private static ClipboardData Clipboard;

	private static IntPtr PostAtom;

	private static IntPtr AsyncAtom;

	private static Hashtable MessageQueues;

	private static ArrayList unattached_timer_list;

	private static Pollfd[] pollfds;

	private static bool wake_waiting;

	private static object wake_waiting_lock = new object();

	private static X11Keyboard Keyboard;

	private static X11Dnd Dnd;

	private static Socket listen;

	private static Socket wake;

	private static Socket wake_receive;

	private static byte[] network_buffer;

	private static bool detectable_key_auto_repeat;

	private static IntPtr ActiveWindow;

	private static IntPtr FocusWindow;

	private static Stack ModalWindows;

	private static IntPtr SystrayMgrWindow;

	private static IntPtr LastCursorWindow;

	private static IntPtr LastCursorHandle;

	private static IntPtr OverrideCursorHandle;

	private static CaretStruct Caret;

	private static IntPtr LastPointerWindow;

	private static IntPtr WM_PROTOCOLS;

	private static IntPtr WM_DELETE_WINDOW;

	private static IntPtr WM_TAKE_FOCUS;

	private static IntPtr _NET_DESKTOP_GEOMETRY;

	private static IntPtr _NET_CURRENT_DESKTOP;

	private static IntPtr _NET_ACTIVE_WINDOW;

	private static IntPtr _NET_WORKAREA;

	private static IntPtr _NET_WM_NAME;

	private static IntPtr _NET_WM_WINDOW_TYPE;

	private static IntPtr _NET_WM_STATE;

	private static IntPtr _NET_WM_ICON;

	private static IntPtr _NET_WM_USER_TIME;

	private static IntPtr _NET_FRAME_EXTENTS;

	private static IntPtr _NET_SYSTEM_TRAY_S;

	private static IntPtr _NET_SYSTEM_TRAY_OPCODE;

	private static IntPtr _NET_WM_STATE_MAXIMIZED_HORZ;

	private static IntPtr _NET_WM_STATE_MAXIMIZED_VERT;

	private static IntPtr _XEMBED;

	private static IntPtr _XEMBED_INFO;

	private static IntPtr _MOTIF_WM_HINTS;

	private static IntPtr _NET_WM_STATE_SKIP_TASKBAR;

	private static IntPtr _NET_WM_STATE_ABOVE;

	private static IntPtr _NET_WM_STATE_MODAL;

	private static IntPtr _NET_WM_STATE_HIDDEN;

	private static IntPtr _NET_WM_CONTEXT_HELP;

	private static IntPtr _NET_WM_WINDOW_OPACITY;

	private static IntPtr _NET_WM_WINDOW_TYPE_UTILITY;

	private static IntPtr _NET_WM_WINDOW_TYPE_NORMAL;

	private static IntPtr CLIPBOARD;

	private static IntPtr PRIMARY;

	private static IntPtr OEMTEXT;

	private static IntPtr UTF8_STRING;

	private static IntPtr UTF16_STRING;

	private static IntPtr RICHTEXTFORMAT;

	private static IntPtr TARGETS;

	private static HoverStruct HoverState;

	private static ClickStruct ClickPending;

	private static GrabStruct Grab;

	private Point mouse_position;

	internal static MouseButtons MouseState;

	internal static bool in_doevents;

	private static int DoubleClickInterval;

	private static readonly object lockobj = new object();

	private static Hashtable messageHold;

	public int Reference => RefCount;

	internal static IntPtr Display
	{
		get
		{
			return DisplayHandle;
		}
		set
		{
			GetInstance().SetDisplay(value);
		}
	}

	internal static int Screen
	{
		get
		{
			return ScreenNo;
		}
		set
		{
			ScreenNo = value;
		}
	}

	internal static IntPtr RootWindowHandle
	{
		get
		{
			return RootWindow;
		}
		set
		{
			RootWindow = value;
		}
	}

	internal static IntPtr Visual
	{
		get
		{
			return CustomVisual;
		}
		set
		{
			CustomVisual = value;
		}
	}

	internal static IntPtr ColorMap
	{
		get
		{
			return CustomColormap;
		}
		set
		{
			CustomColormap = value;
		}
	}

	internal override int CaptionHeight => 19;

	internal override Size CursorSize
	{
		get
		{
			if (XQueryBestCursor(DisplayHandle, RootWindow, 32, 32, out var best_width, out var best_height) != 0)
			{
				return new Size(best_width, best_height);
			}
			return new Size(16, 16);
		}
	}

	internal override bool DragFullWindows => true;

	internal override Size DragSize => new Size(4, 4);

	internal override Size FrameBorderSize => new Size(4, 4);

	internal override Size IconSize
	{
		get
		{
			if (XGetIconSizes(DisplayHandle, RootWindow, out var size_list, out var count) != 0)
			{
				long num = (long)size_list;
				int num2 = 0;
				XIconSize xIconSize = default(XIconSize);
				for (int i = 0; i < count; i++)
				{
					xIconSize = (XIconSize)Marshal.PtrToStructure((IntPtr)num, xIconSize.GetType());
					num += Marshal.SizeOf(xIconSize);
					if (xIconSize.min_width == 32)
					{
						XFree(size_list);
						return new Size(32, 32);
					}
					if (xIconSize.max_width == 32)
					{
						XFree(size_list);
						return new Size(32, 32);
					}
					if (xIconSize.min_width < 32 && xIconSize.max_width > 32)
					{
						int num3 = xIconSize.min_width;
						while (num3 < xIconSize.max_width)
						{
							num3 += xIconSize.width_inc;
							if (num3 == 32)
							{
								XFree(size_list);
								return new Size(32, 32);
							}
						}
					}
					if (num2 < xIconSize.max_width)
					{
						num2 = xIconSize.max_width;
					}
				}
				return new Size(num2, num2);
			}
			return new Size(32, 32);
		}
	}

	internal override int KeyboardSpeed => 0;

	internal override int KeyboardDelay => 1;

	internal override Size MaxWindowTrackSize => new Size(WorkingArea.Width, WorkingArea.Height);

	internal override bool MenuAccessKeysUnderlined => false;

	internal override Size MinimizedWindowSpacingSize => new Size(1, 1);

	internal override Size MinimumWindowSize => new Size(110, 22);

	internal override Size MinimumFixedToolWindowSize => new Size(27, 22);

	internal override Size MinimumSizeableToolWindowSize => new Size(37, 22);

	internal override Size MinimumNoBorderWindowSize => new Size(2, 2);

	internal override Keys ModifierKeys => Keyboard.ModifierKeys;

	internal override Size SmallIconSize
	{
		get
		{
			if (XGetIconSizes(DisplayHandle, RootWindow, out var size_list, out var count) != 0)
			{
				long num = (long)size_list;
				int num2 = 0;
				XIconSize xIconSize = default(XIconSize);
				for (int i = 0; i < count; i++)
				{
					xIconSize = (XIconSize)Marshal.PtrToStructure((IntPtr)num, xIconSize.GetType());
					num += Marshal.SizeOf(xIconSize);
					if (xIconSize.min_width == 16)
					{
						XFree(size_list);
						return new Size(16, 16);
					}
					if (xIconSize.max_width == 16)
					{
						XFree(size_list);
						return new Size(16, 16);
					}
					if (xIconSize.min_width < 16 && xIconSize.max_width > 16)
					{
						int num3 = xIconSize.min_width;
						while (num3 < xIconSize.max_width)
						{
							num3 += xIconSize.width_inc;
							if (num3 == 16)
							{
								XFree(size_list);
								return new Size(16, 16);
							}
						}
					}
					if (num2 == 0 || num2 > xIconSize.min_width)
					{
						num2 = xIconSize.min_width;
					}
				}
				return new Size(num2, num2);
			}
			return new Size(16, 16);
		}
	}

	internal override int MouseButtonCount => 3;

	internal override bool MouseButtonsSwapped => false;

	internal override Point MousePosition => mouse_position;

	internal override Size MouseHoverSize => new Size(1, 1);

	internal override int MouseHoverTime => HoverState.Interval;

	internal override bool MouseWheelPresent => true;

	internal override MouseButtons MouseButtons => MouseState;

	internal override Rectangle VirtualScreen
	{
		get
		{
			IntPtr prop = IntPtr.Zero;
			XGetWindowProperty(DisplayHandle, RootWindow, _NET_DESKTOP_GEOMETRY, IntPtr.Zero, new IntPtr(256), delete: false, (IntPtr)6, out var _, out var _, out var nitems, out var _, ref prop);
			if ((long)nitems >= 2)
			{
				int width = Marshal.ReadIntPtr(prop, 0).ToInt32();
				int height = Marshal.ReadIntPtr(prop, IntPtr.Size).ToInt32();
				XFree(prop);
				return new Rectangle(0, 0, width, height);
			}
			XWindowAttributes attributes = default(XWindowAttributes);
			lock (XlibLock)
			{
				XGetWindowAttributes(DisplayHandle, XRootWindow(DisplayHandle, 0), ref attributes);
			}
			return new Rectangle(0, 0, attributes.width, attributes.height);
		}
	}

	internal override Rectangle WorkingArea
	{
		get
		{
			IntPtr prop = IntPtr.Zero;
			XGetWindowProperty(DisplayHandle, RootWindow, _NET_CURRENT_DESKTOP, IntPtr.Zero, new IntPtr(1), delete: false, (IntPtr)6, out var actual_type, out var actual_format, out var nitems, out var bytes_after, ref prop);
			if ((long)nitems >= 1)
			{
				int num = Marshal.ReadIntPtr(prop, 0).ToInt32();
				XFree(prop);
				XGetWindowProperty(DisplayHandle, RootWindow, _NET_WORKAREA, IntPtr.Zero, new IntPtr(256), delete: false, (IntPtr)6, out actual_type, out actual_format, out nitems, out bytes_after, ref prop);
				if ((long)nitems >= 4 * num)
				{
					int x = Marshal.ReadIntPtr(prop, IntPtr.Size * 4 * num).ToInt32();
					int y = Marshal.ReadIntPtr(prop, IntPtr.Size * 4 * num + IntPtr.Size).ToInt32();
					int width = Marshal.ReadIntPtr(prop, IntPtr.Size * 4 * num + IntPtr.Size * 2).ToInt32();
					int height = Marshal.ReadIntPtr(prop, IntPtr.Size * 4 * num + IntPtr.Size * 3).ToInt32();
					XFree(prop);
					return new Rectangle(x, y, width, height);
				}
			}
			XWindowAttributes attributes = default(XWindowAttributes);
			lock (XlibLock)
			{
				XGetWindowAttributes(DisplayHandle, XRootWindow(DisplayHandle, 0), ref attributes);
			}
			return new Rectangle(0, 0, attributes.width, attributes.height);
		}
	}

	internal override bool ThemesEnabled => themes_enabled;

	internal override event EventHandler Idle;

	private XplatUIX11()
	{
		RefCount = 0;
		in_doevents = false;
		XlibLock = new object();
		X11Keyboard.XlibLock = XlibLock;
		MessageQueues = Hashtable.Synchronized(new Hashtable(7));
		unattached_timer_list = ArrayList.Synchronized(new ArrayList(3));
		messageHold = Hashtable.Synchronized(new Hashtable(3));
		Clipboard = new ClipboardData();
		XInitThreads();
		ErrorExceptions = false;
		SetDisplay(XOpenDisplay(IntPtr.Zero));
		X11DesktopColors.Initialize();
		try
		{
			XkbSetDetectableAutoRepeat(DisplayHandle, detectable: true, IntPtr.Zero);
			detectable_key_auto_repeat = true;
		}
		catch
		{
			Console.Error.WriteLine("Could not disable keyboard auto repeat, will attempt to disable manually.");
			detectable_key_auto_repeat = false;
		}
		ErrorHandler = HandleError;
		XSetErrorHandler(ErrorHandler);
	}

	~XplatUIX11()
	{
		Graphics.FromHdcInternal(IntPtr.Zero);
	}

	public static XplatUIX11 GetInstance()
	{
		lock (lockobj)
		{
			if (Instance == null)
			{
				Instance = new XplatUIX11();
			}
			RefCount++;
		}
		return Instance;
	}

	internal void SetDisplay(IntPtr display_handle)
	{
		if (display_handle != IntPtr.Zero)
		{
			Hwnd hwnd;
			if (DisplayHandle != IntPtr.Zero && FosterParent != IntPtr.Zero)
			{
				hwnd = Hwnd.ObjectFromHandle(FosterParent);
				XDestroyWindow(DisplayHandle, FosterParent);
				hwnd.Dispose();
			}
			if (DisplayHandle != IntPtr.Zero)
			{
				XCloseDisplay(DisplayHandle);
			}
			DisplayHandle = display_handle;
			Graphics.FromHdcInternal(DisplayHandle);
			XQueryExtension(DisplayHandle, "RENDER", ref render_major_opcode, ref render_first_event, ref render_first_error);
			if (Environment.GetEnvironmentVariable("MONO_XSYNC") != null)
			{
				XSynchronize(DisplayHandle, onoff: true);
			}
			if (Environment.GetEnvironmentVariable("MONO_XEXCEPTIONS") != null)
			{
				ErrorExceptions = true;
			}
			ScreenNo = XDefaultScreen(DisplayHandle);
			RootWindow = XRootWindow(DisplayHandle, ScreenNo);
			DefaultColormap = XDefaultColormap(DisplayHandle, ScreenNo);
			FosterParent = XCreateSimpleWindow(DisplayHandle, RootWindow, 0, 0, 1, 1, 0, UIntPtr.Zero, UIntPtr.Zero);
			if (FosterParent == IntPtr.Zero)
			{
				Console.WriteLine("XplatUIX11 Constructor failed to create FosterParent");
			}
			hwnd = new Hwnd();
			hwnd.Queue = ThreadQueue(Thread.CurrentThread);
			hwnd.WholeWindow = FosterParent;
			hwnd.ClientWindow = FosterParent;
			hwnd = new Hwnd();
			hwnd.Queue = ThreadQueue(Thread.CurrentThread);
			hwnd.whole_window = RootWindow;
			hwnd.ClientWindow = RootWindow;
			listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			IPEndPoint local_end = new IPEndPoint(IPAddress.Loopback, 0);
			listen.Bind(local_end);
			listen.Listen(1);
			network_buffer = new byte[10];
			wake = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
			wake.Connect(listen.LocalEndPoint);
			wake_receive = listen.Accept();
			pollfds = new Pollfd[2];
			pollfds[0] = default(Pollfd);
			pollfds[0].fd = XConnectionNumber(DisplayHandle);
			pollfds[0].events = PollEvents.POLLIN;
			pollfds[1] = default(Pollfd);
			pollfds[1].fd = wake_receive.Handle.ToInt32();
			pollfds[1].events = PollEvents.POLLIN;
			Keyboard = new X11Keyboard(DisplayHandle, FosterParent);
			Dnd = new X11Dnd(DisplayHandle, Keyboard);
			DoubleClickInterval = 500;
			HoverState.Interval = 500;
			HoverState.Timer = new Timer();
			HoverState.Timer.Enabled = false;
			HoverState.Timer.Interval = HoverState.Interval;
			HoverState.Timer.Tick += MouseHover;
			HoverState.Size = new Size(4, 4);
			HoverState.X = -1;
			HoverState.Y = -1;
			ActiveWindow = IntPtr.Zero;
			FocusWindow = IntPtr.Zero;
			ModalWindows = new Stack(3);
			MouseState = MouseButtons.None;
			mouse_position = new Point(0, 0);
			Caret.Timer = new Timer();
			Caret.Timer.Interval = 500;
			Caret.Timer.Tick += CaretCallback;
			SetupAtoms();
			XSelectInput(DisplayHandle, RootWindow, new IntPtr((int)(EventMask.PropertyChangeMask | Keyboard.KeyEventMask)));
			ErrorHandler = HandleError;
			XSetErrorHandler(ErrorHandler);
			return;
		}
		throw new ArgumentNullException("Display", "Could not open display (X-Server required. Check you DISPLAY environment variable)");
	}

	private int unixtime()
	{
		return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	private static void SetupAtoms()
	{
		string[] array = new string[37]
		{
			"WM_PROTOCOLS", "WM_DELETE_WINDOW", "WM_TAKE_FOCUS", "_NET_DESKTOP_GEOMETRY", "_NET_CURRENT_DESKTOP", "_NET_ACTIVE_WINDOW", "_NET_WORKAREA", "_NET_WM_NAME", "_NET_WM_WINDOW_TYPE", "_NET_WM_STATE",
			"_NET_WM_ICON", "_NET_WM_USER_TIME", "_NET_FRAME_EXTENTS", "_NET_SYSTEM_TRAY_OPCODE", "_NET_WM_STATE_MAXIMIZED_HORZ", "_NET_WM_STATE_MAXIMIZED_VERT", "_NET_WM_STATE_HIDDEN", "_XEMBED", "_XEMBED_INFO", "_MOTIF_WM_HINTS",
			"_NET_WM_STATE_SKIP_TASKBAR", "_NET_WM_STATE_ABOVE", "_NET_WM_STATE_MODAL", "_NET_WM_CONTEXT_HELP", "_NET_WM_WINDOW_OPACITY", "_NET_WM_WINDOW_TYPE_UTILITY", "_NET_WM_WINDOW_TYPE_NORMAL", "CLIPBOARD", "PRIMARY", "COMPOUND_TEXT",
			"UTF8_STRING", "UTF16_STRING", "RICHTEXTFORMAT", "TARGETS", "_SWF_AsyncAtom", "_SWF_PostMessageAtom", "_SWF_HoverAtom"
		};
		IntPtr[] array2 = new IntPtr[array.Length];
		XInternAtoms(DisplayHandle, array, array.Length, only_if_exists: false, array2);
		int num = 0;
		WM_PROTOCOLS = array2[num++];
		WM_DELETE_WINDOW = array2[num++];
		WM_TAKE_FOCUS = array2[num++];
		_NET_DESKTOP_GEOMETRY = array2[num++];
		_NET_CURRENT_DESKTOP = array2[num++];
		_NET_ACTIVE_WINDOW = array2[num++];
		_NET_WORKAREA = array2[num++];
		_NET_WM_NAME = array2[num++];
		_NET_WM_WINDOW_TYPE = array2[num++];
		_NET_WM_STATE = array2[num++];
		_NET_WM_ICON = array2[num++];
		_NET_WM_USER_TIME = array2[num++];
		_NET_FRAME_EXTENTS = array2[num++];
		_NET_SYSTEM_TRAY_OPCODE = array2[num++];
		_NET_WM_STATE_MAXIMIZED_HORZ = array2[num++];
		_NET_WM_STATE_MAXIMIZED_VERT = array2[num++];
		_NET_WM_STATE_HIDDEN = array2[num++];
		_XEMBED = array2[num++];
		_XEMBED_INFO = array2[num++];
		_MOTIF_WM_HINTS = array2[num++];
		_NET_WM_STATE_SKIP_TASKBAR = array2[num++];
		_NET_WM_STATE_ABOVE = array2[num++];
		_NET_WM_STATE_MODAL = array2[num++];
		_NET_WM_CONTEXT_HELP = array2[num++];
		_NET_WM_WINDOW_OPACITY = array2[num++];
		_NET_WM_WINDOW_TYPE_UTILITY = array2[num++];
		_NET_WM_WINDOW_TYPE_NORMAL = array2[num++];
		CLIPBOARD = array2[num++];
		PRIMARY = array2[num++];
		OEMTEXT = array2[num++];
		UTF8_STRING = array2[num++];
		UTF16_STRING = array2[num++];
		RICHTEXTFORMAT = array2[num++];
		TARGETS = array2[num++];
		AsyncAtom = array2[num++];
		PostAtom = array2[num++];
		HoverState.Atom = array2[num++];
		_NET_SYSTEM_TRAY_S = XInternAtom(DisplayHandle, "_NET_SYSTEM_TRAY_S" + ScreenNo, only_if_exists: false);
	}

	private void GetSystrayManagerWindow()
	{
		XGrabServer(DisplayHandle);
		SystrayMgrWindow = XGetSelectionOwner(DisplayHandle, _NET_SYSTEM_TRAY_S);
		XUngrabServer(DisplayHandle);
		XFlush(DisplayHandle);
	}

	private void SendNetWMMessage(IntPtr window, IntPtr message_type, IntPtr l0, IntPtr l1, IntPtr l2)
	{
		XEvent send_event = default(XEvent);
		send_event.ClientMessageEvent.type = XEventName.ClientMessage;
		send_event.ClientMessageEvent.send_event = true;
		send_event.ClientMessageEvent.window = window;
		send_event.ClientMessageEvent.message_type = message_type;
		send_event.ClientMessageEvent.format = 32;
		send_event.ClientMessageEvent.ptr1 = l0;
		send_event.ClientMessageEvent.ptr2 = l1;
		send_event.ClientMessageEvent.ptr3 = l2;
		XSendEvent(DisplayHandle, RootWindow, propagate: false, new IntPtr(1572864), ref send_event);
	}

	private void SendNetClientMessage(IntPtr window, IntPtr message_type, IntPtr l0, IntPtr l1, IntPtr l2)
	{
		XEvent send_event = default(XEvent);
		send_event.ClientMessageEvent.type = XEventName.ClientMessage;
		send_event.ClientMessageEvent.send_event = true;
		send_event.ClientMessageEvent.window = window;
		send_event.ClientMessageEvent.message_type = message_type;
		send_event.ClientMessageEvent.format = 32;
		send_event.ClientMessageEvent.ptr1 = l0;
		send_event.ClientMessageEvent.ptr2 = l1;
		send_event.ClientMessageEvent.ptr3 = l2;
		XSendEvent(DisplayHandle, window, propagate: false, new IntPtr(0), ref send_event);
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

	internal static Rectangle TranslateClientRectangleToXClientRectangle(Hwnd hwnd)
	{
		return TranslateClientRectangleToXClientRectangle(hwnd, Control.FromHandle(hwnd.Handle));
	}

	internal static Rectangle TranslateClientRectangleToXClientRectangle(Hwnd hwnd, Control ctrl)
	{
		Rectangle rectangle = hwnd.ClientRect;
		Form form = ctrl as Form;
		CreateParams createParams = null;
		if (form != null)
		{
			createParams = form.GetCreateParams();
		}
		if (form != null && form.window_manager == null && !createParams.IsSet(WindowExStyles.WS_EX_TOOLWINDOW))
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

	internal static Size TranslateWindowSizeToXWindowSize(CreateParams cp)
	{
		return TranslateWindowSizeToXWindowSize(cp, new Size(cp.Width, cp.Height));
	}

	internal static Size TranslateWindowSizeToXWindowSize(CreateParams cp, Size size)
	{
		if (cp.control is Form form && form.window_manager == null && !cp.IsSet(WindowExStyles.WS_EX_TOOLWINDOW))
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

	internal static Size TranslateXWindowSizeToWindowSize(CreateParams cp, int xWidth, int xHeight)
	{
		Size size = new Size(xWidth, xHeight);
		if (cp.control is Form form && form.window_manager == null && !cp.IsSet(WindowExStyles.WS_EX_TOOLWINDOW))
		{
			Hwnd.Borders borders = Hwnd.GetBorders(cp, null);
			Size result = size;
			result.Width += borders.left + borders.right;
			result.Height += borders.top + borders.bottom;
			return result;
		}
		return size;
	}

	internal static Point GetTopLevelWindowLocation(Hwnd hwnd)
	{
		XTranslateCoordinates(DisplayHandle, hwnd.whole_window, RootWindow, 0, 0, out var intdest_x_return, out var dest_y_return, out var _);
		Hwnd.Borders borders = FrameExtents(hwnd.whole_window);
		intdest_x_return -= borders.left;
		dest_y_return -= borders.top;
		return new Point(intdest_x_return, dest_y_return);
	}

	private void DeriveStyles(int Style, int ExStyle, out FormBorderStyle border_style, out bool border_static, out TitleStyle title_style, out int caption_height, out int tool_caption_height)
	{
		caption_height = 0;
		tool_caption_height = 19;
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
				caption_height = 19;
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
				caption_height = 19;
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

	private void SetWMStyles(Hwnd hwnd, CreateParams cp)
	{
		if (cp.HasWindowManager && !cp.IsSet(WindowExStyles.WS_EX_TOOLWINDOW))
		{
			return;
		}
		int[] array = new int[8];
		MotifWmHints data = default(MotifWmHints);
		MotifFunctions motifFunctions = (MotifFunctions)0;
		MotifDecorations motifDecorations = (MotifDecorations)0;
		IntPtr nET_WM_WINDOW_TYPE_NORMAL = _NET_WM_WINDOW_TYPE_NORMAL;
		IntPtr intPtr = IntPtr.Zero;
		data.flags = (IntPtr)3;
		data.functions = (IntPtr)0;
		data.decorations = (IntPtr)0;
		Form form = cp.control as Form;
		if (ExStyleSet(cp.ExStyle, WindowExStyles.WS_EX_TOOLWINDOW))
		{
			motifFunctions |= MotifFunctions.Resize | MotifFunctions.Move | MotifFunctions.Minimize | MotifFunctions.Maximize;
		}
		else if (form != null && form.FormBorderStyle == FormBorderStyle.None)
		{
			motifFunctions |= MotifFunctions.All | MotifFunctions.Resize;
		}
		else
		{
			if (StyleSet(cp.Style, WindowStyles.WS_CAPTION))
			{
				motifFunctions |= MotifFunctions.Move;
				motifDecorations |= MotifDecorations.Title | MotifDecorations.Menu;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_THICKFRAME))
			{
				motifFunctions |= MotifFunctions.Resize | MotifFunctions.Move;
				motifDecorations |= MotifDecorations.Border | MotifDecorations.ResizeH;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_GROUP))
			{
				motifFunctions |= MotifFunctions.Minimize;
				motifDecorations |= MotifDecorations.Minimize;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_TABSTOP))
			{
				motifFunctions |= MotifFunctions.Maximize;
				motifDecorations |= MotifDecorations.Maximize;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_THICKFRAME))
			{
				motifFunctions |= MotifFunctions.Resize;
				motifDecorations |= MotifDecorations.ResizeH;
			}
			if (ExStyleSet(cp.ExStyle, WindowExStyles.WS_EX_DLGMODALFRAME))
			{
				motifDecorations |= MotifDecorations.Border;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_BORDER))
			{
				motifDecorations |= MotifDecorations.Border;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_DLGFRAME))
			{
				motifDecorations |= MotifDecorations.Border;
			}
			if (StyleSet(cp.Style, WindowStyles.WS_SYSMENU))
			{
				motifFunctions |= MotifFunctions.Close;
			}
			else
			{
				motifFunctions &= ~(MotifFunctions.Minimize | MotifFunctions.Maximize | MotifFunctions.Close);
				motifDecorations &= ~(MotifDecorations.Menu | MotifDecorations.Minimize | MotifDecorations.Maximize);
				if (cp.Caption == string.Empty)
				{
					motifFunctions &= ~MotifFunctions.Move;
					motifDecorations &= ~(MotifDecorations.ResizeH | MotifDecorations.Title);
				}
			}
		}
		if ((motifFunctions & MotifFunctions.Resize) == 0)
		{
			hwnd.fixed_size = true;
			Rectangle maximized = new Rectangle(cp.X, cp.Y, cp.Width, cp.Height);
			SetWindowMinMax(hwnd.Handle, maximized, maximized.Size, maximized.Size, cp);
		}
		else
		{
			hwnd.fixed_size = false;
		}
		data.functions = (IntPtr)(int)motifFunctions;
		data.decorations = (IntPtr)(int)motifDecorations;
		nET_WM_WINDOW_TYPE_NORMAL = ((!cp.IsSet(WindowExStyles.WS_EX_TOOLWINDOW)) ? _NET_WM_WINDOW_TYPE_NORMAL : _NET_WM_WINDOW_TYPE_UTILITY);
		bool flag = !cp.IsSet(WindowExStyles.WS_EX_APPWINDOW) || ((cp.IsSet(WindowExStyles.WS_EX_TOOLWINDOW) && form != null && form.Parent != null && !form.ShowInTaskbar) ? true : false);
		if (ExStyleSet(cp.ExStyle, WindowExStyles.WS_EX_TOOLWINDOW) && form != null && !hwnd.reparented && form.Owner != null && form.Owner.Handle != IntPtr.Zero)
		{
			Hwnd hwnd2 = Hwnd.ObjectFromHandle(form.Owner.Handle);
			if (hwnd2 != null)
			{
				intPtr = hwnd2.whole_window;
			}
		}
		if (StyleSet(cp.Style, WindowStyles.WS_POPUP) && hwnd.parent != null && hwnd.parent.whole_window != IntPtr.Zero)
		{
			intPtr = hwnd.parent.whole_window;
		}
		FormWindowState formWindowState = GetWindowState(hwnd.Handle);
		if (formWindowState == (FormWindowState)(-1))
		{
			formWindowState = FormWindowState.Normal;
		}
		Rectangle rectangle = TranslateClientRectangleToXClientRectangle(hwnd);
		lock (XlibLock)
		{
			int nelements = 0;
			array[0] = nET_WM_WINDOW_TYPE_NORMAL.ToInt32();
			XChangeProperty(DisplayHandle, hwnd.whole_window, _NET_WM_WINDOW_TYPE, (IntPtr)4, 32, PropertyMode.Replace, array, 1);
			XChangeProperty(DisplayHandle, hwnd.whole_window, _MOTIF_WM_HINTS, _MOTIF_WM_HINTS, 32, PropertyMode.Replace, ref data, 5);
			if (intPtr != IntPtr.Zero)
			{
				XSetTransientForHint(DisplayHandle, hwnd.whole_window, intPtr);
			}
			MoveResizeWindow(DisplayHandle, hwnd.client_window, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			if (flag)
			{
				array[nelements++] = _NET_WM_STATE_SKIP_TASKBAR.ToInt32();
			}
			if (formWindowState == FormWindowState.Maximized)
			{
				array[nelements++] = _NET_WM_STATE_MAXIMIZED_HORZ.ToInt32();
				array[nelements++] = _NET_WM_STATE_MAXIMIZED_VERT.ToInt32();
			}
			if (form != null && form.Modal)
			{
				array[nelements++] = _NET_WM_STATE_MODAL.ToInt32();
			}
			XChangeProperty(DisplayHandle, hwnd.whole_window, _NET_WM_STATE, (IntPtr)4, 32, PropertyMode.Replace, array, nelements);
			nelements = 0;
			IntPtr[] array2 = new IntPtr[2];
			ref IntPtr reference = ref array2[nelements++];
			reference = WM_DELETE_WINDOW;
			if (ExStyleSet(cp.ExStyle, WindowExStyles.WS_EX_CONTEXTHELP))
			{
				ref IntPtr reference2 = ref array2[nelements++];
				reference2 = _NET_WM_CONTEXT_HELP;
			}
			XSetWMProtocols(DisplayHandle, hwnd.whole_window, array2, nelements);
		}
	}

	private void SetIcon(Hwnd hwnd, Icon icon)
	{
		if (icon == null)
		{
			XDeleteProperty(DisplayHandle, hwnd.whole_window, _NET_WM_ICON);
			return;
		}
		Bitmap bitmap = icon.ToBitmap();
		int num = 0;
		int num2 = bitmap.Width * bitmap.Height + 2;
		IntPtr[] array = new IntPtr[num2];
		ref IntPtr reference = ref array[num++];
		reference = (IntPtr)bitmap.Width;
		ref IntPtr reference2 = ref array[num++];
		reference2 = (IntPtr)bitmap.Height;
		for (int i = 0; i < bitmap.Height; i++)
		{
			for (int j = 0; j < bitmap.Width; j++)
			{
				ref IntPtr reference3 = ref array[num++];
				reference3 = (IntPtr)bitmap.GetPixel(j, i).ToArgb();
			}
		}
		XChangeProperty(DisplayHandle, hwnd.whole_window, _NET_WM_ICON, (IntPtr)6, 32, PropertyMode.Replace, array, num2);
	}

	private void WakeupMain()
	{
		wake.Send(new byte[1] { 255 });
	}

	private XEventQueue ThreadQueue(Thread thread)
	{
		XEventQueue xEventQueue = (XEventQueue)MessageQueues[thread];
		if (xEventQueue == null)
		{
			xEventQueue = new XEventQueue(thread);
			MessageQueues[thread] = xEventQueue;
		}
		return xEventQueue;
	}

	private void TranslatePropertyToClipboard(IntPtr property)
	{
		IntPtr prop = IntPtr.Zero;
		Clipboard.Item = null;
		XGetWindowProperty(DisplayHandle, FosterParent, property, IntPtr.Zero, new IntPtr(int.MaxValue), delete: true, (IntPtr)0, out var _, out var _, out var nitems, out var _, ref prop);
		if ((long)nitems <= 0)
		{
			return;
		}
		if (property == (IntPtr)31)
		{
			Clipboard.Item = Marshal.PtrToStringAnsi(prop);
		}
		else if (!(property == (IntPtr)5) && !(property == (IntPtr)20))
		{
			if (property == OEMTEXT)
			{
				Clipboard.Item = Marshal.PtrToStringAnsi(prop);
			}
			else if (property == UTF8_STRING)
			{
				byte[] array = new byte[(int)nitems];
				for (int i = 0; i < (int)nitems; i++)
				{
					array[i] = Marshal.ReadByte(prop, i);
				}
				Clipboard.Item = Encoding.UTF8.GetString(array);
			}
			else if (property == UTF16_STRING)
			{
				Clipboard.Item = Marshal.PtrToStringUni(prop, Encoding.Unicode.GetMaxCharCount((int)nitems));
			}
			else if (property == RICHTEXTFORMAT)
			{
				Clipboard.Item = Marshal.PtrToStringAnsi(prop);
			}
			else if (DataFormats.ContainsFormat(property.ToInt32()) && DataFormats.GetFormat(property.ToInt32()).is_serializable)
			{
				MemoryStream memoryStream = new MemoryStream((int)nitems);
				for (int j = 0; j < (int)nitems; j++)
				{
					memoryStream.WriteByte(Marshal.ReadByte(prop, j));
				}
				memoryStream.Position = 0L;
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				Clipboard.Item = binaryFormatter.Deserialize(memoryStream);
				memoryStream.Close();
			}
		}
		XFree(prop);
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
			if (!hwnd.expose_pending)
			{
				if (!hwnd.nc_expose_pending)
				{
					hwnd.Queue.Paint.Enqueue(hwnd);
				}
				hwnd.expose_pending = true;
			}
			return;
		}
		hwnd.AddNcInvalidArea(x, y, width, height);
		if (!hwnd.nc_expose_pending)
		{
			if (!hwnd.expose_pending)
			{
				hwnd.Queue.Paint.Enqueue(hwnd);
			}
			hwnd.nc_expose_pending = true;
		}
	}

	private static Hwnd.Borders FrameExtents(IntPtr window)
	{
		IntPtr prop = IntPtr.Zero;
		Hwnd.Borders result = default(Hwnd.Borders);
		XGetWindowProperty(DisplayHandle, window, _NET_FRAME_EXTENTS, IntPtr.Zero, new IntPtr(16), delete: false, (IntPtr)6, out var _, out var _, out var nitems, out var _, ref prop);
		if (prop != IntPtr.Zero)
		{
			if (nitems.ToInt32() == 4)
			{
				result.left = Marshal.ReadInt32(prop, 0);
				result.right = Marshal.ReadInt32(prop, IntPtr.Size);
				result.top = Marshal.ReadInt32(prop, 2 * IntPtr.Size);
				result.bottom = Marshal.ReadInt32(prop, 3 * IntPtr.Size);
			}
			XFree(prop);
		}
		return result;
	}

	private void AddConfigureNotify(XEvent xevent)
	{
		Hwnd objectFromWindow = Hwnd.GetObjectFromWindow(xevent.ConfigureEvent.window);
		if (objectFromWindow == null || objectFromWindow.zombie || !(xevent.ConfigureEvent.window == objectFromWindow.whole_window))
		{
			return;
		}
		if (objectFromWindow.parent == null)
		{
			Point topLevelWindowLocation = GetTopLevelWindowLocation(objectFromWindow);
			objectFromWindow.x = topLevelWindowLocation.X;
			objectFromWindow.y = topLevelWindowLocation.Y;
		}
		Control control = Control.FromHandle(objectFromWindow.Handle);
		Size size = ((control == null) ? new Size(xevent.ConfigureEvent.width, xevent.ConfigureEvent.height) : TranslateXWindowSizeToWindowSize(control.GetCreateParams(), xevent.ConfigureEvent.width, xevent.ConfigureEvent.height));
		objectFromWindow.width = size.Width;
		objectFromWindow.height = size.Height;
		objectFromWindow.ClientRect = Rectangle.Empty;
		lock (objectFromWindow.configure_lock)
		{
			if (!objectFromWindow.configure_pending)
			{
				objectFromWindow.Queue.EnqueueLocked(xevent);
				objectFromWindow.configure_pending = true;
			}
		}
	}

	private void ShowCaret()
	{
		if (Caret.gc == IntPtr.Zero || Caret.On)
		{
			return;
		}
		Caret.On = true;
		lock (XlibLock)
		{
			XDrawLine(DisplayHandle, Caret.Window, Caret.gc, Caret.X, Caret.Y, Caret.X, Caret.Y + Caret.Height);
		}
	}

	private void HideCaret()
	{
		if (Caret.gc == IntPtr.Zero || !Caret.On)
		{
			return;
		}
		Caret.On = false;
		lock (XlibLock)
		{
			XDrawLine(DisplayHandle, Caret.Window, Caret.gc, Caret.X, Caret.Y, Caret.X, Caret.Y + Caret.Height);
		}
	}

	private int NextTimeout(ArrayList timers, DateTime now)
	{
		int num = 0;
		foreach (Timer timer in timers)
		{
			int num2 = (int)(timer.Expires - now).TotalMilliseconds;
			if (num2 < 0)
			{
				return 0;
			}
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (num < Timer.Minimum)
		{
			num = Timer.Minimum;
		}
		if (num > 1000)
		{
			num = 1000;
		}
		return num;
	}

	private void CheckTimers(ArrayList timers, DateTime now)
	{
		if (timers.Count == 0)
		{
			return;
		}
		for (int i = 0; i < timers.Count; i++)
		{
			Timer timer = (Timer)timers[i];
			if (timer.Enabled && timer.Expires <= now && !timer.Busy && (in_doevents || (Application.MWFThread.Current.Context != null && (Application.MWFThread.Current.Context.MainForm == null || Application.MWFThread.Current.Context.MainForm.IsLoaded))))
			{
				timer.Busy = true;
				timer.Update(now);
				timer.FireTick();
				timer.Busy = false;
			}
		}
	}

	private void WaitForHwndMessage(Hwnd hwnd, Msg message)
	{
		WaitForHwndMessage(hwnd, message, process: false);
	}

	private void WaitForHwndMessage(Hwnd hwnd, Msg message, bool process)
	{
		MSG msg = default(MSG);
		XEventQueue xEventQueue = ThreadQueue(Thread.CurrentThread);
		xEventQueue.DispatchIdle = false;
		bool flag = false;
		string key = hwnd.Handle + ":" + message;
		if (!messageHold.ContainsKey(key))
		{
			messageHold.Add(key, 1);
		}
		else
		{
			messageHold[key] = (int)messageHold[key] + 1;
		}
		do
		{
			if (PeekMessage(xEventQueue, ref msg, IntPtr.Zero, 0, 0, 1u))
			{
				if (msg.message == Msg.WM_QUIT)
				{
					PostQuitMessage(0);
					flag = true;
				}
				else
				{
					if (msg.hwnd == hwnd.Handle)
					{
						if (msg.message == message)
						{
							if (process)
							{
								TranslateMessage(ref msg);
								DispatchMessage(ref msg);
							}
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
			}
			flag = !messageHold.ContainsKey(key) || (int)messageHold[key] < 1 || flag;
		}
		while (!flag);
		messageHold.Remove(key);
		xEventQueue.DispatchIdle = true;
	}

	private void MapWindow(Hwnd hwnd, WindowType windows)
	{
		if (hwnd.mapped)
		{
			return;
		}
		Form form = Control.FromHandle(hwnd.Handle) as Form;
		if (form != null && form.WindowState == FormWindowState.Normal)
		{
			form.waiting_showwindow = true;
			SendMessage(hwnd.Handle, Msg.WM_SHOWWINDOW, (IntPtr)1, IntPtr.Zero);
		}
		if (hwnd.zombie)
		{
			return;
		}
		if ((windows & WindowType.Whole) != 0)
		{
			XMapWindow(DisplayHandle, hwnd.whole_window);
		}
		if ((windows & WindowType.Client) != 0)
		{
			XMapWindow(DisplayHandle, hwnd.client_window);
		}
		hwnd.mapped = true;
		if (form != null && form.waiting_showwindow)
		{
			WaitForHwndMessage(hwnd, Msg.WM_SHOWWINDOW);
			CreateParams createParams = form.GetCreateParams();
			if (!ExStyleSet(createParams.ExStyle, WindowExStyles.WS_EX_MDICHILD) && !StyleSet(createParams.Style, WindowStyles.WS_CHILD))
			{
				WaitForHwndMessage(hwnd, Msg.WM_ACTIVATE, process: true);
			}
		}
	}

	private void UnmapWindow(Hwnd hwnd, WindowType windows)
	{
		if (!hwnd.mapped)
		{
			return;
		}
		Form form = null;
		if (Control.FromHandle(hwnd.Handle) is Form)
		{
			form = Control.FromHandle(hwnd.Handle) as Form;
			if (form.WindowState == FormWindowState.Normal)
			{
				form.waiting_showwindow = true;
				SendMessage(hwnd.Handle, Msg.WM_SHOWWINDOW, IntPtr.Zero, IntPtr.Zero);
			}
		}
		if (hwnd.zombie)
		{
			return;
		}
		if ((windows & WindowType.Client) != 0)
		{
			XUnmapWindow(DisplayHandle, hwnd.client_window);
		}
		if ((windows & WindowType.Whole) != 0)
		{
			XUnmapWindow(DisplayHandle, hwnd.whole_window);
		}
		hwnd.mapped = false;
		if (form != null && form.waiting_showwindow)
		{
			WaitForHwndMessage(hwnd, Msg.WM_SHOWWINDOW);
			CreateParams createParams = form.GetCreateParams();
			if (!ExStyleSet(createParams.ExStyle, WindowExStyles.WS_EX_MDICHILD) && !StyleSet(createParams.Style, WindowStyles.WS_CHILD))
			{
				WaitForHwndMessage(hwnd, Msg.WM_ACTIVATE, process: true);
			}
		}
	}

	private void UpdateMessageQueue(XEventQueue queue)
	{
		DateTime utcNow = DateTime.UtcNow;
		int num;
		lock (XlibLock)
		{
			num = XPending(DisplayHandle);
		}
		if (num == 0)
		{
			if ((queue == null || queue.DispatchIdle) && Idle != null)
			{
				Idle(this, EventArgs.Empty);
			}
			lock (XlibLock)
			{
				num = XPending(DisplayHandle);
			}
		}
		if (num == 0)
		{
			int num2 = 0;
			if (queue != null)
			{
				if (queue.Paint.Count > 0)
				{
					return;
				}
				num2 = NextTimeout(queue.timer_list, utcNow);
			}
			if (num2 > 0)
			{
				int num3 = pollfds.Length - 1;
				lock (wake_waiting_lock)
				{
					if (!wake_waiting)
					{
						num3++;
						wake_waiting = true;
					}
				}
				Syscall.poll(pollfds, (uint)num3, num2);
				if (num3 == pollfds.Length)
				{
					if (pollfds[1].revents != 0)
					{
						wake_receive.Receive(network_buffer, 0, 1, SocketFlags.None);
					}
					lock (wake_waiting_lock)
					{
						wake_waiting = false;
					}
				}
				lock (XlibLock)
				{
					num = XPending(DisplayHandle);
				}
			}
		}
		if (queue != null)
		{
			CheckTimers(queue.timer_list, utcNow);
		}
		while (true)
		{
			XEvent xevent = default(XEvent);
			lock (XlibLock)
			{
				if (XPending(DisplayHandle) == 0)
				{
					break;
				}
				XNextEvent(DisplayHandle, ref xevent);
				if (xevent.AnyEvent.type == XEventName.KeyPress || xevent.AnyEvent.type == XEventName.KeyRelease)
				{
					Keyboard.PreFilter(xevent);
					if (XFilterEvent(ref xevent, Keyboard.ClientWindow))
					{
						continue;
					}
				}
				else if (XFilterEvent(ref xevent, IntPtr.Zero))
				{
					continue;
				}
			}
			Hwnd objectFromWindow = Hwnd.GetObjectFromWindow(xevent.AnyEvent.window);
			if (objectFromWindow == null)
			{
				continue;
			}
			switch (xevent.type)
			{
			case XEventName.Expose:
				AddExpose(objectFromWindow, xevent.ExposeEvent.window == objectFromWindow.ClientWindow, xevent.ExposeEvent.x, xevent.ExposeEvent.y, xevent.ExposeEvent.width, xevent.ExposeEvent.height);
				break;
			case XEventName.SelectionRequest:
			{
				if (Dnd.HandleSelectionRequestEvent(ref xevent))
				{
					break;
				}
				XEvent send_event = default(XEvent);
				send_event.SelectionEvent.type = XEventName.SelectionNotify;
				send_event.SelectionEvent.send_event = true;
				send_event.SelectionEvent.display = DisplayHandle;
				send_event.SelectionEvent.selection = xevent.SelectionRequestEvent.selection;
				send_event.SelectionEvent.target = xevent.SelectionRequestEvent.target;
				send_event.SelectionEvent.requestor = xevent.SelectionRequestEvent.requestor;
				send_event.SelectionEvent.time = xevent.SelectionRequestEvent.time;
				send_event.SelectionEvent.property = IntPtr.Zero;
				IntPtr target = xevent.SelectionRequestEvent.target;
				if (target == TARGETS)
				{
					int[] array = new int[5];
					int nelements = 0;
					if (Clipboard.IsSourceText)
					{
						array[nelements++] = 31;
						array[nelements++] = (int)OEMTEXT;
						array[nelements++] = (int)UTF8_STRING;
						array[nelements++] = (int)UTF16_STRING;
						array[nelements++] = (int)RICHTEXTFORMAT;
					}
					else if (Clipboard.IsSourceImage)
					{
						array[nelements++] = 20;
						array[nelements++] = 5;
					}
					XChangeProperty(DisplayHandle, xevent.SelectionRequestEvent.requestor, xevent.SelectionRequestEvent.property, xevent.SelectionRequestEvent.target, 32, PropertyMode.Replace, array, nelements);
					send_event.SelectionEvent.property = xevent.SelectionRequestEvent.property;
				}
				else if (target == RICHTEXTFORMAT)
				{
					string rtfText = Clipboard.GetRtfText();
					if (rtfText != null)
					{
						byte[] bytes = Encoding.ASCII.GetBytes(rtfText);
						int num4 = bytes.Length;
						IntPtr intPtr = Marshal.AllocHGlobal(num4);
						for (int i = 0; i < num4; i++)
						{
							Marshal.WriteByte(intPtr, i, bytes[i]);
						}
						XChangeProperty(DisplayHandle, xevent.SelectionRequestEvent.requestor, xevent.SelectionRequestEvent.property, xevent.SelectionRequestEvent.target, 8, PropertyMode.Replace, intPtr, num4);
						send_event.SelectionEvent.property = xevent.SelectionRequestEvent.property;
						Marshal.FreeHGlobal(intPtr);
					}
				}
				else if (Clipboard.IsSourceText && (target == (IntPtr)31 || target == OEMTEXT || target == UTF16_STRING || target == UTF8_STRING))
				{
					IntPtr zero = IntPtr.Zero;
					Encoding encoding = null;
					int num5 = 0;
					IntPtr target2 = xevent.SelectionRequestEvent.target;
					if (target2 == (IntPtr)31 || target2 == OEMTEXT)
					{
						encoding = Encoding.ASCII;
					}
					else if (target2 == UTF16_STRING)
					{
						encoding = Encoding.Unicode;
					}
					else if (target2 == UTF8_STRING)
					{
						encoding = Encoding.UTF8;
					}
					byte[] bytes2 = encoding.GetBytes(Clipboard.GetPlainText());
					zero = Marshal.AllocHGlobal(bytes2.Length);
					num5 = bytes2.Length;
					for (int j = 0; j < num5; j++)
					{
						Marshal.WriteByte(zero, j, bytes2[j]);
					}
					if (zero != IntPtr.Zero)
					{
						XChangeProperty(DisplayHandle, xevent.SelectionRequestEvent.requestor, xevent.SelectionRequestEvent.property, xevent.SelectionRequestEvent.target, 8, PropertyMode.Replace, zero, num5);
						send_event.SelectionEvent.property = xevent.SelectionRequestEvent.property;
						Marshal.FreeHGlobal(zero);
					}
				}
				else if (Clipboard.GetSource(target.ToInt32()) != null)
				{
					if (DataFormats.GetFormat(target.ToInt32()).is_serializable)
					{
						object source = Clipboard.GetSource(target.ToInt32());
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						MemoryStream memoryStream = new MemoryStream();
						binaryFormatter.Serialize(memoryStream, source);
						int num6 = (int)memoryStream.Length;
						IntPtr intPtr2 = Marshal.AllocHGlobal(num6);
						memoryStream.Position = 0L;
						for (int k = 0; k < num6; k++)
						{
							Marshal.WriteByte(intPtr2, k, (byte)memoryStream.ReadByte());
						}
						memoryStream.Close();
						XChangeProperty(DisplayHandle, xevent.SelectionRequestEvent.requestor, xevent.SelectionRequestEvent.property, xevent.SelectionRequestEvent.target, 8, PropertyMode.Replace, intPtr2, num6);
						send_event.SelectionEvent.property = xevent.SelectionRequestEvent.property;
						Marshal.FreeHGlobal(intPtr2);
					}
				}
				else if (Clipboard.IsSourceImage && !(xevent.SelectionEvent.target == (IntPtr)20) && !(xevent.SelectionEvent.target == (IntPtr)20))
				{
				}
				XSendEvent(DisplayHandle, xevent.SelectionRequestEvent.requestor, propagate: false, new IntPtr(0), ref send_event);
				break;
			}
			case XEventName.SelectionNotify:
				if (Clipboard.Enumerating)
				{
					Clipboard.Enumerating = false;
					if (xevent.SelectionEvent.property != IntPtr.Zero)
					{
						XDeleteProperty(DisplayHandle, FosterParent, xevent.SelectionEvent.property);
						if (!Clipboard.Formats.Contains(xevent.SelectionEvent.property))
						{
							Clipboard.Formats.Add(xevent.SelectionEvent.property);
						}
					}
				}
				else if (Clipboard.Retrieving)
				{
					Clipboard.Retrieving = false;
					if (xevent.SelectionEvent.property != IntPtr.Zero)
					{
						TranslatePropertyToClipboard(xevent.SelectionEvent.property);
						break;
					}
					Clipboard.ClearSources();
					Clipboard.Item = null;
				}
				else
				{
					Dnd.HandleSelectionNotifyEvent(ref xevent);
				}
				break;
			case XEventName.KeyRelease:
				if (!detectable_key_auto_repeat && XPending(DisplayHandle) != 0)
				{
					XEvent xevent2 = default(XEvent);
					XPeekEvent(DisplayHandle, ref xevent2);
					if (xevent2.type == XEventName.KeyPress && xevent2.KeyEvent.keycode == xevent.KeyEvent.keycode && xevent2.KeyEvent.time == xevent.KeyEvent.time)
					{
						break;
					}
				}
				goto case XEventName.KeyPress;
			case XEventName.MotionNotify:
				if (Thread.CurrentThread == objectFromWindow.Queue.Thread && objectFromWindow.Queue.Count > 0 && objectFromWindow.Queue.Peek().AnyEvent.type == XEventName.MotionNotify)
				{
					break;
				}
				goto case XEventName.KeyPress;
			case XEventName.KeyPress:
				objectFromWindow.Queue.EnqueueLocked(xevent);
				return;
			case XEventName.ButtonPress:
			case XEventName.ButtonRelease:
			case XEventName.EnterNotify:
			case XEventName.LeaveNotify:
			case XEventName.FocusIn:
			case XEventName.FocusOut:
			case XEventName.CreateNotify:
			case XEventName.DestroyNotify:
			case XEventName.UnmapNotify:
			case XEventName.MapNotify:
			case XEventName.ReparentNotify:
			case XEventName.ClientMessage:
				objectFromWindow.Queue.EnqueueLocked(xevent);
				break;
			case XEventName.ConfigureNotify:
				AddConfigureNotify(xevent);
				break;
			case XEventName.PropertyNotify:
				if (xevent.PropertyEvent.atom == _NET_ACTIVE_WINDOW)
				{
					IntPtr prop = IntPtr.Zero;
					IntPtr activeWindow = ActiveWindow;
					XGetWindowProperty(DisplayHandle, RootWindow, _NET_ACTIVE_WINDOW, IntPtr.Zero, new IntPtr(1), delete: false, (IntPtr)33, out var _, out var _, out var nitems, out var _, ref prop);
					if ((long)nitems <= 0 || !(prop != IntPtr.Zero))
					{
						break;
					}
					ActiveWindow = Hwnd.GetHandleFromWindow((IntPtr)Marshal.ReadInt32(prop));
					XFree(prop);
					if (activeWindow != ActiveWindow)
					{
						if (activeWindow != IntPtr.Zero)
						{
							PostMessage(activeWindow, Msg.WM_ACTIVATE, (IntPtr)0, IntPtr.Zero);
						}
						if (ActiveWindow != IntPtr.Zero)
						{
							PostMessage(ActiveWindow, Msg.WM_ACTIVATE, (IntPtr)1, IntPtr.Zero);
						}
					}
					if (ModalWindows.Count != 0 && Control.FromHandle(ActiveWindow) is Form form)
					{
						Form form2 = Control.FromHandle((IntPtr)ModalWindows.Peek()) as Form;
						if (ActiveWindow != (IntPtr)ModalWindows.Peek() && (form2 == null || form.context == form2.context))
						{
							Activate((IntPtr)ModalWindows.Peek());
						}
					}
				}
				else if (xevent.PropertyEvent.atom == _NET_WM_STATE)
				{
					objectFromWindow.cached_window_state = (FormWindowState)(-1);
					PostMessage(objectFromWindow.Handle, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
				}
				break;
			}
		}
	}

	private IntPtr GetMousewParam(int Delta)
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

	private IntPtr XGetParent(IntPtr handle)
	{
		IntPtr parent_return;
		IntPtr children_return;
		lock (XlibLock)
		{
			XQueryTree(DisplayHandle, handle, out var _, out parent_return, out children_return, out var _);
		}
		if (children_return != IntPtr.Zero)
		{
			lock (XlibLock)
			{
				XFree(children_return);
			}
		}
		return parent_return;
	}

	private int HandleError(IntPtr display, ref XErrorEvent error_event)
	{
		if ((uint)error_event.request_code == (byte)render_major_opcode && error_event.minor_code == 7 && error_event.error_code == render_first_error + 1)
		{
			return 0;
		}
		if (ErrorExceptions)
		{
			XUngrabPointer(display, IntPtr.Zero);
			throw new XException(error_event.display, error_event.resourceid, error_event.serial, error_event.error_code, error_event.request_code, error_event.minor_code);
		}
		Console.WriteLine("X11 Error encountered: {0}{1}\n", XException.GetMessage(error_event.display, error_event.resourceid, error_event.serial, error_event.error_code, error_event.request_code, error_event.minor_code), Environment.StackTrace);
		return 0;
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

	private void PerformNCCalc(Hwnd hwnd)
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
		rectangle = TranslateClientRectangleToXClientRectangle(hwnd);
		if (hwnd.visible)
		{
			MoveResizeWindow(DisplayHandle, hwnd.client_window, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
		AddExpose(hwnd, hwnd.WholeWindow == hwnd.ClientWindow, 0, 0, hwnd.Width, hwnd.Height);
	}

	private void MouseHover(object sender, EventArgs e)
	{
		HoverState.Timer.Enabled = false;
		if (HoverState.Window != IntPtr.Zero)
		{
			Hwnd objectFromWindow = Hwnd.GetObjectFromWindow(HoverState.Window);
			if (objectFromWindow != null)
			{
				XEvent xevent = default(XEvent);
				xevent.type = XEventName.ClientMessage;
				xevent.ClientMessageEvent.display = DisplayHandle;
				xevent.ClientMessageEvent.window = HoverState.Window;
				xevent.ClientMessageEvent.message_type = HoverState.Atom;
				xevent.ClientMessageEvent.format = 32;
				xevent.ClientMessageEvent.ptr1 = (IntPtr)((HoverState.Y << 16) | HoverState.X);
				objectFromWindow.Queue.EnqueueLocked(xevent);
				WakeupMain();
			}
		}
	}

	private void CaretCallback(object sender, EventArgs e)
	{
		if (!Caret.Paused)
		{
			Caret.On = !Caret.On;
			XDrawLine(DisplayHandle, Caret.Hwnd, Caret.gc, Caret.X, Caret.Y, Caret.X, Caret.Y + Caret.Height);
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
		lock (this)
		{
			if (DisplayHandle == IntPtr.Zero)
			{
				SetDisplay(XOpenDisplay(IntPtr.Zero));
			}
		}
		return IntPtr.Zero;
	}

	internal override void ShutdownDriver(IntPtr token)
	{
		lock (this)
		{
			if (DisplayHandle != IntPtr.Zero)
			{
				XCloseDisplay(DisplayHandle);
				DisplayHandle = IntPtr.Zero;
			}
		}
	}

	internal override void EnableThemes()
	{
		themes_enabled = true;
	}

	internal override void Activate(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null)
		{
			return;
		}
		lock (XlibLock)
		{
			SendNetWMMessage(hwnd.whole_window, _NET_ACTIVE_WINDOW, (IntPtr)1, IntPtr.Zero, IntPtr.Zero);
			XEventQueue xEventQueue = null;
			lock (unattached_timer_list)
			{
				foreach (Timer item in unattached_timer_list)
				{
					if (xEventQueue == null)
					{
						xEventQueue = (XEventQueue)MessageQueues[Thread.CurrentThread];
					}
					item.thread = xEventQueue.Thread;
					xEventQueue.timer_list.Add(item);
				}
				unattached_timer_list.Clear();
			}
		}
	}

	internal override void AudibleAlert(AlertType alert)
	{
		XBell(DisplayHandle, 0);
	}

	internal override void CaretVisible(IntPtr handle, bool visible)
	{
		if (!(Caret.Hwnd == handle))
		{
			return;
		}
		if (visible)
		{
			if (!Caret.Visible)
			{
				Caret.Visible = true;
				ShowCaret();
				Caret.Timer.Start();
			}
		}
		else
		{
			Caret.Visible = false;
			Caret.Timer.Stop();
			HideCaret();
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
		int intdest_x_return;
		int dest_y_return;
		lock (XlibLock)
		{
			XTranslateCoordinates(DisplayHandle, hwnd.client_window, RootWindow, x, y, out intdest_x_return, out dest_y_return, out var _);
		}
		x = intdest_x_return;
		y = dest_y_return;
	}

	internal override int[] ClipboardAvailableFormats(IntPtr handle)
	{
		DataFormats.Format format = DataFormats.Format.List;
		if (XGetSelectionOwner(DisplayHandle, CLIPBOARD) == IntPtr.Zero)
		{
			return null;
		}
		Clipboard.Formats = new ArrayList();
		while (format != null)
		{
			XConvertSelection(DisplayHandle, CLIPBOARD, (IntPtr)format.Id, (IntPtr)format.Id, FosterParent, IntPtr.Zero);
			Clipboard.Enumerating = true;
			while (Clipboard.Enumerating)
			{
				UpdateMessageQueue(null);
			}
			format = format.Next;
		}
		int[] array = new int[Clipboard.Formats.Count];
		for (int i = 0; i < Clipboard.Formats.Count; i++)
		{
			array[i] = ((IntPtr)Clipboard.Formats[i]).ToInt32();
		}
		Clipboard.Formats = null;
		return array;
	}

	internal override void ClipboardClose(IntPtr handle)
	{
		if (handle != ClipMagic)
		{
			throw new ArgumentException("handle is not a valid clipboard handle");
		}
	}

	internal override int ClipboardGetID(IntPtr handle, string format)
	{
		if (handle != ClipMagic)
		{
			throw new ArgumentException("handle is not a valid clipboard handle");
		}
		return format switch
		{
			"Text" => 31, 
			"Bitmap" => 5, 
			"OEMText" => OEMTEXT.ToInt32(), 
			"DeviceIndependentBitmap" => 20, 
			"Palette" => 7, 
			"UnicodeText" => UTF16_STRING.ToInt32(), 
			"Rich Text Format" => RICHTEXTFORMAT.ToInt32(), 
			_ => XInternAtom(DisplayHandle, format, only_if_exists: false).ToInt32(), 
		};
	}

	internal override IntPtr ClipboardOpen(bool primary_selection)
	{
		if (!primary_selection)
		{
			ClipMagic = CLIPBOARD;
		}
		else
		{
			ClipMagic = PRIMARY;
		}
		return ClipMagic;
	}

	internal override object ClipboardRetrieve(IntPtr handle, int type, XplatUI.ClipboardToObject converter)
	{
		XConvertSelection(DisplayHandle, handle, (IntPtr)type, (IntPtr)type, FosterParent, IntPtr.Zero);
		Clipboard.Retrieving = true;
		while (Clipboard.Retrieving)
		{
			UpdateMessageQueue(null);
		}
		return Clipboard.Item;
	}

	internal override void ClipboardStore(IntPtr handle, object obj, int type, XplatUI.ObjectToClipboard converter)
	{
		Clipboard.Converter = converter;
		if (obj != null)
		{
			Clipboard.AddSource(type, obj);
			XSetSelectionOwner(DisplayHandle, CLIPBOARD, FosterParent, IntPtr.Zero);
		}
		else
		{
			Clipboard.ClearSources();
			XSetSelectionOwner(DisplayHandle, CLIPBOARD, IntPtr.Zero, IntPtr.Zero);
		}
	}

	internal override void CreateCaret(IntPtr handle, int width, int height)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (Caret.Hwnd != IntPtr.Zero)
		{
			DestroyCaret(Caret.Hwnd);
		}
		Caret.Hwnd = handle;
		Caret.Window = hwnd.client_window;
		Caret.Width = width;
		Caret.Height = height;
		Caret.Visible = false;
		Caret.On = false;
		XGCValues values = default(XGCValues);
		values.line_width = width;
		Caret.gc = XCreateGC(DisplayHandle, Caret.Window, new IntPtr(16), ref values);
		if (Caret.gc == IntPtr.Zero)
		{
			Caret.Hwnd = IntPtr.Zero;
		}
		else
		{
			XSetFunction(DisplayHandle, Caret.gc, GXFunction.GXinvert);
		}
	}

	internal override IntPtr CreateWindow(CreateParams cp)
	{
		Hwnd hwnd = null;
		Hwnd hwnd2 = new Hwnd();
		XSetWindowAttributes attributes = default(XSetWindowAttributes);
		int x = cp.X;
		int y = cp.Y;
		int num = cp.Width;
		int num2 = cp.Height;
		if (num < 1)
		{
			num = 1;
		}
		if (num2 < 1)
		{
			num2 = 1;
		}
		IntPtr intPtr;
		if (!(cp.Parent != IntPtr.Zero))
		{
			intPtr = ((!StyleSet(cp.Style, WindowStyles.WS_CHILD)) ? RootWindow : FosterParent);
		}
		else
		{
			hwnd = Hwnd.ObjectFromHandle(cp.Parent);
			intPtr = hwnd.client_window;
		}
		if (cp.control is Form)
		{
			Point nextStackedFormLocation = Hwnd.GetNextStackedFormLocation(cp, hwnd);
			x = nextStackedFormLocation.X;
			y = nextStackedFormLocation.Y;
		}
		SetWindowValuemask setWindowValuemask = SetWindowValuemask.BitGravity | SetWindowValuemask.WinGravity;
		attributes.bit_gravity = Gravity.NorthWestGravity;
		attributes.win_gravity = Gravity.NorthWestGravity;
		if (ExStyleSet(cp.ExStyle, WindowExStyles.WS_EX_TOOLWINDOW))
		{
			attributes.save_under = true;
			setWindowValuemask |= SetWindowValuemask.SaveUnder;
		}
		if (StyleSet(cp.Style, WindowStyles.WS_POPUP) && !StyleSet(cp.Style, WindowStyles.WS_CAPTION))
		{
			attributes.override_redirect = true;
			setWindowValuemask |= SetWindowValuemask.OverrideRedirect;
		}
		hwnd2.x = x;
		hwnd2.y = y;
		hwnd2.width = num;
		hwnd2.height = num2;
		hwnd2.parent = Hwnd.ObjectFromHandle(cp.Parent);
		hwnd2.initial_style = cp.WindowStyle;
		hwnd2.initial_ex_style = cp.WindowExStyle;
		if (StyleSet(cp.Style, WindowStyles.WS_DISABLED))
		{
			hwnd2.enabled = false;
		}
		IntPtr intPtr2 = IntPtr.Zero;
		Size size = TranslateWindowSizeToXWindowSize(cp);
		Rectangle rectangle = TranslateClientRectangleToXClientRectangle(hwnd2, cp.control);
		IntPtr intPtr3;
		lock (XlibLock)
		{
			intPtr3 = XCreateWindow(DisplayHandle, intPtr, x, y, size.Width, size.Height, 0, 0, 1, IntPtr.Zero, new UIntPtr((uint)setWindowValuemask), ref attributes);
			if (intPtr3 != IntPtr.Zero)
			{
				setWindowValuemask &= ~(SetWindowValuemask.OverrideRedirect | SetWindowValuemask.SaveUnder);
				if (CustomVisual != IntPtr.Zero && CustomColormap != IntPtr.Zero)
				{
					setWindowValuemask = SetWindowValuemask.ColorMap;
					attributes.colormap = CustomColormap;
				}
				intPtr2 = XCreateWindow(DisplayHandle, intPtr3, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0, 0, 1, CustomVisual, new UIntPtr((uint)setWindowValuemask), ref attributes);
			}
		}
		if (intPtr3 == IntPtr.Zero || intPtr2 == IntPtr.Zero)
		{
			throw new Exception("Could not create X11 windows");
		}
		hwnd2.Queue = ThreadQueue(Thread.CurrentThread);
		hwnd2.WholeWindow = intPtr3;
		hwnd2.ClientWindow = intPtr2;
		if (!StyleSet(cp.Style, WindowStyles.WS_CHILD) && x != int.MinValue && y != int.MinValue)
		{
			XSizeHints hints = default(XSizeHints);
			hints.x = x;
			hints.y = y;
			hints.flags = (IntPtr)5;
			XSetWMNormalHints(DisplayHandle, intPtr3, ref hints);
		}
		lock (XlibLock)
		{
			XSelectInput(DisplayHandle, hwnd2.whole_window, new IntPtr((int)(EventMask.KeyPressMask | EventMask.KeyReleaseMask | EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.EnterWindowMask | EventMask.LeaveWindowMask | EventMask.PointerMotionMask | EventMask.PointerMotionHintMask | EventMask.ExposureMask | EventMask.StructureNotifyMask | EventMask.SubstructureNotifyMask | EventMask.FocusChangeMask | EventMask.PropertyChangeMask | Keyboard.KeyEventMask)));
			if (hwnd2.whole_window != hwnd2.client_window)
			{
				XSelectInput(DisplayHandle, hwnd2.client_window, new IntPtr((int)(EventMask.KeyPressMask | EventMask.KeyReleaseMask | EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.EnterWindowMask | EventMask.LeaveWindowMask | EventMask.PointerMotionMask | EventMask.PointerMotionHintMask | EventMask.ExposureMask | EventMask.StructureNotifyMask | EventMask.SubstructureNotifyMask | EventMask.FocusChangeMask | Keyboard.KeyEventMask)));
			}
		}
		if (ExStyleSet(cp.ExStyle, WindowExStyles.WS_EX_TOPMOST))
		{
			XChangeProperty(data: new int[2]
			{
				_NET_WM_WINDOW_TYPE_NORMAL.ToInt32(),
				0
			}, display: DisplayHandle, window: hwnd2.whole_window, property: _NET_WM_WINDOW_TYPE, type: (IntPtr)4, format: 32, mode: PropertyMode.Replace, nelements: 1);
			XSetTransientForHint(DisplayHandle, hwnd2.whole_window, RootWindow);
		}
		SetWMStyles(hwnd2, cp);
		XWMHints wmhints = default(XWMHints);
		wmhints.flags = (IntPtr)67;
		wmhints.input = !StyleSet(cp.Style, WindowStyles.WS_DISABLED);
		wmhints.initial_state = ((!StyleSet(cp.Style, WindowStyles.WS_MINIMIZE)) ? XInitialState.NormalState : XInitialState.IconicState);
		if (intPtr != RootWindow)
		{
			wmhints.window_group = hwnd2.whole_window;
		}
		else
		{
			wmhints.window_group = intPtr;
		}
		lock (XlibLock)
		{
			XSetWMHints(DisplayHandle, hwnd2.whole_window, ref wmhints);
		}
		if (StyleSet(cp.Style, WindowStyles.WS_MINIMIZE))
		{
			SetWindowState(hwnd2.Handle, FormWindowState.Minimized);
		}
		else if (StyleSet(cp.Style, WindowStyles.WS_MAXIMIZE))
		{
			SetWindowState(hwnd2.Handle, FormWindowState.Maximized);
		}
		Dnd.SetAllowDrop(hwnd2, allow: true);
		Text(hwnd2.Handle, cp.Caption);
		SendMessage(hwnd2.Handle, Msg.WM_CREATE, (IntPtr)1, IntPtr.Zero);
		SendParentNotify(hwnd2.Handle, Msg.WM_CREATE, int.MaxValue, int.MaxValue);
		if (StyleSet(cp.Style, WindowStyles.WS_VISIBLE))
		{
			hwnd2.visible = true;
			MapWindow(hwnd2, WindowType.Both);
			if (!(Control.FromHandle(hwnd2.Handle) is Form))
			{
				SendMessage(hwnd2.Handle, Msg.WM_SHOWWINDOW, (IntPtr)1, IntPtr.Zero);
			}
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

	internal override IntPtr DefineCursor(Bitmap bitmap, Bitmap mask, Color cursor_pixel, Color mask_pixel, int xHotSpot, int yHotSpot)
	{
		if (XQueryBestCursor(DisplayHandle, RootWindow, bitmap.Width, bitmap.Height, out var best_width, out var best_height) == 0)
		{
			return IntPtr.Zero;
		}
		Bitmap bitmap2;
		Bitmap bitmap3;
		if (bitmap.Width != best_width || bitmap.Width != best_height)
		{
			bitmap2 = new Bitmap(bitmap, new Size(best_width, best_height));
			bitmap3 = new Bitmap(mask, new Size(best_width, best_height));
		}
		else
		{
			bitmap2 = bitmap;
			bitmap3 = mask;
		}
		best_width = bitmap2.Width;
		best_height = bitmap2.Height;
		byte[] array = new byte[best_width / 8 * best_height];
		byte[] array2 = new byte[best_width / 8 * best_height];
		for (int i = 0; i < best_height; i++)
		{
			for (int j = 0; j < best_width; j++)
			{
				Color pixel = bitmap2.GetPixel(j, i);
				Color pixel2 = bitmap3.GetPixel(j, i);
				bool flag = pixel == cursor_pixel;
				bool flag2 = pixel2 == mask_pixel;
				if (!flag && !flag2)
				{
					array2[i * best_width / 8 + j / 8] |= (byte)(1 << j % 8);
				}
				else if (flag && !flag2)
				{
					array[i * best_width / 8 + j / 8] |= (byte)(1 << j % 8);
					array2[i * best_width / 8 + j / 8] |= (byte)(1 << j % 8);
				}
			}
		}
		IntPtr intPtr = XCreatePixmapFromBitmapData(DisplayHandle, RootWindow, array, best_width, best_height, (IntPtr)1, (IntPtr)0, 1);
		IntPtr intPtr2 = XCreatePixmapFromBitmapData(DisplayHandle, RootWindow, array2, best_width, best_height, (IntPtr)1, (IntPtr)0, 1);
		XColor foreground_color = default(XColor);
		XColor background_color = default(XColor);
		foreground_color.pixel = XWhitePixel(DisplayHandle, ScreenNo);
		foreground_color.red = ushort.MaxValue;
		foreground_color.green = ushort.MaxValue;
		foreground_color.blue = ushort.MaxValue;
		background_color.pixel = XBlackPixel(DisplayHandle, ScreenNo);
		IntPtr result = XCreatePixmapCursor(DisplayHandle, intPtr, intPtr2, ref foreground_color, ref background_color, xHotSpot, yHotSpot);
		XFreePixmap(DisplayHandle, intPtr);
		XFreePixmap(DisplayHandle, intPtr2);
		return result;
	}

	internal override Bitmap DefineStdCursorBitmap(StdCursor id)
	{
		Bitmap bitmap = null;
		try
		{
			CursorFontShape cursorFontShape = StdCursorToFontShape(id);
			string file = cursorFontShape.ToString().Replace("XC_", string.Empty);
			int size = XcursorGetDefaultSize(DisplayHandle);
			IntPtr theme = XcursorGetTheme(DisplayHandle);
			IntPtr intPtr = XcursorLibraryLoadImages(file, theme, size);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			XcursorImages xcursorImages = (XcursorImages)Marshal.PtrToStructure(intPtr, typeof(XcursorImages));
			if (xcursorImages.nimage > 0)
			{
				XcursorImage xcursorImage = (XcursorImage)Marshal.PtrToStructure(Marshal.ReadIntPtr(xcursorImages.images), typeof(XcursorImage));
				if (xcursorImage.width <= 32767 && xcursorImage.height <= 32767)
				{
					int[] array = new int[xcursorImage.width * xcursorImage.height];
					Marshal.Copy(xcursorImage.pixels, array, 0, array.Length);
					bitmap = new Bitmap(xcursorImage.width, xcursorImage.height);
					for (int i = 0; i < xcursorImage.width; i++)
					{
						for (int j = 0; j < xcursorImage.height; j++)
						{
							bitmap.SetPixel(i, j, Color.FromArgb(array[j * xcursorImage.width + i]));
						}
					}
				}
			}
			XcursorImagesDestroy(intPtr);
			return bitmap;
		}
		catch (DllNotFoundException ex)
		{
			Console.WriteLine("Could not load libXcursor: " + ex.Message + " (" + ex.GetType().Name + ")");
			return null;
		}
	}

	internal override IntPtr DefineStdCursor(StdCursor id)
	{
		CursorFontShape shape = StdCursorToFontShape(id);
		lock (XlibLock)
		{
			return XCreateFontCursor(DisplayHandle, shape);
		}
	}

	internal static CursorFontShape StdCursorToFontShape(StdCursor id)
	{
		return id switch
		{
			StdCursor.AppStarting => CursorFontShape.XC_watch, 
			StdCursor.Arrow => CursorFontShape.XC_top_left_arrow, 
			StdCursor.Cross => CursorFontShape.XC_crosshair, 
			StdCursor.Default => CursorFontShape.XC_top_left_arrow, 
			StdCursor.Hand => CursorFontShape.XC_hand1, 
			StdCursor.Help => CursorFontShape.XC_question_arrow, 
			StdCursor.HSplit => CursorFontShape.XC_sb_v_double_arrow, 
			StdCursor.IBeam => CursorFontShape.XC_xterm, 
			StdCursor.No => CursorFontShape.XC_circle, 
			StdCursor.NoMove2D => CursorFontShape.XC_fleur, 
			StdCursor.NoMoveHoriz => CursorFontShape.XC_fleur, 
			StdCursor.NoMoveVert => CursorFontShape.XC_fleur, 
			StdCursor.PanEast => CursorFontShape.XC_fleur, 
			StdCursor.PanNE => CursorFontShape.XC_fleur, 
			StdCursor.PanNorth => CursorFontShape.XC_fleur, 
			StdCursor.PanNW => CursorFontShape.XC_fleur, 
			StdCursor.PanSE => CursorFontShape.XC_fleur, 
			StdCursor.PanSouth => CursorFontShape.XC_fleur, 
			StdCursor.PanSW => CursorFontShape.XC_fleur, 
			StdCursor.PanWest => CursorFontShape.XC_sizing, 
			StdCursor.SizeAll => CursorFontShape.XC_fleur, 
			StdCursor.SizeNESW => CursorFontShape.XC_top_right_corner, 
			StdCursor.SizeNS => CursorFontShape.XC_sb_v_double_arrow, 
			StdCursor.SizeNWSE => CursorFontShape.XC_top_left_corner, 
			StdCursor.SizeWE => CursorFontShape.XC_sb_h_double_arrow, 
			StdCursor.UpArrow => CursorFontShape.XC_center_ptr, 
			StdCursor.VSplit => CursorFontShape.XC_sb_h_double_arrow, 
			StdCursor.WaitCursor => CursorFontShape.XC_watch, 
			_ => CursorFontShape.XC_X_cursor, 
		};
	}

	internal override IntPtr DefWndProc(ref Message msg)
	{
		switch ((Msg)msg.Msg)
		{
		case Msg.WM_IME_COMPOSITION:
		{
			string compositionString = Keyboard.GetCompositionString();
			string text = compositionString;
			foreach (char c in text)
			{
				SendMessage(msg.HWnd, Msg.WM_IME_CHAR, (IntPtr)c, msg.LParam);
			}
			return IntPtr.Zero;
		}
		case Msg.WM_IME_CHAR:
			SendMessage(msg.HWnd, Msg.WM_CHAR, msg.WParam, msg.LParam);
			return IntPtr.Zero;
		case Msg.WM_PAINT:
		{
			Hwnd objectFromWindow5 = Hwnd.GetObjectFromWindow(msg.HWnd);
			if (objectFromWindow5 != null)
			{
				objectFromWindow5.expose_pending = false;
			}
			return IntPtr.Zero;
		}
		case Msg.WM_NCPAINT:
		{
			Hwnd objectFromWindow3 = Hwnd.GetObjectFromWindow(msg.HWnd);
			if (objectFromWindow3 != null)
			{
				objectFromWindow3.nc_expose_pending = false;
			}
			return IntPtr.Zero;
		}
		case Msg.WM_NCCALCSIZE:
			if (msg.WParam == (IntPtr)1)
			{
				Hwnd objectFromWindow = Hwnd.GetObjectFromWindow(msg.HWnd);
				XplatUIWin32.NCCALCSIZE_PARAMS nCCALCSIZE_PARAMS = (XplatUIWin32.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(msg.LParam, typeof(XplatUIWin32.NCCALCSIZE_PARAMS));
				Control control = Control.FromHandle(objectFromWindow.Handle);
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
			return IntPtr.Zero;
		case Msg.WM_CONTEXTMENU:
		{
			Hwnd objectFromWindow2 = Hwnd.GetObjectFromWindow(msg.HWnd);
			if (objectFromWindow2 != null && objectFromWindow2.parent != null)
			{
				SendMessage(objectFromWindow2.parent.client_window, Msg.WM_CONTEXTMENU, msg.WParam, msg.LParam);
			}
			return IntPtr.Zero;
		}
		case Msg.WM_MOUSEWHEEL:
		{
			Hwnd objectFromWindow4 = Hwnd.GetObjectFromWindow(msg.HWnd);
			if (objectFromWindow4 != null && objectFromWindow4.parent != null)
			{
				SendMessage(objectFromWindow4.parent.client_window, Msg.WM_MOUSEWHEEL, msg.WParam, msg.LParam);
				if (msg.Result == IntPtr.Zero)
				{
					return IntPtr.Zero;
				}
			}
			return IntPtr.Zero;
		}
		case Msg.WM_SETCURSOR:
		{
			Hwnd hwnd = Hwnd.GetObjectFromWindow(msg.HWnd);
			if (hwnd == null)
			{
				break;
			}
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
						AudibleAlert(AlertType.Default);
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
		}
		return IntPtr.Zero;
	}

	internal override void DestroyCaret(IntPtr handle)
	{
		if (Caret.Hwnd == handle)
		{
			if (Caret.Visible)
			{
				HideCaret();
				Caret.Timer.Stop();
			}
			if (Caret.gc != IntPtr.Zero)
			{
				XFreeGC(DisplayHandle, Caret.gc);
				Caret.gc = IntPtr.Zero;
			}
			Caret.Hwnd = IntPtr.Zero;
			Caret.Visible = false;
			Caret.On = false;
		}
	}

	internal override void DestroyCursor(IntPtr cursor)
	{
		lock (XlibLock)
		{
			XFreeCursor(DisplayHandle, cursor);
		}
	}

	internal override void DestroyWindow(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null || hwnd.zombie)
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
		lock (XlibLock)
		{
			if (hwnd.whole_window != IntPtr.Zero)
			{
				Keyboard.DestroyICForWindow(hwnd.whole_window);
				XDestroyWindow(DisplayHandle, hwnd.whole_window);
			}
			else if (hwnd.client_window != IntPtr.Zero)
			{
				Keyboard.DestroyICForWindow(hwnd.client_window);
				XDestroyWindow(DisplayHandle, hwnd.client_window);
			}
		}
	}

	internal override IntPtr DispatchMessage(ref MSG msg)
	{
		return NativeWindow.WndProc(msg.hwnd, msg.message, msg.wParam, msg.lParam);
	}

	private IntPtr GetReversibleScreenGC(Color backColor)
	{
		XColor colorcell_def = default(XColor);
		colorcell_def.red = (ushort)(backColor.R * 257);
		colorcell_def.green = (ushort)(backColor.G * 257);
		colorcell_def.blue = (ushort)(backColor.B * 257);
		XAllocColor(DisplayHandle, DefaultColormap, ref colorcell_def);
		uint num = (uint)colorcell_def.pixel.ToInt32();
		XGCValues values = default(XGCValues);
		values.subwindow_mode = GCSubwindowMode.IncludeInferiors;
		values.foreground = (IntPtr)num;
		IntPtr intPtr = XCreateGC(DisplayHandle, RootWindow, new IntPtr(32772), ref values);
		XSetForeground(DisplayHandle, intPtr, (UIntPtr)num);
		XSetFunction(DisplayHandle, intPtr, GXFunction.GXxor);
		return intPtr;
	}

	private IntPtr GetReversibleControlGC(Control control, int line_width)
	{
		XGCValues values = default(XGCValues);
		values.subwindow_mode = GCSubwindowMode.IncludeInferiors;
		values.line_width = line_width;
		values.foreground = XBlackPixel(DisplayHandle, ScreenNo);
		IntPtr intPtr = XCreateGC(DisplayHandle, control.Handle, new IntPtr(32788), ref values);
		XColor colorcell_def = default(XColor);
		colorcell_def.red = (ushort)(control.ForeColor.R * 257);
		colorcell_def.green = (ushort)(control.ForeColor.G * 257);
		colorcell_def.blue = (ushort)(control.ForeColor.B * 257);
		XAllocColor(DisplayHandle, DefaultColormap, ref colorcell_def);
		uint num = (uint)colorcell_def.pixel.ToInt32();
		colorcell_def.red = (ushort)(control.BackColor.R * 257);
		colorcell_def.green = (ushort)(control.BackColor.G * 257);
		colorcell_def.blue = (ushort)(control.BackColor.B * 257);
		XAllocColor(DisplayHandle, DefaultColormap, ref colorcell_def);
		uint num2 = (uint)colorcell_def.pixel.ToInt32();
		uint num3 = num ^ num2;
		XSetForeground(DisplayHandle, intPtr, (UIntPtr)uint.MaxValue);
		XSetBackground(DisplayHandle, intPtr, (UIntPtr)num2);
		XSetFunction(DisplayHandle, intPtr, GXFunction.GXxor);
		XSetPlaneMask(DisplayHandle, intPtr, (IntPtr)num3);
		return intPtr;
	}

	internal override void DrawReversibleLine(Point start, Point end, Color backColor)
	{
		if ((double)backColor.GetBrightness() < 0.5)
		{
			backColor = Color.FromArgb(255 - backColor.R, 255 - backColor.G, 255 - backColor.B);
		}
		IntPtr reversibleScreenGC = GetReversibleScreenGC(backColor);
		XDrawLine(DisplayHandle, RootWindow, reversibleScreenGC, start.X, start.Y, end.X, end.Y);
		XFreeGC(DisplayHandle, reversibleScreenGC);
	}

	internal override void DrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style)
	{
		if ((double)backColor.GetBrightness() < 0.5)
		{
			backColor = Color.FromArgb(255 - backColor.R, 255 - backColor.G, 255 - backColor.B);
		}
		IntPtr reversibleScreenGC = GetReversibleScreenGC(backColor);
		if (rectangle.Width < 0)
		{
			rectangle.X += rectangle.Width;
			rectangle.Width = -rectangle.Width;
		}
		if (rectangle.Height < 0)
		{
			rectangle.Y += rectangle.Height;
			rectangle.Height = -rectangle.Height;
		}
		int line_width = 1;
		GCLineStyle line_style = GCLineStyle.LineSolid;
		GCCapStyle cap_style = GCCapStyle.CapButt;
		GCJoinStyle join_style = GCJoinStyle.JoinMiter;
		switch (style)
		{
		case FrameStyle.Dashed:
			line_style = GCLineStyle.LineOnOffDash;
			break;
		case FrameStyle.Thick:
			line_width = 2;
			break;
		}
		XSetLineAttributes(DisplayHandle, reversibleScreenGC, line_width, line_style, cap_style, join_style);
		XDrawRectangle(DisplayHandle, RootWindow, reversibleScreenGC, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
		XFreeGC(DisplayHandle, reversibleScreenGC);
	}

	internal override void FillReversibleRectangle(Rectangle rectangle, Color backColor)
	{
		if ((double)backColor.GetBrightness() < 0.5)
		{
			backColor = Color.FromArgb(255 - backColor.R, 255 - backColor.G, 255 - backColor.B);
		}
		IntPtr reversibleScreenGC = GetReversibleScreenGC(backColor);
		if (rectangle.Width < 0)
		{
			rectangle.X += rectangle.Width;
			rectangle.Width = -rectangle.Width;
		}
		if (rectangle.Height < 0)
		{
			rectangle.Y += rectangle.Height;
			rectangle.Height = -rectangle.Height;
		}
		XFillRectangle(DisplayHandle, RootWindow, reversibleScreenGC, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
		XFreeGC(DisplayHandle, reversibleScreenGC);
	}

	internal override void DrawReversibleRectangle(IntPtr handle, Rectangle rect, int line_width)
	{
		Control control = Control.FromHandle(handle);
		IntPtr reversibleControlGC = GetReversibleControlGC(control, line_width);
		if (rect.Width > 0 && rect.Height > 0)
		{
			XDrawRectangle(DisplayHandle, control.Handle, reversibleControlGC, rect.Left, rect.Top, rect.Width, rect.Height);
		}
		else if (rect.Width > 0)
		{
			XDrawLine(DisplayHandle, control.Handle, reversibleControlGC, rect.X, rect.Y, rect.Right, rect.Y);
		}
		else
		{
			XDrawLine(DisplayHandle, control.Handle, reversibleControlGC, rect.X, rect.Y, rect.X, rect.Bottom);
		}
		XFreeGC(DisplayHandle, reversibleControlGC);
	}

	internal override void DoEvents()
	{
		MSG msg = default(MSG);
		if (OverrideCursorHandle != IntPtr.Zero)
		{
			OverrideCursorHandle = IntPtr.Zero;
		}
		XEventQueue xEventQueue = ThreadQueue(Thread.CurrentThread);
		xEventQueue.DispatchIdle = false;
		in_doevents = true;
		while (PeekMessage(xEventQueue, ref msg, IntPtr.Zero, 0, 0, 1u))
		{
			Message message = Message.Create(msg.hwnd, (int)msg.message, msg.wParam, msg.lParam);
			if (!Application.FilterMessage(ref message))
			{
				TranslateMessage(ref msg);
				DispatchMessage(ref msg);
				string key = msg.hwnd + ":" + msg.message;
				if (messageHold[key] != null)
				{
					messageHold[key] = (int)messageHold[key] - 1;
				}
			}
		}
		in_doevents = false;
		xEventQueue.DispatchIdle = true;
	}

	internal override void EnableWindow(IntPtr handle, bool Enable)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null)
		{
			hwnd.Enabled = Enable;
		}
	}

	internal override void EndLoop(Thread thread)
	{
	}

	internal override IntPtr GetActive()
	{
		IntPtr prop = IntPtr.Zero;
		IntPtr intPtr = IntPtr.Zero;
		XGetWindowProperty(DisplayHandle, RootWindow, _NET_ACTIVE_WINDOW, IntPtr.Zero, new IntPtr(1), delete: false, (IntPtr)33, out var _, out var _, out var nitems, out var _, ref prop);
		if ((long)nitems > 0 && prop != IntPtr.Zero)
		{
			intPtr = (IntPtr)Marshal.ReadInt32(prop);
			XFree(prop);
		}
		if (intPtr != IntPtr.Zero)
		{
			intPtr = Hwnd.GetObjectFromWindow(intPtr)?.Handle ?? IntPtr.Zero;
		}
		return intPtr;
	}

	internal override Region GetClipRegion(IntPtr handle)
	{
		return Hwnd.ObjectFromHandle(handle)?.UserClip;
	}

	internal override void GetCursorInfo(IntPtr cursor, out int width, out int height, out int hotspot_x, out int hotspot_y)
	{
		width = 20;
		height = 20;
		hotspot_x = 0;
		hotspot_y = 0;
	}

	internal override void GetDisplaySize(out Size size)
	{
		XWindowAttributes attributes = default(XWindowAttributes);
		lock (XlibLock)
		{
			XGetWindowAttributes(DisplayHandle, XRootWindow(DisplayHandle, 0), ref attributes);
		}
		size = new Size(attributes.width, attributes.height);
	}

	internal override SizeF GetAutoScaleSize(Font font)
	{
		string text = "The quick brown fox jumped over the lazy dog.";
		double num = 44.54999694824219;
		Graphics graphics = Graphics.FromHwnd(FosterParent);
		float width = (float)((double)graphics.MeasureString(text, font).Width / num);
		return new SizeF(width, font.Height);
	}

	internal override IntPtr GetParent(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null && hwnd.parent != null)
		{
			return hwnd.parent.Handle;
		}
		return IntPtr.Zero;
	}

	internal override IntPtr GetPreviousWindow(IntPtr handle)
	{
		return handle;
	}

	internal override void GetCursorPos(IntPtr handle, out int x, out int y)
	{
		IntPtr w = ((!(handle != IntPtr.Zero)) ? RootWindow : Hwnd.ObjectFromHandle(handle).client_window);
		int root_x;
		int root_y;
		int child_x;
		int child_y;
		lock (XlibLock)
		{
			QueryPointer(DisplayHandle, w, out var _, out var _, out root_x, out root_y, out child_x, out child_y, out var _);
		}
		if (handle != IntPtr.Zero)
		{
			x = child_x;
			y = child_y;
		}
		else
		{
			x = root_x;
			y = root_y;
		}
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

	[System.MonoTODO("Implement filtering")]
	internal override bool GetMessage(object queue_id, ref MSG msg, IntPtr handle, int wFilterMin, int wFilterMax)
	{
		while (true)
		{
			IL_0000:
			XEvent xevent;
			if (((XEventQueue)queue_id).Count > 0)
			{
				xevent = ((XEventQueue)queue_id).Dequeue();
			}
			else
			{
				UpdateMessageQueue((XEventQueue)queue_id);
				if (((XEventQueue)queue_id).Count > 0)
				{
					xevent = ((XEventQueue)queue_id).Dequeue();
				}
				else
				{
					if (((XEventQueue)queue_id).Paint.Count <= 0)
					{
						msg.hwnd = IntPtr.Zero;
						msg.message = Msg.WM_ENTERIDLE;
						return true;
					}
					xevent = ((XEventQueue)queue_id).Paint.Dequeue();
				}
			}
			Hwnd objectFromWindow = Hwnd.GetObjectFromWindow(xevent.AnyEvent.window);
			if (objectFromWindow != null && objectFromWindow.zombie && xevent.type == XEventName.Expose)
			{
				objectFromWindow.expose_pending = (objectFromWindow.nc_expose_pending = false);
				objectFromWindow.Queue.Paint.Remove(objectFromWindow);
			}
			else
			{
				if (objectFromWindow == null || (objectFromWindow.zombie && xevent.AnyEvent.type != XEventName.ClientMessage))
				{
					continue;
				}
				if (objectFromWindow.zombie)
				{
					objectFromWindow.resizing_or_moving = false;
				}
				bool flag = ((objectFromWindow.client_window == xevent.AnyEvent.window) ? true : false);
				msg.hwnd = objectFromWindow.Handle;
				if (objectFromWindow.resizing_or_moving)
				{
					XQueryPointer(DisplayHandle, objectFromWindow.Handle, out var _, out var _, out var _, out var _, out var _, out var _, out var keys_buttons);
					if ((keys_buttons & 0x100) == 0 && (keys_buttons & 0x200) == 0 && (keys_buttons & 0x400) == 0)
					{
						objectFromWindow.resizing_or_moving = false;
						SendMessage(objectFromWindow.Handle, Msg.WM_EXITSIZEMOVE, IntPtr.Zero, IntPtr.Zero);
					}
				}
				switch (xevent.type)
				{
				default:
					continue;
				case XEventName.KeyPress:
					Keyboard.KeyEvent(FocusWindow, xevent, ref msg);
					if (msg.wParam == (IntPtr)112 || msg.wParam == (IntPtr)47)
					{
						HELPINFO hELPINFO = default(HELPINFO);
						GetCursorPos(IntPtr.Zero, out hELPINFO.MousePos.x, out hELPINFO.MousePos.y);
						IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(hELPINFO));
						Marshal.StructureToPtr(hELPINFO, intPtr, fDeleteOld: true);
						NativeWindow.WndProc(FocusWindow, Msg.WM_HELP, IntPtr.Zero, intPtr);
						Marshal.FreeHGlobal(intPtr);
					}
					break;
				case XEventName.KeyRelease:
					Keyboard.KeyEvent(FocusWindow, xevent, ref msg);
					break;
				case XEventName.ButtonPress:
					switch (xevent.ButtonEvent.button)
					{
					case 1:
						MouseState |= MouseButtons.Left;
						if (flag)
						{
							msg.message = Msg.WM_LBUTTONDOWN;
							msg.wParam = GetMousewParam(0);
						}
						else
						{
							msg.message = Msg.WM_NCLBUTTONDOWN;
							msg.wParam = (IntPtr)(int)NCHitTest(objectFromWindow, xevent.MotionEvent.x, xevent.MotionEvent.y);
							MenuToScreen(xevent.AnyEvent.window, ref xevent.ButtonEvent.x, ref xevent.ButtonEvent.y);
						}
						break;
					case 2:
						MouseState |= MouseButtons.Middle;
						if (flag)
						{
							msg.message = Msg.WM_MBUTTONDOWN;
							msg.wParam = GetMousewParam(0);
						}
						else
						{
							msg.message = Msg.WM_NCMBUTTONDOWN;
							msg.wParam = (IntPtr)(int)NCHitTest(objectFromWindow, xevent.MotionEvent.x, xevent.MotionEvent.y);
							MenuToScreen(xevent.AnyEvent.window, ref xevent.ButtonEvent.x, ref xevent.ButtonEvent.y);
						}
						break;
					case 3:
						MouseState |= MouseButtons.Right;
						if (flag)
						{
							msg.message = Msg.WM_RBUTTONDOWN;
							msg.wParam = GetMousewParam(0);
						}
						else
						{
							msg.message = Msg.WM_NCRBUTTONDOWN;
							msg.wParam = (IntPtr)(int)NCHitTest(objectFromWindow, xevent.MotionEvent.x, xevent.MotionEvent.y);
							MenuToScreen(xevent.AnyEvent.window, ref xevent.ButtonEvent.x, ref xevent.ButtonEvent.y);
						}
						break;
					case 4:
						msg.hwnd = FocusWindow;
						msg.message = Msg.WM_MOUSEWHEEL;
						msg.wParam = GetMousewParam(120);
						break;
					case 5:
						msg.hwnd = FocusWindow;
						msg.message = Msg.WM_MOUSEWHEEL;
						msg.wParam = GetMousewParam(-120);
						break;
					}
					msg.lParam = (IntPtr)((xevent.ButtonEvent.y << 16) | xevent.ButtonEvent.x);
					mouse_position.X = xevent.ButtonEvent.x;
					mouse_position.Y = xevent.ButtonEvent.y;
					if (!objectFromWindow.Enabled)
					{
						msg.hwnd = objectFromWindow.EnabledHwnd;
						XTranslateCoordinates(DisplayHandle, xevent.AnyEvent.window, Hwnd.ObjectFromHandle(msg.hwnd).ClientWindow, xevent.ButtonEvent.x, xevent.ButtonEvent.y, out xevent.ButtonEvent.x, out xevent.ButtonEvent.y, out var _);
						msg.lParam = (IntPtr)((mouse_position.Y << 16) | mouse_position.X);
					}
					if (Grab.Hwnd != IntPtr.Zero)
					{
						msg.hwnd = Grab.Hwnd;
					}
					if (ClickPending.Pending && (long)xevent.ButtonEvent.time - ClickPending.Time < DoubleClickInterval && msg.wParam == ClickPending.wParam && msg.lParam == ClickPending.lParam && msg.message == ClickPending.Message)
					{
						switch (xevent.ButtonEvent.button)
						{
						case 1:
							msg.message = ((!flag) ? Msg.WM_NCLBUTTONDBLCLK : Msg.WM_LBUTTONDBLCLK);
							break;
						case 2:
							msg.message = ((!flag) ? Msg.WM_NCMBUTTONDBLCLK : Msg.WM_MBUTTONDBLCLK);
							break;
						case 3:
							msg.message = ((!flag) ? Msg.WM_NCRBUTTONDBLCLK : Msg.WM_RBUTTONDBLCLK);
							break;
						}
						ClickPending.Pending = false;
					}
					else
					{
						ClickPending.Pending = true;
						ClickPending.Hwnd = msg.hwnd;
						ClickPending.Message = msg.message;
						ClickPending.wParam = msg.wParam;
						ClickPending.lParam = msg.lParam;
						ClickPending.Time = (long)xevent.ButtonEvent.time;
					}
					if (msg.message == Msg.WM_LBUTTONDOWN || msg.message == Msg.WM_MBUTTONDOWN || msg.message == Msg.WM_RBUTTONDOWN)
					{
						SendParentNotify(msg.hwnd, msg.message, mouse_position.X, mouse_position.Y);
					}
					break;
				case XEventName.ButtonRelease:
					switch (xevent.ButtonEvent.button)
					{
					case 4:
					case 5:
						continue;
					case 1:
						if (flag)
						{
							msg.message = Msg.WM_LBUTTONUP;
						}
						else
						{
							msg.message = Msg.WM_NCLBUTTONUP;
							msg.wParam = (IntPtr)(int)NCHitTest(objectFromWindow, xevent.MotionEvent.x, xevent.MotionEvent.y);
							MenuToScreen(xevent.AnyEvent.window, ref xevent.ButtonEvent.x, ref xevent.ButtonEvent.y);
						}
						MouseState &= ~MouseButtons.Left;
						msg.wParam = GetMousewParam(0);
						break;
					case 2:
						if (flag)
						{
							msg.message = Msg.WM_MBUTTONUP;
						}
						else
						{
							msg.message = Msg.WM_NCMBUTTONUP;
							msg.wParam = (IntPtr)(int)NCHitTest(objectFromWindow, xevent.MotionEvent.x, xevent.MotionEvent.y);
							MenuToScreen(xevent.AnyEvent.window, ref xevent.ButtonEvent.x, ref xevent.ButtonEvent.y);
						}
						MouseState &= ~MouseButtons.Middle;
						msg.wParam = GetMousewParam(0);
						break;
					case 3:
						if (flag)
						{
							msg.message = Msg.WM_RBUTTONUP;
						}
						else
						{
							msg.message = Msg.WM_NCRBUTTONUP;
							msg.wParam = (IntPtr)(int)NCHitTest(objectFromWindow, xevent.MotionEvent.x, xevent.MotionEvent.y);
							MenuToScreen(xevent.AnyEvent.window, ref xevent.ButtonEvent.x, ref xevent.ButtonEvent.y);
						}
						MouseState &= ~MouseButtons.Right;
						msg.wParam = GetMousewParam(0);
						break;
					}
					if (!objectFromWindow.Enabled)
					{
						msg.hwnd = objectFromWindow.EnabledHwnd;
						XTranslateCoordinates(DisplayHandle, xevent.AnyEvent.window, Hwnd.ObjectFromHandle(msg.hwnd).ClientWindow, xevent.ButtonEvent.x, xevent.ButtonEvent.y, out xevent.ButtonEvent.x, out xevent.ButtonEvent.y, out var _);
						msg.lParam = (IntPtr)((mouse_position.Y << 16) | mouse_position.X);
					}
					if (Grab.Hwnd != IntPtr.Zero)
					{
						msg.hwnd = Grab.Hwnd;
					}
					msg.lParam = (IntPtr)((xevent.ButtonEvent.y << 16) | xevent.ButtonEvent.x);
					mouse_position.X = xevent.ButtonEvent.x;
					mouse_position.Y = xevent.ButtonEvent.y;
					if (msg.message == Msg.WM_LBUTTONUP || msg.message == Msg.WM_MBUTTONUP || msg.message == Msg.WM_RBUTTONUP)
					{
						XEvent xevent4 = default(XEvent);
						xevent4.type = XEventName.MotionNotify;
						xevent4.MotionEvent.display = DisplayHandle;
						xevent4.MotionEvent.window = xevent.ButtonEvent.window;
						xevent4.MotionEvent.x = xevent.ButtonEvent.x;
						xevent4.MotionEvent.y = xevent.ButtonEvent.y;
						objectFromWindow.Queue.EnqueueLocked(xevent4);
					}
					break;
				case XEventName.MotionNotify:
					if (flag)
					{
						if (Grab.Hwnd != IntPtr.Zero)
						{
							msg.hwnd = Grab.Hwnd;
						}
						else if (objectFromWindow.Enabled)
						{
							NativeWindow.WndProc(msg.hwnd, Msg.WM_SETCURSOR, msg.hwnd, (IntPtr)1);
						}
						if (xevent.MotionEvent.is_hint != 0)
						{
							XQueryPointer(DisplayHandle, xevent.AnyEvent.window, out var _, out var _, out xevent.MotionEvent.x_root, out xevent.MotionEvent.y_root, out xevent.MotionEvent.x, out xevent.MotionEvent.y, out var _);
						}
						msg.message = Msg.WM_MOUSEMOVE;
						msg.wParam = GetMousewParam(0);
						msg.lParam = (IntPtr)((xevent.MotionEvent.y << 16) | (xevent.MotionEvent.x & 0xFFFF));
						if (!objectFromWindow.Enabled)
						{
							msg.hwnd = objectFromWindow.EnabledHwnd;
							XTranslateCoordinates(DisplayHandle, xevent.AnyEvent.window, Hwnd.ObjectFromHandle(msg.hwnd).ClientWindow, xevent.MotionEvent.x, xevent.MotionEvent.y, out xevent.MotionEvent.x, out xevent.MotionEvent.y, out var _);
							msg.lParam = (IntPtr)((mouse_position.Y << 16) | mouse_position.X);
						}
						mouse_position.X = xevent.MotionEvent.x;
						mouse_position.Y = xevent.MotionEvent.y;
						if (HoverState.Timer.Enabled && (mouse_position.X + HoverState.Size.Width < HoverState.X || mouse_position.X - HoverState.Size.Width > HoverState.X || mouse_position.Y + HoverState.Size.Height < HoverState.Y || mouse_position.Y - HoverState.Size.Height > HoverState.Y))
						{
							HoverState.Timer.Stop();
							HoverState.Timer.Start();
							HoverState.X = mouse_position.X;
							HoverState.Y = mouse_position.Y;
						}
					}
					else
					{
						msg.message = Msg.WM_NCMOUSEMOVE;
						if (!objectFromWindow.Enabled)
						{
							msg.hwnd = objectFromWindow.EnabledHwnd;
							XTranslateCoordinates(DisplayHandle, xevent.AnyEvent.window, Hwnd.ObjectFromHandle(msg.hwnd).ClientWindow, xevent.MotionEvent.x, xevent.MotionEvent.y, out xevent.MotionEvent.x, out xevent.MotionEvent.y, out var _);
							msg.lParam = (IntPtr)((mouse_position.Y << 16) | mouse_position.X);
						}
						HitTest hitTest = NCHitTest(objectFromWindow, xevent.MotionEvent.x, xevent.MotionEvent.y);
						NativeWindow.WndProc(objectFromWindow.client_window, Msg.WM_SETCURSOR, msg.hwnd, (IntPtr)(int)hitTest);
						mouse_position.X = xevent.MotionEvent.x;
						mouse_position.Y = xevent.MotionEvent.y;
					}
					break;
				case XEventName.EnterNotify:
				{
					if (!objectFromWindow.Enabled || xevent.CrossingEvent.mode == NotifyMode.NotifyGrab || xevent.AnyEvent.window != objectFromWindow.client_window)
					{
						continue;
					}
					if (xevent.CrossingEvent.mode == NotifyMode.NotifyUngrab)
					{
						if (LastPointerWindow == xevent.AnyEvent.window)
						{
							continue;
						}
						if (LastPointerWindow != IntPtr.Zero)
						{
							Point pt = new Point(xevent.ButtonEvent.x, xevent.ButtonEvent.y);
							Control control = Control.FromHandle(objectFromWindow.client_window);
							Control[] allControls = control.Controls.GetAllControls();
							foreach (Control control2 in allControls)
							{
								if (control2.Bounds.Contains(pt))
								{
									goto IL_0000;
								}
							}
							int x = xevent.CrossingEvent.x_root;
							int y = xevent.CrossingEvent.y_root;
							ScreenToClient(LastPointerWindow, ref x, ref y);
							XEvent xevent2 = default(XEvent);
							xevent2.type = XEventName.LeaveNotify;
							xevent2.CrossingEvent.display = DisplayHandle;
							xevent2.CrossingEvent.window = LastPointerWindow;
							xevent2.CrossingEvent.x = x;
							xevent2.CrossingEvent.y = y;
							xevent2.CrossingEvent.mode = NotifyMode.NotifyNormal;
							Hwnd hwnd = Hwnd.ObjectFromHandle(LastPointerWindow);
							hwnd.Queue.EnqueueLocked(xevent2);
						}
					}
					LastPointerWindow = xevent.AnyEvent.window;
					msg.message = Msg.WM_MOUSE_ENTER;
					HoverState.X = xevent.CrossingEvent.x;
					HoverState.Y = xevent.CrossingEvent.y;
					HoverState.Timer.Enabled = true;
					HoverState.Window = xevent.CrossingEvent.window;
					XEvent xevent3 = default(XEvent);
					xevent3.type = XEventName.MotionNotify;
					xevent3.MotionEvent.display = DisplayHandle;
					xevent3.MotionEvent.window = xevent.ButtonEvent.window;
					xevent3.MotionEvent.x = xevent.ButtonEvent.x;
					xevent3.MotionEvent.y = xevent.ButtonEvent.y;
					objectFromWindow.Queue.EnqueueLocked(xevent3);
					break;
				}
				case XEventName.LeaveNotify:
					if (xevent.CrossingEvent.mode == NotifyMode.NotifyUngrab)
					{
						WindowUngrabbed(objectFromWindow.Handle);
						continue;
					}
					if (!objectFromWindow.Enabled || xevent.CrossingEvent.mode != 0 || xevent.CrossingEvent.window != objectFromWindow.client_window || Grab.Hwnd != IntPtr.Zero)
					{
						continue;
					}
					SetCursor(objectFromWindow.client_window, IntPtr.Zero);
					msg.message = Msg.WM_MOUSELEAVE;
					HoverState.Timer.Enabled = false;
					HoverState.Window = IntPtr.Zero;
					break;
				case XEventName.ReparentNotify:
					if (objectFromWindow.parent != null)
					{
						continue;
					}
					if (xevent.ReparentEvent.parent != IntPtr.Zero && xevent.ReparentEvent.window == objectFromWindow.whole_window)
					{
						objectFromWindow.Reparented = true;
						Point topLevelWindowLocation = GetTopLevelWindowLocation(objectFromWindow);
						objectFromWindow.X = topLevelWindowLocation.X;
						objectFromWindow.Y = topLevelWindowLocation.Y;
						if (objectFromWindow.opacity != uint.MaxValue)
						{
							IntPtr value = (IntPtr)(int)objectFromWindow.opacity;
							XChangeProperty(DisplayHandle, XGetParent(objectFromWindow.whole_window), _NET_WM_WINDOW_OPACITY, (IntPtr)6, 32, PropertyMode.Replace, ref value, 1);
						}
						SendMessage(msg.hwnd, Msg.WM_WINDOWPOSCHANGED, msg.wParam, msg.lParam);
					}
					else
					{
						objectFromWindow.Reparented = false;
					}
					continue;
				case XEventName.ConfigureNotify:
					if (flag || !(xevent.ConfigureEvent.xevent == xevent.ConfigureEvent.window))
					{
						continue;
					}
					lock (objectFromWindow.configure_lock)
					{
						if (Control.FromHandle(objectFromWindow.client_window) is Form form && !objectFromWindow.resizing_or_moving)
						{
							if (objectFromWindow.x != form.Bounds.X || objectFromWindow.y != form.Bounds.Y)
							{
								SendMessage(form.Handle, Msg.WM_SYSCOMMAND, (IntPtr)61456, IntPtr.Zero);
								objectFromWindow.resizing_or_moving = true;
							}
							else if (objectFromWindow.width != form.Bounds.Width || objectFromWindow.height != form.Bounds.Height)
							{
								SendMessage(form.Handle, Msg.WM_SYSCOMMAND, (IntPtr)61440, IntPtr.Zero);
								objectFromWindow.resizing_or_moving = true;
							}
							if (objectFromWindow.resizing_or_moving)
							{
								SendMessage(form.Handle, Msg.WM_ENTERSIZEMOVE, IntPtr.Zero, IntPtr.Zero);
							}
						}
						SendMessage(msg.hwnd, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
						objectFromWindow.configure_pending = false;
						if (objectFromWindow.whole_window != objectFromWindow.client_window)
						{
							PerformNCCalc(objectFromWindow);
						}
					}
					continue;
				case XEventName.FocusIn:
					if (xevent.FocusChangeEvent.detail != NotifyDetail.NotifyNonlinear)
					{
						continue;
					}
					if (FocusWindow == IntPtr.Zero)
					{
						Control control3 = Control.FromHandle(objectFromWindow.client_window);
						if (control3 != null)
						{
							Form form2 = control3.FindForm();
							if (form2 != null && ActiveWindow != form2.Handle)
							{
								ActiveWindow = form2.Handle;
								SendMessage(ActiveWindow, Msg.WM_ACTIVATE, (IntPtr)1, IntPtr.Zero);
							}
						}
					}
					else
					{
						SendMessage(FocusWindow, Msg.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
						Keyboard.FocusIn(FocusWindow);
					}
					continue;
				case XEventName.FocusOut:
					if (xevent.FocusChangeEvent.detail == NotifyDetail.NotifyNonlinear)
					{
						while (Keyboard.ResetKeyState(FocusWindow, ref msg))
						{
							SendMessage(FocusWindow, msg.message, msg.wParam, msg.lParam);
						}
						Keyboard.FocusOut(objectFromWindow.client_window);
						SendMessage(FocusWindow, Msg.WM_KILLFOCUS, IntPtr.Zero, IntPtr.Zero);
					}
					continue;
				case XEventName.Expose:
				{
					if (!objectFromWindow.Mapped)
					{
						if (flag)
						{
							objectFromWindow.expose_pending = false;
						}
						else
						{
							objectFromWindow.nc_expose_pending = false;
						}
						continue;
					}
					if (flag)
					{
						if (!objectFromWindow.expose_pending)
						{
							continue;
						}
						if (Caret.Visible)
						{
							Caret.Paused = true;
							HideCaret();
						}
						if (Caret.Visible)
						{
							ShowCaret();
							Caret.Paused = false;
						}
						msg.message = Msg.WM_PAINT;
						break;
					}
					if (!objectFromWindow.nc_expose_pending)
					{
						continue;
					}
					switch (objectFromWindow.border_style)
					{
					case FormBorderStyle.Fixed3D:
					{
						Graphics graphics2 = Graphics.FromHwnd(objectFromWindow.whole_window);
						if (objectFromWindow.border_static)
						{
							ControlPaint.DrawBorder3D(graphics2, new Rectangle(0, 0, objectFromWindow.Width, objectFromWindow.Height), Border3DStyle.SunkenOuter);
						}
						else
						{
							ControlPaint.DrawBorder3D(graphics2, new Rectangle(0, 0, objectFromWindow.Width, objectFromWindow.Height), Border3DStyle.Sunken);
						}
						graphics2.Dispose();
						break;
					}
					case FormBorderStyle.FixedSingle:
					{
						Graphics graphics = Graphics.FromHwnd(objectFromWindow.whole_window);
						ControlPaint.DrawBorder(graphics, new Rectangle(0, 0, objectFromWindow.Width, objectFromWindow.Height), Color.Black, ButtonBorderStyle.Solid);
						graphics.Dispose();
						break;
					}
					}
					Rectangle rect = new Rectangle(xevent.ExposeEvent.x, xevent.ExposeEvent.y, xevent.ExposeEvent.width, xevent.ExposeEvent.height);
					Region region = new Region(rect);
					IntPtr hrgn = region.GetHrgn(null);
					msg.message = Msg.WM_NCPAINT;
					msg.wParam = ((!(hrgn == IntPtr.Zero)) ? hrgn : ((IntPtr)1));
					msg.refobject = region;
					break;
				}
				case XEventName.DestroyNotify:
					objectFromWindow = Hwnd.ObjectFromHandle(xevent.DestroyWindowEvent.window);
					if (objectFromWindow != null && objectFromWindow.client_window == xevent.DestroyWindowEvent.window)
					{
						CleanupCachedWindows(objectFromWindow);
						msg.hwnd = objectFromWindow.client_window;
						msg.message = Msg.WM_DESTROY;
						objectFromWindow.Dispose();
						break;
					}
					continue;
				case XEventName.ClientMessage:
					if (Dnd.HandleClientMessage(ref xevent))
					{
						continue;
					}
					if (xevent.ClientMessageEvent.message_type == AsyncAtom)
					{
						XplatUIDriverSupport.ExecuteClientMessage((GCHandle)xevent.ClientMessageEvent.ptr1);
						continue;
					}
					if (xevent.ClientMessageEvent.message_type == HoverState.Atom)
					{
						msg.message = Msg.WM_MOUSEHOVER;
						msg.wParam = GetMousewParam(0);
						msg.lParam = xevent.ClientMessageEvent.ptr1;
						return true;
					}
					if (xevent.ClientMessageEvent.message_type == PostAtom)
					{
						msg.hwnd = xevent.ClientMessageEvent.ptr1;
						msg.message = (Msg)xevent.ClientMessageEvent.ptr2.ToInt32();
						msg.wParam = xevent.ClientMessageEvent.ptr3;
						msg.lParam = xevent.ClientMessageEvent.ptr4;
						if (msg.message == Msg.WM_QUIT)
						{
							return false;
						}
						return true;
					}
					if (xevent.ClientMessageEvent.message_type == _XEMBED && xevent.ClientMessageEvent.ptr2.ToInt32() == 0)
					{
						XSizeHints hints = default(XSizeHints);
						XGetWMNormalHints(DisplayHandle, objectFromWindow.whole_window, ref hints, out var _);
						objectFromWindow.width = hints.max_width;
						objectFromWindow.height = hints.max_height;
						objectFromWindow.ClientRect = Rectangle.Empty;
						SendMessage(msg.hwnd, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
					}
					if (xevent.ClientMessageEvent.message_type == WM_PROTOCOLS)
					{
						if (xevent.ClientMessageEvent.ptr1 == WM_DELETE_WINDOW)
						{
							SendMessage(msg.hwnd, Msg.WM_SYSCOMMAND, (IntPtr)61536, IntPtr.Zero);
							msg.message = Msg.WM_CLOSE;
							return true;
						}
						if (!(xevent.ClientMessageEvent.ptr1 == WM_TAKE_FOCUS))
						{
						}
					}
					continue;
				}
				break;
			}
		}
		return true;
	}

	private HitTest NCHitTest(Hwnd hwnd, int x, int y)
	{
		XTranslateCoordinates(DisplayHandle, hwnd.WholeWindow, RootWindow, x, y, out var intdest_x_return, out var dest_y_return, out var _);
		return (HitTest)(int)NativeWindow.WndProc(hwnd.client_window, Msg.WM_NCHITTEST, IntPtr.Zero, (IntPtr)((dest_y_return << 16) | (intdest_x_return & 0xFFFF)));
	}

	internal override bool GetText(IntPtr handle, out string text)
	{
		lock (XlibLock)
		{
			IntPtr prop = IntPtr.Zero;
			XGetWindowProperty(DisplayHandle, handle, _NET_WM_NAME, IntPtr.Zero, new IntPtr(1), delete: false, UTF8_STRING, out var _, out var _, out var nitems, out var _, ref prop);
			if ((long)nitems > 0 && prop != IntPtr.Zero)
			{
				text = Marshal.PtrToStringUni(prop, (int)nitems);
				XFree(prop);
				return true;
			}
			IntPtr window_name = IntPtr.Zero;
			XFetchName(DisplayHandle, Hwnd.ObjectFromHandle(handle).whole_window, ref window_name);
			if (window_name != IntPtr.Zero)
			{
				text = Marshal.PtrToStringAnsi(window_name);
				XFree(window_name);
				return true;
			}
			text = string.Empty;
			return false;
		}
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

	internal override FormWindowState GetWindowState(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd.cached_window_state == (FormWindowState)(-1))
		{
			hwnd.cached_window_state = UpdateWindowState(handle);
		}
		return hwnd.cached_window_state;
	}

	private FormWindowState UpdateWindowState(IntPtr handle)
	{
		IntPtr prop = IntPtr.Zero;
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		int num = 0;
		bool flag = false;
		XGetWindowProperty(DisplayHandle, hwnd.whole_window, _NET_WM_STATE, IntPtr.Zero, new IntPtr(256), delete: false, (IntPtr)4, out var _, out var _, out var nitems, out var _, ref prop);
		if ((long)nitems > 0 && prop != IntPtr.Zero)
		{
			for (int i = 0; i < (long)nitems; i++)
			{
				IntPtr intPtr = (IntPtr)Marshal.ReadInt32(prop, i * 4);
				if (intPtr == _NET_WM_STATE_MAXIMIZED_HORZ || intPtr == _NET_WM_STATE_MAXIMIZED_VERT)
				{
					num++;
				}
				else if (intPtr == _NET_WM_STATE_HIDDEN)
				{
					flag = true;
				}
			}
			XFree(prop);
		}
		if (flag)
		{
			return FormWindowState.Minimized;
		}
		if (num == 2)
		{
			return FormWindowState.Maximized;
		}
		XWindowAttributes attributes = default(XWindowAttributes);
		XGetWindowAttributes(DisplayHandle, hwnd.client_window, ref attributes);
		if (attributes.map_state == MapState.IsUnmapped)
		{
			return (FormWindowState)(-1);
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
		IntPtr confine_to = IntPtr.Zero;
		Hwnd hwnd;
		if (confine_to_handle != IntPtr.Zero)
		{
			XWindowAttributes attributes = default(XWindowAttributes);
			hwnd = Hwnd.ObjectFromHandle(confine_to_handle);
			lock (XlibLock)
			{
				XGetWindowAttributes(DisplayHandle, hwnd.client_window, ref attributes);
			}
			Grab.Area.X = attributes.x;
			Grab.Area.Y = attributes.y;
			Grab.Area.Width = attributes.width;
			Grab.Area.Height = attributes.height;
			Grab.Confined = true;
			confine_to = hwnd.client_window;
		}
		Grab.Hwnd = handle;
		hwnd = Hwnd.ObjectFromHandle(handle);
		lock (XlibLock)
		{
			XGrabPointer(DisplayHandle, hwnd.client_window, owner_events: false, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.LeaveWindowMask | EventMask.PointerMotionMask | EventMask.PointerMotionHintMask | EventMask.ButtonMotionMask, GrabMode.GrabModeAsync, GrabMode.GrabModeAsync, confine_to, IntPtr.Zero, IntPtr.Zero);
		}
	}

	internal override void UngrabWindow(IntPtr hwnd)
	{
		lock (XlibLock)
		{
			XUngrabPointer(DisplayHandle, IntPtr.Zero);
			XFlush(DisplayHandle);
		}
		WindowUngrabbed(hwnd);
	}

	private void WindowUngrabbed(IntPtr hwnd)
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
		StackTrace stackTrace = new StackTrace(e, fNeedFileInfo: true);
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
		AddExpose(hwnd, hwnd.WholeWindow == hwnd.ClientWindow, 0, 0, hwnd.Width, hwnd.Height);
	}

	internal override bool IsEnabled(IntPtr handle)
	{
		return Hwnd.ObjectFromHandle(handle)?.Enabled ?? false;
	}

	internal override bool IsVisible(IntPtr handle)
	{
		return Hwnd.ObjectFromHandle(handle)?.visible ?? false;
	}

	internal override void KillTimer(Timer timer)
	{
		XEventQueue xEventQueue = (XEventQueue)MessageQueues[timer.thread];
		if (xEventQueue == null)
		{
			lock (unattached_timer_list)
			{
				if (unattached_timer_list.Contains(timer))
				{
					unattached_timer_list.Remove(timer);
				}
				return;
			}
		}
		xEventQueue.timer_list.Remove(timer);
	}

	internal override void MenuToScreen(IntPtr handle, ref int x, ref int y)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		int intdest_x_return;
		int dest_y_return;
		lock (XlibLock)
		{
			XTranslateCoordinates(DisplayHandle, hwnd.whole_window, RootWindow, x, y, out intdest_x_return, out dest_y_return, out var _);
		}
		x = intdest_x_return;
		y = dest_y_return;
	}

	internal override void OverrideCursor(IntPtr cursor)
	{
		if (Grab.Hwnd != IntPtr.Zero)
		{
			XChangeActivePointerGrab(DisplayHandle, EventMask.ButtonPressMask | EventMask.ButtonReleaseMask | EventMask.PointerMotionMask | EventMask.PointerMotionHintMask | EventMask.ButtonMotionMask, cursor, IntPtr.Zero);
		}
		else
		{
			OverrideCursorHandle = cursor;
		}
	}

	internal override PaintEventArgs PaintEventStart(ref Message msg, IntPtr handle, bool client)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(msg.HWnd);
		Hwnd hwnd2 = ((!(msg.HWnd == handle)) ? Hwnd.ObjectFromHandle(handle) : hwnd);
		if (Caret.Visible)
		{
			Caret.Paused = true;
			HideCaret();
		}
		Graphics graphics;
		PaintEventArgs paintEventArgs;
		if (client)
		{
			graphics = Graphics.FromHwnd(hwnd2.client_window);
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
			return paintEventArgs;
		}
		graphics = Graphics.FromHwnd(hwnd2.whole_window);
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
		return paintEventArgs;
	}

	internal override void PaintEventEnd(ref Message msg, IntPtr handle, bool client)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(msg.HWnd);
		Graphics graphics = (Graphics)hwnd.drawing_stack.Pop();
		graphics.Flush();
		graphics.Dispose();
		PaintEventArgs paintEventArgs = (PaintEventArgs)hwnd.drawing_stack.Pop();
		paintEventArgs.SetGraphics(null);
		paintEventArgs.Dispose();
		if (Caret.Visible)
		{
			ShowCaret();
			Caret.Paused = false;
		}
	}

	[System.MonoTODO("Implement filtering and PM_NOREMOVE")]
	internal override bool PeekMessage(object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax, uint flags)
	{
		XEventQueue xEventQueue = (XEventQueue)queue_id;
		if ((flags & 1) == 0)
		{
			throw new NotImplementedException("PeekMessage PM_NOREMOVE is not implemented yet");
		}
		bool flag = false;
		if (xEventQueue.Count > 0)
		{
			flag = true;
		}
		else if (XPending(DisplayHandle) != 0)
		{
			UpdateMessageQueue((XEventQueue)queue_id);
			flag = true;
		}
		else if (((XEventQueue)queue_id).Paint.Count > 0)
		{
			flag = true;
		}
		CheckTimers(xEventQueue.timer_list, DateTime.UtcNow);
		if (!flag)
		{
			return false;
		}
		return GetMessage(queue_id, ref msg, hWnd, wFilterMin, wFilterMax);
	}

	internal override bool PostMessage(IntPtr handle, Msg message, IntPtr wparam, IntPtr lparam)
	{
		XEvent xevent = default(XEvent);
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		xevent.type = XEventName.ClientMessage;
		xevent.ClientMessageEvent.display = DisplayHandle;
		if (hwnd != null)
		{
			xevent.ClientMessageEvent.window = hwnd.whole_window;
		}
		else
		{
			xevent.ClientMessageEvent.window = IntPtr.Zero;
		}
		xevent.ClientMessageEvent.message_type = PostAtom;
		xevent.ClientMessageEvent.format = 32;
		xevent.ClientMessageEvent.ptr1 = handle;
		xevent.ClientMessageEvent.ptr2 = (IntPtr)(int)message;
		xevent.ClientMessageEvent.ptr3 = wparam;
		xevent.ClientMessageEvent.ptr4 = lparam;
		if (hwnd != null)
		{
			hwnd.Queue.EnqueueLocked(xevent);
		}
		else
		{
			ThreadQueue(Thread.CurrentThread).EnqueueLocked(xevent);
		}
		return true;
	}

	internal override void PostQuitMessage(int exitCode)
	{
		Form form = Application.MWFThread.Current.Context?.MainForm;
		if (form != null)
		{
			PostMessage(Application.MWFThread.Current.Context.MainForm.window.Handle, Msg.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
		}
		else
		{
			PostMessage(FosterParent, Msg.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
		}
		XFlush(DisplayHandle);
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

	internal override void ResetMouseHover(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null)
		{
			HoverState.Timer.Enabled = true;
			HoverState.X = mouse_position.X;
			HoverState.Y = mouse_position.Y;
			HoverState.Window = handle;
		}
	}

	internal override void ScreenToClient(IntPtr handle, ref int x, ref int y)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		int intdest_x_return;
		int dest_y_return;
		lock (XlibLock)
		{
			XTranslateCoordinates(DisplayHandle, RootWindow, hwnd.client_window, x, y, out intdest_x_return, out dest_y_return, out var _);
		}
		x = intdest_x_return;
		y = dest_y_return;
	}

	internal override void ScreenToMenu(IntPtr handle, ref int x, ref int y)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		int intdest_x_return;
		int dest_y_return;
		lock (XlibLock)
		{
			XTranslateCoordinates(DisplayHandle, RootWindow, hwnd.whole_window, x, y, out intdest_x_return, out dest_y_return, out var _);
		}
		if (Control.FromHandle(handle) is Form form && form.window_manager != null)
		{
			dest_y_return -= form.window_manager.TitleBarHeight;
		}
		x = intdest_x_return;
		y = dest_y_return;
	}

	private bool GraphicsExposePredicate(IntPtr display, ref XEvent xevent, IntPtr arg)
	{
		return (xevent.type == XEventName.GraphicsExpose || xevent.type == XEventName.NoExpose) && arg == xevent.GraphicsExposeEvent.drawable;
	}

	private void ProcessGraphicsExpose(Hwnd hwnd)
	{
		XEvent xevent = default(XEvent);
		IntPtr arg = Hwnd.HandleFromObject(hwnd);
		EventPredicate event_predicate = GraphicsExposePredicate;
		do
		{
			XIfEvent(Display, ref xevent, event_predicate, arg);
			if (xevent.type != XEventName.GraphicsExpose)
			{
				break;
			}
			AddExpose(hwnd, xevent.ExposeEvent.window == hwnd.ClientWindow, xevent.GraphicsExposeEvent.x, xevent.GraphicsExposeEvent.y, xevent.GraphicsExposeEvent.width, xevent.GraphicsExposeEvent.height);
		}
		while (xevent.GraphicsExposeEvent.count != 0);
	}

	internal override void ScrollWindow(IntPtr handle, Rectangle area, int XAmount, int YAmount, bool with_children)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Rectangle rect = Rectangle.Intersect(hwnd.Invalid, area);
		if (!rect.IsEmpty)
		{
			rect.X += XAmount;
			rect.Y += YAmount;
			if (rect.X < 0)
			{
				rect.Width += rect.X;
				rect.X = 0;
			}
			if (rect.Y < 0)
			{
				rect.Height += rect.Y;
				rect.Y = 0;
			}
			if (area.Contains(hwnd.Invalid))
			{
				hwnd.ClearInvalidArea();
			}
			hwnd.AddInvalidArea(rect);
		}
		XGCValues values = default(XGCValues);
		if (with_children)
		{
			values.subwindow_mode = GCSubwindowMode.IncludeInferiors;
		}
		IntPtr gc = XCreateGC(DisplayHandle, hwnd.client_window, IntPtr.Zero, ref values);
		Rectangle totalVisibleArea = GetTotalVisibleArea(hwnd.client_window);
		totalVisibleArea.Intersect(area);
		Rectangle valid_area = totalVisibleArea;
		valid_area.Y += YAmount;
		valid_area.X += XAmount;
		valid_area.Intersect(area);
		Point point = new Point(valid_area.X - XAmount, valid_area.Y - YAmount);
		XCopyArea(DisplayHandle, hwnd.client_window, hwnd.client_window, gc, point.X, point.Y, valid_area.Width, valid_area.Height, valid_area.X, valid_area.Y);
		Rectangle dirtyArea = GetDirtyArea(area, valid_area, XAmount, YAmount);
		AddExpose(hwnd, client: true, dirtyArea.X, dirtyArea.Y, dirtyArea.Width, dirtyArea.Height);
		ProcessGraphicsExpose(hwnd);
		XFreeGC(DisplayHandle, gc);
	}

	internal override void ScrollWindow(IntPtr handle, int XAmount, int YAmount, bool with_children)
	{
		Hwnd objectFromWindow = Hwnd.GetObjectFromWindow(handle);
		Rectangle clientRect = objectFromWindow.ClientRect;
		clientRect.X = 0;
		clientRect.Y = 0;
		ScrollWindow(handle, clientRect, XAmount, YAmount, with_children);
	}

	private Rectangle GetDirtyArea(Rectangle total_area, Rectangle valid_area, int XAmount, int YAmount)
	{
		Rectangle result = total_area;
		if (YAmount > 0)
		{
			result.Height -= valid_area.Height;
		}
		else if (YAmount < 0)
		{
			result.Height -= valid_area.Height;
			result.Y += valid_area.Height;
		}
		if (XAmount > 0)
		{
			result.Width -= valid_area.Width;
		}
		else if (XAmount < 0)
		{
			result.Width -= valid_area.Width;
			result.X += valid_area.Width;
		}
		return result;
	}

	private Rectangle GetTotalVisibleArea(IntPtr handle)
	{
		Control control = Control.FromHandle(handle);
		Rectangle clientRectangle = control.ClientRectangle;
		clientRectangle.Location = control.PointToScreen(Point.Empty);
		for (Control parent = control.Parent; parent != null; parent = parent.Parent)
		{
			if (!parent.IsHandleCreated || !parent.Visible)
			{
				return clientRectangle;
			}
			Rectangle clientRectangle2 = parent.ClientRectangle;
			clientRectangle2.Location = parent.PointToScreen(Point.Empty);
			clientRectangle.Intersect(clientRectangle2);
		}
		clientRectangle.Location = control.PointToClient(clientRectangle.Location);
		return clientRectangle;
	}

	internal override void SendAsyncMethod(AsyncMethodData method)
	{
		XEvent xevent = default(XEvent);
		Hwnd hwnd = Hwnd.ObjectFromHandle(method.Handle);
		xevent.type = XEventName.ClientMessage;
		xevent.ClientMessageEvent.display = DisplayHandle;
		xevent.ClientMessageEvent.window = method.Handle;
		xevent.ClientMessageEvent.message_type = AsyncAtom;
		xevent.ClientMessageEvent.format = 32;
		xevent.ClientMessageEvent.ptr1 = (IntPtr)GCHandle.Alloc(method);
		hwnd.Queue.EnqueueLocked(xevent);
		WakeupMain();
	}

	internal override IntPtr SendMessage(IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam)
	{
		Hwnd hwnd2 = Hwnd.ObjectFromHandle(hwnd);
		if (hwnd2 != null && hwnd2.queue != ThreadQueue(Thread.CurrentThread))
		{
			AsyncMethodResult result = new AsyncMethodResult();
			AsyncMethodData asyncMethodData = new AsyncMethodData();
			asyncMethodData.Handle = hwnd;
			asyncMethodData.Method = new WndProcDelegate(NativeWindow.WndProc);
			asyncMethodData.Args = new object[4] { hwnd, message, wParam, lParam };
			asyncMethodData.Result = result;
			SendAsyncMethod(asyncMethodData);
			return IntPtr.Zero;
		}
		string key = hwnd + ":" + message;
		if (messageHold[key] != null)
		{
			messageHold[key] = (int)messageHold[key] - 1;
		}
		return NativeWindow.WndProc(hwnd, message, wParam, lParam);
	}

	internal override int SendInput(IntPtr handle, Queue keys)
	{
		if (handle == IntPtr.Zero)
		{
			return 0;
		}
		int count = keys.Count;
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		while (keys.Count > 0)
		{
			MSG mSG = (MSG)keys.Dequeue();
			XEvent xevent = default(XEvent);
			xevent.type = ((mSG.message != Msg.WM_KEYUP) ? XEventName.KeyPress : XEventName.KeyRelease);
			xevent.KeyEvent.display = DisplayHandle;
			if (hwnd != null)
			{
				xevent.KeyEvent.window = hwnd.whole_window;
			}
			else
			{
				xevent.KeyEvent.window = IntPtr.Zero;
			}
			xevent.KeyEvent.keycode = Keyboard.ToKeycode((int)mSG.wParam);
			hwnd.Queue.EnqueueLocked(xevent);
		}
		return count;
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
		if (Control.FromHandle(handle) is Form form && form.window_manager == null)
		{
			CreateParams createParams = form.GetCreateParams();
			if (border_style == FormBorderStyle.FixedToolWindow || border_style == FormBorderStyle.SizableToolWindow || createParams.IsSet(WindowExStyles.WS_EX_TOOLWINDOW))
			{
				form.window_manager = new ToolWindowManager(form);
			}
		}
		RequestNCRecalc(handle);
	}

	internal override void SetCaretPos(IntPtr handle, int x, int y)
	{
		if (Caret.Hwnd == handle)
		{
			Caret.Timer.Stop();
			HideCaret();
			Caret.X = x;
			Caret.Y = y;
			Keyboard.SetCaretPos(Caret, handle, x, y);
			if (Caret.Visible)
			{
				ShowCaret();
				Caret.Timer.Start();
			}
		}
	}

	internal override void SetClipRegion(IntPtr handle, Region region)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null)
		{
			hwnd.UserClip = region;
		}
	}

	internal override void SetCursor(IntPtr handle, IntPtr cursor)
	{
		Hwnd hwnd;
		if (OverrideCursorHandle == IntPtr.Zero)
		{
			if (LastCursorWindow == handle && LastCursorHandle == cursor)
			{
				return;
			}
			LastCursorHandle = cursor;
			LastCursorWindow = handle;
			hwnd = Hwnd.ObjectFromHandle(handle);
			lock (XlibLock)
			{
				if (cursor != IntPtr.Zero)
				{
					XDefineCursor(DisplayHandle, hwnd.whole_window, cursor);
				}
				else
				{
					XUndefineCursor(DisplayHandle, hwnd.whole_window);
				}
				XFlush(DisplayHandle);
				return;
			}
		}
		hwnd = Hwnd.ObjectFromHandle(handle);
		lock (XlibLock)
		{
			XDefineCursor(DisplayHandle, hwnd.whole_window, OverrideCursorHandle);
		}
	}

	private void QueryPointer(IntPtr display, IntPtr w, out IntPtr root, out IntPtr child, out int root_x, out int root_y, out int child_x, out int child_y, out int mask)
	{
		XGrabServer(display);
		XQueryPointer(display, w, out root, out var child2, out root_x, out root_y, out child_x, out child_y, out mask);
		if (root != w)
		{
			child2 = root;
		}
		IntPtr intPtr = IntPtr.Zero;
		while (child2 != IntPtr.Zero)
		{
			intPtr = child2;
			XQueryPointer(display, child2, out root, out child2, out root_x, out root_y, out child_x, out child_y, out mask);
		}
		XUngrabServer(display);
		XFlush(display);
		child = intPtr;
	}

	internal override void SetCursorPos(IntPtr handle, int x, int y)
	{
		if (handle == IntPtr.Zero)
		{
			lock (XlibLock)
			{
				QueryPointer(DisplayHandle, RootWindow, out var root, out var child, out var root_x, out var root_y, out var child_x, out var child_y, out var mask);
				XWarpPointer(DisplayHandle, IntPtr.Zero, IntPtr.Zero, 0, 0, 0u, 0u, x - root_x, y - root_y);
				XFlush(DisplayHandle);
				QueryPointer(DisplayHandle, RootWindow, out root, out child, out root_x, out root_y, out child_x, out child_y, out mask);
				Hwnd hwnd = Hwnd.ObjectFromHandle(child);
				if (hwnd != null)
				{
					XEvent xevent = default(XEvent);
					xevent.type = XEventName.MotionNotify;
					xevent.MotionEvent.display = DisplayHandle;
					xevent.MotionEvent.window = hwnd.client_window;
					xevent.MotionEvent.root = RootWindow;
					xevent.MotionEvent.x = child_x;
					xevent.MotionEvent.y = child_y;
					xevent.MotionEvent.x_root = root_x;
					xevent.MotionEvent.y_root = root_y;
					xevent.MotionEvent.state = mask;
					hwnd.Queue.EnqueueLocked(xevent);
				}
				return;
			}
		}
		Hwnd hwnd2 = Hwnd.ObjectFromHandle(handle);
		lock (XlibLock)
		{
			XWarpPointer(DisplayHandle, IntPtr.Zero, hwnd2.client_window, 0, 0, 0u, 0u, x, y);
		}
	}

	internal override void SetFocus(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (!(hwnd.client_window == FocusWindow) && hwnd.enabled)
		{
			IntPtr focusWindow = FocusWindow;
			FocusWindow = hwnd.client_window;
			if (focusWindow != IntPtr.Zero)
			{
				SendMessage(focusWindow, Msg.WM_KILLFOCUS, FocusWindow, IntPtr.Zero);
			}
			SendMessage(FocusWindow, Msg.WM_SETFOCUS, focusWindow, IntPtr.Zero);
			Keyboard.FocusIn(FocusWindow);
		}
	}

	internal override void SetIcon(IntPtr handle, Icon icon)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null)
		{
			SetIcon(hwnd, icon);
		}
	}

	internal override void SetMenu(IntPtr handle, Menu menu)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		hwnd.menu = menu;
		RequestNCRecalc(handle);
	}

	internal override void SetModal(IntPtr handle, bool Modal)
	{
		if (Modal)
		{
			ModalWindows.Push(handle);
		}
		else
		{
			if (ModalWindows.Contains(handle))
			{
				ModalWindows.Pop();
			}
			if (ModalWindows.Count > 0)
			{
				Activate((IntPtr)ModalWindows.Peek());
			}
		}
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Control control = Control.FromHandle(handle);
		SetWMStyles(hwnd, control.GetCreateParams());
	}

	internal override IntPtr SetParent(IntPtr handle, IntPtr parent)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		hwnd.parent = Hwnd.ObjectFromHandle(parent);
		lock (XlibLock)
		{
			XReparentWindow(DisplayHandle, hwnd.whole_window, (hwnd.parent != null) ? hwnd.parent.client_window : FosterParent, hwnd.x, hwnd.y);
		}
		return IntPtr.Zero;
	}

	internal override void SetTimer(Timer timer)
	{
		XEventQueue xEventQueue = (XEventQueue)MessageQueues[timer.thread];
		if (xEventQueue == null)
		{
			unattached_timer_list.Add(timer);
			return;
		}
		xEventQueue.timer_list.Add(timer);
		WakeupMain();
	}

	internal override bool SetTopmost(IntPtr handle, bool enabled)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (enabled)
		{
			lock (XlibLock)
			{
				if (!hwnd.Mapped)
				{
					XChangeProperty(data: new int[8]
					{
						_NET_WM_STATE_ABOVE.ToInt32(),
						0,
						0,
						0,
						0,
						0,
						0,
						0
					}, display: DisplayHandle, window: hwnd.whole_window, property: _NET_WM_STATE, type: (IntPtr)4, format: 32, mode: PropertyMode.Replace, nelements: 1);
				}
				else
				{
					SendNetWMMessage(hwnd.WholeWindow, _NET_WM_STATE, (IntPtr)1, _NET_WM_STATE_ABOVE, IntPtr.Zero);
				}
			}
		}
		else
		{
			lock (XlibLock)
			{
				if (hwnd.Mapped)
				{
					SendNetWMMessage(hwnd.WholeWindow, _NET_WM_STATE, (IntPtr)0, _NET_WM_STATE_ABOVE, IntPtr.Zero);
				}
				else
				{
					XDeleteProperty(DisplayHandle, hwnd.whole_window, _NET_WM_STATE);
				}
			}
		}
		return true;
	}

	internal override bool SetOwner(IntPtr handle, IntPtr handle_owner)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (handle_owner != IntPtr.Zero)
		{
			Hwnd hwnd2 = Hwnd.ObjectFromHandle(handle_owner);
			lock (XlibLock)
			{
				XChangeProperty(data: new int[8]
				{
					_NET_WM_WINDOW_TYPE_NORMAL.ToInt32(),
					0,
					0,
					0,
					0,
					0,
					0,
					0
				}, display: DisplayHandle, window: hwnd.whole_window, property: _NET_WM_WINDOW_TYPE, type: (IntPtr)4, format: 32, mode: PropertyMode.Replace, nelements: 1);
				if (hwnd2 != null)
				{
					XSetTransientForHint(DisplayHandle, hwnd.whole_window, hwnd2.whole_window);
				}
				else
				{
					XSetTransientForHint(DisplayHandle, hwnd.whole_window, RootWindow);
				}
			}
		}
		else
		{
			lock (XlibLock)
			{
				XDeleteProperty(DisplayHandle, hwnd.whole_window, (IntPtr)68);
			}
		}
		return true;
	}

	internal override bool SetVisible(IntPtr handle, bool visible, bool activate)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		hwnd.visible = visible;
		lock (XlibLock)
		{
			if (visible)
			{
				MapWindow(hwnd, WindowType.Both);
				if (Control.FromHandle(handle) is Form)
				{
					switch (((Form)Control.FromHandle(handle)).WindowState)
					{
					case FormWindowState.Minimized:
						SetWindowState(handle, FormWindowState.Minimized);
						break;
					case FormWindowState.Maximized:
						SetWindowState(handle, FormWindowState.Maximized);
						break;
					}
				}
				SendMessage(handle, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
			}
			else
			{
				UnmapWindow(hwnd, WindowType.Both);
			}
		}
		return true;
	}

	internal override void SetWindowMinMax(IntPtr handle, Rectangle maximized, Size min, Size max)
	{
		SetWindowMinMax(handle, maximized, min, max, Control.FromHandle(handle)?.GetCreateParams());
	}

	internal void SetWindowMinMax(IntPtr handle, Rectangle maximized, Size min, Size max, CreateParams cp)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null)
		{
			return;
		}
		min.Width = Math.Max(min.Width, SystemInformation.MinimumWindowSize.Width);
		min.Height = Math.Max(min.Height, SystemInformation.MinimumWindowSize.Height);
		XSizeHints hints = default(XSizeHints);
		XGetWMNormalHints(DisplayHandle, hwnd.whole_window, ref hints, out var _);
		if (min != Size.Empty && min.Width > 0 && min.Height > 0)
		{
			if (cp != null)
			{
				min = TranslateWindowSizeToXWindowSize(cp, min);
			}
			hints.flags = (IntPtr)((int)hints.flags | 0x10);
			hints.min_width = min.Width;
			hints.min_height = min.Height;
		}
		if (max != Size.Empty && max.Width > 0 && max.Height > 0)
		{
			if (cp != null)
			{
				max = TranslateWindowSizeToXWindowSize(cp, max);
			}
			hints.flags = (IntPtr)((int)hints.flags | 0x20);
			hints.max_width = max.Width;
			hints.max_height = max.Height;
		}
		if (hints.flags != IntPtr.Zero)
		{
			XSetWMNormalHints(DisplayHandle, hwnd.whole_window, ref hints);
		}
		if (maximized != Rectangle.Empty && maximized.Width > 0 && maximized.Height > 0)
		{
			if (cp != null)
			{
				maximized.Size = TranslateWindowSizeToXWindowSize(cp);
			}
			hints.flags = (IntPtr)4;
			hints.x = maximized.X;
			hints.y = maximized.Y;
			hints.width = maximized.Width;
			hints.height = maximized.Height;
			XSetZoomHints(DisplayHandle, hwnd.whole_window, ref hints);
		}
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
				MapWindow(hwnd, WindowType.Whole);
			}
			hwnd.zero_sized = false;
		}
		if (width < 1 || height < 1)
		{
			hwnd.zero_sized = true;
			UnmapWindow(hwnd, WindowType.Whole);
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
			if (hwnd.fixed_size)
			{
				SetWindowMinMax(handle, Rectangle.Empty, new Size(width, height), new Size(width, height));
			}
			lock (XlibLock)
			{
				Control control = Control.FromHandle(handle);
				Size size = TranslateWindowSizeToXWindowSize(control.GetCreateParams(), new Size(width, height));
				MoveResizeWindow(DisplayHandle, hwnd.whole_window, x, y, size.Width, size.Height);
				PerformNCCalc(hwnd);
			}
		}
		hwnd.x = x;
		hwnd.y = y;
		hwnd.width = width;
		hwnd.height = height;
		hwnd.ClientRect = Rectangle.Empty;
	}

	internal override void SetWindowState(IntPtr handle, FormWindowState state)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		FormWindowState windowState = GetWindowState(handle);
		if (windowState == state)
		{
			return;
		}
		switch (state)
		{
		case FormWindowState.Normal:
			lock (XlibLock)
			{
				switch (windowState)
				{
				case FormWindowState.Minimized:
					MapWindow(hwnd, WindowType.Both);
					break;
				case FormWindowState.Maximized:
					SendNetWMMessage(hwnd.whole_window, _NET_WM_STATE, (IntPtr)2, _NET_WM_STATE_MAXIMIZED_HORZ, _NET_WM_STATE_MAXIMIZED_VERT);
					break;
				}
			}
			Activate(handle);
			break;
		case FormWindowState.Minimized:
			lock (XlibLock)
			{
				if (windowState == FormWindowState.Maximized)
				{
					SendNetWMMessage(hwnd.whole_window, _NET_WM_STATE, (IntPtr)2, _NET_WM_STATE_MAXIMIZED_HORZ, _NET_WM_STATE_MAXIMIZED_VERT);
				}
				XIconifyWindow(DisplayHandle, hwnd.whole_window, ScreenNo);
				break;
			}
		case FormWindowState.Maximized:
			lock (XlibLock)
			{
				if (windowState == FormWindowState.Minimized)
				{
					MapWindow(hwnd, WindowType.Both);
				}
				SendNetWMMessage(hwnd.whole_window, _NET_WM_STATE, (IntPtr)1, _NET_WM_STATE_MAXIMIZED_HORZ, _NET_WM_STATE_MAXIMIZED_VERT);
			}
			Activate(handle);
			break;
		}
	}

	internal override void SetWindowStyle(IntPtr handle, CreateParams cp)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		SetHwndStyles(hwnd, cp);
		SetWMStyles(hwnd, cp);
	}

	internal override double GetWindowTransparency(IntPtr handle)
	{
		return 1.0;
	}

	internal override void SetWindowTransparency(IntPtr handle, double transparency, Color key)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd != null)
		{
			hwnd.opacity = (uint)(4294967295.0 * transparency);
			IntPtr value = (IntPtr)(int)hwnd.opacity;
			IntPtr window = hwnd.whole_window;
			if (hwnd.reparented)
			{
				window = XGetParent(hwnd.whole_window);
			}
			XChangeProperty(DisplayHandle, window, _NET_WM_WINDOW_OPACITY, (IntPtr)6, 32, PropertyMode.Replace, ref value, 1);
		}
	}

	internal override bool SetZOrder(IntPtr handle, IntPtr after_handle, bool top, bool bottom)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (!hwnd.mapped)
		{
			return false;
		}
		if (top)
		{
			lock (XlibLock)
			{
				XRaiseWindow(DisplayHandle, hwnd.whole_window);
			}
			return true;
		}
		if (!bottom)
		{
			Hwnd hwnd2 = null;
			if (after_handle != IntPtr.Zero)
			{
				hwnd2 = Hwnd.ObjectFromHandle(after_handle);
			}
			XWindowChanges values = default(XWindowChanges);
			if (hwnd2 == null)
			{
				XChangeProperty(data: new int[2]
				{
					unixtime(),
					0
				}, display: DisplayHandle, window: hwnd.whole_window, property: _NET_WM_USER_TIME, type: (IntPtr)6, format: 32, mode: PropertyMode.Replace, nelements: 1);
				XRaiseWindow(DisplayHandle, hwnd.whole_window);
				SendNetWMMessage(hwnd.whole_window, _NET_ACTIVE_WINDOW, (IntPtr)1, IntPtr.Zero, IntPtr.Zero);
				return true;
			}
			values.sibling = hwnd2.whole_window;
			values.stack_mode = StackMode.Below;
			lock (XlibLock)
			{
				XConfigureWindow(DisplayHandle, hwnd.whole_window, ChangeWindowFlags.CWSibling | ChangeWindowFlags.CWStackMode, ref values);
			}
			return false;
		}
		lock (XlibLock)
		{
			XLowerWindow(DisplayHandle, hwnd.whole_window);
		}
		return true;
	}

	internal override void ShowCursor(bool show)
	{
	}

	internal override object StartLoop(Thread thread)
	{
		return ThreadQueue(thread);
	}

	internal override TransparencySupport SupportsTransparency()
	{
		return TransparencySupport.Set;
	}

	internal override bool SystrayAdd(IntPtr handle, string tip, Icon icon, out ToolTip tt)
	{
		GetSystrayManagerWindow();
		if (SystrayMgrWindow != IntPtr.Zero)
		{
			Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
			if (hwnd.client_window != hwnd.whole_window)
			{
				Keyboard.DestroyICForWindow(hwnd.client_window);
				XDestroyWindow(DisplayHandle, hwnd.client_window);
				hwnd.client_window = hwnd.whole_window;
			}
			if (hwnd.nc_expose_pending)
			{
				hwnd.nc_expose_pending = false;
				if (!hwnd.expose_pending)
				{
					hwnd.Queue.Paint.Remove(hwnd);
				}
			}
			XSizeHints hints = default(XSizeHints);
			hints.flags = (IntPtr)304;
			hints.min_width = 24;
			hints.min_height = 24;
			hints.max_width = 24;
			hints.max_height = 24;
			hints.base_width = 24;
			hints.base_height = 24;
			XSetWMNormalHints(DisplayHandle, hwnd.whole_window, ref hints);
			XChangeProperty(data: new int[2] { 1, 1 }, display: DisplayHandle, window: hwnd.whole_window, property: _XEMBED_INFO, type: _XEMBED_INFO, format: 32, mode: PropertyMode.Replace, nelements: 2);
			tt = new ToolTip();
			tt.AutomaticDelay = 350;
			tt.InitialDelay = 250;
			tt.ReshowDelay = 250;
			tt.ShowAlways = true;
			if (tip != null && tip != string.Empty)
			{
				tt.SetToolTip(Control.FromHandle(handle), tip);
				tt.Active = true;
			}
			else
			{
				tt.Active = false;
			}
			SendNetClientMessage(SystrayMgrWindow, _NET_SYSTEM_TRAY_OPCODE, IntPtr.Zero, (IntPtr)0, hwnd.whole_window);
			return true;
		}
		tt = null;
		return false;
	}

	internal override bool SystrayChange(IntPtr handle, string tip, Icon icon, ref ToolTip tt)
	{
		Control control = Control.FromHandle(handle);
		if (control != null && tt != null)
		{
			tt.SetToolTip(control, tip);
			tt.Active = true;
			SendMessage(handle, Msg.WM_PAINT, IntPtr.Zero, IntPtr.Zero);
			return true;
		}
		return false;
	}

	internal override void SystrayRemove(IntPtr handle, ref ToolTip tt)
	{
		SetVisible(handle, visible: false, activate: false);
		if (tt != null)
		{
			tt.Dispose();
			tt = null;
		}
	}

	internal override void SystrayBalloon(IntPtr handle, int timeout, string title, string text, ToolTipIcon icon)
	{
		ThemeEngine.Current.ShowBalloonWindow(handle, timeout, title, text, icon);
		SendMessage(handle, Msg.WM_USER, IntPtr.Zero, (IntPtr)1026);
	}

	internal override bool Text(IntPtr handle, string text)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		lock (XlibLock)
		{
			XChangeProperty(DisplayHandle, hwnd.whole_window, _NET_WM_NAME, UTF8_STRING, 8, PropertyMode.Replace, text, Encoding.UTF8.GetByteCount(text));
			XStoreName(DisplayHandle, Hwnd.ObjectFromHandle(handle).whole_window, text);
		}
		return true;
	}

	internal override bool TranslateMessage(ref MSG msg)
	{
		return Keyboard.TranslateMessage(ref msg);
	}

	internal override void UpdateWindow(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd.visible && hwnd.expose_pending && hwnd.Mapped)
		{
			SendMessage(handle, Msg.WM_PAINT, IntPtr.Zero, IntPtr.Zero);
			hwnd.Queue.Paint.Remove(hwnd);
		}
	}

	internal override void CreateOffscreenDrawable(IntPtr handle, int width, int height, out object offscreen_drawable)
	{
		XGetGeometry(DisplayHandle, handle, out var _, out var _, out var _, out var _, out var _, out var _, out var depth);
		IntPtr intPtr = XCreatePixmap(DisplayHandle, handle, width, height, depth);
		offscreen_drawable = intPtr;
	}

	internal override void DestroyOffscreenDrawable(object offscreen_drawable)
	{
		XFreePixmap(DisplayHandle, (IntPtr)offscreen_drawable);
	}

	internal override Graphics GetOffscreenGraphics(object offscreen_drawable)
	{
		return Graphics.FromHwnd((IntPtr)offscreen_drawable);
	}

	internal override void BlitFromOffscreen(IntPtr dest_handle, Graphics dest_dc, object offscreen_drawable, Graphics offscreen_dc, Rectangle r)
	{
		XGCValues values = default(XGCValues);
		IntPtr gc = XCreateGC(DisplayHandle, dest_handle, IntPtr.Zero, ref values);
		XCopyArea(DisplayHandle, (IntPtr)offscreen_drawable, dest_handle, gc, r.X, r.Y, r.Width, r.Height, r.X, r.Y);
		XFreeGC(DisplayHandle, gc);
	}

	[DllImport("libXcursor")]
	internal static extern IntPtr XcursorLibraryLoadCursor(IntPtr display, [MarshalAs(UnmanagedType.LPStr)] string name);

	[DllImport("libXcursor")]
	internal static extern IntPtr XcursorLibraryLoadImages([MarshalAs(UnmanagedType.LPStr)] string file, IntPtr theme, int size);

	[DllImport("libXcursor")]
	internal static extern void XcursorImagesDestroy(IntPtr images);

	[DllImport("libXcursor")]
	internal static extern int XcursorGetDefaultSize(IntPtr display);

	[DllImport("libXcursor")]
	internal static extern IntPtr XcursorImageLoadCursor(IntPtr display, IntPtr image);

	[DllImport("libXcursor")]
	internal static extern IntPtr XcursorGetTheme(IntPtr display);

	[DllImport("libX11")]
	internal static extern IntPtr XOpenDisplay(IntPtr display);

	[DllImport("libX11")]
	internal static extern int XCloseDisplay(IntPtr display);

	[DllImport("libX11")]
	internal static extern IntPtr XSynchronize(IntPtr display, bool onoff);

	[DllImport("libX11")]
	internal static extern IntPtr XCreateWindow(IntPtr display, IntPtr parent, int x, int y, int width, int height, int border_width, int depth, int xclass, IntPtr visual, UIntPtr valuemask, ref XSetWindowAttributes attributes);

	[DllImport("libX11")]
	internal static extern IntPtr XCreateSimpleWindow(IntPtr display, IntPtr parent, int x, int y, int width, int height, int border_width, UIntPtr border, UIntPtr background);

	[DllImport("libX11")]
	internal static extern int XMapWindow(IntPtr display, IntPtr window);

	[DllImport("libX11")]
	internal static extern int XUnmapWindow(IntPtr display, IntPtr window);

	[DllImport("libX11", EntryPoint = "XMapSubwindows")]
	internal static extern int XMapSubindows(IntPtr display, IntPtr window);

	[DllImport("libX11")]
	internal static extern int XUnmapSubwindows(IntPtr display, IntPtr window);

	[DllImport("libX11")]
	internal static extern IntPtr XRootWindow(IntPtr display, int screen_number);

	[DllImport("libX11")]
	internal static extern IntPtr XNextEvent(IntPtr display, ref XEvent xevent);

	[DllImport("libX11")]
	internal static extern int XConnectionNumber(IntPtr display);

	[DllImport("libX11")]
	internal static extern int XPending(IntPtr display);

	[DllImport("libX11")]
	internal static extern IntPtr XSelectInput(IntPtr display, IntPtr window, IntPtr mask);

	[DllImport("libX11")]
	internal static extern int XDestroyWindow(IntPtr display, IntPtr window);

	[DllImport("libX11")]
	internal static extern int XReparentWindow(IntPtr display, IntPtr window, IntPtr parent, int x, int y);

	[DllImport("libX11")]
	private static extern int XMoveResizeWindow(IntPtr display, IntPtr window, int x, int y, int width, int height);

	internal static int MoveResizeWindow(IntPtr display, IntPtr window, int x, int y, int width, int height)
	{
		int result = XMoveResizeWindow(display, window, x, y, width, height);
		Keyboard.MoveCurrentCaretPos();
		return result;
	}

	[DllImport("libX11")]
	internal static extern int XResizeWindow(IntPtr display, IntPtr window, int width, int height);

	[DllImport("libX11")]
	internal static extern int XGetWindowAttributes(IntPtr display, IntPtr window, ref XWindowAttributes attributes);

	[DllImport("libX11")]
	internal static extern int XFlush(IntPtr display);

	[DllImport("libX11")]
	internal static extern int XSetWMName(IntPtr display, IntPtr window, ref XTextProperty text_prop);

	[DllImport("libX11")]
	internal static extern int XStoreName(IntPtr display, IntPtr window, string window_name);

	[DllImport("libX11")]
	internal static extern int XFetchName(IntPtr display, IntPtr window, ref IntPtr window_name);

	[DllImport("libX11")]
	internal static extern int XSendEvent(IntPtr display, IntPtr window, bool propagate, IntPtr event_mask, ref XEvent send_event);

	[DllImport("libX11")]
	internal static extern int XQueryTree(IntPtr display, IntPtr window, out IntPtr root_return, out IntPtr parent_return, out IntPtr children_return, out int nchildren_return);

	[DllImport("libX11")]
	internal static extern int XFree(IntPtr data);

	[DllImport("libX11")]
	internal static extern int XRaiseWindow(IntPtr display, IntPtr window);

	[DllImport("libX11")]
	internal static extern uint XLowerWindow(IntPtr display, IntPtr window);

	[DllImport("libX11")]
	internal static extern uint XConfigureWindow(IntPtr display, IntPtr window, ChangeWindowFlags value_mask, ref XWindowChanges values);

	[DllImport("libX11")]
	internal static extern IntPtr XInternAtom(IntPtr display, string atom_name, bool only_if_exists);

	[DllImport("libX11")]
	internal static extern int XInternAtoms(IntPtr display, string[] atom_names, int atom_count, bool only_if_exists, IntPtr[] atoms);

	[DllImport("libX11")]
	internal static extern int XSetWMProtocols(IntPtr display, IntPtr window, IntPtr[] protocols, int count);

	[DllImport("libX11")]
	internal static extern int XGrabPointer(IntPtr display, IntPtr window, bool owner_events, EventMask event_mask, GrabMode pointer_mode, GrabMode keyboard_mode, IntPtr confine_to, IntPtr cursor, IntPtr timestamp);

	[DllImport("libX11")]
	internal static extern int XUngrabPointer(IntPtr display, IntPtr timestamp);

	[DllImport("libX11")]
	internal static extern bool XQueryPointer(IntPtr display, IntPtr window, out IntPtr root, out IntPtr child, out int root_x, out int root_y, out int win_x, out int win_y, out int keys_buttons);

	[DllImport("libX11")]
	internal static extern bool XTranslateCoordinates(IntPtr display, IntPtr src_w, IntPtr dest_w, int src_x, int src_y, out int intdest_x_return, out int dest_y_return, out IntPtr child_return);

	[DllImport("libX11")]
	internal static extern bool XGetGeometry(IntPtr display, IntPtr window, out IntPtr root, out int x, out int y, out int width, out int height, out int border_width, out int depth);

	[DllImport("libX11")]
	internal static extern bool XGetGeometry(IntPtr display, IntPtr window, IntPtr root, out int x, out int y, out int width, out int height, IntPtr border_width, IntPtr depth);

	[DllImport("libX11")]
	internal static extern bool XGetGeometry(IntPtr display, IntPtr window, IntPtr root, out int x, out int y, IntPtr width, IntPtr height, IntPtr border_width, IntPtr depth);

	[DllImport("libX11")]
	internal static extern bool XGetGeometry(IntPtr display, IntPtr window, IntPtr root, IntPtr x, IntPtr y, out int width, out int height, IntPtr border_width, IntPtr depth);

	[DllImport("libX11")]
	internal static extern uint XWarpPointer(IntPtr display, IntPtr src_w, IntPtr dest_w, int src_x, int src_y, uint src_width, uint src_height, int dest_x, int dest_y);

	[DllImport("libX11")]
	internal static extern int XClearWindow(IntPtr display, IntPtr window);

	[DllImport("libX11")]
	internal static extern int XClearArea(IntPtr display, IntPtr window, int x, int y, int width, int height, bool exposures);

	[DllImport("libX11")]
	internal static extern IntPtr XDefaultScreenOfDisplay(IntPtr display);

	[DllImport("libX11")]
	internal static extern int XScreenNumberOfScreen(IntPtr display, IntPtr Screen);

	[DllImport("libX11")]
	internal static extern IntPtr XDefaultVisual(IntPtr display, int screen_number);

	[DllImport("libX11")]
	internal static extern uint XDefaultDepth(IntPtr display, int screen_number);

	[DllImport("libX11")]
	internal static extern int XDefaultScreen(IntPtr display);

	[DllImport("libX11")]
	internal static extern IntPtr XDefaultColormap(IntPtr display, int screen_number);

	[DllImport("libX11")]
	internal static extern int XLookupColor(IntPtr display, IntPtr Colormap, string Coloranem, ref XColor exact_def_color, ref XColor screen_def_color);

	[DllImport("libX11")]
	internal static extern int XAllocColor(IntPtr display, IntPtr Colormap, ref XColor colorcell_def);

	[DllImport("libX11")]
	internal static extern int XSetTransientForHint(IntPtr display, IntPtr window, IntPtr prop_window);

	[DllImport("libX11")]
	internal static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, PropertyMode mode, ref MotifWmHints data, int nelements);

	[DllImport("libX11")]
	internal static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, PropertyMode mode, ref uint value, int nelements);

	[DllImport("libX11")]
	internal static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, PropertyMode mode, ref IntPtr value, int nelements);

	[DllImport("libX11")]
	internal static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, PropertyMode mode, uint[] data, int nelements);

	[DllImport("libX11")]
	internal static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, PropertyMode mode, int[] data, int nelements);

	[DllImport("libX11")]
	internal static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, PropertyMode mode, IntPtr[] data, int nelements);

	[DllImport("libX11")]
	internal static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, PropertyMode mode, IntPtr atoms, int nelements);

	[DllImport("libX11", CharSet = CharSet.Ansi)]
	internal static extern int XChangeProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr type, int format, PropertyMode mode, string text, int text_length);

	[DllImport("libX11")]
	internal static extern int XDeleteProperty(IntPtr display, IntPtr window, IntPtr property);

	[DllImport("libX11")]
	internal static extern IntPtr XCreateGC(IntPtr display, IntPtr window, IntPtr valuemask, ref XGCValues values);

	[DllImport("libX11")]
	internal static extern int XFreeGC(IntPtr display, IntPtr gc);

	[DllImport("libX11")]
	internal static extern int XSetFunction(IntPtr display, IntPtr gc, GXFunction function);

	[DllImport("libX11")]
	internal static extern int XSetLineAttributes(IntPtr display, IntPtr gc, int line_width, GCLineStyle line_style, GCCapStyle cap_style, GCJoinStyle join_style);

	[DllImport("libX11")]
	internal static extern int XDrawLine(IntPtr display, IntPtr drawable, IntPtr gc, int x1, int y1, int x2, int y2);

	[DllImport("libX11")]
	internal static extern int XDrawRectangle(IntPtr display, IntPtr drawable, IntPtr gc, int x1, int y1, int width, int height);

	[DllImport("libX11")]
	internal static extern int XFillRectangle(IntPtr display, IntPtr drawable, IntPtr gc, int x1, int y1, int width, int height);

	[DllImport("libX11")]
	internal static extern int XSetWindowBackground(IntPtr display, IntPtr window, IntPtr background);

	[DllImport("libX11")]
	internal static extern int XCopyArea(IntPtr display, IntPtr src, IntPtr dest, IntPtr gc, int src_x, int src_y, int width, int height, int dest_x, int dest_y);

	[DllImport("libX11")]
	internal static extern int XGetWindowProperty(IntPtr display, IntPtr window, IntPtr atom, IntPtr long_offset, IntPtr long_length, bool delete, IntPtr req_type, out IntPtr actual_type, out int actual_format, out IntPtr nitems, out IntPtr bytes_after, ref IntPtr prop);

	[DllImport("libX11")]
	internal static extern int XSetInputFocus(IntPtr display, IntPtr window, RevertTo revert_to, IntPtr time);

	[DllImport("libX11")]
	internal static extern int XIconifyWindow(IntPtr display, IntPtr window, int screen_number);

	[DllImport("libX11")]
	internal static extern int XDefineCursor(IntPtr display, IntPtr window, IntPtr cursor);

	[DllImport("libX11")]
	internal static extern int XUndefineCursor(IntPtr display, IntPtr window);

	[DllImport("libX11")]
	internal static extern int XFreeCursor(IntPtr display, IntPtr cursor);

	[DllImport("libX11")]
	internal static extern IntPtr XCreateFontCursor(IntPtr display, CursorFontShape shape);

	[DllImport("libX11")]
	internal static extern IntPtr XCreatePixmapCursor(IntPtr display, IntPtr source, IntPtr mask, ref XColor foreground_color, ref XColor background_color, int x_hot, int y_hot);

	[DllImport("libX11")]
	internal static extern IntPtr XCreatePixmapFromBitmapData(IntPtr display, IntPtr drawable, byte[] data, int width, int height, IntPtr fg, IntPtr bg, int depth);

	[DllImport("libX11")]
	internal static extern IntPtr XCreatePixmap(IntPtr display, IntPtr d, int width, int height, int depth);

	[DllImport("libX11")]
	internal static extern IntPtr XFreePixmap(IntPtr display, IntPtr pixmap);

	[DllImport("libX11")]
	internal static extern int XQueryBestCursor(IntPtr display, IntPtr drawable, int width, int height, out int best_width, out int best_height);

	[DllImport("libX11")]
	internal static extern int XQueryExtension(IntPtr display, string extension_name, ref int major, ref int first_event, ref int first_error);

	[DllImport("libX11")]
	internal static extern IntPtr XWhitePixel(IntPtr display, int screen_no);

	[DllImport("libX11")]
	internal static extern IntPtr XBlackPixel(IntPtr display, int screen_no);

	[DllImport("libX11")]
	internal static extern void XGrabServer(IntPtr display);

	[DllImport("libX11")]
	internal static extern void XUngrabServer(IntPtr display);

	[DllImport("libX11")]
	internal static extern void XGetWMNormalHints(IntPtr display, IntPtr window, ref XSizeHints hints, out IntPtr supplied_return);

	[DllImport("libX11")]
	internal static extern void XSetWMNormalHints(IntPtr display, IntPtr window, ref XSizeHints hints);

	[DllImport("libX11")]
	internal static extern void XSetZoomHints(IntPtr display, IntPtr window, ref XSizeHints hints);

	[DllImport("libX11")]
	internal static extern void XSetWMHints(IntPtr display, IntPtr window, ref XWMHints wmhints);

	[DllImport("libX11")]
	internal static extern int XGetIconSizes(IntPtr display, IntPtr window, out IntPtr size_list, out int count);

	[DllImport("libX11")]
	internal static extern IntPtr XSetErrorHandler(XErrorHandler error_handler);

	[DllImport("libX11")]
	internal static extern IntPtr XGetErrorText(IntPtr display, byte code, StringBuilder buffer, int length);

	[DllImport("libX11")]
	internal static extern int XInitThreads();

	[DllImport("libX11")]
	internal static extern int XConvertSelection(IntPtr display, IntPtr selection, IntPtr target, IntPtr property, IntPtr requestor, IntPtr time);

	[DllImport("libX11")]
	internal static extern IntPtr XGetSelectionOwner(IntPtr display, IntPtr selection);

	[DllImport("libX11")]
	internal static extern int XSetSelectionOwner(IntPtr display, IntPtr selection, IntPtr owner, IntPtr time);

	[DllImport("libX11")]
	internal static extern int XSetPlaneMask(IntPtr display, IntPtr gc, IntPtr mask);

	[DllImport("libX11")]
	internal static extern int XSetForeground(IntPtr display, IntPtr gc, UIntPtr foreground);

	[DllImport("libX11")]
	internal static extern int XSetBackground(IntPtr display, IntPtr gc, UIntPtr background);

	[DllImport("libX11")]
	internal static extern int XBell(IntPtr display, int percent);

	[DllImport("libX11")]
	internal static extern int XChangeActivePointerGrab(IntPtr display, EventMask event_mask, IntPtr cursor, IntPtr time);

	[DllImport("libX11")]
	internal static extern bool XFilterEvent(ref XEvent xevent, IntPtr window);

	[DllImport("libX11")]
	internal static extern void XkbSetDetectableAutoRepeat(IntPtr display, bool detectable, IntPtr supported);

	[DllImport("libX11")]
	internal static extern void XPeekEvent(IntPtr display, ref XEvent xevent);

	[DllImport("libX11")]
	internal static extern void XIfEvent(IntPtr display, ref XEvent xevent, Delegate event_predicate, IntPtr arg);
}
