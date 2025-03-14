namespace System.Windows.Forms;

public class UICuesEventArgs : EventArgs
{
	private UICues cues;

	public UICues Changed => cues & UICues.Changed;

	public bool ChangeFocus
	{
		get
		{
			if ((cues & UICues.ChangeFocus) == 0)
			{
				return false;
			}
			return true;
		}
	}

	public bool ChangeKeyboard
	{
		get
		{
			if ((cues & UICues.ChangeKeyboard) == 0)
			{
				return false;
			}
			return true;
		}
	}

	public bool ShowFocus
	{
		get
		{
			if ((cues & UICues.ShowFocus) == 0)
			{
				return false;
			}
			return true;
		}
	}

	public bool ShowKeyboard
	{
		get
		{
			if ((cues & UICues.ShowKeyboard) == 0)
			{
				return false;
			}
			return true;
		}
	}

	public UICuesEventArgs(UICues uicues)
	{
		cues = uicues;
	}
}
