using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms;

[ListBindable(false)]
public class ListViewGroupCollection : ICollection, IEnumerable, IList
{
	private List<ListViewGroup> list;

	private ListView list_view_owner;

	private ListViewGroup default_group;

	bool ICollection.IsSynchronized => true;

	object ICollection.SyncRoot => this;

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
			if (value is ListViewGroup)
			{
				this[index] = (ListViewGroup)value;
			}
		}
	}

	internal ListView ListViewOwner
	{
		get
		{
			return list_view_owner;
		}
		set
		{
			list_view_owner = value;
		}
	}

	public int Count => list.Count;

	public ListViewGroup this[int index]
	{
		get
		{
			if (list.Count <= index || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return list[index];
		}
		set
		{
			if (list.Count <= index || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (!Contains(value))
			{
				if (value != null)
				{
					CheckListViewItemsInGroup(value);
				}
				list[index] = value;
				if (list_view_owner != null)
				{
					list_view_owner.Redraw(recalculate: true);
				}
			}
		}
	}

	public ListViewGroup this[string key]
	{
		get
		{
			int num = IndexOfKey(key);
			if (num != -1)
			{
				return this[num];
			}
			return null;
		}
		set
		{
			int num = IndexOfKey(key);
			if (num != -1)
			{
				this[num] = value;
			}
		}
	}

	internal int InternalCount => list.Count + 1;

	internal ListViewGroup DefaultGroup => default_group;

	private ListViewGroupCollection()
	{
		list = new List<ListViewGroup>();
		default_group = new ListViewGroup("Default Group");
		default_group.IsDefault = true;
	}

	internal ListViewGroupCollection(ListView listViewOwner)
		: this()
	{
		list_view_owner = listViewOwner;
		default_group.ListViewOwner = listViewOwner;
	}

	int IList.Add(object value)
	{
		if (!(value is ListViewGroup))
		{
			throw new ArgumentException("value");
		}
		return Add((ListViewGroup)value);
	}

	bool IList.Contains(object value)
	{
		if (value is ListViewGroup)
		{
			return Contains((ListViewGroup)value);
		}
		return false;
	}

	int IList.IndexOf(object value)
	{
		if (value is ListViewGroup)
		{
			return IndexOf((ListViewGroup)value);
		}
		return -1;
	}

	void IList.Insert(int index, object value)
	{
		if (value is ListViewGroup)
		{
			Insert(index, (ListViewGroup)value);
		}
	}

	void IList.Remove(object value)
	{
		Remove((ListViewGroup)value);
	}

	public IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public void CopyTo(Array array, int index)
	{
		((ICollection)list).CopyTo(array, index);
	}

	public int Add(ListViewGroup group)
	{
		if (Contains(group))
		{
			return -1;
		}
		AddGroup(group);
		if (list_view_owner != null)
		{
			list_view_owner.Redraw(recalculate: true);
		}
		return list.Count - 1;
	}

	public ListViewGroup Add(string key, string headerText)
	{
		ListViewGroup listViewGroup = new ListViewGroup(key, headerText);
		Add(listViewGroup);
		return listViewGroup;
	}

	public void Clear()
	{
		foreach (ListViewGroup item in list)
		{
			item.ListViewOwner = null;
		}
		list.Clear();
		if (list_view_owner != null)
		{
			list_view_owner.Redraw(recalculate: true);
		}
	}

	public bool Contains(ListViewGroup value)
	{
		return list.Contains(value);
	}

	public int IndexOf(ListViewGroup value)
	{
		return list.IndexOf(value);
	}

	public void Insert(int index, ListViewGroup group)
	{
		if (!Contains(group))
		{
			CheckListViewItemsInGroup(group);
			group.ListViewOwner = list_view_owner;
			list.Insert(index, group);
			if (list_view_owner != null)
			{
				list_view_owner.Redraw(recalculate: true);
			}
		}
	}

	public void Remove(ListViewGroup group)
	{
		int num = list.IndexOf(group);
		if (num != -1)
		{
			RemoveAt(num);
		}
	}

	public void RemoveAt(int index)
	{
		if (list.Count > index && index >= 0)
		{
			ListViewGroup listViewGroup = list[index];
			listViewGroup.ListViewOwner = null;
			list.RemoveAt(index);
			if (list_view_owner != null)
			{
				list_view_owner.Redraw(recalculate: true);
			}
		}
	}

	private int IndexOfKey(string key)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].Name == key)
			{
				return i;
			}
		}
		return -1;
	}

	public void AddRange(ListViewGroup[] groups)
	{
		foreach (ListViewGroup group in groups)
		{
			AddGroup(group);
		}
		if (list_view_owner != null)
		{
			list_view_owner.Redraw(recalculate: true);
		}
	}

	public void AddRange(ListViewGroupCollection groups)
	{
		foreach (ListViewGroup group in groups)
		{
			AddGroup(group);
		}
		if (list_view_owner != null)
		{
			list_view_owner.Redraw(recalculate: true);
		}
	}

	internal ListViewGroup GetInternalGroup(int index)
	{
		if (index == 0)
		{
			return default_group;
		}
		return list[index - 1];
	}

	private void AddGroup(ListViewGroup group)
	{
		if (!Contains(group))
		{
			CheckListViewItemsInGroup(group);
			group.ListViewOwner = list_view_owner;
			list.Add(group);
		}
	}

	private void CheckListViewItemsInGroup(ListViewGroup value)
	{
		foreach (ListViewItem item in value.Items)
		{
			if (item.ListView != null && item.ListView != list_view_owner)
			{
				throw new ArgumentException("ListViewItem belongs to a ListView control other than the one that owns this ListViewGroupCollection.", "ListViewGroup");
			}
		}
	}
}
