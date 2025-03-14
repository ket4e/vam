using System.Drawing;
using System.Reflection;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

internal abstract class Theme
{
	public const int ManagedWindowSpacingAfterLastTitleButton = 2;

	protected Array syscolors;

	private readonly Font default_font;

	protected Font window_border_font;

	protected Color defaultWindowBackColor;

	protected Color defaultWindowForeColor;

	internal SystemResPool ResPool = new SystemResPool();

	private MethodInfo update;

	public abstract Version Version { get; }

	public virtual Color ColorScrollBar
	{
		get
		{
			return SystemColors.ScrollBar;
		}
		set
		{
			SetSystemColors(KnownColor.ScrollBar, value);
		}
	}

	public virtual Color ColorDesktop
	{
		get
		{
			return SystemColors.Desktop;
		}
		set
		{
			SetSystemColors(KnownColor.Desktop, value);
		}
	}

	public virtual Color ColorActiveCaption
	{
		get
		{
			return SystemColors.ActiveCaption;
		}
		set
		{
			SetSystemColors(KnownColor.ActiveCaption, value);
		}
	}

	public virtual Color ColorInactiveCaption
	{
		get
		{
			return SystemColors.InactiveCaption;
		}
		set
		{
			SetSystemColors(KnownColor.InactiveCaption, value);
		}
	}

	public virtual Color ColorMenu
	{
		get
		{
			return SystemColors.Menu;
		}
		set
		{
			SetSystemColors(KnownColor.Menu, value);
		}
	}

	public virtual Color ColorWindow
	{
		get
		{
			return SystemColors.Window;
		}
		set
		{
			SetSystemColors(KnownColor.Window, value);
		}
	}

	public virtual Color ColorWindowFrame
	{
		get
		{
			return SystemColors.WindowFrame;
		}
		set
		{
			SetSystemColors(KnownColor.WindowFrame, value);
		}
	}

	public virtual Color ColorMenuText
	{
		get
		{
			return SystemColors.MenuText;
		}
		set
		{
			SetSystemColors(KnownColor.MenuText, value);
		}
	}

	public virtual Color ColorWindowText
	{
		get
		{
			return SystemColors.WindowText;
		}
		set
		{
			SetSystemColors(KnownColor.WindowText, value);
		}
	}

	public virtual Color ColorActiveCaptionText
	{
		get
		{
			return SystemColors.ActiveCaptionText;
		}
		set
		{
			SetSystemColors(KnownColor.ActiveCaptionText, value);
		}
	}

	public virtual Color ColorActiveBorder
	{
		get
		{
			return SystemColors.ActiveBorder;
		}
		set
		{
			SetSystemColors(KnownColor.ActiveBorder, value);
		}
	}

	public virtual Color ColorInactiveBorder
	{
		get
		{
			return SystemColors.InactiveBorder;
		}
		set
		{
			SetSystemColors(KnownColor.InactiveBorder, value);
		}
	}

	public virtual Color ColorAppWorkspace
	{
		get
		{
			return SystemColors.AppWorkspace;
		}
		set
		{
			SetSystemColors(KnownColor.AppWorkspace, value);
		}
	}

	public virtual Color ColorHighlight
	{
		get
		{
			return SystemColors.Highlight;
		}
		set
		{
			SetSystemColors(KnownColor.Highlight, value);
		}
	}

	public virtual Color ColorHighlightText
	{
		get
		{
			return SystemColors.HighlightText;
		}
		set
		{
			SetSystemColors(KnownColor.HighlightText, value);
		}
	}

	public virtual Color ColorControl
	{
		get
		{
			return SystemColors.Control;
		}
		set
		{
			SetSystemColors(KnownColor.Control, value);
		}
	}

	public virtual Color ColorControlDark
	{
		get
		{
			return SystemColors.ControlDark;
		}
		set
		{
			SetSystemColors(KnownColor.ControlDark, value);
		}
	}

	public virtual Color ColorGrayText
	{
		get
		{
			return SystemColors.GrayText;
		}
		set
		{
			SetSystemColors(KnownColor.GrayText, value);
		}
	}

	public virtual Color ColorControlText
	{
		get
		{
			return SystemColors.ControlText;
		}
		set
		{
			SetSystemColors(KnownColor.ControlText, value);
		}
	}

	public virtual Color ColorInactiveCaptionText
	{
		get
		{
			return SystemColors.InactiveCaptionText;
		}
		set
		{
			SetSystemColors(KnownColor.InactiveCaptionText, value);
		}
	}

	public virtual Color ColorControlLight
	{
		get
		{
			return SystemColors.ControlLight;
		}
		set
		{
			SetSystemColors(KnownColor.ControlLight, value);
		}
	}

	public virtual Color ColorControlDarkDark
	{
		get
		{
			return SystemColors.ControlDarkDark;
		}
		set
		{
			SetSystemColors(KnownColor.ControlDarkDark, value);
		}
	}

	public virtual Color ColorControlLightLight
	{
		get
		{
			return SystemColors.ControlLightLight;
		}
		set
		{
			SetSystemColors(KnownColor.ControlLightLight, value);
		}
	}

	public virtual Color ColorInfoText
	{
		get
		{
			return SystemColors.InfoText;
		}
		set
		{
			SetSystemColors(KnownColor.InfoText, value);
		}
	}

	public virtual Color ColorInfo
	{
		get
		{
			return SystemColors.Info;
		}
		set
		{
			SetSystemColors(KnownColor.Info, value);
		}
	}

	public virtual Color ColorHotTrack
	{
		get
		{
			return SystemColors.HotTrack;
		}
		set
		{
			SetSystemColors(KnownColor.HotTrack, value);
		}
	}

	public virtual Color DefaultControlBackColor
	{
		get
		{
			return ColorControl;
		}
		set
		{
			ColorControl = value;
		}
	}

	public virtual Color DefaultControlForeColor
	{
		get
		{
			return ColorControlText;
		}
		set
		{
			ColorControlText = value;
		}
	}

	public virtual Font DefaultFont => default_font;

	public virtual Color DefaultWindowBackColor => defaultWindowBackColor;

	public virtual Color DefaultWindowForeColor => defaultWindowForeColor;

	public virtual ArrangeDirection ArrangeDirection => ArrangeDirection.Down;

	public virtual ArrangeStartingPosition ArrangeStartingPosition => ArrangeStartingPosition.BottomLeft;

	public virtual int BorderMultiplierFactor => 1;

	public virtual Size BorderSizableSize => new Size(3, 3);

	public virtual Size Border3DSize => XplatUI.Border3DSize;

	public virtual Size BorderStaticSize => new Size(1, 1);

	public virtual Size BorderSize => XplatUI.BorderSize;

	public virtual Size CaptionButtonSize => XplatUI.CaptionButtonSize;

	public virtual int CaptionHeight => XplatUI.CaptionHeight;

	public virtual Size DoubleClickSize => new Size(4, 4);

	public virtual int DoubleClickTime => XplatUI.DoubleClickTime;

	public virtual Size FixedFrameBorderSize => XplatUI.FixedFrameBorderSize;

	public virtual Size FrameBorderSize => XplatUI.FrameBorderSize;

	public virtual int HorizontalFocusThickness => 1;

	public virtual int HorizontalScrollBarArrowWidth => 16;

	public virtual int HorizontalScrollBarHeight => 16;

	public virtual int HorizontalScrollBarThumbWidth => 16;

	public virtual Size IconSpacingSize => new Size(75, 75);

	public virtual bool MenuAccessKeysUnderlined => XplatUI.MenuAccessKeysUnderlined;

	public virtual Size MenuBarButtonSize => XplatUI.MenuBarButtonSize;

	public virtual Size MenuButtonSize => XplatUI.MenuButtonSize;

	public virtual Size MenuCheckSize => new Size(13, 13);

	public virtual Font MenuFont => default_font;

	public virtual int MenuHeight => XplatUI.MenuHeight;

	public virtual int MouseWheelScrollLines => 3;

	public virtual bool RightAlignedMenus => false;

	public virtual Size ToolWindowCaptionButtonSize => XplatUI.ToolWindowCaptionButtonSize;

	public virtual int ToolWindowCaptionHeight => XplatUI.ToolWindowCaptionHeight;

	public virtual int VerticalFocusThickness => 1;

	public virtual int VerticalScrollBarArrowHeight => 16;

	public virtual int VerticalScrollBarThumbHeight => 16;

	public virtual int VerticalScrollBarWidth => 16;

	public virtual Font WindowBorderFont => window_border_font;

	public abstract bool DoubleBufferingSupported { get; }

	public abstract Size ButtonBaseDefaultSize { get; }

	public abstract int DataGridPreferredColumnWidth { get; }

	public abstract int DataGridMinimumColumnCheckBoxHeight { get; }

	public abstract int DataGridMinimumColumnCheckBoxWidth { get; }

	public abstract Color DataGridAlternatingBackColor { get; }

	public abstract Color DataGridBackColor { get; }

	public abstract Color DataGridBackgroundColor { get; }

	public abstract Color DataGridCaptionBackColor { get; }

	public abstract Color DataGridCaptionForeColor { get; }

	public abstract Color DataGridGridLineColor { get; }

	public abstract Color DataGridHeaderBackColor { get; }

	public abstract Color DataGridHeaderForeColor { get; }

	public abstract Color DataGridLinkColor { get; }

	public abstract Color DataGridLinkHoverColor { get; }

	public abstract Color DataGridParentRowsBackColor { get; }

	public abstract Color DataGridParentRowsForeColor { get; }

	public abstract Color DataGridSelectionBackColor { get; }

	public abstract Color DataGridSelectionForeColor { get; }

	public abstract bool DateTimePickerBorderHasHotElementStyle { get; }

	public abstract bool DateTimePickerDropDownButtonHasHotElementStyle { get; }

	public abstract Size GroupBoxDefaultSize { get; }

	public abstract Size HScrollBarDefaultSize { get; }

	public abstract bool ListViewHasHotHeaderStyle { get; }

	public abstract Size ListViewCheckBoxSize { get; }

	public abstract int ListViewColumnHeaderHeight { get; }

	public abstract int ListViewDefaultColumnWidth { get; }

	public abstract int ListViewVerticalSpacing { get; }

	public abstract int ListViewEmptyColumnWidth { get; }

	public abstract int ListViewHorizontalSpacing { get; }

	public abstract Size ListViewDefaultSize { get; }

	public abstract int ListViewGroupHeight { get; }

	public abstract int ListViewItemPaddingWidth { get; }

	public abstract int ListViewTileWidthFactor { get; }

	public abstract int ListViewTileHeightFactor { get; }

	public abstract Size PanelDefaultSize { get; }

	public abstract Size PictureBoxDefaultSize { get; }

	public abstract int PrintPreviewControlPadding { get; }

	public abstract Size ProgressBarDefaultSize { get; }

	public abstract Size RadioButtonDefaultSize { get; }

	public abstract int ScrollBarButtonSize { get; }

	public abstract bool ScrollBarHasHotElementStyles { get; }

	public abstract bool ScrollBarHasPressedThumbStyle { get; }

	public abstract bool ScrollBarHasHoverArrowButtonStyle { get; }

	public abstract int StatusBarSizeGripWidth { get; }

	public abstract int StatusBarHorzGapWidth { get; }

	public abstract Size StatusBarDefaultSize { get; }

	public abstract Size TabControlDefaultItemSize { get; }

	public abstract Point TabControlDefaultPadding { get; }

	public abstract int TabControlMinimumTabWidth { get; }

	public abstract Rectangle TabControlSelectedDelta { get; }

	public abstract int TabControlSelectedSpacing { get; }

	public abstract int TabPanelOffsetX { get; }

	public abstract int TabPanelOffsetY { get; }

	public abstract int TabControlColSpacing { get; }

	public abstract Point TabControlImagePadding { get; }

	public abstract int TabControlScrollerWidth { get; }

	public abstract int ToolBarGripWidth { get; }

	public abstract int ToolBarImageGripWidth { get; }

	public abstract int ToolBarSeparatorWidth { get; }

	public abstract int ToolBarDropDownWidth { get; }

	public abstract int ToolBarDropDownArrowWidth { get; }

	public abstract int ToolBarDropDownArrowHeight { get; }

	public abstract Size ToolBarDefaultSize { get; }

	public abstract bool ToolBarHasHotCheckedElementStyles { get; }

	public abstract bool ToolTipTransparentBackground { get; }

	public abstract Size TrackBarDefaultSize { get; }

	public abstract bool TrackBarHasHotThumbStyle { get; }

	public abstract bool UpDownBaseHasHotButtonStyle { get; }

	public abstract Size VScrollBarDefaultSize { get; }

	public abstract Size TreeViewDefaultSize { get; }

	protected Theme()
	{
		default_font = SystemFonts.DefaultFont;
		syscolors = null;
	}

	private void SetSystemColors(KnownColor kc, Color value)
	{
		if (update == null)
		{
			Type type = Type.GetType("System.Drawing.KnownColors, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
			if (type != null)
			{
				update = type.GetMethod("Update", BindingFlags.Static | BindingFlags.Public);
			}
		}
		if (update != null)
		{
			update.Invoke(null, new object[2]
			{
				(int)kc,
				value.ToArgb()
			});
		}
	}

	public virtual Color GetColor(XplatUIWin32.GetSysColorIndex idx)
	{
		return (Color)syscolors.GetValue((int)idx);
	}

	public virtual void SetColor(XplatUIWin32.GetSysColorIndex idx, Color color)
	{
		syscolors.SetValue(color, (int)idx);
	}

	public int Clamp(int value, int lower, int upper)
	{
		if (value < lower)
		{
			return lower;
		}
		if (value > upper)
		{
			return upper;
		}
		return value;
	}

	[System.MonoInternalNote("Figure out where to point for My Network Places")]
	public virtual string Places(UIIcon index)
	{
		return index switch
		{
			UIIcon.PlacesRecentDocuments => Environment.GetFolderPath(Environment.SpecialFolder.Recent), 
			UIIcon.PlacesDesktop => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), 
			UIIcon.PlacesPersonal => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
			UIIcon.PlacesMyComputer => Environment.GetFolderPath(Environment.SpecialFolder.MyComputer), 
			UIIcon.PlacesMyNetwork => "/tmp", 
			_ => throw new ArgumentOutOfRangeException("index", index, "Unsupported place"), 
		};
	}

	private Image GetSizedResourceImage(string name, int width)
	{
		Image uIImage = ResPool.GetUIImage(name, width);
		if (uIImage != null)
		{
			return uIImage;
		}
		if (width > 0)
		{
			string name2 = string.Format("{1}_{0}", name, width);
			uIImage = ResourceImageLoader.Get(name2);
			if (uIImage != null)
			{
				ResPool.AddUIImage(uIImage, name, width);
				return uIImage;
			}
		}
		uIImage = ResourceImageLoader.Get(name);
		if (uIImage == null)
		{
			return null;
		}
		ResPool.AddUIImage(uIImage, name, 0);
		if (uIImage.Width != width && width != 0)
		{
			Console.Error.WriteLine("warning: requesting icon that not been tuned {0}_{1} {2}", width, name, uIImage.Width);
			int height = uIImage.Height * width / uIImage.Width;
			Bitmap bitmap = new Bitmap(width, height);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.DrawImage(uIImage, 0, 0, width, height);
			ResPool.AddUIImage(bitmap, name, width);
			return bitmap;
		}
		return uIImage;
	}

	public virtual Image Images(UIIcon index)
	{
		return Images(index, 0);
	}

	public virtual Image Images(UIIcon index, int size)
	{
		return index switch
		{
			UIIcon.PlacesRecentDocuments => GetSizedResourceImage("document-open.png", size), 
			UIIcon.PlacesDesktop => GetSizedResourceImage("user-desktop.png", size), 
			UIIcon.PlacesPersonal => GetSizedResourceImage("user-home.png", size), 
			UIIcon.PlacesMyComputer => GetSizedResourceImage("computer.png", size), 
			UIIcon.PlacesMyNetwork => GetSizedResourceImage("folder-remote.png", size), 
			UIIcon.MessageBoxError => GetSizedResourceImage("dialog-error.png", size), 
			UIIcon.MessageBoxInfo => GetSizedResourceImage("dialog-information.png", size), 
			UIIcon.MessageBoxQuestion => GetSizedResourceImage("dialog-question.png", size), 
			UIIcon.MessageBoxWarning => GetSizedResourceImage("dialog-warning.png", size), 
			UIIcon.NormalFolder => GetSizedResourceImage("folder.png", size), 
			_ => throw new ArgumentException("Invalid Icon type requested", "index"), 
		};
	}

	public virtual Image Images(string mimetype, string extension, int size)
	{
		return null;
	}

	public abstract void ResetDefaults();

	public abstract void DrawOwnerDrawBackground(DrawItemEventArgs e);

	public abstract void DrawOwnerDrawFocusRectangle(DrawItemEventArgs e);

	public abstract Size CalculateButtonAutoSize(Button button);

	public abstract void CalculateButtonTextAndImageLayout(ButtonBase b, out Rectangle textRectangle, out Rectangle imageRectangle);

	public abstract void DrawButton(Graphics g, Button b, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle);

	public abstract void DrawFlatButton(Graphics g, ButtonBase b, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle);

	public abstract void DrawPopupButton(Graphics g, Button b, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle);

	public abstract void DrawButtonBase(Graphics dc, Rectangle clip_area, ButtonBase button);

	public abstract Size CalculateCheckBoxAutoSize(CheckBox checkBox);

	public abstract void CalculateCheckBoxTextAndImageLayout(ButtonBase b, Point offset, out Rectangle glyphArea, out Rectangle textRectangle, out Rectangle imageRectangle);

	public abstract void DrawCheckBox(Graphics g, CheckBox cb, Rectangle glyphArea, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle);

	public abstract void DrawCheckBox(Graphics dc, Rectangle clip_area, CheckBox checkbox);

	public abstract void DrawCheckedListBoxItem(CheckedListBox ctrl, DrawItemEventArgs e);

	public abstract void DrawComboBoxItem(ComboBox ctrl, DrawItemEventArgs e);

	public abstract void DrawFlatStyleComboButton(Graphics graphics, Rectangle rectangle, ButtonState state);

	public abstract void ComboBoxDrawNormalDropDownButton(ComboBox comboBox, Graphics g, Rectangle clippingArea, Rectangle area, ButtonState state);

	public abstract bool ComboBoxNormalDropDownButtonHasTransparentBackground(ComboBox comboBox, ButtonState state);

	public abstract bool ComboBoxDropDownButtonHasHotElementStyle(ComboBox comboBox);

	public abstract void ComboBoxDrawBackground(ComboBox comboBox, Graphics g, Rectangle clippingArea, FlatStyle style);

	public abstract bool CombBoxBackgroundHasHotElementStyle(ComboBox comboBox);

	public abstract Font GetLinkFont(Control control);

	public abstract void DataGridPaint(PaintEventArgs pe, DataGrid grid);

	public abstract void DataGridPaintCaption(Graphics g, Rectangle clip, DataGrid grid);

	public abstract void DataGridPaintColumnHeaders(Graphics g, Rectangle clip, DataGrid grid);

	public abstract void DataGridPaintColumnHeader(Graphics g, Rectangle bounds, DataGrid grid, int col);

	public abstract void DataGridPaintRowContents(Graphics g, int row, Rectangle row_rect, bool is_newrow, Rectangle clip, DataGrid grid);

	public abstract void DataGridPaintRowHeader(Graphics g, Rectangle bounds, int row, DataGrid grid);

	public abstract void DataGridPaintRowHeaderArrow(Graphics g, Rectangle bounds, DataGrid grid);

	public abstract void DataGridPaintRowHeaderStar(Graphics g, Rectangle bounds, DataGrid grid);

	public abstract void DataGridPaintParentRows(Graphics g, Rectangle bounds, DataGrid grid);

	public abstract void DataGridPaintParentRow(Graphics g, Rectangle bounds, DataGridDataSource row, DataGrid grid);

	public abstract void DataGridPaintRows(Graphics g, Rectangle cells, Rectangle clip, DataGrid grid);

	public abstract void DataGridPaintRow(Graphics g, int row, Rectangle row_rect, bool is_newrow, Rectangle clip, DataGrid grid);

	public abstract void DataGridPaintRelationRow(Graphics g, int row, Rectangle row_rect, bool is_newrow, Rectangle clip, DataGrid grid);

	public abstract bool DataGridViewRowHeaderCellDrawBackground(DataGridViewRowHeaderCell cell, Graphics g, Rectangle bounds);

	public abstract bool DataGridViewRowHeaderCellDrawSelectionBackground(DataGridViewRowHeaderCell cell);

	public abstract bool DataGridViewRowHeaderCellDrawBorder(DataGridViewRowHeaderCell cell, Graphics g, Rectangle bounds);

	public abstract bool DataGridViewColumnHeaderCellDrawBackground(DataGridViewColumnHeaderCell cell, Graphics g, Rectangle bounds);

	public abstract bool DataGridViewColumnHeaderCellDrawBorder(DataGridViewColumnHeaderCell cell, Graphics g, Rectangle bounds);

	public abstract bool DataGridViewHeaderCellHasPressedStyle(DataGridView dataGridView);

	public abstract bool DataGridViewHeaderCellHasHotStyle(DataGridView dataGridView);

	public abstract void DrawDateTimePicker(Graphics dc, Rectangle clip_rectangle, DateTimePicker dtp);

	public abstract Rectangle DateTimePickerGetDropDownButtonArea(DateTimePicker dateTimePicker);

	public abstract Rectangle DateTimePickerGetDateArea(DateTimePicker dateTimePicker);

	public abstract void DrawGroupBox(Graphics dc, Rectangle clip_area, GroupBox box);

	public abstract void DrawListBoxItem(ListBox ctrl, DrawItemEventArgs e);

	public abstract void DrawListViewItems(Graphics dc, Rectangle clip_rectangle, ListView control);

	public abstract void DrawListViewHeader(Graphics dc, Rectangle clip_rectangle, ListView control);

	public abstract void DrawListViewHeaderDragDetails(Graphics dc, ListView control, ColumnHeader drag_column, int target_x);

	public abstract int ListViewGetHeaderHeight(ListView listView, Font font);

	public abstract void CalcItemSize(Graphics dc, MenuItem item, int y, int x, bool menuBar);

	public abstract void CalcPopupMenuSize(Graphics dc, Menu menu);

	public abstract int CalcMenuBarSize(Graphics dc, Menu menu, int width);

	public abstract void DrawMenuBar(Graphics dc, Menu menu, Rectangle rect);

	public abstract void DrawMenuItem(MenuItem item, DrawItemEventArgs e);

	public abstract void DrawPopupMenu(Graphics dc, Menu menu, Rectangle cliparea, Rectangle rect);

	public abstract void DrawMonthCalendar(Graphics dc, Rectangle clip_rectangle, MonthCalendar month_calendar);

	public abstract void DrawPictureBox(Graphics dc, Rectangle clip, PictureBox pb);

	public abstract Size PrintPreviewControlGetPageSize(PrintPreviewControl preview);

	public abstract void PrintPreviewControlPaint(PaintEventArgs pe, PrintPreviewControl preview, Size page_image_size);

	public abstract void DrawProgressBar(Graphics dc, Rectangle clip_rectangle, ProgressBar progress_bar);

	public abstract Size CalculateRadioButtonAutoSize(RadioButton rb);

	public abstract void CalculateRadioButtonTextAndImageLayout(ButtonBase b, Point offset, out Rectangle glyphArea, out Rectangle textRectangle, out Rectangle imageRectangle);

	public abstract void DrawRadioButton(Graphics g, RadioButton rb, Rectangle glyphArea, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle);

	public abstract void DrawRadioButton(Graphics dc, Rectangle clip_rectangle, RadioButton radio_button);

	public abstract void DrawScrollBar(Graphics dc, Rectangle clip_rectangle, ScrollBar bar);

	public abstract void DrawStatusBar(Graphics dc, Rectangle clip_rectangle, StatusBar sb);

	public abstract Rectangle TabControlGetDisplayRectangle(TabControl tab);

	public abstract Rectangle TabControlGetPanelRect(TabControl tab);

	public abstract Size TabControlGetSpacing(TabControl tab);

	public abstract void DrawTabControl(Graphics dc, Rectangle area, TabControl tab);

	public abstract void TextBoxBaseFillBackground(TextBoxBase textBoxBase, Graphics g, Rectangle clippingArea);

	public abstract bool TextBoxBaseHandleWmNcPaint(TextBoxBase textBoxBase, ref Message m);

	public abstract bool TextBoxBaseShouldPaintBackground(TextBoxBase textBoxBase);

	public abstract void DrawToolBar(Graphics dc, Rectangle clip_rectangle, ToolBar control);

	public abstract bool ToolBarHasHotElementStyles(ToolBar toolBar);

	public abstract void DrawToolTip(Graphics dc, Rectangle clip_rectangle, ToolTip.ToolTipWindow control);

	public abstract Size ToolTipSize(ToolTip.ToolTipWindow tt, string text);

	public abstract void ShowBalloonWindow(IntPtr handle, int timeout, string title, string text, ToolTipIcon icon);

	public abstract void DrawBalloonWindow(Graphics dc, Rectangle clip, NotifyIcon.BalloonWindow control);

	public abstract Rectangle BalloonWindowRect(NotifyIcon.BalloonWindow control);

	public abstract void DrawTrackBar(Graphics dc, Rectangle clip_rectangle, TrackBar tb);

	public abstract int TrackBarValueFromMousePosition(int x, int y, TrackBar tb);

	public abstract void UpDownBaseDrawButton(Graphics g, Rectangle bounds, bool top, PushButtonState state);

	public abstract void TreeViewDrawNodePlusMinus(TreeView treeView, TreeNode node, Graphics dc, int x, int middle);

	public abstract void DrawManagedWindowDecorations(Graphics dc, Rectangle clip, InternalWindowManager wm);

	public abstract int ManagedWindowTitleBarHeight(InternalWindowManager wm);

	public abstract int ManagedWindowBorderWidth(InternalWindowManager wm);

	public abstract int ManagedWindowIconWidth(InternalWindowManager wm);

	public abstract Size ManagedWindowButtonSize(InternalWindowManager wm);

	public abstract void ManagedWindowSetButtonLocations(InternalWindowManager wm);

	public abstract Rectangle ManagedWindowGetTitleBarIconArea(InternalWindowManager wm);

	public abstract Size ManagedWindowGetMenuButtonSize(InternalWindowManager wm);

	public abstract bool ManagedWindowTitleButtonHasHotElementStyle(TitleButton button, Form form);

	public abstract void ManagedWindowDrawMenuButton(Graphics dc, TitleButton button, Rectangle clip, InternalWindowManager wm);

	public abstract void ManagedWindowOnSizeInitializedOrChanged(Form form);

	public abstract void CPDrawBorder(Graphics graphics, Rectangle bounds, Color leftColor, int leftWidth, ButtonBorderStyle leftStyle, Color topColor, int topWidth, ButtonBorderStyle topStyle, Color rightColor, int rightWidth, ButtonBorderStyle rightStyle, Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle);

	public abstract void CPDrawBorder(Graphics graphics, RectangleF bounds, Color leftColor, int leftWidth, ButtonBorderStyle leftStyle, Color topColor, int topWidth, ButtonBorderStyle topStyle, Color rightColor, int rightWidth, ButtonBorderStyle rightStyle, Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle);

	public abstract void CPDrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style, Border3DSide sides);

	public abstract void CPDrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style, Border3DSide sides, Color control_color);

	public abstract void CPDrawButton(Graphics graphics, Rectangle rectangle, ButtonState state);

	public abstract void CPDrawCaptionButton(Graphics graphics, Rectangle rectangle, CaptionButton button, ButtonState state);

	public abstract void CPDrawCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state);

	public abstract void CPDrawComboButton(Graphics graphics, Rectangle rectangle, ButtonState state);

	public abstract void CPDrawContainerGrabHandle(Graphics graphics, Rectangle bounds);

	public abstract void CPDrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color foreColor, Color backColor);

	public abstract void CPDrawGrabHandle(Graphics graphics, Rectangle rectangle, bool primary, bool enabled);

	public abstract void CPDrawGrid(Graphics graphics, Rectangle area, Size pixelsBetweenDots, Color backColor);

	public abstract void CPDrawImageDisabled(Graphics graphics, Image image, int x, int y, Color background);

	public abstract void CPDrawLockedFrame(Graphics graphics, Rectangle rectangle, bool primary);

	public abstract void CPDrawMenuGlyph(Graphics graphics, Rectangle rectangle, MenuGlyph glyph, Color color, Color backColor);

	public abstract void CPDrawMixedCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state);

	public abstract void CPDrawRadioButton(Graphics graphics, Rectangle rectangle, ButtonState state);

	public abstract void CPDrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style);

	public abstract void CPDrawReversibleLine(Point start, Point end, Color backColor);

	public abstract void CPDrawScrollButton(Graphics graphics, Rectangle rectangle, ScrollButton button, ButtonState state);

	public abstract void CPDrawSelectionFrame(Graphics graphics, bool active, Rectangle outsideRect, Rectangle insideRect, Color backColor);

	public abstract void CPDrawSizeGrip(Graphics graphics, Color backColor, Rectangle bounds);

	public abstract void CPDrawStringDisabled(Graphics graphics, string s, Font font, Color color, RectangleF layoutRectangle, StringFormat format);

	public abstract void CPDrawStringDisabled(IDeviceContext dc, string s, Font font, Color color, Rectangle layoutRectangle, TextFormatFlags format);

	public abstract void CPDrawVisualStyleBorder(Graphics graphics, Rectangle bounds);

	public abstract void CPDrawBorderStyle(Graphics dc, Rectangle area, BorderStyle border_style);
}
