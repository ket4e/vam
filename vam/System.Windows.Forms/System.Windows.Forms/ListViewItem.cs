using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

[Serializable]
[DefaultProperty("Text")]
[ToolboxItem(false)]
[TypeConverter(typeof(ListViewItemConverter))]
[DesignTimeVisible(false)]
public class ListViewItem : ISerializable, ICloneable
{
	[Serializable]
	[ToolboxItem(false)]
	[DefaultProperty("Text")]
	[DesignTimeVisible(false)]
	[TypeConverter(typeof(ListViewSubItemConverter))]
	public class ListViewSubItem
	{
		[Serializable]
		private class SubItemStyle
		{
			public Color backColor;

			public Color foreColor;

			public Font font;

			public SubItemStyle()
			{
			}

			public SubItemStyle(Color foreColor, Color backColor, Font font)
			{
				this.foreColor = foreColor;
				this.backColor = backColor;
				this.font = font;
			}

			public void Reset()
			{
				foreColor = Color.Empty;
				backColor = Color.Empty;
				font = null;
			}
		}

		[NonSerialized]
		internal ListViewItem owner;

		private string text = string.Empty;

		private string name;

		private object userData;

		private SubItemStyle style;

		[NonSerialized]
		internal Rectangle bounds;

		public Color BackColor
		{
			get
			{
				if (style.backColor != Color.Empty)
				{
					return style.backColor;
				}
				if (owner != null && owner.ListView != null)
				{
					return owner.ListView.BackColor;
				}
				return ThemeEngine.Current.ColorWindow;
			}
			set
			{
				style.backColor = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		public Rectangle Bounds
		{
			get
			{
				Rectangle result = bounds;
				if (owner != null)
				{
					result.X += owner.Bounds.X;
					result.Y += owner.Bounds.Y;
				}
				return result;
			}
		}

		[Localizable(true)]
		public Font Font
		{
			get
			{
				if (style.font != null)
				{
					return style.font;
				}
				if (owner != null)
				{
					return owner.Font;
				}
				return ThemeEngine.Current.DefaultFont;
			}
			set
			{
				if (style.font != value)
				{
					style.font = value;
					Invalidate();
				}
			}
		}

		public Color ForeColor
		{
			get
			{
				if (style.foreColor != Color.Empty)
				{
					return style.foreColor;
				}
				if (owner != null && owner.ListView != null)
				{
					return owner.ListView.ForeColor;
				}
				return ThemeEngine.Current.ColorWindowText;
			}
			set
			{
				style.foreColor = value;
				Invalidate();
			}
		}

		[Localizable(true)]
		public string Name
		{
			get
			{
				if (name == null)
				{
					return string.Empty;
				}
				return name;
			}
			set
			{
				name = value;
			}
		}

		[DefaultValue(null)]
		[Localizable(false)]
		[Bindable(true)]
		[TypeConverter(typeof(StringConverter))]
		public object Tag
		{
			get
			{
				return userData;
			}
			set
			{
				userData = value;
			}
		}

		[Localizable(true)]
		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				if (!(text == value))
				{
					if (value == null)
					{
						text = string.Empty;
					}
					else
					{
						text = value;
					}
					Invalidate();
					OnUIATextChanged();
				}
			}
		}

		internal int Height => bounds.Height;

		[field: NonSerialized]
		internal event EventHandler UIATextChanged;

		public ListViewSubItem()
			: this(null, string.Empty, Color.Empty, Color.Empty, null)
		{
		}

		public ListViewSubItem(ListViewItem owner, string text)
			: this(owner, text, Color.Empty, Color.Empty, null)
		{
		}

		public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
		{
			this.owner = owner;
			Text = text;
			style = new SubItemStyle(foreColor, backColor, font);
		}

		private void OnUIATextChanged()
		{
			if (this.UIATextChanged != null)
			{
				this.UIATextChanged(this, EventArgs.Empty);
			}
		}

		public void ResetStyle()
		{
			style.Reset();
			Invalidate();
		}

		public override string ToString()
		{
			return string.Format("ListViewSubItem {{0}}", text);
		}

		private void Invalidate()
		{
			if (owner != null && owner.owner != null)
			{
				owner.Invalidate();
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			name = null;
			userData = null;
		}

		internal void SetBounds(int x, int y, int width, int height)
		{
			bounds = new Rectangle(x, y, width, height);
		}
	}

	public class ListViewSubItemCollection : ICollection, IEnumerable, IList
	{
		private ArrayList list;

		internal ListViewItem owner;

		bool ICollection.IsSynchronized => list.IsSynchronized;

		object ICollection.SyncRoot => list.SyncRoot;

		bool IList.IsFixedSize => list.IsFixedSize;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is ListViewSubItem))
				{
					throw new ArgumentException("Not of type ListViewSubItem", "value");
				}
				this[index] = (ListViewSubItem)value;
			}
		}

		[Browsable(false)]
		public int Count => list.Count;

		public bool IsReadOnly => false;

		public ListViewSubItem this[int index]
		{
			get
			{
				return (ListViewSubItem)list[index];
			}
			set
			{
				value.owner = owner;
				list[index] = value;
				owner.Layout();
				owner.Invalidate();
			}
		}

		public virtual ListViewSubItem this[string key]
		{
			get
			{
				int num = IndexOfKey(key);
				if (num == -1)
				{
					return null;
				}
				return (ListViewSubItem)list[num];
			}
		}

		public ListViewSubItemCollection(ListViewItem owner)
			: this(owner, owner.Text)
		{
		}

		internal ListViewSubItemCollection(ListViewItem owner, string text)
		{
			this.owner = owner;
			list = new ArrayList();
			if (text != null)
			{
				Add(text);
			}
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			list.CopyTo(dest, index);
		}

		int IList.Add(object item)
		{
			if (!(item is ListViewSubItem))
			{
				throw new ArgumentException("Not of type ListViewSubItem", "item");
			}
			ListViewSubItem listViewSubItem = (ListViewSubItem)item;
			listViewSubItem.owner = owner;
			listViewSubItem.UIATextChanged += OnUIASubItemTextChanged;
			return list.Add(listViewSubItem);
		}

		bool IList.Contains(object subItem)
		{
			if (!(subItem is ListViewSubItem))
			{
				throw new ArgumentException("Not of type ListViewSubItem", "subItem");
			}
			return Contains((ListViewSubItem)subItem);
		}

		int IList.IndexOf(object subItem)
		{
			if (!(subItem is ListViewSubItem))
			{
				throw new ArgumentException("Not of type ListViewSubItem", "subItem");
			}
			return IndexOf((ListViewSubItem)subItem);
		}

		void IList.Insert(int index, object item)
		{
			if (!(item is ListViewSubItem))
			{
				throw new ArgumentException("Not of type ListViewSubItem", "item");
			}
			Insert(index, (ListViewSubItem)item);
		}

		void IList.Remove(object item)
		{
			if (!(item is ListViewSubItem))
			{
				throw new ArgumentException("Not of type ListViewSubItem", "item");
			}
			Remove((ListViewSubItem)item);
		}

		public ListViewSubItem Add(ListViewSubItem item)
		{
			AddSubItem(item);
			owner.Layout();
			owner.Invalidate();
			return item;
		}

		public ListViewSubItem Add(string text)
		{
			ListViewSubItem item = new ListViewSubItem(owner, text);
			return Add(item);
		}

		public ListViewSubItem Add(string text, Color foreColor, Color backColor, Font font)
		{
			ListViewSubItem item = new ListViewSubItem(owner, text, foreColor, backColor, font);
			return Add(item);
		}

		public void AddRange(ListViewSubItem[] items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (ListViewSubItem listViewSubItem in items)
			{
				if (listViewSubItem != null)
				{
					AddSubItem(listViewSubItem);
				}
			}
			owner.Layout();
			owner.Invalidate();
		}

		public void AddRange(string[] items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (string text in items)
			{
				if (text != null)
				{
					AddSubItem(new ListViewSubItem(owner, text));
				}
			}
			owner.Layout();
			owner.Invalidate();
		}

		public void AddRange(string[] items, Color foreColor, Color backColor, Font font)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (string text in items)
			{
				if (text != null)
				{
					AddSubItem(new ListViewSubItem(owner, text, foreColor, backColor, font));
				}
			}
			owner.Layout();
			owner.Invalidate();
		}

		private void AddSubItem(ListViewSubItem subItem)
		{
			subItem.owner = owner;
			list.Add(subItem);
			subItem.UIATextChanged += OnUIASubItemTextChanged;
		}

		public void Clear()
		{
			list.Clear();
		}

		public bool Contains(ListViewSubItem subItem)
		{
			return list.Contains(subItem);
		}

		public virtual bool ContainsKey(string key)
		{
			return IndexOfKey(key) != -1;
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public int IndexOf(ListViewSubItem subItem)
		{
			return list.IndexOf(subItem);
		}

		public virtual int IndexOfKey(string key)
		{
			if (key == null || key.Length == 0)
			{
				return -1;
			}
			for (int i = 0; i < list.Count; i++)
			{
				ListViewSubItem listViewSubItem = (ListViewSubItem)list[i];
				if (string.Compare(listViewSubItem.Name, key, ignoreCase: true) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, ListViewSubItem item)
		{
			item.owner = owner;
			list.Insert(index, item);
			owner.Layout();
			owner.Invalidate();
			item.UIATextChanged += OnUIASubItemTextChanged;
		}

		public void Remove(ListViewSubItem item)
		{
			list.Remove(item);
			owner.Layout();
			owner.Invalidate();
			item.UIATextChanged -= OnUIASubItemTextChanged;
		}

		public virtual void RemoveByKey(string key)
		{
			int num = IndexOfKey(key);
			if (num != -1)
			{
				RemoveAt(num);
			}
		}

		public void RemoveAt(int index)
		{
			if (index >= 0 && index < list.Count)
			{
				((ListViewSubItem)list[index]).UIATextChanged -= OnUIASubItemTextChanged;
			}
			list.RemoveAt(index);
		}

		private void OnUIASubItemTextChanged(object sender, EventArgs args)
		{
			owner.OnUIASubItemTextChanged(new LabelEditEventArgs(list.IndexOf(sender)));
		}
	}

	private int image_index = -1;

	private bool is_checked;

	private int state_image_index = -1;

	private ListViewSubItemCollection sub_items;

	private object tag;

	private bool use_item_style = true;

	private int display_index = -1;

	private ListViewGroup group;

	private string name = string.Empty;

	private string image_key = string.Empty;

	private string tooltip_text = string.Empty;

	private int indent_count;

	private Point position = new Point(-1, -1);

	private Rectangle bounds = Rectangle.Empty;

	private Rectangle checkbox_rect;

	private Rectangle icon_rect;

	private Rectangle item_rect;

	private Rectangle label_rect;

	private ListView owner;

	private Font font;

	private Font hot_font;

	private bool selected;

	internal int row;

	internal int col;

	private Rectangle text_bounds;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color BackColor
	{
		get
		{
			if (sub_items.Count > 0)
			{
				return sub_items[0].BackColor;
			}
			if (owner != null)
			{
				return owner.BackColor;
			}
			return ThemeEngine.Current.ColorWindow;
		}
		set
		{
			SubItems[0].BackColor = value;
		}
	}

	[Browsable(false)]
	public Rectangle Bounds => GetBounds(ItemBoundsPortion.Entire);

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(false)]
	public bool Checked
	{
		get
		{
			return is_checked;
		}
		set
		{
			if (is_checked == value)
			{
				return;
			}
			if (owner != null)
			{
				CheckState checkState = (is_checked ? CheckState.Checked : CheckState.Unchecked);
				CheckState checkState2 = (value ? CheckState.Checked : CheckState.Unchecked);
				ItemCheckEventArgs ice = new ItemCheckEventArgs(Index, checkState2, checkState);
				owner.OnItemCheck(ice);
				if (checkState2 != checkState)
				{
					owner.CheckedItems.Reset();
					is_checked = checkState2 == CheckState.Checked;
					Invalidate();
					ItemCheckedEventArgs e = new ItemCheckedEventArgs(this);
					owner.OnItemChecked(e);
				}
			}
			else
			{
				is_checked = value;
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool Focused
	{
		get
		{
			if (owner == null)
			{
				return false;
			}
			if (owner.VirtualMode)
			{
				return Index == owner.focused_item_index;
			}
			return owner.FocusedItem == this;
		}
		set
		{
			if (owner != null && Focused != value)
			{
				owner.FocusedItem?.UpdateFocusedState();
				owner.focused_item_index = ((!value) ? (-1) : Index);
				if (value)
				{
					owner.OnUIAFocusedItemChanged();
				}
				UpdateFocusedState();
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Localizable(true)]
	public Font Font
	{
		get
		{
			if (font != null)
			{
				return font;
			}
			if (owner != null)
			{
				return owner.Font;
			}
			return ThemeEngine.Current.DefaultFont;
		}
		set
		{
			if (font != value)
			{
				font = value;
				hot_font = null;
				if (owner != null)
				{
					Layout();
				}
				Invalidate();
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Color ForeColor
	{
		get
		{
			if (sub_items.Count > 0)
			{
				return sub_items[0].ForeColor;
			}
			if (owner != null)
			{
				return owner.ForeColor;
			}
			return ThemeEngine.Current.ColorWindowText;
		}
		set
		{
			SubItems[0].ForeColor = value;
		}
	}

	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DefaultValue(-1)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[TypeConverter(typeof(NoneExcludedImageIndexConverter))]
	public int ImageIndex
	{
		get
		{
			return image_index;
		}
		set
		{
			if (value < -1)
			{
				throw new ArgumentException("Invalid ImageIndex. It must be greater than or equal to -1.");
			}
			image_index = value;
			image_key = string.Empty;
			if (owner != null)
			{
				Layout();
			}
			Invalidate();
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DefaultValue("")]
	[Localizable(true)]
	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[RefreshProperties(RefreshProperties.Repaint)]
	[TypeConverter(typeof(ImageKeyConverter))]
	public string ImageKey
	{
		get
		{
			return image_key;
		}
		set
		{
			image_key = ((value != null) ? value : string.Empty);
			image_index = -1;
			if (owner != null)
			{
				Layout();
			}
			Invalidate();
		}
	}

	[Browsable(false)]
	public ImageList ImageList
	{
		get
		{
			if (owner == null)
			{
				return null;
			}
			if (owner.View == View.LargeIcon)
			{
				return owner.large_image_list;
			}
			return owner.small_image_list;
		}
	}

	[DefaultValue(0)]
	public int IndentCount
	{
		get
		{
			return indent_count;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (value != indent_count)
			{
				indent_count = value;
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	public int Index
	{
		get
		{
			if (owner == null)
			{
				return -1;
			}
			if (owner.VirtualMode)
			{
				return display_index;
			}
			if (display_index == -1)
			{
				return owner.Items.IndexOf(this);
			}
			return owner.GetItemIndex(display_index);
		}
	}

	[Browsable(false)]
	public ListView ListView => owner;

	[Localizable(true)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = ((value != null) ? value : string.Empty);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Point Position
	{
		get
		{
			if (owner != null && owner.VirtualMode)
			{
				return owner.GetItemLocation(display_index);
			}
			if (owner != null && !owner.IsHandleCreated)
			{
				return new Point(-1, -1);
			}
			return position;
		}
		set
		{
			if (owner != null && owner.View != View.Details && owner.View != View.List)
			{
				if (owner.VirtualMode)
				{
					throw new InvalidOperationException();
				}
				owner.ChangeItemLocation(display_index, value);
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool Selected
	{
		get
		{
			if (owner != null && owner.VirtualMode)
			{
				return owner.SelectedIndices.Contains(Index);
			}
			return selected;
		}
		set
		{
			if (selected == value && owner != null && !owner.VirtualMode)
			{
				return;
			}
			if (owner != null)
			{
				if (value && !owner.MultiSelect)
				{
					owner.SelectedIndices.Clear();
				}
				if (owner.VirtualMode)
				{
					if (value)
					{
						owner.SelectedIndices.InsertIndex(Index);
					}
					else
					{
						owner.SelectedIndices.RemoveIndex(Index);
					}
				}
				else
				{
					selected = value;
					owner.SelectedIndices.Reset();
				}
				owner.OnItemSelectionChanged(new ListViewItemSelectionChangedEventArgs(this, Index, value));
				owner.OnSelectedIndexChanged();
			}
			else
			{
				selected = value;
			}
			Invalidate();
		}
	}

	[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	[RelatedImageList("ListView.StateImageList")]
	[TypeConverter(typeof(NoneExcludedImageIndexConverter))]
	[DefaultValue(-1)]
	public int StateImageIndex
	{
		get
		{
			return state_image_index;
		}
		set
		{
			if (value < -1 || value > 14)
			{
				throw new ArgumentOutOfRangeException("Invalid StateImageIndex. It must be in the range of [-1, 14].");
			}
			state_image_index = value;
		}
	}

	[Editor("System.Windows.Forms.Design.ListViewSubItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ListViewSubItemCollection SubItems
	{
		get
		{
			if (sub_items.Count == 0)
			{
				sub_items.Add(string.Empty);
			}
			return sub_items;
		}
	}

	[Localizable(false)]
	[DefaultValue(null)]
	[Bindable(true)]
	[TypeConverter(typeof(StringConverter))]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Localizable(true)]
	public string Text
	{
		get
		{
			if (sub_items.Count > 0)
			{
				return sub_items[0].Text;
			}
			return string.Empty;
		}
		set
		{
			if (!(SubItems[0].Text == value))
			{
				sub_items[0].Text = value;
				if (owner != null)
				{
					Layout();
				}
				Invalidate();
				OnUIATextChanged();
			}
		}
	}

	[DefaultValue(true)]
	public bool UseItemStyleForSubItems
	{
		get
		{
			return use_item_style;
		}
		set
		{
			use_item_style = value;
		}
	}

	[DefaultValue(null)]
	[Localizable(true)]
	public ListViewGroup Group
	{
		get
		{
			return group;
		}
		set
		{
			if (group != value)
			{
				if (value == null)
				{
					group.Items.Remove(this);
				}
				else
				{
					value.Items.Add(this);
				}
				group = value;
			}
		}
	}

	[DefaultValue("")]
	public string ToolTipText
	{
		get
		{
			return tooltip_text;
		}
		set
		{
			if (value == null)
			{
				value = string.Empty;
			}
			tooltip_text = value;
		}
	}

	internal Rectangle CheckRectReal
	{
		get
		{
			Rectangle result = checkbox_rect;
			Point itemLocation = owner.GetItemLocation(DisplayIndex);
			result.X += itemLocation.X;
			result.Y += itemLocation.Y;
			return result;
		}
	}

	internal Rectangle TextBounds
	{
		get
		{
			if (owner.VirtualMode && bounds == new Rectangle(-1, -1, -1, -1))
			{
				Layout();
			}
			Rectangle result = text_bounds;
			Point itemLocation = owner.GetItemLocation(DisplayIndex);
			result.X += itemLocation.X;
			result.Y += itemLocation.Y;
			return result;
		}
	}

	internal int DisplayIndex
	{
		get
		{
			if (display_index == -1)
			{
				return owner.Items.IndexOf(this);
			}
			return display_index;
		}
		set
		{
			display_index = value;
		}
	}

	internal bool Hot => Index == owner.HotItemIndex;

	internal Font HotFont
	{
		get
		{
			if (hot_font == null)
			{
				hot_font = new Font(Font, Font.Style | FontStyle.Underline);
			}
			return hot_font;
		}
	}

	internal ListView Owner
	{
		set
		{
			if (owner != value)
			{
				owner = value;
			}
		}
	}

	internal event EventHandler UIATextChanged;

	internal event LabelEditEventHandler UIASubItemTextChanged;

	public ListViewItem()
		: this(string.Empty)
	{
	}

	public ListViewItem(string text)
		: this(text, -1)
	{
	}

	public ListViewItem(string[] items)
		: this(items, -1)
	{
	}

	public ListViewItem(ListViewSubItem[] subItems, int imageIndex)
	{
		sub_items = new ListViewSubItemCollection(this, null);
		for (int i = 0; i < subItems.Length; i++)
		{
			sub_items.Add(subItems[i]);
		}
		image_index = imageIndex;
	}

	public ListViewItem(string text, int imageIndex)
	{
		image_index = imageIndex;
		sub_items = new ListViewSubItemCollection(this, text);
	}

	public ListViewItem(string[] items, int imageIndex)
	{
		sub_items = new ListViewSubItemCollection(this, null);
		if (items != null)
		{
			for (int i = 0; i < items.Length; i++)
			{
				sub_items.Add(new ListViewSubItem(this, items[i]));
			}
		}
		image_index = imageIndex;
	}

	public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font)
		: this(items, imageIndex)
	{
		ForeColor = foreColor;
		BackColor = backColor;
		this.font = font;
	}

	public ListViewItem(string[] items, string imageKey)
		: this(items)
	{
		ImageKey = imageKey;
	}

	public ListViewItem(string text, string imageKey)
		: this(text)
	{
		ImageKey = imageKey;
	}

	public ListViewItem(ListViewSubItem[] subItems, string imageKey)
	{
		sub_items = new ListViewSubItemCollection(this, null);
		for (int i = 0; i < subItems.Length; i++)
		{
			sub_items.Add(subItems[i]);
		}
		ImageKey = imageKey;
	}

	public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font)
		: this(items, imageKey)
	{
		ForeColor = foreColor;
		BackColor = backColor;
		this.font = font;
	}

	public ListViewItem(ListViewGroup group)
		: this()
	{
		Group = group;
	}

	public ListViewItem(string text, ListViewGroup group)
		: this(text)
	{
		Group = group;
	}

	public ListViewItem(string[] items, ListViewGroup group)
		: this(items)
	{
		Group = group;
	}

	public ListViewItem(ListViewSubItem[] subItems, int imageIndex, ListViewGroup group)
		: this(subItems, imageIndex)
	{
		Group = group;
	}

	public ListViewItem(ListViewSubItem[] subItems, string imageKey, ListViewGroup group)
		: this(subItems, imageKey)
	{
		Group = group;
	}

	public ListViewItem(string text, int imageIndex, ListViewGroup group)
		: this(text, imageIndex)
	{
		Group = group;
	}

	public ListViewItem(string text, string imageKey, ListViewGroup group)
		: this(text, imageKey)
	{
		Group = group;
	}

	public ListViewItem(string[] items, int imageIndex, ListViewGroup group)
		: this(items, imageIndex)
	{
		Group = group;
	}

	public ListViewItem(string[] items, string imageKey, ListViewGroup group)
		: this(items, imageKey)
	{
		Group = group;
	}

	public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font, ListViewGroup group)
		: this(items, imageIndex, foreColor, backColor, font)
	{
		Group = group;
	}

	public ListViewItem(string[] items, string imageKey, Color foreColor, Color backColor, Font font, ListViewGroup group)
		: this(items, imageKey, foreColor, backColor, font)
	{
		Group = group;
	}

	protected ListViewItem(SerializationInfo info, StreamingContext context)
	{
		Deserialize(info, context);
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		Serialize(info, context);
	}

	internal void OnUIATextChanged()
	{
		if (this.UIATextChanged != null)
		{
			this.UIATextChanged(this, EventArgs.Empty);
		}
	}

	internal void OnUIASubItemTextChanged(LabelEditEventArgs args)
	{
		if (args.Item == 0)
		{
			OnUIATextChanged();
		}
		if (this.UIASubItemTextChanged != null)
		{
			this.UIASubItemTextChanged(this, args);
		}
	}

	public void BeginEdit()
	{
		if (owner != null && owner.LabelEdit)
		{
			owner.item_control.BeginEdit(this);
		}
	}

	public virtual object Clone()
	{
		ListViewItem listViewItem = new ListViewItem();
		listViewItem.image_index = image_index;
		listViewItem.is_checked = is_checked;
		listViewItem.selected = selected;
		listViewItem.font = font;
		listViewItem.state_image_index = state_image_index;
		listViewItem.sub_items = new ListViewSubItemCollection(this, null);
		foreach (ListViewSubItem sub_item in sub_items)
		{
			listViewItem.sub_items.Add(sub_item.Text, sub_item.ForeColor, sub_item.BackColor, sub_item.Font);
		}
		listViewItem.tag = tag;
		listViewItem.use_item_style = use_item_style;
		listViewItem.owner = null;
		listViewItem.name = name;
		listViewItem.tooltip_text = tooltip_text;
		return listViewItem;
	}

	public virtual void EnsureVisible()
	{
		if (owner != null)
		{
			owner.EnsureVisible(owner.Items.IndexOf(this));
		}
	}

	public ListViewItem FindNearestItem(SearchDirectionHint searchDirection)
	{
		if (owner == null)
		{
			return null;
		}
		Point itemLocation = owner.GetItemLocation(display_index);
		return owner.FindNearestItem(searchDirection, itemLocation);
	}

	public Rectangle GetBounds(ItemBoundsPortion portion)
	{
		if (owner == null)
		{
			return Rectangle.Empty;
		}
		if (owner.VirtualMode && bounds == Rectangle.Empty)
		{
			Layout();
		}
		Rectangle result = portion switch
		{
			ItemBoundsPortion.Icon => icon_rect, 
			ItemBoundsPortion.Label => label_rect, 
			ItemBoundsPortion.ItemOnly => item_rect, 
			ItemBoundsPortion.Entire => bounds, 
			_ => throw new ArgumentException("Invalid value for portion."), 
		};
		Point itemLocation = owner.GetItemLocation(DisplayIndex);
		result.X += itemLocation.X;
		result.Y += itemLocation.Y;
		return result;
	}

	public ListViewSubItem GetSubItemAt(int x, int y)
	{
		if (owner != null && owner.View != View.Details)
		{
			return null;
		}
		foreach (ListViewSubItem sub_item in sub_items)
		{
			if (sub_item.Bounds.Contains(x, y))
			{
				return sub_item;
			}
		}
		return null;
	}

	public virtual void Remove()
	{
		if (owner != null)
		{
			owner.item_control.CancelEdit(this);
			owner.Items.Remove(this);
			owner = null;
		}
	}

	public override string ToString()
	{
		return $"ListViewItem: {Text}";
	}

	protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
	{
		sub_items = new ListViewSubItemCollection(this, null);
		int num = 0;
		SerializationInfoEnumerator enumerator = info.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SerializationEntry current = enumerator.Current;
			switch (current.Name)
			{
			case "Text":
				sub_items.Add((string)current.Value);
				break;
			case "Font":
				font = (Font)current.Value;
				break;
			case "Checked":
				is_checked = (bool)current.Value;
				break;
			case "ImageIndex":
				image_index = (int)current.Value;
				break;
			case "StateImageIndex":
				state_image_index = (int)current.Value;
				break;
			case "UseItemStyleForSubItems":
				use_item_style = (bool)current.Value;
				break;
			case "SubItemCount":
				num = (int)current.Value;
				break;
			case "Group":
				group = (ListViewGroup)current.Value;
				break;
			case "ImageKey":
				if (image_index == -1)
				{
					image_key = (string)current.Value;
				}
				break;
			}
		}
		Type typeFromHandle = typeof(ListViewSubItem);
		if (num > 0)
		{
			sub_items.Clear();
			Text = info.GetString("Text");
			for (int i = 0; i < num - 1; i++)
			{
				sub_items.Add((ListViewSubItem)info.GetValue("SubItem" + (i + 1), typeFromHandle));
			}
		}
		ForeColor = (Color)info.GetValue("ForeColor", typeof(Color));
		BackColor = (Color)info.GetValue("BackColor", typeof(Color));
	}

	protected virtual void Serialize(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Text", Text);
		info.AddValue("Font", Font);
		info.AddValue("ImageIndex", image_index);
		info.AddValue("Checked", is_checked);
		info.AddValue("StateImageIndex", state_image_index);
		info.AddValue("UseItemStyleForSubItems", use_item_style);
		info.AddValue("BackColor", BackColor);
		info.AddValue("ForeColor", ForeColor);
		info.AddValue("ImageKey", image_key);
		info.AddValue("Group", group);
		if (sub_items.Count > 1)
		{
			info.AddValue("SubItemCount", sub_items.Count);
			for (int i = 1; i < sub_items.Count; i++)
			{
				info.AddValue("SubItem" + i, sub_items[i]);
			}
		}
	}

	internal void SetGroup(ListViewGroup group)
	{
		this.group = group;
	}

	internal void SetPosition(Point position)
	{
		this.position = position;
	}

	private void UpdateFocusedState()
	{
		if (owner != null)
		{
			Invalidate();
			Layout();
			Invalidate();
		}
	}

	internal void Invalidate()
	{
		if (owner != null && owner.item_control != null && !owner.updating)
		{
			Rectangle rc = Bounds;
			rc.Inflate(1, 1);
			owner.item_control.Invalidate(rc);
		}
	}

	internal void Layout()
	{
		if (owner == null)
		{
			return;
		}
		Size text_size = owner.text_size;
		checkbox_rect = Rectangle.Empty;
		if (owner.CheckBoxes)
		{
			checkbox_rect.Size = owner.CheckBoxSize;
		}
		switch (owner.View)
		{
		case View.Details:
		{
			int num11 = 0;
			if (owner.SmallImageList != null)
			{
				num11 = indent_count * owner.SmallImageList.ImageSize.Width;
			}
			if (owner.Columns.Count > 0)
			{
				checkbox_rect.X = owner.Columns[0].Rect.X + num11;
			}
			icon_rect = (label_rect = Rectangle.Empty);
			icon_rect.X = checkbox_rect.Right + 2;
			int height = owner.ItemSize.Height;
			if (owner.SmallImageList != null)
			{
				icon_rect.Width = owner.SmallImageList.ImageSize.Width;
			}
			ref Rectangle reference = ref label_rect;
			int height2 = height;
			icon_rect.Height = height2;
			reference.Height = height2;
			checkbox_rect.Y = height - checkbox_rect.Height;
			label_rect.X = ((icon_rect.Width <= 0) ? icon_rect.Right : (icon_rect.Right + 1));
			if (owner.Columns.Count > 0)
			{
				label_rect.Width = owner.Columns[0].Wd - label_rect.X + checkbox_rect.X;
			}
			else
			{
				label_rect.Width = text_size.Width;
			}
			SizeF sizeF3 = TextRenderer.MeasureString(Text, Font);
			text_bounds = label_rect;
			text_bounds.Width = (int)sizeF3.Width;
			Rectangle rectangle = (item_rect = Rectangle.Union(Rectangle.Union(checkbox_rect, icon_rect), label_rect));
			bounds.Size = rectangle.Size;
			item_rect.Width = 0;
			bounds.Width = 0;
			for (int k = 0; k < owner.Columns.Count; k++)
			{
				item_rect.Width += owner.Columns[k].Wd;
				bounds.Width += owner.Columns[k].Wd;
			}
			int num12 = Math.Min(owner.Columns.Count, sub_items.Count);
			for (int l = 0; l < num12; l++)
			{
				Rectangle rect = owner.Columns[l].Rect;
				sub_items[l].SetBounds(rect.X, 0, rect.Width, height);
			}
			break;
		}
		case View.LargeIcon:
		{
			label_rect = (icon_rect = Rectangle.Empty);
			SizeF sizeF2 = TextRenderer.MeasureString(Text, Font);
			if ((int)sizeF2.Width > text_size.Width)
			{
				if (Focused && owner.InternalContainsFocus)
				{
					int width = text_size.Width;
					StringFormat stringFormat = new StringFormat();
					stringFormat.Alignment = StringAlignment.Center;
					text_size.Height = (int)TextRenderer.MeasureString(Text, Font, width, stringFormat).Height;
				}
				else
				{
					text_size.Height = 2 * (int)sizeF2.Height;
				}
			}
			if (owner.LargeImageList != null)
			{
				icon_rect.Width = owner.LargeImageList.ImageSize.Width;
				icon_rect.Height = owner.LargeImageList.ImageSize.Height;
			}
			if (checkbox_rect.Height > icon_rect.Height)
			{
				icon_rect.Y = checkbox_rect.Height - icon_rect.Height;
			}
			else
			{
				checkbox_rect.Y = icon_rect.Height - checkbox_rect.Height;
			}
			if (text_size.Width <= icon_rect.Width)
			{
				icon_rect.X = checkbox_rect.Width + 1;
				label_rect.X = icon_rect.X + (icon_rect.Width - text_size.Width) / 2;
				label_rect.Y = icon_rect.Bottom + 2;
				label_rect.Size = text_size;
			}
			else
			{
				int num10 = text_size.Width / 2;
				icon_rect.X = checkbox_rect.Width + 1 + num10 - icon_rect.Width / 2;
				label_rect.X = checkbox_rect.Width + 1;
				label_rect.Y = icon_rect.Bottom + 2;
				label_rect.Size = text_size;
			}
			item_rect = Rectangle.Union(icon_rect, label_rect);
			Rectangle rectangle = Rectangle.Union(item_rect, checkbox_rect);
			bounds.Size = rectangle.Size;
			break;
		}
		case View.SmallIcon:
		case View.List:
		{
			label_rect = (icon_rect = Rectangle.Empty);
			icon_rect.X = checkbox_rect.Width + 1;
			int height = Math.Max(owner.CheckBoxSize.Height, text_size.Height);
			if (owner.SmallImageList != null)
			{
				height = Math.Max(height, owner.SmallImageList.ImageSize.Height);
				icon_rect.Width = owner.SmallImageList.ImageSize.Width;
				icon_rect.Height = owner.SmallImageList.ImageSize.Height;
			}
			checkbox_rect.Y = height - checkbox_rect.Height;
			label_rect.X = icon_rect.Right + 1;
			label_rect.Width = text_size.Width;
			ref Rectangle reference2 = ref label_rect;
			int height2 = height;
			icon_rect.Height = height2;
			reference2.Height = height2;
			item_rect = Rectangle.Union(icon_rect, label_rect);
			Rectangle rectangle = Rectangle.Union(item_rect, checkbox_rect);
			bounds.Size = rectangle.Size;
			break;
		}
		case View.Tile:
		{
			if (!Application.VisualStylesEnabled)
			{
				goto case View.LargeIcon;
			}
			label_rect = (icon_rect = Rectangle.Empty);
			if (owner.LargeImageList != null)
			{
				icon_rect.Width = owner.LargeImageList.ImageSize.Width;
				icon_rect.Height = owner.LargeImageList.ImageSize.Height;
			}
			int num = 2;
			SizeF sizeF = TextRenderer.MeasureString(Text, Font);
			int num2 = (int)Math.Ceiling(sizeF.Height);
			int num3 = (int)Math.Ceiling(sizeF.Width);
			sub_items[0].bounds.Height = num2;
			int num4 = num2;
			int num5 = num3;
			int num6 = Math.Min(owner.Columns.Count, sub_items.Count);
			for (int i = 1; i < num6; i++)
			{
				ListViewSubItem listViewSubItem = sub_items[i];
				if (listViewSubItem.Text != null && listViewSubItem.Text.Length != 0)
				{
					sizeF = TextRenderer.MeasureString(listViewSubItem.Text, listViewSubItem.Font);
					int num7 = (int)Math.Ceiling(sizeF.Width);
					if (num7 > num5)
					{
						num5 = num7;
					}
					int num8 = (int)Math.Ceiling(sizeF.Height);
					num4 += num8 + num;
					listViewSubItem.bounds.Height = num8;
				}
			}
			num5 = Math.Min(num5, owner.TileSize.Width - (icon_rect.Width + 4));
			label_rect.X = icon_rect.Right + 4;
			label_rect.Y = owner.TileSize.Height / 2 - num4 / 2;
			label_rect.Width = num5;
			label_rect.Height = num4;
			sub_items[0].SetBounds(label_rect.X, label_rect.Y, num5, sub_items[0].bounds.Height);
			int num9 = sub_items[0].bounds.Bottom + num;
			for (int j = 1; j < num6; j++)
			{
				ListViewSubItem listViewSubItem2 = sub_items[j];
				if (listViewSubItem2.Text != null && listViewSubItem2.Text.Length != 0)
				{
					listViewSubItem2.SetBounds(label_rect.X, num9, num5, listViewSubItem2.bounds.Height);
					num9 += listViewSubItem2.Bounds.Height + num;
				}
			}
			item_rect = Rectangle.Union(icon_rect, label_rect);
			bounds.Size = item_rect.Size;
			break;
		}
		}
	}
}
