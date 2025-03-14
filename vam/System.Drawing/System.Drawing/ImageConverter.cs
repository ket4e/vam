using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing;

public class ImageConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(byte[]))
		{
			return true;
		}
		return false;
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(byte[]) || destinationType == typeof(string))
		{
			return true;
		}
		return false;
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (!(value is byte[] buffer))
		{
			return base.ConvertFrom(context, culture, value);
		}
		MemoryStream stream = new MemoryStream(buffer);
		return Image.FromStream(stream);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value == null)
		{
			return "(none)";
		}
		if (value is Image)
		{
			if (destinationType == typeof(string))
			{
				return value.ToString();
			}
			if (CanConvertTo(null, destinationType))
			{
				MemoryStream memoryStream = new MemoryStream();
				((Image)value).Save(memoryStream, ((Image)value).RawFormat);
				return memoryStream.GetBuffer();
			}
		}
		string text = global::Locale.GetText("ImageConverter can not convert from type '{0}'.", value.GetType());
		throw new NotSupportedException(text);
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		return TypeDescriptor.GetProperties(typeof(Image), attributes);
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
