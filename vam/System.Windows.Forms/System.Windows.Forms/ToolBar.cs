using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[DefaultEvent("ButtonClick")]
[DefaultProperty("Buttons")]
[Designer("System.Windows.Forms.Design.ToolBarDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public class ToolBar : Control
{
	public class ToolBarButtonCollection : ICollection, IEnumerable, IList
	{
		private ArrayList list;

		private ToolBar owner;

		private bool redraw;

		private static object UIACollectionChangedEvent;

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
				if (!(value is ToolBarButton))
				{
					throw new ArgumentException("Not of type ToolBarButton", "value");
				}
				this[index] = (ToolBarButton)value;
			}
		}

		[Browsable(false)]
		public int Count => list.Count;

		public bool IsReadOnly => list.IsReadOnly;

		public virtual ToolBarButton this[int index]
		{
			get
			{
				return (ToolBarButton)list[index];
			}
			set
			{
				OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, index));
				value.SetParent(owner);
				list[index] = value;
				owner.Redraw(recalculate: true);
				OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, index));
			}
		}

		public virtual ToolBarButton this[string key]
		{
			get
			{
				if (string.IsNullOrEmpty(key))
				{
					return null;
				}
				foreach (ToolBarButton item in list)
				{
					if (string.Compare(item.Name, key, ignoreCase: true) == 0)
					{
						return item;
					}
				}
				return null;
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

		public ToolBarButtonCollection(ToolBar owner)
		{
			list = new ArrayList();
			this.owner = owner;
			redraw = true;
		}

		static ToolBarButtonCollection()
		{
			UIACollectionChanged = new object();
		}

		void ICollection.CopyTo(Array dest, int index)
		{
			list.CopyTo(dest, index);
		}

		int IList.Add(object button)
		{
			if (!(button is ToolBarButton))
			{
				throw new ArgumentException("Not of type ToolBarButton", "button");
			}
			return Add((ToolBarButton)button);
		}

		bool IList.Contains(object button)
		{
			if (!(button is ToolBarButton))
			{
				throw new ArgumentException("Not of type ToolBarButton", "button");
			}
			return Contains((ToolBarButton)button);
		}

		int IList.IndexOf(object button)
		{
			if (!(button is ToolBarButton))
			{
				throw new ArgumentException("Not of type ToolBarButton", "button");
			}
			return IndexOf((ToolBarButton)button);
		}

		void IList.Insert(int index, object button)
		{
			if (!(button is ToolBarButton))
			{
				throw new ArgumentException("Not of type ToolBarButton", "button");
			}
			Insert(index, (ToolBarButton)button);
		}

		void IList.Remove(object button)
		{
			if (!(button is ToolBarButton))
			{
				throw new ArgumentException("Not of type ToolBarButton", "button");
			}
			Remove((ToolBarButton)button);
		}

		internal void OnUIACollectionChanged(CollectionChangeEventArgs e)
		{
			((CollectionChangeEventHandler)owner.Events[UIACollectionChanged])?.Invoke(owner, e);
		}

		public int Add(string text)
		{
			ToolBarButton button = new ToolBarButton(text);
			return Add(button);
		}

		public int Add(ToolBarButton button)
		{
			button.SetParent(owner);
			int num = list.Add(button);
			if (redraw)
			{
				owner.Redraw(recalculate: true);
			}
			OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, num));
			return num;
		}

		public void AddRange(ToolBarButton[] buttons)
		{
			try
			{
				redraw = false;
				foreach (ToolBarButton button in buttons)
				{
					Add(button);
				}
			}
			finally
			{
				redraw = true;
				owner.Redraw(recalculate: true);
			}
		}

		public void Clear()
		{
			list.Clear();
			owner.Redraw(recalculate: false);
			OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, -1));
		}

		public bool Contains(ToolBarButton button)
		{
			return list.Contains(button);
		}

		public virtual bool ContainsKey(string key)
		{
			return this[key] != null;
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public int IndexOf(ToolBarButton button)
		{
			return list.IndexOf(button);
		}

		public virtual int IndexOfKey(string key)
		{
			return IndexOf(this[key]);
		}

		public void Insert(int index, ToolBarButton button)
		{
			list.Insert(index, button);
			owner.Redraw(recalculate: true);
			OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, index));
		}

		public void Remove(ToolBarButton button)
		{
			list.Remove(button);
			owner.Redraw(recalculate: true);
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
			owner.Redraw(recalculate: true);
			OnUIACollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, index));
		}

		public virtual void RemoveByKey(string key)
		{
			Remove(this[key]);
		}
	}

	internal const int text_padding = 3;

	private bool size_specified;

	private ToolBarItem current_item;

	internal ToolBarItem[] items;

	internal Size default_size;

	private static object ButtonClickEvent;

	private static object ButtonDropDownEvent;

	private ToolBarAppearance appearance;

	private bool autosize = true;

	private ToolBarButtonCollection buttons;

	private Size button_size;

	private bool divider = true;

	private bool drop_down_arrows = true;

	private ImageList image_list;

	private ImeMode ime_mode = ImeMode.Disable;

	private bool show_tooltips = true;

	private ToolBarTextAlign text_alignment;

	private bool wrappable = true;

	private ToolBarButton button_for_focus;

	private int requested_size = -1;

	private ToolTip tip_window;

	private Timer tipdown_timer;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = base.CreateParams;
			if (appearance == ToolBarAppearance.Flat)
			{
				createParams.Style |= 2048;
			}
			return createParams;
		}
	}

	protected override ImeMode DefaultImeMode => ImeMode.Disable;

	protected override Size DefaultSize => ThemeEngine.Current.ToolBarDefaultSize;

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

	[Localizable(true)]
	[DefaultValue(ToolBarAppearance.Normal)]
	public ToolBarAppearance Appearance
	{
		get
		{
			return appearance;
		}
		set
		{
			if (value != appearance)
			{
				appearance = value;
				Redraw(recalculate: true);
			}
		}
	}

	[DefaultValue(true)]
	[Localizable(true)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public override bool AutoSize
	{
		get
		{
			return autosize;
		}
		set
		{
			if (value != autosize)
			{
				autosize = value;
				if (base.IsHandleCreated)
				{
					Redraw(recalculate: true);
				}
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Color BackColor
	{
		get
		{
			return background_color;
		}
		set
		{
			if (!(value == background_color))
			{
				background_color = value;
				OnBackColorChanged(EventArgs.Empty);
				Redraw(recalculate: false);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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
	[DefaultValue(BorderStyle.None)]
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

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Localizable(true)]
	[MergableProperty(false)]
	public ToolBarButtonCollection Buttons => buttons;

	[Localizable(true)]
	[RefreshProperties(RefreshProperties.All)]
	public Size ButtonSize
	{
		get
		{
			if (!button_size.IsEmpty)
			{
				return button_size;
			}
			if (buttons.Count == 0)
			{
				return new Size(39, 36);
			}
			Size result = CalcButtonSize();
			if (result.IsEmpty)
			{
				return new Size(24, 22);
			}
			return result;
		}
		set
		{
			size_specified = value != Size.Empty;
			if (!(button_size == value))
			{
				button_size = value;
				Redraw(recalculate: true);
			}
		}
	}

	[DefaultValue(true)]
	public bool Divider
	{
		get
		{
			return divider;
		}
		set
		{
			if (value != divider)
			{
				divider = value;
				Redraw(recalculate: false);
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(DockStyle.Top)]
	public override DockStyle Dock
	{
		get
		{
			return base.Dock;
		}
		set
		{
			if (base.Dock == value)
			{
				if (value != 0)
				{
					base.Dock = value;
				}
				return;
			}
			if (Vertical)
			{
				SetStyle(ControlStyles.FixedWidth, AutoSize);
				SetStyle(ControlStyles.FixedHeight, value: false);
			}
			else
			{
				SetStyle(ControlStyles.FixedHeight, AutoSize);
				SetStyle(ControlStyles.FixedWidth, value: false);
			}
			LayoutToolBar();
			base.Dock = value;
		}
	}

	[DefaultValue(false)]
	[Localizable(true)]
	public bool DropDownArrows
	{
		get
		{
			return drop_down_arrows;
		}
		set
		{
			if (value != drop_down_arrows)
			{
				drop_down_arrows = value;
				Redraw(recalculate: true);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Color ForeColor
	{
		get
		{
			return foreground_color;
		}
		set
		{
			if (!(value == foreground_color))
			{
				foreground_color = value;
				OnForeColorChanged(EventArgs.Empty);
				Redraw(recalculate: false);
			}
		}
	}

	[DefaultValue(null)]
	public ImageList ImageList
	{
		get
		{
			return image_list;
		}
		set
		{
			if (image_list != value)
			{
				image_list = value;
				Redraw(recalculate: true);
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public Size ImageSize
	{
		get
		{
			if (ImageList == null)
			{
				return Size.Empty;
			}
			return ImageList.ImageSize;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new ImeMode ImeMode
	{
		get
		{
			return ime_mode;
		}
		set
		{
			if (value != ime_mode)
			{
				ime_mode = value;
				OnImeModeChanged(EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override RightToLeft RightToLeft
	{
		get
		{
			return base.RightToLeft;
		}
		set
		{
			if (value != base.RightToLeft)
			{
				base.RightToLeft = value;
				OnRightToLeftChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(false)]
	[Localizable(true)]
	public bool ShowToolTips
	{
		get
		{
			return show_tooltips;
		}
		set
		{
			show_tooltips = value;
		}
	}

	[DefaultValue(false)]
	public new bool TabStop
	{
		get
		{
			return base.TabStop;
		}
		set
		{
			base.TabStop = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

	[Localizable(true)]
	[DefaultValue(ToolBarTextAlign.Underneath)]
	public ToolBarTextAlign TextAlign
	{
		get
		{
			return text_alignment;
		}
		set
		{
			if (value != text_alignment)
			{
				text_alignment = value;
				Redraw(recalculate: true);
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(true)]
	public bool Wrappable
	{
		get
		{
			return wrappable;
		}
		set
		{
			if (value != wrappable)
			{
				wrappable = value;
				Redraw(recalculate: true);
			}
		}
	}

	internal int CurrentItem
	{
		get
		{
			return Array.IndexOf(items, current_item);
		}
		set
		{
			if (current_item != null)
			{
				current_item.Hilight = false;
			}
			current_item = ((value != -1) ? items[value] : null);
			if (current_item != null)
			{
				current_item.Hilight = true;
			}
		}
	}

	private Timer TipDownTimer
	{
		get
		{
			if (tipdown_timer == null)
			{
				tipdown_timer = new Timer();
				tipdown_timer.Enabled = false;
				tipdown_timer.Interval = 5000;
				tipdown_timer.Tick += PopDownTip;
			}
			return tipdown_timer;
		}
	}

	internal bool SizeSpecified => size_specified;

	internal bool Vertical => Dock == DockStyle.Left || Dock == DockStyle.Right;

	private Size AdjustedButtonSize
	{
		get
		{
			Size result = ((!default_size.IsEmpty && Appearance != 0) ? default_size : ButtonSize);
			if (size_specified)
			{
				if (Appearance == ToolBarAppearance.Flat)
				{
					result = CalcButtonSize();
				}
				else
				{
					int toolBarImageGripWidth = ThemeEngine.Current.ToolBarImageGripWidth;
					if (result.Width < ImageSize.Width + 2 * toolBarImageGripWidth)
					{
						result.Width = ImageSize.Width + 2 * toolBarImageGripWidth;
					}
					if (result.Height < ImageSize.Height + 2 * toolBarImageGripWidth)
					{
						result.Height = ImageSize.Height + 2 * toolBarImageGripWidth;
					}
				}
			}
			return result;
		}
	}

	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public new event EventHandler AutoSizeChanged
	{
		add
		{
			base.AutoSizeChanged += value;
		}
		remove
		{
			base.AutoSizeChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
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

	public event ToolBarButtonClickEventHandler ButtonClick
	{
		add
		{
			base.Events.AddHandler(ButtonClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ButtonClickEvent, value);
		}
	}

	public event ToolBarButtonClickEventHandler ButtonDropDown
	{
		add
		{
			base.Events.AddHandler(ButtonDropDownEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ButtonDropDownEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
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
	public new event EventHandler ImeModeChanged
	{
		add
		{
			base.ImeModeChanged += value;
		}
		remove
		{
			base.ImeModeChanged -= value;
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
	public new event EventHandler RightToLeftChanged
	{
		add
		{
			base.RightToLeftChanged += value;
		}
		remove
		{
			base.RightToLeftChanged -= value;
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

	public ToolBar()
	{
		background_color = ThemeEngine.Current.DefaultControlBackColor;
		foreground_color = ThemeEngine.Current.DefaultControlForeColor;
		buttons = new ToolBarButtonCollection(this);
		Dock = DockStyle.Top;
		base.GotFocus += FocusChanged;
		base.LostFocus += FocusChanged;
		base.MouseDown += ToolBar_MouseDown;
		base.MouseHover += ToolBar_MouseHover;
		base.MouseLeave += ToolBar_MouseLeave;
		base.MouseMove += ToolBar_MouseMove;
		base.MouseUp += ToolBar_MouseUp;
		BackgroundImageChanged += ToolBar_BackgroundImageChanged;
		TabStop = false;
		SetStyle(ControlStyles.UserPaint, value: false);
		SetStyle(ControlStyles.FixedHeight, value: true);
		SetStyle(ControlStyles.FixedWidth, value: false);
	}

	static ToolBar()
	{
		ButtonClick = new object();
		ButtonDropDown = new object();
	}

	public override string ToString()
	{
		int count = Buttons.Count;
		if (count == 0)
		{
			return $"System.Windows.Forms.ToolBar, Buttons.Count: 0";
		}
		return $"System.Windows.Forms.ToolBar, Buttons.Count: {count}, Buttons[0]: {Buttons[0].ToString()}";
	}

	protected override void CreateHandle()
	{
		base.CreateHandle();
		default_size = CalcButtonSize();
		if (appearance != ToolBarAppearance.Flat)
		{
			Redraw(recalculate: true);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			ImageList = null;
		}
		base.Dispose(disposing);
	}

	internal void UIAPerformClick(ToolBarButton button)
	{
		ToolBarItem toolBarItem = current_item;
		current_item = null;
		ToolBarItem[] array = items;
		foreach (ToolBarItem toolBarItem2 in array)
		{
			if (toolBarItem2.Button == button)
			{
				current_item = toolBarItem2;
				break;
			}
		}
		try
		{
			if (current_item == null)
			{
				throw new ArgumentException("button", "The button specified is not part of this toolbar");
			}
			PerformButtonClick(new ToolBarButtonClickEventArgs(button));
		}
		finally
		{
			current_item = toolBarItem;
		}
	}

	private void PerformButtonClick(ToolBarButtonClickEventArgs e)
	{
		if (e.Button.Style == ToolBarButtonStyle.ToggleButton)
		{
			if (!e.Button.Pushed)
			{
				e.Button.Pushed = true;
			}
			else
			{
				e.Button.Pushed = false;
			}
		}
		current_item.Pressed = false;
		current_item.Invalidate();
		button_for_focus = current_item.Button;
		button_for_focus.UIAHasFocus = true;
		OnButtonClick(e);
	}

	protected virtual void OnButtonClick(ToolBarButtonClickEventArgs e)
	{
		((ToolBarButtonClickEventHandler)base.Events[ButtonClick])?.Invoke(this, e);
	}

	protected virtual void OnButtonDropDown(ToolBarButtonClickEventArgs e)
	{
		((ToolBarButtonClickEventHandler)base.Events[ButtonDropDown])?.Invoke(this, e);
		if (e.Button.DropDownMenu != null)
		{
			ShowDropDownMenu(current_item);
		}
	}

	internal void ShowDropDownMenu(ToolBarItem item)
	{
		Point pos = new Point(item.Rectangle.X + 1, item.Rectangle.Bottom + 1);
		((ContextMenu)item.Button.DropDownMenu).Show(this, pos);
		item.DDPressed = false;
		item.Hilight = false;
		item.Invalidate();
	}

	protected override void OnFontChanged(EventArgs e)
	{
		base.OnFontChanged(e);
		Redraw(recalculate: true);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		LayoutToolBar();
	}

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		specified &= ~BoundsSpecified.Height;
		base.ScaleControl(factor, specified);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void ScaleCore(float dx, float dy)
	{
		dy = 1f;
		base.ScaleCore(dx, dy);
	}

	protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
	{
		if (Vertical)
		{
			if (!AutoSize && requested_size != width && (specified & BoundsSpecified.Width) != 0)
			{
				requested_size = width;
			}
		}
		else if (!AutoSize && requested_size != height && (specified & BoundsSpecified.Height) != 0)
		{
			requested_size = height;
		}
		base.SetBoundsCore(x, y, width, height, specified);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	internal override bool InternalPreProcessMessage(ref Message msg)
	{
		if (msg.Msg == 256)
		{
			Keys key_data = (Keys)msg.WParam.ToInt32();
			if (HandleKeyDown(ref msg, key_data))
			{
				return true;
			}
		}
		return base.InternalPreProcessMessage(ref msg);
	}

	private void FocusChanged(object sender, EventArgs args)
	{
		if (!Focused && button_for_focus != null)
		{
			button_for_focus.UIAHasFocus = false;
		}
		button_for_focus = null;
		if (Appearance != ToolBarAppearance.Flat || Buttons.Count == 0)
		{
			return;
		}
		ToolBarItem toolBarItem = null;
		ToolBarItem[] array = items;
		foreach (ToolBarItem toolBarItem2 in array)
		{
			if (toolBarItem2.Hilight)
			{
				toolBarItem = toolBarItem2;
				break;
			}
		}
		if (Focused && toolBarItem == null)
		{
			ToolBarItem[] array2 = items;
			foreach (ToolBarItem toolBarItem3 in array2)
			{
				if (toolBarItem3.Button.Enabled)
				{
					toolBarItem3.Hilight = true;
					break;
				}
			}
		}
		else if (toolBarItem != null)
		{
			toolBarItem.Hilight = false;
		}
	}

	private bool HandleKeyDown(ref Message msg, Keys key_data)
	{
		if (Appearance != ToolBarAppearance.Flat || Buttons.Count == 0)
		{
			return false;
		}
		if (HandleKeyOnDropDown(ref msg, key_data))
		{
			return true;
		}
		switch (key_data)
		{
		case Keys.Left:
		case Keys.Up:
			HighlightButton(-1);
			return true;
		case Keys.Right:
		case Keys.Down:
			HighlightButton(1);
			return true;
		case Keys.Return:
		case Keys.Space:
			if (current_item != null)
			{
				OnButtonClick(new ToolBarButtonClickEventArgs(current_item.Button));
				return true;
			}
			break;
		}
		return false;
	}

	private bool HandleKeyOnDropDown(ref Message msg, Keys key_data)
	{
		if (current_item == null || current_item.Button.Style != ToolBarButtonStyle.DropDownButton || current_item.Button.DropDownMenu == null)
		{
			return false;
		}
		Menu dropDownMenu = current_item.Button.DropDownMenu;
		if (dropDownMenu.Tracker.active)
		{
			dropDownMenu.ProcessCmdKey(ref msg, key_data);
			return true;
		}
		if (key_data == Keys.Up || key_data == Keys.Down)
		{
			current_item.DDPressed = true;
			current_item.Invalidate();
			OnButtonDropDown(new ToolBarButtonClickEventArgs(current_item.Button));
			return true;
		}
		return false;
	}

	private void HighlightButton(int offset)
	{
		ArrayList arrayList = new ArrayList();
		int num = 0;
		int num2 = -1;
		ToolBarItem toolBarItem = null;
		ToolBarItem[] array = items;
		foreach (ToolBarItem toolBarItem2 in array)
		{
			if (toolBarItem2.Hilight)
			{
				num2 = num;
				toolBarItem = toolBarItem2;
			}
			if (toolBarItem2.Button.Enabled)
			{
				arrayList.Add(toolBarItem2);
				num++;
			}
		}
		int num3 = (num2 + offset) % num;
		if (num3 < 0)
		{
			num3 = num - 1;
		}
		if (num3 != num2)
		{
			if (toolBarItem != null)
			{
				toolBarItem.Hilight = false;
			}
			current_item = arrayList[num3] as ToolBarItem;
			current_item.Hilight = true;
		}
	}

	private void ToolBar_BackgroundImageChanged(object sender, EventArgs args)
	{
		Redraw(recalculate: false, force: true);
	}

	private void ToolBar_MouseDown(object sender, MouseEventArgs me)
	{
		if (!base.Enabled || (me.Button & MouseButtons.Left) == 0)
		{
			return;
		}
		Point pt = new Point(me.X, me.Y);
		if (ItemAtPoint(pt) == null)
		{
			return;
		}
		if (tip_window != null && tip_window.Visible && (me.Button & MouseButtons.Left) == MouseButtons.Left)
		{
			TipDownTimer.Stop();
			tip_window.Hide(this);
		}
		ToolBarItem[] array = items;
		foreach (ToolBarItem toolBarItem in array)
		{
			if (!toolBarItem.Button.Enabled || !toolBarItem.Rectangle.Contains(pt))
			{
				continue;
			}
			if (toolBarItem.Button.Style == ToolBarButtonStyle.DropDownButton)
			{
				Rectangle rectangle = toolBarItem.Rectangle;
				if (DropDownArrows)
				{
					rectangle.Width = ThemeEngine.Current.ToolBarDropDownWidth;
					rectangle.X = toolBarItem.Rectangle.Right - rectangle.Width;
				}
				if (rectangle.Contains(pt))
				{
					if (toolBarItem.Button.DropDownMenu != null)
					{
						toolBarItem.DDPressed = true;
						Invalidate(rectangle);
					}
					break;
				}
			}
			toolBarItem.Pressed = true;
			toolBarItem.Inside = true;
			toolBarItem.Invalidate();
			break;
		}
	}

	private void ToolBar_MouseUp(object sender, MouseEventArgs me)
	{
		if (!base.Enabled || (me.Button & MouseButtons.Left) == 0)
		{
			return;
		}
		Point pt = new Point(me.X, me.Y);
		ArrayList arrayList = new ArrayList(items);
		foreach (ToolBarItem item in arrayList)
		{
			if (item.Button.Enabled && item.Rectangle.Contains(pt))
			{
				if (item.Button.Style == ToolBarButtonStyle.DropDownButton)
				{
					Rectangle rectangle = item.Rectangle;
					rectangle.Width = ThemeEngine.Current.ToolBarDropDownWidth;
					rectangle.X = item.Rectangle.Right - rectangle.Width;
					if (rectangle.Contains(pt))
					{
						current_item = item;
						if (item.DDPressed)
						{
							OnButtonDropDown(new ToolBarButtonClickEventArgs(item.Button));
						}
						continue;
					}
				}
				current_item = item;
				if (item.Pressed && (me.Button & MouseButtons.Left) == MouseButtons.Left)
				{
					PerformButtonClick(new ToolBarButtonClickEventArgs(item.Button));
				}
			}
			else if (item.Pressed)
			{
				item.Pressed = false;
				item.Invalidate();
			}
		}
	}

	private ToolBarItem ItemAtPoint(Point pt)
	{
		ToolBarItem[] array = items;
		foreach (ToolBarItem toolBarItem in array)
		{
			if (toolBarItem.Rectangle.Contains(pt))
			{
				return toolBarItem;
			}
		}
		return null;
	}

	private void PopDownTip(object o, EventArgs args)
	{
		tip_window.Hide(this);
	}

	private void ToolBar_MouseHover(object sender, EventArgs e)
	{
		if (!base.Capture)
		{
			if (tip_window == null)
			{
				tip_window = new ToolTip();
			}
			ToolBarItem toolBarItem = (current_item = ItemAtPoint(PointToClient(Control.MousePosition)));
			if (toolBarItem != null && toolBarItem.Button.ToolTipText.Length != 0)
			{
				tip_window.Present(this, toolBarItem.Button.ToolTipText);
				TipDownTimer.Start();
			}
		}
	}

	private void ToolBar_MouseLeave(object sender, EventArgs e)
	{
		if (tipdown_timer != null)
		{
			tipdown_timer.Dispose();
		}
		tipdown_timer = null;
		if (tip_window != null)
		{
			tip_window.Dispose();
		}
		tip_window = null;
		if (base.Enabled && current_item != null)
		{
			current_item.Hilight = false;
			current_item = null;
		}
	}

	private void ToolBar_MouseMove(object sender, MouseEventArgs me)
	{
		if (!base.Enabled)
		{
			return;
		}
		if (tip_window != null && tip_window.Visible)
		{
			TipDownTimer.Stop();
			TipDownTimer.Start();
		}
		Point pt = new Point(me.X, me.Y);
		if (base.Capture)
		{
			ToolBarItem[] array = items;
			foreach (ToolBarItem toolBarItem in array)
			{
				if (toolBarItem.Pressed && toolBarItem.Inside != toolBarItem.Rectangle.Contains(pt))
				{
					toolBarItem.Inside = toolBarItem.Rectangle.Contains(pt);
					toolBarItem.Hilight = false;
					break;
				}
			}
			return;
		}
		if (current_item != null && current_item.Rectangle.Contains(pt))
		{
			if (ThemeEngine.Current.ToolBarHasHotElementStyles(this) && !current_item.Hilight && (ThemeEngine.Current.ToolBarHasHotCheckedElementStyles || !current_item.Button.Pushed) && current_item.Button.Enabled)
			{
				current_item.Hilight = true;
			}
			return;
		}
		if (tip_window != null)
		{
			if (tip_window.Visible)
			{
				tip_window.Hide(this);
				TipDownTimer.Stop();
			}
			current_item = ItemAtPoint(pt);
			if (current_item != null && current_item.Button.ToolTipText.Length > 0)
			{
				tip_window.Present(this, current_item.Button.ToolTipText);
				TipDownTimer.Start();
			}
		}
		if (!ThemeEngine.Current.ToolBarHasHotElementStyles(this))
		{
			return;
		}
		ToolBarItem[] array2 = items;
		foreach (ToolBarItem toolBarItem2 in array2)
		{
			if (toolBarItem2.Rectangle.Contains(pt) && toolBarItem2.Button.Enabled)
			{
				current_item = toolBarItem2;
				if (!current_item.Hilight && (ThemeEngine.Current.ToolBarHasHotCheckedElementStyles || !current_item.Button.Pushed))
				{
					current_item.Hilight = true;
				}
			}
			else if (toolBarItem2.Hilight)
			{
				toolBarItem2.Hilight = false;
			}
		}
	}

	internal override void OnPaintInternal(PaintEventArgs pevent)
	{
		if (!GetStyle(ControlStyles.UserPaint))
		{
			ThemeEngine.Current.DrawToolBar(pevent.Graphics, pevent.ClipRectangle, this);
			pevent.Handled = true;
		}
	}

	internal void Redraw(bool recalculate)
	{
		Redraw(recalculate, force: true);
	}

	internal void Redraw(bool recalculate, bool force)
	{
		bool flag = true;
		if (recalculate)
		{
			flag = LayoutToolBar();
		}
		if (force || flag)
		{
			Invalidate();
		}
	}

	private Size CalcButtonSize()
	{
		if (Buttons.Count == 0)
		{
			return Size.Empty;
		}
		string text = Buttons[0].Text;
		for (int i = 1; i < Buttons.Count; i++)
		{
			if (Buttons[i].Text.Length > text.Length)
			{
				text = Buttons[i].Text;
			}
		}
		Size result = Size.Empty;
		if (text != null && text.Length > 0)
		{
			SizeF sizeF = TextRenderer.MeasureString(text, Font);
			if (sizeF != SizeF.Empty)
			{
				result = new Size((int)Math.Ceiling(sizeF.Width) + 6, (int)Math.Ceiling(sizeF.Height));
			}
		}
		Size size = ((ImageList != null) ? ImageSize : new Size(16, 16));
		Theme current = ThemeEngine.Current;
		int num = size.Width + 2 * current.ToolBarImageGripWidth;
		int num2 = size.Height + 2 * current.ToolBarImageGripWidth;
		if (text_alignment == ToolBarTextAlign.Right)
		{
			result.Width = num + result.Width;
			result.Height = ((result.Height <= num2) ? num2 : result.Height);
		}
		else
		{
			result.Height = num2 + result.Height;
			result.Width = ((result.Width <= num) ? num : result.Width);
		}
		result.Width += current.ToolBarImageGripWidth;
		result.Height += current.ToolBarImageGripWidth;
		return result;
	}

	private bool LayoutToolBar()
	{
		bool result = false;
		Theme current = ThemeEngine.Current;
		int num = current.ToolBarGripWidth;
		int num2 = current.ToolBarGripWidth;
		Size adjustedButtonSize = AdjustedButtonSize;
		int num3 = ((!Vertical) ? adjustedButtonSize.Height : adjustedButtonSize.Width) + current.ToolBarGripWidth;
		int num4 = -1;
		items = new ToolBarItem[buttons.Count];
		for (int i = 0; i < buttons.Count; i++)
		{
			ToolBarButton toolBarButton = buttons[i];
			ToolBarItem toolBarItem = new ToolBarItem(toolBarButton);
			items[i] = toolBarItem;
			if (!toolBarButton.Visible)
			{
				continue;
			}
			result = ((!size_specified || toolBarButton.Style == ToolBarButtonStyle.Separator) ? toolBarItem.Layout(Vertical, num3) : toolBarItem.Layout(adjustedButtonSize));
			bool flag = toolBarButton.Style == ToolBarButtonStyle.Separator;
			if (Vertical)
			{
				if (num2 + toolBarItem.Rectangle.Height < base.Height || flag || !Wrappable)
				{
					if (toolBarItem.Location.X != num || toolBarItem.Location.Y != num2)
					{
						result = true;
					}
					toolBarItem.Location = new Point(num, num2);
					num2 += toolBarItem.Rectangle.Height;
					if (flag)
					{
						num4 = i;
					}
				}
				else if (num4 > 0)
				{
					i = num4;
					num4 = -1;
					num2 = current.ToolBarGripWidth;
					num += num3;
				}
				else
				{
					num2 = current.ToolBarGripWidth;
					num += num3;
					if (toolBarItem.Location.X != num || toolBarItem.Location.Y != num2)
					{
						result = true;
					}
					toolBarItem.Location = new Point(num, num2);
					num2 += toolBarItem.Rectangle.Height;
				}
			}
			else if (num + toolBarItem.Rectangle.Width < base.Width || flag || !Wrappable)
			{
				if (toolBarItem.Location.X != num || toolBarItem.Location.Y != num2)
				{
					result = true;
				}
				toolBarItem.Location = new Point(num, num2);
				num += toolBarItem.Rectangle.Width;
				if (flag)
				{
					num4 = i;
				}
			}
			else if (num4 > 0)
			{
				i = num4;
				num4 = -1;
				num = current.ToolBarGripWidth;
				num2 += num3;
			}
			else
			{
				num = current.ToolBarGripWidth;
				num2 += num3;
				if (toolBarItem.Location.X != num || toolBarItem.Location.Y != num2)
				{
					result = true;
				}
				toolBarItem.Location = new Point(num, num2);
				num += toolBarItem.Rectangle.Width;
			}
		}
		if (base.Parent == null)
		{
			return result;
		}
		if (Wrappable)
		{
			num3 += ((!Vertical) ? num2 : num);
		}
		if (base.IsHandleCreated)
		{
			if (Vertical)
			{
				base.Width = num3;
			}
			else
			{
				base.Height = num3;
			}
		}
		return result;
	}
}
