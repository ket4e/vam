using System.Collections;
using System.Drawing;
using System.Drawing.Text;

namespace System.Windows.Forms;

internal class TextBoxTextRenderer
{
	private static Size max_size;

	private static bool use_textrenderer;

	private static StringFormat sf_nonprinting;

	private static StringFormat sf_printing;

	private static Hashtable measure_cache;

	static TextBoxTextRenderer()
	{
		int platform = (int)Environment.OSVersion.Platform;
		if (platform == 4 || platform == 128 || platform == 6)
		{
			use_textrenderer = false;
		}
		else
		{
			use_textrenderer = true;
		}
		max_size = new Size(32767, 32767);
		sf_nonprinting = new StringFormat(StringFormat.GenericTypographic);
		sf_nonprinting.Trimming = StringTrimming.None;
		sf_nonprinting.FormatFlags = StringFormatFlags.DisplayFormatControl;
		sf_nonprinting.HotkeyPrefix = HotkeyPrefix.None;
		sf_printing = StringFormat.GenericTypographic;
		sf_printing.HotkeyPrefix = HotkeyPrefix.None;
		measure_cache = new Hashtable();
	}

	public static void DrawText(Graphics g, string text, Font font, Color color, float x, float y, bool showNonPrint)
	{
		if (!use_textrenderer)
		{
			if (showNonPrint)
			{
				g.DrawString(text, font, ThemeEngine.Current.ResPool.GetSolidBrush(color), x, y, sf_nonprinting);
			}
			else
			{
				g.DrawString(text, font, ThemeEngine.Current.ResPool.GetSolidBrush(color), x, y, sf_printing);
			}
		}
		else if (showNonPrint)
		{
			TextRenderer.DrawTextInternal(g, text, font, new Rectangle(new Point((int)x, (int)y), max_size), color, TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding, useDrawString: false);
		}
		else
		{
			TextRenderer.DrawTextInternal(g, text, font, new Rectangle(new Point((int)x, (int)y), max_size), color, TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding, useDrawString: false);
		}
	}

	public static SizeF MeasureText(Graphics g, string text, Font font)
	{
		if (text.Length == 1)
		{
			string key = font.GetHashCode() + "|" + text;
			if (measure_cache.ContainsKey(key))
			{
				return (SizeF)measure_cache[key];
			}
			SizeF sizeF = (use_textrenderer ? ((SizeF)TextRenderer.MeasureTextInternal(g, text, font, Size.Empty, TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding, useMeasureString: false)) : g.MeasureString(text, font, 10000, sf_nonprinting));
			measure_cache[key] = sizeF;
			return sizeF;
		}
		if (!use_textrenderer)
		{
			return g.MeasureString(text, font, 10000, sf_nonprinting);
		}
		return TextRenderer.MeasureTextInternal(g, text, font, Size.Empty, TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding, useMeasureString: false);
	}
}
