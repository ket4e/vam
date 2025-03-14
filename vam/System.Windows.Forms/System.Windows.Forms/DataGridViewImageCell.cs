using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewImageCell : DataGridViewCell
{
	protected class DataGridViewImageCellAccessibleObject : DataGridViewCellAccessibleObject
	{
		public override string DefaultAction => string.Empty;

		public override string Description => (base.Owner as DataGridViewImageCell).Description;

		public override string Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

		public DataGridViewImageCellAccessibleObject(DataGridViewCell owner)
			: base(owner)
		{
		}

		public override void DoDefaultAction()
		{
		}

		public override int GetChildCount()
		{
			return -1;
		}
	}

	private object defaultNewRowValue;

	private string description;

	private DataGridViewImageCellLayout imageLayout;

	private bool valueIsIcon;

	private static Image missing_image;

	public override object DefaultNewRowValue => missing_image;

	[DefaultValue("")]
	public string Description
	{
		get
		{
			return description;
		}
		set
		{
			description = value;
		}
	}

	public override Type EditType => null;

	public override Type FormattedValueType => (!valueIsIcon) ? typeof(Image) : typeof(Icon);

	[DefaultValue(DataGridViewImageCellLayout.NotSet)]
	public DataGridViewImageCellLayout ImageLayout
	{
		get
		{
			return imageLayout;
		}
		set
		{
			if (!Enum.IsDefined(typeof(DataGridViewImageCellLayout), value))
			{
				throw new InvalidEnumArgumentException("Value is invalid image cell layout.");
			}
			imageLayout = value;
		}
	}

	[DefaultValue(false)]
	public bool ValueIsIcon
	{
		get
		{
			return valueIsIcon;
		}
		set
		{
			valueIsIcon = value;
		}
	}

	public override Type ValueType
	{
		get
		{
			if (base.ValueType != null)
			{
				return base.ValueType;
			}
			if (base.OwningColumn != null && base.OwningColumn.ValueType != null)
			{
				return base.OwningColumn.ValueType;
			}
			if (valueIsIcon)
			{
				return typeof(Icon);
			}
			return typeof(Image);
		}
		set
		{
			base.ValueType = value;
		}
	}

	public DataGridViewImageCell(bool valueIsIcon)
	{
		this.valueIsIcon = valueIsIcon;
		imageLayout = DataGridViewImageCellLayout.NotSet;
	}

	public DataGridViewImageCell()
		: this(valueIsIcon: false)
	{
	}

	static DataGridViewImageCell()
	{
		missing_image = ResourceImageLoader.Get("image-missing.png");
	}

	public override object Clone()
	{
		DataGridViewImageCell dataGridViewImageCell = (DataGridViewImageCell)base.Clone();
		dataGridViewImageCell.defaultNewRowValue = defaultNewRowValue;
		dataGridViewImageCell.description = description;
		dataGridViewImageCell.valueIsIcon = valueIsIcon;
		return dataGridViewImageCell;
	}

	public override string ToString()
	{
		return GetType().Name;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new DataGridViewImageCellAccessibleObject(this);
	}

	protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return Rectangle.Empty;
		}
		Rectangle result = Rectangle.Empty;
		Image image = (Image)GetFormattedValue(base.Value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.PreferredSize);
		if (image == null)
		{
			image = missing_image;
		}
		switch (imageLayout)
		{
		case DataGridViewImageCellLayout.NotSet:
		case DataGridViewImageCellLayout.Normal:
			result = new Rectangle((base.Size.Width - image.Width) / 2, (base.Size.Height - image.Height) / 2, image.Width, image.Height);
			break;
		case DataGridViewImageCellLayout.Stretch:
			result = new Rectangle(Point.Empty, base.Size);
			break;
		case DataGridViewImageCellLayout.Zoom:
		{
			Size size = ((!((float)image.Width / (float)image.Height >= (float)base.Size.Width / (float)base.Size.Height)) ? new Size(image.Width * base.Size.Height / image.Height, base.Size.Height) : new Size(base.Size.Width, image.Height * base.Size.Width / image.Width));
			result = new Rectangle((base.Size.Width - size.Width) / 2, (base.Size.Height - size.Height) / 2, size.Width, size.Height);
			break;
		}
		}
		return result;
	}

	protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null || string.IsNullOrEmpty(base.ErrorText))
		{
			return Rectangle.Empty;
		}
		Size size = new Size(12, 11);
		return new Rectangle(new Point(base.Size.Width - size.Width - 5, (base.Size.Height - size.Height) / 2), size);
	}

	protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
	{
		return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
	}

	protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
	{
		Image image = (Image)base.FormattedValue;
		if (image == null)
		{
			return new Size(21, 20);
		}
		if (image != null)
		{
			return new Size(image.Width + 1, image.Height + 1);
		}
		return new Size(21, 20);
	}

	protected override object GetValue(int rowIndex)
	{
		return base.GetValue(rowIndex);
	}

	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
	}

	internal override void PaintPartContent(Graphics graphics, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle, object formattedValue)
	{
		Image image = ((formattedValue != null) ? ((Image)formattedValue) : missing_image);
		Rectangle rect = Rectangle.Empty;
		switch (imageLayout)
		{
		case DataGridViewImageCellLayout.NotSet:
		case DataGridViewImageCellLayout.Normal:
			rect = AlignInRectangle(new Rectangle(2, 2, cellBounds.Width - 4, cellBounds.Height - 4), image.Size, cellStyle.Alignment);
			break;
		case DataGridViewImageCellLayout.Stretch:
			rect = new Rectangle(Point.Empty, cellBounds.Size);
			break;
		case DataGridViewImageCellLayout.Zoom:
		{
			Size size = ((!((float)image.Width / (float)image.Height >= (float)base.Size.Width / (float)base.Size.Height)) ? new Size(image.Width * base.Size.Height / image.Height, base.Size.Height) : new Size(base.Size.Width, image.Height * base.Size.Width / image.Width));
			rect = new Rectangle((base.Size.Width - size.Width) / 2, (base.Size.Height - size.Height) / 2, size.Width, size.Height);
			break;
		}
		}
		rect.X += cellBounds.Left;
		rect.Y += cellBounds.Top;
		graphics.DrawImage(image, rect);
	}
}
