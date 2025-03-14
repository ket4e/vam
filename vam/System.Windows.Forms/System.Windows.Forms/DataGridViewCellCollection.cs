using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

[ListBindable(false)]
public class DataGridViewCellCollection : BaseCollection, ICollection, IEnumerable, IList
{
	private DataGridViewRow dataGridViewRow;

	bool IList.IsFixedSize => base.List.IsFixedSize;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			this[index] = value as DataGridViewCell;
		}
	}

	public DataGridViewCell this[int index]
	{
		get
		{
			return (DataGridViewCell)base.List[index];
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Insert(index, value);
		}
	}

	public DataGridViewCell this[string columnName]
	{
		get
		{
			if (columnName == null)
			{
				throw new ArgumentNullException("columnName");
			}
			foreach (DataGridViewCell item in base.List)
			{
				if (string.Compare(item.OwningColumn.Name, columnName, ignoreCase: true) == 0)
				{
					return item;
				}
			}
			throw new ArgumentException($"Column name {columnName} cannot be found.", "columnName");
		}
		set
		{
			if (columnName == null)
			{
				throw new ArgumentNullException("columnName");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			for (int i = 0; i < base.List.Count; i++)
			{
				DataGridViewCell dataGridViewCell = (DataGridViewCell)base.List[i];
				if (string.Compare(dataGridViewCell.OwningColumn.Name, columnName, ignoreCase: true) == 0)
				{
					Insert(i, value);
					return;
				}
			}
			Add(value);
		}
	}

	protected override ArrayList List => base.List;

	public event CollectionChangeEventHandler CollectionChanged;

	public DataGridViewCellCollection(DataGridViewRow dataGridViewRow)
	{
		this.dataGridViewRow = dataGridViewRow;
	}

	int IList.Add(object value)
	{
		return Add(value as DataGridViewCell);
	}

	bool IList.Contains(object value)
	{
		return Contains(value as DataGridViewCell);
	}

	int IList.IndexOf(object value)
	{
		return IndexOf(value as DataGridViewCell);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, value as DataGridViewCell);
	}

	void IList.Remove(object value)
	{
		Remove(value as DataGridViewCell);
	}

	internal DataGridViewCell GetCellInternal(int colIndex)
	{
		return (DataGridViewCell)base.List[colIndex];
	}

	internal DataGridViewCell GetBoundCell(string dataPropertyName)
	{
		foreach (DataGridViewCell item in base.List)
		{
			if (string.Compare(item.OwningColumn.DataPropertyName, dataPropertyName, ignoreCase: true) == 0)
			{
				return item;
			}
		}
		return null;
	}

	public virtual int Add(DataGridViewCell dataGridViewCell)
	{
		int num = base.List.Add(dataGridViewCell);
		dataGridViewCell.SetOwningRow(dataGridViewRow);
		dataGridViewCell.SetColumnIndex(num);
		dataGridViewCell.SetDataGridView(dataGridViewRow.DataGridView);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
		return num;
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual void AddRange(params DataGridViewCell[] dataGridViewCells)
	{
		foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
		{
			Add(dataGridViewCell);
		}
	}

	public virtual void Clear()
	{
		base.List.Clear();
	}

	public virtual bool Contains(DataGridViewCell dataGridViewCell)
	{
		return base.List.Contains(dataGridViewCell);
	}

	public void CopyTo(DataGridViewCell[] array, int index)
	{
		base.List.CopyTo(array, index);
	}

	public int IndexOf(DataGridViewCell dataGridViewCell)
	{
		return base.List.IndexOf(dataGridViewCell);
	}

	public virtual void Insert(int index, DataGridViewCell dataGridViewCell)
	{
		base.List.Insert(index, dataGridViewCell);
		dataGridViewCell.SetOwningRow(dataGridViewRow);
		dataGridViewCell.SetColumnIndex(index);
		dataGridViewCell.SetDataGridView(dataGridViewRow.DataGridView);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewCell));
	}

	public virtual void Remove(DataGridViewCell cell)
	{
		base.List.Remove(cell);
		ReIndex();
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, cell));
	}

	public virtual void RemoveAt(int index)
	{
		DataGridViewCell element = this[index];
		base.List.RemoveAt(index);
		ReIndex();
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, element));
	}

	private void ReIndex()
	{
		for (int i = 0; i < base.List.Count; i++)
		{
			this[i].SetColumnIndex(i);
		}
	}

	protected void OnCollectionChanged(CollectionChangeEventArgs e)
	{
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, e);
		}
	}

	virtual bool IList.get_IsReadOnly()
	{
		return base.IsReadOnly;
	}
}
