namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
[ComVisible(true)]
public sealed class CoClassAttribute : Attribute
{
	private Type klass;

	public Type CoClass => klass;

	public CoClassAttribute(Type coClass)
	{
		klass = coClass;
	}
}
