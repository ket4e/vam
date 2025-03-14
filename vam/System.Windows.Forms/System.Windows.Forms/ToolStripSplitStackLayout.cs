using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

internal class ToolStripSplitStackLayout : LayoutEngine
{
	public override bool Layout(object container, LayoutEventArgs args)
	{
		if (container is ToolStrip)
		{
			ToolStrip toolStrip = (ToolStrip)container;
			if (toolStrip.Items == null)
			{
				return false;
			}
			Rectangle displayRectangle = toolStrip.DisplayRectangle;
			if (toolStrip.Orientation == Orientation.Horizontal)
			{
				LayoutHorizontalToolStrip(toolStrip, displayRectangle);
			}
			else
			{
				LayoutVerticalToolStrip(toolStrip, displayRectangle);
			}
			return false;
		}
		ToolStripContentPanel toolStripContentPanel = (ToolStripContentPanel)container;
		int num = toolStripContentPanel.DisplayRectangle.Left;
		int top = toolStripContentPanel.DisplayRectangle.Top;
		foreach (ToolStrip control in toolStripContentPanel.Controls)
		{
			Rectangle rectangle = default(Rectangle);
			num += control.Margin.Left;
			rectangle.Location = new Point(num, top + control.Margin.Top);
			rectangle.Height = toolStripContentPanel.DisplayRectangle.Height - control.Margin.Vertical;
			rectangle.Width = control.GetToolStripPreferredSize(new Size(0, rectangle.Height)).Width;
			control.Width = rectangle.Width + 12;
			num += rectangle.Width + control.Margin.Right;
		}
		return false;
	}

	private void LayoutHorizontalToolStrip(ToolStrip ts, Rectangle bounds)
	{
		ToolStripItemOverflow[] array = new ToolStripItemOverflow[ts.Items.Count];
		ToolStripItemPlacement[] array2 = new ToolStripItemPlacement[ts.Items.Count];
		Size constrainingSize = new Size(0, bounds.Height);
		int[] array3 = new int[ts.Items.Count];
		int num = 0;
		int num2 = bounds.Width;
		int num3 = 0;
		bool flag = ts.CanOverflow && !(ts is MenuStrip) && !(ts is StatusStrip);
		bool flag2 = false;
		foreach (ToolStripItem item in ts.Items)
		{
			array[num3] = item.Overflow;
			array2[num3] = ((item.Overflow == ToolStripItemOverflow.Always) ? ToolStripItemPlacement.Overflow : ToolStripItemPlacement.Main);
			array3[num3] = item.GetPreferredSize(constrainingSize).Width + item.Margin.Horizontal;
			if (!item.Available)
			{
				array2[num3] = ToolStripItemPlacement.None;
			}
			num += ((array2[num3] == ToolStripItemPlacement.Main) ? array3[num3] : 0);
			if (array2[num3] == ToolStripItemPlacement.Overflow)
			{
				flag2 = true;
			}
			num3++;
		}
		if (flag2)
		{
			ts.OverflowButton.Visible = true;
			ts.OverflowButton.SetBounds(new Rectangle(ts.Width - 16, 0, 16, ts.Height));
			num2 -= ts.OverflowButton.Width;
		}
		else
		{
			ts.OverflowButton.Visible = false;
		}
		while (num > num2)
		{
			if (flag && !ts.OverflowButton.Visible)
			{
				ts.OverflowButton.Visible = true;
				ts.OverflowButton.SetBounds(new Rectangle(ts.Width - 16, 0, 16, ts.Height));
				num2 -= ts.OverflowButton.Width;
			}
			bool flag3 = false;
			for (int num4 = array3.Length - 1; num4 >= 0; num4--)
			{
				if (array[num4] == ToolStripItemOverflow.AsNeeded && array2[num4] == ToolStripItemPlacement.Main)
				{
					array2[num4] = ToolStripItemPlacement.Overflow;
					num -= array3[num4];
					flag3 = true;
					break;
				}
			}
			if (!flag3)
			{
				for (int num5 = array3.Length - 1; num5 >= 0; num5--)
				{
					if (array[num5] == ToolStripItemOverflow.Never && array2[num5] == ToolStripItemPlacement.Main)
					{
						array2[num5] = ToolStripItemPlacement.None;
						num -= array3[num5];
						flag3 = true;
						break;
					}
				}
			}
			if (!flag3)
			{
				break;
			}
		}
		num3 = 0;
		Point point = new Point(ts.DisplayRectangle.Left, ts.DisplayRectangle.Top);
		Point point2 = new Point(ts.DisplayRectangle.Right, ts.DisplayRectangle.Top);
		int height = ts.DisplayRectangle.Height;
		foreach (ToolStripItem item2 in ts.Items)
		{
			item2.SetPlacement(array2[num3]);
			if (array2[num3] == ToolStripItemPlacement.Main)
			{
				if (item2.Alignment == ToolStripItemAlignment.Left)
				{
					item2.SetBounds(new Rectangle(point.X + item2.Margin.Left, point.Y + item2.Margin.Top, array3[num3] - item2.Margin.Horizontal, height - item2.Margin.Vertical));
					point.X += array3[num3];
				}
				else
				{
					item2.SetBounds(new Rectangle(point2.X - item2.Margin.Right - item2.Width, point2.Y + item2.Margin.Top, array3[num3] - item2.Margin.Horizontal, height - item2.Margin.Vertical));
					point2.X -= array3[num3];
				}
			}
			num3++;
		}
	}

	private void LayoutVerticalToolStrip(ToolStrip ts, Rectangle bounds)
	{
		if (!ts.Visible)
		{
			return;
		}
		ToolStripItemOverflow[] array = new ToolStripItemOverflow[ts.Items.Count];
		ToolStripItemPlacement[] array2 = new ToolStripItemPlacement[ts.Items.Count];
		Size constrainingSize = new Size(bounds.Width, 0);
		int[] array3 = new int[ts.Items.Count];
		int num = 0;
		int num2 = bounds.Height;
		int num3 = 0;
		bool flag = ts.CanOverflow && !(ts is MenuStrip) && !(ts is StatusStrip);
		foreach (ToolStripItem item in ts.Items)
		{
			array[num3] = item.Overflow;
			array2[num3] = ((item.Overflow == ToolStripItemOverflow.Always) ? ToolStripItemPlacement.Overflow : ToolStripItemPlacement.Main);
			array3[num3] = item.GetPreferredSize(constrainingSize).Height + item.Margin.Vertical;
			if (!item.Available)
			{
				array2[num3] = ToolStripItemPlacement.None;
			}
			num += ((array2[num3] == ToolStripItemPlacement.Main) ? array3[num3] : 0);
			num3++;
		}
		ts.OverflowButton.Visible = false;
		while (num > num2)
		{
			if (flag && !ts.OverflowButton.Visible)
			{
				ts.OverflowButton.Visible = true;
				ts.OverflowButton.SetBounds(new Rectangle(0, ts.Height - 16, ts.Width, 16));
				num2 -= ts.OverflowButton.Height;
			}
			bool flag2 = false;
			for (int num4 = array3.Length - 1; num4 >= 0; num4--)
			{
				if (array[num4] == ToolStripItemOverflow.AsNeeded && array2[num4] == ToolStripItemPlacement.Main)
				{
					array2[num4] = ToolStripItemPlacement.Overflow;
					num -= array3[num4];
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				for (int num5 = array3.Length - 1; num5 >= 0; num5--)
				{
					if (array[num5] == ToolStripItemOverflow.Never && array2[num5] == ToolStripItemPlacement.Main)
					{
						array2[num5] = ToolStripItemPlacement.None;
						num -= array3[num5];
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2)
			{
				break;
			}
		}
		num3 = 0;
		Point point = new Point(ts.DisplayRectangle.Left, ts.DisplayRectangle.Top);
		Point point2 = new Point(ts.DisplayRectangle.Left, ts.DisplayRectangle.Bottom);
		int width = ts.DisplayRectangle.Width;
		foreach (ToolStripItem item2 in ts.Items)
		{
			item2.SetPlacement(array2[num3]);
			if (array2[num3] == ToolStripItemPlacement.Main)
			{
				if (item2.Alignment == ToolStripItemAlignment.Left)
				{
					item2.SetBounds(new Rectangle(point.X + item2.Margin.Left, point.Y + item2.Margin.Top, width - item2.Margin.Horizontal, array3[num3] - item2.Margin.Vertical));
					point.Y += array3[num3];
				}
				else
				{
					item2.SetBounds(new Rectangle(point2.X + item2.Margin.Left, point2.Y - item2.Margin.Bottom - item2.Height, width - item2.Margin.Horizontal, array3[num3] - item2.Margin.Vertical));
					point.Y += array3[num3];
				}
			}
			num3++;
		}
	}
}
