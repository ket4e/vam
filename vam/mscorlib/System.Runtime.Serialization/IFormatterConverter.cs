using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[CLSCompliant(false)]
[ComVisible(true)]
public interface IFormatterConverter
{
	object Convert(object value, Type type);

	object Convert(object value, TypeCode typeCode);

	bool ToBoolean(object value);

	byte ToByte(object value);

	char ToChar(object value);

	DateTime ToDateTime(object value);

	decimal ToDecimal(object value);

	double ToDouble(object value);

	short ToInt16(object value);

	int ToInt32(object value);

	long ToInt64(object value);

	sbyte ToSByte(object value);

	float ToSingle(object value);

	string ToString(object value);

	ushort ToUInt16(object value);

	uint ToUInt32(object value);

	ulong ToUInt64(object value);
}
