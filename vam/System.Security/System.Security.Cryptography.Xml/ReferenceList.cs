using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography.Xml;

public sealed class ReferenceList : IEnumerable, IList, ICollection
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
			this[index] = (EncryptedReference)value;
		}
	}

	bool IList.IsFixedSize => false;

	bool IList.IsReadOnly => false;

	public int Count => list.Count;

	public bool IsSynchronized => list.IsSynchronized;

	[IndexerName("ItemOf")]
	public EncryptedReference this[int index]
	{
		get
		{
			return (EncryptedReference)list[index];
		}
		set
		{
			list[index] = value;
		}
	}

	public object SyncRoot => list.SyncRoot;

	public ReferenceList()
	{
		list = new ArrayList();
	}

	public int Add(object value)
	{
		if (!(value is EncryptedReference))
		{
			throw new ArgumentException("value");
		}
		return list.Add(value);
	}

	public void Clear()
	{
		list.Clear();
	}

	public bool Contains(object value)
	{
		return list.Contains(value);
	}

	public void CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public EncryptedReference Item(int index)
	{
		return (EncryptedReference)list[index];
	}

	public int IndexOf(object value)
	{
		return list.IndexOf(value);
	}

	public void Insert(int index, object value)
	{
		if (!(value is EncryptedReference))
		{
			throw new ArgumentException("value");
		}
		list.Insert(index, value);
	}

	public void Remove(object value)
	{
		list.Remove(value);
	}

	public void RemoveAt(int index)
	{
		list.RemoveAt(index);
	}
}
