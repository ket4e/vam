using System.Collections;
using System.Drawing;
using System.Threading;

namespace System.Windows.Forms;

internal class MenuTracker
{
	private enum KeyNavState
	{
		Idle,
		Startup,
		NoPopups,
		Navigating
	}

	private enum ItemNavigation
	{
		First,
		Last,
		Next,
		Previous
	}

	internal bool active;

	internal bool popup_active;

	internal bool popdown_menu;

	internal bool hotkey_active;

	private bool mouse_down;

	public Menu CurrentMenu;

	public Menu TopMenu;

	public Control GrabControl;

	private Point last_motion = Point.Empty;

	private KeyNavState keynav_state;

	private Hashtable shortcuts = new Hashtable();

	public bool Navigating => keynav_state != 0 || active;

	public MenuTracker(Menu top_menu)
	{
		TopMenu = (CurrentMenu = top_menu);
		foreach (MenuItem menuItem in TopMenu.MenuItems)
		{
			AddShortcuts(menuItem);
		}
	}

	internal static Point ScreenToMenu(Menu menu, Point pnt)
	{
		int x = pnt.X;
		int y = pnt.Y;
		XplatUI.ScreenToMenu(menu.Wnd.window.Handle, ref x, ref y);
		return new Point(x, y);
	}

	private void UpdateCursor()
	{
		Control realChildAtPoint = GrabControl.GetRealChildAtPoint(Cursor.Position);
		if (realChildAtPoint != null)
		{
			if (active)
			{
				XplatUI.SetCursor(realChildAtPoint.Handle, Cursors.Default.handle);
			}
			else
			{
				XplatUI.SetCursor(realChildAtPoint.Handle, realChildAtPoint.Cursor.handle);
			}
		}
	}

	internal void Deactivate()
	{
		bool flag = keynav_state != 0 && TopMenu is MainMenu;
		active = false;
		popup_active = false;
		hotkey_active = false;
		if (GrabControl != null)
		{
			GrabControl.ActiveTracker = null;
		}
		keynav_state = KeyNavState.Idle;
		if (TopMenu is ContextMenu)
		{
			PopUpWindow popUpWindow = TopMenu.Wnd as PopUpWindow;
			DeselectItem(TopMenu.SelectedItem);
			popUpWindow?.HideWindow();
		}
		else
		{
			DeselectItem(TopMenu.SelectedItem);
		}
		CurrentMenu = TopMenu;
		if (flag)
		{
			(TopMenu as MainMenu).Draw();
		}
	}

	private MenuItem FindItemByCoords(Menu menu, Point pt)
	{
		pt = ((!(menu is MainMenu)) ? menu.Wnd.PointToClient(pt) : ScreenToMenu(menu, pt));
		foreach (MenuItem menuItem in menu.MenuItems)
		{
			Rectangle bounds = menuItem.bounds;
			if (bounds.Contains(pt))
			{
				return menuItem;
			}
		}
		return null;
	}

	private MenuItem GetItemAtXY(int x, int y)
	{
		Point pt = new Point(x, y);
		MenuItem menuItem = null;
		if (TopMenu.SelectedItem != null)
		{
			menuItem = FindSubItemByCoord(TopMenu.SelectedItem, Control.MousePosition);
		}
		if (menuItem == null)
		{
			menuItem = FindItemByCoords(TopMenu, pt);
		}
		return menuItem;
	}

	public bool OnMouseDown(MouseEventArgs args)
	{
		MenuItem itemAtXY = GetItemAtXY(args.X, args.Y);
		mouse_down = true;
		if (itemAtXY == null)
		{
			Deactivate();
			return false;
		}
		if ((args.Button & MouseButtons.Left) == 0)
		{
			return true;
		}
		if (!itemAtXY.Enabled)
		{
			return true;
		}
		popdown_menu = active && itemAtXY.VisibleItems;
		if (itemAtXY.IsPopup || itemAtXY.Parent is MainMenu)
		{
			active = true;
			itemAtXY.Parent.InvalidateItem(itemAtXY);
		}
		if (CurrentMenu == TopMenu && !popdown_menu)
		{
			SelectItem(itemAtXY.Parent, itemAtXY, itemAtXY.IsPopup);
		}
		GrabControl.ActiveTracker = this;
		return true;
	}

	public void OnMotion(MouseEventArgs args)
	{
		if (args.Location == last_motion)
		{
			return;
		}
		last_motion = args.Location;
		MenuItem itemAtXY = GetItemAtXY(args.X, args.Y);
		UpdateCursor();
		if (CurrentMenu.SelectedItem == itemAtXY)
		{
			return;
		}
		GrabControl.ActiveTracker = ((!active && itemAtXY == null) ? null : this);
		if (itemAtXY == null)
		{
			MenuItem selectedItem = CurrentMenu.SelectedItem;
			if ((active && selectedItem.VisibleItems && selectedItem.IsPopup && CurrentMenu is MainMenu) || keynav_state == KeyNavState.Navigating)
			{
				return;
			}
			if (selectedItem.Parent is MenuItem)
			{
				MenuItem menuItem = selectedItem.Parent as MenuItem;
				if (menuItem.IsPopup)
				{
					SelectItem(menuItem.Parent, menuItem, execute: false);
					return;
				}
			}
			if (CurrentMenu != TopMenu)
			{
				CurrentMenu = CurrentMenu.parent_menu;
			}
			DeselectItem(selectedItem);
		}
		else
		{
			keynav_state = KeyNavState.Idle;
			SelectItem(itemAtXY.Parent, itemAtXY, active && itemAtXY.IsPopup && popup_active && CurrentMenu.SelectedItem != itemAtXY);
		}
	}

	public void OnMouseUp(MouseEventArgs args)
	{
		if (!mouse_down)
		{
			return;
		}
		mouse_down = false;
		if ((args.Button & MouseButtons.Left) == 0)
		{
			return;
		}
		MenuItem itemAtXY = GetItemAtXY(args.X, args.Y);
		if (itemAtXY == null)
		{
			Deactivate();
		}
		else
		{
			if (!itemAtXY.Enabled)
			{
				return;
			}
			if ((CurrentMenu == TopMenu && !(CurrentMenu is ContextMenu) && popdown_menu) || !itemAtXY.IsPopup)
			{
				Deactivate();
				UpdateCursor();
			}
			if (!itemAtXY.IsPopup)
			{
				DeselectItem(itemAtXY);
				if (TopMenu != null && TopMenu.Wnd != null)
				{
					TopMenu.Wnd.FindForm()?.OnMenuComplete(EventArgs.Empty);
				}
				itemAtXY.PerformClick();
			}
		}
	}

	public static bool TrackPopupMenu(Menu menu, Point pnt)
	{
		if (menu.MenuItems.Count <= 0)
		{
			return true;
		}
		MenuTracker tracker = menu.tracker;
		tracker.active = true;
		tracker.popup_active = true;
		Control sourceControl = (tracker.TopMenu as ContextMenu).SourceControl;
		tracker.GrabControl = sourceControl.FindForm();
		if (tracker.GrabControl == null)
		{
			tracker.GrabControl = sourceControl.FindRootParent();
		}
		tracker.GrabControl.ActiveTracker = tracker;
		menu.Wnd = new PopUpWindow(tracker.GrabControl, menu);
		menu.Wnd.Location = menu.Wnd.PointToClient(pnt);
		((PopUpWindow)menu.Wnd).ShowWindow();
		bool flag = true;
		object queue_id = XplatUI.StartLoop(Thread.CurrentThread);
		while (menu.Wnd != null && menu.Wnd.Visible && flag)
		{
			MSG msg = default(MSG);
			flag = XplatUI.GetMessage(queue_id, ref msg, IntPtr.Zero, 0, 0);
			switch (msg.message)
			{
			case Msg.WM_KEYDOWN:
			case Msg.WM_KEYUP:
			case Msg.WM_CHAR:
			case Msg.WM_SYSKEYDOWN:
			case Msg.WM_SYSKEYUP:
			case Msg.WM_SYSCHAR:
			{
				Control control = Control.FromHandle(msg.hwnd);
				if (control != null)
				{
					Message msg2 = Message.Create(msg.hwnd, (int)msg.message, msg.wParam, msg.lParam);
					control.PreProcessControlMessageInternal(ref msg2);
				}
				break;
			}
			default:
				XplatUI.TranslateMessage(ref msg);
				XplatUI.DispatchMessage(ref msg);
				break;
			}
		}
		if (tracker.GrabControl.IsDisposed)
		{
			return true;
		}
		if (!flag)
		{
			XplatUI.PostQuitMessage(0);
		}
		if (menu.Wnd != null)
		{
			menu.Wnd.Dispose();
			menu.Wnd = null;
		}
		return true;
	}

	private void DeselectItem(MenuItem item)
	{
		if (item == null)
		{
			return;
		}
		item.Selected = false;
		if (item.IsPopup)
		{
			HideSubPopups(item, TopMenu);
			foreach (MenuItem menuItem in item.MenuItems)
			{
				if (menuItem.Selected)
				{
					DeselectItem(menuItem);
				}
			}
		}
		Menu parent = item.Parent;
		parent.InvalidateItem(item);
	}

	private void SelectItem(Menu menu, MenuItem item, bool execute)
	{
		MenuItem selectedItem = CurrentMenu.SelectedItem;
		if (selectedItem != item.Parent)
		{
			DeselectItem(selectedItem);
			if (CurrentMenu != menu && selectedItem.Parent != item && selectedItem.Parent is MenuItem)
			{
				DeselectItem(selectedItem.Parent as MenuItem);
			}
		}
		if (CurrentMenu != menu)
		{
			CurrentMenu = menu;
		}
		item.Selected = true;
		menu.InvalidateItem(item);
		if ((CurrentMenu == TopMenu && execute) || (CurrentMenu != TopMenu && popup_active))
		{
			item.PerformSelect();
		}
		if (execute && (selectedItem == null || item != selectedItem.Parent))
		{
			ExecFocusedItem(menu, item);
		}
	}

	private void ExecFocusedItem(Menu menu, MenuItem item)
	{
		if (item != null && item.Enabled)
		{
			if (item.IsPopup)
			{
				ShowSubPopup(menu, item);
				return;
			}
			Deactivate();
			item.PerformClick();
		}
	}

	private void ShowSubPopup(Menu menu, MenuItem item)
	{
		if (!item.Enabled)
		{
			return;
		}
		if (!popdown_menu || !item.VisibleItems)
		{
			item.PerformPopup();
		}
		if (item.VisibleItems)
		{
			if (item.Wnd != null)
			{
				item.Wnd.Dispose();
			}
			popup_active = true;
			PopUpWindow popUpWindow = new PopUpWindow(GrabControl, item);
			Point p = ((!(menu is MainMenu)) ? new Point(item.X + item.Width - 3, item.Y - 3) : new Point(item.X, item.Y + item.Height - 2 - menu.Height));
			p = menu.Wnd.PointToScreen(p);
			popUpWindow.Location = p;
			item.Wnd = popUpWindow;
			popUpWindow.ShowWindow();
		}
	}

	public static void HideSubPopups(Menu menu, Menu topmenu)
	{
		foreach (MenuItem menuItem in menu.MenuItems)
		{
			if (menuItem.IsPopup)
			{
				HideSubPopups(menuItem, null);
			}
		}
		if (menu.Wnd != null)
		{
			if (menu.Wnd is PopUpWindow popUpWindow)
			{
				popUpWindow.Hide();
				popUpWindow.Dispose();
			}
			menu.Wnd = null;
			if (topmenu != null && topmenu is MainMenu)
			{
				((MainMenu)topmenu).OnCollapse(EventArgs.Empty);
			}
		}
	}

	private MenuItem FindSubItemByCoord(Menu menu, Point pnt)
	{
		foreach (MenuItem menuItem3 in menu.MenuItems)
		{
			if (menuItem3.IsPopup && menuItem3.Wnd != null && menuItem3.Wnd.Visible && menuItem3 == menu.SelectedItem)
			{
				MenuItem menuItem2 = FindSubItemByCoord(menuItem3, pnt);
				if (menuItem2 != null)
				{
					return menuItem2;
				}
			}
			if (menu.Wnd != null && menu.Wnd.Visible)
			{
				Rectangle bounds = menuItem3.bounds;
				Point point = menu.Wnd.PointToScreen(new Point(menuItem3.X, menuItem3.Y));
				bounds.X = point.X;
				bounds.Y = point.Y;
				if (bounds.Contains(pnt))
				{
					return menuItem3;
				}
			}
		}
		return null;
	}

	private static MenuItem FindItemByKey(Menu menu, IntPtr key)
	{
		char c = char.ToUpper((char)((uint)key.ToInt32() & 0xFFu));
		foreach (MenuItem menuItem3 in menu.MenuItems)
		{
			if (menuItem3.Mnemonic == c)
			{
				return menuItem3;
			}
		}
		string value = c.ToString();
		foreach (MenuItem menuItem4 in menu.MenuItems)
		{
			if (menuItem4.Text.StartsWith(value))
			{
				return menuItem4;
			}
		}
		return null;
	}

	private static MenuItem GetNextItem(Menu menu, ItemNavigation navigation)
	{
		int i = 0;
		bool flag = false;
		for (int j = 0; j < menu.MenuItems.Count; j++)
		{
			MenuItem menuItem = menu.MenuItems[j];
			if (!menuItem.Separator && menuItem.Visible)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return null;
		}
		switch (navigation)
		{
		case ItemNavigation.First:
			for (i = 0; i < menu.MenuItems.Count; i++)
			{
				MenuItem menuItem = menu.MenuItems[i];
				if (!menuItem.Separator && menuItem.Visible)
				{
					break;
				}
			}
			break;
		case ItemNavigation.Next:
			i = ((menu.SelectedItem != null) ? menu.SelectedItem.Index : (-1));
			for (i++; i < menu.MenuItems.Count; i++)
			{
				MenuItem menuItem = menu.MenuItems[i];
				if (!menuItem.Separator && menuItem.Visible)
				{
					break;
				}
			}
			if (i < menu.MenuItems.Count)
			{
				break;
			}
			for (i = 0; i < menu.MenuItems.Count; i++)
			{
				MenuItem menuItem = menu.MenuItems[i];
				if (!menuItem.Separator && menuItem.Visible)
				{
					break;
				}
			}
			break;
		case ItemNavigation.Previous:
			if (menu.SelectedItem != null)
			{
				i = menu.SelectedItem.Index;
			}
			for (i--; i >= 0; i--)
			{
				MenuItem menuItem = menu.MenuItems[i];
				if (!menuItem.Separator && menuItem.Visible)
				{
					break;
				}
			}
			if (i >= 0)
			{
				break;
			}
			for (i = menu.MenuItems.Count - 1; i >= 0; i--)
			{
				MenuItem menuItem = menu.MenuItems[i];
				if (!menuItem.Separator && menuItem.Visible)
				{
					break;
				}
			}
			break;
		}
		return menu.MenuItems[i];
	}

	private void ProcessMenuKey(Msg msg_type)
	{
		if (TopMenu.MenuItems.Count == 0)
		{
			return;
		}
		MainMenu mainMenu = TopMenu as MainMenu;
		switch (msg_type)
		{
		case Msg.WM_SYSKEYDOWN:
			switch (keynav_state)
			{
			case KeyNavState.Idle:
				keynav_state = KeyNavState.Startup;
				hotkey_active = true;
				GrabControl.ActiveTracker = this;
				CurrentMenu = TopMenu;
				mainMenu.Draw();
				break;
			case KeyNavState.Startup:
				break;
			default:
				Deactivate();
				mainMenu.Draw();
				break;
			}
			break;
		case Msg.WM_SYSKEYUP:
			switch (keynav_state)
			{
			case KeyNavState.Idle:
			case KeyNavState.Navigating:
				break;
			case KeyNavState.Startup:
				keynav_state = KeyNavState.NoPopups;
				SelectItem(TopMenu, TopMenu.MenuItems[0], execute: false);
				break;
			default:
				Deactivate();
				mainMenu.Draw();
				break;
			}
			break;
		}
	}

	private bool ProcessMnemonic(Message msg, Keys key_data)
	{
		keynav_state = KeyNavState.Navigating;
		MenuItem menuItem = FindItemByKey(CurrentMenu, msg.WParam);
		if (menuItem == null || GrabControl == null)
		{
			return false;
		}
		active = true;
		GrabControl.ActiveTracker = this;
		SelectItem(CurrentMenu, menuItem, execute: true);
		if (menuItem.IsPopup)
		{
			CurrentMenu = menuItem;
			SelectItem(menuItem, menuItem.MenuItems[0], execute: false);
		}
		return true;
	}

	public void AddShortcuts(MenuItem item)
	{
		foreach (MenuItem menuItem in item.MenuItems)
		{
			AddShortcuts(menuItem);
			if (menuItem.Shortcut != 0)
			{
				shortcuts[(int)menuItem.Shortcut] = menuItem;
			}
		}
		if (item.Shortcut != 0)
		{
			shortcuts[(int)item.Shortcut] = item;
		}
	}

	public void RemoveShortcuts(MenuItem item)
	{
		foreach (MenuItem menuItem in item.MenuItems)
		{
			RemoveShortcuts(menuItem);
			if (menuItem.Shortcut != 0)
			{
				shortcuts.Remove((int)menuItem.Shortcut);
			}
		}
		if (item.Shortcut != 0)
		{
			shortcuts.Remove((int)item.Shortcut);
		}
	}

	private bool ProcessShortcut(Keys keyData)
	{
		if (!(shortcuts[(int)keyData] is MenuItem menuItem) || !menuItem.Enabled)
		{
			return false;
		}
		if (active)
		{
			Deactivate();
		}
		menuItem.PerformClick();
		return true;
	}

	public bool ProcessKeys(ref Message msg, Keys keyData)
	{
		if ((keyData & Keys.Alt) == Keys.Alt && active)
		{
			Deactivate();
			return false;
		}
		if ((keyData & Keys.Alt) == Keys.Alt && (keyData & Keys.F4) == Keys.F4)
		{
			if (GrabControl != null)
			{
				GrabControl.ActiveTracker = null;
			}
			return false;
		}
		if (msg.Msg != 261 && ProcessShortcut(keyData))
		{
			return true;
		}
		if ((keyData & Keys.KeyCode) == Keys.Menu && TopMenu is MainMenu)
		{
			ProcessMenuKey((Msg)msg.Msg);
			return true;
		}
		if ((keyData & Keys.Alt) == Keys.Alt)
		{
			return ProcessMnemonic(msg, keyData);
		}
		if (msg.Msg == 261)
		{
			return false;
		}
		if (!Navigating)
		{
			return false;
		}
		switch (keyData)
		{
		case Keys.Up:
		{
			if (CurrentMenu is MainMenu)
			{
				return true;
			}
			if (CurrentMenu.MenuItems.Count == 1 && CurrentMenu.parent_menu == TopMenu)
			{
				DeselectItem(CurrentMenu.SelectedItem);
				CurrentMenu = TopMenu;
				return true;
			}
			MenuItem selectedItem = GetNextItem(CurrentMenu, ItemNavigation.Previous);
			if (selectedItem != null)
			{
				SelectItem(CurrentMenu, selectedItem, execute: false);
			}
			break;
		}
		case Keys.Down:
		{
			MenuItem selectedItem;
			if (CurrentMenu is MainMenu)
			{
				if (CurrentMenu.SelectedItem != null && CurrentMenu.SelectedItem.IsPopup)
				{
					keynav_state = KeyNavState.Navigating;
					selectedItem = CurrentMenu.SelectedItem;
					ShowSubPopup(CurrentMenu, selectedItem);
					SelectItem(selectedItem, selectedItem.MenuItems[0], execute: false);
					CurrentMenu = selectedItem;
					active = true;
					GrabControl.ActiveTracker = this;
				}
				return true;
			}
			selectedItem = GetNextItem(CurrentMenu, ItemNavigation.Next);
			if (selectedItem != null)
			{
				SelectItem(CurrentMenu, selectedItem, execute: false);
			}
			break;
		}
		case Keys.Right:
		{
			if (CurrentMenu is MainMenu)
			{
				MenuItem selectedItem = GetNextItem(CurrentMenu, ItemNavigation.Next);
				bool flag = selectedItem.IsPopup && keynav_state != KeyNavState.NoPopups;
				SelectItem(CurrentMenu, selectedItem, flag);
				if (flag)
				{
					SelectItem(selectedItem, selectedItem.MenuItems[0], execute: false);
					CurrentMenu = selectedItem;
				}
				break;
			}
			if (CurrentMenu.SelectedItem != null && CurrentMenu.SelectedItem.IsPopup)
			{
				MenuItem selectedItem = CurrentMenu.SelectedItem;
				ShowSubPopup(CurrentMenu, selectedItem);
				SelectItem(selectedItem, selectedItem.MenuItems[0], execute: false);
				CurrentMenu = selectedItem;
				break;
			}
			Menu parent_menu = CurrentMenu.parent_menu;
			while (parent_menu != null && !(parent_menu is MainMenu))
			{
				parent_menu = parent_menu.parent_menu;
			}
			if (parent_menu is MainMenu)
			{
				MenuItem selectedItem = GetNextItem(parent_menu, ItemNavigation.Next);
				SelectItem(parent_menu, selectedItem, selectedItem.IsPopup);
				if (selectedItem.IsPopup)
				{
					SelectItem(selectedItem, selectedItem.MenuItems[0], execute: false);
					CurrentMenu = selectedItem;
				}
			}
			break;
		}
		case Keys.Left:
			if (CurrentMenu is MainMenu)
			{
				MenuItem selectedItem = GetNextItem(CurrentMenu, ItemNavigation.Previous);
				bool flag2 = selectedItem.IsPopup && keynav_state != KeyNavState.NoPopups;
				SelectItem(CurrentMenu, selectedItem, flag2);
				if (flag2)
				{
					SelectItem(selectedItem, selectedItem.MenuItems[0], execute: false);
					CurrentMenu = selectedItem;
				}
			}
			else if (CurrentMenu.parent_menu is MainMenu)
			{
				MenuItem selectedItem = GetNextItem(CurrentMenu.parent_menu, ItemNavigation.Previous);
				SelectItem(CurrentMenu.parent_menu, selectedItem, selectedItem.IsPopup);
				if (selectedItem.IsPopup)
				{
					SelectItem(selectedItem, selectedItem.MenuItems[0], execute: false);
					CurrentMenu = selectedItem;
				}
			}
			else if (!(CurrentMenu is ContextMenu))
			{
				HideSubPopups(CurrentMenu, TopMenu);
				if (CurrentMenu.parent_menu != null)
				{
					CurrentMenu = CurrentMenu.parent_menu;
				}
			}
			break;
		case Keys.Return:
			if (CurrentMenu.SelectedItem != null && CurrentMenu.SelectedItem.IsPopup)
			{
				keynav_state = KeyNavState.Navigating;
				MenuItem selectedItem = CurrentMenu.SelectedItem;
				ShowSubPopup(CurrentMenu, selectedItem);
				SelectItem(selectedItem, selectedItem.MenuItems[0], execute: false);
				CurrentMenu = selectedItem;
				active = true;
				GrabControl.ActiveTracker = this;
			}
			else
			{
				ExecFocusedItem(CurrentMenu, CurrentMenu.SelectedItem);
			}
			break;
		case Keys.Escape:
			Deactivate();
			break;
		default:
			ProcessMnemonic(msg, keyData);
			break;
		}
		return active;
	}
}
