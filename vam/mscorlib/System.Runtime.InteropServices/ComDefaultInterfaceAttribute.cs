namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ComDefaultInterfaceAttribute : Attribute
{
	private Type _type;

	public Type Value => _type;

	public ComDefaultInterfaceAttribute(Type defaultInterface)
	{
		_type = defaultInterface;
	}
}
