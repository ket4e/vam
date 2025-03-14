namespace System.ComponentModel;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class ProvidePropertyAttribute : Attribute
{
	private string Property;

	private string Receiver;

	public string PropertyName => Property;

	public string ReceiverTypeName => Receiver;

	public override object TypeId => string.Concat(base.TypeId, Property);

	public ProvidePropertyAttribute(string propertyName, string receiverTypeName)
	{
		Property = propertyName;
		Receiver = receiverTypeName;
	}

	public ProvidePropertyAttribute(string propertyName, Type receiverType)
	{
		Property = propertyName;
		Receiver = receiverType.AssemblyQualifiedName;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is ProvidePropertyAttribute))
		{
			return false;
		}
		if (obj == this)
		{
			return true;
		}
		return ((ProvidePropertyAttribute)obj).PropertyName == Property && ((ProvidePropertyAttribute)obj).ReceiverTypeName == Receiver;
	}

	public override int GetHashCode()
	{
		return (Property + Receiver).GetHashCode();
	}
}
