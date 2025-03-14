namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
public sealed class TypeForwardedToAttribute : Attribute
{
	private Type destination;

	public Type Destination => destination;

	public TypeForwardedToAttribute(Type destination)
	{
		this.destination = destination;
	}
}
