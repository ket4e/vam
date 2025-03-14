using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

public sealed class TextRenderer
{
	private TextRenderer()
	{
	}

	public static void DrawText(IDeviceContext dc, string text, Font font, Point pt, Color foreColor)
	{
		DrawTextInternal(dc, text, font, pt, foreColor, Color.Transparent, TextFormatFlags.Left, useDrawString: false);
	}

	public static void DrawText(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor)
	{
		DrawTextInternal(dc, text, font, bounds, foreColor, Color.Transparent, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, useDrawString: false);
	}

	public static void DrawText(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, Color backColor)
	{
		DrawTextInternal(dc, text, font, pt, foreColor, backColor, TextFormatFlags.Left, useDrawString: false);
	}

	public static void DrawText(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, TextFormatFlags flags)
	{
		DrawTextInternal(dc, text, font, pt, foreColor, Color.Transparent, flags, useDrawString: false);
	}

	public static void DrawText(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, Color backColor)
	{
		DrawTextInternal(dc, text, font, bounds, foreColor, backColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, useDrawString: false);
	}

	public static void DrawText(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, TextFormatFlags flags)
	{
		DrawTextInternal(dc, text, font, bounds, foreColor, Color.Transparent, flags, useDrawString: false);
	}

	public static void DrawText(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, Color backColor, TextFormatFlags flags)
	{
		DrawTextInternal(dc, text, font, pt, foreColor, backColor, flags, useDrawString: false);
	}

	public static void DrawText(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, Color backColor, TextFormatFlags flags)
	{
		DrawTextInternal(dc, text, font, bounds, foreColor, backColor, flags, useDrawString: false);
	}

	public static Size MeasureText(string text, Font font)
	{
		return MeasureTextInternal(Hwnd.GraphicsContext, text, font, Size.Empty, TextFormatFlags.Left, useMeasureString: false);
	}

	public static Size MeasureText(IDeviceContext dc, string text, Font font)
	{
		return MeasureTextInternal(dc, text, font, Size.Empty, TextFormatFlags.Left, useMeasureString: false);
	}

	public static Size MeasureText(string text, Font font, Size proposedSize)
	{
		return MeasureTextInternal(Hwnd.GraphicsContext, text, font, proposedSize, TextFormatFlags.Left, useMeasureString: false);
	}

	public static Size MeasureText(IDeviceContext dc, string text, Font font, Size proposedSize)
	{
		return MeasureTextInternal(dc, text, font, proposedSize, TextFormatFlags.Left, useMeasureString: false);
	}

	public static Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags flags)
	{
		return MeasureTextInternal(Hwnd.GraphicsContext, text, font, proposedSize, flags, useMeasureString: false);
	}

	public static Size MeasureText(IDeviceContext dc, string text, Font font, Size proposedSize, TextFormatFlags flags)
	{
		return MeasureTextInternal(dc, text, font, proposedSize, flags, useMeasureString: false);
	}

	internal static void DrawTextInternal(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, Color backColor, TextFormatFlags flags, bool useDrawString)
	{
		if (dc == null)
		{
			throw new ArgumentNullException("dc");
		}
		if (text == null || text.Length == 0)
		{
			return;
		}
		if (!useDrawString && !XplatUI.RunningOnUnix)
		{
			if ((flags & TextFormatFlags.VerticalCenter) == TextFormatFlags.VerticalCenter || (flags & TextFormatFlags.Bottom) == TextFormatFlags.Bottom)
			{
				flags |= TextFormatFlags.SingleLine;
			}
			Rectangle rectangle = PadRectangle(bounds, flags);
			rectangle.Offset((int)(dc as Graphics).Transform.OffsetX, (int)(dc as Graphics).Transform.OffsetY);
			IntPtr intPtr = IntPtr.Zero;
			bool flag = false;
			if ((flags & TextFormatFlags.PreserveGraphicsClipping) == TextFormatFlags.PreserveGraphicsClipping)
			{
				Graphics graphics = (Graphics)dc;
				Region clip = graphics.Clip;
				if (!clip.IsInfinite(graphics))
				{
					IntPtr hrgn = clip.GetHrgn(graphics);
					intPtr = dc.GetHdc();
					SelectClipRgn(intPtr, hrgn);
					DeleteObject(hrgn);
					flag = true;
				}
			}
			if (intPtr == IntPtr.Zero)
			{
				intPtr = dc.GetHdc();
			}
			if (foreColor != Color.Empty)
			{
				SetTextColor(intPtr, ColorTranslator.ToWin32(foreColor));
			}
			if (backColor != Color.Transparent && backColor != Color.Empty)
			{
				SetBkMode(intPtr, 2);
				SetBkColor(intPtr, ColorTranslator.ToWin32(backColor));
			}
			else
			{
				SetBkMode(intPtr, 1);
			}
			XplatUIWin32.RECT lpRect = XplatUIWin32.RECT.FromRectangle(rectangle);
			if (font != null)
			{
				IntPtr hObject = SelectObject(intPtr, font.ToHfont());
				Win32DrawText(intPtr, text, text.Length, ref lpRect, (int)flags);
				hObject = SelectObject(intPtr, hObject);
				DeleteObject(hObject);
			}
			else
			{
				Win32DrawText(intPtr, text, text.Length, ref lpRect, (int)flags);
			}
			if (flag)
			{
				SelectClipRgn(intPtr, IntPtr.Zero);
			}
			dc.ReleaseHdc();
		}
		else
		{
			IntPtr zero = IntPtr.Zero;
			Graphics graphics2;
			if (dc is Graphics)
			{
				graphics2 = (Graphics)dc;
			}
			else
			{
				zero = dc.GetHdc();
				graphics2 = Graphics.FromHdc(zero);
			}
			StringFormat format = FlagsToStringFormat(flags);
			Rectangle rectangle2 = PadDrawStringRectangle(bounds, flags);
			graphics2.DrawString(text, font, ThemeEngine.Current.ResPool.GetSolidBrush(foreColor), rectangle2, format);
			if (!(dc is Graphics))
			{
				graphics2.Dispose();
				dc.ReleaseHdc();
			}
		}
	}

	internal static Size MeasureTextInternal(IDeviceContext dc, string text, Font font, Size proposedSize, TextFormatFlags flags, bool useMeasureString)
	{
		if (!useMeasureString && !XplatUI.RunningOnUnix)
		{
			flags |= (TextFormatFlags)1024;
			IntPtr hdc = dc.GetHdc();
			XplatUIWin32.RECT lpRect = XplatUIWin32.RECT.FromRectangle(new Rectangle(Point.Empty, proposedSize));
			if (font != null)
			{
				IntPtr hObject = SelectObject(hdc, font.ToHfont());
				Win32DrawText(hdc, text, text.Length, ref lpRect, (int)flags);
				hObject = SelectObject(hdc, hObject);
				DeleteObject(hObject);
			}
			else
			{
				Win32DrawText(hdc, text, text.Length, ref lpRect, (int)flags);
			}
			dc.ReleaseHdc();
			Size size = lpRect.ToRectangle().Size;
			if (size.Width > 0 && (flags & TextFormatFlags.NoPadding) == 0)
			{
				size.Width += 6;
				size.Width += size.Height / 8;
			}
			return size;
		}
		StringFormat format = FlagsToStringFormat(flags);
		Size result = ((!(dc is Graphics)) ? MeasureString(text, font, (proposedSize.Width != 0) ? proposedSize.Width : int.MaxValue, format).ToSize() : (dc as Graphics).MeasureString(text, font, (proposedSize.Width != 0) ? proposedSize.Width : int.MaxValue, format).ToSize());
		if (result.Width > 0 && (flags & TextFormatFlags.NoPadding) == 0)
		{
			result.Width += 9;
		}
		return result;
	}

	internal static void DrawTextInternal(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, bool useDrawString)
	{
		DrawTextInternal(dc, text, font, pt, foreColor, Color.Transparent, TextFormatFlags.Left, useDrawString);
	}

	internal static void DrawTextInternal(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, bool useDrawString)
	{
		DrawTextInternal(dc, text, font, bounds, foreColor, Color.Transparent, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, useDrawString);
	}

	internal static void DrawTextInternal(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, Color backColor, bool useDrawString)
	{
		DrawTextInternal(dc, text, font, pt, foreColor, backColor, TextFormatFlags.Left, useDrawString);
	}

	internal static void DrawTextInternal(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, TextFormatFlags flags, bool useDrawString)
	{
		DrawTextInternal(dc, text, font, pt, foreColor, Color.Transparent, flags, useDrawString);
	}

	internal static void DrawTextInternal(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, Color backColor, bool useDrawString)
	{
		DrawTextInternal(dc, text, font, bounds, foreColor, backColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, useDrawString);
	}

	internal static void DrawTextInternal(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, TextFormatFlags flags, bool useDrawString)
	{
		DrawTextInternal(dc, text, font, bounds, foreColor, Color.Transparent, flags, useDrawString);
	}

	internal static Size MeasureTextInternal(string text, Font font, bool useMeasureString)
	{
		return MeasureTextInternal(Hwnd.GraphicsContext, text, font, Size.Empty, TextFormatFlags.Left, useMeasureString);
	}

	internal static void DrawTextInternal(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, Color backColor, TextFormatFlags flags, bool useDrawString)
	{
		Size size = MeasureTextInternal(dc, text, font, useDrawString);
		DrawTextInternal(dc, text, font, new Rectangle(pt, size), foreColor, backColor, flags, useDrawString);
	}

	internal static Size MeasureTextInternal(IDeviceContext dc, string text, Font font, bool useMeasureString)
	{
		return MeasureTextInternal(dc, text, font, Size.Empty, TextFormatFlags.Left, useMeasureString);
	}

	internal static Size MeasureTextInternal(string text, Font font, Size proposedSize, bool useMeasureString)
	{
		return MeasureTextInternal(Hwnd.GraphicsContext, text, font, proposedSize, TextFormatFlags.Left, useMeasureString);
	}

	internal static Size MeasureTextInternal(IDeviceContext dc, string text, Font font, Size proposedSize, bool useMeasureString)
	{
		return MeasureTextInternal(dc, text, font, proposedSize, TextFormatFlags.Left, useMeasureString);
	}

	internal static Size MeasureTextInternal(string text, Font font, Size proposedSize, TextFormatFlags flags, bool useMeasureString)
	{
		return MeasureTextInternal(Hwnd.GraphicsContext, text, font, proposedSize, flags, useMeasureString);
	}

	internal static SizeF MeasureString(string text, Font font)
	{
		return Hwnd.GraphicsContext.MeasureString(text, font);
	}

	internal static SizeF MeasureString(string text, Font font, int width)
	{
		return Hwnd.GraphicsContext.MeasureString(text, font, width);
	}

	internal static SizeF MeasureString(string text, Font font, SizeF layoutArea)
	{
		return Hwnd.GraphicsContext.MeasureString(text, font, layoutArea);
	}

	internal static SizeF MeasureString(string text, Font font, int width, StringFormat format)
	{
		return Hwnd.GraphicsContext.MeasureString(text, font, width, format);
	}

	internal static SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat)
	{
		return Hwnd.GraphicsContext.MeasureString(text, font, origin, stringFormat);
	}

	internal static SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
	{
		return Hwnd.GraphicsContext.MeasureString(text, font, layoutArea, stringFormat);
	}

	internal static SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
	{
		return Hwnd.GraphicsContext.MeasureString(text, font, layoutArea, stringFormat, out charactersFitted, out linesFilled);
	}

	internal static Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat)
	{
		return Hwnd.GraphicsContext.MeasureCharacterRanges(text, font, layoutRect, stringFormat);
	}

	internal static SizeF GetDpi()
	{
		return new SizeF(Hwnd.GraphicsContext.DpiX, Hwnd.GraphicsContext.DpiY);
	}

	private static StringFormat FlagsToStringFormat(TextFormatFlags flags)
	{
		StringFormat stringFormat = new StringFormat();
		if ((flags & TextFormatFlags.HorizontalCenter) == TextFormatFlags.HorizontalCenter)
		{
			stringFormat.Alignment = StringAlignment.Center;
		}
		else if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right)
		{
			stringFormat.Alignment = StringAlignment.Far;
		}
		else
		{
			stringFormat.Alignment = StringAlignment.Near;
		}
		if ((flags & TextFormatFlags.Bottom) == TextFormatFlags.Bottom)
		{
			stringFormat.LineAlignment = StringAlignment.Far;
		}
		else if ((flags & TextFormatFlags.VerticalCenter) == TextFormatFlags.VerticalCenter)
		{
			stringFormat.LineAlignment = StringAlignment.Center;
		}
		else
		{
			stringFormat.LineAlignment = StringAlignment.Near;
		}
		if ((flags & TextFormatFlags.EndEllipsis) == TextFormatFlags.EndEllipsis)
		{
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
		}
		else if ((flags & TextFormatFlags.PathEllipsis) == TextFormatFlags.PathEllipsis)
		{
			stringFormat.Trimming = StringTrimming.EllipsisPath;
		}
		else if ((flags & TextFormatFlags.WordEllipsis) == TextFormatFlags.WordEllipsis)
		{
			stringFormat.Trimming = StringTrimming.EllipsisWord;
		}
		else
		{
			stringFormat.Trimming = StringTrimming.Character;
		}
		if ((flags & TextFormatFlags.NoPrefix) == TextFormatFlags.NoPrefix)
		{
			stringFormat.HotkeyPrefix = HotkeyPrefix.None;
		}
		else if ((flags & TextFormatFlags.HidePrefix) == TextFormatFlags.HidePrefix)
		{
			stringFormat.HotkeyPrefix = HotkeyPrefix.Hide;
		}
		else
		{
			stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
		}
		if ((flags & TextFormatFlags.NoPadding) == TextFormatFlags.NoPadding)
		{
			stringFormat.FormatFlags |= StringFormatFlags.FitBlackBox;
		}
		if ((flags & TextFormatFlags.SingleLine) == TextFormatFlags.SingleLine)
		{
			stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
		}
		else if ((flags & TextFormatFlags.TextBoxControl) == TextFormatFlags.TextBoxControl)
		{
			stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
		}
		if ((flags & TextFormatFlags.NoClipping) == TextFormatFlags.NoClipping)
		{
			stringFormat.FormatFlags |= StringFormatFlags.NoClip;
		}
		return stringFormat;
	}

	private static Rectangle PadRectangle(Rectangle r, TextFormatFlags flags)
	{
		if ((flags & TextFormatFlags.NoPadding) == 0 && (flags & TextFormatFlags.Right) == 0 && (flags & TextFormatFlags.HorizontalCenter) == 0)
		{
			r.X += 3;
			r.Width -= 3;
		}
		if ((flags & TextFormatFlags.NoPadding) == 0 && (flags & TextFormatFlags.Right) == TextFormatFlags.Right)
		{
			r.Width -= 4;
		}
		if ((flags & TextFormatFlags.LeftAndRightPadding) == TextFormatFlags.LeftAndRightPadding)
		{
			r.X += 2;
			r.Width -= 2;
		}
		if ((flags & TextFormatFlags.WordEllipsis) == TextFormatFlags.WordEllipsis || (flags & TextFormatFlags.EndEllipsis) == TextFormatFlags.EndEllipsis || (flags & TextFormatFlags.WordBreak) == TextFormatFlags.WordBreak)
		{
			r.Width -= 4;
		}
		if ((flags & TextFormatFlags.VerticalCenter) == TextFormatFlags.VerticalCenter)
		{
			r.Y++;
		}
		return r;
	}

	private static Rectangle PadDrawStringRectangle(Rectangle r, TextFormatFlags flags)
	{
		if ((flags & TextFormatFlags.NoPadding) == 0 && (flags & TextFormatFlags.Right) == 0 && (flags & TextFormatFlags.HorizontalCenter) == 0)
		{
			r.X++;
			r.Width--;
		}
		if ((flags & TextFormatFlags.NoPadding) == 0 && (flags & TextFormatFlags.Right) == TextFormatFlags.Right)
		{
			r.Width -= 4;
		}
		if ((flags & TextFormatFlags.NoPadding) == TextFormatFlags.NoPadding)
		{
			r.X -= 2;
		}
		if ((flags & TextFormatFlags.NoPadding) == 0 && (flags & TextFormatFlags.Bottom) == TextFormatFlags.Bottom)
		{
			r.Y++;
		}
		if ((flags & TextFormatFlags.LeftAndRightPadding) == TextFormatFlags.LeftAndRightPadding)
		{
			r.X += 2;
			r.Width -= 2;
		}
		if ((flags & TextFormatFlags.WordEllipsis) == TextFormatFlags.WordEllipsis || (flags & TextFormatFlags.EndEllipsis) == TextFormatFlags.EndEllipsis || (flags & TextFormatFlags.WordBreak) == TextFormatFlags.WordBreak)
		{
			r.Width -= 4;
		}
		if ((flags & TextFormatFlags.VerticalCenter) == TextFormatFlags.VerticalCenter)
		{
			r.Y++;
		}
		return r;
	}

	[DllImport("user32", CharSet = CharSet.Unicode, EntryPoint = "DrawText")]
	private static extern int Win32DrawText(IntPtr hdc, string lpStr, int nCount, ref XplatUIWin32.RECT lpRect, int wFormat);

	[DllImport("gdi32")]
	private static extern int SetTextColor(IntPtr hdc, int crColor);

	[DllImport("gdi32")]
	private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

	[DllImport("gdi32")]
	private static extern int SetBkColor(IntPtr hdc, int crColor);

	[DllImport("gdi32")]
	private static extern int SetBkMode(IntPtr hdc, int iBkMode);

	[DllImport("gdi32")]
	private static extern bool DeleteObject(IntPtr objectHandle);

	[DllImport("gdi32")]
	private static extern bool SelectClipRgn(IntPtr hdc, IntPtr hrgn);
}
