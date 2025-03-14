using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class MenuStrip : ToolStrip
{
	private class MenuStripAccessibleObject : AccessibleObject
	{
	}

	private ToolStripMenuItem mdi_window_list_item;

	private static object MenuActivateEvent;

	private static object MenuDeactivateEvent;

	[DefaultValue(false)]
	[Browsable(false)]
	public new bool CanOverflow
	{
		get
		{
			return base.CanOverflow;
		}
		set
		{
			base.CanOverflow = value;
		}
	}

	[DefaultValue(ToolStripGripStyle.Hidden)]
	public new ToolStripGripStyle GripStyle
	{
		get
		{
			return base.GripStyle;
		}
		set
		{
			base.GripStyle = value;
		}
	}

	[TypeConverter(typeof(MdiWindowListItemConverter))]
	[MergableProperty(false)]
	[DefaultValue(null)]
	public ToolStripMenuItem MdiWindowListItem
	{
		get
		{
			return mdi_window_list_item;
		}
		set
		{
			if (mdi_window_list_item != value)
			{
				mdi_window_list_item = value;
				RefreshMdiItems();
			}
		}
	}

	[DefaultValue(false)]
	public new bool ShowItemToolTips
	{
		get
		{
			return base.ShowItemToolTips;
		}
		set
		{
			base.ShowItemToolTips = value;
		}
	}

	[DefaultValue(true)]
	public new bool Stretch
	{
		get
		{
			return base.Stretch;
		}
		set
		{
			base.Stretch = value;
		}
	}

	protected override Padding DefaultGripMargin => new Padding(2, 2, 0, 2);

	protected override Padding DefaultPadding => new Padding(6, 2, 0, 2);

	protected override bool DefaultShowItemToolTips => false;

	protected override Size DefaultSize => new Size(200, 24);

	internal override bool KeyboardActive
	{
		get
		{
			return base.KeyboardActive;
		}
		set
		{
			if (base.KeyboardActive != value)
			{
				base.KeyboardActive = value;
				if (value)
				{
					OnMenuActivate(EventArgs.Empty);
				}
				else
				{
					OnMenuDeactivate(EventArgs.Empty);
				}
			}
		}
	}

	internal bool MenuDroppedDown
	{
		get
		{
			return menu_selected;
		}
		set
		{
			menu_selected = value;
		}
	}

	public event EventHandler MenuActivate
	{
		add
		{
			base.Events.AddHandler(MenuActivateEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MenuActivateEvent, value);
		}
	}

	public event EventHandler MenuDeactivate
	{
		add
		{
			base.Events.AddHandler(MenuDeactivateEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MenuDeactivateEvent, value);
		}
	}

	public MenuStrip()
	{
		base.CanOverflow = false;
		GripStyle = ToolStripGripStyle.Hidden;
		Stretch = true;
		Dock = DockStyle.Top;
	}

	static MenuStrip()
	{
		MenuActivate = new object();
		MenuDeactivate = new object();
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new MenuStripAccessibleObject();
	}

	protected internal override ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick)
	{
		return new ToolStripMenuItem(text, image, onClick);
	}

	protected virtual void OnMenuActivate(EventArgs e)
	{
		((EventHandler)base.Events[MenuActivate])?.Invoke(this, e);
	}

	protected virtual void OnMenuDeactivate(EventArgs e)
	{
		((EventHandler)base.Events[MenuDeactivate])?.Invoke(this, e);
	}

	protected override bool ProcessCmdKey(ref Message m, Keys keyData)
	{
		return base.ProcessCmdKey(ref m, keyData);
	}

	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	internal override void Dismiss(ToolStripDropDownCloseReason reason)
	{
		MenuDroppedDown = false;
		base.Dismiss(reason);
	}

	internal void FireMenuActivate()
	{
		ToolStripManager.AppClicked += ToolStripMenuTracker_AppClicked;
		ToolStripManager.AppFocusChange += ToolStripMenuTracker_AppFocusChange;
		OnMenuActivate(EventArgs.Empty);
	}

	internal void FireMenuDeactivate()
	{
		ToolStripManager.AppClicked -= ToolStripMenuTracker_AppClicked;
		ToolStripManager.AppFocusChange -= ToolStripMenuTracker_AppFocusChange;
		OnMenuDeactivate(EventArgs.Empty);
	}

	internal override bool OnMenuKey()
	{
		ToolStripManager.SetActiveToolStrip(this, keyboard: true);
		ToolStripItem toolStripItem = SelectNextToolStripItem(null, forward: true);
		if (toolStripItem == null)
		{
			return false;
		}
		if (toolStripItem is MdiControlStrip.SystemMenuItem)
		{
			SelectNextToolStripItem(toolStripItem, forward: true);
		}
		return true;
	}

	private void ToolStripMenuTracker_AppFocusChange(object sender, EventArgs e)
	{
		GetTopLevelToolStrip().Dismiss(ToolStripDropDownCloseReason.AppFocusChange);
	}

	private void ToolStripMenuTracker_AppClicked(object sender, EventArgs e)
	{
		GetTopLevelToolStrip().Dismiss(ToolStripDropDownCloseReason.AppClicked);
	}

	internal void RefreshMdiItems()
	{
		if (mdi_window_list_item == null)
		{
			return;
		}
		Form form = FindForm();
		if (form == null || form.MainMenuStrip != this)
		{
			return;
		}
		MdiClient mdiContainer = form.MdiContainer;
		if (mdiContainer == null)
		{
			return;
		}
		ToolStripItem[] array = new ToolStripItem[mdi_window_list_item.DropDownItems.Count];
		mdi_window_list_item.DropDownItems.CopyTo(array, 0);
		ToolStripItem[] array2 = array;
		foreach (ToolStripItem toolStripItem in array2)
		{
			if (toolStripItem is ToolStripMenuItem && (toolStripItem as ToolStripMenuItem).IsMdiWindowListEntry && (!mdiContainer.mdi_child_list.Contains((toolStripItem as ToolStripMenuItem).MdiClientForm) || !(toolStripItem as ToolStripMenuItem).MdiClientForm.Visible))
			{
				mdi_window_list_item.DropDownItems.Remove(toolStripItem);
			}
		}
		for (int j = 0; j < mdiContainer.mdi_child_list.Count; j++)
		{
			Form form2 = (Form)mdiContainer.mdi_child_list[j];
			if (!form2.Visible)
			{
				continue;
			}
			ToolStripMenuItem toolStripMenuItem;
			if ((toolStripMenuItem = FindMdiMenuItemOfForm(form2)) == null)
			{
				if (CountMdiMenuItems() == 0 && mdi_window_list_item.DropDownItems.Count > 0 && !(mdi_window_list_item.DropDownItems[mdi_window_list_item.DropDownItems.Count - 1] is ToolStripSeparator))
				{
					mdi_window_list_item.DropDownItems.Add(new ToolStripSeparator());
				}
				toolStripMenuItem = new ToolStripMenuItem();
				toolStripMenuItem.MdiClientForm = form2;
				mdi_window_list_item.DropDownItems.Add(toolStripMenuItem);
			}
			toolStripMenuItem.Text = $"&{j + 1} {form2.Text}";
			toolStripMenuItem.Checked = form.ActiveMdiChild == form2;
		}
		if (NeedToReorderMdi())
		{
			ReorderMdiMenu();
		}
	}

	private ToolStripMenuItem FindMdiMenuItemOfForm(Form f)
	{
		foreach (ToolStripItem dropDownItem in mdi_window_list_item.DropDownItems)
		{
			if (dropDownItem is ToolStripMenuItem && (dropDownItem as ToolStripMenuItem).MdiClientForm == f)
			{
				return (ToolStripMenuItem)dropDownItem;
			}
		}
		return null;
	}

	private int CountMdiMenuItems()
	{
		int num = 0;
		foreach (ToolStripItem dropDownItem in mdi_window_list_item.DropDownItems)
		{
			if (dropDownItem is ToolStripMenuItem && (dropDownItem as ToolStripMenuItem).IsMdiWindowListEntry)
			{
				num++;
			}
		}
		return num;
	}

	private bool NeedToReorderMdi()
	{
		bool flag = false;
		foreach (ToolStripItem dropDownItem in mdi_window_list_item.DropDownItems)
		{
			if (!(dropDownItem is ToolStripMenuItem))
			{
				continue;
			}
			if (!(dropDownItem as ToolStripMenuItem).IsMdiWindowListEntry)
			{
				if (flag)
				{
					return true;
				}
			}
			else
			{
				flag = true;
			}
		}
		return false;
	}

	private void ReorderMdiMenu()
	{
		ToolStripItem[] array = new ToolStripItem[mdi_window_list_item.DropDownItems.Count];
		mdi_window_list_item.DropDownItems.CopyTo(array, 0);
		mdi_window_list_item.DropDownItems.Clear();
		ToolStripItem[] array2 = array;
		foreach (ToolStripItem toolStripItem in array2)
		{
			if (toolStripItem is ToolStripSeparator || !(toolStripItem as ToolStripMenuItem).IsMdiWindowListEntry)
			{
				mdi_window_list_item.DropDownItems.Add(toolStripItem);
			}
		}
		int count = mdi_window_list_item.DropDownItems.Count;
		if (count > 0 && !(mdi_window_list_item.DropDownItems[count - 1] is ToolStripSeparator))
		{
			mdi_window_list_item.DropDownItems.Add(new ToolStripSeparator());
		}
		ToolStripItem[] array3 = array;
		foreach (ToolStripItem toolStripItem2 in array3)
		{
			if (toolStripItem2 is ToolStripMenuItem && (toolStripItem2 as ToolStripMenuItem).IsMdiWindowListEntry)
			{
				mdi_window_list_item.DropDownItems.Add(toolStripItem2);
			}
		}
	}
}
