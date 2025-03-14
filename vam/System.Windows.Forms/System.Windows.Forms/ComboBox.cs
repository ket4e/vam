using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[DefaultEvent("SelectedIndexChanged")]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultProperty("Items")]
[Designer("System.Windows.Forms.Design.ComboBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultBindingProperty("Text")]
public class ComboBox : ListControl
{
	[ComVisible(true)]
	public class ChildAccessibleObject : AccessibleObject
	{
		public override string Name => base.Name;

		public ChildAccessibleObject(ComboBox owner, IntPtr handle)
			: base(owner)
		{
		}
	}

	[ListBindable(false)]
	public class ObjectCollection : ICollection, IEnumerable, IList
	{
		private class ObjectComparer : IComparer
		{
			private ListControl owner;

			public ObjectComparer(ListControl owner)
			{
				this.owner = owner;
			}

			public int Compare(object x, object y)
			{
				return string.Compare(owner.GetItemText(x), owner.GetItemText(y));
			}
		}

		private ComboBox owner;

		internal ArrayList object_items = new ArrayList();

		private static object UIACollectionChangedEvent;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => false;

		public int Count => object_items.Count;

		public bool IsReadOnly => false;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return object_items[index];
			}
			set
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, object_items[index]));
				object_items[index] = value;
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
				if (owner.listbox_ctrl != null)
				{
					owner.listbox_ctrl.InvalidateItem(index);
				}
				if (index == owner.SelectedIndex)
				{
					if (owner.textbox_ctrl == null)
					{
						owner.Refresh();
					}
					else
					{
						owner.textbox_ctrl.SelectedText = value.ToString();
					}
				}
			}
		}

		internal event CollectionChangeEventHandler UIACollectionChanged
		{
			add
			{
				owner.Events.AddHandler(UIACollectionChangedEvent, value);
			}
			remove
			{
				owner.Events.RemoveHandler(UIACollectionChangedEvent, value);
			}
		}

		public ObjectCollection(ComboBox owner)
		{
			this.owner = owner;
		}

		static ObjectCollection()
		{
			UIACollectionChanged = new object();
		}

		void ICollection.CopyTo(Array destination, int index)
		{
			object_items.CopyTo(destination, index);
		}

		int IList.Add(object item)
		{
			return Add(item);
		}

		internal void OnUIACollectionChangedEvent(CollectionChangeEventArgs args)
		{
			((CollectionChangeEventHandler)owner.Events[UIACollectionChanged])?.Invoke(owner, args);
		}

		public int Add(object item)
		{
			int result = AddItem(item, suspend: false);
			owner.UpdatedItems();
			return result;
		}

		public void AddRange(object[] items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (object item in items)
			{
				AddItem(item, suspend: true);
			}
			if (owner.sorted)
			{
				Sort();
			}
			owner.UpdatedItems();
		}

		public void Clear()
		{
			owner.selected_index = -1;
			object_items.Clear();
			owner.UpdatedItems();
			owner.Refresh();
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}

		public bool Contains(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return object_items.Contains(value);
		}

		public void CopyTo(object[] destination, int arrayIndex)
		{
			object_items.CopyTo(destination, arrayIndex);
		}

		public IEnumerator GetEnumerator()
		{
			return object_items.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return object_items.IndexOf(value);
		}

		public void Insert(int index, object item)
		{
			if (index < 0 || index > Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			owner.BeginUpdate();
			if (owner.Sorted)
			{
				AddItem(item, suspend: false);
			}
			else
			{
				object_items.Insert(index, item);
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
			}
			owner.EndUpdate();
		}

		public void Remove(object value)
		{
			if (value != null)
			{
				if (IndexOf(value) == owner.SelectedIndex)
				{
					owner.SelectedIndex = -1;
				}
				RemoveAt(IndexOf(value));
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (index == owner.SelectedIndex)
			{
				owner.SelectedIndex = -1;
			}
			object element = object_items[index];
			object_items.RemoveAt(index);
			owner.UpdatedItems();
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
		}

		private int AddItem(object item, bool suspend)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (owner.Sorted && !suspend)
			{
				int num = 0;
				foreach (object object_item in object_items)
				{
					if (string.Compare(item.ToString(), object_item.ToString()) < 0)
					{
						object_items.Insert(num, item);
						if (num <= owner.selected_index && owner.IsHandleCreated)
						{
							owner.selected_index++;
						}
						OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
						return num;
					}
					num++;
				}
			}
			object_items.Add(item);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
			return object_items.Count - 1;
		}

		internal void AddRange(IList items)
		{
			foreach (object item in items)
			{
				AddItem(item, suspend: false);
			}
			if (owner.sorted)
			{
				Sort();
			}
			owner.UpdatedItems();
		}

		internal void Sort()
		{
			if (object_items.Count > 0 && object_items[0] is IComparer)
			{
				object_items.Sort();
			}
			else
			{
				object_items.Sort(new ObjectComparer(owner));
			}
		}
	}

	internal class ComboTextBox : TextBox
	{
		private ComboBox owner;

		public override bool Focused => owner.Focused;

		internal override bool ActivateOnShow => false;

		public ComboTextBox(ComboBox owner)
		{
			this.owner = owner;
			base.ShowSelection = false;
			base.HideSelection = false;
			owner.LostFocus += OwnerLostFocusHandler;
		}

		private void OwnerLostFocusHandler(object o, EventArgs args)
		{
			if (base.IsAutoCompleteAvailable)
			{
				owner.Text = Text;
			}
		}

		protected override void OnKeyDown(KeyEventArgs args)
		{
			if (args.KeyCode == Keys.Return && base.IsAutoCompleteAvailable)
			{
				owner.Text = Text;
			}
			base.OnKeyDown(args);
		}

		internal override void OnAutoCompleteValueSelected(EventArgs args)
		{
			base.OnAutoCompleteValueSelected(args);
			owner.Text = Text;
		}

		internal void SetSelectable(bool selectable)
		{
			SetStyle(ControlStyles.Selectable, selectable);
		}

		internal void ActivateCaret(bool active)
		{
			if (active)
			{
				document.CaretHasFocus();
			}
			else
			{
				document.CaretLostFocus();
			}
		}

		internal override void OnTextUpdate()
		{
			base.OnTextUpdate();
			owner.OnTextUpdate(EventArgs.Empty);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			owner.Select(directed: false, forward: true);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			owner.Select(directed: false, forward: true);
		}
	}

	internal class ComboListBox : Control
	{
		internal enum ItemNavigation
		{
			First,
			Last,
			Next,
			Previous,
			NextPage,
			PreviousPage
		}

		private class VScrollBarLB : VScrollBar
		{
			internal override bool InternalCapture
			{
				get
				{
					return base.Capture;
				}
				set
				{
				}
			}

			public void FireMouseDown(MouseEventArgs e)
			{
				if (base.Visible)
				{
					e = TranslateEvent(e);
					OnMouseDown(e);
				}
			}

			public void FireMouseUp(MouseEventArgs e)
			{
				if (base.Visible)
				{
					e = TranslateEvent(e);
					OnMouseUp(e);
				}
			}

			public void FireMouseMove(MouseEventArgs e)
			{
				if (base.Visible)
				{
					e = TranslateEvent(e);
					OnMouseMove(e);
				}
			}

			private MouseEventArgs TranslateEvent(MouseEventArgs e)
			{
				Point point = PointToClient(Control.MousePosition);
				return new MouseEventArgs(e.Button, e.Clicks, point.X, point.Y, e.Delta);
			}
		}

		private ComboBox owner;

		private VScrollBarLB vscrollbar_ctrl;

		private int top_item;

		private int last_item;

		internal int page_size;

		private Rectangle textarea_drawable;

		private int highlighted_index = -1;

		private bool scrollbar_grabbed;

		internal int UIATopItem => top_item;

		internal int UIALastItem => last_item;

		internal ScrollBar UIAVScrollBar => vscrollbar_ctrl;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				if (owner == null || owner.DropDownStyle == ComboBoxStyle.Simple)
				{
					return createParams;
				}
				createParams.Style ^= 1073741824;
				createParams.Style ^= 268435456;
				createParams.Style |= int.MinValue;
				createParams.ExStyle |= 136;
				return createParams;
			}
		}

		internal override bool InternalCapture
		{
			get
			{
				return base.Capture;
			}
			set
			{
			}
		}

		internal override bool ActivateOnShow => false;

		public int HighlightedIndex
		{
			get
			{
				return highlighted_index;
			}
			set
			{
				if (highlighted_index != value)
				{
					if (highlighted_index != -1 && highlighted_index < owner.Items.Count)
					{
						Invalidate(GetItemDisplayRectangle(highlighted_index, top_item));
					}
					highlighted_index = value;
					if (highlighted_index != -1)
					{
						Invalidate(GetItemDisplayRectangle(highlighted_index, top_item));
					}
				}
			}
		}

		private bool InScrollBar
		{
			get
			{
				if (vscrollbar_ctrl == null || !vscrollbar_ctrl.is_visible)
				{
					return false;
				}
				return vscrollbar_ctrl.Bounds.Contains(PointToClient(Control.MousePosition));
			}
		}

		public ComboListBox(ComboBox owner)
		{
			this.owner = owner;
			top_item = 0;
			last_item = 0;
			page_size = 0;
			base.MouseWheel += OnMouseWheelCLB;
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, value: true);
			SetStyle(ControlStyles.Opaque | ControlStyles.ResizeRedraw, value: true);
			is_visible = false;
			if (owner.DropDownStyle == ComboBoxStyle.Simple)
			{
				base.InternalBorderStyle = BorderStyle.Fixed3D;
			}
			else
			{
				base.InternalBorderStyle = BorderStyle.FixedSingle;
			}
		}

		internal void CalcListBoxArea()
		{
			int num;
			int num2;
			bool flag;
			if (owner.DropDownStyle == ComboBoxStyle.Simple)
			{
				Rectangle listbox_area = owner.listbox_area;
				num = listbox_area.Width;
				num2 = listbox_area.Height;
				flag = owner.Items.Count * owner.ItemHeight > num2;
				if (num2 <= 0 || num <= 0)
				{
					return;
				}
			}
			else
			{
				num = owner.DropDownWidth;
				int num3 = ((owner.Items.Count > owner.MaxDropDownItems) ? owner.MaxDropDownItems : owner.Items.Count);
				if (owner.DrawMode == DrawMode.OwnerDrawVariable)
				{
					num2 = 0;
					for (int i = 0; i < num3; i++)
					{
						num2 += owner.GetItemHeight(i);
					}
					flag = owner.Items.Count > owner.MaxDropDownItems;
				}
				else if (owner.DropDownHeight == 106)
				{
					num2 = owner.ItemHeight * num3;
					flag = owner.Items.Count > owner.MaxDropDownItems;
				}
				else
				{
					num2 = owner.DropDownHeight;
					flag = owner.Items.Count * owner.ItemHeight > num2;
				}
			}
			page_size = Math.Max(num2 / owner.ItemHeight, 1);
			ComboBoxStyle dropDownStyle = owner.DropDownStyle;
			if (!flag)
			{
				if (vscrollbar_ctrl != null)
				{
					vscrollbar_ctrl.Visible = false;
				}
				if (dropDownStyle != 0)
				{
					num2 = owner.ItemHeight * owner.items.Count;
				}
			}
			else
			{
				if (vscrollbar_ctrl == null)
				{
					vscrollbar_ctrl = new VScrollBarLB();
					vscrollbar_ctrl.Minimum = 0;
					vscrollbar_ctrl.SmallChange = 1;
					vscrollbar_ctrl.LargeChange = 1;
					vscrollbar_ctrl.Maximum = 0;
					vscrollbar_ctrl.ValueChanged += VerticalScrollEvent;
					base.Controls.AddImplicit(vscrollbar_ctrl);
				}
				vscrollbar_ctrl.Dock = DockStyle.Right;
				vscrollbar_ctrl.Maximum = owner.Items.Count - 1;
				int num4 = page_size;
				if (num4 < 1)
				{
					num4 = 1;
				}
				vscrollbar_ctrl.LargeChange = num4;
				vscrollbar_ctrl.Visible = true;
				int highlightedIndex = HighlightedIndex;
				if (highlightedIndex > 0)
				{
					highlightedIndex = Math.Min(highlightedIndex, vscrollbar_ctrl.Maximum);
					vscrollbar_ctrl.Value = highlightedIndex;
				}
			}
			base.Size = new Size(num, num2);
			textarea_drawable = base.ClientRectangle;
			textarea_drawable.Width = num;
			textarea_drawable.Height = num2;
			if (vscrollbar_ctrl != null && flag)
			{
				textarea_drawable.Width -= vscrollbar_ctrl.Width;
			}
			last_item = LastVisibleItem();
		}

		private void Draw(Rectangle clip, Graphics dc)
		{
			dc.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(owner.BackColor), clip);
			if (owner.Items.Count <= 0)
			{
				return;
			}
			for (int i = top_item; i <= last_item; i++)
			{
				Rectangle itemDisplayRectangle = GetItemDisplayRectangle(i, top_item);
				if (!clip.IntersectsWith(itemDisplayRectangle))
				{
					continue;
				}
				DrawItemState drawItemState = DrawItemState.None;
				Color backColor = owner.BackColor;
				Color foreColor = owner.ForeColor;
				if (i == HighlightedIndex)
				{
					drawItemState |= DrawItemState.Selected;
					backColor = SystemColors.Highlight;
					foreColor = SystemColors.HighlightText;
					if (owner.DropDownStyle == ComboBoxStyle.DropDownList)
					{
						drawItemState |= DrawItemState.Focus;
					}
				}
				owner.HandleDrawItem(new DrawItemEventArgs(dc, owner.Font, itemDisplayRectangle, i, drawItemState, foreColor, backColor));
			}
		}

		private Rectangle GetItemDisplayRectangle(int index, int top_index)
		{
			if (index < 0 || index >= owner.Items.Count)
			{
				throw new ArgumentOutOfRangeException("GetItemRectangle index out of range.");
			}
			Rectangle result = default(Rectangle);
			int itemHeight = owner.GetItemHeight(index);
			result.X = 0;
			result.Width = textarea_drawable.Width;
			if (owner.DrawMode == DrawMode.OwnerDrawVariable)
			{
				result.Y = 0;
				for (int i = top_index; i < index; i++)
				{
					result.Y += owner.GetItemHeight(i);
				}
			}
			else
			{
				result.Y = itemHeight * (index - top_index);
			}
			result.Height = itemHeight;
			return result;
		}

		public void HideWindow()
		{
			if (owner.DropDownStyle != 0)
			{
				base.Capture = false;
				Hide();
				owner.DropDownListBoxFinished();
			}
		}

		private int IndexFromPointDisplayRectangle(int x, int y)
		{
			for (int i = top_item; i <= last_item; i++)
			{
				if (GetItemDisplayRectangle(i, top_item).Contains(x, y))
				{
					return i;
				}
			}
			return -1;
		}

		public void InvalidateItem(int index)
		{
			if (base.Visible)
			{
				Invalidate(GetItemDisplayRectangle(index, top_item));
			}
		}

		public int LastVisibleItem()
		{
			int num = textarea_drawable.Y + textarea_drawable.Height;
			int num2 = 0;
			for (num2 = top_item; num2 < owner.Items.Count; num2++)
			{
				Rectangle itemDisplayRectangle = GetItemDisplayRectangle(num2, top_item);
				if (itemDisplayRectangle.Y + itemDisplayRectangle.Height > num)
				{
					return num2;
				}
			}
			return num2 - 1;
		}

		public void SetTopItem(int item)
		{
			if (top_item != item)
			{
				top_item = item;
				UpdateLastVisibleItem();
				Invalidate();
			}
		}

		public int FirstVisibleItem()
		{
			return top_item;
		}

		public void EnsureTop(int item)
		{
			if (owner.Items.Count != 0 && vscrollbar_ctrl != null && vscrollbar_ctrl.Visible)
			{
				int num = vscrollbar_ctrl.Maximum - page_size + 1;
				if (item > num)
				{
					item = num;
				}
				else if (item < vscrollbar_ctrl.Minimum)
				{
					item = vscrollbar_ctrl.Minimum;
				}
				vscrollbar_ctrl.Value = item;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (InScrollBar)
			{
				vscrollbar_ctrl.FireMouseDown(e);
				scrollbar_grabbed = true;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (owner.DropDownStyle == ComboBoxStyle.Simple)
			{
				return;
			}
			if (scrollbar_grabbed || (!base.Capture && InScrollBar))
			{
				vscrollbar_ctrl.FireMouseMove(e);
				return;
			}
			Point point = PointToClient(Control.MousePosition);
			int num = IndexFromPointDisplayRectangle(point.X, point.Y);
			if (num != -1)
			{
				HighlightedIndex = num;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			int num = IndexFromPointDisplayRectangle(e.X, e.Y);
			if (scrollbar_grabbed)
			{
				vscrollbar_ctrl.FireMouseUp(e);
				scrollbar_grabbed = false;
				if (num != -1)
				{
					HighlightedIndex = num;
				}
				return;
			}
			if (num == -1)
			{
				HideWindow();
				return;
			}
			bool flag = owner.SelectedIndex != num;
			owner.SetSelectedIndex(num, supressAutoScroll: true);
			owner.OnSelectionChangeCommitted(new EventArgs());
			if (!flag)
			{
				owner.OnSelectedValueChanged(EventArgs.Empty);
				owner.OnSelectedIndexChanged(EventArgs.Empty);
			}
			HideWindow();
		}

		internal override void OnPaintInternal(PaintEventArgs pevent)
		{
			Draw(pevent.ClipRectangle, pevent.Graphics);
		}

		public bool ShowWindow()
		{
			if (owner.DropDownStyle == ComboBoxStyle.Simple && owner.Items.Count == 0)
			{
				return false;
			}
			HighlightedIndex = owner.SelectedIndex;
			CalcListBoxArea();
			Show();
			Refresh();
			owner.OnDropDown(EventArgs.Empty);
			return true;
		}

		public void UpdateLastVisibleItem()
		{
			last_item = LastVisibleItem();
		}

		public void Scroll(int delta)
		{
			if (delta != 0 && vscrollbar_ctrl != null && vscrollbar_ctrl.Visible)
			{
				int num = vscrollbar_ctrl.Maximum - page_size + 1;
				int num2 = vscrollbar_ctrl.Value + delta;
				if (num2 > num)
				{
					num2 = num;
				}
				else if (num2 < vscrollbar_ctrl.Minimum)
				{
					num2 = vscrollbar_ctrl.Minimum;
				}
				vscrollbar_ctrl.Value = num2;
			}
		}

		private void OnMouseWheelCLB(object sender, MouseEventArgs me)
		{
			if (owner.Items.Count != 0)
			{
				int num = me.Delta / 120 * SystemInformation.MouseWheelScrollLines;
				Scroll(-num);
			}
		}

		private void VerticalScrollEvent(object sender, EventArgs e)
		{
			if (top_item != vscrollbar_ctrl.Value)
			{
				top_item = vscrollbar_ctrl.Value;
				UpdateLastVisibleItem();
				Invalidate();
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 7)
			{
				owner.Select(directed: false, forward: true);
			}
			base.WndProc(ref m);
		}
	}

	private const int button_width = 16;

	private const int default_drop_down_height = 106;

	private DrawMode draw_mode;

	private ComboBoxStyle dropdown_style;

	private int dropdown_width = -1;

	private int selected_index = -1;

	private ObjectCollection items;

	private bool suspend_ctrlupdate;

	private int maxdrop_items = 8;

	private bool integral_height = true;

	private bool sorted;

	private int max_length;

	private ComboListBox listbox_ctrl;

	private ComboTextBox textbox_ctrl;

	private bool process_textchanged_event = true;

	private bool process_texchanged_autoscroll = true;

	private bool item_height_specified;

	private int item_height;

	private int requested_height = -1;

	private Hashtable item_heights;

	private bool show_dropdown_button;

	private ButtonState button_state;

	private bool dropped_down;

	private Rectangle text_area;

	private Rectangle button_area;

	private Rectangle listbox_area;

	private bool drop_down_button_entered;

	private AutoCompleteStringCollection auto_complete_custom_source;

	private AutoCompleteMode auto_complete_mode;

	private AutoCompleteSource auto_complete_source = AutoCompleteSource.None;

	private FlatStyle flat_style;

	private int drop_down_height;

	private static object DrawItemEvent;

	private static object DropDownEvent;

	private static object DropDownStyleChangedEvent;

	private static object MeasureItemEvent;

	private static object SelectedIndexChangedEvent;

	private static object SelectionChangeCommittedEvent;

	private static object DropDownClosedEvent;

	private static object TextUpdateEvent;

	[Localizable(true)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public AutoCompleteStringCollection AutoCompleteCustomSource
	{
		get
		{
			if (auto_complete_custom_source == null)
			{
				auto_complete_custom_source = new AutoCompleteStringCollection();
				auto_complete_custom_source.CollectionChanged += OnAutoCompleteCustomSourceChanged;
			}
			return auto_complete_custom_source;
		}
		set
		{
			if (auto_complete_custom_source != value)
			{
				if (auto_complete_custom_source != null)
				{
					auto_complete_custom_source.CollectionChanged -= OnAutoCompleteCustomSourceChanged;
				}
				auto_complete_custom_source = value;
				if (auto_complete_custom_source != null)
				{
					auto_complete_custom_source.CollectionChanged += OnAutoCompleteCustomSourceChanged;
				}
				SetTextBoxAutoCompleteData();
			}
		}
	}

	[DefaultValue(AutoCompleteMode.None)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public AutoCompleteMode AutoCompleteMode
	{
		get
		{
			return auto_complete_mode;
		}
		set
		{
			if (auto_complete_mode != value)
			{
				if (value < AutoCompleteMode.None || value > AutoCompleteMode.SuggestAppend)
				{
					throw new InvalidEnumArgumentException(Locale.GetText("Enum argument value '{0}' is not valid for AutoCompleteMode", value));
				}
				auto_complete_mode = value;
				SetTextBoxAutoCompleteData();
			}
		}
	}

	[DefaultValue(AutoCompleteSource.None)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	[System.MonoTODO("AutoCompletion algorithm is currently not implemented.")]
	public AutoCompleteSource AutoCompleteSource
	{
		get
		{
			return auto_complete_source;
		}
		set
		{
			if (auto_complete_source != value)
			{
				if (!Enum.IsDefined(typeof(AutoCompleteSource), value))
				{
					throw new InvalidEnumArgumentException(Locale.GetText("Enum argument value '{0}' is not valid for AutoCompleteSource", value));
				}
				auto_complete_source = value;
				SetTextBoxAutoCompleteData();
			}
		}
	}

	public override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			if (!(base.BackColor == value))
			{
				base.BackColor = value;
				Refresh();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			if (base.BackgroundImage != value)
			{
				base.BackgroundImage = value;
				Refresh();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	protected override CreateParams CreateParams => base.CreateParams;

	[MWFCategory("Data")]
	[DefaultValue(null)]
	[AttributeProvider(typeof(IListSource))]
	[RefreshProperties(RefreshProperties.Repaint)]
	public new object DataSource
	{
		get
		{
			return base.DataSource;
		}
		set
		{
			base.DataSource = value;
		}
	}

	protected override Size DefaultSize => new Size(121, 21);

	[DefaultValue(DrawMode.Normal)]
	[MWFCategory("Behavior")]
	[RefreshProperties(RefreshProperties.Repaint)]
	public DrawMode DrawMode
	{
		get
		{
			return draw_mode;
		}
		set
		{
			if (!Enum.IsDefined(typeof(DrawMode), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for DrawMode");
			}
			if (draw_mode != value)
			{
				if (draw_mode == DrawMode.OwnerDrawVariable)
				{
					item_heights = null;
				}
				draw_mode = value;
				if (draw_mode == DrawMode.OwnerDrawVariable)
				{
					item_heights = new Hashtable();
				}
				Refresh();
			}
		}
	}

	[Browsable(true)]
	[DefaultValue(106)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[MWFCategory("Behavior")]
	public int DropDownHeight
	{
		get
		{
			return drop_down_height;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("DropDownHeight", "DropDownHeight must be greater than 0.");
			}
			if (value != drop_down_height)
			{
				drop_down_height = value;
				IntegralHeight = false;
			}
		}
	}

	[MWFCategory("Appearance")]
	[DefaultValue(ComboBoxStyle.DropDown)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public ComboBoxStyle DropDownStyle
	{
		get
		{
			return dropdown_style;
		}
		set
		{
			if (!Enum.IsDefined(typeof(ComboBoxStyle), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for ComboBoxStyle");
			}
			if (dropdown_style == value)
			{
				return;
			}
			SuspendLayout();
			if (dropdown_style == ComboBoxStyle.Simple && listbox_ctrl != null)
			{
				base.Controls.RemoveImplicit(listbox_ctrl);
				listbox_ctrl.Dispose();
				listbox_ctrl = null;
			}
			dropdown_style = value;
			if (dropdown_style == ComboBoxStyle.DropDownList && textbox_ctrl != null)
			{
				base.Controls.RemoveImplicit(textbox_ctrl);
				textbox_ctrl.Dispose();
				textbox_ctrl = null;
			}
			if (dropdown_style == ComboBoxStyle.Simple)
			{
				show_dropdown_button = false;
				CreateComboListBox();
				base.Controls.AddImplicit(listbox_ctrl);
				listbox_ctrl.Visible = true;
				if (requested_height == -1)
				{
					requested_height = 150;
				}
			}
			else
			{
				show_dropdown_button = true;
				button_state = ButtonState.Normal;
			}
			if (dropdown_style != ComboBoxStyle.DropDownList && textbox_ctrl == null)
			{
				textbox_ctrl = new ComboTextBox(this);
				object selectedItem = SelectedItem;
				if (selectedItem != null)
				{
					textbox_ctrl.Text = GetItemText(selectedItem);
				}
				textbox_ctrl.BorderStyle = BorderStyle.None;
				textbox_ctrl.TextChanged += OnTextChangedEdit;
				textbox_ctrl.KeyPress += OnTextKeyPress;
				textbox_ctrl.Click += OnTextBoxClick;
				textbox_ctrl.ContextMenu = ContextMenu;
				textbox_ctrl.TopMargin = 1;
				if (base.IsHandleCreated)
				{
					base.Controls.AddImplicit(textbox_ctrl);
				}
				SetTextBoxAutoCompleteData();
			}
			ResumeLayout();
			OnDropDownStyleChanged(EventArgs.Empty);
			LayoutComboBox();
			UpdateComboBoxBounds();
			Refresh();
		}
	}

	[MWFCategory("Behavior")]
	public int DropDownWidth
	{
		get
		{
			if (dropdown_width == -1)
			{
				return base.Width;
			}
			return dropdown_width;
		}
		set
		{
			if (dropdown_width != value)
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("DropDownWidth", "The DropDownWidth value is less than one.");
				}
				dropdown_width = value;
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool DroppedDown
	{
		get
		{
			if (dropdown_style == ComboBoxStyle.Simple)
			{
				return true;
			}
			return dropped_down;
		}
		set
		{
			if (dropdown_style != 0 && dropped_down != value)
			{
				if (value)
				{
					DropDownListBox();
				}
				else
				{
					listbox_ctrl.HideWindow();
				}
			}
		}
	}

	[Localizable(true)]
	[MWFCategory("Appearance")]
	[DefaultValue(FlatStyle.Standard)]
	public FlatStyle FlatStyle
	{
		get
		{
			return flat_style;
		}
		set
		{
			if (!Enum.IsDefined(typeof(FlatStyle), value))
			{
				throw new InvalidEnumArgumentException("FlatStyle", (int)value, typeof(FlatStyle));
			}
			flat_style = value;
			LayoutComboBox();
			Invalidate();
		}
	}

	public override bool Focused => base.Focused;

	public override Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			if (!(base.ForeColor == value))
			{
				base.ForeColor = value;
				Refresh();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(true)]
	[MWFCategory("Behavior")]
	public bool IntegralHeight
	{
		get
		{
			return integral_height;
		}
		set
		{
			if (integral_height != value)
			{
				integral_height = value;
				UpdateComboBoxBounds();
				Refresh();
			}
		}
	}

	[Localizable(true)]
	[MWFCategory("Behavior")]
	public int ItemHeight
	{
		get
		{
			if (item_height == -1)
			{
				item_height = (int)TextRenderer.MeasureString("The quick brown Fox", Font).Height;
			}
			return item_height;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("ItemHeight", "The item height value is less than one.");
			}
			item_height_specified = true;
			item_height = value;
			if (IntegralHeight)
			{
				UpdateComboBoxBounds();
			}
			LayoutComboBox();
			Refresh();
		}
	}

	[Localizable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[MergableProperty(false)]
	[MWFCategory("Data")]
	[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public ObjectCollection Items => items;

	[DefaultValue(8)]
	[MWFCategory("Behavior")]
	[Localizable(true)]
	public int MaxDropDownItems
	{
		get
		{
			return maxdrop_items;
		}
		set
		{
			if (maxdrop_items != value)
			{
				maxdrop_items = value;
			}
		}
	}

	public override Size MaximumSize
	{
		get
		{
			return base.MaximumSize;
		}
		set
		{
			base.MaximumSize = new Size(value.Width, 0);
		}
	}

	[DefaultValue(0)]
	[Localizable(true)]
	[MWFCategory("Behavior")]
	public int MaxLength
	{
		get
		{
			return max_length;
		}
		set
		{
			if (max_length == value)
			{
				return;
			}
			max_length = value;
			if (dropdown_style != ComboBoxStyle.DropDownList)
			{
				if (value < 0)
				{
					value = 0;
				}
				textbox_ctrl.MaxLength = value;
			}
		}
	}

	public override Size MinimumSize
	{
		get
		{
			return base.MinimumSize;
		}
		set
		{
			base.MinimumSize = new Size(value.Width, 0);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public int PreferredHeight => Font.Height + 8;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override int SelectedIndex
	{
		get
		{
			return selected_index;
		}
		set
		{
			SetSelectedIndex(value, supressAutoScroll: false);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[Bindable(true)]
	public object SelectedItem
	{
		get
		{
			return (selected_index != -1) ? Items[selected_index] : null;
		}
		set
		{
			object obj = ((selected_index != -1) ? Items[selected_index] : null);
			if (obj != value)
			{
				if (value == null)
				{
					SelectedIndex = -1;
				}
				else
				{
					SelectedIndex = Items.IndexOf(value);
				}
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public string SelectedText
	{
		get
		{
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				return string.Empty;
			}
			return textbox_ctrl.SelectedText;
		}
		set
		{
			if (dropdown_style != ComboBoxStyle.DropDownList)
			{
				textbox_ctrl.SelectedText = value;
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int SelectionLength
	{
		get
		{
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				return 0;
			}
			int selectionLength = textbox_ctrl.SelectionLength;
			return (selectionLength != -1) ? selectionLength : 0;
		}
		set
		{
			if (dropdown_style != ComboBoxStyle.DropDownList && textbox_ctrl.SelectionLength != value)
			{
				textbox_ctrl.SelectionLength = value;
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public int SelectionStart
	{
		get
		{
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				return 0;
			}
			return textbox_ctrl.SelectionStart;
		}
		set
		{
			if (dropdown_style != ComboBoxStyle.DropDownList && textbox_ctrl.SelectionStart != value)
			{
				textbox_ctrl.SelectionStart = value;
			}
		}
	}

	[MWFCategory("Behavior")]
	[DefaultValue(false)]
	public bool Sorted
	{
		get
		{
			return sorted;
		}
		set
		{
			if (sorted != value)
			{
				sorted = value;
				SelectedIndex = -1;
				if (sorted)
				{
					Items.Sort();
					LayoutComboBox();
				}
			}
		}
	}

	[Bindable(true)]
	[Localizable(true)]
	public override string Text
	{
		get
		{
			if (dropdown_style != ComboBoxStyle.DropDownList && textbox_ctrl != null)
			{
				return textbox_ctrl.Text;
			}
			if (SelectedItem != null)
			{
				return GetItemText(SelectedItem);
			}
			return base.Text;
		}
		set
		{
			if (value == null)
			{
				if (SelectedIndex == -1)
				{
					if (dropdown_style != ComboBoxStyle.DropDownList)
					{
						SetControlText(string.Empty, suppressTextChanged: false);
					}
				}
				else
				{
					SelectedIndex = -1;
				}
			}
			else if (SelectedItem == null || string.Compare(value, GetItemText(SelectedItem), ignoreCase: false, CultureInfo.CurrentCulture) != 0)
			{
				int num = FindStringExact(value, -1, ignoreCase: false);
				if (num == -1)
				{
					num = FindStringExact(value, -1, ignoreCase: true);
				}
				if (num != -1)
				{
					SelectedIndex = num;
				}
				else if (dropdown_style != ComboBoxStyle.DropDownList)
				{
					textbox_ctrl.Text = value;
				}
			}
		}
	}

	internal Rectangle ButtonArea => button_area;

	internal Rectangle TextArea => text_area;

	internal TextBox UIATextBox => textbox_ctrl;

	internal ComboListBox UIAComboListBox => listbox_ctrl;

	internal override bool InternalCapture
	{
		get
		{
			return base.Capture;
		}
		set
		{
		}
	}

	internal bool DropDownButtonEntered
	{
		get
		{
			return drop_down_button_entered;
		}
		private set
		{
			if (drop_down_button_entered != value)
			{
				drop_down_button_entered = value;
				if (ThemeEngine.Current.ComboBoxDropDownButtonHasHotElementStyle(this))
				{
					Invalidate(button_area);
				}
			}
		}
	}

	internal override ContextMenu ContextMenuInternal
	{
		get
		{
			return base.ContextMenuInternal;
		}
		set
		{
			base.ContextMenuInternal = value;
			if (textbox_ctrl != null)
			{
				textbox_ctrl.ContextMenu = value;
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageChanged
	{
		add
		{
			base.BackgroundImageChanged += value;
		}
		remove
		{
			base.BackgroundImageChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler DoubleClick
	{
		add
		{
			base.DoubleClick += value;
		}
		remove
		{
			base.DoubleClick -= value;
		}
	}

	public event DrawItemEventHandler DrawItem
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

	public event EventHandler DropDown
	{
		add
		{
			base.Events.AddHandler(DropDownEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownEvent, value);
		}
	}

	public event EventHandler DropDownClosed
	{
		add
		{
			base.Events.AddHandler(DropDownClosedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownClosedEvent, value);
		}
	}

	public event EventHandler DropDownStyleChanged
	{
		add
		{
			base.Events.AddHandler(DropDownStyleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DropDownStyleChangedEvent, value);
		}
	}

	public event MeasureItemEventHandler MeasureItem
	{
		add
		{
			base.Events.AddHandler(MeasureItemEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MeasureItemEvent, value);
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

	public event EventHandler SelectionChangeCommitted
	{
		add
		{
			base.Events.AddHandler(SelectionChangeCommittedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectionChangeCommittedEvent, value);
		}
	}

	public event EventHandler TextUpdate
	{
		add
		{
			base.Events.AddHandler(TextUpdateEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(TextUpdateEvent, value);
		}
	}

	public ComboBox()
	{
		items = new ObjectCollection(this);
		DropDownStyle = ComboBoxStyle.DropDown;
		item_height = base.FontHeight + 2;
		background_color = ThemeEngine.Current.ColorWindow;
		border_style = BorderStyle.None;
		drop_down_height = 106;
		flat_style = FlatStyle.Standard;
		base.MouseDown += OnMouseDownCB;
		base.MouseUp += OnMouseUpCB;
		base.MouseMove += OnMouseMoveCB;
		base.MouseWheel += OnMouseWheelCB;
		base.MouseEnter += OnMouseEnter;
		base.MouseLeave += OnMouseLeave;
		base.KeyDown += OnKeyDownCB;
	}

	static ComboBox()
	{
		DrawItem = new object();
		DropDown = new object();
		DropDownStyleChanged = new object();
		MeasureItem = new object();
		SelectedIndexChanged = new object();
		SelectionChangeCommitted = new object();
		DropDownClosed = new object();
		TextUpdate = new object();
	}

	private void SetTextBoxAutoCompleteData()
	{
		if (textbox_ctrl != null)
		{
			textbox_ctrl.AutoCompleteMode = auto_complete_mode;
			if (auto_complete_source == AutoCompleteSource.ListItems)
			{
				textbox_ctrl.AutoCompleteSource = AutoCompleteSource.CustomSource;
				textbox_ctrl.AutoCompleteCustomSource = null;
				textbox_ctrl.AutoCompleteInternalSource = this;
			}
			else
			{
				textbox_ctrl.AutoCompleteSource = auto_complete_source;
				textbox_ctrl.AutoCompleteCustomSource = auto_complete_custom_source;
				textbox_ctrl.AutoCompleteInternalSource = null;
			}
		}
	}

	[Obsolete("This method has been deprecated")]
	protected virtual void AddItemsCore(object[] value)
	{
	}

	public void BeginUpdate()
	{
		suspend_ctrlupdate = true;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return base.CreateAccessibilityInstance();
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (listbox_ctrl != null)
			{
				listbox_ctrl.Dispose();
				base.Controls.RemoveImplicit(listbox_ctrl);
				listbox_ctrl = null;
			}
			if (textbox_ctrl != null)
			{
				base.Controls.RemoveImplicit(textbox_ctrl);
				textbox_ctrl.Dispose();
				textbox_ctrl = null;
			}
		}
		base.Dispose(disposing);
	}

	public void EndUpdate()
	{
		suspend_ctrlupdate = false;
		UpdatedItems();
		Refresh();
	}

	public int FindString(string s)
	{
		return FindString(s, -1);
	}

	public int FindString(string s, int startIndex)
	{
		if (s == null || Items.Count == 0)
		{
			return -1;
		}
		if (startIndex < -1 || startIndex >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		int num = startIndex;
		if (num == Items.Count - 1)
		{
			num = -1;
		}
		do
		{
			num++;
			if (string.Compare(s, 0, GetItemText(Items[num]), 0, s.Length, ignoreCase: true) == 0)
			{
				return num;
			}
			if (num == Items.Count - 1)
			{
				num = -1;
			}
		}
		while (num != startIndex);
		return -1;
	}

	public int FindStringExact(string s)
	{
		return FindStringExact(s, -1);
	}

	public int FindStringExact(string s, int startIndex)
	{
		return FindStringExact(s, startIndex, ignoreCase: true);
	}

	private int FindStringExact(string s, int startIndex, bool ignoreCase)
	{
		if (s == null || Items.Count == 0)
		{
			return -1;
		}
		if (startIndex < -1 || startIndex >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		int num = startIndex;
		if (num == Items.Count - 1)
		{
			num = -1;
		}
		do
		{
			num++;
			if (string.Compare(s, GetItemText(Items[num]), ignoreCase, CultureInfo.CurrentCulture) == 0)
			{
				return num;
			}
			if (num == Items.Count - 1)
			{
				num = -1;
			}
		}
		while (num != startIndex);
		return -1;
	}

	public int GetItemHeight(int index)
	{
		if (DrawMode == DrawMode.OwnerDrawVariable && base.IsHandleCreated)
		{
			if (index < 0 || index >= Items.Count)
			{
				throw new ArgumentOutOfRangeException("The item height value is less than zero");
			}
			object key = Items[index];
			if (item_heights.Contains(key))
			{
				return (int)item_heights[key];
			}
			MeasureItemEventArgs measureItemEventArgs = new MeasureItemEventArgs(base.DeviceContext, index, ItemHeight);
			OnMeasureItem(measureItemEventArgs);
			item_heights[key] = measureItemEventArgs.ItemHeight;
			return measureItemEventArgs.ItemHeight;
		}
		return ItemHeight;
	}

	protected override bool IsInputKey(Keys keyData)
	{
		switch (keyData & Keys.KeyCode)
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
			return false;
		}
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
		if (textbox_ctrl != null)
		{
			textbox_ctrl.BackColor = BackColor;
		}
	}

	protected override void OnDataSourceChanged(EventArgs e)
	{
		base.OnDataSourceChanged(e);
		BindDataItems();
		if (DataSource == null || base.DataManager == null)
		{
			SelectedIndex = -1;
		}
		else
		{
			SelectedIndex = base.DataManager.Position;
		}
	}

	protected override void OnDisplayMemberChanged(EventArgs e)
	{
		base.OnDisplayMemberChanged(e);
		if (base.DataManager != null)
		{
			SelectedIndex = base.DataManager.Position;
			if (selected_index != -1 && DropDownStyle != ComboBoxStyle.DropDownList)
			{
				SetControlText(GetItemText(Items[selected_index]), suppressTextChanged: true);
			}
			if (base.IsHandleCreated)
			{
				Invalidate();
			}
		}
	}

	protected virtual void OnDrawItem(DrawItemEventArgs e)
	{
		((DrawItemEventHandler)base.Events[DrawItem])?.Invoke(this, e);
	}

	internal void HandleDrawItem(DrawItemEventArgs e)
	{
		DrawMode drawMode = DrawMode;
		if (drawMode == DrawMode.OwnerDrawFixed || drawMode == DrawMode.OwnerDrawVariable)
		{
			OnDrawItem(e);
		}
		else
		{
			ThemeEngine.Current.DrawComboBoxItem(this, e);
		}
	}

	protected virtual void OnDropDown(EventArgs e)
	{
		((EventHandler)base.Events[DropDown])?.Invoke(this, e);
	}

	protected virtual void OnDropDownClosed(EventArgs e)
	{
		((EventHandler)base.Events[DropDownClosed])?.Invoke(this, e);
	}

	protected virtual void OnDropDownStyleChanged(EventArgs e)
	{
		((EventHandler)base.Events[DropDownStyleChanged])?.Invoke(this, e);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		if (textbox_ctrl != null)
		{
			textbox_ctrl.Font = Font;
		}
		if (!item_height_specified)
		{
			item_height = Font.Height + 2;
		}
		if (IntegralHeight)
		{
			UpdateComboBoxBounds();
		}
		LayoutComboBox();
	}

	protected override void OnForeColorChanged(EventArgs e)
	{
		base.OnForeColorChanged(e);
		if (textbox_ctrl != null)
		{
			textbox_ctrl.ForeColor = ForeColor;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnGotFocus(EventArgs e)
	{
		if (dropdown_style == ComboBoxStyle.DropDownList)
		{
			Invalidate();
		}
		if (textbox_ctrl != null)
		{
			textbox_ctrl.SetSelectable(selectable: false);
			textbox_ctrl.ShowSelection = true;
			textbox_ctrl.ActivateCaret(active: true);
			textbox_ctrl.SelectAll();
		}
		base.OnGotFocus(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnLostFocus(EventArgs e)
	{
		if (dropdown_style == ComboBoxStyle.DropDownList)
		{
			Invalidate();
		}
		if (listbox_ctrl != null && dropped_down)
		{
			listbox_ctrl.HideWindow();
		}
		if (textbox_ctrl != null)
		{
			textbox_ctrl.SetSelectable(selectable: true);
			textbox_ctrl.ActivateCaret(active: false);
			textbox_ctrl.ShowSelection = false;
			textbox_ctrl.SelectionLength = 0;
			textbox_ctrl.HideAutoCompleteList();
		}
		base.OnLostFocus(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		SetBoundsInternal(base.Left, base.Top, base.Width, PreferredHeight, BoundsSpecified.None);
		if (textbox_ctrl != null)
		{
			base.Controls.AddImplicit(textbox_ctrl);
		}
		LayoutComboBox();
		UpdateComboBoxBounds();
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected override void OnKeyPress(KeyPressEventArgs e)
	{
		if (dropdown_style == ComboBoxStyle.DropDownList)
		{
			int num = FindStringCaseInsensitive(e.KeyChar.ToString(), SelectedIndex + 1);
			if (num != -1)
			{
				SelectedIndex = num;
				if (DroppedDown)
				{
					if (SelectedIndex >= listbox_ctrl.LastVisibleItem())
					{
						listbox_ctrl.Scroll(SelectedIndex - listbox_ctrl.LastVisibleItem() + 1);
					}
					if (SelectedIndex < listbox_ctrl.FirstVisibleItem())
					{
						listbox_ctrl.Scroll(SelectedIndex - listbox_ctrl.FirstVisibleItem());
					}
				}
			}
		}
		base.OnKeyPress(e);
	}

	protected virtual void OnMeasureItem(MeasureItemEventArgs e)
	{
		((MeasureItemEventHandler)base.Events[MeasureItem])?.Invoke(this, e);
	}

	protected override void OnParentBackColorChanged(EventArgs e)
	{
		base.OnParentBackColorChanged(e);
	}

	protected override void OnResize(EventArgs e)
	{
		LayoutComboBox();
		if (listbox_ctrl != null)
		{
			listbox_ctrl.CalcListBoxArea();
		}
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		base.OnSelectedIndexChanged(e);
		((EventHandler)base.Events[SelectedIndexChanged])?.Invoke(this, e);
	}

	protected virtual void OnSelectedItemChanged(EventArgs e)
	{
	}

	protected override void OnSelectedValueChanged(EventArgs e)
	{
		base.OnSelectedValueChanged(e);
	}

	protected virtual void OnSelectionChangeCommitted(EventArgs e)
	{
		((EventHandler)base.Events[SelectionChangeCommitted])?.Invoke(this, e);
	}

	protected override void RefreshItem(int index)
	{
		if (index < 0 || index >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (draw_mode == DrawMode.OwnerDrawVariable)
		{
			item_heights.Remove(Items[index]);
		}
	}

	protected override void RefreshItems()
	{
		for (int i = 0; i < Items.Count; i++)
		{
			RefreshItem(i);
		}
		LayoutComboBox();
		Refresh();
		if (selected_index != -1 && DropDownStyle != ComboBoxStyle.DropDownList)
		{
			SetControlText(GetItemText(Items[selected_index]), suppressTextChanged: false);
		}
	}

	public override void ResetText()
	{
		Text = string.Empty;
	}

	protected override bool ProcessKeyEventArgs(ref Message m)
	{
		return base.ProcessKeyEventArgs(ref m);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnValidating(CancelEventArgs e)
	{
		base.OnValidating(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnTextChanged(EventArgs e)
	{
		base.OnTextChanged(e);
	}

	protected virtual void OnTextUpdate(EventArgs e)
	{
		((EventHandler)base.Events[TextUpdate])?.Invoke(this, e);
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		if (flat_style == FlatStyle.Popup)
		{
			Invalidate();
		}
		base.OnMouseLeave(e);
	}

	protected override void OnMouseEnter(EventArgs e)
	{
		if (flat_style == FlatStyle.Popup)
		{
			Invalidate();
		}
		base.OnMouseEnter(e);
	}

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		base.ScaleControl(factor, specified);
	}

	public void Select(int start, int length)
	{
		if (start < 0)
		{
			throw new ArgumentException("Start cannot be less than zero");
		}
		if (length < 0)
		{
			throw new ArgumentException("length cannot be less than zero");
		}
		if (dropdown_style != ComboBoxStyle.DropDownList)
		{
			textbox_ctrl.Select(start, length);
		}
	}

	public void SelectAll()
	{
		if (dropdown_style != ComboBoxStyle.DropDownList && textbox_ctrl != null)
		{
			textbox_ctrl.ShowSelection = true;
			textbox_ctrl.SelectAll();
		}
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		bool flag = (Anchor & AnchorStyles.Top) != 0 && (Anchor & AnchorStyles.Bottom) != 0;
		bool flag2 = Dock == DockStyle.Left || Dock == DockStyle.Right || Dock == DockStyle.Fill;
		if ((specified & BoundsSpecified.Height) != 0 || (specified == BoundsSpecified.None && (flag || flag2)))
		{
			requested_height = height;
			height = SnapHeight(height);
		}
		base.SetBoundsCore(x, y, width, height, specified);
	}

	protected override void SetItemCore(int index, object value)
	{
		if (index >= 0 && index < Items.Count)
		{
			Items[index] = value;
		}
	}

	protected override void SetItemsCore(IList value)
	{
		BeginUpdate();
		try
		{
			Items.Clear();
			Items.AddRange(value);
		}
		finally
		{
			EndUpdate();
		}
	}

	public override string ToString()
	{
		return base.ToString() + ", Items.Count:" + Items.Count;
	}

	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_KEYDOWN:
		case Msg.WM_KEYUP:
		{
			Keys keys = (Keys)m.WParam.ToInt32();
			if (textbox_ctrl != null && textbox_ctrl.CanNavigateAutoCompleteList)
			{
				XplatUI.SendMessage(textbox_ctrl.Handle, (Msg)m.Msg, m.WParam, m.LParam);
				return;
			}
			if (keys == Keys.Up || keys == Keys.Down)
			{
				break;
			}
			goto case Msg.WM_CHAR;
		}
		case Msg.WM_CHAR:
			if (!ProcessKeyMessage(ref m) && textbox_ctrl != null)
			{
				XplatUI.SendMessage(textbox_ctrl.Handle, (Msg)m.Msg, m.WParam, m.LParam);
			}
			return;
		case Msg.WM_MOUSELEAVE:
		{
			Point pt = PointToClient(Control.MousePosition);
			if (base.ClientRectangle.Contains(pt))
			{
				return;
			}
			break;
		}
		}
		base.WndProc(ref m);
	}

	private void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e)
	{
		if (auto_complete_source != AutoCompleteSource.CustomSource)
		{
		}
	}

	private void LayoutComboBox()
	{
		int width = ThemeEngine.Current.Border3DSize.Width;
		text_area = base.ClientRectangle;
		text_area.Height = PreferredHeight;
		listbox_area = base.ClientRectangle;
		listbox_area.Y = text_area.Bottom + 3;
		listbox_area.Height -= text_area.Height + 2;
		Rectangle rectangle = button_area;
		if (DropDownStyle == ComboBoxStyle.Simple)
		{
			button_area = Rectangle.Empty;
		}
		else
		{
			button_area = text_area;
			button_area.X = text_area.Right - 16 - width;
			button_area.Y = text_area.Y + width;
			button_area.Width = 16;
			button_area.Height = text_area.Height - 2 * width;
			if (flat_style == FlatStyle.Popup || flat_style == FlatStyle.Flat)
			{
				button_area.Inflate(1, 1);
				button_area.X += 2;
				button_area.Width -= 2;
			}
		}
		if (button_area != rectangle)
		{
			rectangle.Y -= width;
			rectangle.Width += width;
			rectangle.Height += 2 * width;
			Invalidate(rectangle);
			Invalidate(button_area);
		}
		if (textbox_ctrl != null)
		{
			int num = width + 1;
			textbox_ctrl.Location = new Point(text_area.X + num, text_area.Y + num);
			textbox_ctrl.Width = text_area.Width - button_area.Width - num * 2;
			textbox_ctrl.Height = text_area.Height - num * 2;
		}
		if (listbox_ctrl != null && dropdown_style == ComboBoxStyle.Simple)
		{
			listbox_ctrl.Location = listbox_area.Location;
			listbox_ctrl.CalcListBoxArea();
		}
	}

	private void CreateComboListBox()
	{
		listbox_ctrl = new ComboListBox(this);
		listbox_ctrl.HighlightedIndex = SelectedIndex;
	}

	internal void Draw(Rectangle clip, Graphics dc)
	{
		Theme current = ThemeEngine.Current;
		FlatStyle flatStyle = FlatStyle.Standard;
		bool flag = false;
		flatStyle = FlatStyle;
		flag = flatStyle == FlatStyle.Flat || flatStyle == FlatStyle.Popup;
		current.ComboBoxDrawBackground(this, dc, clip, flatStyle);
		int width = current.Border3DSize.Width;
		if (dropdown_style == ComboBoxStyle.DropDownList)
		{
			DrawItemState drawItemState = DrawItemState.None;
			Color backColor = BackColor;
			Color foreColor = ForeColor;
			Rectangle rect = text_area;
			rect.X += width;
			rect.Y += width;
			rect.Width -= button_area.Width + 2 * width;
			rect.Height -= 2 * width;
			if (Focused)
			{
				drawItemState = DrawItemState.Selected;
				drawItemState |= DrawItemState.Focus;
				backColor = SystemColors.Highlight;
				foreColor = SystemColors.HighlightText;
			}
			drawItemState |= DrawItemState.ComboBoxEdit;
			HandleDrawItem(new DrawItemEventArgs(dc, Font, rect, SelectedIndex, drawItemState, foreColor, backColor));
		}
		if (show_dropdown_button)
		{
			ButtonState state = ((!is_enabled) ? ButtonState.Inactive : button_state);
			if (flag || current.ComboBoxNormalDropDownButtonHasTransparentBackground(this, state))
			{
				dc.FillRectangle(current.ResPool.GetSolidBrush(current.ColorControl), button_area);
			}
			if (flag)
			{
				current.DrawFlatStyleComboButton(dc, button_area, state);
			}
			else
			{
				current.ComboBoxDrawNormalDropDownButton(this, dc, clip, button_area, state);
			}
		}
	}

	internal void DropDownListBox()
	{
		DropDownButtonEntered = false;
		if (DropDownStyle != 0)
		{
			if (listbox_ctrl == null)
			{
				CreateComboListBox();
			}
			listbox_ctrl.Location = PointToScreen(new Point(text_area.X, text_area.Y + text_area.Height));
			FindMatchOrSetIndex(SelectedIndex);
			if (textbox_ctrl != null)
			{
				textbox_ctrl.HideAutoCompleteList();
			}
			if (listbox_ctrl.ShowWindow())
			{
				dropped_down = true;
			}
			button_state = ButtonState.Pushed;
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				Invalidate(text_area);
			}
		}
	}

	internal void DropDownListBoxFinished()
	{
		if (DropDownStyle != 0)
		{
			FindMatchOrSetIndex(SelectedIndex);
			button_state = ButtonState.Normal;
			Invalidate(button_area);
			dropped_down = false;
			OnDropDownClosed(EventArgs.Empty);
			if (listbox_ctrl != null)
			{
				listbox_ctrl.Dispose();
				listbox_ctrl = null;
			}
			if (textbox_ctrl != null)
			{
				textbox_ctrl.HideAutoCompleteList();
			}
		}
	}

	private int FindStringCaseInsensitive(string search)
	{
		if (search.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < Items.Count; i++)
		{
			if (string.Compare(GetItemText(Items[i]), 0, search, 0, search.Length, ignoreCase: true) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	internal int FindStringCaseInsensitive(string search, int start_index)
	{
		if (search.Length == 0)
		{
			return -1;
		}
		if (start_index < 0 || start_index > Items.Count)
		{
			throw new ArgumentOutOfRangeException("start_index");
		}
		for (int i = 0; i < Items.Count; i++)
		{
			int num = (i + start_index) % Items.Count;
			if (string.Compare(GetItemText(Items[num]), 0, search, 0, search.Length, ignoreCase: true) == 0)
			{
				return num;
			}
		}
		return -1;
	}

	internal override bool IsInputCharInternal(char charCode)
	{
		return true;
	}

	internal void RestoreContextMenu()
	{
		textbox_ctrl.RestoreContextMenu();
	}

	private void OnKeyDownCB(object sender, KeyEventArgs e)
	{
		if (Items.Count == 0)
		{
			return;
		}
		switch (e.KeyCode)
		{
		case Keys.Up:
			FindMatchOrSetIndex(Math.Max(SelectedIndex - 1, 0));
			if (DroppedDown && SelectedIndex < listbox_ctrl.FirstVisibleItem())
			{
				listbox_ctrl.Scroll(SelectedIndex - listbox_ctrl.FirstVisibleItem());
			}
			break;
		case Keys.Down:
			if ((e.Modifiers & Keys.Alt) == Keys.Alt)
			{
				DropDownListBox();
			}
			else
			{
				FindMatchOrSetIndex(Math.Min(SelectedIndex + 1, Items.Count - 1));
			}
			if (DroppedDown && SelectedIndex >= listbox_ctrl.LastVisibleItem())
			{
				listbox_ctrl.Scroll(SelectedIndex - listbox_ctrl.LastVisibleItem() + 1);
			}
			break;
		case Keys.PageUp:
		{
			int num = ((listbox_ctrl != null) ? (listbox_ctrl.page_size - 1) : (MaxDropDownItems - 1));
			if (num < 1)
			{
				num = 1;
			}
			SetSelectedIndex(Math.Max(SelectedIndex - num, 0), supressAutoScroll: true);
			if (DroppedDown && SelectedIndex < listbox_ctrl.FirstVisibleItem())
			{
				listbox_ctrl.Scroll(SelectedIndex - listbox_ctrl.FirstVisibleItem());
			}
			break;
		}
		case Keys.PageDown:
		{
			if (SelectedIndex == -1)
			{
				SelectedIndex = 0;
				if (dropdown_style != 0)
				{
					break;
				}
			}
			int num = ((listbox_ctrl != null) ? (listbox_ctrl.page_size - 1) : (MaxDropDownItems - 1));
			if (num < 1)
			{
				num = 1;
			}
			SetSelectedIndex(Math.Min(SelectedIndex + num, Items.Count - 1), supressAutoScroll: true);
			if (DroppedDown && SelectedIndex >= listbox_ctrl.LastVisibleItem())
			{
				listbox_ctrl.Scroll(SelectedIndex - listbox_ctrl.LastVisibleItem() + 1);
			}
			break;
		}
		case Keys.Return:
		case Keys.Escape:
			if (listbox_ctrl != null && listbox_ctrl.Visible)
			{
				DropDownListBoxFinished();
			}
			break;
		case Keys.Home:
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				SelectedIndex = 0;
				if (DroppedDown && SelectedIndex < listbox_ctrl.FirstVisibleItem())
				{
					listbox_ctrl.Scroll(SelectedIndex - listbox_ctrl.FirstVisibleItem());
				}
			}
			break;
		case Keys.End:
			if (dropdown_style == ComboBoxStyle.DropDownList)
			{
				SetSelectedIndex(Items.Count - 1, supressAutoScroll: true);
				if (DroppedDown && SelectedIndex >= listbox_ctrl.LastVisibleItem())
				{
					listbox_ctrl.Scroll(SelectedIndex - listbox_ctrl.LastVisibleItem() + 1);
				}
			}
			break;
		}
	}

	private void SetSelectedIndex(int value, bool supressAutoScroll)
	{
		if (selected_index == value)
		{
			return;
		}
		if (value <= -2 || value >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("SelectedIndex");
		}
		selected_index = value;
		if (dropdown_style != ComboBoxStyle.DropDownList)
		{
			if (value == -1)
			{
				SetControlText(string.Empty, suppressTextChanged: false, supressAutoScroll);
			}
			else
			{
				SetControlText(GetItemText(Items[value]), suppressTextChanged: false, supressAutoScroll);
			}
		}
		if (DropDownStyle == ComboBoxStyle.DropDownList)
		{
			Invalidate();
		}
		if (listbox_ctrl != null)
		{
			listbox_ctrl.HighlightedIndex = value;
		}
		OnSelectedValueChanged(EventArgs.Empty);
		OnSelectedIndexChanged(EventArgs.Empty);
		OnSelectedItemChanged(EventArgs.Empty);
	}

	private void FindMatchOrSetIndex(int index)
	{
		int num = -1;
		if (SelectedIndex == -1 && Text.Length != 0)
		{
			num = FindStringCaseInsensitive(Text);
		}
		if (num != -1)
		{
			SetSelectedIndex(num, supressAutoScroll: true);
		}
		else
		{
			SetSelectedIndex(index, supressAutoScroll: true);
		}
	}

	private void OnMouseDownCB(object sender, MouseEventArgs e)
	{
		if (((DropDownStyle != ComboBoxStyle.DropDownList) ? button_area : base.ClientRectangle).Contains(e.X, e.Y))
		{
			if (Items.Count > 0)
			{
				DropDownListBox();
			}
			else
			{
				button_state = ButtonState.Pushed;
				OnDropDown(EventArgs.Empty);
			}
			Invalidate(button_area);
			Update();
		}
		base.Capture = true;
	}

	private void OnMouseEnter(object sender, EventArgs e)
	{
		if (ThemeEngine.Current.CombBoxBackgroundHasHotElementStyle(this))
		{
			Invalidate();
		}
	}

	private void OnMouseLeave(object sender, EventArgs e)
	{
		if (ThemeEngine.Current.CombBoxBackgroundHasHotElementStyle(this))
		{
			drop_down_button_entered = false;
			Invalidate();
		}
		else if (show_dropdown_button)
		{
			DropDownButtonEntered = false;
		}
	}

	private void OnMouseMoveCB(object sender, MouseEventArgs e)
	{
		if (show_dropdown_button && !dropped_down)
		{
			DropDownButtonEntered = button_area.Contains(e.Location);
		}
		if (DropDownStyle != 0 && listbox_ctrl != null && listbox_ctrl.Visible)
		{
			Point pt = listbox_ctrl.PointToClient(Control.MousePosition);
			if (listbox_ctrl.ClientRectangle.Contains(pt))
			{
				listbox_ctrl.Capture = true;
			}
		}
	}

	private void OnMouseUpCB(object sender, MouseEventArgs e)
	{
		base.Capture = false;
		button_state = ButtonState.Normal;
		Invalidate(button_area);
		OnClick(EventArgs.Empty);
		if (dropped_down)
		{
			listbox_ctrl.Capture = true;
		}
	}

	private void OnMouseWheelCB(object sender, MouseEventArgs me)
	{
		if (Items.Count == 0)
		{
			return;
		}
		if (listbox_ctrl != null && listbox_ctrl.Visible)
		{
			int num = me.Delta / 120 * SystemInformation.MouseWheelScrollLines;
			listbox_ctrl.Scroll(-num);
			return;
		}
		int num2 = me.Delta / 120;
		int num3 = SelectedIndex - num2;
		if (num3 < 0)
		{
			num3 = 0;
		}
		else if (num3 >= Items.Count)
		{
			num3 = Items.Count - 1;
		}
		SelectedIndex = num3;
	}

	internal override void OnPaintInternal(PaintEventArgs pevent)
	{
		if (!suspend_ctrlupdate)
		{
			Draw(base.ClientRectangle, pevent.Graphics);
		}
	}

	private void OnTextBoxClick(object sender, EventArgs e)
	{
		OnClick(e);
	}

	private void OnTextChangedEdit(object sender, EventArgs e)
	{
		if (!process_textchanged_event)
		{
			return;
		}
		int num = FindStringCaseInsensitive(textbox_ctrl.Text);
		if (num == -1)
		{
			OnTextChanged(EventArgs.Empty);
			return;
		}
		if (listbox_ctrl != null && process_texchanged_autoscroll)
		{
			listbox_ctrl.EnsureTop(num);
		}
		base.Text = textbox_ctrl.Text;
	}

	private void OnTextKeyPress(object sender, KeyPressEventArgs e)
	{
		selected_index = -1;
		if (listbox_ctrl != null)
		{
			listbox_ctrl.HighlightedIndex = -1;
		}
	}

	internal void SetControlText(string s, bool suppressTextChanged)
	{
		SetControlText(s, suppressTextChanged, supressAutoScroll: false);
	}

	internal void SetControlText(string s, bool suppressTextChanged, bool supressAutoScroll)
	{
		if (suppressTextChanged)
		{
			process_textchanged_event = false;
		}
		if (supressAutoScroll)
		{
			process_texchanged_autoscroll = false;
		}
		textbox_ctrl.Text = s;
		textbox_ctrl.SelectAll();
		process_textchanged_event = true;
		process_texchanged_autoscroll = true;
	}

	private void UpdateComboBoxBounds()
	{
		if (requested_height != -1)
		{
			int num = requested_height;
			SetBounds(bounds.X, bounds.Y, bounds.Width, SnapHeight(requested_height), BoundsSpecified.Height);
			requested_height = num;
		}
	}

	private int SnapHeight(int height)
	{
		if (DropDownStyle == ComboBoxStyle.Simple && height > PreferredHeight)
		{
			if (IntegralHeight)
			{
				int height2 = ThemeEngine.Current.Border3DSize.Height;
				int num = height - PreferredHeight - 2 - height2 * 2;
				if (num > ItemHeight)
				{
					int num2 = num % ItemHeight;
					height -= num2;
				}
				else if (num < ItemHeight)
				{
					height = PreferredHeight;
				}
			}
		}
		else
		{
			height = PreferredHeight;
		}
		return height;
	}

	private void UpdatedItems()
	{
		if (listbox_ctrl != null)
		{
			listbox_ctrl.UpdateLastVisibleItem();
			listbox_ctrl.CalcListBoxArea();
			listbox_ctrl.Refresh();
		}
	}
}
