namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Module, Inherited = false)]
[ComVisible(true)]
public sealed class DefaultCharSetAttribute : Attribute
{
	private CharSet _set;

	public CharSet CharSet => _set;

	public DefaultCharSetAttribute(CharSet charSet)
	{
		_set = charSet;
	}
}
