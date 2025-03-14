namespace System.Net.FtpClient;

[Flags]
public enum FtpListOption
{
	Modify = 1,
	Size = 2,
	SizeModify = 3,
	AllFiles = 4,
	ForceList = 8,
	NameList = 0x10,
	ForceNameList = 0x18,
	DerefLinks = 0x20,
	UseLS = 0x48,
	Recursive = 0x80,
	NoPath = 0x100
}
