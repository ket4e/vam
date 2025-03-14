namespace System.Windows.Forms;

public class DataGridViewCellToolTipTextNeededEventArgs : DataGridViewCellEventArgs
{
	private string toolTipText;

	public string ToolTipText
	{
		get
		{
			return toolTipText;
		}
		set
		{
			toolTipText = value;
		}
	}

	internal DataGridViewCellToolTipTextNeededEventArgs(string toolTipText, int rowIndex, int columnIndex)
		: base(columnIndex, rowIndex)
	{
		this.toolTipText = toolTipText;
	}
}
