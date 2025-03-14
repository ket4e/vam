using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Drawing;

public class SizeFConverter : TypeConverter
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
		if (!(value is string text))
		{
			return base.ConvertFrom(context, culture, value);
		}
		string[] array = text.Split(culture.TextInfo.ListSeparator.ToCharArray());
		SingleConverter singleConverter = new SingleConverter();
		float[] array2 = new float[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = (float)singleConverter.ConvertFromString(context, culture, array[i]);
		}
		if (array.Length != 2)
		{
			throw new ArgumentException("Failed to parse Text(" + text + ") expected text in the format \"Width,Height.\"");
		}
		return new SizeF(array2[0], array2[1]);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is SizeF sizeF)
		{
			if (destinationType == typeof(string))
			{
				return sizeF.Width.ToString(culture) + culture.TextInfo.ListSeparator + " " + sizeF.Height.ToString(culture);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				ConstructorInfo constructor = typeof(SizeF).GetConstructor(new Type[2]
				{
					typeof(float),
					typeof(float)
				});
				return new InstanceDescriptor(constructor, new object[2] { sizeF.Width, sizeF.Height });
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		float width = (float)propertyValues["Width"];
		float height = (float)propertyValues["Height"];
		return new SizeF(width, height);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		if (value is SizeF)
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
