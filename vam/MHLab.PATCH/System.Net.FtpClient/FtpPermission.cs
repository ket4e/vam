namespace System.Net.FtpClient;

[Flags]
public enum FtpPermission : uint
{
	None = 0u,
	Execute = 1u,
	Write = 2u,
	Read = 4u
}
