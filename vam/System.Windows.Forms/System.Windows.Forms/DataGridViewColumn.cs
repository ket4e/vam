using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms;

[DesignTimeVisible(false)]
[ToolboxItem("")]
[TypeConverter(typeof(DataGridViewColumnConverter))]
[Designer("System.Windows.Forms.Design.DataGridViewColumnDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
public class DataGridViewColumn : DataGridViewBand, IDisposable, IComponent
{
	private bool auto_generated;

	private DataGridViewAutoSizeColumnMode autoSizeMode;

	private DataGridViewCell cellTemplate;

	private ContextMenuStrip contextMenuStrip;

	private string dataPropertyName;

	private int displayIndex;

	private int dividerWidth;

	private float fillWeight;

	private bool frozen;

	private DataGridViewColumnHeaderCell headerCell;

	private bool isDataBound;

	private int minimumWidth = 5;

	private string name = string.Empty;

	private bool readOnly;

	private ISite site;

	private DataGridViewColumnSortMode sortMode;

	private string toolTipText;

	private Type valueType;

	private bool visible = true;

	private int width = 100;

	private int dataColumnIndex;

	private bool headerTextSet;

	[DefaultValue(DataGridViewAutoSizeColumnMode.NotSet)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public DataGridViewAutoSizeColumnMode AutoSizeMode
	{
		get
		{
			return autoSizeMode;
		}
		set
		{
			if (autoSizeMode != value)
			{
				DataGridViewAutoSizeColumnMode previousMode = autoSizeMode;
				autoSizeMode = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnAutoSizeColumnModeChanged(new DataGridViewAutoSizeColumnModeEventArgs(this, previousMode));
					base.DataGridView.AutoResizeColumnsInternal();
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public virtual DataGridViewCell CellTemplate
	{
		get
		{
			return cellTemplate;
		}
		set
		{
			cellTemplate = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public Type CellType
	{
		get
		{
			if (cellTemplate == null)
			{
				return null;
			}
			return cellTemplate.GetType();
		}
	}

	[DefaultValue(null)]
	public override ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return contextMenuStrip;
		}
		set
		{
			if (contextMenuStrip != value)
			{
				contextMenuStrip = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnColumnContextMenuStripChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[DefaultValue("")]
	[TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Editor("System.Windows.Forms.Design.DataGridViewColumnDataPropertyNameEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	[Browsable(true)]
	public string DataPropertyName
	{
		get
		{
			return dataPropertyName;
		}
		set
		{
			if (dataPropertyName != value)
			{
				dataPropertyName = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnColumnDataPropertyNameChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[Browsable(true)]
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
					base.DataGridView.OnColumnDefaultCellStyleChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int DisplayIndex
	{
		get
		{
			if (displayIndex < 0)
			{
				return base.Index;
			}
			return displayIndex;
		}
		set
		{
			if (displayIndex != value)
			{
				if (value < 0 || value > int.MaxValue)
				{
					throw new ArgumentOutOfRangeException("DisplayIndex is out of range");
				}
				displayIndex = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.Columns.RegenerateSortedList();
					base.DataGridView.OnColumnDisplayIndexChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	internal int DataColumnIndex
	{
		get
		{
			return dataColumnIndex;
		}
		set
		{
			dataColumnIndex = value;
		}
	}

	[DefaultValue(0)]
	public int DividerWidth
	{
		get
		{
			return dividerWidth;
		}
		set
		{
			if (dividerWidth != value)
			{
				dividerWidth = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnColumnDividerWidthChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[DefaultValue(100)]
	public float FillWeight
	{
		get
		{
			return fillWeight;
		}
		set
		{
			fillWeight = value;
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[DefaultValue(false)]
	public override bool Frozen
	{
		get
		{
			return frozen;
		}
		set
		{
			frozen = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public DataGridViewColumnHeaderCell HeaderCell
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
				if (base.DataGridView != null)
				{
					base.DataGridView.OnColumnHeaderCellChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[Localizable(true)]
	public string HeaderText
	{
		get
		{
			if (headerCell.Value == null)
			{
				return string.Empty;
			}
			return (string)headerCell.Value;
		}
		set
		{
			headerCell.Value = value;
			headerTextSet = true;
		}
	}

	internal bool AutoGenerated
	{
		get
		{
			return auto_generated;
		}
		set
		{
			auto_generated = value;
		}
	}

	internal bool HeaderTextSet => headerTextSet;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public DataGridViewAutoSizeColumnMode InheritedAutoSizeMode
	{
		get
		{
			if (base.DataGridView == null)
			{
				return autoSizeMode;
			}
			if (autoSizeMode != 0)
			{
				return autoSizeMode;
			}
			return base.DataGridView.AutoSizeColumnsMode switch
			{
				DataGridViewAutoSizeColumnsMode.AllCells => DataGridViewAutoSizeColumnMode.AllCells, 
				DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader => DataGridViewAutoSizeColumnMode.AllCellsExceptHeader, 
				DataGridViewAutoSizeColumnsMode.ColumnHeader => DataGridViewAutoSizeColumnMode.ColumnHeader, 
				DataGridViewAutoSizeColumnsMode.DisplayedCells => DataGridViewAutoSizeColumnMode.DisplayedCells, 
				DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader => DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader, 
				DataGridViewAutoSizeColumnsMode.Fill => DataGridViewAutoSizeColumnMode.Fill, 
				_ => DataGridViewAutoSizeColumnMode.None, 
			};
		}
	}

	[Browsable(false)]
	public override DataGridViewCellStyle InheritedStyle
	{
		get
		{
			if (base.DataGridView == null)
			{
				return base.DefaultCellStyle;
			}
			if (base.DefaultCellStyle == null)
			{
				return base.DataGridView.DefaultCellStyle;
			}
			return base.DefaultCellStyle.Clone();
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool IsDataBound => isDataBound;

	[DefaultValue(5)]
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public int MinimumWidth
	{
		get
		{
			return minimumWidth;
		}
		set
		{
			if (minimumWidth != value)
			{
				if (value < 2 || value > int.MaxValue)
				{
					throw new ArgumentOutOfRangeException("MinimumWidth is out of range");
				}
				minimumWidth = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnColumnMinimumWidthChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[Browsable(false)]
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (name != value)
			{
				if (value == null)
				{
					name = string.Empty;
				}
				else
				{
					name = value;
				}
				if (!headerTextSet)
				{
					headerCell.Value = name;
				}
				if (base.DataGridView != null)
				{
					base.DataGridView.OnColumnNameChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	public override bool ReadOnly
	{
		get
		{
			if (base.DataGridView != null && base.DataGridView.ReadOnly)
			{
				return true;
			}
			return readOnly;
		}
		set
		{
			readOnly = value;
		}
	}

	public override DataGridViewTriState Resizable
	{
		get
		{
			return base.Resizable;
		}
		set
		{
			base.Resizable = value;
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ISite Site
	{
		get
		{
			return site;
		}
		set
		{
			site = value;
		}
	}

	[DefaultValue(DataGridViewColumnSortMode.NotSortable)]
	public DataGridViewColumnSortMode SortMode
	{
		get
		{
			return sortMode;
		}
		set
		{
			if (base.DataGridView != null && value == DataGridViewColumnSortMode.Automatic && (base.DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect || base.DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect))
			{
				throw new InvalidOperationException("Column's SortMode cannot be set to Automatic while the DataGridView control's SelectionMode is set to FullColumnSelect or ColumnHeaderSelect.");
			}
			if (sortMode != value)
			{
				sortMode = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnColumnSortModeChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[DefaultValue("")]
	[Localizable(true)]
	public string ToolTipText
	{
		get
		{
			if (toolTipText == null)
			{
				return string.Empty;
			}
			return toolTipText;
		}
		set
		{
			if (toolTipText != value)
			{
				toolTipText = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.OnColumnToolTipTextChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[DefaultValue(null)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Type ValueType
	{
		get
		{
			return valueType;
		}
		set
		{
			valueType = value;
		}
	}

	[DefaultValue(true)]
	[Localizable(true)]
	public override bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
			if (base.DataGridView != null)
			{
				base.DataGridView.Invalidate();
			}
		}
	}

	[RefreshProperties(RefreshProperties.Repaint)]
	[Localizable(true)]
	public int Width
	{
		get
		{
			return width;
		}
		set
		{
			if (width != value)
			{
				if (value < minimumWidth)
				{
					throw new ArgumentOutOfRangeException("Width is less than MinimumWidth");
				}
				width = value;
				if (base.DataGridView != null)
				{
					base.DataGridView.Invalidate();
					base.DataGridView.OnColumnWidthChanged(new DataGridViewColumnEventArgs(this));
				}
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public event EventHandler Disposed;

	public DataGridViewColumn()
	{
		cellTemplate = null;
		base.DefaultCellStyle = new DataGridViewCellStyle();
		readOnly = false;
		headerCell = new DataGridViewColumnHeaderCell();
		headerCell.SetColumnIndex(base.Index);
		headerCell.Value = string.Empty;
		displayIndex = -1;
		dataColumnIndex = -1;
		dataPropertyName = string.Empty;
		fillWeight = 100f;
		sortMode = DataGridViewColumnSortMode.NotSortable;
		SetState(DataGridViewElementStates.Visible);
	}

	public DataGridViewColumn(DataGridViewCell cellTemplate)
		: this()
	{
		this.cellTemplate = (DataGridViewCell)cellTemplate.Clone();
	}

	public override object Clone()
	{
		return MemberwiseClone();
	}

	public virtual int GetPreferredWidth(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
	{
		if (autoSizeColumnMode == DataGridViewAutoSizeColumnMode.NotSet || autoSizeColumnMode == DataGridViewAutoSizeColumnMode.None || autoSizeColumnMode == DataGridViewAutoSizeColumnMode.Fill)
		{
			throw new ArgumentException("AutoSizeColumnMode is invalid");
		}
		if (fixedHeight)
		{
			return 0;
		}
		return 0;
	}

	public override string ToString()
	{
		return Name + ", Index: " + base.Index + ".";
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposing)
		{
		}
	}

	internal override void SetDataGridView(DataGridView dataGridView)
	{
		if (sortMode == DataGridViewColumnSortMode.Automatic && dataGridView != null && dataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect)
		{
			throw new InvalidOperationException("Column's SortMode cannot be set to Automatic while the DataGridView control's SelectionMode is set to FullColumnSelect.");
		}
		base.SetDataGridView(dataGridView);
		headerCell.SetDataGridView(dataGridView);
	}

	internal override void SetIndex(int index)
	{
		base.SetIndex(index);
		headerCell.SetColumnIndex(base.Index);
	}

	internal void SetIsDataBound(bool value)
	{
		isDataBound = value;
	}

	internal override void SetState(DataGridViewElementStates state)
	{
		if (State != state)
		{
			base.SetState(state);
			if (base.DataGridView != null)
			{
				base.DataGridView.OnColumnStateChanged(new DataGridViewColumnStateChangedEventArgs(this, state));
			}
		}
	}
}
