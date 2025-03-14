using System.Collections;

namespace System.ComponentModel;

public class ListSortDescriptionCollection : IList, ICollection, IEnumerable
{
	private ArrayList list;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new InvalidOperationException("ListSortDescriptorCollection is read only.");
		}
	}

	bool IList.IsFixedSize => list.IsFixedSize;

	bool ICollection.IsSynchronized => list.IsSynchronized;

	object ICollection.SyncRoot => list.SyncRoot;

	bool IList.IsReadOnly => list.IsReadOnly;

	public int Count => list.Count;

	public ListSortDescription this[int index]
	{
		get
		{
			return list[index] as ListSortDescription;
		}
		set
		{
			throw new InvalidOperationException("ListSortDescriptorCollection is read only.");
		}
	}

	public ListSortDescriptionCollection()
	{
		list = new ArrayList();
	}

	public ListSortDescriptionCollection(ListSortDescription[] sorts)
	{
		list = new ArrayList();
		foreach (ListSortDescription value in sorts)
		{
			list.Add(value);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	}

	int IList.Add(object value)
	{
		return list.Add(value);
	}

	void IList.Clear()
	{
		list.Clear();
	}

	void IList.Insert(int index, object value)
	{
		list.Insert(index, value);
	}

	void IList.Remove(object value)
	{
		list.Remove(value);
	}

	void IList.RemoveAt(int index)
	{
		list.RemoveAt(index);
	}

	public bool Contains(object value)
	{
		return list.Contains(value);
	}

	public void CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	public int IndexOf(object value)
	{
		return list.IndexOf(value);
	}
}
