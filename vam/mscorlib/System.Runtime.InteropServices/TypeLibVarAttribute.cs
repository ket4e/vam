namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class TypeLibVarAttribute : Attribute
{
	private TypeLibVarFlags flags;

	public TypeLibVarFlags Value => flags;

	public TypeLibVarAttribute(short flags)
	{
		this.flags = (TypeLibVarFlags)flags;
	}

	public TypeLibVarAttribute(TypeLibVarFlags flags)
	{
		this.flags = flags;
	}
}
