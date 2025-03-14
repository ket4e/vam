namespace System.Windows.Forms;

public class DataGridViewCellErrorTextNeededEventArgs : DataGridViewCellEventArgs
{
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

	internal DataGridViewCellErrorTextNeededEventArgs(string errorText, int rowIndex, int columnIndex)
		: base(columnIndex, rowIndex)
	{
		this.errorText = errorText;
	}
}
