using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public class DataGridBoolColumn : DataGridColumnStyle
{
	[Flags]
	private enum CheckState
	{
		Checked = 1,
		UnChecked = 2,
		Null = 4,
		Selected = 8
	}

	private bool allow_null;

	private object false_value;

	private object null_value;

	private object true_value;

	private int editing_row;

	private CheckState editing_state;

	private CheckState model_state;

	private Size checkbox_size;

	private static object AllowNullChangedEvent;

	private static object FalseValueChangedEvent;

	private static object TrueValueChangedEvent;

	[DefaultValue(true)]
	public bool AllowNull
	{
		get
		{
			return allow_null;
		}
		set
		{
			if (value != allow_null)
			{
				allow_null = value;
				((EventHandler)base.Events[AllowNullChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	[DefaultValue(false)]
	[TypeConverter(typeof(StringConverter))]
	public object FalseValue
	{
		get
		{
			return false_value;
		}
		set
		{
			if (value != false_value)
			{
				false_value = value;
				((EventHandler)base.Events[FalseValueChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	[TypeConverter(typeof(StringConverter))]
	public object NullValue
	{
		get
		{
			return null_value;
		}
		set
		{
			if (value != null_value)
			{
				null_value = value;
			}
		}
	}

	[DefaultValue(true)]
	[TypeConverter(typeof(StringConverter))]
	public object TrueValue
	{
		get
		{
			return true_value;
		}
		set
		{
			if (value != true_value)
			{
				true_value = value;
				((EventHandler)base.Events[TrueValueChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public event EventHandler AllowNullChanged
	{
		add
		{
			base.Events.AddHandler(AllowNullChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AllowNullChangedEvent, value);
		}
	}

	public event EventHandler FalseValueChanged
	{
		add
		{
			base.Events.AddHandler(FalseValueChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(FalseValueChangedEvent, value);
		}
	}

	public event EventHandler TrueValueChanged
	{
		add
		{
			base.Events.AddHandler(TrueValueChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(TrueValueChangedEvent, value);
		}
	}

	public DataGridBoolColumn()
		: this(null, isDefault: false)
	{
	}

	public DataGridBoolColumn(PropertyDescriptor prop)
		: this(prop, isDefault: false)
	{
	}

	public DataGridBoolColumn(PropertyDescriptor prop, bool isDefault)
		: base(prop)
	{
		false_value = false;
		null_value = null;
		true_value = true;
		allow_null = true;
		is_default = isDefault;
		checkbox_size = new Size(ThemeEngine.Current.DataGridMinimumColumnCheckBoxWidth, ThemeEngine.Current.DataGridMinimumColumnCheckBoxHeight);
	}

	static DataGridBoolColumn()
	{
		AllowNullChanged = new object();
		FalseValueChanged = new object();
		TrueValueChanged = new object();
	}

	protected internal override void Abort(int rowNum)
	{
		if (rowNum == editing_row)
		{
			grid.Invalidate(grid.GetCurrentCellBounds());
			editing_row = -1;
		}
	}

	protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
	{
		if (rowNum == editing_row)
		{
			SetColumnValueAtRow(dataSource, rowNum, FromStateToValue(editing_state));
			grid.Invalidate(grid.GetCurrentCellBounds());
			editing_row = -1;
		}
		return true;
	}

	[System.MonoTODO("Stub, does nothing")]
	protected internal override void ConcedeFocus()
	{
		base.ConcedeFocus();
	}

	protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible)
	{
		editing_row = rowNum;
		model_state = FromValueToState(GetColumnValueAtRow(source, rowNum));
		editing_state = model_state | CheckState.Selected;
		grid.Invalidate(grid.GetCurrentCellBounds());
	}

	[System.MonoTODO("Stub, does nothing")]
	protected internal override void EnterNullValue()
	{
		base.EnterNullValue();
	}

	private bool ValueEquals(object value, object obj)
	{
		return value?.Equals(obj) ?? (obj == null);
	}

	protected internal override object GetColumnValueAtRow(CurrencyManager lm, int row)
	{
		object columnValueAtRow = base.GetColumnValueAtRow(lm, row);
		if (ValueEquals(DBNull.Value, columnValueAtRow))
		{
			return null_value;
		}
		if (ValueEquals(true, columnValueAtRow))
		{
			return true_value;
		}
		return false_value;
	}

	protected internal override int GetMinimumHeight()
	{
		return checkbox_size.Height;
	}

	protected internal override int GetPreferredHeight(Graphics g, object value)
	{
		return checkbox_size.Height;
	}

	protected internal override Size GetPreferredSize(Graphics g, object value)
	{
		return checkbox_size;
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
		Rectangle rectangle = default(Rectangle);
		CheckState checkState = ((rowNum != editing_row) ? FromValueToState(GetColumnValueAtRow(source, rowNum)) : editing_state);
		rectangle.X = bounds.X + (bounds.Width - checkbox_size.Width - 2) / 2;
		rectangle.Y = bounds.Y + (bounds.Height - checkbox_size.Height - 2) / 2;
		rectangle.Width = checkbox_size.Width - 2;
		rectangle.Height = checkbox_size.Height - 2;
		if ((checkState & CheckState.Selected) == CheckState.Selected)
		{
			backBrush = ThemeEngine.Current.ResPool.GetSolidBrush(grid.SelectionBackColor);
			checkState &= ~CheckState.Selected;
		}
		g.FillRectangle(backBrush, bounds);
		ThemeEngine.Current.CPDrawCheckBox(g, rectangle, checkState switch
		{
			CheckState.Checked => ButtonState.Checked, 
			CheckState.Null => ButtonState.Inactive | ButtonState.Checked, 
			_ => ButtonState.Normal, 
		});
		PaintGridLine(g, bounds);
	}

	protected internal override void SetColumnValueAtRow(CurrencyManager lm, int row, object value)
	{
		object value2 = null;
		if (ValueEquals(null_value, value))
		{
			value2 = DBNull.Value;
		}
		else if (ValueEquals(true_value, value))
		{
			value2 = true;
		}
		else if (ValueEquals(false_value, value))
		{
			value2 = false;
		}
		base.SetColumnValueAtRow(lm, row, value2);
	}

	private object FromStateToValue(CheckState state)
	{
		if ((state & CheckState.Checked) == CheckState.Checked)
		{
			return true_value;
		}
		if ((state & CheckState.Null) == CheckState.Null)
		{
			return null_value;
		}
		return false_value;
	}

	private CheckState FromValueToState(object obj)
	{
		if (ValueEquals(true_value, obj))
		{
			return CheckState.Checked;
		}
		if (ValueEquals(null_value, obj))
		{
			return CheckState.Null;
		}
		return CheckState.UnChecked;
	}

	private CheckState GetNextState(CheckState state)
	{
		return ((state & ~CheckState.Selected) switch
		{
			CheckState.Checked => (!AllowNull) ? CheckState.UnChecked : CheckState.Null, 
			CheckState.Null => CheckState.UnChecked, 
			_ => CheckState.Checked, 
		}) | (state & CheckState.Selected);
	}

	internal override void OnKeyDown(KeyEventArgs ke, int row, int column)
	{
		Keys keyCode = ke.KeyCode;
		if (keyCode == Keys.Space)
		{
			NextState(row, column);
		}
	}

	internal override void OnMouseDown(MouseEventArgs e, int row, int column)
	{
		NextState(row, column);
	}

	private void NextState(int row, int column)
	{
		grid.ColumnStartedEditing(default(Rectangle));
		editing_state = GetNextState(editing_state);
		grid.Invalidate(grid.GetCellBounds(row, column));
	}
}
