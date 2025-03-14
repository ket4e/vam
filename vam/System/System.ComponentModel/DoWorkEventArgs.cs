namespace System.ComponentModel;

public class DoWorkEventArgs : CancelEventArgs
{
	private object arg;

	private object result;

	public object Argument => arg;

	public object Result
	{
		get
		{
			return result;
		}
		set
		{
			result = value;
		}
	}

	public DoWorkEventArgs(object argument)
	{
		arg = argument;
	}
}
