using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[ComVisible(true)]
public class FormatterConverter : IFormatterConverter
{
	public object Convert(object value, Type type)
	{
		return System.Convert.ChangeType(value, type);
	}

	public object Convert(object value, TypeCode typeCode)
	{
		return System.Convert.ChangeType(value, typeCode);
	}

	public bool ToBoolean(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToBoolean(value);
	}

	public byte ToByte(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToByte(value);
	}

	public char ToChar(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToChar(value);
	}

	public DateTime ToDateTime(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToDateTime(value);
	}

	public decimal ToDecimal(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToDecimal(value);
	}

	public double ToDouble(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToDouble(value);
	}

	public short ToInt16(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToInt16(value);
	}

	public int ToInt32(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToInt32(value);
	}

	public long ToInt64(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToInt64(value);
	}

	public float ToSingle(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToSingle(value);
	}

	public string ToString(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToString(value);
	}

	[CLSCompliant(false)]
	public sbyte ToSByte(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToSByte(value);
	}

	[CLSCompliant(false)]
	public ushort ToUInt16(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToUInt16(value);
	}

	[CLSCompliant(false)]
	public uint ToUInt32(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToUInt32(value);
	}

	[CLSCompliant(false)]
	public ulong ToUInt64(object value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value is null.");
		}
		return System.Convert.ToUInt64(value);
	}
}
