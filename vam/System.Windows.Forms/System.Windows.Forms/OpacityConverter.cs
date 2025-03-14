using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public class OpacityConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}
		return false;
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string)
		{
			string text = (string)value;
			if (text.EndsWith("%"))
			{
				text = ((string)value).Substring(0, ((string)value).Length - 1);
			}
			return double.Parse(text, NumberStyles.Any, culture) / 100.0;
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			return (double)value * 100.0 + "%";
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
