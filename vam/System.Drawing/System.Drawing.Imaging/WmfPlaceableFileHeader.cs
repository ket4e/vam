using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public sealed class WmfPlaceableFileHeader
{
	private int key;

	private short handle;

	private short left;

	private short top;

	private short right;

	private short bottom;

	private short inch;

	private int reserved;

	private short checksum;

	public short BboxBottom
	{
		get
		{
			return bottom;
		}
		set
		{
			bottom = value;
		}
	}

	public short BboxLeft
	{
		get
		{
			return left;
		}
		set
		{
			left = value;
		}
	}

	public short BboxRight
	{
		get
		{
			return right;
		}
		set
		{
			right = value;
		}
	}

	public short BboxTop
	{
		get
		{
			return top;
		}
		set
		{
			top = value;
		}
	}

	public short Checksum
	{
		get
		{
			return checksum;
		}
		set
		{
			checksum = value;
		}
	}

	public short Hmf
	{
		get
		{
			return handle;
		}
		set
		{
			handle = value;
		}
	}

	public short Inch
	{
		get
		{
			return inch;
		}
		set
		{
			inch = value;
		}
	}

	public int Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
		}
	}

	public int Reserved
	{
		get
		{
			return reserved;
		}
		set
		{
			reserved = value;
		}
	}

	public WmfPlaceableFileHeader()
	{
		key = -1698247209;
	}
}
