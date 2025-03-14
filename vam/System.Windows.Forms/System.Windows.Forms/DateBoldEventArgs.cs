namespace System.Windows.Forms;

public class DateBoldEventArgs : EventArgs
{
	private int size;

	private DateTime start;

	private int[] days_to_bold;

	public int[] DaysToBold
	{
		get
		{
			return days_to_bold;
		}
		set
		{
			days_to_bold = value;
		}
	}

	public int Size => size;

	public DateTime StartDate => start;

	private DateBoldEventArgs(DateTime start, int size, int[] daysToBold)
	{
		this.start = start;
		this.size = size;
		days_to_bold = daysToBold;
	}
}
