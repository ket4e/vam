using System.Drawing;

namespace System.Windows.Forms;

internal class MdiWindowManager : InternalWindowManager
{
	private MainMenu merged_menu;

	private MainMenu maximized_menu;

	private MenuItem icon_menu;

	private ContextMenu icon_popup_menu;

	internal bool was_minimized;

	private PaintEventHandler draw_maximized_buttons;

	internal EventHandler form_closed_handler;

	private MdiClient mdi_container;

	private Rectangle prev_virtual_position;

	private Point icon_clicked;

	private DateTime icon_clicked_time;

	private bool icon_dont_show_popup;

	private TitleButtons maximized_title_buttons;

	private bool is_visible_pending;

	private byte last_activation_event;

	public override int MenuHeight => 0;

	internal bool IsVisiblePending
	{
		get
		{
			return is_visible_pending;
		}
		set
		{
			is_visible_pending = value;
		}
	}

	private TitleButtons MaximizedTitleButtons
	{
		get
		{
			if (maximized_title_buttons == null)
			{
				maximized_title_buttons = new TitleButtons(base.Form);
				maximized_title_buttons.CloseButton.Visible = true;
				maximized_title_buttons.RestoreButton.Visible = true;
				maximized_title_buttons.MinimizeButton.Visible = true;
			}
			return maximized_title_buttons;
		}
	}

	internal override Rectangle MaximizedBounds
	{
		get
		{
			Rectangle clientRectangle = mdi_container.ClientRectangle;
			int num = ThemeEngine.Current.ManagedWindowBorderWidth(this);
			int titleBarHeight = base.TitleBarHeight;
			return new Rectangle(clientRectangle.Left - num, clientRectangle.Top - titleBarHeight - num, clientRectangle.Width + num * 2, clientRectangle.Height + titleBarHeight + num * 2);
		}
	}

	public MainMenu MergedMenu
	{
		get
		{
			if (merged_menu == null)
			{
				merged_menu = CreateMergedMenu();
			}
			return merged_menu;
		}
	}

	public MainMenu MaximizedMenu
	{
		get
		{
			if (maximized_menu == null)
			{
				maximized_menu = CreateMaximizedMenu();
			}
			return maximized_menu;
		}
	}

	public override bool IsActive
	{
		get
		{
			if (mdi_container == null)
			{
				return false;
			}
			return mdi_container.ActiveMdiChild == form;
		}
	}

	public MdiWindowManager(Form form, MdiClient mdi_container)
		: base(form)
	{
		this.mdi_container = mdi_container;
		if (form.WindowState == FormWindowState.Normal)
		{
			base.NormalBounds = form.Bounds;
		}
		form_closed_handler = FormClosed;
		form.Closed += form_closed_handler;
		form.TextChanged += FormTextChangedHandler;
		form.SizeChanged += FormSizeChangedHandler;
		form.LocationChanged += FormLocationChangedHandler;
		form.VisibleChanged += FormVisibleChangedHandler;
		draw_maximized_buttons = DrawMaximizedButtons;
		CreateIconMenus();
	}

	public void RaiseActivated()
	{
		if (last_activation_event != 1)
		{
			last_activation_event = 1;
			form.OnActivatedInternal();
			form.SelectActiveControl();
		}
	}

	public void RaiseDeactivate()
	{
		if (last_activation_event == 1)
		{
			last_activation_event = 2;
			form.OnDeactivateInternal();
		}
	}

	private void FormVisibleChangedHandler(object sender, EventArgs e)
	{
		if (mdi_container != null)
		{
			if (form.Visible)
			{
				mdi_container.ActivateChild(form);
			}
			else if (mdi_container.Controls.Count > 1)
			{
				mdi_container.ActivateActiveMdiChild();
			}
		}
	}

	private void FormTextChangedHandler(object sender, EventArgs e)
	{
		mdi_container.SetParentText(text_changed: false);
		if (form.MdiParent.MainMenuStrip != null)
		{
			form.MdiParent.MainMenuStrip.RefreshMdiItems();
		}
	}

	private void FormLocationChangedHandler(object sender, EventArgs e)
	{
		if (form.window_state == FormWindowState.Minimized)
		{
			base.IconicBounds = form.Bounds;
		}
		form.MdiParent.MdiContainer.SizeScrollBars();
	}

	private void FormSizeChangedHandler(object sender, EventArgs e)
	{
		if (form.window_state == FormWindowState.Maximized && form.Bounds != MaximizedBounds)
		{
			form.Bounds = MaximizedBounds;
		}
		form.MdiParent.MdiContainer.SizeScrollBars();
	}

	private MainMenu CreateMergedMenu()
	{
		Form form = (Form)mdi_container.Parent;
		MainMenu mainMenu = ((form.Menu == null) ? new MainMenu() : form.Menu.CloneMenu());
		if (base.form.WindowState == FormWindowState.Maximized)
		{
		}
		mainMenu.MergeMenu(base.form.Menu);
		mainMenu.MenuChanged += MenuChangedHandler;
		mainMenu.SetForm(form);
		return mainMenu;
	}

	private MainMenu CreateMaximizedMenu()
	{
		Form form = (Form)mdi_container.Parent;
		if (base.form.MainMenuStrip != null || form.MainMenuStrip != null)
		{
			return null;
		}
		MainMenu mainMenu = new MainMenu();
		if (form.Menu != null)
		{
			MainMenu menuSrc = form.Menu.CloneMenu();
			mainMenu.MergeMenu(menuSrc);
		}
		if (base.form.Menu != null)
		{
			MainMenu menuSrc2 = base.form.Menu.CloneMenu();
			mainMenu.MergeMenu(menuSrc2);
		}
		if (mainMenu.MenuItems.Count == 0)
		{
			mainMenu.MenuItems.Add(new MenuItem());
		}
		mainMenu.MenuItems.Insert(0, icon_menu);
		mainMenu.SetForm(form);
		return mainMenu;
	}

	private void CreateIconMenus()
	{
		icon_menu = new MenuItem();
		icon_popup_menu = new ContextMenu();
		icon_menu.OwnerDraw = true;
		icon_menu.MeasureItem += MeasureIconMenuItem;
		icon_menu.DrawItem += DrawIconMenuItem;
		icon_menu.Click += ClickIconMenuItem;
		MenuItem menuItem = new MenuItem("&Restore", RestoreItemHandler);
		MenuItem menuItem2 = new MenuItem("&Move", MoveItemHandler);
		MenuItem menuItem3 = new MenuItem("&Size", SizeItemHandler);
		MenuItem menuItem4 = new MenuItem("Mi&nimize", MinimizeItemHandler);
		MenuItem menuItem5 = new MenuItem("Ma&ximize", MaximizeItemHandler);
		MenuItem menuItem6 = new MenuItem("&Close", CloseItemHandler);
		MenuItem menuItem7 = new MenuItem("Nex&t", NextItemHandler);
		icon_menu.MenuItems.AddRange(new MenuItem[7] { menuItem, menuItem2, menuItem3, menuItem4, menuItem5, menuItem6, menuItem7 });
		icon_popup_menu.MenuItems.AddRange(new MenuItem[7] { menuItem, menuItem2, menuItem3, menuItem4, menuItem5, menuItem6, menuItem7 });
	}

	private void ClickIconMenuItem(object sender, EventArgs e)
	{
		if ((DateTime.Now - icon_clicked_time).TotalMilliseconds <= (double)SystemInformation.DoubleClickTime)
		{
			form.Close();
			return;
		}
		icon_clicked_time = DateTime.Now;
		Point empty = Point.Empty;
		empty = form.MdiParent.PointToScreen(empty);
		empty = form.PointToClient(empty);
		ShowPopup(empty);
	}

	internal void ShowPopup(Point pnt)
	{
		if (form.WindowState == FormWindowState.Maximized && form.MdiParent.MainMenuStrip != null && form.MdiParent.MainMenuStrip.Items.Count > 0)
		{
			ToolStripItem toolStripItem = form.MdiParent.MainMenuStrip.Items[0];
			if (toolStripItem is MdiControlStrip.SystemMenuItem)
			{
				(toolStripItem as MdiControlStrip.SystemMenuItem).ShowDropDown();
				return;
			}
		}
		icon_popup_menu.MenuItems[0].Enabled = form.window_state != FormWindowState.Normal;
		icon_popup_menu.MenuItems[1].Enabled = form.window_state != FormWindowState.Maximized;
		icon_popup_menu.MenuItems[2].Enabled = form.window_state != FormWindowState.Maximized;
		icon_popup_menu.MenuItems[3].Enabled = form.window_state != FormWindowState.Minimized;
		icon_popup_menu.MenuItems[4].Enabled = form.window_state != FormWindowState.Maximized;
		icon_popup_menu.MenuItems[5].Enabled = true;
		icon_popup_menu.MenuItems[6].Enabled = true;
		icon_popup_menu.Show(form, pnt);
	}

	private void RestoreItemHandler(object sender, EventArgs e)
	{
		form.WindowState = FormWindowState.Normal;
	}

	private void MoveItemHandler(object sender, EventArgs e)
	{
		int x = 0;
		int y = 0;
		PointToScreen(ref x, ref y);
		Cursor.Position = new Point(x, y);
		form.Cursor = Cursors.Cross;
		state = State.Moving;
		form.Capture = true;
	}

	private void SizeItemHandler(object sender, EventArgs e)
	{
		int x = 0;
		int y = 0;
		PointToScreen(ref x, ref y);
		Cursor.Position = new Point(x, y);
		form.Cursor = Cursors.Cross;
		state = State.Sizing;
		form.Capture = true;
	}

	private void MinimizeItemHandler(object sender, EventArgs e)
	{
		form.WindowState = FormWindowState.Minimized;
	}

	private void MaximizeItemHandler(object sender, EventArgs e)
	{
		if (form.WindowState != FormWindowState.Maximized)
		{
			form.WindowState = FormWindowState.Maximized;
		}
	}

	private void CloseItemHandler(object sender, EventArgs e)
	{
		form.Close();
	}

	private void NextItemHandler(object sender, EventArgs e)
	{
		mdi_container.ActivateNextChild();
	}

	private void DrawIconMenuItem(object sender, DrawItemEventArgs de)
	{
		de.Graphics.DrawIcon(form.Icon, new Rectangle(de.Bounds.X + 2, de.Bounds.Y + 2, de.Bounds.Height - 4, de.Bounds.Height - 4));
	}

	private void MeasureIconMenuItem(object sender, MeasureItemEventArgs me)
	{
		int num = (me.ItemHeight = SystemInformation.MenuHeight);
		me.ItemWidth = num + 2;
	}

	private void MenuChangedHandler(object sender, EventArgs e)
	{
		CreateMergedMenu();
	}

	public override void PointToClient(ref int x, ref int y)
	{
		XplatUI.ScreenToClient(mdi_container.Handle, ref x, ref y);
	}

	public override void PointToScreen(ref int x, ref int y)
	{
		XplatUI.ClientToScreen(mdi_container.Handle, ref x, ref y);
	}

	public override void UpdateWindowDecorations(FormWindowState window_state)
	{
		if (MaximizedMenu != null)
		{
			switch (window_state)
			{
			case FormWindowState.Normal:
			case FormWindowState.Minimized:
				MaximizedMenu.Paint -= draw_maximized_buttons;
				MaximizedTitleButtons.Visible = false;
				base.TitleButtons.Visible = true;
				break;
			case FormWindowState.Maximized:
				MaximizedMenu.Paint += draw_maximized_buttons;
				MaximizedTitleButtons.Visible = true;
				base.TitleButtons.Visible = false;
				break;
			}
		}
		base.UpdateWindowDecorations(window_state);
	}

	public override void SetWindowState(FormWindowState old_state, FormWindowState window_state)
	{
		mdi_container.SetWindowState(form, old_state, window_state, is_activating_child: false);
	}

	private void FormClosed(object sender, EventArgs e)
	{
		mdi_container.ChildFormClosed(form);
		if (form.MdiParent.MainMenuStrip != null)
		{
			form.MdiParent.MainMenuStrip.RefreshMdiItems();
		}
		mdi_container.RemoveControlMenuItems(this);
	}

	public override void DrawMaximizedButtons(object sender, PaintEventArgs pe)
	{
		Size size = ThemeEngine.Current.ManagedWindowGetMenuButtonSize(this);
		Point menuOrigin = XplatUI.GetMenuOrigin(mdi_container.ParentForm.Handle);
		int num = ThemeEngine.Current.ManagedWindowBorderWidth(this);
		TitleButtons maximizedTitleButtons = MaximizedTitleButtons;
		maximizedTitleButtons.Visible = true;
		base.TitleButtons.Visible = false;
		maximizedTitleButtons.CloseButton.Rectangle = new Rectangle(mdi_container.ParentForm.Size.Width - 1 - num - size.Width - 2, menuOrigin.Y + 2, size.Width, size.Height);
		maximizedTitleButtons.RestoreButton.Rectangle = new Rectangle(maximizedTitleButtons.CloseButton.Rectangle.Left - 2 - size.Width, menuOrigin.Y + 2, size.Width, size.Height);
		maximizedTitleButtons.MinimizeButton.Rectangle = new Rectangle(maximizedTitleButtons.RestoreButton.Rectangle.Left - size.Width, menuOrigin.Y + 2, size.Width, size.Height);
		DrawTitleButton(pe.Graphics, maximizedTitleButtons.MinimizeButton, pe.ClipRectangle);
		DrawTitleButton(pe.Graphics, maximizedTitleButtons.RestoreButton, pe.ClipRectangle);
		DrawTitleButton(pe.Graphics, maximizedTitleButtons.CloseButton, pe.ClipRectangle);
		maximizedTitleButtons.MinimizeButton.Rectangle.Y -= menuOrigin.Y;
		maximizedTitleButtons.RestoreButton.Rectangle.Y -= menuOrigin.Y;
		maximizedTitleButtons.CloseButton.Rectangle.Y -= menuOrigin.Y;
	}

	public bool HandleMenuMouseDown(MainMenu menu, int x, int y)
	{
		Point point = MenuTracker.ScreenToMenu(menu, new Point(x, y));
		HandleTitleBarDown(point.X, point.Y);
		return base.TitleButtons.AnyPushedTitleButtons;
	}

	public void HandleMenuMouseUp(MainMenu menu, int x, int y)
	{
		Point point = MenuTracker.ScreenToMenu(menu, new Point(x, y));
		HandleTitleBarUp(point.X, point.Y);
	}

	public void HandleMenuMouseLeave(MainMenu menu, int x, int y)
	{
		Point point = MenuTracker.ScreenToMenu(menu, new Point(x, y));
		HandleTitleBarLeave(point.X, point.Y);
	}

	public void HandleMenuMouseMove(MainMenu menu, int x, int y)
	{
		Point point = MenuTracker.ScreenToMenu(menu, new Point(x, y));
		HandleTitleBarMouseMove(point.X, point.Y);
	}

	protected override void HandleTitleBarLeave(int x, int y)
	{
		base.HandleTitleBarLeave(x, y);
		if (maximized_title_buttons != null)
		{
			maximized_title_buttons.MouseLeave(x, y);
		}
		if (base.IsMaximized)
		{
			XplatUI.InvalidateNC(form.MdiParent.Handle);
		}
	}

	protected override void HandleTitleBarUp(int x, int y)
	{
		if (IconRectangleContains(x, y))
		{
			if (!icon_dont_show_popup)
			{
				if (base.IsMaximized)
				{
					ClickIconMenuItem(null, null);
				}
				else
				{
					ShowPopup(Point.Empty);
				}
			}
			else
			{
				icon_dont_show_popup = false;
			}
			return;
		}
		bool isMaximized = base.IsMaximized;
		base.HandleTitleBarUp(x, y);
		if (maximized_title_buttons != null && isMaximized)
		{
			maximized_title_buttons.MouseUp(x, y);
		}
		if (base.IsMaximized)
		{
			XplatUI.InvalidateNC(mdi_container.Parent.Handle);
		}
	}

	protected override void HandleTitleBarDoubleClick(int x, int y)
	{
		if (IconRectangleContains(x, y))
		{
			form.Close();
		}
		else if (form.MaximizeBox)
		{
			form.WindowState = FormWindowState.Maximized;
		}
		base.HandleTitleBarDoubleClick(x, y);
	}

	protected override void HandleTitleBarDown(int x, int y)
	{
		if (IconRectangleContains(x, y))
		{
			if ((DateTime.Now - icon_clicked_time).TotalMilliseconds <= (double)SystemInformation.DoubleClickTime && icon_clicked.X == x && icon_clicked.Y == y)
			{
				form.Close();
				return;
			}
			icon_clicked_time = DateTime.Now;
			icon_clicked.X = x;
			icon_clicked.Y = y;
			return;
		}
		base.HandleTitleBarDown(x, y);
		if (maximized_title_buttons != null)
		{
			maximized_title_buttons.MouseDown(x, y);
		}
		if (base.IsMaximized)
		{
			XplatUI.InvalidateNC(mdi_container.Parent.Handle);
		}
	}

	protected override void HandleTitleBarMouseMove(int x, int y)
	{
		base.HandleTitleBarMouseMove(x, y);
		if (maximized_title_buttons != null && maximized_title_buttons.MouseMove(x, y))
		{
			XplatUI.InvalidateNC(form.MdiParent.Handle);
		}
	}

	protected override bool HandleLButtonDblClick(ref Message m)
	{
		int x = Control.LowOrder(m.LParam.ToInt32());
		int y = Control.HighOrder(m.LParam.ToInt32());
		NCClientToNC(ref x, ref y);
		if (IconRectangleContains(x, y))
		{
			icon_popup_menu.Wnd.Hide();
			form.Close();
			return true;
		}
		return base.HandleLButtonDblClick(ref m);
	}

	protected override bool HandleLButtonDown(ref Message m)
	{
		int x = Control.LowOrder(m.LParam.ToInt32());
		int y = Control.HighOrder(m.LParam.ToInt32());
		NCClientToNC(ref x, ref y);
		if (IconRectangleContains(x, y))
		{
			if ((DateTime.Now - icon_clicked_time).TotalMilliseconds <= (double)SystemInformation.DoubleClickTime)
			{
				if (icon_popup_menu != null && icon_popup_menu.Wnd != null)
				{
					icon_popup_menu.Wnd.Hide();
				}
				form.Close();
				return true;
			}
			if (form.Capture)
			{
				icon_dont_show_popup = true;
			}
		}
		return base.HandleLButtonDown(ref m);
	}

	protected override bool ShouldRemoveWindowManager(FormBorderStyle style)
	{
		return false;
	}

	protected override void HandleWindowMove(Message m)
	{
		Point position = Cursor.Position;
		Point point = MouseMove(position);
		if (point.X != 0 || point.Y != 0)
		{
			int x = virtual_position.X + point.X;
			int y = virtual_position.Y + point.Y;
			Rectangle clientRectangle = mdi_container.ClientRectangle;
			if (mdi_container.VerticalScrollbarVisible)
			{
				clientRectangle.Width -= SystemInformation.VerticalScrollBarWidth;
			}
			if (mdi_container.HorizontalScrollbarVisible)
			{
				clientRectangle.Height -= SystemInformation.HorizontalScrollBarHeight;
			}
			UpdateVP(x, y, form.Width, form.Height);
			start = position;
		}
	}

	protected override bool HandleNCMouseMove(ref Message m)
	{
		XplatUI.RequestAdditionalWM_NCMessages(form.Handle, hover: true, leave: true);
		return base.HandleNCMouseMove(ref m);
	}

	protected override void DrawVirtualPosition(Rectangle virtual_position)
	{
		ClearVirtualPosition();
		if (form.Parent != null)
		{
			XplatUI.DrawReversibleRectangle(form.Parent.Handle, virtual_position, 2);
		}
		prev_virtual_position = virtual_position;
	}

	protected override void ClearVirtualPosition()
	{
		if (prev_virtual_position != Rectangle.Empty && form.Parent != null)
		{
			XplatUI.DrawReversibleRectangle(form.Parent.Handle, prev_virtual_position, 2);
		}
		prev_virtual_position = Rectangle.Empty;
	}

	protected override void OnWindowFinishedMoving()
	{
		form.Refresh();
	}

	protected override void Activate()
	{
		if (mdi_container.ActiveMdiChild != form)
		{
			mdi_container.ActivateChild(form);
		}
		base.Activate();
	}
}
