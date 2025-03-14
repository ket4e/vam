using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.CarbonInternal;

internal class WindowHandler : EventHandlerBase, IEventHandler
{
	internal const uint kEventWindowUpdate = 1u;

	internal const uint kEventWindowDrawContent = 2u;

	internal const uint kEventWindowActivated = 5u;

	internal const uint kEventWindowDeactivated = 6u;

	internal const uint kEventWindowGetClickActivation = 7u;

	internal const uint kEventWindowShowing = 22u;

	internal const uint kEventWindowHiding = 23u;

	internal const uint kEventWindowShown = 24u;

	internal const uint kEventWindowHidden = 25u;

	internal const uint kEventWindowCollapsing = 86u;

	internal const uint kEventWindowExpanding = 87u;

	internal const uint kEventWindowZoomed = 76u;

	internal const uint kEventWindowBoundsChanging = 26u;

	internal const uint kEventWindowBoundsChanged = 27u;

	internal const uint kEventWindowResizeStarted = 28u;

	internal const uint kEventWindowResizeCompleted = 29u;

	internal const uint kEventWindowDragStarted = 30u;

	internal const uint kEventWindowDragCompleted = 31u;

	internal const uint kEventWindowTransitionStarted = 88u;

	internal const uint kEventWindowTransitionCompleted = 89u;

	internal const uint kEventWindowClickDragRgn = 32u;

	internal const uint kEventWindowClickResizeRgn = 33u;

	internal const uint kEventWindowClickCollapseRgn = 34u;

	internal const uint kEventWindowClickCloseRgn = 35u;

	internal const uint kEventWindowClickZoomRgn = 36u;

	internal const uint kEventWindowClickContentRgn = 37u;

	internal const uint kEventWindowClickProxyIconRgn = 38u;

	internal const uint kEventWindowClickToolbarButtonRgn = 41u;

	internal const uint kEventWindowClickStructureRgn = 42u;

	internal const uint kEventWindowCursorChange = 40u;

	internal const uint kEventWindowCollapse = 66u;

	internal const uint kEventWindowCollapsed = 67u;

	internal const uint kEventWindowCollapseAll = 68u;

	internal const uint kEventWindowExpand = 69u;

	internal const uint kEventWindowExpanded = 70u;

	internal const uint kEventWindowExpandAll = 71u;

	internal const uint kEventWindowClose = 72u;

	internal const uint kEventWindowClosed = 73u;

	internal const uint kEventWindowCloseAll = 74u;

	internal const uint kEventWindowZoom = 75u;

	internal const uint kEventWindowZoomAll = 77u;

	internal const uint kEventWindowContextualMenuSelect = 78u;

	internal const uint kEventWindowPathSelect = 79u;

	internal const uint kEventWindowGetIdealSize = 80u;

	internal const uint kEventWindowGetMinimumSize = 81u;

	internal const uint kEventWindowGetMaximumSize = 82u;

	internal const uint kEventWindowConstrain = 83u;

	internal const uint kEventWindowHandleContentClick = 85u;

	internal const uint kEventWindowGetDockTileMenu = 90u;

	internal const uint kEventWindowHandleActivate = 91u;

	internal const uint kEventWindowHandleDeactivate = 92u;

	internal const uint kEventWindowProxyBeginDrag = 128u;

	internal const uint kEventWindowProxyEndDrag = 129u;

	internal const uint kEventWindowToolbarSwitchMode = 150u;

	internal const uint kEventWindowFocusAcquired = 200u;

	internal const uint kEventWindowFocusRelinquish = 201u;

	internal const uint kEventWindowFocusContent = 202u;

	internal const uint kEventWindowFocusToolbar = 203u;

	internal const uint kEventWindowDrawerOpening = 220u;

	internal const uint kEventWindowDrawerOpened = 221u;

	internal const uint kEventWindowDrawerClosing = 222u;

	internal const uint kEventWindowDrawerClosed = 223u;

	internal const uint kEventWindowDrawFrame = 1000u;

	internal const uint kEventWindowDrawPart = 1001u;

	internal const uint kEventWindowGetRegion = 1002u;

	internal const uint kEventWindowHitTest = 1003u;

	internal const uint kEventWindowInit = 1004u;

	internal const uint kEventWindowDispose = 1005u;

	internal const uint kEventWindowDragHilite = 1006u;

	internal const uint kEventWindowModified = 1007u;

	internal const uint kEventWindowSetupProxyDragImage = 1008u;

	internal const uint kEventWindowStateChanged = 1009u;

	internal const uint kEventWindowMeasureTitle = 1010u;

	internal const uint kEventWindowDrawGrowBox = 1011u;

	internal const uint kEventWindowGetGrowImageRegion = 1012u;

	internal const uint kEventWindowPaint = 1013u;

	internal WindowHandler(XplatUICarbon driver)
		: base(driver)
	{
	}

	public bool ProcessEvent(IntPtr callref, IntPtr eventref, IntPtr handle, uint kind, ref MSG msg)
	{
		IntPtr intPtr = Driver.HandleToWindow(handle);
		Hwnd hwnd = Hwnd.ObjectFromHandle(intPtr);
		if (intPtr != IntPtr.Zero)
		{
			switch (kind)
			{
			case 5u:
			{
				Control control2 = Control.FromHandle(hwnd.client_window);
				if (control2 != null)
				{
					Form form2 = control2.FindForm();
					if (form2 != null && !form2.IsDisposed)
					{
						Driver.SendMessage(form2.Handle, Msg.WM_ACTIVATE, (IntPtr)1, IntPtr.Zero);
						XplatUICarbon.ActiveWindow = hwnd.client_window;
					}
				}
				foreach (IntPtr utilityWindow in XplatUICarbon.UtilityWindows)
				{
					if (utilityWindow != handle && !XplatUICarbon.IsWindowVisible(utilityWindow))
					{
						XplatUICarbon.ShowWindow(utilityWindow);
					}
				}
				break;
			}
			case 87u:
				foreach (IntPtr utilityWindow2 in XplatUICarbon.UtilityWindows)
				{
					if (utilityWindow2 != handle && !XplatUICarbon.IsWindowVisible(utilityWindow2))
					{
						XplatUICarbon.ShowWindow(utilityWindow2);
					}
				}
				msg.hwnd = hwnd.Handle;
				msg.message = Msg.WM_ENTERSIZEMOVE;
				return true;
			case 70u:
				NativeWindow.WndProc(hwnd.Handle, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
				msg.hwnd = hwnd.Handle;
				msg.message = Msg.WM_EXITSIZEMOVE;
				return true;
			case 6u:
			{
				Control control = Control.FromHandle(hwnd.client_window);
				if (control != null)
				{
					Form form = control.FindForm();
					if (form != null)
					{
						Driver.SendMessage(form.Handle, Msg.WM_ACTIVATE, (IntPtr)0, IntPtr.Zero);
						XplatUICarbon.ActiveWindow = IntPtr.Zero;
					}
				}
				foreach (IntPtr utilityWindow3 in XplatUICarbon.UtilityWindows)
				{
					if (utilityWindow3 != handle && XplatUICarbon.IsWindowVisible(utilityWindow3))
					{
						XplatUICarbon.HideWindow(utilityWindow3);
					}
				}
				break;
			}
			case 86u:
				foreach (IntPtr utilityWindow4 in XplatUICarbon.UtilityWindows)
				{
					if (utilityWindow4 != handle && XplatUICarbon.IsWindowVisible(utilityWindow4))
					{
						XplatUICarbon.HideWindow(utilityWindow4);
					}
				}
				msg.hwnd = hwnd.Handle;
				msg.message = Msg.WM_ENTERSIZEMOVE;
				return true;
			case 67u:
				NativeWindow.WndProc(hwnd.Handle, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
				msg.hwnd = hwnd.Handle;
				msg.message = Msg.WM_EXITSIZEMOVE;
				return true;
			case 72u:
				NativeWindow.WndProc(hwnd.Handle, Msg.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
				return false;
			case 24u:
				msg.message = Msg.WM_SHOWWINDOW;
				msg.lParam = (IntPtr)1;
				msg.wParam = (IntPtr)0;
				msg.hwnd = hwnd.Handle;
				return true;
			case 28u:
				msg.message = Msg.WM_ENTERSIZEMOVE;
				msg.hwnd = hwnd.Handle;
				return true;
			case 29u:
				msg.message = Msg.WM_EXITSIZEMOVE;
				msg.hwnd = hwnd.Handle;
				return true;
			case 27u:
			{
				Rect bounds = default(Rect);
				HIRect bounds2 = default(HIRect);
				GetWindowBounds(handle, 33u, ref bounds);
				bounds2.size.width = bounds.right - bounds.left;
				bounds2.size.height = bounds.bottom - bounds.top;
				HIViewSetFrame(hwnd.WholeWindow, ref bounds2);
				Size size = XplatUICarbon.TranslateQuartzWindowSizeToWindowSize(Control.FromHandle(hwnd.Handle).GetCreateParams(), (int)bounds2.size.width, (int)bounds2.size.height);
				hwnd.X = bounds.left;
				hwnd.Y = bounds.top;
				hwnd.Width = size.Width;
				hwnd.Height = size.Height;
				Driver.PerformNCCalc(hwnd);
				msg.hwnd = hwnd.Handle;
				msg.message = Msg.WM_WINDOWPOSCHANGED;
				Driver.SetCaretPos(XplatUICarbon.Caret.Hwnd, XplatUICarbon.Caret.X, XplatUICarbon.Caret.Y);
				return true;
			}
			}
		}
		return false;
	}

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int GetWindowBounds(IntPtr handle, uint region, ref Rect bounds);

	[DllImport("/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon")]
	private static extern int HIViewSetFrame(IntPtr handle, ref HIRect bounds);
}
