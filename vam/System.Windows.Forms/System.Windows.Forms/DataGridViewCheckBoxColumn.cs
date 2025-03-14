using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ToolboxBitmap("")]
public class DataGridViewCheckBoxColumn : DataGridViewColumn
{
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
			base.CellTemplate = value as DataGridViewCheckBoxCell;
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

	[TypeConverter(typeof(StringConverter))]
	[DefaultValue(null)]
	public object FalseValue
	{
		get
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			return (base.CellTemplate as DataGridViewCheckBoxCell).FalseValue;
		}
		set
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			(base.CellTemplate as DataGridViewCheckBoxCell).FalseValue = value;
		}
	}

	[DefaultValue(FlatStyle.Standard)]
	public FlatStyle FlatStyle
	{
		get
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			return (base.CellTemplate as DataGridViewCheckBoxCell).FlatStyle;
		}
		set
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			(base.CellTemplate as DataGridViewCheckBoxCell).FlatStyle = value;
		}
	}

	[TypeConverter(typeof(StringConverter))]
	[DefaultValue(null)]
	public object IndeterminateValue
	{
		get
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			return (base.CellTemplate as DataGridViewCheckBoxCell).IndeterminateValue;
		}
		set
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			(base.CellTemplate as DataGridViewCheckBoxCell).IndeterminateValue = value;
		}
	}

	[DefaultValue(false)]
	public bool ThreeState
	{
		get
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			return (base.CellTemplate as DataGridViewCheckBoxCell).ThreeState;
		}
		set
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			(base.CellTemplate as DataGridViewCheckBoxCell).ThreeState = value;
		}
	}

	[TypeConverter(typeof(StringConverter))]
	[DefaultValue(null)]
	public object TrueValue
	{
		get
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			return (base.CellTemplate as DataGridViewCheckBoxCell).TrueValue;
		}
		set
		{
			if (base.CellTemplate == null)
			{
				throw new InvalidOperationException("CellTemplate is null.");
			}
			(base.CellTemplate as DataGridViewCheckBoxCell).TrueValue = value;
		}
	}

	public DataGridViewCheckBoxColumn(bool threeState)
	{
		CellTemplate = new DataGridViewCheckBoxCell(threeState);
	}

	public DataGridViewCheckBoxColumn()
		: this(threeState: false)
	{
	}

	public override string ToString()
	{
		return $"DataGridViewCheckBoxColumn {{ Name={base.Name}, Index={base.Index} }}";
	}
}
