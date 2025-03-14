using System.ComponentModel;

namespace System.Data;

internal class DataTablePropertyDescriptor : PropertyDescriptor
{
	private DataTable table;

	public DataTable Table => table;

	public override bool IsReadOnly => false;

	public override Type ComponentType => typeof(DataRowView);

	public override Type PropertyType => typeof(IBindingList);

	internal DataTablePropertyDescriptor(DataTable table)
		: base(table.TableName, null)
	{
		this.table = table;
	}

	public override object GetValue(object component)
	{
		if (!(component is DataViewManagerListItemTypeDescriptor dataViewManagerListItemTypeDescriptor))
		{
			return null;
		}
		return new DataView(table, dataViewManagerListItemTypeDescriptor.DataViewManager);
	}

	public override bool CanResetValue(object component)
	{
		return false;
	}

	public override bool Equals(object other)
	{
		return other is DataTablePropertyDescriptor && ((DataTablePropertyDescriptor)other).table == table;
	}

	public override int GetHashCode()
	{
		return table.GetHashCode();
	}

	public override bool ShouldSerializeValue(object component)
	{
		return false;
	}

	public override void ResetValue(object component)
	{
	}

	public override void SetValue(object component, object value)
	{
	}
}
