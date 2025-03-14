using System.ComponentModel;
using System.Data.Common;

namespace System.Data.OleDb;

[TypeConverter("System.Data.OleDb.OleDbParameter+OleDbParameterConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public sealed class OleDbParameter : DbParameter, IDataParameter, IDbDataParameter, ICloneable
{
	private string name;

	private object value;

	private int size;

	private bool isNullable;

	private byte precision;

	private byte scale;

	private DataRowVersion sourceVersion;

	private string sourceColumn;

	private bool sourceColumnNullMapping;

	private ParameterDirection direction;

	private OleDbType oleDbType;

	private DbType dbType;

	private OleDbParameterCollection container;

	private IntPtr gdaParameter;

	[DataCategory("DataCategory_Data")]
	public override DbType DbType
	{
		get
		{
			return dbType;
		}
		set
		{
			dbType = value;
			oleDbType = DbTypeToOleDbType(value);
		}
	}

	[RefreshProperties(RefreshProperties.All)]
	[DataCategory("DataCategory_Data")]
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

	[RefreshProperties(RefreshProperties.All)]
	[DbProviderSpecificTypeProperty(true)]
	[DataCategory("DataCategory_Data")]
	public OleDbType OleDbType
	{
		get
		{
			return oleDbType;
		}
		set
		{
			oleDbType = value;
			dbType = OleDbTypeToDbType(value);
		}
	}

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

	[DataCategory("DataCategory_Data")]
	[DefaultValue(0)]
	public byte Precision
	{
		get
		{
			return precision;
		}
		set
		{
			precision = value;
		}
	}

	[DefaultValue(0)]
	[DataCategory("DataCategory_Data")]
	public byte Scale
	{
		get
		{
			return scale;
		}
		set
		{
			scale = value;
		}
	}

	[DataCategory("DataCategory_Data")]
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

	[DataCategory("DataCategory_Data")]
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

	public override bool SourceColumnNullMapping
	{
		get
		{
			return sourceColumnNullMapping;
		}
		set
		{
			sourceColumnNullMapping = value;
		}
	}

	[DataCategory("DataCategory_Data")]
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

	[DataCategory("DataCategory_Data")]
	[TypeConverter(typeof(StringConverter))]
	[RefreshProperties(RefreshProperties.All)]
	public override object Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	internal OleDbParameterCollection Container
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

	internal IntPtr GdaParameter => gdaParameter;

	public OleDbParameter()
	{
		name = string.Empty;
		isNullable = true;
		sourceColumn = string.Empty;
		gdaParameter = IntPtr.Zero;
	}

	public OleDbParameter(string name, object value)
		: this()
	{
		this.name = name;
		this.value = value;
		OleDbType = GetOleDbType(value);
	}

	public OleDbParameter(string name, OleDbType dataType)
		: this()
	{
		this.name = name;
		OleDbType = dataType;
	}

	public OleDbParameter(string name, OleDbType dataType, int size)
		: this(name, dataType)
	{
		this.size = size;
	}

	public OleDbParameter(string name, OleDbType dataType, int size, string srcColumn)
		: this(name, dataType, size)
	{
		sourceColumn = srcColumn;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public OleDbParameter(string parameterName, OleDbType dbType, int size, ParameterDirection direction, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
		: this(parameterName, dbType, size, srcColumn)
	{
		this.direction = direction;
		this.isNullable = isNullable;
		this.precision = precision;
		this.scale = scale;
		sourceVersion = srcVersion;
		this.value = value;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public OleDbParameter(string parameterName, OleDbType dbType, int size, ParameterDirection direction, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
		: this(parameterName, dbType, size, sourceColumn)
	{
		this.direction = direction;
		this.precision = precision;
		this.scale = scale;
		this.sourceVersion = sourceVersion;
		this.sourceColumnNullMapping = sourceColumnNullMapping;
		this.value = value;
	}

	[System.MonoTODO]
	object ICloneable.Clone()
	{
		throw new NotImplementedException();
	}

	public override void ResetDbType()
	{
		ResetOleDbType();
	}

	public void ResetOleDbType()
	{
		oleDbType = GetOleDbType(Value);
		dbType = OleDbTypeToDbType(oleDbType);
	}

	public override string ToString()
	{
		return ParameterName;
	}

	private OleDbType DbTypeToOleDbType(DbType dbType)
	{
		return dbType switch
		{
			DbType.AnsiString => OleDbType.VarChar, 
			DbType.AnsiStringFixedLength => OleDbType.Char, 
			DbType.Binary => OleDbType.Binary, 
			DbType.Boolean => OleDbType.Boolean, 
			DbType.Byte => OleDbType.UnsignedTinyInt, 
			DbType.Currency => OleDbType.Currency, 
			DbType.Date => OleDbType.Date, 
			DbType.DateTime => throw new NotImplementedException(), 
			DbType.Decimal => OleDbType.Decimal, 
			DbType.Double => OleDbType.Double, 
			DbType.Guid => OleDbType.Guid, 
			DbType.Int16 => OleDbType.SmallInt, 
			DbType.Int32 => OleDbType.Integer, 
			DbType.Int64 => OleDbType.BigInt, 
			DbType.Object => OleDbType.Variant, 
			DbType.SByte => OleDbType.TinyInt, 
			DbType.Single => OleDbType.Single, 
			DbType.String => OleDbType.WChar, 
			DbType.StringFixedLength => OleDbType.VarWChar, 
			DbType.Time => throw new NotImplementedException(), 
			DbType.UInt16 => OleDbType.UnsignedSmallInt, 
			DbType.UInt32 => OleDbType.UnsignedInt, 
			DbType.UInt64 => OleDbType.UnsignedBigInt, 
			DbType.VarNumeric => OleDbType.VarNumeric, 
			_ => OleDbType.Variant, 
		};
	}

	private DbType OleDbTypeToDbType(OleDbType oleDbType)
	{
		return oleDbType switch
		{
			OleDbType.BigInt => DbType.Int64, 
			OleDbType.Binary => DbType.Binary, 
			OleDbType.Boolean => DbType.Boolean, 
			OleDbType.BSTR => DbType.AnsiString, 
			OleDbType.Char => DbType.AnsiStringFixedLength, 
			OleDbType.Currency => DbType.Currency, 
			OleDbType.Date => DbType.DateTime, 
			OleDbType.DBDate => DbType.DateTime, 
			OleDbType.DBTime => throw new NotImplementedException(), 
			OleDbType.DBTimeStamp => DbType.DateTime, 
			OleDbType.Decimal => DbType.Decimal, 
			OleDbType.Double => DbType.Double, 
			OleDbType.Empty => throw new NotImplementedException(), 
			OleDbType.Error => throw new NotImplementedException(), 
			OleDbType.Filetime => DbType.DateTime, 
			OleDbType.Guid => DbType.Guid, 
			OleDbType.IDispatch => DbType.Object, 
			OleDbType.Integer => DbType.Int32, 
			OleDbType.IUnknown => DbType.Object, 
			OleDbType.LongVarBinary => DbType.Binary, 
			OleDbType.LongVarChar => DbType.AnsiString, 
			OleDbType.LongVarWChar => DbType.String, 
			OleDbType.Numeric => DbType.Decimal, 
			OleDbType.PropVariant => DbType.Object, 
			OleDbType.Single => DbType.Single, 
			OleDbType.SmallInt => DbType.Int16, 
			OleDbType.TinyInt => DbType.SByte, 
			OleDbType.UnsignedBigInt => DbType.UInt64, 
			OleDbType.UnsignedInt => DbType.UInt32, 
			OleDbType.UnsignedSmallInt => DbType.UInt16, 
			OleDbType.UnsignedTinyInt => DbType.Byte, 
			OleDbType.VarBinary => DbType.Binary, 
			OleDbType.VarChar => DbType.AnsiString, 
			OleDbType.Variant => DbType.Object, 
			OleDbType.VarNumeric => DbType.VarNumeric, 
			OleDbType.VarWChar => DbType.StringFixedLength, 
			OleDbType.WChar => DbType.String, 
			_ => DbType.Object, 
		};
	}

	private OleDbType GetOleDbType(object value)
	{
		if (value is Guid)
		{
			return OleDbType.Guid;
		}
		if (value is TimeSpan)
		{
			return OleDbType.DBTime;
		}
		switch (Type.GetTypeCode(value.GetType()))
		{
		case TypeCode.Boolean:
			return OleDbType.Boolean;
		case TypeCode.Byte:
			if (value.GetType().IsArray)
			{
				return OleDbType.Binary;
			}
			return OleDbType.UnsignedTinyInt;
		case TypeCode.Char:
			return OleDbType.Char;
		case TypeCode.DateTime:
			return OleDbType.Date;
		case TypeCode.DBNull:
			return OleDbType.Empty;
		case TypeCode.Decimal:
			return OleDbType.Decimal;
		case TypeCode.Double:
			return OleDbType.Double;
		case TypeCode.Empty:
			return OleDbType.Empty;
		case TypeCode.Int16:
			return OleDbType.SmallInt;
		case TypeCode.Int32:
			return OleDbType.Integer;
		case TypeCode.Int64:
			return OleDbType.BigInt;
		case TypeCode.SByte:
			return OleDbType.TinyInt;
		case TypeCode.String:
			return OleDbType.VarChar;
		case TypeCode.Single:
			return OleDbType.Single;
		case TypeCode.UInt64:
			return OleDbType.UnsignedBigInt;
		case TypeCode.UInt32:
			return OleDbType.UnsignedInt;
		case TypeCode.UInt16:
			return OleDbType.UnsignedSmallInt;
		case TypeCode.Object:
			return OleDbType.Variant;
		default:
			return OleDbType.IUnknown;
		}
	}
}
