using System;
using System.Collections.Specialized;

namespace Mono.WebBrowser;

public class AlertEventArgs : EventArgs
{
	private DialogType type;

	private string title;

	private string text;

	private string text2;

	private string username;

	private string password;

	private string checkMsg;

	private bool checkState;

	private DialogButtonFlags dialogButtons;

	private StringCollection buttons;

	private StringCollection options;

	private object returnValue;

	public DialogType Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			title = value;
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public string Text2
	{
		get
		{
			return text2;
		}
		set
		{
			text2 = value;
		}
	}

	public string CheckMessage
	{
		get
		{
			return checkMsg;
		}
		set
		{
			checkMsg = value;
		}
	}

	public bool CheckState
	{
		get
		{
			return checkState;
		}
		set
		{
			checkState = value;
		}
	}

	public DialogButtonFlags DialogButtons
	{
		get
		{
			return dialogButtons;
		}
		set
		{
			dialogButtons = value;
		}
	}

	public StringCollection Buttons
	{
		get
		{
			return buttons;
		}
		set
		{
			buttons = value;
		}
	}

	public StringCollection Options
	{
		get
		{
			return options;
		}
		set
		{
			options = value;
		}
	}

	public string Username
	{
		get
		{
			return username;
		}
		set
		{
			username = value;
		}
	}

	public string Password
	{
		get
		{
			return password;
		}
		set
		{
			password = value;
		}
	}

	public bool BoolReturn
	{
		get
		{
			if (returnValue is bool)
			{
				return (bool)returnValue;
			}
			return false;
		}
		set
		{
			returnValue = value;
		}
	}

	public int IntReturn
	{
		get
		{
			if (returnValue is int)
			{
				return (int)returnValue;
			}
			return -1;
		}
		set
		{
			returnValue = value;
		}
	}

	public string StringReturn
	{
		get
		{
			if (returnValue is string)
			{
				return (string)returnValue;
			}
			return string.Empty;
		}
		set
		{
			returnValue = value;
		}
	}
}
