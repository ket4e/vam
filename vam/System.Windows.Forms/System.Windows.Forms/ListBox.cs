using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.ListBoxDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultProperty("Items")]
[DefaultEvent("SelectedIndexChanged")]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultBindingProperty("SelectedValue")]
public class ListBox : ListControl
{
	internal enum ItemNavigation
	{
		First,
		Last,
		Next,
		Previous,
		NextPage,
		PreviousPage,
		PreviousColumn,
		NextColumn
	}

	public class IntegerCollection : ICollection, IEnumerable, IList
	{
		private ListBox owner;

		private List<int> list;

		bool IList.IsFixedSize => false;

		bool IList.IsReadOnly => false;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (int)value;
			}
		}

		bool ICollection.IsSynchronized => true;

		object ICollection.SyncRoot => this;

		[Browsable(false)]
		public int Count => list.Count;

		public int this[int index]
		{
			get
			{
				return list[index];
			}
			set
			{
				list[index] = value;
				owner.CalculateTabStops();
			}
		}

		public IntegerCollection(ListBox owner)
		{
			this.owner = owner;
			list = new List<int>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		int IList.Add(object item)
		{
			int? num = item as int?;
			if (!num.HasValue)
			{
				throw new ArgumentException("item");
			}
			return Add(num.Value);
		}

		void IList.Clear()
		{
			Clear();
		}

		bool IList.Contains(object item)
		{
			int? num = item as int?;
			if (!num.HasValue)
			{
				return false;
			}
			return Contains(num.Value);
		}

		int IList.IndexOf(object item)
		{
			int? num = item as int?;
			if (!num.HasValue)
			{
				return -1;
			}
			return IndexOf(num.Value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "No items can be inserted into {0}, since it is a sorted collection.", GetType()));
		}

		void IList.Remove(object value)
		{
			int? num = value as int?;
			if (!num.HasValue)
			{
				throw new ArgumentException("value");
			}
			Remove(num.Value);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		public int Add(int item)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
				list.Sort();
				owner.CalculateTabStops();
			}
			return list.IndexOf(item);
		}

		public void AddRange(int[] items)
		{
			AddItems(items);
		}

		public void AddRange(IntegerCollection value)
		{
			AddItems(value);
		}

		private void AddItems(IList items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (int item in items)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			list.Sort();
		}

		public void Clear()
		{
			list.Clear();
			owner.CalculateTabStops();
		}

		public bool Contains(int item)
		{
			return list.Contains(item);
		}

		public void CopyTo(Array destination, int index)
		{
			for (int i = 0; i < list.Count; i++)
			{
				destination.SetValue(list[i], index++);
			}
		}

		public int IndexOf(int item)
		{
			return list.IndexOf(item);
		}

		public void Remove(int item)
		{
			list.Remove(item);
			list.Sort();
			owner.CalculateTabStops();
		}

		public void RemoveAt(int index)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException();
			}
			list.RemoveAt(index);
			list.Sort();
			owner.CalculateTabStops();
		}
	}

	[ListBindable(false)]
	public class ObjectCollection : ICollection, IEnumerable, IList
	{
		internal class ListObjectComparer : IComparer
		{
			public int Compare(object a, object b)
			{
				string text = a.ToString();
				string strB = b.ToString();
				return text.CompareTo(strB);
			}
		}

		private ListBox owner;

		internal ArrayList object_items = new ArrayList();

		private static object UIACollectionChangedEvent;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => false;

		public int Count => object_items.Count;

		public bool IsReadOnly => false;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public virtual object this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("Index of out range");
				}
				return object_items[index];
			}
			set
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("Index of out range");
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, object_items[index]));
				object_items[index] = value;
				OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
				owner.CollectionChanged();
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

		public ObjectCollection(ListBox owner)
		{
			this.owner = owner;
		}

		public ObjectCollection(ListBox owner, object[] value)
		{
			this.owner = owner;
			AddRange(value);
		}

		public ObjectCollection(ListBox owner, ObjectCollection value)
		{
			this.owner = owner;
			AddRange(value);
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
			int result = AddItem(item);
			owner.CollectionChanged();
			if (owner.sorted)
			{
				return IndexOf(item);
			}
			return result;
		}

		public void AddRange(object[] items)
		{
			AddItems(items);
		}

		public void AddRange(ObjectCollection value)
		{
			AddItems(value);
		}

		internal void AddItems(IList items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (object item in items)
			{
				AddItem(item);
			}
			owner.CollectionChanged();
		}

		public virtual void Clear()
		{
			owner.selected_indices.ClearCore();
			object_items.Clear();
			owner.CollectionChanged();
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
				throw new ArgumentOutOfRangeException("Index of out range");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			owner.BeginUpdate();
			object_items.Insert(index, item);
			owner.CollectionChanged();
			owner.EndUpdate();
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}

		public void Remove(object value)
		{
			if (value != null)
			{
				int num = IndexOf(value);
				if (num != -1)
				{
					RemoveAt(num);
				}
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("Index of out range");
			}
			object element = object_items[index];
			UpdateSelection(index);
			object_items.RemoveAt(index);
			owner.CollectionChanged();
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
		}

		internal int AddItem(object item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			int count = object_items.Count;
			object_items.Add(item);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
			return count;
		}

		private void UpdateSelection(int removed_index)
		{
			owner.selected_indices.Remove(removed_index);
			if (owner.selection_mode == SelectionMode.None)
			{
				return;
			}
			int num = object_items.Count - 1;
			if (owner.selected_indices.Contains(num))
			{
				owner.selected_indices.Remove(num);
				int num2 = num - 1;
				if (owner.selection_mode == SelectionMode.One && num2 > -1)
				{
					owner.selected_indices.Add(num2);
				}
			}
		}

		internal void Sort()
		{
			object_items.Sort(new ListObjectComparer());
		}
	}

	public class SelectedIndexCollection : ICollection, IEnumerable, IList
	{
		private ListBox owner;

		private ArrayList selection;

		private bool sorting_needed;

		private static object UIACollectionChangedEvent;

		bool ICollection.IsSynchronized => true;

		bool IList.IsFixedSize => true;

		object ICollection.SyncRoot => selection;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[Browsable(false)]
		public int Count => selection.Count;

		public bool IsReadOnly => true;

		public int this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("Index of out range");
				}
				CheckSorted();
				return (int)selection[index];
			}
		}

		internal ArrayList List
		{
			get
			{
				CheckSorted();
				return selection;
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

		public SelectedIndexCollection(ListBox owner)
		{
			this.owner = owner;
			selection = new ArrayList();
		}

		static SelectedIndexCollection()
		{
			UIACollectionChanged = new object();
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		bool IList.Contains(object selectedIndex)
		{
			return Contains((int)selectedIndex);
		}

		int IList.IndexOf(object selectedIndex)
		{
			return IndexOf((int)selectedIndex);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		internal void OnUIACollectionChangedEvent(CollectionChangeEventArgs args)
		{
			((CollectionChangeEventHandler)owner.Events[UIACollectionChanged])?.Invoke(owner, args);
		}

		public void Add(int index)
		{
			if (AddCore(index))
			{
				owner.OnSelectedIndexChanged(EventArgs.Empty);
				owner.OnSelectedValueChanged(EventArgs.Empty);
			}
		}

		internal bool AddCore(int index)
		{
			if (selection.Contains(index))
			{
				return false;
			}
			if (index == -1)
			{
				return false;
			}
			if (index < -1 || index >= owner.Items.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (owner.selection_mode == SelectionMode.None)
			{
				throw new InvalidOperationException("Cannot call this method when selection mode is SelectionMode.None");
			}
			if (owner.selection_mode == SelectionMode.One && Count > 0)
			{
				RemoveCore((int)selection[0]);
			}
			selection.Add(index);
			sorting_needed = true;
			owner.EnsureVisible(index);
			owner.FocusedItem = index;
			owner.InvalidateItem(index);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, index));
			return true;
		}

		public void Clear()
		{
			if (ClearCore())
			{
				owner.OnSelectedIndexChanged(EventArgs.Empty);
				owner.OnSelectedValueChanged(EventArgs.Empty);
			}
		}

		internal bool ClearCore()
		{
			if (selection.Count == 0)
			{
				return false;
			}
			foreach (int item in selection)
			{
				owner.InvalidateItem(item);
			}
			selection.Clear();
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, -1));
			return true;
		}

		public bool Contains(int selectedIndex)
		{
			foreach (int item in selection)
			{
				if (item == selectedIndex)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(Array destination, int index)
		{
			CheckSorted();
			selection.CopyTo(destination, index);
		}

		public IEnumerator GetEnumerator()
		{
			CheckSorted();
			return selection.GetEnumerator();
		}

		public void Remove(int index)
		{
			if (RemoveCore(index))
			{
				owner.OnSelectedIndexChanged(EventArgs.Empty);
				owner.OnSelectedValueChanged(EventArgs.Empty);
			}
		}

		internal bool RemoveCore(int index)
		{
			int num = IndexOf(index);
			if (num == -1)
			{
				return false;
			}
			selection.RemoveAt(num);
			owner.InvalidateItem(index);
			OnUIACollectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, index));
			return true;
		}

		public int IndexOf(int selectedIndex)
		{
			CheckSorted();
			for (int i = 0; i < selection.Count; i++)
			{
				if ((int)selection[i] == selectedIndex)
				{
					return i;
				}
			}
			return -1;
		}

		private void CheckSorted()
		{
			if (sorting_needed)
			{
				sorting_needed = false;
				selection.Sort();
			}
		}
	}

	public class SelectedObjectCollection : ICollection, IEnumerable, IList
	{
		private ListBox owner;

		bool ICollection.IsSynchronized => true;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => true;

		public int Count => owner.selected_indices.Count;

		public bool IsReadOnly => true;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public object this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("Index of out range");
				}
				return owner.items[owner.selected_indices[index]];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public SelectedObjectCollection(ListBox owner)
		{
			this.owner = owner;
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public void Add(object value)
		{
			if (owner.selection_mode == SelectionMode.None)
			{
				throw new ArgumentException("Cannot call this method if SelectionMode is SelectionMode.None");
			}
			int num = owner.items.IndexOf(value);
			if (num != -1)
			{
				owner.selected_indices.Add(num);
			}
		}

		public void Clear()
		{
			owner.selected_indices.Clear();
		}

		public bool Contains(object selectedObject)
		{
			int num = owner.items.IndexOf(selectedObject);
			return num != -1 && owner.selected_indices.Contains(num);
		}

		public void CopyTo(Array destination, int index)
		{
			for (int i = 0; i < Count; i++)
			{
				destination.SetValue(this[i], index++);
			}
		}

		public void Remove(object value)
		{
			if (value != null)
			{
				int num = owner.items.IndexOf(value);
				if (num != -1)
				{
					owner.selected_indices.Remove(num);
				}
			}
		}

		public int IndexOf(object selectedObject)
		{
			int num = owner.items.IndexOf(selectedObject);
			return (num != -1) ? owner.selected_indices.IndexOf(num) : (-1);
		}

		public IEnumerator GetEnumerator()
		{
			object[] array = new object[Count];
			for (int i = 0; i < Count; i++)
			{
				array[i] = owner.items[owner.selected_indices[i]];
			}
			return array.GetEnumerator();
		}
	}

	public const int DefaultItemHeight = 13;

	public const int NoMatches = -1;

	private Hashtable item_heights;

	private int item_height = -1;

	private int column_width;

	private int requested_height;

	private DrawMode draw_mode;

	private int horizontal_extent;

	private bool horizontal_scrollbar;

	private bool integral_height = true;

	private bool multicolumn;

	private bool scroll_always_visible;

	private SelectedIndexCollection selected_indices;

	private SelectedObjectCollection selected_items;

	private SelectionMode selection_mode = SelectionMode.One;

	private bool sorted;

	private bool use_tabstops = true;

	private int column_width_internal = 120;

	private ImplicitVScrollBar vscrollbar;

	private ImplicitHScrollBar hscrollbar;

	private int hbar_offset;

	private bool suspend_layout;

	private bool ctrl_pressed;

	private bool shift_pressed;

	private bool explicit_item_height;

	private int top_index;

	private int last_visible_index;

	private Rectangle items_area;

	private int focused_item = -1;

	private ObjectCollection items;

	private IntegerCollection custom_tab_offsets;

	private Padding padding;

	private bool use_custom_tab_offsets;

	private static object DrawItemEvent;

	private static object MeasureItemEvent;

	private static object SelectedIndexChangedEvent;

	private static object UIASelectionModeChangedEvent;

	private static object UIAFocusedItemChangedEvent;

	private int row_count = 1;

	private Size canvas_size;

	private int anchor = -1;

	private int[] prev_selection;

	private bool button_pressed;

	private Point button_pressed_loc = new Point(-1, -1);

	private StringFormat string_format;

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
				base.Refresh();
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
			base.BackgroundImage = value;
			base.Refresh();
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
			UpdateListBoxBounds();
		}
	}

	[DefaultValue(0)]
	[Localizable(true)]
	public int ColumnWidth
	{
		get
		{
			return column_width;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("A value less than zero is assigned to the property.");
			}
			column_width = value;
			if (value == 0)
			{
				ColumnWidthInternal = 120;
			}
			else
			{
				ColumnWidthInternal = value;
			}
			base.Refresh();
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public IntegerCollection CustomTabOffsets => custom_tab_offsets;

	protected override Size DefaultSize => new Size(120, 96);

	[DefaultValue(DrawMode.Normal)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public virtual DrawMode DrawMode
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
			if (value == DrawMode.OwnerDrawVariable && multicolumn)
			{
				throw new ArgumentException("Cannot have variable height and multicolumn");
			}
			if (draw_mode != value)
			{
				draw_mode = value;
				if (draw_mode == DrawMode.OwnerDrawVariable)
				{
					item_heights = new Hashtable();
				}
				else
				{
					item_heights = null;
				}
				if (base.Parent != null)
				{
					base.Parent.PerformLayout(this, "DrawMode");
				}
				base.Refresh();
			}
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
		}
	}

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
				base.Refresh();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(0)]
	public int HorizontalExtent
	{
		get
		{
			return horizontal_extent;
		}
		set
		{
			if (horizontal_extent != value)
			{
				horizontal_extent = value;
				base.Refresh();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(false)]
	public bool HorizontalScrollbar
	{
		get
		{
			return horizontal_scrollbar;
		}
		set
		{
			if (horizontal_scrollbar != value)
			{
				horizontal_scrollbar = value;
				UpdateScrollBars();
				base.Refresh();
			}
		}
	}

	[DefaultValue(true)]
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
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
				UpdateListBoxBounds();
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(13)]
	[Localizable(true)]
	public virtual int ItemHeight
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
			if (value > 255)
			{
				throw new ArgumentOutOfRangeException("The ItemHeight property was set beyond 255 pixels");
			}
			explicit_item_height = true;
			if (item_height != value)
			{
				item_height = value;
				if (IntegralHeight)
				{
					UpdateListBoxBounds();
				}
				LayoutListBox();
			}
		}
	}

	[MergableProperty(false)]
	[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Localizable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public ObjectCollection Items => items;

	[DefaultValue(false)]
	public bool MultiColumn
	{
		get
		{
			return multicolumn;
		}
		set
		{
			if (multicolumn != value)
			{
				if (value && DrawMode == DrawMode.OwnerDrawVariable)
				{
					throw new ArgumentException("A multicolumn ListBox cannot have a variable-sized height.");
				}
				multicolumn = value;
				LayoutListBox();
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new Padding Padding
	{
		get
		{
			return padding;
		}
		set
		{
			padding = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int PreferredHeight
	{
		get
		{
			int num = 0;
			if (draw_mode == DrawMode.Normal)
			{
				num = base.FontHeight * items.Count;
			}
			else if (draw_mode == DrawMode.OwnerDrawFixed)
			{
				num = ItemHeight * items.Count;
			}
			else if (draw_mode == DrawMode.OwnerDrawVariable)
			{
				for (int i = 0; i < items.Count; i++)
				{
					num += (int)item_heights[Items[i]];
				}
			}
			return num;
		}
	}

	public override RightToLeft RightToLeft
	{
		get
		{
			return base.RightToLeft;
		}
		set
		{
			base.RightToLeft = value;
			if (base.RightToLeft == RightToLeft.Yes)
			{
				StringFormat.Alignment = StringAlignment.Far;
			}
			else
			{
				StringFormat.Alignment = StringAlignment.Near;
			}
			base.Refresh();
		}
	}

	[DefaultValue(false)]
	[Localizable(true)]
	public bool ScrollAlwaysVisible
	{
		get
		{
			return scroll_always_visible;
		}
		set
		{
			if (scroll_always_visible != value)
			{
				scroll_always_visible = value;
				UpdateScrollBars();
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Bindable(true)]
	[Browsable(false)]
	public override int SelectedIndex
	{
		get
		{
			if (selected_indices == null)
			{
				return -1;
			}
			return (selected_indices.Count <= 0) ? (-1) : selected_indices[0];
		}
		set
		{
			if (value < -1 || value >= Items.Count)
			{
				throw new ArgumentOutOfRangeException("Index of out range");
			}
			if (SelectionMode == SelectionMode.None)
			{
				throw new ArgumentException("cannot call this method if SelectionMode is SelectionMode.None");
			}
			if (value == -1)
			{
				selected_indices.Clear();
			}
			else
			{
				selected_indices.Add(value);
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public SelectedIndexCollection SelectedIndices => selected_indices;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Bindable(true)]
	[Browsable(false)]
	public object SelectedItem
	{
		get
		{
			if (SelectedItems.Count > 0)
			{
				return SelectedItems[0];
			}
			return null;
		}
		set
		{
			if (value == null || Items.Contains(value))
			{
				SelectedIndex = ((value != null) ? Items.IndexOf(value) : (-1));
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public SelectedObjectCollection SelectedItems => selected_items;

	[DefaultValue(SelectionMode.One)]
	public virtual SelectionMode SelectionMode
	{
		get
		{
			return selection_mode;
		}
		set
		{
			if (!Enum.IsDefined(typeof(SelectionMode), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for SelectionMode");
			}
			if (selection_mode == value)
			{
				return;
			}
			selection_mode = value;
			switch (selection_mode)
			{
			case SelectionMode.None:
				SelectedIndices.Clear();
				break;
			case SelectionMode.One:
			{
				ArrayList arrayList = (ArrayList)SelectedIndices.List.Clone();
				for (int i = 1; i < arrayList.Count; i++)
				{
					SelectedIndices.Remove((int)arrayList[i]);
				}
				break;
			}
			}
			OnUIASelectionModeChangedEvent();
		}
	}

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
				if (sorted)
				{
					Sort();
				}
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Bindable(false)]
	[Browsable(false)]
	public override string Text
	{
		get
		{
			if (SelectionMode != 0 && SelectedIndex != -1)
			{
				return GetItemText(SelectedItem);
			}
			return base.Text;
		}
		set
		{
			base.Text = value;
			if (SelectionMode != 0)
			{
				int num = FindStringExact(value);
				if (num != -1)
				{
					SelectedIndex = num;
				}
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public int TopIndex
	{
		get
		{
			return top_index;
		}
		set
		{
			if (value != top_index && value >= 0 && value < Items.Count)
			{
				int num = items_area.Height / ItemHeight;
				if (Items.Count < num)
				{
					value = 0;
				}
				else if (!multicolumn)
				{
					top_index = Math.Min(value, Items.Count - num);
				}
				else
				{
					top_index = value;
				}
				UpdateTopItem();
				base.Refresh();
			}
		}
	}

	[Browsable(false)]
	[DefaultValue(false)]
	public bool UseCustomTabOffsets
	{
		get
		{
			return use_custom_tab_offsets;
		}
		set
		{
			if (use_custom_tab_offsets != value)
			{
				use_custom_tab_offsets = value;
				CalculateTabStops();
			}
		}
	}

	[DefaultValue(true)]
	public bool UseTabStops
	{
		get
		{
			return use_tabstops;
		}
		set
		{
			if (use_tabstops != value)
			{
				use_tabstops = value;
				CalculateTabStops();
			}
		}
	}

	protected override bool AllowSelection => SelectionMode != SelectionMode.None;

	private int ColumnWidthInternal
	{
		get
		{
			return column_width_internal;
		}
		set
		{
			column_width_internal = value;
		}
	}

	private int RowCount => (!MultiColumn) ? Items.Count : row_count;

	internal ScrollBar UIAHScrollBar => hscrollbar;

	internal ScrollBar UIAVScrollBar => vscrollbar;

	internal int FocusedItem
	{
		get
		{
			return focused_item;
		}
		set
		{
			if (focused_item == value)
			{
				return;
			}
			int num = focused_item;
			focused_item = value;
			if (has_focus)
			{
				if (num != -1)
				{
					InvalidateItem(num);
				}
				if (value != -1)
				{
					InvalidateItem(value);
				}
				OnUIAFocusedItemChangedEvent();
			}
		}
	}

	internal StringFormat StringFormat
	{
		get
		{
			if (string_format == null)
			{
				string_format = new StringFormat();
				string_format.FormatFlags = StringFormatFlags.NoWrap;
				if (RightToLeft == RightToLeft.Yes)
				{
					string_format.Alignment = StringAlignment.Far;
				}
				else
				{
					string_format.Alignment = StringAlignment.Near;
				}
				CalculateTabStops();
			}
			return string_format;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public new event EventHandler Click
	{
		add
		{
			base.Click += value;
		}
		remove
		{
			base.Click -= value;
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

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public new event MouseEventHandler MouseClick
	{
		add
		{
			base.MouseClick += value;
		}
		remove
		{
			base.MouseClick -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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
	[EditorBrowsable(EditorBrowsableState.Advanced)]
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

	internal event EventHandler UIASelectionModeChanged
	{
		add
		{
			base.Events.AddHandler(UIASelectionModeChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIASelectionModeChangedEvent, value);
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

	public ListBox()
	{
		items = CreateItemCollection();
		selected_indices = new SelectedIndexCollection(this);
		selected_items = new SelectedObjectCollection(this);
		requested_height = bounds.Height;
		base.InternalBorderStyle = BorderStyle.Fixed3D;
		BackColor = ThemeEngine.Current.ColorWindow;
		vscrollbar = new ImplicitVScrollBar();
		vscrollbar.Minimum = 0;
		vscrollbar.SmallChange = 1;
		vscrollbar.LargeChange = 1;
		vscrollbar.Maximum = 0;
		vscrollbar.ValueChanged += VerticalScrollEvent;
		vscrollbar.Visible = false;
		hscrollbar = new ImplicitHScrollBar();
		hscrollbar.Minimum = 0;
		hscrollbar.SmallChange = 1;
		hscrollbar.LargeChange = 1;
		hscrollbar.Maximum = 0;
		hscrollbar.Visible = false;
		hscrollbar.ValueChanged += HorizontalScrollEvent;
		base.Controls.AddImplicit(vscrollbar);
		base.Controls.AddImplicit(hscrollbar);
		base.MouseDown += OnMouseDownLB;
		base.MouseMove += OnMouseMoveLB;
		base.MouseUp += OnMouseUpLB;
		base.MouseWheel += OnMouseWheelLB;
		base.KeyUp += OnKeyUpLB;
		base.GotFocus += OnGotFocus;
		base.LostFocus += OnLostFocus;
		SetStyle(ControlStyles.UserPaint, value: false);
		custom_tab_offsets = new IntegerCollection(this);
	}

	static ListBox()
	{
		DrawItem = new object();
		MeasureItem = new object();
		SelectedIndexChanged = new object();
		UIASelectionModeChanged = new object();
		UIAFocusedItemChanged = new object();
	}

	internal void OnUIASelectionModeChangedEvent()
	{
		((EventHandler)base.Events[UIASelectionModeChanged])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIAFocusedItemChangedEvent()
	{
		((EventHandler)base.Events[UIAFocusedItemChanged])?.Invoke(this, EventArgs.Empty);
	}

	[Obsolete("this method has been deprecated")]
	protected virtual void AddItemsCore(object[] value)
	{
		Items.AddRange(value);
	}

	public void BeginUpdate()
	{
		suspend_layout = true;
	}

	public void ClearSelected()
	{
		selected_indices.Clear();
	}

	protected virtual ObjectCollection CreateItemCollection()
	{
		return new ObjectCollection(this);
	}

	public void EndUpdate()
	{
		suspend_layout = false;
		LayoutListBox();
		base.Refresh();
	}

	public int FindString(string s)
	{
		return FindString(s, -1);
	}

	public int FindString(string s, int startIndex)
	{
		if (Items.Count == 0)
		{
			return -1;
		}
		if (startIndex < -1 || startIndex >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("Index of out range");
		}
		startIndex = ((startIndex != Items.Count - 1) ? (startIndex + 1) : 0);
		int num = startIndex;
		do
		{
			string itemText = GetItemText(Items[num]);
			if (CultureInfo.CurrentCulture.CompareInfo.IsPrefix(itemText, s, CompareOptions.IgnoreCase))
			{
				return num;
			}
			num = ((num != Items.Count - 1) ? (num + 1) : 0);
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
		if (Items.Count == 0)
		{
			return -1;
		}
		if (startIndex < -1 || startIndex >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("Index of out range");
		}
		startIndex = ((startIndex + 1 != Items.Count) ? (startIndex + 1) : 0);
		int num = startIndex;
		do
		{
			if (string.Compare(GetItemText(Items[num]), s, ignoreCase: true) == 0)
			{
				return num;
			}
			num = ((num + 1 != Items.Count) ? (num + 1) : 0);
		}
		while (num != startIndex);
		return -1;
	}

	public int GetItemHeight(int index)
	{
		if (index < 0 || index >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("Index of out range");
		}
		if (DrawMode == DrawMode.OwnerDrawVariable && base.IsHandleCreated)
		{
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

	public Rectangle GetItemRectangle(int index)
	{
		if (index < 0 || index >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("GetItemRectangle index out of range.");
		}
		Rectangle result = default(Rectangle);
		if (MultiColumn)
		{
			int num = index / RowCount;
			int num2 = index;
			if (num2 < 0)
			{
				num2 += RowCount * (top_index / RowCount);
			}
			result.Y = num2 % RowCount * ItemHeight;
			result.X = (num - top_index / RowCount) * ColumnWidthInternal;
			result.Height = ItemHeight;
			result.Width = ColumnWidthInternal;
		}
		else
		{
			result.X = 0;
			result.Height = GetItemHeight(index);
			result.Width = items_area.Width;
			if (DrawMode == DrawMode.OwnerDrawVariable)
			{
				result.Y = 0;
				if (index >= top_index)
				{
					for (int i = top_index; i < index; i++)
					{
						result.Y += GetItemHeight(i);
					}
				}
				else
				{
					for (int j = index; j < top_index; j++)
					{
						result.Y -= GetItemHeight(j);
					}
				}
			}
			else
			{
				result.Y = ItemHeight * (index - top_index);
			}
		}
		if (this is CheckedListBox)
		{
			result.Width += 15;
		}
		return result;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
	{
		bounds.Height = requested_height;
		return base.GetScaledBounds(bounds, factor, specified);
	}

	public bool GetSelected(int index)
	{
		if (index < 0 || index >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("Index of out range");
		}
		return SelectedIndices.Contains(index);
	}

	public int IndexFromPoint(Point p)
	{
		return IndexFromPoint(p.X, p.Y);
	}

	public int IndexFromPoint(int x, int y)
	{
		if (Items.Count == 0)
		{
			return -1;
		}
		for (int i = top_index; i <= last_visible_index; i++)
		{
			if (GetItemRectangle(i).Contains(x, y))
			{
				return i;
			}
		}
		return -1;
	}

	protected override void OnChangeUICues(UICuesEventArgs e)
	{
		base.OnChangeUICues(e);
	}

	protected override void OnDataSourceChanged(EventArgs e)
	{
		base.OnDataSourceChanged(e);
		BindDataItems();
		if (base.DataSource == null || base.DataManager == null)
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
		if (base.DataManager != null && base.IsHandleCreated)
		{
			BindDataItems();
			base.Refresh();
		}
	}

	protected virtual void OnDrawItem(DrawItemEventArgs e)
	{
		DrawMode drawMode = DrawMode;
		if (drawMode == DrawMode.OwnerDrawFixed || drawMode == DrawMode.OwnerDrawVariable)
		{
			((DrawItemEventHandler)base.Events[DrawItem])?.Invoke(this, e);
		}
		else
		{
			ThemeEngine.Current.DrawListBoxItem(this, e);
		}
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		if (use_tabstops)
		{
			StringFormat.SetTabStops(0f, new float[1] { (float)((double)Font.Height * 3.7) });
		}
		if (explicit_item_height)
		{
			base.Refresh();
			return;
		}
		item_height = (int)TextRenderer.MeasureString("The quick brown Fox", Font).Height;
		if (IntegralHeight)
		{
			UpdateListBoxBounds();
		}
		LayoutListBox();
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		if (IntegralHeight)
		{
			UpdateListBoxBounds();
		}
		LayoutListBox();
		EnsureVisible(focused_item);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected virtual void OnMeasureItem(MeasureItemEventArgs e)
	{
		if (draw_mode == DrawMode.OwnerDrawVariable)
		{
			((MeasureItemEventHandler)base.Events[MeasureItem])?.Invoke(this, e);
		}
	}

	protected override void OnParentChanged(EventArgs e)
	{
		base.OnParentChanged(e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		if (canvas_size.IsEmpty || MultiColumn)
		{
			LayoutListBox();
		}
		Invalidate();
	}

	protected override void OnSelectedIndexChanged(EventArgs e)
	{
		base.OnSelectedIndexChanged(e);
		((EventHandler)base.Events[SelectedIndexChanged])?.Invoke(this, e);
	}

	protected override void OnSelectedValueChanged(EventArgs e)
	{
		base.OnSelectedValueChanged(e);
	}

	public override void Refresh()
	{
		if (draw_mode == DrawMode.OwnerDrawVariable)
		{
			item_heights.Clear();
		}
		base.Refresh();
	}

	protected override void RefreshItem(int index)
	{
		if (index < 0 || index >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("Index of out range");
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
		LayoutListBox();
		Refresh();
	}

	public override void ResetBackColor()
	{
		base.ResetBackColor();
	}

	public override void ResetForeColor()
	{
		base.ResetForeColor();
	}

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		base.ScaleControl(factor, specified);
	}

	private int SnapHeightToIntegral(int height)
	{
		int num = border_style switch
		{
			BorderStyle.Fixed3D => ThemeEngine.Current.Border3DSize.Height, 
			BorderStyle.FixedSingle => ThemeEngine.Current.BorderSize.Height, 
			_ => 0, 
		};
		height -= 2 * num;
		height -= height % ItemHeight;
		height += 2 * num;
		return height;
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height)
		{
			requested_height = height;
		}
		if (IntegralHeight && base.IsHandleCreated)
		{
			height = SnapHeightToIntegral(height);
		}
		base.SetBoundsCore(x, y, width, height, specified);
		UpdateScrollBars();
		last_visible_index = LastVisibleItem();
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
			Items.AddItems(value);
		}
		finally
		{
			EndUpdate();
		}
	}

	public void SetSelected(int index, bool value)
	{
		if (index < 0 || index >= Items.Count)
		{
			throw new ArgumentOutOfRangeException("Index of out range");
		}
		if (SelectionMode == SelectionMode.None)
		{
			throw new InvalidOperationException();
		}
		if (value)
		{
			SelectedIndices.Add(index);
		}
		else
		{
			SelectedIndices.Remove(index);
		}
	}

	protected virtual void Sort()
	{
		Sort(paint: true);
	}

	private void Sort(bool paint)
	{
		if (Items.Count != 0)
		{
			Items.Sort();
			if (paint)
			{
				base.Refresh();
			}
		}
	}

	public override string ToString()
	{
		return base.ToString();
	}

	protected virtual void WmReflectCommand(ref Message m)
	{
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 256)
		{
			if (ProcessKeyMessage(ref m))
			{
				m.Result = IntPtr.Zero;
				return;
			}
			HandleKeyDown((Keys)m.WParam.ToInt32());
			DefWndProc(ref m);
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	private void CalculateTabStops()
	{
		if (use_tabstops)
		{
			if (use_custom_tab_offsets)
			{
				float[] array = new float[custom_tab_offsets.Count];
				custom_tab_offsets.CopyTo(array, 0);
				StringFormat.SetTabStops(0f, array);
			}
			else
			{
				StringFormat.SetTabStops(0f, new float[1] { (float)((double)Font.Height * 3.7) });
			}
		}
		else
		{
			StringFormat.SetTabStops(0f, new float[0]);
		}
		Invalidate();
	}

	private void LayoutListBox()
	{
		if (base.IsHandleCreated && !suspend_layout)
		{
			if (MultiColumn)
			{
				LayoutMultiColumn();
			}
			else
			{
				LayoutSingleColumn();
			}
			last_visible_index = LastVisibleItem();
			UpdateScrollBars();
		}
	}

	private void LayoutSingleColumn()
	{
		int num;
		int num2;
		switch (DrawMode)
		{
		case DrawMode.OwnerDrawVariable:
		{
			num = 0;
			num2 = HorizontalExtent;
			for (int j = 0; j < Items.Count; j++)
			{
				num += GetItemHeight(j);
			}
			break;
		}
		case DrawMode.OwnerDrawFixed:
			num = Items.Count * ItemHeight;
			num2 = HorizontalExtent;
			break;
		default:
		{
			num = Items.Count * ItemHeight;
			num2 = 0;
			for (int i = 0; i < Items.Count; i++)
			{
				int num3 = (int)TextRenderer.MeasureString(GetItemText(Items[i]), Font).Width;
				if (this is CheckedListBox)
				{
					num3 += 15;
				}
				if (num3 > num2)
				{
					num2 = num3;
				}
			}
			break;
		}
		}
		canvas_size = new Size(num2, num);
	}

	private void LayoutMultiColumn()
	{
		int num = base.ClientRectangle.Height - (ScrollAlwaysVisible ? hscrollbar.Height : 0);
		row_count = Math.Max(1, num / ItemHeight);
		int num2 = (int)Math.Ceiling((float)Items.Count / (float)row_count);
		Size size = new Size(num2 * ColumnWidthInternal, row_count * ItemHeight);
		if (!ScrollAlwaysVisible && size.Width > base.ClientRectangle.Width && row_count > 1)
		{
			num = base.ClientRectangle.Height - hscrollbar.Height;
			row_count = Math.Max(1, num / ItemHeight);
			num2 = (int)Math.Ceiling((float)Items.Count / (float)row_count);
			size = new Size(num2 * ColumnWidthInternal, row_count * ItemHeight);
		}
		canvas_size = size;
	}

	internal void Draw(Rectangle clip, Graphics dc)
	{
		Theme current = ThemeEngine.Current;
		if (hscrollbar.Visible && vscrollbar.Visible)
		{
			Rectangle rect = new Rectangle(hscrollbar.Right, vscrollbar.Bottom, vscrollbar.Width, hscrollbar.Height);
			if (rect.IntersectsWith(clip))
			{
				dc.FillRectangle(current.ResPool.GetSolidBrush(current.ColorControl), rect);
			}
		}
		dc.FillRectangle(current.ResPool.GetSolidBrush(BackColor), items_area);
		if (Items.Count == 0)
		{
			return;
		}
		for (int i = top_index; i <= last_visible_index; i++)
		{
			Rectangle itemDisplayRectangle = GetItemDisplayRectangle(i, top_index);
			if (clip.IntersectsWith(itemDisplayRectangle))
			{
				DrawItemState drawItemState = DrawItemState.None;
				if (SelectedIndices.Contains(i))
				{
					drawItemState |= DrawItemState.Selected;
				}
				if (has_focus && FocusedItem == i)
				{
					drawItemState |= DrawItemState.Focus;
				}
				if (!MultiColumn && hscrollbar != null && hscrollbar.Visible)
				{
					itemDisplayRectangle.X -= hscrollbar.Value;
					itemDisplayRectangle.Width += hscrollbar.Value;
				}
				Color foreColor = (((drawItemState & DrawItemState.Selected) == 0) ? ForeColor : ThemeEngine.Current.ColorHighlightText);
				OnDrawItem(new DrawItemEventArgs(dc, Font, itemDisplayRectangle, i, drawItemState, foreColor, BackColor));
			}
		}
	}

	internal Rectangle GetItemDisplayRectangle(int index, int first_displayble)
	{
		Rectangle itemRectangle = GetItemRectangle(first_displayble);
		Rectangle itemRectangle2 = GetItemRectangle(index);
		itemRectangle2.X -= itemRectangle.X;
		itemRectangle2.Y -= itemRectangle.Y;
		if (this is CheckedListBox)
		{
			itemRectangle2.Width -= 14;
		}
		return itemRectangle2;
	}

	private void HorizontalScrollEvent(object sender, EventArgs e)
	{
		if (multicolumn)
		{
			int num = top_index;
			int num2 = last_visible_index;
			top_index = RowCount * hscrollbar.Value;
			last_visible_index = LastVisibleItem();
			if (num != top_index || num2 != last_visible_index)
			{
				Invalidate(items_area);
			}
			return;
		}
		int num3 = hbar_offset;
		hbar_offset = hscrollbar.Value;
		if (hbar_offset < 0)
		{
			hbar_offset = 0;
		}
		if (base.IsHandleCreated)
		{
			XplatUI.ScrollWindow(Handle, items_area, num3 - hbar_offset, 0, with_children: false);
		}
	}

	private int IndexAtClientPoint(int x, int y)
	{
		if (Items.Count == 0)
		{
			return -1;
		}
		if (x < 0)
		{
			x = 0;
		}
		else if (x > base.ClientRectangle.Right)
		{
			x = base.ClientRectangle.Right;
		}
		if (y < 0)
		{
			y = 0;
		}
		else if (y > base.ClientRectangle.Bottom)
		{
			y = base.ClientRectangle.Bottom;
		}
		for (int i = top_index; i <= last_visible_index; i++)
		{
			if (GetItemDisplayRectangle(i, top_index).Contains(x, y))
			{
				return i;
			}
		}
		return -1;
	}

	internal override bool IsInputCharInternal(char charCode)
	{
		return true;
	}

	private int LastVisibleItem()
	{
		int num = items_area.Y + items_area.Height;
		int num2 = 0;
		if (top_index >= Items.Count)
		{
			return top_index;
		}
		for (num2 = top_index; num2 < Items.Count; num2++)
		{
			Rectangle itemDisplayRectangle = GetItemDisplayRectangle(num2, top_index);
			if (MultiColumn)
			{
				if (itemDisplayRectangle.X > items_area.Width)
				{
					return num2 - 1;
				}
			}
			else if (itemDisplayRectangle.Y + itemDisplayRectangle.Height > num)
			{
				return num2;
			}
		}
		return num2 - 1;
	}

	private void UpdateTopItem()
	{
		if (MultiColumn)
		{
			int num = top_index / RowCount;
			if (num > hscrollbar.Maximum)
			{
				hscrollbar.Value = hscrollbar.Maximum;
			}
			else
			{
				hscrollbar.Value = num;
			}
		}
		else
		{
			if (top_index > vscrollbar.Maximum)
			{
				vscrollbar.Value = vscrollbar.Maximum;
			}
			else
			{
				vscrollbar.Value = top_index;
			}
			Scroll(vscrollbar, vscrollbar.Value - top_index);
		}
	}

	private int NavigateItemVisually(ItemNavigation navigation)
	{
		int result = -1;
		int num2;
		if (multicolumn)
		{
			int num = items_area.Width / ColumnWidthInternal;
			num2 = num * RowCount;
			if (num2 == 0)
			{
				num2 = RowCount;
			}
		}
		else
		{
			num2 = items_area.Height / ItemHeight;
		}
		switch (navigation)
		{
		case ItemNavigation.PreviousColumn:
			if (SelectedIndex - RowCount < 0)
			{
				return -1;
			}
			if (SelectedIndex - RowCount < top_index)
			{
				top_index = SelectedIndex - RowCount;
				UpdateTopItem();
			}
			result = SelectedIndex - RowCount;
			break;
		case ItemNavigation.NextColumn:
			if (SelectedIndex + RowCount < Items.Count)
			{
				if (SelectedIndex + RowCount > last_visible_index)
				{
					top_index = SelectedIndex;
					UpdateTopItem();
				}
				result = SelectedIndex + RowCount;
			}
			break;
		case ItemNavigation.First:
			top_index = 0;
			result = 0;
			UpdateTopItem();
			break;
		case ItemNavigation.Last:
		{
			int num4 = items_area.Height / ItemHeight;
			if (multicolumn)
			{
				result = Items.Count - 1;
			}
			else if (Items.Count < num4)
			{
				top_index = 0;
				result = Items.Count - 1;
				UpdateTopItem();
			}
			else
			{
				top_index = Items.Count - num4;
				result = Items.Count - 1;
				UpdateTopItem();
			}
			break;
		}
		case ItemNavigation.Next:
		{
			if (FocusedItem == Items.Count - 1)
			{
				return -1;
			}
			if (multicolumn)
			{
				result = FocusedItem + 1;
				break;
			}
			int num5 = 0;
			ArrayList arrayList = new ArrayList();
			if (draw_mode == DrawMode.OwnerDrawVariable)
			{
				for (int i = top_index; i <= FocusedItem + 1; i++)
				{
					int itemHeight = GetItemHeight(i);
					num5 += itemHeight;
					arrayList.Add(itemHeight);
				}
			}
			else
			{
				num5 = (FocusedItem + 1 - top_index + 1) * ItemHeight;
			}
			if (num5 >= items_area.Height)
			{
				int num6 = num5 - items_area.Height;
				int num7 = 0;
				if (draw_mode == DrawMode.OwnerDrawVariable)
				{
					while (num6 > 0)
					{
						num6 -= (int)arrayList[num7];
					}
				}
				else
				{
					num7 = (int)Math.Ceiling((float)num6 / (float)ItemHeight);
				}
				top_index += num7;
				UpdateTopItem();
			}
			result = FocusedItem + 1;
			break;
		}
		case ItemNavigation.Previous:
			if (FocusedItem > 0)
			{
				if (FocusedItem - 1 < top_index)
				{
					top_index--;
					UpdateTopItem();
				}
				result = FocusedItem - 1;
			}
			break;
		case ItemNavigation.NextPage:
			if (Items.Count < num2)
			{
				NavigateItemVisually(ItemNavigation.Last);
				break;
			}
			if (FocusedItem + num2 - 1 >= Items.Count)
			{
				top_index = Items.Count - num2;
				UpdateTopItem();
				result = Items.Count - 1;
				break;
			}
			if (FocusedItem + num2 - 1 > last_visible_index)
			{
				top_index = FocusedItem;
				UpdateTopItem();
			}
			result = FocusedItem + num2 - 1;
			break;
		case ItemNavigation.PreviousPage:
		{
			int num3 = items_area.Height / ItemHeight;
			if (FocusedItem - (num3 - 1) <= 0)
			{
				top_index = 0;
				UpdateTopItem();
				result = 0;
				break;
			}
			if (SelectedIndex - (num3 - 1) < top_index)
			{
				top_index = FocusedItem - (num3 - 1);
				UpdateTopItem();
			}
			result = FocusedItem - (num3 - 1);
			break;
		}
		}
		return result;
	}

	private void OnGotFocus(object sender, EventArgs e)
	{
		if (Items.Count != 0)
		{
			if (FocusedItem == -1)
			{
				FocusedItem = 0;
			}
			InvalidateItem(FocusedItem);
		}
	}

	private void OnLostFocus(object sender, EventArgs e)
	{
		if (FocusedItem != -1)
		{
			InvalidateItem(FocusedItem);
		}
	}

	private bool KeySearch(Keys key)
	{
		char c = (char)key;
		if (!char.IsLetterOrDigit(c))
		{
			return false;
		}
		int num = FindString(c.ToString(), SelectedIndex);
		if (num != -1)
		{
			SelectedIndex = num;
		}
		return true;
	}

	internal void HandleKeyDown(Keys key)
	{
		int num = -1;
		if (Items.Count == 0 || KeySearch(key))
		{
			return;
		}
		switch (key)
		{
		case Keys.ControlKey:
			ctrl_pressed = true;
			break;
		case Keys.ShiftKey:
			shift_pressed = true;
			break;
		case Keys.Home:
			num = NavigateItemVisually(ItemNavigation.First);
			break;
		case Keys.End:
			num = NavigateItemVisually(ItemNavigation.Last);
			break;
		case Keys.Up:
			num = NavigateItemVisually(ItemNavigation.Previous);
			break;
		case Keys.Down:
			num = NavigateItemVisually(ItemNavigation.Next);
			break;
		case Keys.PageUp:
			num = NavigateItemVisually(ItemNavigation.PreviousPage);
			break;
		case Keys.PageDown:
			num = NavigateItemVisually(ItemNavigation.NextPage);
			break;
		case Keys.Right:
			if (multicolumn)
			{
				num = NavigateItemVisually(ItemNavigation.NextColumn);
			}
			break;
		case Keys.Left:
			if (multicolumn)
			{
				num = NavigateItemVisually(ItemNavigation.PreviousColumn);
			}
			break;
		case Keys.Space:
			if (selection_mode == SelectionMode.MultiSimple)
			{
				SelectedItemFromNavigation(FocusedItem);
			}
			break;
		}
		if (num != -1)
		{
			FocusedItem = num;
			if (selection_mode != SelectionMode.MultiSimple)
			{
				SelectedItemFromNavigation(num);
			}
		}
	}

	private void OnKeyUpLB(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.ControlKey:
			ctrl_pressed = false;
			break;
		case Keys.ShiftKey:
			shift_pressed = false;
			break;
		}
	}

	internal void InvalidateItem(int index)
	{
		if (base.IsHandleCreated)
		{
			Rectangle itemDisplayRectangle = GetItemDisplayRectangle(index, top_index);
			if (base.ClientRectangle.IntersectsWith(itemDisplayRectangle))
			{
				Invalidate(itemDisplayRectangle);
			}
		}
	}

	internal virtual void OnItemClick(int index)
	{
		OnSelectedIndexChanged(EventArgs.Empty);
		OnSelectedValueChanged(EventArgs.Empty);
	}

	private void SelectExtended(int index)
	{
		SuspendLayout();
		ArrayList arrayList = new ArrayList();
		int num = ((anchor >= index) ? index : anchor);
		int num2 = ((anchor <= index) ? index : anchor);
		for (int i = num; i <= num2; i++)
		{
			arrayList.Add(i);
		}
		if (ctrl_pressed)
		{
			int[] array = prev_selection;
			foreach (int num3 in array)
			{
				if (!arrayList.Contains(num3))
				{
					arrayList.Add(num3);
				}
			}
		}
		ArrayList arrayList2 = (ArrayList)selected_indices.List.Clone();
		foreach (int item in arrayList2)
		{
			if (!arrayList.Contains(item))
			{
				selected_indices.Remove(item);
			}
		}
		foreach (int item2 in arrayList)
		{
			if (!arrayList2.Contains(item2))
			{
				selected_indices.AddCore(item2);
			}
		}
		ResumeLayout();
	}

	private void OnMouseDownLB(object sender, MouseEventArgs e)
	{
		if ((e.Button & MouseButtons.Left) == 0)
		{
			return;
		}
		int num = IndexAtClientPoint(e.X, e.Y);
		if (num == -1)
		{
			return;
		}
		switch (SelectionMode)
		{
		default:
			return;
		case SelectionMode.One:
			SelectedIndices.AddCore(num);
			break;
		case SelectionMode.MultiSimple:
			if (SelectedIndices.Contains(num))
			{
				SelectedIndices.RemoveCore(num);
			}
			else
			{
				SelectedIndices.AddCore(num);
			}
			break;
		case SelectionMode.MultiExtended:
			shift_pressed = (XplatUI.State.ModifierKeys & Keys.Shift) != 0;
			ctrl_pressed = (XplatUI.State.ModifierKeys & Keys.Control) != 0;
			if (shift_pressed)
			{
				SelectedIndices.ClearCore();
				SelectExtended(num);
				break;
			}
			anchor = num;
			if (ctrl_pressed)
			{
				prev_selection = new int[SelectedIndices.Count];
				SelectedIndices.CopyTo(prev_selection, 0);
				if (SelectedIndices.Contains(num))
				{
					SelectedIndices.RemoveCore(num);
				}
				else
				{
					SelectedIndices.AddCore(num);
				}
			}
			else
			{
				SelectedIndices.ClearCore();
				SelectedIndices.AddCore(num);
			}
			break;
		case SelectionMode.None:
			break;
		}
		button_pressed = true;
		button_pressed_loc = new Point(e.X, e.Y);
		FocusedItem = num;
	}

	private void OnMouseMoveLB(object sender, MouseEventArgs e)
	{
		if (!button_pressed || button_pressed_loc == new Point(e.X, e.Y))
		{
			return;
		}
		int num = IndexAtClientPoint(e.X, e.Y);
		if (num != -1)
		{
			switch (SelectionMode)
			{
			default:
				return;
			case SelectionMode.One:
				SelectedIndices.AddCore(num);
				break;
			case SelectionMode.MultiExtended:
				SelectExtended(num);
				break;
			case SelectionMode.None:
			case SelectionMode.MultiSimple:
				break;
			}
			FocusedItem = num;
		}
	}

	internal override void OnDragDropEnd(DragDropEffects effects)
	{
		button_pressed = false;
	}

	private void OnMouseUpLB(object sender, MouseEventArgs e)
	{
		if ((e.Button & MouseButtons.Left) != 0)
		{
			if (e.Clicks > 1)
			{
				OnDoubleClick(EventArgs.Empty);
				OnMouseDoubleClick(e);
			}
			else if (e.Clicks == 1)
			{
				OnClick(EventArgs.Empty);
				OnMouseClick(e);
			}
			if (button_pressed)
			{
				int index = IndexAtClientPoint(e.X, e.Y);
				OnItemClick(index);
				button_pressed = (ctrl_pressed = (shift_pressed = false));
			}
		}
	}

	private void Scroll(ScrollBar scrollbar, int delta)
	{
		if (delta != 0 && scrollbar.Visible && scrollbar.Enabled)
		{
			int num = ((scrollbar != hscrollbar) ? (vscrollbar.Maximum - items_area.Height / ItemHeight + 1) : (hscrollbar.Maximum - items_area.Width / ColumnWidthInternal + 1));
			int num2 = scrollbar.Value + delta;
			if (num2 > num)
			{
				num2 = num;
			}
			else if (num2 < scrollbar.Minimum)
			{
				num2 = scrollbar.Minimum;
			}
			scrollbar.Value = num2;
		}
	}

	private void OnMouseWheelLB(object sender, MouseEventArgs me)
	{
		if (Items.Count != 0)
		{
			int num = me.Delta / 120;
			if (MultiColumn)
			{
				Scroll(hscrollbar, -SystemInformation.MouseWheelScrollLines * num);
			}
			else
			{
				Scroll(vscrollbar, -num);
			}
		}
	}

	internal override void OnPaintInternal(PaintEventArgs pevent)
	{
		if (!suspend_layout)
		{
			Draw(pevent.ClipRectangle, pevent.Graphics);
		}
	}

	internal void RepositionScrollBars()
	{
		if (vscrollbar.is_visible)
		{
			vscrollbar.Size = new Size(vscrollbar.Width, items_area.Height);
			vscrollbar.Location = new Point(items_area.Width, 0);
		}
		if (hscrollbar.is_visible)
		{
			hscrollbar.Size = new Size(items_area.Width, hscrollbar.Height);
			hscrollbar.Location = new Point(0, items_area.Height);
		}
	}

	internal void SelectedItemFromNavigation(int index)
	{
		switch (SelectionMode)
		{
		case SelectionMode.None:
			EnsureVisible(index);
			OnSelectedIndexChanged(EventArgs.Empty);
			OnSelectedValueChanged(EventArgs.Empty);
			break;
		case SelectionMode.One:
			SelectedIndex = index;
			break;
		case SelectionMode.MultiSimple:
			if (SelectedIndex == -1)
			{
				SelectedIndex = index;
				break;
			}
			if (SelectedIndices.Contains(index))
			{
				SelectedIndices.Remove(index);
				break;
			}
			SelectedIndices.AddCore(index);
			OnSelectedIndexChanged(EventArgs.Empty);
			OnSelectedValueChanged(EventArgs.Empty);
			break;
		case SelectionMode.MultiExtended:
			if (SelectedIndex == -1)
			{
				SelectedIndex = index;
				break;
			}
			if (!ctrl_pressed && !shift_pressed)
			{
				SelectedIndices.Clear();
			}
			if (shift_pressed)
			{
				ShiftSelection(index);
			}
			else
			{
				SelectedIndices.AddCore(index);
			}
			OnSelectedIndexChanged(EventArgs.Empty);
			OnSelectedValueChanged(EventArgs.Empty);
			break;
		}
	}

	private void ShiftSelection(int index)
	{
		int num = -1;
		int num2 = Items.Count + 1;
		foreach (int selected_index in selected_indices)
		{
			int num4 = ((selected_index <= index) ? (index - selected_index) : (selected_index - index));
			if (num4 < num2)
			{
				num2 = num4;
				num = selected_index;
			}
		}
		if (num != -1)
		{
			int num5;
			int num6;
			if (num > index)
			{
				num5 = index;
				num6 = num;
			}
			else
			{
				num5 = num;
				num6 = index;
			}
			selected_indices.Clear();
			for (int i = num5; i <= num6; i++)
			{
				selected_indices.AddCore(i);
			}
		}
	}

	internal virtual void CollectionChanged()
	{
		if (sorted)
		{
			Sort(paint: false);
		}
		if (Items.Count == 0)
		{
			selected_indices.List.Clear();
			focused_item = -1;
			top_index = 0;
		}
		if (Items.Count <= focused_item)
		{
			focused_item = Items.Count - 1;
		}
		if (base.IsHandleCreated && !suspend_layout)
		{
			LayoutListBox();
			base.Refresh();
		}
	}

	private void EnsureVisible(int index)
	{
		if (!base.IsHandleCreated || index == -1)
		{
			return;
		}
		if (index < top_index)
		{
			top_index = index;
			UpdateTopItem();
			Invalidate();
			return;
		}
		if (!multicolumn)
		{
			int num = items_area.Height / ItemHeight;
			if (index >= top_index + num)
			{
				top_index = index - num + 1;
			}
			UpdateTopItem();
			return;
		}
		int num2 = Math.Max(1, items_area.Height / ItemHeight);
		int num3 = Math.Max(1, items_area.Width / ColumnWidthInternal);
		if (index >= top_index + num2 * num3)
		{
			int num4 = index / num2;
			top_index = (num4 - (num3 - 1)) * num2;
			UpdateTopItem();
			Invalidate();
		}
	}

	private void UpdateListBoxBounds()
	{
		if (base.IsHandleCreated)
		{
			SetBoundsInternal(bounds.X, bounds.Y, bounds.Width, (!IntegralHeight) ? requested_height : SnapHeightToIntegral(requested_height), BoundsSpecified.None);
		}
	}

	private void UpdateScrollBars()
	{
		items_area = base.ClientRectangle;
		if (UpdateHorizontalScrollBar())
		{
			items_area.Height -= hscrollbar.Height;
			if (UpdateVerticalScrollBar())
			{
				items_area.Width -= vscrollbar.Width;
				UpdateHorizontalScrollBar();
			}
		}
		else if (UpdateVerticalScrollBar())
		{
			items_area.Width -= vscrollbar.Width;
			if (UpdateHorizontalScrollBar())
			{
				items_area.Height -= hscrollbar.Height;
				UpdateVerticalScrollBar();
			}
		}
		RepositionScrollBars();
	}

	private bool UpdateHorizontalScrollBar()
	{
		bool flag = false;
		bool enabled = true;
		if (MultiColumn)
		{
			if (canvas_size.Width > items_area.Width)
			{
				flag = true;
				hscrollbar.Maximum = canvas_size.Width / ColumnWidthInternal - 1;
			}
			else if (ScrollAlwaysVisible)
			{
				enabled = false;
				flag = true;
				hscrollbar.Maximum = 0;
			}
		}
		else if (canvas_size.Width > base.ClientRectangle.Width && HorizontalScrollbar)
		{
			flag = true;
			hscrollbar.Maximum = canvas_size.Width;
			hscrollbar.LargeChange = Math.Max(0, items_area.Width);
		}
		else if (scroll_always_visible && horizontal_scrollbar)
		{
			flag = true;
			enabled = false;
			hscrollbar.Maximum = 0;
		}
		hbar_offset = hscrollbar.Value;
		hscrollbar.Enabled = enabled;
		hscrollbar.Visible = flag;
		return flag;
	}

	private bool UpdateVerticalScrollBar()
	{
		if (MultiColumn || (Items.Count == 0 && !scroll_always_visible))
		{
			vscrollbar.Visible = false;
			return false;
		}
		if (Items.Count == 0)
		{
			vscrollbar.Visible = true;
			vscrollbar.Enabled = false;
			vscrollbar.Maximum = 0;
			return true;
		}
		bool flag = false;
		bool enabled = true;
		if (canvas_size.Height > items_area.Height)
		{
			flag = true;
			vscrollbar.Maximum = Items.Count - 1;
			vscrollbar.LargeChange = Math.Max(items_area.Height / ItemHeight, 0);
		}
		else if (ScrollAlwaysVisible)
		{
			flag = true;
			enabled = false;
			vscrollbar.Maximum = 0;
		}
		vscrollbar.Enabled = enabled;
		vscrollbar.Visible = flag;
		return flag;
	}

	private void VerticalScrollEvent(object sender, EventArgs e)
	{
		int num = top_index;
		top_index = vscrollbar.Value;
		last_visible_index = LastVisibleItem();
		int num2 = (num - top_index) * ItemHeight;
		if (DrawMode == DrawMode.OwnerDrawVariable)
		{
			num2 = 0;
			if (top_index < num)
			{
				for (int i = top_index; i < num; i++)
				{
					num2 += GetItemHeight(i);
				}
			}
			else
			{
				for (int j = num; j < top_index; j++)
				{
					num2 -= GetItemHeight(j);
				}
			}
		}
		if (base.IsHandleCreated)
		{
			XplatUI.ScrollWindow(Handle, items_area, 0, num2, with_children: false);
		}
	}
}
