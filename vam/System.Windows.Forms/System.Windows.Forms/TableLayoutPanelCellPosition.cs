using System.ComponentModel;

namespace System.Windows.Forms;

[TypeConverter(typeof(TableLayoutPanelCellPositionTypeConverter))]
public struct TableLayoutPanelCellPosition
{
	private int column;

	private int row;

	public int Column
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

	public int Row
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

	public TableLayoutPanelCellPosition(int column, int row)
	{
		this.column = column;
		this.row = row;
	}

	public override string ToString()
	{
		return column + "," + row;
	}

	public override int GetHashCode()
	{
		return column.GetHashCode() ^ row.GetHashCode();
	}

	public override bool Equals(object other)
	{
		if (other == null)
		{
			return false;
		}
		if (!(other is TableLayoutPanelCellPosition tableLayoutPanelCellPosition))
		{
			return false;
		}
		return tableLayoutPanelCellPosition.column == column && tableLayoutPanelCellPosition.row == row;
	}

	public static bool operator ==(TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2)
	{
		return p1.column == p2.column && p1.row == p2.row;
	}

	public static bool operator !=(TableLayoutPanelCellPosition p1, TableLayoutPanelCellPosition p2)
	{
		return p1.column != p2.column || p1.row != p2.row;
	}
}
