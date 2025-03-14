using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Xml;

public sealed class EncryptionPropertyCollection : IEnumerable, IList, ICollection
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
			this[index] = (EncryptionProperty)value;
		}
	}

	public int Count => list.Count;

	public bool IsFixedSize => list.IsFixedSize;

	public bool IsReadOnly => list.IsReadOnly;

	public bool IsSynchronized => list.IsSynchronized;

	[IndexerName("ItemOf")]
	public EncryptionProperty this[int index]
	{
		get
		{
			return (EncryptionProperty)list[index];
		}
		set
		{
			list[index] = value;
		}
	}

	public object SyncRoot => list.SyncRoot;

	public EncryptionPropertyCollection()
	{
		list = new ArrayList();
	}

	bool IList.Contains(object value)
	{
		return Contains((EncryptionProperty)value);
	}

	int IList.Add(object value)
	{
		return Add((EncryptionProperty)value);
	}

	int IList.IndexOf(object value)
	{
		return IndexOf((EncryptionProperty)value);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, (EncryptionProperty)value);
	}

	void IList.Remove(object value)
	{
		Remove((EncryptionProperty)value);
	}

	public int Add(EncryptionProperty value)
	{
		return list.Add(value);
	}

	public void Clear()
	{
		list.Clear();
	}

	public bool Contains(EncryptionProperty value)
	{
		return list.Contains(value);
	}

	public void CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	public void CopyTo(EncryptionProperty[] array, int index)
	{
		list.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public int IndexOf(EncryptionProperty value)
	{
		return list.IndexOf(value);
	}

	public void Insert(int index, EncryptionProperty value)
	{
		list.Insert(index, value);
	}

	public EncryptionProperty Item(int index)
	{
		return (EncryptionProperty)list[index];
	}

	public void Remove(EncryptionProperty value)
	{
		list.Remove(value);
	}

	public void RemoveAt(int index)
	{
		list.RemoveAt(index);
	}
}
