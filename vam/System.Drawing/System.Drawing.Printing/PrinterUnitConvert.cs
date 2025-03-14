namespace System.Drawing.Printing;

public sealed class PrinterUnitConvert
{
	private PrinterUnitConvert()
	{
	}

	public static double Convert(double value, PrinterUnit fromUnit, PrinterUnit toUnit)
	{
		switch (fromUnit)
		{
		case PrinterUnit.Display:
			switch (toUnit)
			{
			case PrinterUnit.Display:
				return value;
			case PrinterUnit.ThousandthsOfAnInch:
				return value * 10.0;
			case PrinterUnit.HundredthsOfAMillimeter:
				return value * 25.4;
			case PrinterUnit.TenthsOfAMillimeter:
				return value * 2.54;
			}
			break;
		case PrinterUnit.ThousandthsOfAnInch:
			switch (toUnit)
			{
			case PrinterUnit.Display:
				return value / 10.0;
			case PrinterUnit.ThousandthsOfAnInch:
				return value;
			case PrinterUnit.HundredthsOfAMillimeter:
				return value * 2.54;
			case PrinterUnit.TenthsOfAMillimeter:
				return value * 0.254;
			}
			break;
		case PrinterUnit.HundredthsOfAMillimeter:
			switch (toUnit)
			{
			case PrinterUnit.Display:
				return value / 25.4;
			case PrinterUnit.ThousandthsOfAnInch:
				return value / 2.54;
			case PrinterUnit.HundredthsOfAMillimeter:
				return value;
			case PrinterUnit.TenthsOfAMillimeter:
				return value / 10.0;
			}
			break;
		case PrinterUnit.TenthsOfAMillimeter:
			switch (toUnit)
			{
			case PrinterUnit.Display:
				return value / 2.54;
			case PrinterUnit.ThousandthsOfAnInch:
				return value / 0.254;
			case PrinterUnit.HundredthsOfAMillimeter:
				return value * 10.0;
			case PrinterUnit.TenthsOfAMillimeter:
				return value;
			}
			break;
		}
		throw new NotImplementedException();
	}

	public static int Convert(int value, PrinterUnit fromUnit, PrinterUnit toUnit)
	{
		double a = Convert((double)value, fromUnit, toUnit);
		return (int)Math.Round(a);
	}

	public static Margins Convert(Margins value, PrinterUnit fromUnit, PrinterUnit toUnit)
	{
		return new Margins(Convert(value.Left, fromUnit, toUnit), Convert(value.Right, fromUnit, toUnit), Convert(value.Top, fromUnit, toUnit), Convert(value.Bottom, fromUnit, toUnit));
	}

	public static Point Convert(Point value, PrinterUnit fromUnit, PrinterUnit toUnit)
	{
		return new Point(Convert(value.X, fromUnit, toUnit), Convert(value.Y, fromUnit, toUnit));
	}

	public static Rectangle Convert(Rectangle value, PrinterUnit fromUnit, PrinterUnit toUnit)
	{
		return new Rectangle(Convert(value.X, fromUnit, toUnit), Convert(value.Y, fromUnit, toUnit), Convert(value.Width, fromUnit, toUnit), Convert(value.Height, fromUnit, toUnit));
	}

	public static Size Convert(Size value, PrinterUnit fromUnit, PrinterUnit toUnit)
	{
		return new Size(Convert(value.Width, fromUnit, toUnit), Convert(value.Height, fromUnit, toUnit));
	}
}
