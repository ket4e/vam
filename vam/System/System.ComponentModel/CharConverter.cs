using System.Globalization;

namespace System.ComponentModel;

public class CharConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}
		return base.CanConvertFrom(context, sourceType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		string text = value as string;
		if (text != null)
		{
			if (text.Length > 1)
			{
				text = text.Trim();
			}
			if (text.Length > 1)
			{
				throw new FormatException($"String {text} is not a valid Char: it has to be less than or equal to one char long.");
			}
			if (text.Length == 0)
			{
				return '\0';
			}
			return text[0];
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string) && value != null && value is char c)
		{
			if (c == '\0')
			{
				return string.Empty;
			}
			return new string(c, 1);
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
