using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Windows.Forms;

internal class XplatUIWin32 : XplatUIDriver
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	private struct WNDCLASS
	{
		internal int style;

		internal WndProc lpfnWndProc;

		internal int cbClsExtra;

		internal int cbWndExtra;

		internal IntPtr hInstance;

		internal IntPtr hIcon;

		internal IntPtr hCursor;

		internal IntPtr hbrBackground;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string lpszMenuName;

		[MarshalAs(UnmanagedType.LPWStr)]
		internal string lpszClassName;
	}

	internal struct RECT
	{
		internal int left;

		internal int top;

		internal int right;

		internal int bottom;

		public int Height => bottom - top;

		public int Width => right - left;

		public Size Size => new Size(Width, Height);

		public Point Location => new Point(left, top);

		public RECT(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public Rectangle ToRectangle()
		{
			return Rectangle.FromLTRB(left, top, right, bottom);
		}

		public static RECT FromRectangle(Rectangle rectangle)
		{
			return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
		}

		public override int GetHashCode()
		{
			return left ^ ((top << 13) | (top >> 19)) ^ ((Width << 26) | (Width >> 6)) ^ ((Height << 7) | (Height >> 25));
		}

		public override string ToString()
		{
			return $"RECT left={left}, top={top}, right={right}, bottom={bottom}, width={right - left}, height={bottom - top}";
		}

		public static implicit operator Rectangle(RECT rect)
		{
			return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
		}

		public static implicit operator RECT(Rectangle rect)
		{
			return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}
	}

	internal enum SPIAction
	{
		SPI_GETACTIVEWINDOWTRACKING = 4096,
		SPI_GETACTIVEWNDTRKTIMEOUT = 8194,
		SPI_GETANIMATION = 72,
		SPI_GETCARETWIDTH = 8198,
		SPI_GETCOMBOBOXANIMATION = 4100,
		SPI_GETDRAGFULLWINDOWS = 38,
		SPI_GETDROPSHADOW = 4132,
		SPI_GETFONTSMOOTHING = 74,
		SPI_GETFONTSMOOTHINGCONTRAST = 8204,
		SPI_GETFONTSMOOTHINGTYPE = 8202,
		SPI_GETGRADIENTCAPTIONS = 4104,
		SPI_GETHOTTRACKING = 4110,
		SPI_GETICONTITLEWRAP = 25,
		SPI_GETKEYBOARDSPEED = 10,
		SPI_GETKEYBOARDDELAY = 22,
		SPI_GETKEYBOARDCUES = 4106,
		SPI_GETKEYBOARDPREF = 68,
		SPI_GETLISTBOXSMOOTHSCROLLING = 4102,
		SPI_GETMENUANIMATION = 4098,
		SPI_GETMENUDROPALIGNMENT = 27,
		SPI_GETMENUFADE = 4114,
		SPI_GETMENUSHOWDELAY = 106,
		SPI_GETMOUSESPEED = 112,
		SPI_GETSELECTIONFADE = 4116,
		SPI_GETSNAPTODEFBUTTON = 95,
		SPI_GETTOOLTIPANIMATION = 4118,
		SPI_GETWORKAREA = 48,
		SPI_GETMOUSEHOVERWIDTH = 98,
		SPI_GETMOUSEHOVERHEIGHT = 100,
		SPI_GETMOUSEHOVERTIME = 102,
		SPI_GETUIEFFECTS = 4158,
		SPI_GETWHEELSCROLLLINES = 104
	}

	internal enum WindowPlacementFlags
	{
		SW_HIDE = 0,
		SW_SHOWNORMAL = 1,
		SW_NORMAL = 1,
		SW_SHOWMINIMIZED = 2,
		SW_SHOWMAXIMIZED = 3,
		SW_MAXIMIZE = 3,
		SW_SHOWNOACTIVATE = 4,
		SW_SHOW = 5,
		SW_MINIMIZE = 6,
		SW_SHOWMINNOACTIVE = 7,
		SW_SHOWNA = 8,
		SW_RESTORE = 9,
		SW_SHOWDEFAULT = 10,
		SW_FORCEMINIMIZE = 11,
		SW_MAX = 11
	}

	private struct WINDOWPLACEMENT
	{
		internal uint length;

		internal uint flags;

		internal WindowPlacementFlags showCmd;

		internal POINT ptMinPosition;

		internal POINT ptMaxPosition;

		internal RECT rcNormalPosition;
	}

	internal struct NCCALCSIZE_PARAMS
	{
		internal RECT rgrc1;

		internal RECT rgrc2;

		internal RECT rgrc3;

		internal IntPtr lppos;
	}

	[Flags]
	private enum TMEFlags
	{
		TME_HOVER = 1,
		TME_LEAVE = 2,
		TME_NONCLIENT = 0x10,
		TME_QUERY = 0x40000000,
		TME_CANCEL = int.MinValue
	}

	private struct TRACKMOUSEEVENT
	{
		internal int size;

		internal TMEFlags dwFlags;

		internal IntPtr hWnd;

		internal int dwHoverTime;
	}

	private struct PAINTSTRUCT
	{
		internal IntPtr hdc;

		internal int fErase;

		internal RECT rcPaint;

		internal int fRestore;

		internal int fIncUpdate;

		internal int Reserved1;

		internal int Reserved2;

		internal int Reserved3;

		internal int Reserved4;

		internal int Reserved5;

		internal int Reserved6;

		internal int Reserved7;

		internal int Reserved8;
	}

	internal struct KEYBDINPUT
	{
		internal short wVk;

		internal short wScan;

		internal int dwFlags;

		internal int time;

		internal UIntPtr dwExtraInfo;
	}

	internal struct MOUSEINPUT
	{
		internal int dx;

		internal int dy;

		internal int mouseData;

		internal int dwFlags;

		internal int time;

		internal UIntPtr dwExtraInfo;
	}

	internal struct HARDWAREINPUT
	{
		internal int uMsg;

		internal short wParamL;

		internal short wParamH;
	}

	internal struct ICONINFO
	{
		internal bool fIcon;

		internal int xHotspot;

		internal int yHotspot;

		internal IntPtr hbmMask;

		internal IntPtr hbmColor;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct INPUT
	{
		[FieldOffset(0)]
		internal int type;

		[FieldOffset(4)]
		internal MOUSEINPUT mi;

		[FieldOffset(4)]
		internal KEYBDINPUT ki;

		[FieldOffset(4)]
		internal HARDWAREINPUT hi;
	}

	public struct ANIMATIONINFO
	{
		internal uint cbSize;

		internal int iMinAnimate;
	}

	internal enum InputFlags
	{
		KEYEVENTF_EXTENDEDKEY = 1,
		KEYEVENTF_KEYUP,
		KEYEVENTF_SCANCODE,
		KEYEVENTF_UNICODE
	}

	internal enum ClassStyle
	{
		CS_VREDRAW = 1,
		CS_HREDRAW = 2,
		CS_KEYCVTWINDOW = 4,
		CS_DBLCLKS = 8,
		CS_OWNDC = 0x20,
		CS_CLASSDC = 0x40,
		CS_PARENTDC = 0x80,
		CS_NOKEYCVT = 0x100,
		CS_NOCLOSE = 0x200,
		CS_SAVEBITS = 0x800,
		CS_BYTEALIGNCLIENT = 0x1000,
		CS_BYTEALIGNWINDOW = 0x2000,
		CS_GLOBALCLASS = 0x4000,
		CS_IME = 0x10000,
		CS_DROPSHADOW = 0x20000
	}

	internal enum SetWindowPosZOrder
	{
		HWND_TOP = 0,
		HWND_BOTTOM = 1,
		HWND_TOPMOST = -1,
		HWND_NOTOPMOST = -2
	}

	[Flags]
	internal enum SetWindowPosFlags
	{
		SWP_ASYNCWINDOWPOS = 0x4000,
		SWP_DEFERERASE = 0x2000,
		SWP_DRAWFRAME = 0x20,
		SWP_FRAMECHANGED = 0x20,
		SWP_HIDEWINDOW = 0x80,
		SWP_NOACTIVATE = 0x10,
		SWP_NOCOPYBITS = 0x100,
		SWP_NOMOVE = 2,
		SWP_NOOWNERZORDER = 0x200,
		SWP_NOREDRAW = 8,
		SWP_NOREPOSITION = 0x200,
		SWP_NOENDSCHANGING = 0x400,
		SWP_NOSIZE = 1,
		SWP_NOZORDER = 4,
		SWP_SHOWWINDOW = 0x40
	}

	internal enum GetSysColorIndex
	{
		COLOR_SCROLLBAR = 0,
		COLOR_BACKGROUND = 1,
		COLOR_ACTIVECAPTION = 2,
		COLOR_INACTIVECAPTION = 3,
		COLOR_MENU = 4,
		COLOR_WINDOW = 5,
		COLOR_WINDOWFRAME = 6,
		COLOR_MENUTEXT = 7,
		COLOR_WINDOWTEXT = 8,
		COLOR_CAPTIONTEXT = 9,
		COLOR_ACTIVEBORDER = 10,
		COLOR_INACTIVEBORDER = 11,
		COLOR_APPWORKSPACE = 12,
		COLOR_HIGHLIGHT = 13,
		COLOR_HIGHLIGHTTEXT = 14,
		COLOR_BTNFACE = 15,
		COLOR_BTNSHADOW = 16,
		COLOR_GRAYTEXT = 17,
		COLOR_BTNTEXT = 18,
		COLOR_INACTIVECAPTIONTEXT = 19,
		COLOR_BTNHIGHLIGHT = 20,
		COLOR_3DDKSHADOW = 21,
		COLOR_3DLIGHT = 22,
		COLOR_INFOTEXT = 23,
		COLOR_INFOBK = 24,
		COLOR_HOTLIGHT = 26,
		COLOR_GRADIENTACTIVECAPTION = 27,
		COLOR_GRADIENTINACTIVECAPTION = 28,
		COLOR_MENUHIGHLIGHT = 29,
		COLOR_MENUBAR = 30,
		COLOR_DESKTOP = 1,
		COLOR_3DFACE = 16,
		COLOR_3DSHADOW = 16,
		COLOR_3DHIGHLIGHT = 20,
		COLOR_3DHILIGHT = 20,
		COLOR_BTNHILIGHT = 20,
		COLOR_MAXVALUE = 24
	}

	private enum LoadCursorType
	{
		First = 32512,
		IDC_ARROW = 32512,
		IDC_IBEAM = 32513,
		IDC_WAIT = 32514,
		IDC_CROSS = 32515,
		IDC_UPARROW = 32516,
		IDC_SIZE = 32640,
		IDC_ICON = 32641,
		IDC_SIZENWSE = 32642,
		IDC_SIZENESW = 32643,
		IDC_SIZEWE = 32644,
		IDC_SIZENS = 32645,
		IDC_SIZEALL = 32646,
		IDC_NO = 32648,
		IDC_HAND = 32649,
		IDC_APPSTARTING = 32650,
		IDC_HELP = 32651,
		Last = 32651
	}

	private enum AncestorType
	{
		GA_PARENT = 1,
		GA_ROOT,
		GA_ROOTOWNER
	}

	[Flags]
	private enum WindowLong
	{
		GWL_WNDPROC = -4,
		GWL_HINSTANCE = -6,
		GWL_HWNDPARENT = -8,
		GWL_STYLE = -16,
		GWL_EXSTYLE = -20,
		GWL_USERDATA = -21,
		GWL_ID = -12
	}

	[Flags]
	private enum LogBrushStyle
	{
		BS_SOLID = 0,
		BS_NULL = 1,
		BS_HATCHED = 2,
		BS_PATTERN = 3,
		BS_INDEXED = 4,
		BS_DIBPATTERN = 5,
		BS_DIBPATTERNPT = 6,
		BS_PATTERN8X8 = 7,
		BS_DIBPATTERN8X8 = 8,
		BS_MONOPATTERN = 9
	}

	[Flags]
	private enum LogBrushHatch
	{
		HS_HORIZONTAL = 0,
		HS_VERTICAL = 1,
		HS_FDIAGONAL = 2,
		HS_BDIAGONAL = 3,
		HS_CROSS = 4,
		HS_DIAGCROSS = 5
	}

	internal struct COLORREF
	{
		internal byte R;

		internal byte G;

		internal byte B;

		internal byte A;
	}

	private struct LOGBRUSH
	{
		internal LogBrushStyle lbStyle;

		internal COLORREF lbColor;

		internal LogBrushHatch lbHatch;
	}

	internal struct TEXTMETRIC
	{
		internal int tmHeight;

		internal int tmAscent;

		internal int tmDescent;

		internal int tmInternalLeading;

		internal int tmExternalLeading;

		internal int tmAveCharWidth;

		internal int tmMaxCharWidth;

		internal int tmWeight;

		internal int tmOverhang;

		internal int tmDigitizedAspectX;

		internal int tmDigitizedAspectY;

		internal short tmFirstChar;

		internal short tmLastChar;

		internal short tmDefaultChar;

		internal short tmBreakChar;

		internal byte tmItalic;

		internal byte tmUnderlined;

		internal byte tmStruckOut;

		internal byte tmPitchAndFamily;

		internal byte tmCharSet;
	}

	public enum TernaryRasterOperations : uint
	{
		SRCCOPY = 13369376u,
		SRCPAINT = 15597702u,
		SRCAND = 8913094u,
		SRCINVERT = 6684742u,
		SRCERASE = 4457256u,
		NOTSRCCOPY = 3342344u,
		NOTSRCERASE = 1114278u,
		MERGECOPY = 12583114u,
		MERGEPAINT = 12255782u,
		PATCOPY = 15728673u,
		PATPAINT = 16452105u,
		PATINVERT = 5898313u,
		DSTINVERT = 5570569u,
		BLACKNESS = 66u,
		WHITENESS = 16711778u
	}

	[Flags]
	private enum ScrollWindowExFlags
	{
		SW_NONE = 0,
		SW_SCROLLCHILDREN = 1,
		SW_INVALIDATE = 2,
		SW_ERASE = 4,
		SW_SMOOTHSCROLL = 0x10
	}

	internal enum SystemMetrics
	{
		SM_CXSCREEN = 0,
		SM_CYSCREEN = 1,
		SM_CXVSCROLL = 2,
		SM_CYHSCROLL = 3,
		SM_CYCAPTION = 4,
		SM_CXBORDER = 5,
		SM_CYBORDER = 6,
		SM_CXDLGFRAME = 7,
		SM_CYDLGFRAME = 8,
		SM_CYVTHUMB = 9,
		SM_CXHTHUMB = 10,
		SM_CXICON = 11,
		SM_CYICON = 12,
		SM_CXCURSOR = 13,
		SM_CYCURSOR = 14,
		SM_CYMENU = 15,
		SM_CXFULLSCREEN = 16,
		SM_CYFULLSCREEN = 17,
		SM_CYKANJIWINDOW = 18,
		SM_MOUSEPRESENT = 19,
		SM_CYVSCROLL = 20,
		SM_CXHSCROLL = 21,
		SM_DEBUG = 22,
		SM_SWAPBUTTON = 23,
		SM_RESERVED1 = 24,
		SM_RESERVED2 = 25,
		SM_RESERVED3 = 26,
		SM_RESERVED4 = 27,
		SM_CXMIN = 28,
		SM_CYMIN = 29,
		SM_CXSIZE = 30,
		SM_CYSIZE = 31,
		SM_CXFRAME = 32,
		SM_CYFRAME = 33,
		SM_CXMINTRACK = 34,
		SM_CYMINTRACK = 35,
		SM_CXDOUBLECLK = 36,
		SM_CYDOUBLECLK = 37,
		SM_CXICONSPACING = 38,
		SM_CYICONSPACING = 39,
		SM_MENUDROPALIGNMENT = 40,
		SM_PENWINDOWS = 41,
		SM_DBCSENABLED = 42,
		SM_CMOUSEBUTTONS = 43,
		SM_CXFIXEDFRAME = 7,
		SM_CYFIXEDFRAME = 8,
		SM_CXSIZEFRAME = 32,
		SM_CYSIZEFRAME = 33,
		SM_SECURE = 44,
		SM_CXEDGE = 45,
		SM_CYEDGE = 46,
		SM_CXMINSPACING = 47,
		SM_CYMINSPACING = 48,
		SM_CXSMICON = 49,
		SM_CYSMICON = 50,
		SM_CYSMCAPTION = 51,
		SM_CXSMSIZE = 52,
		SM_CYSMSIZE = 53,
		SM_CXMENUSIZE = 54,
		SM_CYMENUSIZE = 55,
		SM_ARRANGE = 56,
		SM_CXMINIMIZED = 57,
		SM_CYMINIMIZED = 58,
		SM_CXMAXTRACK = 59,
		SM_CYMAXTRACK = 60,
		SM_CXMAXIMIZED = 61,
		SM_CYMAXIMIZED = 62,
		SM_NETWORK = 63,
		SM_CLEANBOOT = 67,
		SM_CXDRAG = 68,
		SM_CYDRAG = 69,
		SM_SHOWSOUNDS = 70,
		SM_CXMENUCHECK = 71,
		SM_CYMENUCHECK = 72,
		SM_SLOWMACHINE = 73,
		SM_MIDEASTENABLED = 74,
		SM_MOUSEWHEELPRESENT = 75,
		SM_XVIRTUALSCREEN = 76,
		SM_YVIRTUALSCREEN = 77,
		SM_CXVIRTUALSCREEN = 78,
		SM_CYVIRTUALSCREEN = 79,
		SM_CMONITORS = 80,
		SM_SAMEDISPLAYFORMAT = 81,
		SM_IMMENABLED = 82,
		SM_CXFOCUSBORDER = 83,
		SM_CYFOCUSBORDER = 84,
		SM_TABLETPC = 86,
		SM_MEDIACENTER = 87,
		SM_CMETRICS = 88
	}

	internal enum NotifyIconMessage
	{
		NIM_ADD,
		NIM_MODIFY,
		NIM_DELETE
	}

	[Flags]
	internal enum NotifyIconFlags
	{
		NIF_MESSAGE = 1,
		NIF_ICON = 2,
		NIF_TIP = 4,
		NIF_STATE = 8,
		NIF_INFO = 0x10
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct NOTIFYICONDATA
	{
		internal uint cbSize;

		internal IntPtr hWnd;

		internal uint uID;

		internal NotifyIconFlags uFlags;

		internal uint uCallbackMessage;

		internal IntPtr hIcon;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		internal string szTip;

		internal int dwState;

		internal int dwStateMask;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		internal string szInfo;

		internal int uTimeoutOrVersion;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		internal string szInfoTitle;

		internal ToolTipIcon dwInfoFlags;
	}

	[Flags]
	internal enum DCExFlags
	{
		DCX_WINDOW = 1,
		DCX_CACHE = 2,
		DCX_NORESETATTRS = 4,
		DCX_CLIPCHILDREN = 8,
		DCX_CLIPSIBLINGS = 0x10,
		DCX_PARENTCLIP = 0x20,
		DCX_EXCLUDERGN = 0x40,
		DCX_INTERSECTRGN = 0x80,
		DCX_EXCLUDEUPDATE = 0x100,
		DCX_INTERSECTUPDATE = 0x200,
		DCX_LOCKWINDOWUPDATE = 0x400,
		DCX_USESTYLE = 0x10000,
		DCX_VALIDATE = 0x200000
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CLIENTCREATESTRUCT
	{
		internal IntPtr hWindowMenu;

		internal uint idFirstChild;
	}

	private enum ClassLong
	{
		GCL_MENUNAME = -8,
		GCL_HBRBACKGROUND = -10,
		GCL_HCURSOR = -12,
		GCL_HICON = -14,
		GCL_HMODULE = -16,
		GCL_CBWNDEXTRA = -18,
		GCL_CBCLSEXTRA = -20,
		GCL_WNDPROC = -24,
		GCL_STYLE = -26,
		GCW_ATOM = -32,
		GCL_HICONSM = -34
	}

	[Flags]
	internal enum GAllocFlags : uint
	{
		GMEM_FIXED = 0u,
		GMEM_MOVEABLE = 2u,
		GMEM_NOCOMPACT = 0x10u,
		GMEM_NODISCARD = 0x20u,
		GMEM_ZEROINIT = 0x40u,
		GMEM_MODIFY = 0x80u,
		GMEM_DISCARDABLE = 0x100u,
		GMEM_NOT_BANKED = 0x1000u,
		GMEM_SHARE = 0x2000u,
		GMEM_DDESHARE = 0x2000u,
		GMEM_NOTIFY = 0x4000u,
		GMEM_LOWER = 0x1000u,
		GMEM_VALID_FLAGS = 0x7F72u,
		GMEM_INVALID_HANDLE = 0x8000u,
		GHND = 0x42u,
		GPTR = 0x40u
	}

	internal enum ROP2DrawMode
	{
		R2_BLACK = 1,
		R2_NOTMERGEPEN = 2,
		R2_MASKNOTPEN = 3,
		R2_NOTCOPYPEN = 4,
		R2_MASKPENNOT = 5,
		R2_NOT = 6,
		R2_XORPEN = 7,
		R2_NOTMASKPEN = 8,
		R2_MASKPEN = 9,
		R2_NOTXORPEN = 10,
		R2_NOP = 11,
		R2_MERGENOTPEN = 12,
		R2_COPYPEN = 13,
		R2_MERGEPENNOT = 14,
		R2_MERGEPEN = 15,
		R2_WHITE = 16,
		R2_LAST = 16
	}

	internal enum PenStyle
	{
		PS_SOLID,
		PS_DASH,
		PS_DOT,
		PS_DASHDOT,
		PS_DASHDOTDOT,
		PS_NULL,
		PS_INSIDEFRAME,
		PS_USERSTYLE,
		PS_ALTERNATE
	}

	internal enum PatBltRop
	{
		PATCOPY = 15728673,
		PATINVERT = 5898313,
		DSTINVERT = 5570569,
		BLACKNESS = 66,
		WHITENESS = 16711778
	}

	internal enum StockObject
	{
		WHITE_BRUSH = 0,
		LTGRAY_BRUSH = 1,
		GRAY_BRUSH = 2,
		DKGRAY_BRUSH = 3,
		BLACK_BRUSH = 4,
		NULL_BRUSH = 5,
		HOLLOW_BRUSH = 5,
		WHITE_PEN = 6,
		BLACK_PEN = 7,
		NULL_PEN = 8,
		OEM_FIXED_FONT = 10,
		ANSI_FIXED_FONT = 11,
		ANSI_VAR_FONT = 12,
		SYSTEM_FONT = 13,
		DEVICE_DEFAULT_FONT = 14,
		DEFAULT_PALETTE = 15,
		SYSTEM_FIXED_FONT = 16
	}

	internal enum HatchStyle
	{
		HS_HORIZONTAL,
		HS_VERTICAL,
		HS_FDIAGONAL,
		HS_BDIAGONAL,
		HS_CROSS,
		HS_DIAGCROSS
	}

	[Flags]
	internal enum SndFlags
	{
		SND_SYNC = 0,
		SND_ASYNC = 1,
		SND_NODEFAULT = 2,
		SND_MEMORY = 4,
		SND_LOOP = 8,
		SND_NOSTOP = 0x10,
		SND_NOWAIT = 0x2000,
		SND_ALIAS = 0x10000,
		SND_ALIAS_ID = 0x110000,
		SND_FILENAME = 0x20000,
		SND_RESOURCE = 0x40004,
		SND_PURGE = 0x40,
		SND_APPLICATION = 0x80
	}

	[Flags]
	internal enum LayeredWindowAttributes
	{
		LWA_COLORKEY = 1,
		LWA_ALPHA = 2
	}

	public enum ACLineStatus : byte
	{
		Offline = 0,
		Online = 1,
		Unknown = byte.MaxValue
	}

	public enum BatteryFlag : byte
	{
		High = 1,
		Low = 2,
		Critical = 4,
		Charging = 8,
		NoSystemBattery = 128,
		Unknown = byte.MaxValue
	}

	[StructLayout(LayoutKind.Sequential)]
	public class SYSTEMPOWERSTATUS
	{
		public ACLineStatus _ACLineStatus;

		public BatteryFlag _BatteryFlag;

		public byte _BatteryLifePercent;

		public byte _Reserved1;

		public int _BatteryLifeTime;

		public int _BatteryFullLifeTime;
	}

	private class WinBuffer
	{
		public IntPtr hdc;

		public IntPtr bitmap;

		public WinBuffer(IntPtr hdc, IntPtr bitmap)
		{
			this.hdc = hdc;
			this.bitmap = bitmap;
		}
	}

	private static XplatUIWin32 instance;

	private static int ref_count;

	private static IntPtr FosterParent;

	internal static MouseButtons mouse_state;

	internal static Point mouse_position;

	internal static bool grab_confined;

	internal static IntPtr grab_hwnd;

	internal static Rectangle grab_area;

	internal static WndProc wnd_proc;

	internal static IntPtr prev_mouse_hwnd;

	internal static bool caret_visible;

	internal static bool themes_enabled;

	private Hashtable timer_list;

	private static Queue message_queue;

	private static IntPtr clip_magic = new IntPtr(27051977);

	private static int scroll_width;

	private static int scroll_height;

	private static Hashtable wm_nc_registered;

	private static RECT clipped_cursor_rect;

	private Hashtable registered_classes;

	private Hwnd HwndCreating;

	private TransparencySupport support;

	private bool queried_transparency_support;

	internal override int ActiveWindowTrackingDelay => GetSystemParametersInfoInt(SPIAction.SPI_GETACTIVEWNDTRKTIMEOUT);

	internal override int CaretWidth
	{
		get
		{
			if (Environment.OSVersion.Version.Major < 5)
			{
				throw new NotSupportedException();
			}
			return GetSystemParametersInfoInt(SPIAction.SPI_GETCARETWIDTH);
		}
	}

	internal override int FontSmoothingContrast
	{
		get
		{
			if (Environment.OSVersion.Version.Major < 5 || (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 0))
			{
				throw new NotSupportedException();
			}
			return GetSystemParametersInfoInt(SPIAction.SPI_GETFONTSMOOTHINGCONTRAST);
		}
	}

	internal override int FontSmoothingType
	{
		get
		{
			if (Environment.OSVersion.Version.Major < 5 || (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 0))
			{
				throw new NotSupportedException();
			}
			return GetSystemParametersInfoInt(SPIAction.SPI_GETFONTSMOOTHINGTYPE);
		}
	}

	internal override int HorizontalResizeBorderThickness => Win32GetSystemMetrics(SystemMetrics.SM_CXFRAME);

	internal override bool IsActiveWindowTrackingEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETACTIVEWINDOWTRACKING);

	internal override bool IsComboBoxAnimationEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETCOMBOBOXANIMATION);

	internal override bool IsDropShadowEnabled
	{
		get
		{
			if (Environment.OSVersion.Version.Major < 5 || (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor == 0))
			{
				throw new NotSupportedException();
			}
			return GetSystemParametersInfoBool(SPIAction.SPI_GETDROPSHADOW);
		}
	}

	internal override bool IsFontSmoothingEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETFONTSMOOTHING);

	internal override bool IsHotTrackingEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETHOTTRACKING);

	internal override bool IsIconTitleWrappingEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETICONTITLEWRAP);

	internal override bool IsKeyboardPreferred => GetSystemParametersInfoBool(SPIAction.SPI_GETKEYBOARDPREF);

	internal override bool IsListBoxSmoothScrollingEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETLISTBOXSMOOTHSCROLLING);

	internal override bool IsMenuAnimationEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETMENUANIMATION);

	internal override bool IsMenuFadeEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETMENUFADE);

	internal override bool IsMinimizeRestoreAnimationEnabled
	{
		get
		{
			ANIMATIONINFO value = default(ANIMATIONINFO);
			value.cbSize = (uint)Marshal.SizeOf(value);
			Win32SystemParametersInfo(SPIAction.SPI_GETANIMATION, 0u, ref value, 0u);
			return (value.iMinAnimate != 0) ? true : false;
		}
	}

	internal override bool IsSelectionFadeEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETSELECTIONFADE);

	internal override bool IsSnapToDefaultEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETSNAPTODEFBUTTON);

	internal override bool IsTitleBarGradientEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETGRADIENTCAPTIONS);

	internal override bool IsToolTipAnimationEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETTOOLTIPANIMATION);

	internal override Size MenuBarButtonSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXMENUSIZE), Win32GetSystemMetrics(SystemMetrics.SM_CYMENUSIZE));

	public override Size MenuButtonSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXMENUSIZE), Win32GetSystemMetrics(SystemMetrics.SM_CYMENUSIZE));

	internal override int MenuShowDelay => GetSystemParametersInfoInt(SPIAction.SPI_GETMENUSHOWDELAY);

	internal override int MouseSpeed => GetSystemParametersInfoInt(SPIAction.SPI_GETMOUSESPEED);

	internal override LeftRightAlignment PopupMenuAlignment => (!GetSystemParametersInfoBool(SPIAction.SPI_GETMENUDROPALIGNMENT)) ? LeftRightAlignment.Right : LeftRightAlignment.Left;

	internal override PowerStatus PowerStatus
	{
		get
		{
			SYSTEMPOWERSTATUS sYSTEMPOWERSTATUS = new SYSTEMPOWERSTATUS();
			Win32GetSystemPowerStatus(sYSTEMPOWERSTATUS);
			return new PowerStatus((BatteryChargeStatus)sYSTEMPOWERSTATUS._BatteryFlag, sYSTEMPOWERSTATUS._BatteryFullLifeTime, (float)(int)sYSTEMPOWERSTATUS._BatteryLifePercent / 255f, sYSTEMPOWERSTATUS._BatteryLifeTime, (PowerLineStatus)sYSTEMPOWERSTATUS._ACLineStatus);
		}
	}

	internal override int SizingBorderWidth => Win32GetSystemMetrics(SystemMetrics.SM_CXFRAME);

	internal override Size SmallCaptionButtonSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXSMSIZE), Win32GetSystemMetrics(SystemMetrics.SM_CYSMSIZE));

	internal override bool UIEffectsEnabled => GetSystemParametersInfoBool(SPIAction.SPI_GETUIEFFECTS);

	internal override int VerticalResizeBorderThickness => Win32GetSystemMetrics(SystemMetrics.SM_CYFRAME);

	internal override Keys ModifierKeys
	{
		get
		{
			Keys keys = Keys.None;
			short num = Win32GetKeyState(VirtualKeys.VK_SHIFT);
			if (((uint)num & 0x8000u) != 0)
			{
				keys |= Keys.Shift;
			}
			num = Win32GetKeyState(VirtualKeys.VK_CONTROL);
			if (((uint)num & 0x8000u) != 0)
			{
				keys |= Keys.Control;
			}
			num = Win32GetKeyState(VirtualKeys.VK_MENU);
			if (((uint)num & 0x8000u) != 0)
			{
				keys |= Keys.Alt;
			}
			return keys;
		}
	}

	internal override MouseButtons MouseButtons => mouse_state;

	internal override Point MousePosition => mouse_position;

	internal override Size MouseHoverSize
	{
		get
		{
			int value = 4;
			int value2 = 4;
			Win32SystemParametersInfo(SPIAction.SPI_GETMOUSEHOVERWIDTH, 0u, ref value, 0u);
			Win32SystemParametersInfo(SPIAction.SPI_GETMOUSEHOVERWIDTH, 0u, ref value2, 0u);
			return new Size(value, value2);
		}
	}

	internal override int MouseHoverTime
	{
		get
		{
			int value = 500;
			Win32SystemParametersInfo(SPIAction.SPI_GETMOUSEHOVERTIME, 0u, ref value, 0u);
			return value;
		}
	}

	internal override int MouseWheelScrollDelta
	{
		get
		{
			int value = 120;
			Win32SystemParametersInfo(SPIAction.SPI_GETWHEELSCROLLLINES, 0u, ref value, 0u);
			return value;
		}
	}

	internal override int HorizontalScrollBarHeight => scroll_height;

	internal override bool UserClipWontExposeParent => false;

	internal override int VerticalScrollBarWidth => scroll_width;

	internal override int MenuHeight => Win32GetSystemMetrics(SystemMetrics.SM_CYMENU);

	internal override Size Border3DSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXEDGE), Win32GetSystemMetrics(SystemMetrics.SM_CYEDGE));

	internal override Size BorderSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXBORDER), Win32GetSystemMetrics(SystemMetrics.SM_CYBORDER));

	internal override bool DropTarget
	{
		get
		{
			return false;
		}
		set
		{
			if (!value)
			{
			}
		}
	}

	internal override Size CaptionButtonSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXSIZE), Win32GetSystemMetrics(SystemMetrics.SM_CYSIZE));

	internal override int CaptionHeight => Win32GetSystemMetrics(SystemMetrics.SM_CYCAPTION);

	internal override Size CursorSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXCURSOR), Win32GetSystemMetrics(SystemMetrics.SM_CYCURSOR));

	internal override bool DragFullWindows
	{
		get
		{
			int value = 0;
			Win32SystemParametersInfo(SPIAction.SPI_GETDRAGFULLWINDOWS, 0u, ref value, 0u);
			return value != 0;
		}
	}

	internal override Size DragSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXDRAG), Win32GetSystemMetrics(SystemMetrics.SM_CYDRAG));

	internal override Size DoubleClickSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXDOUBLECLK), Win32GetSystemMetrics(SystemMetrics.SM_CYDOUBLECLK));

	internal override int DoubleClickTime => Win32GetDoubleClickTime();

	internal override Size FixedFrameBorderSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXDLGFRAME), Win32GetSystemMetrics(SystemMetrics.SM_CYDLGFRAME));

	internal override Size FrameBorderSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXFRAME), Win32GetSystemMetrics(SystemMetrics.SM_CYFRAME));

	internal override Size IconSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXICON), Win32GetSystemMetrics(SystemMetrics.SM_CYICON));

	internal override Size MaxWindowTrackSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXMAXTRACK), Win32GetSystemMetrics(SystemMetrics.SM_CYMAXTRACK));

	internal override bool MenuAccessKeysUnderlined
	{
		get
		{
			int value = 0;
			Win32SystemParametersInfo(SPIAction.SPI_GETKEYBOARDCUES, 0u, ref value, 0u);
			return value != 0;
		}
	}

	internal override Size MinimizedWindowSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXMINIMIZED), Win32GetSystemMetrics(SystemMetrics.SM_CYMINIMIZED));

	internal override Size MinimizedWindowSpacingSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXMINSPACING), Win32GetSystemMetrics(SystemMetrics.SM_CYMINSPACING));

	internal override Size MinimumWindowSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXMIN), Win32GetSystemMetrics(SystemMetrics.SM_CYMIN));

	internal override Size MinWindowTrackSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXMINTRACK), Win32GetSystemMetrics(SystemMetrics.SM_CYMINTRACK));

	internal override Size SmallIconSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXSMICON), Win32GetSystemMetrics(SystemMetrics.SM_CYSMICON));

	internal override int MouseButtonCount => Win32GetSystemMetrics(SystemMetrics.SM_CMOUSEBUTTONS);

	internal override bool MouseButtonsSwapped => Win32GetSystemMetrics(SystemMetrics.SM_SWAPBUTTON) != 0;

	internal override bool MouseWheelPresent => Win32GetSystemMetrics(SystemMetrics.SM_MOUSEWHEELPRESENT) != 0;

	internal override Rectangle VirtualScreen => new Rectangle(Win32GetSystemMetrics(SystemMetrics.SM_XVIRTUALSCREEN), Win32GetSystemMetrics(SystemMetrics.SM_YVIRTUALSCREEN), Win32GetSystemMetrics(SystemMetrics.SM_CXVIRTUALSCREEN), Win32GetSystemMetrics(SystemMetrics.SM_CYVIRTUALSCREEN));

	internal override Rectangle WorkingArea
	{
		get
		{
			RECT rect = default(RECT);
			Win32SystemParametersInfo(SPIAction.SPI_GETWORKAREA, 0u, ref rect, 0u);
			return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
		}
	}

	internal override bool ThemesEnabled => themes_enabled;

	internal override bool RequiresPositiveClientAreaSize => false;

	public override int ToolWindowCaptionHeight => Win32GetSystemMetrics(SystemMetrics.SM_CYSMCAPTION);

	public override Size ToolWindowCaptionButtonSize => new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXSMSIZE), Win32GetSystemMetrics(SystemMetrics.SM_CYSMSIZE));

	public int Reference => ref_count;

	internal override int KeyboardSpeed
	{
		get
		{
			int value = 0;
			Win32SystemParametersInfo(SPIAction.SPI_GETKEYBOARDSPEED, 0u, ref value, 0u);
			return value;
		}
	}

	internal override int KeyboardDelay
	{
		get
		{
			int value = 1;
			Win32SystemParametersInfo(SPIAction.SPI_GETKEYBOARDDELAY, 0u, ref value, 0u);
			return value;
		}
	}

	internal override event EventHandler Idle;

	private XplatUIWin32()
	{
		ref_count = 0;
		mouse_state = MouseButtons.None;
		mouse_position = Point.Empty;
		grab_confined = false;
		grab_area = Rectangle.Empty;
		message_queue = new Queue();
		themes_enabled = false;
		wnd_proc = InternalWndProc;
		FosterParent = Win32CreateWindow(WindowExStyles.WS_EX_TOOLWINDOW, "static", "Foster Parent Window", WindowStyles.WS_OVERLAPPEDWINDOW, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		if (FosterParent == IntPtr.Zero)
		{
			Win32MessageBox(IntPtr.Zero, "Could not create foster window, win32 error " + Win32GetLastError(), "Oops", 0u);
		}
		scroll_height = Win32GetSystemMetrics(SystemMetrics.SM_CYHSCROLL);
		scroll_width = Win32GetSystemMetrics(SystemMetrics.SM_CXVSCROLL);
		timer_list = new Hashtable();
		registered_classes = new Hashtable();
	}

	private string RegisterWindowClass(int classStyle)
	{
		lock (registered_classes)
		{
			string text = (string)registered_classes[classStyle];
			if (text != null)
			{
				return text;
			}
			text = $"Mono.WinForms.{Thread.GetDomainID().ToString()}.{classStyle}";
			WNDCLASS wndClass = default(WNDCLASS);
			wndClass.style = classStyle;
			wndClass.lpfnWndProc = wnd_proc;
			wndClass.cbClsExtra = 0;
			wndClass.cbWndExtra = 0;
			wndClass.hbrBackground = (IntPtr)6;
			wndClass.hCursor = Win32LoadCursor(IntPtr.Zero, LoadCursorType.First);
			wndClass.hIcon = IntPtr.Zero;
			wndClass.hInstance = IntPtr.Zero;
			wndClass.lpszClassName = text;
			wndClass.lpszMenuName = string.Empty;
			if (!Win32RegisterClass(ref wndClass))
			{
				Console.WriteLine("Oops: Could not register the window class, win32 error {0}", Win32GetLastError().ToString());
			}
			registered_classes[classStyle] = text;
			return text;
		}
	}

	private static bool RetrieveMessage(ref MSG msg)
	{
		if (message_queue.Count == 0)
		{
			return false;
		}
		MSG mSG = (MSG)message_queue.Dequeue();
		msg = mSG;
		return true;
	}

	private static bool StoreMessage(ref MSG msg)
	{
		MSG mSG = default(MSG);
		mSG = msg;
		message_queue.Enqueue(mSG);
		return true;
	}

	internal static string AnsiToString(IntPtr ansi_data)
	{
		return Marshal.PtrToStringAnsi(ansi_data);
	}

	internal static string UnicodeToString(IntPtr unicode_data)
	{
		return Marshal.PtrToStringUni(unicode_data);
	}

	internal static Image DIBtoImage(IntPtr dib_data)
	{
		BITMAPINFOHEADER bITMAPINFOHEADER = (BITMAPINFOHEADER)Marshal.PtrToStructure(dib_data, typeof(BITMAPINFOHEADER));
		int num = (int)bITMAPINFOHEADER.biClrUsed;
		if (num == 0 && bITMAPINFOHEADER.biBitCount < 24)
		{
			num = 1 << (int)bITMAPINFOHEADER.biBitCount;
		}
		if (bITMAPINFOHEADER.biSizeImage == 0)
		{
			int num2 = (((bITMAPINFOHEADER.biWidth * bITMAPINFOHEADER.biBitCount + 31) & -32) >> 3) * bITMAPINFOHEADER.biHeight;
		}
		Bitmap bitmap;
		int[] array;
		switch (bITMAPINFOHEADER.biBitCount)
		{
		case 1:
			bitmap = new Bitmap(bITMAPINFOHEADER.biWidth, bITMAPINFOHEADER.biHeight, PixelFormat.Format1bppIndexed);
			array = new int[2];
			break;
		case 4:
			bitmap = new Bitmap(bITMAPINFOHEADER.biWidth, bITMAPINFOHEADER.biHeight, PixelFormat.Format4bppIndexed);
			array = new int[16];
			break;
		case 8:
			bitmap = new Bitmap(bITMAPINFOHEADER.biWidth, bITMAPINFOHEADER.biHeight, PixelFormat.Format8bppIndexed);
			array = new int[256];
			break;
		case 24:
		case 32:
			bitmap = new Bitmap(bITMAPINFOHEADER.biWidth, bITMAPINFOHEADER.biHeight, PixelFormat.Format32bppArgb);
			array = new int[0];
			break;
		default:
			throw new Exception("Unexpected number of bits:" + bITMAPINFOHEADER.biBitCount);
		}
		if (bITMAPINFOHEADER.biBitCount < 24)
		{
			ColorPalette palette = bitmap.Palette;
			Marshal.Copy((IntPtr)((int)dib_data + Marshal.SizeOf(typeof(BITMAPINFOHEADER))), array, 0, array.Length);
			for (int i = 0; i < num; i++)
			{
				ref Color reference = ref palette.Entries[i];
				reference = Color.FromArgb(array[i] | -16777216);
			}
			bitmap.Palette = palette;
		}
		int num3 = ((bITMAPINFOHEADER.biWidth * bITMAPINFOHEADER.biBitCount + 31) & -32) >> 3;
		BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
		byte[] array2 = new byte[num3];
		for (int j = 0; j < bITMAPINFOHEADER.biHeight; j++)
		{
			Marshal.Copy((IntPtr)((int)dib_data + Marshal.SizeOf(typeof(BITMAPINFOHEADER)) + array.Length * 4 + num3 * j), array2, 0, num3);
			Marshal.Copy(array2, 0, (IntPtr)((int)bitmapData.Scan0 + bitmapData.Stride * (bITMAPINFOHEADER.biHeight - 1 - j)), array2.Length);
		}
		bitmap.UnlockBits(bitmapData);
		return bitmap;
	}

	internal static byte[] ImageToDIB(Image image)
	{
		MemoryStream memoryStream = new MemoryStream();
		image.Save(memoryStream, ImageFormat.Bmp);
		byte[] buffer = memoryStream.GetBuffer();
		byte[] array = new byte[buffer.Length];
		Array.Copy(buffer, 14, array, 0, buffer.Length - 14);
		return array;
	}

	internal static IntPtr DupGlobalMem(IntPtr mem)
	{
		uint num = Win32GlobalSize(mem);
		IntPtr source = Win32GlobalLock(mem);
		IntPtr intPtr = Win32GlobalAlloc(GAllocFlags.GMEM_MOVEABLE, (int)num);
		IntPtr destination = Win32GlobalLock(intPtr);
		Win32CopyMemory(destination, source, (int)num);
		Win32GlobalUnlock(mem);
		Win32GlobalUnlock(intPtr);
		return intPtr;
	}

	private int GetSystemParametersInfoInt(SPIAction spi)
	{
		int value = 0;
		Win32SystemParametersInfo(spi, 0u, ref value, 0u);
		return value;
	}

	private bool GetSystemParametersInfoBool(SPIAction spi)
	{
		bool value = false;
		Win32SystemParametersInfo(spi, 0u, ref value, 0u);
		return value;
	}

	internal override void RaiseIdle(EventArgs e)
	{
		if (Idle != null)
		{
			Idle(this, e);
		}
	}

	public static XplatUIWin32 GetInstance()
	{
		if (instance == null)
		{
			instance = new XplatUIWin32();
		}
		ref_count++;
		return instance;
	}

	internal override IntPtr InitializeDriver()
	{
		return IntPtr.Zero;
	}

	internal override void ShutdownDriver(IntPtr token)
	{
		Console.WriteLine("XplatUIWin32 ShutdownDriver called");
	}

	internal void Version()
	{
		Console.WriteLine("Xplat version $revision: $");
	}

	private string GetSoundAlias(AlertType alert)
	{
		return alert switch
		{
			AlertType.Error => "SystemHand", 
			AlertType.Question => "SystemQuestion", 
			AlertType.Warning => "SystemExclamation", 
			AlertType.Information => "SystemAsterisk", 
			_ => "SystemDefault", 
		};
	}

	internal override void AudibleAlert(AlertType alert)
	{
		Win32PlaySound(GetSoundAlias(alert), IntPtr.Zero, SndFlags.SND_ALIAS_ID | SndFlags.SND_ASYNC | SndFlags.SND_NOSTOP | SndFlags.SND_NOWAIT);
	}

	internal override void GetDisplaySize(out Size size)
	{
		Win32GetWindowRect(Win32GetDesktopWindow(), out var rect);
		size = new Size(rect.right - rect.left, rect.bottom - rect.top);
	}

	internal override void EnableThemes()
	{
		themes_enabled = true;
	}

	internal override IntPtr CreateWindow(CreateParams cp)
	{
		Hwnd hwnd = new Hwnd();
		IntPtr intPtr = cp.Parent;
		if (intPtr == IntPtr.Zero && ((uint)cp.Style & 0x40000000u) != 0)
		{
			intPtr = FosterParent;
		}
		if ((cp.Style & -1073741824) == 0 && (cp.ExStyle & 0x40000) == 0)
		{
			intPtr = FosterParent;
		}
		Point point = ((!cp.HasWindowManager) ? new Point(cp.X, cp.Y) : Hwnd.GetNextStackedFormLocation(cp, Hwnd.ObjectFromHandle(cp.Parent)));
		string lpClassName = RegisterWindowClass(cp.ClassStyle);
		HwndCreating = hwnd;
		if ((cp.WindowExStyle & WindowExStyles.WS_EX_MDICHILD) == WindowExStyles.WS_EX_MDICHILD)
		{
			cp.WindowExStyle ^= WindowExStyles.WS_EX_MDICHILD;
		}
		IntPtr intPtr2 = Win32CreateWindow(cp.WindowExStyle, lpClassName, cp.Caption, cp.WindowStyle, point.X, point.Y, cp.Width, cp.Height, intPtr, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		HwndCreating = null;
		if (intPtr2 == IntPtr.Zero)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			Win32MessageBox(IntPtr.Zero, "Error : " + lastWin32Error, "Failed to create window, class '" + cp.ClassName + "'", 0u);
		}
		hwnd.ClientWindow = intPtr2;
		hwnd.Mapped = true;
		Win32SetWindowLong(intPtr2, WindowLong.GWL_USERDATA, (uint)ThemeEngine.Current.DefaultControlBackColor.ToArgb());
		return intPtr2;
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

	internal override void DestroyWindow(IntPtr handle)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(handle);
		Win32DestroyWindow(handle);
		hwnd.Dispose();
	}

	internal override void SetWindowMinMax(IntPtr handle, Rectangle maximized, Size min, Size max)
	{
	}

	internal override FormWindowState GetWindowState(IntPtr handle)
	{
		uint num = Win32GetWindowLong(handle, WindowLong.GWL_STYLE);
		if ((num & 0x1000000u) != 0)
		{
			return FormWindowState.Maximized;
		}
		if ((num & 0x20000000u) != 0)
		{
			return FormWindowState.Minimized;
		}
		return FormWindowState.Normal;
	}

	internal override void SetWindowState(IntPtr hwnd, FormWindowState state)
	{
		switch (state)
		{
		case FormWindowState.Normal:
			Win32ShowWindow(hwnd, WindowPlacementFlags.SW_RESTORE);
			break;
		case FormWindowState.Minimized:
			Win32ShowWindow(hwnd, WindowPlacementFlags.SW_MINIMIZE);
			break;
		case FormWindowState.Maximized:
			Win32ShowWindow(hwnd, WindowPlacementFlags.SW_SHOWMAXIMIZED);
			break;
		}
	}

	internal override void SetWindowStyle(IntPtr handle, CreateParams cp)
	{
		Win32SetWindowLong(handle, WindowLong.GWL_STYLE, (uint)cp.Style);
		Win32SetWindowLong(handle, WindowLong.GWL_EXSTYLE, (uint)cp.ExStyle);
		if (cp.control is Form)
		{
			XplatUI.RequestNCRecalc(handle);
		}
	}

	internal override double GetWindowTransparency(IntPtr handle)
	{
		if (Win32GetLayeredWindowAttributes(handle, out var _, out var pbAlpha, out var _) == 0)
		{
			return 1.0;
		}
		return (double)(int)pbAlpha / 255.0;
	}

	internal override void SetWindowTransparency(IntPtr handle, double transparency, Color key)
	{
		LayeredWindowAttributes layeredWindowAttributes = LayeredWindowAttributes.LWA_ALPHA;
		byte bAlpha = (byte)(transparency * 255.0);
		COLORREF crKey = default(COLORREF);
		if (key != Color.Empty)
		{
			crKey.R = key.R;
			crKey.G = key.G;
			crKey.B = key.B;
			layeredWindowAttributes |= LayeredWindowAttributes.LWA_COLORKEY;
		}
		RECT rECT = default(RECT);
		rECT.right = 1000;
		rECT.bottom = 1000;
		Win32SetLayeredWindowAttributes(handle, crKey, bAlpha, layeredWindowAttributes);
	}

	internal override TransparencySupport SupportsTransparency()
	{
		if (queried_transparency_support)
		{
			return support;
		}
		support = TransparencySupport.None;
		bool flag = true;
		try
		{
			Win32SetLayeredWindowAttributes(IntPtr.Zero, default(COLORREF), byte.MaxValue, LayeredWindowAttributes.LWA_ALPHA);
		}
		catch (EntryPointNotFoundException)
		{
			flag = false;
		}
		catch
		{
		}
		if (flag)
		{
			support |= TransparencySupport.Set;
		}
		flag = true;
		try
		{
			Win32GetLayeredWindowAttributes(IntPtr.Zero, out var _, out var _, out var _);
		}
		catch (EntryPointNotFoundException)
		{
			flag = false;
		}
		catch
		{
		}
		if (flag)
		{
			support |= TransparencySupport.Get;
		}
		queried_transparency_support = true;
		return support;
	}

	internal override void UpdateWindow(IntPtr handle)
	{
		Win32UpdateWindow(handle);
	}

	internal override PaintEventArgs PaintEventStart(ref Message msg, IntPtr handle, bool client)
	{
		Rectangle rectangle = default(Rectangle);
		RECT rect = default(RECT);
		PAINTSTRUCT ps = default(PAINTSTRUCT);
		Hwnd hwnd = Hwnd.ObjectFromHandle(msg.HWnd);
		IntPtr intPtr;
		if (client)
		{
			if (Win32GetUpdateRect(msg.HWnd, ref rect, erase: false))
			{
				if (handle != msg.HWnd)
				{
					Win32GetClientRect(msg.HWnd, out rect);
					Win32ValidateRect(msg.HWnd, ref rect);
					intPtr = Win32GetDC(handle);
				}
				else
				{
					intPtr = Win32BeginPaint(handle, ref ps);
					rect = ps.rcPaint;
				}
			}
			else
			{
				intPtr = Win32GetDC(handle);
			}
			rectangle = rect.ToRectangle();
		}
		else
		{
			intPtr = Win32GetWindowDC(handle);
			Win32GetWindowRect(handle, out rect);
			rectangle = new Rectangle(0, 0, rect.Width, rect.Height);
		}
		if (ps.hdc != IntPtr.Zero)
		{
			hwnd.drawing_stack.Push(ps);
		}
		else
		{
			hwnd.drawing_stack.Push(intPtr);
		}
		Graphics graphics = Graphics.FromHdc(intPtr);
		hwnd.drawing_stack.Push(graphics);
		return new PaintEventArgs(graphics, rectangle);
	}

	internal override void PaintEventEnd(ref Message m, IntPtr handle, bool client)
	{
		Hwnd hwnd = Hwnd.ObjectFromHandle(m.HWnd);
		Graphics graphics = (Graphics)hwnd.drawing_stack.Pop();
		graphics.Dispose();
		object obj = hwnd.drawing_stack.Pop();
		if (obj is IntPtr hDC)
		{
			Win32ReleaseDC(handle, hDC);
		}
		else if (obj is PAINTSTRUCT)
		{
			PAINTSTRUCT ps = (PAINTSTRUCT)obj;
			Win32EndPaint(handle, ref ps);
		}
	}

	internal override void SetWindowPos(IntPtr handle, int x, int y, int width, int height)
	{
		Win32MoveWindow(handle, x, y, width, height, repaint: true);
	}

	internal override void GetWindowPos(IntPtr handle, bool is_toplevel, out int x, out int y, out int width, out int height, out int client_width, out int client_height)
	{
		Win32GetWindowRect(handle, out var rect);
		width = rect.right - rect.left;
		height = rect.bottom - rect.top;
		POINT pt = default(POINT);
		pt.x = rect.left;
		pt.y = rect.top;
		IntPtr intPtr = Win32GetAncestor(handle, AncestorType.GA_PARENT);
		if (intPtr != IntPtr.Zero && intPtr != Win32GetDesktopWindow())
		{
			Win32ScreenToClient(intPtr, ref pt);
		}
		x = pt.x;
		y = pt.y;
		Win32GetClientRect(handle, out rect);
		client_width = rect.right - rect.left;
		client_height = rect.bottom - rect.top;
	}

	internal override void Activate(IntPtr handle)
	{
		Win32SetActiveWindow(handle);
		lock (timer_list)
		{
			foreach (Timer value in timer_list.Values)
			{
				if (value.Enabled && value.window == IntPtr.Zero)
				{
					value.window = handle;
					int hashCode = value.GetHashCode();
					Win32SetTimer(handle, hashCode, (uint)value.Interval, IntPtr.Zero);
				}
			}
		}
	}

	internal override void Invalidate(IntPtr handle, Rectangle rc, bool clear)
	{
		RECT lpRect = default(RECT);
		lpRect.left = rc.Left;
		lpRect.top = rc.Top;
		lpRect.right = rc.Right;
		lpRect.bottom = rc.Bottom;
		Win32InvalidateRect(handle, ref lpRect, clear);
	}

	internal override void InvalidateNC(IntPtr handle)
	{
		Win32SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.SWP_DRAWFRAME | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER);
	}

	private IntPtr InternalWndProc(IntPtr hWnd, Msg msg, IntPtr wParam, IntPtr lParam)
	{
		if (HwndCreating != null && HwndCreating.ClientWindow == IntPtr.Zero)
		{
			HwndCreating.ClientWindow = hWnd;
		}
		return NativeWindow.WndProc(hWnd, msg, wParam, lParam);
	}

	internal override IntPtr DefWndProc(ref Message msg)
	{
		msg.Result = Win32DefWindowProc(msg.HWnd, (Msg)msg.Msg, msg.WParam, msg.LParam);
		return msg.Result;
	}

	internal override void HandleException(Exception e)
	{
		StackTrace stackTrace = new StackTrace(e);
		Win32MessageBox(IntPtr.Zero, e.Message + stackTrace.ToString(), "Exception", 0u);
		Console.WriteLine("{0}{1}", e.Message, stackTrace.ToString());
	}

	internal override void DoEvents()
	{
		MSG msg = default(MSG);
		while (GetMessage(ref msg, IntPtr.Zero, 0, 0, blocking: false))
		{
			Message message = Message.Create(msg.hwnd, (int)msg.message, msg.wParam, msg.lParam);
			if (!Application.FilterMessage(ref message))
			{
				XplatUI.TranslateMessage(ref msg);
				XplatUI.DispatchMessage(ref msg);
			}
		}
	}

	internal override bool PeekMessage(object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax, uint flags)
	{
		return Win32PeekMessage(ref msg, hWnd, wFilterMin, wFilterMax, flags);
	}

	internal override void PostQuitMessage(int exitCode)
	{
		Win32PostQuitMessage(exitCode);
	}

	internal override void RequestAdditionalWM_NCMessages(IntPtr hwnd, bool hover, bool leave)
	{
		if (wm_nc_registered == null)
		{
			wm_nc_registered = new Hashtable();
		}
		TMEFlags tMEFlags = TMEFlags.TME_NONCLIENT;
		if (hover)
		{
			tMEFlags |= TMEFlags.TME_HOVER;
		}
		if (leave)
		{
			tMEFlags |= TMEFlags.TME_LEAVE;
		}
		if (tMEFlags == TMEFlags.TME_NONCLIENT)
		{
			if (wm_nc_registered.Contains(hwnd))
			{
				wm_nc_registered.Remove(hwnd);
			}
		}
		else if (!wm_nc_registered.Contains(hwnd))
		{
			wm_nc_registered.Add(hwnd, tMEFlags);
		}
		else
		{
			wm_nc_registered[hwnd] = tMEFlags;
		}
	}

	internal override void RequestNCRecalc(IntPtr handle)
	{
		Win32SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.SWP_DRAWFRAME | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER);
	}

	internal override void ResetMouseHover(IntPtr handle)
	{
		TRACKMOUSEEVENT tme = default(TRACKMOUSEEVENT);
		tme.size = Marshal.SizeOf(tme);
		tme.hWnd = handle;
		tme.dwFlags = TMEFlags.TME_HOVER | TMEFlags.TME_LEAVE;
		Win32TrackMouseEvent(ref tme);
	}

	internal override bool GetMessage(object queue_id, ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax)
	{
		return GetMessage(ref msg, hWnd, wFilterMin, wFilterMax, blocking: true);
	}

	private bool GetMessage(ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax, bool blocking)
	{
		msg.refobject = 0;
		if (RetrieveMessage(ref msg))
		{
			return true;
		}
		bool flag;
		if (blocking)
		{
			flag = Win32GetMessage(ref msg, hWnd, wFilterMin, wFilterMax);
		}
		else
		{
			flag = Win32PeekMessage(ref msg, hWnd, wFilterMin, wFilterMax, 1u);
			if (!flag)
			{
				return false;
			}
		}
		switch (msg.message)
		{
		case Msg.WM_LBUTTONDOWN:
			mouse_state |= MouseButtons.Left;
			break;
		case Msg.WM_MBUTTONDOWN:
			mouse_state |= MouseButtons.Middle;
			break;
		case Msg.WM_RBUTTONDOWN:
			mouse_state |= MouseButtons.Right;
			break;
		case Msg.WM_LBUTTONUP:
			mouse_state &= ~MouseButtons.Left;
			break;
		case Msg.WM_MBUTTONUP:
			mouse_state &= ~MouseButtons.Middle;
			break;
		case Msg.WM_RBUTTONUP:
			mouse_state &= ~MouseButtons.Right;
			break;
		case Msg.WM_ASYNC_MESSAGE:
			XplatUIDriverSupport.ExecuteClientMessage((GCHandle)msg.lParam);
			break;
		case Msg.WM_MOUSEMOVE:
			if (msg.hwnd != prev_mouse_hwnd)
			{
				mouse_state = Control.FromParamToMouseButtons(msg.lParam.ToInt32());
				StoreMessage(ref msg);
				msg.message = Msg.WM_MOUSE_ENTER;
				prev_mouse_hwnd = msg.hwnd;
				TRACKMOUSEEVENT tme2 = default(TRACKMOUSEEVENT);
				tme2.size = Marshal.SizeOf(tme2);
				tme2.hWnd = msg.hwnd;
				tme2.dwFlags = TMEFlags.TME_HOVER | TMEFlags.TME_LEAVE;
				Win32TrackMouseEvent(ref tme2);
				return flag;
			}
			break;
		case Msg.WM_NCMOUSEMOVE:
		{
			if (wm_nc_registered == null || !wm_nc_registered.Contains(msg.hwnd))
			{
				break;
			}
			mouse_state = Control.FromParamToMouseButtons(msg.lParam.ToInt32());
			TRACKMOUSEEVENT tme = default(TRACKMOUSEEVENT);
			tme.size = Marshal.SizeOf(tme);
			tme.hWnd = msg.hwnd;
			tme.dwFlags = (TMEFlags)(int)wm_nc_registered[msg.hwnd];
			Win32TrackMouseEvent(ref tme);
			return flag;
		}
		case Msg.WM_DROPFILES:
			return Win32DnD.HandleWMDropFiles(ref msg);
		case Msg.WM_MOUSELEAVE:
			prev_mouse_hwnd = IntPtr.Zero;
			break;
		case Msg.WM_TIMER:
			((Timer)timer_list[(int)msg.wParam])?.FireTick();
			break;
		}
		return flag;
	}

	internal override bool TranslateMessage(ref MSG msg)
	{
		return Win32TranslateMessage(ref msg);
	}

	internal override IntPtr DispatchMessage(ref MSG msg)
	{
		return Win32DispatchMessage(ref msg);
	}

	internal override bool SetZOrder(IntPtr hWnd, IntPtr AfterhWnd, bool Top, bool Bottom)
	{
		if (Top)
		{
			Win32SetWindowPos(hWnd, SetWindowPosZOrder.HWND_TOP, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
			return true;
		}
		if (!Bottom)
		{
			Win32SetWindowPos(hWnd, AfterhWnd, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
			return false;
		}
		Win32SetWindowPos(hWnd, (IntPtr)1, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
		return true;
	}

	internal override bool SetTopmost(IntPtr hWnd, bool Enabled)
	{
		if (Enabled)
		{
			Win32SetWindowPos(hWnd, SetWindowPosZOrder.HWND_TOPMOST, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
			return true;
		}
		Win32SetWindowPos(hWnd, SetWindowPosZOrder.HWND_NOTOPMOST, 0, 0, 0, 0, SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
		return true;
	}

	internal override bool SetOwner(IntPtr hWnd, IntPtr hWndOwner)
	{
		Win32SetWindowLong(hWnd, WindowLong.GWL_HWNDPARENT, (uint)(int)hWndOwner);
		return true;
	}

	internal override bool Text(IntPtr handle, string text)
	{
		Win32SetWindowText(handle, text);
		return true;
	}

	internal override bool GetText(IntPtr handle, out string text)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		Win32GetWindowText(handle, stringBuilder, stringBuilder.Capacity);
		text = stringBuilder.ToString();
		return true;
	}

	internal override bool SetVisible(IntPtr handle, bool visible, bool activate)
	{
		if (visible)
		{
			Control control = Control.FromHandle(handle);
			if (control is Form)
			{
				Form form = (Form)Control.FromHandle(handle);
				WindowPlacementFlags nCmdShow = WindowPlacementFlags.SW_SHOWNORMAL;
				switch (form.WindowState)
				{
				case FormWindowState.Normal:
					nCmdShow = WindowPlacementFlags.SW_SHOWNORMAL;
					break;
				case FormWindowState.Minimized:
					nCmdShow = WindowPlacementFlags.SW_MINIMIZE;
					break;
				case FormWindowState.Maximized:
					nCmdShow = WindowPlacementFlags.SW_SHOWMAXIMIZED;
					break;
				}
				if (!form.ActivateOnShow)
				{
					nCmdShow = WindowPlacementFlags.SW_SHOWNOACTIVATE;
				}
				Win32ShowWindow(handle, nCmdShow);
			}
			else if (control.ActivateOnShow)
			{
				Win32ShowWindow(handle, WindowPlacementFlags.SW_SHOWNORMAL);
			}
			else
			{
				Win32ShowWindow(handle, WindowPlacementFlags.SW_SHOWNOACTIVATE);
			}
		}
		else
		{
			Win32ShowWindow(handle, WindowPlacementFlags.SW_HIDE);
		}
		return true;
	}

	internal override bool IsEnabled(IntPtr handle)
	{
		return IsWindowEnabled(handle);
	}

	internal override bool IsKeyLocked(VirtualKeys key)
	{
		return (Win32GetKeyState(key) & 1) == 1;
	}

	internal override bool IsVisible(IntPtr handle)
	{
		return IsWindowVisible(handle);
	}

	internal override IntPtr SetParent(IntPtr handle, IntPtr parent)
	{
		Control control = Control.FromHandle(handle);
		if (parent == IntPtr.Zero)
		{
			if (!(control is Form))
			{
				Win32ShowWindow(handle, WindowPlacementFlags.SW_HIDE);
			}
		}
		else if (!(control is Form))
		{
			SetVisible(handle, control.is_visible, activate: true);
		}
		Win32GetWindowRect(handle, out var rect);
		WindowStyles windowStyles = (WindowStyles)Win32GetWindowLong(handle, WindowLong.GWL_STYLE);
		WindowStyles windowStyles2;
		IntPtr result;
		if (parent == IntPtr.Zero)
		{
			windowStyles2 = windowStyles & ~WindowStyles.WS_CHILD;
			result = Win32SetParent(handle, FosterParent);
		}
		else
		{
			windowStyles2 = windowStyles | WindowStyles.WS_CHILD;
			result = Win32SetParent(handle, parent);
		}
		if (windowStyles != windowStyles2 && control is Form)
		{
			Win32SetWindowLong(handle, WindowLong.GWL_STYLE, (uint)windowStyles2);
		}
		Win32GetWindowRect(handle, out var rect2);
		if (rect.top != rect2.top && rect.left != rect2.left && control is Form)
		{
			Win32SetWindowPos(handle, IntPtr.Zero, rect.top, rect.left, rect.Width, rect.Height, SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOENDSCHANGING | SetWindowPosFlags.SWP_NOZORDER);
		}
		return result;
	}

	internal override IntPtr GetParent(IntPtr handle)
	{
		return Win32GetParent(handle);
	}

	internal override IntPtr GetPreviousWindow(IntPtr handle)
	{
		return handle;
	}

	internal override void GrabWindow(IntPtr hWnd, IntPtr ConfineToHwnd)
	{
		grab_hwnd = hWnd;
		Win32SetCapture(hWnd);
		if (ConfineToHwnd != IntPtr.Zero)
		{
			Win32GetWindowRect(ConfineToHwnd, out var rect);
			Win32GetClipCursor(out clipped_cursor_rect);
			Win32ClipCursor(ref rect);
		}
	}

	internal override void GrabInfo(out IntPtr hWnd, out bool GrabConfined, out Rectangle GrabArea)
	{
		hWnd = grab_hwnd;
		GrabConfined = grab_confined;
		GrabArea = grab_area;
	}

	internal override void UngrabWindow(IntPtr hWnd)
	{
		if (clipped_cursor_rect.top != 0 || clipped_cursor_rect.bottom != 0 || clipped_cursor_rect.left != 0 || clipped_cursor_rect.right != 0)
		{
			Win32ClipCursor(ref clipped_cursor_rect);
			clipped_cursor_rect = default(RECT);
		}
		Win32ReleaseCapture();
		grab_hwnd = IntPtr.Zero;
	}

	internal override bool CalculateWindowRect(ref Rectangle ClientRect, CreateParams cp, Menu menu, out Rectangle WindowRect)
	{
		RECT lpRect = default(RECT);
		lpRect.left = ClientRect.Left;
		lpRect.top = ClientRect.Top;
		lpRect.right = ClientRect.Right;
		lpRect.bottom = ClientRect.Bottom;
		if (!Win32AdjustWindowRectEx(ref lpRect, cp.Style, menu != null, cp.ExStyle))
		{
			WindowRect = new Rectangle(ClientRect.Left, ClientRect.Top, ClientRect.Width, ClientRect.Height);
			return false;
		}
		WindowRect = new Rectangle(lpRect.left, lpRect.top, lpRect.right - lpRect.left, lpRect.bottom - lpRect.top);
		return true;
	}

	internal override void SetCursor(IntPtr window, IntPtr cursor)
	{
		Win32SetCursor(cursor);
	}

	internal override void ShowCursor(bool show)
	{
		Win32ShowCursor(show);
	}

	internal override void OverrideCursor(IntPtr cursor)
	{
		Win32SetCursor(cursor);
	}

	internal override IntPtr DefineCursor(Bitmap bitmap, Bitmap mask, Color cursor_pixel, Color mask_pixel, int xHotSpot, int yHotSpot)
	{
		Bitmap bitmap2;
		Bitmap bitmap3;
		if (bitmap.Width != Win32GetSystemMetrics(SystemMetrics.SM_CXCURSOR) || bitmap.Width != Win32GetSystemMetrics(SystemMetrics.SM_CXCURSOR))
		{
			bitmap2 = new Bitmap(bitmap, new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXCURSOR), Win32GetSystemMetrics(SystemMetrics.SM_CXCURSOR)));
			bitmap3 = new Bitmap(mask, new Size(Win32GetSystemMetrics(SystemMetrics.SM_CXCURSOR), Win32GetSystemMetrics(SystemMetrics.SM_CXCURSOR)));
		}
		else
		{
			bitmap2 = bitmap;
			bitmap3 = mask;
		}
		int width = bitmap2.Width;
		int height = bitmap2.Height;
		byte[] array = new byte[width / 8 * height];
		byte[] array2 = new byte[width / 8 * height];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				Color pixel = bitmap2.GetPixel(j, i);
				if (pixel == cursor_pixel)
				{
					array[i * width / 8 + j / 8] |= (byte)(128 >> j % 8);
				}
				pixel = bitmap3.GetPixel(j, i);
				if (pixel == mask_pixel)
				{
					array2[i * width / 8 + j / 8] |= (byte)(128 >> j % 8);
				}
			}
		}
		return Win32CreateCursor(IntPtr.Zero, xHotSpot, yHotSpot, width, height, array2, array);
	}

	internal override Bitmap DefineStdCursorBitmap(StdCursor id)
	{
		IntPtr hIcon = DefineStdCursor(id);
		int width = Win32GetSystemMetrics(SystemMetrics.SM_CXCURSOR);
		int height = Win32GetSystemMetrics(SystemMetrics.SM_CYCURSOR);
		Bitmap bitmap = new Bitmap(width, height);
		Graphics graphics = Graphics.FromImage(bitmap);
		IntPtr hdc = graphics.GetHdc();
		Win32DrawIcon(hdc, 0, 0, hIcon);
		graphics.ReleaseHdc(hdc);
		graphics.Dispose();
		return bitmap;
	}

	[System.MonoTODO("Define the missing cursors")]
	internal override IntPtr DefineStdCursor(StdCursor id)
	{
		return id switch
		{
			StdCursor.AppStarting => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_APPSTARTING), 
			StdCursor.Arrow => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.Cross => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_CROSS), 
			StdCursor.Default => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.Hand => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_HAND), 
			StdCursor.Help => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_HELP), 
			StdCursor.HSplit => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.IBeam => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_IBEAM), 
			StdCursor.No => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_NO), 
			StdCursor.NoMove2D => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.NoMoveHoriz => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.NoMoveVert => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.PanEast => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.PanNE => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.PanNorth => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.PanNW => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.PanSE => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.PanSouth => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.PanSW => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.PanWest => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.SizeAll => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_SIZEALL), 
			StdCursor.SizeNESW => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_SIZENESW), 
			StdCursor.SizeNS => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_SIZENS), 
			StdCursor.SizeNWSE => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_SIZENWSE), 
			StdCursor.SizeWE => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_SIZEWE), 
			StdCursor.UpArrow => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_UPARROW), 
			StdCursor.VSplit => Win32LoadCursor(IntPtr.Zero, LoadCursorType.First), 
			StdCursor.WaitCursor => Win32LoadCursor(IntPtr.Zero, LoadCursorType.IDC_WAIT), 
			_ => throw new NotImplementedException(), 
		};
	}

	internal override void DestroyCursor(IntPtr cursor)
	{
		if (cursor.ToInt32() < 32512 || cursor.ToInt32() > 32651)
		{
			Win32DestroyCursor(cursor);
		}
	}

	[System.MonoTODO]
	internal override void GetCursorInfo(IntPtr cursor, out int width, out int height, out int hotspot_x, out int hotspot_y)
	{
		ICONINFO piconinfo = default(ICONINFO);
		if (!Win32GetIconInfo(cursor, out piconinfo))
		{
			throw new Win32Exception();
		}
		width = 20;
		height = 20;
		hotspot_x = piconinfo.xHotspot;
		hotspot_y = piconinfo.yHotspot;
	}

	internal override void SetCursorPos(IntPtr handle, int x, int y)
	{
		Win32SetCursorPos(x, y);
	}

	internal override Region GetClipRegion(IntPtr hwnd)
	{
		Region region = new Region();
		Win32GetWindowRgn(hwnd, region.GetHrgn(Graphics.FromHwnd(hwnd)));
		return region;
	}

	internal override void SetClipRegion(IntPtr hwnd, Region region)
	{
		if (region == null)
		{
			Win32SetWindowRgn(hwnd, IntPtr.Zero, redraw: true);
		}
		else
		{
			Win32SetWindowRgn(hwnd, region.GetHrgn(Graphics.FromHwnd(hwnd)), redraw: true);
		}
	}

	internal override void EnableWindow(IntPtr handle, bool Enable)
	{
		Win32EnableWindow(handle, Enable);
	}

	internal override void EndLoop(Thread thread)
	{
	}

	internal override object StartLoop(Thread thread)
	{
		return null;
	}

	internal override void SetModal(IntPtr handle, bool Modal)
	{
	}

	internal override void GetCursorPos(IntPtr handle, out int x, out int y)
	{
		Win32GetCursorPos(out var lpPoint);
		if (handle != IntPtr.Zero)
		{
			Win32ScreenToClient(handle, ref lpPoint);
		}
		x = lpPoint.x;
		y = lpPoint.y;
	}

	internal override void ScreenToClient(IntPtr handle, ref int x, ref int y)
	{
		POINT pt = default(POINT);
		pt.x = x;
		pt.y = y;
		Win32ScreenToClient(handle, ref pt);
		x = pt.x;
		y = pt.y;
	}

	internal override void ClientToScreen(IntPtr handle, ref int x, ref int y)
	{
		POINT pt = default(POINT);
		pt.x = x;
		pt.y = y;
		Win32ClientToScreen(handle, ref pt);
		x = pt.x;
		y = pt.y;
	}

	internal override void ScreenToMenu(IntPtr handle, ref int x, ref int y)
	{
		Win32GetWindowRect(handle, out var rect);
		x -= rect.left + SystemInformation.FrameBorderSize.Width;
		y -= rect.top + SystemInformation.FrameBorderSize.Height;
		WindowStyles style = (WindowStyles)Win32GetWindowLong(handle, WindowLong.GWL_STYLE);
		if (CreateParams.IsSet(style, WindowStyles.WS_CAPTION))
		{
			y -= ThemeEngine.Current.CaptionHeight;
		}
	}

	internal override void MenuToScreen(IntPtr handle, ref int x, ref int y)
	{
		Win32GetWindowRect(handle, out var rect);
		x += rect.left + SystemInformation.FrameBorderSize.Width;
		y += rect.top + SystemInformation.FrameBorderSize.Height + ThemeEngine.Current.CaptionHeight;
	}

	internal override void SendAsyncMethod(AsyncMethodData method)
	{
		Win32PostMessage(FosterParent, Msg.WM_ASYNC_MESSAGE, IntPtr.Zero, (IntPtr)GCHandle.Alloc(method));
	}

	internal override void SetTimer(Timer timer)
	{
		int hashCode = timer.GetHashCode();
		lock (timer_list)
		{
			timer_list[hashCode] = timer;
		}
		if (Win32SetTimer(FosterParent, hashCode, (uint)timer.Interval, IntPtr.Zero) != IntPtr.Zero)
		{
			timer.window = FosterParent;
		}
		else
		{
			timer.window = IntPtr.Zero;
		}
	}

	internal override void KillTimer(Timer timer)
	{
		int hashCode = timer.GetHashCode();
		Win32KillTimer(timer.window, hashCode);
		lock (timer_list)
		{
			timer_list.Remove(hashCode);
		}
	}

	internal override void CreateCaret(IntPtr hwnd, int width, int height)
	{
		Win32CreateCaret(hwnd, IntPtr.Zero, width, height);
		caret_visible = false;
	}

	internal override void DestroyCaret(IntPtr hwnd)
	{
		Win32DestroyCaret();
	}

	internal override void SetCaretPos(IntPtr hwnd, int x, int y)
	{
		Win32SetCaretPos(x, y);
	}

	internal override void CaretVisible(IntPtr hwnd, bool visible)
	{
		if (visible)
		{
			if (!caret_visible)
			{
				Win32ShowCaret(hwnd);
				caret_visible = true;
			}
		}
		else if (caret_visible)
		{
			Win32HideCaret(hwnd);
			caret_visible = false;
		}
	}

	internal override IntPtr GetFocus()
	{
		return Win32GetFocus();
	}

	internal override void SetFocus(IntPtr hwnd)
	{
		Win32SetFocus(hwnd);
	}

	internal override IntPtr GetActive()
	{
		return Win32GetActiveWindow();
	}

	internal override bool GetFontMetrics(Graphics g, Font font, out int ascent, out int descent)
	{
		TEXTMETRIC tm = default(TEXTMETRIC);
		IntPtr intPtr = Win32GetDC(IntPtr.Zero);
		IntPtr hgdiobject = Win32SelectObject(intPtr, font.ToHfont());
		if (!Win32GetTextMetrics(intPtr, ref tm))
		{
			hgdiobject = Win32SelectObject(intPtr, hgdiobject);
			Win32DeleteObject(hgdiobject);
			Win32ReleaseDC(IntPtr.Zero, intPtr);
			ascent = 0;
			descent = 0;
			return false;
		}
		hgdiobject = Win32SelectObject(intPtr, hgdiobject);
		Win32DeleteObject(hgdiobject);
		Win32ReleaseDC(IntPtr.Zero, intPtr);
		ascent = tm.tmAscent;
		descent = tm.tmDescent;
		return true;
	}

	internal override void ScrollWindow(IntPtr hwnd, Rectangle rectangle, int XAmount, int YAmount, bool with_children)
	{
		RECT prcClip = default(RECT);
		prcClip.left = rectangle.X;
		prcClip.top = rectangle.Y;
		prcClip.right = rectangle.Right;
		prcClip.bottom = rectangle.Bottom;
		Win32ScrollWindowEx(hwnd, XAmount, YAmount, IntPtr.Zero, ref prcClip, IntPtr.Zero, IntPtr.Zero, ScrollWindowExFlags.SW_INVALIDATE | ScrollWindowExFlags.SW_ERASE | (with_children ? ScrollWindowExFlags.SW_SCROLLCHILDREN : ScrollWindowExFlags.SW_NONE));
		Win32UpdateWindow(hwnd);
	}

	internal override void ScrollWindow(IntPtr hwnd, int XAmount, int YAmount, bool with_children)
	{
		Win32ScrollWindowEx(hwnd, XAmount, YAmount, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ScrollWindowExFlags.SW_INVALIDATE | ScrollWindowExFlags.SW_ERASE | (with_children ? ScrollWindowExFlags.SW_SCROLLCHILDREN : ScrollWindowExFlags.SW_NONE));
	}

	internal override bool SystrayAdd(IntPtr hwnd, string tip, Icon icon, out ToolTip tt)
	{
		NOTIFYICONDATA lpData = default(NOTIFYICONDATA);
		lpData.cbSize = (uint)Marshal.SizeOf(lpData);
		lpData.hWnd = hwnd;
		lpData.uID = 1u;
		lpData.uCallbackMessage = 1024u;
		lpData.uFlags = NotifyIconFlags.NIF_MESSAGE;
		if (tip != null)
		{
			lpData.szTip = tip;
			lpData.uFlags |= NotifyIconFlags.NIF_TIP;
		}
		if (icon != null)
		{
			lpData.hIcon = icon.Handle;
			lpData.uFlags |= NotifyIconFlags.NIF_ICON;
		}
		tt = null;
		return Win32Shell_NotifyIcon(NotifyIconMessage.NIM_ADD, ref lpData);
	}

	internal override bool SystrayChange(IntPtr hwnd, string tip, Icon icon, ref ToolTip tt)
	{
		NOTIFYICONDATA lpData = default(NOTIFYICONDATA);
		lpData.cbSize = (uint)Marshal.SizeOf(lpData);
		lpData.hIcon = icon.Handle;
		lpData.hWnd = hwnd;
		lpData.uID = 1u;
		lpData.uCallbackMessage = 1024u;
		lpData.uFlags = NotifyIconFlags.NIF_MESSAGE;
		if (tip != null)
		{
			lpData.szTip = tip;
			lpData.uFlags |= NotifyIconFlags.NIF_TIP;
		}
		if (icon != null)
		{
			lpData.hIcon = icon.Handle;
			lpData.uFlags |= NotifyIconFlags.NIF_ICON;
		}
		return Win32Shell_NotifyIcon(NotifyIconMessage.NIM_MODIFY, ref lpData);
	}

	internal override void SystrayRemove(IntPtr hwnd, ref ToolTip tt)
	{
		NOTIFYICONDATA lpData = default(NOTIFYICONDATA);
		lpData.cbSize = (uint)Marshal.SizeOf(lpData);
		lpData.hWnd = hwnd;
		lpData.uID = 1u;
		lpData.uFlags = (NotifyIconFlags)0;
		Win32Shell_NotifyIcon(NotifyIconMessage.NIM_DELETE, ref lpData);
	}

	internal override void SystrayBalloon(IntPtr hwnd, int timeout, string title, string text, ToolTipIcon icon)
	{
		NOTIFYICONDATA lpData = default(NOTIFYICONDATA);
		lpData.cbSize = (uint)Marshal.SizeOf(lpData);
		lpData.hWnd = hwnd;
		lpData.uID = 1u;
		lpData.uFlags = NotifyIconFlags.NIF_INFO;
		lpData.uTimeoutOrVersion = timeout;
		lpData.szInfoTitle = title;
		lpData.szInfo = text;
		lpData.dwInfoFlags = icon;
		Win32Shell_NotifyIcon(NotifyIconMessage.NIM_MODIFY, ref lpData);
	}

	internal override void SetBorderStyle(IntPtr handle, FormBorderStyle border_style)
	{
	}

	internal override void SetMenu(IntPtr handle, Menu menu)
	{
		Win32SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.SWP_DRAWFRAME | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
	}

	internal override Point GetMenuOrigin(IntPtr handle)
	{
		if (Control.FromHandle(handle) is Form form)
		{
			if (form.FormBorderStyle == FormBorderStyle.None)
			{
				return Point.Empty;
			}
			int num = (form.Width - form.ClientSize.Width) / 2;
			if (form.FormBorderStyle == FormBorderStyle.FixedToolWindow || form.FormBorderStyle == FormBorderStyle.SizableToolWindow)
			{
				return new Point(num, num + SystemInformation.ToolWindowCaptionHeight);
			}
			return new Point(num, num + SystemInformation.CaptionHeight);
		}
		return new Point(SystemInformation.FrameBorderSize.Width, SystemInformation.FrameBorderSize.Height + ThemeEngine.Current.CaptionHeight);
	}

	internal override void SetIcon(IntPtr hwnd, Icon icon)
	{
		Win32SendMessage(hwnd, Msg.WM_SETICON, (IntPtr)1, icon?.Handle ?? IntPtr.Zero);
	}

	internal override void ClipboardClose(IntPtr handle)
	{
		if (handle != clip_magic)
		{
			throw new ArgumentException("handle is not a valid clipboard handle");
		}
		Win32CloseClipboard();
	}

	internal override int ClipboardGetID(IntPtr handle, string format)
	{
		if (handle != clip_magic)
		{
			throw new ArgumentException("handle is not a valid clipboard handle");
		}
		return format switch
		{
			"Text" => 1, 
			"Bitmap" => 2, 
			"MetaFilePict" => 3, 
			"SymbolicLink" => 4, 
			"DataInterchangeFormat" => 5, 
			"Tiff" => 6, 
			"OEMText" => 7, 
			"DeviceIndependentBitmap" => 8, 
			"Palette" => 9, 
			"PenData" => 10, 
			"RiffAudio" => 11, 
			"WaveAudio" => 12, 
			"UnicodeText" => 13, 
			"EnhancedMetafile" => 14, 
			"FileDrop" => 15, 
			"Locale" => 16, 
			_ => (int)Win32RegisterClipboardFormat(format), 
		};
	}

	internal override IntPtr ClipboardOpen(bool primary_selection)
	{
		Win32OpenClipboard(FosterParent);
		return clip_magic;
	}

	internal override int[] ClipboardAvailableFormats(IntPtr handle)
	{
		if (handle != clip_magic)
		{
			return null;
		}
		int num = 0;
		uint num2 = 0u;
		do
		{
			num2 = Win32EnumClipboardFormats(num2);
			if (num2 != 0)
			{
				num++;
			}
		}
		while (num2 != 0);
		int[] array = new int[num];
		num = 0;
		num2 = 0u;
		do
		{
			num2 = Win32EnumClipboardFormats(num2);
			if (num2 != 0)
			{
				array[num++] = (int)num2;
			}
		}
		while (num2 != 0);
		return array;
	}

	internal override object ClipboardRetrieve(IntPtr handle, int type, XplatUI.ClipboardToObject converter)
	{
		if (handle != clip_magic)
		{
			throw new ArgumentException("handle is not a valid clipboard handle");
		}
		IntPtr intPtr = Win32GetClipboardData((uint)type);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		IntPtr intPtr2 = Win32GlobalLock(intPtr);
		if (intPtr2 == IntPtr.Zero)
		{
			uint num = Win32GetLastError();
			Console.WriteLine("Error: {0}", num);
			return null;
		}
		object obj = null;
		if (type == DataFormats.GetFormat(DataFormats.Rtf).Id)
		{
			obj = AnsiToString(intPtr2);
		}
		else
		{
			switch ((ClipboardFormats)(ushort)type)
			{
			case ClipboardFormats.CF_TEXT:
				obj = AnsiToString(intPtr2);
				break;
			case ClipboardFormats.CF_DIB:
				obj = DIBtoImage(intPtr2);
				break;
			case ClipboardFormats.CF_UNICODETEXT:
				obj = UnicodeToString(intPtr2);
				break;
			default:
				if (converter != null && !converter(type, intPtr2, out obj))
				{
					obj = null;
				}
				break;
			}
		}
		Win32GlobalUnlock(intPtr);
		return obj;
	}

	internal override void ClipboardStore(IntPtr handle, object obj, int type, XplatUI.ObjectToClipboard converter)
	{
		byte[] data = null;
		if (handle != clip_magic)
		{
			throw new ArgumentException("handle is not a valid clipboard handle");
		}
		if (obj == null)
		{
			if (!Win32EmptyClipboard())
			{
				throw new ExternalException("Win32EmptyClipboard");
			}
			return;
		}
		if (type == -1)
		{
			if (obj is string)
			{
				type = 13;
			}
			else if (obj is Image)
			{
				type = 8;
			}
		}
		if (type == DataFormats.GetFormat(DataFormats.Rtf).Id)
		{
			data = StringToAnsi((string)obj);
		}
		else
		{
			switch ((ClipboardFormats)(ushort)type)
			{
			case ClipboardFormats.CF_UNICODETEXT:
				data = StringToUnicode((string)obj);
				break;
			case ClipboardFormats.CF_TEXT:
				data = StringToAnsi((string)obj);
				break;
			case ClipboardFormats.CF_BITMAP:
			case ClipboardFormats.CF_DIB:
				data = ImageToDIB((Image)obj);
				type = 8;
				break;
			default:
				if (converter != null && !converter(ref type, obj, out data))
				{
					data = null;
				}
				break;
			}
		}
		if (data != null)
		{
			SetClipboardData((uint)type, data);
		}
	}

	internal static byte[] StringToUnicode(string text)
	{
		return Encoding.Unicode.GetBytes(text + "\0");
	}

	internal static byte[] StringToAnsi(string text)
	{
		return Encoding.UTF8.GetBytes(text + "\0");
	}

	private void SetClipboardData(uint type, byte[] data)
	{
		if (data.Length != 0)
		{
			IntPtr intPtr = CopyToMoveableMemory(data);
			if (intPtr == IntPtr.Zero)
			{
				throw new ExternalException("CopyToMoveableMemory failed.");
			}
			if (Win32SetClipboardData(type, intPtr) == IntPtr.Zero)
			{
				throw new ExternalException("Win32SetClipboardData");
			}
		}
	}

	internal static IntPtr CopyToMoveableMemory(byte[] data)
	{
		if (data == null || data.Length == 0)
		{
			throw new ArgumentException("Can't create a zero length memory block.");
		}
		IntPtr intPtr = Win32GlobalAlloc(GAllocFlags.GMEM_MOVEABLE | GAllocFlags.GMEM_SHARE, data.Length);
		if (intPtr == IntPtr.Zero)
		{
			throw new Win32Exception();
		}
		IntPtr intPtr2 = Win32GlobalLock(intPtr);
		if (intPtr2 == IntPtr.Zero)
		{
			throw new Win32Exception();
		}
		Marshal.Copy(data, 0, intPtr2, data.Length);
		Win32GlobalUnlock(intPtr);
		return intPtr;
	}

	internal override void SetAllowDrop(IntPtr hwnd, bool allowed)
	{
		if (allowed)
		{
			Win32DnD.RegisterDropTarget(hwnd);
		}
		else
		{
			Win32DnD.UnregisterDropTarget(hwnd);
		}
	}

	internal override DragDropEffects StartDrag(IntPtr hwnd, object data, DragDropEffects allowedEffects)
	{
		return Win32DnD.StartDrag(hwnd, data, allowedEffects);
	}

	internal override void DrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style)
	{
		COLORREF color = default(COLORREF);
		color.R = backColor.R;
		color.G = backColor.G;
		color.B = backColor.B;
		IntPtr intPtr = Win32CreatePen((style != FrameStyle.Thick) ? PenStyle.PS_DASH : PenStyle.PS_SOLID, (style != FrameStyle.Thick) ? 2 : 4, ref color);
		IntPtr intPtr2 = Win32GetDC(IntPtr.Zero);
		Win32SetROP2(intPtr2, ROP2DrawMode.R2_NOT);
		IntPtr hgdiobject = Win32SelectObject(intPtr2, intPtr);
		Win32MoveToEx(intPtr2, rectangle.Left, rectangle.Top, IntPtr.Zero);
		if (rectangle.Width > 0 && rectangle.Height > 0)
		{
			Win32LineTo(intPtr2, rectangle.Right, rectangle.Top);
			Win32LineTo(intPtr2, rectangle.Right, rectangle.Bottom);
			Win32LineTo(intPtr2, rectangle.Left, rectangle.Bottom);
			Win32LineTo(intPtr2, rectangle.Left, rectangle.Top);
		}
		else if (rectangle.Width > 0)
		{
			Win32LineTo(intPtr2, rectangle.Right, rectangle.Top);
		}
		else
		{
			Win32LineTo(intPtr2, rectangle.Left, rectangle.Bottom);
		}
		Win32SelectObject(intPtr2, hgdiobject);
		Win32DeleteObject(intPtr);
		Win32ReleaseDC(IntPtr.Zero, intPtr2);
	}

	internal override void DrawReversibleLine(Point start, Point end, Color backColor)
	{
		COLORREF color = default(COLORREF);
		POINT pt = default(POINT);
		pt.x = 0;
		pt.y = 0;
		Win32ClientToScreen(IntPtr.Zero, ref pt);
		color.R = backColor.R;
		color.G = backColor.G;
		color.B = backColor.B;
		IntPtr intPtr = Win32CreatePen(PenStyle.PS_SOLID, 1, ref color);
		IntPtr intPtr2 = Win32GetDC(IntPtr.Zero);
		Win32SetROP2(intPtr2, ROP2DrawMode.R2_NOT);
		IntPtr hgdiobject = Win32SelectObject(intPtr2, intPtr);
		Win32MoveToEx(intPtr2, pt.x + start.X, pt.y + start.Y, IntPtr.Zero);
		Win32LineTo(intPtr2, pt.x + end.X, pt.y + end.Y);
		Win32SelectObject(intPtr2, hgdiobject);
		Win32DeleteObject(intPtr);
		Win32ReleaseDC(IntPtr.Zero, intPtr2);
	}

	internal override void FillReversibleRectangle(Rectangle rectangle, Color backColor)
	{
		RECT rECT = default(RECT);
		rECT.left = rectangle.Left;
		rECT.top = rectangle.Top;
		rECT.right = rectangle.Right;
		rECT.bottom = rectangle.Bottom;
		COLORREF clrRef = default(COLORREF);
		clrRef.R = backColor.R;
		clrRef.G = backColor.G;
		clrRef.B = backColor.B;
		IntPtr intPtr = Win32CreateSolidBrush(clrRef);
		IntPtr intPtr2 = Win32GetDC(IntPtr.Zero);
		IntPtr hgdiobject = Win32SelectObject(intPtr2, intPtr);
		Win32PatBlt(intPtr2, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, PatBltRop.DSTINVERT);
		Win32SelectObject(intPtr2, hgdiobject);
		Win32DeleteObject(intPtr);
		Win32ReleaseDC(IntPtr.Zero, intPtr2);
	}

	internal override void DrawReversibleRectangle(IntPtr handle, Rectangle rect, int line_width)
	{
		POINT pt = default(POINT);
		pt.x = 0;
		pt.y = 0;
		Win32ClientToScreen(handle, ref pt);
		IntPtr intPtr = Win32CreatePen(PenStyle.PS_SOLID, line_width, IntPtr.Zero);
		IntPtr intPtr2 = Win32GetDC(IntPtr.Zero);
		Win32SetROP2(intPtr2, ROP2DrawMode.R2_NOT);
		IntPtr hgdiobject = Win32SelectObject(intPtr2, intPtr);
		Control control = Control.FromHandle(handle);
		if (control != null)
		{
			Win32GetWindowRect(control.Handle, out var rect2);
			Region region = new Region(new Rectangle(rect2.left, rect2.top, rect2.right - rect2.left, rect2.bottom - rect2.top));
			Win32ExtSelectClipRgn(intPtr2, region.GetHrgn(Graphics.FromHdc(intPtr2)), 1);
		}
		Win32MoveToEx(intPtr2, pt.x + rect.Left, pt.y + rect.Top, IntPtr.Zero);
		if (rect.Width > 0 && rect.Height > 0)
		{
			Win32LineTo(intPtr2, pt.x + rect.Right, pt.y + rect.Top);
			Win32LineTo(intPtr2, pt.x + rect.Right, pt.y + rect.Bottom);
			Win32LineTo(intPtr2, pt.x + rect.Left, pt.y + rect.Bottom);
			Win32LineTo(intPtr2, pt.x + rect.Left, pt.y + rect.Top);
		}
		else if (rect.Width > 0)
		{
			Win32LineTo(intPtr2, pt.x + rect.Right, pt.y + rect.Top);
		}
		else
		{
			Win32LineTo(intPtr2, pt.x + rect.Left, pt.y + rect.Bottom);
		}
		Win32SelectObject(intPtr2, hgdiobject);
		Win32DeleteObject(intPtr);
		if (control != null)
		{
			Win32ExtSelectClipRgn(intPtr2, IntPtr.Zero, 5);
		}
		Win32ReleaseDC(IntPtr.Zero, intPtr2);
	}

	internal override SizeF GetAutoScaleSize(Font font)
	{
		string text = "The quick brown fox jumped over the lazy dog.";
		double num = 44.54999694824219;
		Graphics graphics = Graphics.FromHwnd(FosterParent);
		float width = (float)((double)graphics.MeasureString(text, font).Width / num);
		return new SizeF(width, font.Height);
	}

	internal override IntPtr SendMessage(IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam)
	{
		return Win32SendMessage(hwnd, message, wParam, lParam);
	}

	internal override bool PostMessage(IntPtr hwnd, Msg message, IntPtr wParam, IntPtr lParam)
	{
		return Win32PostMessage(hwnd, message, wParam, lParam);
	}

	internal override int SendInput(IntPtr hwnd, Queue keys)
	{
		INPUT[] array = new INPUT[keys.Count];
		uint num = 0u;
		int num2 = 0;
		while (keys.Count > 0)
		{
			MSG mSG = (MSG)keys.Dequeue();
			array[num2].ki.wScan = 0;
			array[num2].ki.time = 0;
			array[num2].ki.dwFlags = ((mSG.message == Msg.WM_KEYUP) ? 2 : 0);
			array[num2].ki.wVk = (short)mSG.wParam.ToInt32();
			array[num2].type = 1;
			num2++;
		}
		return (int)Win32SendInput((uint)array.Length, array, Marshal.SizeOf(typeof(INPUT)));
	}

	internal override void CreateOffscreenDrawable(IntPtr handle, int width, int height, out object offscreen_drawable)
	{
		Graphics graphics = Graphics.FromHwnd(handle);
		IntPtr hdc = graphics.GetHdc();
		IntPtr hdc2 = Win32CreateCompatibleDC(hdc);
		IntPtr intPtr = Win32CreateCompatibleBitmap(hdc, width, height);
		Win32SelectObject(hdc2, intPtr);
		offscreen_drawable = new WinBuffer(hdc2, intPtr);
		graphics.ReleaseHdc(hdc);
	}

	internal override Graphics GetOffscreenGraphics(object offscreen_drawable)
	{
		return Graphics.FromHdc(((WinBuffer)offscreen_drawable).hdc);
	}

	internal override void BlitFromOffscreen(IntPtr dest_handle, Graphics dest_dc, object offscreen_drawable, Graphics offscreen_dc, Rectangle r)
	{
		WinBuffer winBuffer = (WinBuffer)offscreen_drawable;
		IntPtr hdc = dest_dc.GetHdc();
		Win32BitBlt(hdc, r.Left, r.Top, r.Width, r.Height, winBuffer.hdc, r.Left, r.Top, TernaryRasterOperations.SRCCOPY);
		dest_dc.ReleaseHdc(hdc);
	}

	internal override void DestroyOffscreenDrawable(object offscreen_drawable)
	{
		WinBuffer winBuffer = (WinBuffer)offscreen_drawable;
		Win32DeleteObject(winBuffer.bitmap);
		Win32DeleteDC(winBuffer.hdc);
	}

	internal override void SetForegroundWindow(IntPtr handle)
	{
		Win32SetForegroundWindow(handle);
	}

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetLastError")]
	private static extern uint Win32GetLastError();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW")]
	internal static extern IntPtr Win32CreateWindow(WindowExStyles dwExStyle, string lpClassName, string lpWindowName, WindowStyles dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lParam);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyWindow")]
	internal static extern bool Win32DestroyWindow(IntPtr hWnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "PeekMessageW")]
	internal static extern bool Win32PeekMessage(ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax, uint flags);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetMessageW")]
	internal static extern bool Win32GetMessage(ref MSG msg, IntPtr hWnd, int wFilterMin, int wFilterMax);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "TranslateMessage")]
	internal static extern bool Win32TranslateMessage(ref MSG msg);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "DispatchMessageW")]
	internal static extern IntPtr Win32DispatchMessage(ref MSG msg);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "MoveWindow")]
	internal static extern bool Win32MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWindowPos")]
	internal static extern bool Win32SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags Flags);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWindowPos")]
	internal static extern bool Win32SetWindowPos(IntPtr hWnd, SetWindowPosZOrder pos, int x, int y, int cx, int cy, SetWindowPosFlags Flags);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SetWindowTextW")]
	internal static extern bool Win32SetWindowText(IntPtr hWnd, string lpString);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetWindowTextW")]
	internal static extern bool Win32GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetParent")]
	internal static extern IntPtr Win32SetParent(IntPtr hWnd, IntPtr hParent);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "RegisterClassW")]
	private static extern bool Win32RegisterClass(ref WNDCLASS wndClass);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "LoadCursorW")]
	private static extern IntPtr Win32LoadCursor(IntPtr hInstance, LoadCursorType type);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ShowCursor")]
	private static extern IntPtr Win32ShowCursor(bool bShow);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetCursor")]
	private static extern IntPtr Win32SetCursor(IntPtr hCursor);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateCursor")]
	private static extern IntPtr Win32CreateCursor(IntPtr hInstance, int xHotSpot, int yHotSpot, int nWidth, int nHeight, byte[] pvANDPlane, byte[] pvORPlane);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyCursor")]
	private static extern bool Win32DestroyCursor(IntPtr hCursor);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DrawIcon")]
	private static extern bool Win32DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW")]
	private static extern IntPtr Win32DefWindowProc(IntPtr hWnd, Msg Msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "PostQuitMessage")]
	private static extern IntPtr Win32PostQuitMessage(int nExitCode);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UpdateWindow")]
	private static extern IntPtr Win32UpdateWindow(IntPtr hWnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetUpdateRect")]
	private static extern bool Win32GetUpdateRect(IntPtr hWnd, ref RECT rect, bool erase);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "BeginPaint")]
	private static extern IntPtr Win32BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ValidateRect")]
	private static extern IntPtr Win32ValidateRect(IntPtr hWnd, ref RECT rect);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "EndPaint")]
	private static extern bool Win32EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDC")]
	private static extern IntPtr Win32GetDC(IntPtr hWnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWindowDC")]
	private static extern IntPtr Win32GetWindowDC(IntPtr hWnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ReleaseDC")]
	private static extern IntPtr Win32ReleaseDC(IntPtr hWnd, IntPtr hDC);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "MessageBoxW")]
	private static extern IntPtr Win32MessageBox(IntPtr hParent, string pText, string pCaption, uint uType);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "InvalidateRect")]
	private static extern IntPtr Win32InvalidateRect(IntPtr hWnd, ref RECT lpRect, bool bErase);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetCapture")]
	private static extern IntPtr Win32SetCapture(IntPtr hWnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ReleaseCapture")]
	private static extern IntPtr Win32ReleaseCapture();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWindowRect")]
	private static extern IntPtr Win32GetWindowRect(IntPtr hWnd, out RECT rect);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetClientRect")]
	private static extern IntPtr Win32GetClientRect(IntPtr hWnd, out RECT rect);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ScreenToClient")]
	private static extern bool Win32ScreenToClient(IntPtr hWnd, ref POINT pt);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ClientToScreen")]
	private static extern bool Win32ClientToScreen(IntPtr hWnd, ref POINT pt);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetParent")]
	private static extern IntPtr Win32GetParent(IntPtr hWnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetAncestor")]
	private static extern IntPtr Win32GetAncestor(IntPtr hWnd, AncestorType flags);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetActiveWindow")]
	private static extern IntPtr Win32SetActiveWindow(IntPtr hWnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "AdjustWindowRectEx")]
	private static extern bool Win32AdjustWindowRectEx(ref RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetCursorPos")]
	private static extern bool Win32GetCursorPos(out POINT lpPoint);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetCursorPos")]
	private static extern bool Win32SetCursorPos(int x, int y);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "TrackMouseEvent")]
	private static extern bool Win32TrackMouseEvent(ref TRACKMOUSEEVENT tme);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateSolidBrush")]
	private static extern IntPtr Win32CreateSolidBrush(COLORREF clrRef);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "PatBlt")]
	private static extern int Win32PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, PatBltRop dwRop);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetWindowLong")]
	private static extern uint Win32SetWindowLong(IntPtr hwnd, WindowLong index, uint value);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetWindowLong")]
	private static extern uint Win32GetWindowLong(IntPtr hwnd, WindowLong index);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetLayeredWindowAttributes")]
	private static extern uint Win32SetLayeredWindowAttributes(IntPtr hwnd, COLORREF crKey, byte bAlpha, LayeredWindowAttributes dwFlags);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetLayeredWindowAttributes")]
	private static extern uint Win32GetLayeredWindowAttributes(IntPtr hwnd, out COLORREF pcrKey, out byte pbAlpha, out LayeredWindowAttributes pwdFlags);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DeleteObject")]
	public static extern bool Win32DeleteObject(IntPtr o);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetKeyState")]
	private static extern short Win32GetKeyState(VirtualKeys nVirtKey);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetDesktopWindow")]
	private static extern IntPtr Win32GetDesktopWindow();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetTimer")]
	private static extern IntPtr Win32SetTimer(IntPtr hwnd, int nIDEvent, uint uElapse, IntPtr timerProc);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "KillTimer")]
	private static extern IntPtr Win32KillTimer(IntPtr hwnd, int nIDEvent);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ShowWindow")]
	private static extern IntPtr Win32ShowWindow(IntPtr hwnd, WindowPlacementFlags nCmdShow);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "EnableWindow")]
	private static extern IntPtr Win32EnableWindow(IntPtr hwnd, bool Enabled);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetFocus")]
	internal static extern IntPtr Win32SetFocus(IntPtr hwnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetFocus")]
	internal static extern IntPtr Win32GetFocus();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateCaret")]
	internal static extern bool Win32CreateCaret(IntPtr hwnd, IntPtr hBitmap, int nWidth, int nHeight);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyCaret")]
	private static extern bool Win32DestroyCaret();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ShowCaret")]
	private static extern bool Win32ShowCaret(IntPtr hwnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "HideCaret")]
	private static extern bool Win32HideCaret(IntPtr hwnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetCaretPos")]
	private static extern bool Win32SetCaretPos(int X, int Y);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetTextMetricsW")]
	internal static extern bool Win32GetTextMetrics(IntPtr hdc, ref TEXTMETRIC tm);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SelectObject")]
	internal static extern IntPtr Win32SelectObject(IntPtr hdc, IntPtr hgdiobject);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ScrollWindowEx")]
	private static extern bool Win32ScrollWindowEx(IntPtr hwnd, int dx, int dy, IntPtr prcScroll, ref RECT prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate, ScrollWindowExFlags flags);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ScrollWindowEx")]
	private static extern bool Win32ScrollWindowEx(IntPtr hwnd, int dx, int dy, IntPtr prcScroll, IntPtr prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate, ScrollWindowExFlags flags);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetActiveWindow")]
	private static extern IntPtr Win32GetActiveWindow();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSystemMetrics")]
	private static extern int Win32GetSystemMetrics(SystemMetrics nIndex);

	[DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "Shell_NotifyIconW")]
	private static extern bool Win32Shell_NotifyIcon(NotifyIconMessage dwMessage, ref NOTIFYICONDATA lpData);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateRectRgn")]
	internal static extern IntPtr Win32CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
	private static extern bool IsWindowEnabled(IntPtr hwnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
	private static extern bool IsWindowVisible(IntPtr hwnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
	private static extern IntPtr Win32SendMessage(IntPtr hwnd, Msg msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "PostMessageW")]
	private static extern bool Win32PostMessage(IntPtr hwnd, Msg msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SendInput")]
	private static extern uint Win32SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray)] INPUT[] inputs, int cbSize);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SystemParametersInfoW")]
	private static extern bool Win32SystemParametersInfo(SPIAction uiAction, uint uiParam, ref RECT rect, uint fWinIni);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SystemParametersInfoW")]
	private static extern bool Win32SystemParametersInfo(SPIAction uiAction, uint uiParam, ref int value, uint fWinIni);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SystemParametersInfoW")]
	private static extern bool Win32SystemParametersInfo(SPIAction uiAction, uint uiParam, ref bool value, uint fWinIni);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SystemParametersInfoW")]
	private static extern bool Win32SystemParametersInfo(SPIAction uiAction, uint uiParam, ref ANIMATIONINFO value, uint fWinIni);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "OpenClipboard")]
	private static extern bool Win32OpenClipboard(IntPtr hwnd);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "EmptyClipboard")]
	private static extern bool Win32EmptyClipboard();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "RegisterClipboardFormatW")]
	private static extern uint Win32RegisterClipboardFormat(string format);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CloseClipboard")]
	private static extern bool Win32CloseClipboard();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "EnumClipboardFormats")]
	private static extern uint Win32EnumClipboardFormats(uint format);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetClipboardData")]
	private static extern IntPtr Win32GetClipboardData(uint format);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetClipboardData")]
	private static extern IntPtr Win32SetClipboardData(uint format, IntPtr handle);

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GlobalAlloc")]
	internal static extern IntPtr Win32GlobalAlloc(GAllocFlags Flags, int dwBytes);

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CopyMemory")]
	internal static extern void Win32CopyMemory(IntPtr Destination, IntPtr Source, int length);

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GlobalFree")]
	internal static extern IntPtr Win32GlobalFree(IntPtr hMem);

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GlobalSize")]
	internal static extern uint Win32GlobalSize(IntPtr hMem);

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GlobalLock")]
	internal static extern IntPtr Win32GlobalLock(IntPtr hMem);

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GlobalUnlock")]
	internal static extern IntPtr Win32GlobalUnlock(IntPtr hMem);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetROP2")]
	internal static extern int Win32SetROP2(IntPtr hdc, ROP2DrawMode fnDrawMode);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "MoveToEx")]
	internal static extern bool Win32MoveToEx(IntPtr hdc, int x, int y, ref POINT lpPoint);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "MoveToEx")]
	internal static extern bool Win32MoveToEx(IntPtr hdc, int x, int y, IntPtr lpPoint);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "LineTo")]
	internal static extern bool Win32LineTo(IntPtr hdc, int x, int y);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreatePen")]
	internal static extern IntPtr Win32CreatePen(PenStyle fnPenStyle, int nWidth, ref COLORREF color);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreatePen")]
	internal static extern IntPtr Win32CreatePen(PenStyle fnPenStyle, int nWidth, IntPtr color);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetStockObject")]
	internal static extern IntPtr Win32GetStockObject(StockObject fnObject);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateHatchBrush")]
	internal static extern IntPtr Win32CreateHatchBrush(HatchStyle fnStyle, IntPtr color);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateHatchBrush")]
	internal static extern IntPtr Win32CreateHatchBrush(HatchStyle fnStyle, ref COLORREF color);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExcludeClipRect")]
	internal static extern int Win32ExcludeClipRect(IntPtr hdc, int left, int top, int right, int bottom);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtSelectClipRgn")]
	internal static extern int Win32ExtSelectClipRgn(IntPtr hdc, IntPtr hrgn, int mode);

	[DllImport("winmm.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "PlaySoundW")]
	internal static extern IntPtr Win32PlaySound(string pszSound, IntPtr hmod, SndFlags fdwSound);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetDoubleClickTime")]
	private static extern int Win32GetDoubleClickTime();

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SetWindowRgn")]
	internal static extern int Win32SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "GetWindowRgn")]
	internal static extern IntPtr Win32GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ClipCursor")]
	internal static extern bool Win32ClipCursor(ref RECT lpRect);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetClipCursor")]
	internal static extern bool Win32GetClipCursor(out RECT lpRect);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "BitBlt")]
	internal static extern bool Win32BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateCompatibleDC", ExactSpelling = true, SetLastError = true)]
	internal static extern IntPtr Win32CreateCompatibleDC(IntPtr hdc);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "DeleteDC", ExactSpelling = true, SetLastError = true)]
	internal static extern bool Win32DeleteDC(IntPtr hdc);

	[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "CreateCompatibleBitmap")]
	internal static extern IntPtr Win32CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetSystemPowerStatus")]
	internal static extern bool Win32GetSystemPowerStatus(SYSTEMPOWERSTATUS sps);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetIconInfo")]
	internal static extern bool Win32GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetForegroundWindow")]
	private static extern bool Win32SetForegroundWindow(IntPtr hWnd);
}
