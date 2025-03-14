namespace System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class TypeLibVersionAttribute : Attribute
{
	private int major;

	private int minor;

	public int MajorVersion => major;

	public int MinorVersion => minor;

	public TypeLibVersionAttribute(int major, int minor)
	{
		this.major = major;
		this.minor = minor;
	}
}
