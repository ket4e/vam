namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
[ComVisible(true)]
public sealed class FieldOffsetAttribute : Attribute
{
	private int val;

	public int Value => val;

	public FieldOffsetAttribute(int offset)
	{
		val = offset;
	}
}
