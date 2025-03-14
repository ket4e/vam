using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.CarbonInternal;

internal class ControlHandler : EventHandlerBase, IEventHandler
{
	internal const uint kEventControlInitialize = 1000u;

	internal const uint kEventControlDispose = 1001u;

	internal const uint kEventControlGetOptimalBounds = 1003u;

	internal const uint kEventControlDefInitialize = 1000u;

	internal const uint kEventControlDefDispose = 1001u;

	internal const uint kEventControlHit = 1u;

	internal const uint kEventControlSimulateHit = 2u;

	internal const uint kEventControlHitTest = 3u;

	internal const uint kEventControlDraw = 4u;

	internal const uint kEventControlApplyBackground = 5u;

	internal const uint kEventControlApplyTextColor = 6u;

	internal const uint kEventControlSetFocusPart = 7u;

	internal const uint kEventControlGetFocusPart = 8u;

	internal const uint kEventControlActivate = 9u;

	internal const uint kEventControlDeactivate = 10u;

	internal const uint kEventControlSetCursor = 11u;

	internal const uint kEventControlContextualMenuClick = 12u;

	internal const uint kEventControlClick = 13u;

	internal const uint kEventControlGetNextFocusCandidate = 14u;

	internal const uint kEventControlGetAutoToggleValue = 15u;

	internal const uint kEventControlInterceptSubviewClick = 16u;

	internal const uint kEventControlGetClickActivation = 17u;

	internal const uint kEventControlDragEnter = 18u;

	internal const uint kEventControlDragWithin = 19u;

	internal const uint kEventControlDragLeave = 20u;

	internal const uint kEventControlDragReceive = 21u;

	internal const uint kEventControlInvalidateForSizeChange = 22u;

	internal const uint kEventControlTrackingAreaEntered = 23u;

	internal const uint kEventControlTrackingAreaExited = 24u;

	internal const uint kEventControlTrack = 51u;

	internal const uint kEventControlGetScrollToHereStartPoint = 52u;

	internal const uint kEventControlGetIndicatorDragConstraint = 53u;

	internal const uint kEventControlIndicatorMoved = 54u;

	internal const uint kEventControlGhostingFinished = 55u;

	internal const uint kEventControlGetActionProcPart = 56u;

	internal const uint kEventControlGetPartRegion = 101u;

	internal const uint kEventControlGetPartBounds = 102u;

	internal const uint kEventControlSetData = 103u;

	internal const uint kEventControlGetData = 104u;

	internal const uint kEventControlGetSizeConstraints = 105u;

	internal const uint kEventControlGetFrameMetrics = 106u;

	internal const uint kEventControlValueFieldChanged = 151u;

	internal const uint kEventControlAddedSubControl = 152u;

	internal const uint kEventControlRemovingSubControl = 153u;

	internal const uint kEventControlBoundsChanged = 154u;

	internal const uint kEventControlVisibilityChanged = 157u;

	internal const uint kEventControlTitleChanged = 158u;

	internal const uint kEventControlOwningWindowChanged = 159u;

	internal const uint kEventControlHiliteChanged = 160u;

	internal const uint kEventControlEnabledStateChanged = 161u;

	internal const uint kEventControlLayoutInfoChanged = 162u;

	internal const uint kEventControlArbitraryMessage = 201u;

	internal const uint kEventParamCGContextRef = 1668183160u;

	internal const uint kEventParamDirectObject = 757935405u;

	internal const uint kEventParamControlPart = 1668313716u;

	internal const uint kEventParamControlLikesDrag = 1668047975u;

	internal const uint kEventParamRgnHandle = 1919381096u;

	internal const uint typeControlRef = 1668575852u;

	internal const uint typeCGContextRef = 1668183160u;

	internal const uint typeQDPoint = 1363439732u;

	internal const uint typeQDRgnHandle = 1919381096u;

	internal const uint typeControlPartCode = 1668313716u;

	internal const uint typeBoolean = 1651470188u;

	internal ControlHandler(XplatUICarbon driver)
		: base(driver)
	{
	}

	public bool ProcessEvent(IntPtr callref, IntPtr eventref, IntPtr handle, uint kind, ref MSG msg)
	{
		GetEventParameter(eventref, 757935405u, 1668575852u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(IntPtr)), IntPtr.Zero, ref handle);
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		if (hwnd == null)
		{
			return false;
		}
		msg.hwnd = hwnd.Handle;
		bool flag = ((hwnd.ClientWindow == handle) ? true : false);
		switch (kind)
		{
		case 4u:
		{
			IntPtr data2 = IntPtr.Zero;
			HIRect rect2 = default(HIRect);
			GetEventParameter(eventref, 1919381096u, 1919381096u, IntPtr.Zero, (uint)Marshal.SizeOf(typeof(IntPtr)), IntPtr.Zero, ref data2);
			if (data2 != IntPtr.Zero)
			{
				Rect region = default(Rect);
				GetRegionBounds(data2, ref region);
				rect2.origin.x = region.left;
				rect2.origin.y = region.top;
				rect2.size.width = region.right - region.left;
				rect2.size.height = region.bottom - region.top;
			}
			else
			{
				HIViewGetBounds(handle, ref rect2);
			}
			if (!hwnd.visible)
			{
				if (flag)
				{
					hwnd.expose_pending = false;
				}
				else
				{
					hwnd.nc_expose_pending = false;
				}
				return false;
			}
			if (!flag)
			{
				DrawBorders(hwnd);
			}
			Driver.AddExpose(hwnd, flag, rect2);
			return true;
		}
		case 157u:
			if (flag)
			{
				msg.message = Msg.WM_SHOWWINDOW;
				msg.lParam = (IntPtr)0;
				msg.wParam = ((!HIViewIsVisible(handle)) ? ((IntPtr)0) : ((IntPtr)1));
				return true;
			}
			return false;
		case 154u:
		{
			HIRect rect = default(HIRect);
			HIViewGetFrame(handle, ref rect);
			if (!flag)
			{
				hwnd.X = (int)rect.origin.x;
				hwnd.Y = (int)rect.origin.y;
				hwnd.Width = (int)rect.size.width;
				hwnd.Height = (int)rect.size.height;
				Driver.PerformNCCalc(hwnd);
			}
			msg.message = Msg.WM_WINDOWPOSCHANGED;
			msg.hwnd = hwnd.Handle;
			return true;
		}
		case 8u:
		{
			short data = 0;
			SetEventParameter(eventref, 1668313716u, 1668313716u, (uint)Marshal.SizeOf(typeof(short)), ref data);
			return false;
		}
		case 18u:
		case 19u:
		case 20u:
		case 21u:
			return Dnd.HandleEvent(callref, eventref, handle, kind, ref msg);
		default:
			return false;
		}
	}

	private void DrawBorders(Hwnd hwnd)
	{
		switch (hwnd.border_style)
		{
		case FormBorderStyle.Fixed3D:
		{
			Graphics graphics2 = Graphics.FromHwnd(hwnd.whole_window);
			if (hwnd.border_static)
			{
				ControlPaint.DrawBorder3D(graphics2, new Rectangle(0, 0, hwnd.Width, hwnd.Height), Border3DStyle.SunkenOuter);
			}
			else
			{
				ControlPaint.DrawBorder3D(graphics2, new Rectangle(0, 0, hwnd.Width, hwnd.Height), Border3DStyle.Sunken);
			}
			graphics2.Dispose();
			break;
		}
		case FormBorderStyle.FixedSingle:
		{
			Graphics graphics = Graphics.FromHwnd(hwnd.whole_window);
			ControlPaint.DrawBorder(graphics, new Rectangle(0, 0, hwnd.Width, hwnd.Height), Color.Black, ButtonBorderStyle.Solid);
			graphics.Dispose();
			break;
		}
		}
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetRegionBounds(IntPtr rgnhandle, ref Rect region);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetEventParameter(IntPtr eventref, uint name, uint type, IntPtr outtype, uint size, IntPtr outsize, ref IntPtr data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int SetEventParameter(IntPtr eventref, uint name, uint type, uint size, ref short data);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewGetBounds(IntPtr handle, ref HIRect rect);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewGetFrame(IntPtr handle, ref HIRect rect);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern bool HIViewIsVisible(IntPtr vHnd);
}
