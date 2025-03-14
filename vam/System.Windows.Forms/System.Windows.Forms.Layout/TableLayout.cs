using System.Collections;
using System.Drawing;

namespace System.Windows.Forms.Layout;

internal class TableLayout : LayoutEngine
{
	private static Control dummy_control = new Control("Dummy");

	public override void InitLayout(object child, BoundsSpecified specified)
	{
		base.InitLayout(child, specified);
	}

	public override bool Layout(object container, LayoutEventArgs args)
	{
		TableLayoutPanel tableLayoutPanel = container as TableLayoutPanel;
		TableLayoutSettings layoutSettings = tableLayoutPanel.LayoutSettings;
		tableLayoutPanel.actual_positions = CalculateControlPositions(tableLayoutPanel, Math.Max(layoutSettings.ColumnCount, 1), Math.Max(layoutSettings.RowCount, 1));
		CalculateColumnRowSizes(tableLayoutPanel, tableLayoutPanel.actual_positions.GetLength(0), tableLayoutPanel.actual_positions.GetLength(1));
		LayoutControls(tableLayoutPanel);
		return false;
	}

	internal Control[,] CalculateControlPositions(TableLayoutPanel panel, int columns, int rows)
	{
		Control[,] array = new Control[columns, rows];
		TableLayoutSettings layoutSettings = panel.LayoutSettings;
		foreach (Control control3 in panel.Controls)
		{
			int num = layoutSettings.GetColumn(control3);
			int num2 = layoutSettings.GetRow(control3);
			if (num < 0 || num2 < 0)
			{
				continue;
			}
			if (num >= columns)
			{
				return CalculateControlPositions(panel, num + 1, rows);
			}
			if (num2 >= rows)
			{
				return CalculateControlPositions(panel, columns, num2 + 1);
			}
			if (array[num, num2] != null)
			{
				continue;
			}
			int num3 = Math.Min(layoutSettings.GetColumnSpan(control3), columns);
			int num4 = Math.Min(layoutSettings.GetRowSpan(control3), rows);
			if (num + num3 > columns)
			{
				if (num2 + 1 >= rows)
				{
					if (layoutSettings.GrowStyle == TableLayoutPanelGrowStyle.AddColumns)
					{
						return CalculateControlPositions(panel, columns + 1, rows);
					}
					throw new ArgumentException();
				}
				array[num, num2] = dummy_control;
				num2++;
				num = 0;
			}
			if (num2 + num4 > rows)
			{
				if (layoutSettings.GrowStyle == TableLayoutPanelGrowStyle.AddRows)
				{
					return CalculateControlPositions(panel, columns, rows + 1);
				}
				throw new ArgumentException();
			}
			array[num, num2] = control3;
			for (int i = 1; i < num3; i++)
			{
				array[num + i, num2] = dummy_control;
			}
			for (int j = 1; j < num4; j++)
			{
				array[num, num2 + j] = dummy_control;
			}
		}
		int num5 = 0;
		int num6 = 0;
		foreach (Control control4 in panel.Controls)
		{
			int column = layoutSettings.GetColumn(control4);
			int row = layoutSettings.GetRow(control4);
			if (column >= 0 && column < columns && row >= 0 && row < rows && (array[column, row] == control4 || array[column, row] == dummy_control))
			{
				continue;
			}
			int num7 = num6;
			int num8;
			int num9;
			int num10;
			while (true)
			{
				if (num7 < rows)
				{
					num6 = num7;
					num5 = 0;
					num8 = num5;
					while (num8 < columns)
					{
						num5 = num8;
						if (array[num8, num7] != null)
						{
							if (layoutSettings.GrowStyle == TableLayoutPanelGrowStyle.AddColumns && layoutSettings.RowCount == 0)
							{
								break;
							}
							num8++;
							continue;
						}
						goto IL_0268;
					}
					goto IL_0391;
				}
				TableLayoutPanelGrowStyle tableLayoutPanelGrowStyle = layoutSettings.GrowStyle;
				if (layoutSettings.GrowStyle == TableLayoutPanelGrowStyle.AddColumns && layoutSettings.RowCount == 0)
				{
					tableLayoutPanelGrowStyle = TableLayoutPanelGrowStyle.AddRows;
				}
				return tableLayoutPanelGrowStyle switch
				{
					TableLayoutPanelGrowStyle.AddColumns => CalculateControlPositions(panel, columns + 1, rows), 
					TableLayoutPanelGrowStyle.FixedSize => throw new ArgumentException(), 
					_ => CalculateControlPositions(panel, columns, rows + 1), 
				};
				IL_0268:
				num9 = Math.Min(layoutSettings.GetColumnSpan(control4), columns);
				num10 = Math.Min(layoutSettings.GetRowSpan(control4), rows);
				if (num8 + num9 > columns)
				{
					if (num7 + 1 >= rows)
					{
						if (layoutSettings.GrowStyle == TableLayoutPanelGrowStyle.AddColumns)
						{
							return CalculateControlPositions(panel, columns + 1, rows);
						}
						throw new ArgumentException();
					}
				}
				else
				{
					if (num7 + num10 <= rows)
					{
						break;
					}
					if (num8 + 1 >= columns)
					{
						if (layoutSettings.GrowStyle == TableLayoutPanelGrowStyle.AddRows)
						{
							return CalculateControlPositions(panel, columns, rows + 1);
						}
						throw new ArgumentException();
					}
				}
				goto IL_0391;
				IL_0391:
				num7++;
			}
			array[num8, num7] = control4;
			for (int k = 1; k < num9; k++)
			{
				array[num8 + k, num7] = dummy_control;
			}
			for (int l = 1; l < num10; l++)
			{
				array[num8, num7 + l] = dummy_control;
			}
		}
		return array;
	}

	private void CalculateColumnRowSizes(TableLayoutPanel panel, int columns, int rows)
	{
		TableLayoutSettings layoutSettings = panel.LayoutSettings;
		panel.column_widths = new int[panel.actual_positions.GetLength(0)];
		panel.row_heights = new int[panel.actual_positions.GetLength(1)];
		int cellBorderWidth = TableLayoutPanel.GetCellBorderWidth(panel.CellBorderStyle);
		Rectangle displayRectangle = panel.DisplayRectangle;
		TableLayoutColumnStyleCollection tableLayoutColumnStyleCollection = new TableLayoutColumnStyleCollection(panel);
		foreach (ColumnStyle item in (IEnumerable)layoutSettings.ColumnStyles)
		{
			tableLayoutColumnStyleCollection.Add(new ColumnStyle(item.SizeType, item.Width));
		}
		TableLayoutRowStyleCollection tableLayoutRowStyleCollection = new TableLayoutRowStyleCollection(panel);
		foreach (RowStyle item2 in (IEnumerable)layoutSettings.RowStyles)
		{
			tableLayoutRowStyleCollection.Add(new RowStyle(item2.SizeType, item2.Height));
		}
		if (columns > tableLayoutColumnStyleCollection.Count)
		{
			for (int i = tableLayoutColumnStyleCollection.Count; i < columns; i++)
			{
				tableLayoutColumnStyleCollection.Add(new ColumnStyle());
			}
		}
		if (rows > tableLayoutRowStyleCollection.Count)
		{
			for (int j = tableLayoutRowStyleCollection.Count; j < rows; j++)
			{
				tableLayoutRowStyleCollection.Add(new RowStyle());
			}
		}
		while (tableLayoutRowStyleCollection.Count > rows)
		{
			tableLayoutRowStyleCollection.RemoveAt(tableLayoutRowStyleCollection.Count - 1);
		}
		while (tableLayoutColumnStyleCollection.Count > columns)
		{
			tableLayoutColumnStyleCollection.RemoveAt(tableLayoutColumnStyleCollection.Count - 1);
		}
		int num = displayRectangle.Width - cellBorderWidth * (columns + 1);
		int num2 = 0;
		foreach (ColumnStyle item3 in (IEnumerable)tableLayoutColumnStyleCollection)
		{
			if (item3.SizeType == SizeType.Absolute)
			{
				panel.column_widths[num2] = (int)item3.Width;
				num -= (int)item3.Width;
			}
			num2++;
		}
		num2 = 0;
		foreach (ColumnStyle item4 in (IEnumerable)tableLayoutColumnStyleCollection)
		{
			if (item4.SizeType == SizeType.AutoSize)
			{
				int num3 = 0;
				for (int k = 0; k < rows; k++)
				{
					Control control = panel.actual_positions[num2, k];
					if (control != null && control != dummy_control && control.VisibleInternal && layoutSettings.GetColumnSpan(control) <= 1)
					{
						num3 = ((!control.AutoSize) ? Math.Max(num3, control.ExplicitBounds.Width + control.Margin.Horizontal) : Math.Max(num3, control.PreferredSize.Width + control.Margin.Horizontal));
						if (control.Width + control.Margin.Left + control.Margin.Right > num3)
						{
							num3 = control.Width + control.Margin.Left + control.Margin.Right;
						}
					}
				}
				panel.column_widths[num2] = num3;
				num -= num3;
			}
			num2++;
		}
		num2 = 0;
		float num4 = 0f;
		if (num > 0)
		{
			int num5 = num;
			foreach (ColumnStyle item5 in (IEnumerable)tableLayoutColumnStyleCollection)
			{
				if (item5.SizeType == SizeType.Percent)
				{
					num4 += item5.Width;
				}
			}
			foreach (ColumnStyle item6 in (IEnumerable)tableLayoutColumnStyleCollection)
			{
				if (item6.SizeType == SizeType.Percent)
				{
					panel.column_widths[num2] = (int)(item6.Width / num4 * (float)num5);
					num -= panel.column_widths[num2];
				}
				num2++;
			}
		}
		if (num > 0)
		{
			panel.column_widths[tableLayoutColumnStyleCollection.Count - 1] += num;
		}
		int num6 = displayRectangle.Height - cellBorderWidth * (rows + 1);
		num2 = 0;
		foreach (RowStyle item7 in (IEnumerable)tableLayoutRowStyleCollection)
		{
			if (item7.SizeType == SizeType.Absolute)
			{
				panel.row_heights[num2] = (int)item7.Height;
				num6 -= (int)item7.Height;
			}
			num2++;
		}
		num2 = 0;
		foreach (RowStyle item8 in (IEnumerable)tableLayoutRowStyleCollection)
		{
			if (item8.SizeType == SizeType.AutoSize)
			{
				int num7 = 0;
				for (int l = 0; l < columns; l++)
				{
					Control control2 = panel.actual_positions[l, num2];
					if (control2 != null && control2 != dummy_control && control2.VisibleInternal && layoutSettings.GetRowSpan(control2) <= 1)
					{
						num7 = ((!control2.AutoSize) ? Math.Max(num7, control2.ExplicitBounds.Height + control2.Margin.Vertical) : Math.Max(num7, control2.PreferredSize.Height + control2.Margin.Vertical));
						if (control2.Height + control2.Margin.Top + control2.Margin.Bottom > num7)
						{
							num7 = control2.Height + control2.Margin.Top + control2.Margin.Bottom;
						}
					}
				}
				panel.row_heights[num2] = num7;
				num6 -= num7;
			}
			num2++;
		}
		num2 = 0;
		num4 = 0f;
		if (num6 > 0)
		{
			int num8 = num6;
			foreach (RowStyle item9 in (IEnumerable)tableLayoutRowStyleCollection)
			{
				if (item9.SizeType == SizeType.Percent)
				{
					num4 += item9.Height;
				}
			}
			foreach (RowStyle item10 in (IEnumerable)tableLayoutRowStyleCollection)
			{
				if (item10.SizeType == SizeType.Percent)
				{
					panel.row_heights[num2] = (int)(item10.Height / num4 * (float)num8);
					num6 -= panel.row_heights[num2];
				}
				num2++;
			}
		}
		if (num6 > 0)
		{
			panel.row_heights[tableLayoutRowStyleCollection.Count - 1] += num6;
		}
	}

	private void LayoutControls(TableLayoutPanel panel)
	{
		TableLayoutSettings layoutSettings = panel.LayoutSettings;
		int cellBorderWidth = TableLayoutPanel.GetCellBorderWidth(panel.CellBorderStyle);
		int length = panel.actual_positions.GetLength(0);
		int length2 = panel.actual_positions.GetLength(1);
		Point point = new Point(panel.DisplayRectangle.Left + cellBorderWidth, panel.DisplayRectangle.Top + cellBorderWidth);
		for (int i = 0; i < length2; i++)
		{
			for (int j = 0; j < length; j++)
			{
				Control control = panel.actual_positions[j, i];
				if (control != null && control != dummy_control)
				{
					Size size = ((!control.AutoSize) ? control.ExplicitBounds.Size : control.PreferredSize);
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					int num4 = 0;
					int num5 = panel.column_widths[j];
					for (int k = 1; k < Math.Min(layoutSettings.GetColumnSpan(control), panel.column_widths.Length); k++)
					{
						num5 += panel.column_widths[j + k];
					}
					num3 = ((control.Dock != DockStyle.Fill && control.Dock != DockStyle.Top && control.Dock != DockStyle.Bottom && ((control.Anchor & AnchorStyles.Left) != AnchorStyles.Left || (control.Anchor & AnchorStyles.Right) != AnchorStyles.Right)) ? Math.Min(size.Width, num5 - control.Margin.Left - control.Margin.Right) : (num5 - control.Margin.Left - control.Margin.Right));
					int num6 = panel.row_heights[i];
					for (int l = 1; l < Math.Min(layoutSettings.GetRowSpan(control), panel.row_heights.Length); l++)
					{
						num6 += panel.row_heights[i + l];
					}
					num4 = ((control.Dock != DockStyle.Fill && control.Dock != DockStyle.Left && control.Dock != DockStyle.Right && ((control.Anchor & AnchorStyles.Top) != AnchorStyles.Top || (control.Anchor & AnchorStyles.Bottom) != AnchorStyles.Bottom)) ? Math.Min(size.Height, num6 - control.Margin.Top - control.Margin.Bottom) : (num6 - control.Margin.Top - control.Margin.Bottom));
					num = ((control.Dock != DockStyle.Left && control.Dock != DockStyle.Fill && (control.Anchor & AnchorStyles.Left) != AnchorStyles.Left) ? ((control.Dock != DockStyle.Right && (control.Anchor & AnchorStyles.Right) != AnchorStyles.Right) ? (point.X + (num5 - control.Margin.Left - control.Margin.Right) / 2 + control.Margin.Left - num3 / 2) : (point.X + num5 - num3 - control.Margin.Right)) : (point.X + control.Margin.Left));
					num2 = ((control.Dock != DockStyle.Top && control.Dock != DockStyle.Fill && (control.Anchor & AnchorStyles.Top) != AnchorStyles.Top) ? ((control.Dock != DockStyle.Bottom && (control.Anchor & AnchorStyles.Bottom) != AnchorStyles.Bottom) ? (point.Y + (num6 - control.Margin.Top - control.Margin.Bottom) / 2 + control.Margin.Top - num4 / 2) : (point.Y + num6 - num4 - control.Margin.Bottom)) : (point.Y + control.Margin.Top));
					control.SetBoundsInternal(num, num2, num3, num4, BoundsSpecified.None);
				}
				point.Offset(panel.column_widths[j] + cellBorderWidth, 0);
			}
			point.Offset(-1 * point.X + cellBorderWidth + panel.DisplayRectangle.Left, panel.row_heights[i] + cellBorderWidth);
		}
	}
}
