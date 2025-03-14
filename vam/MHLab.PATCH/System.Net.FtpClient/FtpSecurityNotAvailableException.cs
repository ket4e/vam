namespace System.Net.FtpClient;

public class FtpSecurityNotAvailableException : FtpException
{
	public FtpSecurityNotAvailableException()
		: base("Security is not available on the server.")
	{
	}

	public FtpSecurityNotAvailableException(string message)
		: base(message)
	{
	}
}
