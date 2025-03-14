using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Drawing;

public class SizeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}
		return base.CanConvertFrom(context, sourceType);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			return true;
		}
		if (destinationType == typeof(InstanceDescriptor))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		if (!(value is string text))
		{
			return base.ConvertFrom(context, culture, value);
		}
		string[] array = text.Split(culture.TextInfo.ListSeparator.ToCharArray());
		Int32Converter int32Converter = new Int32Converter();
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = (int)int32Converter.ConvertFromString(context, culture, array[i]);
		}
		if (array.Length != 2)
		{
			throw new ArgumentException("Failed to parse Text(" + text + ") expected text in the format \"Width,Height.\"");
		}
		return new Size(array2[0], array2[1]);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (culture == null)
		{
			culture = CultureInfo.CurrentCulture;
		}
		if (value is Size size)
		{
			if (destinationType == typeof(string))
			{
				return size.Width.ToString(culture) + culture.TextInfo.ListSeparator + " " + size.Height.ToString(culture);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				ConstructorInfo constructor = typeof(Size).GetConstructor(new Type[2]
				{
					typeof(int),
					typeof(int)
				});
				return new InstanceDescriptor(constructor, new object[2] { size.Width, size.Height });
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		object obj = propertyValues["Width"];
		object obj2 = propertyValues["Height"];
		if (obj == null || obj2 == null)
		{
			throw new ArgumentException("propertyValues");
		}
		int width = (int)obj;
		int height = (int)obj2;
		return new Size(width, height);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		if (value is Size)
		{
			return TypeDescriptor.GetProperties(value, attributes);
		}
		return base.GetProperties(context, value, attributes);
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
