using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Theming;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

[ComVisible(true)]
[DefaultProperty("TabPages")]
[Designer("System.Windows.Forms.Design.TabControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent("SelectedIndexChanged")]
public class TabControl : Control
{
	[ComVisible(false)]
	public new class ControlCollection : Control.ControlCollection
	{
		private TabControl owner;

		public ControlCollection(TabControl owner)
			: base(owner)
		{
			this.owner = owner;
		}

		public override void Add(Control value)
		{
			if (!(value is TabPage tabPage))
			{
				throw new ArgumentException("Cannot add " + value.GetType().Name + " to TabControl. Only TabPages can be directly added to TabControls.");
			}
			tabPage.SetVisible(value: false);
			base.Add(value);
			if (owner.TabCount == 1 && owner.selected_index < 0)
			{
				owner.SelectedIndex = 0;
			}
			owner.Redraw();
		}

		public override void Remove(Control value)
		{
			bool flag = false;
			if (value is TabPage tabPage && owner.Controls.Contains(tabPage))
			{
				int num = owner.IndexForTabPage(tabPage);
				if (num < owner.SelectedIndex || owner.SelectedIndex == Count - 1)
				{
					flag = true;
				}
			}
			base.Remove(value);
			if (flag && Count > 0)
			{
				int selectedIndex = owner.SelectedIndex;
				owner.selected_index = -1;
				selectedIndex = (owner.SelectedIndex = selectedIndex - 1);
			}
			else if (flag)
			{
				owner.selected_index = -1;
				owner.OnSelectedIndexChanged(EventArgs.Empty);
			}
			else
			{
				owner.Redraw();
			}
		}
	}

	public class TabPageCollection : ICollection, IEnumerable, IList
	{
		private TabControl owner;

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => false;

		object IList.this[int index]
		{
			get
			{
				return owner.GetTab(index);
			}
			set
			{
				owner.SetTab(index, (TabPage)value);
			}
		}

		[Browsable(false)]
		public int Count => owner.Controls.Count;

		public bool IsReadOnly => false;

		public virtual TabPage this[int index]
		{
			get
			{
				return owner.GetTab(index);
			}
			set
			{
				owner.SetTab(index, value);
			}
		}

		public virtual TabPage this[string key]
		{
			get
			{
				if (string.IsNullOrEmpty(key))
				{
					return null;
				}
				int num = IndexOfKey(key);
				if (num < 0 || num >= Count)
				{
					return null;
				}
				return this[num];
			}
		}

		internal int this[TabPage tabPage]
		{
			get
			{
				if (tabPage == null)
				{
					return -1;
				}
				for (int i = 0; i < Count; i++)
				{
					if (this[i].Equals(tabPage))
					{
						return i;
					}
				}
				return -1;
			}
		}

		public TabPageCollection(TabControl owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("Value cannot be null.");
			}
			this.owner = owner;
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			owner.Controls.CopyTo(dest, index);
		}

		int IList.Add(object value)
		{
			TabPage tabPage = value as TabPage;
			if (value == null)
			{
				throw new ArgumentException("value");
			}
			owner.Controls.Add(tabPage);
			return owner.Controls.IndexOf(tabPage);
		}

		bool IList.Contains(object page)
		{
			if (!(page is TabPage page2))
			{
				return false;
			}
			return Contains(page2);
		}

		int IList.IndexOf(object page)
		{
			if (!(page is TabPage page2))
			{
				return -1;
			}
			return IndexOf(page2);
		}

		void IList.Insert(int index, object tabPage)
		{
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			if (value is TabPage)
			{
				Remove((TabPage)value);
			}
		}

		public void Add(TabPage value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("Value cannot be null.");
			}
			owner.Controls.Add(value);
		}

		public void Add(string text)
		{
			TabPage value = new TabPage(text);
			Add(value);
		}

		public void Add(string key, string text)
		{
			TabPage tabPage = new TabPage(text);
			tabPage.Name = key;
			Add(tabPage);
		}

		public void Add(string key, string text, int imageIndex)
		{
			TabPage tabPage = new TabPage(text);
			tabPage.Name = key;
			tabPage.ImageIndex = imageIndex;
			Add(tabPage);
		}

		public void Add(string key, string text, string imageKey)
		{
			TabPage tabPage = new TabPage(text);
			tabPage.Name = key;
			tabPage.ImageKey = imageKey;
			Add(tabPage);
		}

		public void AddRange(TabPage[] pages)
		{
			if (pages == null)
			{
				throw new ArgumentNullException("Value cannot be null.");
			}
			owner.Controls.AddRange(pages);
		}

		public virtual void Clear()
		{
			owner.Controls.Clear();
			owner.Invalidate();
		}

		public bool Contains(TabPage page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("Value cannot be null.");
			}
			return owner.Controls.Contains(page);
		}

		public virtual bool ContainsKey(string key)
		{
			int num = IndexOfKey(key);
			return num >= 0 && num < Count;
		}

		public IEnumerator GetEnumerator()
		{
			return owner.Controls.GetEnumerator();
		}

		public int IndexOf(TabPage page)
		{
			return owner.Controls.IndexOf(page);
		}

		public virtual int IndexOfKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return -1;
			}
			for (int i = 0; i < Count; i++)
			{
				if (string.Compare(this[i].Name, key, ignoreCase: true, CultureInfo.InvariantCulture) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public void Remove(TabPage value)
		{
			owner.Controls.Remove(value);
			owner.Invalidate();
		}

		public void RemoveAt(int index)
		{
			owner.Controls.RemoveAt(index);
			owner.Invalidate();
		}

		public virtual void RemoveByKey(string key)
		{
			int num = IndexOfKey(key);
			if (num >= 0 && num < Count)
			{
				RemoveAt(num);
			}
		}

		public void Insert(int index, string text)
		{
			owner.InsertTab(index, new TabPage(text));
		}

		public void Insert(int index, TabPage tabPage)
		{
			owner.InsertTab(index, tabPage);
		}

		public void Insert(int index, string key, string text)
		{
			TabPage tabPage = new TabPage(text);
			tabPage.Name = key;
			owner.InsertTab(index, tabPage);
		}

		public void Insert(int index, string key, string text, int imageIndex)
		{
			TabPage tabPage = new TabPage(text);
			tabPage.Name = key;
			owner.InsertTab(index, tabPage);
			tabPage.ImageIndex = imageIndex;
		}

		public void Insert(int index, string key, string text, string imageKey)
		{
			TabPage tabPage = new TabPage(text);
			tabPage.Name = key;
			owner.InsertTab(index, tabPage);
			tabPage.ImageKey = imageKey;
		}
	}

	private int selected_index = -1;

	private TabAlignment alignment;

	private TabAppearance appearance;

	private TabDrawMode draw_mode;

	private bool multiline;

	private ImageList image_list;

	private Size item_size = Size.Empty;

	private Point padding;

	private int row_count;

	private bool hottrack;

	private TabPageCollection tab_pages;

	private bool show_tool_tips;

	private TabSizeMode size_mode;

	private bool show_slider;

	private PushButtonState right_slider_state = PushButtonState.Normal;

	private PushButtonState left_slider_state = PushButtonState.Normal;

	private int slider_pos;

	private TabPage entered_tab_page;

	private bool mouse_down_on_a_tab_page;

	private bool rightToLeftLayout;

	private static object UIAHorizontallyScrollableChangedEvent;

	private static object UIAHorizontallyScrolledEvent;

	private static object DrawItemEvent;

	private static object SelectedIndexChangedEvent;

	private static object SelectedEvent;

	private static object DeselectedEvent;

	private static object SelectingEvent;

	private static object DeselectingEvent;

	private static object RightToLeftLayoutChangedEvent;

	internal double UIAHorizontalViewSize => LeftScrollButtonArea.Left * 100 / TabPages[TabCount - 1].TabBounds.Right;

	[RefreshProperties(RefreshProperties.All)]
	[Localizable(true)]
	[DefaultValue(TabAlignment.Top)]
	public TabAlignment Alignment
	{
		get
		{
			return alignment;
		}
		set
		{
			if (alignment != value)
			{
				alignment = value;
				if (alignment == TabAlignment.Left || alignment == TabAlignment.Right)
				{
					multiline = true;
				}
				Redraw();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(TabAppearance.Normal)]
	public TabAppearance Appearance
	{
		get
		{
			return appearance;
		}
		set
		{
			if (appearance != value)
			{
				appearance = value;
				Redraw();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color BackColor
	{
		get
		{
			return ThemeEngine.Current.ColorControl;
		}
		set
		{
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

	public override Rectangle DisplayRectangle => ThemeEngine.Current.TabControlGetDisplayRectangle(this);

	[EditorBrowsable(EditorBrowsableState.Never)]
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

	[DefaultValue(TabDrawMode.Normal)]
	public TabDrawMode DrawMode
	{
		get
		{
			return draw_mode;
		}
		set
		{
			if (draw_mode != value)
			{
				draw_mode = value;
				Redraw();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color ForeColor
	{
		get
		{
			return base.ForeColor;
		}
		set
		{
			base.ForeColor = value;
		}
	}

	[DefaultValue(false)]
	public bool HotTrack
	{
		get
		{
			return hottrack;
		}
		set
		{
			if (hottrack != value)
			{
				hottrack = value;
				Redraw();
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[DefaultValue(null)]
	public ImageList ImageList
	{
		get
		{
			return image_list;
		}
		set
		{
			image_list = value;
		}
	}

	[Localizable(true)]
	public Size ItemSize
	{
		get
		{
			return item_size;
		}
		set
		{
			if (value.Height < 0 || value.Width < 0)
			{
				throw new ArgumentException(string.Concat("'", value, "' is not a valid value for 'ItemSize'."));
			}
			item_size = value;
			Redraw();
		}
	}

	[DefaultValue(false)]
	public bool Multiline
	{
		get
		{
			return multiline;
		}
		set
		{
			if (multiline != value)
			{
				multiline = value;
				if ((!multiline && alignment == TabAlignment.Left) || alignment == TabAlignment.Right)
				{
					alignment = TabAlignment.Top;
				}
				Redraw();
			}
		}
	}

	[Localizable(true)]
	public new Point Padding
	{
		get
		{
			return padding;
		}
		set
		{
			if (value.X < 0 || value.Y < 0)
			{
				throw new ArgumentException(string.Concat("'", value, "' is not a valid value for 'Padding'."));
			}
			if (!(padding == value))
			{
				padding = value;
				Redraw();
			}
		}
	}

	[Localizable(true)]
	[System.MonoTODO("RTL not supported")]
	[DefaultValue(false)]
	public virtual bool RightToLeftLayout
	{
		get
		{
			return rightToLeftLayout;
		}
		set
		{
			if (value != rightToLeftLayout)
			{
				rightToLeftLayout = value;
				OnRightToLeftLayoutChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int RowCount => row_count;

	[Browsable(false)]
	[DefaultValue(-1)]
	public int SelectedIndex
	{
		get
		{
			return selected_index;
		}
		set
		{
			if (value < -1)
			{
				throw new ArgumentOutOfRangeException("SelectedIndex", "Value of '" + value + "' is valid for 'SelectedIndex'. 'SelectedIndex' must be greater than or equal to -1.");
			}
			if (!base.IsHandleCreated)
			{
				if (selected_index != value)
				{
					selected_index = value;
				}
				return;
			}
			if (value >= TabCount)
			{
				if (value != selected_index)
				{
					OnSelectedIndexChanged(EventArgs.Empty);
				}
				return;
			}
			if (value == selected_index)
			{
				if (selected_index > -1)
				{
					Invalidate(GetTabRect(selected_index));
				}
				return;
			}
			TabControlCancelEventArgs tabControlCancelEventArgs = new TabControlCancelEventArgs(SelectedTab, selected_index, cancel: false, TabControlAction.Deselecting);
			OnDeselecting(tabControlCancelEventArgs);
			if (tabControlCancelEventArgs.Cancel)
			{
				return;
			}
			Focus();
			int num = selected_index;
			int num2 = (selected_index = value);
			tabControlCancelEventArgs = new TabControlCancelEventArgs(SelectedTab, selected_index, cancel: false, TabControlAction.Selecting);
			OnSelecting(tabControlCancelEventArgs);
			if (tabControlCancelEventArgs.Cancel)
			{
				selected_index = num;
				return;
			}
			SuspendLayout();
			Rectangle rectangle = Rectangle.Empty;
			bool flag = false;
			if (num2 != -1 && show_slider && num2 < slider_pos)
			{
				slider_pos = num2;
				flag = true;
			}
			if (num2 != -1)
			{
				int right = TabPages[num2].TabBounds.Right;
				int left = LeftScrollButtonArea.Left;
				if (show_slider && right > left)
				{
					int num3 = 0;
					for (num3 = 0; num3 < num2 - 1; num3++)
					{
						if (TabPages[num3].TabBounds.Left >= 0 && TabPages[num2].TabBounds.Right - TabPages[num3].TabBounds.Right < left)
						{
							num3++;
							break;
						}
					}
					slider_pos = num3;
					flag = true;
				}
			}
			if (num != -1 && num2 != -1)
			{
				if (!flag)
				{
					rectangle = GetTabRect(num);
				}
				((TabPage)base.Controls[num]).SetVisible(value: false);
			}
			TabPage tabPage = null;
			if (num2 != -1)
			{
				tabPage = (TabPage)base.Controls[num2];
				rectangle = Rectangle.Union(rectangle, GetTabRect(num2));
				tabPage.SetVisible(value: true);
			}
			OnSelectedIndexChanged(EventArgs.Empty);
			ResumeLayout();
			if (flag)
			{
				SizeTabs();
				Refresh();
				return;
			}
			if (num2 != -1 && tabPage.Row != BottomRow)
			{
				DropRow(TabPages[num2].Row);
				SizeTabs();
				Refresh();
				return;
			}
			SizeTabs();
			if (appearance == TabAppearance.Normal)
			{
				rectangle.Inflate(6, 4);
				rectangle.Intersect(base.ClientRectangle);
			}
			Invalidate(rectangle);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public TabPage SelectedTab
	{
		get
		{
			if (selected_index == -1)
			{
				return null;
			}
			return tab_pages[selected_index];
		}
		set
		{
			int num = IndexForTabPage(value);
			if (num != selected_index)
			{
				SelectedIndex = num;
			}
		}
	}

	[DefaultValue(false)]
	[Localizable(true)]
	public bool ShowToolTips
	{
		get
		{
			return show_tool_tips;
		}
		set
		{
			if (show_tool_tips != value)
			{
				show_tool_tips = value;
				Redraw();
			}
		}
	}

	[DefaultValue(TabSizeMode.Normal)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public TabSizeMode SizeMode
	{
		get
		{
			return size_mode;
		}
		set
		{
			if (size_mode != value)
			{
				size_mode = value;
				Redraw();
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int TabCount => tab_pages.Count;

	[Editor("System.Windows.Forms.Design.TabPageCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[MergableProperty(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public TabPageCollection TabPages => tab_pages;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[Bindable(false)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	internal bool ShowSlider
	{
		get
		{
			return show_slider;
		}
		set
		{
			show_slider = value;
			OnUIAHorizontallyScrollableChanged(EventArgs.Empty);
		}
	}

	internal int SliderPos => slider_pos;

	internal PushButtonState RightSliderState
	{
		get
		{
			return right_slider_state;
		}
		private set
		{
			if (right_slider_state != value)
			{
				PushButtonState oldState = right_slider_state;
				right_slider_state = value;
				if (NeedsToInvalidateScrollButton(oldState, value))
				{
					Invalidate(RightScrollButtonArea);
				}
			}
		}
	}

	internal PushButtonState LeftSliderState
	{
		get
		{
			return left_slider_state;
		}
		set
		{
			if (left_slider_state != value)
			{
				PushButtonState oldState = left_slider_state;
				left_slider_state = value;
				if (NeedsToInvalidateScrollButton(oldState, value))
				{
					Invalidate(LeftScrollButtonArea);
				}
			}
		}
	}

	internal TabPage EnteredTabPage
	{
		get
		{
			return entered_tab_page;
		}
		private set
		{
			if (entered_tab_page == value)
			{
				return;
			}
			if (HasHotElementStyles)
			{
				Region region = new Region();
				region.MakeEmpty();
				if (entered_tab_page != null)
				{
					region.Union(entered_tab_page.TabBounds);
				}
				entered_tab_page = value;
				if (entered_tab_page != null)
				{
					region.Union(entered_tab_page.TabBounds);
				}
				Invalidate(region);
				region.Dispose();
			}
			else
			{
				entered_tab_page = value;
			}
		}
	}

	protected override CreateParams CreateParams => base.CreateParams;

	protected override Size DefaultSize => new Size(200, 100);

	private bool CanScrollRight => slider_pos < TabCount - 1;

	private bool CanScrollLeft => slider_pos > 0;

	private bool HasHotElementStyles => ThemeElements.CurrentTheme.TabControlPainter.HasHotElementStyles(this);

	private Rectangle LeftScrollButtonArea => ThemeElements.CurrentTheme.TabControlPainter.GetLeftScrollRect(this);

	private Rectangle RightScrollButtonArea => ThemeElements.CurrentTheme.TabControlPainter.GetRightScrollRect(this);

	private int MinimumTabWidth => ThemeEngine.Current.TabControlMinimumTabWidth;

	private Size TabSpacing => ThemeEngine.Current.TabControlGetSpacing(this);

	private int BottomRow => 1;

	private int Direction => 1;

	internal event EventHandler UIAHorizontallyScrollableChanged
	{
		add
		{
			base.Events.AddHandler(UIAHorizontallyScrollableChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAHorizontallyScrollableChangedEvent, value);
		}
	}

	internal event EventHandler UIAHorizontallyScrolled
	{
		add
		{
			base.Events.AddHandler(UIAHorizontallyScrolledEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAHorizontallyScrolledEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler BackColorChanged
	{
		add
		{
			base.BackColorChanged += value;
		}
		remove
		{
			base.BackColorChanged -= value;
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

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler ForeColorChanged
	{
		add
		{
			base.ForeColorChanged += value;
		}
		remove
		{
			base.ForeColorChanged -= value;
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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

	public event TabControlEventHandler Selected
	{
		add
		{
			base.Events.AddHandler(SelectedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectedEvent, value);
		}
	}

	public event TabControlEventHandler Deselected
	{
		add
		{
			base.Events.AddHandler(DeselectedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DeselectedEvent, value);
		}
	}

	public event TabControlCancelEventHandler Selecting
	{
		add
		{
			base.Events.AddHandler(SelectingEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectingEvent, value);
		}
	}

	public event TabControlCancelEventHandler Deselecting
	{
		add
		{
			base.Events.AddHandler(DeselectingEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DeselectingEvent, value);
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

	public TabControl()
	{
		tab_pages = new TabPageCollection(this);
		SetStyle(ControlStyles.UserPaint, value: false);
		padding = ThemeEngine.Current.TabControlDefaultPadding;
		item_size = ThemeEngine.Current.TabControlDefaultItemSize;
		base.MouseDown += MouseDownHandler;
		base.MouseLeave += OnMouseLeave;
		base.MouseMove += OnMouseMove;
		base.MouseUp += MouseUpHandler;
		base.SizeChanged += SizeChangedHandler;
	}

	static TabControl()
	{
		UIAHorizontallyScrollableChanged = new object();
		UIAHorizontallyScrolled = new object();
		DrawItem = new object();
		SelectedIndexChanged = new object();
		Selected = new object();
		Deselected = new object();
		Selecting = new object();
		Deselecting = new object();
		RightToLeftLayoutChanged = new object();
	}

	internal void OnUIAHorizontallyScrollableChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIAHorizontallyScrollableChanged])?.Invoke(this, e);
	}

	internal void OnUIAHorizontallyScrolled(EventArgs e)
	{
		((EventHandler)base.Events[UIAHorizontallyScrolled])?.Invoke(this, e);
	}

	private bool NeedsToInvalidateScrollButton(PushButtonState oldState, PushButtonState newState)
	{
		if ((oldState == PushButtonState.Hot && newState == PushButtonState.Normal) || (oldState == PushButtonState.Normal && newState == PushButtonState.Hot))
		{
			return HasHotElementStyles;
		}
		return true;
	}

	public Rectangle GetTabRect(int index)
	{
		TabPage tab = GetTab(index);
		return tab.TabBounds;
	}

	public Control GetControl(int index)
	{
		return GetTab(index);
	}

	public void SelectTab(TabPage tabPage)
	{
		if (tabPage == null)
		{
			throw new ArgumentNullException("tabPage");
		}
		SelectTab(tab_pages[tabPage]);
	}

	public void SelectTab(string tabPageName)
	{
		if (tabPageName == null)
		{
			throw new ArgumentNullException("tabPageName");
		}
		SelectTab(tab_pages[tabPageName]);
	}

	public void SelectTab(int index)
	{
		if (index < 0 || index > tab_pages.Count - 1)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		SelectedIndex = index;
	}

	public void DeselectTab(TabPage tabPage)
	{
		if (tabPage == null)
		{
			throw new ArgumentNullException("tabPage");
		}
		DeselectTab(tab_pages[tabPage]);
	}

	public void DeselectTab(string tabPageName)
	{
		if (tabPageName == null)
		{
			throw new ArgumentNullException("tabPageName");
		}
		DeselectTab(tab_pages[tabPageName]);
	}

	public void DeselectTab(int index)
	{
		if (index == SelectedIndex)
		{
			if (index >= 0 && index < tab_pages.Count - 1)
			{
				SelectedIndex = ++index;
			}
			else
			{
				SelectedIndex = 0;
			}
		}
	}

	public override string ToString()
	{
		string text = base.ToString() + ", TabPages.Count: " + TabCount;
		if (TabCount > 0)
		{
			text = text + ", TabPages[0]: " + TabPages[0];
		}
		return text;
	}

	protected override Control.ControlCollection CreateControlsInstance()
	{
		return new ControlCollection(this);
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
		selected_index = ((selected_index < TabCount) ? selected_index : ((TabCount <= 0) ? (-1) : 0));
		if (TabCount > 0)
		{
			if (selected_index > -1)
			{
				SelectedTab.SetVisible(value: true);
			}
			else
			{
				tab_pages[0].SetVisible(value: true);
			}
		}
		ResizeTabPages();
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected virtual void OnDrawItem(DrawItemEventArgs e)
	{
		if (DrawMode == TabDrawMode.OwnerDrawFixed)
		{
			((DrawItemEventHandler)base.Events[DrawItem])?.Invoke(this, e);
		}
	}

	internal void OnDrawItemInternal(DrawItemEventArgs e)
	{
		OnDrawItem(e);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		ResizeTabPages();
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
	}

	protected override void OnStyleChanged(EventArgs e)
	{
		base.OnStyleChanged(e);
	}

	protected virtual void OnSelectedIndexChanged(EventArgs e)
	{
		((EventHandler)base.Events[SelectedIndexChanged])?.Invoke(this, e);
	}

	internal override void OnPaintInternal(PaintEventArgs pe)
	{
		if (!GetStyle(ControlStyles.UserPaint))
		{
			Draw(pe.Graphics, pe.ClipRectangle);
			pe.Handled = true;
		}
	}

	protected override void OnEnter(EventArgs e)
	{
		base.OnEnter(e);
		if (SelectedTab != null)
		{
			SelectedTab.FireEnter();
		}
	}

	protected override void OnLeave(EventArgs e)
	{
		if (SelectedTab != null)
		{
			SelectedTab.FireLeave();
		}
		base.OnLeave(e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
	{
		((EventHandler)base.Events[RightToLeftLayoutChanged])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void ScaleCore(float dx, float dy)
	{
		base.ScaleCore(dx, dy);
	}

	protected virtual void OnDeselecting(TabControlCancelEventArgs e)
	{
		((TabControlCancelEventHandler)base.Events[Deselecting])?.Invoke(this, e);
		if (!e.Cancel)
		{
			OnDeselected(new TabControlEventArgs(SelectedTab, selected_index, TabControlAction.Deselected));
		}
	}

	protected virtual void OnDeselected(TabControlEventArgs e)
	{
		((TabControlEventHandler)base.Events[Deselected])?.Invoke(this, e);
		if (SelectedTab != null)
		{
			SelectedTab.FireLeave();
		}
	}

	protected virtual void OnSelecting(TabControlCancelEventArgs e)
	{
		((TabControlCancelEventHandler)base.Events[Selecting])?.Invoke(this, e);
		if (!e.Cancel)
		{
			OnSelected(new TabControlEventArgs(SelectedTab, selected_index, TabControlAction.Selected));
		}
	}

	protected virtual void OnSelected(TabControlEventArgs e)
	{
		((TabControlEventHandler)base.Events[Selected])?.Invoke(this, e);
		if (SelectedTab != null)
		{
			SelectedTab.FireEnter();
		}
	}

	protected override bool ProcessKeyPreview(ref Message m)
	{
		return base.ProcessKeyPreview(ref m);
	}

	protected override void OnKeyDown(KeyEventArgs ke)
	{
		base.OnKeyDown(ke);
		if (ke.Handled)
		{
			return;
		}
		if (ke.KeyCode == Keys.Tab && (ke.KeyData & Keys.Control) != 0)
		{
			if ((ke.KeyData & Keys.Shift) == 0)
			{
				SelectedIndex = (SelectedIndex + 1) % TabCount;
			}
			else
			{
				SelectedIndex = (SelectedIndex + TabCount - 1) % TabCount;
			}
			ke.Handled = true;
		}
		else if (ke.KeyCode == Keys.Home)
		{
			SelectedIndex = 0;
			ke.Handled = true;
		}
		else if (ke.KeyCode == Keys.End)
		{
			SelectedIndex = TabCount - 1;
			ke.Handled = true;
		}
		else if (NavigateTabs(ke.KeyCode))
		{
			ke.Handled = true;
		}
	}

	protected override bool IsInputKey(Keys keyData)
	{
		switch (keyData & Keys.KeyCode)
		{
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

	private bool NavigateTabs(Keys keycode)
	{
		bool flag = false;
		bool flag2 = false;
		if (alignment == TabAlignment.Bottom || alignment == TabAlignment.Top)
		{
			switch (keycode)
			{
			case Keys.Left:
				flag = true;
				break;
			case Keys.Right:
				flag2 = true;
				break;
			}
		}
		else
		{
			switch (keycode)
			{
			case Keys.Up:
				flag = true;
				break;
			case Keys.Down:
				flag2 = true;
				break;
			}
		}
		if (flag && SelectedIndex > 0)
		{
			SelectedIndex--;
			return true;
		}
		if (flag2 && SelectedIndex < TabCount - 1)
		{
			SelectedIndex++;
			return true;
		}
		return false;
	}

	protected void RemoveAll()
	{
		base.Controls.Clear();
	}

	protected virtual object[] GetItems()
	{
		TabPage[] array = new TabPage[base.Controls.Count];
		base.Controls.CopyTo(array, 0);
		return array;
	}

	protected virtual object[] GetItems(Type baseType)
	{
		object[] array = (object[])Array.CreateInstance(baseType, base.Controls.Count);
		base.Controls.CopyTo(array, 0);
		return array;
	}

	protected void UpdateTabSelection(bool updateFocus)
	{
		ResizeTabPages();
	}

	protected string GetToolTipText(object item)
	{
		TabPage tabPage = (TabPage)item;
		return tabPage.ToolTipText;
	}

	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_SETFOCUS:
			if (selected_index == -1 && TabCount > 0)
			{
				SelectedIndex = 0;
			}
			if (selected_index != -1)
			{
				Invalidate(GetTabRect(selected_index));
			}
			base.WndProc(ref m);
			break;
		case Msg.WM_KILLFOCUS:
			if (selected_index != -1)
			{
				Invalidate(GetTabRect(selected_index));
			}
			base.WndProc(ref m);
			break;
		default:
			base.WndProc(ref m);
			break;
		}
	}

	private void MouseDownHandler(object sender, MouseEventArgs e)
	{
		if ((e.Button & MouseButtons.Left) == 0)
		{
			return;
		}
		if (ShowSlider)
		{
			Rectangle rightScrollButtonArea = RightScrollButtonArea;
			Rectangle leftScrollButtonArea = LeftScrollButtonArea;
			if (rightScrollButtonArea.Contains(e.X, e.Y))
			{
				right_slider_state = PushButtonState.Pressed;
				if (CanScrollRight)
				{
					slider_pos++;
					SizeTabs();
					OnUIAHorizontallyScrolled(EventArgs.Empty);
					switch (Alignment)
					{
					case TabAlignment.Top:
						Invalidate(new Rectangle(0, 0, base.Width, ItemSize.Height));
						break;
					case TabAlignment.Bottom:
						Invalidate(new Rectangle(0, DisplayRectangle.Bottom, base.Width, base.Height - DisplayRectangle.Bottom));
						break;
					case TabAlignment.Left:
						Invalidate(new Rectangle(0, 0, DisplayRectangle.Left, base.Height));
						break;
					case TabAlignment.Right:
						Invalidate(new Rectangle(DisplayRectangle.Right, 0, base.Width - DisplayRectangle.Right, base.Height));
						break;
					}
				}
				else
				{
					Invalidate(rightScrollButtonArea);
				}
				return;
			}
			if (leftScrollButtonArea.Contains(e.X, e.Y))
			{
				left_slider_state = PushButtonState.Pressed;
				if (CanScrollLeft)
				{
					slider_pos--;
					SizeTabs();
					OnUIAHorizontallyScrolled(EventArgs.Empty);
					switch (Alignment)
					{
					case TabAlignment.Top:
						Invalidate(new Rectangle(0, 0, base.Width, ItemSize.Height));
						break;
					case TabAlignment.Bottom:
						Invalidate(new Rectangle(0, DisplayRectangle.Bottom, base.Width, base.Height - DisplayRectangle.Bottom));
						break;
					case TabAlignment.Left:
						Invalidate(new Rectangle(0, 0, DisplayRectangle.Left, base.Height));
						break;
					case TabAlignment.Right:
						Invalidate(new Rectangle(DisplayRectangle.Right, 0, base.Width - DisplayRectangle.Right, base.Height));
						break;
					}
				}
				else
				{
					Invalidate(leftScrollButtonArea);
				}
				return;
			}
		}
		int count = base.Controls.Count;
		for (int i = SliderPos; i < count; i++)
		{
			if (GetTabRect(i).Contains(e.X, e.Y))
			{
				SelectedIndex = i;
				mouse_down_on_a_tab_page = true;
				break;
			}
		}
	}

	private void MouseUpHandler(object sender, MouseEventArgs e)
	{
		mouse_down_on_a_tab_page = false;
		if (ShowSlider && (left_slider_state == PushButtonState.Pressed || right_slider_state == PushButtonState.Pressed))
		{
			Rectangle rectangle;
			if (left_slider_state == PushButtonState.Pressed)
			{
				rectangle = LeftScrollButtonArea;
				left_slider_state = GetScrollButtonState(rectangle, e.Location);
			}
			else
			{
				rectangle = RightScrollButtonArea;
				right_slider_state = GetScrollButtonState(rectangle, e.Location);
			}
			Invalidate(rectangle);
		}
	}

	private static PushButtonState GetScrollButtonState(Rectangle scrollButtonArea, Point cursorLocation)
	{
		return (!scrollButtonArea.Contains(cursorLocation)) ? PushButtonState.Normal : PushButtonState.Hot;
	}

	private void SizeChangedHandler(object sender, EventArgs e)
	{
		Redraw();
	}

	internal int IndexForTabPage(TabPage page)
	{
		for (int i = 0; i < tab_pages.Count; i++)
		{
			if (page == tab_pages[i])
			{
				return i;
			}
		}
		return -1;
	}

	private void ResizeTabPages()
	{
		CalcTabRows();
		SizeTabs();
		Rectangle displayRectangle = DisplayRectangle;
		foreach (TabPage control in base.Controls)
		{
			control.Bounds = displayRectangle;
		}
	}

	private void CalcTabRows()
	{
		TabAlignment tabAlignment = Alignment;
		if (tabAlignment == TabAlignment.Left || tabAlignment == TabAlignment.Right)
		{
			CalcTabRows(base.Height);
		}
		else
		{
			CalcTabRows(base.Width);
		}
	}

	private void CalcTabRows(int row_width)
	{
		int xpos = 0;
		int ypos = 0;
		Size tabSpacing = TabSpacing;
		if (TabPages.Count > 0)
		{
			row_count = 1;
		}
		show_slider = false;
		for (int i = 0; i < TabPages.Count; i++)
		{
			TabPage page = TabPages[i];
			int begin_prev = 0;
			SizeTab(page, i, row_width, ref xpos, ref ypos, tabSpacing, 0, ref begin_prev, widthOnly: true);
		}
		if (SelectedIndex != -1 && TabPages.Count > SelectedIndex && TabPages[SelectedIndex].Row != BottomRow)
		{
			DropRow(TabPages[SelectedIndex].Row);
		}
	}

	private void DropRow(int row)
	{
		if (Appearance != 0)
		{
			return;
		}
		int bottomRow = BottomRow;
		int direction = Direction;
		foreach (TabPage tabPage in TabPages)
		{
			if (tabPage.Row == row)
			{
				tabPage.Row = bottomRow;
			}
			else if (direction == 1 && tabPage.Row < row)
			{
				tabPage.Row += direction;
			}
			else if (direction == -1 && tabPage.Row > row)
			{
				tabPage.Row += direction;
			}
		}
	}

	private int CalcYPos()
	{
		if (Alignment == TabAlignment.Bottom || Alignment == TabAlignment.Left)
		{
			return ThemeEngine.Current.TabControlGetPanelRect(this).Bottom;
		}
		if (Appearance == TabAppearance.Normal)
		{
			return base.ClientRectangle.Y + ThemeEngine.Current.TabControlSelectedDelta.Y;
		}
		return base.ClientRectangle.Y;
	}

	private int CalcXPos()
	{
		if (Alignment == TabAlignment.Right)
		{
			return ThemeEngine.Current.TabControlGetPanelRect(this).Right;
		}
		if (Appearance == TabAppearance.Normal)
		{
			return base.ClientRectangle.X + ThemeEngine.Current.TabControlSelectedDelta.X;
		}
		return base.ClientRectangle.X;
	}

	private void SizeTabs()
	{
		TabAlignment tabAlignment = Alignment;
		if (tabAlignment == TabAlignment.Left || tabAlignment == TabAlignment.Right)
		{
			SizeTabs(base.Height, vertical: true);
		}
		else
		{
			SizeTabs(base.Width, vertical: false);
		}
	}

	private void SizeTabs(int row_width, bool vertical)
	{
		int ypos = 0;
		int xpos = 0;
		int num = 1;
		Size tabSpacing = TabSpacing;
		int begin_prev = 0;
		if (TabPages.Count == 0)
		{
			return;
		}
		num = TabPages[0].Row;
		if (!show_slider)
		{
			slider_pos = 0;
		}
		else
		{
			for (int i = 0; i < slider_pos; i++)
			{
				TabPage tabPage = TabPages[i];
				Rectangle tabBounds = tabPage.TabBounds;
				tabBounds.X = -1;
				tabPage.TabBounds = tabBounds;
			}
		}
		for (int j = slider_pos; j < TabPages.Count; j++)
		{
			TabPage tabPage2 = TabPages[j];
			SizeTab(tabPage2, j, row_width, ref xpos, ref ypos, tabSpacing, num, ref begin_prev, widthOnly: false);
			num = tabPage2.Row;
		}
		if (SizeMode == TabSizeMode.FillToRight && !ShowSlider)
		{
			FillRow(begin_prev, TabPages.Count - 1, (row_width - TabPages[TabPages.Count - 1].TabBounds.Right) / (TabPages.Count - begin_prev), tabSpacing, vertical);
		}
		if (SelectedIndex != -1)
		{
			ExpandSelected(TabPages[SelectedIndex], 0, row_width - 1);
		}
	}

	private void SizeTab(TabPage page, int i, int row_width, ref int xpos, ref int ypos, Size spacing, int prev_row, ref int begin_prev, bool widthOnly)
	{
		int num = 0;
		int num2;
		if (SizeMode == TabSizeMode.Fixed)
		{
			num2 = item_size.Width;
		}
		else
		{
			num2 = MeasureStringWidth(base.DeviceContext, page.Text, page.Font);
			num2 += Padding.X * 2 + 2;
			if (ImageList != null && page.ImageIndex >= 0 && page.ImageIndex < ImageList.Images.Count)
			{
				num2 += ImageList.ImageSize.Width + ThemeEngine.Current.TabControlImagePadding.X;
				int num3 = ImageList.ImageSize.Height + ThemeEngine.Current.TabControlImagePadding.Y;
				if (item_size.Height < num3)
				{
					item_size.Height = num3;
				}
			}
		}
		num = item_size.Height - ThemeEngine.Current.TabControlSelectedDelta.Height;
		if (num2 < MinimumTabWidth)
		{
			num2 = MinimumTabWidth;
		}
		if (i == SelectedIndex)
		{
			num2 += ThemeEngine.Current.TabControlSelectedSpacing;
		}
		if (widthOnly)
		{
			page.TabBounds = new Rectangle(xpos, 0, num2, 0);
			page.Row = row_count;
			if (xpos + num2 > row_width && multiline)
			{
				xpos = 0;
				row_count++;
			}
			else if (xpos + num2 > row_width)
			{
				show_slider = true;
			}
			if (i == selected_index && show_slider)
			{
				for (int num4 = i - 1; num4 >= 0; num4--)
				{
					if (TabPages[num4].TabBounds.Left < xpos + num2 - row_width)
					{
						slider_pos = num4 + 1;
						break;
					}
				}
			}
		}
		else
		{
			if (page.Row != prev_row)
			{
				xpos = 0;
			}
			switch (Alignment)
			{
			case TabAlignment.Top:
				page.TabBounds = new Rectangle(xpos + CalcXPos(), ypos + (num + spacing.Height) * (row_count - page.Row) + CalcYPos(), num2, num);
				break;
			case TabAlignment.Bottom:
				page.TabBounds = new Rectangle(xpos + CalcXPos(), ypos + (num + spacing.Height) * (row_count - page.Row) + CalcYPos(), num2, num);
				break;
			case TabAlignment.Left:
				if (Appearance == TabAppearance.Normal)
				{
					page.TabBounds = new Rectangle(ypos + (num + spacing.Height) * (row_count - page.Row) + CalcXPos(), xpos, num, num2);
				}
				else
				{
					page.TabBounds = new Rectangle(ypos + (num + spacing.Height) * (page.Row - 1) + CalcXPos(), xpos, num, num2);
				}
				break;
			case TabAlignment.Right:
				if (Appearance == TabAppearance.Normal)
				{
					page.TabBounds = new Rectangle(ypos + (num + spacing.Height) * (page.Row - 1) + CalcXPos(), xpos, num, num2);
				}
				else
				{
					page.TabBounds = new Rectangle(ypos + (num + spacing.Height) * (row_count - page.Row) + CalcXPos(), xpos, num, num2);
				}
				break;
			}
			if (page.Row != prev_row)
			{
				if (SizeMode == TabSizeMode.FillToRight && !ShowSlider)
				{
					bool flag = alignment == TabAlignment.Right || alignment == TabAlignment.Left;
					int num5 = ((!flag) ? TabPages[i - 1].TabBounds.Right : TabPages[i - 1].TabBounds.Bottom);
					FillRow(begin_prev, i - 1, (row_width - num5) / (i - begin_prev), spacing, flag);
				}
				begin_prev = i;
			}
		}
		xpos += num2 + spacing.Width + ThemeEngine.Current.TabControlColSpacing;
	}

	private void FillRow(int start, int end, int amount, Size spacing, bool vertical)
	{
		if (vertical)
		{
			FillRowV(start, end, amount, spacing);
		}
		else
		{
			FillRow(start, end, amount, spacing);
		}
	}

	private void FillRow(int start, int end, int amount, Size spacing)
	{
		int num = TabPages[start].TabBounds.Left;
		for (int i = start; i <= end; i++)
		{
			TabPage tabPage = TabPages[i];
			int num2 = num;
			int width = ((i != end) ? (tabPage.TabBounds.Width + amount) : (base.Width - num2 - 3));
			tabPage.TabBounds = new Rectangle(num2, tabPage.TabBounds.Top, width, tabPage.TabBounds.Height);
			num = tabPage.TabBounds.Right + 1 + spacing.Width;
		}
	}

	private void FillRowV(int start, int end, int amount, Size spacing)
	{
		int num = TabPages[start].TabBounds.Top;
		for (int i = start; i <= end; i++)
		{
			TabPage tabPage = TabPages[i];
			int num2 = num;
			int height = ((i != end) ? (tabPage.TabBounds.Height + amount) : (base.Height - num2 - 5));
			tabPage.TabBounds = new Rectangle(tabPage.TabBounds.Left, num2, tabPage.TabBounds.Width, height);
			num = tabPage.TabBounds.Bottom + 1;
		}
	}

	private void ExpandSelected(TabPage page, int left_edge, int right_edge)
	{
		if (Appearance == TabAppearance.Normal)
		{
			Rectangle tabBounds = page.TabBounds;
			switch (Alignment)
			{
			case TabAlignment.Top:
			case TabAlignment.Left:
				tabBounds.Y -= ThemeEngine.Current.TabControlSelectedDelta.Y;
				tabBounds.X -= ThemeEngine.Current.TabControlSelectedDelta.X;
				break;
			case TabAlignment.Bottom:
				tabBounds.Y -= ThemeEngine.Current.TabControlSelectedDelta.Y;
				tabBounds.X -= ThemeEngine.Current.TabControlSelectedDelta.X;
				break;
			case TabAlignment.Right:
				tabBounds.Y -= ThemeEngine.Current.TabControlSelectedDelta.Y;
				tabBounds.X -= ThemeEngine.Current.TabControlSelectedDelta.X;
				break;
			}
			tabBounds.Width += ThemeEngine.Current.TabControlSelectedDelta.Width;
			tabBounds.Height += ThemeEngine.Current.TabControlSelectedDelta.Height;
			if (tabBounds.Left < left_edge)
			{
				tabBounds.X = left_edge;
			}
			if (tabBounds.Right > right_edge && SizeMode != 0 && alignment != TabAlignment.Right)
			{
				tabBounds.Width = right_edge - tabBounds.X;
			}
			page.TabBounds = tabBounds;
		}
	}

	private void Draw(Graphics dc, Rectangle clip)
	{
		ThemeEngine.Current.DrawTabControl(dc, clip, this);
	}

	private TabPage GetTab(int index)
	{
		return base.Controls[index] as TabPage;
	}

	private void SetTab(int index, TabPage value)
	{
		if (!tab_pages.Contains(value))
		{
			base.Controls.Add(value);
		}
		base.Controls.RemoveAt(index);
		base.Controls.SetChildIndex(value, index);
		Redraw();
	}

	private void InsertTab(int index, TabPage value)
	{
		if (!tab_pages.Contains(value))
		{
			base.Controls.Add(value);
		}
		base.Controls.SetChildIndex(value, index);
		Redraw();
	}

	internal void Redraw()
	{
		if (base.IsHandleCreated)
		{
			ResizeTabPages();
			Refresh();
		}
	}

	private int MeasureStringWidth(Graphics graphics, string text, Font font)
	{
		if (text == string.Empty)
		{
			return 0;
		}
		StringFormat stringFormat = new StringFormat();
		RectangleF layoutRect = new RectangleF(0f, 0f, 1000f, 1000f);
		CharacterRange[] measurableCharacterRanges = new CharacterRange[1]
		{
			new CharacterRange(0, text.Length)
		};
		Region[] array = new Region[1];
		stringFormat.SetMeasurableCharacterRanges(measurableCharacterRanges);
		stringFormat.FormatFlags = StringFormatFlags.NoClip;
		stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
		array = graphics.MeasureCharacterRanges(text + "I", font, layoutRect, stringFormat);
		return (int)array[0].GetBounds(graphics).Width;
	}

	private void OnMouseMove(object sender, MouseEventArgs e)
	{
		if (!mouse_down_on_a_tab_page && ShowSlider)
		{
			if (LeftSliderState == PushButtonState.Pressed || RightSliderState == PushButtonState.Pressed)
			{
				return;
			}
			if (LeftScrollButtonArea.Contains(e.Location))
			{
				LeftSliderState = PushButtonState.Hot;
				RightSliderState = PushButtonState.Normal;
				EnteredTabPage = null;
				return;
			}
			if (RightScrollButtonArea.Contains(e.Location))
			{
				RightSliderState = PushButtonState.Hot;
				LeftSliderState = PushButtonState.Normal;
				EnteredTabPage = null;
				return;
			}
			LeftSliderState = PushButtonState.Normal;
			RightSliderState = PushButtonState.Normal;
		}
		if (EnteredTabPage != null && EnteredTabPage.TabBounds.Contains(e.Location))
		{
			return;
		}
		for (int i = 0; i < TabCount; i++)
		{
			TabPage tabPage = TabPages[i];
			if (tabPage.TabBounds.Contains(e.Location))
			{
				EnteredTabPage = tabPage;
				return;
			}
		}
		EnteredTabPage = null;
	}

	private void OnMouseLeave(object sender, EventArgs e)
	{
		if (ShowSlider)
		{
			LeftSliderState = PushButtonState.Normal;
			RightSliderState = PushButtonState.Normal;
		}
		EnteredTabPage = null;
	}
}
