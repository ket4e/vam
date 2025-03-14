using System.ComponentModel;

namespace System.Data;

internal class DataRelationPropertyDescriptor : PropertyDescriptor
{
	private DataRelation _relation;

	public override Type ComponentType => typeof(DataRowView);

	public override bool IsReadOnly => false;

	public override Type PropertyType => typeof(IBindingList);

	public DataRelation Relation => _relation;

	internal DataRelationPropertyDescriptor(DataRelation relation)
		: base(relation.RelationName, null)
	{
		_relation = relation;
	}

	public override bool CanResetValue(object obj)
	{
		return false;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is DataRelationPropertyDescriptor dataRelationPropertyDescriptor))
		{
			return false;
		}
		return Relation == dataRelationPropertyDescriptor.Relation;
	}

	public override int GetHashCode()
	{
		return _relation.GetHashCode();
	}

	public override object GetValue(object obj)
	{
		DataRowView dataRowView = (DataRowView)obj;
		return dataRowView.CreateChildView(Relation);
	}

	public override void ResetValue(object obj)
	{
	}

	public override void SetValue(object obj, object val)
	{
	}

	public override bool ShouldSerializeValue(object obj)
	{
		return false;
	}
}
