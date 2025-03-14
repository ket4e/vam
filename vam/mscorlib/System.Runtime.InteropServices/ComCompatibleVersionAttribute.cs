namespace System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
[ComVisible(true)]
public sealed class ComCompatibleVersionAttribute : Attribute
{
	private int major;

	private int minor;

	private int build;

	private int revision;

	public int MajorVersion => major;

	public int MinorVersion => minor;

	public int BuildNumber => build;

	public int RevisionNumber => revision;

	public ComCompatibleVersionAttribute(int major, int minor, int build, int revision)
	{
		this.major = major;
		this.minor = minor;
		this.build = build;
		this.revision = revision;
	}
}
