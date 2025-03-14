using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[DefaultProperty("Text")]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultEvent("Click")]
public class MenuItem : Menu
{
	internal bool separator;

	internal bool break_;

	internal bool bar_break;

	private Shortcut shortcut;

	private string text;

	private bool checked_;

	private bool radiocheck;

	private bool enabled;

	private char mnemonic;

	private bool showshortcut;

	private int index;

	private bool mdilist;

	private Hashtable mdilist_items;

	private Hashtable mdilist_forms;

	private MdiClient mdicontainer;

	private bool is_window_menu_item;

	private bool defaut_item;

	private bool visible;

	private bool ownerdraw;

	private int menuid;

	private int mergeorder;

	private int xtab;

	private int menuheight;

	private bool menubar;

	private MenuMerge mergetype;

	internal Rectangle bounds;

	private static object ClickEvent;

	private static object DrawItemEvent;

	private static object MeasureItemEvent;

	private static object PopupEvent;

	private static object SelectEvent;

	private static object UIACheckedChangedEvent;

	private static object UIARadioCheckChangedEvent;

	private static object UIAEnabledChangedEvent;

	private static object UIATextChangedEvent;

	private bool selected;

	[Browsable(false)]
	[DefaultValue(false)]
	public bool BarBreak
	{
		get
		{
			return break_;
		}
		set
		{
			break_ = value;
		}
	}

	[Browsable(false)]
	[DefaultValue(false)]
	public bool Break
	{
		get
		{
			return bar_break;
		}
		set
		{
			bar_break = value;
		}
	}

	[DefaultValue(false)]
	public bool Checked
	{
		get
		{
			return checked_;
		}
		set
		{
			if (checked_ != value)
			{
				checked_ = value;
				OnUIACheckedChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(false)]
	public bool DefaultItem
	{
		get
		{
			return defaut_item;
		}
		set
		{
			defaut_item = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(true)]
	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			if (enabled != value)
			{
				enabled = value;
				OnUIAEnabledChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			if (Parent != null && Parent.MenuItems != null && (value < 0 || value >= Parent.MenuItems.Count))
			{
				throw new ArgumentException("'" + value + "' is not a valid value for 'value'");
			}
			index = value;
		}
	}

	[Browsable(false)]
	public override bool IsParent => IsPopup;

	[DefaultValue(false)]
	public bool MdiList
	{
		get
		{
			return mdilist;
		}
		set
		{
			if (mdilist == value)
			{
				return;
			}
			mdilist = value;
			if (mdilist || mdilist_items == null)
			{
				return;
			}
			foreach (MenuItem key in mdilist_items.Keys)
			{
				base.MenuItems.Remove(key);
			}
			mdilist_items.Clear();
			mdilist_items = null;
		}
	}

	protected int MenuID => menuid;

	[DefaultValue(0)]
	public int MergeOrder
	{
		get
		{
			return mergeorder;
		}
		set
		{
			mergeorder = value;
		}
	}

	[DefaultValue(MenuMerge.Add)]
	public MenuMerge MergeType
	{
		get
		{
			return mergetype;
		}
		set
		{
			if (!Enum.IsDefined(typeof(MenuMerge), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for MenuMerge");
			}
			mergetype = value;
		}
	}

	[Browsable(false)]
	public char Mnemonic => mnemonic;

	[DefaultValue(false)]
	public bool OwnerDraw
	{
		get
		{
			return ownerdraw;
		}
		set
		{
			ownerdraw = value;
		}
	}

	[Browsable(false)]
	public Menu Parent => parent_menu;

	[DefaultValue(false)]
	public bool RadioCheck
	{
		get
		{
			return radiocheck;
		}
		set
		{
			if (radiocheck != value)
			{
				radiocheck = value;
				OnUIARadioCheckChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(Shortcut.None)]
	[Localizable(true)]
	public Shortcut Shortcut
	{
		get
		{
			return shortcut;
		}
		set
		{
			if (!Enum.IsDefined(typeof(Shortcut), value))
			{
				throw new InvalidEnumArgumentException($"Enum argument value '{value}' is not valid for Shortcut");
			}
			shortcut = value;
			UpdateMenuItem();
		}
	}

	[Localizable(true)]
	[DefaultValue(true)]
	public bool ShowShortcut
	{
		get
		{
			return showshortcut;
		}
		set
		{
			showshortcut = value;
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
			text = value;
			if (text == "-")
			{
				separator = true;
			}
			else
			{
				separator = false;
			}
			OnUIATextChanged(EventArgs.Empty);
			ProcessMnemonic();
			Invalidate();
		}
	}

	[Localizable(true)]
	[DefaultValue(true)]
	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			if (value == visible)
			{
				return;
			}
			visible = value;
			if (menu_items != null)
			{
				foreach (MenuItem menu_item in menu_items)
				{
					menu_item.Visible = value;
				}
			}
			if (parent_menu != null)
			{
				parent_menu.OnMenuChanged(EventArgs.Empty);
			}
		}
	}

	internal new int Height
	{
		get
		{
			return bounds.Height;
		}
		set
		{
			bounds.Height = value;
		}
	}

	internal bool IsPopup
	{
		get
		{
			if (menu_items.Count > 0)
			{
				return true;
			}
			return false;
		}
	}

	internal bool MeasureEventDefined
	{
		get
		{
			if (ownerdraw && (object)base.Events[MeasureItem] != null)
			{
				return true;
			}
			return false;
		}
	}

	internal bool MenuBar
	{
		get
		{
			return menubar;
		}
		set
		{
			menubar = value;
		}
	}

	internal int MenuHeight
	{
		get
		{
			return menuheight;
		}
		set
		{
			menuheight = value;
		}
	}

	internal bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
		}
	}

	internal bool Separator
	{
		get
		{
			return separator;
		}
		set
		{
			separator = value;
		}
	}

	internal DrawItemState Status
	{
		get
		{
			DrawItemState drawItemState = DrawItemState.None;
			MenuTracker menuTracker = Parent.Tracker;
			if (Selected)
			{
				drawItemState |= ((menuTracker.active || menuTracker.Navigating) ? DrawItemState.Selected : DrawItemState.HotLight);
			}
			if (!Enabled)
			{
				drawItemState |= DrawItemState.Grayed | DrawItemState.Disabled;
			}
			if (Checked)
			{
				drawItemState |= DrawItemState.Checked;
			}
			if (!menuTracker.Navigating)
			{
				drawItemState |= DrawItemState.NoAccelerator;
			}
			return drawItemState;
		}
	}

	internal bool VisibleItems
	{
		get
		{
			if (menu_items != null)
			{
				foreach (MenuItem menu_item in menu_items)
				{
					if (menu_item.Visible)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	internal new int Width
	{
		get
		{
			return bounds.Width;
		}
		set
		{
			bounds.Width = value;
		}
	}

	internal new int X
	{
		get
		{
			return bounds.X;
		}
		set
		{
			bounds.X = value;
		}
	}

	internal int XTab
	{
		get
		{
			return xtab;
		}
		set
		{
			xtab = value;
		}
	}

	internal new int Y
	{
		get
		{
			return bounds.Y;
		}
		set
		{
			bounds.Y = value;
		}
	}

	public event EventHandler Click
	{
		add
		{
			base.Events.AddHandler(ClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ClickEvent, value);
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

	public event EventHandler Popup
	{
		add
		{
			base.Events.AddHandler(PopupEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PopupEvent, value);
		}
	}

	public event EventHandler Select
	{
		add
		{
			base.Events.AddHandler(SelectEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(SelectEvent, value);
		}
	}

	internal event EventHandler UIACheckedChanged
	{
		add
		{
			base.Events.AddHandler(UIACheckedChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIACheckedChangedEvent, value);
		}
	}

	internal event EventHandler UIARadioCheckChanged
	{
		add
		{
			base.Events.AddHandler(UIARadioCheckChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIARadioCheckChangedEvent, value);
		}
	}

	internal event EventHandler UIAEnabledChanged
	{
		add
		{
			base.Events.AddHandler(UIAEnabledChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAEnabledChangedEvent, value);
		}
	}

	internal event EventHandler UIATextChanged
	{
		add
		{
			base.Events.AddHandler(UIATextChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIATextChangedEvent, value);
		}
	}

	public MenuItem()
		: base(null)
	{
		CommonConstructor(string.Empty);
		shortcut = Shortcut.None;
	}

	public MenuItem(string text)
		: base(null)
	{
		CommonConstructor(text);
		shortcut = Shortcut.None;
	}

	public MenuItem(string text, EventHandler onClick)
		: base(null)
	{
		CommonConstructor(text);
		shortcut = Shortcut.None;
		Click += onClick;
	}

	public MenuItem(string text, MenuItem[] items)
		: base(items)
	{
		CommonConstructor(text);
		shortcut = Shortcut.None;
	}

	public MenuItem(string text, EventHandler onClick, Shortcut shortcut)
		: base(null)
	{
		CommonConstructor(text);
		Click += onClick;
		this.shortcut = shortcut;
	}

	public MenuItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut, string text, EventHandler onClick, EventHandler onPopup, EventHandler onSelect, MenuItem[] items)
		: base(items)
	{
		CommonConstructor(text);
		this.shortcut = shortcut;
		mergeorder = mergeOrder;
		mergetype = mergeType;
		Click += onClick;
		Popup += onPopup;
		Select += onSelect;
	}

	static MenuItem()
	{
		Click = new object();
		DrawItem = new object();
		MeasureItem = new object();
		Popup = new object();
		Select = new object();
		UIACheckedChanged = new object();
		UIARadioCheckChanged = new object();
		UIAEnabledChanged = new object();
		UIATextChanged = new object();
	}

	private void CommonConstructor(string text)
	{
		defaut_item = false;
		separator = false;
		break_ = false;
		bar_break = false;
		checked_ = false;
		radiocheck = false;
		enabled = true;
		showshortcut = true;
		visible = true;
		ownerdraw = false;
		menubar = false;
		menuheight = 0;
		xtab = 0;
		index = -1;
		mnemonic = '\0';
		menuid = -1;
		mergeorder = 0;
		mergetype = MenuMerge.Add;
		Text = text;
	}

	internal void OnUIACheckedChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIACheckedChanged])?.Invoke(this, e);
	}

	internal void OnUIARadioCheckChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIARadioCheckChanged])?.Invoke(this, e);
	}

	internal void OnUIAEnabledChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIAEnabledChanged])?.Invoke(this, e);
	}

	internal void OnUIATextChanged(EventArgs e)
	{
		((EventHandler)base.Events[UIATextChanged])?.Invoke(this, e);
	}

	public virtual MenuItem CloneMenu()
	{
		MenuItem menuItem = new MenuItem();
		menuItem.CloneMenu(this);
		return menuItem;
	}

	protected void CloneMenu(MenuItem itemSrc)
	{
		CloneMenu((Menu)itemSrc);
		MdiList = itemSrc.MdiList;
		is_window_menu_item = itemSrc.is_window_menu_item;
		bool flag = false;
		for (int num = base.MenuItems.Count - 1; num >= 0; num--)
		{
			if (base.MenuItems[num].is_window_menu_item)
			{
				base.MenuItems.RemoveAt(num);
				flag = true;
			}
		}
		if (flag)
		{
			PopulateWindowMenu();
		}
		BarBreak = itemSrc.BarBreak;
		Break = itemSrc.Break;
		Checked = itemSrc.Checked;
		DefaultItem = itemSrc.DefaultItem;
		Enabled = itemSrc.Enabled;
		MergeOrder = itemSrc.MergeOrder;
		MergeType = itemSrc.MergeType;
		OwnerDraw = itemSrc.OwnerDraw;
		RadioCheck = itemSrc.RadioCheck;
		Shortcut = itemSrc.Shortcut;
		ShowShortcut = itemSrc.ShowShortcut;
		Text = itemSrc.Text;
		Visible = itemSrc.Visible;
		base.Name = itemSrc.Name;
		base.Tag = itemSrc.Tag;
		base.Events[Click] = itemSrc.Events[Click];
		base.Events[DrawItem] = itemSrc.Events[DrawItem];
		base.Events[MeasureItem] = itemSrc.Events[MeasureItem];
		base.Events[Popup] = itemSrc.Events[Popup];
		base.Events[Select] = itemSrc.Events[Select];
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && parent_menu != null)
		{
			parent_menu.MenuItems.Remove(this);
		}
		base.Dispose(disposing);
	}

	public virtual MenuItem MergeMenu()
	{
		MenuItem menuItem = new MenuItem();
		menuItem.CloneMenu(this);
		return menuItem;
	}

	public void MergeMenu(MenuItem itemSrc)
	{
		base.MergeMenu(itemSrc);
	}

	protected virtual void OnClick(EventArgs e)
	{
		((EventHandler)base.Events[Click])?.Invoke(this, e);
	}

	protected virtual void OnDrawItem(DrawItemEventArgs e)
	{
		((DrawItemEventHandler)base.Events[DrawItem])?.Invoke(this, e);
	}

	protected virtual void OnInitMenuPopup(EventArgs e)
	{
		OnPopup(e);
	}

	protected virtual void OnMeasureItem(MeasureItemEventArgs e)
	{
		if (OwnerDraw)
		{
			((MeasureItemEventHandler)base.Events[MeasureItem])?.Invoke(this, e);
		}
	}

	protected virtual void OnPopup(EventArgs e)
	{
		((EventHandler)base.Events[Popup])?.Invoke(this, e);
	}

	protected virtual void OnSelect(EventArgs e)
	{
		((EventHandler)base.Events[Select])?.Invoke(this, e);
	}

	public void PerformClick()
	{
		OnClick(EventArgs.Empty);
	}

	public virtual void PerformSelect()
	{
		OnSelect(EventArgs.Empty);
	}

	public override string ToString()
	{
		return base.ToString() + ", Items.Count: " + base.MenuItems.Count + ", Text: " + text;
	}

	internal virtual void Invalidate()
	{
		if (Parent != null && Parent is MainMenu && Parent.Wnd != null)
		{
			Form form = Parent.Wnd.FindForm();
			if (form != null && form.IsHandleCreated)
			{
				XplatUI.RequestNCRecalc(form.Handle);
			}
		}
	}

	internal void PerformPopup()
	{
		OnPopup(EventArgs.Empty);
	}

	internal void PerformDrawItem(DrawItemEventArgs e)
	{
		PopulateWindowMenu();
		if (OwnerDraw)
		{
			OnDrawItem(e);
		}
		else
		{
			ThemeEngine.Current.DrawMenuItem(this, e);
		}
	}

	private void PopulateWindowMenu()
	{
		if (mdilist)
		{
			if (mdilist_items == null)
			{
				mdilist_items = new Hashtable();
				mdilist_forms = new Hashtable();
			}
			MainMenu mainMenu = GetMainMenu();
			if (mainMenu == null || mainMenu.GetForm() == null)
			{
				return;
			}
			Form form = mainMenu.GetForm();
			mdicontainer = form.MdiContainer;
			if (mdicontainer == null)
			{
				return;
			}
			MenuItem[] array = new MenuItem[mdilist_items.Count];
			mdilist_items.Keys.CopyTo(array, 0);
			MenuItem[] array2 = array;
			foreach (MenuItem menuItem in array2)
			{
				Form form2 = (Form)mdilist_items[menuItem];
				if (!mdicontainer.mdi_child_list.Contains(form2))
				{
					mdilist_items.Remove(menuItem);
					mdilist_forms.Remove(form2);
					base.MenuItems.Remove(menuItem);
				}
			}
			for (int j = 0; j < mdicontainer.mdi_child_list.Count; j++)
			{
				Form form3 = (Form)mdicontainer.mdi_child_list[j];
				MenuItem menuItem2;
				if (mdilist_forms.Contains(form3))
				{
					menuItem2 = (MenuItem)mdilist_forms[form3];
				}
				else
				{
					menuItem2 = new MenuItem();
					menuItem2.is_window_menu_item = true;
					menuItem2.Click += MdiWindowClickHandler;
					mdilist_items[menuItem2] = form3;
					mdilist_forms[form3] = menuItem2;
					base.MenuItems.AddNoEvents(menuItem2);
				}
				menuItem2.Visible = form3.Visible;
				menuItem2.Text = "&" + (j + 1) + " " + form3.Text;
				menuItem2.Checked = form.ActiveMdiChild == form3;
			}
		}
		else
		{
			if (mdilist_items == null)
			{
				return;
			}
			foreach (MenuItem value in mdilist_items.Values)
			{
				base.MenuItems.Remove(value);
			}
			mdilist_forms.Clear();
			mdilist_items.Clear();
		}
	}

	internal void PerformMeasureItem(MeasureItemEventArgs e)
	{
		OnMeasureItem(e);
	}

	private void ProcessMnemonic()
	{
		if (text == null || text.Length < 2)
		{
			mnemonic = '\0';
			return;
		}
		bool flag = false;
		for (int i = 0; i < text.Length - 1; i++)
		{
			if (text[i] == '&')
			{
				if (!flag && text[i + 1] != '&')
				{
					mnemonic = char.ToUpper(text[i + 1]);
					return;
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		mnemonic = '\0';
	}

	private string GetShortCutTextCtrl()
	{
		return "Ctrl";
	}

	private string GetShortCutTextAlt()
	{
		return "Alt";
	}

	private string GetShortCutTextShift()
	{
		return "Shift";
	}

	internal string GetShortCutText()
	{
		if (Shortcut >= Shortcut.CtrlA && Shortcut <= Shortcut.CtrlZ)
		{
			return GetShortCutTextCtrl() + "+" + (char)(65 + (Shortcut - 131137));
		}
		if (Shortcut >= Shortcut.Alt0 && Shortcut <= Shortcut.Alt9)
		{
			return GetShortCutTextAlt() + "+" + (char)(48 + (Shortcut - 262192));
		}
		if (Shortcut >= Shortcut.AltF1 && Shortcut <= Shortcut.AltF9)
		{
			return GetShortCutTextAlt() + "+F" + (char)(49 + (Shortcut - 262256));
		}
		if (Shortcut >= Shortcut.Ctrl0 && Shortcut <= Shortcut.Ctrl9)
		{
			return GetShortCutTextCtrl() + "+" + (char)(48 + (Shortcut - 131120));
		}
		if (Shortcut >= Shortcut.CtrlF1 && Shortcut <= Shortcut.CtrlF9)
		{
			return GetShortCutTextCtrl() + "+F" + (char)(49 + (Shortcut - 131184));
		}
		if (Shortcut >= Shortcut.CtrlShift0 && Shortcut <= Shortcut.CtrlShift9)
		{
			return GetShortCutTextCtrl() + "+" + GetShortCutTextShift() + "+" + (char)(48 + (Shortcut - 196656));
		}
		if (Shortcut >= Shortcut.CtrlShiftA && Shortcut <= Shortcut.CtrlShiftZ)
		{
			return GetShortCutTextCtrl() + "+" + GetShortCutTextShift() + "+" + (char)(65 + (Shortcut - 196673));
		}
		if (Shortcut >= Shortcut.CtrlShiftF1 && Shortcut <= Shortcut.CtrlShiftF9)
		{
			return GetShortCutTextCtrl() + "+" + GetShortCutTextShift() + "+F" + (char)(49 + (Shortcut - 196720));
		}
		if (Shortcut >= Shortcut.F1 && Shortcut <= Shortcut.F9)
		{
			return "F" + (char)(49 + (Shortcut - 112));
		}
		if (Shortcut >= Shortcut.ShiftF1 && Shortcut <= Shortcut.ShiftF9)
		{
			return GetShortCutTextShift() + "+F" + (char)(49 + (Shortcut - 65648));
		}
		return Shortcut switch
		{
			Shortcut.AltBksp => "AltBksp", 
			Shortcut.AltF10 => GetShortCutTextAlt() + "+F10", 
			Shortcut.AltF11 => GetShortCutTextAlt() + "+F11", 
			Shortcut.AltF12 => GetShortCutTextAlt() + "+F12", 
			Shortcut.CtrlDel => GetShortCutTextCtrl() + "+Del", 
			Shortcut.CtrlF10 => GetShortCutTextCtrl() + "+F10", 
			Shortcut.CtrlF11 => GetShortCutTextCtrl() + "+F11", 
			Shortcut.CtrlF12 => GetShortCutTextCtrl() + "+F12", 
			Shortcut.CtrlIns => GetShortCutTextCtrl() + "+Ins", 
			Shortcut.CtrlShiftF10 => GetShortCutTextCtrl() + "+" + GetShortCutTextShift() + "+F10", 
			Shortcut.CtrlShiftF11 => GetShortCutTextCtrl() + "+" + GetShortCutTextShift() + "+F11", 
			Shortcut.CtrlShiftF12 => GetShortCutTextCtrl() + "+" + GetShortCutTextShift() + "+F12", 
			Shortcut.Del => "Del", 
			Shortcut.F10 => "F10", 
			Shortcut.F11 => "F11", 
			Shortcut.F12 => "F12", 
			Shortcut.Ins => "Ins", 
			Shortcut.None => "None", 
			Shortcut.ShiftDel => GetShortCutTextShift() + "+Del", 
			Shortcut.ShiftF10 => GetShortCutTextShift() + "+F10", 
			Shortcut.ShiftF11 => GetShortCutTextShift() + "+F11", 
			Shortcut.ShiftF12 => GetShortCutTextShift() + "+F12", 
			Shortcut.ShiftIns => GetShortCutTextShift() + "+Ins", 
			_ => string.Empty, 
		};
	}

	private void MdiWindowClickHandler(object sender, EventArgs e)
	{
		Form form = (Form)mdilist_items[sender];
		if (form != null)
		{
			mdicontainer.ActivateChild(form);
		}
	}

	private void UpdateMenuItem()
	{
		if (parent_menu != null && parent_menu.Tracker != null)
		{
			parent_menu.Tracker.RemoveShortcuts(this);
			parent_menu.Tracker.AddShortcuts(this);
		}
	}
}
