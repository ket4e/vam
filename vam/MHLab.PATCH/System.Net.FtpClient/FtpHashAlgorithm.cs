namespace System.Net.FtpClient;

[Flags]
public enum FtpHashAlgorithm
{
	NONE = 0,
	SHA1 = 1,
	SHA256 = 2,
	SHA512 = 4,
	MD5 = 8,
	CRC = 0x10
}
