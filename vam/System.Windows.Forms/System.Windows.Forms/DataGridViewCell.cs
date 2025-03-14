using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[TypeConverter(typeof(DataGridViewCellConverter))]
public abstract class DataGridViewCell : DataGridViewElement, IDisposable, ICloneable
{
	[ComVisible(true)]
	protected class DataGridViewCellAccessibleObject : AccessibleObject
	{
		private DataGridViewCell dataGridViewCell;

		public override Rectangle Bounds
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string DefaultAction => "Edit";

		public override string Help => base.Help;

		public override string Name => dataGridViewCell.OwningColumn.HeaderText + ": " + dataGridViewCell.RowIndex;

		public DataGridViewCell Owner
		{
			get
			{
				return dataGridViewCell;
			}
			set
			{
				dataGridViewCell = value;
			}
		}

		public override AccessibleObject Parent => dataGridViewCell.OwningRow.AccessibilityObject;

		public override AccessibleRole Role => AccessibleRole.Cell;

		public override AccessibleStates State
		{
			get
			{
				if (dataGridViewCell.Selected)
				{
					return AccessibleStates.Selected;
				}
				return AccessibleStates.Focused;
			}
		}

		public override string Value
		{
			get
			{
				if (dataGridViewCell.FormattedValue == null)
				{
					return "(null)";
				}
				return dataGridViewCell.FormattedValue.ToString();
			}
			set
			{
				if (owner == null)
				{
					throw new InvalidOperationException("owner is null");
				}
				throw new NotImplementedException();
			}
		}

		public DataGridViewCellAccessibleObject()
		{
		}

		public DataGridViewCellAccessibleObject(DataGridViewCell owner)
		{
			dataGridViewCell = owner;
		}

		public override void DoDefaultAction()
		{
			if (dataGridViewCell.DataGridView.EditMode != DataGridViewEditMode.EditProgrammatically && !dataGridViewCell.IsInEditMode)
			{
			}
		}

		public override AccessibleObject GetChild(int index)
		{
			throw new NotImplementedException();
		}

		public override int GetChildCount()
		{
			if (dataGridViewCell.IsInEditMode)
			{
				return 1;
			}
			return -1;
		}

		public override AccessibleObject GetFocused()
		{
			return null;
		}

		public override AccessibleObject GetSelected()
		{
			return null;
		}

		public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
		{
			switch (navigationDirection)
			{
			default:
				return null;
			case AccessibleNavigation.Up:
			case AccessibleNavigation.Down:
			case AccessibleNavigation.Left:
			case AccessibleNavigation.Right:
			case AccessibleNavigation.Next:
			case AccessibleNavigation.Previous:
				return null;
			}
		}

		public override void Select(AccessibleSelection flags)
		{
			switch (flags)
			{
			case AccessibleSelection.TakeFocus:
				dataGridViewCell.dataGridViewOwner.Focus();
				break;
			case AccessibleSelection.AddSelection:
				dataGridViewCell.dataGridViewOwner.SelectedCells.InternalAdd(dataGridViewCell);
				break;
			case AccessibleSelection.RemoveSelection:
				dataGridViewCell.dataGridViewOwner.SelectedCells.InternalRemove(dataGridViewCell);
				break;
			}
		}
	}

	private DataGridView dataGridViewOwner;

	private AccessibleObject accessibilityObject;

	private int columnIndex;

	private ContextMenuStrip contextMenuStrip;

	private bool displayed;

	private string errorText;

	private bool isInEditMode;

	private DataGridViewRow owningRow;

	private DataGridViewTriState readOnly;

	private bool selected;

	private DataGridViewCellStyle style;

	private object tag;

	private string toolTipText;

	private object valuex;

	private Type valueType;

	[Browsable(false)]
	public AccessibleObject AccessibilityObject
	{
		get
		{
			if (accessibilityObject == null)
			{
				accessibilityObject = CreateAccessibilityInstance();
			}
			return accessibilityObject;
		}
	}

	public int ColumnIndex
	{
		get
		{
			if (base.DataGridView == null)
			{
				return -1;
			}
			return columnIndex;
		}
	}

	[Browsable(false)]
	public Rectangle ContentBounds => GetContentBounds(RowIndex);

	[DefaultValue(null)]
	public virtual ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return contextMenuStrip;
		}
		set
		{
			contextMenuStrip = value;
		}
	}

	[Browsable(false)]
	public virtual object DefaultNewRowValue => null;

	[Browsable(false)]
	public virtual bool Displayed => displayed;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public object EditedFormattedValue => GetEditedFormattedValue(RowIndex, DataGridViewDataErrorContexts.Formatting);

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public virtual Type EditType => typeof(DataGridViewTextBoxEditingControl);

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public Rectangle ErrorIconBounds
	{
		get
		{
			if (this is DataGridViewTopLeftHeaderCell)
			{
				return GetErrorIconBounds(null, null, RowIndex);
			}
			if (base.DataGridView == null || columnIndex < 0)
			{
				throw new InvalidOperationException();
			}
			if (RowIndex < 0 || RowIndex >= base.DataGridView.Rows.Count)
			{
				throw new ArgumentOutOfRangeException("rowIndex", "Specified argument was out of the range of valid values.");
			}
			return GetErrorIconBounds(null, null, RowIndex);
		}
	}

	[Browsable(false)]
	public string ErrorText
	{
		get
		{
			if (this is DataGridViewTopLeftHeaderCell)
			{
				return GetErrorText(-1);
			}
			if (OwningRow == null)
			{
				return string.Empty;
			}
			return GetErrorText(OwningRow.Index);
		}
		set
		{
			if (errorText != value)
			{
				errorText = value;
				OnErrorTextChanged(new DataGridViewCellEventArgs(ColumnIndex, RowIndex));
			}
		}
	}

	[Browsable(false)]
	public object FormattedValue
	{
		get
		{
			if (base.DataGridView == null)
			{
				return null;
			}
			DataGridViewCellStyle cellStyle = InheritedStyle;
			return GetFormattedValue(Value, RowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);
		}
	}

	[Browsable(false)]
	public virtual Type FormattedValueType => null;

	[Browsable(false)]
	public virtual bool Frozen
	{
		get
		{
			if (base.DataGridView == null)
			{
				return false;
			}
			if (RowIndex >= 0)
			{
				return OwningRow.Frozen && OwningColumn.Frozen;
			}
			return false;
		}
	}

	[Browsable(false)]
	public bool HasStyle => style != null;

	[Browsable(false)]
	public DataGridViewElementStates InheritedState => GetInheritedState(RowIndex);

	[Browsable(false)]
	public DataGridViewCellStyle InheritedStyle => GetInheritedStyle(null, RowIndex, includeColors: true);

	[Browsable(false)]
	public bool IsInEditMode
	{
		get
		{
			if (base.DataGridView == null)
			{
				return false;
			}
			if (RowIndex == -1)
			{
				throw new InvalidOperationException("Operation cannot be performed on a cell of a shared row.");
			}
			return isInEditMode;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public DataGridViewColumn OwningColumn
	{
		get
		{
			if (base.DataGridView == null || columnIndex < 0 || columnIndex >= base.DataGridView.Columns.Count)
			{
				return null;
			}
			return base.DataGridView.Columns[columnIndex];
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public DataGridViewRow OwningRow => owningRow;

	[Browsable(false)]
	public Size PreferredSize
	{
		get
		{
			if (base.DataGridView == null)
			{
				return new Size(-1, -1);
			}
			return GetPreferredSize(Hwnd.GraphicsContext, InheritedStyle, RowIndex, Size.Empty);
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual bool ReadOnly
	{
		get
		{
			if (base.DataGridView != null && base.DataGridView.ReadOnly)
			{
				return true;
			}
			if (readOnly != 0)
			{
				return readOnly == DataGridViewTriState.True;
			}
			if (OwningRow != null && !OwningRow.IsShared && OwningRow.ReadOnly)
			{
				return true;
			}
			if (OwningColumn != null && OwningColumn.ReadOnly)
			{
				return true;
			}
			return false;
		}
		set
		{
			readOnly = (value ? DataGridViewTriState.True : DataGridViewTriState.False);
			if (value)
			{
				SetState(DataGridViewElementStates.ReadOnly | State);
			}
			else
			{
				SetState(~DataGridViewElementStates.ReadOnly & State);
			}
		}
	}

	[Browsable(false)]
	public virtual bool Resizable
	{
		get
		{
			if (base.DataGridView == null)
			{
				return false;
			}
			if (RowIndex == -1 || columnIndex == -1)
			{
				return false;
			}
			return OwningRow.Resizable == DataGridViewTriState.True || OwningColumn.Resizable == DataGridViewTriState.True;
		}
	}

	[Browsable(false)]
	public int RowIndex
	{
		get
		{
			if (owningRow == null)
			{
				return -1;
			}
			return owningRow.Index;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual bool Selected
	{
		get
		{
			if (selected)
			{
				return true;
			}
			if (base.DataGridView != null)
			{
				if (RowIndex >= 0 && RowIndex < base.DataGridView.Rows.Count && base.DataGridView.Rows[RowIndex].Selected)
				{
					return true;
				}
				if (ColumnIndex >= 0 && ColumnIndex < base.DataGridView.Columns.Count && base.DataGridView.Columns[ColumnIndex].Selected)
				{
					return true;
				}
			}
			return false;
		}
		set
		{
			bool flag = selected != value;
			selected = value;
			if (value != ((State & DataGridViewElementStates.Selected) != 0))
			{
				SetState(State ^ DataGridViewElementStates.Selected);
			}
			if (!selected && OwningRow != null && OwningRow.Selected)
			{
				OwningRow.Selected = false;
				if (columnIndex != 0 && OwningRow.Cells.Count > 0)
				{
					OwningRow.Cells[0].Selected = true;
				}
				else if (OwningRow.Cells.Count > 1)
				{
					OwningRow.Cells[1].Selected = true;
				}
			}
			if (flag && base.DataGridView != null && base.DataGridView.IsHandleCreated)
			{
				base.DataGridView.Invalidate();
			}
		}
	}

	[Browsable(false)]
	public Size Size
	{
		get
		{
			if (base.DataGridView == null)
			{
				return new Size(-1, -1);
			}
			return GetSize(RowIndex);
		}
	}

	[Browsable(true)]
	public DataGridViewCellStyle Style
	{
		get
		{
			if (style == null)
			{
				style = new DataGridViewCellStyle();
				style.StyleChanged += OnStyleChanged;
			}
			return style;
		}
		set
		{
			style = value;
		}
	}

	[Localizable(false)]
	[TypeConverter("System.ComponentModel.StringConverter, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	[Bindable(true, BindingDirection.OneWay)]
	[DefaultValue(null)]
	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string ToolTipText
	{
		get
		{
			return (toolTipText != null) ? toolTipText : string.Empty;
		}
		set
		{
			toolTipText = value;
		}
	}

	[Browsable(false)]
	public object Value
	{
		get
		{
			return GetValue(RowIndex);
		}
		set
		{
			SetValue(RowIndex, value);
		}
	}

	[Browsable(false)]
	public virtual Type ValueType
	{
		get
		{
			if (valueType == null)
			{
				if (DataProperty != null)
				{
					valueType = DataProperty.PropertyType;
				}
				else if (OwningColumn != null)
				{
					valueType = OwningColumn.ValueType;
				}
			}
			return valueType;
		}
		set
		{
			valueType = value;
		}
	}

	[Browsable(false)]
	public virtual bool Visible
	{
		get
		{
			DataGridViewColumn owningColumn = OwningColumn;
			DataGridViewRow dataGridViewRow = OwningRow;
			bool flag = true;
			bool flag2 = true;
			if (dataGridViewRow == null && owningColumn == null)
			{
				return false;
			}
			if (dataGridViewRow != null)
			{
				flag = !dataGridViewRow.IsShared && dataGridViewRow.Visible;
			}
			if (owningColumn != null)
			{
				flag2 = owningColumn.Index >= 0 && owningColumn.Visible;
			}
			return flag && flag2;
		}
	}

	private PropertyDescriptor DataProperty
	{
		get
		{
			if (OwningColumn != null && OwningColumn.DataColumnIndex != -1 && base.DataGridView != null && base.DataGridView.DataManager != null)
			{
				return base.DataGridView.DataManager.GetItemProperties()[OwningColumn.DataColumnIndex];
			}
			return null;
		}
	}

	private TypeConverter FormattedValueTypeConverter
	{
		get
		{
			if (FormattedValueType != null)
			{
				return TypeDescriptor.GetConverter(FormattedValueType);
			}
			return null;
		}
	}

	private TypeConverter ValueTypeConverter
	{
		get
		{
			if (DataProperty != null && DataProperty.Converter != null)
			{
				return DataProperty.Converter;
			}
			if (Value != null)
			{
				return TypeDescriptor.GetConverter(Value);
			}
			if (ValueType != null)
			{
				return TypeDescriptor.GetConverter(ValueType);
			}
			return null;
		}
	}

	internal virtual Rectangle InternalErrorIconsBounds => GetErrorIconBounds(null, null, -1);

	protected DataGridViewCell()
	{
		columnIndex = -1;
		dataGridViewOwner = null;
		errorText = string.Empty;
	}

	~DataGridViewCell()
	{
		Dispose(disposing: false);
	}

	internal override void SetState(DataGridViewElementStates state)
	{
		base.SetState(state);
		if (base.DataGridView != null)
		{
			base.DataGridView.OnCellStateChangedInternal(new DataGridViewCellStateChangedEventArgs(this, state));
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual DataGridViewAdvancedBorderStyle AdjustCellBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput, DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
	{
		return dataGridViewAdvancedBorderStyleInput;
	}

	public virtual object Clone()
	{
		DataGridViewCell dataGridViewCell = (DataGridViewCell)Activator.CreateInstance(GetType());
		dataGridViewCell.accessibilityObject = accessibilityObject;
		dataGridViewCell.columnIndex = columnIndex;
		dataGridViewCell.displayed = displayed;
		dataGridViewCell.errorText = errorText;
		dataGridViewCell.isInEditMode = isInEditMode;
		dataGridViewCell.owningRow = owningRow;
		dataGridViewCell.readOnly = readOnly;
		dataGridViewCell.selected = selected;
		dataGridViewCell.style = style;
		dataGridViewCell.tag = tag;
		dataGridViewCell.toolTipText = toolTipText;
		dataGridViewCell.valuex = valuex;
		dataGridViewCell.valueType = valueType;
		return dataGridViewCell;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual void DetachEditingControl()
	{
	}

	public void Dispose()
	{
	}

	public Rectangle GetContentBounds(int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return Rectangle.Empty;
		}
		return GetContentBounds(Hwnd.GraphicsContext, InheritedStyle, rowIndex);
	}

	public object GetEditedFormattedValue(int rowIndex, DataGridViewDataErrorContexts context)
	{
		if (base.DataGridView == null)
		{
			return null;
		}
		if (rowIndex < 0 || rowIndex >= base.DataGridView.RowCount)
		{
			throw new ArgumentOutOfRangeException("rowIndex", "Specified argument was out of the range of valid values.");
		}
		if (IsInEditMode)
		{
			if (base.DataGridView.EditingControl != null)
			{
				return (base.DataGridView.EditingControl as IDataGridViewEditingControl).GetEditingControlFormattedValue(context);
			}
			return (this as IDataGridViewEditingCell).GetEditingCellFormattedValue(context);
		}
		DataGridViewCellStyle cellStyle = InheritedStyle;
		return GetFormattedValue(GetValue(rowIndex), rowIndex, ref cellStyle, null, null, context);
	}

	public virtual ContextMenuStrip GetInheritedContextMenuStrip(int rowIndex)
	{
		if (base.DataGridView == null)
		{
			return null;
		}
		if (rowIndex < 0 || rowIndex >= base.DataGridView.Rows.Count)
		{
			throw new ArgumentOutOfRangeException("rowIndex");
		}
		if (columnIndex < 0)
		{
			throw new InvalidOperationException("cannot perform this on a column header cell");
		}
		if (contextMenuStrip != null)
		{
			return contextMenuStrip;
		}
		if (OwningRow.ContextMenuStrip != null)
		{
			return OwningRow.ContextMenuStrip;
		}
		if (OwningColumn.ContextMenuStrip != null)
		{
			return OwningColumn.ContextMenuStrip;
		}
		return base.DataGridView.ContextMenuStrip;
	}

	public virtual DataGridViewElementStates GetInheritedState(int rowIndex)
	{
		if (base.DataGridView == null && rowIndex != -1)
		{
			throw new ArgumentException("msg?");
		}
		if (base.DataGridView != null && (rowIndex < 0 || rowIndex >= base.DataGridView.Rows.Count))
		{
			throw new ArgumentOutOfRangeException("rowIndex", "Specified argument was out of the range of valid values.");
		}
		DataGridViewElementStates dataGridViewElementStates = DataGridViewElementStates.ResizableSet | State;
		DataGridViewColumn owningColumn = OwningColumn;
		DataGridViewRow dataGridViewRow = OwningRow;
		if (base.DataGridView == null)
		{
			if (dataGridViewRow != null)
			{
				if (dataGridViewRow.Resizable == DataGridViewTriState.True)
				{
					dataGridViewElementStates |= DataGridViewElementStates.Resizable;
				}
				if (dataGridViewRow.Visible)
				{
					dataGridViewElementStates |= DataGridViewElementStates.Visible;
				}
				if (dataGridViewRow.ReadOnly)
				{
					dataGridViewElementStates |= DataGridViewElementStates.ReadOnly;
				}
				if (dataGridViewRow.Frozen)
				{
					dataGridViewElementStates |= DataGridViewElementStates.Frozen;
				}
				if (dataGridViewRow.Displayed)
				{
					dataGridViewElementStates |= DataGridViewElementStates.Displayed;
				}
				if (dataGridViewRow.Selected)
				{
					dataGridViewElementStates |= DataGridViewElementStates.Selected;
				}
			}
			return dataGridViewElementStates;
		}
		if (owningColumn != null)
		{
			if (owningColumn.Resizable == DataGridViewTriState.True && dataGridViewRow.Resizable == DataGridViewTriState.True)
			{
				dataGridViewElementStates |= DataGridViewElementStates.Resizable;
			}
			if (owningColumn.Visible && dataGridViewRow.Visible)
			{
				dataGridViewElementStates |= DataGridViewElementStates.Visible;
			}
			if (owningColumn.ReadOnly || dataGridViewRow.ReadOnly)
			{
				dataGridViewElementStates |= DataGridViewElementStates.ReadOnly;
			}
			if (owningColumn.Frozen || dataGridViewRow.Frozen)
			{
				dataGridViewElementStates |= DataGridViewElementStates.Frozen;
			}
			if (owningColumn.Displayed && dataGridViewRow.Displayed)
			{
				dataGridViewElementStates |= DataGridViewElementStates.Displayed;
			}
			if (owningColumn.Selected || dataGridViewRow.Selected)
			{
				dataGridViewElementStates |= DataGridViewElementStates.Selected;
			}
		}
		return dataGridViewElementStates;
	}

	public virtual DataGridViewCellStyle GetInheritedStyle(DataGridViewCellStyle inheritedCellStyle, int rowIndex, bool includeColors)
	{
		if (base.DataGridView == null)
		{
			throw new InvalidOperationException("Cell is not in a DataGridView. The cell cannot retrieve the inherited cell style.");
		}
		if (rowIndex < 0 || rowIndex >= base.DataGridView.Rows.Count)
		{
			throw new ArgumentOutOfRangeException("rowIndex");
		}
		DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle(base.DataGridView.DefaultCellStyle);
		if (OwningColumn != null)
		{
			dataGridViewCellStyle.ApplyStyle(OwningColumn.DefaultCellStyle);
		}
		dataGridViewCellStyle.ApplyStyle(base.DataGridView.RowsDefaultCellStyle);
		if (rowIndex % 2 == 1)
		{
			dataGridViewCellStyle.ApplyStyle(base.DataGridView.AlternatingRowsDefaultCellStyle);
		}
		dataGridViewCellStyle.ApplyStyle(base.DataGridView.Rows.SharedRow(rowIndex).DefaultCellStyle);
		if (HasStyle)
		{
			dataGridViewCellStyle.ApplyStyle(Style);
		}
		return dataGridViewCellStyle;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
	{
		if (base.DataGridView == null || base.DataGridView.EditingControl == null)
		{
			throw new InvalidOperationException("No editing control defined");
		}
	}

	public virtual bool KeyEntersEditMode(KeyEventArgs e)
	{
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static int MeasureTextHeight(Graphics graphics, string text, Font font, int maxWidth, TextFormatFlags flags)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("Graphics argument null");
		}
		if (font == null)
		{
			throw new ArgumentNullException("Font argument null");
		}
		if (maxWidth < 1)
		{
			throw new ArgumentOutOfRangeException("maxWidth is less than 1.");
		}
		return TextRenderer.MeasureText(graphics, text, font, new Size(maxWidth, 0), flags).Height;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[System.MonoTODO("does not use widthTruncated parameter")]
	public static int MeasureTextHeight(Graphics graphics, string text, Font font, int maxWidth, TextFormatFlags flags, out bool widthTruncated)
	{
		widthTruncated = false;
		return TextRenderer.MeasureText(graphics, text, font, new Size(maxWidth, 0), flags).Height;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Size MeasureTextPreferredSize(Graphics graphics, string text, Font font, float maxRatio, TextFormatFlags flags)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("Graphics argument null");
		}
		if (font == null)
		{
			throw new ArgumentNullException("Font argument null");
		}
		if (maxRatio <= 0f)
		{
			throw new ArgumentOutOfRangeException("maxRatio is less than or equals to 0.");
		}
		return MeasureTextSize(graphics, text, font, flags);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Size MeasureTextSize(Graphics graphics, string text, Font font, TextFormatFlags flags)
	{
		return TextRenderer.MeasureText(graphics, text, font, Size.Empty, flags);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static int MeasureTextWidth(Graphics graphics, string text, Font font, int maxHeight, TextFormatFlags flags)
	{
		if (graphics == null)
		{
			throw new ArgumentNullException("Graphics argument null");
		}
		if (font == null)
		{
			throw new ArgumentNullException("Font argument null");
		}
		if (maxHeight < 1)
		{
			throw new ArgumentOutOfRangeException("maxHeight is less than 1.");
		}
		return TextRenderer.MeasureText(graphics, text, font, new Size(0, maxHeight), flags).Width;
	}

	public virtual object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
	{
		if (cellStyle == null)
		{
			throw new ArgumentNullException("cellStyle is null.");
		}
		if (FormattedValueType == null)
		{
			throw new FormatException("The System.Windows.Forms.DataGridViewCell.FormattedValueType property value is null.");
		}
		if (formattedValue == null)
		{
			throw new ArgumentException("formattedValue is null.");
		}
		if (ValueType == null)
		{
			throw new FormatException("valuetype is null");
		}
		if (!FormattedValueType.IsAssignableFrom(formattedValue.GetType()))
		{
			throw new ArgumentException("formattedValue is not of formattedValueType.");
		}
		if (formattedValueTypeConverter == null)
		{
			formattedValueTypeConverter = FormattedValueTypeConverter;
		}
		if (valueTypeConverter == null)
		{
			valueTypeConverter = ValueTypeConverter;
		}
		if (valueTypeConverter != null && valueTypeConverter.CanConvertFrom(FormattedValueType))
		{
			return valueTypeConverter.ConvertFrom(formattedValue);
		}
		if (formattedValueTypeConverter != null && formattedValueTypeConverter.CanConvertTo(ValueType))
		{
			return formattedValueTypeConverter.ConvertTo(formattedValue, ValueType);
		}
		return Convert.ChangeType(formattedValue, ValueType);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual void PositionEditingControl(bool setLocation, bool setSize, Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
	{
		if (base.DataGridView.EditingControl != null)
		{
			if (setLocation && setSize)
			{
				base.DataGridView.EditingControl.Bounds = cellBounds;
			}
			else if (setLocation)
			{
				base.DataGridView.EditingControl.Location = cellBounds.Location;
			}
			else if (setSize)
			{
				base.DataGridView.EditingControl.Size = cellBounds.Size;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual Rectangle PositionEditingPanel(Rectangle cellBounds, Rectangle cellClip, DataGridViewCellStyle cellStyle, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow)
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		return $"{GetType().Name} {{ ColumnIndex={ColumnIndex}, RowIndex={RowIndex} }}";
	}

	protected virtual Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle)
	{
		Rectangle empty = Rectangle.Empty;
		empty.X = BorderToWidth(advancedBorderStyle.Left);
		empty.Y = BorderToWidth(advancedBorderStyle.Top);
		empty.Width = BorderToWidth(advancedBorderStyle.Right);
		empty.Height = BorderToWidth(advancedBorderStyle.Bottom);
		if (OwningColumn != null)
		{
			empty.Width += OwningColumn.DividerWidth;
		}
		if (OwningRow != null)
		{
			empty.Height += OwningRow.DividerHeight;
		}
		return empty;
	}

	private int BorderToWidth(DataGridViewAdvancedCellBorderStyle style)
	{
		switch (style)
		{
		case DataGridViewAdvancedCellBorderStyle.None:
			return 0;
		default:
			return 1;
		case DataGridViewAdvancedCellBorderStyle.InsetDouble:
		case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
			return 2;
		}
	}

	protected virtual bool ClickUnsharesRow(DataGridViewCellEventArgs e)
	{
		return false;
	}

	protected virtual bool ContentClickUnsharesRow(DataGridViewCellEventArgs e)
	{
		return false;
	}

	protected virtual bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e)
	{
		return false;
	}

	protected virtual AccessibleObject CreateAccessibilityInstance()
	{
		return new DataGridViewCellAccessibleObject(this);
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	protected virtual bool DoubleClickUnsharesRow(DataGridViewCellEventArgs e)
	{
		return false;
	}

	protected virtual bool EnterUnsharesRow(int rowIndex, bool throughMouseClick)
	{
		return false;
	}

	protected virtual object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
	{
		if (base.DataGridView == null)
		{
			return null;
		}
		if (rowIndex < 0 || rowIndex >= base.DataGridView.RowCount)
		{
			throw new ArgumentOutOfRangeException("rowIndex", "Specified argument was out of the range of valid values.");
		}
		string text = null;
		if (Selected)
		{
			DataGridViewCellStyle inheritedStyle = GetInheritedStyle(null, rowIndex, includeColors: false);
			text = GetEditedFormattedValue(rowIndex, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.ClipboardContent) as string;
		}
		if (text == null)
		{
			text = string.Empty;
		}
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
			if (inFirstRow && firstCell)
			{
				text2 = "<TABLE>";
			}
			if (inLastRow && lastCell)
			{
				text5 = "</TABLE>";
			}
			if (firstCell)
			{
				text4 = "<TR>";
			}
			if (lastCell)
			{
				text7 = "</TR>";
			}
			text3 = "<TD>";
			text6 = "</TD>";
			if (!Selected)
			{
				text = "&nbsp;";
			}
		}
		return text2 + text4 + text3 + text + text6 + text7 + text5;
	}

	internal object GetClipboardContentInternal(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
	{
		return GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format);
	}

	protected virtual Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		return Rectangle.Empty;
	}

	protected virtual Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
	{
		return Rectangle.Empty;
	}

	protected internal virtual string GetErrorText(int rowIndex)
	{
		return errorText;
	}

	protected virtual object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
	{
		if (base.DataGridView == null)
		{
			return null;
		}
		if (rowIndex < 0 || rowIndex >= base.DataGridView.RowCount)
		{
			throw new ArgumentOutOfRangeException("rowIndex");
		}
		if (!(this is DataGridViewRowHeaderCell))
		{
			DataGridViewCellFormattingEventArgs dataGridViewCellFormattingEventArgs = new DataGridViewCellFormattingEventArgs(ColumnIndex, rowIndex, value, FormattedValueType, cellStyle);
			base.DataGridView.OnCellFormattingInternal(dataGridViewCellFormattingEventArgs);
			if (dataGridViewCellFormattingEventArgs.FormattingApplied)
			{
				return dataGridViewCellFormattingEventArgs.Value;
			}
			cellStyle = dataGridViewCellFormattingEventArgs.CellStyle;
			value = dataGridViewCellFormattingEventArgs.Value;
		}
		if ((value == null || (cellStyle != null && value == cellStyle.DataSourceNullValue)) && FormattedValueType == typeof(string))
		{
			return string.Empty;
		}
		if (FormattedValueType == typeof(string) && value is IFormattable && !string.IsNullOrEmpty(cellStyle.Format))
		{
			return ((IFormattable)value).ToString(cellStyle.Format, cellStyle.FormatProvider);
		}
		if (value != null && FormattedValueType.IsAssignableFrom(value.GetType()))
		{
			return value;
		}
		if (formattedValueTypeConverter == null)
		{
			formattedValueTypeConverter = FormattedValueTypeConverter;
		}
		if (valueTypeConverter == null)
		{
			valueTypeConverter = ValueTypeConverter;
		}
		if (valueTypeConverter != null && valueTypeConverter.CanConvertTo(FormattedValueType))
		{
			return valueTypeConverter.ConvertTo(value, FormattedValueType);
		}
		if (formattedValueTypeConverter != null && formattedValueTypeConverter.CanConvertFrom(ValueType))
		{
			return formattedValueTypeConverter.ConvertFrom(value);
		}
		return Convert.ChangeType(value, FormattedValueType);
	}

	protected virtual Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
	{
		return new Size(-1, -1);
	}

	protected virtual Size GetSize(int rowIndex)
	{
		if (RowIndex == -1)
		{
			throw new InvalidOperationException("Getting the Size property of a cell in a shared row is not a valid operation.");
		}
		return new Size(OwningColumn.Width, OwningRow.Height);
	}

	protected virtual object GetValue(int rowIndex)
	{
		if (base.DataGridView != null && (RowIndex < 0 || RowIndex >= base.DataGridView.Rows.Count))
		{
			throw new ArgumentOutOfRangeException("rowIndex", "Specified argument was out of the range of valid values.");
		}
		if (OwningRow != null && OwningRow.Index == base.DataGridView.NewRowIndex)
		{
			return DefaultNewRowValue;
		}
		if (DataProperty != null && OwningRow.DataBoundItem != null)
		{
			return DataProperty.GetValue(OwningRow.DataBoundItem);
		}
		if (valuex != null)
		{
			return valuex;
		}
		DataGridViewCellValueEventArgs dataGridViewCellValueEventArgs = new DataGridViewCellValueEventArgs(columnIndex, rowIndex);
		if (base.DataGridView != null)
		{
			base.DataGridView.OnCellValueNeeded(dataGridViewCellValueEventArgs);
		}
		return dataGridViewCellValueEventArgs.Value;
	}

	protected virtual bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex)
	{
		return false;
	}

	protected virtual bool KeyPressUnsharesRow(KeyPressEventArgs e, int rowIndex)
	{
		return false;
	}

	protected virtual bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex)
	{
		return false;
	}

	protected virtual bool LeaveUnsharesRow(int rowIndex, bool throughMouseClick)
	{
		return false;
	}

	protected virtual bool MouseClickUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return false;
	}

	protected virtual bool MouseDoubleClickUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return false;
	}

	protected virtual bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return false;
	}

	protected virtual bool MouseEnterUnsharesRow(int rowIndex)
	{
		return false;
	}

	protected virtual bool MouseLeaveUnsharesRow(int rowIndex)
	{
		return false;
	}

	protected virtual bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return false;
	}

	protected virtual bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e)
	{
		return false;
	}

	protected virtual void OnClick(DataGridViewCellEventArgs e)
	{
	}

	internal void OnClickInternal(DataGridViewCellEventArgs e)
	{
		OnClick(e);
	}

	protected virtual void OnContentClick(DataGridViewCellEventArgs e)
	{
	}

	internal void OnContentClickInternal(DataGridViewCellEventArgs e)
	{
		OnContentClick(e);
	}

	protected virtual void OnContentDoubleClick(DataGridViewCellEventArgs e)
	{
	}

	internal void OnContentDoubleClickInternal(DataGridViewCellEventArgs e)
	{
		OnContentDoubleClick(e);
	}

	protected override void OnDataGridViewChanged()
	{
	}

	internal void OnDataGridViewChangedInternal()
	{
		OnDataGridViewChanged();
	}

	protected virtual void OnDoubleClick(DataGridViewCellEventArgs e)
	{
	}

	internal void OnDoubleClickInternal(DataGridViewCellEventArgs e)
	{
		OnDoubleClick(e);
	}

	protected virtual void OnEnter(int rowIndex, bool throughMouseClick)
	{
	}

	internal void OnEnterInternal(int rowIndex, bool throughMouseClick)
	{
		OnEnter(rowIndex, throughMouseClick);
	}

	protected virtual void OnKeyDown(KeyEventArgs e, int rowIndex)
	{
	}

	internal void OnKeyDownInternal(KeyEventArgs e, int rowIndex)
	{
		OnKeyDown(e, rowIndex);
	}

	protected virtual void OnKeyPress(KeyPressEventArgs e, int rowIndex)
	{
	}

	internal void OnKeyPressInternal(KeyPressEventArgs e, int rowIndex)
	{
		OnKeyPress(e, rowIndex);
	}

	protected virtual void OnKeyUp(KeyEventArgs e, int rowIndex)
	{
	}

	internal void OnKeyUpInternal(KeyEventArgs e, int rowIndex)
	{
		OnKeyUp(e, rowIndex);
	}

	protected virtual void OnLeave(int rowIndex, bool throughMouseClick)
	{
	}

	internal void OnLeaveInternal(int rowIndex, bool throughMouseClick)
	{
		OnLeave(rowIndex, throughMouseClick);
	}

	protected virtual void OnMouseClick(DataGridViewCellMouseEventArgs e)
	{
	}

	internal void OnMouseClickInternal(DataGridViewCellMouseEventArgs e)
	{
		OnMouseClick(e);
	}

	protected virtual void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e)
	{
	}

	internal void OnMouseDoubleClickInternal(DataGridViewCellMouseEventArgs e)
	{
		OnMouseDoubleClick(e);
	}

	protected virtual void OnMouseDown(DataGridViewCellMouseEventArgs e)
	{
	}

	internal void OnMouseDownInternal(DataGridViewCellMouseEventArgs e)
	{
		OnMouseDown(e);
	}

	protected virtual void OnMouseEnter(int rowIndex)
	{
	}

	internal void OnMouseEnterInternal(int rowIndex)
	{
		OnMouseEnter(rowIndex);
	}

	protected virtual void OnMouseLeave(int rowIndex)
	{
	}

	internal void OnMouseLeaveInternal(int e)
	{
		OnMouseLeave(e);
	}

	protected virtual void OnMouseMove(DataGridViewCellMouseEventArgs e)
	{
	}

	internal void OnMouseMoveInternal(DataGridViewCellMouseEventArgs e)
	{
		OnMouseMove(e);
	}

	protected virtual void OnMouseUp(DataGridViewCellMouseEventArgs e)
	{
	}

	internal void OnMouseUpInternal(DataGridViewCellMouseEventArgs e)
	{
		OnMouseUp(e);
	}

	internal void PaintInternal(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
	}

	protected virtual void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
		{
			PaintPartBackground(graphics, cellBounds, cellStyle);
		}
		if ((paintParts & DataGridViewPaintParts.SelectionBackground) == DataGridViewPaintParts.SelectionBackground)
		{
			PaintPartSelectionBackground(graphics, cellBounds, cellState, cellStyle);
		}
		if ((paintParts & DataGridViewPaintParts.ContentForeground) == DataGridViewPaintParts.ContentForeground)
		{
			PaintPartContent(graphics, cellBounds, rowIndex, cellState, cellStyle, formattedValue);
		}
		if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
		{
			PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
		}
		if ((paintParts & DataGridViewPaintParts.Focus) == DataGridViewPaintParts.Focus)
		{
			PaintPartFocus(graphics, cellBounds);
		}
		if ((paintParts & DataGridViewPaintParts.ErrorIcon) == DataGridViewPaintParts.ErrorIcon && !string.IsNullOrEmpty(ErrorText))
		{
			PaintErrorIcon(graphics, clipBounds, cellBounds, ErrorText);
		}
	}

	protected virtual void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle bounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
	{
		Pen pen = new Pen(base.DataGridView.GridColor);
		switch (advancedBorderStyle.Left)
		{
		case DataGridViewAdvancedCellBorderStyle.Single:
			if (base.DataGridView.CellBorderStyle != DataGridViewCellBorderStyle.Single)
			{
				graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height - 1);
			}
			break;
		case DataGridViewAdvancedCellBorderStyle.Inset:
		case DataGridViewAdvancedCellBorderStyle.Outset:
			graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height - 1);
			break;
		case DataGridViewAdvancedCellBorderStyle.InsetDouble:
		case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
			graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height - 1);
			graphics.DrawLine(pen, bounds.X + 2, bounds.Y, bounds.X + 2, bounds.Y + bounds.Height - 1);
			break;
		}
		switch (advancedBorderStyle.Right)
		{
		case DataGridViewAdvancedCellBorderStyle.Single:
			graphics.DrawLine(pen, bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
			break;
		case DataGridViewAdvancedCellBorderStyle.Inset:
		case DataGridViewAdvancedCellBorderStyle.InsetDouble:
		case DataGridViewAdvancedCellBorderStyle.Outset:
		case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
			graphics.DrawLine(pen, bounds.X + bounds.Width, bounds.Y, bounds.X + bounds.Width, bounds.Y + bounds.Height - 1);
			break;
		}
		switch (advancedBorderStyle.Top)
		{
		case DataGridViewAdvancedCellBorderStyle.Single:
			if (base.DataGridView.CellBorderStyle != DataGridViewCellBorderStyle.Single)
			{
				graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y);
			}
			break;
		case DataGridViewAdvancedCellBorderStyle.Inset:
		case DataGridViewAdvancedCellBorderStyle.InsetDouble:
		case DataGridViewAdvancedCellBorderStyle.Outset:
		case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
			graphics.DrawLine(pen, bounds.X, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y);
			break;
		}
		switch (advancedBorderStyle.Bottom)
		{
		case DataGridViewAdvancedCellBorderStyle.Single:
		case DataGridViewAdvancedCellBorderStyle.Inset:
		case DataGridViewAdvancedCellBorderStyle.InsetDouble:
		case DataGridViewAdvancedCellBorderStyle.Outset:
		case DataGridViewAdvancedCellBorderStyle.OutsetDouble:
			graphics.DrawLine(pen, bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
			break;
		}
	}

	protected virtual void PaintErrorIcon(Graphics graphics, Rectangle clipBounds, Rectangle cellValueBounds, string errorText)
	{
		Rectangle errorIconBounds = GetErrorIconBounds(graphics, null, RowIndex);
		if (!errorIconBounds.IsEmpty)
		{
			Point location = errorIconBounds.Location;
			location.X += cellValueBounds.Left;
			location.Y += cellValueBounds.Top;
			graphics.FillRectangle(Brushes.Red, new Rectangle(location.X + 1, location.Y + 2, 10, 7));
			graphics.FillRectangle(Brushes.Red, new Rectangle(location.X + 2, location.Y + 1, 8, 9));
			graphics.FillRectangle(Brushes.Red, new Rectangle(location.X + 4, location.Y, 4, 11));
			graphics.FillRectangle(Brushes.Red, new Rectangle(location.X, location.Y + 4, 12, 3));
			graphics.FillRectangle(Brushes.White, new Rectangle(location.X + 5, location.Y + 2, 2, 4));
			graphics.FillRectangle(Brushes.White, new Rectangle(location.X + 5, location.Y + 7, 2, 2));
		}
	}

	internal virtual void PaintPartBackground(Graphics graphics, Rectangle cellBounds, DataGridViewCellStyle style)
	{
		Color backColor = style.BackColor;
		graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(backColor), cellBounds);
	}

	internal Pen GetBorderPen()
	{
		return ThemeEngine.Current.ResPool.GetPen(base.DataGridView.GridColor);
	}

	internal virtual void PaintPartContent(Graphics graphics, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle, object formattedValue)
	{
		if (!IsInEditMode)
		{
			Color foreColor = ((!Selected) ? cellStyle.ForeColor : cellStyle.SelectionForeColor);
			TextFormatFlags textFormatFlags = TextFormatFlags.VerticalCenter | TextFormatFlags.TextBoxControl | TextFormatFlags.EndEllipsis;
			textFormatFlags |= AlignmentToFlags(style.Alignment);
			cellBounds.Height -= 2;
			cellBounds.Width -= 2;
			if (formattedValue != null)
			{
				TextRenderer.DrawText(graphics, formattedValue.ToString(), cellStyle.Font, cellBounds, foreColor, textFormatFlags);
			}
		}
	}

	private void PaintPartFocus(Graphics graphics, Rectangle cellBounds)
	{
		cellBounds.Width--;
		cellBounds.Height--;
		if (base.DataGridView.ShowFocusCues && base.DataGridView.CurrentCell == this && base.DataGridView.Focused)
		{
			ControlPaint.DrawFocusRectangle(graphics, cellBounds);
		}
	}

	internal virtual void PaintPartSelectionBackground(Graphics graphics, Rectangle cellBounds, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle)
	{
		if ((cellState & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected && (RowIndex < 0 || !IsInEditMode || EditType == null))
		{
			Color selectionBackColor = cellStyle.SelectionBackColor;
			graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(selectionBackColor), cellBounds);
		}
	}

	internal void PaintWork(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		object value;
		object formattedValue;
		if (RowIndex == -1 && !(this is DataGridViewColumnHeaderCell))
		{
			value = null;
			formattedValue = null;
		}
		else if (RowIndex == -1)
		{
			value = Value;
			formattedValue = Value;
		}
		else
		{
			value = Value;
			formattedValue = GetFormattedValue(Value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);
		}
		DataGridViewCellPaintingEventArgs dataGridViewCellPaintingEventArgs = new DataGridViewCellPaintingEventArgs(base.DataGridView, graphics, clipBounds, cellBounds, rowIndex, columnIndex, cellState, value, formattedValue, ErrorText, cellStyle, advancedBorderStyle, paintParts);
		base.DataGridView.OnCellPaintingInternal(dataGridViewCellPaintingEventArgs);
		if (!dataGridViewCellPaintingEventArgs.Handled)
		{
			dataGridViewCellPaintingEventArgs.Paint(dataGridViewCellPaintingEventArgs.ClipBounds, dataGridViewCellPaintingEventArgs.PaintParts);
		}
	}

	protected virtual bool SetValue(int rowIndex, object value)
	{
		object value2 = Value;
		if (DataProperty != null && !DataProperty.IsReadOnly)
		{
			DataProperty.SetValue(OwningRow.DataBoundItem, value);
		}
		else
		{
			valuex = value;
		}
		if (!object.ReferenceEquals(value2, value) || !object.Equals(value2, value))
		{
			RaiseCellValueChanged(new DataGridViewCellEventArgs(ColumnIndex, RowIndex));
			if (this is IDataGridViewEditingCell)
			{
				(this as IDataGridViewEditingCell).EditingCellValueChanged = false;
			}
			if (base.DataGridView != null)
			{
				base.DataGridView.InvalidateCell(this);
			}
			return true;
		}
		return false;
	}

	private void OnStyleChanged(object sender, EventArgs args)
	{
		if (base.DataGridView != null)
		{
			base.DataGridView.RaiseCellStyleChanged(new DataGridViewCellEventArgs(ColumnIndex, RowIndex));
		}
	}

	internal void InternalPaint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
	{
		Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
	}

	internal void SetOwningRow(DataGridViewRow row)
	{
		owningRow = row;
	}

	internal void SetOwningColumn(DataGridViewColumn col)
	{
		columnIndex = col.Index;
	}

	internal void SetColumnIndex(int index)
	{
		columnIndex = index;
	}

	internal void SetIsInEditMode(bool isInEditMode)
	{
		this.isInEditMode = isInEditMode;
	}

	internal void OnErrorTextChanged(DataGridViewCellEventArgs args)
	{
		if (base.DataGridView != null)
		{
			base.DataGridView.OnCellErrorTextChanged(args);
		}
	}

	internal TextFormatFlags AlignmentToFlags(DataGridViewContentAlignment align)
	{
		TextFormatFlags textFormatFlags = TextFormatFlags.Left;
		switch (align)
		{
		case DataGridViewContentAlignment.BottomCenter:
			textFormatFlags |= TextFormatFlags.Bottom;
			textFormatFlags |= TextFormatFlags.HorizontalCenter;
			break;
		case DataGridViewContentAlignment.BottomLeft:
			textFormatFlags |= TextFormatFlags.Bottom;
			break;
		case DataGridViewContentAlignment.BottomRight:
			textFormatFlags |= TextFormatFlags.Bottom;
			textFormatFlags |= TextFormatFlags.Right;
			break;
		case DataGridViewContentAlignment.MiddleCenter:
			textFormatFlags |= TextFormatFlags.VerticalCenter;
			textFormatFlags |= TextFormatFlags.HorizontalCenter;
			break;
		case DataGridViewContentAlignment.MiddleLeft:
			textFormatFlags |= TextFormatFlags.VerticalCenter;
			break;
		case DataGridViewContentAlignment.MiddleRight:
			textFormatFlags |= TextFormatFlags.VerticalCenter;
			textFormatFlags |= TextFormatFlags.Right;
			break;
		case DataGridViewContentAlignment.TopLeft:
			textFormatFlags |= TextFormatFlags.Left;
			break;
		case DataGridViewContentAlignment.TopCenter:
			textFormatFlags |= TextFormatFlags.HorizontalCenter;
			textFormatFlags |= TextFormatFlags.Left;
			break;
		case DataGridViewContentAlignment.TopRight:
			textFormatFlags |= TextFormatFlags.Right;
			textFormatFlags |= TextFormatFlags.Left;
			break;
		}
		return textFormatFlags;
	}

	internal Rectangle AlignInRectangle(Rectangle outer, Size inner, DataGridViewContentAlignment align)
	{
		int x = 0;
		int y = 0;
		switch (align)
		{
		case DataGridViewContentAlignment.TopLeft:
		case DataGridViewContentAlignment.MiddleLeft:
		case DataGridViewContentAlignment.BottomLeft:
			x = outer.X;
			break;
		case DataGridViewContentAlignment.TopCenter:
		case DataGridViewContentAlignment.MiddleCenter:
		case DataGridViewContentAlignment.BottomCenter:
			x = Math.Max(outer.X + (outer.Width - inner.Width) / 2, outer.Left);
			break;
		case DataGridViewContentAlignment.TopRight:
		case DataGridViewContentAlignment.MiddleRight:
		case DataGridViewContentAlignment.BottomRight:
			x = Math.Max(outer.Right - inner.Width, outer.X);
			break;
		}
		switch (align)
		{
		case DataGridViewContentAlignment.TopLeft:
		case DataGridViewContentAlignment.TopCenter:
		case DataGridViewContentAlignment.TopRight:
			y = outer.Y;
			break;
		case DataGridViewContentAlignment.MiddleLeft:
		case DataGridViewContentAlignment.MiddleCenter:
		case DataGridViewContentAlignment.MiddleRight:
			y = Math.Max(outer.Y + (outer.Height - inner.Height) / 2, outer.Y);
			break;
		case DataGridViewContentAlignment.BottomLeft:
		case DataGridViewContentAlignment.BottomCenter:
		case DataGridViewContentAlignment.BottomRight:
			y = Math.Max(outer.Bottom - inner.Height, outer.Y);
			break;
		}
		return new Rectangle(x, y, Math.Min(inner.Width, outer.Width), Math.Min(inner.Height, outer.Height));
	}
}
