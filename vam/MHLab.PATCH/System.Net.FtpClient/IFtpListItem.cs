namespace System.Net.FtpClient;

public interface IFtpListItem
{
	FtpFileSystemObjectType Type { get; set; }

	string FullName { get; set; }

	string Name { get; set; }

	string LinkTarget { get; set; }

	FtpListItem LinkObject { get; set; }

	DateTime Modified { get; set; }

	DateTime Created { get; set; }

	long Size { get; set; }

	FtpSpecialPermissions SpecialPermissions { get; set; }

	FtpPermission OwnerPermissions { get; set; }

	FtpPermission GroupPermissions { get; set; }

	FtpPermission OthersPermissions { get; set; }

	string Input { get; }

	new string ToString();
}
