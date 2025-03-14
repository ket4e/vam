using System.Drawing;

namespace System.Windows.Forms.Theming.Default;

internal class RadioButtonPainter
{
	protected SystemResPool ResPool => ThemeEngine.Current.ResPool;

	public void PaintRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, ElementState state, FlatStyle style, bool isChecked)
	{
		switch (style)
		{
		case FlatStyle.Standard:
		case FlatStyle.System:
			switch (state)
			{
			case ElementState.Normal:
				DrawNormalRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Hot:
				DrawHotRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Pressed:
				DrawPressedRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Disabled:
				DrawDisabledRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			}
			break;
		case FlatStyle.Flat:
			switch (state)
			{
			case ElementState.Normal:
				DrawFlatNormalRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Hot:
				DrawFlatHotRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Pressed:
				DrawFlatPressedRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Disabled:
				DrawFlatDisabledRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			}
			break;
		case FlatStyle.Popup:
			switch (state)
			{
			case ElementState.Normal:
				DrawPopupNormalRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Hot:
				DrawPopupHotRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Pressed:
				DrawPopupPressedRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			case ElementState.Disabled:
				DrawPopupDisabledRadioButton(g, bounds, backColor, foreColor, isChecked);
				break;
			}
			break;
		}
	}

	public virtual void DrawNormalRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		ButtonState buttonState = ButtonState.Normal;
		if (isChecked)
		{
			buttonState |= ButtonState.Checked;
		}
		ControlPaint.DrawRadioButton(g, bounds, buttonState);
	}

	public virtual void DrawHotRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		DrawNormalRadioButton(g, bounds, backColor, foreColor, isChecked);
	}

	public virtual void DrawPressedRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		ButtonState buttonState = ButtonState.Pushed;
		if (isChecked)
		{
			buttonState |= ButtonState.Checked;
		}
		ControlPaint.DrawRadioButton(g, bounds, buttonState);
	}

	public virtual void DrawDisabledRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		ButtonState buttonState = ButtonState.Inactive;
		if (isChecked)
		{
			buttonState |= ButtonState.Checked;
		}
		ControlPaint.DrawRadioButton(g, bounds, buttonState);
	}

	public virtual void DrawFlatNormalRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		g.DrawArc(SystemPens.ControlDarkDark, bounds, 0f, 359f);
		g.FillPie(SystemBrushes.ControlLightLight, bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2, 0, 359);
		if (isChecked)
		{
			DrawFlatRadioGlyphDot(g, bounds, SystemColors.ControlDarkDark);
		}
	}

	public virtual void DrawFlatHotRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		g.DrawArc(SystemPens.ControlDarkDark, bounds, 0f, 359f);
		g.FillPie(SystemBrushes.ControlLight, bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2, 0, 359);
		if (isChecked)
		{
			DrawFlatRadioGlyphDot(g, bounds, SystemColors.ControlDarkDark);
		}
	}

	public virtual void DrawFlatPressedRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		g.DrawArc(SystemPens.ControlDarkDark, bounds, 0f, 359f);
		g.FillPie(SystemBrushes.ControlLightLight, bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2, 0, 359);
		if (isChecked)
		{
			DrawFlatRadioGlyphDot(g, bounds, SystemColors.ControlDarkDark);
		}
	}

	public virtual void DrawFlatDisabledRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		g.FillPie(SystemBrushes.Control, bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2, 0, 359);
		g.DrawArc(SystemPens.ControlDark, bounds, 0f, 359f);
		if (isChecked)
		{
			DrawFlatRadioGlyphDot(g, bounds, SystemColors.ControlDark);
		}
	}

	public virtual void DrawPopupNormalRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		g.FillPie(SystemBrushes.ControlLightLight, bounds, 0f, 359f);
		g.DrawArc(SystemPens.ControlDark, bounds, 0f, 359f);
		if (isChecked)
		{
			DrawFlatRadioGlyphDot(g, bounds, SystemColors.ControlDarkDark);
		}
	}

	public virtual void DrawPopupHotRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		g.FillPie(SystemBrushes.ControlLightLight, bounds, 0f, 359f);
		g.DrawArc(SystemPens.ControlLight, bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2, 0, 359);
		g.DrawArc(SystemPens.ControlDark, bounds, 135f, 180f);
		g.DrawArc(SystemPens.ControlLightLight, bounds, 315f, 180f);
		if (isChecked)
		{
			DrawFlatRadioGlyphDot(g, bounds, SystemColors.ControlDarkDark);
		}
	}

	public virtual void DrawPopupPressedRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		g.FillPie(SystemBrushes.ControlLightLight, bounds, 0f, 359f);
		g.DrawArc(SystemPens.ControlLight, bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2, 0, 359);
		g.DrawArc(SystemPens.ControlDark, bounds, 135f, 180f);
		g.DrawArc(SystemPens.ControlLightLight, bounds, 315f, 180f);
		if (isChecked)
		{
			DrawFlatRadioGlyphDot(g, bounds, SystemColors.ControlDarkDark);
		}
	}

	public virtual void DrawPopupDisabledRadioButton(Graphics g, Rectangle bounds, Color backColor, Color foreColor, bool isChecked)
	{
		g.FillPie(SystemBrushes.Control, bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2, 0, 359);
		g.DrawArc(SystemPens.ControlDark, bounds, 0f, 359f);
		if (isChecked)
		{
			DrawFlatRadioGlyphDot(g, bounds, SystemColors.ControlDarkDark);
		}
	}

	protected void DrawFlatRadioGlyphDot(Graphics g, Rectangle bounds, Color dotColor)
	{
		int num = Math.Max(1, Math.Min(bounds.Width, bounds.Height) / 3);
		Pen pen = ResPool.GetPen(dotColor);
		Brush solidBrush = ResPool.GetSolidBrush(dotColor);
		if (bounds.Height > 13)
		{
			g.FillPie(solidBrush, bounds.X + num, bounds.Y + num, bounds.Width - num * 2, bounds.Height - num * 2, 0, 359);
			return;
		}
		int num2 = bounds.Width / 2 + bounds.X;
		int num3 = bounds.Height / 2 + bounds.Y;
		g.DrawLine(pen, num2 - 1, num3, num2 + 2, num3);
		g.DrawLine(pen, num2 - 1, num3 + 1, num2 + 2, num3 + 1);
		g.DrawLine(pen, num2, num3 - 1, num2, num3 + 2);
		g.DrawLine(pen, num2 + 1, num3 - 1, num2 + 1, num3 + 2);
	}
}
