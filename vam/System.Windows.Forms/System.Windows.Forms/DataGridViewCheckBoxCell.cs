using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public class DataGridViewCheckBoxCell : DataGridViewCell, IDataGridViewEditingCell
{
	protected class DataGridViewCheckBoxCellAccessibleObject : DataGridViewCellAccessibleObject
	{
		public override string DefaultAction
		{
			get
			{
				if (base.Owner.ReadOnly)
				{
					return string.Empty;
				}
				throw new NotImplementedException();
			}
		}

		public DataGridViewCheckBoxCellAccessibleObject(DataGridViewCell owner)
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

	private object editingCellFormattedValue;

	private bool editingCellValueChanged;

	private object falseValue;

	private FlatStyle flatStyle;

	private object indeterminateValue;

	private bool threeState;

	private object trueValue;

	private PushButtonState check_state;

	public virtual object EditingCellFormattedValue
	{
		get
		{
			return editingCellFormattedValue;
		}
		set
		{
			if (FormattedValueType == null || value == null || value.GetType() != FormattedValueType || !(value is bool) || !(value is CheckState))
			{
				throw new ArgumentException("Cannot set this property.");
			}
			editingCellFormattedValue = value;
		}
	}

	public virtual bool EditingCellValueChanged
	{
		get
		{
			return editingCellValueChanged;
		}
		set
		{
			editingCellValueChanged = value;
		}
	}

	public override Type EditType => null;

	[DefaultValue(null)]
	public object FalseValue
	{
		get
		{
			return falseValue;
		}
		set
		{
			falseValue = value;
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
			if (!Enum.IsDefined(typeof(FlatStyle), value))
			{
				throw new InvalidEnumArgumentException("Value is not valid FlatStyle.");
			}
			if (value == FlatStyle.Popup)
			{
				throw new Exception("FlatStyle cannot be set to Popup in this control.");
			}
		}
	}

	public override Type FormattedValueType
	{
		get
		{
			if (ThreeState)
			{
				return typeof(CheckState);
			}
			return typeof(bool);
		}
	}

	[DefaultValue(null)]
	public object IndeterminateValue
	{
		get
		{
			return indeterminateValue;
		}
		set
		{
			indeterminateValue = value;
		}
	}

	[DefaultValue(false)]
	public bool ThreeState
	{
		get
		{
			return threeState;
		}
		set
		{
			threeState = value;
		}
	}

	[DefaultValue(null)]
	public object TrueValue
	{
		get
		{
			return trueValue;
		}
		set
		{
			trueValue = value;
		}
	}

	public override Type ValueType
	{
		get
		{
			if (base.ValueType == null)
			{
				if (base.OwningColumn != null && base.OwningColumn.ValueType != null)
				{
					return base.OwningColumn.ValueType;
				}
				if (ThreeState)
				{
					return typeof(CheckState);
				}
				return typeof(bool);
			}
			return base.ValueType;
		}
		set
		{
			base.ValueType = value;
		}
	}

	public DataGridViewCheckBoxCell()
	{
		check_state = PushButtonState.Normal;
		editingCellFormattedValue = false;
		editingCellValueChanged = false;
		falseValue = null;
		flatStyle = FlatStyle.Standard;
		indeterminateValue = null;
		threeState = false;
		trueValue = null;
		ValueType = null;
	}

	public DataGridViewCheckBoxCell(bool threeState)
		: this()
	{
		this.threeState = threeState;
		editingCellFormattedValue = CheckState.Unchecked;
	}

	public override object Clone()
	{
		DataGridViewCheckBoxCell dataGridViewCheckBoxCell = (DataGridViewCheckBoxCell)base.Clone();
		dataGridViewCheckBoxCell.editingCellValueChanged = editingCellValueChanged;
		dataGridViewCheckBoxCell.editingCellFormattedValue = editingCellFormattedValue;
		dataGridViewCheckBoxCell.falseValue = falseValue;
		dataGridViewCheckBoxCell.flatStyle = flatStyle;
		dataGridViewCheckBoxCell.indeterminateValue = indeterminateValue;
		dataGridViewCheckBoxCell.threeState = threeState;
		dataGridViewCheckBoxCell.trueValue = trueValue;
		dataGridViewCheckBoxCell.ValueType = ValueType;
		return dataGridViewCheckBoxCell;
	}

	public virtual object GetEditingCellFormattedValue(DataGridViewDataErrorContexts context)
	{
		if (FormattedValueType == null)
		{
			throw new InvalidOperationException("FormattedValueType is null.");
		}
		if ((context & DataGridViewDataErrorContexts.ClipboardContent) != 0)
		{
			return Convert.ToString(base.Value);
		}
		if (editingCellFormattedValue == null)
		{
			if (threeState)
			{
				return CheckState.Indeterminate;
			}
			return false;
		}
		return editingCellFormattedValue;
	}

	public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
	{
		if (cellStyle == null)
		{
			throw new ArgumentNullException("CellStyle is null");
		}
		if (FormattedValueType == null)
		{
			throw new FormatException("FormattedValueType is null.");
		}
		if (formattedValue == null || formattedValue.GetType() != FormattedValueType)
		{
			throw new ArgumentException("FormattedValue is null or is not instance of FormattedValueType.");
		}
		return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
	}

	public virtual void PrepareEditingCellForEdit(bool selectAll)
	{
		editingCellFormattedValue = GetCurrentValue();
	}

	public override string ToString()
	{
		return $"DataGridViewCheckBoxCell {{ ColumnIndex={base.ColumnIndex}, RowIndex={base.RowIndex} }}";
	}

	protected override bool ContentClickUnsharesRow(DataGridViewCellEventArgs e)
	{
		return base.IsInEditMode;
	}

	protected override bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e)
	{
		return base.IsInEditMode;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new DataGridViewCheckBoxCellAccessibleObject(this);
	}

	protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return Rectangle.Empty;
		}
		return new Rectangle((base.Size.Width - 13) / 2, (base.Size.Height - 13) / 2, 13, 13);
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
		if (base.DataGridView == null || value == null)
		{
			if (threeState)
			{
				return CheckState.Indeterminate;
			}
			return false;
		}
		return value;
	}

	protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
	{
		return new Size(21, 20);
	}

	protected override bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
	{
		return e.KeyData == Keys.Space;
	}

	protected override bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
	{
		return e.KeyData == Keys.Space;
	}

	protected override bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return e.Button == MouseButtons.Left;
	}

	protected override bool MouseEnterUnsharesRow(int rowIndex)
	{
		return false;
	}

	protected override bool MouseLeaveUnsharesRow(int rowIndex)
	{
		return check_state == PushButtonState.Pressed;
	}

	protected override bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return e.Button == MouseButtons.Left;
	}

	protected override void OnContentClick(DataGridViewCellEventArgs e)
	{
		if (ReadOnly)
		{
			return;
		}
		if (!base.IsInEditMode)
		{
			base.DataGridView.BeginEdit(selectAll: false);
		}
		CheckState currentValue = GetCurrentValue();
		if (threeState)
		{
			switch (currentValue)
			{
			case CheckState.Indeterminate:
				editingCellFormattedValue = CheckState.Unchecked;
				break;
			case CheckState.Checked:
				editingCellFormattedValue = CheckState.Indeterminate;
				break;
			default:
				editingCellFormattedValue = CheckState.Checked;
				break;
			}
		}
		else if (currentValue == CheckState.Checked)
		{
			editingCellFormattedValue = false;
		}
		else
		{
			editingCellFormattedValue = true;
		}
		editingCellValueChanged = true;
	}

	protected override void OnContentDoubleClick(DataGridViewCellEventArgs e)
	{
	}

	protected override void OnKeyDown(KeyEventArgs e, int rowIndex)
	{
		if (!ReadOnly && (e.KeyData & Keys.Space) == Keys.Space)
		{
			check_state = PushButtonState.Pressed;
			base.DataGridView.InvalidateCell(this);
		}
	}

	protected override void OnKeyUp(KeyEventArgs e, int rowIndex)
	{
		if (!ReadOnly && (e.KeyData & Keys.Space) == Keys.Space)
		{
			check_state = PushButtonState.Normal;
			base.DataGridView.InvalidateCell(this);
		}
	}

	protected override void OnLeave(int rowIndex, bool throughMouseClick)
	{
		if (!ReadOnly && check_state != PushButtonState.Normal)
		{
			check_state = PushButtonState.Normal;
			base.DataGridView.InvalidateCell(this);
		}
	}

	protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
	{
		if (!ReadOnly && (e.Button & MouseButtons.Left) == MouseButtons.Left)
		{
			check_state = PushButtonState.Pressed;
			base.DataGridView.InvalidateCell(this);
		}
	}

	protected override void OnMouseLeave(int rowIndex)
	{
		if (!ReadOnly && check_state != PushButtonState.Normal)
		{
			check_state = PushButtonState.Normal;
			base.DataGridView.InvalidateCell(this);
		}
	}

	protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
	{
		if (!ReadOnly && check_state != PushButtonState.Normal && check_state != PushButtonState.Hot)
		{
			check_state = PushButtonState.Hot;
			base.DataGridView.InvalidateCell(this);
		}
	}

	protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
	{
		if (!ReadOnly && (e.Button & MouseButtons.Left) == MouseButtons.Left)
		{
			check_state = PushButtonState.Normal;
			base.DataGridView.InvalidateCell(this);
		}
	}

	protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
	}

	internal override void PaintPartContent(Graphics graphics, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle, object formattedValue)
	{
		CheckBoxState checkBoxState = GetCurrentValue() switch
		{
			CheckState.Unchecked => (CheckBoxState)check_state, 
			CheckState.Checked => (CheckBoxState)(check_state + 4), 
			_ => (CheckBoxState)((!threeState) ? check_state : (check_state + 8)), 
		};
		Point glyphLocation = new Point(cellBounds.X + (base.Size.Width - 13) / 2, cellBounds.Y + (base.Size.Height - 13) / 2);
		CheckBoxRenderer.DrawCheckBox(graphics, glyphLocation, checkBoxState);
	}

	private CheckState GetCurrentValue()
	{
		CheckState result = CheckState.Indeterminate;
		object obj = ((!editingCellValueChanged) ? base.Value : editingCellFormattedValue);
		if (obj == null)
		{
			result = CheckState.Indeterminate;
		}
		else if (obj is bool)
		{
			if ((bool)obj)
			{
				result = CheckState.Checked;
			}
			else if (!(bool)obj)
			{
				result = CheckState.Unchecked;
			}
		}
		else if (obj is CheckState)
		{
			result = (CheckState)(int)obj;
		}
		return result;
	}
}
