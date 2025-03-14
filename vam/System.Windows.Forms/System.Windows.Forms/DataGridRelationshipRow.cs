using System.Drawing;

namespace System.Windows.Forms;

internal class DataGridRelationshipRow
{
	private DataGrid owner;

	public int height;

	public bool IsSelected;

	public bool IsExpanded;

	public int VerticalOffset;

	public int RelationHeight;

	public Rectangle relation_area;

	public int Height
	{
		get
		{
			return height;
		}
		set
		{
			if (height != value)
			{
				height = value;
				owner.UpdateRowsFrom(this);
			}
		}
	}

	public DataGridRelationshipRow(DataGrid owner)
	{
		this.owner = owner;
		IsSelected = false;
		IsExpanded = false;
		height = 0;
		VerticalOffset = 0;
		RelationHeight = 0;
		relation_area = Rectangle.Empty;
	}
}
