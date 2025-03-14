using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

public class CursorConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(byte[]))
		{
			return true;
		}
		return base.CanConvertFrom(context, sourceType);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(byte[]) || destinationType == typeof(InstanceDescriptor))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (!(value is byte[] buffer))
		{
			return base.ConvertFrom(context, culture, value);
		}
		using MemoryStream stream = new MemoryStream(buffer);
		return new Cursor(stream);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == null)
		{
			throw new ArgumentNullException("destinationType");
		}
		if (value == null && destinationType == typeof(string))
		{
			return "(none)";
		}
		if (!(value is Cursor))
		{
			throw new ArgumentException("object must be of class Cursor", "value");
		}
		if (destinationType == typeof(byte[]))
		{
			if (value == null)
			{
				return new byte[0];
			}
			Cursor cursor = (Cursor)value;
			SerializationInfo serializationInfo = new SerializationInfo(typeof(Cursor), new FormatterConverter());
			((ISerializable)cursor).GetObjectData(serializationInfo, new StreamingContext(StreamingContextStates.Remoting));
			return (byte[])serializationInfo.GetValue("CursorData", typeof(byte[]));
		}
		if (destinationType == typeof(InstanceDescriptor))
		{
			PropertyInfo[] properties = typeof(Cursors).GetProperties();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (propertyInfo.GetValue(null, null) == value)
				{
					return new InstanceDescriptor(propertyInfo, null);
				}
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		PropertyInfo[] properties = typeof(Cursors).GetProperties();
		ArrayList arrayList = new ArrayList();
		for (int i = 0; i < properties.Length; i++)
		{
			arrayList.Add(properties[i].GetValue(null, null));
		}
		return new StandardValuesCollection(arrayList);
	}

	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
