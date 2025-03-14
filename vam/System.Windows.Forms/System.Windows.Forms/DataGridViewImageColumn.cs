using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[ToolboxBitmap("")]
public class DataGridViewImageColumn : DataGridViewColumn
{
	private Icon icon;

	private Image image;

	private bool valuesAreIcons;

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
			base.CellTemplate = value as DataGridViewImageCell;
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

	[DefaultValue("")]
	[Browsable(true)]
	public string Description
	{
		get
		{
			return (base.CellTemplate as DataGridViewImageCell).Description;
		}
		set
		{
			(base.CellTemplate as DataGridViewImageCell).Description = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Icon Icon
	{
		get
		{
			return icon;
		}
		set
		{
			icon = value;
		}
	}

	[DefaultValue(null)]
	public Image Image
	{
		get
		{
			return image;
		}
		set
		{
			image = value;
		}
	}

	[DefaultValue(DataGridViewImageCellLayout.Normal)]
	public DataGridViewImageCellLayout ImageLayout
	{
		get
		{
			return (base.CellTemplate as DataGridViewImageCell).ImageLayout;
		}
		set
		{
			(base.CellTemplate as DataGridViewImageCell).ImageLayout = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool ValuesAreIcons
	{
		get
		{
			return valuesAreIcons;
		}
		set
		{
			valuesAreIcons = value;
		}
	}

	public DataGridViewImageColumn()
		: this(valuesAreIcons: false)
	{
	}

	public DataGridViewImageColumn(bool valuesAreIcons)
	{
		this.valuesAreIcons = valuesAreIcons;
		base.CellTemplate = new DataGridViewImageCell(valuesAreIcons);
		(base.CellTemplate as DataGridViewImageCell).ImageLayout = DataGridViewImageCellLayout.Normal;
		DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		icon = null;
		image = null;
	}

	public override object Clone()
	{
		DataGridViewImageColumn dataGridViewImageColumn = (DataGridViewImageColumn)base.Clone();
		dataGridViewImageColumn.icon = icon;
		dataGridViewImageColumn.image = image;
		return dataGridViewImageColumn;
	}

	public override string ToString()
	{
		return GetType().Name;
	}
}
