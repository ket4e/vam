using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ToolboxBitmap("")]
public class DataGridViewButtonColumn : DataGridViewColumn
{
	private FlatStyle flatStyle;

	private string text;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override DataGridViewCell CellTemplate
	{
		get
		{
			return base.CellTemplate;
		}
		set
		{
			base.CellTemplate = value as DataGridViewButtonCell;
		}
	}

	[Browsable(true)]
	public override DataGridViewCellStyle DefaultCellStyle
	{
		get
		{
			return base.DefaultCellStyle;
		}
		set
		{
			base.DefaultCellStyle = value;
		}
	}

	[DefaultValue(FlatStyle.Standard)]
	public FlatStyle FlatStyle
	{
		get
		{
			return flatStyle;
		}
		set
		{
			flatStyle = value;
		}
	}

	[DefaultValue(null)]
	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	[DefaultValue(false)]
	public bool UseColumnTextForButtonValue
	{
		get
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null when getting this property.");
			}
			return (base.CellTemplate as DataGridViewButtonCell).UseColumnTextForButtonValue;
		}
		set
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null when setting this property.");
			}
			(base.CellTemplate as DataGridViewButtonCell).UseColumnTextForButtonValue = value;
		}
	}

	public DataGridViewButtonColumn()
	{
		base.CellTemplate = new DataGridViewButtonCell();
		flatStyle = FlatStyle.Standard;
		text = string.Empty;
	}

	public override object Clone()
	{
		DataGridViewButtonColumn dataGridViewButtonColumn = (DataGridViewButtonColumn)base.Clone();
		dataGridViewButtonColumn.flatStyle = flatStyle;
		dataGridViewButtonColumn.text = text;
		return dataGridViewButtonColumn;
	}

	public override string ToString()
	{
		return $"DataGridViewButtonColumn {{ Name={base.Name}, Index={base.Index} }}";
	}
}
