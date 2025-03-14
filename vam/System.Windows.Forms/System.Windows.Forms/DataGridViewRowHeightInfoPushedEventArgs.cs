using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewRowHeightInfoPushedEventArgs : HandledEventArgs
{
	private int height;

	private int minimumHeight;

	private int rowIndex;

	public int Height => height;

	public int MinimumHeight => minimumHeight;

	public int RowIndex => rowIndex;

	internal DataGridViewRowHeightInfoPushedEventArgs(int rowIndex, int height, int minimumHeight)
	{
		this.rowIndex = rowIndex;
		this.height = height;
		this.minimumHeight = minimumHeight;
	}
}
