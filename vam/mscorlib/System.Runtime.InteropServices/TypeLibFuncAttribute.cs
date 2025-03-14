namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
[ComVisible(true)]
public sealed class TypeLibFuncAttribute : Attribute
{
	private TypeLibFuncFlags flags;

	public TypeLibFuncFlags Value => flags;

	public TypeLibFuncAttribute(short flags)
	{
		this.flags = (TypeLibFuncFlags)flags;
	}

	public TypeLibFuncAttribute(TypeLibFuncFlags flags)
	{
		this.flags = flags;
	}
}
