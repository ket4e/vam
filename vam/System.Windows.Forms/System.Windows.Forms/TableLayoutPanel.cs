using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[ProvideProperty("RowSpan", typeof(Control))]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[Designer("System.Windows.Forms.Design.TableLayoutPanelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
[DefaultProperty("ColumnCount")]
[ComVisible(true)]
[ProvideProperty("Row", typeof(Control))]
[ProvideProperty("ColumnSpan", typeof(Control))]
[ProvideProperty("CellPosition", typeof(Control))]
[DesignerSerializer("System.Windows.Forms.Design.TableLayoutPanelCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[ProvideProperty("Column", typeof(Control))]
[Docking(DockingBehavior.Never)]
public class TableLayoutPanel : Panel, IExtenderProvider
{
	private TableLayoutSettings settings;

	private static TableLayout layout_engine = new TableLayout();

	private TableLayoutPanelCellBorderStyle cell_border_style;

	internal Control[,] actual_positions;

	internal int[] column_widths;

	internal int[] row_heights;

	private static object CellPaintEvent;

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Localizable(true)]
	public new BorderStyle BorderStyle
	{
		get
		{
			return base.BorderStyle;
		}
		set
		{
			base.BorderStyle = value;
		}
	}

	[DefaultValue(TableLayoutPanelCellBorderStyle.None)]
	[Localizable(true)]
	public TableLayoutPanelCellBorderStyle CellBorderStyle
	{
		get
		{
			return cell_border_style;
		}
		set
		{
			if (cell_border_style != value)
			{
				cell_border_style = value;
				PerformLayout(this, "CellBorderStyle");
				Invalidate();
			}
		}
	}

	[DefaultValue(0)]
	[Localizable(true)]
	public int ColumnCount
	{
		get
		{
			return settings.ColumnCount;
		}
		set
		{
			settings.ColumnCount = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[MergableProperty(false)]
	[DisplayName("Columns")]
	[Browsable(false)]
	public TableLayoutColumnStyleCollection ColumnStyles => settings.ColumnStyles;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Browsable(false)]
	public new TableLayoutControlCollection Controls => (TableLayoutControlCollection)base.Controls;

	[DefaultValue(TableLayoutPanelGrowStyle.AddRows)]
	public TableLayoutPanelGrowStyle GrowStyle
	{
		get
		{
			return settings.GrowStyle;
		}
		set
		{
			settings.GrowStyle = value;
		}
	}

	public override LayoutEngine LayoutEngine => layout_engine;

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public TableLayoutSettings LayoutSettings
	{
		get
		{
			return settings;
		}
		set
		{
			if (value.isSerialized)
			{
				value.ColumnCount = value.ColumnStyles.Count;
				value.RowCount = value.RowStyles.Count;
				value.panel = this;
				settings = value;
				value.isSerialized = false;
				return;
			}
			throw new NotSupportedException("LayoutSettings value cannot be set directly.");
		}
	}

	[DefaultValue(0)]
	[Localizable(true)]
	public int RowCount
	{
		get
		{
			return settings.RowCount;
		}
		set
		{
			settings.RowCount = value;
		}
	}

	[DisplayName("Rows")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[MergableProperty(false)]
	[Browsable(false)]
	public TableLayoutRowStyleCollection RowStyles => settings.RowStyles;

	public event TableLayoutCellPaintEventHandler CellPaint
	{
		add
		{
			base.Events.AddHandler(CellPaintEvent, value);
		}
		remove
		{
			base.Events.RemoveHandler(CellPaintEvent, value);
		}
	}

	public TableLayoutPanel()
	{
		settings = new TableLayoutSettings(this);
		cell_border_style = TableLayoutPanelCellBorderStyle.None;
		column_widths = new int[0];
		row_heights = new int[0];
	}

	static TableLayoutPanel()
	{
		CellPaint = new object();
	}

	bool IExtenderProvider.CanExtend(object obj)
	{
		if (obj is Control && (obj as Control).Parent == this)
		{
			return true;
		}
		return false;
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DefaultValue(-1)]
	[DisplayName("Cell")]
	public TableLayoutPanelCellPosition GetCellPosition(Control control)
	{
		return settings.GetCellPosition(control);
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DisplayName("Column")]
	[DefaultValue(-1)]
	public int GetColumn(Control control)
	{
		return settings.GetColumn(control);
	}

	[DefaultValue(1)]
	[DisplayName("ColumnSpan")]
	public int GetColumnSpan(Control control)
	{
		return settings.GetColumnSpan(control);
	}

	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int[] GetColumnWidths()
	{
		return column_widths;
	}

	public Control GetControlFromPosition(int column, int row)
	{
		if (column < 0 || row < 0)
		{
			throw new ArgumentException();
		}
		TableLayoutPanelCellPosition tableLayoutPanelCellPosition = new TableLayoutPanelCellPosition(column, row);
		foreach (Control control in Controls)
		{
			if (settings.GetCellPosition(control) == tableLayoutPanelCellPosition)
			{
				return control;
			}
		}
		return null;
	}

	public TableLayoutPanelCellPosition GetPositionFromControl(Control control)
	{
		for (int i = 0; i < actual_positions.GetLength(0); i++)
		{
			for (int j = 0; j < actual_positions.GetLength(1); j++)
			{
				if (actual_positions[i, j] == control)
				{
					return new TableLayoutPanelCellPosition(i, j);
				}
			}
		}
		return new TableLayoutPanelCellPosition(-1, -1);
	}

	[DefaultValue("-1")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[DisplayName("Row")]
	public int GetRow(Control control)
	{
		return settings.GetRow(control);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public int[] GetRowHeights()
	{
		return row_heights;
	}

	[DisplayName("RowSpan")]
	[DefaultValue(1)]
	public int GetRowSpan(Control control)
	{
		return settings.GetRowSpan(control);
	}

	public void SetCellPosition(Control control, TableLayoutPanelCellPosition position)
	{
		settings.SetCellPosition(control, position);
	}

	public void SetColumn(Control control, int column)
	{
		settings.SetColumn(control, column);
	}

	public void SetColumnSpan(Control control, int value)
	{
		settings.SetColumnSpan(control, value);
	}

	public void SetRow(Control control, int row)
	{
		settings.SetRow(control, row);
	}

	public void SetRowSpan(Control control, int value)
	{
		settings.SetRowSpan(control, value);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override ControlCollection CreateControlsInstance()
	{
		return new TableLayoutControlCollection(this);
	}

	protected virtual void OnCellPaint(TableLayoutCellPaintEventArgs e)
	{
		((TableLayoutCellPaintEventHandler)base.Events[CellPaint])?.Invoke(this, e);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	protected override void OnLayout(LayoutEventArgs levent)
	{
		base.OnLayout(levent);
		Invalidate();
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
		DrawCellBorders(e);
		int cellBorderWidth = GetCellBorderWidth(CellBorderStyle);
		int num = cellBorderWidth;
		int num2 = cellBorderWidth;
		for (int i = 0; i < column_widths.Length; i++)
		{
			for (int j = 0; j < row_heights.Length; j++)
			{
				OnCellPaint(new TableLayoutCellPaintEventArgs(e.Graphics, e.ClipRectangle, new Rectangle(num, num2, column_widths[i] + cellBorderWidth, row_heights[j] + cellBorderWidth), i, j));
				num2 += row_heights[j] + cellBorderWidth;
			}
			num += column_widths[i] + cellBorderWidth;
			num2 = cellBorderWidth;
		}
	}

	protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
	{
		base.ScaleControl(factor, specified);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void ScaleCore(float dx, float dy)
	{
		base.ScaleCore(dx, dy);
	}

	internal static int GetCellBorderWidth(TableLayoutPanelCellBorderStyle style)
	{
		switch (style)
		{
		case TableLayoutPanelCellBorderStyle.Single:
			return 1;
		case TableLayoutPanelCellBorderStyle.Inset:
		case TableLayoutPanelCellBorderStyle.Outset:
			return 2;
		case TableLayoutPanelCellBorderStyle.InsetDouble:
		case TableLayoutPanelCellBorderStyle.OutsetDouble:
		case TableLayoutPanelCellBorderStyle.OutsetPartial:
			return 3;
		default:
			return 0;
		}
	}

	private void DrawCellBorders(PaintEventArgs e)
	{
		Rectangle rect = new Rectangle(Point.Empty, base.Size);
		switch (CellBorderStyle)
		{
		case TableLayoutPanelCellBorderStyle.Single:
			DrawSingleBorder(e.Graphics, rect);
			break;
		case TableLayoutPanelCellBorderStyle.Inset:
			DrawInsetBorder(e.Graphics, rect);
			break;
		case TableLayoutPanelCellBorderStyle.InsetDouble:
			DrawInsetDoubleBorder(e.Graphics, rect);
			break;
		case TableLayoutPanelCellBorderStyle.Outset:
			DrawOutsetBorder(e.Graphics, rect);
			break;
		case TableLayoutPanelCellBorderStyle.OutsetDouble:
		case TableLayoutPanelCellBorderStyle.OutsetPartial:
			DrawOutsetDoubleBorder(e.Graphics, rect);
			break;
		}
	}

	private void DrawSingleBorder(Graphics g, Rectangle rect)
	{
		ControlPaint.DrawBorder(g, rect, SystemColors.ControlDark, ButtonBorderStyle.Solid);
		int num = DisplayRectangle.X;
		int num2 = DisplayRectangle.Y;
		for (int i = 0; i < column_widths.Length - 1; i++)
		{
			num += column_widths[i] + 1;
			g.DrawLine(SystemPens.ControlDark, new Point(num, 1), new Point(num, base.Bottom - 2));
		}
		for (int j = 0; j < row_heights.Length - 1; j++)
		{
			num2 += row_heights[j] + 1;
			g.DrawLine(SystemPens.ControlDark, new Point(1, num2), new Point(base.Right - 2, num2));
		}
	}

	private void DrawInsetBorder(Graphics g, Rectangle rect)
	{
		ControlPaint.DrawBorder3D(g, rect, Border3DStyle.Etched);
		int num = DisplayRectangle.X;
		int num2 = DisplayRectangle.Y;
		for (int i = 0; i < column_widths.Length - 1; i++)
		{
			num += column_widths[i] + 2;
			g.DrawLine(SystemPens.ControlDark, new Point(num, 1), new Point(num, base.Bottom - 3));
			g.DrawLine(Pens.White, new Point(num + 1, 1), new Point(num + 1, base.Bottom - 3));
		}
		for (int j = 0; j < row_heights.Length - 1; j++)
		{
			num2 += row_heights[j] + 2;
			g.DrawLine(SystemPens.ControlDark, new Point(1, num2), new Point(base.Right - 3, num2));
			g.DrawLine(Pens.White, new Point(1, num2 + 1), new Point(base.Right - 3, num2 + 1));
		}
	}

	private void DrawOutsetBorder(Graphics g, Rectangle rect)
	{
		g.DrawRectangle(SystemPens.ControlDark, new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2));
		g.DrawRectangle(Pens.White, new Rectangle(rect.Left, rect.Top, rect.Width - 2, rect.Height - 2));
		int num = DisplayRectangle.X;
		int num2 = DisplayRectangle.Y;
		for (int i = 0; i < column_widths.Length - 1; i++)
		{
			num += column_widths[i] + 2;
			g.DrawLine(Pens.White, new Point(num, 1), new Point(num, base.Bottom - 3));
			g.DrawLine(SystemPens.ControlDark, new Point(num + 1, 1), new Point(num + 1, base.Bottom - 3));
		}
		for (int j = 0; j < row_heights.Length - 1; j++)
		{
			num2 += row_heights[j] + 2;
			g.DrawLine(Pens.White, new Point(1, num2), new Point(base.Right - 3, num2));
			g.DrawLine(SystemPens.ControlDark, new Point(1, num2 + 1), new Point(base.Right - 3, num2 + 1));
		}
	}

	private void DrawOutsetDoubleBorder(Graphics g, Rectangle rect)
	{
		rect.Width--;
		rect.Height--;
		g.DrawRectangle(SystemPens.ControlDark, new Rectangle(rect.Left + 2, rect.Top + 2, rect.Width - 2, rect.Height - 2));
		g.DrawRectangle(Pens.White, new Rectangle(rect.Left, rect.Top, rect.Width - 2, rect.Height - 2));
		int num = DisplayRectangle.X;
		int num2 = DisplayRectangle.Y;
		for (int i = 0; i < column_widths.Length - 1; i++)
		{
			num += column_widths[i] + 3;
			g.DrawLine(Pens.White, new Point(num, 3), new Point(num, base.Bottom - 5));
			g.DrawLine(SystemPens.ControlDark, new Point(num + 2, 3), new Point(num + 2, base.Bottom - 5));
		}
		for (int j = 0; j < row_heights.Length - 1; j++)
		{
			num2 += row_heights[j] + 3;
			g.DrawLine(Pens.White, new Point(3, num2), new Point(base.Right - 4, num2));
			g.DrawLine(SystemPens.ControlDark, new Point(3, num2 + 2), new Point(base.Right - 4, num2 + 2));
		}
		num = DisplayRectangle.X;
		num2 = DisplayRectangle.Y;
		for (int k = 0; k < column_widths.Length - 1; k++)
		{
			num += column_widths[k] + 3;
			g.DrawLine(ThemeEngine.Current.ResPool.GetPen(BackColor), new Point(num + 1, 3), new Point(num + 1, base.Bottom - 5));
		}
		for (int l = 0; l < row_heights.Length - 1; l++)
		{
			num2 += row_heights[l] + 3;
			g.DrawLine(ThemeEngine.Current.ResPool.GetPen(BackColor), new Point(3, num2 + 1), new Point(base.Right - 4, num2 + 1));
		}
	}

	private void DrawInsetDoubleBorder(Graphics g, Rectangle rect)
	{
		rect.Width--;
		rect.Height--;
		g.DrawRectangle(Pens.White, new Rectangle(rect.Left + 2, rect.Top + 2, rect.Width - 2, rect.Height - 2));
		g.DrawRectangle(SystemPens.ControlDark, new Rectangle(rect.Left, rect.Top, rect.Width - 2, rect.Height - 2));
		int num = DisplayRectangle.X;
		int num2 = DisplayRectangle.Y;
		for (int i = 0; i < column_widths.Length - 1; i++)
		{
			num += column_widths[i] + 3;
			g.DrawLine(SystemPens.ControlDark, new Point(num, 3), new Point(num, base.Bottom - 5));
			g.DrawLine(Pens.White, new Point(num + 2, 3), new Point(num + 2, base.Bottom - 5));
		}
		for (int j = 0; j < row_heights.Length - 1; j++)
		{
			num2 += row_heights[j] + 3;
			g.DrawLine(SystemPens.ControlDark, new Point(3, num2), new Point(base.Right - 4, num2));
			g.DrawLine(Pens.White, new Point(3, num2 + 2), new Point(base.Right - 4, num2 + 2));
		}
		num = DisplayRectangle.X;
		num2 = DisplayRectangle.Y;
		for (int k = 0; k < column_widths.Length - 1; k++)
		{
			num += column_widths[k] + 3;
			g.DrawLine(ThemeEngine.Current.ResPool.GetPen(BackColor), new Point(num + 1, 3), new Point(num + 1, base.Bottom - 5));
		}
		for (int l = 0; l < row_heights.Length - 1; l++)
		{
			num2 += row_heights[l] + 3;
			g.DrawLine(ThemeEngine.Current.ResPool.GetPen(BackColor), new Point(3, num2 + 1), new Point(base.Right - 4, num2 + 1));
		}
	}

	internal override Size GetPreferredSizeCore(Size proposedSize)
	{
		actual_positions = (LayoutEngine as TableLayout).CalculateControlPositions(this, Math.Max(ColumnCount, 1), Math.Max(RowCount, 1));
		int length = actual_positions.GetLength(0);
		int length2 = actual_positions.GetLength(1);
		int[] array = new int[length];
		float num = 0f;
		for (int i = 0; i < length; i++)
		{
			if (i < ColumnStyles.Count && ColumnStyles[i].SizeType == SizeType.Percent)
			{
				num += ColumnStyles[i].Width;
			}
			int num2 = 0;
			for (int j = 0; j < length2; j++)
			{
				Control control = actual_positions[i, j];
				if (control != null)
				{
					num2 = (control.AutoSize ? Math.Max(num2, control.PreferredSize.Width + control.Margin.Horizontal + base.Padding.Horizontal) : Math.Max(num2, control.ExplicitBounds.Width + control.Margin.Horizontal + base.Padding.Horizontal));
				}
			}
			array[i] = num2;
		}
		int num3 = 0;
		int num4 = 0;
		for (int k = 0; k < length; k++)
		{
			if (k < ColumnStyles.Count && ColumnStyles[k].SizeType == SizeType.Percent)
			{
				num4 = Math.Max(num4, (int)((float)array[k] / (ColumnStyles[k].Width / num)));
			}
			else
			{
				num3 += array[k];
			}
		}
		int[] array2 = new int[length2];
		float num5 = 0f;
		for (int l = 0; l < length2; l++)
		{
			if (l < RowStyles.Count && RowStyles[l].SizeType == SizeType.Percent)
			{
				num5 += RowStyles[l].Height;
			}
			int num6 = 0;
			for (int m = 0; m < length; m++)
			{
				Control control2 = actual_positions[m, l];
				if (control2 != null)
				{
					num6 = (control2.AutoSize ? Math.Max(num6, control2.PreferredSize.Height + control2.Margin.Vertical + base.Padding.Vertical) : Math.Max(num6, control2.ExplicitBounds.Height + control2.Margin.Vertical + base.Padding.Vertical));
				}
			}
			array2[l] = num6;
		}
		int num7 = 0;
		int num8 = 0;
		for (int n = 0; n < length2; n++)
		{
			if (n < RowStyles.Count && RowStyles[n].SizeType == SizeType.Percent)
			{
				num8 = Math.Max(num8, (int)((float)array2[n] / (RowStyles[n].Height / num5)));
			}
			else
			{
				num7 += array2[n];
			}
		}
		int cellBorderWidth = GetCellBorderWidth(CellBorderStyle);
		return new Size(num3 + num4 + cellBorderWidth * (length + 1), num7 + num8 + cellBorderWidth * (length2 + 1));
	}
}
