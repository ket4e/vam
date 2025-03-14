using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ComVisible(true)]
public class SplitterEventArgs : EventArgs
{
	internal int split_x;

	internal int split_y;

	internal int x;

	internal int y;

	public int SplitX
	{
		get
		{
			return split_x;
		}
		set
		{
			split_x = value;
		}
	}

	public int SplitY
	{
		get
		{
			return split_y;
		}
		set
		{
			split_y = value;
		}
	}

	public int X => x;

	public int Y => y;

	public SplitterEventArgs(int x, int y, int splitX, int splitY)
	{
		this.x = x;
		this.y = y;
		SplitX = splitX;
		SplitY = splitY;
	}
}
