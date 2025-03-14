using System.Collections.Generic;
using System.Drawing;

namespace System.Windows.Forms.Layout;

internal class FlowLayout : LayoutEngine
{
	private static FlowLayoutSettings default_settings = new FlowLayoutSettings();

	public override void InitLayout(object child, BoundsSpecified specified)
	{
		base.InitLayout(child, specified);
	}

	public override bool Layout(object container, LayoutEventArgs args)
	{
		if (container is ToolStripPanel)
		{
			return false;
		}
		if (container is ToolStrip)
		{
			return LayoutToolStrip((ToolStrip)container);
		}
		Control control = container as Control;
		FlowLayoutSettings flowLayoutSettings = ((!(control is FlowLayoutPanel)) ? default_settings : (control as FlowLayoutPanel).LayoutSettings);
		if (control.Controls.Count == 0)
		{
			return false;
		}
		Rectangle displayRectangle = control.DisplayRectangle;
		Point point = flowLayoutSettings.FlowDirection switch
		{
			FlowDirection.BottomUp => new Point(displayRectangle.Left, displayRectangle.Bottom), 
			FlowDirection.RightToLeft => new Point(displayRectangle.Right, displayRectangle.Top), 
			_ => displayRectangle.Location, 
		};
		bool flag = false;
		List<Control> list = new List<Control>();
		foreach (Control control2 in control.Controls)
		{
			if (!control2.Visible)
			{
				continue;
			}
			if (control2.AutoSize)
			{
				Size preferredSize = control2.GetPreferredSize(control2.Size);
				control2.SetBoundsInternal(control2.Left, control2.Top, preferredSize.Width, preferredSize.Height, BoundsSpecified.None);
			}
			switch (flowLayoutSettings.FlowDirection)
			{
			case FlowDirection.BottomUp:
				if (flowLayoutSettings.WrapContents && (point.Y < control2.Height + control2.Margin.Top + control2.Margin.Bottom || flag))
				{
					point.X = FinishColumn(list);
					point.Y = displayRectangle.Bottom;
					flag = false;
					list.Clear();
				}
				point.Offset(0, control2.Margin.Bottom * -1);
				control2.SetBoundsInternal(point.X + control2.Margin.Left, point.Y - control2.Height, control2.Width, control2.Height, BoundsSpecified.None);
				point.Y -= control2.Height + control2.Margin.Top;
				break;
			default:
				if (flowLayoutSettings.WrapContents && !(control is ToolStripPanel) && (displayRectangle.Width + displayRectangle.Left - point.X < control2.Width + control2.Margin.Left + control2.Margin.Right || flag))
				{
					point.Y = FinishRow(list);
					point.X = displayRectangle.Left;
					flag = false;
					list.Clear();
				}
				point.Offset(control2.Margin.Left, 0);
				control2.SetBoundsInternal(point.X, point.Y + control2.Margin.Top, control2.Width, control2.Height, BoundsSpecified.None);
				point.X += control2.Width + control2.Margin.Right;
				break;
			case FlowDirection.RightToLeft:
				if (flowLayoutSettings.WrapContents && (point.X < control2.Width + control2.Margin.Left + control2.Margin.Right || flag))
				{
					point.Y = FinishRow(list);
					point.X = displayRectangle.Right;
					flag = false;
					list.Clear();
				}
				point.Offset(control2.Margin.Right * -1, 0);
				control2.SetBoundsInternal(point.X - control2.Width, point.Y + control2.Margin.Top, control2.Width, control2.Height, BoundsSpecified.None);
				point.X -= control2.Width + control2.Margin.Left;
				break;
			case FlowDirection.TopDown:
				if (flowLayoutSettings.WrapContents && (displayRectangle.Height + displayRectangle.Top - point.Y < control2.Height + control2.Margin.Top + control2.Margin.Bottom || flag))
				{
					point.X = FinishColumn(list);
					point.Y = displayRectangle.Top;
					flag = false;
					list.Clear();
				}
				point.Offset(0, control2.Margin.Top);
				control2.SetBoundsInternal(point.X + control2.Margin.Left, point.Y, control2.Width, control2.Height, BoundsSpecified.None);
				point.Y += control2.Height + control2.Margin.Bottom;
				break;
			}
			list.Add(control2);
			if (flowLayoutSettings.GetFlowBreak(control2))
			{
				flag = true;
			}
		}
		if (flowLayoutSettings.FlowDirection == FlowDirection.LeftToRight || flowLayoutSettings.FlowDirection == FlowDirection.RightToLeft)
		{
			FinishRow(list);
		}
		else
		{
			FinishColumn(list);
		}
		return false;
	}

	private int FinishRow(List<Control> row)
	{
		if (row.Count == 0)
		{
			return 0;
		}
		int num = int.MaxValue;
		int num2 = 0;
		bool flag = true;
		bool flag2 = true;
		foreach (Control item in row)
		{
			if (item.Dock != DockStyle.Fill && ((item.Anchor & AnchorStyles.Top) != AnchorStyles.Top || (item.Anchor & AnchorStyles.Bottom) != AnchorStyles.Bottom))
			{
				flag = false;
			}
			if (item.AutoSize)
			{
				flag2 = false;
			}
		}
		foreach (Control item2 in row)
		{
			if (item2.Bottom + item2.Margin.Bottom > num2 && item2.Dock != DockStyle.Fill && ((item2.Anchor & AnchorStyles.Top) != AnchorStyles.Top || (item2.Anchor & AnchorStyles.Bottom) != AnchorStyles.Bottom || item2.AutoSize))
			{
				num2 = item2.Bottom + item2.Margin.Bottom;
			}
			if (item2.Top - item2.Margin.Top < num)
			{
				num = item2.Top - item2.Margin.Top;
			}
		}
		if (num2 == 0)
		{
			foreach (Control item3 in row)
			{
				if (item3.Bottom + item3.Margin.Bottom > num2 && item3.Dock != DockStyle.Fill && item3.AutoSize)
				{
					num2 = item3.Bottom + item3.Margin.Bottom;
				}
			}
		}
		if (num2 == 0)
		{
			foreach (Control item4 in row)
			{
				if (item4.Bottom + item4.Margin.Bottom > num2 && item4.Dock == DockStyle.Fill)
				{
					num2 = item4.Bottom + item4.Margin.Bottom;
				}
			}
		}
		foreach (Control item5 in row)
		{
			if (flag && flag2)
			{
				item5.SetBoundsInternal(item5.Left, item5.Top, item5.Width, 0, BoundsSpecified.None);
			}
			else if (item5.Dock == DockStyle.Fill || ((item5.Anchor & AnchorStyles.Top) == AnchorStyles.Top && (item5.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom))
			{
				item5.SetBoundsInternal(item5.Left, item5.Top, item5.Width, num2 - item5.Top - item5.Margin.Bottom, BoundsSpecified.None);
			}
			else if (item5.Dock == DockStyle.Bottom || (item5.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
			{
				item5.SetBoundsInternal(item5.Left, num2 - item5.Margin.Bottom - item5.Height, item5.Width, item5.Height, BoundsSpecified.None);
			}
			else if (item5.Dock != DockStyle.Top && (item5.Anchor & AnchorStyles.Top) != AnchorStyles.Top)
			{
				item5.SetBoundsInternal(item5.Left, (num2 - num) / 2 - item5.Height / 2 + (int)Math.Floor((double)(item5.Margin.Top - item5.Margin.Bottom) / 2.0) + num, item5.Width, item5.Height, BoundsSpecified.None);
			}
		}
		if (num2 == 0)
		{
			return num;
		}
		return num2;
	}

	private int FinishColumn(List<Control> col)
	{
		if (col.Count == 0)
		{
			return 0;
		}
		int num = int.MaxValue;
		int num2 = 0;
		bool flag = true;
		bool flag2 = true;
		foreach (Control item in col)
		{
			if (item.Dock != DockStyle.Fill && ((item.Anchor & AnchorStyles.Left) != AnchorStyles.Left || (item.Anchor & AnchorStyles.Right) != AnchorStyles.Right))
			{
				flag = false;
			}
			if (item.AutoSize)
			{
				flag2 = false;
			}
		}
		foreach (Control item2 in col)
		{
			if (item2.Right + item2.Margin.Right > num2 && item2.Dock != DockStyle.Fill && ((item2.Anchor & AnchorStyles.Left) != AnchorStyles.Left || (item2.Anchor & AnchorStyles.Right) != AnchorStyles.Right || item2.AutoSize))
			{
				num2 = item2.Right + item2.Margin.Right;
			}
			if (item2.Left - item2.Margin.Left < num)
			{
				num = item2.Left - item2.Margin.Left;
			}
		}
		if (num2 == 0)
		{
			foreach (Control item3 in col)
			{
				if (item3.Right + item3.Margin.Right > num2 && item3.Dock != DockStyle.Fill && item3.AutoSize)
				{
					num2 = item3.Right + item3.Margin.Right;
				}
			}
		}
		if (num2 == 0)
		{
			foreach (Control item4 in col)
			{
				if (item4.Right + item4.Margin.Right > num2 && item4.Dock == DockStyle.Fill)
				{
					num2 = item4.Right + item4.Margin.Right;
				}
			}
		}
		foreach (Control item5 in col)
		{
			if (flag && flag2)
			{
				item5.SetBoundsInternal(item5.Left, item5.Top, 0, item5.Height, BoundsSpecified.None);
			}
			else if (item5.Dock == DockStyle.Fill || ((item5.Anchor & AnchorStyles.Left) == AnchorStyles.Left && (item5.Anchor & AnchorStyles.Right) == AnchorStyles.Right))
			{
				item5.SetBoundsInternal(item5.Left, item5.Top, num2 - item5.Left - item5.Margin.Right, item5.Height, BoundsSpecified.None);
			}
			else if (item5.Dock == DockStyle.Right || (item5.Anchor & AnchorStyles.Right) == AnchorStyles.Right)
			{
				item5.SetBoundsInternal(num2 - item5.Margin.Right - item5.Width, item5.Top, item5.Width, item5.Height, BoundsSpecified.None);
			}
			else if (item5.Dock != DockStyle.Left && (item5.Anchor & AnchorStyles.Left) != AnchorStyles.Left)
			{
				item5.SetBoundsInternal((num2 - num) / 2 - item5.Width / 2 + (int)Math.Floor((double)(item5.Margin.Left - item5.Margin.Right) / 2.0) + num, item5.Top, item5.Width, item5.Height, BoundsSpecified.None);
			}
		}
		if (num2 == 0)
		{
			return num;
		}
		return num2;
	}

	private bool LayoutToolStrip(ToolStrip parent)
	{
		FlowLayoutSettings flowLayoutSettings = (FlowLayoutSettings)parent.LayoutSettings;
		if (parent.Items.Count == 0)
		{
			return false;
		}
		foreach (ToolStripItem item in parent.Items)
		{
			item.SetPlacement(ToolStripItemPlacement.Main);
		}
		Rectangle displayRectangle = parent.DisplayRectangle;
		Point point = flowLayoutSettings.FlowDirection switch
		{
			FlowDirection.BottomUp => new Point(displayRectangle.Left, displayRectangle.Bottom), 
			FlowDirection.RightToLeft => new Point(displayRectangle.Right, displayRectangle.Top), 
			_ => displayRectangle.Location, 
		};
		bool flag = false;
		List<ToolStripItem> list = new List<ToolStripItem>();
		foreach (ToolStripItem item2 in parent.Items)
		{
			if (!item2.Available)
			{
				continue;
			}
			if (item2.AutoSize)
			{
				Size preferredSize = item2.GetPreferredSize(item2.Size);
				preferredSize.Height = displayRectangle.Height;
				item2.SetBounds(new Rectangle(item2.Location, preferredSize));
			}
			switch (flowLayoutSettings.FlowDirection)
			{
			case FlowDirection.BottomUp:
				if (flowLayoutSettings.WrapContents && (point.Y < item2.Height + item2.Margin.Top + item2.Margin.Bottom || flag))
				{
					point.X = FinishColumn(list);
					point.Y = displayRectangle.Bottom;
					flag = false;
					list.Clear();
				}
				point.Offset(0, item2.Margin.Bottom * -1);
				item2.Location = new Point(point.X + item2.Margin.Left, point.Y - item2.Height);
				point.Y -= item2.Height + item2.Margin.Top;
				break;
			default:
				if (flowLayoutSettings.WrapContents && (displayRectangle.Width - point.X < item2.Width + item2.Margin.Left + item2.Margin.Right || flag))
				{
					point.Y = FinishRow(list);
					point.X = displayRectangle.Left;
					flag = false;
					list.Clear();
				}
				point.Offset(item2.Margin.Left, 0);
				item2.Location = new Point(point.X, point.Y + item2.Margin.Top);
				point.X += item2.Width + item2.Margin.Right;
				break;
			case FlowDirection.RightToLeft:
				if (flowLayoutSettings.WrapContents && (point.X < item2.Width + item2.Margin.Left + item2.Margin.Right || flag))
				{
					point.Y = FinishRow(list);
					point.X = displayRectangle.Right;
					flag = false;
					list.Clear();
				}
				point.Offset(item2.Margin.Right * -1, 0);
				item2.Location = new Point(point.X - item2.Width, point.Y + item2.Margin.Top);
				point.X -= item2.Width + item2.Margin.Left;
				break;
			case FlowDirection.TopDown:
				if (flowLayoutSettings.WrapContents && (displayRectangle.Height - point.Y < item2.Height + item2.Margin.Top + item2.Margin.Bottom || flag))
				{
					point.X = FinishColumn(list);
					point.Y = displayRectangle.Top;
					flag = false;
					list.Clear();
				}
				point.Offset(0, item2.Margin.Top);
				item2.Location = new Point(point.X + item2.Margin.Left, point.Y);
				point.Y += item2.Height + item2.Margin.Bottom;
				break;
			}
			list.Add(item2);
			if (flowLayoutSettings.GetFlowBreak(item2))
			{
				flag = true;
			}
		}
		int num = 0;
		if (flowLayoutSettings.FlowDirection == FlowDirection.LeftToRight || flowLayoutSettings.FlowDirection == FlowDirection.RightToLeft)
		{
			num = FinishRow(list);
		}
		else
		{
			FinishColumn(list);
		}
		if (num > 0)
		{
			parent.SetBoundsInternal(parent.Left, parent.Top, parent.Width, num + parent.Padding.Bottom, BoundsSpecified.None);
		}
		return false;
	}

	private int FinishRow(List<ToolStripItem> row)
	{
		if (row.Count == 0)
		{
			return 0;
		}
		int num = int.MaxValue;
		int num2 = 0;
		bool flag = true;
		bool flag2 = true;
		foreach (ToolStripItem item in row)
		{
			if (item.Dock != DockStyle.Fill && ((item.Anchor & AnchorStyles.Top) != AnchorStyles.Top || (item.Anchor & AnchorStyles.Bottom) != AnchorStyles.Bottom))
			{
				flag = false;
			}
			if (item.AutoSize)
			{
				flag2 = false;
			}
		}
		foreach (ToolStripItem item2 in row)
		{
			if (item2.Bottom + item2.Margin.Bottom > num2 && item2.Dock != DockStyle.Fill && ((item2.Anchor & AnchorStyles.Top) != AnchorStyles.Top || (item2.Anchor & AnchorStyles.Bottom) != AnchorStyles.Bottom || item2.AutoSize))
			{
				num2 = item2.Bottom + item2.Margin.Bottom;
			}
			if (item2.Top - item2.Margin.Top < num)
			{
				num = item2.Top - item2.Margin.Top;
			}
		}
		if (num2 == 0)
		{
			foreach (ToolStripItem item3 in row)
			{
				if (item3.Bottom + item3.Margin.Bottom > num2 && item3.Dock != DockStyle.Fill && item3.AutoSize)
				{
					num2 = item3.Bottom + item3.Margin.Bottom;
				}
			}
		}
		if (num2 == 0)
		{
			foreach (ToolStripItem item4 in row)
			{
				if (item4.Bottom + item4.Margin.Bottom > num2 && item4.Dock == DockStyle.Fill)
				{
					num2 = item4.Bottom + item4.Margin.Bottom;
				}
			}
		}
		foreach (ToolStripItem item5 in row)
		{
			if (flag && flag2)
			{
				item5.Height = 0;
			}
			else if (item5.Dock == DockStyle.Fill || ((item5.Anchor & AnchorStyles.Top) == AnchorStyles.Top && (item5.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom))
			{
				item5.Height = num2 - item5.Top - item5.Margin.Bottom;
			}
			else if (item5.Dock == DockStyle.Bottom || (item5.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
			{
				item5.Top = num2 - item5.Margin.Bottom - item5.Height;
			}
			else if (item5.Dock != DockStyle.Top && (item5.Anchor & AnchorStyles.Top) != AnchorStyles.Top)
			{
				item5.Top = (num2 - num) / 2 - item5.Height / 2 + (int)Math.Floor((double)(item5.Margin.Top - item5.Margin.Bottom) / 2.0) + num;
			}
		}
		if (num2 == 0)
		{
			return num;
		}
		return num2;
	}

	private int FinishColumn(List<ToolStripItem> col)
	{
		if (col.Count == 0)
		{
			return 0;
		}
		int num = int.MaxValue;
		int num2 = 0;
		bool flag = true;
		bool flag2 = true;
		foreach (ToolStripItem item in col)
		{
			if (item.Dock != DockStyle.Fill && ((item.Anchor & AnchorStyles.Left) != AnchorStyles.Left || (item.Anchor & AnchorStyles.Right) != AnchorStyles.Right))
			{
				flag = false;
			}
			if (item.AutoSize)
			{
				flag2 = false;
			}
		}
		foreach (ToolStripItem item2 in col)
		{
			if (item2.Right + item2.Margin.Right > num2 && item2.Dock != DockStyle.Fill && ((item2.Anchor & AnchorStyles.Left) != AnchorStyles.Left || (item2.Anchor & AnchorStyles.Right) != AnchorStyles.Right || item2.AutoSize))
			{
				num2 = item2.Right + item2.Margin.Right;
			}
			if (item2.Left - item2.Margin.Left < num)
			{
				num = item2.Left - item2.Margin.Left;
			}
		}
		if (num2 == 0)
		{
			foreach (ToolStripItem item3 in col)
			{
				if (item3.Right + item3.Margin.Right > num2 && item3.Dock != DockStyle.Fill && item3.AutoSize)
				{
					num2 = item3.Right + item3.Margin.Right;
				}
			}
		}
		if (num2 == 0)
		{
			foreach (ToolStripItem item4 in col)
			{
				if (item4.Right + item4.Margin.Right > num2 && item4.Dock == DockStyle.Fill)
				{
					num2 = item4.Right + item4.Margin.Right;
				}
			}
		}
		foreach (ToolStripItem item5 in col)
		{
			if (flag && flag2)
			{
				item5.Width = 0;
			}
			else if (item5.Dock == DockStyle.Fill || ((item5.Anchor & AnchorStyles.Left) == AnchorStyles.Left && (item5.Anchor & AnchorStyles.Right) == AnchorStyles.Right))
			{
				item5.Width = num2 - item5.Left - item5.Margin.Right;
			}
			else if (item5.Dock == DockStyle.Right || (item5.Anchor & AnchorStyles.Right) == AnchorStyles.Right)
			{
				item5.Left = num2 - item5.Margin.Right - item5.Width;
			}
			else if (item5.Dock != DockStyle.Left && (item5.Anchor & AnchorStyles.Left) != AnchorStyles.Left)
			{
				item5.Left = (num2 - num) / 2 - item5.Width / 2 + (int)Math.Floor((double)(item5.Margin.Left - item5.Margin.Right) / 2.0) + num;
			}
		}
		if (num2 == 0)
		{
			return num;
		}
		return num2;
	}
}
