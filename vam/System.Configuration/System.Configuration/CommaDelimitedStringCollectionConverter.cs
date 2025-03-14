using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration;

public sealed class CommaDelimitedStringCollectionConverter : ConfigurationConverterBase
{
	public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
	{
		CommaDelimitedStringCollection commaDelimitedStringCollection = new CommaDelimitedStringCollection();
		string[] array = ((string)data).Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			commaDelimitedStringCollection.Add(text.Trim());
		}
		return commaDelimitedStringCollection;
	}

	public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
	{
		if (value == null)
		{
			return null;
		}
		if (!typeof(StringCollection).IsAssignableFrom(value.GetType()))
		{
			throw new ArgumentException();
		}
		return value.ToString();
	}
}
