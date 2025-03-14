namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
public sealed class ComEventInterfaceAttribute : Attribute
{
	private Type si;

	private Type ep;

	public Type EventProvider => ep;

	public Type SourceInterface => si;

	public ComEventInterfaceAttribute(Type SourceInterface, Type EventProvider)
	{
		si = SourceInterface;
		ep = EventProvider;
	}
}
