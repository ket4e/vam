using System;

namespace Mono.WebBrowser;

public class StatusChangedEventArgs : EventArgs
{
	private string message;

	private int status;

	public string Message
	{
		get
		{
			return message;
		}
		set
		{
			message = value;
		}
	}

	public int Status
	{
		get
		{
			return status;
		}
		set
		{
			status = value;
		}
	}

	public StatusChangedEventArgs(string message, int status)
	{
		this.message = message;
		this.status = status;
	}
}
