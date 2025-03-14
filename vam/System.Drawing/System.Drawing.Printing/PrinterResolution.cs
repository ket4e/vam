namespace System.Drawing.Printing;

[Serializable]
public class PrinterResolution
{
	private PrinterResolutionKind kind;

	private int x;

	private int y;

	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public PrinterResolutionKind Kind
	{
		get
		{
			return kind;
		}
		set
		{
			kind = value;
		}
	}

	public PrinterResolution()
	{
	}

	internal PrinterResolution(int x, int y, PrinterResolutionKind kind)
	{
		this.x = x;
		this.y = y;
		this.kind = kind;
	}

	public override string ToString()
	{
		if (kind != 0)
		{
			return "[PrinterResolution " + kind.ToString() + "]";
		}
		return "[PrinterResolution X=" + x + " Y=" + y + "]";
	}
}
