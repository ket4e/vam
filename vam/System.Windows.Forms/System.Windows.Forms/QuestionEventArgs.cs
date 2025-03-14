namespace System.Windows.Forms;

public class QuestionEventArgs : EventArgs
{
	private bool response;

	public bool Response
	{
		get
		{
			return response;
		}
		set
		{
			response = value;
		}
	}

	public QuestionEventArgs()
	{
		response = false;
	}

	public QuestionEventArgs(bool response)
	{
		this.response = response;
	}
}
