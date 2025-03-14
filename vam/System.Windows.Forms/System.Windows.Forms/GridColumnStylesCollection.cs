using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms;

[ListBindable(false)]
[Editor("System.Windows.Forms.Design.DataGridColumnCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public class GridColumnStylesCollection : BaseCollection, ICollection, IEnumerable, IList
{
	private ArrayList items;

	private DataGridTableStyle owner;

	private bool fire_event;

	int ICollection.Count => items.Count;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	bool IList.IsFixedSize => false;

	bool IList.IsReadOnly => false;

	object IList.this[int index]
	{
		get
		{
			return items[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public DataGridColumnStyle this[string columnName]
	{
		get
		{
			int num = FromColumnNameToIndex(columnName);
			return (num != -1) ? this[num] : null;
		}
	}

	public DataGridColumnStyle this[int index] => (DataGridColumnStyle)items[index];

	public DataGridColumnStyle this[PropertyDescriptor propertyDesciptor]
	{
		get
		{
			for (int i = 0; i < items.Count; i++)
			{
				DataGridColumnStyle dataGridColumnStyle = (DataGridColumnStyle)items[i];
				if (dataGridColumnStyle.PropertyDescriptor.Equals(propertyDesciptor))
				{
					return dataGridColumnStyle;
				}
			}
			return null;
		}
	}

	protected override ArrayList List => items;

	internal bool FireEvents
	{
		get
		{
			return fire_event;
		}
		set
		{
			fire_event = value;
		}
	}

	public event CollectionChangeEventHandler CollectionChanged;

	internal GridColumnStylesCollection(DataGridTableStyle tablestyle)
	{
		items = new ArrayList();
		owner = tablestyle;
		fire_event = true;
	}

	void ICollection.CopyTo(Array array, int index)
	{
		items.CopyTo(array, index);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return items.GetEnumerator();
	}

	int IList.Add(object value)
	{
		return Add((DataGridColumnStyle)value);
	}

	void IList.Clear()
	{
		Clear();
	}

	bool IList.Contains(object value)
	{
		return Contains((DataGridColumnStyle)value);
	}

	int IList.IndexOf(object value)
	{
		return IndexOf((DataGridColumnStyle)value);
	}

	void IList.Insert(int index, object value)
	{
		throw new NotSupportedException();
	}

	void IList.Remove(object value)
	{
		Remove((DataGridColumnStyle)value);
	}

	void IList.RemoveAt(int index)
	{
		RemoveAt(index);
	}

	public virtual int Add(DataGridColumnStyle column)
	{
		if (FromColumnNameToIndex(column.MappingName) != -1)
		{
			throw new ArgumentException("The ColumnStyles collection already has a column with this mapping name");
		}
		column.TableStyle = owner;
		column.SetDataGridInternal(owner.DataGrid);
		ConnectColumnEvents(column);
		int result = items.Add(column);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
		return result;
	}

	public void AddRange(DataGridColumnStyle[] columns)
	{
		foreach (DataGridColumnStyle column in columns)
		{
			Add(column);
		}
	}

	public void Clear()
	{
		items.Clear();
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
	}

	public bool Contains(DataGridColumnStyle column)
	{
		return FromColumnNameToIndex(column.MappingName) != -1;
	}

	public bool Contains(PropertyDescriptor propertyDescriptor)
	{
		return this[propertyDescriptor] != null;
	}

	public bool Contains(string name)
	{
		return FromColumnNameToIndex(name) != -1;
	}

	public int IndexOf(DataGridColumnStyle element)
	{
		return items.IndexOf(element);
	}

	protected void OnCollectionChanged(CollectionChangeEventArgs e)
	{
		if (fire_event && this.CollectionChanged != null)
		{
			this.CollectionChanged(this, e);
		}
	}

	public void Remove(DataGridColumnStyle column)
	{
		items.Remove(column);
		DisconnectColumnEvents(column);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, column));
	}

	public void RemoveAt(int index)
	{
		DataGridColumnStyle dataGridColumnStyle = (DataGridColumnStyle)items[index];
		items.RemoveAt(index);
		DisconnectColumnEvents(dataGridColumnStyle);
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataGridColumnStyle));
	}

	public void ResetPropertyDescriptors()
	{
		for (int i = 0; i < items.Count; i++)
		{
			DataGridColumnStyle dataGridColumnStyle = (DataGridColumnStyle)items[i];
			if (dataGridColumnStyle.PropertyDescriptor != null)
			{
				dataGridColumnStyle.PropertyDescriptor = null;
			}
		}
	}

	private void ConnectColumnEvents(DataGridColumnStyle col)
	{
		col.AlignmentChanged += ColumnAlignmentChangedEvent;
		col.FontChanged += ColumnFontChangedEvent;
		col.HeaderTextChanged += ColumnHeaderTextChanged;
		col.MappingNameChanged += ColumnMappingNameChangedEvent;
		col.NullTextChanged += ColumnNullTextChangedEvent;
		col.PropertyDescriptorChanged += ColumnPropertyDescriptorChanged;
		col.ReadOnlyChanged += ColumnReadOnlyChangedEvent;
		col.WidthChanged += ColumnWidthChangedEvent;
	}

	private void DisconnectColumnEvents(DataGridColumnStyle col)
	{
		col.AlignmentChanged -= ColumnAlignmentChangedEvent;
		col.FontChanged -= ColumnFontChangedEvent;
		col.HeaderTextChanged -= ColumnHeaderTextChanged;
		col.MappingNameChanged -= ColumnMappingNameChangedEvent;
		col.NullTextChanged -= ColumnNullTextChangedEvent;
		col.PropertyDescriptorChanged -= ColumnPropertyDescriptorChanged;
		col.ReadOnlyChanged -= ColumnReadOnlyChangedEvent;
		col.WidthChanged -= ColumnWidthChangedEvent;
	}

	private void ColumnAlignmentChangedEvent(object sender, EventArgs e)
	{
	}

	private void ColumnFontChangedEvent(object sender, EventArgs e)
	{
	}

	private void ColumnHeaderTextChanged(object sender, EventArgs e)
	{
	}

	private void ColumnMappingNameChangedEvent(object sender, EventArgs e)
	{
	}

	private void ColumnNullTextChangedEvent(object sender, EventArgs e)
	{
	}

	private void ColumnPropertyDescriptorChanged(object sender, EventArgs e)
	{
		OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
	}

	private void ColumnReadOnlyChangedEvent(object sender, EventArgs e)
	{
	}

	private void ColumnWidthChangedEvent(object sender, EventArgs e)
	{
	}

	private int FromColumnNameToIndex(string columnName)
	{
		for (int i = 0; i < items.Count; i++)
		{
			DataGridColumnStyle dataGridColumnStyle = (DataGridColumnStyle)items[i];
			if (dataGridColumnStyle.MappingName != null && !(dataGridColumnStyle.MappingName == string.Empty) && string.Compare(dataGridColumnStyle.MappingName, columnName, ignoreCase: true) == 0)
			{
				return i;
			}
		}
		return -1;
	}
}
