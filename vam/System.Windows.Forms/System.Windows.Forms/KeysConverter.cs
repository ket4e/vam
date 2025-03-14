using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace System.Windows.Forms;

public class KeysConverter : TypeConverter, IComparer
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
		if (destinationType == typeof(Enum[]))
		{
			return true;
		}
		return base.CanConvertTo(context, destinationType);
	}

	public int Compare(object a, object b)
	{
		if (a is string && b is string)
		{
			return string.Compare((string)a, (string)b);
		}
		return string.Compare(a.ToString(), b.ToString());
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string)
		{
			string[] array = ((string)value).Split('+');
			Keys keys = Keys.None;
			if (array.Length > 1)
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					keys = ((!array[i].Equals("Ctrl")) ? ((Keys)((int)keys | (int)Enum.Parse(typeof(Keys), array[i], ignoreCase: true))) : (keys | Keys.Control));
				}
			}
			keys = ((!array[array.Length - 1].Equals("Ctrl")) ? ((Keys)((int)keys | (int)Enum.Parse(typeof(Keys), array[array.Length - 1], ignoreCase: true))) : (keys | Keys.Control));
			return keys;
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			StringBuilder stringBuilder = new StringBuilder();
			Keys keys = (Keys)(int)value;
			if ((keys & Keys.Control) != 0)
			{
				stringBuilder.Append("Ctrl+");
			}
			if ((keys & Keys.Alt) != 0)
			{
				stringBuilder.Append("Alt+");
			}
			if ((keys & Keys.Shift) != 0)
			{
				stringBuilder.Append("Shift+");
			}
			stringBuilder.Append(Enum.GetName(typeof(Keys), keys & Keys.KeyCode));
			return stringBuilder.ToString();
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		Keys[] values = new Keys[33]
		{
			Keys.D0,
			Keys.D1,
			Keys.D2,
			Keys.D3,
			Keys.D4,
			Keys.D5,
			Keys.D6,
			Keys.D7,
			Keys.D8,
			Keys.D9,
			Keys.Alt,
			Keys.Back,
			Keys.Control,
			Keys.Delete,
			Keys.End,
			Keys.Return,
			Keys.F1,
			Keys.F10,
			Keys.F11,
			Keys.F12,
			Keys.F2,
			Keys.F3,
			Keys.F4,
			Keys.F5,
			Keys.F6,
			Keys.F7,
			Keys.F8,
			Keys.F9,
			Keys.Home,
			Keys.Insert,
			Keys.PageDown,
			Keys.PageUp,
			Keys.Shift
		};
		return new StandardValuesCollection(values);
	}

	public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
	{
		return false;
	}

	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return true;
	}
}
