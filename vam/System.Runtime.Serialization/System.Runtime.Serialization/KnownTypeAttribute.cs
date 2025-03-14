namespace System.Runtime.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
public sealed class KnownTypeAttribute : Attribute
{
	private string method_name;

	private Type type;

	public string MethodName => method_name;

	public Type Type => type;

	public KnownTypeAttribute(string methodName)
	{
		method_name = methodName;
	}

	public KnownTypeAttribute(Type type)
	{
		this.type = type;
	}
}
