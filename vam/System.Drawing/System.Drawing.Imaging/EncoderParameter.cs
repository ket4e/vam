using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
public sealed class EncoderParameter : IDisposable
{
	private Encoder encoder;

	private int valuesCount;

	private EncoderParameterValueType type;

	private IntPtr valuePtr;

	public Encoder Encoder
	{
		get
		{
			return encoder;
		}
		set
		{
			encoder = value;
		}
	}

	public int NumberOfValues => valuesCount;

	public EncoderParameterValueType Type => type;

	public EncoderParameterValueType ValueType => type;

	internal EncoderParameter()
	{
	}

	public EncoderParameter(Encoder encoder, byte value)
	{
		this.encoder = encoder;
		valuesCount = 1;
		type = EncoderParameterValueType.ValueTypeByte;
		valuePtr = Marshal.AllocHGlobal(1);
		Marshal.WriteByte(valuePtr, value);
	}

	public EncoderParameter(Encoder encoder, byte[] value)
	{
		this.encoder = encoder;
		valuesCount = value.Length;
		type = EncoderParameterValueType.ValueTypeByte;
		valuePtr = Marshal.AllocHGlobal(1 * valuesCount);
		Marshal.Copy(value, 0, valuePtr, valuesCount);
	}

	public EncoderParameter(Encoder encoder, short value)
	{
		this.encoder = encoder;
		valuesCount = 1;
		type = EncoderParameterValueType.ValueTypeShort;
		valuePtr = Marshal.AllocHGlobal(2);
		Marshal.WriteInt16(valuePtr, value);
	}

	public EncoderParameter(Encoder encoder, short[] value)
	{
		this.encoder = encoder;
		valuesCount = value.Length;
		type = EncoderParameterValueType.ValueTypeShort;
		valuePtr = Marshal.AllocHGlobal(2 * valuesCount);
		Marshal.Copy(value, 0, valuePtr, valuesCount);
	}

	public EncoderParameter(Encoder encoder, long value)
	{
		this.encoder = encoder;
		valuesCount = 1;
		type = EncoderParameterValueType.ValueTypeLong;
		valuePtr = Marshal.AllocHGlobal(4);
		Marshal.WriteInt32(valuePtr, (int)value);
	}

	public EncoderParameter(Encoder encoder, long[] value)
	{
		this.encoder = encoder;
		valuesCount = value.Length;
		type = EncoderParameterValueType.ValueTypeLong;
		valuePtr = Marshal.AllocHGlobal(4 * valuesCount);
		int[] array = new int[value.Length];
		for (int i = 0; i < value.Length; i++)
		{
			array[i] = (int)value[i];
		}
		Marshal.Copy(array, 0, valuePtr, valuesCount);
	}

	public EncoderParameter(Encoder encoder, string value)
	{
		this.encoder = encoder;
		ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
		int byteCount = aSCIIEncoding.GetByteCount(value);
		byte[] array = new byte[byteCount];
		aSCIIEncoding.GetBytes(value, 0, value.Length, array, 0);
		valuesCount = array.Length;
		type = EncoderParameterValueType.ValueTypeAscii;
		valuePtr = Marshal.AllocHGlobal(valuesCount);
		Marshal.Copy(array, 0, valuePtr, valuesCount);
	}

	public EncoderParameter(Encoder encoder, byte value, bool undefined)
	{
		this.encoder = encoder;
		valuesCount = 1;
		if (undefined)
		{
			type = EncoderParameterValueType.ValueTypeUndefined;
		}
		else
		{
			type = EncoderParameterValueType.ValueTypeByte;
		}
		valuePtr = Marshal.AllocHGlobal(1);
		Marshal.WriteByte(valuePtr, value);
	}

	public EncoderParameter(Encoder encoder, byte[] value, bool undefined)
	{
		this.encoder = encoder;
		valuesCount = value.Length;
		if (undefined)
		{
			type = EncoderParameterValueType.ValueTypeUndefined;
		}
		else
		{
			type = EncoderParameterValueType.ValueTypeByte;
		}
		valuePtr = Marshal.AllocHGlobal(valuesCount);
		Marshal.Copy(value, 0, valuePtr, valuesCount);
	}

	public EncoderParameter(Encoder encoder, int numerator, int denominator)
	{
		this.encoder = encoder;
		valuesCount = 1;
		type = EncoderParameterValueType.ValueTypeRational;
		valuePtr = Marshal.AllocHGlobal(8);
		int[] array = new int[2] { numerator, denominator };
		Marshal.Copy(array, 0, valuePtr, array.Length);
	}

	public EncoderParameter(Encoder encoder, int[] numerator, int[] denominator)
	{
		if (numerator.Length != denominator.Length)
		{
			throw new ArgumentException("Invalid parameter used.");
		}
		this.encoder = encoder;
		valuesCount = numerator.Length;
		type = EncoderParameterValueType.ValueTypeRational;
		valuePtr = Marshal.AllocHGlobal(4 * valuesCount * 2);
		for (int i = 0; i < valuesCount; i++)
		{
			Marshal.WriteInt32(valuePtr, i * 4, numerator[i]);
			Marshal.WriteInt32(valuePtr, (i + 1) * 4, denominator[i]);
		}
	}

	public EncoderParameter(Encoder encoder, long rangebegin, long rangeend)
	{
		this.encoder = encoder;
		valuesCount = 1;
		type = EncoderParameterValueType.ValueTypeLongRange;
		valuePtr = Marshal.AllocHGlobal(8);
		int[] array = new int[2]
		{
			(int)rangebegin,
			(int)rangeend
		};
		Marshal.Copy(array, 0, valuePtr, array.Length);
	}

	public EncoderParameter(Encoder encoder, long[] rangebegin, long[] rangeend)
	{
		if (rangebegin.Length != rangeend.Length)
		{
			throw new ArgumentException("Invalid parameter used.");
		}
		this.encoder = encoder;
		valuesCount = rangebegin.Length;
		type = EncoderParameterValueType.ValueTypeLongRange;
		valuePtr = Marshal.AllocHGlobal(4 * valuesCount * 2);
		IntPtr ptr = valuePtr;
		for (int i = 0; i < valuesCount; i++)
		{
			Marshal.WriteInt32(ptr, i * 4, (int)rangebegin[i]);
			Marshal.WriteInt32(ptr, (i + 1) * 4, (int)rangeend[i]);
		}
	}

	public EncoderParameter(Encoder encoder, int numberOfValues, int type, int value)
	{
		this.encoder = encoder;
		valuePtr = (IntPtr)value;
		valuesCount = numberOfValues;
		this.type = (EncoderParameterValueType)type;
	}

	public EncoderParameter(Encoder encoder, int numerator1, int denominator1, int numerator2, int denominator2)
	{
		this.encoder = encoder;
		valuesCount = 1;
		type = EncoderParameterValueType.ValueTypeRationalRange;
		valuePtr = Marshal.AllocHGlobal(16);
		int[] source = new int[4] { numerator1, denominator1, numerator2, denominator2 };
		Marshal.Copy(source, 0, valuePtr, 4);
	}

	public EncoderParameter(Encoder encoder, int[] numerator1, int[] denominator1, int[] numerator2, int[] denominator2)
	{
		if (numerator1.Length != denominator1.Length || numerator2.Length != denominator2.Length || numerator1.Length != numerator2.Length)
		{
			throw new ArgumentException("Invalid parameter used.");
		}
		this.encoder = encoder;
		valuesCount = numerator1.Length;
		type = EncoderParameterValueType.ValueTypeRationalRange;
		valuePtr = Marshal.AllocHGlobal(4 * valuesCount * 4);
		IntPtr ptr = valuePtr;
		for (int i = 0; i < valuesCount; i++)
		{
			Marshal.WriteInt32(ptr, i * 4, numerator1[i]);
			Marshal.WriteInt32(ptr, (i + 1) * 4, denominator1[i]);
			Marshal.WriteInt32(ptr, (i + 2) * 4, numerator2[i]);
			Marshal.WriteInt32(ptr, (i + 3) * 4, denominator2[i]);
		}
	}

	private void Dispose(bool disposing)
	{
		if (valuePtr != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(valuePtr);
			valuePtr = IntPtr.Zero;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~EncoderParameter()
	{
		Dispose(disposing: false);
	}

	internal static int NativeSize()
	{
		return Marshal.SizeOf(typeof(GdipEncoderParameter));
	}

	internal void ToNativePtr(IntPtr epPtr)
	{
		GdipEncoderParameter gdipEncoderParameter = default(GdipEncoderParameter);
		gdipEncoderParameter.guid = encoder.Guid;
		gdipEncoderParameter.numberOfValues = (uint)valuesCount;
		gdipEncoderParameter.type = type;
		gdipEncoderParameter.value = valuePtr;
		Marshal.StructureToPtr(gdipEncoderParameter, epPtr, fDeleteOld: false);
	}

	internal unsafe static EncoderParameter FromNativePtr(IntPtr epPtr)
	{
		GdipEncoderParameter gdipEncoderParameter = (GdipEncoderParameter)Marshal.PtrToStructure(epPtr, typeof(GdipEncoderParameter));
		Type typeFromHandle;
		uint num;
		switch (gdipEncoderParameter.type)
		{
		case EncoderParameterValueType.ValueTypeByte:
		case EncoderParameterValueType.ValueTypeAscii:
		case EncoderParameterValueType.ValueTypeUndefined:
			typeFromHandle = typeof(byte);
			num = gdipEncoderParameter.numberOfValues;
			break;
		case EncoderParameterValueType.ValueTypeShort:
			typeFromHandle = typeof(short);
			num = gdipEncoderParameter.numberOfValues;
			break;
		case EncoderParameterValueType.ValueTypeLong:
			typeFromHandle = typeof(int);
			num = gdipEncoderParameter.numberOfValues;
			break;
		case EncoderParameterValueType.ValueTypeRational:
		case EncoderParameterValueType.ValueTypeLongRange:
			typeFromHandle = typeof(int);
			num = gdipEncoderParameter.numberOfValues * 2;
			break;
		case EncoderParameterValueType.ValueTypeRationalRange:
			typeFromHandle = typeof(int);
			num = gdipEncoderParameter.numberOfValues * 4;
			break;
		default:
			return null;
		}
		EncoderParameter encoderParameter = new EncoderParameter();
		encoderParameter.encoder = new Encoder(gdipEncoderParameter.guid);
		encoderParameter.valuesCount = (int)gdipEncoderParameter.numberOfValues;
		encoderParameter.type = gdipEncoderParameter.type;
		encoderParameter.valuePtr = Marshal.AllocHGlobal((int)(num * Marshal.SizeOf(typeFromHandle)));
		byte* ptr = (byte*)(void*)gdipEncoderParameter.value;
		byte* ptr2 = (byte*)(void*)encoderParameter.valuePtr;
		for (int i = 0; i < num * Marshal.SizeOf(typeFromHandle); i++)
		{
			*(ptr2++) = *(ptr++);
		}
		return encoderParameter;
	}
}
