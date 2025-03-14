using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

internal class ThemeVisualStyles : ThemeWin32Classic
{
	private class TrackBarHorizontalTickPainter : ITrackBarTickPainter
	{
		private readonly Graphics g;

		private readonly VisualStyleRenderer renderer;

		public TrackBarHorizontalTickPainter(Graphics g)
		{
			this.g = g;
			renderer = new VisualStyleRenderer(VisualStyleElement.TrackBar.Ticks.Normal);
		}

		public void Paint(float x1, float y1, float x2, float y2)
		{
			renderer.DrawEdge(g, new Rectangle((int)Math.Round(x1), (int)Math.Round(y1), 1, (int)Math.Round(y2 - y1) + 1), Edges.Left, EdgeStyle.Bump, EdgeEffects.None);
		}
	}

	private class TrackBarVerticalTickPainter : ITrackBarTickPainter
	{
		private readonly Graphics g;

		private readonly VisualStyleRenderer renderer;

		public TrackBarVerticalTickPainter(Graphics g)
		{
			this.g = g;
			renderer = new VisualStyleRenderer(VisualStyleElement.TrackBar.TicksVertical.Normal);
		}

		public void Paint(float x1, float y1, float x2, float y2)
		{
			renderer.DrawEdge(g, new Rectangle((int)Math.Round(x1), (int)Math.Round(y1), (int)Math.Round(x2 - x1) + 1, 1), Edges.Top, EdgeStyle.Bump, EdgeEffects.None);
		}
	}

	private const int DateTimePickerDropDownWidthOnWindowsVista = 34;

	private const int DateTimePickerDropDownHeightOnWindowsVista = 20;

	private const int WindowsVistaMajorVersion = 6;

	private const EdgeStyle TrackBarTickEdgeStyle = EdgeStyle.Bump;

	private const EdgeEffects TrackBarTickEdgeEffects = EdgeEffects.None;

	private static bool render_client_areas;

	private static bool render_non_client_areas;

	private static bool ScrollBarHasHoverArrowButtonStyleVisualStyles = Environment.OSVersion.Version.Major >= 6;

	private static Control control;

	public static bool RenderClientAreas => render_client_areas;

	public override bool DateTimePickerBorderHasHotElementStyle
	{
		get
		{
			if (RenderClientAreas && VisualStyleRenderer.IsElementDefined(VisualStyleElement.DatePicker.DateBorder.Hot))
			{
				return true;
			}
			return base.DateTimePickerBorderHasHotElementStyle;
		}
	}

	public override bool DateTimePickerDropDownButtonHasHotElementStyle
	{
		get
		{
			if (RenderClientAreas && VisualStyleRenderer.IsElementDefined(VisualStyleElement.DatePicker.ShowCalendarButtonRight.Hot))
			{
				return true;
			}
			return base.DateTimePickerDropDownButtonHasHotElementStyle;
		}
	}

	public override bool ListViewHasHotHeaderStyle
	{
		get
		{
			if (!RenderClientAreas || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.Header.Item.Hot))
			{
				return base.ListViewHasHotHeaderStyle;
			}
			return true;
		}
	}

	public override bool ScrollBarHasHotElementStyles
	{
		get
		{
			if (!RenderClientAreas)
			{
				return base.ScrollBarHasHotElementStyles;
			}
			return ScrollBarAreElementsDefined;
		}
	}

	public override bool ScrollBarHasPressedThumbStyle
	{
		get
		{
			if (!RenderClientAreas)
			{
				return base.ScrollBarHasPressedThumbStyle;
			}
			return ScrollBarAreElementsDefined;
		}
	}

	public override bool ScrollBarHasHoverArrowButtonStyle
	{
		get
		{
			if (RenderClientAreas && ScrollBarHasHoverArrowButtonStyleVisualStyles)
			{
				return ScrollBarAreElementsDefined;
			}
			return base.ScrollBarHasHoverArrowButtonStyle;
		}
	}

	private static bool ScrollBarAreElementsDefined => VisualStyleRenderer.IsElementDefined(VisualStyleElement.ScrollBar.ArrowButton.DownDisabled) && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ScrollBar.LeftTrackHorizontal.Disabled) && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ScrollBar.LowerTrackVertical.Disabled) && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ScrollBar.RightTrackHorizontal.Disabled) && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Disabled) && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ScrollBar.ThumbButtonVertical.Disabled) && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ScrollBar.UpperTrackVertical.Disabled);

	public override bool ToolBarHasHotCheckedElementStyles
	{
		get
		{
			if (!RenderClientAreas)
			{
				return base.ToolBarHasHotCheckedElementStyles;
			}
			return true;
		}
	}

	public override bool ToolTipTransparentBackground
	{
		get
		{
			if (!RenderClientAreas)
			{
				return base.ToolTipTransparentBackground;
			}
			VisualStyleElement normal = VisualStyleElement.ToolTip.Standard.Normal;
			if (!VisualStyleRenderer.IsElementDefined(normal))
			{
				return base.ToolTipTransparentBackground;
			}
			return new VisualStyleRenderer(normal).IsBackgroundPartiallyTransparent();
		}
	}

	public override bool TrackBarHasHotThumbStyle
	{
		get
		{
			if (!RenderClientAreas)
			{
				return base.TrackBarHasHotThumbStyle;
			}
			return true;
		}
	}

	public override bool UpDownBaseHasHotButtonStyle
	{
		get
		{
			if (!RenderClientAreas)
			{
				return base.UpDownBaseHasHotButtonStyle;
			}
			return true;
		}
	}

	public ThemeVisualStyles()
	{
		Update();
	}

	public override void ResetDefaults()
	{
		base.ResetDefaults();
		Update();
	}

	private static void Update()
	{
		bool isEnabledByUser = VisualStyleInformation.IsEnabledByUser;
		render_client_areas = isEnabledByUser && (Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled || Application.VisualStyleState == VisualStyleState.ClientAreaEnabled);
		render_non_client_areas = isEnabledByUser && Application.VisualStyleState == VisualStyleState.ClientAndNonClientAreasEnabled;
	}

	public override void DrawButtonBase(Graphics dc, Rectangle clip_area, ButtonBase button)
	{
		if (button.FlatStyle == FlatStyle.System)
		{
			ButtonRenderer.DrawButton(dc, new Rectangle(Point.Empty, button.Size), button.Text, button.Font, button.TextFormatFlags, null, Rectangle.Empty, ThemeWin32Classic.ShouldPaintFocusRectagle(button), GetPushButtonState(button));
		}
		else
		{
			base.DrawButtonBase(dc, clip_area, button);
		}
	}

	private static PushButtonState GetPushButtonState(ButtonBase button)
	{
		if (!button.Enabled)
		{
			return PushButtonState.Disabled;
		}
		if (button.Pressed)
		{
			return PushButtonState.Pressed;
		}
		if (button.Entered)
		{
			return PushButtonState.Hot;
		}
		if (button.IsDefault || button.Focused || button.paint_as_acceptbutton)
		{
			return PushButtonState.Default;
		}
		return PushButtonState.Normal;
	}

	public override void DrawButtonBackground(Graphics g, Button button, Rectangle clipArea)
	{
		if (!RenderClientAreas || !button.UseVisualStyleBackColor)
		{
			base.DrawButtonBackground(g, button, clipArea);
		}
		else
		{
			ButtonRenderer.GetPushButtonRenderer(GetPushButtonState(button)).DrawBackground(g, new Rectangle(Point.Empty, button.Size));
		}
	}

	protected override void CheckBox_DrawCheckBox(Graphics dc, CheckBox checkbox, ButtonState state, Rectangle checkbox_rectangle)
	{
		if (checkbox.Appearance == Appearance.Normal && checkbox.FlatStyle == FlatStyle.System)
		{
			CheckBoxRenderer.DrawCheckBox(dc, new Point(checkbox_rectangle.Left, checkbox_rectangle.Top), GetCheckBoxState(checkbox));
		}
		else
		{
			base.CheckBox_DrawCheckBox(dc, checkbox, state, checkbox_rectangle);
		}
	}

	private static CheckBoxState GetCheckBoxState(CheckBox checkBox)
	{
		switch (checkBox.CheckState)
		{
		case CheckState.Checked:
			if (!checkBox.Enabled)
			{
				return CheckBoxState.CheckedDisabled;
			}
			if (checkBox.Pressed)
			{
				return CheckBoxState.CheckedPressed;
			}
			if (checkBox.Entered)
			{
				return CheckBoxState.CheckedHot;
			}
			return CheckBoxState.CheckedNormal;
		case CheckState.Indeterminate:
			if (!checkBox.Enabled)
			{
				return CheckBoxState.MixedDisabled;
			}
			if (checkBox.Pressed)
			{
				return CheckBoxState.MixedPressed;
			}
			if (checkBox.Entered)
			{
				return CheckBoxState.MixedHot;
			}
			return CheckBoxState.MixedNormal;
		default:
			if (!checkBox.Enabled)
			{
				return CheckBoxState.UncheckedDisabled;
			}
			if (checkBox.Pressed)
			{
				return CheckBoxState.UncheckedPressed;
			}
			if (checkBox.Entered)
			{
				return CheckBoxState.UncheckedHot;
			}
			return CheckBoxState.UncheckedNormal;
		}
	}

	private static VisualStyleElement ComboBoxGetVisualStyleElement(ComboBox comboBox, ButtonState state)
	{
		switch (state)
		{
		case ButtonState.Inactive:
			return VisualStyleElement.ComboBox.DropDownButton.Disabled;
		case ButtonState.Pushed:
			return VisualStyleElement.ComboBox.DropDownButton.Pressed;
		default:
			if (comboBox.DropDownButtonEntered)
			{
				return VisualStyleElement.ComboBox.DropDownButton.Hot;
			}
			return VisualStyleElement.ComboBox.DropDownButton.Normal;
		}
	}

	public override void ComboBoxDrawNormalDropDownButton(ComboBox comboBox, Graphics g, Rectangle clippingArea, Rectangle area, ButtonState state)
	{
		if (!RenderClientAreas)
		{
			base.ComboBoxDrawNormalDropDownButton(comboBox, g, clippingArea, area, state);
			return;
		}
		VisualStyleElement element = ComboBoxGetVisualStyleElement(comboBox, state);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.ComboBoxDrawNormalDropDownButton(comboBox, g, clippingArea, area, state);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(g, area, clippingArea);
		}
	}

	public override bool ComboBoxNormalDropDownButtonHasTransparentBackground(ComboBox comboBox, ButtonState state)
	{
		if (!RenderClientAreas)
		{
			return base.ComboBoxNormalDropDownButtonHasTransparentBackground(comboBox, state);
		}
		VisualStyleElement element = ComboBoxGetVisualStyleElement(comboBox, state);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			return base.ComboBoxNormalDropDownButtonHasTransparentBackground(comboBox, state);
		}
		return new VisualStyleRenderer(element).IsBackgroundPartiallyTransparent();
	}

	public override bool ComboBoxDropDownButtonHasHotElementStyle(ComboBox comboBox)
	{
		if (!RenderClientAreas)
		{
			return base.ComboBoxDropDownButtonHasHotElementStyle(comboBox);
		}
		FlatStyle flatStyle = comboBox.FlatStyle;
		if (flatStyle == FlatStyle.Flat || flatStyle == FlatStyle.Popup)
		{
			return base.ComboBoxDropDownButtonHasHotElementStyle(comboBox);
		}
		return true;
	}

	private static bool ComboBoxShouldPaintBackground(ComboBox comboBox)
	{
		if (comboBox.DropDownStyle == ComboBoxStyle.Simple)
		{
			return false;
		}
		FlatStyle flatStyle = comboBox.FlatStyle;
		if (flatStyle == FlatStyle.Flat || flatStyle == FlatStyle.Popup)
		{
			return false;
		}
		return true;
	}

	public override void ComboBoxDrawBackground(ComboBox comboBox, Graphics g, Rectangle clippingArea, FlatStyle style)
	{
		if (!RenderClientAreas || !ComboBoxShouldPaintBackground(comboBox))
		{
			base.ComboBoxDrawBackground(comboBox, g, clippingArea, style);
			return;
		}
		VisualStyleElement element = ((!comboBox.Enabled) ? VisualStyleElement.ComboBox.Border.Disabled : (comboBox.Entered ? VisualStyleElement.ComboBox.Border.Hot : ((!comboBox.Focused) ? VisualStyleElement.ComboBox.Border.Normal : VisualStyleElement.ComboBox.Border.Focused)));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.ComboBoxDrawBackground(comboBox, g, clippingArea, style);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(g, new Rectangle(Point.Empty, comboBox.Size), clippingArea);
		}
	}

	public override bool CombBoxBackgroundHasHotElementStyle(ComboBox comboBox)
	{
		if (RenderClientAreas && ComboBoxShouldPaintBackground(comboBox) && comboBox.Enabled && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ComboBox.Border.Hot))
		{
			return true;
		}
		return base.CombBoxBackgroundHasHotElementStyle(comboBox);
	}

	public override void CPDrawButton(Graphics dc, Rectangle rectangle, ButtonState state)
	{
		if (!RenderClientAreas || (state & ButtonState.Flat) == ButtonState.Flat || (state & ButtonState.Checked) == ButtonState.Checked)
		{
			base.CPDrawButton(dc, rectangle, state);
			return;
		}
		VisualStyleElement element = (((state & ButtonState.Inactive) == ButtonState.Inactive) ? VisualStyleElement.Button.PushButton.Disabled : (((state & ButtonState.Pushed) != ButtonState.Pushed) ? VisualStyleElement.Button.PushButton.Normal : VisualStyleElement.Button.PushButton.Pressed));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.CPDrawButton(dc, rectangle, state);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, rectangle);
		}
	}

	public override void CPDrawCaptionButton(Graphics graphics, Rectangle rectangle, CaptionButton button, ButtonState state)
	{
		if (!RenderClientAreas || (state & ButtonState.Flat) == ButtonState.Flat || (state & ButtonState.Checked) == ButtonState.Checked)
		{
			base.CPDrawCaptionButton(graphics, rectangle, button, state);
			return;
		}
		VisualStyleElement captionButtonVisualStyleElement = GetCaptionButtonVisualStyleElement(button, state);
		if (!VisualStyleRenderer.IsElementDefined(captionButtonVisualStyleElement))
		{
			base.CPDrawCaptionButton(graphics, rectangle, button, state);
		}
		else
		{
			new VisualStyleRenderer(captionButtonVisualStyleElement).DrawBackground(graphics, rectangle);
		}
	}

	private static VisualStyleElement GetCaptionButtonVisualStyleElement(CaptionButton button, ButtonState state)
	{
		switch (button)
		{
		case CaptionButton.Minimize:
			if ((state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				return VisualStyleElement.Window.MinButton.Disabled;
			}
			if ((state & ButtonState.Pushed) == ButtonState.Pushed)
			{
				return VisualStyleElement.Window.MinButton.Pressed;
			}
			return VisualStyleElement.Window.MinButton.Normal;
		case CaptionButton.Maximize:
			if ((state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				return VisualStyleElement.Window.MaxButton.Disabled;
			}
			if ((state & ButtonState.Pushed) == ButtonState.Pushed)
			{
				return VisualStyleElement.Window.MaxButton.Pressed;
			}
			return VisualStyleElement.Window.MaxButton.Normal;
		case CaptionButton.Close:
			if ((state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				return VisualStyleElement.Window.CloseButton.Disabled;
			}
			if ((state & ButtonState.Pushed) == ButtonState.Pushed)
			{
				return VisualStyleElement.Window.CloseButton.Pressed;
			}
			return VisualStyleElement.Window.CloseButton.Normal;
		case CaptionButton.Restore:
			if ((state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				return VisualStyleElement.Window.RestoreButton.Disabled;
			}
			if ((state & ButtonState.Pushed) == ButtonState.Pushed)
			{
				return VisualStyleElement.Window.RestoreButton.Pressed;
			}
			return VisualStyleElement.Window.RestoreButton.Normal;
		default:
			if ((state & ButtonState.Inactive) == ButtonState.Inactive)
			{
				return VisualStyleElement.Window.HelpButton.Disabled;
			}
			if ((state & ButtonState.Pushed) == ButtonState.Pushed)
			{
				return VisualStyleElement.Window.HelpButton.Pressed;
			}
			return VisualStyleElement.Window.HelpButton.Normal;
		}
	}

	public override void CPDrawCheckBox(Graphics dc, Rectangle rectangle, ButtonState state)
	{
		if (!RenderClientAreas || (state & ButtonState.Flat) == ButtonState.Flat)
		{
			base.CPDrawCheckBox(dc, rectangle, state);
			return;
		}
		VisualStyleElement element = (((state & ButtonState.Checked) == ButtonState.Checked) ? (((state & ButtonState.Inactive) == ButtonState.Inactive) ? VisualStyleElement.Button.CheckBox.CheckedDisabled : (((state & ButtonState.Pushed) != ButtonState.Pushed) ? VisualStyleElement.Button.CheckBox.CheckedNormal : VisualStyleElement.Button.CheckBox.CheckedPressed)) : (((state & ButtonState.Inactive) == ButtonState.Inactive) ? VisualStyleElement.Button.CheckBox.UncheckedDisabled : (((state & ButtonState.Pushed) != ButtonState.Pushed) ? VisualStyleElement.Button.CheckBox.UncheckedNormal : VisualStyleElement.Button.CheckBox.UncheckedPressed)));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.CPDrawCheckBox(dc, rectangle, state);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, rectangle);
		}
	}

	public override void CPDrawComboButton(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		if (!RenderClientAreas || (state & ButtonState.Flat) == ButtonState.Flat || (state & ButtonState.Checked) == ButtonState.Checked)
		{
			base.CPDrawComboButton(graphics, rectangle, state);
			return;
		}
		VisualStyleElement element = (((state & ButtonState.Inactive) == ButtonState.Inactive) ? VisualStyleElement.ComboBox.DropDownButton.Disabled : (((state & ButtonState.Pushed) != ButtonState.Pushed) ? VisualStyleElement.ComboBox.DropDownButton.Normal : VisualStyleElement.ComboBox.DropDownButton.Pressed));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.CPDrawComboButton(graphics, rectangle, state);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(graphics, rectangle);
		}
	}

	public override void CPDrawMixedCheckBox(Graphics dc, Rectangle rectangle, ButtonState state)
	{
		if (!RenderClientAreas || (state & ButtonState.Flat) == ButtonState.Flat)
		{
			base.CPDrawMixedCheckBox(dc, rectangle, state);
			return;
		}
		VisualStyleElement element = (((state & ButtonState.Checked) == ButtonState.Checked) ? (((state & ButtonState.Inactive) == ButtonState.Inactive) ? VisualStyleElement.Button.CheckBox.MixedDisabled : (((state & ButtonState.Pushed) != ButtonState.Pushed) ? VisualStyleElement.Button.CheckBox.MixedNormal : VisualStyleElement.Button.CheckBox.MixedPressed)) : (((state & ButtonState.Inactive) == ButtonState.Inactive) ? VisualStyleElement.Button.CheckBox.UncheckedDisabled : (((state & ButtonState.Pushed) != ButtonState.Pushed) ? VisualStyleElement.Button.CheckBox.UncheckedNormal : VisualStyleElement.Button.CheckBox.UncheckedPressed)));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.CPDrawMixedCheckBox(dc, rectangle, state);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, rectangle);
		}
	}

	public override void CPDrawRadioButton(Graphics dc, Rectangle rectangle, ButtonState state)
	{
		if (!RenderClientAreas || (state & ButtonState.Flat) == ButtonState.Flat)
		{
			base.CPDrawRadioButton(dc, rectangle, state);
			return;
		}
		VisualStyleElement element = (((state & ButtonState.Checked) == ButtonState.Checked) ? (((state & ButtonState.Inactive) == ButtonState.Inactive) ? VisualStyleElement.Button.RadioButton.CheckedDisabled : (((state & ButtonState.Pushed) != ButtonState.Pushed) ? VisualStyleElement.Button.RadioButton.CheckedNormal : VisualStyleElement.Button.RadioButton.CheckedPressed)) : (((state & ButtonState.Inactive) == ButtonState.Inactive) ? VisualStyleElement.Button.RadioButton.UncheckedDisabled : (((state & ButtonState.Pushed) != ButtonState.Pushed) ? VisualStyleElement.Button.RadioButton.UncheckedNormal : VisualStyleElement.Button.RadioButton.UncheckedPressed)));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.CPDrawRadioButton(dc, rectangle, state);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, rectangle);
		}
	}

	public override void CPDrawScrollButton(Graphics dc, Rectangle area, ScrollButton type, ButtonState state)
	{
		if (!RenderClientAreas || (state & ButtonState.Flat) == ButtonState.Flat || (state & ButtonState.Checked) == ButtonState.Checked)
		{
			base.CPDrawScrollButton(dc, area, type, state);
			return;
		}
		VisualStyleElement scrollButtonVisualStyleElement = GetScrollButtonVisualStyleElement(type, state);
		if (!VisualStyleRenderer.IsElementDefined(scrollButtonVisualStyleElement))
		{
			base.CPDrawScrollButton(dc, area, type, state);
		}
		else
		{
			new VisualStyleRenderer(scrollButtonVisualStyleElement).DrawBackground(dc, area);
		}
	}

	private static VisualStyleElement GetScrollButtonVisualStyleElement(ScrollButton type, ButtonState state)
	{
		switch (type)
		{
		case ScrollButton.Left:
			if (IsDisabled(state))
			{
				return VisualStyleElement.ScrollBar.ArrowButton.LeftDisabled;
			}
			if (IsPressed(state))
			{
				return VisualStyleElement.ScrollBar.ArrowButton.LeftPressed;
			}
			return VisualStyleElement.ScrollBar.ArrowButton.LeftNormal;
		case ScrollButton.Right:
			if (IsDisabled(state))
			{
				return VisualStyleElement.ScrollBar.ArrowButton.RightDisabled;
			}
			if (IsPressed(state))
			{
				return VisualStyleElement.ScrollBar.ArrowButton.RightPressed;
			}
			return VisualStyleElement.ScrollBar.ArrowButton.RightNormal;
		case ScrollButton.Min:
			if (IsDisabled(state))
			{
				return VisualStyleElement.ScrollBar.ArrowButton.UpDisabled;
			}
			if (IsPressed(state))
			{
				return VisualStyleElement.ScrollBar.ArrowButton.UpPressed;
			}
			return VisualStyleElement.ScrollBar.ArrowButton.UpNormal;
		default:
			if (IsDisabled(state))
			{
				return VisualStyleElement.ScrollBar.ArrowButton.DownDisabled;
			}
			if (IsPressed(state))
			{
				return VisualStyleElement.ScrollBar.ArrowButton.DownPressed;
			}
			return VisualStyleElement.ScrollBar.ArrowButton.DownNormal;
		}
	}

	private static bool IsDisabled(ButtonState state)
	{
		return (state & ButtonState.Inactive) == ButtonState.Inactive;
	}

	private static bool IsPressed(ButtonState state)
	{
		return (state & ButtonState.Pushed) == ButtonState.Pushed;
	}

	public override bool DataGridViewRowHeaderCellDrawBackground(DataGridViewRowHeaderCell cell, Graphics g, Rectangle bounds)
	{
		if (!RenderClientAreas || !cell.DataGridView.EnableHeadersVisualStyles)
		{
			return base.DataGridViewRowHeaderCellDrawBackground(cell, g, bounds);
		}
		VisualStyleElement visualStyleElement = DataGridViewRowHeaderCellGetVisualStyleElement(cell);
		if (!VisualStyleRenderer.IsElementDefined(visualStyleElement))
		{
			return base.DataGridViewRowHeaderCellDrawBackground(cell, g, bounds);
		}
		bounds.Width--;
		Bitmap bitmap = new Bitmap(bounds.Height, bounds.Width);
		Graphics graphics = Graphics.FromImage(bitmap);
		Rectangle bounds2 = new Rectangle(Point.Empty, bitmap.Size);
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(visualStyleElement);
		if (!AreEqual(visualStyleElement, VisualStyleElement.Header.Item.Normal) && visualStyleRenderer.IsBackgroundPartiallyTransparent())
		{
			new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal).DrawBackground(graphics, bounds2);
		}
		visualStyleRenderer.DrawBackground(graphics, bounds2);
		graphics.Dispose();
		g.Transform = new Matrix(0f, 1f, 1f, 0f, 0f, 0f);
		g.DrawImage(bitmap, bounds.Y, bounds.X);
		bitmap.Dispose();
		g.ResetTransform();
		return true;
	}

	public override bool DataGridViewRowHeaderCellDrawSelectionBackground(DataGridViewRowHeaderCell cell)
	{
		if (!RenderClientAreas || !cell.DataGridView.EnableHeadersVisualStyles || !VisualStyleRenderer.IsElementDefined(DataGridViewRowHeaderCellGetVisualStyleElement(cell)))
		{
			return base.DataGridViewRowHeaderCellDrawSelectionBackground(cell);
		}
		return true;
	}

	public override bool DataGridViewRowHeaderCellDrawBorder(DataGridViewRowHeaderCell cell, Graphics g, Rectangle bounds)
	{
		if (!RenderClientAreas || !cell.DataGridView.EnableHeadersVisualStyles || !VisualStyleRenderer.IsElementDefined(DataGridViewRowHeaderCellGetVisualStyleElement(cell)))
		{
			return base.DataGridViewRowHeaderCellDrawBorder(cell, g, bounds);
		}
		g.DrawLine(cell.GetBorderPen(), bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
		return true;
	}

	private static VisualStyleElement DataGridViewRowHeaderCellGetVisualStyleElement(DataGridViewRowHeaderCell cell)
	{
		if (cell.DataGridView.PressedHeaderCell == cell)
		{
			return VisualStyleElement.Header.Item.Pressed;
		}
		if (cell.DataGridView.EnteredHeaderCell == cell)
		{
			return VisualStyleElement.Header.Item.Hot;
		}
		if (cell.OwningRow.SelectedInternal)
		{
			return VisualStyleElement.Header.Item.Pressed;
		}
		return VisualStyleElement.Header.Item.Normal;
	}

	public override bool DataGridViewColumnHeaderCellDrawBackground(DataGridViewColumnHeaderCell cell, Graphics g, Rectangle bounds)
	{
		if (!RenderClientAreas || !cell.DataGridView.EnableHeadersVisualStyles || cell is DataGridViewTopLeftHeaderCell)
		{
			return base.DataGridViewColumnHeaderCellDrawBackground(cell, g, bounds);
		}
		VisualStyleElement visualStyleElement = DataGridViewColumnHeaderCellGetVisualStyleElement(cell);
		if (!VisualStyleRenderer.IsElementDefined(visualStyleElement))
		{
			return base.DataGridViewColumnHeaderCellDrawBackground(cell, g, bounds);
		}
		bounds.Height--;
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(visualStyleElement);
		if (!AreEqual(visualStyleElement, VisualStyleElement.Header.Item.Normal) && visualStyleRenderer.IsBackgroundPartiallyTransparent())
		{
			new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal).DrawBackground(g, bounds);
		}
		visualStyleRenderer.DrawBackground(g, bounds);
		return true;
	}

	public override bool DataGridViewColumnHeaderCellDrawBorder(DataGridViewColumnHeaderCell cell, Graphics g, Rectangle bounds)
	{
		if (!RenderClientAreas || !cell.DataGridView.EnableHeadersVisualStyles || cell is DataGridViewTopLeftHeaderCell || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.Header.Item.Normal))
		{
			return base.DataGridViewColumnHeaderCellDrawBorder(cell, g, bounds);
		}
		g.DrawLine(cell.GetBorderPen(), bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
		return true;
	}

	private static VisualStyleElement DataGridViewColumnHeaderCellGetVisualStyleElement(DataGridViewColumnHeaderCell cell)
	{
		if (cell.DataGridView.PressedHeaderCell == cell)
		{
			return VisualStyleElement.Header.Item.Pressed;
		}
		if (cell.DataGridView.EnteredHeaderCell == cell)
		{
			return VisualStyleElement.Header.Item.Hot;
		}
		return VisualStyleElement.Header.Item.Normal;
	}

	public override bool DataGridViewHeaderCellHasPressedStyle(DataGridView dataGridView)
	{
		if (!RenderClientAreas || !dataGridView.EnableHeadersVisualStyles || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.Header.Item.Pressed))
		{
			return base.DataGridViewHeaderCellHasPressedStyle(dataGridView);
		}
		return true;
	}

	public override bool DataGridViewHeaderCellHasHotStyle(DataGridView dataGridView)
	{
		if (!RenderClientAreas || !dataGridView.EnableHeadersVisualStyles || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.Header.Item.Hot))
		{
			return base.DataGridViewHeaderCellHasHotStyle(dataGridView);
		}
		return true;
	}

	protected override void DateTimePickerDrawBorder(DateTimePicker dateTimePicker, Graphics g, Rectangle clippingArea)
	{
		if (!RenderClientAreas)
		{
			base.DateTimePickerDrawBorder(dateTimePicker, g, clippingArea);
			return;
		}
		VisualStyleElement element = ((!dateTimePicker.Enabled) ? VisualStyleElement.DatePicker.DateBorder.Disabled : (dateTimePicker.Entered ? VisualStyleElement.DatePicker.DateBorder.Hot : ((!dateTimePicker.Focused) ? VisualStyleElement.DatePicker.DateBorder.Normal : VisualStyleElement.DatePicker.DateBorder.Focused)));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.DateTimePickerDrawBorder(dateTimePicker, g, clippingArea);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(g, new Rectangle(Point.Empty, dateTimePicker.Size), clippingArea);
		}
	}

	protected override void DateTimePickerDrawDropDownButton(DateTimePicker dateTimePicker, Graphics g, Rectangle clippingArea)
	{
		if (!RenderClientAreas)
		{
			base.DateTimePickerDrawDropDownButton(dateTimePicker, g, clippingArea);
			return;
		}
		VisualStyleElement element = ((!dateTimePicker.Enabled) ? VisualStyleElement.DatePicker.ShowCalendarButtonRight.Disabled : (dateTimePicker.is_drop_down_visible ? VisualStyleElement.DatePicker.ShowCalendarButtonRight.Pressed : ((!dateTimePicker.DropDownButtonEntered) ? VisualStyleElement.DatePicker.ShowCalendarButtonRight.Normal : VisualStyleElement.DatePicker.ShowCalendarButtonRight.Hot)));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.DateTimePickerDrawDropDownButton(dateTimePicker, g, clippingArea);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(g, dateTimePicker.drop_down_arrow_rect, clippingArea);
		}
	}

	public override Rectangle DateTimePickerGetDropDownButtonArea(DateTimePicker dateTimePicker)
	{
		if (!RenderClientAreas)
		{
			return base.DateTimePickerGetDropDownButtonArea(dateTimePicker);
		}
		VisualStyleElement pressed = VisualStyleElement.DatePicker.ShowCalendarButtonRight.Pressed;
		if (!VisualStyleRenderer.IsElementDefined(pressed))
		{
			return base.DateTimePickerGetDropDownButtonArea(dateTimePicker);
		}
		Size size = new Size(34, 20);
		return new Rectangle(dateTimePicker.Width - size.Width, 0, size.Width, size.Height);
	}

	public override Rectangle DateTimePickerGetDateArea(DateTimePicker dateTimePicker)
	{
		if (!RenderClientAreas || dateTimePicker.ShowUpDown)
		{
			return base.DateTimePickerGetDateArea(dateTimePicker);
		}
		VisualStyleElement normal = VisualStyleElement.DatePicker.DateBorder.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			return base.DateTimePickerGetDateArea(dateTimePicker);
		}
		Graphics graphics = dateTimePicker.CreateGraphics();
		Rectangle backgroundContentRectangle = new VisualStyleRenderer(normal).GetBackgroundContentRectangle(graphics, dateTimePicker.ClientRectangle);
		graphics.Dispose();
		backgroundContentRectangle.Width -= 34;
		return backgroundContentRectangle;
	}

	protected override void ListViewDrawColumnHeaderBackground(ListView listView, ColumnHeader columnHeader, Graphics g, Rectangle area, Rectangle clippingArea)
	{
		if (!RenderClientAreas)
		{
			base.ListViewDrawColumnHeaderBackground(listView, columnHeader, g, area, clippingArea);
			return;
		}
		VisualStyleElement element = ((listView.HeaderStyle != ColumnHeaderStyle.Clickable) ? VisualStyleElement.Header.Item.Normal : (columnHeader.Pressed ? VisualStyleElement.Header.Item.Pressed : ((columnHeader != listView.EnteredColumnHeader) ? VisualStyleElement.Header.Item.Normal : VisualStyleElement.Header.Item.Hot)));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.ListViewDrawColumnHeaderBackground(listView, columnHeader, g, area, clippingArea);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(g, area, clippingArea);
		}
	}

	protected override void ListViewDrawUnusedHeaderBackground(ListView listView, Graphics g, Rectangle area, Rectangle clippingArea)
	{
		if (!RenderClientAreas)
		{
			base.ListViewDrawUnusedHeaderBackground(listView, g, area, clippingArea);
			return;
		}
		VisualStyleElement normal = VisualStyleElement.Header.Item.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			base.ListViewDrawUnusedHeaderBackground(listView, g, area, clippingArea);
		}
		else
		{
			new VisualStyleRenderer(normal).DrawBackground(g, area, clippingArea);
		}
	}

	public override int ListViewGetHeaderHeight(ListView listView, Font font)
	{
		if (!RenderClientAreas)
		{
			return base.ListViewGetHeaderHeight(listView, font);
		}
		VisualStyleElement normal = VisualStyleElement.Header.Item.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			return base.ListViewGetHeaderHeight(listView, font);
		}
		Control control = null;
		Graphics graphics;
		if (listView == null)
		{
			control = new Control();
			graphics = control.CreateGraphics();
		}
		else
		{
			graphics = listView.CreateGraphics();
		}
		int height = new VisualStyleRenderer(normal).GetPartSize(graphics, ThemeSizeType.True).Height;
		graphics.Dispose();
		if (listView == null)
		{
			control.Dispose();
		}
		return height;
	}

	public override void DrawGroupBox(Graphics dc, Rectangle area, GroupBox box)
	{
		GroupBoxRenderer.DrawGroupBox(dc, new Rectangle(Point.Empty, box.Size), box.Text, box.Font, (!(box.ForeColor == Control.DefaultForeColor)) ? box.ForeColor : Color.Empty, box.Enabled ? GroupBoxState.Normal : GroupBoxState.Disabled);
	}

	private Rectangle ManagedWindowGetTitleBarRectangle(InternalWindowManager wm)
	{
		return new Rectangle(0, 0, wm.Form.Width, ManagedWindowTitleBarHeight(wm) + ManagedWindowBorderWidth(wm) * ((!wm.IsMinimized) ? 1 : 2));
	}

	private Region ManagedWindowGetWindowRegion(Form form)
	{
		if (form.WindowManager is MdiWindowManager && form.WindowManager.IsMaximized)
		{
			return null;
		}
		VisualStyleElement element = ManagedWindowGetTitleBarVisualStyleElement(form.WindowManager);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			return null;
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(element);
		if (!visualStyleRenderer.IsBackgroundPartiallyTransparent())
		{
			return null;
		}
		IDeviceContext measurementDeviceContext = GetMeasurementDeviceContext();
		Rectangle bounds = ManagedWindowGetTitleBarRectangle(form.WindowManager);
		Region backgroundRegion = visualStyleRenderer.GetBackgroundRegion(measurementDeviceContext, bounds);
		ReleaseMeasurementDeviceContext(measurementDeviceContext);
		backgroundRegion.Union(new Rectangle(0, bounds.Bottom, form.Width, form.Height));
		return backgroundRegion;
	}

	public override void ManagedWindowOnSizeInitializedOrChanged(Form form)
	{
		base.ManagedWindowOnSizeInitializedOrChanged(form);
		if (render_non_client_areas)
		{
			form.Region = ManagedWindowGetWindowRegion(form);
		}
	}

	protected override Rectangle ManagedWindowDrawTitleBarAndBorders(Graphics dc, Rectangle clip, InternalWindowManager wm)
	{
		if (!render_non_client_areas)
		{
			return base.ManagedWindowDrawTitleBarAndBorders(dc, clip, wm);
		}
		VisualStyleElement element = ManagedWindowGetTitleBarVisualStyleElement(wm);
		ManagedWindowGetBorderVisualStyleElements(wm, out var left, out var right, out var bottom);
		if (!VisualStyleRenderer.IsElementDefined(element) || (!wm.IsMinimized && (!VisualStyleRenderer.IsElementDefined(left) || !VisualStyleRenderer.IsElementDefined(right) || !VisualStyleRenderer.IsElementDefined(bottom))))
		{
			return base.ManagedWindowDrawTitleBarAndBorders(dc, clip, wm);
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(element);
		Rectangle rectangle = ManagedWindowGetTitleBarRectangle(wm);
		visualStyleRenderer.DrawBackground(dc, rectangle, clip);
		if (!wm.IsMinimized)
		{
			int num = ManagedWindowBorderWidth(wm);
			visualStyleRenderer.SetParameters(left);
			visualStyleRenderer.DrawBackground(dc, new Rectangle(0, rectangle.Bottom, num, wm.Form.Height - rectangle.Bottom), clip);
			visualStyleRenderer.SetParameters(right);
			visualStyleRenderer.DrawBackground(dc, new Rectangle(wm.Form.Width - num, rectangle.Bottom, num, wm.Form.Height - rectangle.Bottom), clip);
			visualStyleRenderer.SetParameters(bottom);
			visualStyleRenderer.DrawBackground(dc, new Rectangle(0, wm.Form.Height - num, wm.Form.Width, num), clip);
		}
		return rectangle;
	}

	private static FormWindowState ManagedWindowGetWindowState(InternalWindowManager wm)
	{
		return wm.GetWindowState();
	}

	private static bool ManagedWindowIsDisabled(InternalWindowManager wm)
	{
		return !wm.Form.Enabled;
	}

	private static bool ManagedWindowIsActive(InternalWindowManager wm)
	{
		return wm.IsActive;
	}

	private static VisualStyleElement ManagedWindowGetTitleBarVisualStyleElement(InternalWindowManager wm)
	{
		if (wm.IsToolWindow)
		{
			switch (ManagedWindowGetWindowState(wm))
			{
			case FormWindowState.Minimized:
				if (ManagedWindowIsDisabled(wm))
				{
					return VisualStyleElement.Window.SmallMinCaption.Disabled;
				}
				if (ManagedWindowIsActive(wm))
				{
					return VisualStyleElement.Window.SmallMinCaption.Active;
				}
				return VisualStyleElement.Window.SmallMinCaption.Inactive;
			case FormWindowState.Maximized:
				if (ManagedWindowIsDisabled(wm))
				{
					return VisualStyleElement.Window.SmallMaxCaption.Disabled;
				}
				if (ManagedWindowIsActive(wm))
				{
					return VisualStyleElement.Window.SmallMaxCaption.Active;
				}
				return VisualStyleElement.Window.SmallMaxCaption.Inactive;
			default:
				if (ManagedWindowIsDisabled(wm))
				{
					return VisualStyleElement.Window.SmallCaption.Disabled;
				}
				if (ManagedWindowIsActive(wm))
				{
					return VisualStyleElement.Window.SmallCaption.Active;
				}
				return VisualStyleElement.Window.SmallCaption.Inactive;
			}
		}
		switch (ManagedWindowGetWindowState(wm))
		{
		case FormWindowState.Minimized:
			if (ManagedWindowIsDisabled(wm))
			{
				return VisualStyleElement.Window.MinCaption.Disabled;
			}
			if (ManagedWindowIsActive(wm))
			{
				return VisualStyleElement.Window.MinCaption.Active;
			}
			return VisualStyleElement.Window.MinCaption.Inactive;
		case FormWindowState.Maximized:
			if (ManagedWindowIsDisabled(wm))
			{
				return VisualStyleElement.Window.MaxCaption.Disabled;
			}
			if (ManagedWindowIsActive(wm))
			{
				return VisualStyleElement.Window.MaxCaption.Active;
			}
			return VisualStyleElement.Window.MaxCaption.Inactive;
		default:
			if (ManagedWindowIsDisabled(wm))
			{
				return VisualStyleElement.Window.Caption.Disabled;
			}
			if (ManagedWindowIsActive(wm))
			{
				return VisualStyleElement.Window.Caption.Active;
			}
			return VisualStyleElement.Window.Caption.Inactive;
		}
	}

	private static void ManagedWindowGetBorderVisualStyleElements(InternalWindowManager wm, out VisualStyleElement left, out VisualStyleElement right, out VisualStyleElement bottom)
	{
		bool flag = !ManagedWindowIsDisabled(wm) && ManagedWindowIsActive(wm);
		if (wm.IsToolWindow)
		{
			if (flag)
			{
				left = VisualStyleElement.Window.SmallFrameLeft.Active;
				right = VisualStyleElement.Window.SmallFrameRight.Active;
				bottom = VisualStyleElement.Window.SmallFrameBottom.Active;
			}
			else
			{
				left = VisualStyleElement.Window.SmallFrameLeft.Inactive;
				right = VisualStyleElement.Window.SmallFrameRight.Inactive;
				bottom = VisualStyleElement.Window.SmallFrameBottom.Inactive;
			}
		}
		else if (flag)
		{
			left = VisualStyleElement.Window.FrameLeft.Active;
			right = VisualStyleElement.Window.FrameRight.Active;
			bottom = VisualStyleElement.Window.FrameBottom.Active;
		}
		else
		{
			left = VisualStyleElement.Window.FrameLeft.Inactive;
			right = VisualStyleElement.Window.FrameRight.Inactive;
			bottom = VisualStyleElement.Window.FrameBottom.Inactive;
		}
	}

	public override bool ManagedWindowTitleButtonHasHotElementStyle(TitleButton button, Form form)
	{
		if (render_non_client_areas && (button.State & ButtonState.Inactive) != ButtonState.Inactive)
		{
			VisualStyleElement element = (ManagedWindowIsMaximizedMdiChild(form) ? (button.Caption switch
			{
				CaptionButton.Close => VisualStyleElement.Window.MdiCloseButton.Hot, 
				CaptionButton.Help => VisualStyleElement.Window.MdiHelpButton.Hot, 
				CaptionButton.Minimize => VisualStyleElement.Window.MdiMinButton.Hot, 
				_ => VisualStyleElement.Window.MdiRestoreButton.Hot, 
			}) : (form.WindowManager.IsToolWindow ? VisualStyleElement.Window.SmallCloseButton.Hot : (button.Caption switch
			{
				CaptionButton.Close => VisualStyleElement.Window.CloseButton.Hot, 
				CaptionButton.Help => VisualStyleElement.Window.HelpButton.Hot, 
				CaptionButton.Maximize => VisualStyleElement.Window.MaxButton.Hot, 
				CaptionButton.Minimize => VisualStyleElement.Window.MinButton.Hot, 
				_ => VisualStyleElement.Window.RestoreButton.Hot, 
			})));
			if (VisualStyleRenderer.IsElementDefined(element))
			{
				return true;
			}
		}
		return base.ManagedWindowTitleButtonHasHotElementStyle(button, form);
	}

	private static bool ManagedWindowIsMaximizedMdiChild(Form form)
	{
		return form.WindowManager is MdiWindowManager && ManagedWindowGetWindowState(form.WindowManager) == FormWindowState.Maximized;
	}

	private static bool ManagedWindowTitleButtonIsDisabled(TitleButton button, InternalWindowManager wm)
	{
		return (button.State & ButtonState.Inactive) == ButtonState.Inactive;
	}

	private static bool ManagedWindowTitleButtonIsPressed(TitleButton button)
	{
		return (button.State & ButtonState.Pushed) == ButtonState.Pushed;
	}

	private static VisualStyleElement ManagedWindowGetTitleButtonVisualStyleElement(TitleButton button, Form form)
	{
		if (form.WindowManager.IsToolWindow)
		{
			if (ManagedWindowTitleButtonIsDisabled(button, form.WindowManager))
			{
				return VisualStyleElement.Window.SmallCloseButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.SmallCloseButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.SmallCloseButton.Hot;
			}
			return VisualStyleElement.Window.SmallCloseButton.Normal;
		}
		switch (button.Caption)
		{
		case CaptionButton.Close:
			if (ManagedWindowTitleButtonIsDisabled(button, form.WindowManager))
			{
				return VisualStyleElement.Window.CloseButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.CloseButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.CloseButton.Hot;
			}
			return VisualStyleElement.Window.CloseButton.Normal;
		case CaptionButton.Help:
			if (ManagedWindowTitleButtonIsDisabled(button, form.WindowManager))
			{
				return VisualStyleElement.Window.HelpButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.HelpButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.HelpButton.Hot;
			}
			return VisualStyleElement.Window.HelpButton.Normal;
		case CaptionButton.Maximize:
			if (ManagedWindowTitleButtonIsDisabled(button, form.WindowManager))
			{
				return VisualStyleElement.Window.MaxButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.MaxButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.MaxButton.Hot;
			}
			return VisualStyleElement.Window.MaxButton.Normal;
		case CaptionButton.Minimize:
			if (ManagedWindowTitleButtonIsDisabled(button, form.WindowManager))
			{
				return VisualStyleElement.Window.MinButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.MinButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.MinButton.Hot;
			}
			return VisualStyleElement.Window.MinButton.Normal;
		default:
			if (ManagedWindowTitleButtonIsDisabled(button, form.WindowManager))
			{
				return VisualStyleElement.Window.RestoreButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.RestoreButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.RestoreButton.Hot;
			}
			return VisualStyleElement.Window.RestoreButton.Normal;
		}
	}

	protected override void ManagedWindowDrawTitleButton(Graphics dc, TitleButton button, Rectangle clip, Form form)
	{
		if (!render_non_client_areas)
		{
			base.ManagedWindowDrawTitleButton(dc, button, clip, form);
			return;
		}
		VisualStyleElement element = ManagedWindowGetTitleButtonVisualStyleElement(button, form);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.ManagedWindowDrawTitleButton(dc, button, clip, form);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, button.Rectangle, clip);
		}
	}

	public override Size ManagedWindowButtonSize(InternalWindowManager wm)
	{
		if (!render_non_client_areas)
		{
			return base.ManagedWindowButtonSize(wm);
		}
		VisualStyleElement element = ((!wm.IsToolWindow || wm.IsMinimized) ? VisualStyleElement.Window.CloseButton.Normal : VisualStyleElement.Window.SmallCloseButton.Normal);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			return base.ManagedWindowButtonSize(wm);
		}
		IDeviceContext measurementDeviceContext = GetMeasurementDeviceContext();
		Size partSize = new VisualStyleRenderer(element).GetPartSize(measurementDeviceContext, ThemeSizeType.True);
		ReleaseMeasurementDeviceContext(measurementDeviceContext);
		return partSize;
	}

	public override void ManagedWindowDrawMenuButton(Graphics dc, TitleButton button, Rectangle clip, InternalWindowManager wm)
	{
		if (!render_non_client_areas)
		{
			base.ManagedWindowDrawMenuButton(dc, button, clip, wm);
			return;
		}
		VisualStyleElement element = ManagedWindowGetMenuButtonVisualStyleElement(button, wm);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.ManagedWindowDrawMenuButton(dc, button, clip, wm);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, button.Rectangle, clip);
		}
	}

	private static VisualStyleElement ManagedWindowGetMenuButtonVisualStyleElement(TitleButton button, InternalWindowManager wm)
	{
		switch (button.Caption)
		{
		case CaptionButton.Close:
			if (ManagedWindowTitleButtonIsDisabled(button, wm))
			{
				return VisualStyleElement.Window.MdiCloseButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.MdiCloseButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.MdiCloseButton.Hot;
			}
			return VisualStyleElement.Window.MdiCloseButton.Normal;
		case CaptionButton.Help:
			if (ManagedWindowTitleButtonIsDisabled(button, wm))
			{
				return VisualStyleElement.Window.MdiHelpButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.MdiHelpButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.MdiHelpButton.Hot;
			}
			return VisualStyleElement.Window.MdiHelpButton.Normal;
		case CaptionButton.Minimize:
			if (ManagedWindowTitleButtonIsDisabled(button, wm))
			{
				return VisualStyleElement.Window.MdiMinButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.MdiMinButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.MdiMinButton.Hot;
			}
			return VisualStyleElement.Window.MdiMinButton.Normal;
		default:
			if (ManagedWindowTitleButtonIsDisabled(button, wm))
			{
				return VisualStyleElement.Window.MdiRestoreButton.Disabled;
			}
			if (ManagedWindowTitleButtonIsPressed(button))
			{
				return VisualStyleElement.Window.MdiRestoreButton.Pressed;
			}
			if (button.Entered)
			{
				return VisualStyleElement.Window.MdiRestoreButton.Hot;
			}
			return VisualStyleElement.Window.MdiRestoreButton.Normal;
		}
	}

	public override void DrawProgressBar(Graphics dc, Rectangle clip_rect, ProgressBar ctrl)
	{
		if (!RenderClientAreas || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.ProgressBar.Bar.Normal) || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.ProgressBar.Chunk.Normal))
		{
			base.DrawProgressBar(dc, clip_rect, ctrl);
			return;
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.Bar.Normal);
		visualStyleRenderer.DrawBackground(dc, ctrl.ClientRectangle, clip_rect);
		Rectangle backgroundContentRectangle = visualStyleRenderer.GetBackgroundContentRectangle(dc, new Rectangle(Point.Empty, ctrl.Size));
		visualStyleRenderer = new VisualStyleRenderer(VisualStyleElement.ProgressBar.Chunk.Normal);
		int num = 0;
		int num2 = int.MaxValue;
		int x = backgroundContentRectangle.X;
		switch ((int)ctrl.Style)
		{
		case 1:
			backgroundContentRectangle.Width = (int)((double)backgroundContentRectangle.Width * ((double)(ctrl.Value - ctrl.Minimum) / (double)Math.Max(ctrl.Maximum - ctrl.Minimum, 1)));
			visualStyleRenderer.DrawBackground(dc, backgroundContentRectangle, clip_rect);
			return;
		case 2:
		{
			int num3 = (int)(DateTime.Now - ctrl.start).TotalMilliseconds;
			double num4 = (double)num3 % (double)ctrl.MarqueeAnimationSpeed / (double)ctrl.MarqueeAnimationSpeed;
			num2 = 5;
			x = backgroundContentRectangle.X + (int)((double)backgroundContentRectangle.Width * num4);
			break;
		}
		}
		int integer = visualStyleRenderer.GetInteger(IntegerProperty.ProgressChunkSize);
		integer = Math.Max(integer, 0);
		int num5 = (int)((double)(ctrl.Value - ctrl.Minimum) * (double)backgroundContentRectangle.Width / (double)Math.Max(ctrl.Maximum - ctrl.Minimum, 1)) + backgroundContentRectangle.X;
		int num6 = 0;
		int num7 = integer + visualStyleRenderer.GetInteger(IntegerProperty.ProgressSpaceSize);
		Rectangle rectangle = new Rectangle(x, backgroundContentRectangle.Y, integer, backgroundContentRectangle.Height);
		while (true)
		{
			if (num2 != int.MaxValue)
			{
				if (num6 == num2)
				{
					break;
				}
				if (rectangle.Right >= backgroundContentRectangle.Width)
				{
					rectangle.X -= backgroundContentRectangle.Width;
				}
			}
			else
			{
				if (rectangle.X >= num5)
				{
					break;
				}
				if (rectangle.Right >= num5)
				{
					if (num5 != backgroundContentRectangle.Right)
					{
						break;
					}
					rectangle.Width = num5 - rectangle.X;
				}
			}
			if (clip_rect.IntersectsWith(rectangle))
			{
				visualStyleRenderer.DrawBackground(dc, rectangle, clip_rect);
			}
			rectangle.X += num7;
			num6++;
		}
	}

	protected override void RadioButton_DrawButton(RadioButton radio_button, Graphics dc, ButtonState state, Rectangle radiobutton_rectangle)
	{
		if (radio_button.Appearance == Appearance.Normal && radio_button.FlatStyle == FlatStyle.System)
		{
			RadioButtonRenderer.DrawRadioButton(dc, new Point(radiobutton_rectangle.Left, radiobutton_rectangle.Top), GetRadioButtonState(radio_button));
		}
		else
		{
			base.RadioButton_DrawButton(radio_button, dc, state, radiobutton_rectangle);
		}
	}

	private static RadioButtonState GetRadioButtonState(RadioButton checkBox)
	{
		if (checkBox.Checked)
		{
			if (!checkBox.Enabled)
			{
				return RadioButtonState.CheckedDisabled;
			}
			if (checkBox.Pressed)
			{
				return RadioButtonState.CheckedPressed;
			}
			if (checkBox.Entered)
			{
				return RadioButtonState.CheckedHot;
			}
			return RadioButtonState.CheckedNormal;
		}
		if (!checkBox.Enabled)
		{
			return RadioButtonState.UncheckedDisabled;
		}
		if (checkBox.Pressed)
		{
			return RadioButtonState.UncheckedPressed;
		}
		if (checkBox.Entered)
		{
			return RadioButtonState.UncheckedHot;
		}
		return RadioButtonState.UncheckedNormal;
	}

	public override void DrawScrollBar(Graphics dc, Rectangle clip, ScrollBar bar)
	{
		if (!RenderClientAreas || !ScrollBarAreElementsDefined)
		{
			base.DrawScrollBar(dc, clip, bar);
			return;
		}
		int scrollbutton_width = bar.scrollbutton_width;
		int scrollbutton_height = bar.scrollbutton_height;
		VisualStyleElement element;
		VisualStyleRenderer visualStyleRenderer;
		if (bar.vert)
		{
			bar.FirstArrowArea = new Rectangle(0, 0, bar.Width, scrollbutton_height);
			bar.SecondArrowArea = new Rectangle(0, bar.ClientRectangle.Height - scrollbutton_height, bar.Width, scrollbutton_height);
			Rectangle thumbPos = bar.ThumbPos;
			thumbPos.Width = bar.Width;
			bar.ThumbPos = thumbPos;
			element = ((bar.thumb_moving != ScrollBar.ThumbMoving.Backwards) ? ((!bar.Enabled) ? VisualStyleElement.ScrollBar.LowerTrackVertical.Disabled : VisualStyleElement.ScrollBar.LowerTrackVertical.Normal) : VisualStyleElement.ScrollBar.LowerTrackVertical.Pressed);
			visualStyleRenderer = new VisualStyleRenderer(element);
			Rectangle rectangle = new Rectangle(0, 0, bar.ClientRectangle.Width, bar.ThumbPos.Top);
			if (clip.IntersectsWith(rectangle))
			{
				visualStyleRenderer.DrawBackground(dc, rectangle, clip);
			}
			element = ((bar.thumb_moving != ScrollBar.ThumbMoving.Forward) ? ((!bar.Enabled) ? VisualStyleElement.ScrollBar.LowerTrackVertical.Disabled : VisualStyleElement.ScrollBar.LowerTrackVertical.Normal) : VisualStyleElement.ScrollBar.LowerTrackVertical.Pressed);
			visualStyleRenderer = new VisualStyleRenderer(element);
			Rectangle rectangle2 = new Rectangle(0, bar.ThumbPos.Bottom, bar.ClientRectangle.Width, bar.ClientRectangle.Height - bar.ThumbPos.Bottom);
			if (clip.IntersectsWith(rectangle2))
			{
				visualStyleRenderer.DrawBackground(dc, rectangle2, clip);
			}
			if (clip.IntersectsWith(bar.FirstArrowArea))
			{
				element = ((!bar.Enabled) ? VisualStyleElement.ScrollBar.ArrowButton.UpDisabled : ((bar.firstbutton_state == ButtonState.Pushed) ? VisualStyleElement.ScrollBar.ArrowButton.UpPressed : (bar.FirstButtonEntered ? VisualStyleElement.ScrollBar.ArrowButton.UpHot : ((!ScrollBarHasHoverArrowButtonStyleVisualStyles || !bar.Entered) ? VisualStyleElement.ScrollBar.ArrowButton.UpNormal : VisualStyleElement.ScrollBar.ArrowButton.UpHover))));
				visualStyleRenderer = new VisualStyleRenderer(element);
				visualStyleRenderer.DrawBackground(dc, bar.FirstArrowArea);
			}
			if (clip.IntersectsWith(bar.SecondArrowArea))
			{
				element = ((!bar.Enabled) ? VisualStyleElement.ScrollBar.ArrowButton.DownDisabled : ((bar.secondbutton_state == ButtonState.Pushed) ? VisualStyleElement.ScrollBar.ArrowButton.DownPressed : (bar.SecondButtonEntered ? VisualStyleElement.ScrollBar.ArrowButton.DownHot : ((!ScrollBarHasHoverArrowButtonStyleVisualStyles || !bar.Entered) ? VisualStyleElement.ScrollBar.ArrowButton.DownNormal : VisualStyleElement.ScrollBar.ArrowButton.DownHover))));
				visualStyleRenderer = new VisualStyleRenderer(element);
				visualStyleRenderer.DrawBackground(dc, bar.SecondArrowArea);
			}
			element = ((!bar.Enabled) ? VisualStyleElement.ScrollBar.LowerTrackVertical.Disabled : (bar.ThumbPressed ? VisualStyleElement.ScrollBar.ThumbButtonVertical.Pressed : ((!bar.ThumbEntered) ? VisualStyleElement.ScrollBar.ThumbButtonVertical.Normal : VisualStyleElement.ScrollBar.ThumbButtonVertical.Hot)));
			visualStyleRenderer = new VisualStyleRenderer(element);
			visualStyleRenderer.DrawBackground(dc, bar.ThumbPos, clip);
			if (bar.Enabled && bar.ThumbPos.Height >= 20)
			{
				element = VisualStyleElement.ScrollBar.GripperVertical.Normal;
				if (VisualStyleRenderer.IsElementDefined(element))
				{
					visualStyleRenderer = new VisualStyleRenderer(element);
					visualStyleRenderer.DrawBackground(dc, bar.ThumbPos, clip);
				}
			}
			return;
		}
		bar.FirstArrowArea = new Rectangle(0, 0, scrollbutton_width, bar.Height);
		bar.SecondArrowArea = new Rectangle(bar.ClientRectangle.Width - scrollbutton_width, 0, scrollbutton_width, bar.Height);
		Rectangle thumbPos2 = bar.ThumbPos;
		thumbPos2.Height = bar.Height;
		bar.ThumbPos = thumbPos2;
		element = ((bar.thumb_moving != ScrollBar.ThumbMoving.Backwards) ? ((!bar.Enabled) ? VisualStyleElement.ScrollBar.LeftTrackHorizontal.Disabled : VisualStyleElement.ScrollBar.LeftTrackHorizontal.Normal) : VisualStyleElement.ScrollBar.LeftTrackHorizontal.Pressed);
		visualStyleRenderer = new VisualStyleRenderer(element);
		Rectangle rectangle3 = new Rectangle(0, 0, bar.ThumbPos.Left, bar.ClientRectangle.Height);
		if (clip.IntersectsWith(rectangle3))
		{
			visualStyleRenderer.DrawBackground(dc, rectangle3, clip);
		}
		element = ((bar.thumb_moving != ScrollBar.ThumbMoving.Forward) ? ((!bar.Enabled) ? VisualStyleElement.ScrollBar.RightTrackHorizontal.Disabled : VisualStyleElement.ScrollBar.RightTrackHorizontal.Normal) : VisualStyleElement.ScrollBar.RightTrackHorizontal.Pressed);
		visualStyleRenderer = new VisualStyleRenderer(element);
		Rectangle rectangle4 = new Rectangle(bar.ThumbPos.Right, 0, bar.ClientRectangle.Width - bar.ThumbPos.Right, bar.ClientRectangle.Height);
		if (clip.IntersectsWith(rectangle4))
		{
			visualStyleRenderer.DrawBackground(dc, rectangle4, clip);
		}
		if (clip.IntersectsWith(bar.FirstArrowArea))
		{
			element = ((!bar.Enabled) ? VisualStyleElement.ScrollBar.ArrowButton.LeftDisabled : ((bar.firstbutton_state == ButtonState.Pushed) ? VisualStyleElement.ScrollBar.ArrowButton.LeftPressed : (bar.FirstButtonEntered ? VisualStyleElement.ScrollBar.ArrowButton.LeftHot : ((!ScrollBarHasHoverArrowButtonStyleVisualStyles || !bar.Entered) ? VisualStyleElement.ScrollBar.ArrowButton.LeftNormal : VisualStyleElement.ScrollBar.ArrowButton.LeftHover))));
			visualStyleRenderer = new VisualStyleRenderer(element);
			visualStyleRenderer.DrawBackground(dc, bar.FirstArrowArea);
		}
		if (clip.IntersectsWith(bar.SecondArrowArea))
		{
			element = ((!bar.Enabled) ? VisualStyleElement.ScrollBar.ArrowButton.RightDisabled : ((bar.secondbutton_state == ButtonState.Pushed) ? VisualStyleElement.ScrollBar.ArrowButton.RightPressed : (bar.SecondButtonEntered ? VisualStyleElement.ScrollBar.ArrowButton.RightHot : ((!ScrollBarHasHoverArrowButtonStyleVisualStyles || !bar.Entered) ? VisualStyleElement.ScrollBar.ArrowButton.RightNormal : VisualStyleElement.ScrollBar.ArrowButton.RightHover))));
			visualStyleRenderer = new VisualStyleRenderer(element);
			visualStyleRenderer.DrawBackground(dc, bar.SecondArrowArea);
		}
		element = ((!bar.Enabled) ? VisualStyleElement.ScrollBar.RightTrackHorizontal.Disabled : (bar.ThumbPressed ? VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Pressed : ((!bar.ThumbEntered) ? VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Normal : VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Hot)));
		visualStyleRenderer = new VisualStyleRenderer(element);
		visualStyleRenderer.DrawBackground(dc, bar.ThumbPos, clip);
		if (bar.Enabled && bar.ThumbPos.Height >= 20)
		{
			element = VisualStyleElement.ScrollBar.GripperHorizontal.Normal;
			if (VisualStyleRenderer.IsElementDefined(element))
			{
				visualStyleRenderer = new VisualStyleRenderer(element);
				visualStyleRenderer.DrawBackground(dc, bar.ThumbPos, clip);
			}
		}
	}

	protected override void DrawStatusBarBackground(Graphics dc, Rectangle clip, StatusBar sb)
	{
		if (!RenderClientAreas)
		{
			base.DrawStatusBarBackground(dc, clip, sb);
			return;
		}
		VisualStyleElement normal = VisualStyleElement.Status.Bar.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			base.DrawStatusBarBackground(dc, clip, sb);
		}
		else
		{
			new VisualStyleRenderer(normal).DrawBackground(dc, sb.ClientRectangle, clip);
		}
	}

	protected override void DrawStatusBarSizingGrip(Graphics dc, Rectangle clip, StatusBar sb, Rectangle area)
	{
		if (!RenderClientAreas)
		{
			base.DrawStatusBarSizingGrip(dc, clip, sb, area);
			return;
		}
		VisualStyleElement normal = VisualStyleElement.Status.Gripper.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			base.DrawStatusBarSizingGrip(dc, clip, sb, area);
			return;
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(normal);
		Rectangle bounds = new Rectangle(Point.Empty, visualStyleRenderer.GetPartSize(dc, ThemeSizeType.True));
		bounds.X = sb.Width - bounds.Width;
		bounds.Y = sb.Height - bounds.Height;
		visualStyleRenderer.DrawBackground(dc, bounds, clip);
	}

	protected override void DrawStatusBarPanelBackground(Graphics dc, Rectangle area, StatusBarPanel panel)
	{
		if (!RenderClientAreas)
		{
			base.DrawStatusBarPanelBackground(dc, area, panel);
			return;
		}
		VisualStyleElement normal = VisualStyleElement.Status.Pane.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			base.DrawStatusBarPanelBackground(dc, area, panel);
		}
		else
		{
			new VisualStyleRenderer(normal).DrawBackground(dc, area);
		}
	}

	private static bool TextBoxBaseShouldPaint(TextBoxBase textBoxBase)
	{
		return textBoxBase.BorderStyle == BorderStyle.Fixed3D;
	}

	private static VisualStyleElement TextBoxBaseGetVisualStyleElement(TextBoxBase textBoxBase)
	{
		if (!textBoxBase.Enabled)
		{
			return VisualStyleElement.TextBox.TextEdit.Disabled;
		}
		if (textBoxBase.ReadOnly)
		{
			return VisualStyleElement.TextBox.TextEdit.ReadOnly;
		}
		if (textBoxBase.Entered)
		{
			return VisualStyleElement.TextBox.TextEdit.Hot;
		}
		if (textBoxBase.Focused)
		{
			return VisualStyleElement.TextBox.TextEdit.Focused;
		}
		return VisualStyleElement.TextBox.TextEdit.Normal;
	}

	public override void TextBoxBaseFillBackground(TextBoxBase textBoxBase, Graphics g, Rectangle clippingArea)
	{
		if (!RenderClientAreas || !TextBoxBaseShouldPaint(textBoxBase))
		{
			base.TextBoxBaseFillBackground(textBoxBase, g, clippingArea);
			return;
		}
		VisualStyleElement element = TextBoxBaseGetVisualStyleElement(textBoxBase);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.TextBoxBaseFillBackground(textBoxBase, g, clippingArea);
			return;
		}
		Rectangle bounds = new Rectangle(Point.Empty, textBoxBase.Size);
		bounds.X -= (bounds.Width - textBoxBase.ClientSize.Width) / 2;
		bounds.Y -= (bounds.Height - textBoxBase.ClientSize.Height) / 2;
		new VisualStyleRenderer(element).DrawBackground(g, bounds, clippingArea);
	}

	public override bool TextBoxBaseHandleWmNcPaint(TextBoxBase textBoxBase, ref Message m)
	{
		if (!RenderClientAreas || !TextBoxBaseShouldPaint(textBoxBase))
		{
			return base.TextBoxBaseHandleWmNcPaint(textBoxBase, ref m);
		}
		VisualStyleElement element = TextBoxBaseGetVisualStyleElement(textBoxBase);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			return base.TextBoxBaseHandleWmNcPaint(textBoxBase, ref m);
		}
		PaintEventArgs paintEventArgs = XplatUI.PaintEventStart(ref m, textBoxBase.Handle, client: false);
		new VisualStyleRenderer(element).DrawBackgroundExcludingArea(paintEventArgs.Graphics, new Rectangle(Point.Empty, textBoxBase.Size), new Rectangle(new Point((textBoxBase.Width - textBoxBase.ClientSize.Width) / 2, (textBoxBase.Height - textBoxBase.ClientSize.Height) / 2), textBoxBase.ClientSize));
		XplatUI.PaintEventEnd(ref m, textBoxBase.Handle, client: false);
		return true;
	}

	public override bool TextBoxBaseShouldPaintBackground(TextBoxBase textBoxBase)
	{
		if (!RenderClientAreas || !TextBoxBaseShouldPaint(textBoxBase))
		{
			return base.TextBoxBaseShouldPaintBackground(textBoxBase);
		}
		VisualStyleElement element = TextBoxBaseGetVisualStyleElement(textBoxBase);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			return base.TextBoxBaseShouldPaintBackground(textBoxBase);
		}
		return new VisualStyleRenderer(element).IsBackgroundPartiallyTransparent();
	}

	private static bool ToolBarIsDisabled(ToolBarItem item)
	{
		return !item.Button.Enabled;
	}

	private static bool ToolBarIsPressed(ToolBarItem item)
	{
		return item.Pressed;
	}

	private static bool ToolBarIsChecked(ToolBarItem item)
	{
		return item.Button.Pushed;
	}

	private static bool ToolBarIsHot(ToolBarItem item)
	{
		return item.Hilight;
	}

	protected override void DrawToolBarButtonBorder(Graphics dc, ToolBarItem item, bool is_flat)
	{
		if (!RenderClientAreas)
		{
			base.DrawToolBarButtonBorder(dc, item, is_flat);
		}
		else
		{
			if (item.Button.Style == ToolBarButtonStyle.Separator)
			{
				return;
			}
			VisualStyleElement element = ((item.Button.Style != ToolBarButtonStyle.DropDownButton) ? ToolBarGetButtonVisualStyleElement(item) : ToolBarGetDropDownButtonVisualStyleElement(item));
			if (!VisualStyleRenderer.IsElementDefined(element))
			{
				base.DrawToolBarButtonBorder(dc, item, is_flat);
				return;
			}
			Rectangle rectangle = item.Rectangle;
			if (item.Button.Style == ToolBarButtonStyle.DropDownButton && item.Button.Parent.DropDownArrows)
			{
				rectangle.Width -= ToolBarDropDownWidth;
			}
			new VisualStyleRenderer(element).DrawBackground(dc, rectangle);
		}
	}

	private static VisualStyleElement ToolBarGetDropDownButtonVisualStyleElement(ToolBarItem item)
	{
		if (item.Button.Parent.DropDownArrows)
		{
			if (ToolBarIsDisabled(item))
			{
				return VisualStyleElement.ToolBar.SplitButton.Disabled;
			}
			if (ToolBarIsPressed(item))
			{
				return VisualStyleElement.ToolBar.SplitButton.Pressed;
			}
			if (ToolBarIsChecked(item))
			{
				if (ToolBarIsHot(item))
				{
					return VisualStyleElement.ToolBar.SplitButton.HotChecked;
				}
				return VisualStyleElement.ToolBar.SplitButton.Checked;
			}
			if (ToolBarIsHot(item))
			{
				return VisualStyleElement.ToolBar.SplitButton.Hot;
			}
			return VisualStyleElement.ToolBar.SplitButton.Normal;
		}
		if (ToolBarIsDisabled(item))
		{
			return VisualStyleElement.ToolBar.DropDownButton.Disabled;
		}
		if (ToolBarIsPressed(item))
		{
			return VisualStyleElement.ToolBar.DropDownButton.Pressed;
		}
		if (ToolBarIsChecked(item))
		{
			if (ToolBarIsHot(item))
			{
				return VisualStyleElement.ToolBar.DropDownButton.HotChecked;
			}
			return VisualStyleElement.ToolBar.DropDownButton.Checked;
		}
		if (ToolBarIsHot(item))
		{
			return VisualStyleElement.ToolBar.DropDownButton.Hot;
		}
		return VisualStyleElement.ToolBar.DropDownButton.Normal;
	}

	private static VisualStyleElement ToolBarGetButtonVisualStyleElement(ToolBarItem item)
	{
		if (ToolBarIsDisabled(item))
		{
			return VisualStyleElement.ToolBar.Button.Disabled;
		}
		if (ToolBarIsPressed(item))
		{
			return VisualStyleElement.ToolBar.Button.Pressed;
		}
		if (ToolBarIsChecked(item))
		{
			if (ToolBarIsHot(item))
			{
				return VisualStyleElement.ToolBar.Button.HotChecked;
			}
			return VisualStyleElement.ToolBar.Button.Checked;
		}
		if (ToolBarIsHot(item))
		{
			return VisualStyleElement.ToolBar.Button.Hot;
		}
		return VisualStyleElement.ToolBar.Button.Normal;
	}

	protected override void DrawToolBarSeparator(Graphics dc, ToolBarItem item)
	{
		if (!RenderClientAreas)
		{
			base.DrawToolBarSeparator(dc, item);
			return;
		}
		VisualStyleElement element = ToolBarGetSeparatorVisualStyleElement(item);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.DrawToolBarSeparator(dc, item);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, item.Rectangle);
		}
	}

	private static VisualStyleElement ToolBarGetSeparatorVisualStyleElement(ToolBarItem toolBarItem)
	{
		return (!toolBarItem.Button.Parent.Vertical) ? VisualStyleElement.ToolBar.SeparatorHorizontal.Normal : VisualStyleElement.ToolBar.SeparatorVertical.Normal;
	}

	protected override void DrawToolBarToggleButtonBackground(Graphics dc, ToolBarItem item)
	{
		if (!RenderClientAreas || !VisualStyleRenderer.IsElementDefined(ToolBarGetButtonVisualStyleElement(item)))
		{
			base.DrawToolBarToggleButtonBackground(dc, item);
		}
	}

	protected override void DrawToolBarDropDownArrow(Graphics dc, ToolBarItem item, bool is_flat)
	{
		if (!RenderClientAreas)
		{
			base.DrawToolBarDropDownArrow(dc, item, is_flat);
			return;
		}
		VisualStyleElement element = ToolBarGetDropDownArrowVisualStyleElement(item);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.DrawToolBarDropDownArrow(dc, item, is_flat);
			return;
		}
		Rectangle rectangle = item.Rectangle;
		rectangle.X = item.Rectangle.Right - ToolBarDropDownWidth;
		rectangle.Width = ToolBarDropDownWidth;
		new VisualStyleRenderer(element).DrawBackground(dc, rectangle);
	}

	private static VisualStyleElement ToolBarGetDropDownArrowVisualStyleElement(ToolBarItem item)
	{
		if (ToolBarIsDisabled(item))
		{
			return VisualStyleElement.ToolBar.SplitButtonDropDown.Disabled;
		}
		if (ToolBarIsPressed(item))
		{
			return VisualStyleElement.ToolBar.SplitButtonDropDown.Pressed;
		}
		if (ToolBarIsChecked(item))
		{
			if (ToolBarIsHot(item))
			{
				return VisualStyleElement.ToolBar.SplitButtonDropDown.HotChecked;
			}
			return VisualStyleElement.ToolBar.SplitButtonDropDown.Checked;
		}
		if (ToolBarIsHot(item))
		{
			return VisualStyleElement.ToolBar.SplitButtonDropDown.Hot;
		}
		return VisualStyleElement.ToolBar.SplitButtonDropDown.Normal;
	}

	public override bool ToolBarHasHotElementStyles(ToolBar toolBar)
	{
		if (!RenderClientAreas)
		{
			return base.ToolBarHasHotElementStyles(toolBar);
		}
		return true;
	}

	protected override void ToolTipDrawBackground(Graphics dc, Rectangle clip_rectangle, ToolTip.ToolTipWindow control)
	{
		if (!RenderClientAreas)
		{
			base.ToolTipDrawBackground(dc, clip_rectangle, control);
			return;
		}
		VisualStyleElement normal = VisualStyleElement.ToolTip.Standard.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			base.ToolTipDrawBackground(dc, clip_rectangle, control);
		}
		else
		{
			new VisualStyleRenderer(normal).DrawBackground(dc, control.ClientRectangle);
		}
	}

	protected override Size TrackBarGetThumbSize(TrackBar trackBar)
	{
		if (!RenderClientAreas)
		{
			return base.TrackBarGetThumbSize(trackBar);
		}
		VisualStyleElement element = TrackBarGetThumbVisualStyleElement(trackBar);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			return base.TrackBarGetThumbSize(trackBar);
		}
		Graphics graphics = trackBar.CreateGraphics();
		Size partSize = new VisualStyleRenderer(element).GetPartSize(graphics, ThemeSizeType.True);
		graphics.Dispose();
		return (trackBar.Orientation != 0) ? TrackBarRotateVerticalThumbSize(partSize) : partSize;
	}

	private static VisualStyleElement TrackBarGetThumbVisualStyleElement(TrackBar trackBar)
	{
		if (trackBar.Orientation == Orientation.Horizontal)
		{
			switch (trackBar.TickStyle)
			{
			case TickStyle.None:
			case TickStyle.BottomRight:
				return TrackBarGetHorizontalThumbBottomVisualStyleElement(trackBar);
			case TickStyle.TopLeft:
				return TrackBarGetHorizontalThumbTopVisualStyleElement(trackBar);
			default:
				return TrackBarGetHorizontalThumbVisualStyleElement(trackBar);
			}
		}
		switch (trackBar.TickStyle)
		{
		case TickStyle.None:
		case TickStyle.BottomRight:
			return TrackBarGetVerticalThumbRightVisualStyleElement(trackBar);
		case TickStyle.TopLeft:
			return TrackBarGetVerticalThumbLeftVisualStyleElement(trackBar);
		default:
			return TrackBarGetVerticalThumbVisualStyleElement(trackBar);
		}
	}

	private static Size TrackBarRotateVerticalThumbSize(Size value)
	{
		int width = value.Width;
		value.Width = value.Height;
		value.Height = width;
		return value;
	}

	protected override void TrackBarDrawHorizontalTrack(Graphics dc, Rectangle thumb_area, Point channel_startpoint, Rectangle clippingArea)
	{
		if (!RenderClientAreas)
		{
			base.TrackBarDrawHorizontalTrack(dc, thumb_area, channel_startpoint, clippingArea);
			return;
		}
		VisualStyleElement normal = VisualStyleElement.TrackBar.Track.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			base.TrackBarDrawHorizontalTrack(dc, thumb_area, channel_startpoint, clippingArea);
			return;
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(normal);
		visualStyleRenderer.DrawBackground(dc, new Rectangle(channel_startpoint, new Size(thumb_area.Width, visualStyleRenderer.GetPartSize(dc, ThemeSizeType.True).Height)), clippingArea);
	}

	protected override void TrackBarDrawVerticalTrack(Graphics dc, Rectangle thumb_area, Point channel_startpoint, Rectangle clippingArea)
	{
		if (!RenderClientAreas)
		{
			base.TrackBarDrawVerticalTrack(dc, thumb_area, channel_startpoint, clippingArea);
			return;
		}
		VisualStyleElement normal = VisualStyleElement.TrackBar.TrackVertical.Normal;
		if (!VisualStyleRenderer.IsElementDefined(normal))
		{
			base.TrackBarDrawVerticalTrack(dc, thumb_area, channel_startpoint, clippingArea);
			return;
		}
		VisualStyleRenderer visualStyleRenderer = new VisualStyleRenderer(normal);
		visualStyleRenderer.DrawBackground(dc, new Rectangle(channel_startpoint, new Size(visualStyleRenderer.GetPartSize(dc, ThemeSizeType.True).Width, thumb_area.Height)), clippingArea);
	}

	private static bool TrackBarIsDisabled(TrackBar trackBar)
	{
		return !trackBar.Enabled;
	}

	private static bool TrackBarIsHot(TrackBar trackBar)
	{
		return trackBar.ThumbEntered;
	}

	private static bool TrackBarIsPressed(TrackBar trackBar)
	{
		return trackBar.thumb_pressed;
	}

	private static bool TrackBarIsFocused(TrackBar trackBar)
	{
		return trackBar.Focused;
	}

	protected override void TrackBarDrawHorizontalThumbBottom(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		if (!RenderClientAreas)
		{
			base.TrackBarDrawHorizontalThumbBottom(dc, thumb_pos, br_thumb, clippingArea, trackBar);
			return;
		}
		VisualStyleElement element = TrackBarGetHorizontalThumbBottomVisualStyleElement(trackBar);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.TrackBarDrawHorizontalThumbBottom(dc, thumb_pos, br_thumb, clippingArea, trackBar);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, thumb_pos, clippingArea);
		}
	}

	private static VisualStyleElement TrackBarGetHorizontalThumbBottomVisualStyleElement(TrackBar trackBar)
	{
		if (TrackBarIsDisabled(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbBottom.Disabled;
		}
		if (TrackBarIsPressed(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbBottom.Pressed;
		}
		if (TrackBarIsHot(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbBottom.Hot;
		}
		if (TrackBarIsFocused(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbBottom.Focused;
		}
		return VisualStyleElement.TrackBar.ThumbBottom.Normal;
	}

	protected override void TrackBarDrawHorizontalThumbTop(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		if (!RenderClientAreas)
		{
			base.TrackBarDrawHorizontalThumbTop(dc, thumb_pos, br_thumb, clippingArea, trackBar);
			return;
		}
		VisualStyleElement element = TrackBarGetHorizontalThumbTopVisualStyleElement(trackBar);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.TrackBarDrawHorizontalThumbTop(dc, thumb_pos, br_thumb, clippingArea, trackBar);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, thumb_pos, clippingArea);
		}
	}

	private static VisualStyleElement TrackBarGetHorizontalThumbTopVisualStyleElement(TrackBar trackBar)
	{
		if (TrackBarIsDisabled(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbTop.Disabled;
		}
		if (TrackBarIsPressed(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbTop.Pressed;
		}
		if (TrackBarIsHot(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbTop.Hot;
		}
		if (TrackBarIsFocused(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbTop.Focused;
		}
		return VisualStyleElement.TrackBar.ThumbTop.Normal;
	}

	protected override void TrackBarDrawHorizontalThumb(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		if (!RenderClientAreas)
		{
			base.TrackBarDrawHorizontalThumb(dc, thumb_pos, br_thumb, clippingArea, trackBar);
			return;
		}
		VisualStyleElement element = TrackBarGetHorizontalThumbVisualStyleElement(trackBar);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.TrackBarDrawHorizontalThumb(dc, thumb_pos, br_thumb, clippingArea, trackBar);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, thumb_pos, clippingArea);
		}
	}

	private static VisualStyleElement TrackBarGetHorizontalThumbVisualStyleElement(TrackBar trackBar)
	{
		if (TrackBarIsDisabled(trackBar))
		{
			return VisualStyleElement.TrackBar.Thumb.Disabled;
		}
		if (TrackBarIsPressed(trackBar))
		{
			return VisualStyleElement.TrackBar.Thumb.Pressed;
		}
		if (TrackBarIsHot(trackBar))
		{
			return VisualStyleElement.TrackBar.Thumb.Hot;
		}
		if (TrackBarIsFocused(trackBar))
		{
			return VisualStyleElement.TrackBar.Thumb.Focused;
		}
		return VisualStyleElement.TrackBar.Thumb.Normal;
	}

	private static Rectangle TrackBarRotateVerticalThumbSize(Rectangle value)
	{
		int width = value.Width;
		value.Width = value.Height;
		value.Height = width;
		return value;
	}

	protected override void TrackBarDrawVerticalThumbRight(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		if (!RenderClientAreas)
		{
			base.TrackBarDrawVerticalThumbRight(dc, thumb_pos, br_thumb, clippingArea, trackBar);
			return;
		}
		VisualStyleElement element = TrackBarGetVerticalThumbRightVisualStyleElement(trackBar);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.TrackBarDrawVerticalThumbRight(dc, thumb_pos, br_thumb, clippingArea, trackBar);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, TrackBarRotateVerticalThumbSize(thumb_pos), clippingArea);
		}
	}

	private static VisualStyleElement TrackBarGetVerticalThumbRightVisualStyleElement(TrackBar trackBar)
	{
		if (TrackBarIsDisabled(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbRight.Disabled;
		}
		if (TrackBarIsPressed(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbRight.Pressed;
		}
		if (TrackBarIsHot(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbRight.Hot;
		}
		if (TrackBarIsFocused(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbRight.Focused;
		}
		return VisualStyleElement.TrackBar.ThumbRight.Normal;
	}

	protected override void TrackBarDrawVerticalThumbLeft(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		if (!RenderClientAreas)
		{
			base.TrackBarDrawVerticalThumbLeft(dc, thumb_pos, br_thumb, clippingArea, trackBar);
			return;
		}
		VisualStyleElement element = TrackBarGetVerticalThumbLeftVisualStyleElement(trackBar);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.TrackBarDrawVerticalThumbLeft(dc, thumb_pos, br_thumb, clippingArea, trackBar);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, TrackBarRotateVerticalThumbSize(thumb_pos), clippingArea);
		}
	}

	private static VisualStyleElement TrackBarGetVerticalThumbLeftVisualStyleElement(TrackBar trackBar)
	{
		if (TrackBarIsDisabled(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbLeft.Disabled;
		}
		if (TrackBarIsPressed(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbLeft.Pressed;
		}
		if (TrackBarIsHot(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbLeft.Hot;
		}
		if (TrackBarIsFocused(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbLeft.Focused;
		}
		return VisualStyleElement.TrackBar.ThumbLeft.Normal;
	}

	protected override void TrackBarDrawVerticalThumb(Graphics dc, Rectangle thumb_pos, Brush br_thumb, Rectangle clippingArea, TrackBar trackBar)
	{
		if (!RenderClientAreas)
		{
			base.TrackBarDrawVerticalThumb(dc, thumb_pos, br_thumb, clippingArea, trackBar);
			return;
		}
		VisualStyleElement element = TrackBarGetVerticalThumbVisualStyleElement(trackBar);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.TrackBarDrawVerticalThumb(dc, thumb_pos, br_thumb, clippingArea, trackBar);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, TrackBarRotateVerticalThumbSize(thumb_pos), clippingArea);
		}
	}

	private static VisualStyleElement TrackBarGetVerticalThumbVisualStyleElement(TrackBar trackBar)
	{
		if (TrackBarIsDisabled(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbVertical.Disabled;
		}
		if (TrackBarIsPressed(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbVertical.Pressed;
		}
		if (TrackBarIsHot(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbVertical.Hot;
		}
		if (TrackBarIsFocused(trackBar))
		{
			return VisualStyleElement.TrackBar.ThumbVertical.Focused;
		}
		return VisualStyleElement.TrackBar.ThumbVertical.Normal;
	}

	protected override ITrackBarTickPainter TrackBarGetHorizontalTickPainter(Graphics g)
	{
		if (!RenderClientAreas || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.TrackBar.Ticks.Normal))
		{
			return base.TrackBarGetHorizontalTickPainter(g);
		}
		return new TrackBarHorizontalTickPainter(g);
	}

	protected override ITrackBarTickPainter TrackBarGetVerticalTickPainter(Graphics g)
	{
		if (!RenderClientAreas || !VisualStyleRenderer.IsElementDefined(VisualStyleElement.TrackBar.TicksVertical.Normal))
		{
			return base.TrackBarGetVerticalTickPainter(g);
		}
		return new TrackBarVerticalTickPainter(g);
	}

	[System.MonoInternalNote("Use the sizing information provided by the VisualStyles API.")]
	public override void TreeViewDrawNodePlusMinus(TreeView treeView, TreeNode node, Graphics dc, int x, int middle)
	{
		if (!RenderClientAreas)
		{
			base.TreeViewDrawNodePlusMinus(treeView, node, dc, x, middle);
			return;
		}
		VisualStyleElement element = ((!node.IsExpanded) ? VisualStyleElement.TreeView.Glyph.Closed : VisualStyleElement.TreeView.Glyph.Opened);
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.TreeViewDrawNodePlusMinus(treeView, node, dc, x, middle);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(dc, new Rectangle(x, middle - 4, 9, 9));
		}
	}

	public override void UpDownBaseDrawButton(Graphics g, Rectangle bounds, bool top, PushButtonState state)
	{
		if (!RenderClientAreas)
		{
			base.UpDownBaseDrawButton(g, bounds, top, state);
			return;
		}
		VisualStyleElement element = (top ? (state switch
		{
			PushButtonState.Disabled => VisualStyleElement.Spin.Up.Disabled, 
			PushButtonState.Pressed => VisualStyleElement.Spin.Up.Pressed, 
			PushButtonState.Hot => VisualStyleElement.Spin.Up.Hot, 
			_ => VisualStyleElement.Spin.Up.Normal, 
		}) : (state switch
		{
			PushButtonState.Disabled => VisualStyleElement.Spin.Down.Disabled, 
			PushButtonState.Pressed => VisualStyleElement.Spin.Down.Pressed, 
			PushButtonState.Hot => VisualStyleElement.Spin.Down.Hot, 
			_ => VisualStyleElement.Spin.Down.Normal, 
		}));
		if (!VisualStyleRenderer.IsElementDefined(element))
		{
			base.UpDownBaseDrawButton(g, bounds, top, state);
		}
		else
		{
			new VisualStyleRenderer(element).DrawBackground(g, bounds);
		}
	}

	private static bool AreEqual(VisualStyleElement value1, VisualStyleElement value2)
	{
		return value1.ClassName == value1.ClassName && value1.Part == value2.Part && value1.State == value2.State;
	}

	private static IDeviceContext GetMeasurementDeviceContext()
	{
		if (control == null)
		{
			control = new Control();
		}
		return control.CreateGraphics();
	}

	private static void ReleaseMeasurementDeviceContext(IDeviceContext dc)
	{
		dc.Dispose();
	}
}
