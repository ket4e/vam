using System.Runtime.InteropServices;

namespace System.Windows.Forms.CarbonInternal;

internal class MouseHandler : EventHandlerBase, IEventHandler
{
	internal const uint kEventMouseDown = 1u;

	internal const uint kEventMouseUp = 2u;

	internal const uint kEventMouseMoved = 5u;

	internal const uint kEventMouseDragged = 6u;

	internal const uint kEventMouseEntered = 8u;

	internal const uint kEventMouseExited = 9u;

	internal const uint kEventMouseWheelMoved = 10u;

	internal const uint kEventMouseScroll = 11u;

	internal const uint kEventParamMouseLocation = 1835822947u;

	internal const uint kEventParamMouseButton = 1835168878u;

	internal const uint kEventParamMouseWheelAxis = 1836540280u;

	internal const uint kEventParamMouseWheelDelta = 1836541036u;

	internal const uint typeLongInteger = 1819242087u;

	internal const uint typeMouseWheelAxis = 1836540280u;

	internal const uint typeMouseButton = 1835168878u;

	internal const uint typeQDPoint = 1363439732u;

	internal const uint kEventMouseWheelAxisX = 0u;

	internal const uint kEventMouseWheelAxisY = 1u;

	internal const uint DoubleClickInterval = 7500000u;

	internal static ClickStruct ClickPending;

	internal MouseHandler(XplatUICarbon driver)
		: base(driver)
	{
	}

	public bool ProcessEvent(IntPtr callref, IntPtr eventref, IntPtr handle, uint kind, ref MSG msg)
	{
		QDPoint data = default(QDPoint);
		CGPoint point = default(CGPoint);
		Rect bounds = default(Rect);
		IntPtr hit_view = IntPtr.Zero;
		IntPtr handle2 = IntPtr.Zero;
		bool flag = true;
		ushort data2 = 0;
		GetEventParameter(eventref, 1835822947u, 1363439732u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(QDPoint)), IntPtr.Zero, ref data);
		GetEventParameter(eventref, 1835168878u, 1835168878u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(ushort)), IntPtr.Zero, ref data2);
		if (data2 == 1 && (Driver.ModifierKeys & Keys.Control) != 0)
		{
			data2 = 2;
		}
		point.x = data.x;
		point.y = data.y;
		if (FindWindow(data, ref handle2) == 5)
		{
			return true;
		}
		GetWindowBounds(handle, 33u, ref bounds);
		HIViewFindByID(HIViewGetRoot(handle), new HIViewID(2003398244u, 1u), ref handle2);
		point.x -= bounds.left;
		point.y -= bounds.top;
		HIViewGetSubviewHit(handle2, ref point, tval: true, ref hit_view);
		HIViewConvertPoint(ref point, handle2, hit_view);
		Hwnd hwnd = Hwnd.ObjectFromHandle(hit_view);
		if (hwnd != null)
		{
			flag = ((hwnd.ClientWindow == hit_view) ? true : false);
		}
		if (XplatUICarbon.Grab.Hwnd != IntPtr.Zero)
		{
			hwnd = Hwnd.ObjectFromHandle(XplatUICarbon.Grab.Hwnd);
			flag = true;
		}
		if (hwnd == null)
		{
			return true;
		}
		if (flag)
		{
			data.x = (short)point.x;
			data.y = (short)point.y;
			Driver.ScreenToClient(hwnd.Handle, ref data);
		}
		else
		{
			point.x = data.x;
			point.y = data.y;
		}
		msg.hwnd = hwnd.Handle;
		msg.lParam = (IntPtr)(((ushort)point.y << 16) | (ushort)point.x);
		switch (kind)
		{
		case 1u:
			UpdateMouseState(data2, down: true);
			msg.message = (Msg)(((!flag) ? 160 : 512) + (data2 - 1) * 3 + 1);
			msg.wParam = Driver.GetMousewParam(0);
			if (ClickPending.Pending && DateTime.Now.Ticks - ClickPending.Time < 7500000 && msg.hwnd == ClickPending.Hwnd && msg.wParam == ClickPending.wParam && msg.lParam == ClickPending.lParam && msg.message == ClickPending.Message)
			{
				msg.message = (Msg)(((!flag) ? 160 : 512) + (data2 - 1) * 3 + 3);
				ClickPending.Pending = false;
				break;
			}
			ClickPending.Pending = true;
			ClickPending.Hwnd = msg.hwnd;
			ClickPending.Message = msg.message;
			ClickPending.wParam = msg.wParam;
			ClickPending.lParam = msg.lParam;
			ClickPending.Time = DateTime.Now.Ticks;
			break;
		case 2u:
			UpdateMouseState(data2, down: false);
			msg.message = (Msg)(((!flag) ? 160 : 512) + (data2 - 1) * 3 + 2);
			msg.wParam = Driver.GetMousewParam(0);
			break;
		case 5u:
		case 6u:
			if (XplatUICarbon.Grab.Hwnd == IntPtr.Zero)
			{
				IntPtr zero = IntPtr.Zero;
				if (flag)
				{
					zero = (IntPtr)1;
					NativeWindow.WndProc(msg.hwnd, Msg.WM_SETCURSOR, msg.hwnd, (IntPtr)1);
				}
				else
				{
					zero = (IntPtr)NativeWindow.WndProc(hwnd.client_window, Msg.WM_NCHITTEST, IntPtr.Zero, msg.lParam).ToInt32();
					NativeWindow.WndProc(hwnd.client_window, Msg.WM_SETCURSOR, msg.hwnd, zero);
				}
			}
			msg.message = ((!flag) ? Msg.WM_NCMOUSEMOVE : Msg.WM_MOUSEMOVE);
			msg.wParam = Driver.GetMousewParam(0);
			break;
		case 10u:
		case 11u:
		{
			ushort data3 = 0;
			int data4 = 0;
			GetEventParameter(eventref, 1836540280u, 1836540280u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(ushort)), IntPtr.Zero, ref data3);
			GetEventParameter(eventref, 1836541036u, 1819242087u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(int)), IntPtr.Zero, ref data4);
			if (data3 == 1)
			{
				msg.hwnd = XplatUICarbon.FocusWindow;
				msg.message = Msg.WM_MOUSEWHEEL;
				msg.wParam = Driver.GetMousewParam(data4 * 40);
				return true;
			}
			break;
		}
		default:
			return false;
		}
		Driver.mouse_position.X = (int)point.x;
		Driver.mouse_position.Y = (int)point.y;
		return true;
	}

	internal bool TranslateMessage(ref MSG msg)
	{
		if (msg.message == Msg.WM_MOUSEMOVE || msg.message == Msg.WM_NCMOUSEMOVE)
		{
			Hwnd hwnd = Hwnd.ObjectFromHandle(msg.hwnd);
			if (XplatUICarbon.MouseHwnd == null)
			{
				Driver.PostMessage(hwnd.Handle, Msg.WM_MOUSE_ENTER, IntPtr.Zero, IntPtr.Zero);
				Cursor.SetCursor(hwnd.Cursor);
			}
			else if (XplatUICarbon.MouseHwnd.Handle != hwnd.Handle)
			{
				Driver.PostMessage(XplatUICarbon.MouseHwnd.Handle, Msg.WM_MOUSELEAVE, IntPtr.Zero, IntPtr.Zero);
				Driver.PostMessage(hwnd.Handle, Msg.WM_MOUSE_ENTER, IntPtr.Zero, IntPtr.Zero);
				Cursor.SetCursor(hwnd.Cursor);
			}
			XplatUICarbon.MouseHwnd = hwnd;
		}
		return false;
	}

	private void UpdateMouseState(int button, bool down)
	{
		switch (button)
		{
		case 1:
			if (down)
			{
				XplatUICarbon.MouseState |= MouseButtons.Left;
			}
			else
			{
				XplatUICarbon.MouseState &= ~MouseButtons.Left;
			}
			break;
		case 2:
			if (down)
			{
				XplatUICarbon.MouseState |= MouseButtons.Right;
			}
			else
			{
				XplatUICarbon.MouseState &= ~MouseButtons.Right;
			}
			break;
		case 3:
			if (down)
			{
				XplatUICarbon.MouseState |= MouseButtons.Middle;
			}
			else
			{
				XplatUICarbon.MouseState &= ~MouseButtons.Middle;
			}
			break;
		}
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, ref QDPoint data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, ref int data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, ref ushort data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern short FindWindow(QDPoint point, ref IntPtr handle);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int GetWindowBounds(IntPtr handle, uint region, ref Rect bounds);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int HIViewConvertPoint(ref CGPoint point, IntPtr source_view, IntPtr target_view);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern IntPtr HIViewGetRoot(IntPtr handle);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int HIViewGetSubviewHit(IntPtr content_view, ref CGPoint point, bool tval, ref IntPtr hit_view);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int HIViewFindByID(IntPtr root_window, HIViewID id, ref IntPtr view_handle);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	internal static extern int GetCurrentEventButtonState();
}
