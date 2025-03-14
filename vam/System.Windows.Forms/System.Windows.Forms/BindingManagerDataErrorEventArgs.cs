namespace System.Windows.Forms;

public class BindingManagerDataErrorEventArgs : EventArgs
{
	private Exception exception;

	public Exception Exception => exception;

	public BindingManagerDataErrorEventArgs(Exception exception)
	{
		this.exception = exception;
	}
}
