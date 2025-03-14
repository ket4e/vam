using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

[ListBindable(false)]
public class DataGridViewSelectedRowCollection : BaseCollection, ICollection, IEnumerable, IList
{
	private DataGridView dataGridView;

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

	public DataGridViewRow this[int index] => (DataGridViewRow)base.List[index];

	protected override ArrayList List => base.List;

	internal DataGridViewSelectedRowCollection(DataGridView dataGridView)
	{
		this.dataGridView = dataGridView;
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
		return Contains(value as DataGridViewRow);
	}

	int IList.IndexOf(object value)
	{
		return base.List.IndexOf(value);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, value as DataGridViewRow);
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

	public bool Contains(DataGridViewRow dataGridViewRow)
	{
		return base.List.Contains(dataGridViewRow);
	}

	public void CopyTo(DataGridViewRow[] array, int index)
	{
		base.List.CopyTo(array, index);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Insert(int index, DataGridViewRow dataGridViewRow)
	{
		throw new NotSupportedException("Insert is not allowed.");
	}

	internal void InternalAdd(DataGridViewRow dataGridViewRow)
	{
		base.List.Add(dataGridViewRow);
	}

	internal void InternalAddRange(DataGridViewSelectedRowCollection rows)
	{
		if (rows == null)
		{
			return;
		}
		DataGridViewRow dataGridViewRow = ((dataGridView == null) ? null : dataGridView.EditingRow);
		for (int num = rows.Count - 1; num >= 0; num--)
		{
			if (rows[num] != dataGridViewRow)
			{
				base.List.Add(rows[num]);
			}
		}
	}

	internal void InternalClear()
	{
		List.Clear();
	}

	internal void InternalRemove(DataGridViewRow dataGridViewRow)
	{
		base.List.Remove(dataGridViewRow);
	}

	virtual bool IList.get_IsReadOnly()
	{
		return base.IsReadOnly;
	}
}
