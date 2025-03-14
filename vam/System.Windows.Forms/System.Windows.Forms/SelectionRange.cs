using System.ComponentModel;

namespace System.Windows.Forms;

[TypeConverter(typeof(SelectionRangeConverter))]
public sealed class SelectionRange
{
	private DateTime end;

	private DateTime start;

	public DateTime End
	{
		get
		{
			return end;
		}
		set
		{
			if (end != value)
			{
				end = value;
			}
		}
	}

	public DateTime Start
	{
		get
		{
			return start;
		}
		set
		{
			if (start != value)
			{
				start = value;
			}
		}
	}

	public SelectionRange()
	{
		end = DateTime.MaxValue.Date;
		start = DateTime.MinValue.Date;
	}

	public SelectionRange(SelectionRange range)
	{
		end = range.End;
		start = range.Start;
	}

	public SelectionRange(DateTime lower, DateTime upper)
	{
		if (lower <= upper)
		{
			end = upper.Date;
			start = lower.Date;
		}
		else
		{
			end = lower.Date;
			start = upper.Date;
		}
	}

	public override string ToString()
	{
		return "SelectionRange: Start: " + Start.ToString() + ", End: " + End.ToString();
	}
}
