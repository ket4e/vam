using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewColumnHeaderCell : DataGridViewHeaderCell
{
	protected class DataGridViewColumnHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
	{
		public override Rectangle Bounds => base.Bounds;

		public override string DefaultAction => base.DefaultAction;

		public override string Name => base.Name;

		public override AccessibleObject Parent => base.Parent;

		public override AccessibleRole Role => base.Role;

		public override AccessibleStates State => base.State;

		public override string Value => base.Value;

		public DataGridViewColumnHeaderCellAccessibleObject(DataGridViewColumnHeaderCell owner)
			: base(owner)
		{
		}

		public override void DoDefaultAction()
		{
			base.DoDefaultAction();
		}

		public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
		{
			return base.Navigate(navigationDirection);
		}

		public override void Select(AccessibleSelection flags)
		{
			base.Select(flags);
		}
	}

	private SortOrder sortGlyphDirection;

	private object header_text;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public SortOrder SortGlyphDirection
	{
		get
		{
			return sortGlyphDirection;
		}
		set
		{
			sortGlyphDirection = value;
		}
	}

	public override object Clone()
	{
		return MemberwiseClone();
	}

	public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
	{
		if (rowIndex != -1)
		{
			throw new ArgumentOutOfRangeException("RowIndex is not -1");
		}
		if (base.ContextMenuStrip != null)
		{
			return base.ContextMenuStrip;
		}
		return base.GetInheritedContextMenuStrip(rowIndex);
	}

	public override DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
	{
		DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle(base.DataGridView.DefaultCellStyle);
		dataGridViewCellStyle.ApplyStyle(base.DataGridView.ColumnHeadersDefaultCellStyle);
		if (base.HasStyle)
		{
			dataGridViewCellStyle.ApplyStyle(base.Style);
		}
		return dataGridViewCellStyle;
	}

	public override string ToString()
	{
		return GetType().Name;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new DataGridViewColumnHeaderCellAccessibleObject(this);
	}

	protected override object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
	{
		if (rowIndex != -1)
		{
			throw new ArgumentOutOfRangeException("rowIndex");
		}
		string text = GetValue(rowIndex) as string;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		string text5 = string.Empty;
		string text6 = string.Empty;
		string text7 = string.Empty;
		if (format == DataFormats.UnicodeText || format == DataFormats.Text)
		{
			if (lastCell && !inLastRow)
			{
				text6 = Environment.NewLine;
			}
			else if (!lastCell)
			{
				text6 = "\t";
			}
		}
		else if (format == DataFormats.CommaSeparatedValue)
		{
			if (lastCell && !inLastRow)
			{
				text6 = Environment.NewLine;
			}
			else if (!lastCell)
			{
				text6 = ",";
			}
		}
		else
		{
			if (!(format == DataFormats.Html))
			{
				return text;
			}
			if (firstCell)
			{
				text2 = "<TABLE>";
				text4 = "<THEAD>";
			}
			text3 = "<TH>";
			text6 = "</TH>";
			if (lastCell)
			{
				text7 = "</THEAD>";
				if (inLastRow)
				{
					text5 = "</TABLE>";
				}
			}
			if (text == null)
			{
				text = "&nbsp;";
			}
		}
		if (text == null)
		{
			text = string.Empty;
		}
		return text2 + text4 + text3 + text + text6 + text7 + text5;
	}

	protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return Rectangle.Empty;
		}
		object value = GetValue(-1);
		if (value == null || value.ToString() == string.Empty)
		{
			return Rectangle.Empty;
		}
		Size size = Size.Empty;
		if (value != null)
		{
			size = DataGridViewCell.MeasureTextSize(graphics, value.ToString(), cellStyle.Font, TextFormatFlags.Left);
		}
		return new Rectangle(3, (base.DataGridView.ColumnHeadersHeight - size.Height) / 2, size.Width, size.Height);
	}

	protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
	{
		object obj = header_text;
		if (obj != null)
		{
			Size result = DataGridViewCell.MeasureTextSize(graphics, obj.ToString(), cellStyle.Font, TextFormatFlags.Left);
			result.Height = Math.Max(result.Height, 18);
			result.Width += 25;
			return result;
		}
		return new Size(19, 12);
	}

	protected override object GetValue(int rowIndex)
	{
		if (header_text != null)
		{
			return header_text;
		}
		if (base.OwningColumn != null && !base.OwningColumn.HeaderTextSet)
		{
			return base.OwningColumn.Name;
		}
		return null;
	}

	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates dataGridViewElementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		DataGridViewPaintParts dataGridViewPaintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground;
		dataGridViewPaintParts &= paintParts;
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, dataGridViewPaintParts);
		if ((paintParts & DataGridViewPaintParts.ContentForeground) == DataGridViewPaintParts.ContentForeground)
		{
			Color color = ((!Selected) ? cellStyle.ForeColor : cellStyle.SelectionForeColor);
			TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis;
			Rectangle bounds = cellBounds;
			bounds.Height -= 2;
			bounds.Width -= 2;
			if (formattedValue != null)
			{
				TextRenderer.DrawText(graphics, formattedValue.ToString(), cellStyle.Font, bounds, color, flags);
			}
			Point point = new Point(cellBounds.Right - 14, cellBounds.Y + (cellBounds.Height - 4) / 2);
			if (sortGlyphDirection == SortOrder.Ascending)
			{
				using Pen pen = new Pen(color);
				graphics.DrawLine(pen, point.X + 4, point.Y + 1, point.X + 4, point.Y + 2);
				graphics.DrawLine(pen, point.X + 3, point.Y + 2, point.X + 5, point.Y + 2);
				graphics.DrawLine(pen, point.X + 2, point.Y + 3, point.X + 6, point.Y + 3);
				graphics.DrawLine(pen, point.X + 1, point.Y + 4, point.X + 7, point.Y + 4);
				graphics.DrawLine(pen, point.X, point.Y + 5, point.X + 8, point.Y + 5);
			}
			else if (sortGlyphDirection == SortOrder.Descending)
			{
				using Pen pen2 = new Pen(color);
				graphics.DrawLine(pen2, point.X + 4, point.Y + 5, point.X + 4, point.Y + 4);
				graphics.DrawLine(pen2, point.X + 3, point.Y + 4, point.X + 5, point.Y + 4);
				graphics.DrawLine(pen2, point.X + 2, point.Y + 3, point.X + 6, point.Y + 3);
				graphics.DrawLine(pen2, point.X + 1, point.Y + 2, point.X + 7, point.Y + 2);
				graphics.DrawLine(pen2, point.X, point.Y + 1, point.X + 8, point.Y + 1);
			}
		}
		DataGridViewPaintParts dataGridViewPaintParts2 = DataGridViewPaintParts.Border;
		if (this is DataGridViewTopLeftHeaderCell)
		{
			dataGridViewPaintParts2 |= DataGridViewPaintParts.ErrorIcon;
		}
		dataGridViewPaintParts2 &= paintParts;
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, dataGridViewPaintParts2);
	}

	protected override void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
	{
		if (ThemeEngine.Current.DataGridViewColumnHeaderCellDrawBorder(this, graphics, cellBounds))
		{
			return;
		}
		Pen borderPen = GetBorderPen();
		if (base.ColumnIndex == -1)
		{
			graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Top, cellBounds.Left, cellBounds.Bottom - 1);
			graphics.DrawLine(borderPen, cellBounds.Right - 1, cellBounds.Top, cellBounds.Right - 1, cellBounds.Bottom - 1);
			graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Bottom - 1, cellBounds.Right - 1, cellBounds.Bottom - 1);
			graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Top, cellBounds.Right - 1, cellBounds.Top);
			return;
		}
		graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Bottom - 1, cellBounds.Right - 1, cellBounds.Bottom - 1);
		graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Top, cellBounds.Right - 1, cellBounds.Top);
		if (base.ColumnIndex == base.DataGridView.Columns.Count - 1 || base.ColumnIndex == -1)
		{
			graphics.DrawLine(borderPen, cellBounds.Right - 1, cellBounds.Top, cellBounds.Right - 1, cellBounds.Bottom - 1);
		}
		else
		{
			graphics.DrawLine(borderPen, cellBounds.Right - 1, cellBounds.Top + 3, cellBounds.Right - 1, cellBounds.Bottom - 3);
		}
	}

	internal override void PaintPartBackground(Graphics graphics, Rectangle cellBounds, DataGridViewCellStyle style)
	{
		if (!ThemeEngine.Current.DataGridViewColumnHeaderCellDrawBackground(this, graphics, cellBounds))
		{
			base.PaintPartBackground(graphics, cellBounds, style);
		}
	}

	protected override bool SetValue(int rowIndex, object value)
	{
		header_text = value;
		return true;
	}
}
