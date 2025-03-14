using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ToolboxBitmap("")]
public class DataGridViewTextBoxColumn : DataGridViewColumn
{
	private int maxInputLength;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public override DataGridViewCell CellTemplate
	{
		get
		{
			return base.CellTemplate;
		}
		set
		{
			base.CellTemplate = value as DataGridViewTextBoxCell;
		}
	}

	[DefaultValue(32767)]
	public int MaxInputLength
	{
		get
		{
			return maxInputLength;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("Value is less than 0.");
			}
			maxInputLength = value;
		}
	}

	[DefaultValue(DataGridViewColumnSortMode.Automatic)]
	public new DataGridViewColumnSortMode SortMode
	{
		get
		{
			return base.SortMode;
		}
		set
		{
			base.SortMode = value;
		}
	}

	public DataGridViewTextBoxColumn()
	{
		base.CellTemplate = new DataGridViewTextBoxCell();
		maxInputLength = 32767;
		base.SortMode = DataGridViewColumnSortMode.Automatic;
	}

	public override string ToString()
	{
		return $"DataGridViewTextBoxColumn {{ Name={base.Name}, Index={base.Index} }}";
	}
}
