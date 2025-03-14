namespace System.Net.FtpClient;

[Flags]
public enum FtpCapability
{
	NONE = 0,
	MLSD = 1,
	SIZE = 2,
	MDTM = 4,
	REST = 8,
	UTF8 = 0x10,
	PRET = 0x20,
	MFMT = 0x40,
	MFCT = 0x80,
	MFF = 0x100,
	STAT = 0x200,
	HASH = 0x400,
	MD5 = 0x800,
	XMD5 = 0x1000,
	XCRC = 0x2000,
	XSHA1 = 0x4000,
	XSHA256 = 0x8000,
	XSHA512 = 0x10000
}
