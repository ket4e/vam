using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class SystemInformation
{
	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int ActiveWindowTrackingDelay => XplatUI.ActiveWindowTrackingDelay;

	public static ArrangeDirection ArrangeDirection => ThemeEngine.Current.ArrangeDirection;

	public static ArrangeStartingPosition ArrangeStartingPosition => ThemeEngine.Current.ArrangeStartingPosition;

	public static BootMode BootMode => BootMode.Normal;

	public static Size Border3DSize => ThemeEngine.Current.Border3DSize;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int BorderMultiplierFactor => ThemeEngine.Current.BorderMultiplierFactor;

	public static Size BorderSize => ThemeEngine.Current.BorderSize;

	public static Size CaptionButtonSize => ThemeEngine.Current.CaptionButtonSize;

	public static int CaptionHeight => ThemeEngine.Current.CaptionHeight;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int CaretBlinkTime => XplatUI.CaretBlinkTime;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int CaretWidth => XplatUI.CaretWidth;

	public static string ComputerName => Environment.MachineName;

	public static Size CursorSize => XplatUI.CursorSize;

	public static bool DbcsEnabled => false;

	public static bool DebugOS => false;

	public static Size DoubleClickSize => ThemeEngine.Current.DoubleClickSize;

	public static int DoubleClickTime => ThemeEngine.Current.DoubleClickTime;

	public static bool DragFullWindows => XplatUI.DragFullWindows;

	public static Size DragSize => XplatUI.DragSize;

	public static Size FixedFrameBorderSize => ThemeEngine.Current.FixedFrameBorderSize;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int FontSmoothingContrast => XplatUI.FontSmoothingContrast;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int FontSmoothingType => XplatUI.FontSmoothingType;

	public static Size FrameBorderSize => ThemeEngine.Current.FrameBorderSize;

	public static bool HighContrast => false;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int HorizontalFocusThickness => ThemeEngine.Current.HorizontalFocusThickness;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int HorizontalResizeBorderThickness => XplatUI.HorizontalResizeBorderThickness;

	public static int HorizontalScrollBarArrowWidth => ThemeEngine.Current.HorizontalScrollBarArrowWidth;

	public static int HorizontalScrollBarHeight => ThemeEngine.Current.HorizontalScrollBarHeight;

	public static int HorizontalScrollBarThumbWidth => ThemeEngine.Current.HorizontalScrollBarThumbWidth;

	public static Size IconSize => XplatUI.IconSize;

	public static int IconHorizontalSpacing => IconSpacingSize.Width;

	public static int IconVerticalSpacing => IconSpacingSize.Height;

	public static Size IconSpacingSize => ThemeEngine.Current.IconSpacingSize;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsActiveWindowTrackingEnabled => XplatUI.IsActiveWindowTrackingEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsComboBoxAnimationEnabled => XplatUI.IsComboBoxAnimationEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsDropShadowEnabled => XplatUI.IsDropShadowEnabled;

	public static bool IsFlatMenuEnabled => false;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsFontSmoothingEnabled => XplatUI.IsFontSmoothingEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsHotTrackingEnabled => XplatUI.IsHotTrackingEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsIconTitleWrappingEnabled => XplatUI.IsIconTitleWrappingEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsKeyboardPreferred => XplatUI.IsKeyboardPreferred;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsListBoxSmoothScrollingEnabled => XplatUI.IsListBoxSmoothScrollingEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsMenuAnimationEnabled => XplatUI.IsMenuAnimationEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsMenuFadeEnabled => XplatUI.IsMenuFadeEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsMinimizeRestoreAnimationEnabled => XplatUI.IsMinimizeRestoreAnimationEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsSelectionFadeEnabled => XplatUI.IsSelectionFadeEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsSnapToDefaultEnabled => XplatUI.IsSnapToDefaultEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsTitleBarGradientEnabled => XplatUI.IsTitleBarGradientEnabled;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool IsToolTipAnimationEnabled => XplatUI.IsToolTipAnimationEnabled;

	public static int KanjiWindowHeight => 0;

	public static int KeyboardDelay => XplatUI.KeyboardDelay;

	public static int KeyboardSpeed => XplatUI.KeyboardSpeed;

	public static Size MaxWindowTrackSize => XplatUI.MaxWindowTrackSize;

	public static bool MenuAccessKeysUnderlined => ThemeEngine.Current.MenuAccessKeysUnderlined;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static Size MenuBarButtonSize => ThemeEngine.Current.MenuBarButtonSize;

	public static Size MenuButtonSize => ThemeEngine.Current.MenuButtonSize;

	public static Size MenuCheckSize => ThemeEngine.Current.MenuCheckSize;

	public static Font MenuFont => (Font)ThemeEngine.Current.MenuFont.Clone();

	public static int MenuHeight => ThemeEngine.Current.MenuHeight;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int MenuShowDelay => XplatUI.MenuShowDelay;

	public static bool MidEastEnabled => false;

	public static Size MinimizedWindowSize => XplatUI.MinimizedWindowSize;

	public static Size MinimizedWindowSpacingSize => XplatUI.MinimizedWindowSpacingSize;

	public static Size MinimumWindowSize => XplatUI.MinimumWindowSize;

	public static Size MinWindowTrackSize => XplatUI.MinWindowTrackSize;

	public static int MonitorCount => 1;

	public static bool MonitorsSameDisplayFormat => true;

	public static int MouseButtons => XplatUI.MouseButtonCount;

	public static bool MouseButtonsSwapped => XplatUI.MouseButtonsSwapped;

	public static Size MouseHoverSize => XplatUI.MouseHoverSize;

	public static int MouseHoverTime => XplatUI.MouseHoverTime;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int MouseSpeed => XplatUI.MouseSpeed;

	public static int MouseWheelScrollDelta => XplatUI.MouseWheelScrollDelta;

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool MousePresent => true;

	public static bool MouseWheelPresent => XplatUI.MouseWheelPresent;

	public static int MouseWheelScrollLines => ThemeEngine.Current.MouseWheelScrollLines;

	public static bool NativeMouseWheelSupport => MouseWheelPresent;

	public static bool Network => true;

	public static bool PenWindows => false;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static LeftRightAlignment PopupMenuAlignment => XplatUI.PopupMenuAlignment;

	[System.MonoTODO("Only implemented for Win32.")]
	public static PowerStatus PowerStatus => XplatUI.PowerStatus;

	public static Size PrimaryMonitorMaximizedWindowSize => new Size(WorkingArea.Width, WorkingArea.Height);

	public static Size PrimaryMonitorSize => new Size(WorkingArea.Width, WorkingArea.Height);

	public static bool RightAlignedMenus => ThemeEngine.Current.RightAlignedMenus;

	public static ScreenOrientation ScreenOrientation => ScreenOrientation.Angle0;

	public static bool Secure => true;

	public static bool ShowSounds => false;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int SizingBorderWidth => XplatUI.SizingBorderWidth;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static Size SmallCaptionButtonSize => XplatUI.SmallCaptionButtonSize;

	public static Size SmallIconSize => XplatUI.SmallIconSize;

	public static bool TerminalServerSession => false;

	public static Size ToolWindowCaptionButtonSize => ThemeEngine.Current.ToolWindowCaptionButtonSize;

	public static int ToolWindowCaptionHeight => ThemeEngine.Current.ToolWindowCaptionHeight;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static bool UIEffectsEnabled => XplatUI.UIEffectsEnabled;

	public static string UserDomainName => Environment.UserDomainName;

	public static bool UserInteractive => Environment.UserInteractive;

	public static string UserName => Environment.UserName;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int VerticalFocusThickness => ThemeEngine.Current.VerticalFocusThickness;

	[System.MonoInternalNote("Determine if we need an X11 implementation or if defaults are good.")]
	public static int VerticalResizeBorderThickness => XplatUI.VerticalResizeBorderThickness;

	public static int VerticalScrollBarArrowHeight => ThemeEngine.Current.VerticalScrollBarArrowHeight;

	public static int VerticalScrollBarThumbHeight => ThemeEngine.Current.VerticalScrollBarThumbHeight;

	public static int VerticalScrollBarWidth => ThemeEngine.Current.VerticalScrollBarWidth;

	public static Rectangle VirtualScreen => XplatUI.VirtualScreen;

	public static Rectangle WorkingArea => XplatUI.WorkingArea;

	private SystemInformation()
	{
	}
}
