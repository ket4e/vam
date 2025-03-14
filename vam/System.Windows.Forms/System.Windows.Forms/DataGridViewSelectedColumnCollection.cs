using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

[ListBindable(false)]
public class DataGridViewSelectedColumnCollection : BaseCollection, ICollection, IEnumerable, IList
{
	bool IList.IsFixedSize => base.List.IsFixedSize;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			throw new NotSupportedException("Can't insert or modify this collection.");
		}
	}

	public DataGridViewColumn this[int index] => (DataGridViewColumn)base.List[index];

	protected override ArrayList List => base.List;

	internal DataGridViewSelectedColumnCollection()
	{
	}

	int IList.Add(object value)
	{
		throw new NotSupportedException("Can't add elements to this collection.");
	}

	void IList.Clear()
	{
		Clear();
	}

	bool IList.Contains(object value)
	{
		return Contains(value as DataGridViewColumn);
	}

	int IList.IndexOf(object value)
	{
		return base.List.IndexOf(value);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, value as DataGridViewColumn);
	}

	void IList.Remove(object value)
	{
		throw new NotSupportedException("Can't remove elements of this collection.");
	}

	void IList.RemoveAt(int index)
	{
		throw new NotSupportedException("Can't remove elements of this collection.");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Clear()
	{
		throw new NotSupportedException("This collection cannot be cleared.");
	}

	public bool Contains(DataGridViewColumn dataGridViewColumn)
	{
		return base.List.Contains(dataGridViewColumn);
	}

	public void CopyTo(DataGridViewColumn[] array, int index)
	{
		base.List.CopyTo(array, index);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Insert(int index, DataGridViewColumn dataGridViewColumn)
	{
		throw new NotSupportedException("Insert is not allowed.");
	}

	internal void InternalAdd(DataGridViewColumn dataGridViewColumn)
	{
		base.List.Add(dataGridViewColumn);
	}

	internal void InternalAddRange(DataGridViewSelectedColumnCollection columns)
	{
		if (columns != null)
		{
			for (int num = columns.Count - 1; num >= 0; num--)
			{
				base.List.Add(columns[num]);
			}
		}
	}

	internal void InternalClear()
	{
		List.Clear();
	}

	internal void InternalRemove(DataGridViewColumn dataGridViewColumn)
	{
		base.List.Remove(dataGridViewColumn);
	}

	virtual bool IList.get_IsReadOnly()
	{
		return base.IsReadOnly;
	}
}
