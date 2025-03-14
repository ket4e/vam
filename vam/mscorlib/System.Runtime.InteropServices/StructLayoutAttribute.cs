namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class StructLayoutAttribute : Attribute
{
	public CharSet CharSet = CharSet.Auto;

	public int Pack = 8;

	public int Size;

	private LayoutKind lkind;

	public LayoutKind Value => lkind;

	public StructLayoutAttribute(short layoutKind)
	{
		lkind = (LayoutKind)layoutKind;
	}

	public StructLayoutAttribute(LayoutKind layoutKind)
	{
		lkind = layoutKind;
	}
}
