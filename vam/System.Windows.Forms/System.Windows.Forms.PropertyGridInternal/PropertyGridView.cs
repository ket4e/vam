using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Threading;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal;

internal class PropertyGridView : ScrollableControl, IWindowsFormsEditorService
{
	internal class PropertyGridDropDown : Form
	{
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.Style = -2046820352;
				createParams.ExStyle |= 8;
				return createParams;
			}
		}
	}

	private const char PASSWORD_PAINT_CHAR = '●';

	private const char PASSWORD_TEXT_CHAR = '*';

	private const int V_INDENT = 16;

	private const int ENTRY_SPACING = 2;

	private const int RESIZE_WIDTH = 3;

	private const int BUTTON_WIDTH = 25;

	private const int VALUE_PAINT_WIDTH = 19;

	private const int VALUE_PAINT_INDENT = 27;

	private double splitter_percent = 0.5;

	private int row_height;

	private int font_height_padding = 3;

	private PropertyGridTextBox grid_textbox;

	private PropertyGrid property_grid;

	private bool resizing_grid;

	private PropertyGridDropDown dropdown_form;

	private Form dialog_form;

	private ImplicitVScrollBar vbar;

	private StringFormat string_format;

	private Font bold_font;

	private Brush inactive_text_brush;

	private ListBox dropdown_list;

	private Point last_click;

	private Padding dropdown_form_padding;

	private GridEntry RootGridItem => (GridEntry)property_grid.RootGridItem;

	private GridEntry SelectedGridItem
	{
		get
		{
			return (GridEntry)property_grid.SelectedGridItem;
		}
		set
		{
			property_grid.SelectedGridItem = value;
		}
	}

	private int SplitterLocation => (int)(splitter_percent * (double)base.Width);

	private double SplitterPercent
	{
		get
		{
			return splitter_percent;
		}
		set
		{
			int splitterLocation = SplitterLocation;
			splitter_percent = Math.Max(Math.Min(value, 0.9), 0.1);
			if (splitterLocation != SplitterLocation)
			{
				int num = ((splitterLocation <= SplitterLocation) ? splitterLocation : SplitterLocation);
				Invalidate(new Rectangle(num, 0, base.Width - num - (vbar.Visible ? vbar.Width : 0), base.Height));
				UpdateItem(SelectedGridItem);
			}
		}
	}

	public PropertyGridView(PropertyGrid propertyGrid)
	{
		property_grid = propertyGrid;
		string_format = new StringFormat();
		string_format.FormatFlags = StringFormatFlags.NoWrap;
		string_format.Trimming = StringTrimming.None;
		grid_textbox = new PropertyGridTextBox();
		grid_textbox.DropDownButtonClicked += DropDownButtonClicked;
		grid_textbox.DialogButtonClicked += DialogButtonClicked;
		dropdown_form = new PropertyGridDropDown();
		dropdown_form.FormBorderStyle = FormBorderStyle.None;
		dropdown_form.StartPosition = FormStartPosition.Manual;
		dropdown_form.ShowInTaskbar = false;
		dialog_form = new Form();
		dialog_form.StartPosition = FormStartPosition.Manual;
		dialog_form.FormBorderStyle = FormBorderStyle.None;
		dialog_form.ShowInTaskbar = false;
		dropdown_form_padding = new Padding(0, 0, 2, 2);
		row_height = Font.Height + font_height_padding;
		grid_textbox.Visible = false;
		grid_textbox.Font = Font;
		grid_textbox.BackColor = SystemColors.Window;
		grid_textbox.Validate += grid_textbox_Validate;
		grid_textbox.ToggleValue += grid_textbox_ToggleValue;
		grid_textbox.KeyDown += grid_textbox_KeyDown;
		base.Controls.Add(grid_textbox);
		vbar = new ImplicitVScrollBar();
		vbar.Visible = false;
		vbar.Value = 0;
		vbar.ValueChanged += VScrollBar_HandleValueChanged;
		vbar.Dock = DockStyle.Right;
		base.Controls.AddImplicit(vbar);
		resizing_grid = false;
		bold_font = new Font(Font, FontStyle.Bold);
		inactive_text_brush = new SolidBrush(ThemeEngine.Current.ColorGrayText);
		base.ForeColorChanged += RedrawEvent;
		base.BackColorChanged += RedrawEvent;
		base.FontChanged += RedrawEvent;
		SetStyle(ControlStyles.Selectable, value: true);
		SetStyle(ControlStyles.DoubleBuffer, value: true);
		SetStyle(ControlStyles.UserPaint, value: true);
		SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.ResizeRedraw, value: true);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		bold_font = new Font(Font, FontStyle.Bold);
		row_height = Font.Height + font_height_padding;
	}

	private void InvalidateItemLabel(GridEntry item)
	{
		Invalidate(new Rectangle(0, item.Top, SplitterLocation, row_height));
	}

	private void InvalidateItem(GridEntry item)
	{
		if (item != null)
		{
			Rectangle rc = new Rectangle(0, item.Top, base.Width, row_height);
			Invalidate(rc);
			if (item.Expanded)
			{
				rc = new Rectangle(0, item.Top + row_height, base.Width, base.Height - (item.Top + row_height));
				Invalidate(rc);
			}
		}
	}

	protected override void OnDoubleClick(EventArgs e)
	{
		if (SelectedGridItem != null && SelectedGridItem.Expandable && !SelectedGridItem.PlusMinusBounds.Contains(last_click))
		{
			SelectedGridItem.Expanded = !SelectedGridItem.Expanded;
		}
		else
		{
			ToggleValue(SelectedGridItem);
		}
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(BackColor), base.ClientRectangle);
		int yLoc = -vbar.Value * row_height;
		if (RootGridItem != null)
		{
			DrawGridItems(RootGridItem.GridItems, e, 1, ref yLoc);
		}
		UpdateScrollBar();
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if (vbar != null && vbar.Visible)
		{
			if (e.Delta < 0)
			{
				vbar.Value = Math.Min(vbar.Maximum - GetVisibleRowsCount() + 1, vbar.Value + SystemInformation.MouseWheelScrollLines);
			}
			else
			{
				vbar.Value = Math.Max(0, vbar.Value - SystemInformation.MouseWheelScrollLines);
			}
			base.OnMouseWheel(e);
		}
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		if (RootGridItem != null)
		{
			if (resizing_grid)
			{
				int num = Math.Max(e.X, 32);
				SplitterPercent = 1.0 * (double)num / (double)base.Width;
			}
			if (e.X > SplitterLocation - 3 && e.X < SplitterLocation + 3)
			{
				Cursor = Cursors.SizeWE;
			}
			else
			{
				Cursor = Cursors.Default;
			}
			base.OnMouseMove(e);
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		last_click = e.Location;
		if (RootGridItem == null)
		{
			return;
		}
		if (e.X > SplitterLocation - 3 && e.X < SplitterLocation + 3)
		{
			resizing_grid = true;
			return;
		}
		int current = -vbar.Value * row_height;
		GridItem selectedGridItem = GetSelectedGridItem(RootGridItem.GridItems, e.Y, ref current);
		if (selectedGridItem != null)
		{
			if (selectedGridItem.Expandable && ((GridEntry)selectedGridItem).PlusMinusBounds.Contains(e.X, e.Y))
			{
				selectedGridItem.Expanded = !selectedGridItem.Expanded;
			}
			SelectedGridItem = (GridEntry)selectedGridItem;
			if (!GridLabelHitTest(e.X))
			{
				grid_textbox.SendMouseDown(PointToScreen(e.Location));
			}
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		resizing_grid = false;
		base.OnMouseUp(e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		if (SelectedGridItem != null)
		{
			UpdateView();
		}
	}

	private void UnfocusSelection()
	{
		Select(this);
	}

	private void FocusSelection()
	{
		Select(grid_textbox);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		GridEntry selectedGridItem = SelectedGridItem;
		if (selectedGridItem != null && grid_textbox.Visible)
		{
			switch (keyData)
			{
			case Keys.Return:
				if (TrySetEntry(selectedGridItem, grid_textbox.Text))
				{
					UnfocusSelection();
				}
				return true;
			case Keys.Escape:
				if (selectedGridItem.IsEditable)
				{
					UpdateItem(selectedGridItem);
				}
				UnfocusSelection();
				return true;
			case Keys.Tab:
				FocusSelection();
				return true;
			default:
				return false;
			}
		}
		return base.ProcessDialogKey(keyData);
	}

	private bool TrySetEntry(GridEntry entry, object value)
	{
		if (entry == null || grid_textbox.Text.Equals(entry.ValueText))
		{
			return true;
		}
		if (entry.IsEditable || (!entry.IsEditable && (entry.HasCustomEditor || entry.AcceptedValues != null)) || !entry.IsMerged || entry.HasMergedValue || (!entry.HasMergedValue && grid_textbox.Text != string.Empty))
		{
			string error = null;
			if (!entry.SetValue(value, out error) && error != null)
			{
				if (property_grid.ShowError(error, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
				{
					UpdateItem(entry);
					UnfocusSelection();
				}
				return false;
			}
		}
		UpdateItem(entry);
		return true;
	}

	protected override bool IsInputKey(Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Return:
		case Keys.Escape:
		case Keys.PageUp:
		case Keys.PageDown:
		case Keys.End:
		case Keys.Home:
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
			return true;
		default:
			return false;
		}
	}

	private GridEntry MoveUpFromItem(GridEntry item, int up_count)
	{
		while (up_count > 0)
		{
			GridItemCollection gridItemCollection = ((item.Parent == null) ? RootGridItem.GridItems : item.Parent.GridItems);
			int num = gridItemCollection.IndexOf(item);
			if (num == 0)
			{
				if (item.Parent.GridItemType == GridItemType.Root)
				{
					return item;
				}
				item = (GridEntry)item.Parent;
				up_count--;
			}
			else
			{
				GridEntry gridEntry = (GridEntry)gridItemCollection[num - 1];
				item = ((!gridEntry.Expandable || !gridEntry.Expanded) ? gridEntry : ((GridEntry)gridEntry.GridItems[gridEntry.GridItems.Count - 1]));
				up_count--;
			}
		}
		return item;
	}

	private GridEntry MoveDownFromItem(GridEntry item, int down_count)
	{
		while (down_count > 0)
		{
			if (item.Expandable && item.Expanded)
			{
				item = (GridEntry)item.GridItems[0];
				down_count--;
				continue;
			}
			GridItem gridItem = item;
			GridItemCollection gridItems = gridItem.Parent.GridItems;
			int num;
			for (num = gridItems.IndexOf(gridItem); num == gridItems.Count - 1; num = gridItems.IndexOf(gridItem))
			{
				gridItem = gridItem.Parent;
				if (gridItem == null || gridItem.Parent == null)
				{
					break;
				}
				gridItems = gridItem.Parent.GridItems;
			}
			if (num == gridItems.Count - 1)
			{
				return item;
			}
			item = (GridEntry)gridItems[num + 1];
			down_count--;
		}
		return item;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		GridEntry selectedGridItem = SelectedGridItem;
		if (selectedGridItem == null)
		{
			base.OnKeyDown(e);
			return;
		}
		switch (e.KeyData & Keys.KeyCode)
		{
		case Keys.Left:
			if (e.Control)
			{
				if (SplitterLocation > 32)
				{
					SplitterPercent -= 0.01;
				}
				e.Handled = true;
				break;
			}
			if (selectedGridItem.Expandable && selectedGridItem.Expanded)
			{
				selectedGridItem.Expanded = false;
				e.Handled = true;
				break;
			}
			goto case Keys.Up;
		case Keys.Right:
			if (e.Control)
			{
				if (SplitterLocation < base.Width)
				{
					SplitterPercent += 0.01;
				}
				e.Handled = true;
				break;
			}
			if (selectedGridItem.Expandable && !selectedGridItem.Expanded)
			{
				selectedGridItem.Expanded = true;
				e.Handled = true;
				break;
			}
			goto case Keys.Down;
		case Keys.Return:
			if (selectedGridItem.Expandable)
			{
				selectedGridItem.Expanded = !selectedGridItem.Expanded;
			}
			e.Handled = true;
			break;
		case Keys.Up:
			SelectedGridItem = MoveUpFromItem(selectedGridItem, 1);
			e.Handled = true;
			break;
		case Keys.Down:
			SelectedGridItem = MoveDownFromItem(selectedGridItem, 1);
			e.Handled = true;
			break;
		case Keys.PageUp:
			SelectedGridItem = MoveUpFromItem(selectedGridItem, vbar.LargeChange);
			e.Handled = true;
			break;
		case Keys.PageDown:
			SelectedGridItem = MoveDownFromItem(selectedGridItem, vbar.LargeChange);
			e.Handled = true;
			break;
		case Keys.End:
		{
			GridEntry gridEntry = (GridEntry)RootGridItem.GridItems[RootGridItem.GridItems.Count - 1];
			while (gridEntry.Expandable && gridEntry.Expanded)
			{
				gridEntry = (GridEntry)gridEntry.GridItems[gridEntry.GridItems.Count - 1];
			}
			SelectedGridItem = gridEntry;
			e.Handled = true;
			break;
		}
		case Keys.Home:
			SelectedGridItem = (GridEntry)RootGridItem.GridItems[0];
			e.Handled = true;
			break;
		}
		base.OnKeyDown(e);
	}

	private bool GridLabelHitTest(int x)
	{
		if (0 <= x && (double)x <= splitter_percent * (double)base.Width)
		{
			return true;
		}
		return false;
	}

	private GridItem GetSelectedGridItem(GridItemCollection grid_items, int y, ref int current)
	{
		foreach (GridItem grid_item in grid_items)
		{
			if (y > current && y < current + row_height)
			{
				return grid_item;
			}
			current += row_height;
			if (grid_item.Expanded)
			{
				GridItem selectedGridItem = GetSelectedGridItem(grid_item.GridItems, y, ref current);
				if (selectedGridItem != null)
				{
					return selectedGridItem;
				}
			}
		}
		return null;
	}

	private int GetVisibleItemsCount(GridEntry entry)
	{
		if (entry == null)
		{
			return 0;
		}
		int num = 0;
		foreach (GridEntry gridItem in entry.GridItems)
		{
			num++;
			if (gridItem.Expandable && gridItem.Expanded)
			{
				num += GetVisibleItemsCount(gridItem);
			}
		}
		return num;
	}

	private int GetVisibleRowsCount()
	{
		return base.Height / row_height;
	}

	private void UpdateScrollBar()
	{
		if (RootGridItem != null)
		{
			int visibleRowsCount = GetVisibleRowsCount();
			int visibleItemsCount = GetVisibleItemsCount(RootGridItem);
			if (visibleItemsCount > visibleRowsCount)
			{
				vbar.Visible = true;
				vbar.SmallChange = 1;
				vbar.LargeChange = visibleRowsCount;
				vbar.Maximum = Math.Max(0, visibleItemsCount - 1);
			}
			else
			{
				vbar.Value = 0;
				vbar.Visible = false;
			}
			UpdateGridTextBoxBounds(SelectedGridItem);
		}
	}

	private void DrawGridItems(GridItemCollection grid_items, PaintEventArgs pevent, int depth, ref int yLoc)
	{
		foreach (GridItem grid_item in grid_items)
		{
			DrawGridItem((GridEntry)grid_item, pevent, depth, ref yLoc);
			if (grid_item.Expanded)
			{
				DrawGridItems(grid_item.GridItems, pevent, (grid_item.GridItemType != GridItemType.Category) ? (depth + 1) : depth, ref yLoc);
			}
		}
	}

	private void DrawGridItemLabel(GridEntry grid_item, PaintEventArgs pevent, int depth, Rectangle rect)
	{
		Font font = Font;
		Brush brush;
		if (grid_item.GridItemType == GridItemType.Category)
		{
			font = bold_font;
			brush = SystemBrushes.ControlText;
			pevent.Graphics.DrawString(grid_item.Label, font, brush, rect.X + 1, rect.Y + 2);
			if (grid_item == SelectedGridItem)
			{
				SizeF sizeF = pevent.Graphics.MeasureString(grid_item.Label, font);
				ControlPaint.DrawFocusRectangle(pevent.Graphics, new Rectangle(rect.X + 1, rect.Y + 2, (int)sizeF.Width, (int)sizeF.Height));
			}
		}
		else if (grid_item == SelectedGridItem)
		{
			Rectangle rect2 = rect;
			if (depth > 1)
			{
				rect2.X -= 16;
				rect2.Width += 16;
			}
			pevent.Graphics.FillRectangle(SystemBrushes.Highlight, rect2);
			brush = SystemBrushes.HighlightText;
		}
		else
		{
			brush = ((!grid_item.IsReadOnly) ? SystemBrushes.ControlText : inactive_text_brush);
		}
		pevent.Graphics.DrawString(grid_item.Label, font, brush, new Rectangle(rect.X + 1, rect.Y + 2, rect.Width - 2, rect.Height - 2), string_format);
	}

	private void DrawGridItemValue(GridEntry grid_item, PaintEventArgs pevent, int depth, Rectangle rect)
	{
		if (grid_item.PropertyDescriptor != null)
		{
			int num = SplitterLocation + 2;
			if (grid_item.PaintValueSupported)
			{
				pevent.Graphics.DrawRectangle(Pens.Black, SplitterLocation + 2, rect.Y + 2, 20, row_height - 4);
				grid_item.PaintValue(pevent.Graphics, new Rectangle(SplitterLocation + 2 + 1, rect.Y + 2 + 1, 19, row_height - 5));
				num += 27;
			}
			Font font = Font;
			if (grid_item.IsResetable || !grid_item.HasDefaultValue)
			{
				font = bold_font;
			}
			Brush brush = ((!grid_item.IsReadOnly) ? SystemBrushes.ControlText : inactive_text_brush);
			string s = string.Empty;
			if (!grid_item.IsMerged || (grid_item.IsMerged && grid_item.HasMergedValue))
			{
				s = ((!grid_item.IsPassword) ? grid_item.ValueText : new string('●', grid_item.ValueText.Length));
			}
			pevent.Graphics.DrawString(s, font, brush, new RectangleF(num + 2, rect.Y + 2, base.ClientRectangle.Width - num, row_height - 4), string_format);
		}
	}

	private void DrawGridItem(GridEntry grid_item, PaintEventArgs pevent, int depth, ref int yLoc)
	{
		if (yLoc > -row_height && yLoc < base.ClientRectangle.Height)
		{
			pevent.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(property_grid.LineColor), 0, yLoc, 16, row_height);
			if (grid_item.GridItemType == GridItemType.Category)
			{
				pevent.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(property_grid.CategoryForeColor), depth * 16, yLoc, base.ClientRectangle.Width - depth * 16, row_height);
			}
			DrawGridItemLabel(grid_item, pevent, depth, new Rectangle(depth * 16, yLoc, SplitterLocation - depth * 16, row_height));
			DrawGridItemValue(grid_item, pevent, depth, new Rectangle(SplitterLocation + 2, yLoc, base.ClientRectangle.Width - SplitterLocation - 2 - (vbar.Visible ? vbar.Width : 0), row_height));
			if (grid_item.GridItemType != GridItemType.Category)
			{
				Pen pen = ThemeEngine.Current.ResPool.GetPen(property_grid.LineColor);
				pevent.Graphics.DrawLine(pen, SplitterLocation, yLoc, SplitterLocation, yLoc + row_height);
				pevent.Graphics.DrawLine(pen, 0, yLoc + row_height, base.ClientRectangle.Width, yLoc + row_height);
			}
			if (grid_item.Expandable)
			{
				int y = yLoc + row_height / 2 - 2 + 1;
				grid_item.PlusMinusBounds = DrawPlusMinus(pevent.Graphics, (depth - 1) * 16 + 2 + 1, y, grid_item.Expanded, grid_item.GridItemType == GridItemType.Category);
			}
		}
		grid_item.Top = yLoc;
		yLoc += row_height;
	}

	private Rectangle DrawPlusMinus(Graphics g, int x, int y, bool expanded, bool category)
	{
		Rectangle rectangle = new Rectangle(x, y, 8, 8);
		if (!category)
		{
			g.FillRectangle(Brushes.White, rectangle);
		}
		Pen pen = ThemeEngine.Current.ResPool.GetPen(property_grid.ViewForeColor);
		g.DrawRectangle(pen, rectangle);
		g.DrawLine(pen, x + 2, y + 4, x + 6, y + 4);
		if (!expanded)
		{
			g.DrawLine(pen, x + 4, y + 2, x + 4, y + 6);
		}
		return rectangle;
	}

	private void RedrawEvent(object sender, EventArgs e)
	{
		Refresh();
	}

	private void listBox_MouseUp(object sender, MouseEventArgs e)
	{
		AcceptListBoxSelection(sender);
	}

	private void listBox_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyData & Keys.KeyCode)
		{
		case Keys.Return:
			AcceptListBoxSelection(sender);
			break;
		case Keys.Escape:
			CloseDropDown();
			break;
		}
	}

	private void AcceptListBoxSelection(object sender)
	{
		GridEntry selectedGridItem = SelectedGridItem;
		if (selectedGridItem != null)
		{
			grid_textbox.Text = (string)((ListBox)sender).SelectedItem;
			CloseDropDown();
			if (TrySetEntry(selectedGridItem, grid_textbox.Text))
			{
				UnfocusSelection();
			}
		}
	}

	private void DropDownButtonClicked(object sender, EventArgs e)
	{
		DropDownEdit();
	}

	private void DropDownEdit()
	{
		GridEntry selectedGridItem = SelectedGridItem;
		if (selectedGridItem == null)
		{
			return;
		}
		if (selectedGridItem.HasCustomEditor)
		{
			selectedGridItem.EditValue(this);
			return;
		}
		if (dropdown_form.Visible)
		{
			CloseDropDown();
			return;
		}
		ICollection acceptedValues = selectedGridItem.AcceptedValues;
		if (acceptedValues == null)
		{
			return;
		}
		if (dropdown_list == null)
		{
			dropdown_list = new ListBox();
			dropdown_list.KeyDown += listBox_KeyDown;
			dropdown_list.MouseUp += listBox_MouseUp;
		}
		dropdown_list.Items.Clear();
		dropdown_list.BorderStyle = BorderStyle.FixedSingle;
		int selectedIndex = 0;
		int num = 0;
		string valueText = selectedGridItem.ValueText;
		foreach (object item in acceptedValues)
		{
			dropdown_list.Items.Add(item);
			if (valueText != null && valueText.Equals(item))
			{
				selectedIndex = num;
			}
			num++;
		}
		dropdown_list.Height = row_height * Math.Min(dropdown_list.Items.Count, 15);
		dropdown_list.Width = base.ClientRectangle.Width - SplitterLocation - (vbar.Visible ? vbar.Width : 0);
		if (acceptedValues.Count > 0)
		{
			dropdown_list.SelectedIndex = selectedIndex;
		}
		DropDownControl(dropdown_list);
	}

	private void DialogButtonClicked(object sender, EventArgs e)
	{
		GridEntry selectedGridItem = SelectedGridItem;
		if (selectedGridItem != null && selectedGridItem.HasCustomEditor)
		{
			selectedGridItem.EditValue(this);
		}
	}

	private void VScrollBar_HandleValueChanged(object sender, EventArgs e)
	{
		UpdateView();
	}

	private void grid_textbox_ToggleValue(object sender, EventArgs args)
	{
		ToggleValue(SelectedGridItem);
	}

	private void grid_textbox_KeyDown(object sender, KeyEventArgs e)
	{
		Keys keys = e.KeyData & Keys.KeyCode;
		if (keys == Keys.Down && e.Alt)
		{
			DropDownEdit();
			e.Handled = true;
		}
	}

	private void grid_textbox_Validate(object sender, CancelEventArgs args)
	{
		if (!TrySetEntry(SelectedGridItem, grid_textbox.Text))
		{
			args.Cancel = true;
		}
	}

	private void ToggleValue(GridEntry entry)
	{
		if (entry != null && !entry.IsReadOnly && entry.GridItemType == GridItemType.Property)
		{
			entry.ToggleValue();
		}
	}

	internal void UpdateItem(GridEntry entry)
	{
		if (entry == null || entry.GridItemType == GridItemType.Category || entry.GridItemType == GridItemType.Root)
		{
			grid_textbox.Visible = false;
			InvalidateItem(entry);
		}
		else if (SelectedGridItem == entry)
		{
			SuspendLayout();
			grid_textbox.Visible = false;
			if (entry.IsResetable || !entry.HasDefaultValue)
			{
				grid_textbox.Font = bold_font;
			}
			else
			{
				grid_textbox.Font = Font;
			}
			if (entry.IsReadOnly)
			{
				grid_textbox.DropDownButtonVisible = false;
				grid_textbox.DialogButtonVisible = false;
				grid_textbox.ReadOnly = true;
				grid_textbox.ForeColor = SystemColors.GrayText;
			}
			else
			{
				grid_textbox.DropDownButtonVisible = entry.AcceptedValues != null || entry.EditorStyle == UITypeEditorEditStyle.DropDown;
				grid_textbox.DialogButtonVisible = entry.EditorStyle == UITypeEditorEditStyle.Modal;
				grid_textbox.ForeColor = SystemColors.ControlText;
				grid_textbox.ReadOnly = !entry.IsEditable;
			}
			UpdateGridTextBoxBounds(entry);
			grid_textbox.PasswordChar = (entry.IsPassword ? '*' : '\0');
			grid_textbox.Text = ((!entry.IsMerged || entry.HasMergedValue) ? entry.ValueText : string.Empty);
			grid_textbox.Visible = true;
			InvalidateItem(entry);
			ResumeLayout(performLayout: false);
		}
		else
		{
			grid_textbox.Visible = false;
		}
	}

	private void UpdateGridTextBoxBounds(GridEntry entry)
	{
		if (entry != null && RootGridItem != null)
		{
			int y = -vbar.Value * row_height;
			CalculateItemY(entry, RootGridItem.GridItems, ref y);
			int num = SplitterLocation + 2 + (entry.PaintValueSupported ? 27 : 0);
			grid_textbox.SetBounds(num + 2, y + 2, base.ClientRectangle.Width - 2 - num - (vbar.Visible ? vbar.Width : 0), row_height - 2);
		}
	}

	private bool CalculateItemY(GridEntry entry, GridItemCollection items, ref int y)
	{
		foreach (GridItem item in items)
		{
			if (item == entry)
			{
				return true;
			}
			y += row_height;
			if (item.Expandable && item.Expanded && CalculateItemY(entry, item.GridItems, ref y))
			{
				return true;
			}
		}
		return false;
	}

	private void ScrollToItem(GridEntry item)
	{
		if (item != null && RootGridItem != null)
		{
			int y = -vbar.Value * row_height;
			int num = vbar.Value;
			CalculateItemY(item, RootGridItem.GridItems, ref y);
			if (y < 0)
			{
				num += y / row_height;
			}
			else if (y + row_height > base.Height)
			{
				num += (y + row_height - base.Height) / row_height + 1;
			}
			if (num >= vbar.Minimum && num <= vbar.Maximum)
			{
				vbar.Value = num;
			}
		}
	}

	internal void SelectItem(GridEntry oldItem, GridEntry newItem)
	{
		if (oldItem != null)
		{
			InvalidateItemLabel(oldItem);
		}
		if (newItem != null)
		{
			UpdateItem(newItem);
			ScrollToItem(newItem);
		}
		else
		{
			grid_textbox.Visible = false;
			vbar.Visible = false;
		}
	}

	internal void UpdateView()
	{
		UpdateScrollBar();
		Invalidate();
		Update();
		UpdateItem(SelectedGridItem);
	}

	internal void ExpandItem(GridEntry item)
	{
		UpdateItem(SelectedGridItem);
		Invalidate(new Rectangle(0, item.Top, base.Width, base.Height - item.Top));
	}

	internal void CollapseItem(GridEntry item)
	{
		UpdateItem(SelectedGridItem);
		Invalidate(new Rectangle(0, item.Top, base.Width, base.Height - item.Top));
	}

	private void ShowDropDownControl(Control control, bool resizeable)
	{
		dropdown_form.Size = control.Size;
		control.Dock = DockStyle.Fill;
		if (resizeable)
		{
			dropdown_form.Padding = dropdown_form_padding;
			dropdown_form.Width += dropdown_form_padding.Right;
			dropdown_form.Height += dropdown_form_padding.Bottom;
			dropdown_form.FormBorderStyle = FormBorderStyle.Sizable;
			dropdown_form.SizeGripStyle = SizeGripStyle.Show;
		}
		else
		{
			dropdown_form.FormBorderStyle = FormBorderStyle.None;
			dropdown_form.SizeGripStyle = SizeGripStyle.Hide;
			dropdown_form.Padding = Padding.Empty;
		}
		dropdown_form.Controls.Add(control);
		dropdown_form.Width = Math.Max(base.ClientRectangle.Width - SplitterLocation - (vbar.Visible ? vbar.Width : 0), control.Width);
		dropdown_form.Location = PointToScreen(new Point(grid_textbox.Right - dropdown_form.Width, grid_textbox.Location.Y + row_height));
		RepositionInScreenWorkingArea(dropdown_form);
		Point location = dropdown_form.Location;
		Form form = FindForm();
		form.AddOwnedForm(dropdown_form);
		dropdown_form.Show();
		if (dropdown_form.Location != location)
		{
			dropdown_form.Location = location;
		}
		MSG msg = default(MSG);
		object queue_id = XplatUI.StartLoop(Thread.CurrentThread);
		control.Focus();
		while (dropdown_form.Visible && XplatUI.GetMessage(queue_id, ref msg, IntPtr.Zero, 0, 0))
		{
			switch (msg.message)
			{
			case Msg.WM_NCLBUTTONDOWN:
			case Msg.WM_NCRBUTTONDOWN:
			case Msg.WM_NCMBUTTONDOWN:
			case Msg.WM_LBUTTONDOWN:
			case Msg.WM_RBUTTONDOWN:
			case Msg.WM_MBUTTONDOWN:
				if (!HwndInControl(dropdown_form, msg.hwnd))
				{
					CloseDropDown();
				}
				break;
			case Msg.WM_ACTIVATE:
			case Msg.WM_NCPAINT:
				if (form.window.Handle == msg.hwnd)
				{
					CloseDropDown();
				}
				break;
			}
			XplatUI.TranslateMessage(ref msg);
			XplatUI.DispatchMessage(ref msg);
		}
		XplatUI.EndLoop(Thread.CurrentThread);
	}

	private void RepositionInScreenWorkingArea(Form form)
	{
		Rectangle workingArea = Screen.FromControl(form).WorkingArea;
		if (!workingArea.Contains(form.Bounds))
		{
			int x = form.Location.X;
			int y = form.Location.Y;
			if (form.Location.X < workingArea.X)
			{
				x = workingArea.X;
			}
			if (form.Location.Y + form.Size.Height > workingArea.Height)
			{
				y = PointToScreen(new Point(grid_textbox.Right - form.Width, grid_textbox.Location.Y)).Y - form.Size.Height;
			}
			form.Location = new Point(x, y);
		}
	}

	private bool HwndInControl(Control c, IntPtr hwnd)
	{
		if (hwnd == c.window.Handle)
		{
			return true;
		}
		Control[] allControls = c.Controls.GetAllControls();
		foreach (Control c2 in allControls)
		{
			if (HwndInControl(c2, hwnd))
			{
				return true;
			}
		}
		return false;
	}

	public void CloseDropDown()
	{
		dropdown_form.Hide();
		dropdown_form.Controls.Clear();
	}

	public void DropDownControl(Control control)
	{
		bool resizeable = SelectedGridItem != null && SelectedGridItem.EditorResizeable;
		ShowDropDownControl(control, resizeable);
	}

	public DialogResult ShowDialog(Form dialog)
	{
		return dialog.ShowDialog(this);
	}
}
