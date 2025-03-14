using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public class TreeViewImageIndexConverter : ImageIndexConverter
{
	protected override bool IncludeNoneAsStandardValue => false;

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value != null && value is string)
		{
			string text = (string)value;
			if (text.Equals("(default)", StringComparison.InvariantCultureIgnoreCase))
			{
				return -1;
			}
			if (text.Equals("(none)", StringComparison.InvariantCultureIgnoreCase))
			{
				return -2;
			}
			return int.Parse(text);
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			if (value == null)
			{
				return string.Empty;
			}
			if (value is int && (int)value == -1)
			{
				return "(default)";
			}
			if (value is int && (int)value == -2)
			{
				return "(none)";
			}
			if (value is string && ((string)value).Length == 0)
			{
				return string.Empty;
			}
			return value.ToString();
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		int[] values = new int[2] { -1, -2 };
		return new StandardValuesCollection(values);
	}
}
