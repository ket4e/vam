using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ListBindable(false)]
[ToolboxItemFilter("System.Windows.Forms", ToolboxItemFilterType.Allow)]
public abstract class Menu : Component
{
	[ListBindable(false)]
	public class MenuItemCollection : ICollection, IEnumerable, IList
	{
		private Menu owner;

		private ArrayList items = new ArrayList();

		bool ICollection.IsSynchronized => false;

		object ICollection.SyncRoot => this;

		bool IList.IsFixedSize => false;

		object IList.this[int index]
		{
			get
			{
				return items[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count => items.Count;

		public bool IsReadOnly => false;

		public virtual MenuItem this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("Index of out range");
				}
				return (MenuItem)items[index];
			}
		}

		public virtual MenuItem this[string key]
		{
			get
			{
				if (string.IsNullOrEmpty(key))
				{
					return null;
				}
				foreach (MenuItem item in items)
				{
					if (string.Compare(item.Name, key, ignoreCase: true) == 0)
					{
						return item;
					}
				}
				return null;
			}
		}

		public MenuItemCollection(Menu owner)
		{
			this.owner = owner;
		}

		int IList.Add(object value)
		{
			return Add((MenuItem)value);
		}

		bool IList.Contains(object value)
		{
			return Contains((MenuItem)value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((MenuItem)value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (MenuItem)value);
		}

		void IList.Remove(object value)
		{
			Remove((MenuItem)value);
		}

		public virtual int Add(MenuItem item)
		{
			if (item.Parent != null)
			{
				item.Parent.MenuItems.Remove(item);
			}
			items.Add(item);
			item.Index = items.Count - 1;
			UpdateItem(item);
			owner.OnMenuChanged(EventArgs.Empty);
			if (owner.parent_menu != null)
			{
				owner.parent_menu.OnMenuChanged(EventArgs.Empty);
			}
			return items.Count - 1;
		}

		internal void AddNoEvents(MenuItem mi)
		{
			if (mi.Parent != null)
			{
				mi.Parent.MenuItems.Remove(mi);
			}
			items.Add(mi);
			mi.Index = items.Count - 1;
			mi.parent_menu = owner;
		}

		public virtual MenuItem Add(string caption)
		{
			MenuItem menuItem = new MenuItem(caption);
			Add(menuItem);
			return menuItem;
		}

		public virtual int Add(int index, MenuItem item)
		{
			if (index < 0 || index > Count)
			{
				throw new ArgumentOutOfRangeException("Index of out range");
			}
			ArrayList arrayList = new ArrayList(Count + 1);
			for (int i = 0; i < index; i++)
			{
				arrayList.Add(items[i]);
			}
			arrayList.Add(item);
			for (int j = index; j < Count; j++)
			{
				arrayList.Add(items[j]);
			}
			items = arrayList;
			UpdateItemsIndices();
			UpdateItem(item);
			return index;
		}

		private void UpdateItem(MenuItem mi)
		{
			mi.parent_menu = owner;
			owner.OnMenuChanged(EventArgs.Empty);
			if (owner.parent_menu != null)
			{
				owner.parent_menu.OnMenuChanged(EventArgs.Empty);
			}
			if (owner.Tracker != null)
			{
				owner.Tracker.AddShortcuts(mi);
			}
		}

		internal void Insert(int index, MenuItem mi)
		{
			if (index < 0 || index > Count)
			{
				throw new ArgumentOutOfRangeException("Index of out range");
			}
			items.Insert(index, mi);
			UpdateItemsIndices();
			UpdateItem(mi);
		}

		public virtual MenuItem Add(string caption, EventHandler onClick)
		{
			MenuItem menuItem = new MenuItem(caption, onClick);
			Add(menuItem);
			return menuItem;
		}

		public virtual MenuItem Add(string caption, MenuItem[] items)
		{
			MenuItem menuItem = new MenuItem(caption, items);
			Add(menuItem);
			return menuItem;
		}

		public virtual void AddRange(MenuItem[] items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (MenuItem item in items)
			{
				Add(item);
			}
		}

		public virtual void Clear()
		{
			MenuTracker tracker = owner.Tracker;
			foreach (MenuItem item in items)
			{
				tracker?.RemoveShortcuts(item);
				item.parent_menu = null;
			}
			items.Clear();
			owner.OnMenuChanged(EventArgs.Empty);
		}

		public bool Contains(MenuItem value)
		{
			return items.Contains(value);
		}

		public virtual bool ContainsKey(string key)
		{
			return this[key] != null;
		}

		public void CopyTo(Array dest, int index)
		{
			items.CopyTo(dest, index);
		}

		public MenuItem[] Find(string key, bool searchAllChildren)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			List<MenuItem> list = new List<MenuItem>();
			foreach (MenuItem item in items)
			{
				if (string.Compare(item.Name, key, ignoreCase: true) == 0)
				{
					list.Add(item);
				}
			}
			if (searchAllChildren)
			{
				foreach (MenuItem item2 in items)
				{
					list.AddRange(item2.MenuItems.Find(key, searchAllChildren: true));
				}
			}
			return list.ToArray();
		}

		public IEnumerator GetEnumerator()
		{
			return items.GetEnumerator();
		}

		public int IndexOf(MenuItem value)
		{
			return items.IndexOf(value);
		}

		public virtual int IndexOfKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return -1;
			}
			return IndexOf(this[key]);
		}

		public virtual void Remove(MenuItem item)
		{
			RemoveAt(item.Index);
		}

		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new ArgumentOutOfRangeException("Index of out range");
			}
			MenuItem menuItem = (MenuItem)items[index];
			owner.Tracker?.RemoveShortcuts(menuItem);
			menuItem.parent_menu = null;
			items.RemoveAt(index);
			UpdateItemsIndices();
			owner.OnMenuChanged(EventArgs.Empty);
		}

		public virtual void RemoveByKey(string key)
		{
			Remove(this[key]);
		}

		private void UpdateItemsIndices()
		{
			for (int i = 0; i < Count; i++)
			{
				((MenuItem)items[i]).Index = i;
			}
		}
	}

	public const int FindHandle = 0;

	public const int FindShortcut = 1;

	internal MenuItemCollection menu_items;

	internal IntPtr menu_handle = IntPtr.Zero;

	internal Menu parent_menu;

	private Rectangle rect;

	internal Control Wnd;

	internal MenuTracker tracker;

	private string control_name;

	private object control_tag;

	private static object MenuChangedEvent;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IntPtr Handle => menu_handle;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual bool IsParent
	{
		get
		{
			if (menu_items != null && menu_items.Count > 0)
			{
				return true;
			}
			return false;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public MenuItem MdiListItem
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	[MergableProperty(false)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public MenuItemCollection MenuItems => menu_items;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string Name
	{
		get
		{
			return control_name;
		}
		set
		{
			control_name = value;
		}
	}

	[MWFCategory("Data")]
	[DefaultValue(null)]
	[TypeConverter(typeof(StringConverter))]
	[Bindable(true)]
	[Localizable(false)]
	public object Tag
	{
		get
		{
			return control_tag;
		}
		set
		{
			control_tag = value;
		}
	}

	internal Rectangle Rect => rect;

	internal MenuItem SelectedItem
	{
		get
		{
			foreach (MenuItem menuItem in MenuItems)
			{
				if (menuItem.Selected)
				{
					return menuItem;
				}
			}
			return null;
		}
	}

	internal int Height
	{
		get
		{
			return rect.Height;
		}
		set
		{
			rect.Height = value;
		}
	}

	internal int Width
	{
		get
		{
			return rect.Width;
		}
		set
		{
			rect.Width = value;
		}
	}

	internal int X
	{
		get
		{
			return rect.X;
		}
		set
		{
			rect.X = value;
		}
	}

	internal int Y
	{
		get
		{
			return rect.Y;
		}
		set
		{
			rect.Y = value;
		}
	}

	internal MenuTracker Tracker
	{
		get
		{
			Menu menu = this;
			while (menu.parent_menu != null)
			{
				menu = menu.parent_menu;
			}
			return menu.tracker;
		}
	}

	internal event EventHandler MenuChanged
	{
		add
		{
			base.Events.AddHandler(MenuChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MenuChangedEvent, value);
		}
	}

	protected Menu(MenuItem[] items)
	{
		menu_items = new MenuItemCollection(this);
		if (items != null)
		{
			menu_items.AddRange(items);
		}
	}

	static Menu()
	{
		MenuChanged = new object();
	}

	internal virtual void OnMenuChanged(EventArgs e)
	{
		((EventHandler)base.Events[MenuChanged])?.Invoke(this, e);
	}

	protected void CloneMenu(Menu menuSrc)
	{
		Dispose(disposing: true);
		menu_items = new MenuItemCollection(this);
		for (int i = 0; i < menuSrc.MenuItems.Count; i++)
		{
			menu_items.Add(menuSrc.MenuItems[i].CloneMenu());
		}
	}

	protected virtual IntPtr CreateMenuHandle()
	{
		return IntPtr.Zero;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && menu_handle != IntPtr.Zero)
		{
			menu_handle = IntPtr.Zero;
		}
	}

	public MenuItem FindMenuItem(int type, IntPtr value)
	{
		return null;
	}

	protected int FindMergePosition(int mergeOrder)
	{
		int num = MenuItems.Count;
		int num2 = 0;
		while (num2 < num)
		{
			int num3 = (num2 + num) / 2;
			if (MenuItems[num3].MergeOrder > mergeOrder)
			{
				num = num3;
			}
			else
			{
				num2 = num3 + 1;
			}
		}
		return num2;
	}

	public ContextMenu GetContextMenu()
	{
		for (Menu menu = this; menu != null; menu = menu.parent_menu)
		{
			if (menu is ContextMenu)
			{
				return (ContextMenu)menu;
			}
		}
		return null;
	}

	public MainMenu GetMainMenu()
	{
		for (Menu menu = this; menu != null; menu = menu.parent_menu)
		{
			if (menu is MainMenu)
			{
				return (MainMenu)menu;
			}
		}
		return null;
	}

	internal virtual void InvalidateItem(MenuItem item)
	{
		if (Wnd != null)
		{
			Wnd.Invalidate(item.bounds);
		}
	}

	public virtual void MergeMenu(Menu menuSrc)
	{
		if (menuSrc == this)
		{
			throw new ArgumentException("The menu cannot be merged with itself");
		}
		if (menuSrc == null)
		{
			return;
		}
		for (int i = 0; i < menuSrc.MenuItems.Count; i++)
		{
			MenuItem menuItem = menuSrc.MenuItems[i];
			switch (menuItem.MergeType)
			{
			case MenuMerge.Add:
			{
				int index = FindMergePosition(menuItem.MergeOrder);
				MenuItems.Add(index, menuItem.CloneMenu());
				break;
			}
			case MenuMerge.Replace:
			case MenuMerge.MergeItems:
			{
				for (int j = FindMergePosition(menuItem.MergeOrder - 1); j <= MenuItems.Count; j++)
				{
					if (j >= MenuItems.Count || MenuItems[j].MergeOrder != menuItem.MergeOrder)
					{
						MenuItems.Add(j, menuItem.CloneMenu());
						break;
					}
					MenuItem menuItem2 = MenuItems[j];
					if (menuItem2.MergeType != 0)
					{
						if (menuItem.MergeType == MenuMerge.MergeItems && menuItem2.MergeType == MenuMerge.MergeItems)
						{
							menuItem2.MergeMenu(menuItem);
							break;
						}
						MenuItems.Remove(menuItem);
						MenuItems.Add(j, menuItem.CloneMenu());
						break;
					}
				}
				break;
			}
			}
		}
	}

	protected internal virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (tracker == null)
		{
			return false;
		}
		return tracker.ProcessKeys(ref msg, keyData);
	}

	public override string ToString()
	{
		return base.ToString() + ", Items.Count: " + MenuItems.Count;
	}
}
