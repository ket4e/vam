namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
[ComVisible(true)]
public sealed class LCIDConversionAttribute : Attribute
{
	private int id;

	public int Value => id;

	public LCIDConversionAttribute(int lcid)
	{
		id = lcid;
	}
}
