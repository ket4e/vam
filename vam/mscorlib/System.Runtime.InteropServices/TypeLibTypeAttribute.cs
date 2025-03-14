namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, Inherited = false)]
[ComVisible(true)]
public sealed class TypeLibTypeAttribute : Attribute
{
	private TypeLibTypeFlags flags;

	public TypeLibTypeFlags Value => flags;

	public TypeLibTypeAttribute(short flags)
	{
		this.flags = (TypeLibTypeFlags)flags;
	}

	public TypeLibTypeAttribute(TypeLibTypeFlags flags)
	{
		this.flags = flags;
	}
}
