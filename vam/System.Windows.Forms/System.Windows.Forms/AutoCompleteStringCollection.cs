using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public class AutoCompleteStringCollection : ICollection, IEnumerable, IList
{
	private ArrayList list;

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
			this[index] = (string)value;
		}
	}

	public int Count => list.Count;

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	public bool IsReadOnly => false;

	public string this[int index]
	{
		get
		{
			return (string)list[index];
		}
		set
		{
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, list[index]));
			list[index] = value;
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
	}

	public event CollectionChangeEventHandler CollectionChanged;

	public AutoCompleteStringCollection()
	{
		list = new ArrayList();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	int IList.Add(object value)
	{
		return Add((string)value);
	}

	bool IList.Contains(object value)
	{
		return Contains((string)value);
	}

	int IList.IndexOf(object value)
	{
		return IndexOf((string)value);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, (string)value);
	}

	void IList.Remove(object value)
	{
		Remove((string)value);
	}

	protected void OnCollectionChanged(CollectionChangeEventArgs e)
	{
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, e);
		}
	}

	public IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public void CopyTo(string[] array, int index)
	{
		list.CopyTo(array, index);
	}

	public int Add(string value)
	{
		int result = list.Add(value);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		return result;
	}

	public void AddRange(string[] value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value", "Argument cannot be null!");
		}
		list.AddRange(value);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
	}

	public void Clear()
	{
		list.Clear();
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
	}

	public bool Contains(string value)
	{
		return list.Contains(value);
	}

	public int IndexOf(string value)
	{
		return list.IndexOf(value);
	}

	public void Insert(int index, string value)
	{
		list.Insert(index, value);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
	}

	public void Remove(string value)
	{
		list.Remove(value);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
	}

	public void RemoveAt(int index)
	{
		string element = this[index];
		list.RemoveAt(index);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
	}
}
