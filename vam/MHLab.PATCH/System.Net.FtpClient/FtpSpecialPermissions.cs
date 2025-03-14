namespace System.Net.FtpClient;

[Flags]
public enum FtpSpecialPermissions
{
	None = 0,
	Sticky = 1,
	SetGroupID = 2,
	SetUserID = 4
}
