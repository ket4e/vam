namespace System.Net.FtpClient;

public class FtpCommandException : FtpException
{
	private string _code;

	public string CompletionCode
	{
		get
		{
			return _code;
		}
		private set
		{
			_code = value;
		}
	}

	public FtpResponseType ResponseType
	{
		get
		{
			if (_code != null)
			{
				switch (_code[0])
				{
				case '4':
					return FtpResponseType.TransientNegativeCompletion;
				case '5':
					return FtpResponseType.PermanentNegativeCompletion;
				}
			}
			return FtpResponseType.None;
		}
	}

	public FtpCommandException(string code, string message)
		: base(message)
	{
		CompletionCode = code;
	}

	public FtpCommandException(FtpReply reply)
		: this(reply.Code, reply.ErrorMessage)
	{
	}
}
