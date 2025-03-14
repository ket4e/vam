namespace System.Net.FtpClient;

public class FtpException : Exception
{
	public FtpException(string message)
		: base(message)
	{
	}
}
