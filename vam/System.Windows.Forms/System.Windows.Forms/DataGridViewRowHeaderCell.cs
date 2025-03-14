using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewRowHeaderCell : DataGridViewHeaderCell
{
	protected class DataGridViewRowHeaderCellAccessibleObject : DataGridViewCellAccessibleObject
	{
		public override Rectangle Bounds => base.Bounds;

		public override string DefaultAction => base.DefaultAction;

		public override string Name => base.Name;

		public override AccessibleObject Parent => base.Parent;

		public override AccessibleRole Role => base.Role;

		public override AccessibleStates State => base.State;

		public override string Value => base.Value;

		public DataGridViewRowHeaderCellAccessibleObject(DataGridViewRowHeaderCell owner)
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

	private string headerText;

	internal override Rectangle InternalErrorIconsBounds => GetErrorIconBounds(null, null, base.RowIndex);

	public override object Clone()
	{
		return MemberwiseClone();
	}

	public override ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return null;
		}
		if (rowIndex < 0 || rowIndex >= base.DataGridView.Rows.Count)
		{
			throw new ArgumentOutOfRangeException("rowIndex");
		}
		if (ContextMenuStrip != null)
		{
			return ContextMenuStrip;
		}
		return base.DataGridView.ContextMenuStrip;
	}

	public override DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
	{
		DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle(base.DataGridView.DefaultCellStyle);
		dataGridViewCellStyle.ApplyStyle(base.DataGridView.RowHeadersDefaultCellStyle);
		if (base.HasStyle)
		{
			dataGridViewCellStyle.ApplyStyle(base.Style);
		}
		return dataGridViewCellStyle;
	}

	public override string ToString()
	{
		return base.ToString();
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new DataGridViewRowHeaderCellAccessibleObject(this);
	}

	protected override object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
	{
		if (base.DataGridView == null)
		{
			return null;
		}
		if (rowIndex < 0 || rowIndex >= base.DataGridView.RowCount)
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
			if (inFirstRow)
			{
				text2 = "<TABLE>";
			}
			text4 = "<TR>";
			if (lastCell)
			{
				text7 = "</TR>";
				if (inLastRow)
				{
					text5 = "</TABLE>";
				}
			}
			text3 = "<TD ALIGN=\"center\">";
			text6 = "</TD>";
			text = ((text != null) ? ("<B>" + text + "</B>") : "&nbsp;");
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
		Size size = new Size(11, 18);
		return new Rectangle(24, (base.OwningRow.Height - size.Height) / 2, size.Width, size.Height);
	}

	protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null || string.IsNullOrEmpty(base.DataGridView.GetRowInternal(rowIndex).ErrorText))
		{
			return Rectangle.Empty;
		}
		Size size = new Size(12, 11);
		return new Rectangle(new Point(base.Size.Width - size.Width - 5, (base.Size.Height - size.Height) / 2), size);
	}

	protected internal override string GetErrorText(int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return string.Empty;
		}
		return base.DataGridView.GetRowInternal(rowIndex).ErrorText;
	}

	protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
	{
		object formattedValue = base.FormattedValue;
		if (formattedValue != null)
		{
			Size result = DataGridViewCell.MeasureTextSize(graphics, formattedValue.ToString(), cellStyle.Font, TextFormatFlags.Left);
			result.Height = Math.Max(result.Height, 17);
			result.Width += 48;
			return result;
		}
		return new Size(39, 17);
	}

	protected override object GetValue(int rowIndex)
	{
		if (headerText != null)
		{
			return headerText;
		}
		return null;
	}

	[System.MonoInternalNote("Needs row header cell selected/edit pencil glyphs")]
	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		DataGridViewPaintParts dataGridViewPaintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground;
		dataGridViewPaintParts &= paintParts;
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, dataGridViewPaintParts);
		if ((paintParts & DataGridViewPaintParts.ContentBackground) == DataGridViewPaintParts.ContentBackground)
		{
			Color color = ((!Selected) ? cellStyle.ForeColor : cellStyle.SelectionForeColor);
			Pen pen = ThemeEngine.Current.ResPool.GetPen(color);
			int num = cellBounds.Left + 6;
			if (base.DataGridView.CurrentRow != null && base.DataGridView.CurrentRow.Index == rowIndex)
			{
				DrawRightArrowGlyph(graphics, pen, num, cellBounds.Top + cellBounds.Height / 2 - 4);
				num += 7;
			}
			if (base.DataGridView.Rows[rowIndex].IsNewRow)
			{
				DrawNewRowGlyph(graphics, pen, num, cellBounds.Top + cellBounds.Height / 2 - 4);
			}
		}
		if ((paintParts & DataGridViewPaintParts.ContentForeground) == DataGridViewPaintParts.ContentForeground)
		{
			Color foreColor = ((!Selected) ? cellStyle.ForeColor : cellStyle.SelectionForeColor);
			TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis;
			Rectangle bounds = cellBounds;
			bounds.Height -= 2;
			bounds.Width -= 2;
			if (formattedValue != null)
			{
				TextRenderer.DrawText(graphics, formattedValue.ToString(), cellStyle.Font, bounds, foreColor, flags);
			}
		}
		DataGridViewPaintParts dataGridViewPaintParts2 = DataGridViewPaintParts.Border | DataGridViewPaintParts.ErrorIcon;
		dataGridViewPaintParts2 &= paintParts;
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, dataGridViewPaintParts2);
	}

	protected override void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
	{
		if (!ThemeEngine.Current.DataGridViewRowHeaderCellDrawBorder(this, graphics, cellBounds))
		{
			Pen borderPen = GetBorderPen();
			graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Top, cellBounds.Left, cellBounds.Bottom - 1);
			graphics.DrawLine(borderPen, cellBounds.Right - 1, cellBounds.Top, cellBounds.Right - 1, cellBounds.Bottom - 1);
			if (base.RowIndex == base.DataGridView.Rows.Count - 1 || base.RowIndex == -1)
			{
				graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Bottom - 1, cellBounds.Right - 1, cellBounds.Bottom - 1);
			}
			else
			{
				graphics.DrawLine(borderPen, cellBounds.Left + 3, cellBounds.Bottom - 1, cellBounds.Right - 3, cellBounds.Bottom - 1);
			}
			if (base.RowIndex == -1)
			{
				graphics.DrawLine(borderPen, cellBounds.Left, cellBounds.Top, cellBounds.Right - 1, cellBounds.Top);
			}
		}
	}

	internal override void PaintPartBackground(Graphics graphics, Rectangle cellBounds, DataGridViewCellStyle style)
	{
		if (!ThemeEngine.Current.DataGridViewRowHeaderCellDrawBackground(this, graphics, cellBounds))
		{
			base.PaintPartBackground(graphics, cellBounds, style);
		}
	}

	internal override void PaintPartSelectionBackground(Graphics graphics, Rectangle cellBounds, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle)
	{
		if (!ThemeEngine.Current.DataGridViewRowHeaderCellDrawSelectionBackground(this))
		{
			base.PaintPartSelectionBackground(graphics, cellBounds, cellState, cellStyle);
		}
	}

	private void DrawRightArrowGlyph(Graphics g, Pen p, int x, int y)
	{
		g.DrawLine(p, x, y, x, y + 8);
		g.DrawLine(p, x + 1, y + 1, x + 1, y + 7);
		g.DrawLine(p, x + 2, y + 2, x + 2, y + 6);
		g.DrawLine(p, x + 3, y + 3, x + 3, y + 5);
		g.DrawLine(p, x + 3, y + 4, x + 4, y + 4);
	}

	private void DrawNewRowGlyph(Graphics g, Pen p, int x, int y)
	{
		g.DrawLine(p, x, y + 4, x + 8, y + 4);
		g.DrawLine(p, x + 4, y, x + 4, y + 8);
		g.DrawLine(p, x + 1, y + 1, x + 7, y + 7);
		g.DrawLine(p, x + 7, y + 1, x + 1, y + 7);
	}

	protected override bool SetValue(int rowIndex, object value)
	{
		headerText = (string)value;
		return true;
	}
}
