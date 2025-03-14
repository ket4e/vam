using System.Drawing;

namespace System.Windows.Forms;

public class MeasureItemEventArgs : EventArgs
{
	private Graphics graphics;

	private int index;

	private int itemHeight;

	private int itemWidth;

	public Graphics Graphics => graphics;

	public int Index => index;

	public int ItemHeight
	{
		get
		{
			return itemHeight;
		}
		set
		{
			itemHeight = value;
		}
	}

	public int ItemWidth
	{
		get
		{
			return itemWidth;
		}
		set
		{
			itemWidth = value;
		}
	}

	public MeasureItemEventArgs(Graphics graphics, int index)
	{
		this.graphics = graphics;
		this.index = index;
		itemHeight = 0;
	}

	public MeasureItemEventArgs(Graphics graphics, int index, int itemHeight)
	{
		this.graphics = graphics;
		this.index = index;
		this.itemHeight = itemHeight;
	}
}
