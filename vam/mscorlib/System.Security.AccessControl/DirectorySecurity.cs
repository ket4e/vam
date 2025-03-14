namespace System.Security.AccessControl;

public sealed class DirectorySecurity : FileSystemSecurity
{
	public DirectorySecurity()
		: base(isContainer: true)
	{
		throw new PlatformNotSupportedException();
	}

	public DirectorySecurity(string name, AccessControlSections includeSections)
		: base(isContainer: true, name, includeSections)
	{
		throw new PlatformNotSupportedException();
	}
}
