using System.Collections;

namespace System.Data.Common;

internal sealed class FieldNameLookup : IEnumerable, ICollection
{
	private ArrayList list;

	public int Count => list.Count;

	public bool IsFixedSize => false;

	public bool IsReadOnly => false;

	public bool IsSynchronized => list.IsSynchronized;

	public string this[int index]
	{
		get
		{
			return (string)list[index];
		}
		set
		{
			list[index] = value;
		}
	}

	public object SyncRoot => list.SyncRoot;

	public FieldNameLookup()
	{
		list = new ArrayList();
	}

	public FieldNameLookup(DataTable schemaTable)
		: this()
	{
		foreach (DataRow row in schemaTable.Rows)
		{
			list.Add((string)row["ColumnName"]);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	}

	public int Add(object value)
	{
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

	public int IndexOf(object value)
	{
		return list.IndexOf(value);
	}

	public void Insert(int index, object value)
	{
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
