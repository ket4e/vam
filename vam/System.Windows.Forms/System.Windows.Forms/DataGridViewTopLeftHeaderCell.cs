using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewTopLeftHeaderCell : DataGridViewColumnHeaderCell
{
	protected class DataGridViewTopLeftHeaderCellAccessibleObject : DataGridViewColumnHeaderCellAccessibleObject
	{
		public override Rectangle Bounds
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string DefaultAction
		{
			get
			{
				if (base.Owner.DataGridView != null && base.Owner.DataGridView.MultiSelect)
				{
					return "Press to Select All";
				}
				return string.Empty;
			}
		}

		public override string Name => base.Name;

		public override AccessibleStates State => base.State;

		public override string Value => base.Value;

		public DataGridViewTopLeftHeaderCellAccessibleObject(DataGridViewTopLeftHeaderCell owner)
			: base(owner)
		{
		}

		public override void DoDefaultAction()
		{
			if (base.Owner.DataGridView != null)
			{
				base.Owner.DataGridView.SelectAll();
			}
		}

		public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
		{
			throw new NotImplementedException();
		}

		public override void Select(AccessibleSelection flags)
		{
			base.Select(flags);
		}
	}

	public override string ToString()
	{
		return GetType().Name;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new DataGridViewTopLeftHeaderCellAccessibleObject(this);
	}

	protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return Rectangle.Empty;
		}
		Size size = new Size(36, 13);
		return new Rectangle(2, (base.DataGridView.ColumnHeadersHeight - size.Height) / 2, size.Width, size.Height);
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

	protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
	{
		object value = base.Value;
		if (value != null)
		{
			Size result = DataGridViewCell.MeasureTextSize(graphics, value.ToString(), cellStyle.Font, TextFormatFlags.Left);
			result.Height = Math.Max(result.Height, 17);
			result.Width += 29;
			return result;
		}
		return new Size(39, 17);
	}

	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
	}

	protected override void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle bounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
	{
		base.PaintBorder(graphics, clipBounds, bounds, cellStyle, advancedBorderStyle);
	}
}
