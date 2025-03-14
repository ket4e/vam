namespace System.Windows.Forms;

public class DataGridViewDataErrorEventArgs : DataGridViewCellCancelEventArgs
{
	private Exception exception;

	private DataGridViewDataErrorContexts context;

	private bool throwException;

	public DataGridViewDataErrorContexts Context => context;

	public Exception Exception => exception;

	public bool ThrowException
	{
		get
		{
			return throwException;
		}
		set
		{
			throwException = value;
		}
	}

	public DataGridViewDataErrorEventArgs(Exception exception, int columnIndex, int rowIndex, DataGridViewDataErrorContexts context)
		: base(columnIndex, rowIndex)
	{
		this.exception = exception;
		this.context = context;
		throwException = false;
	}
}
