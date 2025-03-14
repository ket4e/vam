using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Windows.Forms;

internal class XplatUI
{
	public class State
	{
		public static Keys ModifierKeys => driver.ModifierKeys;

		public static MouseButtons MouseButtons => driver.MouseButtons;

		public static Point MousePosition => driver.MousePosition;
	}

	public delegate bool ClipboardToObject(int type, IntPtr data, out object obj);

	public delegate bool ObjectToClipboard(ref int type, object obj, out byte[] data);

	private static XplatUIDriver driver;

	private static string default_class_name;

	internal static ArrayList key_filters;

	public static bool RunningOnUnix
	{
		get
		{
			int platform = (int)Environment.OSVersion.Platform;
			return platform == 4 || platform == 6 || platform == 128;
		}
	}

	public static int ActiveWindowTrackingDelay => driver.ActiveWindowTrackingDelay;

	internal static string DefaultClassName
	{
		get
		{
			return default_class_name;
		}
		set
		{
			default_class_name = value;
		}
	}

	public static Size Border3DSize => driver.Border3DSize;

	public static Size BorderSize => driver.BorderSize;

	public static Size CaptionButtonSize => driver.CaptionButtonSize;

	public static int CaptionHeight => driver.CaptionHeight;

	public static int CaretBlinkTime => driver.CaretBlinkTime;

	public static int CaretWidth => driver.CaretWidth;

	public static Size CursorSize => driver.CursorSize;

	public static Size DoubleClickSize => driver.DoubleClickSize;

	public static int DoubleClickTime => driver.DoubleClickTime;

	public static bool DragFullWindows => driver.DragFullWindows;

	public static Size DragSize => driver.DragSize;

	public static Size FixedFrameBorderSize => driver.FixedFrameBorderSize;

	public static int FontSmoothingContrast => driver.FontSmoothingContrast;

	public static int FontSmoothingType => driver.FontSmoothingType;

	public static Size FrameBorderSize => driver.FrameBorderSize;

	public static int HorizontalResizeBorderThickness => driver.HorizontalResizeBorderThickness;

	public static int HorizontalScrollBarHeight => driver.HorizontalScrollBarHeight;

	public static Size IconSize => driver.IconSize;

	public static bool IsActiveWindowTrackingEnabled => driver.IsActiveWindowTrackingEnabled;

	public static bool IsComboBoxAnimationEnabled => driver.IsComboBoxAnimationEnabled;

	public static bool IsDropShadowEnabled => driver.IsDropShadowEnabled;

	public static bool IsFontSmoothingEnabled => driver.IsFontSmoothingEnabled;

	public static bool IsHotTrackingEnabled => driver.IsHotTrackingEnabled;

	public static bool IsIconTitleWrappingEnabled => driver.IsIconTitleWrappingEnabled;

	public static bool IsKeyboardPreferred => driver.IsKeyboardPreferred;

	public static bool IsListBoxSmoothScrollingEnabled => driver.IsListBoxSmoothScrollingEnabled;

	public static bool IsMenuAnimationEnabled => driver.IsMenuAnimationEnabled;

	public static bool IsMenuFadeEnabled => driver.IsMenuFadeEnabled;

	public static bool IsMinimizeRestoreAnimationEnabled => driver.IsMinimizeRestoreAnimationEnabled;

	public static bool IsSelectionFadeEnabled => driver.IsSelectionFadeEnabled;

	public static bool IsSnapToDefaultEnabled => driver.IsSnapToDefaultEnabled;

	public static bool IsTitleBarGradientEnabled => driver.IsTitleBarGradientEnabled;

	public static bool IsToolTipAnimationEnabled => driver.IsToolTipAnimationEnabled;

	public static int KeyboardSpeed => driver.KeyboardSpeed;

	public static int KeyboardDelay => driver.KeyboardDelay;

	public static Size MaxWindowTrackSize => driver.MaxWindowTrackSize;

	public static bool MenuAccessKeysUnderlined => driver.MenuAccessKeysUnderlined;

	public static Size MenuBarButtonSize => driver.MenuBarButtonSize;

	public static Size MenuButtonSize => driver.MenuButtonSize;

	public static int MenuShowDelay => driver.MenuShowDelay;

	public static Size MinimizedWindowSize => driver.MinimizedWindowSize;

	public static Size MinimizedWindowSpacingSize => driver.MinimizedWindowSpacingSize;

	public static Size MinimumWindowSize => driver.MinimumWindowSize;

	public static Size MinimumFixedToolWindowSize => driver.MinimumFixedToolWindowSize;

	public static Size MinimumSizeableToolWindowSize => driver.MinimumSizeableToolWindowSize;

	public static Size MinimumNoBorderWindowSize => driver.MinimumNoBorderWindowSize;

	public static Size MinWindowTrackSize => driver.MinWindowTrackSize;

	public static int MouseSpeed => driver.MouseSpeed;

	public static Size SmallIconSize => driver.SmallIconSize;

	public static int MenuHeight => driver.MenuHeight;

	public static int MouseButtonCount => driver.MouseButtonCount;

	public static bool MouseButtonsSwapped => driver.MouseButtonsSwapped;

	public static Size MouseHoverSize => driver.MouseHoverSize;

	public static int MouseHoverTime => driver.MouseHoverTime;

	public static int MouseWheelScrollDelta => driver.MouseWheelScrollDelta;

	public static bool MouseWheelPresent => driver.MouseWheelPresent;

	public static LeftRightAlignment PopupMenuAlignment => driver.PopupMenuAlignment;

	public static PowerStatus PowerStatus => driver.PowerStatus;

	public static bool RequiresPositiveClientAreaSize => driver.RequiresPositiveClientAreaSize;

	public static int SizingBorderWidth => driver.SizingBorderWidth;

	public static Size SmallCaptionButtonSize => driver.SmallCaptionButtonSize;

	public static bool UIEffectsEnabled => driver.UIEffectsEnabled;

	public static bool UserClipWontExposeParent => driver.UserClipWontExposeParent;

	public static int VerticalResizeBorderThickness => driver.VerticalResizeBorderThickness;

	public static int VerticalScrollBarWidth => driver.VerticalScrollBarWidth;

	public static Rectangle VirtualScreen => driver.VirtualScreen;

	public static Rectangle WorkingArea => driver.WorkingArea;

	public static bool ThemesEnabled => driver.ThemesEnabled;

	public static int ToolWindowCaptionHeight => driver.ToolWindowCaptionHeight;

	public static Size ToolWindowCaptionButtonSize => driver.ToolWindowCaptionButtonSize;

	internal static event EventHandler Idle
	{
		add
		{
			driver.Idle += value;
		}
		remove
		{
			driver.Idle -= value;
		}
	}

	static XplatUI()
	{
		key_filters = new ArrayList();
		default_class_name = "SWFClass" + Thread.GetDomainID();
		if (RunningOnUnix)
		{
			if (Environment.GetEnvironmentVariable("MONO_MWF_MAC_FORCE_X11") != null)
			{
				driver = XplatUIX11.GetInstance();
			}
			else
			{
				IntPtr intPtr = Marshal.AllocHGlobal(8192);
				if (uname(intPtr) != 0)
				{
					driver = XplatUIX11.GetInstance();
				}
				else
				{
					string text = Marshal.PtrToStringAnsi(intPtr);
					if (text == "Darwin")
					{
						driver = XplatUICarbon.GetInstance();
					}
					else
					{
						driver = XplatUIX11.GetInstance();
					}
				}
				Marshal.FreeHGlobal(intPtr);
			}
		}
		else
		{
			driver = XplatUIWin32.GetInstance();
		}
		driver.InitializeDriver();
		DataFormats.GetFormat(0);
		Application.FirePreRun();
	}

	internal static string Window(IntPtr handle)
	{
		return $"'{Control.FromHandle(handle)}' ({handle.ToInt32():X})";
	}

	internal static void Activate(IntPtr handle)
	{
		driver.Activate(handle);
	}

	internal static void AudibleAlert(AlertType alert)
	{
		driver.AudibleAlert(alert);
	}

	internal static bool CalculateWindowRect(ref Rectangle ClientRect, CreateParams cp, Menu menu, out Rectangle WindowRect)
	{
		return driver.CalculateWindowRect(ref ClientRect, cp, menu, out WindowRect);
	}

	internal static void CaretVisible(IntPtr handle, bool visible)
	{
		driver.CaretVisible(handle, visible);
	}

	internal static void CreateCaret(IntPtr handle, int width, int height)
	{
		driver.CreateCaret(handle, width, height);
	}

	internal static IntPtr CreateWindow(CreateParams cp)
	{
		return driver.CreateWindow(cp);
	}

	internal static IntPtr CreateWindow(IntPtr Parent, int X, int Y, int Width, int Height)
	{
		return driver.CreateWindow(Parent, X, Y, Width, Height);
	}

	internal static void ClientToScreen(IntPtr handle, ref int x, ref int y)
	{
		driver.ClientToScreen(handle, ref x, ref y);
	}

	internal static int[] ClipboardAvailableFormats(IntPtr handle)
	{
		return driver.ClipboardAvailableFormats(handle);
	}

	internal static void ClipboardClose(IntPtr handle)
	{
		driver.ClipboardClose(handle);
	}

	internal static int ClipboardGetID(IntPtr handle, string format)
	{
		return driver.ClipboardGetID(handle, format);
	}

	internal static IntPtr ClipboardOpen(bool primary_selection)
	{
		return driver.ClipboardOpen(primary_selection);
	}

	internal static void ClipboardStore(IntPtr handle, object obj, int type, ObjectToClipboard converter)
	{
		driver.ClipboardStore(handle, obj, type, converter);
	}

	internal static object ClipboardRetrieve(IntPtr handle, int type, ClipboardToObject converter)
	{
		return driver.ClipboardRetrieve(handle, type, converter);
	}

	internal static IntPtr DefineCursor(Bitmap bitmap, Bitmap mask, Color cursor_pixel, Color mask_pixel, int xHotSpot, int yHotSpot)
	{
		return driver.DefineCursor(bitmap, mask, cursor_pixel, mask_pixel, xHotSpot, yHotSpot);
	}

	internal static IntPtr DefineStdCursor(StdCursor id)
	{
		return driver.DefineStdCursor(id);
	}

	internal static Bitmap DefineStdCursorBitmap(StdCursor id)
	{
		return driver.DefineStdCursorBitmap(id);
	}

	internal static IntPtr DefWndProc(ref Message msg)
	{
		return driver.DefWndProc(ref msg);
	}

	internal static void DestroyCaret(IntPtr handle)
	{
		driver.DestroyCaret(handle);
	}

	internal static void DestroyCursor(IntPtr cursor)
	{
		driver.DestroyCursor(cursor);
	}

	internal static void DestroyWindow(IntPtr handle)
	{
		driver.DestroyWindow(handle);
	}

	internal static IntPtr DispatchMessage(ref MSG msg)
	{
		return driver.DispatchMessage(ref msg);
	}

	internal static void DoEvents()
	{
		driver.DoEvents();
	}

	internal static void DrawReversibleRectangle(IntPtr handle, Rectangle rect, int line_width)
	{
		driver.DrawReversibleRectangle(handle, rect, line_width);
	}

	internal static void FillReversibleRectangle(Rectangle rectangle, Color backColor)
	{
		driver.FillReversibleRectangle(rectangle, backColor);
	}

	internal static void DrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style)
	{
		driver.DrawReversibleFrame(rectangle, backColor, style);
	}

	internal static void DrawReversibleLine(Point start, Point end, Color backColor)
	{
		driver.DrawReversibleLine(start, end, backColor);
	}

	internal static void EnableThemes()
	{
		driver.EnableThemes();
	}

	internal static void EnableWindow(IntPtr handle, bool Enable)
	{
		driver.EnableWindow(handle, Enable);
	}

	internal static void EndLoop(Thread thread)
	{
		driver.EndLoop(thread);
	}

	internal static IntPtr GetActive()
	{
		return driver.GetActive();
	}

	internal static SizeF GetAutoScaleSize(Font font)
	{
		return driver.GetAutoScaleSize(font);
	}

	internal static Region GetClipRegion(IntPtr handle)
	{
		return driver.GetClipRegion(handle);
	}

	internal static void GetCursorInfo(IntPtr cursor, out int width, out int height, out int hotspot_x, out int hotspot_y)
	{
		driver.GetCursorInfo(cursor, out width, out height, out hotspot_x, out hotspot_y);
	}

	internal static void GetCursorPos(IntPtr handle, out int x, out int y)
	{
		driver.GetCursorPos(handle, out x, out y);
	}

	internal static void GetDisplaySize(out Size size)
	{
		driver.GetDisplaySize(out size);
	}

	internal static IntPtr GetFocus()
	{
		return driver.GetFocus();
	}

	internal static bool GetFontMetrics(Graphics g, Font font, out int ascent, out int descent)
	{
		return driver.GetFontMetrics(g, font, out ascent, out descent);
	}

	internal static Point GetMenuOrigin(IntPtr handle)
	{
		return driver.GetMenuOrigin(handle);
	}

	internal static bool GetMessage(object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax)
	{
		return driver.GetMessage(queue_id, ref msg, hWnd, wFilterMin, wFilterMax);
	}

	internal static IntPtr GetParent(IntPtr handle)
	{
		return driver.GetParent(handle);
	}

	internal static IntPtr GetPreviousWindow(IntPtr handle)
	{
		return driver.GetPreviousWindow(handle);
	}

	internal static bool GetText(IntPtr handle, out string text)
	{
		return driver.GetText(handle, out text);
	}

	internal static void GetWindowPos(IntPtr handle, bool is_toplevel, out int x, out int y, out int width, out int height, out int client_width, out int client_height)
	{
		driver.GetWindowPos(handle, is_toplevel, out x, out y, out width, out height, out client_width, out client_height);
	}

	internal static FormWindowState GetWindowState(IntPtr handle)
	{
		return driver.GetWindowState(handle);
	}

	internal static void GrabInfo(out IntPtr handle, out bool GrabConfined, out Rectangle GrabArea)
	{
		driver.GrabInfo(out handle, out GrabConfined, out GrabArea);
	}

	internal static void GrabWindow(IntPtr handle, IntPtr ConfineToHwnd)
	{
		driver.GrabWindow(handle, ConfineToHwnd);
	}

	internal static void HandleException(Exception e)
	{
		driver.HandleException(e);
	}

	internal static void Invalidate(IntPtr handle, Rectangle rc, bool clear)
	{
		driver.Invalidate(handle, rc, clear);
	}

	internal static void InvalidateNC(IntPtr handle)
	{
		driver.InvalidateNC(handle);
	}

	internal static bool IsEnabled(IntPtr handle)
	{
		return driver.IsEnabled(handle);
	}

	internal static bool IsKeyLocked(VirtualKeys key)
	{
		return driver.IsKeyLocked(key);
	}

	internal static bool IsVisible(IntPtr handle)
	{
		return driver.IsVisible(handle);
	}

	internal static void KillTimer(Timer timer)
	{
		driver.KillTimer(timer);
	}

	internal static void MenuToScreen(IntPtr handle, ref int x, ref int y)
	{
		driver.MenuToScreen(handle, ref x, ref y);
	}

	internal static void OverrideCursor(IntPtr cursor)
	{
		driver.OverrideCursor(cursor);
	}

	internal static void PaintEventEnd(ref Message msg, IntPtr handle, bool client)
	{
		driver.PaintEventEnd(ref msg, handle, client);
	}

	internal static PaintEventArgs PaintEventStart(ref Message msg, IntPtr handle, bool client)
	{
		return driver.PaintEventStart(ref msg, handle, client);
	}

	internal static bool PeekMessage(object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax, uint flags)
	{
		return driver.PeekMessage(queue_id, ref msg, hWnd, wFilterMin, wFilterMax, flags);
	}

	internal static bool PostMessage(IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam)
	{
		return driver.PostMessage(hwnd, message, wParam, lParam);
	}

	internal static bool PostMessage(ref MSG msg)
	{
		return driver.PostMessage(msg.hwnd, msg.message, msg.wParam, msg.lParam);
	}

	internal static void PostQuitMessage(int exitCode)
	{
		driver.PostQuitMessage(exitCode);
	}

	internal static void RaiseIdle(EventArgs e)
	{
		driver.RaiseIdle(e);
	}

	internal static void RequestAdditionalWM_NCMessages(IntPtr handle, bool hover, bool leave)
	{
		driver.RequestAdditionalWM_NCMessages(handle, hover, leave);
	}

	internal static void RequestNCRecalc(IntPtr handle)
	{
		driver.RequestNCRecalc(handle);
	}

	internal static void ResetMouseHover(IntPtr handle)
	{
		driver.ResetMouseHover(handle);
	}

	internal static void ScreenToClient(IntPtr handle, ref int x, ref int y)
	{
		driver.ScreenToClient(handle, ref x, ref y);
	}

	internal static void ScreenToMenu(IntPtr handle, ref int x, ref int y)
	{
		driver.ScreenToMenu(handle, ref x, ref y);
	}

	internal static void ScrollWindow(IntPtr handle, Rectangle rectangle, int XAmount, int YAmount, bool with_children)
	{
		driver.ScrollWindow(handle, rectangle, XAmount, YAmount, with_children);
	}

	internal static void ScrollWindow(IntPtr handle, int XAmount, int YAmount, bool with_children)
	{
		driver.ScrollWindow(handle, XAmount, YAmount, with_children);
	}

	internal static void SendAsyncMethod(AsyncMethodData data)
	{
		driver.SendAsyncMethod(data);
	}

	internal static int SendInput(IntPtr hwnd, Queue keys)
	{
		return driver.SendInput(hwnd, keys);
	}

	internal static IntPtr SendMessage(IntPtr handle, Msg message, IntPtr wParam, IntPtr lParam)
	{
		return driver.SendMessage(handle, message, wParam, lParam);
	}

	internal static void SendMessage(ref Message m)
	{
		m.Result = driver.SendMessage(m.HWnd, (Msg)m.Msg, m.WParam, m.LParam);
	}

	internal static void SetAllowDrop(IntPtr handle, bool value)
	{
		driver.SetAllowDrop(handle, value);
	}

	internal static void SetBorderStyle(IntPtr handle, FormBorderStyle border_style)
	{
		driver.SetBorderStyle(handle, border_style);
	}

	internal static void SetCaretPos(IntPtr handle, int x, int y)
	{
		driver.SetCaretPos(handle, x, y);
	}

	internal static void SetClipRegion(IntPtr handle, Region region)
	{
		driver.SetClipRegion(handle, region);
	}

	internal static void SetCursor(IntPtr handle, IntPtr cursor)
	{
		driver.SetCursor(handle, cursor);
	}

	internal static void SetCursorPos(IntPtr handle, int x, int y)
	{
		driver.SetCursorPos(handle, x, y);
	}

	internal static void SetFocus(IntPtr handle)
	{
		driver.SetFocus(handle);
	}

	internal static void SetForegroundWindow(IntPtr handle)
	{
		driver.SetForegroundWindow(handle);
	}

	internal static void SetIcon(IntPtr handle, Icon icon)
	{
		driver.SetIcon(handle, icon);
	}

	internal static void SetMenu(IntPtr handle, Menu menu)
	{
		driver.SetMenu(handle, menu);
	}

	internal static void SetModal(IntPtr handle, bool Modal)
	{
		driver.SetModal(handle, Modal);
	}

	internal static IntPtr SetParent(IntPtr handle, IntPtr hParent)
	{
		return driver.SetParent(handle, hParent);
	}

	internal static void SetTimer(Timer timer)
	{
		driver.SetTimer(timer);
	}

	internal static bool SetTopmost(IntPtr handle, bool Enabled)
	{
		return driver.SetTopmost(handle, Enabled);
	}

	internal static bool SetOwner(IntPtr handle, IntPtr hWndOwner)
	{
		return driver.SetOwner(handle, hWndOwner);
	}

	internal static bool SetVisible(IntPtr handle, bool visible, bool activate)
	{
		return driver.SetVisible(handle, visible, activate);
	}

	internal static void SetWindowMinMax(IntPtr handle, Rectangle maximized, Size min, Size max)
	{
		driver.SetWindowMinMax(handle, maximized, min, max);
	}

	internal static void SetWindowPos(IntPtr handle, int x, int y, int width, int height)
	{
		driver.SetWindowPos(handle, x, y, width, height);
	}

	internal static void SetWindowState(IntPtr handle, FormWindowState state)
	{
		driver.SetWindowState(handle, state);
	}

	internal static void SetWindowStyle(IntPtr handle, CreateParams cp)
	{
		driver.SetWindowStyle(handle, cp);
	}

	internal static double GetWindowTransparency(IntPtr handle)
	{
		return driver.GetWindowTransparency(handle);
	}

	internal static void SetWindowTransparency(IntPtr handle, double transparency, Color key)
	{
		driver.SetWindowTransparency(handle, transparency, key);
	}

	internal static bool SetZOrder(IntPtr handle, IntPtr AfterhWnd, bool Top, bool Bottom)
	{
		return driver.SetZOrder(handle, AfterhWnd, Top, Bottom);
	}

	internal static void ShowCursor(bool show)
	{
		driver.ShowCursor(show);
	}

	internal static DragDropEffects StartDrag(IntPtr handle, object data, DragDropEffects allowedEffects)
	{
		return driver.StartDrag(handle, data, allowedEffects);
	}

	internal static object StartLoop(Thread thread)
	{
		return driver.StartLoop(thread);
	}

	internal static TransparencySupport SupportsTransparency()
	{
		return driver.SupportsTransparency();
	}

	internal static bool SystrayAdd(IntPtr handle, string tip, Icon icon, out ToolTip tt)
	{
		return driver.SystrayAdd(handle, tip, icon, out tt);
	}

	internal static void SystrayChange(IntPtr handle, string tip, Icon icon, ref ToolTip tt)
	{
		driver.SystrayChange(handle, tip, icon, ref tt);
	}

	internal static void SystrayRemove(IntPtr handle, ref ToolTip tt)
	{
		driver.SystrayRemove(handle, ref tt);
	}

	internal static void SystrayBalloon(IntPtr handle, int timeout, string title, string text, ToolTipIcon icon)
	{
		driver.SystrayBalloon(handle, timeout, title, text, icon);
	}

	internal static bool Text(IntPtr handle, string text)
	{
		return driver.Text(handle, text);
	}

	internal static bool TranslateMessage(ref MSG msg)
	{
		return driver.TranslateMessage(ref msg);
	}

	internal static void UngrabWindow(IntPtr handle)
	{
		driver.UngrabWindow(handle);
	}

	internal static void UpdateWindow(IntPtr handle)
	{
		driver.UpdateWindow(handle);
	}

	internal static void CreateOffscreenDrawable(IntPtr handle, int width, int height, out object offscreen_drawable)
	{
		driver.CreateOffscreenDrawable(handle, width, height, out offscreen_drawable);
	}

	internal static void DestroyOffscreenDrawable(object offscreen_drawable)
	{
		driver.DestroyOffscreenDrawable(offscreen_drawable);
	}

	internal static Graphics GetOffscreenGraphics(object offscreen_drawable)
	{
		return driver.GetOffscreenGraphics(offscreen_drawable);
	}

	internal static void BlitFromOffscreen(IntPtr dest_handle, Graphics dest_dc, object offscreen_drawable, Graphics offscreen_dc, Rectangle r)
	{
		driver.BlitFromOffscreen(dest_handle, dest_dc, offscreen_drawable, offscreen_dc, r);
	}

	internal static void Version()
	{
		Console.WriteLine("Xplat version $Revision: $");
	}

	internal static void AddKeyFilter(IKeyFilter value)
	{
		lock (key_filters)
		{
			key_filters.Add(value);
		}
	}

	internal static bool FilterKey(KeyFilterData key)
	{
		lock (key_filters)
		{
			for (int i = 0; i < key_filters.Count; i++)
			{
				IKeyFilter keyFilter = (IKeyFilter)key_filters[i];
				if (keyFilter.PreFilterKey(key))
				{
					return true;
				}
			}
		}
		return false;
	}

	[DllImport("libc")]
	private static extern int uname(IntPtr buf);
}
