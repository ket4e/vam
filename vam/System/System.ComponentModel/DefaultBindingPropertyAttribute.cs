namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DefaultBindingPropertyAttribute : Attribute
{
	public static readonly DefaultBindingPropertyAttribute Default;

	private string name;

	public string Name => name;

	public DefaultBindingPropertyAttribute()
	{
	}

	public DefaultBindingPropertyAttribute(string name)
	{
		this.name = name;
	}

	static DefaultBindingPropertyAttribute()
	{
		Default = new DefaultBindingPropertyAttribute();
	}

	public override bool Equals(object obj)
	{
		DefaultBindingPropertyAttribute defaultBindingPropertyAttribute = obj as DefaultBindingPropertyAttribute;
		if (obj == null)
		{
			return false;
		}
		return name == defaultBindingPropertyAttribute.Name;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
