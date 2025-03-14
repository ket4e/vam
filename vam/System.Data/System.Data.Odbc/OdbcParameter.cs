using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Odbc;

[TypeConverter("System.Data.Odbc.OdbcParameter+OdbcParameterConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public sealed class OdbcParameter : DbParameter, IDataParameter, IDbDataParameter, ICloneable
{
	private string name;

	private ParameterDirection direction;

	private bool isNullable;

	private int size;

	private DataRowVersion sourceVersion;

	private string sourceColumn;

	private byte _precision;

	private byte _scale;

	private object _value;

	private OdbcTypeMap _typeMap;

	private NativeBuffer _nativeBuffer = new NativeBuffer();

	private NativeBuffer _cbLengthInd;

	private OdbcParameterCollection container;

	internal OdbcParameterCollection Container
	{
		get
		{
			return container;
		}
		set
		{
			container = value;
		}
	}

	[OdbcDescription("The parameter generic type")]
	[OdbcCategory("Data")]
	public override DbType DbType
	{
		get
		{
			return _typeMap.DbType;
		}
		set
		{
			if (value != _typeMap.DbType)
			{
				_typeMap = OdbcTypeConverter.GetTypeMap(value);
			}
		}
	}

	[OdbcDescription("Input, output, or bidirectional parameter")]
	[RefreshProperties(RefreshProperties.All)]
	[OdbcCategory("Data")]
	public override ParameterDirection Direction
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;
		}
	}

	[OdbcDescription("A design-time property used for strongly typed code generation")]
	public override bool IsNullable
	{
		get
		{
			return isNullable;
		}
		set
		{
			isNullable = value;
		}
	}

	[OdbcDescription("The parameter native type")]
	[DbProviderSpecificTypeProperty(true)]
	[OdbcCategory("Data")]
	[DefaultValue(OdbcType.NChar)]
	[RefreshProperties(RefreshProperties.All)]
	public OdbcType OdbcType
	{
		get
		{
			return _typeMap.OdbcType;
		}
		set
		{
			if (value != _typeMap.OdbcType)
			{
				_typeMap = OdbcTypeConverter.GetTypeMap(value);
			}
		}
	}

	[OdbcDescription("DataParameter_ParameterName")]
	public override string ParameterName
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	[OdbcCategory("DataCategory_Data")]
	[OdbcDescription("DbDataParameter_Precision")]
	[DefaultValue(0)]
	public byte Precision
	{
		get
		{
			return _precision;
		}
		set
		{
			_precision = value;
		}
	}

	[DefaultValue(0)]
	[OdbcDescription("DbDataParameter_Scale")]
	[OdbcCategory("DataCategory_Data")]
	public byte Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
		}
	}

	[OdbcCategory("DataCategory_Data")]
	[OdbcDescription("DbDataParameter_Size")]
	public override int Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}

	[OdbcCategory("DataCategory_Data")]
	[OdbcDescription("DataParameter_SourceColumn")]
	public override string SourceColumn
	{
		get
		{
			return sourceColumn;
		}
		set
		{
			sourceColumn = value;
		}
	}

	[OdbcCategory("DataCategory_Data")]
	[OdbcDescription("DataParameter_SourceVersion")]
	public override DataRowVersion SourceVersion
	{
		get
		{
			return sourceVersion;
		}
		set
		{
			sourceVersion = value;
		}
	}

	[OdbcDescription("DataParameter_Value")]
	[RefreshProperties(RefreshProperties.All)]
	[TypeConverter(typeof(StringConverter))]
	[OdbcCategory("DataCategory_Data")]
	public override object Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public override bool SourceColumnNullMapping
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public OdbcParameter()
	{
		_cbLengthInd = new NativeBuffer();
		ParameterName = string.Empty;
		IsNullable = false;
		SourceColumn = string.Empty;
		Direction = ParameterDirection.Input;
		_typeMap = OdbcTypeConverter.GetTypeMap(OdbcType.NVarChar);
	}

	public OdbcParameter(string name, object value)
		: this()
	{
		ParameterName = name;
		Value = value;
		_typeMap = OdbcTypeConverter.InferFromValue(value);
		if (value != null && !value.GetType().IsValueType)
		{
			Type type = value.GetType();
			if (type.IsArray)
			{
				Size = ((type.GetElementType() == typeof(byte)) ? ((Array)value).Length : 0);
			}
			else
			{
				Size = value.ToString().Length;
			}
		}
	}

	public OdbcParameter(string name, OdbcType type)
		: this()
	{
		ParameterName = name;
		_typeMap = OdbcTypeConverter.GetTypeMap(type);
	}

	public OdbcParameter(string name, OdbcType type, int size)
		: this(name, type)
	{
		Size = size;
	}

	public OdbcParameter(string name, OdbcType type, int size, string sourcecolumn)
		: this(name, type, size)
	{
		SourceColumn = sourcecolumn;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public OdbcParameter(string parameterName, OdbcType odbcType, int size, ParameterDirection parameterDirection, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
		: this(parameterName, odbcType, size, srcColumn)
	{
		Direction = parameterDirection;
		IsNullable = isNullable;
		SourceVersion = srcVersion;
	}

	[System.MonoTODO]
	object ICloneable.Clone()
	{
		throw new NotImplementedException();
	}

	internal void Bind(OdbcCommand command, IntPtr hstmt, int ParamNum)
	{
		OdbcInputOutputDirection inputOutputType = libodbc.ConvertParameterDirection(Direction);
		_cbLengthInd.EnsureAlloc(Marshal.SizeOf(typeof(int)));
		int val;
		if (Value is DBNull)
		{
			val = -1;
		}
		else
		{
			val = GetNativeSize();
			AllocateBuffer();
		}
		Marshal.WriteInt32(_cbLengthInd, val);
		OdbcReturn odbcReturn = libodbc.SQLBindParameter(hstmt, (ushort)ParamNum, (short)inputOutputType, _typeMap.NativeType, _typeMap.SqlType, Convert.ToUInt32(Size), 0, _nativeBuffer, 0, _cbLengthInd);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw command.Connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
		}
	}

	public override string ToString()
	{
		return ParameterName;
	}

	private int GetNativeSize()
	{
		TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
		Encoding encoding = Encoding.GetEncoding(textInfo.ANSICodePage);
		switch (_typeMap.OdbcType)
		{
		case OdbcType.Binary:
			if (Value.GetType().IsArray && Value.GetType().GetElementType() == typeof(byte))
			{
				return ((Array)Value).Length;
			}
			return Value.ToString().Length;
		case OdbcType.Bit:
			return Marshal.SizeOf(typeof(byte));
		case OdbcType.Double:
			return Marshal.SizeOf(typeof(double));
		case OdbcType.Real:
			return Marshal.SizeOf(typeof(float));
		case OdbcType.Int:
			return Marshal.SizeOf(typeof(int));
		case OdbcType.BigInt:
			return Marshal.SizeOf(typeof(long));
		case OdbcType.Decimal:
		case OdbcType.Numeric:
			return 19;
		case OdbcType.SmallInt:
			return Marshal.SizeOf(typeof(short));
		case OdbcType.TinyInt:
			return Marshal.SizeOf(typeof(byte));
		case OdbcType.Char:
		case OdbcType.Text:
		case OdbcType.VarChar:
			return encoding.GetByteCount(Convert.ToString(Value)) + 1;
		case OdbcType.NChar:
		case OdbcType.NText:
		case OdbcType.NVarChar:
			return encoding.GetByteCount(Convert.ToString(Value)) + 1;
		case OdbcType.Image:
		case OdbcType.VarBinary:
			if (Value.GetType().IsArray && Value.GetType().GetElementType() == typeof(byte))
			{
				return ((Array)Value).Length;
			}
			throw new ArgumentException("Unsupported Native Type!");
		case OdbcType.DateTime:
		case OdbcType.SmallDateTime:
		case OdbcType.Timestamp:
		case OdbcType.Date:
		case OdbcType.Time:
			return 18;
		case OdbcType.UniqueIdentifier:
			return Marshal.SizeOf(typeof(Guid));
		default:
			if (Value.GetType().IsArray && Value.GetType().GetElementType() == typeof(byte))
			{
				return ((Array)Value).Length;
			}
			return Value.ToString().Length;
		}
	}

	private void AllocateBuffer()
	{
		int nativeSize = GetNativeSize();
		if (_nativeBuffer.Size != nativeSize)
		{
			_nativeBuffer.AllocBuffer(nativeSize);
		}
	}

	internal void CopyValue()
	{
		if (_nativeBuffer.Handle == IntPtr.Zero || Value is DBNull)
		{
			return;
		}
		TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
		Encoding encoding = Encoding.GetEncoding(textInfo.ANSICodePage);
		switch (_typeMap.OdbcType)
		{
		case OdbcType.Bit:
			Marshal.WriteByte(_nativeBuffer, Convert.ToByte(Value));
			break;
		case OdbcType.Double:
			Marshal.StructureToPtr(Convert.ToDouble(Value), _nativeBuffer, fDeleteOld: false);
			break;
		case OdbcType.Real:
			Marshal.StructureToPtr(Convert.ToSingle(Value), _nativeBuffer, fDeleteOld: false);
			break;
		case OdbcType.Int:
			Marshal.WriteInt32(_nativeBuffer, Convert.ToInt32(Value));
			break;
		case OdbcType.BigInt:
			Marshal.WriteInt64(_nativeBuffer, Convert.ToInt64(Value));
			break;
		case OdbcType.Decimal:
		case OdbcType.Numeric:
		{
			int[] bits = decimal.GetBits(Convert.ToDecimal(Value));
			byte[] array = new byte[19]
			{
				Precision,
				(byte)((bits[3] & 0xFF0000) >> 16),
				(byte)(((bits[3] & 0x80000000u) <= 0) ? 1u : 2u),
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			Buffer.BlockCopy(bits, 0, array, 3, 12);
			for (int i = 16; i < 19; i++)
			{
				array[i] = 0;
			}
			Marshal.Copy(array, 0, _nativeBuffer, 19);
			break;
		}
		case OdbcType.SmallInt:
			Marshal.WriteInt16(_nativeBuffer, Convert.ToInt16(Value));
			break;
		case OdbcType.TinyInt:
			Marshal.WriteByte(_nativeBuffer, Convert.ToByte(Value));
			break;
		case OdbcType.Char:
		case OdbcType.Text:
		case OdbcType.VarChar:
		{
			byte[] array = new byte[GetNativeSize()];
			byte[] bytes = encoding.GetBytes(Convert.ToString(Value));
			Array.Copy(bytes, 0, array, 0, bytes.Length);
			array[array.Length - 1] = 0;
			Marshal.Copy(array, 0, _nativeBuffer, array.Length);
			Marshal.WriteInt32(_cbLengthInd, -3);
			break;
		}
		case OdbcType.NChar:
		case OdbcType.NText:
		case OdbcType.NVarChar:
		{
			byte[] array = new byte[GetNativeSize()];
			byte[] bytes = encoding.GetBytes(Convert.ToString(Value));
			Array.Copy(bytes, 0, array, 0, bytes.Length);
			array[array.Length - 1] = 0;
			Marshal.Copy(array, 0, _nativeBuffer, array.Length);
			Marshal.WriteInt32(_cbLengthInd, -3);
			break;
		}
		case OdbcType.Binary:
		case OdbcType.Image:
		case OdbcType.VarBinary:
			if (Value.GetType().IsArray && Value.GetType().GetElementType() == typeof(byte))
			{
				Marshal.Copy((byte[])Value, 0, _nativeBuffer, ((byte[])Value).Length);
				break;
			}
			throw new ArgumentException("Unsupported Native Type!");
		case OdbcType.Date:
		{
			DateTime dateTime = (DateTime)Value;
			Marshal.WriteInt16(_nativeBuffer, 0, (short)dateTime.Year);
			Marshal.WriteInt16(_nativeBuffer, 2, (short)dateTime.Month);
			Marshal.WriteInt16(_nativeBuffer, 4, (short)dateTime.Day);
			break;
		}
		case OdbcType.Time:
		{
			DateTime dateTime = (DateTime)Value;
			Marshal.WriteInt16(_nativeBuffer, 0, (short)dateTime.Hour);
			Marshal.WriteInt16(_nativeBuffer, 2, (short)dateTime.Minute);
			Marshal.WriteInt16(_nativeBuffer, 4, (short)dateTime.Second);
			break;
		}
		case OdbcType.DateTime:
		case OdbcType.SmallDateTime:
		case OdbcType.Timestamp:
		{
			DateTime dateTime = (DateTime)Value;
			Marshal.WriteInt16(_nativeBuffer, 0, (short)dateTime.Year);
			Marshal.WriteInt16(_nativeBuffer, 2, (short)dateTime.Month);
			Marshal.WriteInt16(_nativeBuffer, 4, (short)dateTime.Day);
			Marshal.WriteInt16(_nativeBuffer, 6, (short)dateTime.Hour);
			Marshal.WriteInt16(_nativeBuffer, 8, (short)dateTime.Minute);
			Marshal.WriteInt16(_nativeBuffer, 10, (short)dateTime.Second);
			Marshal.WriteInt32(_nativeBuffer, 12, (int)(dateTime.Ticks % 10000000) * 100);
			break;
		}
		case OdbcType.UniqueIdentifier:
			throw new NotImplementedException();
		default:
			if (Value.GetType().IsArray && Value.GetType().GetElementType() == typeof(byte))
			{
				Marshal.Copy((byte[])Value, 0, _nativeBuffer, ((byte[])Value).Length);
				break;
			}
			throw new ArgumentException("Unsupported Native Type!");
		}
	}

	public override void ResetDbType()
	{
		_typeMap = OdbcTypeConverter.GetTypeMap(OdbcType.NVarChar);
	}

	public void ResetOdbcType()
	{
		_typeMap = OdbcTypeConverter.GetTypeMap(OdbcType.NVarChar);
	}
}
