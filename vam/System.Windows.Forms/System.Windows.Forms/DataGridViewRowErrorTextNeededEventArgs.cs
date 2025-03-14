namespace System.Windows.Forms;

public class DataGridViewRowErrorTextNeededEventArgs : EventArgs
{
	private int rowIndex;

	private string errorText;

	public string ErrorText
	{
		get
		{
			return errorText;
		}
		set
		{
			errorText = value;
		}
	}

	public int RowIndex => rowIndex;

	internal DataGridViewRowErrorTextNeededEventArgs(int rowIndex, string errorText)
	{
		this.rowIndex = rowIndex;
		this.errorText = errorText;
	}
}
