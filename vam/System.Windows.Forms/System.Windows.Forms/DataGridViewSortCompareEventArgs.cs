using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewSortCompareEventArgs : HandledEventArgs
{
	private DataGridViewColumn dataGridViewColumn;

	private object cellValue1;

	private object cellValue2;

	private int rowIndex1;

	private int rowIndex2;

	private int sortResult;

	public object CellValue1 => cellValue1;

	public object CellValue2 => cellValue2;

	public DataGridViewColumn Column => dataGridViewColumn;

	public int RowIndex1 => rowIndex1;

	public int RowIndex2 => rowIndex2;

	public int SortResult
	{
		get
		{
			return sortResult;
		}
		set
		{
			sortResult = value;
		}
	}

	public DataGridViewSortCompareEventArgs(DataGridViewColumn dataGridViewColumn, object cellValue1, object cellValue2, int rowIndex1, int rowIndex2)
	{
		this.dataGridViewColumn = dataGridViewColumn;
		this.cellValue1 = cellValue1;
		this.cellValue2 = cellValue2;
		this.rowIndex1 = rowIndex1;
		this.rowIndex2 = rowIndex2;
	}
}
