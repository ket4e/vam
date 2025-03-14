using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Theming.Default;

internal class TabControlPainter
{
	private Size defaultItemSize;

	private Point defaultPadding;

	private int minimumTabWidth;

	private Rectangle selectedTabDelta;

	private Point tabPanelOffset;

	private int selectedSpacing;

	private Size rowSpacingNormal;

	private Size rowSpacingButtons;

	private Size rowSpacingFlatButtons;

	private int scrollerWidth;

	private Point focusRectSpacing;

	private Rectangle tabPageSpacing;

	private int colSpacing;

	private int flatButtonSpacing;

	private Point imagePadding;

	private StringFormat defaultFormatting;

	private Rectangle borderThickness;

	protected SystemResPool ResPool => ThemeEngine.Current.ResPool;

	public virtual Size DefaultItemSize
	{
		get
		{
			return defaultItemSize;
		}
		set
		{
			defaultItemSize = value;
		}
	}

	public virtual Point DefaultPadding
	{
		get
		{
			return defaultPadding;
		}
		set
		{
			defaultPadding = value;
		}
	}

	public virtual int MinimumTabWidth
	{
		get
		{
			return minimumTabWidth;
		}
		set
		{
			minimumTabWidth = value;
		}
	}

	public virtual Rectangle SelectedTabDelta
	{
		get
		{
			return selectedTabDelta;
		}
		set
		{
			selectedTabDelta = value;
		}
	}

	public virtual Point TabPanelOffset
	{
		get
		{
			return tabPanelOffset;
		}
		set
		{
			tabPanelOffset = value;
		}
	}

	public virtual int SelectedSpacing
	{
		get
		{
			return selectedSpacing;
		}
		set
		{
			selectedSpacing = value;
		}
	}

	public virtual Size RowSpacingNormal
	{
		get
		{
			return rowSpacingNormal;
		}
		set
		{
			rowSpacingNormal = value;
		}
	}

	public virtual Size RowSpacingButtons
	{
		get
		{
			return rowSpacingButtons;
		}
		set
		{
			rowSpacingButtons = value;
		}
	}

	public virtual Size RowSpacingFlatButtons
	{
		get
		{
			return rowSpacingFlatButtons;
		}
		set
		{
			rowSpacingFlatButtons = value;
		}
	}

	public virtual Point FocusRectSpacing
	{
		get
		{
			return focusRectSpacing;
		}
		set
		{
			focusRectSpacing = value;
		}
	}

	public virtual int ColSpacing
	{
		get
		{
			return colSpacing;
		}
		set
		{
			colSpacing = value;
		}
	}

	public virtual int FlatButtonSpacing
	{
		get
		{
			return flatButtonSpacing;
		}
		set
		{
			flatButtonSpacing = value;
		}
	}

	public virtual Rectangle TabPageSpacing
	{
		get
		{
			return tabPageSpacing;
		}
		set
		{
			tabPageSpacing = value;
		}
	}

	public virtual Point ImagePadding
	{
		get
		{
			return imagePadding;
		}
		set
		{
			imagePadding = value;
		}
	}

	public virtual StringFormat DefaultFormatting
	{
		get
		{
			return defaultFormatting;
		}
		set
		{
			defaultFormatting = value;
		}
	}

	public virtual Rectangle BorderThickness
	{
		get
		{
			return borderThickness;
		}
		set
		{
			borderThickness = value;
		}
	}

	public virtual int ScrollerWidth
	{
		get
		{
			return scrollerWidth;
		}
		set
		{
			scrollerWidth = value;
		}
	}

	public TabControlPainter()
	{
		defaultItemSize = new Size(42, 21);
		defaultPadding = new Point(6, 3);
		selectedTabDelta = new Rectangle(2, 2, 4, 3);
		selectedSpacing = 0;
		rowSpacingNormal = new Size(0, 0);
		rowSpacingButtons = new Size(3, 3);
		rowSpacingFlatButtons = new Size(9, 3);
		colSpacing = 0;
		minimumTabWidth = 42;
		scrollerWidth = 17;
		focusRectSpacing = new Point(2, 2);
		tabPanelOffset = new Point(4, 0);
		flatButtonSpacing = 8;
		tabPageSpacing = new Rectangle(4, 2, 3, 4);
		imagePadding = new Point(2, 3);
		defaultFormatting = new StringFormat();
		defaultFormatting.Alignment = StringAlignment.Near;
		defaultFormatting.LineAlignment = StringAlignment.Near;
		defaultFormatting.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
		defaultFormatting.HotkeyPrefix = HotkeyPrefix.Show;
		borderThickness = new Rectangle(1, 1, 2, 2);
	}

	public virtual Size RowSpacing(TabControl tab)
	{
		return tab.Appearance switch
		{
			TabAppearance.Normal => rowSpacingNormal, 
			TabAppearance.Buttons => rowSpacingButtons, 
			TabAppearance.FlatButtons => rowSpacingFlatButtons, 
			_ => throw new Exception("Invalid Appearance value: " + tab.Appearance), 
		};
	}

	public virtual Rectangle GetLeftScrollRect(TabControl tab)
	{
		if (tab.Alignment == TabAlignment.Top)
		{
			return new Rectangle(tab.ClientRectangle.Right - scrollerWidth * 2, tab.ClientRectangle.Top + 1, scrollerWidth, scrollerWidth);
		}
		Rectangle tabPanelRect = GetTabPanelRect(tab);
		return new Rectangle(tab.ClientRectangle.Right - scrollerWidth * 2, tabPanelRect.Bottom + 2, scrollerWidth, scrollerWidth);
	}

	public virtual Rectangle GetRightScrollRect(TabControl tab)
	{
		if (tab.Alignment == TabAlignment.Top)
		{
			return new Rectangle(tab.ClientRectangle.Right - scrollerWidth, tab.ClientRectangle.Top + 1, scrollerWidth, scrollerWidth);
		}
		Rectangle tabPanelRect = GetTabPanelRect(tab);
		return new Rectangle(tab.ClientRectangle.Right - scrollerWidth, tabPanelRect.Bottom + 2, scrollerWidth, scrollerWidth);
	}

	public Rectangle GetDisplayRectangle(TabControl tab)
	{
		Rectangle tabPanelRect = GetTabPanelRect(tab);
		return new Rectangle(tabPanelRect.Left + tabPageSpacing.X, tabPanelRect.Top + tabPageSpacing.Y, tabPanelRect.Width - tabPageSpacing.X - tabPageSpacing.Width, tabPanelRect.Height - tabPageSpacing.Y - tabPageSpacing.Height);
	}

	public Rectangle GetTabPanelRect(TabControl tab)
	{
		Rectangle result = new Rectangle(tab.ClientRectangle.X, tab.ClientRectangle.Y, tab.ClientRectangle.Width, tab.ClientRectangle.Height);
		if (tab.TabCount == 0)
		{
			return result;
		}
		int height = RowSpacing(tab).Height;
		int num = (tab.ItemSize.Height + height - selectedTabDelta.Height) * tab.RowCount + selectedTabDelta.Y;
		switch (tab.Alignment)
		{
		case TabAlignment.Top:
			result.Y += num;
			result.Height -= num;
			break;
		case TabAlignment.Bottom:
			result.Height -= num;
			break;
		case TabAlignment.Left:
			result.X += num;
			result.Width -= num;
			break;
		case TabAlignment.Right:
			result.Width -= num;
			break;
		}
		return result;
	}

	public virtual void Draw(Graphics dc, Rectangle area, TabControl tab)
	{
		DrawBackground(dc, area, tab);
		int num = 0;
		int num2 = tab.TabPages.Count;
		int num3 = 1;
		if (tab.Alignment == TabAlignment.Top)
		{
			num = num2;
			num2 = 0;
			num3 = -1;
		}
		for (int i = num; i != num2; i += num3)
		{
			for (int j = tab.SliderPos; j < tab.TabPages.Count; j++)
			{
				if (j != tab.SelectedIndex && i == tab.TabPages[j].Row)
				{
					Rectangle tabRect = tab.GetTabRect(j);
					if (tabRect.IntersectsWith(area))
					{
						DrawTab(dc, tab.TabPages[j], tab, tabRect, is_selected: false);
					}
				}
			}
		}
		if (tab.SelectedIndex != -1 && tab.SelectedIndex >= tab.SliderPos)
		{
			Rectangle tabRect2 = tab.GetTabRect(tab.SelectedIndex);
			if (tabRect2.IntersectsWith(area))
			{
				DrawTab(dc, tab.TabPages[tab.SelectedIndex], tab, tabRect2, is_selected: true);
			}
		}
		if (tab.ShowSlider)
		{
			Rectangle rightScrollRect = GetRightScrollRect(tab);
			Rectangle leftScrollRect = GetLeftScrollRect(tab);
			DrawScrollButton(dc, rightScrollRect, area, ScrollButton.Right, tab.RightSliderState);
			DrawScrollButton(dc, leftScrollRect, area, ScrollButton.Left, tab.LeftSliderState);
		}
	}

	protected virtual void DrawScrollButton(Graphics dc, Rectangle bounds, Rectangle clippingArea, ScrollButton button, PushButtonState state)
	{
		ControlPaint.DrawScrollButton(dc, bounds, button, GetButtonState(state));
	}

	private static ButtonState GetButtonState(PushButtonState state)
	{
		if (state == PushButtonState.Pressed)
		{
			return ButtonState.Pushed;
		}
		return ButtonState.Normal;
	}

	protected virtual void DrawBackground(Graphics dc, Rectangle area, TabControl tab)
	{
		Brush control = SystemBrushes.Control;
		dc.FillRectangle(control, area);
		Rectangle tabPanelRect = GetTabPanelRect(tab);
		if (tab.Appearance == TabAppearance.Normal)
		{
			ControlPaint.DrawBorder3D(dc, tabPanelRect, Border3DStyle.RaisedInner, Border3DSide.Left | Border3DSide.Top);
			ControlPaint.DrawBorder3D(dc, tabPanelRect, Border3DStyle.Raised, Border3DSide.Right | Border3DSide.Bottom);
		}
	}

	protected virtual int DrawTab(Graphics dc, TabPage page, TabControl tab, Rectangle bounds, bool is_selected)
	{
		int width = bounds.Width;
		dc.FillRectangle(ResPool.GetSolidBrush(tab.BackColor), bounds);
		if (tab.Appearance == TabAppearance.Buttons || tab.Appearance == TabAppearance.FlatButtons)
		{
			if (tab.Appearance == TabAppearance.FlatButtons)
			{
				int width2 = bounds.Width;
				bounds.Width += flatButtonSpacing - 2;
				width = bounds.Width;
				if (tab.Alignment == TabAlignment.Top || tab.Alignment == TabAlignment.Bottom)
				{
					ThemeEngine.Current.CPDrawBorder3D(dc, bounds, Border3DStyle.Etched, Border3DSide.Right);
				}
				else
				{
					ThemeEngine.Current.CPDrawBorder3D(dc, bounds, Border3DStyle.Etched, Border3DSide.Top);
				}
				bounds.Width = width2;
			}
			if (is_selected)
			{
				ThemeEngine.Current.CPDrawBorder3D(dc, bounds, Border3DStyle.Sunken, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
			}
			else if (tab.Appearance != TabAppearance.FlatButtons)
			{
				ThemeEngine.Current.CPDrawBorder3D(dc, bounds, Border3DStyle.Raised, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
			}
		}
		else
		{
			CPColor cPColor = ResPool.GetCPColor(tab.BackColor);
			Pen pen = ResPool.GetPen(cPColor.LightLight);
			switch (tab.Alignment)
			{
			case TabAlignment.Top:
				dc.DrawLine(pen, bounds.Left, bounds.Bottom - 1, bounds.Left, bounds.Top + 3);
				dc.DrawLine(pen, bounds.Left, bounds.Top + 3, bounds.Left + 2, bounds.Top);
				dc.DrawLine(pen, bounds.Left + 2, bounds.Top, bounds.Right - 3, bounds.Top);
				dc.DrawLine(SystemPens.ControlDark, bounds.Right - 2, bounds.Top + 1, bounds.Right - 2, bounds.Bottom - 1);
				dc.DrawLine(SystemPens.ControlDarkDark, bounds.Right - 2, bounds.Top + 1, bounds.Right - 1, bounds.Top + 2);
				dc.DrawLine(SystemPens.ControlDarkDark, bounds.Right - 1, bounds.Top + 2, bounds.Right - 1, bounds.Bottom - 1);
				break;
			case TabAlignment.Bottom:
				dc.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 2);
				dc.DrawLine(pen, bounds.Left, bounds.Bottom - 2, bounds.Left + 3, bounds.Bottom);
				dc.DrawLine(SystemPens.ControlDarkDark, bounds.Left + 3, bounds.Bottom, bounds.Right - 3, bounds.Bottom);
				dc.DrawLine(SystemPens.ControlDark, bounds.Left + 3, bounds.Bottom - 1, bounds.Right - 3, bounds.Bottom - 1);
				dc.DrawLine(SystemPens.ControlDark, bounds.Right - 2, bounds.Bottom - 1, bounds.Right - 2, bounds.Top + 1);
				dc.DrawLine(SystemPens.ControlDarkDark, bounds.Right - 2, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 2);
				dc.DrawLine(SystemPens.ControlDarkDark, bounds.Right - 1, bounds.Bottom - 2, bounds.Right - 1, bounds.Top + 1);
				break;
			case TabAlignment.Left:
				dc.DrawLine(pen, bounds.Left - 2, bounds.Top, bounds.Right, bounds.Top);
				dc.DrawLine(pen, bounds.Left, bounds.Top + 2, bounds.Left - 2, bounds.Top);
				dc.DrawLine(pen, bounds.Left, bounds.Top + 2, bounds.Left, bounds.Bottom - 2);
				dc.DrawLine(SystemPens.ControlDark, bounds.Left, bounds.Bottom - 2, bounds.Left + 2, bounds.Bottom - 1);
				dc.DrawLine(SystemPens.ControlDark, bounds.Left + 2, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
				dc.DrawLine(SystemPens.ControlDarkDark, bounds.Left + 2, bounds.Bottom, bounds.Right, bounds.Bottom);
				break;
			default:
				dc.DrawLine(pen, bounds.Left, bounds.Top, bounds.Right - 3, bounds.Top);
				dc.DrawLine(pen, bounds.Right - 3, bounds.Top, bounds.Right, bounds.Top + 3);
				dc.DrawLine(SystemPens.ControlDark, bounds.Right - 1, bounds.Top + 1, bounds.Right - 1, bounds.Bottom - 1);
				dc.DrawLine(SystemPens.ControlDark, bounds.Left, bounds.Bottom - 1, bounds.Right - 2, bounds.Bottom - 1);
				dc.DrawLine(SystemPens.ControlDarkDark, bounds.Right, bounds.Top + 3, bounds.Right, bounds.Bottom - 3);
				dc.DrawLine(SystemPens.ControlDarkDark, bounds.Left, bounds.Bottom, bounds.Right - 3, bounds.Bottom);
				break;
			}
		}
		Rectangle rectangle = new Rectangle(bounds.Left + focusRectSpacing.X + borderThickness.Left, bounds.Top + focusRectSpacing.Y + borderThickness.Top, bounds.Width - focusRectSpacing.X * 2 - borderThickness.Width + 1, bounds.Height - focusRectSpacing.Y * 2 - borderThickness.Height);
		if (tab.DrawMode == TabDrawMode.Normal && page.Text != null)
		{
			if (tab.Alignment == TabAlignment.Left)
			{
				dc.TranslateTransform(bounds.Left, bounds.Bottom);
				dc.RotateTransform(-90f);
				dc.DrawString(page.Text, page.Font, SystemBrushes.ControlText, tab.Padding.X - 2, tab.Padding.Y, defaultFormatting);
				dc.ResetTransform();
			}
			else if (tab.Alignment == TabAlignment.Right)
			{
				dc.TranslateTransform(bounds.Right, bounds.Top);
				dc.RotateTransform(90f);
				dc.DrawString(page.Text, page.Font, SystemBrushes.ControlText, tab.Padding.X - 2, tab.Padding.Y, defaultFormatting);
				dc.ResetTransform();
			}
			else
			{
				Rectangle rectangle2 = rectangle;
				if (tab.ImageList != null && page.ImageIndex >= 0 && page.ImageIndex < tab.ImageList.Images.Count)
				{
					int y = rectangle.Y + (rectangle.Height - tab.ImageList.ImageSize.Height) / 2;
					tab.ImageList.Draw(dc, new Point(rectangle.X, y), page.ImageIndex);
					rectangle2.X += tab.ImageList.ImageSize.Width + 2;
					rectangle2.Width -= tab.ImageList.ImageSize.Width + 2;
				}
				dc.DrawString(page.Text, page.Font, SystemBrushes.ControlText, rectangle2, defaultFormatting);
			}
		}
		else if (page.Text != null)
		{
			DrawItemState drawItemState = DrawItemState.None;
			if (page == tab.SelectedTab)
			{
				drawItemState |= DrawItemState.Selected;
			}
			DrawItemEventArgs e = new DrawItemEventArgs(dc, tab.Font, bounds, tab.IndexForTabPage(page), drawItemState, page.ForeColor, page.BackColor);
			tab.OnDrawItemInternal(e);
			return width;
		}
		if (page.Parent.Focused && is_selected && tab.ShowFocusCues)
		{
			rectangle.Width--;
			ThemeEngine.Current.CPDrawFocusRectangle(dc, rectangle, tab.ForeColor, tab.BackColor);
		}
		return width;
	}

	public virtual bool HasHotElementStyles(TabControl tabControl)
	{
		return false;
	}
}
