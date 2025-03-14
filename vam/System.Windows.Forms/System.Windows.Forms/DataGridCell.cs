namespace System.Windows.Forms;

public struct DataGridCell
{
	private int row;

	private int column;

	public int ColumnNumber
	{
		get
		{
			return column;
		}
		set
		{
			column = value;
		}
	}

	public int RowNumber
	{
		get
		{
			return row;
		}
		set
		{
			row = value;
		}
	}

	public DataGridCell(int r, int c)
	{
		row = r;
		column = c;
	}

	public override bool Equals(object o)
	{
		if (!(o is DataGridCell dataGridCell))
		{
			return false;
		}
		return dataGridCell.ColumnNumber == column && dataGridCell.RowNumber == row;
	}

	public override int GetHashCode()
	{
		return row ^ column;
	}

	public override string ToString()
	{
		return "DataGridCell {RowNumber = " + row + ", ColumnNumber = " + column + "}";
	}
}
