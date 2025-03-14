using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

public class SelectionRangeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}
		return false;
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			return true;
		}
		return false;
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value == null || !(value is string))
		{
			return base.ConvertFrom(context, culture, value);
		}
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		string[] array = ((string)value).Split(culture.TextInfo.ListSeparator.ToCharArray());
		DateTime lower = (DateTime)TypeDescriptor.GetConverter(typeof(DateTime)).ConvertFromString(context, culture, array[0]);
		DateTime upper = (DateTime)TypeDescriptor.GetConverter(typeof(DateTime)).ConvertFromString(context, culture, array[1]);
		return new SelectionRange(lower, upper);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value == null || !(value is SelectionRange) || destinationType != typeof(string))
		{
			return base.ConvertTo(context, culture, value, destinationType);
		}
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		SelectionRange selectionRange = (SelectionRange)value;
		return selectionRange.Start.ToShortDateString() + culture.TextInfo.ListSeparator + selectionRange.End.ToShortDateString();
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		return new SelectionRange((DateTime)propertyValues["Start"], (DateTime)propertyValues["End"]);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		return TypeDescriptor.GetProperties(typeof(SelectionRange), attributes);
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
