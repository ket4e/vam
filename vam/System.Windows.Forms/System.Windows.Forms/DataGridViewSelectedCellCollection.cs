using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

[ListBindable(false)]
public class DataGridViewSelectedCellCollection : BaseCollection, ICollection, IEnumerable, IList
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
			throw new NotSupportedException();
		}
	}

	public DataGridViewCell this[int index] => (DataGridViewCell)base.List[index];

	protected override ArrayList List => base.List;

	internal DataGridViewSelectedCellCollection()
	{
	}

	int IList.Add(object value)
	{
		throw new NotSupportedException();
	}

	void IList.Clear()
	{
		Clear();
	}

	bool IList.Contains(object value)
	{
		return Contains(value as DataGridViewCell);
	}

	int IList.IndexOf(object value)
	{
		return base.List.IndexOf(value as DataGridViewCell);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, value as DataGridViewCell);
	}

	void IList.Remove(object value)
	{
		throw new NotSupportedException("Can't remove elements of selected cell base.List.");
	}

	void IList.RemoveAt(int index)
	{
		throw new NotSupportedException("Can't remove elements of selected cell base.List.");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Clear()
	{
		throw new NotSupportedException("Cannot clear this base.List");
	}

	public bool Contains(DataGridViewCell dataGridViewCell)
	{
		return base.List.Contains(dataGridViewCell);
	}

	public void CopyTo(DataGridViewCell[] array, int index)
	{
		base.List.CopyTo(array, index);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Insert(int index, DataGridViewCell dataGridViewCell)
	{
		throw new NotSupportedException("Can't insert to selected cell base.List");
	}

	internal void InternalAdd(DataGridViewCell dataGridViewCell)
	{
		base.List.Add(dataGridViewCell);
	}

	internal void InternalRemove(DataGridViewCell dataGridViewCell)
	{
		base.List.Remove(dataGridViewCell);
	}

	virtual bool IList.get_IsReadOnly()
	{
		return base.IsReadOnly;
	}
}
