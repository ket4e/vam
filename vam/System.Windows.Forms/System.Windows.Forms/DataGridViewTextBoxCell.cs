using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class DataGridViewTextBoxCell : DataGridViewCell
{
	private int maxInputLength = 32767;

	private static DataGridViewTextBoxEditingControl editingControl;

	public override Type FormattedValueType => typeof(string);

	[DefaultValue(32767)]
	public virtual int MaxInputLength
	{
		get
		{
			return maxInputLength;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("MaxInputLength coudn't be less than 0.");
			}
			maxInputLength = value;
		}
	}

	public override Type ValueType => base.ValueType;

	public DataGridViewTextBoxCell()
	{
		base.ValueType = typeof(object);
	}

	static DataGridViewTextBoxCell()
	{
		editingControl = new DataGridViewTextBoxEditingControl();
		editingControl.Multiline = false;
		editingControl.BorderStyle = BorderStyle.None;
	}

	public override object Clone()
	{
		DataGridViewTextBoxCell dataGridViewTextBoxCell = (DataGridViewTextBoxCell)base.Clone();
		dataGridViewTextBoxCell.maxInputLength = maxInputLength;
		return dataGridViewTextBoxCell;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public override void DetachEditingControl()
	{
		if (base.DataGridView == null)
		{
			throw new InvalidOperationException("There is no associated DataGridView.");
		}
		base.DataGridView.EditingControlInternal = null;
	}

	public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
	{
		if (base.DataGridView == null)
		{
			throw new InvalidOperationException("There is no associated DataGridView.");
		}
		base.DataGridView.EditingControlInternal = editingControl;
		editingControl.EditingControlDataGridView = base.DataGridView;
		editingControl.MaxLength = maxInputLength;
		if (initialFormattedValue == null || initialFormattedValue.ToString() == string.Empty)
		{
			editingControl.Text = string.Empty;
		}
		else
		{
			editingControl.Text = initialFormattedValue.ToString();
		}
		editingControl.ApplyCellStyleToEditingControl(dataGridViewCellStyle);
		editingControl.PrepareEditingControlForEdit(selectAll: true);
	}

	public override bool KeyEntersEditMode(KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Space)
		{
			return true;
		}
		if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.Z)
		{
			return true;
		}
		if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.Divide)
		{
			return true;
		}
		if (e.KeyCode == Keys.BrowserSearch || e.KeyCode == Keys.SelectMedia)
		{
			return true;
		}
		if (e.KeyCode >= Keys.OemSemicolon && e.KeyCode <= Keys.ProcessKey)
		{
			return true;
		}
		if (e.KeyCode == Keys.Attn || e.KeyCode == Keys.Packet)
		{
			return true;
		}
		if (e.KeyCode >= Keys.Exsel && e.KeyCode <= Keys.OemClear)
		{
			return true;
		}
		return false;
	}

	public override void PositionEditingControl(bool setLocation, bool setSize, Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
	{
		cellBounds.Size = new Size(cellBounds.Width - 5, cellBounds.Height + 2);
		cellBounds.Location = new Point(cellBounds.X + 3, (cellBounds.Height - editingControl.Height) / 2 + cellBounds.Y - 1);
		base.PositionEditingControl(setLocation, setSize, cellBounds, cellClip, cellStyle, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow);
		editingControl.Invalidate();
	}

	public override string ToString()
	{
		return $"DataGridViewTextBoxCell {{ ColumnIndex={base.ColumnIndex}, RowIndex={base.RowIndex} }}";
	}

	protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return Rectangle.Empty;
		}
		object formattedValue = base.FormattedValue;
		Size size = Size.Empty;
		if (formattedValue != null)
		{
			size = DataGridViewCell.MeasureTextSize(graphics, formattedValue.ToString(), cellStyle.Font, TextFormatFlags.Left);
			size.Height += 2;
		}
		return new Rectangle(0, (base.OwningRow.Height - size.Height) / 2, size.Width, size.Height);
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
		object formattedValue = base.FormattedValue;
		if (formattedValue != null)
		{
			Size result = DataGridViewCell.MeasureTextSize(graphics, formattedValue.ToString(), cellStyle.Font, TextFormatFlags.Left);
			result.Height = Math.Max(result.Height, 20);
			result.Width += 2;
			return result;
		}
		return new Size(21, 20);
	}

	protected override void OnEnter(int rowIndex, bool throughMouseClick)
	{
	}

	protected override void OnLeave(int rowIndex, bool throughMouseClick)
	{
	}

	protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
	{
	}

	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		DataGridViewPaintParts dataGridViewPaintParts = DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground;
		dataGridViewPaintParts &= paintParts;
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, dataGridViewPaintParts);
		if (!base.IsInEditMode && (paintParts & DataGridViewPaintParts.ContentForeground) == DataGridViewPaintParts.ContentForeground)
		{
			Color foreColor = ((!Selected) ? cellStyle.ForeColor : cellStyle.SelectionForeColor);
			TextFormatFlags textFormatFlags = TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis;
			textFormatFlags |= AlignmentToFlags(cellStyle.Alignment);
			Rectangle bounds = cellBounds;
			bounds.Height -= 2;
			bounds.Width -= 2;
			if ((cellStyle.Alignment & (DataGridViewContentAlignment)7) > DataGridViewContentAlignment.NotSet)
			{
				bounds.Offset(0, 2);
				bounds.Height -= 2;
			}
			if (formattedValue != null)
			{
				TextRenderer.DrawText(graphics, formattedValue.ToString(), cellStyle.Font, bounds, foreColor, textFormatFlags);
			}
		}
		DataGridViewPaintParts dataGridViewPaintParts2 = DataGridViewPaintParts.Border | DataGridViewPaintParts.ErrorIcon | DataGridViewPaintParts.Focus;
		dataGridViewPaintParts2 &= paintParts;
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, dataGridViewPaintParts2);
	}
}
