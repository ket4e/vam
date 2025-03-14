namespace System.ComponentModel;

public class RunWorkerCompletedEventArgs : AsyncCompletedEventArgs
{
	private object result;

	public object Result
	{
		get
		{
			RaiseExceptionIfNecessary();
			return result;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new object UserState => null;

	public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled)
		: base(error, cancelled, null)
	{
		this.result = result;
	}
}
