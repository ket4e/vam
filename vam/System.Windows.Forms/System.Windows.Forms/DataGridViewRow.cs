using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[TypeConverter(typeof(DataGridViewRowConverter))]
public class DataGridViewRow : DataGridViewBand
{
	[ComVisible(true)]
	protected class DataGridViewRowAccessibleObject : AccessibleObject
	{
		private DataGridViewRow dataGridViewRow;

		public override Rectangle Bounds
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string Name => "Index: " + dataGridViewRow.Index;

		public DataGridViewRow Owner
		{
			get
			{
				return dataGridViewRow;
			}
			set
			{
				dataGridViewRow = value;
			}
		}

		public override AccessibleObject Parent => dataGridViewRow.AccessibilityObject;

		public override AccessibleRole Role => AccessibleRole.Row;

		public override AccessibleStates State
		{
			get
			{
				if (dataGridViewRow.Selected)
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
				if (dataGridViewRow.Cells.Count == 0)
				{
					return "(Create New)";
				}
				string text = string.Empty;
				foreach (DataGridViewCell cell in dataGridViewRow.Cells)
				{
					text += cell.AccessibilityObject.Value;
				}
				return text;
			}
		}

		public DataGridViewRowAccessibleObject()
		{
		}

		public DataGridViewRowAccessibleObject(DataGridViewRow owner)
		{
			dataGridViewRow = owner;
		}

		public override AccessibleObject GetChild(int index)
		{
			throw new NotImplementedException();
		}

		public override int GetChildCount()
		{
			throw new NotImplementedException();
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
				dataGridViewRow.DataGridView.Focus();
				break;
			case AccessibleSelection.AddSelection:
				dataGridViewRow.DataGridView.SelectedRows.InternalAdd(dataGridViewRow);
				break;
			case AccessibleSelection.RemoveSelection:
				dataGridViewRow.DataGridView.SelectedRows.InternalRemove(dataGridViewRow);
				break;
			}
		}
	}

	private AccessibleObject accessibilityObject;

	private DataGridViewCellCollection cells;

	private ContextMenuStrip contextMenuStrip;

	private int dividerHeight;

	private string errorText;

	private DataGridViewRowHeaderCell headerCell;

	private int height;

	private int minimumHeight;

	private int explicit_height;

	[Browsable(false)]
	public AccessibleObject AccessibilityObject => accessibilityObject;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public DataGridViewCellCollection Cells => cells;

	[DefaultValue(null)]
	public override ContextMenuStrip ContextMenuStrip
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Operation cannot be performed on a shared row.");
			}
			return contextMenuStrip;
		}
		set
		{
			if (contextMenuStrip != value)
			{
				contextMenuStrip = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnRowContextMenuStripChanged(new DataGridViewRowEventArgs(this));
				}
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public object DataBoundItem
	{
		get
		{
			if (base.DataGridView != null && base.DataGridView.DataManager != null && base.DataGridView.DataManager.Count > base.Index)
			{
				return base.DataGridView.DataManager[base.Index];
			}
			return null;
		}
	}

	[NotifyParentProperty(true)]
	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public override DataGridViewCellStyle DefaultCellStyle
	{
		get
		{
			return base.DefaultCellStyle;
		}
		set
		{
			if (DefaultCellStyle != value)
			{
				base.DefaultCellStyle = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnRowDefaultCellStyleChanged(new DataGridViewRowEventArgs(this));
				}
			}
		}
	}

	[Browsable(false)]
	public override bool Displayed
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Getting the Displayed property of a shared row is not a valid operation.");
			}
			return base.Displayed;
		}
	}

	[NotifyParentProperty(true)]
	[DefaultValue(0)]
	public int DividerHeight
	{
		get
		{
			return dividerHeight;
		}
		set
		{
			dividerHeight = value;
		}
	}

	[NotifyParentProperty(true)]
	[DefaultValue("")]
	public string ErrorText
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Operation cannot be performed on a shared row.");
			}
			return (errorText != null) ? errorText : string.Empty;
		}
		set
		{
			if (errorText != value)
			{
				errorText = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnRowErrorTextChanged(new DataGridViewRowEventArgs(this));
				}
			}
		}
	}

	[Browsable(false)]
	public override bool Frozen
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Getting the Frozen property of a shared row is not a valid operation.");
			}
			return base.Frozen;
		}
		set
		{
			base.Frozen = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public DataGridViewRowHeaderCell HeaderCell
	{
		get
		{
			return headerCell;
		}
		set
		{
			if (headerCell != value)
			{
				headerCell = value;
				headerCell.SetOwningRow(this);
				if (base.DataGridView != null)
				{
					headerCell.SetDataGridView(base.DataGridView);
					base.DataGridView.OnRowHeaderCellChanged(new DataGridViewRowEventArgs(this));
				}
			}
		}
	}

	[DefaultValue(22)]
	[NotifyParentProperty(true)]
	public int Height
	{
		get
		{
			if (height < 0)
			{
				if (DefaultCellStyle != null && DefaultCellStyle.Font != null)
				{
					return DefaultCellStyle.Font.Height + 9;
				}
				if (base.Index >= 0 && InheritedStyle != null && InheritedStyle.Font != null)
				{
					return InheritedStyle.Font.Height + 9;
				}
				return Control.DefaultFont.Height + 9;
			}
			return height;
		}
		set
		{
			explicit_height = value;
			if (height != value)
			{
				if (value < minimumHeight)
				{
					throw new ArgumentOutOfRangeException("Height can't be less than MinimumHeight.");
				}
				height = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.Invalidate();
					base.DataGridView.OnRowHeightChanged(new DataGridViewRowEventArgs(this));
				}
			}
		}
	}

	public override DataGridViewCellStyle InheritedStyle
	{
		get
		{
			if (base.Index == -1)
			{
				throw new InvalidOperationException("Getting the InheritedStyle property of a shared row is not a valid operation.");
			}
			if (base.DataGridView == null)
			{
				return DefaultCellStyle;
			}
			if (DefaultCellStyle == null)
			{
				return base.DataGridView.DefaultCellStyle;
			}
			return DefaultCellStyle.Clone();
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool IsNewRow
	{
		get
		{
			if (base.DataGridView != null && base.DataGridView.Rows[base.DataGridView.Rows.Count - 1] == this && base.DataGridView.NewRowIndex == base.Index)
			{
				return true;
			}
			return false;
		}
	}

	internal bool IsShared => base.Index == -1 && base.DataGridView != null;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int MinimumHeight
	{
		get
		{
			return minimumHeight;
		}
		set
		{
			if (minimumHeight != value)
			{
				if (value < 2 || value > int.MaxValue)
				{
					throw new ArgumentOutOfRangeException("MinimumHeight should be between 2 and Int32.MaxValue.");
				}
				minimumHeight = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnRowMinimumHeightChanged(new DataGridViewRowEventArgs(this));
				}
			}
		}
	}

	[Browsable(true)]
	[DefaultValue(false)]
	[NotifyParentProperty(true)]
	public override bool ReadOnly
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Getting the ReadOnly property of a shared row is not a valid operation.");
			}
			if (base.DataGridView != null && base.DataGridView.ReadOnly)
			{
				return true;
			}
			return base.ReadOnly;
		}
		set
		{
			base.ReadOnly = value;
		}
	}

	[NotifyParentProperty(true)]
	public override DataGridViewTriState Resizable
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Getting the Resizable property of a shared row is not a valid operation.");
			}
			return base.Resizable;
		}
		set
		{
			base.Resizable = value;
		}
	}

	public override bool Selected
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Getting the Selected property of a shared row is not a valid operation.");
			}
			return base.Selected;
		}
		set
		{
			if (base.Index == -1)
			{
				throw new InvalidOperationException("The row is a shared row.");
			}
			if (base.DataGridView == null)
			{
				throw new InvalidOperationException("The row has not been added to a DataGridView control.");
			}
			base.Selected = value;
		}
	}

	public override DataGridViewElementStates State
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Getting the State property of a shared row is not a valid operation.");
			}
			return base.State;
		}
	}

	[Browsable(false)]
	public override bool Visible
	{
		get
		{
			if (IsShared)
			{
				throw new InvalidOperationException("Getting the Visible property of a shared row is not a valid operation.");
			}
			return base.Visible;
		}
		set
		{
			if (IsNewRow && !value)
			{
				throw new InvalidOperationException("Cant make invisible a new row.");
			}
			if (!value && base.DataGridView != null && base.DataGridView.DataManager != null && base.DataGridView.DataManager.Position == base.Index)
			{
				throw new InvalidOperationException("Row associated with the currency manager's position cannot be made invisible.");
			}
			base.Visible = value;
			if (base.DataGridView != null)
			{
				base.DataGridView.Invalidate();
			}
		}
	}

	public DataGridViewRow()
	{
		cells = new DataGridViewCellCollection(this);
		minimumHeight = 3;
		height = -1;
		explicit_height = -1;
		headerCell = new DataGridViewRowHeaderCell();
		headerCell.SetOwningRow(this);
		accessibilityObject = new AccessibleObject();
		SetState(DataGridViewElementStates.Visible);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual DataGridViewAdvancedBorderStyle AdjustRowHeaderBorderStyle(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput, DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder, bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedRow, bool isLastVisibleRow)
	{
		throw new NotImplementedException();
	}

	public override object Clone()
	{
		DataGridViewRow dataGridViewRow = (DataGridViewRow)MemberwiseClone();
		dataGridViewRow.HeaderCell = (DataGridViewRowHeaderCell)HeaderCell.Clone();
		dataGridViewRow.SetIndex(-1);
		dataGridViewRow.cells = new DataGridViewCellCollection(dataGridViewRow);
		foreach (DataGridViewCell cell in cells)
		{
			dataGridViewRow.cells.Add(cell.Clone() as DataGridViewCell);
		}
		dataGridViewRow.SetDataGridView(null);
		return dataGridViewRow;
	}

	public void CreateCells(DataGridView dataGridView)
	{
		if (dataGridView == null)
		{
			throw new ArgumentNullException("DataGridView is null.");
		}
		if (dataGridView.Rows.Contains(this))
		{
			throw new InvalidOperationException("The row already exists in the DataGridView.");
		}
		DataGridViewCellCollection dataGridViewCellCollection = new DataGridViewCellCollection(this);
		foreach (DataGridViewColumn column in dataGridView.Columns)
		{
			if (column.CellTemplate == null)
			{
				throw new InvalidOperationException("Cell template not set in column: " + column.Index + ".");
			}
			dataGridViewCellCollection.Add((DataGridViewCell)column.CellTemplate.Clone());
		}
		cells = dataGridViewCellCollection;
	}

	public void CreateCells(DataGridView dataGridView, params object[] values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values is null");
		}
		CreateCells(dataGridView);
		for (int i = 0; i < values.Length; i++)
		{
			cells[i].Value = values[i];
		}
	}

	public ContextMenuStrip GetContextMenuStrip(int rowIndex)
	{
		if (rowIndex == -1)
		{
			throw new InvalidOperationException("rowIndex is -1");
		}
		if (rowIndex < 0 || rowIndex >= base.DataGridView.Rows.Count)
		{
			throw new ArgumentOutOfRangeException("rowIndex is out of range");
		}
		return null;
	}

	public string GetErrorText(int rowIndex)
	{
		return string.Empty;
	}

	public virtual int GetPreferredHeight(int rowIndex, DataGridViewAutoSizeRowMode autoSizeRowMode, bool fixedWidth)
	{
		DataGridViewRow dataGridViewRow = ((base.DataGridView == null) ? this : base.DataGridView.Rows.SharedRow(rowIndex));
		int num = 0;
		if (autoSizeRowMode == DataGridViewAutoSizeRowMode.AllCells || autoSizeRowMode == DataGridViewAutoSizeRowMode.RowHeader)
		{
			num = Math.Max(num, dataGridViewRow.HeaderCell.PreferredSize.Height);
		}
		if (autoSizeRowMode == DataGridViewAutoSizeRowMode.AllCells || autoSizeRowMode == DataGridViewAutoSizeRowMode.AllCellsExceptHeader)
		{
			foreach (DataGridViewCell cell in dataGridViewRow.Cells)
			{
				num = Math.Max(num, cell.PreferredSize.Height);
			}
		}
		return num;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public virtual DataGridViewElementStates GetState(int rowIndex)
	{
		DataGridViewElementStates dataGridViewElementStates = DataGridViewElementStates.None;
		if (rowIndex == -1)
		{
			dataGridViewElementStates |= DataGridViewElementStates.Displayed;
			if (base.DataGridView.ReadOnly)
			{
				dataGridViewElementStates |= DataGridViewElementStates.ReadOnly;
			}
			if (base.DataGridView.AllowUserToResizeRows)
			{
				dataGridViewElementStates |= DataGridViewElementStates.Resizable;
			}
			if (base.DataGridView.Visible)
			{
				dataGridViewElementStates |= DataGridViewElementStates.Visible;
			}
			return dataGridViewElementStates;
		}
		DataGridViewRow dataGridViewRow = base.DataGridView.Rows[rowIndex];
		if (dataGridViewRow.Displayed)
		{
			dataGridViewElementStates |= DataGridViewElementStates.Displayed;
		}
		if (dataGridViewRow.Frozen)
		{
			dataGridViewElementStates |= DataGridViewElementStates.Frozen;
		}
		if (dataGridViewRow.ReadOnly)
		{
			dataGridViewElementStates |= DataGridViewElementStates.ReadOnly;
		}
		if (dataGridViewRow.Resizable == DataGridViewTriState.True || (dataGridViewRow.Resizable == DataGridViewTriState.NotSet && base.DataGridView.AllowUserToResizeRows))
		{
			dataGridViewElementStates |= DataGridViewElementStates.Resizable;
		}
		if (dataGridViewRow.Resizable == DataGridViewTriState.True)
		{
			dataGridViewElementStates |= DataGridViewElementStates.ResizableSet;
		}
		if (dataGridViewRow.Selected)
		{
			dataGridViewElementStates |= DataGridViewElementStates.Selected;
		}
		if (dataGridViewRow.Visible)
		{
			dataGridViewElementStates |= DataGridViewElementStates.Visible;
		}
		return dataGridViewElementStates;
	}

	public bool SetValues(params object[] values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("vues is null");
		}
		if (base.DataGridView != null && base.DataGridView.VirtualMode)
		{
			throw new InvalidOperationException("DataGridView is operating in virtual mode");
		}
		for (int i = 0; i < values.Length; i++)
		{
			DataGridViewCell dataGridViewCell;
			if (cells.Count > i)
			{
				dataGridViewCell = cells[i];
			}
			else
			{
				dataGridViewCell = new DataGridViewTextBoxCell();
				cells.Add(dataGridViewCell);
			}
			dataGridViewCell.Value = values[i];
		}
		return true;
	}

	public override string ToString()
	{
		return GetType().Name + ", Band Index: " + base.Index;
	}

	protected virtual AccessibleObject CreateAccessibilityInstance()
	{
		return new DataGridViewRowAccessibleObject(this);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected virtual DataGridViewCellCollection CreateCellsInstance()
	{
		cells = new DataGridViewCellCollection(this);
		return cells;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected internal virtual void DrawFocus(Graphics graphics, Rectangle clipBounds, Rectangle bounds, int rowIndex, DataGridViewElementStates rowState, DataGridViewCellStyle cellStyle, bool cellsPaintSelectionBackground)
	{
	}

	protected internal virtual void Paint(Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow)
	{
		DataGridViewCellStyle inheritedRowStyle = ((base.Index != -1) ? InheritedStyle : base.DataGridView.RowsDefaultCellStyle);
		DataGridViewRowPrePaintEventArgs dataGridViewRowPrePaintEventArgs = new DataGridViewRowPrePaintEventArgs(base.DataGridView, graphics, clipBounds, rowBounds, rowIndex, rowState, string.Empty, inheritedRowStyle, isFirstDisplayedRow, isLastVisibleRow);
		dataGridViewRowPrePaintEventArgs.PaintParts = DataGridViewPaintParts.All;
		base.DataGridView.OnRowPrePaint(dataGridViewRowPrePaintEventArgs);
		if (!dataGridViewRowPrePaintEventArgs.Handled)
		{
			if (base.DataGridView.RowHeadersVisible)
			{
				PaintHeader(graphics, dataGridViewRowPrePaintEventArgs.ClipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, dataGridViewRowPrePaintEventArgs.PaintParts);
			}
			PaintCells(graphics, dataGridViewRowPrePaintEventArgs.ClipBounds, rowBounds, rowIndex, rowState, isFirstDisplayedRow, isLastVisibleRow, dataGridViewRowPrePaintEventArgs.PaintParts);
			DataGridViewRowPostPaintEventArgs e = new DataGridViewRowPostPaintEventArgs(base.DataGridView, graphics, dataGridViewRowPrePaintEventArgs.ClipBounds, rowBounds, rowIndex, rowState, dataGridViewRowPrePaintEventArgs.ErrorText, inheritedRowStyle, isFirstDisplayedRow, isLastVisibleRow);
			base.DataGridView.OnRowPostPaint(e);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected internal virtual void PaintCells(Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
	{
		List<DataGridViewColumn> columnDisplayIndexSortedArrayList = base.DataGridView.Columns.ColumnDisplayIndexSortedArrayList;
		Rectangle rectangle = rowBounds;
		if (base.DataGridView.RowHeadersVisible)
		{
			rectangle.X += base.DataGridView.RowHeadersWidth;
			rectangle.Width -= base.DataGridView.RowHeadersWidth;
		}
		for (int i = base.DataGridView.first_col_index; i < columnDisplayIndexSortedArrayList.Count; i++)
		{
			DataGridViewColumn dataGridViewColumn = columnDisplayIndexSortedArrayList[i];
			if (dataGridViewColumn.Visible)
			{
				if (!dataGridViewColumn.Displayed)
				{
					break;
				}
				rectangle.Width = dataGridViewColumn.Width;
				DataGridViewCell dataGridViewCell = Cells[dataGridViewColumn.Index];
				if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
				{
					graphics.FillRectangle(Brushes.White, rectangle);
				}
				DataGridViewCellStyle cellStyle = ((dataGridViewCell.RowIndex != -1) ? dataGridViewCell.InheritedStyle : DefaultCellStyle);
				object value;
				DataGridViewElementStates inheritedState;
				if (dataGridViewCell.RowIndex == -1)
				{
					value = null;
					object obj = null;
					string text = null;
					inheritedState = dataGridViewCell.State;
				}
				else
				{
					value = dataGridViewCell.Value;
					object obj = dataGridViewCell.FormattedValue;
					string text = dataGridViewCell.ErrorText;
					inheritedState = dataGridViewCell.InheritedState;
				}
				DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = (DataGridViewAdvancedBorderStyle)((ICloneable)base.DataGridView.AdvancedCellBorderStyle).Clone();
				DataGridViewAdvancedBorderStyle advancedBorderStyle = dataGridViewCell.AdjustCellBorderStyle(base.DataGridView.AdvancedCellBorderStyle, dataGridViewAdvancedBorderStylePlaceholder, singleVerticalBorderAdded: true, singleHorizontalBorderAdded: true, dataGridViewCell.ColumnIndex == 0, dataGridViewCell.RowIndex == 0);
				base.DataGridView.OnCellFormattingInternal(new DataGridViewCellFormattingEventArgs(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex, value, dataGridViewCell.FormattedValueType, cellStyle));
				dataGridViewCell.PaintWork(graphics, clipBounds, rectangle, rowIndex, inheritedState, cellStyle, advancedBorderStyle, paintParts);
				rectangle.X += rectangle.Width;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected internal virtual void PaintHeader(Graphics graphics, Rectangle clipBounds, Rectangle rowBounds, int rowIndex, DataGridViewElementStates rowState, bool isFirstDisplayedRow, bool isLastVisibleRow, DataGridViewPaintParts paintParts)
	{
		rowBounds.Width = base.DataGridView.RowHeadersWidth;
		graphics.FillRectangle(Brushes.White, rowBounds);
		HeaderCell.PaintWork(graphics, clipBounds, rowBounds, rowIndex, rowState, HeaderCell.InheritedStyle, base.DataGridView.AdvancedRowHeadersBorderStyle, paintParts);
	}

	internal override void SetDataGridView(DataGridView dataGridView)
	{
		base.SetDataGridView(dataGridView);
		headerCell.SetDataGridView(dataGridView);
		foreach (DataGridViewCell cell in cells)
		{
			cell.SetDataGridView(dataGridView);
		}
	}

	internal override void SetState(DataGridViewElementStates state)
	{
		if (State != state)
		{
			base.SetState(state);
			if (base.DataGridView != null)
			{
				base.DataGridView.OnRowStateChanged(base.Index, new DataGridViewRowStateChangedEventArgs(this, state));
			}
		}
	}

	internal void SetAutoSizeHeight(int height)
	{
		this.height = height;
		if (base.DataGridView != null)
		{
			base.DataGridView.Invalidate();
			base.DataGridView.OnRowHeightChanged(new DataGridViewRowEventArgs(this));
		}
	}

	internal void ResetToExplicitHeight()
	{
		height = explicit_height;
		if (base.DataGridView != null)
		{
			base.DataGridView.OnRowHeightChanged(new DataGridViewRowEventArgs(this));
		}
	}
}
