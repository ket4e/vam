using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Windows.Forms;

public class DataGridTextBoxColumn : DataGridColumnStyle
{
	private string format;

	private IFormatProvider format_provider;

	private StringFormat string_format = new StringFormat();

	private DataGridTextBox textbox;

	private static readonly int offset_x = 2;

	private static readonly int offset_y = 2;

	[Editor("System.Windows.Forms.Design.DataGridColumnStyleFormatEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DefaultValue(null)]
	public string Format
	{
		get
		{
			return format;
		}
		set
		{
			if (value != format)
			{
				format = value;
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IFormatProvider FormatInfo
	{
		get
		{
			return format_provider;
		}
		set
		{
			if (value != format_provider)
			{
				format_provider = value;
			}
		}
	}

	[DefaultValue(null)]
	public override PropertyDescriptor PropertyDescriptor
	{
		set
		{
			base.PropertyDescriptor = value;
		}
	}

	public override bool ReadOnly
	{
		get
		{
			return base.ReadOnly;
		}
		set
		{
			base.ReadOnly = value;
		}
	}

	[Browsable(false)]
	public virtual TextBox TextBox => textbox;

	public DataGridTextBoxColumn()
		: this(null, string.Empty, isDefault: false)
	{
	}

	public DataGridTextBoxColumn(PropertyDescriptor prop)
		: this(prop, string.Empty, isDefault: false)
	{
	}

	public DataGridTextBoxColumn(PropertyDescriptor prop, bool isDefault)
		: this(prop, string.Empty, isDefault)
	{
	}

	public DataGridTextBoxColumn(PropertyDescriptor prop, string format)
		: this(prop, format, isDefault: false)
	{
	}

	public DataGridTextBoxColumn(PropertyDescriptor prop, string format, bool isDefault)
		: base(prop)
	{
		Format = format;
		is_default = isDefault;
		textbox = new DataGridTextBox();
		textbox.Multiline = true;
		textbox.WordWrap = false;
		textbox.BorderStyle = BorderStyle.None;
		textbox.Visible = false;
	}

	protected internal override void Abort(int rowNum)
	{
		EndEdit();
	}

	protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
	{
		textbox.Bounds = Rectangle.Empty;
		if (textbox.IsInEditOrNavigateMode)
		{
			return true;
		}
		try
		{
			string formattedValue = GetFormattedValue(dataSource, rowNum);
			if (formattedValue != textbox.Text)
			{
				if (textbox.Text == NullText)
				{
					SetColumnValueAtRow(dataSource, rowNum, DBNull.Value);
				}
				else
				{
					object value = textbox.Text;
					TypeConverter converter = TypeDescriptor.GetConverter(PropertyDescriptor.PropertyType);
					if (converter != null && converter.CanConvertFrom(typeof(string)))
					{
						value = converter.ConvertFrom(null, CultureInfo.CurrentCulture, textbox.Text);
						if (converter.CanConvertTo(typeof(string)))
						{
							textbox.Text = (string)converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));
						}
					}
					SetColumnValueAtRow(dataSource, rowNum, value);
				}
			}
		}
		catch
		{
			return false;
		}
		EndEdit();
		return true;
	}

	protected internal override void ConcedeFocus()
	{
		HideEditBox();
	}

	protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
	{
		grid.SuspendLayout();
		textbox.TextChanged -= textbox_TextChanged;
		textbox.TextAlign = alignment;
		bool flag = false;
		flag = base.TableStyleReadOnly || ReadOnly || readOnly;
		if (!flag && displayText != null)
		{
			textbox.Text = displayText;
			textbox.IsInEditOrNavigateMode = false;
		}
		else
		{
			textbox.Text = GetFormattedValue(source, rowNum);
		}
		textbox.TextChanged += textbox_TextChanged;
		textbox.ReadOnly = flag;
		textbox.Bounds = new Rectangle(new Point(bounds.X + offset_x, bounds.Y + offset_y), new Size(bounds.Width - offset_x - 1, bounds.Height - offset_y - 1));
		textbox.Visible = cellIsVisible;
		textbox.SelectAll();
		textbox.Focus();
		grid.ResumeLayout(performLayout: false);
	}

	private void textbox_TextChanged(object o, EventArgs e)
	{
		textbox.IsInEditOrNavigateMode = false;
		grid.EditRowChanged(this);
	}

	protected void EndEdit()
	{
		textbox.TextChanged -= textbox_TextChanged;
		HideEditBox();
	}

	protected internal override void EnterNullValue()
	{
		textbox.Text = NullText;
	}

	protected internal override int GetMinimumHeight()
	{
		return base.FontHeight + 3;
	}

	protected internal override int GetPreferredHeight(Graphics g, object value)
	{
		string formattedValue = GetFormattedValue(value);
		Regex regex = new Regex("/\r\n/");
		int count = regex.Matches(formattedValue).Count;
		return DataGridTableStyle.DataGrid.Font.Height * (count + 1) + 1;
	}

	protected internal override Size GetPreferredSize(Graphics g, object value)
	{
		string formattedValue = GetFormattedValue(value);
		Size result = Size.Ceiling(g.MeasureString(formattedValue, DataGridTableStyle.DataGrid.Font));
		result.Width += 4;
		return result;
	}

	protected void HideEditBox()
	{
		if (textbox.Visible)
		{
			grid.SuspendLayout();
			textbox.Bounds = Rectangle.Empty;
			textbox.Visible = false;
			textbox.IsInEditOrNavigateMode = true;
			grid.ResumeLayout(performLayout: false);
		}
	}

	protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
	{
		Paint(g, bounds, source, rowNum, alignToRight: false);
	}

	protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
	{
		Paint(g, bounds, source, rowNum, ThemeEngine.Current.ResPool.GetSolidBrush(DataGridTableStyle.BackColor), ThemeEngine.Current.ResPool.GetSolidBrush(DataGridTableStyle.ForeColor), alignToRight);
	}

	protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
	{
		PaintText(g, bounds, GetFormattedValue(source, rowNum), backBrush, foreBrush, alignToRight);
	}

	protected void PaintText(Graphics g, Rectangle bounds, string text, bool alignToRight)
	{
		PaintText(g, bounds, text, ThemeEngine.Current.ResPool.GetSolidBrush(DataGridTableStyle.BackColor), ThemeEngine.Current.ResPool.GetSolidBrush(DataGridTableStyle.ForeColor), alignToRight);
	}

	protected void PaintText(Graphics g, Rectangle textBounds, string text, Brush backBrush, Brush foreBrush, bool alignToRight)
	{
		if (alignToRight)
		{
			string_format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
		}
		else
		{
			string_format.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
		}
		switch (alignment)
		{
		case HorizontalAlignment.Center:
			string_format.Alignment = StringAlignment.Center;
			break;
		case HorizontalAlignment.Right:
			string_format.Alignment = StringAlignment.Far;
			break;
		default:
			string_format.Alignment = StringAlignment.Near;
			break;
		}
		g.FillRectangle(backBrush, textBounds);
		PaintGridLine(g, textBounds);
		textBounds.X += offset_x;
		textBounds.Width -= offset_x;
		textBounds.Y += offset_y;
		textBounds.Height -= offset_y;
		string_format.FormatFlags |= StringFormatFlags.NoWrap;
		g.DrawString(text, DataGridTableStyle.DataGrid.Font, foreBrush, textBounds, string_format);
	}

	protected internal override void ReleaseHostedControl()
	{
		if (textbox != null)
		{
			grid.SuspendLayout();
			grid.Controls.Remove(textbox);
			grid.Invalidate(new Rectangle(textbox.Location, textbox.Size));
			textbox.Dispose();
			textbox = null;
			grid.ResumeLayout(performLayout: false);
		}
	}

	protected override void SetDataGridInColumn(DataGrid value)
	{
		base.SetDataGridInColumn(value);
		if (value != null)
		{
			textbox.SetDataGrid(grid);
			grid.SuspendLayout();
			grid.Controls.Add(textbox);
			grid.ResumeLayout(performLayout: false);
		}
	}

	protected internal override void UpdateUI(CurrencyManager source, int rowNum, string displayText)
	{
		if (textbox.Visible && textbox.IsInEditOrNavigateMode)
		{
			textbox.Text = GetFormattedValue(source, rowNum);
		}
		else
		{
			textbox.Text = displayText;
		}
	}

	private string GetFormattedValue(CurrencyManager source, int rowNum)
	{
		object columnValueAtRow = GetColumnValueAtRow(source, rowNum);
		return GetFormattedValue(columnValueAtRow);
	}

	private string GetFormattedValue(object obj)
	{
		if (DBNull.Value.Equals(obj) || obj == null)
		{
			return NullText;
		}
		if (format != null && format != string.Empty && obj is IFormattable)
		{
			return ((IFormattable)obj).ToString(format, format_provider);
		}
		TypeConverter converter = TypeDescriptor.GetConverter(PropertyDescriptor.PropertyType);
		if (converter != null && converter.CanConvertTo(typeof(string)))
		{
			return (string)converter.ConvertTo(null, CultureInfo.CurrentCulture, obj, typeof(string));
		}
		return obj.ToString();
	}
}
