using System.Collections;

namespace System.Windows.Forms;

internal class Matchlet
{
	private byte[] byteValue;

	private byte[] mask;

	private int offset;

	private int offsetLength;

	private int wordSize = 1;

	private ArrayList matchlets = new ArrayList();

	public byte[] ByteValue
	{
		get
		{
			return byteValue;
		}
		set
		{
			byteValue = value;
		}
	}

	public byte[] Mask
	{
		get
		{
			return mask;
		}
		set
		{
			mask = value;
		}
	}

	public int Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
		}
	}

	public int OffsetLength
	{
		get
		{
			return offsetLength;
		}
		set
		{
			offsetLength = value;
		}
	}

	public int WordSize
	{
		get
		{
			return wordSize;
		}
		set
		{
			wordSize = value;
		}
	}

	public ArrayList Matchlets => matchlets;
}
