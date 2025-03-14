using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System.Drawing.Printing;

public class MarginsConverter : ExpandableObjectConverter
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
		if (value is string)
		{
			if (value == null)
			{
				return new Margins();
			}
			string text = "( |\\t)*";
			text = text + ";" + text;
			string pattern = "(?<left>\\d+)" + text + "(?<right>\\d+)" + text + "(?<top>\\d+)" + text + "(?<bottom>\\d+)";
			Match match = new Regex(pattern).Match(value as string);
			if (!match.Success)
			{
				throw new ArgumentException("value");
			}
			int left;
			int right;
			int top;
			int bottom;
			try
			{
				left = int.Parse(match.Groups["left"].Value);
				right = int.Parse(match.Groups["right"].Value);
				top = int.Parse(match.Groups["top"].Value);
				bottom = int.Parse(match.Groups["bottom"].Value);
			}
			catch (Exception innerException)
			{
				throw new ArgumentException("value", innerException);
			}
			return new Margins(left, right, top, bottom);
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string) && value is Margins)
		{
			Margins margins = value as Margins;
			string format = "{0}; {1}; {2}; {3}";
			return string.Format(format, margins.Left, margins.Right, margins.Top, margins.Bottom);
		}
		if (destinationType == typeof(InstanceDescriptor) && value is Margins)
		{
			Margins margins2 = (Margins)value;
			ConstructorInfo constructor = typeof(Margins).GetConstructor(new Type[4]
			{
				typeof(int),
				typeof(int),
				typeof(int),
				typeof(int)
			});
			return new InstanceDescriptor(constructor, new object[4] { margins2.Left, margins2.Right, margins2.Top, margins2.Bottom });
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
	{
		try
		{
			Margins margins = new Margins();
			margins.Left = int.Parse(propertyValues["Left"].ToString());
			margins.Right = int.Parse(propertyValues["Right"].ToString());
			margins.Top = int.Parse(propertyValues["Top"].ToString());
			margins.Bottom = int.Parse(propertyValues["Bottom"].ToString());
			return margins;
		}
		catch (Exception)
		{
			return null;
		}
	}
}
