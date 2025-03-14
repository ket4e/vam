namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AttributeProviderAttribute : Attribute
{
	private string type_name;

	private string property_name;

	public string PropertyName => property_name;

	public string TypeName => type_name;

	public AttributeProviderAttribute(Type type)
	{
		type_name = type.AssemblyQualifiedName;
	}

	public AttributeProviderAttribute(string typeName, string propertyName)
	{
		type_name = typeName;
		property_name = propertyName;
	}

	public AttributeProviderAttribute(string typeName)
	{
		type_name = typeName;
	}
}
