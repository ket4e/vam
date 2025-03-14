using System.Text.RegularExpressions;

namespace System.Net.FtpClient;

public struct FtpReply : IFtpReply
{
	private string m_respCode;

	private string m_respMessage;

	private string m_infoMessages;

	public FtpResponseType Type
	{
		get
		{
			if (Code != null && Code.Length > 0 && int.TryParse(Code[0].ToString(), out var result))
			{
				return (FtpResponseType)result;
			}
			return FtpResponseType.None;
		}
	}

	public string Code
	{
		get
		{
			return m_respCode;
		}
		set
		{
			m_respCode = value;
		}
	}

	public string Message
	{
		get
		{
			return m_respMessage;
		}
		set
		{
			m_respMessage = value;
		}
	}

	public string InfoMessages
	{
		get
		{
			return m_infoMessages;
		}
		set
		{
			m_infoMessages = value;
		}
	}

	public bool Success
	{
		get
		{
			if (Code != null && Code.Length > 0 && int.TryParse(Code[0].ToString(), out var result) && result >= 1 && result <= 3)
			{
				return true;
			}
			return false;
		}
	}

	public string ErrorMessage
	{
		get
		{
			string text = "";
			if (Success)
			{
				return text;
			}
			if (InfoMessages != null && InfoMessages.Length > 0)
			{
				string[] array = InfoMessages.Split('\n');
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = Regex.Replace(array[i], "^[0-9]{3}-", "");
					text += $"{text2.Trim()}; ";
				}
			}
			return text + Message;
		}
	}
}
