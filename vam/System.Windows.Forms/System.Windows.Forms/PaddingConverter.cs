using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms;

public class PaddingConverter : TypeConverter
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
		if (destinationType == typeof(InstanceDescriptor))
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
		return new Padding(int.Parse(array[0].Trim()), int.Parse(array[1].Trim()), int.Parse(array[2].Trim()), int.Parse(array[3].Trim()));
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Padding padding)
		{
			if (destinationType == typeof(string))
			{
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				return string.Format("{0}{4} {1}{4} {2}{4} {3}", padding.Left, padding.Top, padding.Right, padding.Bottom, culture.TextInfo.ListSeparator);
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				Type[] types;
				object[] arguments;
				if (padding.All != -1)
				{
					types = new Type[1] { typeof(int) };
					arguments = new object[1] { padding.All };
				}
				else
				{
					types = new Type[4]
					{
						typeof(int),
						typeof(int),
						typeof(int),
						typeof(int)
					};
					arguments = new object[4] { padding.Left, padding.Top, padding.Right, padding.Bottom };
				}
				ConstructorInfo constructor = typeof(Padding).GetConstructor(types);
				return new InstanceDescriptor(constructor, arguments);
			}
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		if (propertyValues == null)
		{
			throw new ArgumentNullException("propertyValues");
		}
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		if (((Padding)context.PropertyDescriptor.GetValue(context.Instance)).All == (int)propertyValues["All"])
		{
			return new Padding((int)propertyValues["Left"], (int)propertyValues["Top"], (int)propertyValues["Right"], (int)propertyValues["Bottom"]);
		}
		return new Padding((int)propertyValues["All"]);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		return TypeDescriptor.GetProperties(typeof(Padding), attributes);
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
