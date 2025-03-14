namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class ComplexBindingPropertiesAttribute : Attribute
{
	private string data_source;

	private string data_member;

	public static readonly ComplexBindingPropertiesAttribute Default = new ComplexBindingPropertiesAttribute();

	public string DataMember => data_member;

	public string DataSource => data_source;

	public ComplexBindingPropertiesAttribute(string dataSource, string dataMember)
	{
		data_source = dataSource;
		data_member = dataMember;
	}

	public ComplexBindingPropertiesAttribute(string dataSource)
	{
		data_source = dataSource;
	}

	public ComplexBindingPropertiesAttribute()
	{
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ComplexBindingPropertiesAttribute complexBindingPropertiesAttribute))
		{
			return false;
		}
		return complexBindingPropertiesAttribute.DataMember == data_member && complexBindingPropertiesAttribute.DataSource == data_source;
	}

	public override int GetHashCode()
	{
		int hashCode = (data_source + data_member).GetHashCode();
		if (hashCode == 0)
		{
			return base.GetHashCode();
		}
		return hashCode;
	}
}
