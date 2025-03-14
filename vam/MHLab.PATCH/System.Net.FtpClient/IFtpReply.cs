namespace System.Net.FtpClient;

public interface IFtpReply
{
	FtpResponseType Type { get; }

	string Code { get; set; }

	string Message { get; set; }

	string InfoMessages { get; set; }

	bool Success { get; }

	string ErrorMessage { get; }
}
