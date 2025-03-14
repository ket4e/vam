using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

[Serializable]
[TypeConverter(typeof(LinkAreaConverter))]
public struct LinkArea
{
	public class LinkAreaConverter : TypeConverter
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
			return base.CanConvertTo(context, destinationType);
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
			int start = int.Parse(array[0].Trim());
			int length = int.Parse(array[1].Trim());
			return new LinkArea(start, length);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value == null || !(value is LinkArea) || destinationType != typeof(string))
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			LinkArea linkArea = (LinkArea)value;
			return linkArea.Start + culture.TextInfo.ListSeparator + linkArea.Length;
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return new LinkArea((int)propertyValues["Start"], (int)propertyValues["Length"]);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(LinkArea), attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}

	private int start;

	private int length;

	public int Start
	{
		get
		{
			return start;
		}
		set
		{
			start = value;
		}
	}

	public int Length
	{
		get
		{
			return length;
		}
		set
		{
			length = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public bool IsEmpty
	{
		get
		{
			if (start == 0 && length == 0)
			{
				return true;
			}
			return false;
		}
	}

	public LinkArea(int start, int length)
	{
		this.start = start;
		this.length = length;
	}

	public override bool Equals(object o)
	{
		if (!(o is LinkArea linkArea))
		{
			return false;
		}
		return linkArea.Start == start && linkArea.Length == length;
	}

	public override int GetHashCode()
	{
		return (start << 4) | length;
	}

	public override string ToString()
	{
		return $"{{Start={start.ToString()}, Length={length.ToString()}}}";
	}

	public static bool operator ==(LinkArea linkArea1, LinkArea linkArea2)
	{
		return linkArea1.Length == linkArea2.Length && linkArea1.Start == linkArea2.Start;
	}

	public static bool operator !=(LinkArea linkArea1, LinkArea linkArea2)
	{
		return !(linkArea1 == linkArea2);
	}
}
