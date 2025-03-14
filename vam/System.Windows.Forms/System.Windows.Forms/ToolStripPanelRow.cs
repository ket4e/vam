using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

[ToolboxItem(false)]
public class ToolStripPanelRow : Component, IDisposable, IComponent, IBounds
{
	private Rectangle bounds;

	internal List<Control> controls;

	private LayoutEngine layout_engine;

	private Padding margin;

	private Padding padding;

	private ToolStripPanel parent;

	public Rectangle Bounds => bounds;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Control[] Controls => controls.ToArray();

	public Rectangle DisplayRectangle => Bounds;

	public LayoutEngine LayoutEngine
	{
		get
		{
			if (layout_engine == null)
			{
				layout_engine = new DefaultLayout();
			}
			return layout_engine;
		}
	}

	public Padding Margin
	{
		get
		{
			return margin;
		}
		set
		{
			margin = value;
		}
	}

	public Orientation Orientation => parent.Orientation;

	public virtual Padding Padding
	{
		get
		{
			return padding;
		}
		set
		{
			padding = value;
		}
	}

	public ToolStripPanel ToolStripPanel => parent;

	protected virtual Padding DefaultMargin => Padding.Empty;

	protected virtual Padding DefaultPadding => Padding.Empty;

	public ToolStripPanelRow(ToolStripPanel parent)
	{
		bounds = Rectangle.Empty;
		controls = new List<Control>();
		layout_engine = new DefaultLayout();
		this.parent = parent;
	}

	public bool CanMove(ToolStrip toolStripToDrag)
	{
		if (controls.Count > 0 && (toolStripToDrag.Stretch || (controls[0] as ToolStrip).Stretch))
		{
			return false;
		}
		int num = 0;
		foreach (ToolStrip control in controls)
		{
			num += control.Width + control.Margin.Horizontal;
		}
		if (num + toolStripToDrag.Width + toolStripToDrag.Margin.Horizontal <= bounds.Width)
		{
			return true;
		}
		return false;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	protected void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds)
	{
	}

	protected internal virtual void OnControlAdded(Control control, int index)
	{
		control.SizeChanged += control_SizeChanged;
		controls.Add(control);
		OnLayout(new LayoutEventArgs(control, string.Empty));
	}

	protected internal virtual void OnControlRemoved(Control control, int index)
	{
		control.SizeChanged -= control_SizeChanged;
		controls.Remove(control);
		OnLayout(new LayoutEventArgs(control, string.Empty));
	}

	protected virtual void OnLayout(LayoutEventArgs e)
	{
		int num = 0;
		if (Orientation == Orientation.Horizontal)
		{
			foreach (ToolStrip control in controls)
			{
				if (control.Height > num)
				{
					num = control.Height;
				}
			}
			if (num != bounds.Height)
			{
				bounds.Height = num;
			}
		}
		else
		{
			foreach (ToolStrip control2 in controls)
			{
				if (control2.GetPreferredSize(Size.Empty).Width > num)
				{
					num = control2.GetPreferredSize(Size.Empty).Width;
				}
			}
			if (num != bounds.Width)
			{
				bounds.Width = num;
			}
		}
		Layout(this, e);
	}

	protected internal virtual void OnOrientationChanged()
	{
	}

	internal void SetBounds(Rectangle bounds)
	{
		if (this.bounds != bounds)
		{
			Rectangle oldBounds = this.bounds;
			this.bounds = bounds;
			OnBoundsChanged(oldBounds, bounds);
			OnLayout(new LayoutEventArgs(null, "Bounds"));
		}
	}

	private bool Layout(object container, LayoutEventArgs args)
	{
		ToolStripPanelRow toolStripPanelRow = (ToolStripPanelRow)container;
		Point location = toolStripPanelRow.DisplayRectangle.Location;
		Control[] array = toolStripPanelRow.Controls;
		for (int i = 0; i < array.Length; i++)
		{
			ToolStrip toolStrip = (ToolStrip)array[i];
			if (Orientation == Orientation.Horizontal)
			{
				if (toolStrip.Stretch)
				{
					toolStrip.Width = bounds.Width - toolStrip.Margin.Horizontal - Padding.Horizontal;
				}
				else
				{
					toolStrip.Width = toolStrip.GetToolStripPreferredSize(Size.Empty).Width;
				}
				location.X += toolStrip.Margin.Left;
				toolStrip.Location = location;
				location.X += toolStrip.Width + toolStrip.Margin.Left;
			}
			else
			{
				if (toolStrip.Stretch)
				{
					toolStrip.Size = new Size(toolStrip.GetToolStripPreferredSize(Size.Empty).Width, bounds.Height - toolStrip.Margin.Vertical - Padding.Vertical);
				}
				else
				{
					toolStrip.Size = toolStrip.GetToolStripPreferredSize(Size.Empty);
				}
				location.Y += toolStrip.Margin.Top;
				toolStrip.Location = location;
				location.Y += toolStrip.Height + toolStrip.Margin.Top;
			}
		}
		return false;
	}

	private void control_SizeChanged(object sender, EventArgs e)
	{
		OnLayout(new LayoutEventArgs((Control)sender, string.Empty));
	}
}
