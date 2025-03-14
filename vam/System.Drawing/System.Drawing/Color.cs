using System.ComponentModel;
using System.Drawing.Design;

namespace System.Drawing;

[Serializable]
[TypeConverter(typeof(ColorConverter))]
[Editor("System.Drawing.Design.ColorEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
public struct Color
{
	[Flags]
	internal enum ColorType : short
	{
		Empty = 0,
		Known = 1,
		ARGB = 2,
		Named = 4,
		System = 8
	}

	private long value;

	internal short state;

	internal short knownColor;

	internal string name;

	public static readonly Color Empty;

	public string Name
	{
		get
		{
			if (name == null)
			{
				if (IsNamedColor)
				{
					name = KnownColors.GetName(knownColor);
				}
				else
				{
					name = $"{ToArgb():x}";
				}
			}
			return name;
		}
	}

	public bool IsKnownColor => (state & 1) != 0;

	public bool IsSystemColor => (state & 8) != 0;

	public bool IsNamedColor => (state & 5) != 0;

	internal long Value
	{
		get
		{
			if (value == 0L && IsKnownColor)
			{
				value = KnownColors.FromKnownColor((KnownColor)knownColor).ToArgb() & 0xFFFFFFFFu;
			}
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public bool IsEmpty => state == 0;

	public byte A => (byte)(Value >> 24);

	public byte R => (byte)(Value >> 16);

	public byte G => (byte)(Value >> 8);

	public byte B => (byte)Value;

	public static Color Transparent => KnownColors.FromKnownColor(KnownColor.Transparent);

	public static Color AliceBlue => KnownColors.FromKnownColor(KnownColor.AliceBlue);

	public static Color AntiqueWhite => KnownColors.FromKnownColor(KnownColor.AntiqueWhite);

	public static Color Aqua => KnownColors.FromKnownColor(KnownColor.Aqua);

	public static Color Aquamarine => KnownColors.FromKnownColor(KnownColor.Aquamarine);

	public static Color Azure => KnownColors.FromKnownColor(KnownColor.Azure);

	public static Color Beige => KnownColors.FromKnownColor(KnownColor.Beige);

	public static Color Bisque => KnownColors.FromKnownColor(KnownColor.Bisque);

	public static Color Black => KnownColors.FromKnownColor(KnownColor.Black);

	public static Color BlanchedAlmond => KnownColors.FromKnownColor(KnownColor.BlanchedAlmond);

	public static Color Blue => KnownColors.FromKnownColor(KnownColor.Blue);

	public static Color BlueViolet => KnownColors.FromKnownColor(KnownColor.BlueViolet);

	public static Color Brown => KnownColors.FromKnownColor(KnownColor.Brown);

	public static Color BurlyWood => KnownColors.FromKnownColor(KnownColor.BurlyWood);

	public static Color CadetBlue => KnownColors.FromKnownColor(KnownColor.CadetBlue);

	public static Color Chartreuse => KnownColors.FromKnownColor(KnownColor.Chartreuse);

	public static Color Chocolate => KnownColors.FromKnownColor(KnownColor.Chocolate);

	public static Color Coral => KnownColors.FromKnownColor(KnownColor.Coral);

	public static Color CornflowerBlue => KnownColors.FromKnownColor(KnownColor.CornflowerBlue);

	public static Color Cornsilk => KnownColors.FromKnownColor(KnownColor.Cornsilk);

	public static Color Crimson => KnownColors.FromKnownColor(KnownColor.Crimson);

	public static Color Cyan => KnownColors.FromKnownColor(KnownColor.Cyan);

	public static Color DarkBlue => KnownColors.FromKnownColor(KnownColor.DarkBlue);

	public static Color DarkCyan => KnownColors.FromKnownColor(KnownColor.DarkCyan);

	public static Color DarkGoldenrod => KnownColors.FromKnownColor(KnownColor.DarkGoldenrod);

	public static Color DarkGray => KnownColors.FromKnownColor(KnownColor.DarkGray);

	public static Color DarkGreen => KnownColors.FromKnownColor(KnownColor.DarkGreen);

	public static Color DarkKhaki => KnownColors.FromKnownColor(KnownColor.DarkKhaki);

	public static Color DarkMagenta => KnownColors.FromKnownColor(KnownColor.DarkMagenta);

	public static Color DarkOliveGreen => KnownColors.FromKnownColor(KnownColor.DarkOliveGreen);

	public static Color DarkOrange => KnownColors.FromKnownColor(KnownColor.DarkOrange);

	public static Color DarkOrchid => KnownColors.FromKnownColor(KnownColor.DarkOrchid);

	public static Color DarkRed => KnownColors.FromKnownColor(KnownColor.DarkRed);

	public static Color DarkSalmon => KnownColors.FromKnownColor(KnownColor.DarkSalmon);

	public static Color DarkSeaGreen => KnownColors.FromKnownColor(KnownColor.DarkSeaGreen);

	public static Color DarkSlateBlue => KnownColors.FromKnownColor(KnownColor.DarkSlateBlue);

	public static Color DarkSlateGray => KnownColors.FromKnownColor(KnownColor.DarkSlateGray);

	public static Color DarkTurquoise => KnownColors.FromKnownColor(KnownColor.DarkTurquoise);

	public static Color DarkViolet => KnownColors.FromKnownColor(KnownColor.DarkViolet);

	public static Color DeepPink => KnownColors.FromKnownColor(KnownColor.DeepPink);

	public static Color DeepSkyBlue => KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);

	public static Color DimGray => KnownColors.FromKnownColor(KnownColor.DimGray);

	public static Color DodgerBlue => KnownColors.FromKnownColor(KnownColor.DodgerBlue);

	public static Color Firebrick => KnownColors.FromKnownColor(KnownColor.Firebrick);

	public static Color FloralWhite => KnownColors.FromKnownColor(KnownColor.FloralWhite);

	public static Color ForestGreen => KnownColors.FromKnownColor(KnownColor.ForestGreen);

	public static Color Fuchsia => KnownColors.FromKnownColor(KnownColor.Fuchsia);

	public static Color Gainsboro => KnownColors.FromKnownColor(KnownColor.Gainsboro);

	public static Color GhostWhite => KnownColors.FromKnownColor(KnownColor.GhostWhite);

	public static Color Gold => KnownColors.FromKnownColor(KnownColor.Gold);

	public static Color Goldenrod => KnownColors.FromKnownColor(KnownColor.Goldenrod);

	public static Color Gray => KnownColors.FromKnownColor(KnownColor.Gray);

	public static Color Green => KnownColors.FromKnownColor(KnownColor.Green);

	public static Color GreenYellow => KnownColors.FromKnownColor(KnownColor.GreenYellow);

	public static Color Honeydew => KnownColors.FromKnownColor(KnownColor.Honeydew);

	public static Color HotPink => KnownColors.FromKnownColor(KnownColor.HotPink);

	public static Color IndianRed => KnownColors.FromKnownColor(KnownColor.IndianRed);

	public static Color Indigo => KnownColors.FromKnownColor(KnownColor.Indigo);

	public static Color Ivory => KnownColors.FromKnownColor(KnownColor.Ivory);

	public static Color Khaki => KnownColors.FromKnownColor(KnownColor.Khaki);

	public static Color Lavender => KnownColors.FromKnownColor(KnownColor.Lavender);

	public static Color LavenderBlush => KnownColors.FromKnownColor(KnownColor.LavenderBlush);

	public static Color LawnGreen => KnownColors.FromKnownColor(KnownColor.LawnGreen);

	public static Color LemonChiffon => KnownColors.FromKnownColor(KnownColor.LemonChiffon);

	public static Color LightBlue => KnownColors.FromKnownColor(KnownColor.LightBlue);

	public static Color LightCoral => KnownColors.FromKnownColor(KnownColor.LightCoral);

	public static Color LightCyan => KnownColors.FromKnownColor(KnownColor.LightCyan);

	public static Color LightGoldenrodYellow => KnownColors.FromKnownColor(KnownColor.LightGoldenrodYellow);

	public static Color LightGreen => KnownColors.FromKnownColor(KnownColor.LightGreen);

	public static Color LightGray => KnownColors.FromKnownColor(KnownColor.LightGray);

	public static Color LightPink => KnownColors.FromKnownColor(KnownColor.LightPink);

	public static Color LightSalmon => KnownColors.FromKnownColor(KnownColor.LightSalmon);

	public static Color LightSeaGreen => KnownColors.FromKnownColor(KnownColor.LightSeaGreen);

	public static Color LightSkyBlue => KnownColors.FromKnownColor(KnownColor.LightSkyBlue);

	public static Color LightSlateGray => KnownColors.FromKnownColor(KnownColor.LightSlateGray);

	public static Color LightSteelBlue => KnownColors.FromKnownColor(KnownColor.LightSteelBlue);

	public static Color LightYellow => KnownColors.FromKnownColor(KnownColor.LightYellow);

	public static Color Lime => KnownColors.FromKnownColor(KnownColor.Lime);

	public static Color LimeGreen => KnownColors.FromKnownColor(KnownColor.LimeGreen);

	public static Color Linen => KnownColors.FromKnownColor(KnownColor.Linen);

	public static Color Magenta => KnownColors.FromKnownColor(KnownColor.Magenta);

	public static Color Maroon => KnownColors.FromKnownColor(KnownColor.Maroon);

	public static Color MediumAquamarine => KnownColors.FromKnownColor(KnownColor.MediumAquamarine);

	public static Color MediumBlue => KnownColors.FromKnownColor(KnownColor.MediumBlue);

	public static Color MediumOrchid => KnownColors.FromKnownColor(KnownColor.MediumOrchid);

	public static Color MediumPurple => KnownColors.FromKnownColor(KnownColor.MediumPurple);

	public static Color MediumSeaGreen => KnownColors.FromKnownColor(KnownColor.MediumSeaGreen);

	public static Color MediumSlateBlue => KnownColors.FromKnownColor(KnownColor.MediumSlateBlue);

	public static Color MediumSpringGreen => KnownColors.FromKnownColor(KnownColor.MediumSpringGreen);

	public static Color MediumTurquoise => KnownColors.FromKnownColor(KnownColor.MediumTurquoise);

	public static Color MediumVioletRed => KnownColors.FromKnownColor(KnownColor.MediumVioletRed);

	public static Color MidnightBlue => KnownColors.FromKnownColor(KnownColor.MidnightBlue);

	public static Color MintCream => KnownColors.FromKnownColor(KnownColor.MintCream);

	public static Color MistyRose => KnownColors.FromKnownColor(KnownColor.MistyRose);

	public static Color Moccasin => KnownColors.FromKnownColor(KnownColor.Moccasin);

	public static Color NavajoWhite => KnownColors.FromKnownColor(KnownColor.NavajoWhite);

	public static Color Navy => KnownColors.FromKnownColor(KnownColor.Navy);

	public static Color OldLace => KnownColors.FromKnownColor(KnownColor.OldLace);

	public static Color Olive => KnownColors.FromKnownColor(KnownColor.Olive);

	public static Color OliveDrab => KnownColors.FromKnownColor(KnownColor.OliveDrab);

	public static Color Orange => KnownColors.FromKnownColor(KnownColor.Orange);

	public static Color OrangeRed => KnownColors.FromKnownColor(KnownColor.OrangeRed);

	public static Color Orchid => KnownColors.FromKnownColor(KnownColor.Orchid);

	public static Color PaleGoldenrod => KnownColors.FromKnownColor(KnownColor.PaleGoldenrod);

	public static Color PaleGreen => KnownColors.FromKnownColor(KnownColor.PaleGreen);

	public static Color PaleTurquoise => KnownColors.FromKnownColor(KnownColor.PaleTurquoise);

	public static Color PaleVioletRed => KnownColors.FromKnownColor(KnownColor.PaleVioletRed);

	public static Color PapayaWhip => KnownColors.FromKnownColor(KnownColor.PapayaWhip);

	public static Color PeachPuff => KnownColors.FromKnownColor(KnownColor.PeachPuff);

	public static Color Peru => KnownColors.FromKnownColor(KnownColor.Peru);

	public static Color Pink => KnownColors.FromKnownColor(KnownColor.Pink);

	public static Color Plum => KnownColors.FromKnownColor(KnownColor.Plum);

	public static Color PowderBlue => KnownColors.FromKnownColor(KnownColor.PowderBlue);

	public static Color Purple => KnownColors.FromKnownColor(KnownColor.Purple);

	public static Color Red => KnownColors.FromKnownColor(KnownColor.Red);

	public static Color RosyBrown => KnownColors.FromKnownColor(KnownColor.RosyBrown);

	public static Color RoyalBlue => KnownColors.FromKnownColor(KnownColor.RoyalBlue);

	public static Color SaddleBrown => KnownColors.FromKnownColor(KnownColor.SaddleBrown);

	public static Color Salmon => KnownColors.FromKnownColor(KnownColor.Salmon);

	public static Color SandyBrown => KnownColors.FromKnownColor(KnownColor.SandyBrown);

	public static Color SeaGreen => KnownColors.FromKnownColor(KnownColor.SeaGreen);

	public static Color SeaShell => KnownColors.FromKnownColor(KnownColor.SeaShell);

	public static Color Sienna => KnownColors.FromKnownColor(KnownColor.Sienna);

	public static Color Silver => KnownColors.FromKnownColor(KnownColor.Silver);

	public static Color SkyBlue => KnownColors.FromKnownColor(KnownColor.SkyBlue);

	public static Color SlateBlue => KnownColors.FromKnownColor(KnownColor.SlateBlue);

	public static Color SlateGray => KnownColors.FromKnownColor(KnownColor.SlateGray);

	public static Color Snow => KnownColors.FromKnownColor(KnownColor.Snow);

	public static Color SpringGreen => KnownColors.FromKnownColor(KnownColor.SpringGreen);

	public static Color SteelBlue => KnownColors.FromKnownColor(KnownColor.SteelBlue);

	public static Color Tan => KnownColors.FromKnownColor(KnownColor.Tan);

	public static Color Teal => KnownColors.FromKnownColor(KnownColor.Teal);

	public static Color Thistle => KnownColors.FromKnownColor(KnownColor.Thistle);

	public static Color Tomato => KnownColors.FromKnownColor(KnownColor.Tomato);

	public static Color Turquoise => KnownColors.FromKnownColor(KnownColor.Turquoise);

	public static Color Violet => KnownColors.FromKnownColor(KnownColor.Violet);

	public static Color Wheat => KnownColors.FromKnownColor(KnownColor.Wheat);

	public static Color White => KnownColors.FromKnownColor(KnownColor.White);

	public static Color WhiteSmoke => KnownColors.FromKnownColor(KnownColor.WhiteSmoke);

	public static Color Yellow => KnownColors.FromKnownColor(KnownColor.Yellow);

	public static Color YellowGreen => KnownColors.FromKnownColor(KnownColor.YellowGreen);

	public static Color FromArgb(int red, int green, int blue)
	{
		return FromArgb(255, red, green, blue);
	}

	public static Color FromArgb(int alpha, int red, int green, int blue)
	{
		CheckARGBValues(alpha, red, green, blue);
		Color result = default(Color);
		result.state = 2;
		result.Value = (alpha << 24) + (red << 16) + (green << 8) + blue;
		return result;
	}

	public int ToArgb()
	{
		return (int)Value;
	}

	public static Color FromArgb(int alpha, Color baseColor)
	{
		return FromArgb(alpha, baseColor.R, baseColor.G, baseColor.B);
	}

	public static Color FromArgb(int argb)
	{
		return FromArgb((argb >> 24) & 0xFF, (argb >> 16) & 0xFF, (argb >> 8) & 0xFF, argb & 0xFF);
	}

	public static Color FromKnownColor(KnownColor color)
	{
		return KnownColors.FromKnownColor(color);
	}

	public static Color FromName(string name)
	{
		try
		{
			KnownColor kc = (KnownColor)(int)Enum.Parse(typeof(KnownColor), name, ignoreCase: true);
			return KnownColors.FromKnownColor(kc);
		}
		catch
		{
			Color result = FromArgb(0, 0, 0, 0);
			result.name = name;
			result.state |= 4;
			return result;
		}
	}

	public float GetBrightness()
	{
		byte b = Math.Min(R, Math.Min(G, B));
		byte b2 = Math.Max(R, Math.Max(G, B));
		return (float)(b2 + b) / 510f;
	}

	public float GetSaturation()
	{
		byte b = Math.Min(R, Math.Min(G, B));
		byte b2 = Math.Max(R, Math.Max(G, B));
		if (b2 == b)
		{
			return 0f;
		}
		int num = b2 + b;
		if (num > 255)
		{
			num = 510 - num;
		}
		return (float)(b2 - b) / (float)num;
	}

	public float GetHue()
	{
		int r = R;
		int g = G;
		int b = B;
		byte b2 = (byte)Math.Min(r, Math.Min(g, b));
		byte b3 = (byte)Math.Max(r, Math.Max(g, b));
		if (b3 == b2)
		{
			return 0f;
		}
		float num = b3 - b2;
		float num2 = (float)(b3 - r) / num;
		float num3 = (float)(b3 - g) / num;
		float num4 = (float)(b3 - b) / num;
		float num5 = 0f;
		if (r == b3)
		{
			num5 = 60f * (6f + num4 - num3);
		}
		if (g == b3)
		{
			num5 = 60f * (2f + num2 - num4);
		}
		if (b == b3)
		{
			num5 = 60f * (4f + num3 - num2);
		}
		if (num5 > 360f)
		{
			num5 -= 360f;
		}
		return num5;
	}

	public KnownColor ToKnownColor()
	{
		return (KnownColor)knownColor;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Color color))
		{
			return false;
		}
		return this == color;
	}

	public override int GetHashCode()
	{
		int num = (int)(Value ^ (Value >> 32) ^ state ^ (knownColor >> 16));
		if (IsNamedColor)
		{
			num ^= Name.GetHashCode();
		}
		return num;
	}

	public override string ToString()
	{
		if (IsEmpty)
		{
			return "Color [Empty]";
		}
		if (IsNamedColor)
		{
			return "Color [" + Name + "]";
		}
		return $"Color [A={A}, R={R}, G={G}, B={B}]";
	}

	private static void CheckRGBValues(int red, int green, int blue)
	{
		if (red > 255 || red < 0)
		{
			throw CreateColorArgumentException(red, "red");
		}
		if (green > 255 || green < 0)
		{
			throw CreateColorArgumentException(green, "green");
		}
		if (blue > 255 || blue < 0)
		{
			throw CreateColorArgumentException(blue, "blue");
		}
	}

	private static ArgumentException CreateColorArgumentException(int value, string color)
	{
		return new ArgumentException(string.Format("'{0}' is not a valid value for '{1}'. '{1}' should be greater or equal to 0 and less than or equal to 255.", value, color));
	}

	private static void CheckARGBValues(int alpha, int red, int green, int blue)
	{
		if (alpha > 255 || alpha < 0)
		{
			throw CreateColorArgumentException(alpha, "alpha");
		}
		CheckRGBValues(red, green, blue);
	}

	public static bool operator ==(Color left, Color right)
	{
		if (left.Value != right.Value)
		{
			return false;
		}
		if (left.IsNamedColor != right.IsNamedColor)
		{
			return false;
		}
		if (left.IsSystemColor != right.IsSystemColor)
		{
			return false;
		}
		if (left.IsEmpty != right.IsEmpty)
		{
			return false;
		}
		if (left.IsNamedColor && left.Name != right.Name)
		{
			return false;
		}
		return true;
	}

	public static bool operator !=(Color left, Color right)
	{
		return !(left == right);
	}
}
