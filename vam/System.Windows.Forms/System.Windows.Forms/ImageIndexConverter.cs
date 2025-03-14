using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public class ImageIndexConverter : Int32Converter
{
	protected virtual bool IncludeNoneAsStandardValue => true;

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value != null && value is string)
		{
			string text = (string)value;
			if (text == "(none)")
			{
				return -1;
			}
			return int.Parse(text);
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value != null && destinationType == typeof(string))
		{
			if (value is int && (int)value == -1)
			{
				return "(none)";
			}
			return value.ToString();
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		int[] values = new int[1] { -1 };
		return new StandardValuesCollection(values);
	}

	public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
	{
		return false;
	}

	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
