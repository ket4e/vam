using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms;

[DesignerSerializer("System.Windows.Forms.Design.DataGridViewRowCollectionCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[ListBindable(false)]
public class DataGridViewRowCollection : ICollection, IEnumerable, IList
{
	private class RowIndexComparator : IComparer
	{
		public int Compare(object o1, object o2)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)o1;
			DataGridViewRow dataGridViewRow2 = (DataGridViewRow)o2;
			if (dataGridViewRow.Index < dataGridViewRow2.Index)
			{
				return -1;
			}
			if (dataGridViewRow.Index > dataGridViewRow2.Index)
			{
				return 1;
			}
			return 0;
		}
	}

	private ArrayList list;

	private DataGridView dataGridView;

	private bool raiseEvent = true;

	int ICollection.Count => Count;

	bool IList.IsFixedSize => list.IsFixedSize;

	bool IList.IsReadOnly => list.IsReadOnly;

	bool ICollection.IsSynchronized => list.IsSynchronized;

	object IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			list[index] = value as DataGridViewRow;
		}
	}

	object ICollection.SyncRoot => list.SyncRoot;

	public int Count => list.Count;

	public DataGridViewRow this[int index]
	{
		get
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[index];
			if (dataGridViewRow.Index == -1)
			{
				dataGridViewRow = (DataGridViewRow)dataGridViewRow.Clone();
				dataGridViewRow.SetIndex(index);
				list[index] = dataGridViewRow;
			}
			return dataGridViewRow;
		}
	}

	protected DataGridView DataGridView => dataGridView;

	protected ArrayList List => list;

	internal ArrayList RowIndexSortedArrayList
	{
		get
		{
			ArrayList arrayList = (ArrayList)list.Clone();
			arrayList.Sort(new RowIndexComparator());
			return arrayList;
		}
	}

	public event CollectionChangeEventHandler CollectionChanged;

	public DataGridViewRowCollection(DataGridView dataGridView)
	{
		this.dataGridView = dataGridView;
		list = new ArrayList();
	}

	int IList.Add(object value)
	{
		return Add(value as DataGridViewRow);
	}

	bool IList.Contains(object value)
	{
		return Contains(value as DataGridViewRow);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		list.CopyTo(array, index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	}

	int IList.IndexOf(object value)
	{
		return IndexOf(value as DataGridViewRow);
	}

	void IList.Insert(int index, object value)
	{
		Insert(index, value as DataGridViewRow);
	}

	void IList.Remove(object value)
	{
		Remove(value as DataGridViewRow);
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual int Add()
	{
		return Add(dataGridView.RowTemplateFull);
	}

	private int AddCore(DataGridViewRow dataGridViewRow, bool sharable)
	{
		if (dataGridView.Columns.Count == 0)
		{
			throw new InvalidOperationException("DataGridView has no columns.");
		}
		dataGridViewRow.SetDataGridView(dataGridView);
		int num = -1;
		if (DataGridView != null && DataGridView.EditingRow != null && DataGridView.EditingRow != dataGridViewRow)
		{
			num = list.Count - 1;
			DataGridView.EditingRow.SetIndex(list.Count);
		}
		int num2;
		if (num >= 0)
		{
			list.Insert(num, dataGridViewRow);
			num2 = num;
		}
		else
		{
			num2 = list.Add(dataGridViewRow);
		}
		if (sharable && CanBeShared(dataGridViewRow))
		{
			dataGridViewRow.SetIndex(-1);
		}
		else
		{
			dataGridViewRow.SetIndex(num2);
		}
		CompleteRowCells(dataGridViewRow);
		for (int i = 0; i < dataGridViewRow.Cells.Count; i++)
		{
			dataGridViewRow.Cells[i].SetOwningColumn(dataGridView.Columns[i]);
		}
		if (raiseEvent)
		{
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRow));
			DataGridView.OnRowsAddedInternal(new DataGridViewRowsAddedEventArgs(num2, 1));
		}
		return num2;
	}

	private void CompleteRowCells(DataGridViewRow row)
	{
		if (row != null && DataGridView != null && row.Cells.Count < DataGridView.ColumnCount)
		{
			for (int i = row.Cells.Count; i < DataGridView.ColumnCount; i++)
			{
				row.Cells.Add((DataGridViewCell)DataGridView.Columns[i].CellTemplate.Clone());
			}
		}
	}

	public virtual int Add(DataGridViewRow dataGridViewRow)
	{
		if (dataGridView.DataSource != null)
		{
			throw new InvalidOperationException("DataSource of DataGridView is not null.");
		}
		return AddCore(dataGridViewRow, sharable: true);
	}

	private bool CanBeShared(DataGridViewRow row)
	{
		return false;
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual int Add(int count)
	{
		if (count <= 0)
		{
			throw new ArgumentOutOfRangeException("Count is less than or equeal to 0.");
		}
		if (dataGridView.DataSource != null)
		{
			throw new InvalidOperationException("DataSource of DataGridView is not null.");
		}
		if (dataGridView.Columns.Count == 0)
		{
			throw new InvalidOperationException("DataGridView has no columns.");
		}
		raiseEvent = false;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			num = Add(dataGridView.RowTemplateFull);
		}
		DataGridView.OnRowsAddedInternal(new DataGridViewRowsAddedEventArgs(num - count + 1, count));
		raiseEvent = true;
		return num;
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual int Add(params object[] values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values is null.");
		}
		if (dataGridView.VirtualMode)
		{
			throw new InvalidOperationException("DataGridView is in virtual mode.");
		}
		DataGridViewRow rowTemplateFull = dataGridView.RowTemplateFull;
		int result = AddCore(rowTemplateFull, sharable: false);
		rowTemplateFull.SetValues(values);
		return result;
	}

	public virtual int AddCopies(int indexSource, int count)
	{
		raiseEvent = false;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			num = AddCopy(indexSource);
		}
		DataGridView.OnRowsAddedInternal(new DataGridViewRowsAddedEventArgs(num - count + 1, count));
		raiseEvent = true;
		return num;
	}

	public virtual int AddCopy(int indexSource)
	{
		return Add((list[indexSource] as DataGridViewRow).Clone() as DataGridViewRow);
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual void AddRange(params DataGridViewRow[] dataGridViewRows)
	{
		if (dataGridView.DataSource != null)
		{
			throw new InvalidOperationException("DataSource of DataGridView is not null.");
		}
		int num = 0;
		int num2 = -1;
		raiseEvent = false;
		foreach (DataGridViewRow dataGridViewRow in dataGridViewRows)
		{
			num2 = Add(dataGridViewRow);
			num++;
		}
		raiseEvent = true;
		DataGridView.OnRowsAddedInternal(new DataGridViewRowsAddedEventArgs(num2 - num + 1, num));
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRows));
	}

	public virtual void Clear()
	{
		int count = list.Count;
		DataGridView.OnRowsPreRemovedInternal(new DataGridViewRowsRemovedEventArgs(0, count));
		for (int i = 0; i < count; i++)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[0];
			if (dataGridViewRow.IsNewRow)
			{
				break;
			}
			list.Remove(dataGridViewRow);
			ReIndex();
		}
		DataGridView.OnRowsPostRemovedInternal(new DataGridViewRowsRemovedEventArgs(0, count));
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
	}

	internal void ClearInternal()
	{
		list.Clear();
	}

	public virtual bool Contains(DataGridViewRow dataGridViewRow)
	{
		return list.Contains(dataGridViewRow);
	}

	public void CopyTo(DataGridViewRow[] array, int index)
	{
		list.CopyTo(array, index);
	}

	public int GetFirstRow(DataGridViewElementStates includeFilter)
	{
		for (int i = 0; i < list.Count; i++)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[i];
			if ((dataGridViewRow.State & includeFilter) != 0)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetFirstRow(DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
	{
		for (int i = 0; i < list.Count; i++)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[i];
			if ((dataGridViewRow.State & includeFilter) != 0 && (dataGridViewRow.State & excludeFilter) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetLastRow(DataGridViewElementStates includeFilter)
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[num];
			if ((dataGridViewRow.State & includeFilter) != 0)
			{
				return num;
			}
		}
		return -1;
	}

	public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter)
	{
		for (int i = indexStart + 1; i < list.Count; i++)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[i];
			if ((dataGridViewRow.State & includeFilter) != 0)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetNextRow(int indexStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
	{
		for (int i = indexStart + 1; i < list.Count; i++)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[i];
			if ((dataGridViewRow.State & includeFilter) != 0 && (dataGridViewRow.State & excludeFilter) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter)
	{
		for (int num = indexStart - 1; num >= 0; num--)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[num];
			if ((dataGridViewRow.State & includeFilter) != 0)
			{
				return num;
			}
		}
		return -1;
	}

	public int GetPreviousRow(int indexStart, DataGridViewElementStates includeFilter, DataGridViewElementStates excludeFilter)
	{
		for (int num = indexStart - 1; num >= 0; num--)
		{
			DataGridViewRow dataGridViewRow = (DataGridViewRow)list[num];
			if ((dataGridViewRow.State & includeFilter) != 0 && (dataGridViewRow.State & excludeFilter) == 0)
			{
				return num;
			}
		}
		return -1;
	}

	public int GetRowCount(DataGridViewElementStates includeFilter)
	{
		int num = 0;
		foreach (DataGridViewRow item in list)
		{
			if ((item.State & includeFilter) != 0)
			{
				num++;
			}
		}
		return num;
	}

	public int GetRowsHeight(DataGridViewElementStates includeFilter)
	{
		int num = 0;
		foreach (DataGridViewRow item in list)
		{
			if ((item.State & includeFilter) != 0)
			{
				num += item.Height;
			}
		}
		return num;
	}

	public virtual DataGridViewElementStates GetRowState(int rowIndex)
	{
		return (list[rowIndex] as DataGridViewRow).State;
	}

	public int IndexOf(DataGridViewRow dataGridViewRow)
	{
		return list.IndexOf(dataGridViewRow);
	}

	public virtual void Insert(int rowIndex, DataGridViewRow dataGridViewRow)
	{
		dataGridViewRow.SetIndex(rowIndex);
		dataGridViewRow.SetDataGridView(dataGridView);
		CompleteRowCells(dataGridViewRow);
		list.Insert(rowIndex, dataGridViewRow);
		ReIndex();
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataGridViewRow));
		if (raiseEvent)
		{
			DataGridView.OnRowsAddedInternal(new DataGridViewRowsAddedEventArgs(rowIndex, 1));
		}
	}

	public virtual void Insert(int rowIndex, int count)
	{
		int num = rowIndex;
		raiseEvent = false;
		for (int i = 0; i < count; i++)
		{
			Insert(num++, dataGridView.RowTemplateFull);
		}
		DataGridView.OnRowsAddedInternal(new DataGridViewRowsAddedEventArgs(rowIndex, count));
		raiseEvent = true;
	}

	public virtual void Insert(int rowIndex, params object[] values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("Values is null.");
		}
		if (dataGridView.VirtualMode || dataGridView.DataSource != null)
		{
			throw new InvalidOperationException();
		}
		DataGridViewRow dataGridViewRow = new DataGridViewRow();
		dataGridViewRow.SetValues(values);
		Insert(rowIndex, dataGridViewRow);
	}

	public virtual void InsertCopies(int indexSource, int indexDestination, int count)
	{
		raiseEvent = false;
		int num = indexDestination;
		for (int i = 0; i < count; i++)
		{
			InsertCopy(indexSource, num++);
		}
		DataGridView.OnRowsAddedInternal(new DataGridViewRowsAddedEventArgs(indexDestination, count));
		raiseEvent = true;
	}

	public virtual void InsertCopy(int indexSource, int indexDestination)
	{
		Insert(indexDestination, (list[indexSource] as DataGridViewRow).Clone());
	}

	public virtual void InsertRange(int rowIndex, params DataGridViewRow[] dataGridViewRows)
	{
		raiseEvent = false;
		int num = rowIndex;
		int num2 = 0;
		foreach (DataGridViewRow dataGridViewRow in dataGridViewRows)
		{
			Insert(num++, dataGridViewRow);
			num2++;
		}
		DataGridView.OnRowsAddedInternal(new DataGridViewRowsAddedEventArgs(rowIndex, num2));
		raiseEvent = true;
	}

	public virtual void Remove(DataGridViewRow dataGridViewRow)
	{
		if (dataGridViewRow.IsNewRow)
		{
			throw new InvalidOperationException("Cannot delete the new row");
		}
		DataGridView.OnRowsPreRemovedInternal(new DataGridViewRowsRemovedEventArgs(dataGridViewRow.Index, 1));
		list.Remove(dataGridViewRow);
		ReIndex();
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataGridViewRow));
		DataGridView.OnRowsPostRemovedInternal(new DataGridViewRowsRemovedEventArgs(dataGridViewRow.Index, 1));
	}

	internal virtual void RemoveInternal(DataGridViewRow dataGridViewRow)
	{
		DataGridView.OnRowsPreRemovedInternal(new DataGridViewRowsRemovedEventArgs(dataGridViewRow.Index, 1));
		list.Remove(dataGridViewRow);
		ReIndex();
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataGridViewRow));
		DataGridView.OnRowsPostRemovedInternal(new DataGridViewRowsRemovedEventArgs(dataGridViewRow.Index, 1));
	}

	public virtual void RemoveAt(int index)
	{
		DataGridViewRow dataGridViewRow = this[index];
		Remove(dataGridViewRow);
	}

	internal void RemoveAtInternal(int index)
	{
		DataGridViewRow dataGridViewRow = this[index];
		RemoveInternal(dataGridViewRow);
	}

	public DataGridViewRow SharedRow(int rowIndex)
	{
		return (DataGridViewRow)list[rowIndex];
	}

	internal int SharedRowIndexOf(DataGridViewRow row)
	{
		return list.IndexOf(row);
	}

	protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
	{
		if (this.CollectionChanged != null)
		{
			this.CollectionChanged(this, e);
		}
	}

	internal void AddInternal(DataGridViewRow dataGridViewRow, bool sharable)
	{
		raiseEvent = false;
		AddCore(dataGridViewRow, sharable);
		raiseEvent = true;
	}

	internal void ReIndex()
	{
		for (int i = 0; i < Count; i++)
		{
			(list[i] as DataGridViewRow).SetIndex(i);
		}
	}

	internal void Sort(IComparer comparer)
	{
		if (DataGridView != null && DataGridView.EditingRow != null)
		{
			list.Sort(0, Count - 1, comparer);
		}
		else
		{
			list.Sort(comparer);
		}
		for (int i = 0; i < list.Count; i++)
		{
			(list[i] as DataGridViewRow).SetIndex(i);
		}
	}
}
