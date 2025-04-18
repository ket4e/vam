using System.Globalization;

namespace System.ComponentModel;

public class StringConverter : TypeConverter
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
		if (value == null)
		{
			return string.Empty;
		}
		if (value is string)
		{
			return (string)value;
		}
		return base.ConvertFrom(context, culture, value);
	}
}
