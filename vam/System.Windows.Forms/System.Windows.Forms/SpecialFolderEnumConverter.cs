using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

internal class SpecialFolderEnumConverter : TypeConverter
{
	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value == null || !(value is string))
		{
			return base.ConvertFrom(context, culture, value);
		}
		return Enum.Parse(typeof(Environment.SpecialFolder), (string)value, ignoreCase: true);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value == null || !(value is Environment.SpecialFolder) || destinationType != typeof(string))
		{
			return base.ConvertTo(context, culture, value, destinationType);
		}
		return ((Environment.SpecialFolder)(int)value).ToString();
	}
}
