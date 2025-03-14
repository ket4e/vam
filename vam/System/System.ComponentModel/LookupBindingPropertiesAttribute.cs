namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public sealed class LookupBindingPropertiesAttribute : Attribute
{
	private string data_source;

	private string display_member;

	private string value_member;

	private string lookup_member;

	public static readonly LookupBindingPropertiesAttribute Default;

	public string DataSource => data_source;

	public string DisplayMember => display_member;

	public string LookupMember => lookup_member;

	public string ValueMember => value_member;

	public LookupBindingPropertiesAttribute(string dataSource, string displayMember, string valueMember, string lookupMember)
	{
		data_source = dataSource;
		display_member = displayMember;
		value_member = valueMember;
		lookup_member = lookupMember;
	}

	public LookupBindingPropertiesAttribute()
	{
	}

	static LookupBindingPropertiesAttribute()
	{
		Default = new LookupBindingPropertiesAttribute();
	}

	public override int GetHashCode()
	{
		return ((data_source == null) ? 1 : data_source.GetHashCode()) << 24 + ((display_member == null) ? 1 : display_member.GetHashCode()) << 16 + ((lookup_member == null) ? 1 : lookup_member.GetHashCode()) << 8 + ((value_member == null) ? 1 : value_member.GetHashCode());
	}

	public override bool Equals(object obj)
	{
		if (!(obj is LookupBindingPropertiesAttribute lookupBindingPropertiesAttribute))
		{
			return false;
		}
		if (data_source != lookupBindingPropertiesAttribute.data_source || display_member != lookupBindingPropertiesAttribute.display_member)
		{
			return false;
		}
		if (value_member != lookupBindingPropertiesAttribute.value_member || lookup_member != lookupBindingPropertiesAttribute.lookup_member)
		{
			return false;
		}
		return true;
	}
}
