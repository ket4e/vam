using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public class ImageKeyConverter : StringConverter
{
	protected virtual bool IncludeNoneAsStandardValue => true;

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
		if (value != null && value is string)
		{
			return (string)value;
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value == null)
		{
			return "(none)";
		}
		if (destinationType == typeof(string))
		{
			if (value is string && (string)value == string.Empty)
			{
				return "(none)";
			}
			return value.ToString();
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		string[] values = new string[1] { string.Empty };
		return new StandardValuesCollection(values);
	}

	public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
	{
		return true;
	}

	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
