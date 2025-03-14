using System.Drawing;

namespace System.Windows.Forms;

public sealed class ControlPaint
{
	private static int RGBMax = 255;

	private static int HLSMax = 255;

	[System.MonoTODO("Stub, does nothing")]
	private static bool DSFNotImpl;

	public static Color ContrastControlDark => SystemColors.ControlDark;

	private ControlPaint()
	{
	}

	internal static void Color2HBS(Color color, out int h, out int l, out int s)
	{
		int r = color.R;
		int g = color.G;
		int b = color.B;
		int num = Math.Max(Math.Max(r, g), b);
		int num2 = Math.Min(Math.Min(r, g), b);
		l = ((num + num2) * HLSMax + RGBMax) / (2 * RGBMax);
		if (num == num2)
		{
			h = 0;
			s = 0;
			return;
		}
		if (l <= HLSMax / 2)
		{
			s = ((num - num2) * HLSMax + (num + num2) / 2) / (num + num2);
		}
		else
		{
			s = ((num - num2) * HLSMax + (2 * RGBMax - num - num2) / 2) / (2 * RGBMax - num - num2);
		}
		int num3 = ((num - r) * (HLSMax / 6) + (num - num2) / 2) / (num - num2);
		int num4 = ((num - g) * (HLSMax / 6) + (num - num2) / 2) / (num - num2);
		int num5 = ((num - b) * (HLSMax / 6) + (num - num2) / 2) / (num - num2);
		if (r == num)
		{
			h = num5 - num4;
		}
		else if (g == num)
		{
			h = HLSMax / 3 + num3 - num5;
		}
		else
		{
			h = 2 * HLSMax / 3 + num4 - num3;
		}
		if (h < 0)
		{
			h += HLSMax;
		}
		if (h > HLSMax)
		{
			h -= HLSMax;
		}
	}

	private static int HueToRGB(int n1, int n2, int hue)
	{
		if (hue < 0)
		{
			hue += HLSMax;
		}
		if (hue > HLSMax)
		{
			hue -= HLSMax;
		}
		if (hue < HLSMax / 6)
		{
			return n1 + ((n2 - n1) * hue + HLSMax / 12) / (HLSMax / 6);
		}
		if (hue < HLSMax / 2)
		{
			return n2;
		}
		if (hue < HLSMax * 2 / 3)
		{
			return n1 + ((n2 - n1) * (HLSMax * 2 / 3 - hue) + HLSMax / 12) / (HLSMax / 6);
		}
		return n1;
	}

	internal static Color HBS2Color(int hue, int lum, int sat)
	{
		int red;
		int green;
		int blue;
		if (sat == 0)
		{
			red = (green = (blue = lum * RGBMax / HLSMax));
		}
		else
		{
			int num = ((lum > HLSMax / 2) ? (sat + lum - (sat * lum + HLSMax / 2) / HLSMax) : ((lum * (HLSMax + sat) + HLSMax / 2) / HLSMax));
			int n = 2 * lum - num;
			red = Math.Min(255, (HueToRGB(n, num, hue + HLSMax / 3) * RGBMax + HLSMax / 2) / HLSMax);
			green = Math.Min(255, (HueToRGB(n, num, hue) * RGBMax + HLSMax / 2) / HLSMax);
			blue = Math.Min(255, (HueToRGB(n, num, hue - HLSMax / 3) * RGBMax + HLSMax / 2) / HLSMax);
		}
		return Color.FromArgb(red, green, blue);
	}

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	public static IntPtr CreateHBitmap16Bit(Bitmap bitmap, Color background)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	public static IntPtr CreateHBitmapColorMask(Bitmap bitmap, IntPtr monochromeMask)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("Not implemented, will throw NotImplementedException")]
	public static IntPtr CreateHBitmapTransparencyMask(Bitmap bitmap)
	{
		throw new NotImplementedException();
	}

	public static Color Light(Color baseColor)
	{
		return Light(baseColor, 0.5f);
	}

	public static Color Light(Color baseColor, float percOfLightLight)
	{
		if (baseColor.ToArgb() == ThemeEngine.Current.ColorControl.ToArgb())
		{
			if (percOfLightLight <= 0f)
			{
				return ThemeEngine.Current.ColorControlLight;
			}
			if (percOfLightLight == 1f)
			{
				return ThemeEngine.Current.ColorControlLightLight;
			}
			int num = ThemeEngine.Current.ColorControlLightLight.R - ThemeEngine.Current.ColorControlLight.R;
			int num2 = ThemeEngine.Current.ColorControlLightLight.G - ThemeEngine.Current.ColorControlLight.G;
			int num3 = ThemeEngine.Current.ColorControlLightLight.B - ThemeEngine.Current.ColorControlLight.B;
			return Color.FromArgb(ThemeEngine.Current.ColorControlLight.A, (int)((float)(int)ThemeEngine.Current.ColorControlLight.R + (float)num * percOfLightLight), (int)((float)(int)ThemeEngine.Current.ColorControlLight.G + (float)num2 * percOfLightLight), (int)((float)(int)ThemeEngine.Current.ColorControlLight.B + (float)num3 * percOfLightLight));
		}
		Color2HBS(baseColor, out var h, out var l, out var s);
		int lum = Math.Min(255, l + (int)((float)(255 - l) * 0.5f * percOfLightLight));
		return HBS2Color(h, lum, s);
	}

	public static Color LightLight(Color baseColor)
	{
		return Light(baseColor, 1f);
	}

	public static Color Dark(Color baseColor)
	{
		return Dark(baseColor, 0.5f);
	}

	public static Color Dark(Color baseColor, float percOfDarkDark)
	{
		if (baseColor.ToArgb() == ThemeEngine.Current.ColorControl.ToArgb())
		{
			if (percOfDarkDark <= 0f)
			{
				return ThemeEngine.Current.ColorControlDark;
			}
			if (percOfDarkDark == 1f)
			{
				return ThemeEngine.Current.ColorControlDarkDark;
			}
			int num = ThemeEngine.Current.ColorControlDarkDark.R - ThemeEngine.Current.ColorControlDark.R;
			int num2 = ThemeEngine.Current.ColorControlDarkDark.G - ThemeEngine.Current.ColorControlDark.G;
			int num3 = ThemeEngine.Current.ColorControlDarkDark.B - ThemeEngine.Current.ColorControlDark.B;
			return Color.FromArgb(ThemeEngine.Current.ColorControlDark.A, (int)((float)(int)ThemeEngine.Current.ColorControlDark.R + (float)num * percOfDarkDark), (int)((float)(int)ThemeEngine.Current.ColorControlDark.G + (float)num2 * percOfDarkDark), (int)((float)(int)ThemeEngine.Current.ColorControlDark.B + (float)num3 * percOfDarkDark));
		}
		Color2HBS(baseColor, out var h, out var l, out var s);
		int num4 = Math.Max(0, l - (int)((float)l * 0.333f));
		int lum = Math.Max(0, num4 - (int)((float)num4 * percOfDarkDark));
		return HBS2Color(h, lum, s);
	}

	public static Color DarkDark(Color baseColor)
	{
		return Dark(baseColor, 1f);
	}

	public static void DrawBorder(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
	{
		int num = 1;
		int num2 = 1;
		if (style == ButtonBorderStyle.Inset)
		{
			num = 2;
		}
		if (style == ButtonBorderStyle.Outset)
		{
			num2 = 2;
			num = 2;
		}
		DrawBorder(graphics, bounds, color, num, style, color, num, style, color, num2, style, color, num2, style);
	}

	internal static void DrawBorder(Graphics graphics, RectangleF bounds, Color color, ButtonBorderStyle style)
	{
		int num = 1;
		int num2 = 1;
		if (style == ButtonBorderStyle.Inset)
		{
			num = 2;
		}
		if (style == ButtonBorderStyle.Outset)
		{
			num2 = 2;
			num = 2;
		}
		ThemeEngine.Current.CPDrawBorder(graphics, bounds, color, num, style, color, num, style, color, num2, style, color, num2, style);
	}

	public static void DrawBorder(Graphics graphics, Rectangle bounds, Color leftColor, int leftWidth, ButtonBorderStyle leftStyle, Color topColor, int topWidth, ButtonBorderStyle topStyle, Color rightColor, int rightWidth, ButtonBorderStyle rightStyle, Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle)
	{
		ThemeEngine.Current.CPDrawBorder(graphics, bounds, leftColor, leftWidth, leftStyle, topColor, topWidth, topStyle, rightColor, rightWidth, rightStyle, bottomColor, bottomWidth, bottomStyle);
	}

	public static void DrawBorder3D(Graphics graphics, Rectangle rectangle)
	{
		DrawBorder3D(graphics, rectangle, Border3DStyle.Etched, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
	}

	public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style)
	{
		DrawBorder3D(graphics, rectangle, style, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
	}

	public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height)
	{
		DrawBorder3D(graphics, new Rectangle(x, y, width, height), Border3DStyle.Etched, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
	}

	public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style)
	{
		DrawBorder3D(graphics, new Rectangle(x, y, width, height), style, Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);
	}

	public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style, Border3DSide sides)
	{
		DrawBorder3D(graphics, new Rectangle(x, y, width, height), style, sides);
	}

	public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style, Border3DSide sides)
	{
		ThemeEngine.Current.CPDrawBorder3D(graphics, rectangle, style, sides);
	}

	public static void DrawButton(Graphics graphics, int x, int y, int width, int height, ButtonState state)
	{
		DrawButton(graphics, new Rectangle(x, y, width, height), state);
	}

	public static void DrawButton(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		ThemeEngine.Current.CPDrawButton(graphics, rectangle, state);
	}

	public static void DrawCaptionButton(Graphics graphics, int x, int y, int width, int height, CaptionButton button, ButtonState state)
	{
		DrawCaptionButton(graphics, new Rectangle(x, y, width, height), button, state);
	}

	public static void DrawCaptionButton(Graphics graphics, Rectangle rectangle, CaptionButton button, ButtonState state)
	{
		ThemeEngine.Current.CPDrawCaptionButton(graphics, rectangle, button, state);
	}

	public static void DrawCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState state)
	{
		DrawCheckBox(graphics, new Rectangle(x, y, width, height), state);
	}

	public static void DrawCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		ThemeEngine.Current.CPDrawCheckBox(graphics, rectangle, state);
	}

	public static void DrawComboButton(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		ThemeEngine.Current.CPDrawComboButton(graphics, rectangle, state);
	}

	public static void DrawComboButton(Graphics graphics, int x, int y, int width, int height, ButtonState state)
	{
		DrawComboButton(graphics, new Rectangle(x, y, width, height), state);
	}

	public static void DrawContainerGrabHandle(Graphics graphics, Rectangle bounds)
	{
		ThemeEngine.Current.CPDrawContainerGrabHandle(graphics, bounds);
	}

	public static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle)
	{
		DrawFocusRectangle(graphics, rectangle, SystemColors.Control, SystemColors.ControlText);
	}

	public static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color foreColor, Color backColor)
	{
		ThemeEngine.Current.CPDrawFocusRectangle(graphics, rectangle, foreColor, backColor);
	}

	public static void DrawGrabHandle(Graphics graphics, Rectangle rectangle, bool primary, bool enabled)
	{
		ThemeEngine.Current.CPDrawGrabHandle(graphics, rectangle, primary, enabled);
	}

	public static void DrawGrid(Graphics graphics, Rectangle area, Size pixelsBetweenDots, Color backColor)
	{
		ThemeEngine.Current.CPDrawGrid(graphics, area, pixelsBetweenDots, backColor);
	}

	public static void DrawImageDisabled(Graphics graphics, Image image, int x, int y, Color background)
	{
		ThemeEngine.Current.CPDrawImageDisabled(graphics, image, x, y, background);
	}

	public static void DrawLockedFrame(Graphics graphics, Rectangle rectangle, bool primary)
	{
		ThemeEngine.Current.CPDrawLockedFrame(graphics, rectangle, primary);
	}

	public static void DrawMenuGlyph(Graphics graphics, Rectangle rectangle, MenuGlyph glyph)
	{
		ThemeEngine.Current.CPDrawMenuGlyph(graphics, rectangle, glyph, ThemeEngine.Current.ColorMenuText, Color.Empty);
	}

	public static void DrawMenuGlyph(Graphics graphics, Rectangle rectangle, MenuGlyph glyph, Color foreColor, Color backColor)
	{
		ThemeEngine.Current.CPDrawMenuGlyph(graphics, rectangle, glyph, foreColor, backColor);
	}

	public static void DrawMenuGlyph(Graphics graphics, int x, int y, int width, int height, MenuGlyph glyph)
	{
		DrawMenuGlyph(graphics, new Rectangle(x, y, width, height), glyph);
	}

	public static void DrawMenuGlyph(Graphics graphics, int x, int y, int width, int height, MenuGlyph glyph, Color foreColor, Color backColor)
	{
		DrawMenuGlyph(graphics, new Rectangle(x, y, width, height), glyph, foreColor, backColor);
	}

	public static void DrawMixedCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		ThemeEngine.Current.CPDrawMixedCheckBox(graphics, rectangle, state);
	}

	public static void DrawMixedCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState state)
	{
		DrawMixedCheckBox(graphics, new Rectangle(x, y, width, height), state);
	}

	public static void DrawRadioButton(Graphics graphics, int x, int y, int width, int height, ButtonState state)
	{
		DrawRadioButton(graphics, new Rectangle(x, y, width, height), state);
	}

	public static void DrawRadioButton(Graphics graphics, Rectangle rectangle, ButtonState state)
	{
		ThemeEngine.Current.CPDrawRadioButton(graphics, rectangle, state);
	}

	public static void DrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style)
	{
		XplatUI.DrawReversibleFrame(rectangle, backColor, style);
	}

	public static void DrawReversibleLine(Point start, Point end, Color backColor)
	{
		XplatUI.DrawReversibleLine(start, end, backColor);
	}

	public static void FillReversibleRectangle(Rectangle rectangle, Color backColor)
	{
		XplatUI.FillReversibleRectangle(rectangle, backColor);
	}

	public static void DrawScrollButton(Graphics graphics, int x, int y, int width, int height, ScrollButton button, ButtonState state)
	{
		ThemeEngine.Current.CPDrawScrollButton(graphics, new Rectangle(x, y, width, height), button, state);
	}

	public static void DrawScrollButton(Graphics graphics, Rectangle rectangle, ScrollButton button, ButtonState state)
	{
		ThemeEngine.Current.CPDrawScrollButton(graphics, rectangle, button, state);
	}

	public static void DrawSelectionFrame(Graphics graphics, bool active, Rectangle outsideRect, Rectangle insideRect, Color backColor)
	{
		if (!DSFNotImpl)
		{
			DSFNotImpl = true;
			Console.WriteLine("NOT IMPLEMENTED: DrawSelectionFrame(Graphics graphics, bool active, Rectangle outsideRect, Rectangle insideRect, Color backColor)");
		}
	}

	public static void DrawSizeGrip(Graphics graphics, Color backColor, Rectangle bounds)
	{
		ThemeEngine.Current.CPDrawSizeGrip(graphics, backColor, bounds);
	}

	public static void DrawSizeGrip(Graphics graphics, Color backColor, int x, int y, int width, int height)
	{
		DrawSizeGrip(graphics, backColor, new Rectangle(x, y, width, height));
	}

	public static void DrawStringDisabled(Graphics graphics, string s, Font font, Color color, RectangleF layoutRectangle, StringFormat format)
	{
		ThemeEngine.Current.CPDrawStringDisabled(graphics, s, font, color, layoutRectangle, format);
	}

	public static void DrawStringDisabled(IDeviceContext dc, string s, Font font, Color color, Rectangle layoutRectangle, TextFormatFlags format)
	{
		ThemeEngine.Current.CPDrawStringDisabled(dc, s, font, color, layoutRectangle, format);
	}

	public static void DrawVisualStyleBorder(Graphics graphics, Rectangle bounds)
	{
		ThemeEngine.Current.CPDrawVisualStyleBorder(graphics, bounds);
	}
}
