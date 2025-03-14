namespace System.Windows.Forms;

public class NumericUpDownAcceleration
{
	private decimal increment;

	private int seconds;

	public decimal Increment
	{
		get
		{
			return increment;
		}
		set
		{
			increment = value;
		}
	}

	public int Seconds
	{
		get
		{
			return seconds;
		}
		set
		{
			seconds = value;
		}
	}

	public NumericUpDownAcceleration(int seconds, decimal increment)
	{
		if (seconds < 0)
		{
			throw new ArgumentOutOfRangeException("Invalid seconds value. The seconds value must be equal or greater than zero.");
		}
		if (increment < 0m)
		{
			throw new ArgumentOutOfRangeException("Invalid increment value. The increment value must be equal or greater than zero.");
		}
		this.increment = increment;
		this.seconds = seconds;
	}
}
