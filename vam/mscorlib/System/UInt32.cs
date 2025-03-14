using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace System;

[Serializable]
[CLSCompliant(false)]
[ComVisible(true)]
public struct UInt32 : IFormattable, IConvertible, IComparable, IComparable<uint>, IEquatable<uint>
{
	public const uint MaxValue = 4294967295u;

	public const uint MinValue = 0u;

	internal uint m_value;

	bool IConvertible.ToBoolean(IFormatProvider provider)
	{
		return Convert.ToBoolean(this);
	}

	byte IConvertible.ToByte(IFormatProvider provider)
	{
		return Convert.ToByte(this);
	}

	char IConvertible.ToChar(IFormatProvider provider)
	{
		return Convert.ToChar(this);
	}

	DateTime IConvertible.ToDateTime(IFormatProvider provider)
	{
		return Convert.ToDateTime(this);
	}

	decimal IConvertible.ToDecimal(IFormatProvider provider)
	{
		return Convert.ToDecimal(this);
	}

	double IConvertible.ToDouble(IFormatProvider provider)
	{
		return Convert.ToDouble(this);
	}

	short IConvertible.ToInt16(IFormatProvider provider)
	{
		return Convert.ToInt16(this);
	}

	int IConvertible.ToInt32(IFormatProvider provider)
	{
		return Convert.ToInt32(this);
	}

	long IConvertible.ToInt64(IFormatProvider provider)
	{
		return Convert.ToInt64(this);
	}

	sbyte IConvertible.ToSByte(IFormatProvider provider)
	{
		return Convert.ToSByte(this);
	}

	float IConvertible.ToSingle(IFormatProvider provider)
	{
		return Convert.ToSingle(this);
	}

	object IConvertible.ToType(Type targetType, IFormatProvider provider)
	{
		if (targetType == null)
		{
			throw new ArgumentNullException("targetType");
		}
		return Convert.ToType(this, targetType, provider, try_target_to_type: false);
	}

	ushort IConvertible.ToUInt16(IFormatProvider provider)
	{
		return Convert.ToUInt16(this);
	}

	uint IConvertible.ToUInt32(IFormatProvider provider)
	{
		return this;
	}

	ulong IConvertible.ToUInt64(IFormatProvider provider)
	{
		return Convert.ToUInt64(this);
	}

	public int CompareTo(object value)
	{
		if (value == null)
		{
			return 1;
		}
		if (!(value is uint num))
		{
			throw new ArgumentException(Locale.GetText("Value is not a System.UInt32."));
		}
		if (this == num)
		{
			return 0;
		}
		return (this >= num) ? 1 : (-1);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is uint))
		{
			return false;
		}
		return (uint)obj == this;
	}

	public override int GetHashCode()
	{
		return (int)this;
	}

	public int CompareTo(uint value)
	{
		if (this == value)
		{
			return 0;
		}
		if (this > value)
		{
			return 1;
		}
		return -1;
	}

	public bool Equals(uint obj)
	{
		return obj == this;
	}

	internal static bool Parse(string s, bool tryParse, out uint result, out Exception exc)
	{
		uint num = 0u;
		bool flag = false;
		bool flag2 = false;
		result = 0u;
		exc = null;
		if (s == null)
		{
			if (!tryParse)
			{
				exc = new ArgumentNullException("s");
			}
			return false;
		}
		int length = s.Length;
		int i;
		for (i = 0; i < length; i++)
		{
			char c = s[i];
			if (!char.IsWhiteSpace(c))
			{
				break;
			}
		}
		if (i == length)
		{
			if (!tryParse)
			{
				exc = int.GetFormatException();
			}
			return false;
		}
		if (s[i] == '+')
		{
			i++;
		}
		else if (s[i] == '-')
		{
			i++;
			flag2 = true;
		}
		for (; i < length; i++)
		{
			char c = s[i];
			if (c >= '0' && c <= '9')
			{
				uint num2 = (uint)(c - 48);
				if (num > 429496729 || (num == 429496729 && num2 > 5))
				{
					if (!tryParse)
					{
						exc = new OverflowException(Locale.GetText("Value is too large"));
					}
					return false;
				}
				num = num * 10 + num2;
				flag = true;
			}
			else if (!int.ProcessTrailingWhitespace(tryParse, s, i, ref exc))
			{
				return false;
			}
		}
		if (!flag)
		{
			if (!tryParse)
			{
				exc = int.GetFormatException();
			}
			return false;
		}
		if (flag2 && num != 0)
		{
			if (!tryParse)
			{
				exc = new OverflowException(Locale.GetText("Negative number"));
			}
			return false;
		}
		result = num;
		return true;
	}

	internal static bool Parse(string s, NumberStyles style, IFormatProvider provider, bool tryParse, out uint result, out Exception exc)
	{
		result = 0u;
		exc = null;
		if (s == null)
		{
			if (!tryParse)
			{
				exc = new ArgumentNullException("s");
			}
			return false;
		}
		if (s.Length == 0)
		{
			if (!tryParse)
			{
				exc = int.GetFormatException();
			}
			return false;
		}
		NumberFormatInfo numberFormatInfo = null;
		if (provider != null)
		{
			Type typeFromHandle = typeof(NumberFormatInfo);
			numberFormatInfo = (NumberFormatInfo)provider.GetFormat(typeFromHandle);
		}
		if (numberFormatInfo == null)
		{
			numberFormatInfo = Thread.CurrentThread.CurrentCulture.NumberFormat;
		}
		if (!int.CheckStyle(style, tryParse, ref exc))
		{
			return false;
		}
		bool flag = (style & NumberStyles.AllowCurrencySymbol) != 0;
		bool flag2 = (style & NumberStyles.AllowHexSpecifier) != 0;
		bool flag3 = (style & NumberStyles.AllowThousands) != 0;
		bool flag4 = (style & NumberStyles.AllowDecimalPoint) != 0;
		bool flag5 = (style & NumberStyles.AllowParentheses) != 0;
		bool flag6 = (style & NumberStyles.AllowTrailingSign) != 0;
		bool flag7 = (style & NumberStyles.AllowLeadingSign) != 0;
		bool flag8 = (style & NumberStyles.AllowTrailingWhite) != 0;
		bool flag9 = (style & NumberStyles.AllowLeadingWhite) != 0;
		int pos = 0;
		if (flag9 && !int.JumpOverWhite(ref pos, s, reportError: true, tryParse, ref exc))
		{
			return false;
		}
		bool flag10 = false;
		bool negative = false;
		bool foundSign = false;
		bool foundCurrency = false;
		if (flag5 && s[pos] == '(')
		{
			flag10 = true;
			foundSign = true;
			negative = true;
			pos++;
			if (flag9 && !int.JumpOverWhite(ref pos, s, reportError: true, tryParse, ref exc))
			{
				return false;
			}
			if (s.Substring(pos, numberFormatInfo.NegativeSign.Length) == numberFormatInfo.NegativeSign)
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			if (s.Substring(pos, numberFormatInfo.PositiveSign.Length) == numberFormatInfo.PositiveSign)
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
		}
		if (flag7 && !foundSign)
		{
			int.FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
			if (foundSign)
			{
				if (flag9 && !int.JumpOverWhite(ref pos, s, reportError: true, tryParse, ref exc))
				{
					return false;
				}
				if (flag)
				{
					int.FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
					if (foundCurrency && flag9 && !int.JumpOverWhite(ref pos, s, reportError: true, tryParse, ref exc))
					{
						return false;
					}
				}
			}
		}
		if (flag && !foundCurrency)
		{
			int.FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
			if (foundCurrency)
			{
				if (flag9 && !int.JumpOverWhite(ref pos, s, reportError: true, tryParse, ref exc))
				{
					return false;
				}
				if (foundCurrency && !foundSign && flag7)
				{
					int.FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
					if (foundSign && flag9 && !int.JumpOverWhite(ref pos, s, reportError: true, tryParse, ref exc))
					{
						return false;
					}
				}
			}
		}
		uint num = 0u;
		int num2 = 0;
		bool flag11 = false;
		do
		{
			if (!int.ValidDigit(s[pos], flag2))
			{
				if (!flag3 || !int.FindOther(ref pos, s, numberFormatInfo.NumberGroupSeparator))
				{
					if (flag11 || !flag4 || !int.FindOther(ref pos, s, numberFormatInfo.NumberDecimalSeparator))
					{
						break;
					}
					flag11 = true;
				}
				continue;
			}
			if (flag2)
			{
				num2++;
				char c = s[pos++];
				uint num3 = (uint)(char.IsDigit(c) ? (c - 48) : ((!char.IsLower(c)) ? (c - 65 + 10) : (c - 97 + 10)));
				if (tryParse)
				{
					ulong num4 = num * 16 + num3;
					if (num4 > uint.MaxValue)
					{
						return false;
					}
					num = (uint)num4;
				}
				else
				{
					num = checked(num * 16 + num3);
				}
				continue;
			}
			if (flag11)
			{
				num2++;
				if (s[pos++] != '0')
				{
					if (!tryParse)
					{
						exc = new OverflowException(Locale.GetText("Value too large or too small."));
					}
					return false;
				}
				continue;
			}
			num2++;
			try
			{
				num = checked(num * 10 + (uint)(s[pos++] - 48));
			}
			catch (OverflowException)
			{
				if (!tryParse)
				{
					exc = new OverflowException(Locale.GetText("Value too large or too small."));
				}
				return false;
			}
		}
		while (pos < s.Length);
		if (num2 == 0)
		{
			if (!tryParse)
			{
				exc = int.GetFormatException();
			}
			return false;
		}
		if (flag6 && !foundSign)
		{
			int.FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
			if (foundSign)
			{
				if (flag8 && !int.JumpOverWhite(ref pos, s, reportError: true, tryParse, ref exc))
				{
					return false;
				}
				if (flag)
				{
					int.FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
				}
			}
		}
		if (flag && !foundCurrency)
		{
			int.FindCurrency(ref pos, s, numberFormatInfo, ref foundCurrency);
			if (foundCurrency)
			{
				if (flag8 && !int.JumpOverWhite(ref pos, s, reportError: true, tryParse, ref exc))
				{
					return false;
				}
				if (!foundSign && flag6)
				{
					int.FindSign(ref pos, s, numberFormatInfo, ref foundSign, ref negative);
				}
			}
		}
		if (flag8 && pos < s.Length && !int.JumpOverWhite(ref pos, s, reportError: false, tryParse, ref exc))
		{
			return false;
		}
		if (flag10)
		{
			if (pos >= s.Length || s[pos++] != ')')
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			if (flag8 && pos < s.Length && !int.JumpOverWhite(ref pos, s, reportError: false, tryParse, ref exc))
			{
				return false;
			}
		}
		if (pos < s.Length && s[pos] != 0)
		{
			if (!tryParse)
			{
				exc = int.GetFormatException();
			}
			return false;
		}
		if (negative && num != 0)
		{
			if (!tryParse)
			{
				exc = new OverflowException(Locale.GetText("Negative number"));
			}
			return false;
		}
		result = num;
		return true;
	}

	[CLSCompliant(false)]
	public static uint Parse(string s)
	{
		if (!Parse(s, tryParse: false, out var result, out var exc))
		{
			throw exc;
		}
		return result;
	}

	[CLSCompliant(false)]
	public static uint Parse(string s, NumberStyles style, IFormatProvider provider)
	{
		if (!Parse(s, style, provider, tryParse: false, out var result, out var exc))
		{
			throw exc;
		}
		return result;
	}

	[CLSCompliant(false)]
	public static uint Parse(string s, IFormatProvider provider)
	{
		return Parse(s, NumberStyles.Integer, provider);
	}

	[CLSCompliant(false)]
	public static uint Parse(string s, NumberStyles style)
	{
		return Parse(s, style, null);
	}

	[CLSCompliant(false)]
	public static bool TryParse(string s, out uint result)
	{
		if (!Parse(s, tryParse: true, out result, out var _))
		{
			result = 0u;
			return false;
		}
		return true;
	}

	[CLSCompliant(false)]
	public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out uint result)
	{
		if (!Parse(s, style, provider, tryParse: true, out result, out var _))
		{
			result = 0u;
			return false;
		}
		return true;
	}

	public override string ToString()
	{
		return NumberFormatter.NumberToString(this, null);
	}

	public string ToString(IFormatProvider provider)
	{
		return NumberFormatter.NumberToString(this, provider);
	}

	public string ToString(string format)
	{
		return ToString(format, null);
	}

	public string ToString(string format, IFormatProvider provider)
	{
		return NumberFormatter.NumberToString(format, this, provider);
	}

	public TypeCode GetTypeCode()
	{
		return TypeCode.UInt32;
	}
}
