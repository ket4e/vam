namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[ComVisible(true)]
public sealed class ProgIdAttribute : Attribute
{
	private string pid;

	public string Value => pid;

	public ProgIdAttribute(string progId)
	{
		pid = progId;
	}
}
