using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ToolboxItem(false)]
[DefaultProperty("Header")]
[DesignTimeVisible(false)]
public abstract class DataGridColumnStyle : Component, IDataGridColumnStyleEditingNotificationService
{
	[ComVisible(true)]
	protected class DataGridColumnHeaderAccessibleObject : AccessibleObject
	{
		private new DataGridColumnStyle owner;

		[System.MonoTODO("Not implemented, will throw NotImplementedException")]
		public override Rectangle Bounds
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected DataGridColumnStyle Owner => owner;

		public override AccessibleObject Parent
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override AccessibleRole Role
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DataGridColumnHeaderAccessibleObject()
		{
		}

		public DataGridColumnHeaderAccessibleObject(DataGridColumnStyle owner)
		{
			this.owner = owner;
		}

		[System.MonoTODO("Not implemented, will throw NotImplementedException")]
		public override AccessibleObject Navigate(AccessibleNavigation navdir)
		{
			throw new NotImplementedException();
		}
	}

	protected class CompModSwitches
	{
		[System.MonoTODO("Not implemented, will throw NotImplementedException")]
		public static TraceSwitch DGEditColumnEditing
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}

	internal enum ArrowDrawing
	{
		No,
		Ascending,
		Descending
	}

	internal HorizontalAlignment alignment;

	private int fontheight;

	internal DataGridTableStyle table_style;

	private string header_text;

	private string mapping_name;

	private string null_text;

	private PropertyDescriptor property_descriptor;

	private bool _readonly;

	private int width;

	internal bool is_default;

	internal DataGrid grid;

	private DataGridColumnHeaderAccessibleObject accesible_object;

	private static string def_null_text = "(null)";

	private ArrowDrawing arrow_drawing;

	internal bool bound;

	private static object AlignmentChangedEvent;

	private static object FontChangedEvent;

	private static object HeaderTextChangedEvent;

	private static object MappingNameChangedEvent;

	private static object NullTextChangedEvent;

	private static object PropertyDescriptorChangedEvent;

	private static object ReadOnlyChangedEvent;

	private static object WidthChangedEvent;

	[Localizable(true)]
	[DefaultValue(HorizontalAlignment.Left)]
	public virtual HorizontalAlignment Alignment
	{
		get
		{
			return alignment;
		}
		set
		{
			if (value != alignment)
			{
				alignment = value;
				if (table_style != null && table_style.DataGrid != null)
				{
					table_style.DataGrid.Invalidate();
				}
				((EventHandler)base.Events[AlignmentChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	public virtual DataGridTableStyle DataGridTableStyle => table_style;

	protected int FontHeight
	{
		get
		{
			if (fontheight != -1)
			{
				return fontheight;
			}
			if (table_style != null)
			{
				return -1;
			}
			return -1;
		}
	}

	[Browsable(false)]
	public AccessibleObject HeaderAccessibleObject => accesible_object;

	[Localizable(true)]
	public virtual string HeaderText
	{
		get
		{
			return header_text;
		}
		set
		{
			if (value != header_text)
			{
				header_text = value;
				Invalidate();
				((EventHandler)base.Events[HeaderTextChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	[Editor("System.Windows.Forms.Design.DataGridColumnStyleMappingNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[DefaultValue("")]
	[Localizable(true)]
	public string MappingName
	{
		get
		{
			return mapping_name;
		}
		set
		{
			if (value != mapping_name)
			{
				mapping_name = value;
				((EventHandler)base.Events[MappingNameChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	[Localizable(true)]
	public virtual string NullText
	{
		get
		{
			return null_text;
		}
		set
		{
			if (value != null_text)
			{
				null_text = value;
				if (table_style != null && table_style.DataGrid != null)
				{
					table_style.DataGrid.Invalidate();
				}
				((EventHandler)base.Events[NullTextChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DefaultValue(null)]
	public virtual PropertyDescriptor PropertyDescriptor
	{
		get
		{
			return property_descriptor;
		}
		set
		{
			if (value != property_descriptor)
			{
				property_descriptor = value;
				((EventHandler)base.Events[PropertyDescriptorChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ReadOnly
	{
		get
		{
			return _readonly;
		}
		set
		{
			if (value != _readonly)
			{
				_readonly = value;
				if (table_style != null && table_style.DataGrid != null)
				{
					table_style.DataGrid.CalcAreasAndInvalidate();
				}
				((EventHandler)base.Events[ReadOnlyChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	[Localizable(true)]
	[DefaultValue(100)]
	public virtual int Width
	{
		get
		{
			return width;
		}
		set
		{
			if (value != width)
			{
				width = value;
				if (table_style != null && table_style.DataGrid != null)
				{
					table_style.DataGrid.CalcAreasAndInvalidate();
				}
				((EventHandler)base.Events[WidthChanged])?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	internal ArrowDrawing ArrowDrawingMode
	{
		get
		{
			return arrow_drawing;
		}
		set
		{
			arrow_drawing = value;
		}
	}

	internal bool TableStyleReadOnly => table_style != null && table_style.ReadOnly;

	internal DataGridTableStyle TableStyle
	{
		set
		{
			table_style = value;
		}
	}

	internal bool IsDefault => is_default;

	public event EventHandler AlignmentChanged
	{
		add
		{
			base.Events.AddHandler(AlignmentChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AlignmentChangedEvent, value);
		}
	}

	public event EventHandler FontChanged
	{
		add
		{
			base.Events.AddHandler(FontChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(FontChangedEvent, value);
		}
	}

	public event EventHandler HeaderTextChanged
	{
		add
		{
			base.Events.AddHandler(HeaderTextChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(HeaderTextChangedEvent, value);
		}
	}

	public event EventHandler MappingNameChanged
	{
		add
		{
			base.Events.AddHandler(MappingNameChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(MappingNameChangedEvent, value);
		}
	}

	public event EventHandler NullTextChanged
	{
		add
		{
			base.Events.AddHandler(NullTextChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(NullTextChangedEvent, value);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	public event EventHandler PropertyDescriptorChanged
	{
		add
		{
			base.Events.AddHandler(PropertyDescriptorChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(PropertyDescriptorChangedEvent, value);
		}
	}

	public event EventHandler ReadOnlyChanged
	{
		add
		{
			base.Events.AddHandler(ReadOnlyChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ReadOnlyChangedEvent, value);
		}
	}

	public event EventHandler WidthChanged
	{
		add
		{
			base.Events.AddHandler(WidthChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(WidthChangedEvent, value);
		}
	}

	public DataGridColumnStyle()
		: this(null)
	{
	}

	public DataGridColumnStyle(PropertyDescriptor prop)
	{
		property_descriptor = prop;
		fontheight = -1;
		table_style = null;
		header_text = string.Empty;
		mapping_name = string.Empty;
		null_text = def_null_text;
		accesible_object = new DataGridColumnHeaderAccessibleObject(this);
		_readonly = prop?.IsReadOnly ?? false;
		width = -1;
		grid = null;
		is_default = false;
		alignment = HorizontalAlignment.Left;
	}

	static DataGridColumnStyle()
	{
		AlignmentChanged = new object();
		FontChanged = new object();
		HeaderTextChanged = new object();
		MappingNameChanged = new object();
		NullTextChanged = new object();
		PropertyDescriptorChanged = new object();
		ReadOnlyChanged = new object();
		WidthChanged = new object();
	}

	void IDataGridColumnStyleEditingNotificationService.ColumnStartedEditing(Control editingControl)
	{
		ColumnStartedEditing(editingControl);
	}

	protected internal abstract void Abort(int rowNum);

	[System.MonoTODO("Will not suspend updates")]
	protected void BeginUpdate()
	{
	}

	protected void CheckValidDataSource(CurrencyManager value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("CurrencyManager cannot be null");
		}
		if (property_descriptor == null)
		{
			property_descriptor = value.GetItemProperties()[mapping_name];
			if (property_descriptor == null)
			{
				throw new InvalidOperationException("The PropertyDescriptor for this column is a null reference");
			}
		}
	}

	protected internal virtual void ColumnStartedEditing(Control editingControl)
	{
	}

	protected internal abstract bool Commit(CurrencyManager dataSource, int rowNum);

	protected internal virtual void ConcedeFocus()
	{
	}

	protected virtual AccessibleObject CreateHeaderAccessibleObject()
	{
		return new DataGridColumnHeaderAccessibleObject(this);
	}

	protected internal virtual void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly)
	{
		Edit(source, rowNum, bounds, readOnly, string.Empty);
	}

	protected internal virtual void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText)
	{
		Edit(source, rowNum, bounds, readOnly, displayText, cellIsVisible: true);
	}

	protected internal abstract void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible);

	protected void EndUpdate()
	{
	}

	protected internal virtual void EnterNullValue()
	{
	}

	protected internal virtual object GetColumnValueAtRow(CurrencyManager source, int rowNum)
	{
		CheckValidDataSource(source);
		if (rowNum >= source.Count)
		{
			return DBNull.Value;
		}
		return property_descriptor.GetValue(source[rowNum]);
	}

	protected internal abstract int GetMinimumHeight();

	protected internal abstract int GetPreferredHeight(Graphics g, object value);

	protected internal abstract Size GetPreferredSize(Graphics g, object value);

	protected virtual void Invalidate()
	{
		if (grid != null)
		{
			grid.InvalidateColumn(this);
		}
	}

	protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum);

	protected internal abstract void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight);

	protected internal virtual void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
	{
	}

	protected internal virtual void ReleaseHostedControl()
	{
	}

	public void ResetHeaderText()
	{
		HeaderText = string.Empty;
	}

	protected internal virtual void SetColumnValueAtRow(CurrencyManager source, int rowNum, object value)
	{
		CheckValidDataSource(source);
		if (source[rowNum] is IEditableObject editableObject)
		{
			editableObject.BeginEdit();
		}
		property_descriptor.SetValue(source[rowNum], value);
	}

	protected virtual void SetDataGrid(DataGrid value)
	{
		grid = value;
		property_descriptor = null;
	}

	protected virtual void SetDataGridInColumn(DataGrid value)
	{
		SetDataGrid(value);
	}

	internal void SetDataGridInternal(DataGrid value)
	{
		SetDataGridInColumn(value);
	}

	protected internal virtual void UpdateUI(CurrencyManager source, int rowNum, string displayText)
	{
	}

	internal virtual void OnMouseDown(MouseEventArgs e, int row, int column)
	{
	}

	internal virtual void OnKeyDown(KeyEventArgs ke, int row, int column)
	{
	}

	internal void PaintHeader(Graphics g, Rectangle bounds, int colNum)
	{
		ThemeEngine.Current.DataGridPaintColumnHeader(g, bounds, grid, colNum);
	}

	internal void PaintNewRow(Graphics g, Rectangle bounds, Brush backBrush, Brush foreBrush)
	{
		g.FillRectangle(backBrush, bounds);
		PaintGridLine(g, bounds);
	}

	internal void PaintGridLine(Graphics g, Rectangle bounds)
	{
		if (table_style.CurrentGridLineStyle == DataGridLineStyle.Solid)
		{
			g.DrawLine(ThemeEngine.Current.ResPool.GetPen(table_style.CurrentGridLineColor), bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
			g.DrawLine(ThemeEngine.Current.ResPool.GetPen(table_style.CurrentGridLineColor), bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height);
		}
	}
}
