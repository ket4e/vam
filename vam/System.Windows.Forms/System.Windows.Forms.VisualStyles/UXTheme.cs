using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms.VisualStyles;

internal class UXTheme
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class LOGFONT
	{
		public int lfHeight;

		public int lfWidth;

		public int lfEscapement;

		public int lfOrientation;

		public int lfWeight;

		public byte lfItalic;

		public byte lfUnderline;

		public byte lfStrikeOut;

		public byte lfCharSet;

		public byte lfOutPrecision;

		public byte lfClipPrecision;

		public byte lfQuality;

		public byte lfPitchAndFamily;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string lfFaceName = string.Empty;
	}

	public struct MARGINS
	{
		public int leftWidth;

		public int rightWidth;

		public int topHeight;

		public int bottomHeight;

		public Padding ToPadding()
		{
			return new Padding(leftWidth, topHeight, rightWidth, bottomHeight);
		}
	}

	public struct SIZE
	{
		public int cx;

		public int cy;

		public Size ToSize()
		{
			return new Size(cx, cy);
		}
	}

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int CloseThemeData(IntPtr hTheme);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pRect, ref XplatUIWin32.RECT pClipRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pRect, IntPtr pClipRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int DrawThemeEdge(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pDestRect, uint egde, uint flags, out XplatUIWin32.RECT pRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int DrawThemeEdge(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pDestRect, uint edge, uint flags, int pRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int DrawThemeIcon(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pRect, IntPtr himl, int iImageIndex);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref XplatUIWin32.RECT pRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, int pRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int DrawThemeText(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int textLength, uint textFlags, uint textFlags2, ref XplatUIWin32.RECT pRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int EnableTheming(int fEnable);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern IntPtr OpenThemeData(IntPtr hWnd, string classList);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeBackgroundContentRect(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pBoundingRect, out XplatUIWin32.RECT pContentRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeBackgroundExtent(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pRect, ref XplatUIWin32.RECT pClipRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeBackgroundRegion(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pRect, out IntPtr pRegion);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeBool(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out int pfVal);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out int pColor);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeEnumValue(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out int piVal);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeFilename(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder themeFileName, int themeFileNameLength);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeFont(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, [MarshalAs(UnmanagedType.LPStruct)] out LOGFONT lf);

	[DllImport("gdi32", CharSet = CharSet.Auto)]
	public static extern IntPtr CreateFontIndirect([In][MarshalAs(UnmanagedType.LPStruct)] LOGFONT lplf);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeInt(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out int piVal);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeMargins(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, int iPropId, out XplatUIWin32.RECT prc, out MARGINS pMargins);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemePartSize(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref XplatUIWin32.RECT pRect, int eSize, out SIZE size);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemePartSize(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, IntPtr pRect, int eSize, out SIZE size);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemePosition(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out POINT pPoint);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeString(IntPtr hTheme, int iPartId, int iStateId, int iPropId, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder themeString, int themeStringLength);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeTextExtent(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int textLength, int textFlags, ref XplatUIWin32.RECT boundingRect, out XplatUIWin32.RECT extentRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeTextExtent(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int textLength, int textFlags, int boundingRect, out XplatUIWin32.RECT extentRect);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeTextMetrics(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, out XplatUIWin32.TEXTMETRIC textMetric);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int HitTestThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, uint dwOptions, ref XplatUIWin32.RECT pRect, IntPtr hrgn, POINT ptTest, out HitTestCode code);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int IsThemeBackgroundPartiallyTransparent(IntPtr hTheme, int iPartId, int iStateId);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern bool IsThemePartDefined(IntPtr hTheme, int iPartId, int iStateId);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern bool IsThemeActive();

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern bool IsAppThemed();

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int HitTestThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, uint dwOptions, ref XplatUIWin32.RECT pRect, IntPtr hrgn, POINT ptTest, out int code);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeDocumentationProperty(string stringThemeName, string stringPropertyName, StringBuilder stringValue, int lengthValue);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetCurrentThemeName(StringBuilder stringThemeName, int lengthThemeName, StringBuilder stringColorName, int lengthColorName, StringBuilder stringSizeName, int lengthSizeName);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern uint GetThemeSysColor(IntPtr hTheme, int iColorId);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeSysInt(IntPtr hTheme, int iIntId, out int piVal);

	[DllImport("uxtheme", CharSet = CharSet.Unicode, ExactSpelling = true)]
	public static extern int GetThemeSysBool(IntPtr hTheme, int iBoolId);
}
