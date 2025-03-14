using System.Collections;

namespace System.Data.Odbc;

[Serializable]
public sealed class OdbcErrorCollection : IEnumerable, ICollection
{
	private readonly ArrayList _items = new ArrayList();

	object ICollection.SyncRoot => _items.SyncRoot;

	bool ICollection.IsSynchronized => _items.IsSynchronized;

	public int Count => _items.Count;

	public OdbcError this[int i] => (OdbcError)_items[i];

	internal OdbcErrorCollection()
	{
	}

	internal void Add(OdbcError error)
	{
		_items.Add(error);
	}

	public void CopyTo(Array array, int i)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (i < array.GetLowerBound(0) || i > array.GetUpperBound(0))
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (array.IsFixedSize || i + Count > array.GetUpperBound(0))
		{
			throw new ArgumentException("array");
		}
		((OdbcError[])_items.ToArray()).CopyTo(array, i);
	}

	public IEnumerator GetEnumerator()
	{
		return _items.GetEnumerator();
	}

	public void CopyTo(OdbcError[] array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < array.GetLowerBound(0) || index > array.GetUpperBound(0))
		{
			throw new ArgumentOutOfRangeException("index");
		}
		((OdbcError[])_items.ToArray()).CopyTo(array, index);
	}
}
