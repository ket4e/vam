using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class StatusStrip : ToolStrip
{
	private class StatusStripAccessibleObject : AccessibleObject
	{
	}

	private bool sizing_grip;

	[DefaultValue(DockStyle.Bottom)]
	public override DockStyle Dock
	{
		get
		{
			return base.Dock;
		}
		set
		{
			base.Dock = value;
		}
	}

	[DefaultValue(false)]
	[Browsable(false)]
	public new bool CanOverflow
	{
		get
		{
			return base.CanOverflow;
		}
		set
		{
			base.CanOverflow = value;
		}
	}

	[DefaultValue(ToolStripGripStyle.Hidden)]
	public new ToolStripGripStyle GripStyle
	{
		get
		{
			return base.GripStyle;
		}
		set
		{
			base.GripStyle = value;
		}
	}

	[DefaultValue(ToolStripLayoutStyle.Table)]
	public new ToolStripLayoutStyle LayoutStyle
	{
		get
		{
			return base.LayoutStyle;
		}
		set
		{
			base.LayoutStyle = value;
		}
	}

	[Browsable(false)]
	public new Padding Padding
	{
		get
		{
			return base.Padding;
		}
		set
		{
			base.Padding = value;
		}
	}

	[DefaultValue(false)]
	public new bool ShowItemToolTips
	{
		get
		{
			return base.ShowItemToolTips;
		}
		set
		{
			base.ShowItemToolTips = value;
		}
	}

	[Browsable(false)]
	public Rectangle SizeGripBounds => new Rectangle(base.Width - 12, 0, 12, base.Height);

	[DefaultValue(true)]
	public bool SizingGrip
	{
		get
		{
			return sizing_grip;
		}
		set
		{
			sizing_grip = value;
		}
	}

	[DefaultValue(true)]
	public new bool Stretch
	{
		get
		{
			return base.Stretch;
		}
		set
		{
			base.Stretch = value;
		}
	}

	protected override DockStyle DefaultDock => DockStyle.Bottom;

	protected override Padding DefaultPadding => new Padding(1, 0, 14, 0);

	protected override bool DefaultShowItemToolTips => false;

	protected override Size DefaultSize => new Size(200, 22);

	[Browsable(false)]
	public new event EventHandler PaddingChanged
	{
		add
		{
			base.PaddingChanged += value;
		}
		remove
		{
			base.PaddingChanged -= value;
		}
	}

	public StatusStrip()
	{
		SetStyle(ControlStyles.ResizeRedraw, value: true);
		base.CanOverflow = false;
		GripStyle = ToolStripGripStyle.Hidden;
		base.LayoutStyle = ToolStripLayoutStyle.Table;
		base.RenderMode = ToolStripRenderMode.System;
		sizing_grip = true;
		base.Stretch = true;
	}

	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return new StatusStripAccessibleObject();
	}

	protected internal override ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick)
	{
		if (text == "-")
		{
			return new ToolStripSeparator();
		}
		return new ToolStripLabel(text, image, isLink: false, onClick);
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected override void OnLayout(LayoutEventArgs levent)
	{
		OnSpringTableLayoutCore();
		Invalidate();
	}

	protected override void OnPaintBackground(PaintEventArgs e)
	{
		base.OnPaintBackground(e);
		if (sizing_grip)
		{
			base.Renderer.DrawStatusStripSizingGrip(new ToolStripRenderEventArgs(e.Graphics, this, Bounds, SystemColors.Control));
		}
	}

	protected virtual void OnSpringTableLayoutCore()
	{
		if (!base.Created)
		{
			return;
		}
		ToolStripItemOverflow[] array = new ToolStripItemOverflow[Items.Count];
		ToolStripItemPlacement[] array2 = new ToolStripItemPlacement[Items.Count];
		Size constrainingSize = new Size(0, Bounds.Height);
		int[] array3 = new int[Items.Count];
		int num = 0;
		int width = DisplayRectangle.Width;
		int num2 = 0;
		int num3 = 0;
		foreach (ToolStripItem item in Items)
		{
			array[num2] = item.Overflow;
			array3[num2] = item.GetPreferredSize(constrainingSize).Width + item.Margin.Horizontal;
			array2[num2] = ((item.Overflow == ToolStripItemOverflow.Always) ? ToolStripItemPlacement.None : ToolStripItemPlacement.Main);
			array2[num2] = ((!item.Available || !item.InternalVisible) ? ToolStripItemPlacement.None : array2[num2]);
			num += ((array2[num2] == ToolStripItemPlacement.Main) ? array3[num2] : 0);
			if (item is ToolStripStatusLabel && (item as ToolStripStatusLabel).Spring)
			{
				num3++;
			}
			num2++;
		}
		while (num > width)
		{
			bool flag = false;
			for (int num4 = array3.Length - 1; num4 >= 0; num4--)
			{
				if (array[num4] == ToolStripItemOverflow.AsNeeded && array2[num4] == ToolStripItemPlacement.Main)
				{
					array2[num4] = ToolStripItemPlacement.None;
					num -= array3[num4];
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int num5 = array3.Length - 1; num5 >= 0; num5--)
				{
					if (array[num5] == ToolStripItemOverflow.Never && array2[num5] == ToolStripItemPlacement.Main)
					{
						array2[num5] = ToolStripItemPlacement.None;
						num -= array3[num5];
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				break;
			}
		}
		if (num3 > 0)
		{
			int num6 = (width - num) / num3;
			num2 = 0;
			foreach (ToolStripItem item2 in Items)
			{
				if (item2 is ToolStripStatusLabel && (item2 as ToolStripStatusLabel).Spring)
				{
					array3[num2] += num6;
				}
				num2++;
			}
		}
		num2 = 0;
		Point point = new Point(DisplayRectangle.Left, DisplayRectangle.Top);
		int height = DisplayRectangle.Height;
		foreach (ToolStripItem item3 in Items)
		{
			item3.SetPlacement(array2[num2]);
			if (array2[num2] == ToolStripItemPlacement.Main)
			{
				item3.SetBounds(new Rectangle(point.X + item3.Margin.Left, point.Y + item3.Margin.Top, array3[num2] - item3.Margin.Horizontal, height - item3.Margin.Vertical));
				point.X += array3[num2];
			}
			num2++;
		}
		SetDisplayedItems();
	}

	protected override void SetDisplayedItems()
	{
		displayed_items.Clear();
		foreach (ToolStripItem item in Items)
		{
			if (item.Placement == ToolStripItemPlacement.Main && item.Available)
			{
				displayed_items.AddNoOwnerOrLayout(item);
				item.Parent = this;
			}
		}
	}

	protected override void WndProc(ref Message m)
	{
		switch ((Msg)m.Msg)
		{
		case Msg.WM_MOUSEMOVE:
			if (Control.FromParamToMouseButtons(m.WParam.ToInt32()) == MouseButtons.None)
			{
				Point pt2 = new Point(Control.LowOrder(m.LParam.ToInt32()), Control.HighOrder(m.LParam.ToInt32()));
				if (SizingGrip && SizeGripBounds.Contains(pt2))
				{
					Cursor = Cursors.SizeNWSE;
					return;
				}
				Cursor = Cursors.Default;
			}
			break;
		case Msg.WM_LBUTTONDOWN:
		{
			Point pt = new Point(Control.LowOrder(m.LParam.ToInt32()), Control.HighOrder(m.LParam.ToInt32()));
			if (SizingGrip && SizeGripBounds.Contains(pt))
			{
				XplatUI.SendMessage(FindForm().Handle, Msg.WM_NCLBUTTONDOWN, (IntPtr)17, IntPtr.Zero);
				return;
			}
			break;
		}
		}
		base.WndProc(ref m);
	}
}
