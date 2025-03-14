using System.Drawing;

namespace System.Windows.Forms.Theming.Default;

internal class ButtonPainter
{
	protected SystemResPool ResPool => ThemeEngine.Current.ResPool;

	public virtual void Draw(Graphics g, Rectangle bounds, ButtonThemeState state, Color backColor, Color foreColor)
	{
		bool flag = ((backColor.ToArgb() == ThemeEngine.Current.ColorControl.ToArgb() || backColor == Color.Empty) ? true : false);
		CPColor cPColor = ((!flag) ? ResPool.GetCPColor(backColor) : CPColor.Empty);
		switch (state)
		{
		case ButtonThemeState.Normal:
		case ButtonThemeState.Entered:
		case ButtonThemeState.Disabled:
		{
			Pen pen = ((!flag) ? ResPool.GetPen(cPColor.LightLight) : SystemPens.ControlLightLight);
			g.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 2);
			g.DrawLine(pen, bounds.X + 1, bounds.Y, bounds.Right - 2, bounds.Y);
			pen = ((!flag) ? ResPool.GetPen(backColor) : SystemPens.Control);
			g.DrawLine(pen, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Bottom - 3);
			g.DrawLine(pen, bounds.X + 2, bounds.Y + 1, bounds.Right - 3, bounds.Y + 1);
			pen = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark);
			g.DrawLine(pen, bounds.X + 1, bounds.Bottom - 2, bounds.Right - 2, bounds.Bottom - 2);
			g.DrawLine(pen, bounds.Right - 2, bounds.Y + 1, bounds.Right - 2, bounds.Bottom - 3);
			pen = ((!flag) ? ResPool.GetPen(cPColor.DarkDark) : SystemPens.ControlDarkDark);
			g.DrawLine(pen, bounds.X, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
			g.DrawLine(pen, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 2);
			break;
		}
		case ButtonThemeState.Pressed:
		{
			g.DrawRectangle(ResPool.GetPen(foreColor), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			bounds.Inflate(-1, -1);
			Pen pen = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark);
			g.DrawRectangle(pen, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			break;
		}
		case ButtonThemeState.Default:
		{
			g.DrawRectangle(ResPool.GetPen(foreColor), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			bounds.Inflate(-1, -1);
			Pen pen = ((!flag) ? ResPool.GetPen(cPColor.LightLight) : SystemPens.ControlLightLight);
			g.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 2);
			g.DrawLine(pen, bounds.X + 1, bounds.Y, bounds.Right - 2, bounds.Y);
			pen = ((!flag) ? ResPool.GetPen(backColor) : SystemPens.Control);
			g.DrawLine(pen, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Bottom - 3);
			g.DrawLine(pen, bounds.X + 2, bounds.Y + 1, bounds.Right - 3, bounds.Y + 1);
			pen = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark);
			g.DrawLine(pen, bounds.X + 1, bounds.Bottom - 2, bounds.Right - 2, bounds.Bottom - 2);
			g.DrawLine(pen, bounds.Right - 2, bounds.Y + 1, bounds.Right - 2, bounds.Bottom - 3);
			pen = ((!flag) ? ResPool.GetPen(cPColor.DarkDark) : SystemPens.ControlDarkDark);
			g.DrawLine(pen, bounds.X, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
			g.DrawLine(pen, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 2);
			break;
		}
		}
	}

	public virtual void DrawFlat(Graphics g, Rectangle bounds, ButtonThemeState state, Color backColor, Color foreColor, FlatButtonAppearance appearance)
	{
		bool flag = ((backColor.ToArgb() == ThemeEngine.Current.ColorControl.ToArgb() || backColor == Color.Empty) ? true : false);
		CPColor cPColor = ((!flag) ? ResPool.GetCPColor(backColor) : CPColor.Empty);
		switch (state)
		{
		case ButtonThemeState.Entered:
		case ButtonThemeState.Entered | ButtonThemeState.Default:
			if (appearance.MouseOverBackColor != Color.Empty)
			{
				g.FillRectangle(ResPool.GetSolidBrush(appearance.MouseOverBackColor), bounds);
			}
			else
			{
				g.FillRectangle(ResPool.GetSolidBrush(ChangeIntensity(backColor, 0.9f)), bounds);
			}
			break;
		case ButtonThemeState.Pressed:
			if (appearance.MouseDownBackColor != Color.Empty)
			{
				g.FillRectangle(ResPool.GetSolidBrush(appearance.MouseDownBackColor), bounds);
			}
			else
			{
				g.FillRectangle(ResPool.GetSolidBrush(ChangeIntensity(backColor, 0.95f)), bounds);
			}
			break;
		case ButtonThemeState.Default:
			if (appearance.CheckedBackColor != Color.Empty)
			{
				g.FillRectangle(ResPool.GetSolidBrush(appearance.CheckedBackColor), bounds);
			}
			break;
		}
		Pen pen = ((!(appearance.BorderColor == Color.Empty)) ? ResPool.GetSizedPen(appearance.BorderColor, appearance.BorderSize) : ((!flag) ? ResPool.GetSizedPen(cPColor.DarkDark, appearance.BorderSize) : SystemPens.ControlDarkDark));
		bounds.Width--;
		bounds.Height--;
		if (appearance.BorderSize > 0)
		{
			g.DrawRectangle(pen, bounds);
		}
	}

	public virtual void DrawPopup(Graphics g, Rectangle bounds, ButtonThemeState state, Color backColor, Color foreColor)
	{
		bool flag = ((backColor.ToArgb() == ThemeEngine.Current.ColorControl.ToArgb() || backColor == Color.Empty) ? true : false);
		CPColor cPColor = ((!flag) ? ResPool.GetCPColor(backColor) : CPColor.Empty);
		switch (state)
		{
		case ButtonThemeState.Normal:
		case ButtonThemeState.Pressed:
		case ButtonThemeState.Disabled:
		case ButtonThemeState.Default:
		{
			Pen pen = ((!flag) ? ResPool.GetPen(cPColor.DarkDark) : SystemPens.ControlDarkDark);
			bounds.Width--;
			bounds.Height--;
			g.DrawRectangle(pen, bounds);
			if (state == ButtonThemeState.Default || state == ButtonThemeState.Pressed)
			{
				bounds.Inflate(-1, -1);
				g.DrawRectangle(pen, bounds);
			}
			break;
		}
		case ButtonThemeState.Entered:
		{
			Pen pen = ((!flag) ? ResPool.GetPen(cPColor.LightLight) : SystemPens.ControlLightLight);
			g.DrawLine(pen, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 2);
			g.DrawLine(pen, bounds.X + 1, bounds.Y, bounds.Right - 2, bounds.Y);
			pen = ((!flag) ? ResPool.GetPen(cPColor.Dark) : SystemPens.ControlDark);
			g.DrawLine(pen, bounds.X, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
			g.DrawLine(pen, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 2);
			break;
		}
		}
	}

	private static Color ChangeIntensity(Color baseColor, float percent)
	{
		ControlPaint.Color2HBS(baseColor, out var h, out var l, out var s);
		int lum = Math.Min(255, (int)((float)l * percent));
		return ControlPaint.HBS2Color(h, lum, s);
	}
}
