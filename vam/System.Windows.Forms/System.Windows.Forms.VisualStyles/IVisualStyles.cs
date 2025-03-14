using System.Drawing;

namespace System.Windows.Forms.VisualStyles;

internal interface IVisualStyles
{
	string VisualStyleInformationAuthor { get; }

	string VisualStyleInformationColorScheme { get; }

	string VisualStyleInformationCompany { get; }

	Color VisualStyleInformationControlHighlightHot { get; }

	string VisualStyleInformationCopyright { get; }

	string VisualStyleInformationDescription { get; }

	string VisualStyleInformationDisplayName { get; }

	string VisualStyleInformationFileName { get; }

	bool VisualStyleInformationIsSupportedByOS { get; }

	int VisualStyleInformationMinimumColorDepth { get; }

	string VisualStyleInformationSize { get; }

	bool VisualStyleInformationSupportsFlatMenus { get; }

	Color VisualStyleInformationTextControlBorder { get; }

	string VisualStyleInformationUrl { get; }

	string VisualStyleInformationVersion { get; }

	int UxThemeCloseThemeData(IntPtr hTheme);

	int UxThemeDrawThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds);

	int UxThemeDrawThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, Rectangle clipRectangle);

	int UxThemeDrawThemeEdge(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, Edges edges, EdgeStyle style, EdgeEffects effects, out Rectangle result);

	int UxThemeDrawThemeParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl);

	int UxThemeDrawThemeText(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string text, TextFormatFlags textFlags, Rectangle bounds);

	int UxThemeGetThemeBackgroundContentRect(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, out Rectangle result);

	int UxThemeGetThemeBackgroundExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle contentBounds, out Rectangle result);

	int UxThemeGetThemeBackgroundRegion(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, out Region result);

	int UxThemeGetThemeBool(IntPtr hTheme, int iPartId, int iStateId, BooleanProperty prop, out bool result);

	int UxThemeGetThemeColor(IntPtr hTheme, int iPartId, int iStateId, ColorProperty prop, out Color result);

	int UxThemeGetThemeEnumValue(IntPtr hTheme, int iPartId, int iStateId, EnumProperty prop, out int result);

	int UxThemeGetThemeFilename(IntPtr hTheme, int iPartId, int iStateId, FilenameProperty prop, out string result);

	int UxThemeGetThemeInt(IntPtr hTheme, int iPartId, int iStateId, IntegerProperty prop, out int result);

	int UxThemeGetThemeMargins(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, MarginProperty prop, out Padding result);

	int UxThemeGetThemePartSize(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, Rectangle bounds, ThemeSizeType type, out Size result);

	int UxThemeGetThemePartSize(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, ThemeSizeType type, out Size result);

	int UxThemeGetThemePosition(IntPtr hTheme, int iPartId, int iStateId, PointProperty prop, out Point result);

	int UxThemeGetThemeString(IntPtr hTheme, int iPartId, int iStateId, StringProperty prop, out string result);

	int UxThemeGetThemeTextExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string textToDraw, TextFormatFlags flags, Rectangle bounds, out Rectangle result);

	int UxThemeGetThemeTextExtent(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, string textToDraw, TextFormatFlags flags, out Rectangle result);

	int UxThemeGetThemeTextMetrics(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, out TextMetrics result);

	int UxThemeHitTestThemeBackground(IntPtr hTheme, IDeviceContext dc, int iPartId, int iStateId, HitTestOptions options, Rectangle backgroundRectangle, IntPtr hrgn, Point pt, out HitTestCode result);

	bool UxThemeIsAppThemed();

	bool UxThemeIsThemeActive();

	bool UxThemeIsThemeBackgroundPartiallyTransparent(IntPtr hTheme, int iPartId, int iStateId);

	bool UxThemeIsThemePartDefined(IntPtr hTheme, int iPartId);

	IntPtr UxThemeOpenThemeData(IntPtr hWnd, string classList);

	void VisualStyleRendererDrawBackgroundExcludingArea(IntPtr theme, IDeviceContext dc, int part, int state, Rectangle bounds, Rectangle excludedArea);
}
