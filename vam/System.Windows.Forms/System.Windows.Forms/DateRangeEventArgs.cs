namespace System.Windows.Forms;

public class DateRangeEventArgs : EventArgs
{
	private DateTime end;

	private DateTime start;

	public DateTime End => end;

	public DateTime Start => start;

	public DateRangeEventArgs(DateTime start, DateTime end)
	{
		this.start = start;
		this.end = end;
	}
}
