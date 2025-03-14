using System.Collections;
using System.ComponentModel;

namespace System.Data.OleDb;

[Serializable]
[ListBindable(false)]
public sealed class OleDbErrorCollection : IEnumerable, ICollection
{
	private ArrayList items;

	object ICollection.SyncRoot => items.SyncRoot;

	bool ICollection.IsSynchronized => items.IsSynchronized;

	public int Count => items.Count;

	public OleDbError this[int index] => (OleDbError)items[index];

	internal OleDbErrorCollection()
	{
	}

	internal void Add(OleDbError error)
	{
		items.Add(error);
	}

	public void CopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < array.GetLowerBound(0) || index > array.GetUpperBound(0))
		{
			throw new ArgumentOutOfRangeException("index");
		}
		if (array.IsFixedSize || index + Count > array.GetUpperBound(0))
		{
			throw new ArgumentException("array");
		}
		((OleDbError[])items.ToArray()).CopyTo(array, index);
	}

	public void CopyTo(OleDbError[] array, int index)
	{
		items.CopyTo(array, index);
	}

	public IEnumerator GetEnumerator()
	{
		return items.GetEnumerator();
	}
}
