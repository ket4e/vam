namespace System.Security.AccessControl;

public sealed class FileSecurity : FileSystemSecurity
{
	public FileSecurity()
		: base(isContainer: false)
	{
		throw new PlatformNotSupportedException();
	}

	public FileSecurity(string fileName, AccessControlSections includeSections)
		: base(isContainer: false, fileName, includeSections)
	{
		throw new PlatformNotSupportedException();
	}
}
