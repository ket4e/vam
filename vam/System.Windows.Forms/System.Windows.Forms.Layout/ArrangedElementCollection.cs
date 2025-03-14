using System.Collections;

namespace System.Windows.Forms.Layout;

public class ArrangedElementCollection : ICollection, IEnumerable, IList
{
	internal ArrayList list;

	bool IList.IsFixedSize => IsFixedSize;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			this[index] = value;
		}
	}

	bool ICollection.IsSynchronized => list.IsSynchronized;

	object ICollection.SyncRoot => list.IsSynchronized;

	public virtual int Count => list.Count;

	public virtual bool IsReadOnly => list.IsReadOnly;

	internal bool IsFixedSize => list.IsFixedSize;

	internal object this[int index]
	{
		get
		{
			return list[index];
		}
		set
		{
			list[index] = value;
		}
	}

	internal ArrangedElementCollection()
	{
		list = new ArrayList();
	}

	int IList.Add(object value)
	{
		return Add(value);
	}

	void IList.Clear()
	{
		Clear();
	}

	bool IList.Contains(object value)
	{
		return Contains(value);
	}

	int IList.IndexOf(object value)
	{
		return IndexOf(value);
	}

	void IList.Insert(int index, object value)
	{
		throw new NotSupportedException();
	}

	void IList.Remove(object value)
	{
		Remove(value);
	}

	void IList.RemoveAt(int index)
	{
		list.RemoveAt(index);
	}

	public void CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	public override bool Equals(object obj)
	{
		if (obj is ArrangedElementCollection && this == obj)
		{
			return true;
		}
		return false;
	}

	public virtual IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	internal int Add(object value)
	{
		return list.Add(value);
	}

	internal void Clear()
	{
		list.Clear();
	}

	internal bool Contains(object value)
	{
		return list.Contains(value);
	}

	internal int IndexOf(object value)
	{
		return list.IndexOf(value);
	}

	internal void Insert(int index, object value)
	{
		list.Insert(index, value);
	}

	internal void Remove(object value)
	{
		list.Remove(value);
	}

	internal void InternalRemoveAt(int index)
	{
		list.RemoveAt(index);
	}
}
