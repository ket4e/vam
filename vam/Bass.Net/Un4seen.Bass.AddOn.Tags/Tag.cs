using System;
using System.Security;

namespace Un4seen.Bass.AddOn.Tags;

[Serializable]
[SuppressUnmanagedCodeSecurity]
internal sealed class Tag
{
	private WMT_ATTR_DATATYPE _dataType;

	private object _value;

	private string _name;

	private int _index;

	public int Index => _index;

	public string Name => _name;

	public WMT_ATTR_DATATYPE DataType => _dataType;

	public object Value
	{
		get
		{
			return _value;
		}
		set
		{
			switch (_dataType)
			{
			case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:
				_value = (bool)value;
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:
				_value = (uint)value;
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_WORD:
				_value = (ushort)value;
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:
				_value = (ulong)value;
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_GUID:
				_value = (Guid)value;
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
				_value = (string)value;
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_BINARY:
				_value = (byte[])value;
				break;
			}
		}
	}

	public string ValueAsString
	{
		get
		{
			string result = string.Empty;
			switch (_dataType)
			{
			case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:
				result = ((bool)_value).ToString();
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:
				result = ((uint)_value).ToString();
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_WORD:
				result = ((ushort)_value).ToString();
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:
				result = ((ulong)_value).ToString();
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_GUID:
				result = ((Guid)_value).ToString();
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
				result = (string)_value;
				break;
			case WMT_ATTR_DATATYPE.WMT_TYPE_BINARY:
				result = "[" + ((byte[])_value).Length + " bytes]";
				break;
			}
			return result;
		}
	}

	public Tag(int index, string name, WMT_ATTR_DATATYPE type, object val)
	{
		_index = index;
		_name = name.TrimEnd(default(char));
		_dataType = type;
		switch (type)
		{
		case WMT_ATTR_DATATYPE.WMT_TYPE_BINARY:
			_value = (byte[])val;
			break;
		case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:
			_value = Convert.ToBoolean(val);
			break;
		case WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:
			_value = Convert.ToUInt32(val);
			break;
		case WMT_ATTR_DATATYPE.WMT_TYPE_GUID:
			_value = (Guid)val;
			break;
		case WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:
			_value = Convert.ToUInt64(val);
			break;
		case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
			_value = Convert.ToString(val).Trim();
			break;
		case WMT_ATTR_DATATYPE.WMT_TYPE_WORD:
			_value = Convert.ToUInt16(val);
			break;
		default:
			throw new ArgumentException("Invalid data type", "type");
		}
	}

	public override string ToString()
	{
		return $"{_name}={ValueAsString}";
	}

	public static explicit operator string(Tag tag)
	{
		if (tag._dataType == WMT_ATTR_DATATYPE.WMT_TYPE_STRING)
		{
			return (string)tag._value;
		}
		throw new InvalidCastException("Tag can not be converted to a string.");
	}

	public static explicit operator bool(Tag tag)
	{
		if (tag._dataType == WMT_ATTR_DATATYPE.WMT_TYPE_BOOL)
		{
			return (bool)tag._value;
		}
		throw new InvalidCastException("Tag can not be converted to a bool.");
	}

	public static explicit operator Guid(Tag tag)
	{
		if (tag._dataType == WMT_ATTR_DATATYPE.WMT_TYPE_GUID)
		{
			return (Guid)tag._value;
		}
		throw new InvalidCastException("Tag can not be converted to a Guid.");
	}

	public static explicit operator byte[](Tag tag)
	{
		if (tag._dataType == WMT_ATTR_DATATYPE.WMT_TYPE_BINARY)
		{
			return (byte[])tag._value;
		}
		throw new InvalidCastException("Tag can not be converted to a byte array.");
	}

	public static explicit operator ulong(Tag tag)
	{
		WMT_ATTR_DATATYPE dataType = tag._dataType;
		if (dataType == WMT_ATTR_DATATYPE.WMT_TYPE_DWORD || dataType == WMT_ATTR_DATATYPE.WMT_TYPE_QWORD || dataType == WMT_ATTR_DATATYPE.WMT_TYPE_WORD)
		{
			return (ulong)tag._value;
		}
		throw new InvalidCastException("Tag can not be converted to a number.");
	}

	public static explicit operator long(Tag tag)
	{
		return (long)(ulong)tag;
	}

	public static explicit operator int(Tag tag)
	{
		return (int)(ulong)tag;
	}

	public static explicit operator uint(Tag tag)
	{
		return (uint)(ulong)tag;
	}

	public static explicit operator ushort(Tag tag)
	{
		return (ushort)(ulong)tag;
	}
}
