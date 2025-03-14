using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultEvent("SelectedIndexChanged")]
[Docking(DockingBehavior.Ask)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[Designer("System.Windows.Forms.Design.ListViewDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultProperty("Items")]
public class ListView : Control
{
	internal class ItemControl : Control
	{
		private enum BoxSelect
		{
			None,
			Normal,
			Shift,
			Control
		}

		private ListView owner;

		private ListViewItem clicked_item;

		private ListViewItem last_clicked_item;

		private bool hover_processed;

		private bool checking;

		private ListViewItem prev_hovered_item;

		private ListViewItem prev_tooltip_item;

		private int clicks;

		private Point drag_begin = new Point(-1, -1);

		internal int dragged_item_index = -1;

		private ListViewLabelEditTextBox edit_text_box;

		internal ListViewItem edit_item;

		private LabelEditEventArgs edit_args;

		private BoxSelect box_select_mode;

		private IList prev_selection;

		private Point box_select_start;

		private Rectangle box_select_rect;

		internal Rectangle BoxSelectRectangle
		{
			get
			{
				return box_select_rect;
			}
			set
			{
				if (!(box_select_rect == value))
				{
					InvalidateBoxSelectRect();
					box_select_rect = value;
					InvalidateBoxSelectRect();
				}
			}
		}

		private ArrayList BoxSelectedItems
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < owner.Items.Count; i++)
				{
					if ((owner.View != View.Details || owner.FullRowSelect || owner.VirtualMode) ? BoxIntersectsItem(i) : BoxIntersectsText(i))
					{
						arrayList.Add(owner.GetItemAtDisplayIndex(i));
					}
				}
				return arrayList;
			}
		}

		public ItemControl(ListView owner)
		{
			this.owner = owner;
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			base.DoubleClick += ItemsDoubleClick;
			base.MouseDown += ItemsMouseDown;
			base.MouseMove += ItemsMouseMove;
			base.MouseHover += ItemsMouseHover;
			base.MouseUp += ItemsMouseUp;
		}

		private void ItemsDoubleClick(object sender, EventArgs e)
		{
			if (owner.activation == ItemActivation.Standard)
			{
				owner.OnItemActivate(EventArgs.Empty);
			}
		}

		private void InvalidateBoxSelectRect()
		{
			if (!BoxSelectRectangle.Size.IsEmpty)
			{
				Rectangle boxSelectRectangle = BoxSelectRectangle;
				boxSelectRectangle.X--;
				boxSelectRectangle.Y--;
				boxSelectRectangle.Width += 2;
				boxSelectRectangle.Height = 2;
				Invalidate(boxSelectRectangle);
				boxSelectRectangle.Y = BoxSelectRectangle.Bottom - 1;
				Invalidate(boxSelectRectangle);
				boxSelectRectangle.Y = BoxSelectRectangle.Y - 1;
				boxSelectRectangle.Width = 2;
				boxSelectRectangle.Height = BoxSelectRectangle.Height + 2;
				Invalidate(boxSelectRectangle);
				boxSelectRectangle.X = BoxSelectRectangle.Right - 1;
				Invalidate(boxSelectRectangle);
			}
		}

		private Rectangle CalculateBoxSelectRectangle(Point pt)
		{
			int left = Math.Min(box_select_start.X, pt.X);
			int right = Math.Max(box_select_start.X, pt.X);
			int top = Math.Min(box_select_start.Y, pt.Y);
			int bottom = Math.Max(box_select_start.Y, pt.Y);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}

		private bool BoxIntersectsItem(int index)
		{
			Rectangle rect = new Rectangle(owner.GetItemLocation(index), owner.ItemSize);
			if (owner.View != View.Details)
			{
				rect.X += rect.Width / 4;
				rect.Y += rect.Height / 4;
				rect.Width /= 2;
				rect.Height /= 2;
			}
			return BoxSelectRectangle.IntersectsWith(rect);
		}

		private bool BoxIntersectsText(int index)
		{
			Rectangle textBounds = owner.GetItemAtDisplayIndex(index).TextBounds;
			return BoxSelectRectangle.IntersectsWith(textBounds);
		}

		private bool PerformBoxSelection(Point pt)
		{
			if (box_select_mode == BoxSelect.None)
			{
				return false;
			}
			BoxSelectRectangle = CalculateBoxSelectRectangle(pt);
			ArrayList boxSelectedItems = BoxSelectedItems;
			ArrayList arrayList;
			switch (box_select_mode)
			{
			case BoxSelect.Normal:
				arrayList = boxSelectedItems;
				break;
			case BoxSelect.Control:
				arrayList = new ArrayList();
				foreach (int item in prev_selection)
				{
					if (!boxSelectedItems.Contains(owner.Items[item]))
					{
						arrayList.Add(owner.Items[item]);
					}
				}
				foreach (ListViewItem item2 in boxSelectedItems)
				{
					if (!prev_selection.Contains(item2.Index))
					{
						arrayList.Add(item2);
					}
				}
				break;
			case BoxSelect.Shift:
				arrayList = boxSelectedItems;
				foreach (ListViewItem item3 in boxSelectedItems)
				{
					prev_selection.Remove(item3.Index);
				}
				foreach (int item4 in prev_selection)
				{
					arrayList.Add(owner.Items[item4]);
				}
				break;
			default:
				throw new Exception("Unexpected Selection mode: " + box_select_mode);
			}
			SuspendLayout();
			owner.SelectItems(arrayList);
			ResumeLayout();
			return true;
		}

		private void ItemsMouseDown(object sender, MouseEventArgs me)
		{
			owner.OnMouseDown(owner.TranslateMouseEventArgs(me));
			if (owner.items.Count == 0)
			{
				return;
			}
			bool flag = false;
			Size itemSize = owner.ItemSize;
			Point pt = new Point(me.X, me.Y);
			for (int i = 0; i < owner.items.Count; i++)
			{
				if (!new Rectangle(owner.GetItemLocation(i), itemSize).Contains(pt))
				{
					continue;
				}
				ListViewItem itemAtDisplayIndex = owner.GetItemAtDisplayIndex(i);
				if (itemAtDisplayIndex.CheckRectReal.Contains(pt))
				{
					if (owner.StateImageList == null || owner.StateImageList.Images.Count >= 2)
					{
						if (me.Clicks == 2)
						{
							itemAtDisplayIndex.Checked = !itemAtDisplayIndex.Checked;
						}
						itemAtDisplayIndex.Checked = !itemAtDisplayIndex.Checked;
						checking = true;
					}
					return;
				}
				if (owner.View == View.Details)
				{
					bool flag2 = itemAtDisplayIndex.TextBounds.Contains(pt);
					if (owner.FullRowSelect)
					{
						clicked_item = itemAtDisplayIndex;
						bool flag3 = me.X > owner.Columns[0].X && me.X < owner.Columns[0].X + owner.Columns[0].Width;
						if (!flag2 && flag3 && owner.MultiSelect)
						{
							flag = true;
						}
					}
					else if (flag2)
					{
						clicked_item = itemAtDisplayIndex;
					}
					else
					{
						owner.SetFocusedItem(i);
					}
				}
				else
				{
					clicked_item = itemAtDisplayIndex;
				}
				break;
			}
			if (clicked_item != null)
			{
				bool flag4 = !clicked_item.Selected;
				if (me.Button == MouseButtons.Left || (XplatUI.State.ModifierKeys == Keys.None && flag4))
				{
					owner.SetFocusedItem(clicked_item.DisplayIndex);
				}
				if (owner.MultiSelect)
				{
					bool reselect = !owner.LabelEdit || flag4;
					if (me.Button == MouseButtons.Left || (XplatUI.State.ModifierKeys == Keys.None && flag4))
					{
						owner.UpdateMultiSelection(clicked_item.DisplayIndex, reselect);
					}
				}
				else
				{
					clicked_item.Selected = true;
				}
				if (owner.VirtualMode && flag4)
				{
					ListViewVirtualItemsSelectionRangeChangedEventArgs e = new ListViewVirtualItemsSelectionRangeChangedEventArgs(0, owner.items.Count - 1, isSelected: false);
					owner.OnVirtualItemsSelectionRangeChanged(e);
				}
				clicks = me.Clicks;
				if (me.Clicks > 1)
				{
					if (owner.CheckBoxes)
					{
						clicked_item.Checked = !clicked_item.Checked;
					}
				}
				else if (me.Clicks == 1 && owner.LabelEdit && !flag4)
				{
					BeginEdit(clicked_item);
				}
			}
			else if (owner.MultiSelect)
			{
				flag = true;
			}
			else if (owner.SelectedItems.Count > 0)
			{
				owner.SelectedItems.Clear();
			}
			if (flag)
			{
				Keys modifierKeys = XplatUI.State.ModifierKeys;
				if ((modifierKeys & Keys.Shift) != 0)
				{
					box_select_mode = BoxSelect.Shift;
				}
				else if ((modifierKeys & Keys.Control) != 0)
				{
					box_select_mode = BoxSelect.Control;
				}
				else
				{
					box_select_mode = BoxSelect.Normal;
				}
				box_select_start = pt;
				prev_selection = owner.SelectedIndices.List.Clone() as IList;
			}
		}

		private void ItemsMouseMove(object sender, MouseEventArgs me)
		{
			bool flag = PerformBoxSelection(new Point(me.X, me.Y));
			owner.OnMouseMove(owner.TranslateMouseEventArgs(me));
			if (flag || (me.Button != MouseButtons.Left && me.Button != MouseButtons.Right && !hover_processed && owner.Activation != ItemActivation.OneClick && !owner.ShowItemToolTips))
			{
				return;
			}
			Point point = PointToClient(Control.MousePosition);
			ListViewItem itemAt = owner.GetItemAt(point.X, point.Y);
			if (hover_processed && itemAt != null && itemAt != prev_hovered_item)
			{
				hover_processed = false;
				XplatUI.ResetMouseHover(Handle);
			}
			if (owner.Activation == ItemActivation.OneClick)
			{
				if (itemAt == null && owner.HotItemIndex != -1)
				{
					if (owner.HotTracking)
					{
						Invalidate(owner.Items[owner.HotItemIndex].Bounds);
					}
					Cursor = Cursors.Default;
					owner.HotItemIndex = -1;
				}
				else if (itemAt != null && owner.HotItemIndex == -1)
				{
					if (owner.HotTracking)
					{
						Invalidate(itemAt.Bounds);
					}
					Cursor = Cursors.Hand;
					owner.HotItemIndex = itemAt.Index;
				}
			}
			if (me.Button == MouseButtons.Left || me.Button == MouseButtons.Right)
			{
				if (drag_begin.X == -1 && drag_begin.Y == -1)
				{
					if (itemAt != null)
					{
						drag_begin = new Point(me.X, me.Y);
						dragged_item_index = itemAt.Index;
					}
				}
				else if (!new Rectangle(drag_begin, SystemInformation.DragSize).Contains(me.X, me.Y))
				{
					ListViewItem item = owner.items[dragged_item_index];
					owner.OnItemDrag(new ItemDragEventArgs(me.Button, item));
					drag_begin = new Point(-1, -1);
					dragged_item_index = -1;
				}
			}
			if (owner.ShowItemToolTips)
			{
				if (itemAt == null)
				{
					owner.item_tooltip.Active = false;
					prev_tooltip_item = null;
				}
				else if (itemAt != prev_tooltip_item && itemAt.ToolTipText.Length > 0)
				{
					owner.item_tooltip.Active = true;
					owner.item_tooltip.SetToolTip(owner, itemAt.ToolTipText);
					prev_tooltip_item = itemAt;
				}
			}
		}

		private void ItemsMouseHover(object sender, EventArgs e)
		{
			if (owner.hover_pending)
			{
				owner.OnMouseHover(e);
				owner.hover_pending = false;
			}
			if (base.Capture)
			{
				return;
			}
			hover_processed = true;
			Point point = PointToClient(Control.MousePosition);
			ListViewItem itemAt = owner.GetItemAt(point.X, point.Y);
			if (itemAt == null)
			{
				return;
			}
			prev_hovered_item = itemAt;
			if (owner.HoverSelection)
			{
				if (owner.MultiSelect)
				{
					owner.UpdateMultiSelection(itemAt.Index, reselect: true);
				}
				else
				{
					itemAt.Selected = true;
				}
				owner.SetFocusedItem(itemAt.DisplayIndex);
				Select();
			}
			owner.OnItemMouseHover(new ListViewItemMouseHoverEventArgs(itemAt));
		}

		private void HandleClicks(MouseEventArgs me)
		{
			if (clicks > 1)
			{
				owner.OnDoubleClick(EventArgs.Empty);
				owner.OnMouseDoubleClick(me);
			}
			else if (clicks == 1)
			{
				owner.OnClick(EventArgs.Empty);
				owner.OnMouseClick(me);
			}
			clicks = 0;
		}

		private void ItemsMouseUp(object sender, MouseEventArgs me)
		{
			MouseEventArgs mouseEventArgs = owner.TranslateMouseEventArgs(me);
			HandleClicks(mouseEventArgs);
			base.Capture = false;
			if (owner.Items.Count == 0)
			{
				ResetMouseState();
				owner.OnMouseUp(mouseEventArgs);
				return;
			}
			Point pt = new Point(me.X, me.Y);
			Rectangle empty = Rectangle.Empty;
			if (clicked_item != null)
			{
				if (((owner.view != View.Details || owner.full_row_select) ? clicked_item.Bounds : clicked_item.GetBounds(ItemBoundsPortion.Label)).Contains(pt))
				{
					switch (owner.activation)
					{
					case ItemActivation.OneClick:
						owner.OnItemActivate(EventArgs.Empty);
						break;
					case ItemActivation.TwoClick:
						if (last_clicked_item == clicked_item)
						{
							owner.OnItemActivate(EventArgs.Empty);
							last_clicked_item = null;
						}
						else
						{
							last_clicked_item = clicked_item;
						}
						break;
					}
				}
			}
			else if (!checking && owner.SelectedItems.Count > 0 && BoxSelectRectangle.Size.IsEmpty)
			{
				owner.SelectedItems.Clear();
			}
			ResetMouseState();
			owner.OnMouseUp(mouseEventArgs);
		}

		private void ResetMouseState()
		{
			clicked_item = null;
			box_select_start = Point.Empty;
			BoxSelectRectangle = Rectangle.Empty;
			prev_selection = null;
			box_select_mode = BoxSelect.None;
			checking = false;
			dragged_item_index = -1;
			drag_begin = new Point(-1, -1);
		}

		private void LabelEditFinished(object sender, EventArgs e)
		{
			EndEdit(edit_item);
		}

		private void LabelEditCancelled(object sender, EventArgs e)
		{
			edit_args.SetLabel(null);
			EndEdit(edit_item);
		}

		private void LabelTextChanged(object sender, EventArgs e)
		{
			if (edit_args != null)
			{
				edit_args.SetLabel(edit_text_box.Text);
			}
		}

		internal void BeginEdit(ListViewItem item)
		{
			if (edit_item != null)
			{
				EndEdit(edit_item);
			}
			if (edit_text_box == null)
			{
				edit_text_box = new ListViewLabelEditTextBox();
				edit_text_box.BorderStyle = BorderStyle.FixedSingle;
				edit_text_box.EditingCancelled += LabelEditCancelled;
				edit_text_box.EditingFinished += LabelEditFinished;
				edit_text_box.TextChanged += LabelTextChanged;
				edit_text_box.Visible = false;
				base.Controls.Add(edit_text_box);
			}
			item.EnsureVisible();
			edit_text_box.Reset();
			switch (owner.view)
			{
			case View.Details:
			case View.SmallIcon:
			case View.List:
			{
				edit_text_box.TextAlign = HorizontalAlignment.Left;
				edit_text_box.Bounds = item.GetBounds(ItemBoundsPortion.Label);
				SizeF sizeF = TextRenderer.MeasureString(item.Text, item.Font);
				edit_text_box.Width = (int)sizeF.Width + 4;
				edit_text_box.MaxWidth = owner.ClientRectangle.Width - edit_text_box.Bounds.X;
				edit_text_box.WordWrap = false;
				edit_text_box.Multiline = false;
				break;
			}
			case View.LargeIcon:
			{
				edit_text_box.TextAlign = HorizontalAlignment.Center;
				edit_text_box.Bounds = item.GetBounds(ItemBoundsPortion.Label);
				SizeF sizeF = TextRenderer.MeasureString(item.Text, item.Font);
				edit_text_box.Width = (int)sizeF.Width + 4;
				edit_text_box.MaxWidth = item.GetBounds(ItemBoundsPortion.Entire).Width;
				edit_text_box.MaxHeight = owner.ClientRectangle.Height - edit_text_box.Bounds.Y;
				edit_text_box.WordWrap = true;
				edit_text_box.Multiline = true;
				break;
			}
			}
			edit_item = item;
			edit_text_box.Text = item.Text;
			edit_text_box.Font = item.Font;
			edit_text_box.Visible = true;
			edit_text_box.Focus();
			edit_text_box.SelectAll();
			edit_args = new LabelEditEventArgs(owner.Items.IndexOf(edit_item));
			owner.OnBeforeLabelEdit(edit_args);
			if (edit_args.CancelEdit)
			{
				EndEdit(item);
			}
		}

		internal void CancelEdit(ListViewItem item)
		{
			if (edit_item != null && edit_item == item)
			{
				edit_args.SetLabel(null);
				EndEdit(item);
			}
		}

		internal void EndEdit(ListViewItem item)
		{
			if (edit_item == null || edit_item != item)
			{
				return;
			}
			if (edit_text_box != null)
			{
				if (edit_text_box.Visible)
				{
					edit_text_box.Visible = false;
				}
				owner.Focus();
			}
			Application.DoEvents();
			LabelEditEventArgs labelEditEventArgs = new LabelEditEventArgs(item.Index, edit_args.Label);
			edit_item = null;
			owner.OnAfterLabelEdit(labelEditEventArgs);
			if (!labelEditEventArgs.CancelEdit && labelEditEventArgs.Label != null)
			{
				item.Text = labelEditEventArgs.Label;
			}
		}

		internal override void OnPaintInternal(PaintEventArgs pe)
		{
			ThemeEngine.Current.DrawListViewItems(pe.Graphics, pe.ClipRectangle, owner);
		}

		protected override void WndProc(ref Message m)
		{
			switch ((Msg)m.Msg)
			{
			case Msg.WM_KILLFOCUS:
				owner.Select(directed: false, forward: true);
				break;
			case Msg.WM_SETFOCUS:
				owner.Select(directed: false, forward: true);
				break;
			case Msg.WM_LBUTTONDOWN:
				if (!Focused)
				{
					owner.Select(directed: false, forward: true);
				}
				break;
			case Msg.WM_RBUTTONDOWN:
				if (!Focused)
				{
					owner.Select(directed: false, forward: true);
				}
				break;
			}
			base.WndProc(ref m);
		}
	}

	internal class ListViewLabelEditTextBox : TextBox
	{
		private int max_width = -1;

		private int min_width = -1;

		private int max_height = -1;

		private int min_height = -1;

		private int old_number_lines = 1;

		private SizeF text_size_one_char;

		private static object EditingCancelledEvent;

		private static object EditingFinishedEvent;

		public int MaxWidth
		{
			set
			{
				if (value < min_width)
				{
					max_width = min_width;
				}
				else
				{
					max_width = value;
				}
			}
		}

		public int MaxHeight
		{
			set
			{
				if (value < min_height)
				{
					max_height = min_height;
				}
				else
				{
					max_height = value;
				}
			}
		}

		public new int Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				min_width = value;
				base.Width = value;
			}
		}

		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
				text_size_one_char = TextRenderer.MeasureString("B", Font);
			}
		}

		public event EventHandler EditingCancelled
		{
			add
			{
				base.Events.AddHandler(EditingCancelledEvent, value);
			}
			remove
			{
				base.Events.RemoveHandler(EditingCancelledEvent, value);
			}
		}

		public event EventHandler EditingFinished
		{
			add
			{
				base.Events.AddHandler(EditingFinishedEvent, value);
			}
			remove
			{
				base.Events.RemoveHandler(EditingFinishedEvent, value);
			}
		}

		public ListViewLabelEditTextBox()
		{
			min_height = DefaultSize.Height;
			text_size_one_char = TextRenderer.MeasureString("B", Font);
		}

		static ListViewLabelEditTextBox()
		{
			EditingCancelled = new object();
			EditingFinished = new object();
		}

		protected override void OnTextChanged(EventArgs e)
		{
			int new_width = (int)TextRenderer.MeasureString(Text, Font).Width + 8;
			if (!Multiline)
			{
				ResizeTextBoxWidth(new_width);
			}
			else
			{
				if (Width != max_width)
				{
					ResizeTextBoxWidth(new_width);
				}
				int num = base.Lines.Length;
				if (num != old_number_lines)
				{
					int new_height = num * (int)text_size_one_char.Height + 4;
					old_number_lines = num;
					ResizeTextBoxHeight(new_height);
				}
			}
			base.OnTextChanged(e);
		}

		protected override bool IsInputKey(Keys key_data)
		{
			if ((key_data & Keys.Alt) == 0)
			{
				switch (key_data & Keys.KeyCode)
				{
				case Keys.Return:
					return true;
				case Keys.Escape:
					return true;
				}
			}
			return base.IsInputKey(key_data);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (base.Visible)
			{
				switch (e.KeyCode)
				{
				case Keys.Return:
					base.Visible = false;
					e.Handled = true;
					OnEditingFinished(e);
					break;
				case Keys.Escape:
					base.Visible = false;
					e.Handled = true;
					OnEditingCancelled(e);
					break;
				}
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if (base.Visible)
			{
				OnEditingFinished(e);
			}
		}

		protected void OnEditingCancelled(EventArgs e)
		{
			((EventHandler)base.Events[EditingCancelled])?.Invoke(this, e);
		}

		protected void OnEditingFinished(EventArgs e)
		{
			((EventHandler)base.Events[EditingFinished])?.Invoke(this, e);
		}

		private void ResizeTextBoxWidth(int new_width)
		{
			if (new_width > max_width)
			{
				base.Width = max_width;
			}
			else if (new_width >= min_width)
			{
				base.Width = new_width;
			}
			else
			{
				base.Width = min_width;
			}
		}

		private void ResizeTextBoxHeight(int new_height)
		{
			if (new_height > max_height)
			{
				base.Height = max_height;
			}
			else if (new_height >= min_height)
			{
				base.Height = new_height;
			}
			else
			{
				base.Height = min_height;
			}
		}

		public void Reset()
		{
			max_width = -1;
			min_width = -1;
			max_height = -1;
			old_number_lines = 1;
			Text = string.Empty;
			base.Size = DefaultSize;
		}
	}

	internal class HeaderControl : Control
	{
		private ListView owner;

		private bool column_resize_active;

		private ColumnHeader resize_column;

		private ColumnHeader clicked_column;

		private ColumnHeader drag_column;

		private int drag_x;

		private int drag_to_index = -1;

		private ColumnHeader entered_column_header;

		internal ColumnHeader EnteredColumnHeader
		{
			get
			{
				return entered_column_header;
			}
			private set
			{
				if (entered_column_header == value)
				{
					return;
				}
				if (ThemeEngine.Current.ListViewHasHotHeaderStyle)
				{
					Region region = new Region();
					region.MakeEmpty();
					if (entered_column_header != null)
					{
						region.Union(GetColumnHeaderInvalidateArea(entered_column_header));
					}
					entered_column_header = value;
					if (entered_column_header != null)
					{
						region.Union(GetColumnHeaderInvalidateArea(entered_column_header));
					}
					Invalidate(region);
					region.Dispose();
				}
				else
				{
					entered_column_header = value;
				}
			}
		}

		public HeaderControl(ListView owner)
		{
			this.owner = owner;
			SetStyle(ControlStyles.DoubleBuffer, value: true);
			base.MouseDown += HeaderMouseDown;
			base.MouseMove += HeaderMouseMove;
			base.MouseUp += HeaderMouseUp;
			base.MouseLeave += OnMouseLeave;
		}

		private void OnMouseLeave(object sender, EventArgs e)
		{
			EnteredColumnHeader = null;
		}

		private ColumnHeader ColumnAtX(int x)
		{
			Point pt = new Point(x, 0);
			ColumnHeader result = null;
			foreach (ColumnHeader column in owner.Columns)
			{
				if (column.Rect.Contains(pt))
				{
					result = column;
					break;
				}
			}
			return result;
		}

		private int GetReorderedIndex(ColumnHeader col)
		{
			if (owner.reordered_column_indices == null)
			{
				return col.Index;
			}
			for (int i = 0; i < owner.Columns.Count; i++)
			{
				if (owner.reordered_column_indices[i] == col.Index)
				{
					return i;
				}
			}
			throw new Exception("Column index missing from reordered array");
		}

		private void HeaderMouseDown(object sender, MouseEventArgs me)
		{
			if (resize_column != null)
			{
				column_resize_active = true;
				base.Capture = true;
				return;
			}
			clicked_column = ColumnAtX(me.X + owner.h_marker);
			if (clicked_column != null)
			{
				base.Capture = true;
				if (owner.AllowColumnReorder)
				{
					drag_x = me.X;
					drag_column = (ColumnHeader)((ICloneable)clicked_column).Clone();
					drag_column.Rect = clicked_column.Rect;
					drag_to_index = GetReorderedIndex(clicked_column);
				}
				clicked_column.Pressed = true;
				Invalidate(clicked_column);
			}
		}

		private void Invalidate(ColumnHeader columnHeader)
		{
			Invalidate(GetColumnHeaderInvalidateArea(columnHeader));
		}

		private Rectangle GetColumnHeaderInvalidateArea(ColumnHeader columnHeader)
		{
			Rectangle rect = columnHeader.Rect;
			rect.X -= owner.h_marker;
			return rect;
		}

		private void StopResize()
		{
			column_resize_active = false;
			resize_column = null;
			base.Capture = false;
			Cursor = Cursors.Default;
		}

		private void HeaderMouseMove(object sender, MouseEventArgs me)
		{
			Point pt = new Point(me.X + owner.h_marker, me.Y);
			if (column_resize_active)
			{
				int num = pt.X - resize_column.X;
				if (num < 0)
				{
					num = 0;
				}
				if (!owner.CanProceedWithResize(resize_column, num))
				{
					StopResize();
				}
				else
				{
					resize_column.Width = num;
				}
				return;
			}
			resize_column = null;
			if (clicked_column != null)
			{
				if (owner.AllowColumnReorder)
				{
					Rectangle rect = drag_column.Rect;
					rect.X = clicked_column.Rect.X + me.X - drag_x;
					drag_column.Rect = rect;
					int num2 = me.X + owner.h_marker;
					ColumnHeader columnHeader = ColumnAtX(num2);
					if (columnHeader == null)
					{
						drag_to_index = owner.Columns.Count;
					}
					else if (num2 < columnHeader.X + columnHeader.Width / 2)
					{
						drag_to_index = GetReorderedIndex(columnHeader);
					}
					else
					{
						drag_to_index = GetReorderedIndex(columnHeader) + 1;
					}
					Invalidate();
				}
				else
				{
					ColumnHeader columnHeader2 = ColumnAtX(me.X + owner.h_marker);
					bool pressed = clicked_column.Pressed;
					clicked_column.Pressed = columnHeader2 == clicked_column;
					if (clicked_column.Pressed ^ pressed)
					{
						Invalidate(clicked_column);
					}
				}
				return;
			}
			for (int i = 0; i < owner.Columns.Count; i++)
			{
				Rectangle rect2 = owner.Columns[i].Rect;
				if (rect2.Contains(pt))
				{
					EnteredColumnHeader = owner.Columns[i];
				}
				rect2.X = rect2.Right - 5;
				rect2.Width = 10;
				if (rect2.Contains(pt))
				{
					if (i < owner.Columns.Count - 1 && owner.Columns[i + 1].Width == 0)
					{
						i++;
					}
					resize_column = owner.Columns[i];
					break;
				}
			}
			if (resize_column == null)
			{
				Cursor = Cursors.Default;
			}
			else
			{
				Cursor = Cursors.VSplit;
			}
		}

		private void HeaderMouseUp(object sender, MouseEventArgs me)
		{
			base.Capture = false;
			if (column_resize_active)
			{
				int index = resize_column.Index;
				StopResize();
				owner.RaiseColumnWidthChanged(index);
				return;
			}
			if (clicked_column != null && clicked_column.Pressed)
			{
				clicked_column.Pressed = false;
				Invalidate(clicked_column);
				owner.OnColumnClick(new ColumnClickEventArgs(clicked_column.Index));
			}
			if (drag_column != null && owner.AllowColumnReorder)
			{
				drag_column = null;
				if (drag_to_index > GetReorderedIndex(clicked_column))
				{
					drag_to_index--;
				}
				if (owner.GetReorderedColumn(drag_to_index) != clicked_column)
				{
					owner.ReorderColumn(clicked_column, drag_to_index, fireEvent: true);
				}
				drag_to_index = -1;
				Invalidate();
			}
			clicked_column = null;
		}

		internal override void OnPaintInternal(PaintEventArgs pe)
		{
			if (!owner.updating)
			{
				Theme current = ThemeEngine.Current;
				current.DrawListViewHeader(pe.Graphics, pe.ClipRectangle, owner);
				if (drag_column != null)
				{
					current.DrawListViewHeaderDragDetails(target_x: (drag_to_index != owner.Columns.Count) ? (owner.GetReorderedColumn(drag_to_index).Rect.X - owner.h_marker) : (owner.GetReorderedColumn(drag_to_index - 1).Rect.Right - owner.h_marker), dc: pe.Graphics, control: owner, drag_column: drag_column);
				}
			}
		}

		protected override void WndProc(ref Message m)
		{
			Msg msg = (Msg)m.Msg;
			if (msg == Msg.WM_SETFOCUS)
			{
				owner.Focus();
			}
			else
			{
				base.WndProc(ref m);
			}
		}
	}

	private class ItemComparer : IComparer
	{
		private readonly SortOrder sort_order;

		public ItemComparer(SortOrder sortOrder)
		{
			sort_order = sortOrder;
		}

		public int Compare(object x, object y)
		{
			ListViewItem listViewItem = x as ListViewItem;
			ListViewItem listViewItem2 = y as ListViewItem;
			if (sort_order == SortOrder.Ascending)
			{
				return string.Compare(listViewItem.Text, listViewItem2.Text);
			}
			return string.Compare(listViewItem2.Text, listViewItem.Text);
		}
	}

	[ListBindable(false)]
	public class CheckedIndexCollection : ICollection, IEnumerable, IList
	{
		private readonly ListView owner;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => true;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("SetItem operation is not supported.");
			}
		}

		[Browsable(false)]
		public int Count => owner.CheckedItems.Count;

		public bool IsReadOnly => true;

		public int this[int index]
		{
			get
			{
				int[] indices = GetIndices();
				if (index < 0 || index >= indices.Length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return indices[index];
			}
		}

		public CheckedIndexCollection(ListView owner)
		{
			this.owner = owner;
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			int[] indices = GetIndices();
			Array.Copy(indices, 0, dest, index, indices.Length);
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Add operation is not supported.");
		}

		void IList.Clear()
		{
			throw new NotSupportedException("Clear operation is not supported.");
		}

		bool IList.Contains(object checkedIndex)
		{
			if (!(checkedIndex is int))
			{
				return false;
			}
			return Contains((int)checkedIndex);
		}

		int IList.IndexOf(object checkedIndex)
		{
			if (!(checkedIndex is int))
			{
				return -1;
			}
			return IndexOf((int)checkedIndex);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Insert operation is not supported.");
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Remove operation is not supported.");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("RemoveAt operation is not supported.");
		}

		public bool Contains(int checkedIndex)
		{
			int[] indices = GetIndices();
			for (int i = 0; i < indices.Length; i++)
			{
				if (indices[i] == checkedIndex)
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerator GetEnumerator()
		{
			int[] indices = GetIndices();
			return indices.GetEnumerator();
		}

		public int IndexOf(int checkedIndex)
		{
			int[] indices = GetIndices();
			for (int i = 0; i < indices.Length; i++)
			{
				if (indices[i] == checkedIndex)
				{
					return i;
				}
			}
			return -1;
		}

		private int[] GetIndices()
		{
			ArrayList list = owner.CheckedItems.List;
			int[] array = new int[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				ListViewItem listViewItem = (ListViewItem)list[i];
				array[i] = listViewItem.Index;
			}
			return array;
		}
	}

	[ListBindable(false)]
	public class CheckedListViewItemCollection : ICollection, IEnumerable, IList
	{
		private readonly ListView owner;

		private ArrayList list;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => true;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("SetItem operation is not supported.");
			}
		}

		[Browsable(false)]
		public int Count
		{
			get
			{
				if (!owner.CheckBoxes)
				{
					return 0;
				}
				return List.Count;
			}
		}

		public bool IsReadOnly => true;

		public ListViewItem this[int index]
		{
			get
			{
				if (owner.VirtualMode)
				{
					throw new InvalidOperationException();
				}
				ArrayList arrayList = List;
				if (index < 0 || index >= arrayList.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (ListViewItem)arrayList[index];
			}
		}

		public virtual ListViewItem this[string key]
		{
			get
			{
				int num = IndexOfKey(key);
				return (num != -1) ? ((ListViewItem)List[num]) : null;
			}
		}

		internal ArrayList List
		{
			get
			{
				if (list == null)
				{
					list = new ArrayList();
					foreach (ListViewItem item in owner.Items)
					{
						if (item.Checked)
						{
							list.Add(item);
						}
					}
				}
				return list;
			}
		}

		public CheckedListViewItemCollection(ListView owner)
		{
			this.owner = owner;
			this.owner.Items.Changed += ItemsCollection_Changed;
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Add operation is not supported.");
		}

		void IList.Clear()
		{
			throw new NotSupportedException("Clear operation is not supported.");
		}

		bool IList.Contains(object item)
		{
			if (!(item is ListViewItem))
			{
				return false;
			}
			return Contains((ListViewItem)item);
		}

		int IList.IndexOf(object item)
		{
			if (!(item is ListViewItem))
			{
				return -1;
			}
			return IndexOf((ListViewItem)item);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Insert operation is not supported.");
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Remove operation is not supported.");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("RemoveAt operation is not supported.");
		}

		public bool Contains(ListViewItem item)
		{
			if (!owner.CheckBoxes)
			{
				return false;
			}
			return List.Contains(item);
		}

		public virtual bool ContainsKey(string key)
		{
			return IndexOfKey(key) != -1;
		}

		public void CopyTo(Array dest, int index)
		{
			if (owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			if (owner.CheckBoxes)
			{
				List.CopyTo(dest, index);
			}
		}

		public IEnumerator GetEnumerator()
		{
			if (owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			if (!owner.CheckBoxes)
			{
				return new ListViewItem[0].GetEnumerator();
			}
			return List.GetEnumerator();
		}

		public int IndexOf(ListViewItem item)
		{
			if (owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			if (!owner.CheckBoxes)
			{
				return -1;
			}
			return List.IndexOf(item);
		}

		public virtual int IndexOfKey(string key)
		{
			if (owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			if (key == null || key.Length == 0)
			{
				return -1;
			}
			ArrayList arrayList = List;
			for (int i = 0; i < arrayList.Count; i++)
			{
				ListViewItem listViewItem = (ListViewItem)arrayList[i];
				if (string.Compare(key, listViewItem.Name, ignoreCase: true) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		internal void Reset()
		{
			list = null;
		}

		private void ItemsCollection_Changed()
		{
			Reset();
		}
	}

	[ListBindable(false)]
	public class ColumnHeaderCollection : ICollection, IEnumerable, IList
	{
		internal ArrayList list;

		private ListView owner;

		private static object UIACollectionChangedEvent;

		bool ICollection.IsSynchronized => true;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => list.IsFixedSize;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("SetItem operation is not supported.");
			}
		}

		[Browsable(false)]
		public int Count => list.Count;

		public bool IsReadOnly => false;

		public virtual ColumnHeader this[int index]
		{
			get
			{
				if (index < 0 || index >= list.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (ColumnHeader)list[index];
			}
		}

		public virtual ColumnHeader this[string key]
		{
			get
			{
				int num = IndexOfKey(key);
				if (num == -1)
				{
					return null;
				}
				return (ColumnHeader)list[num];
			}
		}

		internal event CollectionChangeEventHandler UIACollectionChanged
		{
			add
			{
				if (owner != null)
				{
					owner.Events.AddHandler(UIACollectionChangedEvent, value);
				}
			}
			remove
			{
				if (owner != null)
				{
					owner.Events.RemoveHandler(UIACollectionChangedEvent, value);
				}
			}
		}

		public ColumnHeaderCollection(ListView owner)
		{
			list = new ArrayList();
			this.owner = owner;
		}

		static ColumnHeaderCollection()
		{
			UIACollectionChanged = new object();
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			list.CopyTo(dest, index);
		}

		int IList.Add(object value)
		{
			if (!(value is ColumnHeader))
			{
				throw new ArgumentException("Not of type ColumnHeader", "value");
			}
			return Add((ColumnHeader)value);
		}

		bool IList.Contains(object value)
		{
			if (!(value is ColumnHeader))
			{
				throw new ArgumentException("Not of type ColumnHeader", "value");
			}
			return Contains((ColumnHeader)value);
		}

		int IList.IndexOf(object value)
		{
			if (!(value is ColumnHeader))
			{
				throw new ArgumentException("Not of type ColumnHeader", "value");
			}
			return IndexOf((ColumnHeader)value);
		}

		void IList.Insert(int index, object value)
		{
			if (!(value is ColumnHeader))
			{
				throw new ArgumentException("Not of type ColumnHeader", "value");
			}
			Insert(index, (ColumnHeader)value);
		}

		void IList.Remove(object value)
		{
			if (!(value is ColumnHeader))
			{
				throw new ArgumentException("Not of type ColumnHeader", "value");
			}
			Remove((ColumnHeader)value);
		}

		internal void OnUIACollectionChangedEvent(CollectionChangeEventArgs args)
		{
			if (owner != null)
			{
				((CollectionChangeEventHandler)owner.Events[UIACollectionChanged])?.Invoke(owner, args);
			}
		}

		public virtual int Add(ColumnHeader value)
		{
			int num = list.Add(value);
			owner.AddColumn(value, num, redraw: true);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
			return num;
		}

		public virtual ColumnHeader Add(string text, int width, HorizontalAlignment textAlign)
		{
			ColumnHeader columnHeader = new ColumnHeader(owner, text, textAlign, width);
			Add(columnHeader);
			return columnHeader;
		}

		public virtual ColumnHeader Add(string text)
		{
			return Add(string.Empty, text);
		}

		public virtual ColumnHeader Add(string text, int width)
		{
			return Add(string.Empty, text, width);
		}

		public virtual ColumnHeader Add(string key, string text)
		{
			ColumnHeader columnHeader = new ColumnHeader();
			columnHeader.Name = key;
			columnHeader.Text = text;
			Add(columnHeader);
			return columnHeader;
		}

		public virtual ColumnHeader Add(string key, string text, int width)
		{
			return Add(key, text, width, HorizontalAlignment.Left, -1);
		}

		public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
		{
			ColumnHeader columnHeader = new ColumnHeader(key, text, width, textAlign);
			columnHeader.ImageIndex = imageIndex;
			Add(columnHeader);
			return columnHeader;
		}

		public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
		{
			ColumnHeader columnHeader = new ColumnHeader(key, text, width, textAlign);
			columnHeader.ImageKey = imageKey;
			Add(columnHeader);
			return columnHeader;
		}

		public virtual void AddRange(ColumnHeader[] values)
		{
			foreach (ColumnHeader columnHeader in values)
			{
				int index = list.Add(columnHeader);
				owner.AddColumn(columnHeader, index, redraw: false);
			}
			owner.Redraw(recalculate: true);
		}

		public virtual void Clear()
		{
			foreach (ColumnHeader item in list)
			{
				item.SetListView(null);
			}
			list.Clear();
			owner.ReorderColumns(new int[0], redraw: true);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}

		public bool Contains(ColumnHeader value)
		{
			return list.Contains(value);
		}

		public virtual bool ContainsKey(string key)
		{
			return IndexOfKey(key) != -1;
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public int IndexOf(ColumnHeader value)
		{
			return list.IndexOf(value);
		}

		public virtual int IndexOfKey(string key)
		{
			if (key == null || key.Length == 0)
			{
				return -1;
			}
			for (int i = 0; i < list.Count; i++)
			{
				ColumnHeader columnHeader = (ColumnHeader)list[i];
				if (string.Compare(key, columnHeader.Name, ignoreCase: true) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, ColumnHeader value)
		{
			if (index < 0 || index > list.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			list.Insert(index, value);
			owner.AddColumn(value, index, redraw: true);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}

		public void Insert(int index, string text)
		{
			Insert(index, string.Empty, text);
		}

		public void Insert(int index, string text, int width)
		{
			Insert(index, string.Empty, text, width);
		}

		public void Insert(int index, string key, string text)
		{
			ColumnHeader columnHeader = new ColumnHeader();
			columnHeader.Name = key;
			columnHeader.Text = text;
			Insert(index, columnHeader);
		}

		public void Insert(int index, string key, string text, int width)
		{
			ColumnHeader value = new ColumnHeader(key, text, width, HorizontalAlignment.Left);
			Insert(index, value);
		}

		public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
		{
			ColumnHeader columnHeader = new ColumnHeader(key, text, width, textAlign);
			columnHeader.ImageIndex = imageIndex;
			Insert(index, columnHeader);
		}

		public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
		{
			ColumnHeader columnHeader = new ColumnHeader(key, text, width, textAlign);
			columnHeader.ImageKey = imageKey;
			Insert(index, columnHeader);
		}

		public void Insert(int index, string text, int width, HorizontalAlignment textAlign)
		{
			ColumnHeader value = new ColumnHeader(owner, text, textAlign, width);
			Insert(index, value);
		}

		public virtual void Remove(ColumnHeader column)
		{
			if (!Contains(column))
			{
				return;
			}
			list.Remove(column);
			column.SetListView(null);
			int internalDisplayIndex = column.InternalDisplayIndex;
			int[] array = new int[list.Count];
			for (int i = 0; i < array.Length; i++)
			{
				ColumnHeader columnHeader = (ColumnHeader)list[i];
				int internalDisplayIndex2 = columnHeader.InternalDisplayIndex;
				if (internalDisplayIndex2 < internalDisplayIndex)
				{
					array[i] = internalDisplayIndex2;
				}
				else
				{
					array[i] = internalDisplayIndex2 - 1;
				}
			}
			column.InternalDisplayIndex = -1;
			owner.ReorderColumns(array, redraw: true);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, column));
		}

		public virtual void RemoveByKey(string key)
		{
			int num = IndexOfKey(key);
			if (num != -1)
			{
				RemoveAt(num);
			}
		}

		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= list.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			ColumnHeader column = (ColumnHeader)list[index];
			Remove(column);
		}
	}

	[ListBindable(false)]
	public class ListViewItemCollection : ICollection, IEnumerable, IList
	{
		private readonly ArrayList list;

		private ListView owner;

		private ListViewGroup group;

		private static object UIACollectionChangedEvent;

		private bool is_main_collection = true;

		bool ICollection.IsSynchronized => true;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => list.IsFixedSize;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, this[index]));
				if (value is ListViewItem)
				{
					this[index] = (ListViewItem)value;
				}
				else
				{
					this[index] = new ListViewItem(value.ToString());
				}
				OnChange();
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
			}
		}

		[Browsable(false)]
		public int Count
		{
			get
			{
				if (owner != null && owner.VirtualMode)
				{
					return owner.VirtualListSize;
				}
				return list.Count;
			}
		}

		public bool IsReadOnly => false;

		public virtual ListViewItem this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (owner != null && owner.VirtualMode)
				{
					return RetrieveVirtualItemFromOwner(index);
				}
				return (ListViewItem)list[index];
			}
			set
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (owner != null && owner.VirtualMode)
				{
					throw new InvalidOperationException();
				}
				if (list.Contains(value))
				{
					throw new ArgumentException("An item cannot be added more than once. To add an item again, you need to clone it.", "value");
				}
				if (value.ListView != null && value.ListView != owner)
				{
					throw new ArgumentException("Cannot add or insert the item '" + value.Text + "' in more than one place. You must first remove it from its current location or clone it.", "value");
				}
				if (is_main_collection)
				{
					value.Owner = owner;
				}
				else
				{
					if (value.Group != null)
					{
						value.Group.Items.Remove(value);
					}
					value.SetGroup(group);
				}
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, list[index]));
				list[index] = value;
				CollectionChanged(sort: true);
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
			}
		}

		public virtual ListViewItem this[string key]
		{
			get
			{
				int num = IndexOfKey(key);
				if (num == -1)
				{
					return null;
				}
				return this[num];
			}
		}

		internal ListView Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}

		internal ListViewGroup Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		internal event CollectionChangeEventHandler UIACollectionChanged
		{
			add
			{
				if (owner != null)
				{
					owner.Events.AddHandler(UIACollectionChangedEvent, value);
				}
			}
			remove
			{
				if (owner != null)
				{
					owner.Events.RemoveHandler(UIACollectionChangedEvent, value);
				}
			}
		}

		internal event CollectionChangedHandler Changed;

		public ListViewItemCollection(ListView owner)
		{
			list = new ArrayList(0);
			this.owner = owner;
		}

		internal ListViewItemCollection(ListView owner, ListViewGroup group)
			: this(owner)
		{
			this.group = group;
			is_main_collection = false;
		}

		static ListViewItemCollection()
		{
			UIACollectionChanged = new object();
		}

		int IList.Add(object item)
		{
			if (owner != null && owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			ListViewItem listViewItem;
			if (item is ListViewItem)
			{
				listViewItem = (ListViewItem)item;
				if (list.Contains(listViewItem))
				{
					throw new ArgumentException("An item cannot be added more than once. To add an item again, you need to clone it.", "item");
				}
				if (listViewItem.ListView != null && listViewItem.ListView != owner)
				{
					throw new ArgumentException("Cannot add or insert the item '" + listViewItem.Text + "' in more than one place. You must first remove it from its current location or clone it.", "item");
				}
			}
			else
			{
				listViewItem = new ListViewItem(item.ToString());
			}
			listViewItem.Owner = owner;
			int result = list.Add(listViewItem);
			CollectionChanged(sort: true);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, listViewItem));
			return result;
		}

		bool IList.Contains(object item)
		{
			return Contains((ListViewItem)item);
		}

		int IList.IndexOf(object item)
		{
			return IndexOf((ListViewItem)item);
		}

		void IList.Insert(int index, object item)
		{
			if (item is ListViewItem)
			{
				Insert(index, (ListViewItem)item);
			}
			else
			{
				Insert(index, item.ToString());
			}
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, this[index]));
		}

		void IList.Remove(object item)
		{
			Remove((ListViewItem)item);
		}

		internal void OnUIACollectionChangedEvent(CollectionChangeEventArgs args)
		{
			if (owner != null)
			{
				((CollectionChangeEventHandler)owner.Events[UIACollectionChanged])?.Invoke(owner, args);
			}
		}

		public virtual ListViewItem Add(ListViewItem value)
		{
			if (owner != null && owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			AddItem(value);
			if (is_main_collection || value.ListView != null)
			{
				CollectionChanged(sort: true);
			}
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
			return value;
		}

		public virtual ListViewItem Add(string text)
		{
			ListViewItem value = new ListViewItem(text);
			return Add(value);
		}

		public virtual ListViewItem Add(string text, int imageIndex)
		{
			ListViewItem value = new ListViewItem(text, imageIndex);
			return Add(value);
		}

		public virtual ListViewItem Add(string text, string imageKey)
		{
			ListViewItem value = new ListViewItem(text, imageKey);
			return Add(value);
		}

		public virtual ListViewItem Add(string key, string text, int imageIndex)
		{
			ListViewItem listViewItem = new ListViewItem(text, imageIndex);
			listViewItem.Name = key;
			return Add(listViewItem);
		}

		public virtual ListViewItem Add(string key, string text, string imageKey)
		{
			ListViewItem listViewItem = new ListViewItem(text, imageKey);
			listViewItem.Name = key;
			return Add(listViewItem);
		}

		public void AddRange(ListViewItem[] items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("Argument cannot be null!", "items");
			}
			if (owner != null && owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			owner.BeginUpdate();
			foreach (ListViewItem listViewItem in items)
			{
				AddItem(listViewItem);
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, listViewItem));
			}
			owner.EndUpdate();
			CollectionChanged(sort: true);
		}

		public void AddRange(ListViewItemCollection items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("Argument cannot be null!", "items");
			}
			ListViewItem[] array = new ListViewItem[items.Count];
			items.CopyTo(array, 0);
			AddRange(array);
		}

		public virtual void Clear()
		{
			if (owner != null && owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			if (is_main_collection && owner != null)
			{
				owner.SetFocusedItem(-1);
				ScrollBar h_scroll = owner.h_scroll;
				int value = 0;
				owner.v_scroll.Value = value;
				h_scroll.Value = value;
				foreach (ListViewGroup group in owner.groups)
				{
					group.Items.ClearItemsWithSameListView();
				}
				foreach (ListViewItem item in list)
				{
					owner.item_control.CancelEdit(item);
					item.Owner = null;
				}
			}
			else
			{
				foreach (ListViewItem item2 in list)
				{
					item2.SetGroup(null);
				}
			}
			list.Clear();
			CollectionChanged(sort: false);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}

		private void ClearItemsWithSameListView()
		{
			if (is_main_collection)
			{
				return;
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				ListViewItem listViewItem = list[num] as ListViewItem;
				if (listViewItem.ListView == group.ListView)
				{
					list.RemoveAt(num);
					listViewItem.SetGroup(null);
				}
			}
		}

		public bool Contains(ListViewItem item)
		{
			return IndexOf(item) != -1;
		}

		public virtual bool ContainsKey(string key)
		{
			return IndexOfKey(key) != -1;
		}

		public void CopyTo(Array dest, int index)
		{
			list.CopyTo(dest, index);
		}

		public ListViewItem[] Find(string key, bool searchAllSubItems)
		{
			if (key == null)
			{
				return new ListViewItem[0];
			}
			List<ListViewItem> list = new List<ListViewItem>();
			for (int i = 0; i < this.list.Count; i++)
			{
				ListViewItem listViewItem = (ListViewItem)this.list[i];
				if (string.Compare(key, listViewItem.Name, ignoreCase: true) == 0)
				{
					list.Add(listViewItem);
				}
			}
			ListViewItem[] array = new ListViewItem[list.Count];
			list.CopyTo(array);
			return array;
		}

		public IEnumerator GetEnumerator()
		{
			if (owner != null && owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			return new ControlCollection.ControlCollectionEnumerator(list);
		}

		public int IndexOf(ListViewItem item)
		{
			if (owner != null && owner.VirtualMode)
			{
				for (int i = 0; i < Count; i++)
				{
					if (RetrieveVirtualItemFromOwner(i) == item)
					{
						return i;
					}
				}
				return -1;
			}
			return list.IndexOf(item);
		}

		public virtual int IndexOfKey(string key)
		{
			if (key == null || key.Length == 0)
			{
				return -1;
			}
			for (int i = 0; i < Count; i++)
			{
				ListViewItem listViewItem = this[i];
				if (string.Compare(key, listViewItem.Name, ignoreCase: true) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public ListViewItem Insert(int index, ListViewItem item)
		{
			if (index < 0 || index > list.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (owner != null && owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			if (list.Contains(item))
			{
				throw new ArgumentException("An item cannot be added more than once. To add an item again, you need to clone it.", "item");
			}
			if (item.ListView != null && item.ListView != owner)
			{
				throw new ArgumentException("Cannot add or insert the item '" + item.Text + "' in more than one place. You must first remove it from its current location or clone it.", "item");
			}
			if (is_main_collection)
			{
				item.Owner = owner;
			}
			else
			{
				if (item.Group != null)
				{
					item.Group.Items.Remove(item);
				}
				item.SetGroup(group);
			}
			list.Insert(index, item);
			if (is_main_collection || item.ListView != null)
			{
				CollectionChanged(sort: true);
			}
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
			return item;
		}

		public ListViewItem Insert(int index, string text)
		{
			return Insert(index, new ListViewItem(text));
		}

		public ListViewItem Insert(int index, string text, int imageIndex)
		{
			return Insert(index, new ListViewItem(text, imageIndex));
		}

		public ListViewItem Insert(int index, string text, string imageKey)
		{
			ListViewItem item = new ListViewItem(text, imageKey);
			return Insert(index, item);
		}

		public virtual ListViewItem Insert(int index, string key, string text, int imageIndex)
		{
			ListViewItem listViewItem = new ListViewItem(text, imageIndex);
			listViewItem.Name = key;
			return Insert(index, listViewItem);
		}

		public virtual ListViewItem Insert(int index, string key, string text, string imageKey)
		{
			ListViewItem listViewItem = new ListViewItem(text, imageKey);
			listViewItem.Name = key;
			return Insert(index, listViewItem);
		}

		public virtual void Remove(ListViewItem item)
		{
			if (owner != null && owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			int num = list.IndexOf(item);
			if (num != -1)
			{
				RemoveAt(num);
			}
		}

		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (owner != null && owner.VirtualMode)
			{
				throw new InvalidOperationException();
			}
			ListViewItem listViewItem = (ListViewItem)list[index];
			bool flag = false;
			if (is_main_collection && owner != null)
			{
				int displayIndex = listViewItem.DisplayIndex;
				if (listViewItem.Focused && displayIndex + 1 == Count)
				{
					owner.SetFocusedItem((displayIndex != 0) ? (displayIndex - 1) : (-1));
				}
				flag = owner.SelectedIndices.Contains(index);
				owner.item_control.CancelEdit(listViewItem);
			}
			list.RemoveAt(index);
			if (is_main_collection)
			{
				listViewItem.Owner = null;
				if (listViewItem.Group != null)
				{
					listViewItem.Group.Items.Remove(listViewItem);
				}
			}
			else
			{
				listViewItem.SetGroup(null);
			}
			CollectionChanged(sort: false);
			if (flag && owner != null)
			{
				owner.OnSelectedIndexChanged(EventArgs.Empty);
			}
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, listViewItem));
		}

		public virtual void RemoveByKey(string key)
		{
			int num = IndexOfKey(key);
			if (num != -1)
			{
				RemoveAt(num);
			}
		}

		private void AddItem(ListViewItem value)
		{
			if (list.Contains(value))
			{
				throw new ArgumentException("An item cannot be added more than once. To add an item again, you need to clone it.", "value");
			}
			if (value.ListView != null && value.ListView != owner)
			{
				throw new ArgumentException("Cannot add or insert the item '" + value.Text + "' in more than one place. You must first remove it from its current location or clone it.", "value");
			}
			if (is_main_collection)
			{
				value.Owner = owner;
			}
			else
			{
				if (value.Group != null)
				{
					value.Group.Items.Remove(value);
				}
				value.SetGroup(group);
			}
			list.Add(value);
		}

		private void CollectionChanged(bool sort)
		{
			if (owner != null)
			{
				if (sort)
				{
					owner.Sort(redraw: false);
				}
				OnChange();
				owner.Redraw(recalculate: true);
			}
		}

		private ListViewItem RetrieveVirtualItemFromOwner(int displayIndex)
		{
			RetrieveVirtualItemEventArgs retrieveVirtualItemEventArgs = new RetrieveVirtualItemEventArgs(displayIndex);
			owner.OnRetrieveVirtualItem(retrieveVirtualItemEventArgs);
			ListViewItem item = retrieveVirtualItemEventArgs.Item;
			item.Owner = owner;
			item.DisplayIndex = displayIndex;
			return item;
		}

		internal void Sort(IComparer comparer)
		{
			list.Sort(comparer);
			OnChange();
		}

		internal void OnChange()
		{
			if (this.Changed != null)
			{
				this.Changed();
			}
		}
	}

	[ListBindable(false)]
	public class SelectedIndexCollection : ICollection, IEnumerable, IList
	{
		private readonly ListView owner;

		private ArrayList list;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => false;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("SetItem operation is not supported.");
			}
		}

		[Browsable(false)]
		public int Count
		{
			get
			{
				if (!owner.IsHandleCreated)
				{
					return 0;
				}
				return List.Count;
			}
		}

		public bool IsReadOnly => false;

		public int this[int index]
		{
			get
			{
				if (!owner.IsHandleCreated || index < 0 || index >= List.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return (int)List[index];
			}
		}

		internal ArrayList List
		{
			get
			{
				if (list == null)
				{
					list = new ArrayList();
					if (!owner.VirtualMode)
					{
						for (int i = 0; i < owner.Items.Count; i++)
						{
							if (owner.Items[i].Selected)
							{
								list.Add(i);
							}
						}
					}
				}
				return list;
			}
		}

		public SelectedIndexCollection(ListView owner)
		{
			this.owner = owner;
			owner.Items.Changed += ItemsCollection_Changed;
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Add operation is not supported.");
		}

		void IList.Clear()
		{
			Clear();
		}

		bool IList.Contains(object selectedIndex)
		{
			if (!(selectedIndex is int))
			{
				return false;
			}
			return Contains((int)selectedIndex);
		}

		int IList.IndexOf(object selectedIndex)
		{
			if (!(selectedIndex is int))
			{
				return -1;
			}
			return IndexOf((int)selectedIndex);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Insert operation is not supported.");
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Remove operation is not supported.");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("RemoveAt operation is not supported.");
		}

		public int Add(int itemIndex)
		{
			if (itemIndex < 0 || itemIndex >= owner.Items.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (owner.virtual_mode && !owner.IsHandleCreated)
			{
				return -1;
			}
			owner.Items[itemIndex].Selected = true;
			if (!owner.IsHandleCreated)
			{
				return 0;
			}
			return List.Count;
		}

		public void Clear()
		{
			if (owner.IsHandleCreated)
			{
				int[] array = (int[])List.ToArray(typeof(int));
				int[] array2 = array;
				foreach (int index in array2)
				{
					owner.Items[index].Selected = false;
				}
			}
		}

		public bool Contains(int selectedIndex)
		{
			return IndexOf(selectedIndex) != -1;
		}

		public void CopyTo(Array dest, int index)
		{
			List.CopyTo(dest, index);
		}

		public IEnumerator GetEnumerator()
		{
			return List.GetEnumerator();
		}

		public int IndexOf(int selectedIndex)
		{
			if (!owner.IsHandleCreated)
			{
				return -1;
			}
			return List.IndexOf(selectedIndex);
		}

		public void Remove(int itemIndex)
		{
			if (itemIndex < 0 || itemIndex >= owner.Items.Count)
			{
				throw new ArgumentOutOfRangeException("itemIndex");
			}
			owner.Items[itemIndex].Selected = false;
		}

		internal void Reset()
		{
			list = null;
		}

		private void ItemsCollection_Changed()
		{
			Reset();
		}

		internal void RemoveIndex(int index)
		{
			int num = List.BinarySearch(index);
			if (num != -1)
			{
				List.RemoveAt(num);
			}
		}

		internal void InsertIndex(int index)
		{
			int num = 0;
			int num2 = List.Count - 1;
			while (num <= num2)
			{
				int num3 = (num + num2) / 2;
				int num4 = (int)List[num3];
				if (num4 == index)
				{
					return;
				}
				if (num4 > index)
				{
					num2 = num3 - 1;
				}
				else
				{
					num = num3 + 1;
				}
			}
			List.Insert(num, index);
		}
	}

	[ListBindable(false)]
	public class SelectedListViewItemCollection : ICollection, IEnumerable, IList
	{
		private readonly ListView owner;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => true;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException("SetItem operation is not supported.");
			}
		}

		[Browsable(false)]
		public int Count => owner.SelectedIndices.Count;

		public bool IsReadOnly => true;

		public ListViewItem this[int index]
		{
			get
			{
				if (!owner.IsHandleCreated || index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				int index2 = owner.SelectedIndices[index];
				return owner.Items[index2];
			}
		}

		public virtual ListViewItem this[string key]
		{
			get
			{
				int num = IndexOfKey(key);
				if (num == -1)
				{
					return null;
				}
				return this[num];
			}
		}

		public SelectedListViewItemCollection(ListView owner)
		{
			this.owner = owner;
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException("Add operation is not supported.");
		}

		bool IList.Contains(object item)
		{
			if (!(item is ListViewItem))
			{
				return false;
			}
			return Contains((ListViewItem)item);
		}

		int IList.IndexOf(object item)
		{
			if (!(item is ListViewItem))
			{
				return -1;
			}
			return IndexOf((ListViewItem)item);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException("Insert operation is not supported.");
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException("Remove operation is not supported.");
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException("RemoveAt operation is not supported.");
		}

		public void Clear()
		{
			owner.SelectedIndices.Clear();
		}

		public bool Contains(ListViewItem item)
		{
			return IndexOf(item) != -1;
		}

		public virtual bool ContainsKey(string key)
		{
			return IndexOfKey(key) != -1;
		}

		public void CopyTo(Array dest, int index)
		{
			if (owner.IsHandleCreated)
			{
				if (index > Count)
				{
					throw new ArgumentException("index");
				}
				for (int i = 0; i < Count; i++)
				{
					dest.SetValue(this[i], index++);
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			if (!owner.IsHandleCreated)
			{
				return new ListViewItem[0].GetEnumerator();
			}
			ListViewItem[] array = new ListViewItem[Count];
			for (int i = 0; i < Count; i++)
			{
				array[i] = this[i];
			}
			return array.GetEnumerator();
		}

		public int IndexOf(ListViewItem item)
		{
			if (!owner.IsHandleCreated)
			{
				return -1;
			}
			for (int i = 0; i < Count; i++)
			{
				if (this[i] == item)
				{
					return i;
				}
			}
			return -1;
		}

		public virtual int IndexOfKey(string key)
		{
			if (!owner.IsHandleCreated || key == null || key.Length == 0)
			{
				return -1;
			}
			for (int i = 0; i < Count; i++)
			{
				ListViewItem listViewItem = this[i];
				if (string.Compare(listViewItem.Name, key, ignoreCase: true) == 0)
				{
					return i;
				}
			}
			return -1;
		}
	}

	private struct ItemMatrixLocation
	{
		private int row;

		private int col;

		public int Col
		{
			get
			{
				return col;
			}
			set
			{
				col = value;
			}
		}

		public int Row
		{
			get
			{
				return row;
			}
			set
			{
				row = value;
			}
		}

		public ItemMatrixLocation(int row, int col)
		{
			this.row = row;
			this.col = col;
		}
	}

	internal delegate void CollectionChangedHandler();

	private const int text_padding = 15;

	private const int max_wrap_padding = 30;

	private ItemActivation activation;

	private ListViewAlignment alignment = ListViewAlignment.Top;

	private bool allow_column_reorder;

	private bool auto_arrange = true;

	private bool check_boxes;

	private readonly CheckedIndexCollection checked_indices;

	private readonly CheckedListViewItemCollection checked_items;

	private readonly ColumnHeaderCollection columns;

	internal int focused_item_index = -1;

	private bool full_row_select;

	private bool grid_lines;

	private ColumnHeaderStyle header_style = ColumnHeaderStyle.Clickable;

	private bool hide_selection = true;

	private bool hover_selection;

	private IComparer item_sorter;

	private readonly ListViewItemCollection items;

	private readonly ListViewGroupCollection groups;

	private bool owner_draw;

	private bool show_groups = true;

	private bool label_edit;

	private bool label_wrap = true;

	private bool multiselect = true;

	private bool scrollable = true;

	private bool hover_pending;

	private readonly SelectedIndexCollection selected_indices;

	private readonly SelectedListViewItemCollection selected_items;

	private SortOrder sort_order;

	private ImageList state_image_list;

	internal bool updating;

	private View view;

	private int layout_wd;

	private int layout_ht;

	internal HeaderControl header_control;

	internal ItemControl item_control;

	internal ScrollBar h_scroll;

	internal ScrollBar v_scroll;

	internal int h_marker;

	internal int v_marker;

	private int keysearch_tickcnt;

	private string keysearch_text;

	private static readonly int keysearch_keydelay = 1000;

	private int[] reordered_column_indices;

	private int[] reordered_items_indices;

	private Point[] items_location;

	private ItemMatrixLocation[] items_matrix_location;

	private Size item_size;

	private int custom_column_width;

	private int hot_item_index = -1;

	private bool hot_tracking;

	private ListViewInsertionMark insertion_mark;

	private bool show_item_tooltips;

	private ToolTip item_tooltip;

	private Size tile_size;

	private bool virtual_mode;

	private int virtual_list_size;

	private bool right_to_left_layout;

	internal ImageList large_image_list;

	internal ImageList small_image_list;

	internal Size text_size = Size.Empty;

	private static object AfterLabelEditEvent;

	private static object BeforeLabelEditEvent;

	private static object ColumnClickEvent;

	private static object ItemActivateEvent;

	private static object ItemCheckEvent;

	private static object ItemDragEvent;

	private static object SelectedIndexChangedEvent;

	private static object DrawColumnHeaderEvent;

	private static object DrawItemEvent;

	private static object DrawSubItemEvent;

	private static object ItemCheckedEvent;

	private static object ItemMouseHoverEvent;

	private static object ItemSelectionChangedEvent;

	private static object CacheVirtualItemsEvent;

	private static object RetrieveVirtualItemEvent;

	private static object RightToLeftLayoutChangedEvent;

	private static object SearchForVirtualItemEvent;

	private static object VirtualItemsSelectionRangeChangedEvent;

	private int x_spacing;

	private int y_spacing;

	private int rows;

	private int cols;

	private int[,] item_index_matrix;

	private ListViewItem selection_start;

	private bool refocusing;

	private static object ColumnReorderedEvent;

	private static object ColumnWidthChangedEvent;

	private static object ColumnWidthChangingEvent;

	private static object UIALabelEditChangedEvent;

	private static object UIAShowGroupsChangedEvent;

	private static object UIAMultiSelectChangedEvent;

	private static object UIAViewChangedEvent;

	private static object UIACheckBoxesChangedEvent;

	private static object UIAFocusedItemChangedEvent;

	internal Size CheckBoxSize
	{
		get
		{
			if (check_boxes)
			{
				if (state_image_list != null)
				{
					return state_image_list.ImageSize;
				}
				return ThemeEngine.Current.ListViewCheckBoxSize;
			}
			return Size.Empty;
		}
	}

	internal Size ItemSize
	{
		get
		{
			if (view != View.Details)
			{
				return item_size;
			}
			Size result = default(Size);
			result.Height = item_size.Height;
			for (int i = 0; i < columns.Count; i++)
			{
				result.Width += columns[i].Wd;
			}
			return result;
		}
		set
		{
			item_size = value;
		}
	}

	internal int HotItemIndex
	{
		get
		{
			return hot_item_index;
		}
		set
		{
			hot_item_index = value;
		}
	}

	internal bool UsingGroups => show_groups && groups.Count > 0 && view != View.List && Application.VisualStylesEnabled;

	internal override bool ScaleChildrenInternal => false;

	internal bool UseCustomColumnWidth => (view == View.List || view == View.SmallIcon) && columns.Count > 0;

	internal ColumnHeader EnteredColumnHeader => header_control.EnteredColumnHeader;

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => ThemeEngine.Current.ListViewDefaultSize;

	protected override bool DoubleBuffered
	{
		get
		{
			return base.DoubleBuffered;
		}
		set
		{
			base.DoubleBuffered = value;
		}
	}

	[DefaultValue(ItemActivation.Standard)]
	public ItemActivation Activation
	{
		get
		{
			return activation;
		}
		set
		{
			if (value != 0 && value != ItemActivation.OneClick && value != ItemActivation.TwoClick)
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for Activation");
			}
			if (hot_tracking && value != ItemActivation.OneClick)
			{
				throw new ArgumentException("When HotTracking is on, activation must be ItemActivation.OneClick");
			}
			activation = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(ListViewAlignment.Top)]
	public ListViewAlignment Alignment
	{
		get
		{
			return alignment;
		}
		set
		{
			if (value != 0 && value != ListViewAlignment.Left && value != ListViewAlignment.SnapToGrid && value != ListViewAlignment.Top)
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for Alignment");
			}
			if (alignment != value)
			{
				alignment = value;
				if (view == View.LargeIcon || View == View.SmallIcon)
				{
					Redraw(recalculate: true);
				}
			}
		}
	}

	[DefaultValue(false)]
	public bool AllowColumnReorder
	{
		get
		{
			return allow_column_reorder;
		}
		set
		{
			allow_column_reorder = value;
		}
	}

	[DefaultValue(true)]
	public bool AutoArrange
	{
		get
		{
			return auto_arrange;
		}
		set
		{
			if (auto_arrange != value)
			{
				auto_arrange = value;
				if (view == View.LargeIcon || View == View.SmallIcon)
				{
					Redraw(recalculate: true);
				}
			}
		}
	}

	public override Color BackColor
	{
		get
		{
			if (background_color.IsEmpty)
			{
				return ThemeEngine.Current.ColorWindow;
			}
			return background_color;
		}
		set
		{
			background_color = value;
			item_control.BackColor = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
			base.BackgroundImageLayout = value;
		}
	}

	[DefaultValue(false)]
	public bool BackgroundImageTiled
	{
		get
		{
			return item_control.BackgroundImageLayout == ImageLayout.Tile;
		}
		set
		{
			ImageLayout imageLayout = (value ? ImageLayout.Tile : ImageLayout.None);
			if (imageLayout != item_control.BackgroundImageLayout)
			{
				item_control.BackgroundImageLayout = imageLayout;
			}
		}
	}

	[DispId(-504)]
	[DefaultValue(BorderStyle.Fixed3D)]
	public BorderStyle BorderStyle
	{
		get
		{
			return base.InternalBorderStyle;
		}
		set
		{
			base.InternalBorderStyle = value;
		}
	}

	[DefaultValue(false)]
	public bool CheckBoxes
	{
		get
		{
			return check_boxes;
		}
		set
		{
			if (check_boxes != value)
			{
				if (value && View == View.Tile)
				{
					throw new NotSupportedException("CheckBoxes are not supported in Tile view. Choose a different view or set CheckBoxes to false.");
				}
				check_boxes = value;
				Redraw(recalculate: true);
				OnUIACheckBoxesChanged();
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public CheckedIndexCollection CheckedIndices => checked_indices;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public CheckedListViewItemCollection CheckedItems => checked_items;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Editor("System.Windows.Forms.Design.ColumnHeaderCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Localizable(true)]
	[MergableProperty(false)]
	public ColumnHeaderCollection Columns => columns;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ListViewItem FocusedItem
	{
		get
		{
			if (focused_item_index == -1)
			{
				return null;
			}
			return GetItemAtDisplayIndex(focused_item_index);
		}
		set
		{
			if (value != null && value.ListView == this && base.IsHandleCreated)
			{
				SetFocusedItem(value.DisplayIndex);
			}
		}
	}

	public override Color ForeColor
	{
		get
		{
			if (foreground_color.IsEmpty)
			{
				return ThemeEngine.Current.ColorWindowText;
			}
			return foreground_color;
		}
		set
		{
			foreground_color = value;
		}
	}

	[DefaultValue(false)]
	public bool FullRowSelect
	{
		get
		{
			return full_row_select;
		}
		set
		{
			if (full_row_select != value)
			{
				full_row_select = value;
				InvalidateSelection();
			}
		}
	}

	[DefaultValue(false)]
	public bool GridLines
	{
		get
		{
			return grid_lines;
		}
		set
		{
			if (grid_lines != value)
			{
				grid_lines = value;
				Redraw(recalculate: false);
			}
		}
	}

	[DefaultValue(ColumnHeaderStyle.Clickable)]
	public ColumnHeaderStyle HeaderStyle
	{
		get
		{
			return header_style;
		}
		set
		{
			if (header_style == value)
			{
				return;
			}
			switch (value)
			{
			default:
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ColumnHeaderStyle");
			case ColumnHeaderStyle.None:
			case ColumnHeaderStyle.Nonclickable:
			case ColumnHeaderStyle.Clickable:
				header_style = value;
				if (view == View.Details)
				{
					Redraw(recalculate: true);
				}
				break;
			}
		}
	}

	[DefaultValue(true)]
	public bool HideSelection
	{
		get
		{
			return hide_selection;
		}
		set
		{
			if (hide_selection != value)
			{
				hide_selection = value;
				InvalidateSelection();
			}
		}
	}

	[DefaultValue(false)]
	public bool HotTracking
	{
		get
		{
			return hot_tracking;
		}
		set
		{
			if (hot_tracking != value)
			{
				hot_tracking = value;
				if (hot_tracking)
				{
					hover_selection = true;
					activation = ItemActivation.OneClick;
				}
			}
		}
	}

	[DefaultValue(false)]
	public bool HoverSelection
	{
		get
		{
			return hover_selection;
		}
		set
		{
			if (hot_tracking && !value)
			{
				throw new ArgumentException("When HotTracking is on, hover selection must be true");
			}
			hover_selection = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public ListViewInsertionMark InsertionMark => insertion_mark;

	[Editor("System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Localizable(true)]
	[MergableProperty(false)]
	public ListViewItemCollection Items => items;

	[DefaultValue(false)]
	public bool LabelEdit
	{
		get
		{
			return label_edit;
		}
		set
		{
			if (value != label_edit)
			{
				label_edit = value;
				OnUIALabelEditChanged();
			}
		}
	}

	[DefaultValue(true)]
	[Localizable(true)]
	public bool LabelWrap
	{
		get
		{
			return label_wrap;
		}
		set
		{
			if (label_wrap != value)
			{
				label_wrap = value;
				Redraw(recalculate: true);
			}
		}
	}

	[DefaultValue(null)]
	public ImageList LargeImageList
	{
		get
		{
			return large_image_list;
		}
		set
		{
			large_image_list = value;
			Redraw(recalculate: true);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IComparer ListViewItemSorter
	{
		get
		{
			if (View != View.SmallIcon && View != 0 && item_sorter is ItemComparer)
			{
				return null;
			}
			return item_sorter;
		}
		set
		{
			if (item_sorter != value)
			{
				item_sorter = value;
				Sort();
			}
		}
	}

	[DefaultValue(true)]
	public bool MultiSelect
	{
		get
		{
			return multiselect;
		}
		set
		{
			if (value != multiselect)
			{
				multiselect = value;
				OnUIAMultiSelectChanged();
			}
		}
	}

	[DefaultValue(false)]
	public bool OwnerDraw
	{
		get
		{
			return owner_draw;
		}
		set
		{
			owner_draw = value;
			Redraw(recalculate: true);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			base.Padding = value;
		}
	}

	[System.MonoTODO("RTL not supported")]
	[Localizable(true)]
	[DefaultValue(false)]
	public virtual bool RightToLeftLayout
	{
		get
		{
			return right_to_left_layout;
		}
		set
		{
			if (right_to_left_layout != value)
			{
				right_to_left_layout = value;
				OnRightToLeftLayoutChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(true)]
	public bool Scrollable
	{
		get
		{
			return scrollable;
		}
		set
		{
			if (scrollable != value)
			{
				scrollable = value;
				Redraw(recalculate: true);
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public SelectedIndexCollection SelectedIndices => selected_indices;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public SelectedListViewItemCollection SelectedItems => selected_items;

	[DefaultValue(true)]
	public bool ShowGroups
	{
		get
		{
			return show_groups;
		}
		set
		{
			if (show_groups != value)
			{
				show_groups = value;
				Redraw(recalculate: true);
				OnUIAShowGroupsChanged();
			}
		}
	}

	[Localizable(true)]
	[MergableProperty(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Editor("System.Windows.Forms.Design.ListViewGroupCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public ListViewGroupCollection Groups => groups;

	[DefaultValue(false)]
	public bool ShowItemToolTips
	{
		get
		{
			return show_item_tooltips;
		}
		set
		{
			show_item_tooltips = value;
			item_tooltip.Active = false;
		}
	}

	[DefaultValue(null)]
	public ImageList SmallImageList
	{
		get
		{
			return small_image_list;
		}
		set
		{
			small_image_list = value;
			Redraw(recalculate: true);
		}
	}

	[DefaultValue(SortOrder.None)]
	public SortOrder Sorting
	{
		get
		{
			return sort_order;
		}
		set
		{
			if (!Enum.IsDefined(typeof(SortOrder), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(SortOrder));
			}
			if (sort_order == value)
			{
				return;
			}
			sort_order = value;
			if (virtual_mode)
			{
				return;
			}
			if (value == SortOrder.None)
			{
				if (item_sorter != null && View != View.SmallIcon && View != 0)
				{
					item_sorter = null;
				}
				Redraw(recalculate: false);
				return;
			}
			if (item_sorter == null)
			{
				item_sorter = new ItemComparer(value);
			}
			if (item_sorter is ItemComparer)
			{
				item_sorter = new ItemComparer(value);
			}
			Sort();
		}
	}

	[DefaultValue(null)]
	public ImageList StateImageList
	{
		get
		{
			return state_image_list;
		}
		set
		{
			if (state_image_list != value)
			{
				if (state_image_list != null)
				{
					state_image_list.Images.Changed -= OnImageListChanged;
				}
				state_image_list = value;
				if (state_image_list != null)
				{
					state_image_list.Images.Changed += OnImageListChanged;
				}
				Redraw(recalculate: true);
			}
		}
	}

	[Bindable(false)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			if (!(value == base.Text))
			{
				base.Text = value;
				Redraw(recalculate: true);
			}
		}
	}

	[Browsable(true)]
	public Size TileSize
	{
		get
		{
			return tile_size;
		}
		set
		{
			if (value.Width <= 0 || value.Height <= 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			tile_size = value;
			if (view == View.Tile)
			{
				Redraw(recalculate: true);
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public ListViewItem TopItem
	{
		get
		{
			if (view == View.LargeIcon || view == View.SmallIcon || view == View.Tile)
			{
				throw new InvalidOperationException("Cannot get the top item in LargeIcon, SmallIcon or Tile view.");
			}
			if (items.Count == 0)
			{
				return null;
			}
			if (h_marker == 0 && v_marker == 0)
			{
				return items[0];
			}
			int height = header_control.Height;
			for (int i = 0; i < items.Count; i++)
			{
				Point itemLocation = GetItemLocation(i);
				if (itemLocation.X >= 0 && itemLocation.Y - height >= 0)
				{
					return items[i];
				}
			}
			return null;
		}
		set
		{
			if (view == View.LargeIcon || view == View.SmallIcon || view == View.Tile)
			{
				throw new InvalidOperationException("Cannot set the top item in LargeIcon, SmallIcon or Tile view.");
			}
			if (value != null && value.ListView == this)
			{
				SetScrollValue(v_scroll, item_size.Height * value.Index);
			}
		}
	}

	[System.MonoInternalNote("Stub, not implemented")]
	[Browsable(false)]
	[DefaultValue(true)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public bool UseCompatibleStateImageBehavior
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	[DefaultValue(View.LargeIcon)]
	public View View
	{
		get
		{
			return view;
		}
		set
		{
			if (!Enum.IsDefined(typeof(View), value))
			{
				throw new InvalidEnumArgumentException("value", (int)value, typeof(View));
			}
			if (view != value)
			{
				if (CheckBoxes && value == View.Tile)
				{
					throw new NotSupportedException("CheckBoxes are not supported in Tile view. Choose a different view or set CheckBoxes to false.");
				}
				if (VirtualMode && value == View.Tile)
				{
					throw new NotSupportedException("VirtualMode is not supported in Tile view. Choose a different view or set ViewMode to false.");
				}
				ScrollBar scrollBar = h_scroll;
				int value2 = 0;
				v_scroll.Value = value2;
				scrollBar.Value = value2;
				view = value;
				Redraw(recalculate: true);
				OnUIAViewChanged();
			}
		}
	}

	[DefaultValue(false)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public bool VirtualMode
	{
		get
		{
			return virtual_mode;
		}
		set
		{
			if (virtual_mode != value)
			{
				if (!virtual_mode && items.Count > 0)
				{
					throw new InvalidOperationException();
				}
				if (value && view == View.Tile)
				{
					throw new NotSupportedException("VirtualMode is not supported in Tile view. Choose a different view or set ViewMode to false.");
				}
				virtual_mode = value;
				Redraw(recalculate: true);
			}
		}
	}

	[DefaultValue(0)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public int VirtualListSize
	{
		get
		{
			return virtual_list_size;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("value");
			}
			if (virtual_list_size != value)
			{
				virtual_list_size = value;
				if (virtual_mode)
				{
					selected_indices.Reset();
					Redraw(recalculate: true);
				}
			}
		}
	}

	internal int FirstVisibleIndex
	{
		get
		{
			if (items.Count == 0)
			{
				return 0;
			}
			if (h_marker == 0 && v_marker == 0)
			{
				return 0;
			}
			Size itemSize = ItemSize;
			if (virtual_mode)
			{
				int num = 0;
				switch (view)
				{
				case View.Details:
					num = v_marker / itemSize.Height;
					break;
				case View.LargeIcon:
				case View.SmallIcon:
					num = v_marker / (itemSize.Height + y_spacing) * cols;
					break;
				case View.List:
					num = h_marker / (itemSize.Width * x_spacing) * rows;
					break;
				}
				if (num >= items.Count)
				{
					num = items.Count;
				}
				return num;
			}
			for (int i = 0; i < items.Count; i++)
			{
				Rectangle rectangle = new Rectangle(GetItemLocation(i), itemSize);
				if (rectangle.Right >= 0 && rectangle.Bottom >= 0)
				{
					return i;
				}
			}
			return 0;
		}
	}

	internal int LastVisibleIndex
	{
		get
		{
			for (int i = FirstVisibleIndex; i < Items.Count; i++)
			{
				if (View == View.List || Alignment == ListViewAlignment.Left)
				{
					if (GetItemLocation(i).X > item_control.ClientRectangle.Right)
					{
						return i - 1;
					}
				}
				else if (GetItemLocation(i).Y > item_control.ClientRectangle.Bottom)
				{
					return i - 1;
				}
			}
			return Items.Count - 1;
		}
	}

	internal int TotalWidth => Math.Max(base.Width, layout_wd);

	internal int TotalHeight => Math.Max(base.Height, layout_ht);

	private Size LargeIconItemSize
	{
		get
		{
			int val = ((LargeImageList != null) ? LargeImageList.ImageSize.Width : 12);
			int val2 = ((LargeImageList != null) ? LargeImageList.ImageSize.Height : 2);
			int height = text_size.Height + 2 + Math.Max(CheckBoxSize.Height, val2);
			int num = Math.Max(text_size.Width, val);
			if (check_boxes)
			{
				num += 2 + CheckBoxSize.Width;
			}
			return new Size(num, height);
		}
	}

	private Size SmallIconItemSize
	{
		get
		{
			int num = ((SmallImageList != null) ? SmallImageList.ImageSize.Width : 0);
			int val = ((SmallImageList != null) ? SmallImageList.ImageSize.Height : 0);
			int height = Math.Max(text_size.Height, Math.Max(CheckBoxSize.Height, val));
			int num2 = text_size.Width + num;
			if (check_boxes)
			{
				num2 += 2 + CheckBoxSize.Width;
			}
			return new Size(num2, height);
		}
	}

	private Size TileItemSize
	{
		get
		{
			if (tile_size == Size.Empty)
			{
				int num = ((LargeImageList != null) ? LargeImageList.ImageSize.Width : 0);
				int val = ((LargeImageList != null) ? LargeImageList.ImageSize.Height : 0);
				int width = (int)Font.Size * ThemeEngine.Current.ListViewTileWidthFactor + num + 4;
				int height = Math.Max((int)Font.Size * ThemeEngine.Current.ListViewTileHeightFactor, val);
				tile_size = new Size(width, height);
			}
			return tile_size;
		}
	}

	internal Rectangle UIAHeaderControl => header_control.Bounds;

	internal int UIAColumns => cols;

	internal int UIARows => rows;

	internal ListViewGroup UIADefaultListViewGroup => groups.DefaultGroup;

	internal ScrollBar UIAHScrollBar => h_scroll;

	internal ScrollBar UIAVScrollBar => v_scroll;

	internal int UIAItemsLocationLength => items_location.Length;

	public event LabelEditEventHandler AfterLabelEdit
	{
		add
		{
			base.Events.AddHandler(AfterLabelEditEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AfterLabelEditEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageLayoutChanged
	{
		add
		{
			base.BackgroundImageLayoutChanged += value;
		}
		remove
		{
			base.BackgroundImageLayoutChanged -= value;
		}
	}

	public event LabelEditEventHandler BeforeLabelEdit
	{
		add
		{
			base.Events.AddHandler(BeforeLabelEditEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BeforeLabelEditEvent, value);
		}
	}

	public event ColumnClickEventHandler ColumnClick
	{
		add
		{
			base.Events.AddHandler(ColumnClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ColumnClickEvent, value);
		}
	}

	public event DrawListViewColumnHeaderEventHandler DrawColumnHeader
	{
		add
		{
			base.Events.AddHandler(DrawColumnHeaderEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DrawColumnHeaderEvent, value);
		}
	}

	public event DrawListViewItemEventHandler DrawItem
	{
		add
		{
			base.Events.AddHandler(DrawItemEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DrawItemEvent, value);
		}
	}

	public event DrawListViewSubItemEventHandler DrawSubItem
	{
		add
		{
			base.Events.AddHandler(DrawSubItemEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DrawSubItemEvent, value);
		}
	}

	public event EventHandler ItemActivate
	{
		add
		{
			base.Events.AddHandler(ItemActivateEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemActivateEvent, value);
		}
	}

	public event ItemCheckEventHandler ItemCheck
	{
		add
		{
			base.Events.AddHandler(ItemCheckEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemCheckEvent, value);
		}
	}

	public event ItemCheckedEventHandler ItemChecked
	{
		add
		{
			base.Events.AddHandler(ItemCheckedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemCheckedEvent, value);
		}
	}

	public event ItemDragEventHandler ItemDrag
	{
		add
		{
			base.Events.AddHandler(ItemDragEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemDragEvent, value);
		}
	}

	public event ListViewItemMouseHoverEventHandler ItemMouseHover
	{
		add
		{
			base.Events.AddHandler(ItemMouseHoverEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemMouseHoverEvent, value);
		}
	}

	public event ListViewItemSelectionChangedEventHandler ItemSelectionChanged
	{
		add
		{
			base.Events.AddHandler(ItemSelectionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ItemSelectionChangedEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler PaddingChanged
	{
		add
		{
			base.PaddingChanged += value;
		}
		remove
		{
			base.PaddingChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event PaintEventHandler Paint
	{
		add
		{
			base.Paint += value;
		}
		remove
		{
			base.Paint -= value;
		}
	}

	public event EventHandler SelectedIndexChanged
	{
		add
		{
			base.Events.AddHandler(SelectedIndexChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectedIndexChangedEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler TextChanged
	{
		add
		{
			base.TextChanged += value;
		}
		remove
		{
			base.TextChanged -= value;
		}
	}

	public event CacheVirtualItemsEventHandler CacheVirtualItems
	{
		add
		{
			base.Events.AddHandler(CacheVirtualItemsEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CacheVirtualItemsEvent, value);
		}
	}

	public event RetrieveVirtualItemEventHandler RetrieveVirtualItem
	{
		add
		{
			base.Events.AddHandler(RetrieveVirtualItemEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(RetrieveVirtualItemEvent, value);
		}
	}

	public event EventHandler RightToLeftLayoutChanged
	{
		add
		{
			base.Events.AddHandler(RightToLeftLayoutChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(RightToLeftLayoutChangedEvent, value);
		}
	}

	public event SearchForVirtualItemEventHandler SearchForVirtualItem
	{
		add
		{
			base.Events.AddHandler(SearchForVirtualItemEvent, value);
		}
		remove
		{
			base.Events.AddHandler(SearchForVirtualItemEvent, value);
		}
	}

	public event ListViewVirtualItemsSelectionRangeChangedEventHandler VirtualItemsSelectionRangeChanged
	{
		add
		{
			base.Events.AddHandler(VirtualItemsSelectionRangeChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(VirtualItemsSelectionRangeChangedEvent, value);
		}
	}

	public event ColumnReorderedEventHandler ColumnReordered
	{
		add
		{
			base.Events.AddHandler(ColumnReorderedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ColumnReorderedEvent, value);
		}
	}

	public event ColumnWidthChangedEventHandler ColumnWidthChanged
	{
		add
		{
			base.Events.AddHandler(ColumnWidthChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ColumnWidthChangedEvent, value);
		}
	}

	public event ColumnWidthChangingEventHandler ColumnWidthChanging
	{
		add
		{
			base.Events.AddHandler(ColumnWidthChangingEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ColumnWidthChangingEvent, value);
		}
	}

	internal event EventHandler UIAShowGroupsChanged
	{
		add
		{
			base.Events.AddHandler(UIAShowGroupsChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAShowGroupsChangedEvent, value);
		}
	}

	internal event EventHandler UIACheckBoxesChanged
	{
		add
		{
			base.Events.AddHandler(UIACheckBoxesChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIACheckBoxesChangedEvent, value);
		}
	}

	internal event EventHandler UIAMultiSelectChanged
	{
		add
		{
			base.Events.AddHandler(UIAMultiSelectChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAMultiSelectChangedEvent, value);
		}
	}

	internal event EventHandler UIALabelEditChanged
	{
		add
		{
			base.Events.AddHandler(UIALabelEditChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIALabelEditChangedEvent, value);
		}
	}

	internal event EventHandler UIAViewChanged
	{
		add
		{
			base.Events.AddHandler(UIAViewChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAViewChangedEvent, value);
		}
	}

	internal event EventHandler UIAFocusedItemChanged
	{
		add
		{
			base.Events.AddHandler(UIAFocusedItemChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAFocusedItemChangedEvent, value);
		}
	}

	public ListView()
	{
		background_color = ThemeEngine.Current.ColorWindow;
		groups = new ListViewGroupCollection(this);
		items = new ListViewItemCollection(this);
		items.Changed += OnItemsChanged;
		checked_indices = new CheckedIndexCollection(this);
		checked_items = new CheckedListViewItemCollection(this);
		columns = new ColumnHeaderCollection(this);
		foreground_color = SystemColors.WindowText;
		selected_indices = new SelectedIndexCollection(this);
		selected_items = new SelectedListViewItemCollection(this);
		items_location = new Point[16];
		items_matrix_location = new ItemMatrixLocation[16];
		reordered_items_indices = new int[16];
		item_tooltip = new ToolTip();
		item_tooltip.Active = false;
		insertion_mark = new ListViewInsertionMark(this);
		base.InternalBorderStyle = BorderStyle.Fixed3D;
		header_control = new HeaderControl(this);
		header_control.Visible = false;
		base.Controls.AddImplicit(header_control);
		item_control = new ItemControl(this);
		base.Controls.AddImplicit(item_control);
		h_scroll = new ImplicitHScrollBar();
		base.Controls.AddImplicit(h_scroll);
		v_scroll = new ImplicitVScrollBar();
		base.Controls.AddImplicit(v_scroll);
		h_marker = (v_marker = 0);
		keysearch_tickcnt = 0;
		h_scroll.Visible = false;
		h_scroll.ValueChanged += HorizontalScroller;
		v_scroll.Visible = false;
		v_scroll.ValueChanged += VerticalScroller;
		base.KeyDown += ListView_KeyDown;
		base.SizeChanged += ListView_SizeChanged;
		base.GotFocus += FocusChanged;
		base.LostFocus += FocusChanged;
		base.MouseWheel += ListView_MouseWheel;
		base.MouseEnter += ListView_MouseEnter;
		base.Invalidated += ListView_Invalidated;
		BackgroundImageTiled = false;
		SetStyle(ControlStyles.UserPaint | ControlStyles.StandardClick | ControlStyles.UseTextForAccessibility, value: false);
	}

	static ListView()
	{
		AfterLabelEdit = new object();
		BeforeLabelEdit = new object();
		ColumnClick = new object();
		ItemActivate = new object();
		ItemCheck = new object();
		ItemDrag = new object();
		SelectedIndexChanged = new object();
		DrawColumnHeader = new object();
		DrawItem = new object();
		DrawSubItem = new object();
		ItemChecked = new object();
		ItemMouseHover = new object();
		ItemSelectionChanged = new object();
		CacheVirtualItems = new object();
		RetrieveVirtualItem = new object();
		RightToLeftLayoutChanged = new object();
		SearchForVirtualItem = new object();
		VirtualItemsSelectionRangeChanged = new object();
		ColumnReordered = new object();
		ColumnWidthChanged = new object();
		ColumnWidthChanging = new object();
		UIALabelEditChanged = new object();
		UIAShowGroupsChanged = new object();
		UIAMultiSelectChanged = new object();
		UIAViewChanged = new object();
		UIACheckBoxesChanged = new object();
		UIAFocusedItemChanged = new object();
	}

	private void OnImageListChanged(object sender, EventArgs args)
	{
		item_control.Invalidate();
	}

	internal void OnSelectedIndexChanged()
	{
		if (base.IsHandleCreated)
		{
			OnSelectedIndexChanged(EventArgs.Empty);
		}
	}

	internal void Redraw(bool recalculate)
	{
		if (!updating && (!virtual_mode || base.IsHandleCreated))
		{
			if (recalculate)
			{
				CalculateListView(alignment);
			}
			Invalidate(invalidateChildren: true);
		}
	}

	private void InvalidateSelection()
	{
		foreach (int selectedIndex in SelectedIndices)
		{
			items[selectedIndex].Invalidate();
		}
	}

	internal Size GetChildColumnSize(int index)
	{
		Size empty = Size.Empty;
		ColumnHeader columnHeader = columns[index];
		if (columnHeader.Width == -2)
		{
			Size size = Size.Ceiling(TextRenderer.MeasureString(columnHeader.Text, Font));
			size.Width += 15;
			empty = BiggestItem(index);
			if (size.Width > empty.Width)
			{
				empty = size;
			}
		}
		else
		{
			empty = BiggestItem(index);
			if (empty.IsEmpty)
			{
				empty.Width = ThemeEngine.Current.ListViewEmptyColumnWidth;
				if (columnHeader.Text.Length > 0)
				{
					empty.Height = Size.Ceiling(TextRenderer.MeasureString(columnHeader.Text, Font)).Height;
				}
				else
				{
					empty.Height = Font.Height;
				}
			}
		}
		empty.Height += 15;
		if (index == 0)
		{
			empty.Width += CheckBoxSize.Width + 4;
			if (small_image_list != null)
			{
				empty.Width += small_image_list.ImageSize.Width;
			}
		}
		return empty;
	}

	private Size BiggestItem(int col)
	{
		Size empty = Size.Empty;
		Size result = Size.Empty;
		bool flag = small_image_list != null;
		if (virtual_mode && items.Count > 0)
		{
			ListViewItem listViewItem = items[0];
			result = Size.Ceiling(TextRenderer.MeasureString(listViewItem.SubItems[col].Text, Font));
			if (flag)
			{
				result.Width += listViewItem.IndentCount * small_image_list.ImageSize.Width;
			}
		}
		else
		{
			foreach (ListViewItem item in items)
			{
				if (col < item.SubItems.Count)
				{
					empty = Size.Ceiling(TextRenderer.MeasureString(item.SubItems[col].Text, Font));
					if (flag)
					{
						empty.Width += item.IndentCount * small_image_list.ImageSize.Width;
					}
					if (empty.Width > result.Width)
					{
						result = empty;
					}
				}
			}
		}
		if (!result.IsEmpty && view == View.Details)
		{
			result.Width += ThemeEngine.Current.ListViewItemPaddingWidth;
		}
		return result;
	}

	private void CalcTextSize()
	{
		text_size = Size.Empty;
		if (items.Count == 0)
		{
			return;
		}
		text_size = BiggestItem(0);
		if (view == View.LargeIcon && label_wrap)
		{
			Size empty = Size.Empty;
			if (check_boxes)
			{
				empty.Width += 2 * CheckBoxSize.Width;
			}
			int num = ((LargeImageList != null) ? LargeImageList.ImageSize.Width : 12);
			empty.Width += num + 30;
			if (text_size.Width > empty.Width)
			{
				text_size.Width = empty.Width;
				text_size.Height *= 2;
			}
		}
		else if (view == View.List)
		{
			int num2 = base.Width - (CheckBoxSize.Width - 2);
			if (small_image_list != null)
			{
				num2 -= small_image_list.ImageSize.Width;
			}
			if (text_size.Width > num2)
			{
				text_size.Width = num2;
			}
		}
		if (text_size.Height <= 0)
		{
			text_size.Height = Font.Height;
		}
		if (text_size.Width <= 0)
		{
			text_size.Width = base.Width;
		}
		text_size.Width += 2;
		text_size.Height += 2;
	}

	private void SetScrollValue(ScrollBar scrollbar, int val)
	{
		int num = ((scrollbar != h_scroll) ? (v_scroll.Maximum - item_control.Height) : (h_scroll.Maximum - item_control.Width));
		if (val > num)
		{
			val = num;
		}
		else if (val < scrollbar.Minimum)
		{
			val = scrollbar.Minimum;
		}
		scrollbar.Value = val;
	}

	private void Scroll(ScrollBar scrollbar, int delta)
	{
		if (delta != 0 && scrollbar.Visible)
		{
			SetScrollValue(scrollbar, scrollbar.Value + delta);
		}
	}

	private void CalculateScrollBars()
	{
		Rectangle clientRectangle = base.ClientRectangle;
		int num = clientRectangle.Height;
		int num2 = clientRectangle.Width;
		if (!scrollable)
		{
			h_scroll.Visible = false;
			v_scroll.Visible = false;
			item_control.Size = new Size(num2, num);
			header_control.Width = num2;
		}
		else
		{
			if (clientRectangle.Height < 0 || clientRectangle.Width < 0)
			{
				return;
			}
			if (layout_wd > clientRectangle.Right)
			{
				h_scroll.Visible = true;
				if (layout_ht + h_scroll.Height > clientRectangle.Bottom)
				{
					v_scroll.Visible = true;
				}
				else
				{
					v_scroll.Visible = false;
				}
			}
			else if (layout_ht > clientRectangle.Bottom)
			{
				v_scroll.Visible = true;
				if (layout_wd + v_scroll.Width > clientRectangle.Right)
				{
					h_scroll.Visible = true;
				}
				else
				{
					h_scroll.Visible = false;
				}
			}
			else
			{
				h_scroll.Visible = false;
				v_scroll.Visible = false;
			}
			if (h_scroll.is_visible)
			{
				h_scroll.Location = new Point(clientRectangle.X, clientRectangle.Bottom - h_scroll.Height);
				h_scroll.Minimum = 0;
				if (v_scroll.Visible)
				{
					h_scroll.Maximum = layout_wd + v_scroll.Width;
					h_scroll.Width = clientRectangle.Width - v_scroll.Width;
				}
				else
				{
					h_scroll.Maximum = layout_wd;
					h_scroll.Width = clientRectangle.Width;
				}
				h_scroll.LargeChange = clientRectangle.Width;
				h_scroll.SmallChange = item_size.Width + ThemeEngine.Current.ListViewHorizontalSpacing;
				num -= h_scroll.Height;
			}
			if (v_scroll.is_visible)
			{
				v_scroll.Location = new Point(clientRectangle.Right - v_scroll.Width, clientRectangle.Y);
				v_scroll.Minimum = 0;
				v_scroll.Maximum = layout_ht;
				if (h_scroll.Visible)
				{
					v_scroll.Height = clientRectangle.Height - h_scroll.Height;
				}
				else
				{
					v_scroll.Height = clientRectangle.Height;
				}
				v_scroll.LargeChange = clientRectangle.Height;
				v_scroll.SmallChange = Font.Height;
				num2 -= v_scroll.Width;
			}
			item_control.Size = new Size(num2, num);
			if (header_control.is_visible)
			{
				header_control.Width = num2;
			}
		}
	}

	internal int GetReorderedColumnIndex(ColumnHeader column)
	{
		if (reordered_column_indices == null)
		{
			return column.Index;
		}
		for (int i = 0; i < Columns.Count; i++)
		{
			if (reordered_column_indices[i] == column.Index)
			{
				return i;
			}
		}
		return -1;
	}

	internal ColumnHeader GetReorderedColumn(int index)
	{
		if (reordered_column_indices == null)
		{
			return Columns[index];
		}
		return Columns[reordered_column_indices[index]];
	}

	internal void ReorderColumn(ColumnHeader col, int index, bool fireEvent)
	{
		if (fireEvent)
		{
			ColumnReorderedEventHandler columnReorderedEventHandler = (ColumnReorderedEventHandler)base.Events[ColumnReordered];
			if (columnReorderedEventHandler != null)
			{
				ColumnReorderedEventArgs columnReorderedEventArgs = new ColumnReorderedEventArgs(col.Index, index, col);
				columnReorderedEventHandler(this, columnReorderedEventArgs);
				if (columnReorderedEventArgs.Cancel)
				{
					header_control.Invalidate();
					item_control.Invalidate();
					return;
				}
			}
		}
		int count = Columns.Count;
		if (reordered_column_indices == null)
		{
			reordered_column_indices = new int[count];
			for (int i = 0; i < count; i++)
			{
				reordered_column_indices[i] = i;
			}
		}
		if (reordered_column_indices[index] == col.Index)
		{
			return;
		}
		int[] array = reordered_column_indices;
		int[] array2 = new int[count];
		int num = 0;
		for (int j = 0; j < count; j++)
		{
			if (num < count && array[num] == col.Index)
			{
				num++;
			}
			if (j == index)
			{
				array2[j] = col.Index;
			}
			else
			{
				array2[j] = array[num++];
			}
		}
		ReorderColumns(array2, redraw: true);
	}

	internal void ReorderColumns(int[] display_indices, bool redraw)
	{
		reordered_column_indices = display_indices;
		for (int i = 0; i < Columns.Count; i++)
		{
			ColumnHeader columnHeader = Columns[i];
			columnHeader.InternalDisplayIndex = reordered_column_indices[i];
		}
		if (redraw && view == View.Details && base.IsHandleCreated)
		{
			LayoutDetails();
			header_control.Invalidate();
			item_control.Invalidate();
		}
	}

	internal void AddColumn(ColumnHeader newCol, int index, bool redraw)
	{
		int count = Columns.Count;
		newCol.SetListView(this);
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			ColumnHeader columnHeader = Columns[i];
			if (i == index)
			{
				array[i] = index;
				continue;
			}
			int internalDisplayIndex = columnHeader.InternalDisplayIndex;
			if (internalDisplayIndex < index)
			{
				array[i] = internalDisplayIndex;
			}
			else
			{
				array[i] = internalDisplayIndex + 1;
			}
		}
		ReorderColumns(array, redraw);
		Invalidate();
	}

	private int GetDetailsItemHeight()
	{
		int val = (CheckBoxes ? CheckBoxSize.Height : 0);
		int val2 = ((SmallImageList != null) ? SmallImageList.ImageSize.Height : 0);
		int val3 = Math.Max(val, text_size.Height);
		return Math.Max(val3, val2);
	}

	private void SetItemLocation(int index, int x, int y, int row, int col)
	{
		Point point = items_location[index];
		if (point.X != x || point.Y != y)
		{
			ref Point reference = ref items_location[index];
			reference = new Point(x, y);
			ref ItemMatrixLocation reference2 = ref items_matrix_location[index];
			reference2 = new ItemMatrixLocation(row, col);
			reordered_items_indices[index] = index;
		}
	}

	private void ShiftItemsPositions(int from, int to, bool forward)
	{
		if (forward)
		{
			for (int num = to + 1; num > from; num--)
			{
				reordered_items_indices[num] = reordered_items_indices[num - 1];
				ListViewItem listViewItem = items[reordered_items_indices[num]];
				listViewItem.Invalidate();
				listViewItem.DisplayIndex = num;
				listViewItem.Invalidate();
			}
		}
		else
		{
			for (int i = from - 1; i < to; i++)
			{
				reordered_items_indices[i] = reordered_items_indices[i + 1];
				ListViewItem listViewItem2 = items[reordered_items_indices[i]];
				listViewItem2.Invalidate();
				listViewItem2.DisplayIndex = i;
				listViewItem2.Invalidate();
			}
		}
	}

	internal void ChangeItemLocation(int display_index, Point new_pos)
	{
		int displayIndexFromLocation = GetDisplayIndexFromLocation(new_pos);
		if (displayIndexFromLocation != display_index)
		{
			int num = reordered_items_indices[display_index];
			ListViewItem listViewItem = items[num];
			bool flag = displayIndexFromLocation < display_index;
			int from;
			int to;
			if (flag)
			{
				from = displayIndexFromLocation;
				to = display_index - 1;
			}
			else
			{
				from = display_index + 1;
				to = displayIndexFromLocation;
			}
			ShiftItemsPositions(from, to, flag);
			reordered_items_indices[displayIndexFromLocation] = num;
			listViewItem.Invalidate();
			listViewItem.DisplayIndex = displayIndexFromLocation;
			listViewItem.Invalidate();
		}
	}

	private int GetDisplayIndexFromLocation(Point loc)
	{
		int num = -1;
		if (loc.X < 0 || loc.Y < 0)
		{
			return 0;
		}
		loc.X -= item_size.Width / 2;
		if (loc.X < 0)
		{
			loc.X = 0;
		}
		for (int i = 0; i < items.Count; i++)
		{
			Rectangle rectangle = new Rectangle(GetItemLocation(i), item_size);
			rectangle.Inflate(ThemeEngine.Current.ListViewHorizontalSpacing, ThemeEngine.Current.ListViewVerticalSpacing);
			if (rectangle.Contains(loc))
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			num = items.Count - 1;
		}
		return num;
	}

	private int GetDefaultGroupItems()
	{
		int num = 0;
		foreach (ListViewItem item in items)
		{
			if (item.Group == null)
			{
				num++;
			}
		}
		return num;
	}

	private void CalculateRowsAndCols(Size item_size, bool left_aligned, int x_spacing, int y_spacing)
	{
		Rectangle clientRectangle = base.ClientRectangle;
		if (UseCustomColumnWidth)
		{
			CalculateCustomColumnWidth();
		}
		if (UsingGroups)
		{
			rows = 0;
			cols = 0;
			int num = 0;
			groups.DefaultGroup.ItemCount = GetDefaultGroupItems();
			for (int i = 0; i < groups.InternalCount; i++)
			{
				ListViewGroup internalGroup = groups.GetInternalGroup(i);
				int actualItemCount = internalGroup.GetActualItemCount();
				if (actualItemCount != 0)
				{
					int num2 = (int)Math.Floor((double)(clientRectangle.Width - v_scroll.Width + x_spacing) / (double)(item_size.Width + x_spacing));
					if (num2 <= 0)
					{
						num2 = 1;
					}
					int num3 = (int)Math.Ceiling((double)actualItemCount / (double)num2);
					internalGroup.starting_row = rows;
					internalGroup.rows = num3;
					internalGroup.starting_item = num;
					internalGroup.current_item = 0;
					cols = Math.Max(num2, cols);
					rows += num3;
					num += actualItemCount;
				}
			}
		}
		else if (left_aligned)
		{
			rows = (int)Math.Floor((double)(clientRectangle.Height - h_scroll.Height + y_spacing) / (double)(item_size.Height + y_spacing));
			if (rows <= 0)
			{
				rows = 1;
			}
			cols = (int)Math.Ceiling((double)items.Count / (double)rows);
		}
		else
		{
			if (UseCustomColumnWidth)
			{
				cols = (int)Math.Floor((double)(clientRectangle.Width - v_scroll.Width) / (double)custom_column_width);
			}
			else
			{
				cols = (int)Math.Floor((double)(clientRectangle.Width - v_scroll.Width + x_spacing) / (double)(item_size.Width + x_spacing));
			}
			if (cols < 1)
			{
				cols = 1;
			}
			rows = (int)Math.Ceiling((double)items.Count / (double)cols);
		}
		item_index_matrix = new int[rows, cols];
	}

	private void CalculateCustomColumnWidth()
	{
		int num = int.MaxValue;
		for (int i = 0; i < columns.Count; i++)
		{
			int width = columns[i].Width;
			if (width < num)
			{
				num = width;
			}
		}
		custom_column_width = num;
	}

	private void LayoutIcons(Size item_size, bool left_aligned, int x_spacing, int y_spacing)
	{
		header_control.Visible = false;
		header_control.Size = Size.Empty;
		item_control.Visible = true;
		item_control.Location = Point.Empty;
		ItemSize = item_size;
		this.x_spacing = x_spacing;
		this.y_spacing = y_spacing;
		if (items.Count == 0)
		{
			return;
		}
		Size size = item_size;
		CalculateRowsAndCols(size, left_aligned, x_spacing, y_spacing);
		layout_wd = ((!UseCustomColumnWidth) ? (cols * (size.Width + x_spacing) - x_spacing) : (cols * custom_column_width));
		layout_ht = rows * (size.Height + y_spacing) - y_spacing;
		if (virtual_mode)
		{
			item_control.Size = new Size(layout_wd, layout_ht);
			return;
		}
		bool usingGroups = UsingGroups;
		if (usingGroups)
		{
			CalculateGroupsLayout(size, y_spacing, 0);
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < items.Count; i++)
		{
			ListViewItem listViewItem = items[i];
			if (usingGroups)
			{
				ListViewGroup listViewGroup = listViewItem.Group;
				if (listViewGroup == null)
				{
					listViewGroup = groups.DefaultGroup;
				}
				Point items_area_location = listViewGroup.items_area_location;
				int num6 = listViewGroup.current_item++;
				int starting_row = listViewGroup.starting_row;
				num5 = listViewGroup.starting_item + num6;
				num = num6 / cols;
				num2 = num6 % cols;
				num3 = ((!UseCustomColumnWidth) ? (num2 * (item_size.Width + x_spacing)) : (num2 * custom_column_width));
				num4 = num * (item_size.Height + y_spacing) + items_area_location.Y;
				SetItemLocation(num5, num3, num4, num + starting_row, num2);
				SetItemAtDisplayIndex(num5, i);
				item_index_matrix[num + starting_row, num2] = i;
			}
			else
			{
				num3 = ((!UseCustomColumnWidth) ? (num2 * (item_size.Width + x_spacing)) : (num2 * custom_column_width));
				num4 = num * (item_size.Height + y_spacing);
				num5 = i;
				SetItemLocation(i, num3, num4, num, num2);
				item_index_matrix[num, num2] = i;
				if (left_aligned)
				{
					num++;
					if (num == rows)
					{
						num = 0;
						num2++;
					}
				}
				else if (++num2 == cols)
				{
					num2 = 0;
					num++;
				}
			}
			listViewItem.Layout();
			listViewItem.DisplayIndex = num5;
			listViewItem.SetPosition(new Point(num3, num4));
		}
		item_control.Size = new Size(layout_wd, layout_ht);
	}

	private void CalculateGroupsLayout(Size item_size, int y_spacing, int y_origin)
	{
		int num = y_origin;
		bool flag = view == View.Details;
		for (int i = 0; i < groups.InternalCount; i++)
		{
			ListViewGroup internalGroup = groups.GetInternalGroup(i);
			if (internalGroup.ItemCount != 0)
			{
				num += LayoutGroupHeader(internalGroup, num, item_size.Height, y_spacing, (!flag) ? internalGroup.rows : internalGroup.ItemCount);
			}
		}
		layout_ht = num;
	}

	private int LayoutGroupHeader(ListViewGroup group, int y_origin, int item_height, int y_spacing, int rows)
	{
		Rectangle clientRectangle = base.ClientRectangle;
		int num = Font.Height + 15;
		group.HeaderBounds = new Rectangle(0, y_origin, clientRectangle.Width - v_scroll.Width, num);
		group.items_area_location = new Point(0, y_origin + num);
		int num2 = (item_height + y_spacing) * rows;
		return num + num2 + 10;
	}

	private void CalculateDetailsGroupItemsCount()
	{
		int num = 0;
		groups.DefaultGroup.ItemCount = GetDefaultGroupItems();
		for (int i = 0; i < groups.InternalCount; i++)
		{
			ListViewGroup internalGroup = groups.GetInternalGroup(i);
			int actualItemCount = internalGroup.GetActualItemCount();
			if (actualItemCount != 0)
			{
				internalGroup.starting_item = num;
				internalGroup.current_item = 0;
				num += actualItemCount;
			}
		}
	}

	private void LayoutHeader()
	{
		int num = 0;
		for (int i = 0; i < Columns.Count; i++)
		{
			ColumnHeader reorderedColumn = GetReorderedColumn(i);
			reorderedColumn.X = num;
			reorderedColumn.Y = 0;
			reorderedColumn.CalcColumnHeader();
			num += reorderedColumn.Wd;
		}
		layout_wd = num;
		if (num < base.ClientRectangle.Width)
		{
			num = base.ClientRectangle.Width;
		}
		if (header_style == ColumnHeaderStyle.None)
		{
			header_control.Visible = false;
			header_control.Size = Size.Empty;
			layout_wd = base.ClientRectangle.Width;
		}
		else
		{
			header_control.Width = num;
			header_control.Height = ((columns.Count <= 0) ? ThemeEngine.Current.ListViewGetHeaderHeight(this, Font) : columns[0].Ht);
			header_control.Visible = true;
		}
	}

	private void LayoutDetails()
	{
		LayoutHeader();
		if (columns.Count == 0)
		{
			item_control.Visible = false;
			layout_wd = base.ClientRectangle.Width;
			layout_ht = base.ClientRectangle.Height;
			return;
		}
		item_control.Visible = true;
		item_control.Location = Point.Empty;
		item_control.Width = base.ClientRectangle.Width;
		int detailsItemHeight = GetDetailsItemHeight();
		ItemSize = new Size(0, detailsItemHeight);
		int num = header_control.Height;
		layout_ht = num + detailsItemHeight * items.Count;
		if (items.Count > 0 && grid_lines)
		{
			layout_ht += 2;
		}
		bool usingGroups = UsingGroups;
		if (usingGroups)
		{
			CalculateDetailsGroupItemsCount();
			CalculateGroupsLayout(ItemSize, 2, num);
		}
		if (virtual_mode)
		{
			return;
		}
		for (int i = 0; i < items.Count; i++)
		{
			ListViewItem listViewItem = items[i];
			int num3;
			int y;
			if (usingGroups)
			{
				ListViewGroup listViewGroup = listViewItem.Group;
				if (listViewGroup == null)
				{
					listViewGroup = groups.DefaultGroup;
				}
				int num2 = listViewGroup.current_item++;
				Point items_area_location = listViewGroup.items_area_location;
				num3 = listViewGroup.starting_item + num2;
				num = (y = num2 * (detailsItemHeight + 2) + items_area_location.Y);
				SetItemLocation(num3, 0, y, 0, 0);
				SetItemAtDisplayIndex(num3, i);
			}
			else
			{
				num3 = i;
				y = num;
				SetItemLocation(i, 0, y, 0, 0);
				num += detailsItemHeight;
			}
			listViewItem.Layout();
			listViewItem.DisplayIndex = num3;
			listViewItem.SetPosition(new Point(0, y));
		}
	}

	private void AdjustItemsPositionArray(int count)
	{
		if (!virtual_mode && items_location.Length < count)
		{
			count = Math.Max(count, items_location.Length * 2);
			items_location = new Point[count];
			items_matrix_location = new ItemMatrixLocation[count];
			reordered_items_indices = new int[count];
		}
	}

	private void CalculateListView(ListViewAlignment align)
	{
		CalcTextSize();
		AdjustItemsPositionArray(items.Count);
		switch (view)
		{
		case View.Details:
			LayoutDetails();
			break;
		case View.SmallIcon:
			LayoutIcons(SmallIconItemSize, alignment == ListViewAlignment.Left, ThemeEngine.Current.ListViewHorizontalSpacing, 2);
			break;
		case View.LargeIcon:
			LayoutIcons(LargeIconItemSize, alignment == ListViewAlignment.Left, ThemeEngine.Current.ListViewHorizontalSpacing, ThemeEngine.Current.ListViewVerticalSpacing);
			break;
		case View.List:
			LayoutIcons(SmallIconItemSize, left_aligned: true, ThemeEngine.Current.ListViewHorizontalSpacing, 2);
			break;
		case View.Tile:
			if (!Application.VisualStylesEnabled)
			{
				goto case View.LargeIcon;
			}
			LayoutIcons(TileItemSize, alignment == ListViewAlignment.Left, ThemeEngine.Current.ListViewHorizontalSpacing, ThemeEngine.Current.ListViewVerticalSpacing);
			break;
		}
		CalculateScrollBars();
	}

	internal Point GetItemLocation(int index)
	{
		Point empty = Point.Empty;
		empty = ((!virtual_mode) ? items_location[index] : GetFixedItemLocation(index));
		empty.X -= h_marker;
		empty.Y -= v_marker;
		return empty;
	}

	private Point GetFixedItemLocation(int index)
	{
		Point empty = Point.Empty;
		switch (view)
		{
		case View.LargeIcon:
		case View.SmallIcon:
			empty.X = index % cols * (item_size.Width + x_spacing);
			empty.Y = index / cols * (item_size.Height + y_spacing);
			break;
		case View.List:
			empty.X = index / rows * (item_size.Width + x_spacing);
			empty.Y = index % rows * (item_size.Height + y_spacing);
			break;
		case View.Details:
			empty.Y = header_control.Height + index * item_size.Height;
			break;
		}
		return empty;
	}

	internal int GetItemIndex(int display_index)
	{
		if (virtual_mode)
		{
			return display_index;
		}
		return reordered_items_indices[display_index];
	}

	internal ListViewItem GetItemAtDisplayIndex(int display_index)
	{
		if (virtual_mode)
		{
			return items[display_index];
		}
		return items[reordered_items_indices[display_index]];
	}

	internal void SetItemAtDisplayIndex(int display_index, int index)
	{
		reordered_items_indices[display_index] = index;
	}

	private bool KeySearchString(KeyEventArgs ke)
	{
		int tickCount = Environment.TickCount;
		if (keysearch_tickcnt > 0 && tickCount - keysearch_tickcnt > keysearch_keydelay)
		{
			keysearch_text = string.Empty;
		}
		if (!char.IsLetterOrDigit((char)ke.KeyCode))
		{
			return false;
		}
		keysearch_text += (char)ke.KeyCode;
		keysearch_tickcnt = tickCount;
		int num = ((FocusedItem != null) ? FocusedItem.DisplayIndex : 0);
		int startIndex = ((num + 1 < Items.Count) ? (num + 1) : 0);
		ListViewItem listViewItem = FindItemWithText(keysearch_text, includeSubItemsInSearch: false, startIndex, isPrefixSearch: true, roundtrip: true);
		if (listViewItem != null && num != listViewItem.DisplayIndex)
		{
			selected_indices.Clear();
			SetFocusedItem(listViewItem.DisplayIndex);
			listViewItem.Selected = true;
			EnsureVisible(GetItemIndex(listViewItem.DisplayIndex));
		}
		return true;
	}

	private void OnItemsChanged()
	{
		ResetSearchString();
	}

	private void ResetSearchString()
	{
		keysearch_text = string.Empty;
	}

	private int GetAdjustedIndex(Keys key)
	{
		int num = -1;
		if (View == View.Details)
		{
			switch (key)
			{
			case Keys.Up:
				num = FocusedItem.DisplayIndex - 1;
				break;
			case Keys.Down:
				num = FocusedItem.DisplayIndex + 1;
				if (num == items.Count)
				{
					num = -1;
				}
				break;
			case Keys.PageDown:
			{
				int num4 = LastVisibleIndex;
				if (new Rectangle(GetItemLocation(num4), ItemSize).Bottom > item_control.ClientRectangle.Bottom)
				{
					num4--;
				}
				if (FocusedItem.DisplayIndex == num4)
				{
					if (FocusedItem.DisplayIndex < Items.Count - 1)
					{
						int num5 = item_control.Height / ItemSize.Height - 1;
						num = FocusedItem.DisplayIndex + num5 - 1;
						if (num >= Items.Count)
						{
							num = Items.Count - 1;
						}
					}
				}
				else
				{
					num = num4;
				}
				break;
			}
			case Keys.PageUp:
			{
				int num2 = FirstVisibleIndex;
				if (GetItemLocation(num2).Y < 0)
				{
					num2++;
				}
				if (FocusedItem.DisplayIndex == num2)
				{
					if (num2 > 0)
					{
						int num3 = item_control.Height / ItemSize.Height - 1;
						num = num2 - num3 + 1;
						if (num < 0)
						{
							num = 0;
						}
					}
				}
				else
				{
					num = num2;
				}
				break;
			}
			}
			return num;
		}
		if (virtual_mode)
		{
			return GetFixedAdjustedIndex(key);
		}
		ItemMatrixLocation itemMatrixLocation = items_matrix_location[FocusedItem.DisplayIndex];
		int num6 = itemMatrixLocation.Row;
		int num7 = itemMatrixLocation.Col;
		int num8 = -1;
		switch (key)
		{
		case Keys.Left:
			if (num7 == 0)
			{
				return -1;
			}
			num8 = item_index_matrix[num6, num7 - 1];
			break;
		case Keys.Right:
			if (num7 == cols - 1)
			{
				return -1;
			}
			while (item_index_matrix[num6, num7 + 1] == 0)
			{
				num6--;
				if (num6 < 0)
				{
					return -1;
				}
			}
			num8 = item_index_matrix[num6, num7 + 1];
			break;
		case Keys.Up:
			if (num6 == 0)
			{
				return -1;
			}
			while (item_index_matrix[num6 - 1, num7] == 0 && num6 != 1)
			{
				num7--;
				if (num7 < 0)
				{
					return -1;
				}
			}
			num8 = item_index_matrix[num6 - 1, num7];
			break;
		case Keys.Down:
			if (num6 == rows - 1 || num6 == Items.Count - 1)
			{
				return -1;
			}
			while (item_index_matrix[num6 + 1, num7] == 0)
			{
				num7--;
				if (num7 < 0)
				{
					return -1;
				}
			}
			num8 = item_index_matrix[num6 + 1, num7];
			break;
		default:
			return -1;
		}
		return items[num8].DisplayIndex;
	}

	private int GetFixedAdjustedIndex(Keys key)
	{
		int num;
		switch (key)
		{
		case Keys.Left:
			num = ((view != View.List) ? (focused_item_index - 1) : (focused_item_index - rows));
			break;
		case Keys.Right:
			num = ((view != View.List) ? (focused_item_index + 1) : (focused_item_index + rows));
			break;
		case Keys.Up:
			num = ((view == View.List) ? (focused_item_index - 1) : (focused_item_index - cols));
			break;
		case Keys.Down:
			num = ((view == View.List) ? (focused_item_index + 1) : (focused_item_index + cols));
			break;
		default:
			return -1;
		}
		if (num < 0 || num >= items.Count)
		{
			num = focused_item_index;
		}
		return num;
	}

	private bool SelectItems(ArrayList sel_items)
	{
		bool result = false;
		foreach (ListViewItem selectedItem in SelectedItems)
		{
			if (!sel_items.Contains(selectedItem))
			{
				selectedItem.Selected = false;
				result = true;
			}
		}
		foreach (ListViewItem sel_item in sel_items)
		{
			if (!sel_item.Selected)
			{
				sel_item.Selected = true;
				result = true;
			}
		}
		return result;
	}

	private void UpdateMultiSelection(int index, bool reselect)
	{
		bool flag = (XplatUI.State.ModifierKeys & Keys.Shift) != 0;
		bool flag2 = (XplatUI.State.ModifierKeys & Keys.Control) != 0;
		ListViewItem itemAtDisplayIndex = GetItemAtDisplayIndex(index);
		if (flag && selection_start != null)
		{
			ArrayList arrayList = new ArrayList();
			int displayIndex = selection_start.DisplayIndex;
			int num = Math.Min(displayIndex, index);
			int num2 = Math.Max(displayIndex, index);
			if (View == View.Details)
			{
				for (int i = num; i <= num2; i++)
				{
					arrayList.Add(GetItemAtDisplayIndex(i));
				}
			}
			else
			{
				ItemMatrixLocation itemMatrixLocation = items_matrix_location[num];
				ItemMatrixLocation itemMatrixLocation2 = items_matrix_location[num2];
				int num3 = Math.Min(itemMatrixLocation.Col, itemMatrixLocation2.Col);
				int num4 = Math.Max(itemMatrixLocation.Col, itemMatrixLocation2.Col);
				int num5 = Math.Min(itemMatrixLocation.Row, itemMatrixLocation2.Row);
				int num6 = Math.Max(itemMatrixLocation.Row, itemMatrixLocation2.Row);
				for (int j = 0; j < items.Count; j++)
				{
					ItemMatrixLocation itemMatrixLocation3 = items_matrix_location[j];
					if (itemMatrixLocation3.Row >= num5 && itemMatrixLocation3.Row <= num6 && itemMatrixLocation3.Col >= num3 && itemMatrixLocation3.Col <= num4)
					{
						arrayList.Add(GetItemAtDisplayIndex(j));
					}
				}
			}
			SelectItems(arrayList);
			return;
		}
		if (flag2)
		{
			itemAtDisplayIndex.Selected = !itemAtDisplayIndex.Selected;
			selection_start = itemAtDisplayIndex;
			return;
		}
		if (!reselect)
		{
			foreach (int selectedIndex in SelectedIndices)
			{
				if (index != selectedIndex)
				{
					items[selectedIndex].Selected = false;
				}
			}
		}
		else
		{
			SelectedItems.Clear();
			itemAtDisplayIndex.Selected = true;
		}
		selection_start = itemAtDisplayIndex;
	}

	internal override bool InternalPreProcessMessage(ref Message msg)
	{
		if (msg.Msg == 256)
		{
			Keys key_data = (Keys)msg.WParam.ToInt32();
			HandleNavKeys(key_data);
		}
		return base.InternalPreProcessMessage(ref msg);
	}

	private bool HandleNavKeys(Keys key_data)
	{
		if (Items.Count == 0 || !item_control.Visible)
		{
			return false;
		}
		if (FocusedItem == null)
		{
			SetFocusedItem(0);
		}
		switch (key_data)
		{
		case Keys.End:
			SelectIndex(Items.Count - 1);
			break;
		case Keys.Home:
			SelectIndex(0);
			break;
		case Keys.PageUp:
		case Keys.PageDown:
		case Keys.Left:
		case Keys.Up:
		case Keys.Right:
		case Keys.Down:
			SelectIndex(GetAdjustedIndex(key_data));
			break;
		case Keys.Space:
			SelectIndex(focused_item_index);
			ToggleItemsCheckState();
			break;
		case Keys.Return:
			if (selected_indices.Count > 0)
			{
				OnItemActivate(EventArgs.Empty);
			}
			break;
		default:
			return false;
		}
		return true;
	}

	private void ToggleItemsCheckState()
	{
		if (!CheckBoxes || (StateImageList != null && StateImageList.Images.Count < 2))
		{
			return;
		}
		if (SelectedIndices.Count > 0)
		{
			for (int i = 0; i < SelectedIndices.Count; i++)
			{
				ListViewItem listViewItem = Items[SelectedIndices[i]];
				listViewItem.Checked = !listViewItem.Checked;
			}
		}
		else if (FocusedItem != null)
		{
			FocusedItem.Checked = !FocusedItem.Checked;
			SelectIndex(FocusedItem.Index);
		}
	}

	private void SelectIndex(int display_index)
	{
		if (display_index != -1)
		{
			if (MultiSelect)
			{
				UpdateMultiSelection(display_index, reselect: true);
			}
			else if (!GetItemAtDisplayIndex(display_index).Selected)
			{
				GetItemAtDisplayIndex(display_index).Selected = true;
			}
			SetFocusedItem(display_index);
			EnsureVisible(GetItemIndex(display_index));
		}
	}

	private void ListView_KeyDown(object sender, KeyEventArgs ke)
	{
		if (!ke.Handled && Items.Count != 0 && item_control.Visible && !ke.Alt && !ke.Control)
		{
			ke.Handled = KeySearchString(ke);
		}
	}

	private MouseEventArgs TranslateMouseEventArgs(MouseEventArgs args)
	{
		Point point = PointToClient(Control.MousePosition);
		return new MouseEventArgs(args.Button, args.Clicks, point.X, point.Y, args.Delta);
	}

	internal override void OnPaintInternal(PaintEventArgs pe)
	{
		if (!updating)
		{
			CalculateScrollBars();
		}
	}

	private void FocusChanged(object o, EventArgs args)
	{
		if (Items.Count != 0)
		{
			if (FocusedItem == null)
			{
				SetFocusedItem(0);
			}
			ListViewItem focusedItem = FocusedItem;
			if (focusedItem.ListView != null)
			{
				focusedItem.Invalidate();
				focusedItem.Layout();
				focusedItem.Invalidate();
			}
		}
	}

	private void ListView_Invalidated(object sender, InvalidateEventArgs e)
	{
		header_control.Invalidate();
		item_control.Invalidate();
	}

	private void ListView_MouseEnter(object sender, EventArgs args)
	{
		hover_pending = true;
	}

	private void ListView_MouseWheel(object sender, MouseEventArgs me)
	{
		if (Items.Count == 0)
		{
			return;
		}
		int num = me.Delta / 120;
		if (num == 0)
		{
			return;
		}
		switch (View)
		{
		case View.Details:
		case View.SmallIcon:
			Scroll(v_scroll, -ItemSize.Height * SystemInformation.MouseWheelScrollLines * num);
			break;
		case View.LargeIcon:
			Scroll(v_scroll, -(ItemSize.Height + ThemeEngine.Current.ListViewVerticalSpacing) * num);
			break;
		case View.List:
			Scroll(h_scroll, -ItemSize.Width * num);
			break;
		case View.Tile:
			if (!Application.VisualStylesEnabled)
			{
				goto case View.LargeIcon;
			}
			Scroll(v_scroll, -(ItemSize.Height + ThemeEngine.Current.ListViewVerticalSpacing) * 2 * num);
			break;
		}
	}

	private void ListView_SizeChanged(object sender, EventArgs e)
	{
		Redraw(recalculate: true);
	}

	private void SetFocusedItem(int display_index)
	{
		if (display_index != -1)
		{
			GetItemAtDisplayIndex(display_index).Focused = true;
		}
		else if (focused_item_index != -1 && focused_item_index < items.Count)
		{
			GetItemAtDisplayIndex(focused_item_index).Focused = false;
		}
		focused_item_index = display_index;
		if (display_index == -1)
		{
			OnUIAFocusedItemChanged();
		}
	}

	private void HorizontalScroller(object sender, EventArgs e)
	{
		item_control.EndEdit(item_control.edit_item);
		if (h_marker != h_scroll.Value)
		{
			int xAmount = h_marker - h_scroll.Value;
			h_marker = h_scroll.Value;
			if (header_control.Visible)
			{
				XplatUI.ScrollWindow(header_control.Handle, xAmount, 0, with_children: false);
			}
			XplatUI.ScrollWindow(item_control.Handle, xAmount, 0, with_children: false);
		}
	}

	private void VerticalScroller(object sender, EventArgs e)
	{
		item_control.EndEdit(item_control.edit_item);
		if (v_marker != v_scroll.Value)
		{
			int yAmount = v_marker - v_scroll.Value;
			Rectangle clientRectangle = item_control.ClientRectangle;
			if (header_control.Visible)
			{
				clientRectangle.Y += header_control.Height;
				clientRectangle.Height -= header_control.Height;
			}
			v_marker = v_scroll.Value;
			XplatUI.ScrollWindow(item_control.Handle, clientRectangle, 0, yAmount, with_children: false);
		}
	}

	internal override bool IsInputCharInternal(char charCode)
	{
		return true;
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
		for (int i = 0; i < SelectedItems.Count; i++)
		{
			OnSelectedIndexChanged(EventArgs.Empty);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			h_scroll.Dispose();
			v_scroll.Dispose();
			large_image_list = null;
			small_image_list = null;
			state_image_list = null;
			foreach (ColumnHeader column in columns)
			{
				column.SetListView(null);
			}
			if (!virtual_mode)
			{
				foreach (ListViewItem item in items)
				{
					item.Owner = null;
				}
			}
		}
		base.Dispose(disposing);
	}

	protected override bool IsInputKey(Keys keyData)
	{
		switch (keyData)
		{
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
			return base.IsInputKey(keyData);
		}
	}

	protected virtual void OnAfterLabelEdit(LabelEditEventArgs e)
	{
		((LabelEditEventHandler)base.Events[AfterLabelEdit])?.Invoke(this, e);
	}

	protected override void OnBackgroundImageChanged(EventArgs e)
	{
		item_control.BackgroundImage = BackgroundImage;
		base.OnBackgroundImageChanged(e);
	}

	protected virtual void OnBeforeLabelEdit(LabelEditEventArgs e)
	{
		((LabelEditEventHandler)base.Events[BeforeLabelEdit])?.Invoke(this, e);
	}

	protected internal virtual void OnColumnClick(ColumnClickEventArgs e)
	{
		((ColumnClickEventHandler)base.Events[ColumnClick])?.Invoke(this, e);
	}

	protected internal virtual void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
	{
		((DrawListViewColumnHeaderEventHandler)base.Events[DrawColumnHeader])?.Invoke(this, e);
	}

	protected internal virtual void OnDrawItem(DrawListViewItemEventArgs e)
	{
		((DrawListViewItemEventHandler)base.Events[DrawItem])?.Invoke(this, e);
	}

	protected internal virtual void OnDrawSubItem(DrawListViewSubItemEventArgs e)
	{
		((DrawListViewSubItemEventHandler)base.Events[DrawSubItem])?.Invoke(this, e);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		Redraw(recalculate: true);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		CalculateListView(alignment);
		if (!virtual_mode)
		{
			Sort();
		}
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected virtual void OnItemActivate(EventArgs e)
	{
		((EventHandler)base.Events[ItemActivate])?.Invoke(this, e);
	}

	protected internal virtual void OnItemCheck(ItemCheckEventArgs ice)
	{
		((ItemCheckEventHandler)base.Events[ItemCheck])?.Invoke(this, ice);
	}

	protected internal virtual void OnItemChecked(ItemCheckedEventArgs e)
	{
		((ItemCheckedEventHandler)base.Events[ItemChecked])?.Invoke(this, e);
	}

	protected virtual void OnItemDrag(ItemDragEventArgs e)
	{
		((ItemDragEventHandler)base.Events[ItemDrag])?.Invoke(this, e);
	}

	protected virtual void OnItemMouseHover(ListViewItemMouseHoverEventArgs e)
	{
		((ListViewItemMouseHoverEventHandler)base.Events[ItemMouseHover])?.Invoke(this, e);
	}

	protected internal virtual void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
	{
		((ListViewItemSelectionChangedEventHandler)base.Events[ItemSelectionChanged])?.Invoke(this, e);
	}

	protected override void OnMouseHover(EventArgs e)
	{
		base.OnMouseHover(e);
	}

	protected override void OnParentChanged(EventArgs e)
	{
		base.OnParentChanged(e);
	}

	protected virtual void OnSelectedIndexChanged(EventArgs e)
	{
		((EventHandler)base.Events[SelectedIndexChanged])?.Invoke(this, e);
	}

	protected override void OnSystemColorsChanged(EventArgs e)
	{
		base.OnSystemColorsChanged(e);
	}

	protected internal virtual void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
	{
		((CacheVirtualItemsEventHandler)base.Events[CacheVirtualItems])?.Invoke(this, e);
	}

	protected virtual void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
	{
		((RetrieveVirtualItemEventHandler)base.Events[RetrieveVirtualItem])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
	{
		((EventHandler)base.Events[RightToLeftLayoutChanged])?.Invoke(this, e);
	}

	protected virtual void OnSearchForVirtualItem(SearchForVirtualItemEventArgs e)
	{
		((SearchForVirtualItemEventHandler)base.Events[SearchForVirtualItem])?.Invoke(this, e);
	}

	protected virtual void OnVirtualItemsSelectionRangeChanged(ListViewVirtualItemsSelectionRangeChangedEventArgs e)
	{
		((ListViewVirtualItemsSelectionRangeChangedEventHandler)base.Events[VirtualItemsSelectionRangeChanged])?.Invoke(this, e);
	}

	protected void RealizeProperties()
	{
	}

	protected void UpdateExtendedStyles()
	{
	}

	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_KILLFOCUS:
		{
			Control control = Control.FromHandle(m.WParam);
			if (control == item_control)
			{
				has_focus = false;
				refocusing = true;
				return;
			}
			break;
		}
		case Msg.WM_SETFOCUS:
			if (refocusing)
			{
				has_focus = true;
				refocusing = false;
				return;
			}
			break;
		}
		base.WndProc(ref m);
	}

	public void ArrangeIcons()
	{
		ArrangeIcons(alignment);
	}

	public void ArrangeIcons(ListViewAlignment value)
	{
		if (view == View.LargeIcon || view == View.SmallIcon)
		{
			Redraw(recalculate: true);
		}
	}

	public void AutoResizeColumn(int columnIndex, ColumnHeaderAutoResizeStyle headerAutoResize)
	{
		if (columnIndex < 0 || columnIndex >= columns.Count)
		{
			throw new ArgumentOutOfRangeException("columnIndex");
		}
		columns[columnIndex].AutoResize(headerAutoResize);
	}

	public void AutoResizeColumns(ColumnHeaderAutoResizeStyle headerAutoResize)
	{
		BeginUpdate();
		foreach (ColumnHeader column in columns)
		{
			column.AutoResize(headerAutoResize);
		}
		EndUpdate();
	}

	public void BeginUpdate()
	{
		updating = true;
	}

	public void Clear()
	{
		columns.Clear();
		items.Clear();
	}

	public void EndUpdate()
	{
		updating = false;
		Redraw(recalculate: true);
	}

	public void EnsureVisible(int index)
	{
		if (index < 0 || index >= items.Count || !scrollable || updating)
		{
			return;
		}
		Rectangle clientRectangle = item_control.ClientRectangle;
		Rectangle rect = ((!virtual_mode) ? items[index].Bounds : new Rectangle(GetItemLocation(index), ItemSize));
		if (view == View.Details && header_style != 0)
		{
			clientRectangle.Y += header_control.Height;
			clientRectangle.Height -= header_control.Height;
		}
		if (clientRectangle.Contains(rect))
		{
			return;
		}
		if (View != View.Details)
		{
			if (rect.Left < 0)
			{
				h_scroll.Value += rect.Left;
			}
			else if (rect.Right > clientRectangle.Right)
			{
				h_scroll.Value += rect.Right - clientRectangle.Right;
			}
		}
		if (rect.Top < clientRectangle.Y)
		{
			v_scroll.Value += rect.Top - clientRectangle.Y;
		}
		else if (rect.Bottom > clientRectangle.Bottom)
		{
			v_scroll.Value += rect.Bottom - clientRectangle.Bottom;
		}
	}

	public ListViewItem FindItemWithText(string text)
	{
		if (items.Count == 0)
		{
			return null;
		}
		return FindItemWithText(text, includeSubItemsInSearch: true, 0, isPrefixSearch: true);
	}

	public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex)
	{
		return FindItemWithText(text, includeSubItemsInSearch, startIndex, isPrefixSearch: true, roundtrip: false);
	}

	public ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch)
	{
		return FindItemWithText(text, includeSubItemsInSearch, startIndex, isPrefixSearch, roundtrip: false);
	}

	internal ListViewItem FindItemWithText(string text, bool includeSubItemsInSearch, int startIndex, bool isPrefixSearch, bool roundtrip)
	{
		if (startIndex < 0 || startIndex >= items.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		if (virtual_mode)
		{
			SearchForVirtualItemEventArgs searchForVirtualItemEventArgs = new SearchForVirtualItemEventArgs(isTextSearch: true, isPrefixSearch, includeSubItemsInSearch, text, Point.Empty, SearchDirectionHint.Down, startIndex);
			OnSearchForVirtualItem(searchForVirtualItemEventArgs);
			int index = searchForVirtualItemEventArgs.Index;
			if (index >= 0 && index < virtual_list_size)
			{
				return items[index];
			}
			return null;
		}
		int num = startIndex;
		do
		{
			ListViewItem listViewItem = items[num];
			if (isPrefixSearch)
			{
				if (CultureInfo.CurrentCulture.CompareInfo.IsPrefix(listViewItem.Text, text, CompareOptions.IgnoreCase))
				{
					return listViewItem;
				}
			}
			else if (string.Compare(listViewItem.Text, text, ignoreCase: true) == 0)
			{
				return listViewItem;
			}
			if (num + 1 >= items.Count)
			{
				if (!roundtrip)
				{
					break;
				}
				num = 0;
			}
			else
			{
				num++;
			}
		}
		while (num != startIndex);
		if (includeSubItemsInSearch)
		{
			for (num = startIndex; num < items.Count; num++)
			{
				ListViewItem listViewItem2 = items[num];
				foreach (ListViewItem.ListViewSubItem subItem in listViewItem2.SubItems)
				{
					if (isPrefixSearch)
					{
						if (CultureInfo.CurrentCulture.CompareInfo.IsPrefix(subItem.Text, text, CompareOptions.IgnoreCase))
						{
							return listViewItem2;
						}
					}
					else if (string.Compare(subItem.Text, text, ignoreCase: true) == 0)
					{
						return listViewItem2;
					}
				}
			}
		}
		return null;
	}

	public ListViewItem FindNearestItem(SearchDirectionHint searchDirection, int x, int y)
	{
		return FindNearestItem(searchDirection, new Point(x, y));
	}

	public ListViewItem FindNearestItem(SearchDirectionHint dir, Point point)
	{
		if (dir < SearchDirectionHint.Left || dir > SearchDirectionHint.Down)
		{
			throw new ArgumentOutOfRangeException("searchDirection");
		}
		if (view != 0 && view != View.SmallIcon)
		{
			throw new InvalidOperationException();
		}
		if (virtual_mode)
		{
			SearchForVirtualItemEventArgs searchForVirtualItemEventArgs = new SearchForVirtualItemEventArgs(isTextSearch: false, isPrefixSearch: false, includeSubItemsInSearch: false, string.Empty, point, dir, 0);
			OnSearchForVirtualItem(searchForVirtualItemEventArgs);
			int index = searchForVirtualItemEventArgs.Index;
			if (index >= 0 && index < virtual_list_size)
			{
				return items[index];
			}
			return null;
		}
		ListViewItem result = null;
		int num = int.MaxValue;
		switch (dir)
		{
		case SearchDirectionHint.Up:
			point.Y -= item_size.Height;
			break;
		case SearchDirectionHint.Down:
			point.Y += item_size.Height;
			break;
		case SearchDirectionHint.Left:
			point.X -= item_size.Width;
			break;
		case SearchDirectionHint.Right:
			point.X += item_size.Width;
			break;
		}
		for (int i = 0; i < items.Count; i++)
		{
			Point itemLocation = GetItemLocation(i);
			switch (dir)
			{
			case SearchDirectionHint.Up:
				if (point.Y < itemLocation.Y)
				{
					continue;
				}
				break;
			case SearchDirectionHint.Down:
				if (point.Y > itemLocation.Y)
				{
					continue;
				}
				break;
			case SearchDirectionHint.Left:
				if (point.X < itemLocation.X)
				{
					continue;
				}
				break;
			case SearchDirectionHint.Right:
				if (point.X > itemLocation.X)
				{
					continue;
				}
				break;
			}
			int num2 = point.X - itemLocation.X;
			int num3 = point.Y - itemLocation.Y;
			int num4 = num2 * num2 + num3 * num3;
			if (num4 < num)
			{
				result = items[i];
				num = num4;
			}
		}
		return result;
	}

	public ListViewItem GetItemAt(int x, int y)
	{
		Size itemSize = ItemSize;
		for (int i = 0; i < items.Count; i++)
		{
			Point itemLocation = GetItemLocation(i);
			if (new Rectangle(itemLocation, itemSize).Contains(x, y))
			{
				return items[i];
			}
		}
		return null;
	}

	public Rectangle GetItemRect(int index)
	{
		return GetItemRect(index, ItemBoundsPortion.Entire);
	}

	public Rectangle GetItemRect(int index, ItemBoundsPortion portion)
	{
		if (index < 0 || index >= items.Count)
		{
			throw new IndexOutOfRangeException("index");
		}
		return items[index].GetBounds(portion);
	}

	public ListViewHitTestInfo HitTest(Point point)
	{
		return HitTest(point.X, point.Y);
	}

	public ListViewHitTestInfo HitTest(int x, int y)
	{
		if (x < 0)
		{
			throw new ArgumentOutOfRangeException("x");
		}
		if (y < 0)
		{
			throw new ArgumentOutOfRangeException("y");
		}
		ListViewItem itemAt = GetItemAt(x, y);
		if (itemAt == null)
		{
			return new ListViewHitTestInfo(null, null, ListViewHitTestLocations.None);
		}
		ListViewHitTestLocations listViewHitTestLocations = (ListViewHitTestLocations)0;
		if (itemAt.GetBounds(ItemBoundsPortion.Label).Contains(x, y))
		{
			listViewHitTestLocations |= ListViewHitTestLocations.Label;
		}
		else if (itemAt.GetBounds(ItemBoundsPortion.Icon).Contains(x, y))
		{
			listViewHitTestLocations |= ListViewHitTestLocations.Image;
		}
		else if (itemAt.CheckRectReal.Contains(x, y))
		{
			listViewHitTestLocations |= ListViewHitTestLocations.StateImage;
		}
		ListViewItem.ListViewSubItem hitSubItem = null;
		if (view == View.Details)
		{
			foreach (ListViewItem.ListViewSubItem subItem in itemAt.SubItems)
			{
				if (subItem.Bounds.Contains(x, y))
				{
					hitSubItem = subItem;
					break;
				}
			}
		}
		return new ListViewHitTestInfo(itemAt, hitSubItem, listViewHitTestLocations);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void RedrawItems(int startIndex, int endIndex, bool invalidateOnly)
	{
		if (startIndex < 0 || startIndex >= items.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		if (endIndex < 0 || endIndex >= items.Count)
		{
			throw new ArgumentOutOfRangeException("endIndex");
		}
		if (startIndex > endIndex)
		{
			throw new ArgumentException("startIndex");
		}
		if (!updating)
		{
			for (int i = startIndex; i <= endIndex; i++)
			{
				items[i].Invalidate();
			}
			if (!invalidateOnly)
			{
				Update();
			}
		}
	}

	public void Sort()
	{
		if (virtual_mode)
		{
			throw new InvalidOperationException();
		}
		Sort(redraw: true);
	}

	private void Sort(bool redraw)
	{
		if (base.IsHandleCreated && item_sorter != null)
		{
			items.Sort(item_sorter);
			if (redraw)
			{
				Redraw(recalculate: true);
			}
		}
	}

	public override string ToString()
	{
		int count = Items.Count;
		if (count == 0)
		{
			return $"System.Windows.Forms.ListView, Items.Count: 0";
		}
		return $"System.Windows.Forms.ListView, Items.Count: {count}, Items[0]: {Items[0].ToString()}";
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
	}

	protected virtual void OnColumnReordered(ColumnReorderedEventArgs e)
	{
		((ColumnReorderedEventHandler)base.Events[ColumnReordered])?.Invoke(this, e);
	}

	protected virtual void OnColumnWidthChanged(ColumnWidthChangedEventArgs e)
	{
		((ColumnWidthChangedEventHandler)base.Events[ColumnWidthChanged])?.Invoke(this, e);
	}

	private void RaiseColumnWidthChanged(int resize_column)
	{
		ColumnWidthChangedEventArgs e = new ColumnWidthChangedEventArgs(resize_column);
		OnColumnWidthChanged(e);
	}

	protected virtual void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
	{
		((ColumnWidthChangingEventHandler)base.Events[ColumnWidthChanging])?.Invoke(this, e);
	}

	private bool CanProceedWithResize(ColumnHeader col, int width)
	{
		ColumnWidthChangingEventHandler columnWidthChangingEventHandler = (ColumnWidthChangingEventHandler)base.Events[ColumnWidthChanging];
		if (columnWidthChangingEventHandler == null)
		{
			return true;
		}
		ColumnWidthChangingEventArgs columnWidthChangingEventArgs = new ColumnWidthChangingEventArgs(col.Index, width);
		columnWidthChangingEventHandler(this, columnWidthChangingEventArgs);
		return !columnWidthChangingEventArgs.Cancel;
	}

	internal void RaiseColumnWidthChanged(ColumnHeader column)
	{
		int resize_column = Columns.IndexOf(column);
		RaiseColumnWidthChanged(resize_column);
	}

	internal Rectangle UIAGetHeaderBounds(ListViewGroup group)
	{
		return group.HeaderBounds;
	}

	private void OnUIACheckBoxesChanged()
	{
		((EventHandler)base.Events[UIACheckBoxesChanged])?.Invoke(this, EventArgs.Empty);
	}

	private void OnUIAShowGroupsChanged()
	{
		((EventHandler)base.Events[UIAShowGroupsChanged])?.Invoke(this, EventArgs.Empty);
	}

	private void OnUIAMultiSelectChanged()
	{
		((EventHandler)base.Events[UIAMultiSelectChanged])?.Invoke(this, EventArgs.Empty);
	}

	private void OnUIALabelEditChanged()
	{
		((EventHandler)base.Events[UIALabelEditChanged])?.Invoke(this, EventArgs.Empty);
	}

	private void OnUIAViewChanged()
	{
		((EventHandler)base.Events[UIAViewChanged])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIAFocusedItemChanged()
	{
		((EventHandler)base.Events[UIAFocusedItemChanged])?.Invoke(this, EventArgs.Empty);
	}
}
