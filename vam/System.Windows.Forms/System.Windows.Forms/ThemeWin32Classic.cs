using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms.Theming;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

internal class ThemeWin32Classic : Theme
{
	private enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}

	protected interface ITrackBarTickPainter
	{
		void Paint(float x1, float y1, float x2, float y2);
	}

	private class TrackBarTickPainter : ITrackBarTickPainter
	{
		private readonly Graphics g;

		private readonly Pen pen;

		public TrackBarTickPainter(Graphics g, Pen pen)
		{
			this.g = g;
			this.pen = pen;
		}

		public void Paint(float x1, float y1, float x2, float y2)
		{
			g.DrawLine(pen, x1, y1, x2, y2);
		}
	}

	private const int SEPARATOR_HEIGHT = 6;

	private const int SEPARATOR_MIN_WIDTH = 20;

	private const int SM_CXBORDER = 1;

	private const int SM_CYBORDER = 1;

	private const int MENU_TAB_SPACE = 8;

	private const int MENU_BAR_ITEMS_SPACE = 8;

	public const int ProgressBarChunkSpacing = 2;

	private const int ProgressBarDefaultHeight = 23;

	private const Border3DSide all_sides = Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom;

	private const int balloon_iconsize = 16;

	private const int balloon_bordersize = 8;

	public const int TrackBarVerticalTrackWidth = 4;

	public const int TrackBarHorizontalTrackHeight = 4;

	protected static readonly Color arrow_color = Color.Black;

	protected static readonly Color pen_ticks_color = Color.Black;

	protected static StringFormat string_format_menu_text;

	protected static StringFormat string_format_menu_shortcut;

	protected static StringFormat string_format_menu_menubar_text;

	private static ImageAttributes imagedisabled_attributes = null;

	private NotifyIcon.BalloonWindow balloon_window;

	public override Version Version => new Version(0, 1, 0, 0);

	public override bool DoubleBufferingSupported => true;

	public override int HorizontalScrollBarHeight => XplatUI.HorizontalScrollBarHeight;

	public override int VerticalScrollBarWidth => XplatUI.VerticalScrollBarWidth;

	public override Size ButtonBaseDefaultSize => new Size(75, 23);

	public override int DataGridPreferredColumnWidth => 75;

	public override int DataGridMinimumColumnCheckBoxHeight => 16;

	public override int DataGridMinimumColumnCheckBoxWidth => 16;

	public override Color DataGridAlternatingBackColor => ColorWindow;

	public override Color DataGridBackColor => ColorWindow;

	public override Color DataGridBackgroundColor => ColorAppWorkspace;

	public override Color DataGridCaptionBackColor => ColorActiveCaption;

	public override Color DataGridCaptionForeColor => ColorActiveCaptionText;

	public override Color DataGridGridLineColor => ColorControl;

	public override Color DataGridHeaderBackColor => ColorControl;

	public override Color DataGridHeaderForeColor => ColorControlText;

	public override Color DataGridLinkColor => ColorHotTrack;

	public override Color DataGridLinkHoverColor => ColorHotTrack;

	public override Color DataGridParentRowsBackColor => ColorControl;

	public override Color DataGridParentRowsForeColor => ColorWindowText;

	public override Color DataGridSelectionBackColor => ColorActiveCaption;

	public override Color DataGridSelectionForeColor => ColorActiveCaptionText;

	public override bool DateTimePickerBorderHasHotElementStyle => false;

	public override bool DateTimePickerDropDownButtonHasHotElementStyle => false;

	public override Size GroupBoxDefaultSize => new Size(200, 100);

	public override Size HScrollBarDefaultSize => new Size(80, ScrollBarButtonSize);

	public override bool ListViewHasHotHeaderStyle => false;

	public override Size ListViewCheckBoxSize => new Size(16, 16);

	public override int ListViewColumnHeaderHeight => 16;

	public override int ListViewDefaultColumnWidth => 60;

	public override int ListViewVerticalSpacing => 22;

	public override int ListViewEmptyColumnWidth => 10;

	public override int ListViewHorizontalSpacing => 4;

	public override int ListViewItemPaddingWidth => 6;

	public override Size ListViewDefaultSize => new Size(121, 97);

	public override int ListViewGroupHeight => 20;

	public int ListViewGroupLineWidth => 200;

	public override int ListViewTileWidthFactor => 22;

	public override int ListViewTileHeightFactor => 3;

	public override Size PanelDefaultSize => new Size(200, 100);

	public override Size PictureBoxDefaultSize => new Size(100, 50);

	public override int PrintPreviewControlPadding => 8;

	public override Size ProgressBarDefaultSize => new Size(100, 23);

	public override Size RadioButtonDefaultSize => new Size(104, 24);

	public override int ScrollBarButtonSize => 16;

	public override bool ScrollBarHasHotElementStyles => false;

	public override bool ScrollBarHasPressedThumbStyle => false;

	public override bool ScrollBarHasHoverArrowButtonStyle => false;

	public override int StatusBarSizeGripWidth => 15;

	public override int StatusBarHorzGapWidth => 3;

	public override Size StatusBarDefaultSize => new Size(100, 22);

	public override Size TabControlDefaultItemSize => ThemeElements.CurrentTheme.TabControlPainter.DefaultItemSize;

	public override Point TabControlDefaultPadding => ThemeElements.CurrentTheme.TabControlPainter.DefaultPadding;

	public override int TabControlMinimumTabWidth => ThemeElements.CurrentTheme.TabControlPainter.MinimumTabWidth;

	public override Rectangle TabControlSelectedDelta => ThemeElements.CurrentTheme.TabControlPainter.SelectedTabDelta;

	public override int TabControlSelectedSpacing => ThemeElements.CurrentTheme.TabControlPainter.SelectedSpacing;

	public override int TabPanelOffsetX => ThemeElements.CurrentTheme.TabControlPainter.TabPanelOffset.X;

	public override int TabPanelOffsetY => ThemeElements.CurrentTheme.TabControlPainter.TabPanelOffset.Y;

	public override int TabControlColSpacing => ThemeElements.CurrentTheme.TabControlPainter.ColSpacing;

	public override Point TabControlImagePadding => ThemeElements.CurrentTheme.TabControlPainter.ImagePadding;

	public override int TabControlScrollerWidth => ThemeElements.CurrentTheme.TabControlPainter.ScrollerWidth;

	public override int ToolBarGripWidth => 2;

	public override int ToolBarImageGripWidth => 2;

	public override int ToolBarSeparatorWidth => 4;

	public override int ToolBarDropDownWidth => 13;

	public override int ToolBarDropDownArrowWidth => 5;

	public override int ToolBarDropDownArrowHeight => 3;

	public override Size ToolBarDefaultSize => new Size(100, 42);

	public override bool ToolBarHasHotCheckedElementStyles => false;

	public override bool ToolTipTransparentBackground => false;

	public override Size TrackBarDefaultSize => new Size(104, 42);

	public override bool TrackBarHasHotThumbStyle => false;

	public override bool UpDownBaseHasHotButtonStyle => false;

	public override Size VScrollBarDefaultSize => new Size(ScrollBarButtonSize, 80);

	public override Size TreeViewDefaultSize => new Size(121, 97);

	public ThemeWin32Classic()
	{
		defaultWindowBackColor = ColorWindow;
		defaultWindowForeColor = ColorControlText;
		window_border_font = new Font(FontFamily.GenericSansSerif, 8.25f, FontStyle.Bold);
		string_format_menu_text = new StringFormat();
		string_format_menu_text.LineAlignment = StringAlignment.Center;
		string_format_menu_text.Alignment = StringAlignment.Near;
		string_format_menu_text.HotkeyPrefix = HotkeyPrefix.Show;
		string_format_menu_text.SetTabStops(0f, new float[1] { 50f });
		string_format_menu_text.FormatFlags |= StringFormatFlags.NoWrap;
		string_format_menu_shortcut = new StringFormat();
		string_format_menu_shortcut.LineAlignment = StringAlignment.Center;
		string_format_menu_shortcut.Alignment = StringAlignment.Far;
		string_format_menu_menubar_text = new StringFormat();
		string_format_menu_menubar_text.LineAlignment = StringAlignment.Center;
		string_format_menu_menubar_text.Alignment = StringAlignment.Center;
		string_format_menu_menubar_text.HotkeyPrefix = HotkeyPrefix.Show;
	}

	public override void ResetDefaults()
	{
		Console.WriteLine("NOT IMPLEMENTED: ResetDefault()");
	}

	protected Brush GetControlBackBrush(Color c)
	{
		if (c.ToArgb() == DefaultControlBackColor.ToArgb())
		{
			return SystemBrushes.Control;
		}
		return ResPool.GetSolidBrush(c);
	}

	protected Brush GetControlForeBrush(Color c)
	{
		if (c.ToArgb() == DefaultControlForeColor.ToArgb())
		{
			return SystemBrushes.ControlText;
		}
		return ResPool.GetSolidBrush(c);
	}

	public override Font GetLinkFont(Control control)
	{
		return new Font(control.Font.FontFamily, control.Font.Size, control.Font.Style | FontStyle.Underline, control.Font.Unit);
	}

	public override void DrawOwnerDrawBackground(DrawItemEventArgs e)
	{
		if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
		{
			e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
		}
		else
		{
			e.Graphics.FillRectangle(ResPool.GetSolidBrush(e.BackColor), e.Bounds);
		}
	}

	public override void DrawOwnerDrawFocusRectangle(DrawItemEventArgs e)
	{
		if (e.State == DrawItemState.Focus)
		{
			CPDrawFocusRectangle(e.Graphics, e.Bounds, e.ForeColor, e.BackColor);
		}
	}

	public override void DrawButton(Graphics g, Button b, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle)
	{
		DrawButtonBackground(g, b, clipRectangle);
		if (imageBounds.Size != Size.Empty)
		{
			DrawButtonImage(g, b, imageBounds);
		}
		if (b.Focused && b.Enabled && b.ShowFocusCues)
		{
			DrawButtonFocus(g, b);
		}
		if (textBounds != Rectangle.Empty)
		{
			DrawButtonText(g, b, textBounds);
		}
	}

	public virtual void DrawButtonBackground(Graphics g, Button button, Rectangle clipArea)
	{
		if (button.Pressed)
		{
			ThemeElements.DrawButton(g, button.ClientRectangle, ButtonThemeState.Pressed, button.BackColor, button.ForeColor);
		}
		else if (button.InternalSelected)
		{
			ThemeElements.DrawButton(g, button.ClientRectangle, ButtonThemeState.Default, button.BackColor, button.ForeColor);
		}
		else if (button.Entered)
		{
			ThemeElements.DrawButton(g, button.ClientRectangle, ButtonThemeState.Entered, button.BackColor, button.ForeColor);
		}
		else if (!button.Enabled)
		{
			ThemeElements.DrawButton(g, button.ClientRectangle, ButtonThemeState.Disabled, button.BackColor, button.ForeColor);
		}
		else
		{
			ThemeElements.DrawButton(g, button.ClientRectangle, ButtonThemeState.Normal, button.BackColor, button.ForeColor);
		}
	}

	public virtual void DrawButtonFocus(Graphics g, Button button)
	{
		ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(button.ClientRectangle, -4, -4));
	}

	public virtual void DrawButtonImage(Graphics g, ButtonBase button, Rectangle imageBounds)
	{
		if (button.Enabled)
		{
			g.DrawImage(button.Image, imageBounds);
		}
		else
		{
			CPDrawImageDisabled(g, button.Image, imageBounds.Left, imageBounds.Top, ColorControl);
		}
	}

	public virtual void DrawButtonText(Graphics g, ButtonBase button, Rectangle textBounds)
	{
		textBounds.Height = Math.Max(textBounds.Height, button.Font.Height);
		if (button.Enabled)
		{
			TextRenderer.DrawTextInternal(g, button.Text, button.Font, textBounds, button.ForeColor, button.TextFormatFlags, button.UseCompatibleTextRendering);
		}
		else
		{
			DrawStringDisabled20(g, button.Text, button.Font, textBounds, button.BackColor, button.TextFormatFlags, button.UseCompatibleTextRendering);
		}
	}

	public override void DrawFlatButton(Graphics g, ButtonBase b, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle)
	{
		if (b.BackgroundImage == null)
		{
			DrawFlatButtonBackground(g, b, clipRectangle);
		}
		if (imageBounds.Size != Size.Empty)
		{
			DrawFlatButtonImage(g, b, imageBounds);
		}
		if (b.Focused && b.Enabled && b.ShowFocusCues)
		{
			DrawFlatButtonFocus(g, b);
		}
		if (textBounds != Rectangle.Empty)
		{
			DrawFlatButtonText(g, b, textBounds);
		}
	}

	public virtual void DrawFlatButtonBackground(Graphics g, ButtonBase button, Rectangle clipArea)
	{
		if (button.Pressed)
		{
			ThemeElements.DrawFlatButton(g, button.ClientRectangle, ButtonThemeState.Pressed, button.BackColor, button.ForeColor, button.FlatAppearance);
		}
		else if (button.InternalSelected)
		{
			if (button.Entered)
			{
				ThemeElements.DrawFlatButton(g, button.ClientRectangle, ButtonThemeState.Entered | ButtonThemeState.Default, button.BackColor, button.ForeColor, button.FlatAppearance);
			}
			else
			{
				ThemeElements.DrawFlatButton(g, button.ClientRectangle, ButtonThemeState.Default, button.BackColor, button.ForeColor, button.FlatAppearance);
			}
		}
		else if (button.Entered)
		{
			ThemeElements.DrawFlatButton(g, button.ClientRectangle, ButtonThemeState.Entered, button.BackColor, button.ForeColor, button.FlatAppearance);
		}
		else if (!button.Enabled)
		{
			ThemeElements.DrawFlatButton(g, button.ClientRectangle, ButtonThemeState.Disabled, button.BackColor, button.ForeColor, button.FlatAppearance);
		}
		else
		{
			ThemeElements.DrawFlatButton(g, button.ClientRectangle, ButtonThemeState.Normal, button.BackColor, button.ForeColor, button.FlatAppearance);
		}
	}

	public virtual void DrawFlatButtonFocus(Graphics g, ButtonBase button)
	{
		if (!button.Pressed)
		{
			Color color = ControlPaint.Dark(button.BackColor);
			g.DrawRectangle(ResPool.GetPen(color), new Rectangle(button.ClientRectangle.Left + 4, button.ClientRectangle.Top + 4, button.ClientRectangle.Width - 9, button.ClientRectangle.Height - 9));
		}
	}

	public virtual void DrawFlatButtonImage(Graphics g, ButtonBase button, Rectangle imageBounds)
	{
		DrawButtonImage(g, button, imageBounds);
	}

	public virtual void DrawFlatButtonText(Graphics g, ButtonBase button, Rectangle textBounds)
	{
		DrawButtonText(g, button, textBounds);
	}

	public override void DrawPopupButton(Graphics g, Button b, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle)
	{
		DrawPopupButtonBackground(g, b, clipRectangle);
		if (imageBounds.Size != Size.Empty)
		{
			DrawPopupButtonImage(g, b, imageBounds);
		}
		if (b.Focused && b.Enabled && b.ShowFocusCues)
		{
			DrawPopupButtonFocus(g, b);
		}
		if (textBounds != Rectangle.Empty)
		{
			DrawPopupButtonText(g, b, textBounds);
		}
	}

	public virtual void DrawPopupButtonBackground(Graphics g, Button button, Rectangle clipArea)
	{
		if (button.Pressed)
		{
			ThemeElements.DrawPopupButton(g, button.ClientRectangle, ButtonThemeState.Pressed, button.BackColor, button.ForeColor);
		}
		else if (button.Entered)
		{
			ThemeElements.DrawPopupButton(g, button.ClientRectangle, ButtonThemeState.Entered, button.BackColor, button.ForeColor);
		}
		else if (button.InternalSelected)
		{
			ThemeElements.DrawPopupButton(g, button.ClientRectangle, ButtonThemeState.Default, button.BackColor, button.ForeColor);
		}
		else if (!button.Enabled)
		{
			ThemeElements.DrawPopupButton(g, button.ClientRectangle, ButtonThemeState.Disabled, button.BackColor, button.ForeColor);
		}
		else
		{
			ThemeElements.DrawPopupButton(g, button.ClientRectangle, ButtonThemeState.Normal, button.BackColor, button.ForeColor);
		}
	}

	public virtual void DrawPopupButtonFocus(Graphics g, Button button)
	{
		DrawButtonFocus(g, button);
	}

	public virtual void DrawPopupButtonImage(Graphics g, Button button, Rectangle imageBounds)
	{
		DrawButtonImage(g, button, imageBounds);
	}

	public virtual void DrawPopupButtonText(Graphics g, Button button, Rectangle textBounds)
	{
		DrawButtonText(g, button, textBounds);
	}

	public override Size CalculateButtonAutoSize(Button button)
	{
		Size empty = Size.Empty;
		Size size = TextRenderer.MeasureTextInternal(button.Text, button.Font, button.UseCompatibleTextRendering);
		Size size2 = ((button.Image != null) ? button.Image.Size : Size.Empty);
		if (button.Text.Length != 0)
		{
			size.Height += 4;
			size.Width += 4;
		}
		switch (button.TextImageRelation)
		{
		case TextImageRelation.Overlay:
			empty.Height = Math.Max((button.Text.Length != 0) ? size.Height : 0, size2.Height);
			empty.Width = Math.Max(size.Width, size2.Width);
			break;
		case TextImageRelation.ImageAboveText:
		case TextImageRelation.TextAboveImage:
			empty.Height = size.Height + size2.Height;
			empty.Width = Math.Max(size.Width, size2.Width);
			break;
		case TextImageRelation.ImageBeforeText:
		case TextImageRelation.TextBeforeImage:
			empty.Height = Math.Max(size.Height, size2.Height);
			empty.Width = size.Width + size2.Width;
			break;
		}
		empty.Height += button.Padding.Vertical + 6;
		empty.Width += button.Padding.Horizontal + 6;
		return empty;
	}

	public override void CalculateButtonTextAndImageLayout(ButtonBase button, out Rectangle textRectangle, out Rectangle imageRectangle)
	{
		Image image = button.Image;
		string text = button.Text;
		Rectangle clientRectangle = button.ClientRectangle;
		Size textSize = TextRenderer.MeasureTextInternal(text, button.Font, clientRectangle.Size, button.TextFormatFlags, button.UseCompatibleTextRendering);
		Size imageSize = image?.Size ?? Size.Empty;
		textRectangle = Rectangle.Empty;
		imageRectangle = Rectangle.Empty;
		switch (button.TextImageRelation)
		{
		case TextImageRelation.Overlay:
			textRectangle = Rectangle.Inflate(clientRectangle, -4, -4);
			if (button.Pressed)
			{
				textRectangle.Offset(1, 1);
			}
			if (image != null)
			{
				int num = 0;
				int num2 = 0;
				int height = image.Height;
				int width = image.Width;
				switch (button.ImageAlign)
				{
				case System.Drawing.ContentAlignment.TopLeft:
					num = 5;
					num2 = 5;
					break;
				case System.Drawing.ContentAlignment.TopCenter:
					num = (clientRectangle.Width - width) / 2;
					num2 = 5;
					break;
				case System.Drawing.ContentAlignment.TopRight:
					num = clientRectangle.Width - width - 5;
					num2 = 5;
					break;
				case System.Drawing.ContentAlignment.MiddleLeft:
					num = 5;
					num2 = (clientRectangle.Height - height) / 2;
					break;
				case System.Drawing.ContentAlignment.MiddleCenter:
					num = (clientRectangle.Width - width) / 2;
					num2 = (clientRectangle.Height - height) / 2;
					break;
				case System.Drawing.ContentAlignment.MiddleRight:
					num = clientRectangle.Width - width - 4;
					num2 = (clientRectangle.Height - height) / 2;
					break;
				case System.Drawing.ContentAlignment.BottomLeft:
					num = 5;
					num2 = clientRectangle.Height - height - 4;
					break;
				case System.Drawing.ContentAlignment.BottomCenter:
					num = (clientRectangle.Width - width) / 2;
					num2 = clientRectangle.Height - height - 4;
					break;
				case System.Drawing.ContentAlignment.BottomRight:
					num = clientRectangle.Width - width - 4;
					num2 = clientRectangle.Height - height - 4;
					break;
				default:
					num = 5;
					num2 = 5;
					break;
				}
				imageRectangle = new Rectangle(num, num2, width, height);
			}
			break;
		case TextImageRelation.ImageAboveText:
			clientRectangle.Inflate(-4, -4);
			LayoutTextAboveOrBelowImage(clientRectangle, textFirst: false, textSize, imageSize, button.TextAlign, button.ImageAlign, out textRectangle, out imageRectangle);
			break;
		case TextImageRelation.TextAboveImage:
			clientRectangle.Inflate(-4, -4);
			LayoutTextAboveOrBelowImage(clientRectangle, textFirst: true, textSize, imageSize, button.TextAlign, button.ImageAlign, out textRectangle, out imageRectangle);
			break;
		case TextImageRelation.ImageBeforeText:
			clientRectangle.Inflate(-4, -4);
			LayoutTextBeforeOrAfterImage(clientRectangle, textFirst: false, textSize, imageSize, button.TextAlign, button.ImageAlign, out textRectangle, out imageRectangle);
			break;
		case TextImageRelation.TextBeforeImage:
			clientRectangle.Inflate(-4, -4);
			LayoutTextBeforeOrAfterImage(clientRectangle, textFirst: true, textSize, imageSize, button.TextAlign, button.ImageAlign, out textRectangle, out imageRectangle);
			break;
		case (TextImageRelation)3:
		case (TextImageRelation)5:
		case (TextImageRelation)6:
		case (TextImageRelation)7:
			break;
		}
	}

	private void LayoutTextBeforeOrAfterImage(Rectangle totalArea, bool textFirst, Size textSize, Size imageSize, System.Drawing.ContentAlignment textAlign, System.Drawing.ContentAlignment imageAlign, out Rectangle textRect, out Rectangle imageRect)
	{
		int num = 0;
		int num2 = textSize.Width + num + imageSize.Width;
		if (!textFirst)
		{
			num += 2;
		}
		if (num2 > totalArea.Width)
		{
			textSize.Width = totalArea.Width - num - imageSize.Width;
			num2 = totalArea.Width;
		}
		int num3 = totalArea.Width - num2;
		int num4 = 0;
		HorizontalAlignment horizontalAlignment = GetHorizontalAlignment(textAlign);
		HorizontalAlignment horizontalAlignment2 = GetHorizontalAlignment(imageAlign);
		num4 = ((horizontalAlignment2 != 0) ? ((horizontalAlignment2 == HorizontalAlignment.Right && horizontalAlignment == HorizontalAlignment.Right) ? num3 : ((horizontalAlignment2 != HorizontalAlignment.Center || (horizontalAlignment != 0 && horizontalAlignment != HorizontalAlignment.Center)) ? (num4 + 2 * (num3 / 3)) : (num4 + num3 / 3))) : 0);
		Rectangle rectangle;
		Rectangle rectangle2;
		if (textFirst)
		{
			rectangle = new Rectangle(totalArea.Left + num4, AlignInRectangle(totalArea, textSize, textAlign).Top, textSize.Width, textSize.Height);
			rectangle2 = new Rectangle(rectangle.Right + num, AlignInRectangle(totalArea, imageSize, imageAlign).Top, imageSize.Width, imageSize.Height);
		}
		else
		{
			rectangle2 = new Rectangle(totalArea.Left + num4, AlignInRectangle(totalArea, imageSize, imageAlign).Top, imageSize.Width, imageSize.Height);
			rectangle = new Rectangle(rectangle2.Right + num, AlignInRectangle(totalArea, textSize, textAlign).Top, textSize.Width, textSize.Height);
		}
		textRect = rectangle;
		imageRect = rectangle2;
	}

	private void LayoutTextAboveOrBelowImage(Rectangle totalArea, bool textFirst, Size textSize, Size imageSize, System.Drawing.ContentAlignment textAlign, System.Drawing.ContentAlignment imageAlign, out Rectangle textRect, out Rectangle imageRect)
	{
		int num = 0;
		int num2 = textSize.Height + num + imageSize.Height;
		if (textFirst)
		{
			num += 2;
		}
		if (textSize.Width > totalArea.Width)
		{
			textSize.Width = totalArea.Width;
		}
		if (num2 > totalArea.Height && textFirst)
		{
			imageSize = Size.Empty;
			num2 = totalArea.Height;
		}
		int num3 = totalArea.Height - num2;
		int num4 = 0;
		VerticalAlignment verticalAlignment = GetVerticalAlignment(textAlign);
		VerticalAlignment verticalAlignment2 = GetVerticalAlignment(imageAlign);
		num4 = ((verticalAlignment2 != 0) ? ((verticalAlignment2 == VerticalAlignment.Bottom && verticalAlignment == VerticalAlignment.Bottom) ? num3 : ((verticalAlignment2 != VerticalAlignment.Center || (verticalAlignment != 0 && verticalAlignment != VerticalAlignment.Center)) ? (num4 + 2 * (num3 / 3)) : (num4 + num3 / 3))) : 0);
		Rectangle rectangle;
		Rectangle rectangle2;
		if (textFirst)
		{
			rectangle = new Rectangle(AlignInRectangle(totalArea, textSize, textAlign).Left, totalArea.Top + num4, textSize.Width, textSize.Height);
			rectangle2 = new Rectangle(AlignInRectangle(totalArea, imageSize, imageAlign).Left, rectangle.Bottom + num, imageSize.Width, imageSize.Height);
		}
		else
		{
			rectangle2 = new Rectangle(AlignInRectangle(totalArea, imageSize, imageAlign).Left, totalArea.Top + num4, imageSize.Width, imageSize.Height);
			rectangle = new Rectangle(AlignInRectangle(totalArea, textSize, textAlign).Left, rectangle2.Bottom + num, textSize.Width, textSize.Height);
			if (rectangle.Bottom > totalArea.Bottom)
			{
				rectangle.Y = totalArea.Top;
			}
		}
		textRect = rectangle;
		imageRect = rectangle2;
	}

	private HorizontalAlignment GetHorizontalAlignment(System.Drawing.ContentAlignment align)
	{
		switch (align)
		{
		case System.Drawing.ContentAlignment.TopLeft:
		case System.Drawing.ContentAlignment.MiddleLeft:
		case System.Drawing.ContentAlignment.BottomLeft:
			return HorizontalAlignment.Left;
		case System.Drawing.ContentAlignment.TopCenter:
		case System.Drawing.ContentAlignment.MiddleCenter:
		case System.Drawing.ContentAlignment.BottomCenter:
			return HorizontalAlignment.Center;
		case System.Drawing.ContentAlignment.TopRight:
		case System.Drawing.ContentAlignment.MiddleRight:
		case System.Drawing.ContentAlignment.BottomRight:
			return HorizontalAlignment.Right;
		default:
			return HorizontalAlignment.Left;
		}
	}

	private VerticalAlignment GetVerticalAlignment(System.Drawing.ContentAlignment align)
	{
		switch (align)
		{
		case System.Drawing.ContentAlignment.TopLeft:
		case System.Drawing.ContentAlignment.TopCenter:
		case System.Drawing.ContentAlignment.TopRight:
			return VerticalAlignment.Top;
		case System.Drawing.ContentAlignment.MiddleLeft:
		case System.Drawing.ContentAlignment.MiddleCenter:
		case System.Drawing.ContentAlignment.MiddleRight:
			return VerticalAlignment.Center;
		case System.Drawing.ContentAlignment.BottomLeft:
		case System.Drawing.ContentAlignment.BottomCenter:
		case System.Drawing.ContentAlignment.BottomRight:
			return VerticalAlignment.Bottom;
		default:
			return VerticalAlignment.Top;
		}
	}

	internal Rectangle AlignInRectangle(Rectangle outer, Size inner, System.Drawing.ContentAlignment align)
	{
		int x = 0;
		int y = 0;
		switch (align)
		{
		case System.Drawing.ContentAlignment.TopLeft:
		case System.Drawing.ContentAlignment.MiddleLeft:
		case System.Drawing.ContentAlignment.BottomLeft:
			x = outer.X;
			break;
		case System.Drawing.ContentAlignment.TopCenter:
		case System.Drawing.ContentAlignment.MiddleCenter:
		case System.Drawing.ContentAlignment.BottomCenter:
			x = Math.Max(outer.X + (outer.Width - inner.Width) / 2, outer.Left);
			break;
		case System.Drawing.ContentAlignment.TopRight:
		case System.Drawing.ContentAlignment.MiddleRight:
		case System.Drawing.ContentAlignment.BottomRight:
			x = outer.Right - inner.Width;
			break;
		}
		switch (align)
		{
		case System.Drawing.ContentAlignment.TopLeft:
		case System.Drawing.ContentAlignment.TopCenter:
		case System.Drawing.ContentAlignment.TopRight:
			y = outer.Y;
			break;
		case System.Drawing.ContentAlignment.MiddleLeft:
		case System.Drawing.ContentAlignment.MiddleCenter:
		case System.Drawing.ContentAlignment.MiddleRight:
			y = outer.Y + (outer.Height - inner.Height) / 2;
			break;
		case System.Drawing.ContentAlignment.BottomLeft:
		case System.Drawing.ContentAlignment.BottomCenter:
		case System.Drawing.ContentAlignment.BottomRight:
			y = outer.Bottom - inner.Height;
			break;
		}
		return new Rectangle(x, y, Math.Min(inner.Width, outer.Width), Math.Min(inner.Height, outer.Height));
	}

	public override void DrawButtonBase(Graphics dc, Rectangle clip_area, ButtonBase button)
	{
		ButtonBase_DrawButton(button, dc);
		if (button.FlatStyle != FlatStyle.System && (button.image != null || button.image_list != null))
		{
			ButtonBase_DrawImage(button, dc);
		}
		if (ShouldPaintFocusRectagle(button))
		{
			ButtonBase_DrawFocus(button, dc);
		}
		if (button.Text != null && button.Text != string.Empty)
		{
			ButtonBase_DrawText(button, dc);
		}
	}

	protected static bool ShouldPaintFocusRectagle(ButtonBase button)
	{
		return (button.Focused || button.paint_as_acceptbutton) && button.Enabled && button.ShowFocusCues;
	}

	protected virtual void ButtonBase_DrawButton(ButtonBase button, Graphics dc)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = ((button.BackColor.ToArgb() == ColorControl.ToArgb()) ? true : false);
		CPColor cpcolor = ((!flag3) ? ResPool.GetCPColor(button.BackColor) : CPColor.Empty);
		if (button is CheckBox)
		{
			flag = true;
			flag2 = ((CheckBox)button).Checked;
		}
		else if (button is RadioButton)
		{
			flag = true;
			flag2 = ((RadioButton)button).Checked;
		}
		Rectangle rect = ((!button.Focused || !button.Enabled || flag) ? button.ClientRectangle : Rectangle.Inflate(button.ClientRectangle, -1, -1));
		if (button.FlatStyle == FlatStyle.Popup)
		{
			if (!button.is_pressed && !button.is_entered && !flag2)
			{
				Internal_DrawButton(dc, rect, 1, cpcolor, flag3, button.BackColor);
			}
			else if (!button.is_pressed && button.is_entered && !flag2)
			{
				Internal_DrawButton(dc, rect, 2, cpcolor, flag3, button.BackColor);
			}
			else if (button.is_pressed || flag2)
			{
				Internal_DrawButton(dc, rect, 1, cpcolor, flag3, button.BackColor);
			}
		}
		else if (button.FlatStyle == FlatStyle.Flat)
		{
			if (button.is_entered && !button.is_pressed && !flag2)
			{
				if (button.image == null && button.image_list == null)
				{
					Brush brush = ((!flag3) ? ResPool.GetSolidBrush(cpcolor.Dark) : SystemBrushes.ControlDark);
					dc.FillRectangle(brush, rect);
				}
			}
			else if (button.is_pressed || flag2)
			{
				if (button.image == null && button.image_list == null)
				{
					Brush brush2 = ((!flag3) ? ResPool.GetSolidBrush(cpcolor.LightLight) : SystemBrushes.ControlLightLight);
					dc.FillRectangle(brush2, rect);
				}
				Pen pen = ((!flag3) ? ResPool.GetPen(cpcolor.Dark) : SystemPens.ControlDark);
				dc.DrawRectangle(pen, rect.X + 4, rect.Y + 4, rect.Width - 9, rect.Height - 9);
			}
			Internal_DrawButton(dc, rect, 3, cpcolor, flag3, button.BackColor);
		}
		else if ((!button.is_pressed || !button.Enabled) && !flag2)
		{
			Internal_DrawButton(dc, rect, 0, cpcolor, flag3, button.BackColor);
		}
		else
		{
			Internal_DrawButton(dc, rect, 1, cpcolor, flag3, button.BackColor);
		}
	}

	private void Internal_DrawButton(Graphics dc, Rectangle rect, int state, CPColor cpcolor, bool is_ColorControl, Color backcolor)
	{
		switch (state)
		{
		case 0:
		{
			Pen pen = ((!is_ColorControl) ? ResPool.GetPen(cpcolor.LightLight) : SystemPens.ControlLightLight);
			dc.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom - 2);
			dc.DrawLine(pen, rect.X + 1, rect.Y, rect.Right - 2, rect.Y);
			pen = ((!is_ColorControl) ? ResPool.GetPen(backcolor) : SystemPens.Control);
			dc.DrawLine(pen, rect.X + 1, rect.Y + 1, rect.X + 1, rect.Bottom - 3);
			dc.DrawLine(pen, rect.X + 2, rect.Y + 1, rect.Right - 3, rect.Y + 1);
			pen = ((!is_ColorControl) ? ResPool.GetPen(cpcolor.Dark) : SystemPens.ControlDark);
			dc.DrawLine(pen, rect.X + 1, rect.Bottom - 2, rect.Right - 2, rect.Bottom - 2);
			dc.DrawLine(pen, rect.Right - 2, rect.Y + 1, rect.Right - 2, rect.Bottom - 3);
			pen = ((!is_ColorControl) ? ResPool.GetPen(cpcolor.DarkDark) : SystemPens.ControlDarkDark);
			dc.DrawLine(pen, rect.X, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
			dc.DrawLine(pen, rect.Right - 1, rect.Y, rect.Right - 1, rect.Bottom - 2);
			break;
		}
		case 1:
		{
			Pen pen = ((!is_ColorControl) ? ResPool.GetPen(cpcolor.Dark) : SystemPens.ControlDark);
			dc.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
			break;
		}
		case 2:
		{
			Pen pen = ((!is_ColorControl) ? ResPool.GetPen(cpcolor.LightLight) : SystemPens.ControlLightLight);
			dc.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom - 2);
			dc.DrawLine(pen, rect.X + 1, rect.Y, rect.Right - 2, rect.Y);
			pen = ((!is_ColorControl) ? ResPool.GetPen(cpcolor.Dark) : SystemPens.ControlDark);
			dc.DrawLine(pen, rect.X, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
			dc.DrawLine(pen, rect.Right - 1, rect.Y, rect.Right - 1, rect.Bottom - 2);
			break;
		}
		case 3:
		{
			Pen pen = ((!is_ColorControl) ? ResPool.GetPen(cpcolor.DarkDark) : SystemPens.ControlDarkDark);
			dc.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
			break;
		}
		}
	}

	protected virtual void ButtonBase_DrawImage(ButtonBase button, Graphics dc)
	{
		int width = button.ClientSize.Width;
		int height = button.ClientSize.Height;
		Image image = ((button.ImageIndex == -1) ? button.image : button.image_list.Images[button.ImageIndex]);
		int width2 = image.Width;
		int height2 = image.Height;
		int x;
		int y;
		switch (button.ImageAlign)
		{
		case System.Drawing.ContentAlignment.TopLeft:
			x = 5;
			y = 5;
			break;
		case System.Drawing.ContentAlignment.TopCenter:
			x = (width - width2) / 2;
			y = 5;
			break;
		case System.Drawing.ContentAlignment.TopRight:
			x = width - width2 - 5;
			y = 5;
			break;
		case System.Drawing.ContentAlignment.MiddleLeft:
			x = 5;
			y = (height - height2) / 2;
			break;
		case System.Drawing.ContentAlignment.MiddleCenter:
			x = (width - width2) / 2;
			y = (height - height2) / 2;
			break;
		case System.Drawing.ContentAlignment.MiddleRight:
			x = width - width2 - 4;
			y = (height - height2) / 2;
			break;
		case System.Drawing.ContentAlignment.BottomLeft:
			x = 5;
			y = height - height2 - 4;
			break;
		case System.Drawing.ContentAlignment.BottomCenter:
			x = (width - width2) / 2;
			y = height - height2 - 4;
			break;
		case System.Drawing.ContentAlignment.BottomRight:
			x = width - width2 - 4;
			y = height - height2 - 4;
			break;
		default:
			x = 5;
			y = 5;
			break;
		}
		dc.SetClip(new Rectangle(3, 3, width - 5, height - 5));
		if (button.Enabled)
		{
			dc.DrawImage(image, x, y, width2, height2);
		}
		else
		{
			CPDrawImageDisabled(dc, image, x, y, ColorControl);
		}
		dc.ResetClip();
	}

	protected virtual void ButtonBase_DrawFocus(ButtonBase button, Graphics dc)
	{
		Color color = button.ForeColor;
		int num = -3;
		if (!(button is CheckBox) && !(button is RadioButton))
		{
			num = -4;
			if (button.FlatStyle == FlatStyle.Popup && !button.is_pressed)
			{
				color = ControlPaint.Dark(button.BackColor);
			}
			dc.DrawRectangle(ResPool.GetPen(color), button.ClientRectangle.X, button.ClientRectangle.Y, button.ClientRectangle.Width - 1, button.ClientRectangle.Height - 1);
		}
		if (button.Focused)
		{
			Rectangle rectangle = Rectangle.Inflate(button.ClientRectangle, num, num);
			ControlPaint.DrawFocusRectangle(dc, rectangle);
		}
	}

	protected virtual void ButtonBase_DrawText(ButtonBase button, Graphics dc)
	{
		Rectangle clientRectangle = button.ClientRectangle;
		Rectangle rectangle = Rectangle.Inflate(clientRectangle, -4, -4);
		if (button.is_pressed)
		{
			rectangle.X++;
			rectangle.Y++;
		}
		rectangle.Height = Math.Max(button.Font.Height, rectangle.Height);
		if (button.Enabled)
		{
			dc.DrawString(button.Text, button.Font, ResPool.GetSolidBrush(button.ForeColor), rectangle, button.text_format);
		}
		else if (button.FlatStyle == FlatStyle.Flat || button.FlatStyle == FlatStyle.Popup)
		{
			dc.DrawString(button.Text, button.Font, ResPool.GetSolidBrush(ColorGrayText), rectangle, button.text_format);
		}
		else
		{
			CPDrawStringDisabled(dc, button.Text, button.Font, button.BackColor, rectangle, button.text_format);
		}
	}

	public override void DrawCheckBox(Graphics g, CheckBox cb, Rectangle glyphArea, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle)
	{
		if (cb.Appearance == Appearance.Button && cb.FlatStyle != 0)
		{
			ButtonBase_DrawButton(cb, g);
		}
		else if (cb.Appearance != Appearance.Button)
		{
			DrawCheckBoxGlyph(g, cb, glyphArea);
		}
		if (cb.Appearance == Appearance.Button && cb.FlatStyle == FlatStyle.Flat)
		{
			DrawFlatButton(g, cb, textBounds, imageBounds, clipRectangle);
		}
		if (imageBounds.Size != Size.Empty)
		{
			DrawCheckBoxImage(g, cb, imageBounds);
		}
		if (cb.Focused && cb.Enabled && cb.ShowFocusCues && textBounds != Rectangle.Empty)
		{
			DrawCheckBoxFocus(g, cb, textBounds);
		}
		if (textBounds != Rectangle.Empty)
		{
			DrawCheckBoxText(g, cb, textBounds);
		}
	}

	public virtual void DrawCheckBoxGlyph(Graphics g, CheckBox cb, Rectangle glyphArea)
	{
		if (cb.Pressed)
		{
			ThemeElements.CurrentTheme.CheckBoxPainter.PaintCheckBox(g, glyphArea, cb.BackColor, cb.ForeColor, ElementState.Pressed, cb.FlatStyle, cb.CheckState);
		}
		else if (cb.InternalSelected)
		{
			ThemeElements.CurrentTheme.CheckBoxPainter.PaintCheckBox(g, glyphArea, cb.BackColor, cb.ForeColor, ElementState.Normal, cb.FlatStyle, cb.CheckState);
		}
		else if (cb.Entered)
		{
			ThemeElements.CurrentTheme.CheckBoxPainter.PaintCheckBox(g, glyphArea, cb.BackColor, cb.ForeColor, ElementState.Hot, cb.FlatStyle, cb.CheckState);
		}
		else if (!cb.Enabled)
		{
			ThemeElements.CurrentTheme.CheckBoxPainter.PaintCheckBox(g, glyphArea, cb.BackColor, cb.ForeColor, ElementState.Disabled, cb.FlatStyle, cb.CheckState);
		}
		else
		{
			ThemeElements.CurrentTheme.CheckBoxPainter.PaintCheckBox(g, glyphArea, cb.BackColor, cb.ForeColor, ElementState.Normal, cb.FlatStyle, cb.CheckState);
		}
	}

	public virtual void DrawCheckBoxFocus(Graphics g, CheckBox cb, Rectangle focusArea)
	{
		ControlPaint.DrawFocusRectangle(g, focusArea);
	}

	public virtual void DrawCheckBoxImage(Graphics g, CheckBox cb, Rectangle imageBounds)
	{
		if (cb.Enabled)
		{
			g.DrawImage(cb.Image, imageBounds);
		}
		else
		{
			CPDrawImageDisabled(g, cb.Image, imageBounds.Left, imageBounds.Top, ColorControl);
		}
	}

	public virtual void DrawCheckBoxText(Graphics g, CheckBox cb, Rectangle textBounds)
	{
		if (cb.Enabled)
		{
			TextRenderer.DrawTextInternal(g, cb.Text, cb.Font, textBounds, cb.ForeColor, cb.TextFormatFlags, cb.UseCompatibleTextRendering);
		}
		else
		{
			DrawStringDisabled20(g, cb.Text, cb.Font, textBounds, cb.BackColor, cb.TextFormatFlags, cb.UseCompatibleTextRendering);
		}
	}

	public override void CalculateCheckBoxTextAndImageLayout(ButtonBase button, Point p, out Rectangle glyphArea, out Rectangle textRectangle, out Rectangle imageRectangle)
	{
		int num = 13;
		if (button is CheckBox)
		{
			num = (((button as CheckBox).Appearance == Appearance.Normal) ? 13 : 0);
		}
		glyphArea = new Rectangle(0, 2, num, num);
		Rectangle clientRectangle = button.ClientRectangle;
		System.Drawing.ContentAlignment contentAlignment = System.Drawing.ContentAlignment.TopLeft;
		if (button is CheckBox)
		{
			contentAlignment = (button as CheckBox).CheckAlign;
		}
		else if (button is RadioButton)
		{
			contentAlignment = (button as RadioButton).CheckAlign;
		}
		switch (contentAlignment)
		{
		case System.Drawing.ContentAlignment.BottomCenter:
			glyphArea.Y = button.Height - num;
			glyphArea.X = (button.Width - num) / 2 - 2;
			break;
		case System.Drawing.ContentAlignment.BottomLeft:
			glyphArea.Y = button.Height - num - 2;
			clientRectangle.Width -= num;
			clientRectangle.Offset(num, 0);
			break;
		case System.Drawing.ContentAlignment.BottomRight:
			glyphArea.Y = button.Height - num - 2;
			glyphArea.X = button.Width - num;
			clientRectangle.Width -= num;
			break;
		case System.Drawing.ContentAlignment.MiddleCenter:
			glyphArea.Y = (button.Height - num) / 2;
			glyphArea.X = (button.Width - num) / 2;
			break;
		case System.Drawing.ContentAlignment.MiddleLeft:
			glyphArea.Y = (button.Height - num) / 2;
			clientRectangle.Width -= num;
			clientRectangle.Offset(num, 0);
			break;
		case System.Drawing.ContentAlignment.MiddleRight:
			glyphArea.Y = (button.Height - num) / 2;
			glyphArea.X = button.Width - num;
			clientRectangle.Width -= num;
			break;
		case System.Drawing.ContentAlignment.TopCenter:
			glyphArea.X = (button.Width - num) / 2;
			break;
		case System.Drawing.ContentAlignment.TopLeft:
			clientRectangle.Width -= num;
			clientRectangle.Offset(num, 0);
			break;
		case System.Drawing.ContentAlignment.TopRight:
			glyphArea.X = button.Width - num;
			clientRectangle.Width -= num;
			break;
		}
		Image image = button.Image;
		string text = button.Text;
		Size empty = Size.Empty;
		if (!button.AutoSize)
		{
			empty.Width = button.Width - glyphArea.Width - 2;
		}
		Size size = TextRenderer.MeasureTextInternal(text, button.Font, empty, button.TextFormatFlags, button.UseCompatibleTextRendering);
		size.Height = Math.Min(size.Height, clientRectangle.Height);
		size.Width = Math.Min(size.Width, clientRectangle.Width);
		Size imageSize = image?.Size ?? Size.Empty;
		textRectangle = Rectangle.Empty;
		imageRectangle = Rectangle.Empty;
		switch (button.TextImageRelation)
		{
		case TextImageRelation.Overlay:
			textRectangle.X = clientRectangle.Left + 2;
			textRectangle.Y = (clientRectangle.Height - size.Height) / 2 - 1;
			textRectangle.Size = size;
			if (image != null)
			{
				int num2 = 0;
				int num3 = 0;
				int height = image.Height;
				int width = image.Width;
				switch (button.ImageAlign)
				{
				case System.Drawing.ContentAlignment.TopLeft:
					num2 = 5;
					num3 = 5;
					break;
				case System.Drawing.ContentAlignment.TopCenter:
					num2 = (clientRectangle.Width - width) / 2;
					num3 = 5;
					break;
				case System.Drawing.ContentAlignment.TopRight:
					num2 = clientRectangle.Width - width - 5;
					num3 = 5;
					break;
				case System.Drawing.ContentAlignment.MiddleLeft:
					num2 = 5;
					num3 = (clientRectangle.Height - height) / 2;
					break;
				case System.Drawing.ContentAlignment.MiddleCenter:
					num2 = (clientRectangle.Width - width) / 2;
					num3 = (clientRectangle.Height - height) / 2;
					break;
				case System.Drawing.ContentAlignment.MiddleRight:
					num2 = clientRectangle.Width - width - 4;
					num3 = (clientRectangle.Height - height) / 2;
					break;
				case System.Drawing.ContentAlignment.BottomLeft:
					num2 = 5;
					num3 = clientRectangle.Height - height - 4;
					break;
				case System.Drawing.ContentAlignment.BottomCenter:
					num2 = (clientRectangle.Width - width) / 2;
					num3 = clientRectangle.Height - height - 4;
					break;
				case System.Drawing.ContentAlignment.BottomRight:
					num2 = clientRectangle.Width - width - 4;
					num3 = clientRectangle.Height - height - 4;
					break;
				default:
					num2 = 5;
					num3 = 5;
					break;
				}
				imageRectangle = new Rectangle(num2 + num, num3, width, height);
			}
			break;
		case TextImageRelation.ImageAboveText:
			clientRectangle.Inflate(-4, -4);
			LayoutTextAboveOrBelowImage(clientRectangle, textFirst: false, size, imageSize, button.TextAlign, button.ImageAlign, out textRectangle, out imageRectangle);
			break;
		case TextImageRelation.TextAboveImage:
			clientRectangle.Inflate(-4, -4);
			LayoutTextAboveOrBelowImage(clientRectangle, textFirst: true, size, imageSize, button.TextAlign, button.ImageAlign, out textRectangle, out imageRectangle);
			break;
		case TextImageRelation.ImageBeforeText:
			clientRectangle.Inflate(-4, -4);
			LayoutTextBeforeOrAfterImage(clientRectangle, textFirst: false, size, imageSize, button.TextAlign, button.ImageAlign, out textRectangle, out imageRectangle);
			break;
		case TextImageRelation.TextBeforeImage:
			clientRectangle.Inflate(-4, -4);
			LayoutTextBeforeOrAfterImage(clientRectangle, textFirst: true, size, imageSize, button.TextAlign, button.ImageAlign, out textRectangle, out imageRectangle);
			break;
		case (TextImageRelation)3:
		case (TextImageRelation)5:
		case (TextImageRelation)6:
		case (TextImageRelation)7:
			break;
		}
	}

	public override Size CalculateCheckBoxAutoSize(CheckBox checkBox)
	{
		Size empty = Size.Empty;
		Size size = TextRenderer.MeasureTextInternal(checkBox.Text, checkBox.Font, checkBox.UseCompatibleTextRendering);
		Size size2 = ((checkBox.Image != null) ? checkBox.Image.Size : Size.Empty);
		if (checkBox.Text.Length != 0)
		{
			size.Height += 4;
			size.Width += 4;
		}
		switch (checkBox.TextImageRelation)
		{
		case TextImageRelation.Overlay:
			empty.Height = Math.Max((checkBox.Text.Length != 0) ? size.Height : 0, size2.Height);
			empty.Width = Math.Max(size.Width, size2.Width);
			break;
		case TextImageRelation.ImageAboveText:
		case TextImageRelation.TextAboveImage:
			empty.Height = size.Height + size2.Height;
			empty.Width = Math.Max(size.Width, size2.Width);
			break;
		case TextImageRelation.ImageBeforeText:
		case TextImageRelation.TextBeforeImage:
			empty.Height = Math.Max(size.Height, size2.Height);
			empty.Width = size.Width + size2.Width;
			break;
		}
		empty.Height += checkBox.Padding.Vertical;
		empty.Width += checkBox.Padding.Horizontal + 15;
		if (empty.Height == checkBox.Padding.Vertical)
		{
			empty.Height += 14;
		}
		return empty;
	}

	public override void DrawCheckBox(Graphics dc, Rectangle clip_area, CheckBox checkbox)
	{
		int num = 13;
		int num2 = 4;
		Rectangle clientRectangle = checkbox.ClientRectangle;
		Rectangle text_rectangle = clientRectangle;
		Rectangle checkbox_rectangle = new Rectangle(text_rectangle.X, text_rectangle.Y, num, num);
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Near;
		stringFormat.LineAlignment = StringAlignment.Center;
		if (checkbox.ShowKeyboardCuesInternal)
		{
			stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
		}
		else
		{
			stringFormat.HotkeyPrefix = HotkeyPrefix.Hide;
		}
		if (checkbox.appearance != Appearance.Button)
		{
			switch (checkbox.check_alignment)
			{
			case System.Drawing.ContentAlignment.BottomCenter:
				checkbox_rectangle.X = (clientRectangle.Right - clientRectangle.Left) / 2 - num / 2;
				checkbox_rectangle.Y = clientRectangle.Bottom - num;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width;
				text_rectangle.Height = clientRectangle.Height - checkbox_rectangle.Y - num2;
				break;
			case System.Drawing.ContentAlignment.BottomLeft:
				checkbox_rectangle.X = clientRectangle.Left;
				checkbox_rectangle.Y = clientRectangle.Bottom - num;
				text_rectangle.X = clientRectangle.X + num + num2;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.BottomRight:
				checkbox_rectangle.X = clientRectangle.Right - num;
				checkbox_rectangle.Y = clientRectangle.Bottom - num;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.MiddleCenter:
				checkbox_rectangle.X = (clientRectangle.Right - clientRectangle.Left) / 2 - num / 2;
				checkbox_rectangle.Y = (clientRectangle.Bottom - clientRectangle.Top) / 2 - num / 2;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width;
				break;
			default:
				checkbox_rectangle.X = clientRectangle.Left;
				checkbox_rectangle.Y = (clientRectangle.Bottom - clientRectangle.Top) / 2 - num / 2;
				text_rectangle.X = clientRectangle.X + num + num2;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.MiddleRight:
				checkbox_rectangle.X = clientRectangle.Right - num;
				checkbox_rectangle.Y = (clientRectangle.Bottom - clientRectangle.Top) / 2 - num / 2;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.TopCenter:
				checkbox_rectangle.X = (clientRectangle.Right - clientRectangle.Left) / 2 - num / 2;
				checkbox_rectangle.Y = clientRectangle.Top;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width;
				text_rectangle.Y = num + num2;
				text_rectangle.Height = clientRectangle.Height - num - num2;
				break;
			case System.Drawing.ContentAlignment.TopLeft:
				checkbox_rectangle.X = clientRectangle.Left;
				text_rectangle.X = clientRectangle.X + num + num2;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.TopRight:
				checkbox_rectangle.X = clientRectangle.Right - num;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			}
		}
		else
		{
			text_rectangle.X = clientRectangle.X;
			text_rectangle.Width = clientRectangle.Width;
		}
		switch (checkbox.text_alignment)
		{
		case System.Drawing.ContentAlignment.TopLeft:
		case System.Drawing.ContentAlignment.MiddleLeft:
		case System.Drawing.ContentAlignment.BottomLeft:
			stringFormat.Alignment = StringAlignment.Near;
			break;
		case System.Drawing.ContentAlignment.TopCenter:
		case System.Drawing.ContentAlignment.MiddleCenter:
		case System.Drawing.ContentAlignment.BottomCenter:
			stringFormat.Alignment = StringAlignment.Center;
			break;
		case System.Drawing.ContentAlignment.TopRight:
		case System.Drawing.ContentAlignment.MiddleRight:
		case System.Drawing.ContentAlignment.BottomRight:
			stringFormat.Alignment = StringAlignment.Far;
			break;
		}
		switch (checkbox.text_alignment)
		{
		case System.Drawing.ContentAlignment.TopLeft:
		case System.Drawing.ContentAlignment.TopCenter:
		case System.Drawing.ContentAlignment.TopRight:
			stringFormat.LineAlignment = StringAlignment.Near;
			break;
		case System.Drawing.ContentAlignment.BottomLeft:
		case System.Drawing.ContentAlignment.BottomCenter:
		case System.Drawing.ContentAlignment.BottomRight:
			stringFormat.LineAlignment = StringAlignment.Far;
			break;
		case System.Drawing.ContentAlignment.MiddleLeft:
		case System.Drawing.ContentAlignment.MiddleCenter:
		case System.Drawing.ContentAlignment.MiddleRight:
			stringFormat.LineAlignment = StringAlignment.Center;
			break;
		}
		ButtonState buttonState = ButtonState.Normal;
		if (checkbox.FlatStyle == FlatStyle.Flat)
		{
			buttonState |= ButtonState.Flat;
		}
		if (checkbox.Checked)
		{
			buttonState |= ButtonState.Checked;
		}
		if (checkbox.ThreeState && checkbox.CheckState == CheckState.Indeterminate)
		{
			buttonState |= ButtonState.Checked;
			buttonState |= ButtonState.Pushed;
		}
		if (!checkbox.Enabled)
		{
			buttonState |= ButtonState.Inactive;
		}
		else if (checkbox.is_pressed)
		{
			buttonState |= ButtonState.Pushed;
		}
		CheckBox_DrawCheckBox(dc, checkbox, buttonState, checkbox_rectangle);
		if (checkbox.image != null || checkbox.image_list != null)
		{
			ButtonBase_DrawImage(checkbox, dc);
		}
		CheckBox_DrawText(checkbox, text_rectangle, dc, stringFormat);
		if (checkbox.Focused && checkbox.Enabled && checkbox.appearance != Appearance.Button && checkbox.Text != string.Empty && checkbox.ShowFocusCues)
		{
			SizeF sizeF = dc.MeasureString(checkbox.Text, checkbox.Font);
			Rectangle empty = Rectangle.Empty;
			empty.X = text_rectangle.X;
			empty.Y = (int)(((float)text_rectangle.Height - sizeF.Height) / 2f);
			empty.Size = sizeF.ToSize();
			CheckBox_DrawFocus(checkbox, dc, empty);
		}
		stringFormat.Dispose();
	}

	protected virtual void CheckBox_DrawCheckBox(Graphics dc, CheckBox checkbox, ButtonState state, Rectangle checkbox_rectangle)
	{
		Brush brush = ((checkbox.BackColor.ToArgb() != ColorControl.ToArgb()) ? ResPool.GetSolidBrush(checkbox.BackColor) : SystemBrushes.Control);
		dc.FillRectangle(brush, checkbox.ClientRectangle);
		if (checkbox.appearance == Appearance.Button)
		{
			ButtonBase_DrawButton(checkbox, dc);
			if (checkbox.Focused && checkbox.Enabled)
			{
				ButtonBase_DrawFocus(checkbox, dc);
			}
		}
		else if (checkbox.FlatStyle == FlatStyle.Flat || checkbox.FlatStyle == FlatStyle.Popup)
		{
			DrawFlatStyleCheckBox(dc, checkbox_rectangle, checkbox);
		}
		else
		{
			CPDrawCheckBox(dc, checkbox_rectangle, state);
		}
	}

	protected virtual void CheckBox_DrawText(CheckBox checkbox, Rectangle text_rectangle, Graphics dc, StringFormat text_format)
	{
		DrawCheckBox_and_RadioButtonText(checkbox, text_rectangle, dc, text_format, checkbox.Appearance, checkbox.Checked);
	}

	protected virtual void CheckBox_DrawFocus(CheckBox checkbox, Graphics dc, Rectangle text_rectangle)
	{
		DrawInnerFocusRectangle(dc, text_rectangle, checkbox.BackColor);
	}

	protected virtual void DrawFlatStyleCheckBox(Graphics graphics, Rectangle rectangle, CheckBox checkbox)
	{
		Rectangle rectangle2;
		Rectangle rect;
		if (checkbox.FlatStyle == FlatStyle.Popup && checkbox.is_entered)
		{
			rectangle2 = new Rectangle(rectangle.X, rectangle.Y, Math.Max(rectangle.Width - 1, 0), Math.Max(rectangle.Height - 1, 0));
			rect = new Rectangle(rectangle2.X + 1, rectangle2.Y + 1, Math.Max(rectangle2.Width - 3, 0), Math.Max(rectangle2.Height - 3, 0));
		}
		else
		{
			rectangle2 = new Rectangle(rectangle.X, rectangle.Y, Math.Max(rectangle.Width - 2, 0), Math.Max(rectangle.Height - 2, 0));
			rect = new Rectangle(rectangle2.X + 1, rectangle2.Y + 1, Math.Max(rectangle2.Width - 2, 0), Math.Max(rectangle2.Height - 2, 0));
		}
		if (checkbox.Enabled)
		{
			if (checkbox.is_entered || checkbox.Capture)
			{
				if (checkbox.FlatStyle == FlatStyle.Popup && checkbox.is_entered && checkbox.Capture)
				{
					graphics.FillRectangle(ResPool.GetSolidBrush(checkbox.BackColor), rect);
				}
				else if (checkbox.FlatStyle == FlatStyle.Flat)
				{
					if (!checkbox.is_pressed)
					{
						graphics.FillRectangle(ResPool.GetSolidBrush(checkbox.BackColor), rect);
					}
					else
					{
						graphics.FillRectangle(ResPool.GetSolidBrush(ControlPaint.LightLight(checkbox.BackColor)), rect);
					}
				}
				else
				{
					graphics.FillRectangle(ResPool.GetSolidBrush(ControlPaint.LightLight(checkbox.BackColor)), rect);
				}
				if (checkbox.FlatStyle == FlatStyle.Flat)
				{
					ControlPaint.DrawBorder(graphics, rectangle2, checkbox.ForeColor, ButtonBorderStyle.Solid);
				}
				else
				{
					CPDrawBorder3D(graphics, rectangle2, Border3DStyle.SunkenInner, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, checkbox.BackColor);
				}
			}
			else
			{
				graphics.FillRectangle(ResPool.GetSolidBrush(ControlPaint.LightLight(checkbox.BackColor)), rect);
				if (checkbox.FlatStyle == FlatStyle.Flat)
				{
					ControlPaint.DrawBorder(graphics, rectangle2, checkbox.ForeColor, ButtonBorderStyle.Solid);
				}
				else
				{
					ControlPaint.DrawBorder(graphics, rectangle2, ControlPaint.DarkDark(checkbox.BackColor), ButtonBorderStyle.Solid);
				}
			}
		}
		else
		{
			if (checkbox.FlatStyle == FlatStyle.Popup)
			{
				graphics.FillRectangle(SystemBrushes.Control, rect);
			}
			ControlPaint.DrawBorder(graphics, rectangle2, ColorControlDark, ButtonBorderStyle.Solid);
		}
		if (checkbox.Checked)
		{
			int num = Math.Max(3, rect.Width / 3);
			int num2 = Math.Max(1, rect.Width / 9);
			Rectangle rectangle3 = new Rectangle(rect.X, rect.Y + 1, rect.Width, rect.Height);
			Pen pen = ((!checkbox.Enabled) ? SystemPens.ControlDark : ResPool.GetPen(checkbox.ForeColor));
			for (int i = 0; i < num; i++)
			{
				graphics.DrawLine(pen, rectangle3.Left + num / 2, rectangle3.Top + num + i, rectangle3.Left + num / 2 + 2 * num2, rectangle3.Top + num + 2 * num2 + i);
				graphics.DrawLine(pen, rectangle3.Left + num / 2 + 2 * num2, rectangle3.Top + num + 2 * num2 + i, rectangle3.Left + num / 2 + 6 * num2, rectangle3.Top + num - 2 * num2 + i);
			}
		}
	}

	private void DrawCheckBox_and_RadioButtonText(ButtonBase button_base, Rectangle text_rectangle, Graphics dc, StringFormat text_format, Appearance appearance, bool ischecked)
	{
		if (appearance == Appearance.Button)
		{
			if (ischecked || (button_base.Capture && button_base.FlatStyle != 0))
			{
				text_rectangle.X++;
				text_rectangle.Y++;
			}
			text_rectangle.Inflate(-4, -4);
		}
		if ((float)button_base.Font.Height * 1.5f > (float)text_rectangle.Height)
		{
			text_format.FormatFlags |= StringFormatFlags.NoWrap;
		}
		if (button_base.Enabled)
		{
			dc.DrawString(button_base.Text, button_base.Font, ResPool.GetSolidBrush(button_base.ForeColor), text_rectangle, text_format);
		}
		else if (button_base.FlatStyle == FlatStyle.Flat || button_base.FlatStyle == FlatStyle.Popup)
		{
			dc.DrawString(button_base.Text, button_base.Font, SystemBrushes.ControlDarkDark, text_rectangle, text_format);
		}
		else
		{
			CPDrawStringDisabled(dc, button_base.Text, button_base.Font, button_base.BackColor, text_rectangle, text_format);
		}
	}

	public override void DrawCheckedListBoxItem(CheckedListBox ctrl, DrawItemEventArgs e)
	{
		Rectangle bounds = e.Bounds;
		ButtonState buttonState;
		if ((e.State & DrawItemState.Checked) == DrawItemState.Checked)
		{
			buttonState = ButtonState.Checked;
			if ((e.State & DrawItemState.Inactive) == DrawItemState.Inactive)
			{
				buttonState |= ButtonState.Inactive;
			}
		}
		else
		{
			buttonState = ButtonState.Normal;
		}
		if (!ctrl.ThreeDCheckBoxes)
		{
			buttonState |= ButtonState.Flat;
		}
		Rectangle rectangle = new Rectangle(2, (bounds.Height - 11) / 2, 13, 13);
		ControlPaint.DrawCheckBox(e.Graphics, bounds.X + rectangle.X, bounds.Y + rectangle.Y, rectangle.Width, rectangle.Height, buttonState);
		bounds.X += rectangle.Right;
		bounds.Width -= rectangle.Right;
		Color color;
		Color color2;
		if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
		{
			color = ColorHighlight;
			color2 = ColorHighlightText;
		}
		else
		{
			color = e.BackColor;
			color2 = e.ForeColor;
		}
		e.Graphics.FillRectangle(ResPool.GetSolidBrush(color), bounds);
		e.Graphics.DrawString(ctrl.GetItemText(ctrl.Items[e.Index]), e.Font, ResPool.GetSolidBrush(color2), bounds, ctrl.StringFormat);
		if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
		{
			CPDrawFocusRectangle(e.Graphics, bounds, color2, color);
		}
	}

	public override void DrawComboBoxItem(ComboBox ctrl, DrawItemEventArgs e)
	{
		Rectangle bounds = e.Bounds;
		StringFormat stringFormat = new StringFormat();
		stringFormat.FormatFlags = StringFormatFlags.LineLimit;
		Color color;
		Color color2;
		if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
		{
			color = ColorHighlight;
			color2 = ColorHighlightText;
		}
		else
		{
			color = e.BackColor;
			color2 = e.ForeColor;
		}
		if (!ctrl.Enabled)
		{
			color2 = ColorInactiveCaptionText;
		}
		e.Graphics.FillRectangle(ResPool.GetSolidBrush(color), e.Bounds);
		if (e.Index != -1)
		{
			e.Graphics.DrawString(ctrl.GetItemText(ctrl.Items[e.Index]), e.Font, ResPool.GetSolidBrush(color2), bounds, stringFormat);
		}
		if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
		{
			CPDrawFocusRectangle(e.Graphics, e.Bounds, color2, color);
		}
		stringFormat.Dispose();
	}

	public override void DrawFlatStyleComboButton(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		Point[] array = new Point[3];
		Rectangle rectangle2 = new Rectangle(rectangle.X + rectangle.Width / 4, rectangle.Y + rectangle.Height / 4, rectangle.Width / 2, rectangle.Height / 2);
		int x = rectangle2.Left + rectangle2.Width / 2;
		int num = rectangle2.Top + rectangle2.Height / 2;
		int num2 = Math.Max(1, rectangle2.Width / 8);
		int num3 = Math.Max(1, rectangle2.Height / 8);
		if ((state & ButtonState.Pushed) != 0)
		{
			num2++;
			num3++;
		}
		rectangle2.Y -= num3;
		num -= num3;
		Point point = new Point(rectangle2.Left + 1, num);
		Point point2 = new Point(rectangle2.Right - 1, num);
		Point point3 = new Point(x, rectangle2.Bottom - 1);
		array[0] = point;
		array[1] = point2;
		array[2] = point3;
		if ((state & ButtonState.Inactive) != 0)
		{
			array[0].X++;
			array[0].Y++;
			array[1].X++;
			array[1].Y++;
			array[2].X++;
			array[2].Y++;
			graphics.FillPolygon(SystemBrushes.ControlLightLight, array, FillMode.Winding);
			array[0] = point;
			array[1] = point2;
			array[2] = point3;
			graphics.FillPolygon(SystemBrushes.ControlDark, array, FillMode.Winding);
		}
		else
		{
			graphics.FillPolygon(SystemBrushes.ControlText, array, FillMode.Winding);
		}
	}

	public override void ComboBoxDrawNormalDropDownButton(ComboBox comboBox, Graphics g, Rectangle clippingArea, Rectangle area, ButtonState state)
	{
		CPDrawComboButton(g, area, state);
	}

	public override bool ComboBoxNormalDropDownButtonHasTransparentBackground(ComboBox comboBox, ButtonState state)
	{
		return true;
	}

	public override bool ComboBoxDropDownButtonHasHotElementStyle(ComboBox comboBox)
	{
		return false;
	}

	public override void ComboBoxDrawBackground(ComboBox comboBox, Graphics g, Rectangle clippingArea, FlatStyle style)
	{
		if (!comboBox.Enabled)
		{
			g.FillRectangle(ResPool.GetSolidBrush(ColorControl), comboBox.ClientRectangle);
		}
		if (comboBox.DropDownStyle == ComboBoxStyle.Simple)
		{
			g.FillRectangle(ResPool.GetSolidBrush(comboBox.Parent.BackColor), comboBox.ClientRectangle);
		}
		if (style == FlatStyle.Popup && (comboBox.Entered || comboBox.Focused))
		{
			Rectangle textArea = comboBox.TextArea;
			textArea.Height--;
			textArea.Width--;
			g.DrawRectangle(ResPool.GetPen(SystemColors.ControlDark), textArea);
			g.DrawLine(ResPool.GetPen(SystemColors.ControlDark), comboBox.ButtonArea.X - 1, comboBox.ButtonArea.Top, comboBox.ButtonArea.X - 1, comboBox.ButtonArea.Bottom);
		}
		if (style != 0 && style != FlatStyle.Popup && clippingArea.IntersectsWith(comboBox.TextArea))
		{
			ControlPaint.DrawBorder3D(g, comboBox.TextArea, Border3DStyle.Sunken);
		}
	}

	public override bool CombBoxBackgroundHasHotElementStyle(ComboBox comboBox)
	{
		return false;
	}

	public override void DataGridPaint(PaintEventArgs pe, DataGrid grid)
	{
		DataGridPaintCaption(pe.Graphics, pe.ClipRectangle, grid);
		DataGridPaintParentRows(pe.Graphics, pe.ClipRectangle, grid);
		DataGridPaintColumnHeaders(pe.Graphics, pe.ClipRectangle, grid);
		DataGridPaintRows(pe.Graphics, grid.cells_area, pe.ClipRectangle, grid);
		if (grid.VScrollBar.Visible && grid.HScrollBar.Visible)
		{
			Rectangle rect = new Rectangle(grid.ClientRectangle.X + grid.ClientRectangle.Width - grid.VScrollBar.Width, grid.ClientRectangle.Y + grid.ClientRectangle.Height - grid.HScrollBar.Height, grid.VScrollBar.Width, grid.HScrollBar.Height);
			if (pe.ClipRectangle.IntersectsWith(rect))
			{
				pe.Graphics.FillRectangle(ResPool.GetSolidBrush(grid.ParentRowsBackColor), rect);
			}
		}
	}

	public override void DataGridPaintCaption(Graphics g, Rectangle clip, DataGrid grid)
	{
		Rectangle rect = clip;
		rect.Intersect(grid.caption_area);
		g.FillRectangle(ResPool.GetSolidBrush(grid.CaptionBackColor), rect);
		g.DrawLine(ResPool.GetPen(grid.CurrentTableStyle.CurrentHeaderForeColor), rect.X, rect.Y + rect.Height - 1, rect.X + rect.Width, rect.Y + rect.Height - 1);
		if (grid.CaptionText != string.Empty)
		{
			Rectangle caption_area = grid.caption_area;
			caption_area.Y += caption_area.Height / 2 - grid.CaptionFont.Height / 2;
			caption_area.Height = grid.CaptionFont.Height;
			g.DrawString(grid.CaptionText, grid.CaptionFont, ResPool.GetSolidBrush(grid.CaptionForeColor), caption_area);
		}
		if (rect.IntersectsWith(grid.back_button_rect))
		{
			g.DrawImage(grid.back_button_image, grid.back_button_rect);
			if (grid.back_button_mouseover)
			{
				CPDrawBorder3D(g, grid.back_button_rect, (!grid.back_button_active) ? Border3DStyle.Raised : Border3DStyle.Sunken, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
			}
		}
		if (rect.IntersectsWith(grid.parent_rows_button_rect))
		{
			g.DrawImage(grid.parent_rows_button_image, grid.parent_rows_button_rect);
			if (grid.parent_rows_button_mouseover)
			{
				CPDrawBorder3D(g, grid.parent_rows_button_rect, (!grid.parent_rows_button_active) ? Border3DStyle.Raised : Border3DStyle.Sunken, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
			}
		}
	}

	public override void DataGridPaintColumnHeaders(Graphics g, Rectangle clip, DataGrid grid)
	{
		if (!grid.CurrentTableStyle.ColumnHeadersVisible)
		{
			return;
		}
		Rectangle column_headers_area = grid.column_headers_area;
		if (grid.CurrentTableStyle.CurrentRowHeadersVisible)
		{
			Rectangle column_headers_area2 = grid.column_headers_area;
			column_headers_area2.Width = grid.RowHeaderWidth;
			if (clip.IntersectsWith(column_headers_area2))
			{
				if (grid.FlatMode)
				{
					g.FillRectangle(ResPool.GetSolidBrush(grid.CurrentTableStyle.CurrentHeaderBackColor), column_headers_area2);
				}
				else
				{
					CPDrawBorder3D(g, column_headers_area2, Border3DStyle.RaisedInner, Border3DSide.All, grid.CurrentTableStyle.CurrentHeaderBackColor);
				}
			}
			column_headers_area.X += grid.RowHeaderWidth;
			column_headers_area.Width -= grid.RowHeaderWidth;
		}
		Rectangle rectangle = default(Rectangle);
		Region clip2 = g.Clip;
		rectangle.Y = column_headers_area.Y;
		rectangle.Height = column_headers_area.Height;
		int num = grid.FirstVisibleColumn + grid.VisibleColumnCount;
		for (int i = grid.FirstVisibleColumn; i < num; i++)
		{
			if (grid.CurrentTableStyle.GridColumnStyles[i].bound)
			{
				int columnStartingPixel = grid.GetColumnStartingPixel(i);
				rectangle.X = column_headers_area.X + columnStartingPixel - grid.HorizPixelOffset;
				rectangle.Width = grid.CurrentTableStyle.GridColumnStyles[i].Width;
				if (clip.IntersectsWith(rectangle))
				{
					Region region = new Region(rectangle);
					region.Intersect(column_headers_area);
					region.Intersect(clip2);
					g.Clip = region;
					DataGridPaintColumnHeader(g, rectangle, grid, i);
					region.Dispose();
				}
			}
		}
		g.Clip = clip2;
		Rectangle column_headers_area3 = grid.column_headers_area;
		column_headers_area3.X = ((num != 0) ? (rectangle.X + rectangle.Width) : grid.RowHeaderWidth);
		column_headers_area3.Width = grid.ClientRectangle.X + grid.ClientRectangle.Width - column_headers_area3.X;
		g.FillRectangle(ResPool.GetSolidBrush(grid.BackgroundColor), column_headers_area3);
	}

	public override void DataGridPaintColumnHeader(Graphics g, Rectangle bounds, DataGrid grid, int col)
	{
		g.FillRectangle(ResPool.GetSolidBrush(grid.CurrentTableStyle.HeaderBackColor), bounds);
		if (!grid.FlatMode)
		{
			g.DrawLine(ResPool.GetPen(ColorControlLightLight), bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y);
			if (col == 0)
			{
				g.DrawLine(ResPool.GetPen(ColorControlLightLight), bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height);
			}
			else
			{
				g.DrawLine(ResPool.GetPen(ColorControlLightLight), bounds.X, bounds.Y + 2, bounds.X, bounds.Y + bounds.Height - 3);
			}
			if (col == grid.VisibleColumnCount - 1)
			{
				g.DrawLine(ResPool.GetPen(ColorControlDark), bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height);
			}
			else
			{
				g.DrawLine(ResPool.GetPen(ColorControlDark), bounds.X + bounds.Width - 1, bounds.Y + 2, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 3);
			}
			g.DrawLine(ResPool.GetPen(ColorControlDark), bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width, bounds.Y + bounds.Height - 1);
		}
		bounds.X += 2;
		bounds.Width -= 2;
		DataGridColumnStyle dataGridColumnStyle = grid.CurrentTableStyle.GridColumnStyles[col];
		if (dataGridColumnStyle.ArrowDrawingMode != 0)
		{
			bounds.Width -= 16;
		}
		StringFormat stringFormat = new StringFormat();
		stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.Trimming = StringTrimming.Character;
		g.DrawString(dataGridColumnStyle.HeaderText, grid.CurrentTableStyle.HeaderFont, ResPool.GetSolidBrush(grid.CurrentTableStyle.CurrentHeaderForeColor), bounds, stringFormat);
		if (dataGridColumnStyle.ArrowDrawingMode != 0)
		{
			Point point = new Point(bounds.X + bounds.Width + 4, bounds.Y + (bounds.Height - 6) / 2);
			if (dataGridColumnStyle.ArrowDrawingMode == DataGridColumnStyle.ArrowDrawing.Ascending)
			{
				g.DrawLine(SystemPens.ControlLightLight, point.X + 6, point.Y + 6, point.X + 3, point.Y);
				g.DrawLine(SystemPens.ControlDark, point.X, point.Y + 6, point.X + 6, point.Y + 6);
				g.DrawLine(SystemPens.ControlDark, point.X, point.Y + 6, point.X + 3, point.Y);
			}
			else
			{
				g.DrawLine(SystemPens.ControlLightLight, point.X + 6, point.Y, point.X + 3, point.Y + 6);
				g.DrawLine(SystemPens.ControlDark, point.X, point.Y, point.X + 6, point.Y);
				g.DrawLine(SystemPens.ControlDark, point.X, point.Y, point.X + 3, point.Y + 6);
			}
		}
	}

	public override void DataGridPaintParentRows(Graphics g, Rectangle clip, DataGrid grid)
	{
		Rectangle rectangle = default(Rectangle);
		rectangle.X = grid.ParentRowsArea.X;
		rectangle.Width = grid.ParentRowsArea.Width;
		rectangle.Height = grid.CaptionFont.Height + 3;
		object[] array = grid.data_source_stack.ToArray();
		Region clip2 = g.Clip;
		for (int i = 0; i < array.Length; i++)
		{
			rectangle.Y = grid.ParentRowsArea.Y + i * rectangle.Height;
			if (clip.IntersectsWith(rectangle))
			{
				Region region = new Region(rectangle);
				region.Intersect(clip2);
				g.Clip = region;
				DataGridPaintParentRow(g, rectangle, (DataGridDataSource)array[array.Length - i - 1], grid);
				region.Dispose();
			}
		}
		g.Clip = clip2;
	}

	public override void DataGridPaintParentRow(Graphics g, Rectangle bounds, DataGridDataSource row, DataGrid grid)
	{
		g.FillRectangle(ResPool.GetSolidBrush(grid.ParentRowsBackColor), bounds);
		Font font = new Font(grid.Font.FontFamily, grid.Font.Size, grid.Font.Style | FontStyle.Bold);
		StringFormat stringFormat = new StringFormat();
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.Alignment = StringAlignment.Near;
		string text = string.Empty;
		if (row.view is DataRowView)
		{
			text = ((ITypedList)((DataRowView)row.view).DataView).GetListName((PropertyDescriptor[])null) + ": ";
		}
		Size size = g.MeasureString(text, font).ToSize();
		Rectangle rectangle = new Rectangle(new Point(bounds.X + 3, bounds.Y + bounds.Height - size.Height), size);
		g.DrawString(text, font, ResPool.GetSolidBrush(grid.ParentRowsForeColor), rectangle, stringFormat);
		foreach (PropertyDescriptor property in ((ICustomTypeDescriptor)row.view).GetProperties())
		{
			if (!typeof(IBindingList).IsAssignableFrom(property.PropertyType))
			{
				rectangle.X += rectangle.Size.Width + 5;
				string text2 = $"{property.Name}: {property.GetValue(row.view)}";
				rectangle.Size = g.MeasureString(text2, grid.Font).ToSize();
				rectangle.Y = bounds.Y + bounds.Height - rectangle.Height;
				g.DrawString(text2, grid.Font, ResPool.GetSolidBrush(grid.ParentRowsForeColor), rectangle, stringFormat);
			}
		}
		if (!grid.FlatMode)
		{
			CPDrawBorder3D(g, bounds, Border3DStyle.RaisedInner, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
		}
	}

	public override void DataGridPaintRowHeaderArrow(Graphics g, Rectangle bounds, DataGrid grid)
	{
		Point[] array = new Point[3];
		Rectangle rectangle = new Rectangle(bounds.X + bounds.Width / 4, bounds.Y + bounds.Height / 4, bounds.Width / 2, bounds.Height / 2);
		int num = rectangle.Left + rectangle.Width / 2;
		int y = rectangle.Top + rectangle.Height / 2;
		int num2 = Math.Max(1, rectangle.Width / 8);
		rectangle.X -= num2;
		num -= num2;
		Point point = new Point(num, rectangle.Top - 1);
		Point point2 = new Point(num, rectangle.Bottom);
		Point point3 = new Point(rectangle.Right, y);
		array[0] = point;
		array[1] = point2;
		array[2] = point3;
		g.FillPolygon(ResPool.GetSolidBrush(grid.CurrentTableStyle.CurrentHeaderForeColor), array, FillMode.Winding);
	}

	public override void DataGridPaintRowHeaderStar(Graphics g, Rectangle bounds, DataGrid grid)
	{
		int num = bounds.X + 4;
		int num2 = bounds.Y + 3;
		Pen pen = ResPool.GetPen(grid.CurrentTableStyle.CurrentHeaderForeColor);
		g.DrawLine(pen, num + 4, num2, num + 4, num2 + 8);
		g.DrawLine(pen, num, num2 + 4, num + 8, num2 + 4);
		g.DrawLine(pen, num + 1, num2 + 1, num + 7, num2 + 7);
		g.DrawLine(pen, num + 7, num2 + 1, num + 1, num2 + 7);
	}

	public override void DataGridPaintRowHeader(Graphics g, Rectangle bounds, int row, DataGrid grid)
	{
		bool flag = grid.ShowEditRow && row == grid.DataGridRows.Length - 1;
		bool flag2 = row == grid.CurrentCell.RowNumber;
		g.FillRectangle(ResPool.GetSolidBrush(grid.CurrentTableStyle.CurrentHeaderBackColor), bounds);
		if (flag2)
		{
			if (grid.IsChanging)
			{
				g.DrawString("...", grid.Font, ResPool.GetSolidBrush(grid.CurrentTableStyle.CurrentHeaderForeColor), bounds);
			}
			else
			{
				Rectangle bounds2 = new Rectangle(bounds.X - 2, bounds.Y, 18, 18);
				DataGridPaintRowHeaderArrow(g, bounds2, grid);
			}
		}
		else if (flag)
		{
			DataGridPaintRowHeaderStar(g, bounds, grid);
		}
		if (!grid.FlatMode && !flag)
		{
			CPDrawBorder3D(g, bounds, Border3DStyle.RaisedInner, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
		}
	}

	public override void DataGridPaintRows(Graphics g, Rectangle cells, Rectangle clip, DataGrid grid)
	{
		Rectangle rectangle = default(Rectangle);
		Rectangle rect = default(Rectangle);
		int visibleRowCount = grid.VisibleRowCount;
		bool flag = false;
		if (grid.RowsCount < grid.DataGridRows.Length && grid.FirstVisibleRow + grid.VisibleRowCount >= grid.DataGridRows.Length)
		{
			flag = true;
		}
		rectangle.Width = cells.Width + grid.RowHeadersArea.Width;
		for (int i = 0; i < visibleRowCount; i++)
		{
			int num = grid.FirstVisibleRow + i;
			if (num == grid.DataGridRows.Length - 1)
			{
				rectangle.Height = grid.DataGridRows[num].Height;
			}
			else
			{
				rectangle.Height = grid.DataGridRows[num + 1].VerticalOffset - grid.DataGridRows[num].VerticalOffset;
			}
			rectangle.Y = cells.Y + grid.DataGridRows[num].VerticalOffset - grid.DataGridRows[grid.FirstVisibleRow].VerticalOffset;
			if (clip.IntersectsWith(rectangle))
			{
				if (grid.CurrentTableStyle.HasRelations && (!flag || num != grid.DataGridRows.Length - 1))
				{
					DataGridPaintRelationRow(g, num, rectangle, is_newrow: false, clip, grid);
				}
				else
				{
					DataGridPaintRow(g, num, rectangle, flag && num == grid.DataGridRows.Length - 1, clip, grid);
				}
			}
		}
		rect.X = 0;
		if (visibleRowCount == 0)
		{
			rect.Y = cells.Y;
		}
		else
		{
			rect.Y = rectangle.Y + rectangle.Height;
		}
		rect.Height = cells.Y + cells.Height - rectangle.Y - rectangle.Height;
		rect.Width = cells.Width + grid.RowHeadersArea.Width;
		g.FillRectangle(ResPool.GetSolidBrush(grid.BackgroundColor), rect);
	}

	public override void DataGridPaintRelationRow(Graphics g, int row, Rectangle row_rect, bool is_newrow, Rectangle clip, DataGrid grid)
	{
		Rectangle rect = default(Rectangle);
		Pen pen = ThemeEngine.Current.ResPool.GetPen(grid.CurrentTableStyle.ForeColor);
		if (grid.CurrentTableStyle.CurrentRowHeadersVisible)
		{
			Rectangle rectangle = row_rect;
			rectangle.Width = grid.RowHeaderWidth;
			row_rect.X += grid.RowHeaderWidth;
			if (clip.IntersectsWith(rectangle))
			{
				DataGridPaintRowHeader(g, rectangle, row, grid);
			}
			rect = rectangle;
			rect.X += rect.Width / 2;
			rect.Y += 3;
			rect.Width = 8;
			rect.Height = 8;
			g.DrawRectangle(pen, rect);
			g.DrawLine(pen, rect.X + 2, rect.Y + rect.Height / 2, rect.X + rect.Width - 2, rect.Y + rect.Height / 2);
			if (!grid.IsExpanded(row))
			{
				g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + 2, rect.X + rect.Width / 2, rect.Y + rect.Height - 2);
			}
		}
		Rectangle row_rect2 = row_rect;
		if (grid.DataGridRows[row].IsExpanded)
		{
			row_rect2.Height -= grid.DataGridRows[row].RelationHeight;
		}
		DataGridPaintRowContents(g, row, row_rect2, is_newrow, clip, grid);
		if (!grid.DataGridRows[row].IsExpanded)
		{
			return;
		}
		string[] relations = grid.CurrentTableStyle.Relations;
		StringBuilder stringBuilder = new StringBuilder(string.Empty);
		for (int i = 0; i < relations.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(relations[i]);
		}
		string s = stringBuilder.ToString();
		StringFormat stringFormat = new StringFormat();
		stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
		Rectangle rect2 = row_rect;
		rect2.X = row_rect2.X + grid.GetColumnStartingPixel(grid.FirstVisibleColumn) - grid.HorizPixelOffset;
		rect2.Y += row_rect2.Height;
		rect2.Height = grid.DataGridRows[row].RelationHeight;
		rect2.Width = 0;
		int num = grid.FirstVisibleColumn + grid.VisibleColumnCount;
		for (int j = grid.FirstVisibleColumn; j < num; j++)
		{
			if (grid.CurrentTableStyle.GridColumnStyles[j].bound)
			{
				rect2.Width += grid.CurrentTableStyle.GridColumnStyles[j].Width;
			}
		}
		rect2.Width = Math.Max(rect2.Width, grid.DataGridRows[row].relation_area.Width);
		g.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(grid.CurrentTableStyle.BackColor), rect2);
		Rectangle relation_area = grid.DataGridRows[row].relation_area;
		relation_area.Y = rect2.Y;
		relation_area.Height--;
		g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + rect.Height, rect.X + rect.Width / 2, relation_area.Y + relation_area.Height / 2);
		g.DrawLine(pen, rect.X + rect.Width / 2, relation_area.Y + relation_area.Height / 2, relation_area.X, relation_area.Y + relation_area.Height / 2);
		g.DrawRectangle(pen, relation_area);
		g.DrawString(s, grid.LinkFont, ResPool.GetSolidBrush(grid.LinkColor), relation_area, stringFormat);
		if (row_rect.X + row_rect.Width > rect2.X + rect2.Width)
		{
			Rectangle rect3 = default(Rectangle);
			rect3.X = rect2.X + rect2.Width;
			rect3.Width = row_rect.X + row_rect.Width - rect2.X - rect2.Width;
			rect3.Y = row_rect.Y;
			rect3.Height = row_rect.Height;
			if (clip.IntersectsWith(rect3))
			{
				g.FillRectangle(ResPool.GetSolidBrush(grid.BackgroundColor), rect3);
			}
		}
	}

	public override void DataGridPaintRowContents(Graphics g, int row, Rectangle row_rect, bool is_newrow, Rectangle clip, DataGrid grid)
	{
		Rectangle rectangle = default(Rectangle);
		Rectangle rect = Rectangle.Empty;
		rectangle.Y = row_rect.Y;
		rectangle.Height = row_rect.Height;
		Color color;
		Color color2;
		if (grid.IsSelected(row))
		{
			color = grid.SelectionBackColor;
			color2 = grid.SelectionForeColor;
		}
		else
		{
			color = ((row % 2 != 0) ? grid.AlternatingBackColor : grid.BackColor);
			color2 = grid.ForeColor;
		}
		Brush solidBrush = ResPool.GetSolidBrush(color);
		Brush solidBrush2 = ResPool.GetSolidBrush(color2);
		int num = grid.FirstVisibleColumn + grid.VisibleColumnCount;
		DataGridCell currentCell = grid.CurrentCell;
		if (num > 0)
		{
			Region clip2 = g.Clip;
			for (int i = grid.FirstVisibleColumn; i < num; i++)
			{
				if (!grid.CurrentTableStyle.GridColumnStyles[i].bound)
				{
					continue;
				}
				int columnStartingPixel = grid.GetColumnStartingPixel(i);
				rectangle.X = row_rect.X + columnStartingPixel - grid.HorizPixelOffset;
				rectangle.Width = grid.CurrentTableStyle.GridColumnStyles[i].Width;
				if (clip.IntersectsWith(rectangle))
				{
					Region region = new Region(rectangle);
					region.Intersect(row_rect);
					region.Intersect(clip2);
					g.Clip = region;
					Brush backBrush = solidBrush;
					Brush foreBrush = solidBrush2;
					if (grid.is_editing && i == currentCell.ColumnNumber && row == currentCell.RowNumber)
					{
						backBrush = ResPool.GetSolidBrush(grid.BackColor);
						foreBrush = ResPool.GetSolidBrush(grid.ForeColor);
					}
					if (is_newrow)
					{
						grid.CurrentTableStyle.GridColumnStyles[i].PaintNewRow(g, rectangle, backBrush, foreBrush);
					}
					else
					{
						grid.CurrentTableStyle.GridColumnStyles[i].Paint(g, rectangle, grid.ListManager, row, backBrush, foreBrush, grid.RightToLeft == RightToLeft.Yes);
					}
					region.Dispose();
				}
			}
			g.Clip = clip2;
			if (row_rect.X + row_rect.Width > rectangle.X + rectangle.Width)
			{
				rect.X = rectangle.X + rectangle.Width;
				rect.Width = row_rect.X + row_rect.Width - rectangle.X - rectangle.Width;
				rect.Y = row_rect.Y;
				rect.Height = row_rect.Height;
			}
		}
		else
		{
			rect = row_rect;
		}
		if (!rect.IsEmpty && clip.IntersectsWith(rect))
		{
			g.FillRectangle(ResPool.GetSolidBrush(grid.BackgroundColor), rect);
		}
	}

	public override void DataGridPaintRow(Graphics g, int row, Rectangle row_rect, bool is_newrow, Rectangle clip, DataGrid grid)
	{
		if (grid.CurrentTableStyle.CurrentRowHeadersVisible)
		{
			Rectangle rectangle = row_rect;
			rectangle.Width = grid.RowHeaderWidth;
			row_rect.X += grid.RowHeaderWidth;
			if (clip.IntersectsWith(rectangle))
			{
				DataGridPaintRowHeader(g, rectangle, row, grid);
			}
		}
		DataGridPaintRowContents(g, row, row_rect, is_newrow, clip, grid);
	}

	public override bool DataGridViewRowHeaderCellDrawBackground(DataGridViewRowHeaderCell cell, Graphics g, Rectangle bounds)
	{
		return false;
	}

	public override bool DataGridViewRowHeaderCellDrawSelectionBackground(DataGridViewRowHeaderCell cell)
	{
		return false;
	}

	public override bool DataGridViewRowHeaderCellDrawBorder(DataGridViewRowHeaderCell cell, Graphics g, Rectangle bounds)
	{
		return false;
	}

	public override bool DataGridViewColumnHeaderCellDrawBackground(DataGridViewColumnHeaderCell cell, Graphics g, Rectangle bounds)
	{
		return false;
	}

	public override bool DataGridViewColumnHeaderCellDrawBorder(DataGridViewColumnHeaderCell cell, Graphics g, Rectangle bounds)
	{
		return false;
	}

	public override bool DataGridViewHeaderCellHasPressedStyle(DataGridView dataGridView)
	{
		return false;
	}

	public override bool DataGridViewHeaderCellHasHotStyle(DataGridView dataGridView)
	{
		return false;
	}

	protected virtual void DateTimePickerDrawBorder(DateTimePicker dateTimePicker, Graphics g, Rectangle clippingArea)
	{
		CPDrawBorder3D(g, dateTimePicker.ClientRectangle, Border3DStyle.Sunken, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, dateTimePicker.BackColor);
	}

	protected virtual void DateTimePickerDrawDropDownButton(DateTimePicker dateTimePicker, Graphics g, Rectangle clippingArea)
	{
		ButtonState state = (dateTimePicker.is_drop_down_visible ? ButtonState.Pushed : ButtonState.Normal);
		g.FillRectangle(ResPool.GetSolidBrush(ColorControl), dateTimePicker.drop_down_arrow_rect);
		CPDrawComboButton(g, dateTimePicker.drop_down_arrow_rect, state);
	}

	public override void DrawDateTimePicker(Graphics dc, Rectangle clip_rectangle, DateTimePicker dtp)
	{
		if (!clip_rectangle.IntersectsWith(dtp.ClientRectangle))
		{
			return;
		}
		Rectangle clientRectangle = dtp.ClientRectangle;
		DateTimePickerDrawBorder(dtp, dc, clip_rectangle);
		if (clip_rectangle.IntersectsWith(dtp.drop_down_arrow_rect))
		{
			clientRectangle.Inflate(-2, -2);
			if (!dtp.ShowUpDown)
			{
				DateTimePickerDrawDropDownButton(dtp, dc, clip_rectangle);
			}
			else
			{
				ButtonState state = (dtp.is_up_pressed ? ButtonState.Pushed : ButtonState.Normal);
				ButtonState state2 = (dtp.is_down_pressed ? ButtonState.Pushed : ButtonState.Normal);
				Rectangle drop_down_arrow_rect = dtp.drop_down_arrow_rect;
				Rectangle drop_down_arrow_rect2 = dtp.drop_down_arrow_rect;
				drop_down_arrow_rect.Height /= 2;
				drop_down_arrow_rect2.Y = drop_down_arrow_rect.Height;
				drop_down_arrow_rect2.Height = dtp.Height - drop_down_arrow_rect.Height;
				if (drop_down_arrow_rect2.Height > drop_down_arrow_rect.Height)
				{
					drop_down_arrow_rect2.Y++;
					drop_down_arrow_rect2.Height--;
				}
				drop_down_arrow_rect.Inflate(-1, -1);
				drop_down_arrow_rect2.Inflate(-1, -1);
				ControlPaint.DrawScrollButton(dc, drop_down_arrow_rect, ScrollButton.Min, state);
				ControlPaint.DrawScrollButton(dc, drop_down_arrow_rect2, ScrollButton.Down, state2);
			}
		}
		if (!clip_rectangle.IntersectsWith(dtp.date_area_rect))
		{
			return;
		}
		dc.FillRectangle(SystemBrushes.Window, dtp.date_area_rect);
		Rectangle date_area_rect = dtp.date_area_rect;
		if (dtp.ShowCheckBox)
		{
			Rectangle checkBoxRect = dtp.CheckBoxRect;
			date_area_rect.X = date_area_rect.X + checkBoxRect.Width + 8;
			date_area_rect.Width = date_area_rect.Width - checkBoxRect.Width - 8;
			ButtonState state3 = (dtp.Checked ? ButtonState.Checked : ButtonState.Normal);
			CPDrawCheckBox(dc, checkBoxRect, state3);
			if (dtp.is_checkbox_selected)
			{
				CPDrawFocusRectangle(dc, checkBoxRect, dtp.foreground_color, dtp.background_color);
			}
		}
		using StringFormat stringFormat = StringFormat.GenericTypographic;
		stringFormat.LineAlignment = StringAlignment.Near;
		stringFormat.Alignment = StringAlignment.Near;
		stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox;
		stringFormat.FormatFlags &= ~StringFormatFlags.NoClip;
		if (dtp.part_data.Length > 0 && dtp.part_data[0].drawing_rectangle.IsEmpty)
		{
			for (int i = 0; i < dtp.part_data.Length; i++)
			{
				DateTimePicker.PartData partData = dtp.part_data[i];
				RectangleF drawing_rectangle = default(RectangleF);
				string text = partData.GetText(dtp.Value);
				drawing_rectangle.Size = dc.MeasureString(text, dtp.Font, 250, stringFormat);
				if (!partData.is_literal)
				{
					drawing_rectangle.Width = Math.Max(dtp.CalculateMaxWidth(partData.value, dc, stringFormat), drawing_rectangle.Width);
				}
				if (i > 0)
				{
					drawing_rectangle.X = dtp.part_data[i - 1].drawing_rectangle.Right;
				}
				else
				{
					drawing_rectangle.X = date_area_rect.X;
				}
				drawing_rectangle.Y = 2f;
				drawing_rectangle.Inflate(1f, 0f);
				partData.drawing_rectangle = drawing_rectangle;
			}
		}
		Brush solidBrush = ResPool.GetSolidBrush((!dtp.ShowCheckBox || dtp.Checked) ? dtp.ForeColor : SystemColors.GrayText);
		RectangleF rectangleF = clip_rectangle;
		for (int j = 0; j < dtp.part_data.Length; j++)
		{
			DateTimePicker.PartData partData2 = dtp.part_data[j];
			if (rectangleF.IntersectsWith(partData2.drawing_rectangle))
			{
				string text2 = ((dtp.editing_part_index != j) ? partData2.GetText(dtp.Value) : dtp.editing_text);
				PointF location = default(PointF);
				SizeF size = dc.MeasureString(text2, dtp.Font, 250, stringFormat);
				location.X = partData2.drawing_rectangle.Left + partData2.drawing_rectangle.Width / 2f - size.Width / 2f;
				location.Y = partData2.drawing_rectangle.Top + partData2.drawing_rectangle.Height / 2f - size.Height / 2f;
				RectangleF a = new RectangleF(location, size);
				a = RectangleF.Intersect(a, date_area_rect);
				if (a.IsEmpty)
				{
					break;
				}
				if (a.Right >= (float)date_area_rect.Right)
				{
					stringFormat.FormatFlags &= ~StringFormatFlags.NoClip;
				}
				else
				{
					stringFormat.FormatFlags |= StringFormatFlags.NoClip;
				}
				if (partData2.Selected)
				{
					dc.FillRectangle(SystemBrushes.Highlight, a);
					dc.DrawString(text2, dtp.Font, SystemBrushes.HighlightText, a, stringFormat);
				}
				else
				{
					dc.DrawString(text2, dtp.Font, solidBrush, a, stringFormat);
				}
				if (partData2.drawing_rectangle.Right > (float)date_area_rect.Right)
				{
					break;
				}
			}
		}
	}

	public override Rectangle DateTimePickerGetDropDownButtonArea(DateTimePicker dateTimePicker)
	{
		Rectangle clientRectangle = dateTimePicker.ClientRectangle;
		clientRectangle.X = clientRectangle.Right - SystemInformation.VerticalScrollBarWidth - 2;
		if (clientRectangle.Width > SystemInformation.VerticalScrollBarWidth + 2)
		{
			clientRectangle.Width = SystemInformation.VerticalScrollBarWidth;
		}
		else
		{
			clientRectangle.Width = Math.Max(clientRectangle.Width - 2, 0);
		}
		clientRectangle.Inflate(0, -2);
		return clientRectangle;
	}

	public override Rectangle DateTimePickerGetDateArea(DateTimePicker dateTimePicker)
	{
		Rectangle clientRectangle = dateTimePicker.ClientRectangle;
		if (dateTimePicker.ShowUpDown)
		{
			if (clientRectangle.Width > 17)
			{
				clientRectangle.Width -= 17;
			}
			else
			{
				clientRectangle.Width = 0;
			}
		}
		else if (clientRectangle.Width > SystemInformation.VerticalScrollBarWidth + 4)
		{
			clientRectangle.Width -= SystemInformation.VerticalScrollBarWidth;
		}
		else
		{
			clientRectangle.Width = 0;
		}
		clientRectangle.Inflate(-2, -2);
		return clientRectangle;
	}

	public override void DrawGroupBox(Graphics dc, Rectangle area, GroupBox box)
	{
		dc.FillRectangle(GetControlBackBrush(box.BackColor), box.ClientRectangle);
		StringFormat stringFormat = new StringFormat();
		stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
		SizeF sizeF = dc.MeasureString(box.Text, box.Font);
		int num = 0;
		if (sizeF.Width > 0f)
		{
			num = (int)sizeF.Width + 7;
			if (num > box.Width - 16)
			{
				num = box.Width - 16;
			}
		}
		int num2 = box.Font.Height / 2;
		Region clip = dc.Clip;
		dc.SetClip(new Rectangle(10, 0, num, box.Font.Height), CombineMode.Exclude);
		CPDrawBorder3D(dc, new Rectangle(0, num2, box.Width, box.Height - num2), Border3DStyle.Etched, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, box.BackColor);
		dc.Clip = clip;
		if (box.Text.Length != 0)
		{
			if (box.Enabled)
			{
				dc.DrawString(box.Text, box.Font, ResPool.GetSolidBrush(box.ForeColor), 10f, 0f, stringFormat);
			}
			else
			{
				CPDrawStringDisabled(dc, box.Text, box.Font, box.BackColor, new RectangleF(10f, 0f, num, box.Font.Height), stringFormat);
			}
		}
		stringFormat.Dispose();
	}

	public override void DrawListBoxItem(ListBox ctrl, DrawItemEventArgs e)
	{
		Color color;
		Color color2;
		if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
		{
			color = ColorHighlight;
			color2 = ColorHighlightText;
		}
		else
		{
			color = e.BackColor;
			color2 = e.ForeColor;
		}
		e.Graphics.FillRectangle(ResPool.GetSolidBrush(color), e.Bounds);
		e.Graphics.DrawString(ctrl.GetItemText(ctrl.Items[e.Index]), e.Font, ResPool.GetSolidBrush(color2), e.Bounds, ctrl.StringFormat);
		if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
		{
			CPDrawFocusRectangle(e.Graphics, e.Bounds, color2, color);
		}
	}

	public override void DrawListViewItems(Graphics dc, Rectangle clip, ListView control)
	{
		bool flag = control.View == View.Details;
		int firstVisibleIndex = control.FirstVisibleIndex;
		int lastVisibleIndex = control.LastVisibleIndex;
		if (control.VirtualMode)
		{
			control.OnCacheVirtualItems(new CacheVirtualItemsEventArgs(firstVisibleIndex, lastVisibleIndex));
		}
		for (int i = firstVisibleIndex; i <= lastVisibleIndex; i++)
		{
			ListViewItem itemAtDisplayIndex = control.GetItemAtDisplayIndex(i);
			if (!clip.IntersectsWith(itemAtDisplayIndex.Bounds))
			{
				continue;
			}
			bool flag2 = false;
			if (control.OwnerDraw)
			{
				flag2 = DrawListViewItemOwnerDraw(dc, itemAtDisplayIndex, i);
			}
			if (!flag2)
			{
				DrawListViewItem(dc, control, itemAtDisplayIndex);
				if (control.View == View.Details)
				{
					DrawListViewSubItems(dc, control, itemAtDisplayIndex);
				}
			}
		}
		if (control.UsingGroups)
		{
			for (int j = 0; j < control.Groups.InternalCount; j++)
			{
				ListViewGroup internalGroup = control.Groups.GetInternalGroup(j);
				if (internalGroup.ItemCount > 0 && clip.IntersectsWith(internalGroup.HeaderBounds))
				{
					DrawListViewGroupHeader(dc, control, internalGroup);
				}
			}
		}
		ListViewInsertionMark insertionMark = control.InsertionMark;
		int index = insertionMark.Index;
		if (Application.VisualStylesEnabled && insertionMark.Bounds != Rectangle.Empty && control.View != View.Details && control.View != View.List && index > -1 && index < control.Items.Count)
		{
			Brush solidBrush = ResPool.GetSolidBrush(insertionMark.Color);
			dc.FillRectangle(solidBrush, insertionMark.Line);
			dc.FillPolygon(solidBrush, insertionMark.TopTriangle);
			dc.FillPolygon(solidBrush, insertionMark.BottomTriangle);
		}
		if (flag && control.GridLines && !control.UsingGroups)
		{
			Size clientSize = control.ClientSize;
			int num = ((control.HeaderStyle != 0) ? control.header_control.Height : 0);
			foreach (ColumnHeader column in control.Columns)
			{
				int num2 = column.Rect.Right - control.h_marker;
				dc.DrawLine(SystemPens.Control, num2, num, num2, clientSize.Height);
			}
			int num3 = control.ItemSize.Height;
			if (num3 == 0)
			{
				num3 = control.Font.Height + 2;
			}
			for (int k = num + num3 - control.v_marker % num3; k < clientSize.Height; k += num3)
			{
				dc.DrawLine(SystemPens.Control, 0, k, clientSize.Width, k);
			}
		}
		if (control.h_scroll.Visible && control.v_scroll.Visible)
		{
			Rectangle rect = default(Rectangle);
			rect.X = control.h_scroll.Location.X + control.h_scroll.Width;
			rect.Width = control.v_scroll.Width;
			rect.Y = control.v_scroll.Location.Y + control.v_scroll.Height;
			rect.Height = control.h_scroll.Height;
			dc.FillRectangle(SystemBrushes.Control, rect);
		}
		Rectangle boxSelectRectangle = control.item_control.BoxSelectRectangle;
		if (!boxSelectRectangle.Size.IsEmpty)
		{
			dc.DrawRectangle(ResPool.GetDashPen(ColorControlText, DashStyle.Dot), boxSelectRectangle);
		}
	}

	public override void DrawListViewHeader(Graphics dc, Rectangle clip, ListView control)
	{
		if (control.View != View.Details || control.HeaderStyle == ColumnHeaderStyle.None)
		{
			return;
		}
		dc.FillRectangle(SystemBrushes.Control, 0, 0, control.TotalWidth, control.Font.Height + 5);
		if (control.Columns.Count <= 0)
		{
			return;
		}
		foreach (ColumnHeader column in control.Columns)
		{
			Rectangle rect = column.Rect;
			rect.X -= control.h_marker;
			bool flag = false;
			if (control.OwnerDraw)
			{
				flag = DrawListViewColumnHeaderOwnerDraw(dc, control, column, rect);
			}
			if (flag)
			{
				continue;
			}
			ListViewDrawColumnHeaderBackground(control, column, dc, rect, clip);
			rect.X += 5;
			rect.Width -= 10;
			if (rect.Width <= 0)
			{
				continue;
			}
			int num = ((control.SmallImageList != null) ? ((!(column.ImageKey == string.Empty)) ? control.SmallImageList.Images.IndexOfKey(column.ImageKey) : column.ImageIndex) : (-1));
			if (num > -1 && num < control.SmallImageList.Images.Count)
			{
				int num2 = control.SmallImageList.ImageSize.Width + 5;
				int num3 = (int)dc.MeasureString(column.Text, control.Font).Width;
				int num4 = rect.X;
				switch (column.TextAlign)
				{
				case HorizontalAlignment.Right:
					num4 = rect.Right - (num3 + num2);
					break;
				case HorizontalAlignment.Center:
					num4 = (rect.Width - (num3 + num2)) / 2 + rect.X;
					break;
				}
				if (num4 < rect.X)
				{
					num4 = rect.X;
				}
				control.SmallImageList.Draw(dc, new Point(num4, rect.Y), num);
				rect.X += num2;
				rect.Width -= num2;
			}
			dc.DrawString(column.Text, control.Font, SystemBrushes.ControlText, rect, column.Format);
		}
		int num5 = control.GetReorderedColumn(control.Columns.Count - 1).Rect.Right - control.h_marker;
		if (num5 < control.Right)
		{
			Rectangle rect2 = control.Columns[0].Rect;
			rect2.X = num5;
			rect2.Width = control.Right - num5;
			ListViewDrawUnusedHeaderBackground(control, dc, rect2, clip);
		}
	}

	protected virtual void ListViewDrawColumnHeaderBackground(ListView listView, ColumnHeader columnHeader, Graphics g, Rectangle area, Rectangle clippingArea)
	{
		ButtonState state = ((listView.HeaderStyle != ColumnHeaderStyle.Clickable) ? ButtonState.Flat : (columnHeader.Pressed ? ButtonState.Pushed : ButtonState.Normal));
		CPDrawButton(g, area, state);
	}

	protected virtual void ListViewDrawUnusedHeaderBackground(ListView listView, Graphics g, Rectangle area, Rectangle clippingArea)
	{
		ButtonState state = ((listView.HeaderStyle != ColumnHeaderStyle.Clickable) ? ButtonState.Flat : ButtonState.Normal);
		CPDrawButton(g, area, state);
	}

	public override void DrawListViewHeaderDragDetails(Graphics dc, ListView view, ColumnHeader col, int target_x)
	{
		Rectangle rect = col.Rect;
		rect.X -= view.h_marker;
		Color color = Color.FromArgb(127, ColorControlDark.R, ColorControlDark.G, ColorControlDark.B);
		dc.FillRectangle(ResPool.GetSolidBrush(color), rect);
		rect.X += 3;
		rect.Width -= 8;
		if (rect.Width > 0)
		{
			color = Color.FromArgb(127, ColorControlText.R, ColorControlText.G, ColorControlText.B);
			dc.DrawString(col.Text, view.Font, ResPool.GetSolidBrush(color), rect, col.Format);
			dc.DrawLine(ResPool.GetSizedPen(ColorHighlight, 2), target_x, 0, target_x, col.Rect.Height);
		}
	}

	protected virtual bool DrawListViewColumnHeaderOwnerDraw(Graphics dc, ListView control, ColumnHeader column, Rectangle bounds)
	{
		ListViewItemStates listViewItemStates = ListViewItemStates.ShowKeyboardCues;
		if (column.Pressed)
		{
			listViewItemStates |= ListViewItemStates.Selected;
		}
		DrawListViewColumnHeaderEventArgs drawListViewColumnHeaderEventArgs = new DrawListViewColumnHeaderEventArgs(dc, bounds, column.Index, column, listViewItemStates, SystemColors.ControlText, ThemeEngine.Current.ColorControl, DefaultFont);
		control.OnDrawColumnHeader(drawListViewColumnHeaderEventArgs);
		return !drawListViewColumnHeaderEventArgs.DrawDefault;
	}

	protected virtual bool DrawListViewItemOwnerDraw(Graphics dc, ListViewItem item, int index)
	{
		ListViewItemStates listViewItemStates = ListViewItemStates.ShowKeyboardCues;
		if (item.Selected)
		{
			listViewItemStates |= ListViewItemStates.Selected;
		}
		if (item.Focused)
		{
			listViewItemStates |= ListViewItemStates.Focused;
		}
		DrawListViewItemEventArgs drawListViewItemEventArgs = new DrawListViewItemEventArgs(dc, item, item.Bounds, index, listViewItemStates);
		item.ListView.OnDrawItem(drawListViewItemEventArgs);
		if (drawListViewItemEventArgs.DrawDefault)
		{
			return false;
		}
		if (item.ListView.View == View.Details)
		{
			int num = Math.Min(item.ListView.Columns.Count, item.SubItems.Count);
			for (int i = 0; i < num; i++)
			{
				if (!DrawListViewSubItemOwnerDraw(dc, item, listViewItemStates, i))
				{
					if (i == 0)
					{
						DrawListViewItem(dc, item.ListView, item);
					}
					else
					{
						DrawListViewSubItem(dc, item.ListView, item, i);
					}
				}
			}
		}
		return true;
	}

	protected virtual void DrawListViewItem(Graphics dc, ListView control, ListViewItem item)
	{
		Rectangle checkRectReal = item.CheckRectReal;
		Rectangle bounds = item.GetBounds(ItemBoundsPortion.Icon);
		Rectangle bounds2 = item.GetBounds(ItemBoundsPortion.Entire);
		Rectangle bounds3 = item.GetBounds(ItemBoundsPortion.Label);
		if (control.CheckBoxes && control.View != View.Tile)
		{
			if (control.StateImageList == null)
			{
				int num = Math.Max(3, checkRectReal.Width / 6);
				int num2 = Math.Max(1, checkRectReal.Width / 12);
				dc.FillRectangle(SystemBrushes.Window, checkRectReal);
				Rectangle rect = new Rectangle(checkRectReal.X + 2, checkRectReal.Y + 2, checkRectReal.Width - 4, checkRectReal.Height - 4);
				Pen sizedPen = ResPool.GetSizedPen(ColorWindowText, 2);
				dc.DrawRectangle(sizedPen, rect);
				if (item.Checked)
				{
					Pen sizedPen2 = ResPool.GetSizedPen(ColorWindowText, 1);
					rect.X++;
					rect.Y++;
					int num3 = rect.Width / 5;
					int num4 = rect.Height / 3;
					for (int i = 0; i < num; i++)
					{
						dc.DrawLine(sizedPen2, rect.Left + num3, rect.Top + num4 + i, rect.Left + num3 + 2 * num2, rect.Top + num4 + 2 * num2 + i);
						dc.DrawLine(sizedPen2, rect.Left + num3 + 2 * num2, rect.Top + num4 + 2 * num2 + i, rect.Left + num3 + 6 * num2, rect.Top + num4 - 2 * num2 + i);
					}
				}
			}
			else
			{
				int num5 = ((!item.Checked) ? ((control.StateImageList.Images.Count <= 0) ? (-1) : 0) : ((control.StateImageList.Images.Count > 1) ? 1 : (-1)));
				if (num5 > -1)
				{
					control.StateImageList.Draw(dc, checkRectReal.Location, num5);
				}
			}
		}
		ImageList imageList = ((control.View != 0 && control.View != View.Tile) ? control.SmallImageList : control.LargeImageList);
		if (imageList != null)
		{
			int num6 = ((!(item.ImageKey != string.Empty)) ? item.ImageIndex : imageList.Images.IndexOfKey(item.ImageKey));
			if (num6 > -1 && num6 < imageList.Images.Count)
			{
				imageList.Draw(dc, bounds.Location, num6);
			}
		}
		StringFormat stringFormat = new StringFormat();
		if (control.View == View.SmallIcon || control.View == View.LargeIcon)
		{
			stringFormat.LineAlignment = StringAlignment.Near;
		}
		else
		{
			stringFormat.LineAlignment = StringAlignment.Center;
		}
		if (control.View == View.LargeIcon)
		{
			stringFormat.Alignment = StringAlignment.Center;
		}
		else
		{
			stringFormat.Alignment = StringAlignment.Near;
		}
		if (control.LabelWrap && control.View != View.Details && control.View != View.Tile)
		{
			stringFormat.FormatFlags = StringFormatFlags.LineLimit;
		}
		else
		{
			stringFormat.FormatFlags = StringFormatFlags.NoWrap;
		}
		if ((control.View == View.LargeIcon && !item.Focused) || control.View == View.Details || control.View == View.Tile)
		{
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
		}
		Rectangle rectangle = bounds3;
		if (control.View == View.Details)
		{
			Size size = Size.Ceiling(dc.MeasureString(item.Text, item.Font));
			if (!control.FullRowSelect)
			{
				rectangle.Width = Math.Min(size.Width + 4, bounds3.Width);
			}
		}
		if (item.Selected && control.Focused)
		{
			dc.FillRectangle(SystemBrushes.Highlight, rectangle);
		}
		else if (item.Selected && !control.HideSelection)
		{
			dc.FillRectangle(SystemBrushes.Control, rectangle);
		}
		else
		{
			dc.FillRectangle(ResPool.GetSolidBrush(item.BackColor), bounds3);
		}
		Brush brush = ((!control.Enabled) ? SystemBrushes.ControlLight : ((!item.Selected || !control.Focused) ? ResPool.GetSolidBrush(item.ForeColor) : SystemBrushes.HighlightText));
		if (control.View == View.Tile && Application.VisualStylesEnabled)
		{
			dc.DrawString(item.Text, item.Font, brush, item.SubItems[0].Bounds, stringFormat);
			int num7 = Math.Min(control.Columns.Count, item.SubItems.Count);
			for (int j = 1; j < num7; j++)
			{
				ListViewItem.ListViewSubItem listViewSubItem = item.SubItems[j];
				if (listViewSubItem.Text != null && listViewSubItem.Text.Length != 0)
				{
					Brush brush2 = ((!item.Selected || !control.Focused) ? GetControlForeBrush(listViewSubItem.ForeColor) : SystemBrushes.HighlightText);
					dc.DrawString(listViewSubItem.Text, listViewSubItem.Font, brush2, listViewSubItem.Bounds, stringFormat);
				}
			}
		}
		else if (item.Text != null && item.Text.Length > 0)
		{
			Font font = item.Font;
			if (control.HotTracking && item.Hot)
			{
				font = item.HotFont;
			}
			if (item.Selected && control.Focused)
			{
				dc.DrawString(item.Text, font, brush, rectangle, stringFormat);
			}
			else
			{
				dc.DrawString(item.Text, font, brush, bounds3, stringFormat);
			}
		}
		if (item.Focused && control.Focused)
		{
			Rectangle rectangle2 = rectangle;
			if (control.FullRowSelect && control.View == View.Details)
			{
				int num8 = 0;
				foreach (ColumnHeader column in control.Columns)
				{
					num8 += column.Width;
				}
				rectangle2 = new Rectangle(0, bounds2.Y, num8, bounds2.Height);
			}
			if (control.ShowFocusCues)
			{
				if (item.Selected)
				{
					CPDrawFocusRectangle(dc, rectangle2, ColorHighlightText, ColorHighlight);
				}
				else
				{
					CPDrawFocusRectangle(dc, rectangle2, control.ForeColor, control.BackColor);
				}
			}
		}
		stringFormat.Dispose();
	}

	protected virtual void DrawListViewSubItems(Graphics dc, ListView control, ListViewItem item)
	{
		int count = control.Columns.Count;
		int num = Math.Min(item.SubItems.Count, count);
		for (int i = 1; i < num; i++)
		{
			DrawListViewSubItem(dc, control, item, i);
		}
		Rectangle bounds = item.GetBounds(ItemBoundsPortion.Label);
		if (item.Selected && (control.Focused || !control.HideSelection) && control.FullRowSelect)
		{
			for (int j = num; j < count; j++)
			{
				ColumnHeader columnHeader = control.Columns[j];
				bounds.X = columnHeader.Rect.X - control.h_marker;
				bounds.Width = columnHeader.Wd;
				dc.FillRectangle((!control.Focused) ? SystemBrushes.Control : SystemBrushes.Highlight, bounds);
			}
		}
	}

	protected virtual void DrawListViewSubItem(Graphics dc, ListView control, ListViewItem item, int index)
	{
		ListViewItem.ListViewSubItem listViewSubItem = item.SubItems[index];
		ColumnHeader columnHeader = control.Columns[index];
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = columnHeader.Format.Alignment;
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.FormatFlags = StringFormatFlags.NoWrap;
		stringFormat.Trimming = StringTrimming.EllipsisCharacter;
		Rectangle bounds = listViewSubItem.Bounds;
		Rectangle rectangle = bounds;
		rectangle.X += 3;
		rectangle.Width -= ListViewItemPaddingWidth;
		SolidBrush solidBrush = null;
		SolidBrush solidBrush2 = null;
		Font font = null;
		if (item.UseItemStyleForSubItems)
		{
			solidBrush = ResPool.GetSolidBrush(item.BackColor);
			solidBrush2 = ResPool.GetSolidBrush(item.ForeColor);
			font = ((!control.HotTracking || !item.Hot) ? item.Font : item.HotFont);
		}
		else
		{
			solidBrush = ResPool.GetSolidBrush(listViewSubItem.BackColor);
			solidBrush2 = ResPool.GetSolidBrush(listViewSubItem.ForeColor);
			font = listViewSubItem.Font;
		}
		if (item.Selected && (control.Focused || !control.HideSelection) && control.FullRowSelect)
		{
			Brush brush;
			Brush brush2;
			if (control.Focused)
			{
				brush = SystemBrushes.Highlight;
				brush2 = SystemBrushes.HighlightText;
			}
			else
			{
				brush = SystemBrushes.Control;
				brush2 = solidBrush2;
			}
			dc.FillRectangle(brush, bounds);
			if (listViewSubItem.Text != null && listViewSubItem.Text.Length > 0)
			{
				dc.DrawString(listViewSubItem.Text, font, brush2, rectangle, stringFormat);
			}
		}
		else
		{
			dc.FillRectangle(solidBrush, bounds);
			if (listViewSubItem.Text != null && listViewSubItem.Text.Length > 0)
			{
				dc.DrawString(listViewSubItem.Text, font, solidBrush2, rectangle, stringFormat);
			}
		}
		stringFormat.Dispose();
	}

	protected virtual bool DrawListViewSubItemOwnerDraw(Graphics dc, ListViewItem item, ListViewItemStates state, int index)
	{
		ListView listView = item.ListView;
		ListViewItem.ListViewSubItem listViewSubItem = item.SubItems[index];
		DrawListViewSubItemEventArgs drawListViewSubItemEventArgs = new DrawListViewSubItemEventArgs(dc, listViewSubItem.Bounds, item, listViewSubItem, item.Index, index, listView.Columns[index], state);
		listView.OnDrawSubItem(drawListViewSubItemEventArgs);
		return !drawListViewSubItemEventArgs.DrawDefault;
	}

	protected virtual void DrawListViewGroupHeader(Graphics dc, ListView control, ListViewGroup group)
	{
		Rectangle headerBounds = group.HeaderBounds;
		Rectangle headerBounds2 = group.HeaderBounds;
		headerBounds.Offset(8, 0);
		headerBounds.Inflate(-8, 0);
		int num = control.Font.Height + 2;
		Font font = new Font(control.Font, control.Font.Style | FontStyle.Bold);
		Brush brush = new LinearGradientBrush(new Point(headerBounds2.Left, 0), new Point(headerBounds2.Left + ListViewGroupLineWidth, 0), SystemColors.Desktop, Color.White);
		Pen pen = new Pen(brush);
		StringFormat stringFormat = new StringFormat();
		switch (group.HeaderAlignment)
		{
		case HorizontalAlignment.Left:
			stringFormat.Alignment = StringAlignment.Near;
			break;
		case HorizontalAlignment.Center:
			stringFormat.Alignment = StringAlignment.Center;
			break;
		case HorizontalAlignment.Right:
			stringFormat.Alignment = StringAlignment.Far;
			break;
		}
		stringFormat.LineAlignment = StringAlignment.Near;
		dc.DrawString(group.Header, font, SystemBrushes.ControlText, headerBounds, stringFormat);
		dc.DrawLine(pen, headerBounds2.Left, headerBounds2.Top + num, headerBounds2.Left + ListViewGroupLineWidth, headerBounds2.Top + num);
		stringFormat.Dispose();
		font.Dispose();
		pen.Dispose();
		brush.Dispose();
	}

	public override int ListViewGetHeaderHeight(ListView listView, Font font)
	{
		return ListViewGetHeaderHeight(font);
	}

	private static int ListViewGetHeaderHeight(Font font)
	{
		return font.Height + 5;
	}

	public static int ListViewGetHeaderHeight()
	{
		return ListViewGetHeaderHeight(ThemeEngine.Current.DefaultFont);
	}

	public override void CalcItemSize(Graphics dc, MenuItem item, int y, int x, bool menuBar)
	{
		item.X = x;
		item.Y = y;
		if (!item.Visible)
		{
			item.Width = 0;
			item.Height = 0;
			return;
		}
		if (item.Separator)
		{
			item.Height = 6;
			item.Width = 20;
			return;
		}
		if (item.MeasureEventDefined)
		{
			MeasureItemEventArgs measureItemEventArgs = new MeasureItemEventArgs(dc, item.Index);
			item.PerformMeasureItem(measureItemEventArgs);
			item.Height = measureItemEventArgs.ItemHeight;
			item.Width = measureItemEventArgs.ItemWidth;
			return;
		}
		SizeF sizeF = dc.MeasureString(item.Text, MenuFont, int.MaxValue, string_format_menu_text);
		item.Width = (int)sizeF.Width;
		item.Height = (int)sizeF.Height;
		if (!menuBar)
		{
			if (item.Shortcut != 0 && item.ShowShortcut)
			{
				item.XTab = MenuCheckSize.Width + 8 + (int)sizeF.Width;
				sizeF = dc.MeasureString(" " + item.GetShortCutText(), MenuFont);
				item.Width += 8 + (int)sizeF.Width;
			}
			item.Width += 4 + MenuCheckSize.Width * 2;
		}
		else
		{
			item.Width += 8;
			x += item.Width;
		}
		if (item.Height < MenuHeight)
		{
			item.Height = MenuHeight;
		}
	}

	public override int CalcMenuBarSize(Graphics dc, Menu menu, int width)
	{
		int num = 0;
		int num2 = 0;
		menu.Height = 0;
		foreach (MenuItem menuItem in menu.MenuItems)
		{
			CalcItemSize(dc, menuItem, num2, num, menuBar: true);
			if (num + menuItem.Width > width)
			{
				menuItem.X = 0;
				num2 = (menuItem.Y = num2 + menuItem.Height);
				num = 0;
			}
			num += menuItem.Width;
			menuItem.MenuBar = true;
			if (num2 + menuItem.Height > menu.Height)
			{
				menu.Height = menuItem.Height + num2;
			}
		}
		menu.Width = width;
		return menu.Height;
	}

	public override void CalcPopupMenuSize(Graphics dc, Menu menu)
	{
		int num = 3;
		int num2 = 0;
		menu.Height = 0;
		while (num2 < menu.MenuItems.Count)
		{
			int num3 = 3;
			int num4 = 0;
			int i;
			for (i = num2; i < menu.MenuItems.Count; i++)
			{
				MenuItem menuItem = menu.MenuItems[i];
				if (i != num2 && (menuItem.Break || menuItem.BarBreak))
				{
					break;
				}
				CalcItemSize(dc, menuItem, num3, num, menuBar: false);
				num3 += menuItem.Height;
				if (menuItem.Width > num4)
				{
					num4 = menuItem.Width;
				}
			}
			int num5 = num2;
			while (num5 < i)
			{
				menu.MenuItems[num5].Width = num4;
				num5++;
				num2++;
			}
			if (num3 > menu.Height)
			{
				menu.Height = num3;
			}
			num += num4;
		}
		menu.Width = num;
		menu.Width += 2;
		menu.Height += 2;
		menu.Width++;
		menu.Height++;
	}

	public override void DrawMenuBar(Graphics dc, Menu menu, Rectangle rect)
	{
		if (menu.Height == 0)
		{
			CalcMenuBarSize(dc, menu, rect.Width);
		}
		bool hotkey_active = (menu as MainMenu).tracker.hotkey_active;
		HotkeyPrefix hotkeyPrefix = ((MenuAccessKeysUnderlined || hotkey_active) ? HotkeyPrefix.Show : HotkeyPrefix.Hide);
		string_format_menu_menubar_text.HotkeyPrefix = hotkeyPrefix;
		string_format_menu_text.HotkeyPrefix = hotkeyPrefix;
		rect.Height = menu.Height;
		dc.FillRectangle(SystemBrushes.Menu, rect);
		for (int i = 0; i < menu.MenuItems.Count; i++)
		{
			MenuItem menuItem = menu.MenuItems[i];
			Rectangle bounds = menuItem.bounds;
			bounds.X += rect.X;
			bounds.Y += rect.Y;
			menuItem.MenuHeight = menu.Height;
			menuItem.PerformDrawItem(new DrawItemEventArgs(dc, MenuFont, bounds, i, menuItem.Status));
		}
	}

	protected Bitmap CreateGlyphBitmap(Size size, MenuGlyph glyph, Color color)
	{
		Color color2 = ((color.R != 0 || color.G != 0 || color.B != 0) ? Color.Black : Color.White);
		Bitmap bitmap = new Bitmap(size.Width, size.Height);
		Graphics graphics = Graphics.FromImage(bitmap);
		Rectangle rectangle = new Rectangle(Point.Empty, size);
		graphics.FillRectangle(ResPool.GetSolidBrush(color2), rectangle);
		CPDrawMenuGlyph(graphics, rectangle, glyph, color, Color.Empty);
		bitmap.MakeTransparent(color2);
		graphics.Dispose();
		return bitmap;
	}

	public override void DrawMenuItem(MenuItem item, DrawItemEventArgs e)
	{
		Rectangle bounds = e.Bounds;
		if (!item.Visible)
		{
			return;
		}
		StringFormat format = ((!item.MenuBar) ? string_format_menu_text : string_format_menu_menubar_text);
		if (item.Separator)
		{
			int num = e.Bounds.Y + e.Bounds.Height / 2;
			e.Graphics.DrawLine(SystemPens.ControlDark, e.Bounds.X, num, e.Bounds.X + e.Bounds.Width, num);
			e.Graphics.DrawLine(SystemPens.ControlLight, e.Bounds.X, num + 1, e.Bounds.X + e.Bounds.Width, num + 1);
			return;
		}
		if (!item.MenuBar)
		{
			bounds.X += MenuCheckSize.Width;
		}
		if (item.BarBreak)
		{
			Rectangle bounds2 = e.Bounds;
			bounds2.Y++;
			bounds2.Width = 3;
			bounds2.Height = item.MenuHeight - 6;
			e.Graphics.DrawLine(SystemPens.ControlDark, bounds2.X, bounds2.Y, bounds2.X, bounds2.Y + bounds2.Height);
			e.Graphics.DrawLine(SystemPens.ControlLight, bounds2.X + 1, bounds2.Y, bounds2.X + 1, bounds2.Y + bounds2.Height);
		}
		Brush brush = null;
		Brush brush2 = null;
		Color color;
		Color background;
		if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && !item.MenuBar)
		{
			color = ColorHighlightText;
			background = ColorHighlight;
			brush = SystemBrushes.HighlightText;
			brush2 = SystemBrushes.Highlight;
		}
		else
		{
			color = ColorMenuText;
			background = ColorMenu;
			brush = ResPool.GetSolidBrush(ColorMenuText);
			brush2 = SystemBrushes.Menu;
		}
		if (!item.MenuBar)
		{
			e.Graphics.FillRectangle(brush2, e.Bounds);
		}
		if (item.Enabled)
		{
			e.Graphics.DrawString(item.Text, e.Font, brush, bounds, format);
			if (item.MenuBar)
			{
				Border3DStyle border3DStyle = Border3DStyle.Adjust;
				if ((item.Status & DrawItemState.HotLight) != 0)
				{
					border3DStyle = Border3DStyle.RaisedInner;
				}
				else if ((item.Status & DrawItemState.Selected) != 0)
				{
					border3DStyle = Border3DStyle.SunkenOuter;
				}
				if (border3DStyle != Border3DStyle.Adjust)
				{
					CPDrawBorder3D(e.Graphics, e.Bounds, border3DStyle, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, ColorMenu);
				}
			}
		}
		else
		{
			if ((item.Status & DrawItemState.Selected) != DrawItemState.Selected)
			{
				e.Graphics.DrawString(item.Text, e.Font, Brushes.White, new RectangleF(bounds.X + 1, bounds.Y + 1, bounds.Width, bounds.Height), format);
			}
			e.Graphics.DrawString(item.Text, e.Font, ResPool.GetSolidBrush(ColorGrayText), bounds, format);
		}
		if (!item.MenuBar && item.Shortcut != 0 && item.ShowShortcut)
		{
			string shortCutText = item.GetShortCutText();
			Rectangle rectangle = bounds;
			rectangle.X = item.XTab;
			rectangle.Width -= item.XTab;
			if (item.Enabled)
			{
				e.Graphics.DrawString(shortCutText, e.Font, brush, rectangle, string_format_menu_shortcut);
			}
			else
			{
				if ((item.Status & DrawItemState.Selected) != DrawItemState.Selected)
				{
					e.Graphics.DrawString(shortCutText, e.Font, Brushes.White, new RectangleF(rectangle.X + 1, rectangle.Y + 1, rectangle.Width, bounds.Height), string_format_menu_shortcut);
				}
				e.Graphics.DrawString(shortCutText, e.Font, ResPool.GetSolidBrush(ColorGrayText), rectangle, string_format_menu_shortcut);
			}
		}
		if (!item.MenuBar && (item.IsPopup || item.MdiList))
		{
			int width = MenuCheckSize.Width;
			int height = MenuCheckSize.Height;
			Bitmap bitmap = CreateGlyphBitmap(new Size(width, height), MenuGlyph.Arrow, color);
			if (item.Enabled)
			{
				e.Graphics.DrawImage(bitmap, e.Bounds.X + e.Bounds.Width - width, e.Bounds.Y + (e.Bounds.Height - height) / 2);
			}
			else
			{
				ControlPaint.DrawImageDisabled(e.Graphics, bitmap, e.Bounds.X + e.Bounds.Width - width, e.Bounds.Y + (e.Bounds.Height - height) / 2, background);
			}
			bitmap.Dispose();
		}
		if (!item.MenuBar && item.Checked)
		{
			Rectangle bounds3 = e.Bounds;
			int width2 = MenuCheckSize.Width;
			int height2 = MenuCheckSize.Height;
			Bitmap bitmap2 = CreateGlyphBitmap(new Size(width2, height2), (!item.RadioCheck) ? MenuGlyph.Checkmark : MenuGlyph.Bullet, color);
			e.Graphics.DrawImage(bitmap2, bounds3.X, e.Bounds.Y + (e.Bounds.Height - height2) / 2);
			bitmap2.Dispose();
		}
	}

	public override void DrawPopupMenu(Graphics dc, Menu menu, Rectangle cliparea, Rectangle rect)
	{
		dc.FillRectangle(SystemBrushes.Menu, cliparea);
		CPDrawBorder3D(dc, rect, Border3DStyle.Raised, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
		for (int i = 0; i < menu.MenuItems.Count; i++)
		{
			if (cliparea.IntersectsWith(menu.MenuItems[i].bounds))
			{
				MenuItem menuItem = menu.MenuItems[i];
				menuItem.MenuHeight = menu.Height;
				menuItem.PerformDrawItem(new DrawItemEventArgs(dc, MenuFont, menuItem.bounds, i, menuItem.Status));
			}
		}
	}

	public override void DrawMonthCalendar(Graphics dc, Rectangle clip_rectangle, MonthCalendar mc)
	{
		Rectangle clientRectangle = mc.ClientRectangle;
		Size singleMonthSize = mc.SingleMonthSize;
		Size size = (Size)(object)mc.calendar_spacing;
		Size size2 = (Size)(object)mc.date_cell_size;
		int num = 1;
		int num2 = 1;
		for (int i = 0; i < mc.CalendarDimensions.Height; i++)
		{
			if (i > 0)
			{
				num2 += singleMonthSize.Height + size.Height;
			}
			for (int j = 0; j < mc.CalendarDimensions.Width; j++)
			{
				num = ((j <= 0) ? 1 : (num + (singleMonthSize.Width + size.Width)));
				Rectangle rectangle = new Rectangle(num, num2, singleMonthSize.Width, singleMonthSize.Height);
				if (rectangle.IntersectsWith(clip_rectangle))
				{
					DrawSingleMonth(dc, clip_rectangle, rectangle, mc, i, j);
				}
			}
		}
		Rectangle rect = new Rectangle(clientRectangle.X, Math.Max(clientRectangle.Bottom - size2.Height - 3, 0), clientRectangle.Width, size2.Height + 2);
		if (mc.ShowToday && rect.IntersectsWith(clip_rectangle))
		{
			dc.FillRectangle(GetControlBackBrush(mc.BackColor), rect);
			if (mc.ShowToday)
			{
				int num3 = 5;
				if (mc.ShowTodayCircle)
				{
					Rectangle rectangle2 = new Rectangle(clientRectangle.X + 5, Math.Max(clientRectangle.Bottom - size2.Height - 2, 0), size2.Width, size2.Height);
					DrawTodayCircle(dc, rectangle2);
					num3 += size2.Width + 5;
				}
				StringFormat stringFormat = new StringFormat();
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.Alignment = StringAlignment.Near;
				dc.DrawString(layoutRectangle: new Rectangle(num3 + clientRectangle.X, Math.Max(clientRectangle.Bottom - size2.Height, 0), Math.Max(clientRectangle.Width - num3, 0), size2.Height), s: "Today: " + DateTime.Now.ToShortDateString(), font: mc.bold_font, brush: GetControlForeBrush(mc.ForeColor), format: stringFormat);
				stringFormat.Dispose();
			}
		}
		Brush brush = ((mc.owner != null) ? SystemBrushes.ControlDarkDark : GetControlBackBrush(mc.BackColor));
		for (int k = 0; k <= mc.CalendarDimensions.Width; k++)
		{
			if (k == 0 && clip_rectangle.X == clientRectangle.X)
			{
				dc.FillRectangle(brush, clientRectangle.X, clientRectangle.Y, 1, clientRectangle.Height);
				continue;
			}
			if (k == mc.CalendarDimensions.Width && clip_rectangle.Right == clientRectangle.Right)
			{
				dc.FillRectangle(brush, clientRectangle.Right - 1, clientRectangle.Y, 1, clientRectangle.Height);
				continue;
			}
			Rectangle rect2 = new Rectangle(clientRectangle.X + singleMonthSize.Width * k + size.Width * (k - 1) + 1, clientRectangle.Y, size.Width, clientRectangle.Height);
			if (k < mc.CalendarDimensions.Width && k > 0 && clip_rectangle.IntersectsWith(rect2))
			{
				dc.FillRectangle(brush, rect2);
			}
		}
		for (int l = 0; l <= mc.CalendarDimensions.Height; l++)
		{
			if (l == 0 && clip_rectangle.Y == clientRectangle.Y)
			{
				dc.FillRectangle(brush, clientRectangle.X, clientRectangle.Y, clientRectangle.Width, 1);
				continue;
			}
			if (l == mc.CalendarDimensions.Height && clip_rectangle.Bottom == clientRectangle.Bottom)
			{
				dc.FillRectangle(brush, clientRectangle.X, clientRectangle.Bottom - 1, clientRectangle.Width, 1);
				continue;
			}
			Rectangle rect3 = new Rectangle(clientRectangle.X, clientRectangle.Y + singleMonthSize.Height * l + size.Height * (l - 1) + 1, clientRectangle.Width, size.Height);
			if (l < mc.CalendarDimensions.Height && l > 0 && clip_rectangle.IntersectsWith(rect3))
			{
				dc.FillRectangle(brush, rect3);
			}
		}
		if (mc.owner == null)
		{
			return;
		}
		Rectangle clientRectangle2 = mc.ClientRectangle;
		if (clip_rectangle.Contains(mc.Location))
		{
			if (clip_rectangle.Contains(new Point(clientRectangle2.Left, clientRectangle2.Bottom)))
			{
				dc.DrawLine(SystemPens.ControlText, clientRectangle2.X, clientRectangle2.Y, clientRectangle2.X, clientRectangle2.Bottom - 1);
			}
			if (clip_rectangle.Contains(new Point(clientRectangle2.Right, clientRectangle2.Y)))
			{
				dc.DrawLine(SystemPens.ControlText, clientRectangle2.X, clientRectangle2.Y, clientRectangle2.Right - 1, clientRectangle2.Y);
			}
		}
		if (clip_rectangle.Contains(new Point(clientRectangle2.Right, clientRectangle2.Bottom)))
		{
			if (clip_rectangle.Contains(new Point(clientRectangle2.Left, clientRectangle2.Bottom)))
			{
				dc.DrawLine(SystemPens.ControlText, clientRectangle2.X, clientRectangle2.Bottom - 1, clientRectangle2.Right - 1, clientRectangle2.Bottom - 1);
			}
			if (clip_rectangle.Contains(new Point(clientRectangle2.Right, clientRectangle2.Y)))
			{
				dc.DrawLine(SystemPens.ControlText, clientRectangle2.Right - 1, clientRectangle2.Y, clientRectangle2.Right - 1, clientRectangle2.Bottom - 1);
			}
		}
	}

	private void DrawSingleMonth(Graphics dc, Rectangle clip_rectangle, Rectangle rectangle, MonthCalendar mc, int row, int col)
	{
		Size title_size = (Size)(object)mc.title_size;
		Size size = (Size)(object)mc.date_cell_size;
		DateTime dateTime = (DateTime)(object)mc.current_month;
		DateTime dateTime2 = new DateTime(2006, 10, 1);
		DateTime month = dateTime.AddMonths(row * mc.CalendarDimensions.Width + col);
		Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y, title_size.Width, title_size.Height);
		if (rectangle2.IntersectsWith(clip_rectangle))
		{
			dc.FillRectangle(ResPool.GetSolidBrush(mc.TitleBackColor), rectangle2);
			string s = month.ToString("MMMM yyyy");
			dc.DrawString(s, mc.bold_font, ResPool.GetSolidBrush(mc.TitleForeColor), rectangle2, mc.centered_format);
			if (mc.ShowYearUpDown)
			{
				mc.GetYearNameRectangles(rectangle2, row * mc.CalendarDimensions.Width + col, out var year_rect, out var up_rect, out var down_rect);
				dc.FillRectangle(ResPool.GetSolidBrush(SystemColors.Control), year_rect);
				dc.DrawString(month.ToString("yyyy"), mc.bold_font, ResPool.GetSolidBrush(Color.Black), year_rect, mc.centered_format);
				ButtonState state = (mc.IsYearGoingUp ? ButtonState.Pushed : ButtonState.Normal);
				ButtonState state2 = (mc.IsYearGoingDown ? ButtonState.Pushed : ButtonState.Normal);
				ControlPaint.DrawScrollButton(dc, up_rect, ScrollButton.Min, state);
				ControlPaint.DrawScrollButton(dc, down_rect, ScrollButton.Down, state2);
			}
			if (row == 0 && col == 0)
			{
				DrawMonthCalendarButton(dc, rectangle, mc, title_size, mc.button_x_offset, (Size)(object)mc.button_size, is_previous: true);
			}
			if (row == 0 && col == mc.CalendarDimensions.Width - 1)
			{
				DrawMonthCalendarButton(dc, rectangle, mc, title_size, mc.button_x_offset, (Size)(object)mc.button_size, is_previous: false);
			}
		}
		int num = (mc.ShowWeekNumbers ? 1 : 0);
		Rectangle rect = new Rectangle(rectangle.X, rectangle.Y + title_size.Height, (7 + num) * size.Width, size.Height);
		if (rect.IntersectsWith(clip_rectangle))
		{
			dc.FillRectangle(GetControlBackBrush(mc.BackColor), rect);
			DayOfWeek dayOfWeek = mc.GetDayOfWeek(mc.FirstDayOfWeek);
			for (int i = 0; i < 7; i++)
			{
				int num2 = (int)(i - dayOfWeek);
				if (num2 < 0)
				{
					num2 = 7 + num2;
				}
				dc.DrawString(layoutRectangle: new Rectangle(rect.X + (i + num) * size.Width, rect.Y, size.Width, size.Height), s: dateTime2.AddDays((double)(i + dayOfWeek)).ToString("ddd"), font: mc.Font, brush: ResPool.GetSolidBrush(mc.TitleBackColor), format: mc.centered_format);
			}
			int num3 = Math.Max(title_size.Height + size.Height - 1, 0);
			dc.DrawLine(ResPool.GetPen(mc.ForeColor), rectangle.X + num * size.Width + mc.divider_line_offset, rectangle.Y + num3, rectangle.Right - mc.divider_line_offset, rectangle.Y + num3);
		}
		Rectangle rectangle3 = new Rectangle(rectangle.X, rectangle.Y + title_size.Height + size.Height, size.Width, size.Height);
		int num4 = 0;
		bool flag = false;
		DateTime date = mc.GetFirstDateInMonthGrid(new DateTime(month.Year, month.Month, 1));
		for (int j = 0; j < 6; j++)
		{
			Rectangle rect2 = new Rectangle(rectangle.X, rectangle.Y + title_size.Height + size.Height * (j + 1), size.Width * 7, size.Height);
			if (mc.ShowWeekNumbers)
			{
				rect2.Width += size.Width;
			}
			bool flag2 = rect2.IntersectsWith(clip_rectangle);
			if (flag2)
			{
				dc.FillRectangle(GetControlBackBrush(mc.BackColor), rect2);
			}
			if (mc.IsValidWeekToDraw(month, date, row, col))
			{
				num4 = j;
			}
			if (mc.ShowWeekNumbers && num4 == j)
			{
				if (!flag)
				{
					flag = flag2;
				}
				int weekOfYear = mc.GetWeekOfYear(date);
				if (flag2)
				{
					dc.DrawString(weekOfYear.ToString(), mc.Font, ResPool.GetSolidBrush(mc.TitleBackColor), rectangle3, mc.centered_format);
				}
				rectangle3.Offset(size.Width, 0);
			}
			if (num4 != j)
			{
				continue;
			}
			for (int k = 0; k < 7; k++)
			{
				if (flag2)
				{
					DrawMonthCalendarDate(dc, rectangle3, mc, date, month, row, col);
				}
				date = date.AddDays(1.0);
				rectangle3.Offset(size.Width, 0);
			}
			int num5 = ((!mc.ShowWeekNumbers) ? (-7) : (-8));
			rectangle3.Offset(num5 * size.Width, size.Height);
		}
		num4++;
		if (flag)
		{
			num = 1;
			dc.DrawLine(ResPool.GetPen(mc.ForeColor), rectangle.X + size.Width - 1, rectangle.Y + title_size.Height + size.Height + mc.divider_line_offset, rectangle.X + size.Width - 1, rectangle.Y + title_size.Height + size.Height + num4 * size.Height - mc.divider_line_offset);
		}
	}

	private void DrawMonthCalendarButton(Graphics dc, Rectangle rectangle, MonthCalendar mc, Size title_size, int x_offset, Size button_size, bool is_previous)
	{
		bool flag = false;
		PointF[] array = new PointF[3];
		Rectangle rectangle2;
		if (is_previous)
		{
			flag = mc.is_previous_clicked;
			rectangle2 = new Rectangle(rectangle.X + 1 + x_offset, rectangle.Y + 1 + (title_size.Height - button_size.Height) / 2, Math.Max(button_size.Width - 1, 0), Math.Max(button_size.Height - 1, 0));
			PointF pointF = new PointF((float)rectangle2.X + (float)(rectangle2.Width + 4) / 2f, rectangle.Y + (rectangle2.Height + 7) / 2 + 1);
			if (flag)
			{
				pointF.X += 1f;
				pointF.Y += 1f;
			}
			array[0].X = pointF.X;
			array[0].Y = pointF.Y - 3.5f + 0.5f;
			array[1].X = pointF.X;
			array[1].Y = pointF.Y + 3.5f + 0.5f;
			array[2].X = pointF.X - 4f;
			array[2].Y = pointF.Y + 0.5f;
		}
		else
		{
			flag = mc.is_next_clicked;
			rectangle2 = new Rectangle(rectangle.Right - 1 - x_offset - button_size.Width, rectangle.Y + 1 + (title_size.Height - button_size.Height) / 2, Math.Max(button_size.Width - 1, 0), Math.Max(button_size.Height - 1, 0));
			PointF pointF = new PointF((float)rectangle2.X + (float)(rectangle2.Width + 4) / 2f, rectangle.Y + (rectangle2.Height + 7) / 2 + 1);
			if (flag)
			{
				pointF.X += 1f;
				pointF.Y += 1f;
			}
			array[0].X = pointF.X - 4f;
			array[0].Y = pointF.Y - 3.5f + 0.5f;
			array[1].X = pointF.X - 4f;
			array[1].Y = pointF.Y + 3.5f + 0.5f;
			array[2].X = pointF.X;
			array[2].Y = pointF.Y + 0.5f;
		}
		dc.FillRectangle(SystemBrushes.Control, rectangle2);
		if (flag)
		{
			dc.DrawRectangle(SystemPens.ControlDark, rectangle2);
		}
		else
		{
			CPDrawBorder3D(dc, rectangle2, Border3DStyle.Raised, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
		}
		dc.FillPolygon(SystemBrushes.ControlText, array);
	}

	private void DrawMonthCalendarDate(Graphics dc, Rectangle rectangle, MonthCalendar mc, DateTime date, DateTime month, int row, int col)
	{
		Color foreColor = mc.ForeColor;
		Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y, Math.Max(rectangle.Width - 1, 0), Math.Max(rectangle.Height - 1, 0));
		if (date.Year != month.Year || date.Month != month.Month)
		{
			DateTime dateTime = month.AddMonths(-1);
			if (dateTime.Year == date.Year && dateTime.Month == date.Month && row == 0 && col == 0)
			{
				foreColor = mc.TrailingForeColor;
			}
			else
			{
				dateTime = month.AddMonths(1);
				if (dateTime.Year != date.Year || dateTime.Month != date.Month || row != mc.CalendarDimensions.Height - 1 || col != mc.CalendarDimensions.Width - 1)
				{
					return;
				}
				foreColor = mc.TrailingForeColor;
			}
		}
		else
		{
			foreColor = mc.ForeColor;
		}
		if (date == mc.SelectionStart.Date && date == mc.SelectionEnd.Date)
		{
			foreColor = mc.BackColor;
			Rectangle rect = Rectangle.Inflate(rectangle, -1, -1);
			dc.FillPie(ResPool.GetSolidBrush(mc.TitleBackColor), rect, 0f, 360f);
		}
		else if (date == mc.SelectionStart.Date)
		{
			foreColor = mc.BackColor;
			Rectangle rect2 = Rectangle.Inflate(rectangle, -1, -1);
			dc.FillPie(ResPool.GetSolidBrush(mc.TitleBackColor), rect2, 90f, 180f);
			if (date < mc.SelectionEnd.Date)
			{
				rect2.X = (int)Math.Floor((double)(rectangle.X + rectangle.Width / 2));
				rect2.Width = Math.Max(rectangle.Right - rect2.X, 0);
				dc.FillRectangle(ResPool.GetSolidBrush(mc.TitleBackColor), rect2);
			}
		}
		else if (date == mc.SelectionEnd.Date)
		{
			foreColor = mc.BackColor;
			Rectangle rect3 = Rectangle.Inflate(rectangle, -1, -1);
			dc.FillPie(ResPool.GetSolidBrush(mc.TitleBackColor), rect3, 270f, 180f);
			if (date > mc.SelectionStart.Date)
			{
				rect3.X = rectangle.X;
				rect3.Width = rectangle.Width - rectangle.Width / 2;
				dc.FillRectangle(ResPool.GetSolidBrush(mc.TitleBackColor), rect3);
			}
		}
		else if (date > mc.SelectionStart.Date && date < mc.SelectionEnd.Date)
		{
			foreColor = mc.BackColor;
			Rectangle rect4 = Rectangle.Inflate(rectangle, 0, -1);
			dc.FillRectangle(ResPool.GetSolidBrush(mc.TitleBackColor), rect4);
		}
		Font font = ((!mc.IsBoldedDate(date)) ? mc.Font : mc.bold_font);
		dc.DrawString(date.Day.ToString(), font, ResPool.GetSolidBrush(foreColor), rectangle, mc.centered_format);
		if (mc.ShowTodayCircle && date == DateTime.Now.Date)
		{
			DrawTodayCircle(dc, rectangle2);
		}
		if (mc.is_date_clicked && mc.clicked_date == date)
		{
			Pen dashPen = ResPool.GetDashPen(Color.Black, DashStyle.Dot);
			dc.DrawRectangle(dashPen, rectangle2);
		}
	}

	private void DrawTodayCircle(Graphics dc, Rectangle rectangle)
	{
		Color color = Color.FromArgb(248, 0, 0);
		Rectangle rect = new Rectangle(rectangle.X + 1, rectangle.Y + 4, Math.Max(rectangle.Width - 2, 0), Math.Max(rectangle.Height - 5, 0));
		Rectangle rect2 = new Rectangle(rectangle.X + 1, rectangle.Y + 1, Math.Max(rectangle.Width - 2, 0), Math.Max(rectangle.Height - 2, 0));
		Point[] array = new Point[3]
		{
			new Point(rect.X, rect2.Y + rect2.Height / 12),
			new Point(rect.X + rect.Width / 9, rect2.Y),
			new Point(rect.X + rect.Width / 2 + 1, rect2.Y)
		};
		Pen sizedPen = ResPool.GetSizedPen(color, 2);
		dc.DrawArc(sizedPen, rect, 90f, 180f);
		dc.DrawArc(sizedPen, rect2, 270f, 180f);
		dc.DrawCurve(sizedPen, array);
		dc.DrawLine(ResPool.GetPen(color), array[2], new Point(array[2].X, rect.Y));
	}

	public override void DrawPictureBox(Graphics dc, Rectangle clip, PictureBox pb)
	{
		Rectangle rectangle = pb.ClientRectangle;
		rectangle = new Rectangle(rectangle.Left + pb.Padding.Left, rectangle.Top + pb.Padding.Top, rectangle.Width - pb.Padding.Horizontal, rectangle.Height - pb.Padding.Vertical);
		if (pb.Image != null)
		{
			switch (pb.SizeMode)
			{
			case PictureBoxSizeMode.StretchImage:
				dc.DrawImage(pb.Image, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
				break;
			case PictureBoxSizeMode.CenterImage:
				dc.DrawImage(pb.Image, rectangle.Width / 2 - pb.Image.Width / 2, rectangle.Height / 2 - pb.Image.Height / 2);
				break;
			case PictureBoxSizeMode.Zoom:
			{
				Size size = ((!((float)pb.Image.Width / (float)pb.Image.Height >= (float)rectangle.Width / (float)rectangle.Height)) ? new Size(pb.Image.Width * rectangle.Height / pb.Image.Height, rectangle.Height) : new Size(rectangle.Width, pb.Image.Height * rectangle.Width / pb.Image.Width));
				dc.DrawImage(pb.Image, rectangle.Width / 2 - size.Width / 2, rectangle.Height / 2 - size.Height / 2, size.Width, size.Height);
				break;
			}
			default:
				dc.DrawImage(pb.Image, rectangle.Left, rectangle.Top, pb.Image.Width, pb.Image.Height);
				break;
			}
		}
	}

	public override Size PrintPreviewControlGetPageSize(PrintPreviewControl preview)
	{
		int printPreviewControlPadding = PrintPreviewControlPadding;
		PreviewPageInfo[] page_infos = preview.page_infos;
		int num4;
		int num5;
		if (preview.AutoZoom)
		{
			int num = preview.ClientRectangle.Height - preview.Rows * printPreviewControlPadding - 2 * printPreviewControlPadding;
			int num2 = preview.ClientRectangle.Width - (preview.Columns - 1) * printPreviewControlPadding - 2 * printPreviewControlPadding;
			float num3 = (float)page_infos[0].Image.Width / (float)page_infos[0].Image.Height;
			num4 = num2 / preview.Columns;
			num5 = (int)((float)num4 / num3);
			if (num5 * (preview.Rows + 1) > num)
			{
				num5 = num / (preview.Rows + 1);
				num4 = (int)((float)num5 * num3);
			}
		}
		else
		{
			num4 = (int)((double)page_infos[0].Image.Width * preview.Zoom);
			num5 = (int)((double)page_infos[0].Image.Height * preview.Zoom);
		}
		return new Size(num4, num5);
	}

	public override void PrintPreviewControlPaint(PaintEventArgs pe, PrintPreviewControl preview, Size page_size)
	{
		int num = 8;
		PreviewPageInfo[] page_infos = preview.page_infos;
		if (page_infos == null)
		{
			return;
		}
		int num2 = page_size.Width * preview.Columns + num * (preview.Columns - 1) + 2 * num;
		int num3 = page_size.Height * (preview.Rows + 1) + num * preview.Rows + 2 * num;
		Rectangle viewPort = preview.ViewPort;
		pe.Graphics.Clip = new Region(viewPort);
		int num4 = viewPort.Width / 2 - num2 / 2;
		if (num4 < 0)
		{
			num4 = 0;
		}
		int num5 = viewPort.Height / 2 - num3 / 2;
		if (num5 < 0)
		{
			num5 = 0;
		}
		int num6 = num5 + num - preview.vbar_value;
		if (preview.StartPage <= 0)
		{
			return;
		}
		int num7 = preview.StartPage - 1;
		for (int i = 0; i < preview.Rows + 1; i++)
		{
			int num8 = num4 + num - preview.hbar_value;
			for (int j = 0; j < preview.Columns; j++)
			{
				if (num7 < page_infos.Length)
				{
					Image image = preview.image_cache[num7];
					if (image == null)
					{
						image = page_infos[num7].Image;
					}
					Rectangle destRect = new Rectangle(new Point(num8, num6), page_size);
					pe.Graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
					num8 += num + page_size.Width;
					num7++;
				}
			}
			num6 += num + page_size.Height;
		}
	}

	public override void DrawProgressBar(Graphics dc, Rectangle clip_rect, ProgressBar ctrl)
	{
		Rectangle client_area = ctrl.client_area;
		CPDrawBorder3D(dc, ctrl.ClientRectangle, Border3DStyle.SunkenOuter, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, ColorControl);
		int num = 0;
		int num2 = int.MaxValue;
		int x = client_area.X;
		switch ((int)ctrl.Style)
		{
		case 1:
		{
			int width = (int)((double)client_area.Width * ((double)(ctrl.Value - ctrl.Minimum) / (double)Math.Max(ctrl.Maximum - ctrl.Minimum, 1)));
			dc.FillRectangle(ResPool.GetSolidBrush(ctrl.ForeColor), new Rectangle(client_area.X, client_area.Y, width, client_area.Height));
			return;
		}
		case 2:
			if (XplatUI.ThemesEnabled)
			{
				int num3 = (int)(DateTime.Now - ctrl.start).TotalMilliseconds;
				double num4 = (double)num3 % (double)ctrl.MarqueeAnimationSpeed / (double)ctrl.MarqueeAnimationSpeed;
				num2 = 5;
				x = client_area.X + (int)((double)client_area.Width * num4);
			}
			break;
		}
		int num5 = 2;
		int num6 = 0;
		int val = ProgressBarGetChunkSize(client_area.Height);
		val = Math.Max(val, 0);
		int num7 = (int)((double)(ctrl.Value - ctrl.Minimum) * (double)client_area.Width / (double)Math.Max(ctrl.Maximum - ctrl.Minimum, 1));
		int num8 = val + num5;
		Rectangle rect = new Rectangle(x, client_area.Y, val, client_area.Height);
		while (true)
		{
			if (num2 != int.MaxValue)
			{
				if (num6 >= num2)
				{
					break;
				}
				if (rect.X > client_area.Width)
				{
					rect.X -= client_area.Width;
				}
			}
			else if (rect.X - client_area.X >= num7)
			{
				break;
			}
			if (clip_rect.IntersectsWith(rect))
			{
				dc.FillRectangle(ResPool.GetSolidBrush(ctrl.ForeColor), rect);
			}
			rect.X += num8;
			num6++;
		}
	}

	public static int ProgressBarGetChunkSize()
	{
		return ProgressBarGetChunkSize(23);
	}

	private static int ProgressBarGetChunkSize(int progressBarClientAreaHeight)
	{
		return progressBarClientAreaHeight * 2 / 3;
	}

	public override void DrawRadioButton(Graphics dc, Rectangle clip_rectangle, RadioButton radio_button)
	{
		int num = 13;
		int num2 = 4;
		Rectangle clientRectangle = radio_button.ClientRectangle;
		Rectangle text_rectangle = clientRectangle;
		Rectangle radiobutton_rectangle = new Rectangle(text_rectangle.X, text_rectangle.Y, num, num);
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Near;
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
		if (radio_button.appearance != Appearance.Button)
		{
			switch (radio_button.radiobutton_alignment)
			{
			case System.Drawing.ContentAlignment.BottomCenter:
				radiobutton_rectangle.X = (clientRectangle.Right - clientRectangle.Left) / 2 - num / 2;
				radiobutton_rectangle.Y = clientRectangle.Bottom - num;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width;
				text_rectangle.Height = clientRectangle.Height - num - num2;
				break;
			case System.Drawing.ContentAlignment.BottomLeft:
				radiobutton_rectangle.X = clientRectangle.Left;
				radiobutton_rectangle.Y = clientRectangle.Bottom - num;
				text_rectangle.X = clientRectangle.X + num + num2;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.BottomRight:
				radiobutton_rectangle.X = clientRectangle.Right - num;
				radiobutton_rectangle.Y = clientRectangle.Bottom - num;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.MiddleCenter:
				radiobutton_rectangle.X = (clientRectangle.Right - clientRectangle.Left) / 2 - num / 2;
				radiobutton_rectangle.Y = (clientRectangle.Bottom - clientRectangle.Top) / 2 - num / 2;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width;
				break;
			default:
				radiobutton_rectangle.X = clientRectangle.Left;
				radiobutton_rectangle.Y = (clientRectangle.Bottom - clientRectangle.Top) / 2 - num / 2;
				text_rectangle.X = clientRectangle.X + num + num2;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.MiddleRight:
				radiobutton_rectangle.X = clientRectangle.Right - num;
				radiobutton_rectangle.Y = (clientRectangle.Bottom - clientRectangle.Top) / 2 - num / 2;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.TopCenter:
				radiobutton_rectangle.X = (clientRectangle.Right - clientRectangle.Left) / 2 - num / 2;
				radiobutton_rectangle.Y = clientRectangle.Top;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Y = num + num2;
				text_rectangle.Width = clientRectangle.Width;
				text_rectangle.Height = clientRectangle.Height - num - num2;
				break;
			case System.Drawing.ContentAlignment.TopLeft:
				radiobutton_rectangle.X = clientRectangle.Left;
				radiobutton_rectangle.Y = clientRectangle.Top;
				text_rectangle.X = clientRectangle.X + num + num2;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			case System.Drawing.ContentAlignment.TopRight:
				radiobutton_rectangle.X = clientRectangle.Right - num;
				radiobutton_rectangle.Y = clientRectangle.Top;
				text_rectangle.X = clientRectangle.X;
				text_rectangle.Width = clientRectangle.Width - num - num2;
				break;
			}
		}
		else
		{
			text_rectangle.X = clientRectangle.X;
			text_rectangle.Width = clientRectangle.Width;
		}
		switch (radio_button.text_alignment)
		{
		case System.Drawing.ContentAlignment.TopLeft:
		case System.Drawing.ContentAlignment.MiddleLeft:
		case System.Drawing.ContentAlignment.BottomLeft:
			stringFormat.Alignment = StringAlignment.Near;
			break;
		case System.Drawing.ContentAlignment.TopCenter:
		case System.Drawing.ContentAlignment.MiddleCenter:
		case System.Drawing.ContentAlignment.BottomCenter:
			stringFormat.Alignment = StringAlignment.Center;
			break;
		case System.Drawing.ContentAlignment.TopRight:
		case System.Drawing.ContentAlignment.MiddleRight:
		case System.Drawing.ContentAlignment.BottomRight:
			stringFormat.Alignment = StringAlignment.Far;
			break;
		}
		switch (radio_button.text_alignment)
		{
		case System.Drawing.ContentAlignment.TopLeft:
		case System.Drawing.ContentAlignment.TopCenter:
		case System.Drawing.ContentAlignment.TopRight:
			stringFormat.LineAlignment = StringAlignment.Near;
			break;
		case System.Drawing.ContentAlignment.BottomLeft:
		case System.Drawing.ContentAlignment.BottomCenter:
		case System.Drawing.ContentAlignment.BottomRight:
			stringFormat.LineAlignment = StringAlignment.Far;
			break;
		case System.Drawing.ContentAlignment.MiddleLeft:
		case System.Drawing.ContentAlignment.MiddleCenter:
		case System.Drawing.ContentAlignment.MiddleRight:
			stringFormat.LineAlignment = StringAlignment.Center;
			break;
		}
		ButtonState buttonState = ButtonState.Normal;
		if (radio_button.FlatStyle == FlatStyle.Flat)
		{
			buttonState |= ButtonState.Flat;
		}
		if (radio_button.Checked)
		{
			buttonState |= ButtonState.Checked;
		}
		if (!radio_button.Enabled)
		{
			buttonState |= ButtonState.Inactive;
		}
		RadioButton_DrawButton(radio_button, dc, buttonState, radiobutton_rectangle);
		if (radio_button.image != null || radio_button.image_list != null)
		{
			ButtonBase_DrawImage(radio_button, dc);
		}
		RadioButton_DrawText(radio_button, text_rectangle, dc, stringFormat);
		if (radio_button.Focused && radio_button.Enabled && radio_button.appearance != Appearance.Button && radio_button.Text != string.Empty && radio_button.ShowFocusCues)
		{
			SizeF sizeF = dc.MeasureString(radio_button.Text, radio_button.Font);
			Rectangle empty = Rectangle.Empty;
			empty.X = text_rectangle.X;
			empty.Y = (int)(((float)text_rectangle.Height - sizeF.Height) / 2f);
			empty.Size = sizeF.ToSize();
			RadioButton_DrawFocus(radio_button, dc, empty);
		}
		stringFormat.Dispose();
	}

	protected virtual void RadioButton_DrawButton(RadioButton radio_button, Graphics dc, ButtonState state, Rectangle radiobutton_rectangle)
	{
		dc.FillRectangle(GetControlBackBrush(radio_button.BackColor), radio_button.ClientRectangle);
		if (radio_button.appearance == Appearance.Button)
		{
			ButtonBase_DrawButton(radio_button, dc);
			if (radio_button.Focused && radio_button.Enabled)
			{
				ButtonBase_DrawFocus(radio_button, dc);
			}
		}
		else if (radio_button.FlatStyle == FlatStyle.Flat || radio_button.FlatStyle == FlatStyle.Popup)
		{
			DrawFlatStyleRadioButton(dc, radiobutton_rectangle, radio_button);
		}
		else
		{
			CPDrawRadioButton(dc, radiobutton_rectangle, state);
		}
	}

	protected virtual void RadioButton_DrawText(RadioButton radio_button, Rectangle text_rectangle, Graphics dc, StringFormat text_format)
	{
		DrawCheckBox_and_RadioButtonText(radio_button, text_rectangle, dc, text_format, radio_button.Appearance, radio_button.Checked);
	}

	protected virtual void RadioButton_DrawFocus(RadioButton radio_button, Graphics dc, Rectangle text_rectangle)
	{
		DrawInnerFocusRectangle(dc, text_rectangle, radio_button.BackColor);
	}

	protected virtual void DrawFlatStyleRadioButton(Graphics graphics, Rectangle rectangle, RadioButton radio_button)
	{
		if (radio_button.Enabled)
		{
			if (radio_button.FlatStyle == FlatStyle.Flat)
			{
				graphics.DrawArc(SystemPens.ControlDarkDark, rectangle, 0f, 359f);
				if ((radio_button.is_entered || radio_button.Capture) && !radio_button.is_pressed)
				{
					graphics.FillPie(SystemBrushes.ControlLight, rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2, 0, 359);
				}
				else
				{
					graphics.FillPie(SystemBrushes.ControlLightLight, rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2, 0, 359);
				}
			}
			else
			{
				graphics.FillPie(SystemBrushes.ControlLightLight, rectangle, 0f, 359f);
				if (radio_button.is_entered || radio_button.Capture)
				{
					graphics.DrawArc(SystemPens.ControlLight, rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2, 0, 359);
					graphics.DrawArc(SystemPens.ControlDark, rectangle, 135f, 180f);
					graphics.DrawArc(SystemPens.ControlLightLight, rectangle, 315f, 180f);
				}
				else
				{
					graphics.DrawArc(SystemPens.ControlDark, rectangle, 0f, 359f);
				}
			}
		}
		else
		{
			graphics.FillPie(SystemBrushes.Control, rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2, 0, 359);
			graphics.DrawArc(SystemPens.ControlDark, rectangle, 0f, 359f);
		}
		if (radio_button.Checked)
		{
			int num = Math.Max(1, Math.Min(rectangle.Width, rectangle.Height) / 3);
			Pen pen = SystemPens.ControlDarkDark;
			Brush brush = SystemBrushes.ControlDarkDark;
			if (!radio_button.Enabled || (radio_button.FlatStyle == FlatStyle.Popup && radio_button.is_pressed))
			{
				pen = SystemPens.ControlDark;
				brush = SystemBrushes.ControlDark;
			}
			if (rectangle.Height > 13)
			{
				graphics.FillPie(brush, rectangle.X + num, rectangle.Y + num, rectangle.Width - num * 2, rectangle.Height - num * 2, 0, 359);
				return;
			}
			int num2 = rectangle.Width / 2 + rectangle.X;
			int num3 = rectangle.Height / 2 + rectangle.Y;
			graphics.DrawLine(pen, num2 - 1, num3, num2 + 2, num3);
			graphics.DrawLine(pen, num2 - 1, num3 + 1, num2 + 2, num3 + 1);
			graphics.DrawLine(pen, num2, num3 - 1, num2, num3 + 2);
			graphics.DrawLine(pen, num2 + 1, num3 - 1, num2 + 1, num3 + 2);
		}
	}

	public override void DrawRadioButton(Graphics g, RadioButton rb, Rectangle glyphArea, Rectangle textBounds, Rectangle imageBounds, Rectangle clipRectangle)
	{
		if (rb.FlatStyle == FlatStyle.Flat || rb.FlatStyle == FlatStyle.Popup)
		{
			glyphArea.Height -= 2;
			glyphArea.Width -= 2;
		}
		DrawRadioButtonGlyph(g, rb, glyphArea);
		if (imageBounds.Size != Size.Empty)
		{
			DrawRadioButtonImage(g, rb, imageBounds);
		}
		if (rb.Focused && rb.Enabled && rb.ShowFocusCues && textBounds.Size != Size.Empty)
		{
			DrawRadioButtonFocus(g, rb, textBounds);
		}
		if (textBounds != Rectangle.Empty)
		{
			DrawRadioButtonText(g, rb, textBounds);
		}
	}

	public virtual void DrawRadioButtonGlyph(Graphics g, RadioButton rb, Rectangle glyphArea)
	{
		if (rb.Pressed)
		{
			ThemeElements.CurrentTheme.RadioButtonPainter.PaintRadioButton(g, glyphArea, rb.BackColor, rb.ForeColor, ElementState.Pressed, rb.FlatStyle, rb.Checked);
		}
		else if (rb.InternalSelected)
		{
			ThemeElements.CurrentTheme.RadioButtonPainter.PaintRadioButton(g, glyphArea, rb.BackColor, rb.ForeColor, ElementState.Normal, rb.FlatStyle, rb.Checked);
		}
		else if (rb.Entered)
		{
			ThemeElements.CurrentTheme.RadioButtonPainter.PaintRadioButton(g, glyphArea, rb.BackColor, rb.ForeColor, ElementState.Hot, rb.FlatStyle, rb.Checked);
		}
		else if (!rb.Enabled)
		{
			ThemeElements.CurrentTheme.RadioButtonPainter.PaintRadioButton(g, glyphArea, rb.BackColor, rb.ForeColor, ElementState.Disabled, rb.FlatStyle, rb.Checked);
		}
		else
		{
			ThemeElements.CurrentTheme.RadioButtonPainter.PaintRadioButton(g, glyphArea, rb.BackColor, rb.ForeColor, ElementState.Normal, rb.FlatStyle, rb.Checked);
		}
	}

	public virtual void DrawRadioButtonFocus(Graphics g, RadioButton rb, Rectangle focusArea)
	{
		ControlPaint.DrawFocusRectangle(g, focusArea);
	}

	public virtual void DrawRadioButtonImage(Graphics g, RadioButton rb, Rectangle imageBounds)
	{
		if (rb.Enabled)
		{
			g.DrawImage(rb.Image, imageBounds);
		}
		else
		{
			CPDrawImageDisabled(g, rb.Image, imageBounds.Left, imageBounds.Top, ColorControl);
		}
	}

	public virtual void DrawRadioButtonText(Graphics g, RadioButton rb, Rectangle textBounds)
	{
		if (rb.Enabled)
		{
			TextRenderer.DrawTextInternal(g, rb.Text, rb.Font, textBounds, rb.ForeColor, rb.TextFormatFlags, rb.UseCompatibleTextRendering);
		}
		else
		{
			DrawStringDisabled20(g, rb.Text, rb.Font, textBounds, rb.BackColor, rb.TextFormatFlags, rb.UseCompatibleTextRendering);
		}
	}

	public override Size CalculateRadioButtonAutoSize(RadioButton rb)
	{
		Size empty = Size.Empty;
		Size size = TextRenderer.MeasureTextInternal(rb.Text, rb.Font, rb.UseCompatibleTextRendering);
		Size size2 = ((rb.Image != null) ? rb.Image.Size : Size.Empty);
		if (rb.Text.Length != 0)
		{
			size.Height += 4;
			size.Width += 4;
		}
		switch (rb.TextImageRelation)
		{
		case TextImageRelation.Overlay:
			empty.Height = Math.Max((rb.Text.Length != 0) ? size.Height : 0, size2.Height);
			empty.Width = Math.Max(size.Width, size2.Width);
			break;
		case TextImageRelation.ImageAboveText:
		case TextImageRelation.TextAboveImage:
			empty.Height = size.Height + size2.Height;
			empty.Width = Math.Max(size.Width, size2.Width);
			break;
		case TextImageRelation.ImageBeforeText:
		case TextImageRelation.TextBeforeImage:
			empty.Height = Math.Max(size.Height, size2.Height);
			empty.Width = size.Width + size2.Width;
			break;
		}
		empty.Height += rb.Padding.Vertical;
		empty.Width += rb.Padding.Horizontal + 15;
		if (empty.Height == rb.Padding.Vertical)
		{
			empty.Height += 14;
		}
		return empty;
	}

	public override void CalculateRadioButtonTextAndImageLayout(ButtonBase b, Point offset, out Rectangle glyphArea, out Rectangle textRectangle, out Rectangle imageRectangle)
	{
		CalculateCheckBoxTextAndImageLayout(b, offset, out glyphArea, out textRectangle, out imageRectangle);
	}

	public override void DrawScrollBar(Graphics dc, Rectangle clip, ScrollBar bar)
	{
		int scrollbutton_width = bar.scrollbutton_width;
		int scrollbutton_height = bar.scrollbutton_height;
		Rectangle thumbPos = bar.ThumbPos;
		Rectangle rectangle4;
		if (bar.vert)
		{
			Rectangle rectangle2 = (bar.FirstArrowArea = new Rectangle(0, 0, bar.Width, scrollbutton_height));
			rectangle4 = (bar.SecondArrowArea = new Rectangle(0, bar.ClientRectangle.Height - scrollbutton_height, bar.Width, scrollbutton_height));
			thumbPos.Width = bar.Width;
			bar.ThumbPos = thumbPos;
			Brush brush = ((bar.thumb_moving != ScrollBar.ThumbMoving.Backwards) ? ResPool.GetHatchBrush(HatchStyle.Percent50, ColorScrollBar, Color.White) : ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(255, 63, 63, 63), Color.Black));
			Rectangle rect = new Rectangle(0, 0, bar.ClientRectangle.Width, bar.ThumbPos.Bottom);
			if (clip.IntersectsWith(rect))
			{
				dc.FillRectangle(brush, rect);
			}
			brush = ((bar.thumb_moving != ScrollBar.ThumbMoving.Forward) ? ResPool.GetHatchBrush(HatchStyle.Percent50, ColorScrollBar, Color.White) : ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(255, 63, 63, 63), Color.Black));
			Rectangle rect2 = new Rectangle(0, bar.ThumbPos.Bottom, bar.ClientRectangle.Width, bar.ClientRectangle.Height - bar.ThumbPos.Bottom);
			if (clip.IntersectsWith(rect2))
			{
				dc.FillRectangle(brush, rect2);
			}
			if (clip.IntersectsWith(rectangle2))
			{
				CPDrawScrollButton(dc, rectangle2, ScrollButton.Min, bar.firstbutton_state);
			}
			if (clip.IntersectsWith(rectangle4))
			{
				CPDrawScrollButton(dc, rectangle4, ScrollButton.Down, bar.secondbutton_state);
			}
		}
		else
		{
			Rectangle rectangle2 = (bar.FirstArrowArea = new Rectangle(0, 0, scrollbutton_width, bar.Height));
			rectangle4 = (bar.SecondArrowArea = new Rectangle(bar.ClientRectangle.Width - scrollbutton_width, 0, scrollbutton_width, bar.Height));
			thumbPos.Height = bar.Height;
			bar.ThumbPos = thumbPos;
			Brush brush2 = ((bar.thumb_moving != ScrollBar.ThumbMoving.Backwards) ? ResPool.GetHatchBrush(HatchStyle.Percent50, ColorScrollBar, Color.White) : ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(255, 63, 63, 63), Color.Black));
			Rectangle rect3 = new Rectangle(0, 0, bar.ThumbPos.Right, bar.ClientRectangle.Height);
			if (clip.IntersectsWith(rect3))
			{
				dc.FillRectangle(brush2, rect3);
			}
			brush2 = ((bar.thumb_moving != ScrollBar.ThumbMoving.Forward) ? ResPool.GetHatchBrush(HatchStyle.Percent50, ColorScrollBar, Color.White) : ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(255, 63, 63, 63), Color.Black));
			Rectangle rect4 = new Rectangle(bar.ThumbPos.Right, 0, bar.ClientRectangle.Width - bar.ThumbPos.Right, bar.ClientRectangle.Height);
			if (clip.IntersectsWith(rect4))
			{
				dc.FillRectangle(brush2, rect4);
			}
			if (clip.IntersectsWith(rectangle2))
			{
				CPDrawScrollButton(dc, rectangle2, ScrollButton.Left, bar.firstbutton_state);
			}
			if (clip.IntersectsWith(rectangle4))
			{
				CPDrawScrollButton(dc, rectangle4, ScrollButton.Right, bar.secondbutton_state);
			}
		}
		ScrollBar_DrawThumb(bar, thumbPos, clip, dc);
	}

	protected virtual void ScrollBar_DrawThumb(ScrollBar bar, Rectangle thumb_pos, Rectangle clip, Graphics dc)
	{
		if (bar.Enabled && thumb_pos.Width > 0 && thumb_pos.Height > 0 && clip.IntersectsWith(thumb_pos))
		{
			DrawScrollButtonPrimitive(dc, thumb_pos, ButtonState.Normal);
		}
	}

	public override void DrawStatusBar(Graphics real_dc, Rectangle clip, StatusBar sb)
	{
		Rectangle clientRectangle = sb.ClientRectangle;
		int num = 2;
		int num2 = 2;
		Image image = new Bitmap(sb.ClientSize.Width, sb.ClientSize.Height, real_dc);
		Graphics graphics = Graphics.FromImage(image);
		DrawStatusBarBackground(graphics, clip, sb);
		if (!sb.ShowPanels && sb.Text != string.Empty)
		{
			string text = sb.Text;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.Character;
			stringFormat.FormatFlags = StringFormatFlags.NoWrap;
			if (text.Length > 127)
			{
				text = text.Substring(0, 127);
			}
			if (text[0] == '\t')
			{
				stringFormat.Alignment = StringAlignment.Center;
				text = text.Substring(1);
				if (text[0] == '\t')
				{
					stringFormat.Alignment = StringAlignment.Far;
					text = text.Substring(1);
				}
			}
			graphics.DrawString(text, sb.Font, ResPool.GetSolidBrush(sb.ForeColor), new Rectangle(clientRectangle.X + 2, clientRectangle.Y + 2, clientRectangle.Width - 4, clientRectangle.Height - 4), stringFormat);
			stringFormat.Dispose();
		}
		else if (sb.ShowPanels)
		{
			Brush controlForeBrush = GetControlForeBrush(sb.ForeColor);
			int num3 = clientRectangle.X + num;
			int y = clientRectangle.Y + num2;
			for (int i = 0; i < sb.Panels.Count; i++)
			{
				Rectangle area = new Rectangle(num3, y, sb.Panels[i].Width, clientRectangle.Height);
				num3 += area.Width + StatusBarHorzGapWidth;
				if (area.IntersectsWith(clip))
				{
					DrawStatusBarPanel(graphics, area, i, controlForeBrush, sb.Panels[i]);
				}
			}
		}
		if (sb.SizingGrip)
		{
			DrawStatusBarSizingGrip(graphics, clip, sb, clientRectangle);
		}
		real_dc.DrawImage(image, 0, 0);
		graphics.Dispose();
		image.Dispose();
	}

	protected virtual void DrawStatusBarBackground(Graphics dc, Rectangle clip, StatusBar sb)
	{
		Brush brush = ((sb.BackColor.ToArgb() != ColorControl.ToArgb()) ? ResPool.GetSolidBrush(sb.BackColor) : SystemBrushes.Control);
		dc.FillRectangle(brush, clip);
	}

	protected virtual void DrawStatusBarSizingGrip(Graphics dc, Rectangle clip, StatusBar sb, Rectangle area)
	{
		area = new Rectangle(area.Right - 16 - 2, area.Bottom - 12 - 1, 16, 16);
		CPDrawSizeGrip(dc, ColorControl, area);
	}

	protected virtual void DrawStatusBarPanel(Graphics dc, Rectangle area, int index, Brush br_forecolor, StatusBarPanel panel)
	{
		int num = 3;
		int num2 = 16;
		area.Height -= num;
		DrawStatusBarPanelBackground(dc, area, panel);
		if (panel.Style == StatusBarPanelStyle.OwnerDraw)
		{
			StatusBarDrawItemEventArgs e = new StatusBarDrawItemEventArgs(dc, panel.Parent.Font, area, index, DrawItemState.Default, panel, panel.Parent.ForeColor, panel.Parent.BackColor);
			panel.Parent.OnDrawItemInternal(e);
			return;
		}
		string text = panel.Text;
		StringFormat stringFormat = new StringFormat();
		stringFormat.Trimming = StringTrimming.Character;
		stringFormat.FormatFlags = StringFormatFlags.NoWrap;
		if (text != null && text.Length > 0 && text[0] == '\t')
		{
			stringFormat.Alignment = StringAlignment.Center;
			text = text.Substring(1);
			if (text[0] == '\t')
			{
				stringFormat.Alignment = StringAlignment.Far;
				text = text.Substring(1);
			}
		}
		Rectangle empty = Rectangle.Empty;
		int num3 = 0;
		int num4 = area.Height / 2 - (int)panel.Parent.Font.Size / 2 - 1;
		switch (panel.Alignment)
		{
		case HorizontalAlignment.Right:
		{
			int num7 = (int)dc.MeasureString(text, panel.Parent.Font).Width;
			int num6 = area.Right - num7 - 4;
			empty = new Rectangle(num6, num4, area.Right - num6 - num, area.Bottom - num4 - num);
			if (panel.Icon != null)
			{
				num3 = num6 - num2 - 2;
			}
			break;
		}
		case HorizontalAlignment.Center:
		{
			int num7 = (int)dc.MeasureString(text, panel.Parent.Font).Width;
			int num6 = area.Left + (panel.Width - num7) / 2;
			empty = new Rectangle(num6, num4, area.Right - num6 - num, area.Bottom - num4 - num);
			if (panel.Icon != null)
			{
				num3 = num6 - num2 - 2;
			}
			break;
		}
		default:
		{
			int num5 = area.Left + num;
			if (panel.Icon != null)
			{
				num3 = area.Left + 2;
				num5 = num3 + num2 + 2;
			}
			int num6 = num5;
			empty = new Rectangle(num6, num4, area.Right - num6 - num, area.Bottom - num4 - num);
			break;
		}
		}
		RectangleF clipBounds = dc.ClipBounds;
		dc.SetClip(area);
		dc.DrawString(text, panel.Parent.Font, br_forecolor, empty, stringFormat);
		dc.SetClip(clipBounds);
		if (panel.Icon != null)
		{
			dc.DrawIcon(panel.Icon, new Rectangle(num3, num4, num2, num2));
		}
	}

	protected virtual void DrawStatusBarPanelBackground(Graphics dc, Rectangle area, StatusBarPanel panel)
	{
		if (panel.BorderStyle != StatusBarPanelBorderStyle.None)
		{
			Border3DStyle style = Border3DStyle.SunkenOuter;
			if (panel.BorderStyle == StatusBarPanelBorderStyle.Raised)
			{
				style = Border3DStyle.RaisedInner;
			}
			CPDrawBorder3D(dc, area, style, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, panel.Parent.BackColor);
		}
	}

	public override Size TabControlGetSpacing(TabControl tab)
	{
		try
		{
			return ThemeElements.CurrentTheme.TabControlPainter.RowSpacing(tab);
		}
		catch
		{
			throw new Exception("Invalid Appearance value: " + tab.Appearance);
		}
	}

	public override void DrawTabControl(Graphics dc, Rectangle area, TabControl tab)
	{
		ThemeElements.CurrentTheme.TabControlPainter.Draw(dc, area, tab);
	}

	public override Rectangle TabControlGetDisplayRectangle(TabControl tab)
	{
		return ThemeElements.CurrentTheme.TabControlPainter.GetDisplayRectangle(tab);
	}

	public override Rectangle TabControlGetPanelRect(TabControl tab)
	{
		return ThemeElements.CurrentTheme.TabControlPainter.GetTabPanelRect(tab);
	}

	public override void TextBoxBaseFillBackground(TextBoxBase textBoxBase, Graphics g, Rectangle clippingArea)
	{
		if (textBoxBase.backcolor_set || (textBoxBase.Enabled && !textBoxBase.read_only))
		{
			g.FillRectangle(ResPool.GetSolidBrush(textBoxBase.BackColor), clippingArea);
		}
		else
		{
			g.FillRectangle(ResPool.GetSolidBrush(ColorControl), clippingArea);
		}
	}

	public override bool TextBoxBaseHandleWmNcPaint(TextBoxBase textBoxBase, ref Message m)
	{
		return false;
	}

	public override bool TextBoxBaseShouldPaintBackground(TextBoxBase textBoxBase)
	{
		return true;
	}

	public override void DrawToolBar(Graphics dc, Rectangle clip_rectangle, ToolBar control)
	{
		StringFormat stringFormat = new StringFormat();
		stringFormat.Trimming = StringTrimming.EllipsisCharacter;
		stringFormat.LineAlignment = StringAlignment.Center;
		if (control.ShowKeyboardCuesInternal)
		{
			stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
		}
		else
		{
			stringFormat.HotkeyPrefix = HotkeyPrefix.Hide;
		}
		if (control.TextAlign == ToolBarTextAlign.Underneath)
		{
			stringFormat.Alignment = StringAlignment.Center;
		}
		else
		{
			stringFormat.Alignment = StringAlignment.Near;
		}
		if (control.Appearance != ToolBarAppearance.Flat || control.Parent == null)
		{
			dc.FillRectangle(SystemBrushes.Control, clip_rectangle);
		}
		if (control.Divider && clip_rectangle.Y < 2)
		{
			if (clip_rectangle.Y < 1)
			{
				dc.DrawLine(SystemPens.ControlDark, clip_rectangle.X, 0, clip_rectangle.Right, 0);
			}
			dc.DrawLine(SystemPens.ControlLightLight, clip_rectangle.X, 1, clip_rectangle.Right, 1);
		}
		ToolBarItem[] items = control.items;
		foreach (ToolBarItem toolBarItem in items)
		{
			if (toolBarItem.Button.Visible && clip_rectangle.IntersectsWith(toolBarItem.Rectangle))
			{
				DrawToolBarButton(dc, control, toolBarItem, stringFormat);
			}
		}
		stringFormat.Dispose();
	}

	protected virtual void DrawToolBarButton(Graphics dc, ToolBar control, ToolBarItem item, StringFormat format)
	{
		bool flag = control.Appearance == ToolBarAppearance.Flat;
		DrawToolBarButtonBorder(dc, item, flag);
		switch (item.Button.Style)
		{
		case ToolBarButtonStyle.DropDownButton:
			if (control.DropDownArrows)
			{
				DrawToolBarDropDownArrow(dc, item, flag);
			}
			DrawToolBarButtonContents(dc, control, item, format);
			break;
		case ToolBarButtonStyle.Separator:
			if (flag)
			{
				DrawToolBarSeparator(dc, item);
			}
			break;
		case ToolBarButtonStyle.ToggleButton:
			DrawToolBarToggleButtonBackground(dc, item);
			DrawToolBarButtonContents(dc, control, item, format);
			break;
		default:
			DrawToolBarButtonContents(dc, control, item, format);
			break;
		}
	}

	protected virtual void DrawToolBarButtonBorder(Graphics dc, ToolBarItem item, bool is_flat)
	{
		if (item.Button.Style == ToolBarButtonStyle.Separator)
		{
			return;
		}
		Border3DStyle style;
		if (!is_flat)
		{
			style = ((!item.Button.Pushed && !item.Pressed) ? Border3DStyle.Raised : Border3DStyle.Sunken);
		}
		else if (item.Button.Pushed || item.Pressed)
		{
			style = Border3DStyle.SunkenOuter;
		}
		else
		{
			if (!item.Hilight)
			{
				return;
			}
			style = Border3DStyle.RaisedInner;
		}
		Rectangle rectangle = item.Rectangle;
		if (item.Button.Style == ToolBarButtonStyle.DropDownButton && item.Button.Parent.DropDownArrows && is_flat)
		{
			rectangle.Width -= ToolBarDropDownWidth;
		}
		CPDrawBorder3D(dc, rectangle, style, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
	}

	protected virtual void DrawToolBarSeparator(Graphics dc, ToolBarItem item)
	{
		Rectangle rectangle = item.Rectangle;
		int num = (int)SystemPens.Control.Width + 1;
		dc.DrawLine(SystemPens.ControlDark, rectangle.X + 1, rectangle.Y, rectangle.X + 1, rectangle.Bottom);
		dc.DrawLine(SystemPens.ControlLight, rectangle.X + num, rectangle.Y, rectangle.X + num, rectangle.Bottom);
	}

	protected virtual void DrawToolBarToggleButtonBackground(Graphics dc, ToolBarItem item)
	{
		Rectangle rectangle = item.Rectangle;
		rectangle.X += ToolBarImageGripWidth;
		rectangle.Y += ToolBarImageGripWidth;
		rectangle.Width -= 2 * ToolBarImageGripWidth;
		rectangle.Height -= 2 * ToolBarImageGripWidth;
		Brush brush = (item.Button.Pushed ? ResPool.GetHatchBrush(HatchStyle.Percent50, ColorScrollBar, ColorControlLightLight) : ((!item.Button.PartialPush) ? SystemBrushes.Control : SystemBrushes.ControlLight));
		dc.FillRectangle(brush, rectangle);
	}

	protected virtual void DrawToolBarDropDownArrow(Graphics dc, ToolBarItem item, bool is_flat)
	{
		Rectangle rectangle = item.Rectangle;
		rectangle.X = item.Rectangle.Right - ToolBarDropDownWidth;
		rectangle.Width = ToolBarDropDownWidth;
		if (is_flat)
		{
			if (item.DDPressed)
			{
				CPDrawBorder3D(dc, rectangle, Border3DStyle.SunkenOuter, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
			}
			else if (item.Button.Pushed || item.Pressed)
			{
				CPDrawBorder3D(dc, rectangle, Border3DStyle.SunkenOuter, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
			}
			else if (item.Hilight)
			{
				CPDrawBorder3D(dc, rectangle, Border3DStyle.RaisedInner, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
			}
		}
		else if (item.DDPressed)
		{
			CPDrawBorder3D(dc, rectangle, Border3DStyle.Flat, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
		}
		else if (item.Button.Pushed || item.Pressed)
		{
			CPDrawBorder3D(dc, Rectangle.Inflate(rectangle, -1, -1), Border3DStyle.SunkenOuter, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
		}
		else
		{
			CPDrawBorder3D(dc, rectangle, Border3DStyle.Raised, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
		}
		PointF[] array = new PointF[3];
		PointF pointF = new PointF((float)rectangle.X + (float)rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2);
		if (item.Pressed || item.Button.Pushed || item.DDPressed)
		{
			pointF.X += 1f;
			pointF.Y += 1f;
		}
		array[0].X = pointF.X - (float)ToolBarDropDownArrowWidth / 2f + 0.5f;
		array[0].Y = pointF.Y;
		array[1].X = pointF.X + (float)ToolBarDropDownArrowWidth / 2f + 0.5f;
		array[1].Y = pointF.Y;
		array[2].X = pointF.X + 0.5f;
		array[2].Y = pointF.Y + (float)ToolBarDropDownArrowHeight;
		dc.FillPolygon(SystemBrushes.ControlText, array);
	}

	protected virtual void DrawToolBarButtonContents(Graphics dc, ToolBar control, ToolBarItem item, StringFormat format)
	{
		if (item.Button.Image != null)
		{
			int num = item.ImageRectangle.X + ToolBarImageGripWidth;
			int num2 = item.ImageRectangle.Y + ToolBarImageGripWidth;
			if (item.Pressed || item.Button.Pushed)
			{
				num++;
				num2++;
			}
			if (item.Button.Enabled)
			{
				dc.DrawImage(item.Button.Image, num, num2);
			}
			else
			{
				CPDrawImageDisabled(dc, item.Button.Image, num, num2, ColorControl);
			}
		}
		Rectangle textRectangle = item.TextRectangle;
		if (textRectangle.Width > 0 && textRectangle.Height > 0)
		{
			if (item.Pressed || item.Button.Pushed)
			{
				textRectangle.X++;
				textRectangle.Y++;
			}
			if (item.Button.Enabled)
			{
				dc.DrawString(item.Button.Text, control.Font, SystemBrushes.ControlText, textRectangle, format);
			}
			else
			{
				CPDrawStringDisabled(dc, item.Button.Text, control.Font, control.BackColor, textRectangle, format);
			}
		}
	}

	public override bool ToolBarHasHotElementStyles(ToolBar toolBar)
	{
		return toolBar.Appearance == ToolBarAppearance.Flat;
	}

	public override void DrawToolTip(Graphics dc, Rectangle clip_rectangle, ToolTip.ToolTipWindow control)
	{
		ToolTipDrawBackground(dc, clip_rectangle, control);
		TextFormatFlags flags = TextFormatFlags.HidePrefix;
		Color foreColor = control.ForeColor;
		if (control.title.Length > 0)
		{
			Font font = new Font(control.Font, control.Font.Style | FontStyle.Bold);
			TextRenderer.DrawTextInternal(dc, control.title, font, control.title_rect, foreColor, flags, useDrawString: false);
			font.Dispose();
		}
		if (control.icon != null)
		{
			dc.DrawIcon(control.icon, control.icon_rect);
		}
		TextRenderer.DrawTextInternal(dc, control.Text, control.Font, control.text_rect, foreColor, flags, useDrawString: false);
	}

	protected virtual void ToolTipDrawBackground(Graphics dc, Rectangle clip_rectangle, ToolTip.ToolTipWindow control)
	{
		Brush solidBrush = ResPool.GetSolidBrush(control.BackColor);
		dc.FillRectangle(solidBrush, control.ClientRectangle);
		dc.DrawRectangle(SystemPens.WindowFrame, 0, 0, control.Width - 1, control.Height - 1);
	}

	public override Size ToolTipSize(ToolTip.ToolTipWindow tt, string text)
	{
		Size size = TextRenderer.MeasureTextInternal(text, tt.Font, useMeasureString: false);
		size.Width += 4;
		size.Height += 3;
		Rectangle text_rect = new Rectangle(Point.Empty, size);
		text_rect.Inflate(-2, -1);
		tt.text_rect = text_rect;
		tt.icon_rect = (tt.title_rect = Rectangle.Empty);
		Size size2 = Size.Empty;
		if (tt.title.Length > 0)
		{
			Font font = new Font(tt.Font, tt.Font.Style | FontStyle.Bold);
			size2 = TextRenderer.MeasureTextInternal(tt.title, font, useMeasureString: false);
			font.Dispose();
		}
		Size size3 = Size.Empty;
		if (tt.icon != null)
		{
			size3 = new Size(size.Height, size.Height);
		}
		if (size3 != Size.Empty || size2 != Size.Empty)
		{
			int num = 8;
			int num2 = 0;
			int num3 = ((size3.Height <= size2.Height) ? size2.Height : size3.Height);
			Size size4 = size;
			Point location = new Point(num, num);
			if (size3 != Size.Empty)
			{
				tt.icon_rect = new Rectangle(location, size3);
				num2 = size3.Width + num;
			}
			if (size2 != Size.Empty)
			{
				Rectangle title_rect = new Rectangle(location, new Size(size2.Width, num3));
				if (size3 != Size.Empty)
				{
					title_rect.X += size3.Width + num;
				}
				tt.title_rect = title_rect;
				num2 += size2.Width;
			}
			tt.text_rect = new Rectangle(new Point(location.X, location.Y + num3 + num), size4);
			size.Height += num + num3;
			if (num2 > size.Width)
			{
				size.Width = num2;
			}
			size.Width += num * 2;
			size.Height += num * 2;
		}
		return size;
	}

	public override void ShowBalloonWindow(IntPtr handle, int timeout, string title, string text, ToolTipIcon icon)
	{
		Control control = Control.FromHandle(handle);
		if (control != null)
		{
			if (balloon_window != null)
			{
				balloon_window.Close();
				balloon_window.Dispose();
			}
			balloon_window = new NotifyIcon.BalloonWindow(handle);
			balloon_window.Title = title;
			balloon_window.Text = text;
			balloon_window.Icon = icon;
			balloon_window.Timeout = timeout;
			balloon_window.Show();
		}
	}

	public override void DrawBalloonWindow(Graphics dc, Rectangle clip, NotifyIcon.BalloonWindow control)
	{
		Brush solidBrush = ResPool.GetSolidBrush(ColorInfoText);
		Rectangle clientRectangle = control.ClientRectangle;
		int num = ((control.Icon != 0) ? 16 : 0);
		dc.FillRectangle(ResPool.GetSolidBrush(ColorInfo), clientRectangle);
		dc.DrawRectangle(ResPool.GetPen(ColorWindowFrame), 0, 0, clientRectangle.Width - 1, clientRectangle.Height - 1);
		Image image = control.Icon switch
		{
			ToolTipIcon.Info => ThemeEngine.Current.Images(UIIcon.MessageBoxInfo, 16), 
			ToolTipIcon.Warning => ThemeEngine.Current.Images(UIIcon.MessageBoxError, 16), 
			ToolTipIcon.Error => ThemeEngine.Current.Images(UIIcon.MessageBoxWarning, 16), 
			_ => null, 
		};
		if (control.Icon != 0)
		{
			dc.DrawImage(image, new Rectangle(8, 8, num, num));
		}
		Rectangle rectangle = new Rectangle(clientRectangle.X + 8 + num + ((num > 0) ? 8 : 0), clientRectangle.Y + 8, clientRectangle.Width - (24 + num), clientRectangle.Height - 16);
		Font font = new Font(control.Font.FontFamily, control.Font.Size, control.Font.Style | FontStyle.Bold, control.Font.Unit);
		dc.DrawString(control.Title, font, solidBrush, rectangle, control.Format);
		Rectangle rectangle2 = new Rectangle(clientRectangle.X + 8, clientRectangle.Y + 8, clientRectangle.Width - 16, clientRectangle.Height - 16);
		StringFormat format = control.Format;
		format.LineAlignment = StringAlignment.Far;
		dc.DrawString(control.Text, control.Font, solidBrush, rectangle2, format);
	}

	public override Rectangle BalloonWindowRect(NotifyIcon.BalloonWindow control)
	{
		Rectangle workingArea = Screen.GetWorkingArea(control);
		SizeF layoutArea = new SizeF(250f, 200f);
		SizeF sizeF = TextRenderer.MeasureString(control.Title, control.Font, layoutArea, control.Format);
		SizeF sizeF2 = TextRenderer.MeasureString(control.Text, control.Font, layoutArea, control.Format);
		if (sizeF.Height < 16f)
		{
			sizeF.Height = 16f;
		}
		Rectangle result = default(Rectangle);
		result.Height = (int)(sizeF.Height + sizeF2.Height + 24f);
		result.Width = (int)((!(sizeF.Width > sizeF2.Width)) ? sizeF2.Width : sizeF.Width) + 16;
		result.X = workingArea.Width - result.Width - 2;
		result.Y = workingArea.Height - result.Height - 2;
		return result;
	}

	public override int TrackBarValueFromMousePosition(int x, int y, TrackBar tb)
	{
		int value = tb.Value;
		int value2 = tb.Value;
		Rectangle thumb_pos = Rectangle.Empty;
		Rectangle thumb_area = Rectangle.Empty;
		Point channel_startpoint = Point.Empty;
		Point bottomtick_startpoint = Point.Empty;
		GetTrackBarDrawingInfo(tb, out var pixels_betweenticks, out thumb_area, out thumb_pos, out channel_startpoint, out bottomtick_startpoint, out bottomtick_startpoint);
		if (tb.Orientation == Orientation.Vertical)
		{
			value2 = (int)Math.Round(((float)(thumb_area.Bottom - y) - (float)thumb_pos.Height / 2f) / pixels_betweenticks, 0);
			if (value2 + tb.Minimum > tb.Maximum)
			{
				value2 = tb.Maximum - tb.Minimum;
			}
			else if (value2 + tb.Minimum < tb.Minimum)
			{
				value2 = 0;
			}
			return value2 + tb.Minimum;
		}
		value2 = (int)Math.Round(((float)(x - channel_startpoint.X) - (float)thumb_pos.Width / 2f) / pixels_betweenticks, 0);
		if (value2 + tb.Minimum > tb.Maximum)
		{
			value2 = tb.Maximum - tb.Minimum;
		}
		else if (value2 + tb.Minimum < tb.Minimum)
		{
			value2 = 0;
		}
		return value2 + tb.Minimum;
	}

	private void GetTrackBarDrawingInfo(TrackBar tb, out float pixels_betweenticks, out Rectangle thumb_area, out Rectangle thumb_pos, out Point channel_startpoint, out Point bottomtick_startpoint, out Point toptick_startpoint)
	{
		thumb_area = Rectangle.Empty;
		thumb_pos = Rectangle.Empty;
		if (tb.Orientation == Orientation.Vertical)
		{
			toptick_startpoint = default(Point);
			bottomtick_startpoint = default(Point);
			channel_startpoint = default(Point);
			Rectangle clientRectangle = tb.ClientRectangle;
			switch (tb.TickStyle)
			{
			case TickStyle.None:
			case TickStyle.BottomRight:
				channel_startpoint.Y = 8;
				channel_startpoint.X = 9;
				bottomtick_startpoint.Y = 13;
				bottomtick_startpoint.X = 24;
				break;
			case TickStyle.TopLeft:
				channel_startpoint.Y = 8;
				channel_startpoint.X = 19;
				toptick_startpoint.Y = 13;
				toptick_startpoint.X = 8;
				break;
			case TickStyle.Both:
				channel_startpoint.Y = 8;
				channel_startpoint.X = 18;
				bottomtick_startpoint.Y = 13;
				bottomtick_startpoint.X = 32;
				toptick_startpoint.Y = 13;
				toptick_startpoint.X = 8;
				break;
			}
			thumb_area.X = clientRectangle.X + channel_startpoint.X;
			thumb_area.Y = clientRectangle.Y + channel_startpoint.Y;
			thumb_area.Height = clientRectangle.Height - 8 - 8;
			thumb_area.Width = 4;
			float num = thumb_area.Height - 11;
			if (tb.Maximum == tb.Minimum)
			{
				pixels_betweenticks = 0f;
			}
			else
			{
				pixels_betweenticks = num / (float)(tb.Maximum - tb.Minimum);
			}
			thumb_pos.Y = thumb_area.Bottom - 11 - (int)(pixels_betweenticks * (float)(tb.Value - tb.Minimum));
		}
		else
		{
			toptick_startpoint = default(Point);
			bottomtick_startpoint = default(Point);
			channel_startpoint = default(Point);
			Rectangle clientRectangle2 = tb.ClientRectangle;
			switch (tb.TickStyle)
			{
			case TickStyle.None:
			case TickStyle.BottomRight:
				channel_startpoint.X = 8;
				channel_startpoint.Y = 9;
				bottomtick_startpoint.X = 13;
				bottomtick_startpoint.Y = 24;
				break;
			case TickStyle.TopLeft:
				channel_startpoint.X = 8;
				channel_startpoint.Y = 19;
				toptick_startpoint.X = 13;
				toptick_startpoint.Y = 8;
				break;
			case TickStyle.Both:
				channel_startpoint.X = 8;
				channel_startpoint.Y = 18;
				bottomtick_startpoint.X = 13;
				bottomtick_startpoint.Y = 32;
				toptick_startpoint.X = 13;
				toptick_startpoint.Y = 8;
				break;
			}
			thumb_area.X = clientRectangle2.X + channel_startpoint.X;
			thumb_area.Y = clientRectangle2.Y + channel_startpoint.Y;
			thumb_area.Width = clientRectangle2.Width - 8 - 8;
			thumb_area.Height = 4;
			float num2 = thumb_area.Width - 11;
			if (tb.Maximum == tb.Minimum)
			{
				pixels_betweenticks = 0f;
			}
			else
			{
				pixels_betweenticks = num2 / (float)(tb.Maximum - tb.Minimum);
			}
			thumb_pos.X = channel_startpoint.X + (int)(pixels_betweenticks * (float)(tb.Value - tb.Minimum));
		}
		thumb_pos.Size = TrackBarGetThumbSize(tb);
	}

	protected virtual Size TrackBarGetThumbSize(TrackBar trackBar)
	{
		return TrackBarGetThumbSize();
	}

	public static Size TrackBarGetThumbSize()
	{
		return new Size(10, 22);
	}

	protected virtual ITrackBarTickPainter GetTrackBarTickPainter(Graphics g)
	{
		return new TrackBarTickPainter(g, ResPool.GetPen(pen_ticks_color));
	}

	private void DrawTrackBar_Vertical(Graphics dc, Rectangle clip_rectangle, TrackBar tb, ref Rectangle thumb_pos, ref Rectangle thumb_area, Brush br_thumb, float ticks, int value_pos, bool mouse_value)
	{
		Point toptick_startpoint = default(Point);
		Point bottomtick_startpoint = default(Point);
		Point channel_startpoint = default(Point);
		Rectangle clientRectangle = tb.ClientRectangle;
		GetTrackBarDrawingInfo(tb, out var pixels_betweenticks, out thumb_area, out thumb_pos, out channel_startpoint, out bottomtick_startpoint, out toptick_startpoint);
		TrackBarDrawVerticalTrack(dc, thumb_area, channel_startpoint, clip_rectangle);
		switch (tb.TickStyle)
		{
		case TickStyle.None:
		case TickStyle.BottomRight:
			thumb_pos.X = channel_startpoint.X - 8;
			TrackBarDrawVerticalThumbRight(dc, thumb_pos, br_thumb, clip_rectangle, tb);
			break;
		case TickStyle.TopLeft:
			thumb_pos.X = channel_startpoint.X - 10;
			TrackBarDrawVerticalThumbLeft(dc, thumb_pos, br_thumb, clip_rectangle, tb);
			break;
		default:
			thumb_pos.X = clientRectangle.X + 10;
			TrackBarDrawVerticalThumb(dc, thumb_pos, br_thumb, clip_rectangle, tb);
			break;
		}
		float num = thumb_area.Height - 11;
		pixels_betweenticks = num / ticks;
		thumb_area.X = thumb_pos.X;
		thumb_area.Y = channel_startpoint.Y;
		thumb_area.Width = thumb_pos.Height;
		if (pixels_betweenticks <= 0f || tb.TickStyle == TickStyle.None)
		{
			return;
		}
		Region region = new Region(clientRectangle);
		region.Exclude(thumb_area);
		if (region.IsVisible(clip_rectangle))
		{
			ITrackBarTickPainter trackBarTickPainter = TrackBarGetVerticalTickPainter(dc);
			if ((tb.TickStyle & TickStyle.BottomRight) == TickStyle.BottomRight)
			{
				float num2 = clientRectangle.X + bottomtick_startpoint.X;
				for (float num3 = 0f; num3 < num + 1f; num3 += pixels_betweenticks)
				{
					float num4 = (float)(clientRectangle.Y + bottomtick_startpoint.Y) + num3;
					trackBarTickPainter.Paint(num2, num4, num2 + (float)((num3 != 0f && !(num3 + pixels_betweenticks >= num + 1f)) ? 2 : 3), num4);
				}
			}
			if ((tb.TickStyle & TickStyle.TopLeft) == TickStyle.TopLeft)
			{
				float num5 = clientRectangle.X + toptick_startpoint.X;
				for (float num6 = 0f; num6 < num + 1f; num6 += pixels_betweenticks)
				{
					float num7 = (float)(clientRectangle.Y + toptick_startpoint.Y) + num6;
					trackBarTickPainter.Paint(num5 - (float)((num6 != 0f && !(num6 + pixels_betweenticks >= num + 1f)) ? 2 : 3), num7, num5, num7);
				}
			}
		}
		region.Dispose();
	}

	protected virtual void TrackBarDrawVerticalTrack(Graphics dc, Rectangle thumb_area, Point channel_startpoint, Rectangle clippingArea)
	{
		dc.FillRectangle(SystemBrushes.ControlDark, channel_startpoint.X, channel_startpoint.Y, 1, thumb_area.Height);
		dc.FillRectangle(SystemBrushes.ControlDarkDark, channel_startpoint.X + 1, channel_startpoint.Y, 1, thumb_area.Height);
		dc.FillRectangle(SystemBrushes.ControlLight, channel_startpoint.X + 3, channel_startpoint.Y, 1, thumb_area.Height);
	}

	protected virtual void TrackBarDrawVerticalThumbRight(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		Pen controlLightLight = SystemPens.ControlLightLight;
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y, thumb_pos.X, thumb_pos.Y + 10);
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y, thumb_pos.X + 16, thumb_pos.Y);
		dc.DrawLine(controlLightLight, thumb_pos.X + 16, thumb_pos.Y, thumb_pos.X + 16 + 4, thumb_pos.Y + 4);
		controlLightLight = SystemPens.ControlDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 1, thumb_pos.Y + 9, thumb_pos.X + 15, thumb_pos.Y + 9);
		dc.DrawLine(controlLightLight, thumb_pos.X + 16, thumb_pos.Y + 9, thumb_pos.X + 16 + 4, thumb_pos.Y + 9 - 4);
		controlLightLight = SystemPens.ControlDarkDark;
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y + 10, thumb_pos.X + 16, thumb_pos.Y + 10);
		dc.DrawLine(controlLightLight, thumb_pos.X + 16, thumb_pos.Y + 10, thumb_pos.X + 16 + 5, thumb_pos.Y + 10 - 5);
		dc.FillRectangle(br_thumb, thumb_pos.X + 1, thumb_pos.Y + 1, 16, 8);
		dc.FillRectangle(br_thumb, thumb_pos.X + 17, thumb_pos.Y + 2, 1, 6);
		dc.FillRectangle(br_thumb, thumb_pos.X + 18, thumb_pos.Y + 3, 1, 4);
		dc.FillRectangle(br_thumb, thumb_pos.X + 19, thumb_pos.Y + 4, 1, 2);
	}

	protected virtual void TrackBarDrawVerticalThumbLeft(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		Pen controlLightLight = SystemPens.ControlLightLight;
		dc.DrawLine(controlLightLight, thumb_pos.X + 4, thumb_pos.Y, thumb_pos.X + 4 + 16, thumb_pos.Y);
		dc.DrawLine(controlLightLight, thumb_pos.X + 4, thumb_pos.Y, thumb_pos.X, thumb_pos.Y + 4);
		controlLightLight = SystemPens.ControlDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 4, thumb_pos.Y + 9, thumb_pos.X + 4 + 16, thumb_pos.Y + 9);
		dc.DrawLine(controlLightLight, thumb_pos.X + 4, thumb_pos.Y + 9, thumb_pos.X, thumb_pos.Y + 5);
		dc.DrawLine(controlLightLight, thumb_pos.X + 19, thumb_pos.Y + 9, thumb_pos.X + 19, thumb_pos.Y + 1);
		controlLightLight = SystemPens.ControlDarkDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 4, thumb_pos.Y + 10, thumb_pos.X + 4 + 16, thumb_pos.Y + 10);
		dc.DrawLine(controlLightLight, thumb_pos.X + 4, thumb_pos.Y + 10, thumb_pos.X - 1, thumb_pos.Y + 5);
		dc.DrawLine(controlLightLight, thumb_pos.X + 20, thumb_pos.Y, thumb_pos.X + 20, thumb_pos.Y + 10);
		dc.FillRectangle(br_thumb, thumb_pos.X + 4, thumb_pos.Y + 1, 15, 8);
		dc.FillRectangle(br_thumb, thumb_pos.X + 3, thumb_pos.Y + 2, 1, 6);
		dc.FillRectangle(br_thumb, thumb_pos.X + 2, thumb_pos.Y + 3, 1, 4);
		dc.FillRectangle(br_thumb, thumb_pos.X + 1, thumb_pos.Y + 4, 1, 2);
	}

	protected virtual void TrackBarDrawVerticalThumb(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		Pen controlLightLight = SystemPens.ControlLightLight;
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y, thumb_pos.X, thumb_pos.Y + 9);
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y, thumb_pos.X + 19, thumb_pos.Y);
		controlLightLight = SystemPens.ControlDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 1, thumb_pos.Y + 9, thumb_pos.X + 19, thumb_pos.Y + 9);
		dc.DrawLine(controlLightLight, thumb_pos.X + 10, thumb_pos.Y + 1, thumb_pos.X + 19, thumb_pos.Y + 8);
		controlLightLight = SystemPens.ControlDarkDark;
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y + 10, thumb_pos.X + 20, thumb_pos.Y + 10);
		dc.DrawLine(controlLightLight, thumb_pos.X + 20, thumb_pos.Y, thumb_pos.X + 20, thumb_pos.Y + 9);
		dc.FillRectangle(br_thumb, thumb_pos.X + 1, thumb_pos.Y + 1, 18, 8);
	}

	protected virtual ITrackBarTickPainter TrackBarGetVerticalTickPainter(Graphics g)
	{
		return GetTrackBarTickPainter(g);
	}

	private void DrawTrackBar_Horizontal(Graphics dc, Rectangle clip_rectangle, TrackBar tb, ref Rectangle thumb_pos, ref Rectangle thumb_area, Brush br_thumb, float ticks, int value_pos, bool mouse_value)
	{
		Point toptick_startpoint = default(Point);
		Point bottomtick_startpoint = default(Point);
		Point channel_startpoint = default(Point);
		Rectangle clientRectangle = tb.ClientRectangle;
		GetTrackBarDrawingInfo(tb, out var pixels_betweenticks, out thumb_area, out thumb_pos, out channel_startpoint, out bottomtick_startpoint, out toptick_startpoint);
		TrackBarDrawHorizontalTrack(dc, thumb_area, channel_startpoint, clip_rectangle);
		switch (tb.TickStyle)
		{
		case TickStyle.None:
		case TickStyle.BottomRight:
			thumb_pos.Y = channel_startpoint.Y - 8;
			TrackBarDrawHorizontalThumbBottom(dc, thumb_pos, br_thumb, clip_rectangle, tb);
			break;
		case TickStyle.TopLeft:
			thumb_pos.Y = channel_startpoint.Y - 10;
			TrackBarDrawHorizontalThumbTop(dc, thumb_pos, br_thumb, clip_rectangle, tb);
			break;
		default:
			thumb_pos.Y = clientRectangle.Y + 10;
			TrackBarDrawHorizontalThumb(dc, thumb_pos, br_thumb, clip_rectangle, tb);
			break;
		}
		float num = thumb_area.Width - 11;
		pixels_betweenticks = num / ticks;
		thumb_area.Y = thumb_pos.Y;
		thumb_area.X = channel_startpoint.X;
		thumb_area.Height = thumb_pos.Height;
		if (pixels_betweenticks <= 0f || tb.TickStyle == TickStyle.None)
		{
			return;
		}
		Region region = new Region(clientRectangle);
		region.Exclude(thumb_area);
		if (region.IsVisible(clip_rectangle))
		{
			ITrackBarTickPainter trackBarTickPainter = TrackBarGetHorizontalTickPainter(dc);
			if ((tb.TickStyle & TickStyle.BottomRight) == TickStyle.BottomRight)
			{
				float num2 = clientRectangle.Y + bottomtick_startpoint.Y;
				for (float num3 = 0f; num3 < num + 1f; num3 += pixels_betweenticks)
				{
					float num4 = (float)(clientRectangle.X + bottomtick_startpoint.X) + num3;
					trackBarTickPainter.Paint(num4, num2, num4, num2 + (float)((num3 != 0f && !(num3 + pixels_betweenticks >= num + 1f)) ? 2 : 3));
				}
			}
			if ((tb.TickStyle & TickStyle.TopLeft) == TickStyle.TopLeft)
			{
				float num5 = clientRectangle.Y + toptick_startpoint.Y;
				for (float num6 = 0f; num6 < num + 1f; num6 += pixels_betweenticks)
				{
					float num7 = (float)(clientRectangle.X + toptick_startpoint.X) + num6;
					trackBarTickPainter.Paint(num7, num5 - (float)((num6 != 0f && !(num6 + pixels_betweenticks >= num + 1f)) ? 2 : 3), num7, num5);
				}
			}
		}
		region.Dispose();
	}

	protected virtual void TrackBarDrawHorizontalTrack(Graphics dc, Rectangle thumb_area, Point channel_startpoint, Rectangle clippingArea)
	{
		dc.FillRectangle(SystemBrushes.ControlDark, channel_startpoint.X, channel_startpoint.Y, thumb_area.Width, 1);
		dc.FillRectangle(SystemBrushes.ControlDarkDark, channel_startpoint.X, channel_startpoint.Y + 1, thumb_area.Width, 1);
		dc.FillRectangle(SystemBrushes.ControlLight, channel_startpoint.X, channel_startpoint.Y + 3, thumb_area.Width, 1);
	}

	protected virtual void TrackBarDrawHorizontalThumbBottom(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		Pen controlLightLight = SystemPens.ControlLightLight;
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y, thumb_pos.X + 10, thumb_pos.Y);
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y, thumb_pos.X, thumb_pos.Y + 16);
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y + 16, thumb_pos.X + 4, thumb_pos.Y + 16 + 4);
		controlLightLight = SystemPens.ControlDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 9, thumb_pos.Y + 1, thumb_pos.X + 9, thumb_pos.Y + 15);
		dc.DrawLine(controlLightLight, thumb_pos.X + 9, thumb_pos.Y + 16, thumb_pos.X + 9 - 4, thumb_pos.Y + 16 + 4);
		controlLightLight = SystemPens.ControlDarkDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 10, thumb_pos.Y, thumb_pos.X + 10, thumb_pos.Y + 16);
		dc.DrawLine(controlLightLight, thumb_pos.X + 10, thumb_pos.Y + 16, thumb_pos.X + 10 - 5, thumb_pos.Y + 16 + 5);
		dc.FillRectangle(br_thumb, thumb_pos.X + 1, thumb_pos.Y + 1, 8, 16);
		dc.FillRectangle(br_thumb, thumb_pos.X + 2, thumb_pos.Y + 17, 6, 1);
		dc.FillRectangle(br_thumb, thumb_pos.X + 3, thumb_pos.Y + 18, 4, 1);
		dc.FillRectangle(br_thumb, thumb_pos.X + 4, thumb_pos.Y + 19, 2, 1);
	}

	protected virtual void TrackBarDrawHorizontalThumbTop(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		Pen controlLightLight = SystemPens.ControlLightLight;
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y + 4, thumb_pos.X, thumb_pos.Y + 4 + 16);
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y + 4, thumb_pos.X + 4, thumb_pos.Y);
		controlLightLight = SystemPens.ControlDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 9, thumb_pos.Y + 4, thumb_pos.X + 9, thumb_pos.Y + 4 + 16);
		dc.DrawLine(controlLightLight, thumb_pos.X + 9, thumb_pos.Y + 4, thumb_pos.X + 5, thumb_pos.Y);
		dc.DrawLine(controlLightLight, thumb_pos.X + 9, thumb_pos.Y + 19, thumb_pos.X + 1, thumb_pos.Y + 19);
		controlLightLight = SystemPens.ControlDarkDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 10, thumb_pos.Y + 4, thumb_pos.X + 10, thumb_pos.Y + 4 + 16);
		dc.DrawLine(controlLightLight, thumb_pos.X + 10, thumb_pos.Y + 4, thumb_pos.X + 5, thumb_pos.Y - 1);
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y + 20, thumb_pos.X + 10, thumb_pos.Y + 20);
		dc.FillRectangle(br_thumb, thumb_pos.X + 1, thumb_pos.Y + 4, 8, 15);
		dc.FillRectangle(br_thumb, thumb_pos.X + 2, thumb_pos.Y + 3, 6, 1);
		dc.FillRectangle(br_thumb, thumb_pos.X + 3, thumb_pos.Y + 2, 4, 1);
		dc.FillRectangle(br_thumb, thumb_pos.X + 4, thumb_pos.Y + 1, 2, 1);
	}

	protected virtual void TrackBarDrawHorizontalThumb(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		Pen controlLightLight = SystemPens.ControlLightLight;
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y, thumb_pos.X + 9, thumb_pos.Y);
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y, thumb_pos.X, thumb_pos.Y + 19);
		controlLightLight = SystemPens.ControlDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 9, thumb_pos.Y + 1, thumb_pos.X + 9, thumb_pos.Y + 19);
		dc.DrawLine(controlLightLight, thumb_pos.X + 1, thumb_pos.Y + 10, thumb_pos.X + 8, thumb_pos.Y + 19);
		controlLightLight = SystemPens.ControlDarkDark;
		dc.DrawLine(controlLightLight, thumb_pos.X + 10, thumb_pos.Y, thumb_pos.X + 10, thumb_pos.Y + 20);
		dc.DrawLine(controlLightLight, thumb_pos.X, thumb_pos.Y + 20, thumb_pos.X + 9, thumb_pos.Y + 20);
		dc.FillRectangle(br_thumb, thumb_pos.X + 1, thumb_pos.Y + 1, 8, 18);
	}

	protected virtual ITrackBarTickPainter TrackBarGetHorizontalTickPainter(Graphics g)
	{
		return GetTrackBarTickPainter(g);
	}

	public override void DrawTrackBar(Graphics dc, Rectangle clip_rectangle, TrackBar tb)
	{
		float ticks = (tb.Maximum - tb.Minimum) / tb.tickFrequency;
		Rectangle thumb_pos = tb.ThumbPos;
		Rectangle thumb_area = tb.ThumbArea;
		int value_pos;
		bool mouse_value;
		if (tb.thumb_pressed)
		{
			value_pos = tb.thumb_mouseclick;
			mouse_value = true;
		}
		else
		{
			value_pos = tb.Value - tb.Minimum;
			mouse_value = false;
		}
		Rectangle clientRectangle = tb.ClientRectangle;
		Brush br_thumb = ((!tb.Enabled) ? ResPool.GetHatchBrush(HatchStyle.Percent50, ColorControlLightLight, ColorControlLight) : ((!tb.thumb_pressed) ? SystemBrushes.Control : ResPool.GetHatchBrush(HatchStyle.Percent50, ColorControlLight, ColorControl)));
		if (tb.BackColor.ToArgb() == DefaultControlBackColor.ToArgb())
		{
			dc.FillRectangle(SystemBrushes.Control, clip_rectangle);
		}
		else
		{
			dc.FillRectangle(ResPool.GetSolidBrush(tb.BackColor), clip_rectangle);
		}
		if (tb.Focused)
		{
			CPDrawFocusRectangle(dc, clientRectangle, tb.ForeColor, tb.BackColor);
		}
		if (tb.Orientation == Orientation.Vertical)
		{
			DrawTrackBar_Vertical(dc, clip_rectangle, tb, ref thumb_pos, ref thumb_area, br_thumb, ticks, value_pos, mouse_value);
		}
		else
		{
			DrawTrackBar_Horizontal(dc, clip_rectangle, tb, ref thumb_pos, ref thumb_area, br_thumb, ticks, value_pos, mouse_value);
		}
		tb.ThumbPos = thumb_pos;
		tb.ThumbArea = thumb_area;
	}

	public override void UpDownBaseDrawButton(Graphics g, Rectangle bounds, bool top, PushButtonState state)
	{
		ControlPaint.DrawScrollButton(g, bounds, (!top) ? ScrollButton.Down : ScrollButton.Min, (state == PushButtonState.Pressed) ? ButtonState.Pushed : ButtonState.Normal);
	}

	public override void TreeViewDrawNodePlusMinus(TreeView treeView, TreeNode node, Graphics dc, int x, int middle)
	{
		int num = treeView.ActualItemHeight - 2;
		dc.FillRectangle(ResPool.GetSolidBrush(treeView.BackColor), x + 4 - num / 2, node.GetY() + 1, num, num);
		dc.DrawRectangle(SystemPens.ControlDarkDark, x, middle - 4, 8, 8);
		if (node.IsExpanded)
		{
			dc.DrawLine(SystemPens.ControlDarkDark, x + 2, middle, x + 6, middle);
			return;
		}
		dc.DrawLine(SystemPens.ControlDarkDark, x + 2, middle, x + 6, middle);
		dc.DrawLine(SystemPens.ControlDarkDark, x + 4, middle - 2, x + 4, middle + 2);
	}

	public override int ManagedWindowTitleBarHeight(InternalWindowManager wm)
	{
		if (wm.IsToolWindow && !wm.IsMinimized)
		{
			return SystemInformation.ToolWindowCaptionHeight;
		}
		if (wm.Form.FormBorderStyle == FormBorderStyle.None)
		{
			return 0;
		}
		return SystemInformation.CaptionHeight;
	}

	public override int ManagedWindowBorderWidth(InternalWindowManager wm)
	{
		if ((wm.IsToolWindow && wm.form.FormBorderStyle == FormBorderStyle.FixedToolWindow) || wm.IsMinimized)
		{
			return 3;
		}
		return 4;
	}

	public override int ManagedWindowIconWidth(InternalWindowManager wm)
	{
		return ManagedWindowTitleBarHeight(wm) - 5;
	}

	public override void ManagedWindowSetButtonLocations(InternalWindowManager wm)
	{
		TitleButtons titleButtons = wm.TitleButtons;
		Form form = wm.form;
		titleButtons.HelpButton.Visible = form.HelpButton;
		foreach (TitleButton item in titleButtons)
		{
			item.Visible = false;
		}
		switch (form.FormBorderStyle)
		{
		case FormBorderStyle.None:
			if (form.WindowState != 0)
			{
				goto case FormBorderStyle.FixedSingle;
			}
			break;
		case FormBorderStyle.FixedToolWindow:
		case FormBorderStyle.SizableToolWindow:
			titleButtons.CloseButton.Visible = true;
			if (form.WindowState != 0)
			{
				goto case FormBorderStyle.FixedSingle;
			}
			break;
		case FormBorderStyle.FixedSingle:
		case FormBorderStyle.Fixed3D:
		case FormBorderStyle.FixedDialog:
		case FormBorderStyle.Sizable:
			switch (form.WindowState)
			{
			case FormWindowState.Normal:
				titleButtons.MinimizeButton.Visible = true;
				titleButtons.MaximizeButton.Visible = true;
				titleButtons.RestoreButton.Visible = false;
				break;
			case FormWindowState.Maximized:
				titleButtons.MinimizeButton.Visible = true;
				titleButtons.MaximizeButton.Visible = false;
				titleButtons.RestoreButton.Visible = true;
				break;
			case FormWindowState.Minimized:
				titleButtons.MinimizeButton.Visible = false;
				titleButtons.MaximizeButton.Visible = true;
				titleButtons.RestoreButton.Visible = true;
				break;
			}
			titleButtons.CloseButton.Visible = true;
			break;
		}
		if (!form.MinimizeBox && !form.MaximizeBox)
		{
			titleButtons.MinimizeButton.Visible = false;
			titleButtons.MaximizeButton.Visible = false;
		}
		else if (!form.MinimizeBox)
		{
			titleButtons.MinimizeButton.State = ButtonState.Inactive;
		}
		else if (!form.MaximizeBox)
		{
			titleButtons.MaximizeButton.State = ButtonState.Inactive;
		}
		int num = ManagedWindowBorderWidth(wm);
		Size size = ManagedWindowButtonSize(wm);
		int width = size.Width;
		int height = size.Height;
		int y = num + 2;
		int num2 = form.Width - num - width - 2;
		if ((!wm.IsToolWindow || wm.IsMinimized) && wm.HasBorders)
		{
			titleButtons.CloseButton.Rectangle = new Rectangle(num2, y, width, height);
			num2 -= 2 + width;
			if (titleButtons.MaximizeButton.Visible)
			{
				titleButtons.MaximizeButton.Rectangle = new Rectangle(num2, y, width, height);
				num2 -= 2 + width;
			}
			if (titleButtons.RestoreButton.Visible)
			{
				titleButtons.RestoreButton.Rectangle = new Rectangle(num2, y, width, height);
				num2 -= 2 + width;
			}
			titleButtons.MinimizeButton.Rectangle = new Rectangle(num2, y, width, height);
			num2 -= 2 + width;
		}
		else if (wm.IsToolWindow)
		{
			titleButtons.CloseButton.Rectangle = new Rectangle(num2, y, width, height);
			num2 -= 2 + width;
		}
	}

	protected virtual Rectangle ManagedWindowDrawTitleBarAndBorders(Graphics dc, Rectangle clip, InternalWindowManager wm)
	{
		Form form = wm.Form;
		int num = ManagedWindowTitleBarHeight(wm);
		int num2 = ManagedWindowBorderWidth(wm);
		Color color = Color.FromArgb(255, 10, 36, 106);
		Color color2 = Color.FromArgb(255, 166, 202, 240);
		Color color3 = ThemeEngine.Current.ColorControlDark;
		Color color4 = Color.FromArgb(255, 192, 192, 192);
		Pen pen = ResPool.GetPen(ColorControl);
		Rectangle rectangle = new Rectangle(0, 0, form.Width, form.Height);
		ControlPaint.DrawBorder3D(dc, rectangle, Border3DStyle.Raised);
		rectangle = new Rectangle(2, 2, form.Width - 5, form.Height - 5);
		for (int i = 2; i < num2; i++)
		{
			dc.DrawRectangle(pen, rectangle);
			rectangle.Inflate(-1, -1);
		}
		bool flag = false;
		if (wm.Form.Parent != null && wm.Form.Parent is Form)
		{
			flag = false;
		}
		else if (wm.IsActive && !wm.IsMaximized)
		{
			flag = true;
		}
		if (flag)
		{
			color3 = color;
			color4 = color2;
		}
		Rectangle rectangle2 = new Rectangle(num2, num2, form.Width - num2 * 2, num - 1);
		if (rectangle2.Width > 0 && rectangle2.Height > 0)
		{
			using LinearGradientBrush brush = new LinearGradientBrush(rectangle2, color3, color4, LinearGradientMode.Horizontal);
			dc.FillRectangle(brush, rectangle2);
		}
		if (!wm.IsMinimized)
		{
			dc.DrawLine(ResPool.GetPen(SystemColors.Control), num2, num + num2 - 1, form.Width - num2 - 1, num + num2 - 1);
		}
		return rectangle2;
	}

	public override void DrawManagedWindowDecorations(Graphics dc, Rectangle clip, InternalWindowManager wm)
	{
		Rectangle rectangle = ManagedWindowDrawTitleBarAndBorders(dc, clip, wm);
		Form form = wm.Form;
		if (wm.ShowIcon)
		{
			Rectangle targetRect = ManagedWindowGetTitleBarIconArea(wm);
			if (targetRect.IntersectsWith(clip))
			{
				dc.DrawIcon(form.Icon, targetRect);
			}
			rectangle.Width -= targetRect.Right + 2 - rectangle.X;
			rectangle.X = targetRect.Right + 2;
		}
		TitleButton[] allButtons = wm.TitleButtons.AllButtons;
		foreach (TitleButton button in allButtons)
		{
			rectangle.Width -= Math.Max(0, rectangle.Right - DrawTitleButton(dc, button, clip, form));
		}
		rectangle.Width -= 3;
		string text = form.Text;
		text = text.Replace(Environment.NewLine, string.Empty);
		if (text != null && text != string.Empty)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.FormatFlags = StringFormatFlags.NoWrap;
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			stringFormat.LineAlignment = StringAlignment.Center;
			if (rectangle.IntersectsWith(clip))
			{
				dc.DrawString(text, WindowBorderFont, ThemeEngine.Current.ResPool.GetSolidBrush(Color.White), rectangle, stringFormat);
			}
		}
	}

	public override Size ManagedWindowButtonSize(InternalWindowManager wm)
	{
		int num = ManagedWindowTitleBarHeight(wm);
		if (!wm.IsMaximized && !wm.IsMinimized)
		{
			if (wm.IsToolWindow)
			{
				return new Size(SystemInformation.ToolWindowCaptionButtonSize.Width - 2, num - 5);
			}
			if (wm.Form.FormBorderStyle == FormBorderStyle.None)
			{
				return Size.Empty;
			}
		}
		else
		{
			num = SystemInformation.CaptionHeight;
		}
		return new Size(SystemInformation.CaptionButtonSize.Width - 2, num - 5);
	}

	private int DrawTitleButton(Graphics dc, TitleButton button, Rectangle clip, Form form)
	{
		if (!button.Visible)
		{
			return int.MaxValue;
		}
		if (button.Rectangle.IntersectsWith(clip))
		{
			ManagedWindowDrawTitleButton(dc, button, clip, form);
		}
		return button.Rectangle.Left;
	}

	protected virtual void ManagedWindowDrawTitleButton(Graphics dc, TitleButton button, Rectangle clip, Form form)
	{
		dc.FillRectangle(SystemBrushes.Control, button.Rectangle);
		ControlPaint.DrawCaptionButton(dc, button.Rectangle, button.Caption, button.State);
	}

	public override Rectangle ManagedWindowGetTitleBarIconArea(InternalWindowManager wm)
	{
		int num = ManagedWindowBorderWidth(wm);
		return new Rectangle(num + 3, num + 2, wm.IconWidth, wm.IconWidth);
	}

	public override Size ManagedWindowGetMenuButtonSize(InternalWindowManager wm)
	{
		Size menuButtonSize = SystemInformation.MenuButtonSize;
		menuButtonSize.Width -= 2;
		menuButtonSize.Height -= 4;
		return menuButtonSize;
	}

	public override bool ManagedWindowTitleButtonHasHotElementStyle(TitleButton button, Form form)
	{
		return false;
	}

	public override void ManagedWindowDrawMenuButton(Graphics dc, TitleButton button, Rectangle clip, InternalWindowManager wm)
	{
		dc.FillRectangle(SystemBrushes.Control, button.Rectangle);
		ControlPaint.DrawCaptionButton(dc, button.Rectangle, button.Caption, button.State);
	}

	public override void ManagedWindowOnSizeInitializedOrChanged(Form form)
	{
	}

	public override void CPDrawBorder(Graphics graphics, Rectangle bounds, Color leftColor, int leftWidth, ButtonBorderStyle leftStyle, Color topColor, int topWidth, ButtonBorderStyle topStyle, Color rightColor, int rightWidth, ButtonBorderStyle rightStyle, Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle)
	{
		DrawBorderInternal(graphics, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 1, leftWidth, leftColor, leftStyle, Border3DSide.Left);
		DrawBorderInternal(graphics, bounds.Left, bounds.Top, bounds.Right - 1, bounds.Top, topWidth, topColor, topStyle, Border3DSide.Top);
		DrawBorderInternal(graphics, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1, rightWidth, rightColor, rightStyle, Border3DSide.Right);
		DrawBorderInternal(graphics, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1, bottomWidth, bottomColor, bottomStyle, Border3DSide.Bottom);
	}

	public override void CPDrawBorder(Graphics graphics, RectangleF bounds, Color leftColor, int leftWidth, ButtonBorderStyle leftStyle, Color topColor, int topWidth, ButtonBorderStyle topStyle, Color rightColor, int rightWidth, ButtonBorderStyle rightStyle, Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle)
	{
		DrawBorderInternal(graphics, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 1f, leftWidth, leftColor, leftStyle, Border3DSide.Left);
		DrawBorderInternal(graphics, bounds.Left, bounds.Top, bounds.Right - 1f, bounds.Top, topWidth, topColor, topStyle, Border3DSide.Top);
		DrawBorderInternal(graphics, bounds.Right - 1f, bounds.Top, bounds.Right - 1f, bounds.Bottom - 1f, rightWidth, rightColor, rightStyle, Border3DSide.Right);
		DrawBorderInternal(graphics, bounds.Left, bounds.Bottom - 1f, bounds.Right - 1f, bounds.Bottom - 1f, bottomWidth, bottomColor, bottomStyle, Border3DSide.Bottom);
	}

	public override void CPDrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style, Border3DSide sides)
	{
		CPDrawBorder3D(graphics, rectangle, style, sides, ColorControl);
	}

	public override void CPDrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style, Border3DSide sides, Color control_color)
	{
		Rectangle rect = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		bool flag = ((control_color.ToArgb() == ColorControl.ToArgb()) ? true : false);
		if ((style & Border3DStyle.Adjust) != 0)
		{
			rect.Y -= 2;
			rect.X -= 2;
			rect.Width += 4;
			rect.Height += 4;
		}
		Pen pen3;
		Pen pen2;
		Pen pen;
		Pen pen4 = (pen3 = (pen2 = (pen = ((!flag) ? ResPool.GetPen(control_color) : SystemPens.Control))));
		CPColor cPColor = CPColor.Empty;
		if (!flag)
		{
			cPColor = ResPool.GetCPColor(control_color);
		}
		switch (style)
		{
		case Border3DStyle.Raised:
			pen3 = ((!flag) ? ResPool.GetPen(cPColor.LightLight) : SystemPens.ControlLightLight);
			pen2 = ((!flag) ? ResPool.GetPen(cPColor.DarkDark) : SystemPens.ControlDarkDark);
			pen = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark);
			break;
		case Border3DStyle.Sunken:
			pen4 = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark);
			pen3 = ((!flag) ? ResPool.GetPen(cPColor.DarkDark) : SystemPens.ControlDarkDark);
			pen2 = ((!flag) ? ResPool.GetPen(cPColor.LightLight) : SystemPens.ControlLightLight);
			break;
		case Border3DStyle.Etched:
			pen4 = (pen = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark));
			pen3 = (pen2 = ((!flag) ? ResPool.GetPen(cPColor.LightLight) : SystemPens.ControlLightLight));
			break;
		case Border3DStyle.RaisedOuter:
			pen2 = ((!flag) ? ResPool.GetPen(cPColor.DarkDark) : SystemPens.ControlDarkDark);
			break;
		case Border3DStyle.SunkenOuter:
			pen4 = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark);
			pen2 = ((!flag) ? ResPool.GetPen(cPColor.LightLight) : SystemPens.ControlLightLight);
			break;
		case Border3DStyle.RaisedInner:
			pen4 = ((!flag) ? ResPool.GetPen(cPColor.LightLight) : SystemPens.ControlLightLight);
			pen2 = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark);
			break;
		case Border3DStyle.SunkenInner:
			pen4 = ((!flag) ? ResPool.GetPen(cPColor.DarkDark) : SystemPens.ControlDarkDark);
			break;
		case Border3DStyle.Flat:
			pen4 = (pen2 = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark));
			break;
		case Border3DStyle.Bump:
			pen3 = (pen2 = ((!flag) ? ResPool.GetPen(cPColor.DarkDark) : SystemPens.ControlDarkDark));
			break;
		}
		bool flag2 = style != Border3DStyle.RaisedOuter && style != Border3DStyle.SunkenOuter;
		if ((sides & Border3DSide.Middle) != 0)
		{
			Brush brush = ((!flag) ? ResPool.GetSolidBrush(control_color) : SystemBrushes.Control);
			graphics.FillRectangle(brush, rect);
		}
		if ((sides & Border3DSide.Left) != 0)
		{
			graphics.DrawLine(pen4, rect.Left, rect.Bottom - 2, rect.Left, rect.Top);
			if (rect.Width > 2 && flag2)
			{
				graphics.DrawLine(pen3, rect.Left + 1, rect.Bottom - 2, rect.Left + 1, rect.Top);
			}
		}
		if ((sides & Border3DSide.Top) != 0)
		{
			graphics.DrawLine(pen4, rect.Left, rect.Top, rect.Right - 2, rect.Top);
			if (rect.Height > 2 && flag2)
			{
				graphics.DrawLine(pen3, rect.Left + 1, rect.Top + 1, rect.Right - 3, rect.Top + 1);
			}
		}
		if ((sides & Border3DSide.Right) != 0)
		{
			graphics.DrawLine(pen2, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 1);
			if (rect.Width > 3 && flag2)
			{
				graphics.DrawLine(pen, rect.Right - 2, rect.Top + 1, rect.Right - 2, rect.Bottom - 2);
			}
		}
		if ((sides & Border3DSide.Bottom) != 0)
		{
			graphics.DrawLine(pen2, rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
			if (rect.Height > 3 && flag2)
			{
				graphics.DrawLine(pen, rect.Left + 1, rect.Bottom - 2, rect.Right - 2, rect.Bottom - 2);
			}
		}
	}

	public override void CPDrawButton(Graphics dc, Rectangle rectangle, ButtonState state)
	{
		CPDrawButtonInternal(dc, rectangle, state, SystemPens.ControlDarkDark, SystemPens.ControlDark, SystemPens.ControlLight);
	}

	private void CPDrawButtonInternal(Graphics dc, Rectangle rectangle, ButtonState state, Pen DarkPen, Pen NormalPen, Pen LightPen)
	{
		dc.FillRectangle(ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl), rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2);
		if ((state & ButtonState.All) == ButtonState.All || ((state & ButtonState.Checked) == ButtonState.Checked && (state & ButtonState.Flat) == ButtonState.Flat))
		{
			dc.FillRectangle(ResPool.GetHatchBrush(HatchStyle.Percent50, ColorControlLight, ColorControl), rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 4, rectangle.Height - 4);
			dc.DrawRectangle(SystemPens.ControlDark, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
		}
		else if ((state & ButtonState.Flat) == ButtonState.Flat)
		{
			dc.DrawRectangle(SystemPens.ControlDark, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
		}
		else if ((state & ButtonState.Checked) == ButtonState.Checked)
		{
			dc.FillRectangle(ResPool.GetHatchBrush(HatchStyle.Percent50, ColorControlLight, ColorControl), rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 4, rectangle.Height - 4);
			Pen pen = DarkPen;
			dc.DrawLine(pen, rectangle.X, rectangle.Y, rectangle.X, rectangle.Bottom - 2);
			dc.DrawLine(pen, rectangle.X + 1, rectangle.Y, rectangle.Right - 2, rectangle.Y);
			pen = NormalPen;
			dc.DrawLine(pen, rectangle.X + 1, rectangle.Y + 1, rectangle.X + 1, rectangle.Bottom - 3);
			dc.DrawLine(pen, rectangle.X + 2, rectangle.Y + 1, rectangle.Right - 3, rectangle.Y + 1);
			pen = LightPen;
			dc.DrawLine(pen, rectangle.X, rectangle.Bottom - 1, rectangle.Right - 2, rectangle.Bottom - 1);
			dc.DrawLine(pen, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1, rectangle.Bottom - 1);
		}
		else if ((state & ButtonState.Pushed) == ButtonState.Pushed)
		{
			Pen pen2 = DarkPen;
			dc.DrawLine(pen2, rectangle.X, rectangle.Y, rectangle.X, rectangle.Bottom - 2);
			dc.DrawLine(pen2, rectangle.X + 1, rectangle.Y, rectangle.Right - 2, rectangle.Y);
			pen2 = NormalPen;
			dc.DrawLine(pen2, rectangle.X + 1, rectangle.Y + 1, rectangle.X + 1, rectangle.Bottom - 3);
			dc.DrawLine(pen2, rectangle.X + 2, rectangle.Y + 1, rectangle.Right - 3, rectangle.Y + 1);
			pen2 = LightPen;
			dc.DrawLine(pen2, rectangle.X, rectangle.Bottom - 1, rectangle.Right - 2, rectangle.Bottom - 1);
			dc.DrawLine(pen2, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1, rectangle.Bottom - 1);
		}
		else if ((state & ButtonState.Inactive) == ButtonState.Inactive || true)
		{
			Pen pen3 = LightPen;
			dc.DrawLine(pen3, rectangle.X, rectangle.Y, rectangle.Right - 2, rectangle.Y);
			dc.DrawLine(pen3, rectangle.X, rectangle.Y, rectangle.X, rectangle.Bottom - 2);
			pen3 = NormalPen;
			dc.DrawLine(pen3, rectangle.X + 1, rectangle.Bottom - 2, rectangle.Right - 2, rectangle.Bottom - 2);
			dc.DrawLine(pen3, rectangle.Right - 2, rectangle.Y + 1, rectangle.Right - 2, rectangle.Bottom - 3);
			pen3 = DarkPen;
			dc.DrawLine(pen3, rectangle.X, rectangle.Bottom - 1, rectangle.Right - 1, rectangle.Bottom - 1);
			dc.DrawLine(pen3, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1, rectangle.Bottom - 2);
		}
	}

	public override void CPDrawCaptionButton(Graphics graphics, Rectangle rectangle, CaptionButton button, ButtonState state)
	{
		CPDrawButtonInternal(graphics, rectangle, state, SystemPens.ControlDarkDark, SystemPens.ControlDark, SystemPens.ControlLightLight);
		Rectangle captionRect = ((rectangle.Width >= rectangle.Height) ? new Rectangle(rectangle.X + rectangle.Width / 2 - rectangle.Height / 2 + 1, rectangle.Y + 1, rectangle.Height - 4, rectangle.Height - 4) : new Rectangle(rectangle.X + 1, rectangle.Y + rectangle.Height / 2 - rectangle.Width / 2 + 1, rectangle.Width - 4, rectangle.Width - 4));
		if ((state & ButtonState.Pushed) != 0)
		{
			captionRect = new Rectangle(rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 3, rectangle.Height - 3);
		}
		int num = Math.Max(1, captionRect.Width / 7);
		switch (button)
		{
		case CaptionButton.Close:
			if ((state & ButtonState.Inactive) != 0)
			{
				Pen sizedPen = ResPool.GetSizedPen(ColorControlLight, num);
				DrawCaptionHelper(graphics, ColorControlLight, sizedPen, num, 1, captionRect, button);
				sizedPen = ResPool.GetSizedPen(ColorControlDark, num);
				DrawCaptionHelper(graphics, ColorControlDark, sizedPen, num, 0, captionRect, button);
			}
			else
			{
				Pen sizedPen = ResPool.GetSizedPen(ColorControlText, num);
				DrawCaptionHelper(graphics, ColorControlText, sizedPen, num, 0, captionRect, button);
			}
			break;
		case CaptionButton.Minimize:
		case CaptionButton.Maximize:
		case CaptionButton.Restore:
		case CaptionButton.Help:
			if ((state & ButtonState.Inactive) != 0)
			{
				DrawCaptionHelper(graphics, ColorControlLight, SystemPens.ControlLightLight, num, 1, captionRect, button);
				DrawCaptionHelper(graphics, ColorControlDark, SystemPens.ControlDark, num, 0, captionRect, button);
			}
			else
			{
				DrawCaptionHelper(graphics, ColorControlText, SystemPens.ControlText, num, 0, captionRect, button);
			}
			break;
		}
	}

	public override void CPDrawCheckBox(Graphics dc, Rectangle rectangle, ButtonState state)
	{
		Pen pen = Pens.Black;
		Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		if ((state & ButtonState.All) == ButtonState.All)
		{
			rectangle2.Width -= 2;
			rectangle2.Height -= 2;
			dc.FillRectangle(SystemBrushes.Control, rectangle2.X, rectangle2.Y, rectangle2.Width - 1, rectangle2.Height - 1);
			dc.DrawRectangle(SystemPens.ControlDark, rectangle2.X, rectangle2.Y, rectangle2.Width - 1, rectangle2.Height - 1);
			pen = SystemPens.ControlDark;
		}
		else if ((state & ButtonState.Flat) == ButtonState.Flat)
		{
			rectangle2.Width -= 2;
			rectangle2.Height -= 2;
			if ((state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				dc.FillRectangle(SystemBrushes.ControlLight, rectangle2.X, rectangle2.Y, rectangle2.Width - 1, rectangle2.Height - 1);
			}
			else
			{
				dc.FillRectangle(Brushes.White, rectangle2.X, rectangle2.Y, rectangle2.Width - 1, rectangle2.Height - 1);
			}
			dc.DrawRectangle(SystemPens.ControlDark, rectangle2.X, rectangle2.Y, rectangle2.Width - 1, rectangle2.Height - 1);
		}
		else
		{
			rectangle2.Width--;
			rectangle2.Height--;
			int num = ((rectangle2.Height <= rectangle2.Width) ? rectangle2.Height : rectangle2.Width);
			int x = Math.Max(0, rectangle2.X + rectangle2.Width / 2 - num / 2);
			int y = Math.Max(0, rectangle2.Y + rectangle2.Height / 2 - num / 2);
			Rectangle rectangle3 = new Rectangle(x, y, num, num);
			if ((state & ButtonState.Pushed) == ButtonState.Pushed || (state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				dc.FillRectangle(ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl), rectangle3.X + 2, rectangle3.Y + 2, rectangle3.Width - 3, rectangle3.Height - 3);
			}
			else
			{
				dc.FillRectangle(SystemBrushes.ControlLightLight, rectangle3.X + 2, rectangle3.Y + 2, rectangle3.Width - 3, rectangle3.Height - 3);
			}
			Pen controlDark = SystemPens.ControlDark;
			dc.DrawLine(controlDark, rectangle3.X, rectangle3.Y, rectangle3.X, rectangle3.Bottom - 1);
			dc.DrawLine(controlDark, rectangle3.X + 1, rectangle3.Y, rectangle3.Right - 1, rectangle3.Y);
			controlDark = SystemPens.ControlDarkDark;
			dc.DrawLine(controlDark, rectangle3.X + 1, rectangle3.Y + 1, rectangle3.X + 1, rectangle3.Bottom - 2);
			dc.DrawLine(controlDark, rectangle3.X + 2, rectangle3.Y + 1, rectangle3.Right - 2, rectangle3.Y + 1);
			controlDark = SystemPens.ControlLightLight;
			dc.DrawLine(controlDark, rectangle3.Right, rectangle3.Y, rectangle3.Right, rectangle3.Bottom);
			dc.DrawLine(controlDark, rectangle3.X, rectangle3.Bottom, rectangle3.Right, rectangle3.Bottom);
			using (Pen pen2 = new Pen(ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl)))
			{
				dc.DrawLine(pen2, rectangle3.X + 1, rectangle3.Bottom - 1, rectangle3.Right - 1, rectangle3.Bottom - 1);
				dc.DrawLine(pen2, rectangle3.Right - 1, rectangle3.Y + 1, rectangle3.Right - 1, rectangle3.Bottom - 1);
			}
			if ((state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				pen = SystemPens.ControlDark;
			}
		}
		if ((state & ButtonState.Checked) != ButtonState.Checked)
		{
			return;
		}
		int num2 = ((rectangle2.Height <= rectangle2.Width) ? (rectangle2.Height / 2) : (rectangle2.Width / 2));
		if (num2 < 7)
		{
			int num3 = Math.Max(3, num2 / 3);
			int num4 = Math.Max(1, num2 / 9);
			Rectangle rectangle4 = new Rectangle(rectangle2.X + rectangle2.Width / 2 - (int)Math.Ceiling((float)num2 / 2f) - 1, rectangle2.Y + rectangle2.Height / 2 - num2 / 2 - 1, num2, num2);
			for (int i = 0; i < num3; i++)
			{
				dc.DrawLine(pen, rectangle4.Left + num3 / 2, rectangle4.Top + num3 + i, rectangle4.Left + num3 / 2 + 2 * num4, rectangle4.Top + num3 + 2 * num4 + i);
				dc.DrawLine(pen, rectangle4.Left + num3 / 2 + 2 * num4, rectangle4.Top + num3 + 2 * num4 + i, rectangle4.Left + num3 / 2 + 6 * num4, rectangle4.Top + num3 - 2 * num4 + i);
			}
			return;
		}
		int num5 = Math.Max(3, num2 / 3) + 1;
		int num6 = rectangle2.Width / 2;
		int num7 = rectangle2.Height / 2;
		Rectangle rectangle5 = new Rectangle(rectangle2.X + num6 - num2 / 2 - 1, rectangle2.Y + num7 - num2 / 2, num2, num2);
		int num8 = num2 / 3;
		int num9 = num2 - num8 - 1;
		for (int j = 0; j < num5; j++)
		{
			dc.DrawLine(pen, rectangle5.X, rectangle5.Bottom - 1 - num8 - j, rectangle5.X + num8, rectangle5.Bottom - 1 - j);
			dc.DrawLine(pen, rectangle5.X + num8, rectangle5.Bottom - 1 - j, rectangle5.Right - 1, rectangle5.Bottom - j - 1 - num9);
		}
	}

	public override void CPDrawComboButton(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		Point[] array = new Point[3];
		if ((state & ButtonState.Checked) != 0)
		{
			graphics.FillRectangle(ResPool.GetHatchBrush(HatchStyle.Percent50, ColorControlLightLight, ColorControlLight), rectangle);
		}
		if ((state & ButtonState.Flat) != 0)
		{
			ControlPaint.DrawBorder(graphics, rectangle, ColorControlDark, ButtonBorderStyle.Solid);
		}
		else if ((state & (ButtonState.Pushed | ButtonState.Checked)) != 0)
		{
			graphics.DrawRectangle(rect: new Rectangle(rectangle.X, rectangle.Y, Math.Max(rectangle.Width - 1, 0), Math.Max(rectangle.Height - 1, 0)), pen: SystemPens.ControlDark);
		}
		else
		{
			CPDrawBorder3D(graphics, rectangle, Border3DStyle.Raised, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, ColorControl);
		}
		Rectangle rectangle2 = new Rectangle(rectangle.X + rectangle.Width / 4, rectangle.Y + rectangle.Height / 4, rectangle.Width / 2, rectangle.Height / 2);
		int x = rectangle2.Left + rectangle2.Width / 2;
		int num = rectangle2.Top + rectangle2.Height / 2;
		int num2 = Math.Max(1, rectangle2.Width / 8);
		int num3 = Math.Max(1, rectangle2.Height / 8);
		if ((state & ButtonState.Pushed) != 0)
		{
			num2++;
			num3++;
		}
		rectangle2.Y -= num3;
		num -= num3;
		Point point = new Point(rectangle2.Left, num);
		Point point2 = new Point(rectangle2.Right, num);
		Point point3 = new Point(x, rectangle2.Bottom);
		array[0] = point;
		array[1] = point2;
		array[2] = point3;
		if ((state & ButtonState.Inactive) != 0)
		{
			array[0].X++;
			array[0].Y++;
			array[1].X++;
			array[1].Y++;
			array[2].X++;
			array[2].Y++;
			graphics.FillPolygon(SystemBrushes.ControlLightLight, array, FillMode.Winding);
			array[0] = point;
			array[1] = point2;
			array[2] = point3;
			graphics.FillPolygon(SystemBrushes.ControlDark, array, FillMode.Winding);
		}
		else
		{
			graphics.FillPolygon(SystemBrushes.ControlText, array, FillMode.Winding);
		}
	}

	public override void CPDrawContainerGrabHandle(Graphics graphics, Rectangle bounds)
	{
		Pen black = Pens.Black;
		Rectangle rect = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
		graphics.FillRectangle(SystemBrushes.ControlLightLight, rect);
		graphics.DrawRectangle(black, rect);
		int num = rect.X + rect.Width / 2;
		int num2 = rect.Y + rect.Height / 2;
		graphics.DrawLine(black, num, rect.Y + 2, num, rect.Bottom - 2);
		graphics.DrawLine(black, rect.X + 2, num2, rect.Right - 2, num2);
		graphics.DrawLine(black, num - 1, rect.Y + 3, num + 1, rect.Y + 3);
		graphics.DrawLine(black, num - 1, rect.Bottom - 3, num + 1, rect.Bottom - 3);
		graphics.DrawLine(black, rect.X + 3, num2 - 1, rect.X + 3, num2 + 1);
		graphics.DrawLine(black, rect.Right - 3, num2 - 1, rect.Right - 3, num2 + 1);
	}

	public virtual void DrawFlatStyleFocusRectangle(Graphics graphics, Rectangle rectangle, ButtonBase button, Color foreColor, Color backColor)
	{
		Rectangle rect = new Rectangle(rectangle.X, rectangle.Y, Math.Max(rectangle.Width - 1, 0), Math.Max(rectangle.Height - 1, 0));
		Color color = foreColor;
		if (button.FlatStyle == FlatStyle.Popup && !button.is_pressed)
		{
			color = ((backColor.ToArgb() != ColorControl.ToArgb()) ? ColorControlText : ControlPaint.Dark(ColorControl));
		}
		graphics.DrawRectangle(ResPool.GetPen(color), rect);
		if (button.FlatStyle == FlatStyle.Popup)
		{
			DrawInnerFocusRectangle(graphics, Rectangle.Inflate(rectangle, -4, -4), backColor);
			return;
		}
		Pen pen = ResPool.GetPen(ControlPaint.LightLight(backColor));
		graphics.DrawRectangle(pen, Rectangle.Inflate(rect, -4, -4));
	}

	public virtual void DrawInnerFocusRectangle(Graphics graphics, Rectangle rectangle, Color backColor)
	{
		Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y, Math.Max(rectangle.Width - 1, 0), Math.Max(rectangle.Height - 1, 0));
		CPDrawFocusRectangle(graphics, rectangle2, Color.Wheat, backColor);
	}

	public override void CPDrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color foreColor, Color backColor)
	{
		Rectangle rect = rectangle;
		if ((double)backColor.GetBrightness() >= 0.5)
		{
			foreColor = Color.Transparent;
			backColor = Color.Black;
		}
		else
		{
			backColor = Color.FromArgb(Math.Abs(backColor.R - 255), Math.Abs(backColor.G - 255), Math.Abs(backColor.B - 255));
			foreColor = Color.Black;
		}
		HatchBrush hatchBrush = ResPool.GetHatchBrush(HatchStyle.Percent50, backColor, foreColor);
		Pen pen = new Pen(hatchBrush, 1f);
		rect.Width--;
		rect.Height--;
		graphics.DrawRectangle(pen, rect);
		pen.Dispose();
	}

	public override void CPDrawGrabHandle(Graphics graphics, Rectangle rectangle, bool primary, bool enabled)
	{
		Pen pen;
		Brush brush;
		if (primary)
		{
			pen = Pens.Black;
			brush = ((!enabled) ? SystemBrushes.Control : Brushes.White);
		}
		else
		{
			pen = Pens.White;
			brush = ((!enabled) ? SystemBrushes.Control : Brushes.Black);
		}
		graphics.FillRectangle(brush, rectangle);
		graphics.DrawRectangle(pen, rectangle);
	}

	public override void CPDrawGrid(Graphics graphics, Rectangle area, Size pixelsBetweenDots, Color backColor)
	{
		ControlPaint.Color2HBS(backColor, out var _, out var l, out var _);
		Color color = ((l <= 127) ? Color.White : Color.Black);
		using Pen pen = new Pen(color);
		pen.DashPattern = new float[2]
		{
			1f,
			pixelsBetweenDots.Width - 1
		};
		for (int i = area.Top; i < area.Bottom; i += pixelsBetweenDots.Height)
		{
			graphics.DrawLine(pen, area.X, i, area.Right - 1, i);
		}
	}

	public override void CPDrawImageDisabled(Graphics graphics, Image image, int x, int y, Color background)
	{
		if (imagedisabled_attributes == null)
		{
			imagedisabled_attributes = new ImageAttributes();
			ColorMatrix colorMatrix = new ColorMatrix(new float[6][]
			{
				new float[5] { 0.2f, 0.2f, 0.2f, 0f, 0f },
				new float[5] { 0.41f, 0.41f, 0.41f, 0f, 0f },
				new float[5] { 0.11f, 0.11f, 0.11f, 0f, 0f },
				new float[6] { 0.15f, 0.15f, 0.15f, 1f, 0f, 0f },
				new float[6] { 0.15f, 0.15f, 0.15f, 0f, 1f, 0f },
				new float[6] { 0.15f, 0.15f, 0.15f, 0f, 0f, 1f }
			});
			imagedisabled_attributes.SetColorMatrix(colorMatrix);
		}
		graphics.DrawImage(image, new Rectangle(x, y, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imagedisabled_attributes);
	}

	public override void CPDrawLockedFrame(Graphics graphics, Rectangle rectangle, bool primary)
	{
		Pen sizedPen;
		Pen pen;
		if (primary)
		{
			sizedPen = ResPool.GetSizedPen(Color.White, 2);
			pen = ResPool.GetPen(Color.Black);
		}
		else
		{
			sizedPen = ResPool.GetSizedPen(Color.Black, 2);
			pen = ResPool.GetPen(Color.White);
		}
		sizedPen.Alignment = PenAlignment.Inset;
		pen.Alignment = PenAlignment.Inset;
		graphics.DrawRectangle(sizedPen, rectangle);
		graphics.DrawRectangle(pen, rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 5, rectangle.Height - 5);
	}

	public override void CPDrawMenuGlyph(Graphics graphics, Rectangle rectangle, MenuGlyph glyph, Color color, Color backColor)
	{
		if (backColor != Color.Empty)
		{
			graphics.FillRectangle(ResPool.GetSolidBrush(backColor), rectangle);
		}
		Brush solidBrush = ResPool.GetSolidBrush(color);
		switch (glyph)
		{
		case MenuGlyph.Arrow:
		{
			float num4 = (float)rectangle.Height * 0.7f;
			float num5 = num4 / 2f;
			PointF pointF = new PointF((float)rectangle.X + ((float)rectangle.Width - num5) / 2f, (float)rectangle.Y + (float)rectangle.Height / 2f);
			PointF[] array = new PointF[3];
			array[0].X = pointF.X;
			array[0].Y = pointF.Y - num4 / 2f;
			array[1].X = pointF.X;
			array[1].Y = pointF.Y + num4 / 2f;
			array[2].X = pointF.X + num5 + 0.1f;
			array[2].Y = pointF.Y;
			graphics.FillPolygon(solidBrush, array);
			break;
		}
		case MenuGlyph.Bullet:
		{
			int num = Math.Max(2, rectangle.Width / 3);
			Rectangle rectangle2 = new Rectangle(rectangle.X + num, rectangle.Y + num, rectangle.Width - num * 2, rectangle.Height - num * 2);
			graphics.FillEllipse(solidBrush, rectangle2);
			break;
		}
		case MenuGlyph.Checkmark:
		{
			Pen pen = ResPool.GetPen(color);
			int num = Math.Max(2, rectangle.Width / 6);
			Rectangle rectangle2 = new Rectangle(rectangle.X + num, rectangle.Y + num, rectangle.Width - num * 2, rectangle.Height - num * 2);
			int num2 = Math.Max(1, rectangle.Width / 12);
			int num3 = rectangle2.Y + num + (rectangle2.Height - (2 * num2 + num)) / 2;
			for (int i = 0; i < num; i++)
			{
				graphics.DrawLine(pen, rectangle2.Left + num / 2, num3 + i, rectangle2.Left + num / 2 + 2 * num2, num3 + 2 * num2 + i);
				graphics.DrawLine(pen, rectangle2.Left + num / 2 + 2 * num2, num3 + 2 * num2 + i, rectangle2.Left + num / 2 + 6 * num2, num3 - 2 * num2 + i);
			}
			break;
		}
		}
	}

	[System.MonoInternalNote("Does not respect Mixed")]
	public override void CPDrawMixedCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		CPDrawCheckBox(graphics, rectangle, state);
	}

	public override void CPDrawRadioButton(Graphics dc, Rectangle rectangle, ButtonState state)
	{
		CPColor cPColor = ResPool.GetCPColor(ColorControl);
		Color color = Color.Black;
		Color color2 = Color.Black;
		Color color3 = Color.Black;
		Color color4 = Color.Black;
		Color color5 = Color.Black;
		int num = ((rectangle.Width <= rectangle.Height) ? ((int)((float)rectangle.Width * 0.9f)) : ((int)((float)rectangle.Height * 0.9f)));
		int num2 = num / 2;
		Rectangle rect = new Rectangle(rectangle.X + rectangle.Width / 2 - num2, rectangle.Y + rectangle.Height / 2 - num2, num, num);
		Brush brush = null;
		if ((state & ButtonState.All) == ButtonState.All)
		{
			brush = ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl);
			color = cPColor.Dark;
		}
		else if ((state & ButtonState.Flat) == ButtonState.Flat)
		{
			brush = (((state & ButtonState.Inactive) != ButtonState.Inactive && (state & ButtonState.Pushed) != ButtonState.Pushed) ? SystemBrushes.ControlLightLight : ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl));
		}
		else
		{
			brush = (((state & ButtonState.Inactive) != ButtonState.Inactive && (state & ButtonState.Pushed) != ButtonState.Pushed) ? SystemBrushes.ControlLightLight : ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl));
			color2 = cPColor.Dark;
			color3 = cPColor.DarkDark;
			color4 = cPColor.Light;
			color5 = Color.Transparent;
			if ((state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				color = cPColor.Dark;
			}
		}
		dc.FillEllipse(brush, rect.X + 1, rect.Y + 1, num - 1, num - 1);
		int num3 = Math.Max(1, (int)((float)num * 0.08f));
		dc.DrawArc(ResPool.GetSizedPen(color2, num3), rect, 135f, 180f);
		dc.DrawArc(ResPool.GetSizedPen(color3, num3), Rectangle.Inflate(rect, -num3, -num3), 135f, 180f);
		dc.DrawArc(ResPool.GetSizedPen(color4, num3), rect, 315f, 180f);
		if (color5 != Color.Transparent)
		{
			dc.DrawArc(ResPool.GetSizedPen(color5, num3), Rectangle.Inflate(rect, -num3, -num3), 315f, 180f);
		}
		else
		{
			using Pen pen = new Pen(ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl), num3);
			dc.DrawArc(pen, Rectangle.Inflate(rect, -num3, -num3), 315f, 180f);
		}
		if ((state & ButtonState.Checked) == ButtonState.Checked)
		{
			int num4 = num3 * 4;
			Rectangle rect2 = Rectangle.Inflate(rect, -num4, -num4);
			if (rectangle.Height > 13)
			{
				rect2.X++;
				rect2.Y++;
				rect2.Height--;
				dc.FillEllipse(ResPool.GetSolidBrush(color), rect2);
			}
			else
			{
				Pen pen2 = ResPool.GetPen(color);
				dc.DrawLine(pen2, rect2.X, rect2.Y + rect2.Height / 2, rect2.Right, rect2.Y + rect2.Height / 2);
				dc.DrawLine(pen2, rect2.X, rect2.Y + rect2.Height / 2 + 1, rect2.Right, rect2.Y + rect2.Height / 2 + 1);
				dc.DrawLine(pen2, rect2.X + rect2.Width / 2, rect2.Y, rect2.X + rect2.Width / 2, rect2.Bottom);
				dc.DrawLine(pen2, rect2.X + rect2.Width / 2 + 1, rect2.Y, rect2.X + rect2.Width / 2 + 1, rect2.Bottom);
			}
		}
	}

	public override void CPDrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style)
	{
	}

	public override void CPDrawReversibleLine(Point start, Point end, Color backColor)
	{
	}

	public override void CPDrawScrollButton(Graphics dc, Rectangle area, ScrollButton type, ButtonState state)
	{
		DrawScrollButtonPrimitive(dc, area, state);
		bool flag = true;
		int num = 0;
		if ((state & ButtonState.Pushed) != 0)
		{
			num = 1;
		}
		Rectangle rectangle = new Rectangle(area.X + 2 + num, area.Y + 2 + num, area.Width - 4, area.Height - 4);
		Point[] array = new Point[3];
		for (int i = 0; i < 3; i++)
		{
			array[i] = default(Point);
		}
		Pen pen = SystemPens.ControlText;
		if ((state & ButtonState.Inactive) != 0)
		{
			pen = SystemPens.ControlDark;
		}
		int num4;
		int num2;
		int num5;
		switch (type)
		{
		case ScrollButton.Min:
			num4 = (int)Math.Round((float)rectangle.Width / 2f) - 1;
			num2 = (int)Math.Round((float)rectangle.Height / 2f);
			if (num4 == 1)
			{
				num4 = 2;
			}
			if (num2 == 1)
			{
				num2 = 2;
			}
			if (rectangle.Height >= 8)
			{
				num5 = ((rectangle.Height != 11) ? ((int)Math.Round((float)rectangle.Height / 3f)) : 3);
			}
			else
			{
				num5 = 2;
				flag = false;
			}
			array[0].X = rectangle.X + num4;
			array[0].Y = rectangle.Y + num2 - num5 / 2;
			array[1].X = array[0].X + num5 - 1;
			array[1].Y = array[0].Y + num5 - 1;
			array[2].X = array[0].X - num5 + 1;
			array[2].Y = array[1].Y;
			dc.DrawPolygon(pen, array);
			if ((state & ButtonState.Inactive) != 0)
			{
				dc.DrawLine(SystemPens.ControlLightLight, array[1].X + 1, array[1].Y + 1, array[2].X + 1, array[1].Y + 1);
			}
			if (flag)
			{
				for (int k = 0; k < array[1].Y - array[0].Y; k++)
				{
					dc.DrawLine(pen, array[2].X, array[1].Y - k, array[1].X, array[1].Y - k);
					array[1].X--;
					array[2].X++;
				}
			}
			return;
		case ScrollButton.Left:
		{
			num2 = (int)Math.Round((float)rectangle.Height / 2f) - 1;
			if (num2 == 1)
			{
				num2 = 2;
			}
			int num3;
			if (rectangle.Width >= 8)
			{
				num3 = ((rectangle.Width != 11) ? ((int)Math.Round((float)rectangle.Width / 3f)) : 3);
			}
			else
			{
				num3 = 2;
				flag = false;
			}
			array[0].X = rectangle.Left + num3 - 1;
			array[0].Y = rectangle.Y + num2;
			if (array[0].X - 1 == rectangle.X)
			{
				array[0].X++;
			}
			array[1].X = array[0].X + num3 - 1;
			array[1].Y = array[0].Y - num3 + 1;
			array[2].X = array[1].X;
			array[2].Y = array[0].Y + num3 - 1;
			dc.DrawPolygon(pen, array);
			if ((state & ButtonState.Inactive) != 0)
			{
				dc.DrawLine(SystemPens.ControlLightLight, array[1].X + 1, array[1].Y + 1, array[2].X + 1, array[2].Y + 1);
			}
			if (flag)
			{
				for (int l = 0; l < array[2].X - array[0].X; l++)
				{
					dc.DrawLine(pen, array[2].X - l, array[1].Y, array[2].X - l, array[2].Y);
					array[1].Y++;
					array[2].Y--;
				}
			}
			return;
		}
		case ScrollButton.Right:
		{
			num2 = (int)Math.Round((float)rectangle.Height / 2f) - 1;
			if (num2 == 1)
			{
				num2 = 2;
			}
			int num3;
			if (rectangle.Width >= 8)
			{
				num3 = ((rectangle.Width != 11) ? ((int)Math.Round((float)rectangle.Width / 3f)) : 3);
			}
			else
			{
				num3 = 2;
				flag = false;
			}
			array[0].X = rectangle.Right - num3 - 1;
			array[0].Y = rectangle.Y + num2;
			if (array[0].X - 1 == rectangle.X)
			{
				array[0].X++;
			}
			array[1].X = array[0].X - num3 + 1;
			array[1].Y = array[0].Y - num3 + 1;
			array[2].X = array[1].X;
			array[2].Y = array[0].Y + num3 - 1;
			dc.DrawPolygon(pen, array);
			if ((state & ButtonState.Inactive) != 0)
			{
				dc.DrawLine(SystemPens.ControlLightLight, array[0].X + 1, array[0].Y + 1, array[2].X + 1, array[2].Y + 1);
				dc.DrawLine(SystemPens.ControlLightLight, array[0].X, array[0].Y + 1, array[2].X + 1, array[2].Y);
			}
			if (flag)
			{
				for (int j = 0; j < array[0].X - array[1].X; j++)
				{
					dc.DrawLine(pen, array[2].X + j, array[1].Y, array[2].X + j, array[2].Y);
					array[1].Y++;
					array[2].Y--;
				}
			}
			return;
		}
		}
		num4 = (int)Math.Round((float)rectangle.Width / 2f) - 1;
		num2 = (int)Math.Round((float)rectangle.Height / 2f) - 1;
		if (num4 == 1)
		{
			num4 = 2;
		}
		if (rectangle.Height >= 8)
		{
			num5 = ((rectangle.Height != 11) ? ((int)Math.Round((float)rectangle.Height / 3f)) : 3);
		}
		else
		{
			num5 = 2;
			flag = false;
		}
		array[0].X = rectangle.X + num4;
		array[0].Y = rectangle.Y + num2 + num5 / 2;
		array[1].X = array[0].X + num5 - 1;
		array[1].Y = array[0].Y - num5 + 1;
		array[2].X = array[0].X - num5 + 1;
		array[2].Y = array[1].Y;
		dc.DrawPolygon(pen, array);
		if ((state & ButtonState.Inactive) != 0)
		{
			dc.DrawLine(SystemPens.ControlLightLight, array[1].X + 1, array[1].Y + 1, array[0].X + 1, array[0].Y + 1);
			dc.DrawLine(SystemPens.ControlLightLight, array[1].X, array[1].Y + 1, array[0].X + 1, array[0].Y);
		}
		if (flag)
		{
			for (int m = 0; m < array[0].Y - array[1].Y; m++)
			{
				dc.DrawLine(pen, array[1].X, array[1].Y + m, array[2].X, array[1].Y + m);
				array[1].X--;
				array[2].X++;
			}
		}
	}

	public override void CPDrawSelectionFrame(Graphics graphics, bool active, Rectangle outsideRect, Rectangle insideRect, Color backColor)
	{
	}

	public override void CPDrawSizeGrip(Graphics dc, Color backColor, Rectangle bounds)
	{
		Pen pen = ResPool.GetPen(ControlPaint.Dark(backColor));
		Pen pen2 = ResPool.GetPen(ControlPaint.LightLight(backColor));
		for (int i = 2; i < bounds.Width - 2; i += 4)
		{
			dc.DrawLine(pen2, bounds.X + i, bounds.Bottom - 2, bounds.Right - 1, bounds.Y + i - 1);
			dc.DrawLine(pen, bounds.X + i + 1, bounds.Bottom - 2, bounds.Right - 1, bounds.Y + i);
			dc.DrawLine(pen, bounds.X + i + 2, bounds.Bottom - 2, bounds.Right - 1, bounds.Y + i + 1);
		}
	}

	private void DrawStringDisabled20(Graphics g, string s, Font font, Rectangle layoutRectangle, Color color, TextFormatFlags flags, bool useDrawString)
	{
		CPColor cPColor = ResPool.GetCPColor(color);
		layoutRectangle.Offset(1, 1);
		TextRenderer.DrawTextInternal(g, s, font, layoutRectangle, cPColor.LightLight, flags, useDrawString);
		layoutRectangle.Offset(-1, -1);
		TextRenderer.DrawTextInternal(g, s, font, layoutRectangle, cPColor.Dark, flags, useDrawString);
	}

	public override void CPDrawStringDisabled(Graphics dc, string s, Font font, Color color, RectangleF layoutRectangle, StringFormat format)
	{
		CPColor cPColor = ResPool.GetCPColor(color);
		dc.DrawString(s, font, ResPool.GetSolidBrush(cPColor.LightLight), new RectangleF(layoutRectangle.X + 1f, layoutRectangle.Y + 1f, layoutRectangle.Width, layoutRectangle.Height), format);
		dc.DrawString(s, font, ResPool.GetSolidBrush(cPColor.Dark), layoutRectangle, format);
	}

	public override void CPDrawStringDisabled(IDeviceContext dc, string s, Font font, Color color, Rectangle layoutRectangle, TextFormatFlags format)
	{
		CPColor cPColor = ResPool.GetCPColor(color);
		layoutRectangle.Offset(1, 1);
		TextRenderer.DrawText(dc, s, font, layoutRectangle, cPColor.LightLight, format);
		layoutRectangle.Offset(-1, -1);
		TextRenderer.DrawText(dc, s, font, layoutRectangle, cPColor.Dark, format);
	}

	public override void CPDrawVisualStyleBorder(Graphics graphics, Rectangle bounds)
	{
		graphics.DrawRectangle(SystemPens.ControlDarkDark, bounds);
	}

	private static void DrawBorderInternal(Graphics graphics, int startX, int startY, int endX, int endY, int width, Color color, ButtonBorderStyle style, Border3DSide side)
	{
		DrawBorderInternal(graphics, (float)startX, (float)startY, (float)endX, (float)endY, width, color, style, side);
	}

	private static void DrawBorderInternal(Graphics graphics, float startX, float startY, float endX, float endY, int width, Color color, ButtonBorderStyle style, Border3DSide side)
	{
		Pen pen = null;
		switch (style)
		{
		default:
			return;
		case ButtonBorderStyle.Solid:
		case ButtonBorderStyle.Inset:
		case ButtonBorderStyle.Outset:
			pen = ThemeEngine.Current.ResPool.GetDashPen(color, DashStyle.Solid);
			break;
		case ButtonBorderStyle.Dashed:
			pen = ThemeEngine.Current.ResPool.GetDashPen(color, DashStyle.Dash);
			break;
		case ButtonBorderStyle.Dotted:
			pen = ThemeEngine.Current.ResPool.GetDashPen(color, DashStyle.Dot);
			break;
		case ButtonBorderStyle.None:
			return;
		}
		switch (style)
		{
		case ButtonBorderStyle.Outset:
		{
			ControlPaint.Color2HBS(color, out var h2, out var l2, out var s2);
			int num3 = l2 / width;
			int num4 = ((l2 <= 127) ? ((127 - l2) / width) : Math.Max(6, (160 - l2) / width));
			for (int j = 0; j < width; j++)
			{
				switch (side)
				{
				case Border3DSide.Left:
				{
					Color color3 = ControlPaint.HBS2Color(h2, Math.Min(255, l2 + num4 * (width - j)), s2);
					pen = ThemeEngine.Current.ResPool.GetPen(color3);
					graphics.DrawLine(pen, startX + (float)j, startY + (float)j, endX + (float)j, endY - (float)j);
					break;
				}
				case Border3DSide.Right:
				{
					Color color3 = ControlPaint.HBS2Color(h2, Math.Max(0, l2 - num3 * (width - j)), s2);
					pen = ThemeEngine.Current.ResPool.GetPen(color3);
					graphics.DrawLine(pen, startX - (float)j, startY + (float)j, endX - (float)j, endY - (float)j);
					break;
				}
				case Border3DSide.Top:
				{
					Color color3 = ControlPaint.HBS2Color(h2, Math.Min(255, l2 + num4 * (width - j)), s2);
					pen = ThemeEngine.Current.ResPool.GetPen(color3);
					graphics.DrawLine(pen, startX + (float)j, startY + (float)j, endX - (float)j, endY + (float)j);
					break;
				}
				case Border3DSide.Bottom:
				{
					Color color3 = ControlPaint.HBS2Color(h2, Math.Max(0, l2 - num3 * (width - j)), s2);
					pen = ThemeEngine.Current.ResPool.GetPen(color3);
					graphics.DrawLine(pen, startX + (float)j, startY - (float)j, endX - (float)j, endY - (float)j);
					break;
				}
				}
			}
			return;
		}
		case ButtonBorderStyle.Inset:
		{
			ControlPaint.Color2HBS(color, out var h, out var l, out var s);
			int num = l / width;
			int num2 = ((l <= 127) ? ((127 - l) / width) : Math.Max(6, (160 - l) / width));
			for (int i = 0; i < width; i++)
			{
				switch (side)
				{
				case Border3DSide.Left:
				{
					Color color2 = ControlPaint.HBS2Color(h, Math.Max(0, l - num * (width - i)), s);
					pen = ThemeEngine.Current.ResPool.GetPen(color2);
					graphics.DrawLine(pen, startX + (float)i, startY + (float)i, endX + (float)i, endY - (float)i);
					break;
				}
				case Border3DSide.Right:
				{
					Color color2 = ControlPaint.HBS2Color(h, Math.Min(255, l + num2 * (width - i)), s);
					pen = ThemeEngine.Current.ResPool.GetPen(color2);
					graphics.DrawLine(pen, startX - (float)i, startY + (float)i, endX - (float)i, endY - (float)i);
					break;
				}
				case Border3DSide.Top:
				{
					Color color2 = ControlPaint.HBS2Color(h, Math.Max(0, l - num * (width - i)), s);
					pen = ThemeEngine.Current.ResPool.GetPen(color2);
					graphics.DrawLine(pen, startX + (float)i, startY + (float)i, endX - (float)i, endY + (float)i);
					break;
				}
				case Border3DSide.Bottom:
				{
					Color color2 = ControlPaint.HBS2Color(h, Math.Min(255, l + num2 * (width - i)), s);
					pen = ThemeEngine.Current.ResPool.GetPen(color2);
					graphics.DrawLine(pen, startX + (float)i, startY - (float)i, endX - (float)i, endY - (float)i);
					break;
				}
				}
			}
			return;
		}
		}
		switch (side)
		{
		case Border3DSide.Left:
		{
			for (int num5 = 0; num5 < width; num5++)
			{
				graphics.DrawLine(pen, startX + (float)num5, startY + (float)num5, endX + (float)num5, endY - (float)num5);
			}
			break;
		}
		case Border3DSide.Right:
		{
			for (int m = 0; m < width; m++)
			{
				graphics.DrawLine(pen, startX - (float)m, startY + (float)m, endX - (float)m, endY - (float)m);
			}
			break;
		}
		case Border3DSide.Top:
		{
			for (int n = 0; n < width; n++)
			{
				graphics.DrawLine(pen, startX + (float)n, startY + (float)n, endX - (float)n, endY + (float)n);
			}
			break;
		}
		case Border3DSide.Bottom:
		{
			for (int k = 0; k < width; k++)
			{
				graphics.DrawLine(pen, startX + (float)k, startY - (float)k, endX - (float)k, endY - (float)k);
			}
			break;
		}
		case Border3DSide.Left | Border3DSide.Top:
		case Border3DSide.Left | Border3DSide.Right:
		case Border3DSide.Top | Border3DSide.Right:
		case Border3DSide.Left | Border3DSide.Top | Border3DSide.Right:
			break;
		}
	}

	private void DrawCaptionHelper(Graphics graphics, Color color, Pen pen, int lineWidth, int shift, Rectangle captionRect, CaptionButton button)
	{
		switch (button)
		{
		case CaptionButton.Close:
			if (lineWidth < 2)
			{
				graphics.DrawLine(pen, captionRect.Left + 2 * lineWidth + 1 + shift, captionRect.Top + 2 * lineWidth + shift, captionRect.Right - 2 * lineWidth + 1 + shift, captionRect.Bottom - 2 * lineWidth + shift);
				graphics.DrawLine(pen, captionRect.Right - 2 * lineWidth + 1 + shift, captionRect.Top + 2 * lineWidth + shift, captionRect.Left + 2 * lineWidth + 1 + shift, captionRect.Bottom - 2 * lineWidth + shift);
			}
			graphics.DrawLine(pen, captionRect.Left + 2 * lineWidth + shift, captionRect.Top + 2 * lineWidth + shift, captionRect.Right - 2 * lineWidth + shift, captionRect.Bottom - 2 * lineWidth + shift);
			graphics.DrawLine(pen, captionRect.Right - 2 * lineWidth + shift, captionRect.Top + 2 * lineWidth + shift, captionRect.Left + 2 * lineWidth + shift, captionRect.Bottom - 2 * lineWidth + shift);
			break;
		case CaptionButton.Help:
		{
			StringFormat stringFormat = new StringFormat();
			Font font = new Font("Microsoft Sans Serif", captionRect.Height, FontStyle.Bold, GraphicsUnit.Pixel);
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			graphics.DrawString("?", font, ResPool.GetSolidBrush(color), captionRect.X + captionRect.Width / 2 + shift, captionRect.Y + captionRect.Height / 2 + shift + lineWidth / 2, stringFormat);
			stringFormat.Dispose();
			font.Dispose();
			break;
		}
		case CaptionButton.Maximize:
		{
			for (int num3 = 0; num3 < Math.Max(2, lineWidth); num3++)
			{
				graphics.DrawLine(pen, captionRect.Left + lineWidth + shift, captionRect.Top + 2 * lineWidth + shift + num3, captionRect.Right - lineWidth - lineWidth / 2 + shift, captionRect.Top + 2 * lineWidth + shift + num3);
			}
			for (int num4 = 0; num4 < Math.Max(1, lineWidth / 2); num4++)
			{
				graphics.DrawLine(pen, captionRect.Left + lineWidth + shift + num4, captionRect.Top + 2 * lineWidth + shift, captionRect.Left + lineWidth + shift + num4, captionRect.Bottom - lineWidth + shift);
			}
			for (int num5 = 0; num5 < Math.Max(1, lineWidth / 2); num5++)
			{
				graphics.DrawLine(pen, captionRect.Right - lineWidth - lineWidth / 2 + shift + num5, captionRect.Top + 2 * lineWidth + shift, captionRect.Right - lineWidth - lineWidth / 2 + shift + num5, captionRect.Bottom - lineWidth + shift);
			}
			for (int num6 = 0; num6 < Math.Max(1, lineWidth / 2); num6++)
			{
				graphics.DrawLine(pen, captionRect.Left + lineWidth + shift, captionRect.Bottom - lineWidth + shift - num6, captionRect.Right - lineWidth - lineWidth / 2 + shift, captionRect.Bottom - lineWidth + shift - num6);
			}
			break;
		}
		case CaptionButton.Minimize:
		{
			for (int num7 = 0; num7 < Math.Max(2, lineWidth); num7++)
			{
				graphics.DrawLine(pen, captionRect.Left + lineWidth + shift, captionRect.Bottom - lineWidth + shift - num7, captionRect.Right - 3 * lineWidth + shift, captionRect.Bottom - lineWidth + shift - num7);
			}
			break;
		}
		case CaptionButton.Restore:
		{
			for (int i = 0; i < Math.Max(2, lineWidth); i++)
			{
				graphics.DrawLine(pen, captionRect.Left + 3 * lineWidth + shift, captionRect.Top + 2 * lineWidth + shift - i, captionRect.Right - lineWidth - lineWidth / 2 + shift, captionRect.Top + 2 * lineWidth + shift - i);
			}
			for (int j = 0; j < Math.Max(1, lineWidth / 2); j++)
			{
				graphics.DrawLine(pen, captionRect.Left + 3 * lineWidth + shift + j, captionRect.Top + 2 * lineWidth + shift, captionRect.Left + 3 * lineWidth + shift + j, captionRect.Top + 4 * lineWidth + shift);
			}
			for (int k = 0; k < Math.Max(1, lineWidth / 2); k++)
			{
				graphics.DrawLine(pen, captionRect.Right - lineWidth - lineWidth / 2 + shift - k, captionRect.Top + 2 * lineWidth + shift, captionRect.Right - lineWidth - lineWidth / 2 + shift - k, captionRect.Top + 5 * lineWidth - lineWidth / 2 + shift);
			}
			for (int l = 0; l < Math.Max(1, lineWidth / 2); l++)
			{
				graphics.DrawLine(pen, captionRect.Right - 3 * lineWidth - lineWidth / 2 + shift, captionRect.Top + 5 * lineWidth - lineWidth / 2 + shift + 1 + l, captionRect.Right - lineWidth - lineWidth / 2 + shift, captionRect.Top + 5 * lineWidth - lineWidth / 2 + shift + 1 + l);
			}
			for (int m = 0; m < Math.Max(2, lineWidth); m++)
			{
				graphics.DrawLine(pen, captionRect.Left + lineWidth + shift, captionRect.Top + 4 * lineWidth + shift + 1 - m, captionRect.Right - 3 * lineWidth - lineWidth / 2 + shift, captionRect.Top + 4 * lineWidth + shift + 1 - m);
			}
			for (int n = 0; n < Math.Max(1, lineWidth / 2); n++)
			{
				graphics.DrawLine(pen, captionRect.Left + lineWidth + shift + n, captionRect.Top + 4 * lineWidth + shift + 1, captionRect.Left + lineWidth + shift + n, captionRect.Bottom - lineWidth + shift);
			}
			for (int num = 0; num < Math.Max(1, lineWidth / 2); num++)
			{
				graphics.DrawLine(pen, captionRect.Right - 3 * lineWidth - lineWidth / 2 + shift - num, captionRect.Top + 4 * lineWidth + shift + 1, captionRect.Right - 3 * lineWidth - lineWidth / 2 + shift - num, captionRect.Bottom - lineWidth + shift);
			}
			for (int num2 = 0; num2 < Math.Max(1, lineWidth / 2); num2++)
			{
				graphics.DrawLine(pen, captionRect.Left + lineWidth + shift, captionRect.Bottom - lineWidth + shift - num2, captionRect.Right - 3 * lineWidth - lineWidth / 2 + shift, captionRect.Bottom - lineWidth + shift - num2);
			}
			break;
		}
		}
	}

	public void DrawScrollButtonPrimitive(Graphics dc, Rectangle area, ButtonState state)
	{
		if ((state & ButtonState.Pushed) == ButtonState.Pushed)
		{
			dc.FillRectangle(SystemBrushes.Control, area.X + 1, area.Y + 1, area.Width - 2, area.Height - 2);
			dc.DrawRectangle(SystemPens.ControlDark, area.X, area.Y, area.Width, area.Height);
			return;
		}
		Brush control = SystemBrushes.Control;
		Brush controlLightLight = SystemBrushes.ControlLightLight;
		Brush controlDark = SystemBrushes.ControlDark;
		Brush controlDarkDark = SystemBrushes.ControlDarkDark;
		dc.FillRectangle(control, area.X, area.Y, area.Width, 1);
		dc.FillRectangle(control, area.X, area.Y, 1, area.Height);
		dc.FillRectangle(controlLightLight, area.X + 1, area.Y + 1, area.Width - 1, 1);
		dc.FillRectangle(controlLightLight, area.X + 1, area.Y + 2, 1, area.Height - 4);
		dc.FillRectangle(controlDark, area.X + 1, area.Y + area.Height - 2, area.Width - 2, 1);
		dc.FillRectangle(controlDarkDark, area.X, area.Y + area.Height - 1, area.Width, 1);
		dc.FillRectangle(controlDark, area.X + area.Width - 2, area.Y + 1, 1, area.Height - 3);
		dc.FillRectangle(controlDarkDark, area.X + area.Width - 1, area.Y, 1, area.Height - 1);
		dc.FillRectangle(control, area.X + 2, area.Y + 2, area.Width - 4, area.Height - 4);
	}

	public override void CPDrawBorderStyle(Graphics dc, Rectangle area, BorderStyle border_style)
	{
		switch (border_style)
		{
		case BorderStyle.Fixed3D:
			dc.DrawLine(ResPool.GetPen(ColorControlDark), area.X, area.Y, area.X + area.Width, area.Y);
			dc.DrawLine(ResPool.GetPen(ColorControlDark), area.X, area.Y, area.X, area.Y + area.Height);
			dc.DrawLine(ResPool.GetPen(ColorControlLight), area.X, area.Y + area.Height - 1, area.X + area.Width, area.Y + area.Height - 1);
			dc.DrawLine(ResPool.GetPen(ColorControlLight), area.X + area.Width - 1, area.Y, area.X + area.Width - 1, area.Y + area.Height);
			dc.DrawLine(ResPool.GetPen(ColorActiveBorder), area.X + 1, area.Bottom - 2, area.Right - 2, area.Bottom - 2);
			dc.DrawLine(ResPool.GetPen(ColorActiveBorder), area.Right - 2, area.Top + 1, area.Right - 2, area.Bottom - 2);
			dc.DrawLine(ResPool.GetPen(ColorControlDarkDark), area.X + 1, area.Top + 1, area.X + 1, area.Bottom - 3);
			dc.DrawLine(ResPool.GetPen(ColorControlDarkDark), area.X + 1, area.Top + 1, area.Right - 3, area.Top + 1);
			break;
		case BorderStyle.FixedSingle:
			dc.DrawRectangle(ResPool.GetPen(ColorWindowFrame), area.X, area.Y, area.Width - 1, area.Height - 1);
			break;
		case BorderStyle.None:
			break;
		}
	}
}
