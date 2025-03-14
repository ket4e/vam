using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms;

[Designer("System.Windows.Forms.Design.DataGridDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultEvent("Navigate")]
[ComplexBindingProperties("DataSource", "DataMember")]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
[DefaultProperty("DataSource")]
public class DataGrid : Control, ISupportInitialize, IDataGridEditingService
{
	[Flags]
	public enum HitTestType
	{
		None = 0,
		Cell = 1,
		ColumnHeader = 2,
		RowHeader = 4,
		ColumnResize = 8,
		RowResize = 0x10,
		Caption = 0x20,
		ParentRows = 0x40
	}

	public sealed class HitTestInfo
	{
		public static readonly HitTestInfo Nowhere;

		private int row;

		private int column;

		private HitTestType type;

		public int Column => column;

		public int Row => row;

		public HitTestType Type => type;

		internal HitTestInfo()
			: this(-1, -1, HitTestType.None)
		{
		}

		internal HitTestInfo(int row, int column, HitTestType type)
		{
			this.row = row;
			this.column = column;
			this.type = type;
		}

		public override bool Equals(object value)
		{
			if (!(value is HitTestInfo))
			{
				return false;
			}
			HitTestInfo hitTestInfo = (HitTestInfo)value;
			return hitTestInfo.Column == column && hitTestInfo.Row == row && hitTestInfo.Type == type;
		}

		public override int GetHashCode()
		{
			return row ^ column;
		}

		public override string ToString()
		{
			return string.Concat("{ ", type, ",", row, ",", column, "}");
		}
	}

	private const int RESIZE_HANDLE_HORIZ_SIZE = 5;

	private const int RESIZE_HANDLE_VERT_SIZE = 3;

	private static readonly Color def_background_color = ThemeEngine.Current.DataGridBackgroundColor;

	private static readonly Color def_caption_backcolor = ThemeEngine.Current.DataGridCaptionBackColor;

	private static readonly Color def_caption_forecolor = ThemeEngine.Current.DataGridCaptionForeColor;

	private static readonly Color def_parent_rows_backcolor = ThemeEngine.Current.DataGridParentRowsBackColor;

	private static readonly Color def_parent_rows_forecolor = ThemeEngine.Current.DataGridParentRowsForeColor;

	private new Color background_color;

	private Color caption_backcolor;

	private Color caption_forecolor;

	private Color parent_rows_backcolor;

	private Color parent_rows_forecolor;

	private bool caption_visible;

	private bool parent_rows_visible;

	private GridTableStylesCollection styles_collection;

	private DataGridParentRowsLabelStyle parent_rows_label_style;

	private DataGridTableStyle default_style;

	private DataGridTableStyle grid_style;

	private DataGridTableStyle current_style;

	private DataGridCell current_cell;

	private Hashtable selected_rows;

	private int selection_start;

	private bool allow_navigation;

	private int first_visible_row;

	private int first_visible_column;

	private int visible_row_count;

	private int visible_column_count;

	private Font caption_font;

	private string caption_text;

	private bool flatmode;

	private HScrollBar horiz_scrollbar;

	private VScrollBar vert_scrollbar;

	private int horiz_pixeloffset;

	internal Bitmap back_button_image;

	internal Rectangle back_button_rect;

	internal bool back_button_mouseover;

	internal bool back_button_active;

	internal Bitmap parent_rows_button_image;

	internal Rectangle parent_rows_button_rect;

	internal bool parent_rows_button_mouseover;

	internal bool parent_rows_button_active;

	private object datasource;

	private string datamember;

	private CurrencyManager list_manager;

	private bool refetch_list_manager = true;

	private bool _readonly;

	private DataGridRelationshipRow[] rows;

	private bool column_resize_active;

	private int resize_column_x;

	private int resize_column_width_delta;

	private int resize_column;

	private bool row_resize_active;

	private int resize_row_y;

	private int resize_row_height_delta;

	private int resize_row;

	private bool from_positionchanged_handler;

	private bool cursor_in_add_row;

	private bool add_row_changed;

	internal bool is_editing;

	private bool is_changing;

	private bool commit_row_changes = true;

	private bool adding_new_row;

	internal Stack data_source_stack;

	private bool setting_current_cell;

	private bool in_setdatasource;

	private static object AllowNavigationChangedEvent;

	private static object BackButtonClickEvent;

	private static object BackgroundColorChangedEvent;

	private static object BorderStyleChangedEvent;

	private static object CaptionVisibleChangedEvent;

	private static object CurrentCellChangedEvent;

	private static object DataSourceChangedEvent;

	private static object FlatModeChangedEvent;

	private static object NavigateEvent;

	private static object ParentRowsLabelStyleChangedEvent;

	private static object ParentRowsVisibleChangedEvent;

	private static object ReadOnlyChangedEvent;

	private static object RowHeaderClickEvent;

	private static object ScrollEvent;

	private static object ShowParentDetailsButtonClickEvent;

	private Rectangle parent_rows;

	private int width_of_all_columns;

	internal Rectangle caption_area;

	internal Rectangle column_headers_area;

	internal int column_headers_max_width;

	internal Rectangle row_headers_area;

	internal Rectangle cells_area;

	private bool in_calc_grid_areas;

	private static object UIACollectionChangedEvent;

	private static object UIASelectionChangedEvent;

	private static object UIAColumnHeadersVisibleChangedEvent;

	private static object UIAGridCellChangedEvent;

	[DefaultValue(true)]
	public bool AllowNavigation
	{
		get
		{
			return allow_navigation;
		}
		set
		{
			if (allow_navigation != value)
			{
				allow_navigation = value;
				OnAllowNavigationChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(true)]
	public bool AllowSorting
	{
		get
		{
			return grid_style.AllowSorting;
		}
		set
		{
			grid_style.AllowSorting = value;
		}
	}

	public Color AlternatingBackColor
	{
		get
		{
			return grid_style.AlternatingBackColor;
		}
		set
		{
			grid_style.AlternatingBackColor = value;
		}
	}

	public override Color BackColor
	{
		get
		{
			return grid_style.BackColor;
		}
		set
		{
			grid_style.BackColor = value;
		}
	}

	public Color BackgroundColor
	{
		get
		{
			return background_color;
		}
		set
		{
			if (background_color != value)
			{
				background_color = value;
				OnBackgroundColorChanged(EventArgs.Empty);
				Invalidate();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public override Image BackgroundImage
	{
		get
		{
			return base.BackgroundImage;
		}
		set
		{
			if (base.BackgroundImage != value)
			{
				base.BackgroundImage = value;
				Invalidate();
			}
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override ImageLayout BackgroundImageLayout
	{
		get
		{
			return base.BackgroundImageLayout;
		}
		set
		{
			base.BackgroundImageLayout = value;
		}
	}

	[DefaultValue(BorderStyle.Fixed3D)]
	[DispId(-504)]
	public BorderStyle BorderStyle
	{
		get
		{
			return base.InternalBorderStyle;
		}
		set
		{
			base.InternalBorderStyle = value;
			CalcAreasAndInvalidate();
			OnBorderStyleChanged(EventArgs.Empty);
		}
	}

	public Color CaptionBackColor
	{
		get
		{
			return caption_backcolor;
		}
		set
		{
			if (caption_backcolor != value)
			{
				caption_backcolor = value;
				InvalidateCaption();
			}
		}
	}

	[Localizable(true)]
	[AmbientValue(null)]
	public Font CaptionFont
	{
		get
		{
			if (caption_font == null)
			{
				return new Font(Font, FontStyle.Bold);
			}
			return caption_font;
		}
		set
		{
			if (caption_font == null || !caption_font.Equals(value))
			{
				caption_font = value;
				CalcAreasAndInvalidate();
			}
		}
	}

	public Color CaptionForeColor
	{
		get
		{
			return caption_forecolor;
		}
		set
		{
			if (caption_forecolor != value)
			{
				caption_forecolor = value;
				InvalidateCaption();
			}
		}
	}

	[Localizable(true)]
	[DefaultValue("")]
	public string CaptionText
	{
		get
		{
			return caption_text;
		}
		set
		{
			if (caption_text != value)
			{
				caption_text = value;
				InvalidateCaption();
			}
		}
	}

	[DefaultValue(true)]
	public bool CaptionVisible
	{
		get
		{
			return caption_visible;
		}
		set
		{
			if (caption_visible != value)
			{
				EndEdit();
				caption_visible = value;
				CalcAreasAndInvalidate();
				OnCaptionVisibleChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(true)]
	public bool ColumnHeadersVisible
	{
		get
		{
			return grid_style.ColumnHeadersVisible;
		}
		set
		{
			if (grid_style.ColumnHeadersVisible != value)
			{
				grid_style.ColumnHeadersVisible = value;
				OnUIAColumnHeadersVisibleChanged();
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public DataGridCell CurrentCell
	{
		get
		{
			return current_cell;
		}
		set
		{
			if (setting_current_cell)
			{
				return;
			}
			setting_current_cell = true;
			if (!base.IsHandleCreated)
			{
				setting_current_cell = false;
				throw new Exception("CurrentCell cannot be set at this time.");
			}
			if (current_cell.Equals(value))
			{
				setting_current_cell = false;
				return;
			}
			if (ReadOnly && value.RowNumber > RowsCount - 1)
			{
				value.RowNumber = RowsCount - 1;
			}
			else if (value.RowNumber > RowsCount)
			{
				value.RowNumber = RowsCount;
			}
			if (value.ColumnNumber >= CurrentTableStyle.GridColumnStyles.Count)
			{
				value.ColumnNumber = ((CurrentTableStyle.GridColumnStyles.Count != 0) ? (CurrentTableStyle.GridColumnStyles.Count - 1) : 0);
			}
			if (value.RowNumber < 0)
			{
				value.RowNumber = 0;
			}
			if (value.ColumnNumber < 0)
			{
				value.ColumnNumber = 0;
			}
			bool flag = is_changing;
			add_row_changed = add_row_changed || flag;
			EndEdit();
			if (value.RowNumber != current_cell.RowNumber)
			{
				if (!from_positionchanged_handler)
				{
					try
					{
						if (commit_row_changes)
						{
							ListManager.EndCurrentEdit();
						}
						else
						{
							ListManager.CancelCurrentEdit();
						}
					}
					catch (Exception ex)
					{
						DialogResult dialogResult = MessageBox.Show($"{ex.Message} Do you wish to correct the value?", "Error when committing the row to the original data source", MessageBoxButtons.YesNo);
						if (dialogResult == DialogResult.Yes)
						{
							InvalidateRowHeader(value.RowNumber);
							InvalidateRowHeader(current_cell.RowNumber);
							setting_current_cell = false;
							Edit();
							return;
						}
						ListManager.CancelCurrentEdit();
					}
				}
				if (value.RowNumber == RowsCount && !ListManager.AllowNew)
				{
					value.RowNumber--;
				}
			}
			int rowNumber = current_cell.RowNumber;
			current_cell = value;
			EnsureCellVisibility(value);
			if (CurrentRow == RowsCount && ListManager.AllowNew)
			{
				commit_row_changes = false;
				cursor_in_add_row = true;
				add_row_changed = false;
				adding_new_row = true;
				AddNewRow();
				adding_new_row = false;
			}
			else
			{
				cursor_in_add_row = false;
				commit_row_changes = true;
			}
			InvalidateRowHeader(rowNumber);
			InvalidateRowHeader(current_cell.RowNumber);
			list_manager.Position = current_cell.RowNumber;
			OnCurrentCellChanged(EventArgs.Empty);
			if (!from_positionchanged_handler)
			{
				Edit();
			}
			setting_current_cell = false;
		}
	}

	private int CurrentRow
	{
		get
		{
			return current_cell.RowNumber;
		}
		set
		{
			CurrentCell = new DataGridCell(value, current_cell.ColumnNumber);
		}
	}

	private int CurrentColumn
	{
		get
		{
			return current_cell.ColumnNumber;
		}
		set
		{
			CurrentCell = new DataGridCell(current_cell.RowNumber, value);
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int CurrentRowIndex
	{
		get
		{
			if (ListManager == null)
			{
				return -1;
			}
			return CurrentRow;
		}
		set
		{
			CurrentRow = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override Cursor Cursor
	{
		get
		{
			return base.Cursor;
		}
		set
		{
			base.Cursor = value;
		}
	}

	[DefaultValue(null)]
	[Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public string DataMember
	{
		get
		{
			return datamember;
		}
		set
		{
			if (BindingContext != null)
			{
				SetDataSource(datasource, value);
				return;
			}
			if (list_manager != null)
			{
				list_manager = null;
			}
			datamember = value;
			refetch_list_manager = true;
		}
	}

	[AttributeProvider(typeof(IListSource))]
	[DefaultValue(null)]
	[RefreshProperties(RefreshProperties.Repaint)]
	public object DataSource
	{
		get
		{
			return datasource;
		}
		set
		{
			if (BindingContext != null)
			{
				SetDataSource(value, (ListManager != null) ? string.Empty : datamember);
				return;
			}
			datasource = value;
			if (list_manager != null)
			{
				datamember = string.Empty;
			}
			if (list_manager != null)
			{
				list_manager = null;
			}
			refetch_list_manager = true;
		}
	}

	protected override Size DefaultSize => new Size(130, 80);

	[Browsable(false)]
	public int FirstVisibleColumn => first_visible_column;

	[DefaultValue(false)]
	public bool FlatMode
	{
		get
		{
			return flatmode;
		}
		set
		{
			if (flatmode != value)
			{
				flatmode = value;
				OnFlatModeChanged(EventArgs.Empty);
				Refresh();
			}
		}
	}

	public override Color ForeColor
	{
		get
		{
			return grid_style.ForeColor;
		}
		set
		{
			grid_style.ForeColor = value;
		}
	}

	public Color GridLineColor
	{
		get
		{
			return grid_style.GridLineColor;
		}
		set
		{
			if (value == Color.Empty)
			{
				throw new ArgumentException("Color.Empty value is invalid.");
			}
			grid_style.GridLineColor = value;
		}
	}

	[DefaultValue(DataGridLineStyle.Solid)]
	public DataGridLineStyle GridLineStyle
	{
		get
		{
			return grid_style.GridLineStyle;
		}
		set
		{
			grid_style.GridLineStyle = value;
		}
	}

	public Color HeaderBackColor
	{
		get
		{
			return grid_style.HeaderBackColor;
		}
		set
		{
			if (value == Color.Empty)
			{
				throw new ArgumentException("Color.Empty value is invalid.");
			}
			grid_style.HeaderBackColor = value;
		}
	}

	public Font HeaderFont
	{
		get
		{
			return grid_style.HeaderFont;
		}
		set
		{
			grid_style.HeaderFont = value;
		}
	}

	public Color HeaderForeColor
	{
		get
		{
			return grid_style.HeaderForeColor;
		}
		set
		{
			grid_style.HeaderForeColor = value;
		}
	}

	protected ScrollBar HorizScrollBar => horiz_scrollbar;

	internal ScrollBar HScrollBar => horiz_scrollbar;

	internal int HorizPixelOffset => horiz_pixeloffset;

	internal bool IsChanging => is_changing;

	public object this[DataGridCell cell]
	{
		get
		{
			return this[cell.RowNumber, cell.ColumnNumber];
		}
		set
		{
			this[cell.RowNumber, cell.ColumnNumber] = value;
		}
	}

	public object this[int rowIndex, int columnIndex]
	{
		get
		{
			return CurrentTableStyle.GridColumnStyles[columnIndex].GetColumnValueAtRow(ListManager, rowIndex);
		}
		set
		{
			CurrentTableStyle.GridColumnStyles[columnIndex].SetColumnValueAtRow(ListManager, rowIndex, value);
			OnUIAGridCellChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, new DataGridCell(rowIndex, columnIndex)));
		}
	}

	public Color LinkColor
	{
		get
		{
			return grid_style.LinkColor;
		}
		set
		{
			grid_style.LinkColor = value;
		}
	}

	internal Font LinkFont => new Font(Font, FontStyle.Underline);

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Color LinkHoverColor
	{
		get
		{
			return grid_style.LinkHoverColor;
		}
		set
		{
			grid_style.LinkHoverColor = value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected internal CurrencyManager ListManager
	{
		get
		{
			if (list_manager == null && refetch_list_manager)
			{
				SetDataSource(datasource, datamember);
				refetch_list_manager = false;
			}
			return list_manager;
		}
		set
		{
			throw new NotSupportedException("Operation is not supported.");
		}
	}

	public Color ParentRowsBackColor
	{
		get
		{
			return parent_rows_backcolor;
		}
		set
		{
			if (parent_rows_backcolor != value)
			{
				parent_rows_backcolor = value;
				if (parent_rows_visible)
				{
					Refresh();
				}
			}
		}
	}

	public Color ParentRowsForeColor
	{
		get
		{
			return parent_rows_forecolor;
		}
		set
		{
			if (parent_rows_forecolor != value)
			{
				parent_rows_forecolor = value;
				if (parent_rows_visible)
				{
					Refresh();
				}
			}
		}
	}

	[DefaultValue(DataGridParentRowsLabelStyle.Both)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public DataGridParentRowsLabelStyle ParentRowsLabelStyle
	{
		get
		{
			return parent_rows_label_style;
		}
		set
		{
			if (parent_rows_label_style != value)
			{
				parent_rows_label_style = value;
				if (parent_rows_visible)
				{
					Refresh();
				}
				OnParentRowsLabelStyleChanged(EventArgs.Empty);
			}
		}
	}

	[DefaultValue(true)]
	public bool ParentRowsVisible
	{
		get
		{
			return parent_rows_visible;
		}
		set
		{
			if (parent_rows_visible != value)
			{
				parent_rows_visible = value;
				CalcAreasAndInvalidate();
				OnParentRowsVisibleChanged(EventArgs.Empty);
			}
		}
	}

	[TypeConverter(typeof(DataGridPreferredColumnWidthTypeConverter))]
	[DefaultValue(75)]
	public int PreferredColumnWidth
	{
		get
		{
			return grid_style.PreferredColumnWidth;
		}
		set
		{
			grid_style.PreferredColumnWidth = value;
		}
	}

	public int PreferredRowHeight
	{
		get
		{
			return grid_style.PreferredRowHeight;
		}
		set
		{
			grid_style.PreferredRowHeight = value;
		}
	}

	[DefaultValue(false)]
	public bool ReadOnly
	{
		get
		{
			return _readonly;
		}
		set
		{
			if (_readonly != value)
			{
				_readonly = value;
				OnReadOnlyChanged(EventArgs.Empty);
				CalcAreasAndInvalidate();
			}
		}
	}

	[DefaultValue(true)]
	public bool RowHeadersVisible
	{
		get
		{
			return grid_style.RowHeadersVisible;
		}
		set
		{
			grid_style.RowHeadersVisible = value;
		}
	}

	[DefaultValue(35)]
	public int RowHeaderWidth
	{
		get
		{
			return grid_style.RowHeaderWidth;
		}
		set
		{
			grid_style.RowHeaderWidth = value;
		}
	}

	internal DataGridRelationshipRow[] DataGridRows => rows;

	public Color SelectionBackColor
	{
		get
		{
			return grid_style.SelectionBackColor;
		}
		set
		{
			grid_style.SelectionBackColor = value;
		}
	}

	public Color SelectionForeColor
	{
		get
		{
			return grid_style.SelectionForeColor;
		}
		set
		{
			grid_style.SelectionForeColor = value;
		}
	}

	public override ISite Site
	{
		get
		{
			return base.Site;
		}
		set
		{
			base.Site = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Localizable(true)]
	public GridTableStylesCollection TableStyles => styles_collection;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[Bindable(false)]
	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[Browsable(false)]
	protected ScrollBar VertScrollBar => vert_scrollbar;

	internal ScrollBar VScrollBar => vert_scrollbar;

	[Browsable(false)]
	public int VisibleColumnCount => visible_column_count;

	[Browsable(false)]
	public int VisibleRowCount => visible_row_count;

	internal DataGridTableStyle CurrentTableStyle
	{
		get
		{
			return current_style;
		}
		set
		{
			if (current_style != value)
			{
				if (current_style != null)
				{
					DisconnectTableStyleEvents();
				}
				current_style = value;
				if (current_style != null)
				{
					current_style.DataGrid = this;
					ConnectTableStyleEvents();
				}
				CalcAreasAndInvalidate();
			}
		}
	}

	internal int FirstVisibleRow => first_visible_row;

	internal int RowsCount => (ListManager != null) ? ListManager.Count : 0;

	internal int RowHeight
	{
		get
		{
			if (CurrentTableStyle.CurrentPreferredRowHeight > Font.Height + 3 + 1)
			{
				return CurrentTableStyle.CurrentPreferredRowHeight;
			}
			return Font.Height + 3 + 1;
		}
	}

	internal override bool ScaleChildrenInternal => false;

	internal bool ShowEditRow
	{
		get
		{
			if (ListManager != null && !ListManager.AllowNew)
			{
				return false;
			}
			return !_readonly;
		}
	}

	internal bool ShowParentRows => ParentRowsVisible && data_source_stack.Count > 0;

	internal Rectangle ColumnHeadersArea
	{
		get
		{
			Rectangle result = column_headers_area;
			if (CurrentTableStyle.CurrentRowHeadersVisible)
			{
				result.X += RowHeaderWidth;
				result.Width -= RowHeaderWidth;
			}
			return result;
		}
	}

	internal Rectangle RowHeadersArea => row_headers_area;

	internal Rectangle ParentRowsArea => parent_rows;

	private int VLargeChange => cells_area.Height / RowHeight;

	internal ScrollBar UIAHScrollBar => horiz_scrollbar;

	internal ScrollBar UIAVScrollBar => vert_scrollbar;

	internal DataGridTableStyle UIACurrentTableStyle => current_style;

	internal int UIASelectedRows => selected_rows.Count;

	internal Rectangle UIAColumnHeadersArea => ColumnHeadersArea;

	internal Rectangle UIACaptionArea => caption_area;

	internal Rectangle UIACellsArea => cells_area;

	internal int UIARowHeight => RowHeight;

	public event EventHandler AllowNavigationChanged
	{
		add
		{
			base.Events.AddHandler(AllowNavigationChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(AllowNavigationChangedEvent, value);
		}
	}

	public event EventHandler BackButtonClick
	{
		add
		{
			base.Events.AddHandler(BackButtonClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BackButtonClickEvent, value);
		}
	}

	public event EventHandler BackgroundColorChanged
	{
		add
		{
			base.Events.AddHandler(BackgroundColorChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BackgroundColorChangedEvent, value);
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageChanged
	{
		add
		{
			base.BackgroundImageChanged += value;
		}
		remove
		{
			base.BackgroundImageChanged -= value;
		}
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new event EventHandler BackgroundImageLayoutChanged
	{
		add
		{
			base.BackgroundImageLayoutChanged += value;
		}
		remove
		{
			base.BackgroundImageLayoutChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler TextChanged
	{
		add
		{
			base.TextChanged += value;
		}
		remove
		{
			base.TextChanged -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public new event EventHandler CursorChanged
	{
		add
		{
			base.CursorChanged += value;
		}
		remove
		{
			base.CursorChanged -= value;
		}
	}

	public event EventHandler BorderStyleChanged
	{
		add
		{
			base.Events.AddHandler(BorderStyleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(BorderStyleChangedEvent, value);
		}
	}

	public event EventHandler CaptionVisibleChanged
	{
		add
		{
			base.Events.AddHandler(CaptionVisibleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CaptionVisibleChangedEvent, value);
		}
	}

	public event EventHandler CurrentCellChanged
	{
		add
		{
			base.Events.AddHandler(CurrentCellChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CurrentCellChangedEvent, value);
		}
	}

	public event EventHandler DataSourceChanged
	{
		add
		{
			base.Events.AddHandler(DataSourceChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(DataSourceChangedEvent, value);
		}
	}

	public event EventHandler FlatModeChanged
	{
		add
		{
			base.Events.AddHandler(FlatModeChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(FlatModeChangedEvent, value);
		}
	}

	public event NavigateEventHandler Navigate
	{
		add
		{
			base.Events.AddHandler(NavigateEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(NavigateEvent, value);
		}
	}

	public event EventHandler ParentRowsLabelStyleChanged
	{
		add
		{
			base.Events.AddHandler(ParentRowsLabelStyleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ParentRowsLabelStyleChangedEvent, value);
		}
	}

	public event EventHandler ParentRowsVisibleChanged
	{
		add
		{
			base.Events.AddHandler(ParentRowsVisibleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ParentRowsVisibleChangedEvent, value);
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

	protected event EventHandler RowHeaderClick
	{
		add
		{
			base.Events.AddHandler(RowHeaderClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(RowHeaderClickEvent, value);
		}
	}

	public event EventHandler Scroll
	{
		add
		{
			base.Events.AddHandler(ScrollEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ScrollEvent, value);
		}
	}

	public event EventHandler ShowParentDetailsButtonClick
	{
		add
		{
			base.Events.AddHandler(ShowParentDetailsButtonClickEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(ShowParentDetailsButtonClickEvent, value);
		}
	}

	internal event CollectionChangeEventHandler UIACollectionChanged
	{
		add
		{
			base.Events.AddHandler(UIACollectionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIACollectionChangedEvent, value);
		}
	}

	internal event CollectionChangeEventHandler UIASelectionChanged
	{
		add
		{
			base.Events.AddHandler(UIASelectionChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIASelectionChangedEvent, value);
		}
	}

	internal event EventHandler UIAColumnHeadersVisibleChanged
	{
		add
		{
			base.Events.AddHandler(UIAColumnHeadersVisibleChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAColumnHeadersVisibleChangedEvent, value);
		}
	}

	internal event CollectionChangeEventHandler UIAGridCellChanged
	{
		add
		{
			base.Events.AddHandler(UIAGridCellChangedEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(UIAGridCellChangedEvent, value);
		}
	}

	public DataGrid()
	{
		allow_navigation = true;
		background_color = def_background_color;
		border_style = BorderStyle.Fixed3D;
		caption_backcolor = def_caption_backcolor;
		caption_forecolor = def_caption_forecolor;
		caption_text = string.Empty;
		caption_visible = true;
		datamember = string.Empty;
		parent_rows_backcolor = def_parent_rows_backcolor;
		parent_rows_forecolor = def_parent_rows_forecolor;
		parent_rows_visible = true;
		current_cell = default(DataGridCell);
		parent_rows_label_style = DataGridParentRowsLabelStyle.Both;
		selected_rows = new Hashtable();
		selection_start = -1;
		rows = new DataGridRelationshipRow[0];
		default_style = new DataGridTableStyle(isDefaultTableStyle: true);
		grid_style = new DataGridTableStyle();
		styles_collection = new GridTableStylesCollection(this);
		styles_collection.CollectionChanged += OnTableStylesCollectionChanged;
		CurrentTableStyle = grid_style;
		horiz_scrollbar = new ImplicitHScrollBar();
		horiz_scrollbar.Scroll += GridHScrolled;
		vert_scrollbar = new ImplicitVScrollBar();
		vert_scrollbar.Scroll += GridVScrolled;
		SetStyle(ControlStyles.UserMouse, value: true);
		data_source_stack = new Stack();
		back_button_image = ResourceImageLoader.Get("go-previous.png");
		back_button_image.MakeTransparent(Color.Transparent);
		parent_rows_button_image = ResourceImageLoader.Get("go-top.png");
		parent_rows_button_image.MakeTransparent(Color.Transparent);
	}

	static DataGrid()
	{
		AllowNavigationChanged = new object();
		BackButtonClick = new object();
		BackgroundColorChanged = new object();
		BorderStyleChanged = new object();
		CaptionVisibleChanged = new object();
		CurrentCellChanged = new object();
		DataSourceChanged = new object();
		FlatModeChanged = new object();
		Navigate = new object();
		ParentRowsLabelStyleChanged = new object();
		ParentRowsVisibleChanged = new object();
		ReadOnlyChanged = new object();
		RowHeaderClick = new object();
		Scroll = new object();
		ShowParentDetailsButtonClick = new object();
		UIACollectionChanged = new object();
		UIASelectionChanged = new object();
		UIAColumnHeadersVisibleChanged = new object();
		UIAGridCellChanged = new object();
	}

	internal void EditRowChanged(DataGridColumnStyle column_style)
	{
		if (cursor_in_add_row && !commit_row_changes)
		{
			commit_row_changes = true;
			RecreateDataGridRows(recalc: true);
		}
	}

	private void AbortEditing()
	{
		if (is_changing)
		{
			CurrentTableStyle.GridColumnStyles[current_cell.ColumnNumber].Abort(current_cell.RowNumber);
			is_changing = false;
			InvalidateRowHeader(current_cell.RowNumber);
		}
	}

	public bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber)
	{
		if (is_changing)
		{
			return false;
		}
		int num = CurrentTableStyle.GridColumnStyles.IndexOf(gridColumn);
		if (num < 0)
		{
			return false;
		}
		CurrentCell = new DataGridCell(rowNumber, num);
		Edit();
		return true;
	}

	public void BeginInit()
	{
	}

	protected virtual void CancelEditing()
	{
		if (CurrentTableStyle.GridColumnStyles.Count == 0)
		{
			return;
		}
		CurrentTableStyle.GridColumnStyles[current_cell.ColumnNumber].ConcedeFocus();
		if (is_changing)
		{
			if (current_cell.ColumnNumber < CurrentTableStyle.GridColumnStyles.Count)
			{
				CurrentTableStyle.GridColumnStyles[current_cell.ColumnNumber].Abort(current_cell.RowNumber);
			}
			InvalidateRowHeader(current_cell.RowNumber);
		}
		if (cursor_in_add_row && !is_changing)
		{
			ListManager.CancelCurrentEdit();
		}
		is_changing = false;
		is_editing = false;
	}

	public void Collapse(int row)
	{
		if (rows[row].IsExpanded)
		{
			SuspendLayout();
			rows[row].IsExpanded = false;
			for (int i = 1; i < rows.Length - row; i++)
			{
				rows[row + i].VerticalOffset -= rows[row].RelationHeight;
			}
			rows[row].height -= rows[row].RelationHeight;
			rows[row].RelationHeight = 0;
			ResumeLayout(performLayout: false);
			CalcAreasAndInvalidate();
		}
	}

	protected internal virtual void ColumnStartedEditing(Control editingControl)
	{
		ColumnStartedEditing(editingControl.Bounds);
	}

	protected internal virtual void ColumnStartedEditing(Rectangle bounds)
	{
		bool flag = !is_changing;
		is_changing = true;
		if (cursor_in_add_row && flag)
		{
			RecreateDataGridRows(recalc: true);
		}
		if (flag)
		{
			InvalidateRowHeader(CurrentRow);
		}
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return base.CreateAccessibilityInstance();
	}

	protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop)
	{
		return CreateGridColumn(prop, isDefault: false);
	}

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	protected virtual DataGridColumnStyle CreateGridColumn(PropertyDescriptor prop, bool isDefault)
	{
		throw new NotImplementedException();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	public bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort)
	{
		if (shouldAbort || _readonly || gridColumn.TableStyleReadOnly || gridColumn.ReadOnly)
		{
			gridColumn.Abort(rowNumber);
		}
		else
		{
			gridColumn.Commit(ListManager, rowNumber);
			gridColumn.ConcedeFocus();
		}
		if (is_editing || is_changing)
		{
			is_editing = false;
			is_changing = false;
			InvalidateRowHeader(rowNumber);
		}
		return true;
	}

	public void EndInit()
	{
		if (grid_style != null)
		{
			grid_style.DataGrid = this;
		}
	}

	public void Expand(int row)
	{
		if (rows[row].IsExpanded)
		{
			return;
		}
		rows[row].IsExpanded = true;
		string[] relations = CurrentTableStyle.Relations;
		StringBuilder stringBuilder = new StringBuilder(string.Empty);
		for (int i = 0; i < relations.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append("\n");
			}
			stringBuilder.Append(relations[i]);
		}
		string text = stringBuilder.ToString();
		SizeF sizeF = TextRenderer.MeasureString(text, LinkFont);
		rows[row].relation_area = new Rectangle(cells_area.X + 1, 0, (int)sizeF.Width + 4, Font.Height * relations.Length);
		for (int i = 1; i < rows.Length - row; i++)
		{
			rows[row + i].VerticalOffset += rows[row].relation_area.Height;
		}
		rows[row].height += rows[row].relation_area.Height;
		rows[row].RelationHeight = rows[row].relation_area.Height;
		CalcAreasAndInvalidate();
	}

	public Rectangle GetCellBounds(DataGridCell dgc)
	{
		return GetCellBounds(dgc.RowNumber, dgc.ColumnNumber);
	}

	public Rectangle GetCellBounds(int row, int col)
	{
		Rectangle result = default(Rectangle);
		result.Width = CurrentTableStyle.GridColumnStyles[col].Width;
		result.Height = rows[row].Height - rows[row].RelationHeight;
		result.Y = cells_area.Y + rows[row].VerticalOffset - rows[FirstVisibleRow].VerticalOffset;
		int columnStartingPixel = GetColumnStartingPixel(col);
		result.X = cells_area.X + columnStartingPixel - horiz_pixeloffset;
		return result;
	}

	public Rectangle GetCurrentCellBounds()
	{
		return GetCellBounds(current_cell.RowNumber, current_cell.ColumnNumber);
	}

	protected virtual string GetOutputTextDelimiter()
	{
		return string.Empty;
	}

	protected virtual void GridHScrolled(object sender, ScrollEventArgs se)
	{
		if (se.NewValue != horiz_pixeloffset && se.Type != ScrollEventType.EndScroll)
		{
			ScrollToColumnInPixels(se.NewValue);
		}
	}

	protected virtual void GridVScrolled(object sender, ScrollEventArgs se)
	{
		int num = first_visible_row;
		first_visible_row = se.NewValue;
		if (first_visible_row != num)
		{
			UpdateVisibleRowCount();
			if (first_visible_row != num)
			{
				ScrollToRow(num, first_visible_row);
			}
		}
	}

	public HitTestInfo HitTest(Point position)
	{
		return HitTest(position.X, position.Y);
	}

	public HitTestInfo HitTest(int x, int y)
	{
		if (column_headers_area.Contains(x, y))
		{
			int num = x + horiz_pixeloffset;
			int column_x;
			int num2 = FromPixelToColumn(num, out column_x);
			if (num2 == -1)
			{
				return new HitTestInfo(-1, -1, HitTestType.None);
			}
			if (column_x + CurrentTableStyle.GridColumnStyles[num2].Width - num < 5 && num2 < CurrentTableStyle.GridColumnStyles.Count)
			{
				return new HitTestInfo(-1, num2, HitTestType.ColumnResize);
			}
			return new HitTestInfo(-1, num2, HitTestType.ColumnHeader);
		}
		if (row_headers_area.Contains(x, y))
		{
			int num3 = FirstVisibleRow + VisibleRowCount;
			for (int i = FirstVisibleRow; i < num3; i++)
			{
				int num4 = cells_area.Y + rows[i].VerticalOffset - rows[FirstVisibleRow].VerticalOffset;
				if (y <= num4 + rows[i].Height)
				{
					if (num4 + rows[i].Height - y < 3)
					{
						return new HitTestInfo(i, -1, HitTestType.RowResize);
					}
					return new HitTestInfo(i, -1, HitTestType.RowHeader);
				}
			}
		}
		if (caption_area.Contains(x, y))
		{
			return new HitTestInfo(-1, -1, HitTestType.Caption);
		}
		if (parent_rows.Contains(x, y))
		{
			return new HitTestInfo(-1, -1, HitTestType.ParentRows);
		}
		int num5 = FirstVisibleRow + VisibleRowCount;
		for (int j = FirstVisibleRow; j < num5; j++)
		{
			int num6 = cells_area.Y + rows[j].VerticalOffset - rows[FirstVisibleRow].VerticalOffset;
			if (y > num6 + rows[j].Height)
			{
				continue;
			}
			int num7 = first_visible_column + visible_column_count;
			if (num7 > 0)
			{
				for (int k = first_visible_column; k < num7; k++)
				{
					if (CurrentTableStyle.GridColumnStyles[k].bound)
					{
						int columnStartingPixel = GetColumnStartingPixel(k);
						int num8 = cells_area.X + columnStartingPixel - horiz_pixeloffset;
						int width = CurrentTableStyle.GridColumnStyles[k].Width;
						if (x <= num8 + width)
						{
							return new HitTestInfo(j, k, HitTestType.Cell);
						}
					}
				}
				break;
			}
			if (CurrentTableStyle.HasRelations && x < rows[j].relation_area.X + rows[j].relation_area.Width)
			{
				return new HitTestInfo(j, 0, HitTestType.Cell);
			}
			break;
		}
		return new HitTestInfo();
	}

	public bool IsExpanded(int rowNumber)
	{
		return rows[rowNumber].IsExpanded;
	}

	public bool IsSelected(int row)
	{
		return rows[row].IsSelected;
	}

	public void NavigateBack()
	{
		if (data_source_stack.Count != 0)
		{
			DataGridDataSource dataGridDataSource = (DataGridDataSource)data_source_stack.Pop();
			list_manager = dataGridDataSource.list_manager;
			rows = dataGridDataSource.Rows;
			selected_rows = dataGridDataSource.SelectedRows;
			selection_start = dataGridDataSource.SelectionStart;
			SetDataSource(dataGridDataSource.data_source, dataGridDataSource.data_member);
			CurrentCell = dataGridDataSource.current;
		}
	}

	public void NavigateTo(int rowNumber, string relationName)
	{
		if (allow_navigation)
		{
			DataGridDataSource dataGridDataSource = new DataGridDataSource(this, list_manager, datasource, datamember, list_manager.Current, CurrentCell);
			dataGridDataSource.Rows = rows;
			dataGridDataSource.SelectedRows = selected_rows;
			dataGridDataSource.SelectionStart = selection_start;
			data_source_stack.Push(dataGridDataSource);
			rows = null;
			selected_rows = new Hashtable();
			selection_start = -1;
			DataMember = $"{DataMember}.{relationName}";
			OnDataSourceChanged(EventArgs.Empty);
		}
	}

	protected virtual void OnAllowNavigationChanged(EventArgs e)
	{
		((EventHandler)base.Events[AllowNavigationChanged])?.Invoke(this, e);
	}

	protected void OnBackButtonClicked(object sender, EventArgs e)
	{
		((EventHandler)base.Events[BackButtonClick])?.Invoke(this, e);
	}

	protected override void OnBackColorChanged(EventArgs e)
	{
		base.OnBackColorChanged(e);
	}

	protected virtual void OnBackgroundColorChanged(EventArgs e)
	{
		((EventHandler)base.Events[BackgroundColorChanged])?.Invoke(this, e);
	}

	protected override void OnBindingContextChanged(EventArgs e)
	{
		base.OnBindingContextChanged(e);
		SetDataSource(datasource, datamember);
	}

	protected virtual void OnBorderStyleChanged(EventArgs e)
	{
		((EventHandler)base.Events[BorderStyleChanged])?.Invoke(this, e);
	}

	protected virtual void OnCaptionVisibleChanged(EventArgs e)
	{
		((EventHandler)base.Events[CaptionVisibleChanged])?.Invoke(this, e);
	}

	protected virtual void OnCurrentCellChanged(EventArgs e)
	{
		((EventHandler)base.Events[CurrentCellChanged])?.Invoke(this, e);
	}

	protected virtual void OnDataSourceChanged(EventArgs e)
	{
		((EventHandler)base.Events[DataSourceChanged])?.Invoke(this, e);
	}

	protected override void OnEnter(EventArgs e)
	{
		base.OnEnter(e);
		Edit();
	}

	protected virtual void OnFlatModeChanged(EventArgs e)
	{
		((EventHandler)base.Events[FlatModeChanged])?.Invoke(this, e);
	}

	protected override void OnFontChanged(EventArgs e)
	{
		CalcGridAreas();
		base.OnFontChanged(e);
	}

	protected override void OnForeColorChanged(EventArgs e)
	{
		base.OnForeColorChanged(e);
	}

	protected override void OnHandleCreated(EventArgs e)
	{
		base.OnHandleCreated(e);
		SetDataSource(datasource, datamember);
	}

	protected override void OnHandleDestroyed(EventArgs e)
	{
		base.OnHandleDestroyed(e);
	}

	protected override void OnKeyDown(KeyEventArgs ke)
	{
		base.OnKeyDown(ke);
	}

	protected override void OnKeyPress(KeyPressEventArgs kpe)
	{
		base.OnKeyPress(kpe);
	}

	protected override void OnLayout(LayoutEventArgs levent)
	{
		base.OnLayout(levent);
		CalcAreasAndInvalidate();
	}

	protected override void OnLeave(EventArgs e)
	{
		base.OnLeave(e);
		EndEdit();
		if (cursor_in_add_row)
		{
			ListManager.CancelCurrentEdit();
		}
	}

	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		bool flag = (Control.ModifierKeys & Keys.Control) != 0;
		bool flag2 = (Control.ModifierKeys & Keys.Shift) != 0;
		HitTestInfo hitTestInfo = HitTest(e.X, e.Y);
		switch (hitTestInfo.Type)
		{
		case HitTestType.Cell:
		{
			if (hitTestInfo.Row < 0 || hitTestInfo.Column < 0)
			{
				break;
			}
			if (rows[hitTestInfo.Row].IsExpanded)
			{
				Rectangle relation_area = rows[hitTestInfo.Row].relation_area;
				relation_area.Y = rows[hitTestInfo.Row].VerticalOffset + cells_area.Y + rows[hitTestInfo.Row].Height - rows[hitTestInfo.Row].RelationHeight;
				if (relation_area.Contains(e.X, e.Y))
				{
					int num = e.Y - relation_area.Y;
					NavigateTo(hitTestInfo.Row, CurrentTableStyle.Relations[num / LinkFont.Height]);
					break;
				}
			}
			DataGridCell currentCell = new DataGridCell(hitTestInfo.Row, hitTestInfo.Column);
			if (!currentCell.Equals(current_cell) || !is_editing)
			{
				ResetSelection();
				CurrentCell = currentCell;
				Edit();
			}
			else
			{
				CurrentTableStyle.GridColumnStyles[hitTestInfo.Column].OnMouseDown(e, hitTestInfo.Row, hitTestInfo.Column);
			}
			break;
		}
		case HitTestType.RowHeader:
		{
			bool flag3 = false;
			if (CurrentTableStyle.HasRelations && e.X > row_headers_area.X + row_headers_area.Width / 2)
			{
				if (IsExpanded(hitTestInfo.Row))
				{
					Collapse(hitTestInfo.Row);
				}
				else
				{
					Expand(hitTestInfo.Row);
				}
				flag3 = true;
			}
			CancelEditing();
			CurrentRow = hitTestInfo.Row;
			if (!flag && !flag2 && !flag3)
			{
				ResetSelection();
			}
			if ((flag2 || flag3) && selection_start != -1)
			{
				ShiftSelection(hitTestInfo.Row);
			}
			else
			{
				selection_start = hitTestInfo.Row;
				Select(hitTestInfo.Row);
			}
			OnRowHeaderClick(EventArgs.Empty);
			break;
		}
		case HitTestType.ColumnHeader:
			if (CurrentTableStyle.GridColumnStyles.Count != 0 && AllowSorting && ListManager.List is IBindingList && ListManager.Count != 0)
			{
				ListSortDirection listSortDirection = ListSortDirection.Ascending;
				PropertyDescriptor propertyDescriptor = CurrentTableStyle.GridColumnStyles[hitTestInfo.Column].PropertyDescriptor;
				IBindingList bindingList = (IBindingList)ListManager.List;
				if (bindingList.SortProperty != null)
				{
					CurrentTableStyle.GridColumnStyles[bindingList.SortProperty].ArrowDrawingMode = DataGridColumnStyle.ArrowDrawing.No;
				}
				if (propertyDescriptor == bindingList.SortProperty && bindingList.SortDirection == ListSortDirection.Ascending)
				{
					listSortDirection = ListSortDirection.Descending;
				}
				CurrentTableStyle.GridColumnStyles[hitTestInfo.Column].ArrowDrawingMode = ((listSortDirection == ListSortDirection.Ascending) ? DataGridColumnStyle.ArrowDrawing.Ascending : DataGridColumnStyle.ArrowDrawing.Descending);
				bindingList.ApplySort(propertyDescriptor, listSortDirection);
				Refresh();
				if (is_editing)
				{
					InvalidateColumn(CurrentTableStyle.GridColumnStyles[CurrentColumn]);
				}
			}
			break;
		case HitTestType.ColumnResize:
			if (e.Clicks == 2)
			{
				EndEdit();
				ColumnResize(hitTestInfo.Column);
				break;
			}
			resize_column = hitTestInfo.Column;
			column_resize_active = true;
			resize_column_x = e.X;
			resize_column_width_delta = 0;
			EndEdit();
			DrawResizeLineVert(resize_column_x);
			break;
		case HitTestType.RowResize:
			if (e.Clicks == 2)
			{
				EndEdit();
				RowResize(hitTestInfo.Row);
				break;
			}
			resize_row = hitTestInfo.Row;
			row_resize_active = true;
			resize_row_y = e.Y;
			resize_row_height_delta = 0;
			EndEdit();
			DrawResizeLineHoriz(resize_row_y);
			break;
		case HitTestType.Caption:
			if (back_button_rect.Contains(e.X, e.Y))
			{
				back_button_active = true;
				Invalidate(back_button_rect);
			}
			if (parent_rows_button_rect.Contains(e.X, e.Y))
			{
				parent_rows_button_active = true;
				Invalidate(parent_rows_button_rect);
			}
			break;
		}
	}

	protected override void OnMouseLeave(EventArgs e)
	{
		base.OnMouseLeave(e);
	}

	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		if (column_resize_active)
		{
			DrawResizeLineVert(resize_column_x + resize_column_width_delta);
			resize_column_width_delta = e.X - resize_column_x;
			DrawResizeLineVert(resize_column_x + resize_column_width_delta);
			return;
		}
		if (row_resize_active)
		{
			DrawResizeLineHoriz(resize_row_y + resize_row_height_delta);
			resize_row_height_delta = e.Y - resize_row_y;
			DrawResizeLineHoriz(resize_row_y + resize_row_height_delta);
			return;
		}
		HitTestInfo hitTestInfo = HitTest(e.X, e.Y);
		switch (hitTestInfo.Type)
		{
		case HitTestType.ColumnResize:
			Cursor = Cursors.VSplit;
			break;
		case HitTestType.RowResize:
			Cursor = Cursors.HSplit;
			break;
		case HitTestType.Caption:
			Cursor = Cursors.Default;
			if (back_button_rect.Contains(e.X, e.Y))
			{
				if (!back_button_mouseover)
				{
					Invalidate(back_button_rect);
				}
				back_button_mouseover = true;
			}
			else if (back_button_mouseover)
			{
				Invalidate(back_button_rect);
				back_button_mouseover = false;
			}
			if (parent_rows_button_rect.Contains(e.X, e.Y))
			{
				if (parent_rows_button_mouseover)
				{
					Invalidate(parent_rows_button_rect);
				}
				parent_rows_button_mouseover = true;
			}
			else if (parent_rows_button_mouseover)
			{
				Invalidate(parent_rows_button_rect);
				parent_rows_button_mouseover = false;
			}
			break;
		case HitTestType.Cell:
			if (rows[hitTestInfo.Row].IsExpanded)
			{
				Rectangle relation_area = rows[hitTestInfo.Row].relation_area;
				relation_area.Y = rows[hitTestInfo.Row].VerticalOffset + cells_area.Y + rows[hitTestInfo.Row].Height - rows[hitTestInfo.Row].RelationHeight;
				if (relation_area.Contains(e.X, e.Y))
				{
					Cursor = Cursors.Hand;
					break;
				}
			}
			Cursor = Cursors.Default;
			break;
		case HitTestType.RowHeader:
			if (e.Button == MouseButtons.Left)
			{
				ShiftSelection(hitTestInfo.Row);
			}
			Cursor = Cursors.Default;
			break;
		default:
			Cursor = Cursors.Default;
			break;
		}
	}

	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		if (column_resize_active)
		{
			column_resize_active = false;
			if (resize_column_width_delta + CurrentTableStyle.GridColumnStyles[resize_column].Width < 0)
			{
				resize_column_width_delta = -CurrentTableStyle.GridColumnStyles[resize_column].Width;
			}
			CurrentTableStyle.GridColumnStyles[resize_column].Width += resize_column_width_delta;
			width_of_all_columns += resize_column_width_delta;
			Edit();
			Invalidate();
		}
		else if (row_resize_active)
		{
			row_resize_active = false;
			if (resize_row_height_delta + rows[resize_row].Height < 0)
			{
				resize_row_height_delta = -rows[resize_row].Height;
			}
			rows[resize_row].height = rows[resize_row].Height + resize_row_height_delta;
			for (int i = resize_row + 1; i < rows.Length; i++)
			{
				rows[i].VerticalOffset += resize_row_height_delta;
			}
			Edit();
			CalcAreasAndInvalidate();
		}
		else if (back_button_active)
		{
			if (back_button_rect.Contains(e.X, e.Y))
			{
				Invalidate(back_button_rect);
				NavigateBack();
				OnBackButtonClicked(this, EventArgs.Empty);
			}
			back_button_active = false;
		}
		else if (parent_rows_button_active)
		{
			if (parent_rows_button_rect.Contains(e.X, e.Y))
			{
				Invalidate(parent_rows_button_rect);
				ParentRowsVisible = !ParentRowsVisible;
				OnShowParentDetailsButtonClicked(this, EventArgs.Empty);
			}
			parent_rows_button_active = false;
		}
	}

	protected override void OnMouseWheel(MouseEventArgs e)
	{
		base.OnMouseWheel(e);
		if ((Control.ModifierKeys & Keys.Control) != 0)
		{
			if (horiz_scrollbar.Visible)
			{
				int num = ((e.Delta <= 0) ? Math.Min(horiz_scrollbar.Maximum - horiz_scrollbar.LargeChange + 1, horiz_scrollbar.Value + horiz_scrollbar.LargeChange) : Math.Max(horiz_scrollbar.Minimum, horiz_scrollbar.Value - horiz_scrollbar.LargeChange));
				GridHScrolled(this, new ScrollEventArgs(ScrollEventType.ThumbPosition, num));
				horiz_scrollbar.Value = num;
			}
		}
		else if (vert_scrollbar.Visible)
		{
			int num = ((e.Delta <= 0) ? Math.Min(vert_scrollbar.Maximum - vert_scrollbar.LargeChange + 1, vert_scrollbar.Value + vert_scrollbar.LargeChange) : Math.Max(vert_scrollbar.Minimum, vert_scrollbar.Value - vert_scrollbar.LargeChange));
			GridVScrolled(this, new ScrollEventArgs(ScrollEventType.ThumbPosition, num));
			vert_scrollbar.Value = num;
		}
	}

	protected void OnNavigate(NavigateEventArgs e)
	{
		((EventHandler)base.Events[Navigate])?.Invoke(this, e);
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		ThemeEngine.Current.DataGridPaint(pe, this);
	}

	protected override void OnPaintBackground(PaintEventArgs ebe)
	{
	}

	protected virtual void OnParentRowsLabelStyleChanged(EventArgs e)
	{
		((EventHandler)base.Events[ParentRowsLabelStyleChanged])?.Invoke(this, e);
	}

	protected virtual void OnParentRowsVisibleChanged(EventArgs e)
	{
		((EventHandler)base.Events[ParentRowsVisibleChanged])?.Invoke(this, e);
	}

	protected virtual void OnReadOnlyChanged(EventArgs e)
	{
		((EventHandler)base.Events[ReadOnlyChanged])?.Invoke(this, e);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
	}

	protected void OnRowHeaderClick(EventArgs e)
	{
		((EventHandler)base.Events[RowHeaderClick])?.Invoke(this, e);
	}

	protected void OnScroll(EventArgs e)
	{
		((EventHandler)base.Events[Scroll])?.Invoke(this, e);
	}

	protected void OnShowParentDetailsButtonClicked(object sender, EventArgs e)
	{
		((EventHandler)base.Events[ShowParentDetailsButtonClick])?.Invoke(this, e);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		return ProcessGridKey(new KeyEventArgs(keyData));
	}

	private void UpdateSelectionAfterCursorMove(bool extend_selection)
	{
		if (extend_selection)
		{
			CancelEditing();
			ShiftSelection(CurrentRow);
		}
		else
		{
			ResetSelection();
			selection_start = CurrentRow;
		}
	}

	protected bool ProcessGridKey(KeyEventArgs ke)
	{
		bool flag = (ke.Modifiers & Keys.Control) != 0;
		bool flag2 = (ke.Modifiers & Keys.Shift) != 0;
		switch (ke.KeyCode)
		{
		case Keys.Escape:
			if (is_changing)
			{
				AbortEditing();
			}
			else
			{
				CancelEditing();
				if (cursor_in_add_row && CurrentRow > 0)
				{
					CurrentRow--;
				}
			}
			Edit();
			return true;
		case Keys.D0:
			if (flag)
			{
				if (is_editing)
				{
					CurrentTableStyle.GridColumnStyles[CurrentColumn].EnterNullValue();
				}
				return true;
			}
			return false;
		case Keys.Return:
			if (is_changing)
			{
				CurrentRow++;
			}
			return true;
		case Keys.Tab:
			if (flag2)
			{
				if (CurrentColumn > 0)
				{
					CurrentColumn--;
				}
				else if (CurrentRow > 0 && CurrentColumn == 0)
				{
					CurrentCell = new DataGridCell(CurrentRow - 1, CurrentTableStyle.GridColumnStyles.Count - 1);
				}
			}
			else if (CurrentColumn < CurrentTableStyle.GridColumnStyles.Count - 1)
			{
				CurrentColumn++;
			}
			else if (CurrentRow <= RowsCount && CurrentColumn == CurrentTableStyle.GridColumnStyles.Count - 1)
			{
				CurrentCell = new DataGridCell(CurrentRow + 1, 0);
			}
			UpdateSelectionAfterCursorMove(extend_selection: false);
			return true;
		case Keys.Right:
			if (flag)
			{
				CurrentColumn = CurrentTableStyle.GridColumnStyles.Count - 1;
			}
			else if (CurrentColumn < CurrentTableStyle.GridColumnStyles.Count - 1)
			{
				CurrentColumn++;
			}
			else if (CurrentRow < RowsCount - 1 || (CurrentRow == RowsCount - 1 && !cursor_in_add_row))
			{
				CurrentCell = new DataGridCell(CurrentRow + 1, 0);
			}
			UpdateSelectionAfterCursorMove(extend_selection: false);
			return true;
		case Keys.Left:
			if (flag)
			{
				CurrentColumn = 0;
			}
			else if (current_cell.ColumnNumber > 0)
			{
				CurrentColumn--;
			}
			else if (CurrentRow > 0)
			{
				CurrentCell = new DataGridCell(CurrentRow - 1, CurrentTableStyle.GridColumnStyles.Count - 1);
			}
			UpdateSelectionAfterCursorMove(extend_selection: false);
			return true;
		case Keys.Up:
			if (flag)
			{
				CurrentRow = 0;
			}
			else if (CurrentRow > 0)
			{
				CurrentRow--;
			}
			UpdateSelectionAfterCursorMove(flag2);
			return true;
		case Keys.Down:
			if (flag)
			{
				CurrentRow = RowsCount - 1;
			}
			else if (CurrentRow < RowsCount - 1)
			{
				CurrentRow++;
			}
			else if (CurrentRow == RowsCount - 1 && cursor_in_add_row && (add_row_changed || is_changing))
			{
				CurrentRow++;
			}
			else if (CurrentRow == RowsCount - 1 && !cursor_in_add_row && !flag2)
			{
				CurrentRow++;
			}
			UpdateSelectionAfterCursorMove(flag2);
			return true;
		case Keys.PageUp:
			if (CurrentRow > VLargeChange)
			{
				CurrentRow -= VLargeChange;
			}
			else
			{
				CurrentRow = 0;
			}
			UpdateSelectionAfterCursorMove(flag2);
			return true;
		case Keys.PageDown:
			if (CurrentRow < RowsCount - VLargeChange)
			{
				CurrentRow += VLargeChange;
			}
			else
			{
				CurrentRow = RowsCount - 1;
			}
			UpdateSelectionAfterCursorMove(flag2);
			return true;
		case Keys.Home:
			if (flag)
			{
				CurrentCell = new DataGridCell(0, 0);
			}
			else
			{
				CurrentColumn = 0;
			}
			UpdateSelectionAfterCursorMove(flag && flag2);
			return true;
		case Keys.End:
			if (flag)
			{
				CurrentCell = new DataGridCell(RowsCount - 1, CurrentTableStyle.GridColumnStyles.Count - 1);
			}
			else
			{
				CurrentColumn = CurrentTableStyle.GridColumnStyles.Count - 1;
			}
			UpdateSelectionAfterCursorMove(flag && flag2);
			return true;
		case Keys.Delete:
			if (is_editing)
			{
				return false;
			}
			if (selected_rows.Keys.Count > 0)
			{
				int[] array = new int[selected_rows.Keys.Count];
				selected_rows.Keys.CopyTo(array, 0);
				for (int num = array.Length - 1; num >= 0; num--)
				{
					ListManager.RemoveAt(array[num]);
				}
				CalcAreasAndInvalidate();
			}
			return true;
		default:
			return false;
		}
	}

	protected override bool ProcessKeyPreview(ref Message m)
	{
		if (m.Msg == 256)
		{
			Keys keyData = (Keys)m.WParam.ToInt32();
			KeyEventArgs ke = new KeyEventArgs(keyData);
			if (ProcessGridKey(ke))
			{
				return true;
			}
			if (!is_editing)
			{
				Edit();
				InvalidateRow(current_cell.RowNumber);
				return true;
			}
		}
		return base.ProcessKeyPreview(ref m);
	}

	protected bool ProcessTabKey(Keys keyData)
	{
		return false;
	}

	public void ResetAlternatingBackColor()
	{
		grid_style.AlternatingBackColor = default_style.AlternatingBackColor;
	}

	public override void ResetBackColor()
	{
		grid_style.BackColor = default_style.BackColor;
	}

	public override void ResetForeColor()
	{
		grid_style.ForeColor = default_style.ForeColor;
	}

	public void ResetGridLineColor()
	{
		grid_style.GridLineColor = default_style.GridLineColor;
	}

	public void ResetHeaderBackColor()
	{
		grid_style.HeaderBackColor = default_style.HeaderBackColor;
	}

	public void ResetHeaderFont()
	{
		grid_style.HeaderFont = null;
	}

	public void ResetHeaderForeColor()
	{
		grid_style.HeaderForeColor = default_style.HeaderForeColor;
	}

	public void ResetLinkColor()
	{
		grid_style.LinkColor = default_style.LinkColor;
	}

	public void ResetLinkHoverColor()
	{
		grid_style.LinkHoverColor = default_style.LinkHoverColor;
	}

	protected void ResetSelection()
	{
		InvalidateSelection();
		selected_rows.Clear();
		selection_start = -1;
	}

	private void InvalidateSelection()
	{
		foreach (int key in selected_rows.Keys)
		{
			rows[key].IsSelected = false;
			InvalidateRow(key);
		}
	}

	public void ResetSelectionBackColor()
	{
		grid_style.SelectionBackColor = default_style.SelectionBackColor;
	}

	public void ResetSelectionForeColor()
	{
		grid_style.SelectionForeColor = default_style.SelectionForeColor;
	}

	public void Select(int row)
	{
		EndEdit();
		if (selected_rows.Count == 0)
		{
			selection_start = row;
		}
		bool isSelected = rows[row].IsSelected;
		selected_rows[row] = true;
		rows[row].IsSelected = true;
		InvalidateRow(row);
		if (!isSelected)
		{
			OnUIASelectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Add, row));
		}
	}

	public void SetDataBinding(object dataSource, string dataMember)
	{
		SetDataSource(dataSource, dataMember);
	}

	protected virtual bool ShouldSerializeAlternatingBackColor()
	{
		return grid_style.AlternatingBackColor != default_style.AlternatingBackColor;
	}

	protected virtual bool ShouldSerializeBackgroundColor()
	{
		return background_color != def_background_color;
	}

	protected virtual bool ShouldSerializeCaptionBackColor()
	{
		return caption_backcolor != def_caption_backcolor;
	}

	protected virtual bool ShouldSerializeCaptionForeColor()
	{
		return caption_forecolor != def_caption_forecolor;
	}

	protected virtual bool ShouldSerializeGridLineColor()
	{
		return grid_style.GridLineColor != default_style.GridLineColor;
	}

	protected virtual bool ShouldSerializeHeaderBackColor()
	{
		return grid_style.HeaderBackColor != default_style.HeaderBackColor;
	}

	protected bool ShouldSerializeHeaderFont()
	{
		return grid_style.HeaderFont != default_style.HeaderFont;
	}

	protected virtual bool ShouldSerializeHeaderForeColor()
	{
		return grid_style.HeaderForeColor != default_style.HeaderForeColor;
	}

	protected virtual bool ShouldSerializeLinkHoverColor()
	{
		return grid_style.LinkHoverColor != grid_style.LinkHoverColor;
	}

	protected virtual bool ShouldSerializeParentRowsBackColor()
	{
		return parent_rows_backcolor != def_parent_rows_backcolor;
	}

	protected virtual bool ShouldSerializeParentRowsForeColor()
	{
		return parent_rows_backcolor != def_parent_rows_backcolor;
	}

	protected bool ShouldSerializePreferredRowHeight()
	{
		return grid_style.PreferredRowHeight != default_style.PreferredRowHeight;
	}

	protected bool ShouldSerializeSelectionBackColor()
	{
		return grid_style.SelectionBackColor != default_style.SelectionBackColor;
	}

	protected virtual bool ShouldSerializeSelectionForeColor()
	{
		return grid_style.SelectionForeColor != default_style.SelectionForeColor;
	}

	public void SubObjectsSiteChange(bool site)
	{
	}

	public void UnSelect(int row)
	{
		bool isSelected = rows[row].IsSelected;
		rows[row].IsSelected = false;
		selected_rows.Remove(row);
		InvalidateRow(row);
		if (!isSelected)
		{
			OnUIASelectionChangedEvent(new CollectionChangeEventArgs(CollectionChangeAction.Remove, row));
		}
	}

	internal void CalcAreasAndInvalidate()
	{
		CalcGridAreas();
		Invalidate();
	}

	private void ConnectListManagerEvents()
	{
		list_manager.MetaDataChanged += OnListManagerMetaDataChanged;
		list_manager.PositionChanged += OnListManagerPositionChanged;
		list_manager.ItemChanged += OnListManagerItemChanged;
	}

	private void DisconnectListManagerEvents()
	{
		list_manager.MetaDataChanged -= OnListManagerMetaDataChanged;
		list_manager.PositionChanged -= OnListManagerPositionChanged;
		list_manager.ItemChanged -= OnListManagerItemChanged;
	}

	private void DisconnectTableStyleEvents()
	{
		current_style.AllowSortingChanged -= TableStyleChanged;
		current_style.AlternatingBackColorChanged -= TableStyleChanged;
		current_style.BackColorChanged -= TableStyleChanged;
		current_style.ColumnHeadersVisibleChanged -= TableStyleChanged;
		current_style.ForeColorChanged -= TableStyleChanged;
		current_style.GridLineColorChanged -= TableStyleChanged;
		current_style.GridLineStyleChanged -= TableStyleChanged;
		current_style.HeaderBackColorChanged -= TableStyleChanged;
		current_style.HeaderFontChanged -= TableStyleChanged;
		current_style.HeaderForeColorChanged -= TableStyleChanged;
		current_style.LinkColorChanged -= TableStyleChanged;
		current_style.LinkHoverColorChanged -= TableStyleChanged;
		current_style.MappingNameChanged -= TableStyleChanged;
		current_style.PreferredColumnWidthChanged -= TableStyleChanged;
		current_style.PreferredRowHeightChanged -= TableStyleChanged;
		current_style.ReadOnlyChanged -= TableStyleChanged;
		current_style.RowHeadersVisibleChanged -= TableStyleChanged;
		current_style.RowHeaderWidthChanged -= TableStyleChanged;
		current_style.SelectionBackColorChanged -= TableStyleChanged;
		current_style.SelectionForeColorChanged -= TableStyleChanged;
	}

	private void ConnectTableStyleEvents()
	{
		current_style.AllowSortingChanged += TableStyleChanged;
		current_style.AlternatingBackColorChanged += TableStyleChanged;
		current_style.BackColorChanged += TableStyleChanged;
		current_style.ColumnHeadersVisibleChanged += TableStyleChanged;
		current_style.ForeColorChanged += TableStyleChanged;
		current_style.GridLineColorChanged += TableStyleChanged;
		current_style.GridLineStyleChanged += TableStyleChanged;
		current_style.HeaderBackColorChanged += TableStyleChanged;
		current_style.HeaderFontChanged += TableStyleChanged;
		current_style.HeaderForeColorChanged += TableStyleChanged;
		current_style.LinkColorChanged += TableStyleChanged;
		current_style.LinkHoverColorChanged += TableStyleChanged;
		current_style.MappingNameChanged += TableStyleChanged;
		current_style.PreferredColumnWidthChanged += TableStyleChanged;
		current_style.PreferredRowHeightChanged += TableStyleChanged;
		current_style.ReadOnlyChanged += TableStyleChanged;
		current_style.RowHeadersVisibleChanged += TableStyleChanged;
		current_style.RowHeaderWidthChanged += TableStyleChanged;
		current_style.SelectionBackColorChanged += TableStyleChanged;
		current_style.SelectionForeColorChanged += TableStyleChanged;
	}

	private void TableStyleChanged(object sender, EventArgs args)
	{
		EndEdit();
		CalcAreasAndInvalidate();
	}

	private void EnsureCellVisibility(DataGridCell cell)
	{
		if (cell.ColumnNumber <= first_visible_column || cell.ColumnNumber + 1 >= first_visible_column + visible_column_count)
		{
			first_visible_column = GetFirstColumnForColumnVisibility(first_visible_column, cell.ColumnNumber);
			int columnStartingPixel = GetColumnStartingPixel(first_visible_column);
			ScrollToColumnInPixels(columnStartingPixel);
			horiz_scrollbar.Value = columnStartingPixel;
			Update();
		}
		if (cell.RowNumber < first_visible_row || cell.RowNumber + 1 >= first_visible_row + visible_row_count)
		{
			if (cell.RowNumber + 1 >= first_visible_row + visible_row_count)
			{
				int old_row = first_visible_row;
				first_visible_row = 1 + cell.RowNumber - visible_row_count;
				UpdateVisibleRowCount();
				ScrollToRow(old_row, first_visible_row);
			}
			else
			{
				int old_row2 = first_visible_row;
				first_visible_row = cell.RowNumber;
				UpdateVisibleRowCount();
				ScrollToRow(old_row2, first_visible_row);
			}
			vert_scrollbar.Value = first_visible_row;
		}
	}

	private void SetDataSource(object source, string member)
	{
		SetDataSource(source, member, recreate_rows: true);
	}

	private void SetDataSource(object source, string member, bool recreate_rows)
	{
		CurrencyManager currencyManager = list_manager;
		if (in_setdatasource)
		{
			return;
		}
		in_setdatasource = true;
		if (source != null && source is IListSource && source is IList)
		{
			throw new Exception("Wrong complex data binding source");
		}
		datasource = source;
		datamember = member;
		if (is_editing)
		{
			CancelEditing();
		}
		current_cell = default(DataGridCell);
		if (list_manager != null)
		{
			DisconnectListManagerEvents();
		}
		list_manager = null;
		if (BindingContext != null && datasource != null)
		{
			list_manager = (CurrencyManager)BindingContext[datasource, datamember];
		}
		if (list_manager != null)
		{
			ConnectListManagerEvents();
		}
		if (currencyManager != list_manager)
		{
			BindColumns();
			vert_scrollbar.Value = 0;
			horiz_scrollbar.Value = 0;
			first_visible_row = 0;
			if (recreate_rows)
			{
				RecreateDataGridRows(recalc: false);
			}
		}
		CalcAreasAndInvalidate();
		in_setdatasource = false;
		OnDataSourceChanged(EventArgs.Empty);
	}

	private void RecreateDataGridRows(bool recalc)
	{
		DataGridRelationshipRow[] array = new DataGridRelationshipRow[RowsCount + (ShowEditRow ? 1 : 0)];
		int num = 0;
		if (rows != null)
		{
			num = rows.Length;
			Array.Copy(rows, 0, array, 0, (rows.Length >= array.Length) ? array.Length : rows.Length);
		}
		for (int i = num; i < array.Length; i++)
		{
			array[i] = new DataGridRelationshipRow(this);
			array[i].height = RowHeight;
			if (i > 0)
			{
				array[i].VerticalOffset = array[i - 1].VerticalOffset + array[i - 1].Height;
			}
		}
		CollectionChangeAction action = CollectionChangeAction.Refresh;
		if (rows != null)
		{
			action = ((array.Length - rows.Length > 0) ? CollectionChangeAction.Add : CollectionChangeAction.Remove);
		}
		rows = array;
		if (recalc)
		{
			CalcAreasAndInvalidate();
		}
		OnUIACollectionChangedEvent(new CollectionChangeEventArgs(action, -1));
	}

	internal void UpdateRowsFrom(DataGridRelationshipRow row)
	{
		int num = Array.IndexOf(rows, row);
		if (num != -1)
		{
			for (int i = num + 1; i < rows.Length; i++)
			{
				rows[i].VerticalOffset = rows[i - 1].VerticalOffset + rows[i - 1].Height;
			}
			CalcAreasAndInvalidate();
		}
	}

	private void BindColumns()
	{
		if (list_manager != null)
		{
			string listName = list_manager.GetListName(null);
			if (TableStyles[listName] == null)
			{
				current_style.GridColumnStyles.Clear();
				current_style.CreateColumnsForTable(onlyBind: false);
			}
			else if (CurrentTableStyle == grid_style || CurrentTableStyle.MappingName != listName)
			{
				CurrentTableStyle = styles_collection[listName];
				current_style.CreateColumnsForTable(current_style.GridColumnStyles.Count > 0);
			}
			else
			{
				current_style.CreateColumnsForTable(onlyBind: true);
			}
		}
		else
		{
			current_style.CreateColumnsForTable(onlyBind: false);
		}
	}

	private void OnListManagerMetaDataChanged(object sender, EventArgs e)
	{
		BindColumns();
		CalcAreasAndInvalidate();
	}

	private void OnListManagerPositionChanged(object sender, EventArgs e)
	{
		from_positionchanged_handler = true;
		CurrentRow = list_manager.Position;
		from_positionchanged_handler = false;
	}

	private void OnListManagerItemChanged(object sender, ItemChangedEventArgs e)
	{
		if (adding_new_row)
		{
			return;
		}
		if (e.Index == -1)
		{
			ResetSelection();
			if (rows == null || RowsCount != rows.Length - (ShowEditRow ? 1 : 0))
			{
				RecreateDataGridRows(recalc: true);
			}
		}
		else
		{
			InvalidateRow(e.Index);
		}
	}

	private void OnTableStylesCollectionChanged(object sender, CollectionChangeEventArgs e)
	{
		if (ListManager == null)
		{
			return;
		}
		string listName = ListManager.GetListName(null);
		switch (e.Action)
		{
		case CollectionChangeAction.Add:
			if (e.Element != null && string.Compare(listName, ((DataGridTableStyle)e.Element).MappingName, ignoreCase: true) == 0)
			{
				CurrentTableStyle = (DataGridTableStyle)e.Element;
				((DataGridTableStyle)e.Element).CreateColumnsForTable(CurrentTableStyle.GridColumnStyles.Count > 0);
			}
			break;
		case CollectionChangeAction.Remove:
			if (e.Element != null && string.Compare(listName, ((DataGridTableStyle)e.Element).MappingName, ignoreCase: true) == 0)
			{
				CurrentTableStyle = default_style;
				current_style.GridColumnStyles.Clear();
				current_style.CreateColumnsForTable(onlyBind: false);
			}
			break;
		case CollectionChangeAction.Refresh:
			if (CurrentTableStyle == default_style || string.Compare(listName, CurrentTableStyle.MappingName, ignoreCase: true) != 0)
			{
				DataGridTableStyle dataGridTableStyle = styles_collection[listName];
				if (dataGridTableStyle != null)
				{
					CurrentTableStyle = dataGridTableStyle;
					current_style.CreateColumnsForTable(onlyBind: false);
				}
				else
				{
					CurrentTableStyle = default_style;
					current_style.GridColumnStyles.Clear();
					current_style.CreateColumnsForTable(onlyBind: false);
				}
			}
			break;
		}
		CalcAreasAndInvalidate();
	}

	private void AddNewRow()
	{
		ListManager.EndCurrentEdit();
		ListManager.AddNew();
	}

	private void Edit()
	{
		if (CurrentTableStyle.GridColumnStyles.Count != 0 && CurrentTableStyle.GridColumnStyles[CurrentColumn].bound && (ListManager == null || ListManager.Count != 0))
		{
			is_editing = true;
			is_changing = false;
			CurrentTableStyle.GridColumnStyles[CurrentColumn].Edit(ListManager, CurrentRow, GetCellBounds(CurrentRow, CurrentColumn), _readonly, null, cellIsVisible: true);
		}
	}

	private void EndEdit()
	{
		if (CurrentTableStyle.GridColumnStyles.Count != 0 && CurrentTableStyle.GridColumnStyles[current_cell.ColumnNumber].bound)
		{
			EndEdit(CurrentTableStyle.GridColumnStyles[current_cell.ColumnNumber], current_cell.RowNumber, shouldAbort: false);
		}
	}

	private void ShiftSelection(int index)
	{
		int num = selection_start;
		ResetSelection();
		selection_start = num;
		int num2;
		int num3;
		if (index >= selection_start)
		{
			num2 = selection_start;
			num3 = index;
		}
		else
		{
			num2 = index;
			num3 = selection_start;
		}
		if (num2 == -1)
		{
			num2 = 0;
		}
		for (int i = num2; i <= num3; i++)
		{
			Select(i);
		}
	}

	private void ScrollToColumnInPixels(int pixel)
	{
		int xAmount = ((pixel <= horiz_pixeloffset) ? (horiz_pixeloffset - pixel) : (-1 * (pixel - horiz_pixeloffset)));
		Rectangle rectangle = cells_area;
		if (ColumnHeadersVisible)
		{
			rectangle.Y -= ColumnHeadersArea.Height;
			rectangle.Height += ColumnHeadersArea.Height;
		}
		horiz_pixeloffset = pixel;
		UpdateVisibleColumn();
		EndEdit();
		XplatUI.ScrollWindow(Handle, rectangle, xAmount, 0, with_children: false);
		int columnStartingPixel = GetColumnStartingPixel(CurrentColumn);
		int num = columnStartingPixel + CurrentTableStyle.GridColumnStyles[CurrentColumn].Width;
		if (columnStartingPixel >= horiz_pixeloffset && num < horiz_pixeloffset + cells_area.Width)
		{
			Edit();
		}
	}

	private void ScrollToRow(int old_row, int new_row)
	{
		int num = 0;
		if (new_row > old_row)
		{
			for (int i = old_row; i < new_row; i++)
			{
				num -= rows[i].Height;
			}
		}
		else
		{
			for (int i = new_row; i < old_row; i++)
			{
				num += rows[i].Height;
			}
		}
		if (num != 0)
		{
			Rectangle rectangle = cells_area;
			if (RowHeadersVisible)
			{
				rectangle.X -= RowHeaderWidth;
				rectangle.Width += RowHeaderWidth;
			}
			XplatUI.ScrollWindow(Handle, rectangle, 0, num, with_children: false);
			if (CurrentRow >= first_visible_row && CurrentRow < first_visible_row + visible_row_count)
			{
				Edit();
			}
		}
	}

	private void ColumnResize(int column)
	{
		CurrencyManager listManager = ListManager;
		DataGridColumnStyle dataGridColumnStyle = CurrentTableStyle.GridColumnStyles[column];
		string headerText = dataGridColumnStyle.HeaderText;
		using Graphics graphics = CreateGraphics();
		int count = listManager.Count;
		int num = (int)graphics.MeasureString(headerText, CurrentTableStyle.HeaderFont).Width + 4;
		for (int i = 0; i < count; i++)
		{
			int width = dataGridColumnStyle.GetPreferredSize(graphics, dataGridColumnStyle.GetColumnValueAtRow(listManager, i)).Width;
			if (width > num)
			{
				num = width;
			}
		}
		if (dataGridColumnStyle.Width != num)
		{
			dataGridColumnStyle.Width = num;
		}
	}

	private void RowResize(int row)
	{
		CurrencyManager listManager = ListManager;
		using Graphics g = CreateGraphics();
		GridColumnStylesCollection gridColumnStyles = CurrentTableStyle.GridColumnStyles;
		int count = gridColumnStyles.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			object columnValueAtRow = gridColumnStyles[i].GetColumnValueAtRow(listManager, row);
			num = Math.Max(gridColumnStyles[i].GetPreferredHeight(g, columnValueAtRow), num);
		}
		if (DataGridRows[row].Height != num)
		{
			DataGridRows[row].Height = num;
		}
	}

	private int CalcAllColumnsWidth()
	{
		int num = 0;
		int count = CurrentTableStyle.GridColumnStyles.Count;
		for (int i = 0; i < count; i++)
		{
			if (CurrentTableStyle.GridColumnStyles[i].bound)
			{
				num += CurrentTableStyle.GridColumnStyles[i].Width;
			}
		}
		return num;
	}

	private int FromPixelToColumn(int pixel, out int column_x)
	{
		int num = 0;
		int count = CurrentTableStyle.GridColumnStyles.Count;
		column_x = 0;
		if (count == 0)
		{
			return -1;
		}
		if (CurrentTableStyle.CurrentRowHeadersVisible)
		{
			num += row_headers_area.X + row_headers_area.Width;
			column_x += row_headers_area.X + row_headers_area.Width;
			if (pixel < num)
			{
				return -1;
			}
		}
		for (int i = 0; i < count; i++)
		{
			if (CurrentTableStyle.GridColumnStyles[i].bound)
			{
				num += CurrentTableStyle.GridColumnStyles[i].Width;
				if (pixel < num)
				{
					return i;
				}
				column_x += CurrentTableStyle.GridColumnStyles[i].Width;
			}
		}
		return count - 1;
	}

	internal int GetColumnStartingPixel(int my_col)
	{
		int num = 0;
		int count = CurrentTableStyle.GridColumnStyles.Count;
		for (int i = 0; i < count; i++)
		{
			if (CurrentTableStyle.GridColumnStyles[i].bound)
			{
				if (my_col == i)
				{
					return num;
				}
				num += CurrentTableStyle.GridColumnStyles[i].Width;
			}
		}
		return 0;
	}

	private int GetFirstColumnForColumnVisibility(int current_first_visible_column, int column)
	{
		int num = column;
		int num2 = 0;
		if (column > current_first_visible_column)
		{
			for (num = column; num >= 0; num--)
			{
				if (CurrentTableStyle.GridColumnStyles[num].bound)
				{
					num2 += CurrentTableStyle.GridColumnStyles[num].Width;
					if (num2 >= cells_area.Width)
					{
						return num + 1;
					}
				}
			}
			return 0;
		}
		return column;
	}

	private void CalcGridAreas()
	{
		if (!base.IsHandleCreated || in_calc_grid_areas)
		{
			return;
		}
		in_calc_grid_areas = true;
		horiz_pixeloffset = 0;
		CalcCaption();
		CalcParentRows();
		CalcParentButtons();
		UpdateVisibleRowCount();
		CalcRowHeaders();
		width_of_all_columns = CalcAllColumnsWidth();
		CalcColumnHeaders();
		CalcCellsArea();
		bool flag = false;
		bool flag2 = false;
		int num = cells_area.Width;
		int height = cells_area.Height;
		int num2 = RowsCount;
		if (ShowEditRow && RowsCount > 0)
		{
			num2++;
		}
		for (int i = 0; i < 3; i++)
		{
			if (flag2)
			{
				num = cells_area.Width - vert_scrollbar.Width;
			}
			if (flag)
			{
				height = cells_area.Height - horiz_scrollbar.Height;
			}
			UpdateVisibleRowCount();
			flag = width_of_all_columns > num;
			flag2 = num2 > visible_row_count;
		}
		int num3 = base.ClientRectangle.Width;
		int maximum = 0;
		int height2 = 0;
		int maximum2 = 0;
		if (flag2)
		{
			SetUpVerticalScrollBar(out height2, out maximum2);
		}
		if (flag)
		{
			SetUpHorizontalScrollBar(out maximum);
		}
		cells_area.Width = num;
		cells_area.Height = height;
		if (flag2 && flag)
		{
			if (ShowParentRows)
			{
				parent_rows.Width -= vert_scrollbar.Width;
			}
			if (!ColumnHeadersVisible && column_headers_area.X + column_headers_area.Width > vert_scrollbar.Location.X)
			{
				column_headers_area.Width -= vert_scrollbar.Width;
			}
			num3 -= vert_scrollbar.Width;
			height2 -= horiz_scrollbar.Height;
		}
		if (flag2)
		{
			if (row_headers_area.Y + row_headers_area.Height > base.ClientRectangle.Y + base.ClientRectangle.Height)
			{
				row_headers_area.Height -= horiz_scrollbar.Height;
			}
			vert_scrollbar.Size = new Size(vert_scrollbar.Width, height2);
			vert_scrollbar.Maximum = maximum2;
			base.Controls.Add(vert_scrollbar);
			vert_scrollbar.Visible = true;
		}
		else
		{
			base.Controls.Remove(vert_scrollbar);
			vert_scrollbar.Visible = false;
		}
		if (flag)
		{
			horiz_scrollbar.Size = new Size(num3, horiz_scrollbar.Height);
			horiz_scrollbar.Maximum = maximum;
			base.Controls.Add(horiz_scrollbar);
			horiz_scrollbar.Visible = true;
		}
		else
		{
			base.Controls.Remove(horiz_scrollbar);
			horiz_scrollbar.Visible = false;
		}
		UpdateVisibleColumn();
		UpdateVisibleRowCount();
		in_calc_grid_areas = false;
	}

	private void CalcCaption()
	{
		caption_area.X = base.ClientRectangle.X;
		caption_area.Y = base.ClientRectangle.Y;
		caption_area.Width = base.ClientRectangle.Width;
		if (caption_visible)
		{
			caption_area.Height = CaptionFont.Height;
			if (caption_area.Height < back_button_image.Height)
			{
				caption_area.Height = back_button_image.Height;
			}
			caption_area.Height += 2;
		}
		else
		{
			caption_area.Height = 0;
		}
	}

	private void CalcCellsArea()
	{
		cells_area.X = base.ClientRectangle.X + row_headers_area.Width;
		cells_area.Y = column_headers_area.Y + column_headers_area.Height;
		cells_area.Width = base.ClientRectangle.X + base.ClientRectangle.Width - cells_area.X;
		if (cells_area.Width < 0)
		{
			cells_area.Width = 0;
		}
		cells_area.Height = base.ClientRectangle.Y + base.ClientRectangle.Height - cells_area.Y;
		if (cells_area.Height < 0)
		{
			cells_area.Height = 0;
		}
	}

	private void CalcColumnHeaders()
	{
		column_headers_area.X = base.ClientRectangle.X;
		column_headers_area.Y = parent_rows.Y + parent_rows.Height;
		column_headers_max_width = base.ClientRectangle.X + base.ClientRectangle.Width - column_headers_area.X;
		int num = column_headers_max_width;
		if (CurrentTableStyle.CurrentRowHeadersVisible)
		{
			num -= RowHeaderWidth;
		}
		if (width_of_all_columns > num)
		{
			column_headers_area.Width = column_headers_max_width;
		}
		else
		{
			column_headers_area.Width = width_of_all_columns;
			if (CurrentTableStyle.CurrentRowHeadersVisible)
			{
				column_headers_area.Width += RowHeaderWidth;
			}
		}
		if (ColumnHeadersVisible)
		{
			column_headers_area.Height = CurrentTableStyle.HeaderFont.Height + 6;
		}
		else
		{
			column_headers_area.Height = 0;
		}
	}

	private void CalcParentRows()
	{
		parent_rows.X = base.ClientRectangle.X;
		parent_rows.Y = caption_area.Y + caption_area.Height;
		parent_rows.Width = base.ClientRectangle.Width;
		if (ShowParentRows)
		{
			parent_rows.Height = (CaptionFont.Height + 3) * data_source_stack.Count;
		}
		else
		{
			parent_rows.Height = 0;
		}
	}

	private void CalcParentButtons()
	{
		if (data_source_stack.Count > 0 && CaptionVisible)
		{
			back_button_rect = new Rectangle(base.ClientRectangle.X + base.ClientRectangle.Width - 2 * (caption_area.Height - 2) - 8, caption_area.Height / 2 - back_button_image.Height / 2, back_button_image.Width, back_button_image.Height);
			parent_rows_button_rect = new Rectangle(base.ClientRectangle.X + base.ClientRectangle.Width - (caption_area.Height - 2) - 4, caption_area.Height / 2 - parent_rows_button_image.Height / 2, parent_rows_button_image.Width, parent_rows_button_image.Height);
		}
		else
		{
			back_button_rect = (parent_rows_button_rect = Rectangle.Empty);
		}
	}

	private void CalcRowHeaders()
	{
		row_headers_area.X = base.ClientRectangle.X;
		row_headers_area.Y = column_headers_area.Y + column_headers_area.Height;
		row_headers_area.Height = base.ClientRectangle.Height + base.ClientRectangle.Y - row_headers_area.Y;
		if (CurrentTableStyle.CurrentRowHeadersVisible)
		{
			row_headers_area.Width = RowHeaderWidth;
		}
		else
		{
			row_headers_area.Width = 0;
		}
	}

	private int GetVisibleRowCount(int visibleHeight)
	{
		int num = 0;
		int i;
		for (i = FirstVisibleRow; i < rows.Length && num + rows[i].Height < visibleHeight; i++)
		{
			num += rows[i].Height;
		}
		if (i <= rows.Length - 1)
		{
			i++;
		}
		return i - FirstVisibleRow;
	}

	private void UpdateVisibleColumn()
	{
		visible_column_count = 0;
		if (CurrentTableStyle.GridColumnStyles.Count == 0)
		{
			return;
		}
		int num = horiz_pixeloffset;
		if (CurrentTableStyle.CurrentRowHeadersVisible)
		{
			num += row_headers_area.X + row_headers_area.Width;
		}
		int pixel = num + cells_area.Width;
		first_visible_column = FromPixelToColumn(num, out var column_x);
		int num2 = FromPixelToColumn(pixel, out column_x);
		for (int i = first_visible_column; i <= num2; i++)
		{
			if (CurrentTableStyle.GridColumnStyles[i].bound)
			{
				visible_column_count++;
			}
		}
		if (first_visible_column + visible_column_count < CurrentTableStyle.GridColumnStyles.Count)
		{
			visible_column_count++;
		}
	}

	private void UpdateVisibleRowCount()
	{
		visible_row_count = GetVisibleRowCount(cells_area.Height);
		CalcRowHeaders();
	}

	private void InvalidateCaption()
	{
		if (!caption_area.IsEmpty)
		{
			Invalidate(caption_area);
		}
	}

	private void InvalidateRow(int row)
	{
		if (row >= FirstVisibleRow && row <= FirstVisibleRow + VisibleRowCount)
		{
			Rectangle rc = default(Rectangle);
			rc.X = cells_area.X;
			rc.Width = width_of_all_columns;
			if (rc.Width > cells_area.Width)
			{
				rc.Width = cells_area.Width;
			}
			rc.Height = rows[row].Height;
			rc.Y = cells_area.Y + rows[row].VerticalOffset - rows[FirstVisibleRow].VerticalOffset;
			Invalidate(rc);
		}
	}

	private void InvalidateRowHeader(int row)
	{
		Rectangle rc = default(Rectangle);
		rc.X = row_headers_area.X;
		rc.Width = row_headers_area.Width;
		rc.Height = rows[row].Height;
		rc.Y = row_headers_area.Y + rows[row].VerticalOffset - rows[FirstVisibleRow].VerticalOffset;
		Invalidate(rc);
	}

	internal void InvalidateColumn(DataGridColumnStyle column)
	{
		Rectangle rc = default(Rectangle);
		int num = -1;
		num = CurrentTableStyle.GridColumnStyles.IndexOf(column);
		if (num != -1)
		{
			rc.Width = column.Width;
			int columnStartingPixel = GetColumnStartingPixel(num);
			rc.X = cells_area.X + columnStartingPixel - horiz_pixeloffset;
			rc.Y = cells_area.Y;
			rc.Height = cells_area.Height;
			Invalidate(rc);
		}
	}

	private void DrawResizeLineVert(int x)
	{
		XplatUI.DrawReversibleRectangle(Handle, new Rectangle(x, cells_area.Y, 1, cells_area.Height - 3), 2);
	}

	private void DrawResizeLineHoriz(int y)
	{
		XplatUI.DrawReversibleRectangle(Handle, new Rectangle(cells_area.X, y, cells_area.Width - 3, 1), 2);
	}

	private void SetUpHorizontalScrollBar(out int maximum)
	{
		maximum = width_of_all_columns;
		horiz_scrollbar.Location = new Point(base.ClientRectangle.X, base.ClientRectangle.Y + base.ClientRectangle.Height - horiz_scrollbar.Height);
		horiz_scrollbar.LargeChange = cells_area.Width;
	}

	private void SetUpVerticalScrollBar(out int height, out int maximum)
	{
		int y = base.ClientRectangle.Y + parent_rows.Y + parent_rows.Height;
		height = base.ClientRectangle.Height - parent_rows.Y - parent_rows.Height;
		vert_scrollbar.Location = new Point(base.ClientRectangle.X + base.ClientRectangle.Width - vert_scrollbar.Width, y);
		maximum = RowsCount;
		if (ShowEditRow && RowsCount > 0)
		{
			maximum++;
		}
		vert_scrollbar.LargeChange = VLargeChange;
	}

	internal void OnUIACollectionChangedEvent(CollectionChangeEventArgs args)
	{
		((CollectionChangeEventHandler)base.Events[UIACollectionChanged])?.Invoke(this, args);
	}

	internal void OnUIASelectionChangedEvent(CollectionChangeEventArgs args)
	{
		((CollectionChangeEventHandler)base.Events[UIASelectionChanged])?.Invoke(this, args);
	}

	internal void OnUIAColumnHeadersVisibleChanged()
	{
		((EventHandler)base.Events[UIAColumnHeadersVisibleChanged])?.Invoke(this, EventArgs.Empty);
	}

	internal void OnUIAGridCellChanged(CollectionChangeEventArgs args)
	{
		((CollectionChangeEventHandler)base.Events[UIAGridCellChanged])?.Invoke(this, args);
	}
}
