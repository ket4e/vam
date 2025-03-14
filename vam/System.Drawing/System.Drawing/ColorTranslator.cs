using System.ComponentModel;
using System.Globalization;

namespace System.Drawing;

public sealed class ColorTranslator
{
	private ColorTranslator()
	{
	}

	public static Color FromHtml(string htmlColor)
	{
		if (htmlColor == null || htmlColor.Length == 0)
		{
			return Color.Empty;
		}
		switch (htmlColor.ToLowerInvariant())
		{
		case "buttonface":
		case "threedface":
			return SystemColors.Control;
		case "buttonhighlight":
		case "threedlightshadow":
			return SystemColors.ControlLightLight;
		case "buttonshadow":
			return SystemColors.ControlDark;
		case "captiontext":
			return SystemColors.ActiveCaptionText;
		case "threeddarkshadow":
			return SystemColors.ControlDarkDark;
		case "threedhighlight":
			return SystemColors.ControlLight;
		case "background":
			return SystemColors.Desktop;
		case "buttontext":
			return SystemColors.ControlText;
		case "infobackground":
			return SystemColors.Info;
		case "lightgrey":
			return Color.LightGray;
		default:
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
			return (Color)converter.ConvertFromString(htmlColor);
		}
		}
	}

	internal static Color FromBGR(int bgr)
	{
		Color color = Color.FromArgb(255, bgr & 0xFF, (bgr >> 8) & 0xFF, (bgr >> 16) & 0xFF);
		Color color2 = KnownColors.FindColorMatch(color);
		return (!color2.IsEmpty) ? color2 : color;
	}

	public static Color FromOle(int oleColor)
	{
		return FromBGR(oleColor);
	}

	public static Color FromWin32(int win32Color)
	{
		return FromBGR(win32Color);
	}

	public static string ToHtml(Color c)
	{
		if (c.IsEmpty)
		{
			return string.Empty;
		}
		if (c.IsSystemColor)
		{
			KnownColor knownColor = c.ToKnownColor();
			switch (knownColor)
			{
			case KnownColor.ActiveBorder:
			case KnownColor.ActiveCaption:
			case KnownColor.AppWorkspace:
			case KnownColor.GrayText:
			case KnownColor.Highlight:
			case KnownColor.HighlightText:
			case KnownColor.InactiveBorder:
			case KnownColor.InactiveCaption:
			case KnownColor.InactiveCaptionText:
			case KnownColor.InfoText:
			case KnownColor.Menu:
			case KnownColor.MenuText:
			case KnownColor.ScrollBar:
			case KnownColor.Window:
			case KnownColor.WindowFrame:
			case KnownColor.WindowText:
				return KnownColors.GetName(knownColor).ToLower(CultureInfo.InvariantCulture);
			case KnownColor.ActiveCaptionText:
				return "captiontext";
			case KnownColor.Control:
				return "buttonface";
			case KnownColor.ControlDark:
				return "buttonshadow";
			case KnownColor.ControlDarkDark:
				return "threeddarkshadow";
			case KnownColor.ControlLight:
				return "buttonface";
			case KnownColor.ControlLightLight:
				return "buttonhighlight";
			case KnownColor.ControlText:
				return "buttontext";
			case KnownColor.Desktop:
				return "background";
			case KnownColor.HotTrack:
				return "highlight";
			case KnownColor.Info:
				return "infobackground";
			default:
				return string.Empty;
			}
		}
		if (c.IsNamedColor)
		{
			if (c == Color.LightGray)
			{
				return "LightGrey";
			}
			return c.Name;
		}
		return FormatHtml(c.R, c.G, c.B);
	}

	private static char GetHexNumber(int b)
	{
		return (char)((b <= 9) ? (48 + b) : (55 + b));
	}

	private static string FormatHtml(int r, int g, int b)
	{
		return new string(new char[7]
		{
			'#',
			GetHexNumber((r >> 4) & 0xF),
			GetHexNumber(r & 0xF),
			GetHexNumber((g >> 4) & 0xF),
			GetHexNumber(g & 0xF),
			GetHexNumber((b >> 4) & 0xF),
			GetHexNumber(b & 0xF)
		});
	}

	public static int ToOle(Color c)
	{
		return (c.B << 16) | (c.G << 8) | c.R;
	}

	public static int ToWin32(Color c)
	{
		return (c.B << 16) | (c.G << 8) | c.R;
	}
}
