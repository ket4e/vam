using System.Drawing;
using System.Text;

namespace System.Windows.Forms.VisualStyles;

internal class VisualStylesNative : IVisualStyles
{
	public string VisualStyleInformationAuthor => GetData("AUTHOR");

	public string VisualStyleInformationColorScheme
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			StringBuilder stringBuilder2 = new StringBuilder(260);
			StringBuilder stringBuilder3 = new StringBuilder(260);
			UXTheme.GetCurrentThemeName(stringBuilder, stringBuilder.Capacity, stringBuilder2, stringBuilder2.Capacity, stringBuilder3, stringBuilder3.Capacity);
			return stringBuilder2.ToString();
		}
	}

	public string VisualStyleInformationCompany => GetData("COMPANY");

	public Color VisualStyleInformationControlHighlightHot
	{
		get
		{
			IntPtr hTheme = UXTheme.OpenThemeData(IntPtr.Zero, "BUTTON");
			uint themeSysColor = UXTheme.GetThemeSysColor(hTheme, 1621);
			UXTheme.CloseThemeData(hTheme);
			return Color.FromArgb((int)(0xFF & themeSysColor), (int)(0xFF00 & themeSysColor) >> 8, (int)(0xFF0000 & themeSysColor) >> 16);
		}
	}

	public string VisualStyleInformationCopyright => GetData("COPYRIGHT");

	public string VisualStyleInformationDescription => GetData("DESCRIPTION");

	public string VisualStyleInformationDisplayName => GetData("DISPLAYNAME");

	public string VisualStyleInformationFileName
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			StringBuilder stringBuilder2 = new StringBuilder(260);
			StringBuilder stringBuilder3 = new StringBuilder(260);
			UXTheme.GetCurrentThemeName(stringBuilder, stringBuilder.Capacity, stringBuilder2, stringBuilder2.Capacity, stringBuilder3, stringBuilder3.Capacity);
			return stringBuilder.ToString();
		}
	}

	public bool VisualStyleInformationIsSupportedByOS => IsSupported();

	public int VisualStyleInformationMinimumColorDepth
	{
		get
		{
			IntPtr hTheme = UXTheme.OpenThemeData(IntPtr.Zero, "BUTTON");
			UXTheme.GetThemeSysInt(hTheme, 1301, out var piVal);
			UXTheme.CloseThemeData(hTheme);
			return piVal;
		}
	}

	public string VisualStyleInformationSize
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(260);
			StringBuilder stringBuilder2 = new StringBuilder(260);
			StringBuilder stringBuilder3 = new StringBuilder(260);
			UXTheme.GetCurrentThemeName(stringBuilder, stringBuilder.Capacity, stringBuilder2, stringBuilder2.Capacity, stringBuilder3, stringBuilder3.Capacity);
			return stringBuilder3.ToString();
		}
	}

	public bool VisualStyleInformationSupportsFlatMenus
	{
		get
		{
			IntPtr hTheme = UXTheme.OpenThemeData(IntPtr.Zero, "BUTTON");
			bool result = ((UXTheme.GetThemeSysBool(hTheme, 1001) != 0) ? true : false);
			UXTheme.CloseThemeData(hTheme);
			return result;
		}
	}

	public Color VisualStyleInformationTextControlBorder
	{
		get
		{
			IntPtr hTheme = UXTheme.OpenThemeData(IntPtr.Zero, "EDIT");
			uint themeSysColor = UXTheme.GetThemeSysColor(hTheme, 1611);
			UXTheme.CloseThemeData(hTheme);
			return Color.FromArgb((int)(0xFF & themeSysColor), (int)(0xFF00 & themeSysColor) >> 8, (int)(0xFF0000 & themeSysColor) >> 16);
		}
	}

	public string VisualStyleInformationUrl => GetData("URL");

	public string VisualStyleInformationVersion => GetData("VERSION");

	public int UxThemeCloseThemeData(IntPtr hTheme)
	{
		return UXTheme.CloseThemeData(hTheme);
	}

	public int UxThemeDrawThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(bounds);
		int result = UXTheme.DrawThemeBackground(hTheme, dc.GetHdc(), iPartId, iStateId, ref pRect, IntPtr.Zero);
		dc.ReleaseHdc();
		return result;
	}

	public int UxThemeDrawThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, Rectangle clipRectangle)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(bounds);
		XplatUIWin32.RECT pClipRect = XplatUIWin32.RECT.FromRectangle(clipRectangle);
		int result = UXTheme.DrawThemeBackground(hTheme, dc.GetHdc(), iPartId, iStateId, ref pRect, ref pClipRect);
		dc.ReleaseHdc();
		return result;
	}

	public int UxThemeDrawThemeEdge(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects, out Rectangle result)
	{
		XplatUIWin32.RECT pDestRect = XplatUIWin32.RECT.FromRectangle(bounds);
		XplatUIWin32.RECT pRect;
		int result2 = UXTheme.DrawThemeEdge(hTheme, dc.GetHdc(), iPartId, iStateId, ref pDestRect, (uint)style, (uint)edges + (uint)effects, out pRect);
		dc.ReleaseHdc();
		result = pRect.ToRectangle();
		return result2;
	}

	public int UxThemeDrawThemeParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(bounds);
		using Graphics graphics = Graphics.FromHwnd(childControl.Handle);
		IntPtr hdc = graphics.GetHdc();
		int result = UXTheme.DrawThemeParentBackground(childControl.Handle, hdc, ref pRect);
		graphics.ReleaseHdc(hdc);
		return result;
	}

	public int UxThemeDrawThemeText(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string text, TextFormatFlags textFlags, Rectangle bounds)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(bounds);
		int result = UXTheme.DrawThemeText(hTheme, dc.GetHdc(), iPartId, iStateId, text, text.Length, (uint)textFlags, 0u, ref pRect);
		dc.ReleaseHdc();
		return result;
	}

	public int UxThemeGetThemeBackgroundContentRect(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, out Rectangle result)
	{
		XplatUIWin32.RECT pBoundingRect = XplatUIWin32.RECT.FromRectangle(bounds);
		XplatUIWin32.RECT pContentRect;
		int themeBackgroundContentRect = UXTheme.GetThemeBackgroundContentRect(hTheme, dc.GetHdc(), iPartId, iStateId, ref pBoundingRect, out pContentRect);
		dc.ReleaseHdc();
		result = pContentRect.ToRectangle();
		return themeBackgroundContentRect;
	}

	public int UxThemeGetThemeBackgroundExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle contentBounds, out Rectangle result)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(contentBounds);
		XplatUIWin32.RECT pClipRect = default(XplatUIWin32.RECT);
		int themeBackgroundExtent = UXTheme.GetThemeBackgroundExtent(hTheme, dc.GetHdc(), iPartId, iStateId, ref pRect, ref pClipRect);
		dc.ReleaseHdc();
		result = pClipRect.ToRectangle();
		return themeBackgroundExtent;
	}

	public int UxThemeGetThemeBackgroundRegion(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, out Region result)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(bounds);
		IntPtr pRegion;
		int themeBackgroundRegion = UXTheme.GetThemeBackgroundRegion(hTheme, dc.GetHdc(), iPartId, iStateId, ref pRect, out pRegion);
		dc.ReleaseHdc();
		result = Region.FromHrgn(pRegion);
		return themeBackgroundRegion;
	}

	public int UxThemeGetThemeBool(IntPtr hTheme, int iPartId, int iStateId, BooleanProperty prop, out bool result)
	{
		int pfVal;
		int themeBool = UXTheme.GetThemeBool(hTheme, iPartId, iStateId, (int)prop, out pfVal);
		result = ((pfVal != 0) ? true : false);
		return themeBool;
	}

	public int UxThemeGetThemeColor(IntPtr hTheme, int iPartId, int iStateId, ColorProperty prop, out Color result)
	{
		int pColor;
		int themeColor = UXTheme.GetThemeColor(hTheme, iPartId, iStateId, (int)prop, out pColor);
		result = Color.FromArgb((int)(0xFFL & (long)pColor), (int)(0xFF00L & (long)pColor) >> 8, (int)(0xFF0000L & (long)pColor) >> 16);
		return themeColor;
	}

	public int UxThemeGetThemeEnumValue(IntPtr hTheme, int iPartId, int iStateId, EnumProperty prop, out int result)
	{
		int piVal;
		int themeEnumValue = UXTheme.GetThemeEnumValue(hTheme, iPartId, iStateId, (int)prop, out piVal);
		result = piVal;
		return themeEnumValue;
	}

	public int UxThemeGetThemeFilename(IntPtr hTheme, int iPartId, int iStateId, FilenameProperty prop, out string result)
	{
		StringBuilder stringBuilder = new StringBuilder(255);
		int themeFilename = UXTheme.GetThemeFilename(hTheme, iPartId, iStateId, (int)prop, stringBuilder, stringBuilder.Capacity);
		result = stringBuilder.ToString();
		return themeFilename;
	}

	public int UxThemeGetThemeInt(IntPtr hTheme, int iPartId, int iStateId, IntegerProperty prop, out int result)
	{
		int piVal;
		int themeInt = UXTheme.GetThemeInt(hTheme, iPartId, iStateId, (int)prop, out piVal);
		result = piVal;
		return themeInt;
	}

	public int UxThemeGetThemeMargins(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, MarginProperty prop, out Padding result)
	{
		UXTheme.MARGINS pMargins = default(UXTheme.MARGINS);
		XplatUIWin32.RECT prc;
		int themeMargins = UXTheme.GetThemeMargins(hTheme, dc.GetHdc(), iPartId, iStateId, (int)prop, out prc, out pMargins);
		dc.ReleaseHdc();
		result = pMargins.ToPadding();
		return themeMargins;
	}

	public int UxThemeGetThemePartSize(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, ThemeSizeType type, out Size result)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(bounds);
		UXTheme.SIZE size;
		int themePartSize = UXTheme.GetThemePartSize(hTheme, dc.GetHdc(), iPartId, iStateId, ref pRect, (int)type, out size);
		dc.ReleaseHdc();
		result = size.ToSize();
		return themePartSize;
	}

	public int UxThemeGetThemePartSize(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, ThemeSizeType type, out Size result)
	{
		UXTheme.SIZE size;
		int themePartSize = UXTheme.GetThemePartSize(hTheme, dc.GetHdc(), iPartId, iStateId, IntPtr.Zero, (int)type, out size);
		dc.ReleaseHdc();
		result = size.ToSize();
		return themePartSize;
	}

	public int UxThemeGetThemePosition(IntPtr hTheme, int iPartId, int iStateId, PointProperty prop, out Point result)
	{
		POINT pPoint;
		int themePosition = UXTheme.GetThemePosition(hTheme, iPartId, iStateId, (int)prop, out pPoint);
		result = pPoint.ToPoint();
		return themePosition;
	}

	public int UxThemeGetThemeString(IntPtr hTheme, int iPartId, int iStateId, StringProperty prop, out string result)
	{
		StringBuilder stringBuilder = new StringBuilder(255);
		int themeString = UXTheme.GetThemeString(hTheme, iPartId, iStateId, (int)prop, stringBuilder, stringBuilder.Capacity);
		result = stringBuilder.ToString();
		return themeString;
	}

	public int UxThemeGetThemeTextExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string textToDraw, TextFormatFlags flags, Rectangle bounds, out Rectangle result)
	{
		XplatUIWin32.RECT boundingRect = XplatUIWin32.RECT.FromRectangle(bounds);
		XplatUIWin32.RECT extentRect;
		int themeTextExtent = UXTheme.GetThemeTextExtent(hTheme, dc.GetHdc(), iPartId, iStateId, textToDraw, textToDraw.Length, (int)flags, ref boundingRect, out extentRect);
		dc.ReleaseHdc();
		result = extentRect.ToRectangle();
		return themeTextExtent;
	}

	public int UxThemeGetThemeTextExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string textToDraw, TextFormatFlags flags, out Rectangle result)
	{
		XplatUIWin32.RECT extentRect;
		int themeTextExtent = UXTheme.GetThemeTextExtent(hTheme, dc.GetHdc(), iPartId, iStateId, textToDraw, textToDraw.Length, (int)flags, 0, out extentRect);
		dc.ReleaseHdc();
		result = extentRect.ToRectangle();
		return themeTextExtent;
	}

	public int UxThemeGetThemeTextMetrics(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, out TextMetrics result)
	{
		XplatUIWin32.TEXTMETRIC textMetric;
		int themeTextMetrics = UXTheme.GetThemeTextMetrics(hTheme, dc.GetHdc(), iPartId, iStateId, out textMetric);
		dc.ReleaseHdc();
		TextMetrics textMetrics = default(TextMetrics);
		textMetrics.Ascent = textMetric.tmAscent;
		textMetrics.AverageCharWidth = textMetric.tmAveCharWidth;
		textMetrics.BreakChar = (char)textMetric.tmBreakChar;
		textMetrics.CharSet = (TextMetricsCharacterSet)textMetric.tmCharSet;
		textMetrics.DefaultChar = (char)textMetric.tmDefaultChar;
		textMetrics.Descent = textMetric.tmDescent;
		textMetrics.DigitizedAspectX = textMetric.tmDigitizedAspectX;
		textMetrics.DigitizedAspectY = textMetric.tmDigitizedAspectY;
		textMetrics.ExternalLeading = textMetric.tmExternalLeading;
		textMetrics.FirstChar = (char)textMetric.tmFirstChar;
		textMetrics.Height = textMetric.tmHeight;
		textMetrics.InternalLeading = textMetric.tmInternalLeading;
		textMetrics.Italic = ((textMetric.tmItalic != 0) ? true : false);
		textMetrics.LastChar = (char)textMetric.tmLastChar;
		textMetrics.MaxCharWidth = textMetric.tmMaxCharWidth;
		textMetrics.Overhang = textMetric.tmOverhang;
		textMetrics.PitchAndFamily = (TextMetricsPitchAndFamilyValues)textMetric.tmPitchAndFamily;
		textMetrics.StruckOut = ((textMetric.tmStruckOut != 0) ? true : false);
		textMetrics.Underlined = ((textMetric.tmUnderlined != 0) ? true : false);
		textMetrics.Weight = textMetric.tmWeight;
		result = textMetrics;
		return themeTextMetrics;
	}

	public int UxThemeHitTestThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, HitTestOptions options, Rectangle backgroundRectangle, IntPtr hrgn, Point pt, out HitTestCode result)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(backgroundRectangle);
		int code;
		int result2 = UXTheme.HitTestThemeBackground(hTheme, dc.GetHdc(), iPartId, iStateId, (uint)options, ref pRect, hrgn, new POINT(pt.X, pt.Y), out code);
		dc.ReleaseHdc();
		result = (HitTestCode)code;
		return result2;
	}

	public bool UxThemeIsAppThemed()
	{
		return UXTheme.IsAppThemed();
	}

	public bool UxThemeIsThemeActive()
	{
		return UXTheme.IsThemeActive();
	}

	public bool UxThemeIsThemePartDefined(IntPtr hTheme, int iPartId)
	{
		return UXTheme.IsThemePartDefined(hTheme, iPartId, 0);
	}

	public bool UxThemeIsThemeBackgroundPartiallyTransparent(IntPtr hTheme, int iPartId, int iStateId)
	{
		return (UXTheme.IsThemeBackgroundPartiallyTransparent(hTheme, iPartId, iStateId) != 0) ? true : false;
	}

	public IntPtr UxThemeOpenThemeData(IntPtr hWnd, string classList)
	{
		return UXTheme.OpenThemeData(hWnd, classList);
	}

	private static string GetData(string propertyName)
	{
		StringBuilder stringBuilder = new StringBuilder(260);
		StringBuilder stringBuilder2 = new StringBuilder(260);
		StringBuilder stringBuilder3 = new StringBuilder(260);
		UXTheme.GetCurrentThemeName(stringBuilder, stringBuilder.Capacity, stringBuilder2, stringBuilder2.Capacity, stringBuilder3, stringBuilder3.Capacity);
		StringBuilder stringBuilder4 = new StringBuilder(260);
		UXTheme.GetThemeDocumentationProperty(stringBuilder.ToString(), propertyName, stringBuilder4, stringBuilder4.Capacity);
		return stringBuilder4.ToString();
	}

	public static bool IsSupported()
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(5, 1))
		{
			return true;
		}
		return false;
	}

	public void VisualStyleRendererDrawBackgroundExcludingArea(IntPtr theme, IDeviceContext dc, int part, int state, Rectangle bounds, Rectangle excludedArea)
	{
		XplatUIWin32.RECT pRect = XplatUIWin32.RECT.FromRectangle(bounds);
		IntPtr hdc = dc.GetHdc();
		XplatUIWin32.Win32ExcludeClipRect(hdc, excludedArea.Left, excludedArea.Top, excludedArea.Right, excludedArea.Bottom);
		UXTheme.DrawThemeBackground(theme, hdc, part, state, ref pRect, IntPtr.Zero);
		IntPtr intPtr = XplatUIWin32.Win32CreateRectRgn(excludedArea.Left, excludedArea.Top, excludedArea.Right, excludedArea.Bottom);
		XplatUIWin32.Win32ExtSelectClipRgn(hdc, intPtr, 2);
		XplatUIWin32.Win32DeleteObject(intPtr);
		dc.ReleaseHdc();
	}
}
