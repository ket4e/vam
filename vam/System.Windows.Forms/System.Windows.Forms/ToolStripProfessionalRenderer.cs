using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms;

public class ToolStripProfessionalRenderer : ToolStripRenderer
{
	private ProfessionalColorTable color_table;

	private bool rounded_edges;

	public ProfessionalColorTable ColorTable => color_table;

	public bool RoundedEdges
	{
		get
		{
			return rounded_edges;
		}
		set
		{
			rounded_edges = value;
		}
	}

	public ToolStripProfessionalRenderer()
		: this(new ProfessionalColorTable())
	{
	}

	public ToolStripProfessionalRenderer(ProfessionalColorTable professionalColorTable)
	{
		color_table = professionalColorTable;
		rounded_edges = true;
	}

	protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
	{
		base.OnRenderArrow(e);
	}

	protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
	{
		if (!e.Item.Enabled)
		{
			return;
		}
		Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
		if (e.Item is ToolStripButton && (e.Item as ToolStripButton).Checked && !e.Item.Selected)
		{
			if (ColorTable.UseSystemColors)
			{
				e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.ButtonCheckedHighlight), rect);
			}
			else
			{
				using Brush brush = new LinearGradientBrush(rect, ColorTable.ButtonCheckedGradientBegin, ColorTable.ButtonCheckedGradientEnd, LinearGradientMode.Vertical);
				e.Graphics.FillRectangle(brush, rect);
			}
		}
		else if (e.Item is ToolStripDropDownItem && e.Item.Pressed)
		{
			using Brush brush2 = new LinearGradientBrush(rect, ColorTable.ToolStripGradientBegin, ColorTable.ToolStripGradientEnd, LinearGradientMode.Vertical);
			e.Graphics.FillRectangle(brush2, rect);
		}
		else if (e.Item.Pressed || (e.Item is ToolStripButton && (e.Item as ToolStripButton).Checked))
		{
			using Brush brush3 = new LinearGradientBrush(rect, ColorTable.ButtonPressedGradientBegin, ColorTable.ButtonPressedGradientEnd, LinearGradientMode.Vertical);
			e.Graphics.FillRectangle(brush3, rect);
		}
		else if (e.Item.Selected)
		{
			using Brush brush4 = new LinearGradientBrush(rect, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd, LinearGradientMode.Vertical);
			e.Graphics.FillRectangle(brush4, rect);
		}
		else if (e.Item.BackColor != Control.DefaultBackColor && e.Item.BackColor != Color.Empty)
		{
			using Brush brush5 = new SolidBrush(e.Item.BackColor);
			e.Graphics.FillRectangle(brush5, rect);
		}
		rect.Width--;
		rect.Height--;
		if (e.Item.Selected && !e.Item.Pressed)
		{
			using Pen pen = new Pen(ColorTable.ButtonSelectedBorder);
			e.Graphics.DrawRectangle(pen, rect);
		}
		else if (e.Item.Pressed)
		{
			using Pen pen2 = new Pen(ColorTable.ButtonPressedBorder);
			e.Graphics.DrawRectangle(pen2, rect);
		}
		else if (e.Item is ToolStripButton && (e.Item as ToolStripButton).Checked)
		{
			using Pen pen3 = new Pen(ColorTable.ButtonPressedBorder);
			e.Graphics.DrawRectangle(pen3, rect);
		}
		base.OnRenderButtonBackground(e);
	}

	protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
	{
		Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
		if (e.Item.Selected && !e.Item.Pressed)
		{
			using Brush brush = new LinearGradientBrush(rect, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd, LinearGradientMode.Vertical);
			e.Graphics.FillRectangle(brush, rect);
		}
		else if (e.Item.Pressed)
		{
			using Brush brush2 = new LinearGradientBrush(rect, ColorTable.ImageMarginGradientMiddle, ColorTable.ImageMarginGradientEnd, LinearGradientMode.Vertical);
			e.Graphics.FillRectangle(brush2, rect);
		}
		rect.Width--;
		rect.Height--;
		if (e.Item.Selected && !e.Item.Pressed)
		{
			using Pen pen = new Pen(ColorTable.ButtonSelectedBorder);
			e.Graphics.DrawRectangle(pen, rect);
		}
		else if (e.Item.Pressed)
		{
			using Pen pen2 = new Pen(ColorTable.MenuBorder);
			e.Graphics.DrawRectangle(pen2, rect);
		}
		base.OnRenderDropDownButtonBackground(e);
	}

	protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
	{
		if (e.GripStyle == ToolStripGripStyle.Hidden)
		{
			return;
		}
		if (e.GripDisplayStyle == ToolStripGripDisplayStyle.Vertical)
		{
			Rectangle rect = new Rectangle(e.GripBounds.Left, e.GripBounds.Top + 5, 2, 2);
			for (int i = 0; i < e.GripBounds.Height - 12; i += 4)
			{
				e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.GripLight), rect);
				rect.Offset(0, 4);
			}
			Rectangle rect2 = new Rectangle(e.GripBounds.Left - 1, e.GripBounds.Top + 4, 2, 2);
			for (int j = 0; j < e.GripBounds.Height - 12; j += 4)
			{
				e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.GripDark), rect2);
				rect2.Offset(0, 4);
			}
		}
		else
		{
			Rectangle rect3 = new Rectangle(e.GripBounds.Left + 5, e.GripBounds.Top, 2, 2);
			for (int k = 0; k < e.GripBounds.Width - 11; k += 4)
			{
				e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.GripLight), rect3);
				rect3.Offset(4, 0);
			}
			Rectangle rect4 = new Rectangle(e.GripBounds.Left + 4, e.GripBounds.Top - 1, 2, 2);
			for (int l = 0; l < e.GripBounds.Width - 11; l += 4)
			{
				e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.GripDark), rect4);
				rect4.Offset(4, 0);
			}
		}
		base.OnRenderGrip(e);
	}

	protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
	{
		if (!(e.ToolStrip is ToolStripOverflow))
		{
			Rectangle rect = new Rectangle(1, 2, 24, e.ToolStrip.Height - 3);
			using LinearGradientBrush brush = new LinearGradientBrush(rect, ColorTable.ToolStripGradientBegin, ColorTable.ToolStripGradientEnd, LinearGradientMode.Horizontal);
			e.Graphics.FillRectangle(brush, rect);
		}
		base.OnRenderImageMargin(e);
	}

	protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
	{
		if (e.Item.Selected)
		{
			e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.CheckPressedBackground), e.ImageRectangle);
			e.Graphics.DrawRectangle(ThemeEngine.Current.ResPool.GetPen(ColorTable.ButtonPressedBorder), e.ImageRectangle);
		}
		else if (e.Item.Pressed)
		{
			e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.CheckSelectedBackground), e.ImageRectangle);
			e.Graphics.DrawRectangle(ThemeEngine.Current.ResPool.GetPen(ColorTable.ButtonSelectedBorder), e.ImageRectangle);
		}
		else
		{
			e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.CheckSelectedBackground), e.ImageRectangle);
			e.Graphics.DrawRectangle(ThemeEngine.Current.ResPool.GetPen(ColorTable.ButtonSelectedBorder), e.ImageRectangle);
		}
		if (e.Item.Image == null)
		{
			ControlPaint.DrawMenuGlyph(e.Graphics, new Rectangle(6, 5, 7, 6), MenuGlyph.Checkmark);
		}
		base.OnRenderItemCheck(e);
	}

	protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
	{
		base.OnRenderItemImage(e);
	}

	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
	{
		base.OnRenderItemText(e);
	}

	protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
	{
		base.OnRenderLabelBackground(e);
	}

	protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.Item;
		if (toolStripMenuItem.IsOnDropDown)
		{
			Rectangle rect = new Rectangle(1, 0, e.Item.Bounds.Width - 3, e.Item.Bounds.Height - 1);
			if ((e.Item.Selected || e.Item.Pressed) && e.Item.Enabled)
			{
				e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.MenuItemSelectedGradientEnd), rect);
			}
			if (toolStripMenuItem.Selected || toolStripMenuItem.Pressed)
			{
				using Pen pen = new Pen(ColorTable.MenuItemBorder);
				e.Graphics.DrawRectangle(pen, rect);
			}
		}
		else
		{
			Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
			if (e.Item.Pressed)
			{
				using Brush brush = new LinearGradientBrush(rect, ColorTable.ToolStripGradientBegin, ColorTable.ToolStripGradientEnd, LinearGradientMode.Vertical);
				e.Graphics.FillRectangle(brush, rect);
			}
			else if (e.Item.Selected)
			{
				using Brush brush2 = new LinearGradientBrush(rect, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd, LinearGradientMode.Vertical);
				e.Graphics.FillRectangle(brush2, rect);
			}
			else if (e.Item.BackColor != Control.DefaultBackColor && e.Item.BackColor != Color.Empty)
			{
				using Brush brush3 = new SolidBrush(e.Item.BackColor);
				e.Graphics.FillRectangle(brush3, rect);
			}
			rect.Width--;
			rect.Height--;
			if (toolStripMenuItem.Selected || toolStripMenuItem.Pressed)
			{
				if (toolStripMenuItem.HasDropDownItems && toolStripMenuItem.DropDown.Visible)
				{
					using Pen pen2 = new Pen(ColorTable.MenuBorder);
					e.Graphics.DrawRectangle(pen2, rect);
				}
				else
				{
					using Pen pen3 = new Pen(ColorTable.MenuItemBorder);
					e.Graphics.DrawRectangle(pen3, rect);
				}
			}
		}
		base.OnRenderMenuItemBackground(e);
	}

	protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
	{
		LinearGradientMode linearGradientMode = ((e.ToolStrip.Orientation != Orientation.Vertical) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal);
		Rectangle rectangle = ((e.ToolStrip.Orientation != 0) ? new Rectangle(0, e.Item.Height - 11, e.Item.Width - 1, 11) : new Rectangle(e.Item.Width - 11, 0, 11, e.Item.Height - 1));
		if (e.Item.Selected && !e.Item.Pressed)
		{
			using Brush brush = new LinearGradientBrush(rectangle, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd, linearGradientMode);
			e.Graphics.FillRectangle(brush, rectangle);
		}
		else if (e.Item.Pressed)
		{
			using Brush brush2 = new LinearGradientBrush(rectangle, ColorTable.ButtonPressedGradientBegin, ColorTable.ButtonPressedGradientEnd, linearGradientMode);
			e.Graphics.FillRectangle(brush2, rectangle);
		}
		else
		{
			using Brush brush3 = new LinearGradientBrush(rectangle, ColorTable.OverflowButtonGradientBegin, ColorTable.OverflowButtonGradientEnd, linearGradientMode);
			e.Graphics.FillRectangle(brush3, rectangle);
		}
		PaintOverflowArrow(e, rectangle);
		base.OnRenderOverflowButtonBackground(e);
	}

	protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
	{
		if (e.Vertical)
		{
			Rectangle rect = new Rectangle(4, 6, 1, e.Item.Height - 10);
			e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.SeparatorLight), rect);
			Rectangle rect2 = new Rectangle(3, 5, 1, e.Item.Height - 10);
			e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.SeparatorDark), rect2);
		}
		else
		{
			if (!e.Item.IsOnDropDown)
			{
				Rectangle rect3 = new Rectangle(6, 4, e.Item.Width - 10, 1);
				e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.SeparatorLight), rect3);
			}
			Rectangle rect4 = ((!e.Item.IsOnDropDown) ? new Rectangle(5, 3, e.Item.Width - 10, 1) : ((!e.Item.UseImageMargin) ? new Rectangle(7, 3, e.Item.Width - 7, 1) : new Rectangle(35, 3, e.Item.Width - 36, 1)));
			e.Graphics.FillRectangle(ThemeEngine.Current.ResPool.GetSolidBrush(ColorTable.SeparatorDark), rect4);
		}
		base.OnRenderSeparator(e);
	}

	protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
	{
		ToolStripSplitButton toolStripSplitButton = (ToolStripSplitButton)e.Item;
		Rectangle rect = new Rectangle(0, 0, toolStripSplitButton.Width, toolStripSplitButton.Height);
		Rectangle rect2 = new Rectangle(0, 0, toolStripSplitButton.ButtonBounds.Width, toolStripSplitButton.ButtonBounds.Height);
		if (toolStripSplitButton.ButtonSelected && !toolStripSplitButton.DropDownButtonPressed)
		{
			using Brush brush = new LinearGradientBrush(rect, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd, LinearGradientMode.Vertical);
			e.Graphics.FillRectangle(brush, rect);
		}
		if (toolStripSplitButton.ButtonPressed)
		{
			using Brush brush2 = new LinearGradientBrush(rect2, ColorTable.ButtonPressedGradientBegin, ColorTable.ButtonPressedGradientEnd, LinearGradientMode.Vertical);
			e.Graphics.FillRectangle(brush2, rect2);
		}
		rect.Width--;
		rect.Height--;
		if (e.Item.Selected && !toolStripSplitButton.DropDownButtonPressed)
		{
			using Pen pen = new Pen(ColorTable.ButtonSelectedBorder);
			e.Graphics.DrawRectangle(pen, rect);
			e.Graphics.DrawLine(pen, rect2.Right, 0, rect2.Right, rect2.Height);
		}
		else if (e.Item.Pressed)
		{
			using Pen pen2 = new Pen(ColorTable.MenuBorder);
			e.Graphics.DrawRectangle(pen2, rect);
		}
		base.OnRenderSplitButtonBackground(e);
	}

	protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
	{
		if (e.ToolStrip.BackgroundImage != null)
		{
			if (e.ToolStrip is StatusStrip)
			{
				e.Graphics.DrawLine(Pens.White, e.AffectedBounds.Left, e.AffectedBounds.Top, e.AffectedBounds.Right, e.AffectedBounds.Top);
			}
			return;
		}
		if (e.ToolStrip is ToolStripDropDown)
		{
			e.Graphics.Clear(ColorTable.ToolStripDropDownBackground);
			return;
		}
		if (e.ToolStrip is MenuStrip || e.ToolStrip is StatusStrip)
		{
			using LinearGradientBrush brush = new LinearGradientBrush(e.AffectedBounds, ColorTable.MenuStripGradientBegin, ColorTable.MenuStripGradientEnd, (e.ToolStrip.Orientation != 0) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal);
			e.Graphics.FillRectangle(brush, e.AffectedBounds);
		}
		else
		{
			using LinearGradientBrush brush2 = new LinearGradientBrush(e.AffectedBounds, ColorTable.ToolStripGradientBegin, ColorTable.ToolStripGradientEnd, (e.ToolStrip.Orientation != Orientation.Vertical) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal);
			e.Graphics.FillRectangle(brush2, e.AffectedBounds);
		}
		if (e.ToolStrip is StatusStrip)
		{
			e.Graphics.DrawLine(Pens.White, e.AffectedBounds.Left, e.AffectedBounds.Top, e.AffectedBounds.Right, e.AffectedBounds.Top);
		}
		base.OnRenderToolStripBackground(e);
	}

	protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
	{
		if (e.ToolStrip is ToolStripDropDown)
		{
			if (e.ToolStrip is ToolStripOverflow)
			{
				e.Graphics.DrawLines(ThemeEngine.Current.ResPool.GetPen(ColorTable.MenuBorder), new Point[5]
				{
					e.AffectedBounds.Location,
					new Point(e.AffectedBounds.Left, e.AffectedBounds.Bottom - 1),
					new Point(e.AffectedBounds.Right - 1, e.AffectedBounds.Bottom - 1),
					new Point(e.AffectedBounds.Right - 1, e.AffectedBounds.Top),
					new Point(e.AffectedBounds.Left, e.AffectedBounds.Top)
				});
			}
			else
			{
				e.Graphics.DrawLines(ThemeEngine.Current.ResPool.GetPen(ColorTable.MenuBorder), new Point[6]
				{
					new Point(e.AffectedBounds.Left + e.ConnectedArea.Left, e.AffectedBounds.Top),
					e.AffectedBounds.Location,
					new Point(e.AffectedBounds.Left, e.AffectedBounds.Bottom - 1),
					new Point(e.AffectedBounds.Right - 1, e.AffectedBounds.Bottom - 1),
					new Point(e.AffectedBounds.Right - 1, e.AffectedBounds.Top),
					new Point(e.AffectedBounds.Left + e.ConnectedArea.Right, e.AffectedBounds.Top)
				});
			}
		}
		else
		{
			if (e.ToolStrip is MenuStrip || e.ToolStrip is StatusStrip)
			{
				return;
			}
			using (Pen pen = new Pen(ColorTable.ToolStripBorder))
			{
				if (RoundedEdges)
				{
					e.Graphics.DrawLine(pen, new Point(2, e.ToolStrip.Height - 1), new Point(e.ToolStrip.Width - 3, e.ToolStrip.Height - 1));
					e.Graphics.DrawLine(pen, new Point(e.ToolStrip.Width - 2, e.ToolStrip.Height - 2), new Point(e.ToolStrip.Width - 1, e.ToolStrip.Height - 2));
					e.Graphics.DrawLine(pen, new Point(e.ToolStrip.Width - 1, 2), new Point(e.ToolStrip.Width - 1, e.ToolStrip.Height - 3));
				}
				else
				{
					e.Graphics.DrawLine(pen, new Point(e.ToolStrip.Left, e.ToolStrip.Bottom - 1), new Point(e.ToolStrip.Width, e.ToolStrip.Bottom - 1));
				}
			}
			base.OnRenderToolStripBorder(e);
		}
	}

	protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
	{
		base.OnRenderToolStripContentPanelBackground(e);
	}

	protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
	{
		using (LinearGradientBrush brush = new LinearGradientBrush(e.ToolStripPanel.Bounds, ColorTable.MenuStripGradientBegin, ColorTable.MenuStripGradientEnd, (e.ToolStripPanel.Orientation != 0) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal))
		{
			e.Graphics.FillRectangle(brush, e.ToolStripPanel.Bounds);
		}
		base.OnRenderToolStripPanelBackground(e);
	}

	protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
	{
		base.OnRenderToolStripStatusLabelBackground(e);
	}

	private static void PaintOverflowArrow(ToolStripItemRenderEventArgs e, Rectangle paint_here)
	{
		if (e.ToolStrip.Orientation == Orientation.Horizontal)
		{
			Point point = new Point(paint_here.X + 2, paint_here.Bottom - 9);
			e.Graphics.DrawLine(Pens.White, point.X + 1, point.Y + 1, point.X + 5, point.Y + 1);
			e.Graphics.DrawLine(Pens.Black, point.X, point.Y, point.X + 4, point.Y);
			e.Graphics.DrawLine(Pens.White, point.X + 3, point.Y + 4, point.X + 5, point.Y + 4);
			e.Graphics.DrawLine(Pens.White, point.X + 3, point.Y + 5, point.X + 4, point.Y + 5);
			e.Graphics.DrawLine(Pens.White, point.X + 3, point.Y + 4, point.X + 3, point.Y + 6);
			e.Graphics.DrawLine(Pens.Black, point.X, point.Y + 3, point.X + 4, point.Y + 3);
			e.Graphics.DrawLine(Pens.Black, point.X + 1, point.Y + 4, point.X + 3, point.Y + 4);
			e.Graphics.DrawLine(Pens.Black, point.X + 2, point.Y + 4, point.X + 2, point.Y + 5);
		}
		else
		{
			Point point2 = new Point(paint_here.Right - 9, paint_here.Y + 2);
			e.Graphics.DrawLine(Pens.White, point2.X + 1, point2.Y + 1, point2.X + 1, point2.Y + 5);
			e.Graphics.DrawLine(Pens.Black, point2.X, point2.Y, point2.X, point2.Y + 4);
			e.Graphics.DrawLine(Pens.White, point2.X + 4, point2.Y + 3, point2.X + 4, point2.Y + 5);
			e.Graphics.DrawLine(Pens.White, point2.X + 5, point2.Y + 3, point2.X + 5, point2.Y + 4);
			e.Graphics.DrawLine(Pens.White, point2.X + 4, point2.Y + 3, point2.X + 6, point2.Y + 3);
			e.Graphics.DrawLine(Pens.Black, point2.X + 3, point2.Y, point2.X + 3, point2.Y + 4);
			e.Graphics.DrawLine(Pens.Black, point2.X + 4, point2.Y + 1, point2.X + 4, point2.Y + 3);
			e.Graphics.DrawLine(Pens.Black, point2.X + 4, point2.Y + 2, point2.X + 5, point2.Y + 2);
		}
	}
}
