using System.Drawing;

namespace System.Windows.Forms.Theming.Default;

internal class ToolStripPainter
{
	protected SystemResPool ResPool => ThemeEngine.Current.ResPool;

	public virtual void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (e.Item.Enabled)
		{
			Rectangle rectangle = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
			ToolStripButton toolStripButton = e.Item as ToolStripButton;
			if (e.Item.Pressed || (toolStripButton != null && toolStripButton.Checked))
			{
				ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.SunkenOuter);
			}
			else if (e.Item.Selected)
			{
				ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.RaisedInner);
			}
			else if (e.Item.BackColor != Control.DefaultBackColor && e.Item.BackColor != Color.Empty)
			{
				e.Graphics.FillRectangle(ResPool.GetSolidBrush(e.Item.BackColor), rectangle);
			}
		}
	}

	public virtual void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (e.Item.Enabled)
		{
			Rectangle rectangle = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
			if (e.Item.Pressed)
			{
				ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.SunkenOuter);
			}
			else if (e.Item.Selected)
			{
				ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.RaisedInner);
			}
			else if (e.Item.BackColor != Control.DefaultBackColor && e.Item.BackColor != Color.Empty)
			{
				e.Graphics.FillRectangle(ResPool.GetSolidBrush(e.Item.BackColor), rectangle);
			}
		}
	}

	public virtual void OnRenderGrip(ToolStripGripRenderEventArgs e)
	{
		if (e.GripStyle != 0)
		{
			if (e.GripDisplayStyle == ToolStripGripDisplayStyle.Vertical)
			{
				e.Graphics.DrawLine(Pens.White, 0, 2, 1, 2);
				e.Graphics.DrawLine(Pens.White, 0, 2, 0, e.GripBounds.Height - 3);
				e.Graphics.DrawLine(SystemPens.ControlDark, 2, 2, 2, e.GripBounds.Height - 3);
				e.Graphics.DrawLine(SystemPens.ControlDark, 2, e.GripBounds.Height - 3, 0, e.GripBounds.Height - 3);
			}
			else
			{
				e.Graphics.DrawLine(Pens.White, 2, 0, e.GripBounds.Width - 3, 0);
				e.Graphics.DrawLine(Pens.White, 2, 0, 2, 1);
				e.Graphics.DrawLine(SystemPens.ControlDark, e.GripBounds.Width - 3, 0, e.GripBounds.Width - 3, 2);
				e.Graphics.DrawLine(SystemPens.ControlDark, 2, 2, e.GripBounds.Width - 3, 2);
			}
		}
	}

	public virtual void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.Item;
		Rectangle rectangle = new Rectangle(Point.Empty, toolStripMenuItem.Size);
		if (toolStripMenuItem.IsOnDropDown)
		{
			if (e.Item.Selected || e.Item.Pressed)
			{
				e.Graphics.FillRectangle(SystemBrushes.Highlight, rectangle);
			}
		}
		else if (e.Item.Pressed)
		{
			ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.SunkenOuter);
		}
		else if (e.Item.Selected)
		{
			ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.RaisedInner);
		}
		else if (e.Item.BackColor != Control.DefaultBackColor && e.Item.BackColor != Color.Empty)
		{
			e.Graphics.FillRectangle(ResPool.GetSolidBrush(e.Item.BackColor), rectangle);
		}
	}

	public virtual void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Rectangle rectangle = new Rectangle(Point.Empty, e.Item.Size);
		if (e.Item.Pressed)
		{
			ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.SunkenOuter);
		}
		else if (e.Item.Selected)
		{
			ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.RaisedInner);
		}
		else if (e.Item.BackColor != Control.DefaultBackColor && e.Item.BackColor != Color.Empty)
		{
			e.Graphics.FillRectangle(ResPool.GetSolidBrush(e.Item.BackColor), rectangle);
		}
		ToolStripRenderer.DrawDownArrow(e.Graphics, SystemPens.ControlText, e.Item.Width / 2 - 3, e.Item.Height / 2 - 1);
	}

	public virtual void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		if (e.Vertical)
		{
			e.Graphics.DrawLine(Pens.White, 4, 3, 4, e.Item.Height - 1);
			e.Graphics.DrawLine(SystemPens.ControlDark, 3, 3, 3, e.Item.Height - 1);
		}
		else if (!e.Item.IsOnDropDown)
		{
			e.Graphics.DrawLine(Pens.White, 2, 4, e.Item.Right - 1, 4);
			e.Graphics.DrawLine(SystemPens.ControlDark, 2, 3, e.Item.Right - 1, 3);
		}
		else
		{
			e.Graphics.DrawLine(Pens.White, 3, 4, e.Item.Right - 4, 4);
			e.Graphics.DrawLine(SystemPens.ControlDark, 3, 3, e.Item.Right - 4, 3);
		}
	}

	public virtual void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
	{
		ToolStripSplitButton toolStripSplitButton = (ToolStripSplitButton)e.Item;
		Rectangle rectangle = new Rectangle(Point.Empty, toolStripSplitButton.ButtonBounds.Size);
		Point location = new Point(toolStripSplitButton.Width - toolStripSplitButton.DropDownButtonBounds.Width, 0);
		Rectangle rectangle2 = new Rectangle(location, toolStripSplitButton.DropDownButtonBounds.Size);
		if (toolStripSplitButton.ButtonPressed)
		{
			ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.SunkenOuter);
		}
		else if (toolStripSplitButton.ButtonSelected)
		{
			ControlPaint.DrawBorder3D(e.Graphics, rectangle, Border3DStyle.RaisedInner);
		}
		else if (e.Item.BackColor != Control.DefaultBackColor && e.Item.BackColor != Color.Empty)
		{
			e.Graphics.FillRectangle(ResPool.GetSolidBrush(e.Item.BackColor), rectangle);
		}
		if (toolStripSplitButton.DropDownButtonPressed || toolStripSplitButton.ButtonPressed)
		{
			ControlPaint.DrawBorder3D(e.Graphics, rectangle2, Border3DStyle.SunkenOuter);
		}
		else if (toolStripSplitButton.DropDownButtonSelected || toolStripSplitButton.ButtonSelected)
		{
			ControlPaint.DrawBorder3D(e.Graphics, rectangle2, Border3DStyle.RaisedInner);
		}
		else if (e.Item.BackColor != Control.DefaultBackColor && e.Item.BackColor != Color.Empty)
		{
			e.Graphics.FillRectangle(ResPool.GetSolidBrush(e.Item.BackColor), rectangle2);
		}
	}

	public virtual void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
	{
		if (e.ToolStrip.BackgroundImage == null)
		{
			e.Graphics.Clear(e.BackColor);
		}
		if (e.ToolStrip is StatusStrip)
		{
			e.Graphics.DrawLine(Pens.White, e.AffectedBounds.Left, e.AffectedBounds.Top, e.AffectedBounds.Right, e.AffectedBounds.Top);
		}
	}

	public virtual void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
	{
		if (!(e.ToolStrip is StatusStrip))
		{
			if (e.ToolStrip is ToolStripDropDown)
			{
				ControlPaint.DrawBorder3D(e.Graphics, e.AffectedBounds, Border3DStyle.Raised);
				return;
			}
			e.Graphics.DrawLine(SystemPens.ControlDark, new Point(e.ToolStrip.Left, e.ToolStrip.Height - 2), new Point(e.ToolStrip.Right, e.ToolStrip.Height - 2));
			e.Graphics.DrawLine(Pens.White, new Point(e.ToolStrip.Left, e.ToolStrip.Height - 1), new Point(e.ToolStrip.Right, e.ToolStrip.Height - 1));
		}
	}
}
