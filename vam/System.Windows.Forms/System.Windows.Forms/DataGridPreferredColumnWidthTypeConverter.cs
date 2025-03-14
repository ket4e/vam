using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public class DataGridPreferredColumnWidthTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string) || sourceType == typeof(int))
		{
			return true;
		}
		return base.CanConvertFrom(context, sourceType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string)
		{
			if (((string)value).Equals("AutoColumnResize (-1)"))
			{
				return -1;
			}
			return int.Parse((string)value);
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string) && (int)value == -1)
		{
			return "AutoColumnResize (-1)";
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}
