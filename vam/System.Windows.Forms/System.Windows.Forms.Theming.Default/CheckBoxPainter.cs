using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.Theming.Default;

internal class CheckBoxPainter
{
	protected SystemResPool ResPool => ThemeEngine.Current.ResPool;

	private Color ColorControl => SystemColors.Control;

	public void PaintCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, ElementState state, FlatStyle style, CheckState checkState)
	{
		switch (style)
		{
		case FlatStyle.Standard:
		case FlatStyle.System:
			switch (state)
			{
			case ElementState.Normal:
				DrawNormalCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Hot:
				DrawHotCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Pressed:
				DrawPressedCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Disabled:
				DrawDisabledCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			}
			break;
		case FlatStyle.Flat:
			switch (state)
			{
			case ElementState.Normal:
				DrawFlatNormalCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Hot:
				DrawFlatHotCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Pressed:
				DrawFlatPressedCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Disabled:
				DrawFlatDisabledCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			}
			break;
		case FlatStyle.Popup:
			switch (state)
			{
			case ElementState.Normal:
				DrawPopupNormalCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Hot:
				DrawPopupHotCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Pressed:
				DrawPopupPressedCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			case ElementState.Disabled:
				DrawPopupDisabledCheckBox(g, bounds, backColor, foreColor, checkState);
				break;
			}
			break;
		}
	}

	public virtual void DrawNormalCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		int num = ((bounds.Height <= bounds.Width) ? bounds.Height : bounds.Width);
		int x = Math.Max(0, bounds.X + bounds.Width / 2 - num / 2);
		int y = Math.Max(0, bounds.Y + bounds.Height / 2 - num / 2);
		Rectangle rectangle = new Rectangle(x, y, num, num);
		g.FillRectangle(SystemBrushes.ControlLightLight, rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 3, rectangle.Height - 3);
		Pen controlDark = SystemPens.ControlDark;
		g.DrawLine(controlDark, rectangle.X, rectangle.Y, rectangle.X, rectangle.Bottom - 2);
		g.DrawLine(controlDark, rectangle.X + 1, rectangle.Y, rectangle.Right - 2, rectangle.Y);
		controlDark = SystemPens.ControlDarkDark;
		g.DrawLine(controlDark, rectangle.X + 1, rectangle.Y + 1, rectangle.X + 1, rectangle.Bottom - 3);
		g.DrawLine(controlDark, rectangle.X + 2, rectangle.Y + 1, rectangle.Right - 3, rectangle.Y + 1);
		controlDark = SystemPens.ControlLightLight;
		g.DrawLine(controlDark, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1, rectangle.Bottom - 1);
		g.DrawLine(controlDark, rectangle.X, rectangle.Bottom - 1, rectangle.Right - 1, rectangle.Bottom - 1);
		using (Pen pen = new Pen(ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl)))
		{
			g.DrawLine(pen, rectangle.X + 1, rectangle.Bottom - 2, rectangle.Right - 2, rectangle.Bottom - 2);
			g.DrawLine(pen, rectangle.Right - 2, rectangle.Y + 1, rectangle.Right - 2, rectangle.Bottom - 2);
		}
		switch (state)
		{
		case CheckState.Checked:
			DrawCheck(g, bounds, Color.Black);
			break;
		case CheckState.Indeterminate:
			DrawCheck(g, bounds, SystemColors.ControlDarkDark);
			break;
		}
	}

	public virtual void DrawHotCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawNormalCheckBox(g, bounds, backColor, foreColor, state);
	}

	public virtual void DrawPressedCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		int num = ((bounds.Height <= bounds.Width) ? bounds.Height : bounds.Width);
		int x = Math.Max(0, bounds.X + bounds.Width / 2 - num / 2);
		int y = Math.Max(0, bounds.Y + bounds.Height / 2 - num / 2);
		Rectangle rectangle = new Rectangle(x, y, num, num);
		g.FillRectangle(ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl), rectangle.X + 2, rectangle.Y + 2, rectangle.Width - 3, rectangle.Height - 3);
		Pen controlDark = SystemPens.ControlDark;
		g.DrawLine(controlDark, rectangle.X, rectangle.Y, rectangle.X, rectangle.Bottom - 2);
		g.DrawLine(controlDark, rectangle.X + 1, rectangle.Y, rectangle.Right - 2, rectangle.Y);
		controlDark = SystemPens.ControlDarkDark;
		g.DrawLine(controlDark, rectangle.X + 1, rectangle.Y + 1, rectangle.X + 1, rectangle.Bottom - 3);
		g.DrawLine(controlDark, rectangle.X + 2, rectangle.Y + 1, rectangle.Right - 3, rectangle.Y + 1);
		controlDark = SystemPens.ControlLightLight;
		g.DrawLine(controlDark, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1, rectangle.Bottom - 1);
		g.DrawLine(controlDark, rectangle.X, rectangle.Bottom - 1, rectangle.Right - 1, rectangle.Bottom - 1);
		using (Pen pen = new Pen(ResPool.GetHatchBrush(HatchStyle.Percent50, Color.FromArgb(Clamp(ColorControl.R + 3, 0, 255), ColorControl.G, ColorControl.B), ColorControl)))
		{
			g.DrawLine(pen, rectangle.X + 1, rectangle.Bottom - 2, rectangle.Right - 2, rectangle.Bottom - 2);
			g.DrawLine(pen, rectangle.Right - 2, rectangle.Y + 1, rectangle.Right - 2, rectangle.Bottom - 2);
		}
		switch (state)
		{
		case CheckState.Checked:
			DrawCheck(g, bounds, Color.Black);
			break;
		case CheckState.Indeterminate:
			DrawCheck(g, bounds, SystemColors.ControlDarkDark);
			break;
		}
	}

	public virtual void DrawDisabledCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawPressedCheckBox(g, bounds, backColor, foreColor, CheckState.Unchecked);
		if (state == CheckState.Checked || state == CheckState.Indeterminate)
		{
			DrawCheck(g, bounds, SystemColors.ControlDark);
		}
	}

	public virtual void DrawFlatNormalCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		Rectangle bounds2 = new Rectangle(bounds.X, bounds.Y, Math.Max(bounds.Width - 2, 0), Math.Max(bounds.Height - 2, 0));
		g.FillRectangle(rect: new Rectangle(bounds2.X + 1, bounds2.Y + 1, Math.Max(bounds2.Width - 2, 0), Math.Max(bounds2.Height - 2, 0)), brush: ResPool.GetSolidBrush(ControlPaint.LightLight(backColor)));
		ControlPaint.DrawBorder(g, bounds2, foreColor, ButtonBorderStyle.Solid);
		bounds.Offset(-1, 0);
		switch (state)
		{
		case CheckState.Checked:
			DrawCheck(g, bounds, Color.Black);
			break;
		case CheckState.Indeterminate:
			DrawCheck(g, bounds, SystemColors.ControlDarkDark);
			break;
		}
	}

	public virtual void DrawFlatHotCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		Rectangle bounds2 = new Rectangle(bounds.X, bounds.Y, Math.Max(bounds.Width - 2, 0), Math.Max(bounds.Height - 2, 0));
		g.FillRectangle(rect: new Rectangle(bounds2.X + 1, bounds2.Y + 1, Math.Max(bounds2.Width - 2, 0), Math.Max(bounds2.Height - 2, 0)), brush: ResPool.GetSolidBrush(backColor));
		ControlPaint.DrawBorder(g, bounds2, foreColor, ButtonBorderStyle.Solid);
		bounds.Offset(-1, 0);
		switch (state)
		{
		case CheckState.Checked:
			DrawCheck(g, bounds, Color.Black);
			break;
		case CheckState.Indeterminate:
			DrawCheck(g, bounds, SystemColors.ControlDarkDark);
			break;
		}
	}

	public virtual void DrawFlatPressedCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawFlatNormalCheckBox(g, bounds, backColor, foreColor, state);
	}

	public virtual void DrawFlatDisabledCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		Rectangle bounds2 = new Rectangle(bounds.X, bounds.Y, Math.Max(bounds.Width - 2, 0), Math.Max(bounds.Height - 2, 0));
		ControlPaint.DrawBorder(g, bounds2, foreColor, ButtonBorderStyle.Solid);
		bounds.Offset(-1, 0);
		if (state == CheckState.Checked || state == CheckState.Indeterminate)
		{
			DrawCheck(g, bounds, SystemColors.ControlDarkDark);
		}
	}

	public virtual void DrawPopupNormalCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawFlatNormalCheckBox(g, bounds, backColor, foreColor, state);
	}

	public virtual void DrawPopupHotCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		Rectangle rectangle = new Rectangle(bounds.X, bounds.Y, Math.Max(bounds.Width - 1, 0), Math.Max(bounds.Height - 1, 0));
		g.FillRectangle(rect: new Rectangle(rectangle.X + 1, rectangle.Y + 1, Math.Max(rectangle.Width - 3, 0), Math.Max(rectangle.Height - 3, 0)), brush: ResPool.GetSolidBrush(ControlPaint.LightLight(backColor)));
		ThemeEngine.Current.CPDrawBorder3D(g, rectangle, Border3DStyle.SunkenInner, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, backColor);
		bounds.Offset(-1, 0);
		switch (state)
		{
		case CheckState.Checked:
			DrawCheck(g, bounds, Color.Black);
			break;
		case CheckState.Indeterminate:
			DrawCheck(g, bounds, SystemColors.ControlDarkDark);
			break;
		}
	}

	public virtual void DrawPopupPressedCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		Rectangle rectangle = new Rectangle(bounds.X, bounds.Y, Math.Max(bounds.Width - 1, 0), Math.Max(bounds.Height - 1, 0));
		g.FillRectangle(rect: new Rectangle(rectangle.X + 1, rectangle.Y + 1, Math.Max(rectangle.Width - 3, 0), Math.Max(rectangle.Height - 3, 0)), brush: ResPool.GetSolidBrush(backColor));
		ThemeEngine.Current.CPDrawBorder3D(g, rectangle, Border3DStyle.SunkenInner, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom, backColor);
		bounds.Offset(-1, 0);
		switch (state)
		{
		case CheckState.Checked:
			DrawCheck(g, bounds, Color.Black);
			break;
		case CheckState.Indeterminate:
			DrawCheck(g, bounds, SystemColors.ControlDarkDark);
			break;
		}
	}

	public virtual void DrawPopupDisabledCheckBox(Graphics g, Rectangle bounds, Color backColor, Color foreColor, CheckState state)
	{
		DrawFlatDisabledCheckBox(g, bounds, backColor, foreColor, state);
	}

	public virtual void DrawCheck(Graphics g, Rectangle bounds, Color checkColor)
	{
		int num = ((bounds.Height <= bounds.Width) ? (bounds.Height / 2) : (bounds.Width / 2));
		Pen pen = ResPool.GetPen(checkColor);
		if (num < 7)
		{
			int num2 = Math.Max(3, num / 3);
			int num3 = Math.Max(1, num / 9);
			Rectangle rectangle = new Rectangle(bounds.X + bounds.Width / 2 - num / 2 - 1, bounds.Y + bounds.Height / 2 - num / 2 - 1, num, num);
			for (int i = 0; i < num2; i++)
			{
				g.DrawLine(pen, rectangle.Left + num2 / 2, rectangle.Top + num2 + i, rectangle.Left + num2 / 2 + 2 * num3, rectangle.Top + num2 + 2 * num3 + i);
				g.DrawLine(pen, rectangle.Left + num2 / 2 + 2 * num3, rectangle.Top + num2 + 2 * num3 + i, rectangle.Left + num2 / 2 + 6 * num3, rectangle.Top + num2 - 2 * num3 + i);
			}
			return;
		}
		int num4 = Math.Max(3, num / 3) + 1;
		int num5 = bounds.Width / 2;
		int num6 = bounds.Height / 2;
		Rectangle rectangle2 = new Rectangle(bounds.X + num5 - num / 2 - 1, bounds.Y + num6 - num / 2, num, num);
		int num7 = num / 3;
		int num8 = num - num7 - 1;
		for (int j = 0; j < num4; j++)
		{
			g.DrawLine(pen, rectangle2.X, rectangle2.Bottom - 1 - num7 - j, rectangle2.X + num7, rectangle2.Bottom - 1 - j);
			g.DrawLine(pen, rectangle2.X + num7, rectangle2.Bottom - 1 - j, rectangle2.Right - 1, rectangle2.Bottom - j - 1 - num8);
		}
	}

	private int Clamp(int value, int lower, int upper)
	{
		if (value < lower)
		{
			return lower;
		}
		if (value > upper)
		{
			return upper;
		}
		return value;
	}
}
