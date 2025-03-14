namespace System.Data.Odbc;

internal class OdbcColumn
{
	internal string ColumnName;

	internal OdbcType OdbcType;

	private SQL_TYPE _sqlType = SQL_TYPE.UNASSIGNED;

	private SQL_C_TYPE _sqlCType = SQL_C_TYPE.UNASSIGNED;

	internal bool AllowDBNull;

	internal int MaxLength;

	internal int Digits;

	internal object Value;

	internal Type DataType
	{
		get
		{
			switch (OdbcType)
			{
			case OdbcType.TinyInt:
				return typeof(byte);
			case OdbcType.BigInt:
				return typeof(long);
			case OdbcType.Binary:
			case OdbcType.Image:
			case OdbcType.VarBinary:
				return typeof(byte[]);
			case OdbcType.Bit:
				return typeof(bool);
			case OdbcType.Char:
			case OdbcType.NChar:
				return typeof(string);
			case OdbcType.Time:
				return typeof(TimeSpan);
			case OdbcType.DateTime:
			case OdbcType.SmallDateTime:
			case OdbcType.Timestamp:
			case OdbcType.Date:
				return typeof(DateTime);
			case OdbcType.Decimal:
			case OdbcType.Numeric:
				return typeof(decimal);
			case OdbcType.Double:
				return typeof(double);
			case OdbcType.Int:
				return typeof(int);
			case OdbcType.NText:
			case OdbcType.NVarChar:
			case OdbcType.Text:
			case OdbcType.VarChar:
				return typeof(string);
			case OdbcType.Real:
				return typeof(float);
			case OdbcType.SmallInt:
				return typeof(short);
			case OdbcType.UniqueIdentifier:
				return typeof(Guid);
			default:
				throw new InvalidCastException();
			}
		}
	}

	internal bool IsDateType
	{
		get
		{
			switch (OdbcType)
			{
			case OdbcType.DateTime:
			case OdbcType.SmallDateTime:
			case OdbcType.Timestamp:
			case OdbcType.Date:
			case OdbcType.Time:
				return true;
			default:
				return false;
			}
		}
	}

	internal bool IsStringType
	{
		get
		{
			OdbcType odbcType = OdbcType;
			if (odbcType == OdbcType.NText || odbcType == OdbcType.NVarChar || odbcType == OdbcType.Char || odbcType == OdbcType.Text || odbcType == OdbcType.VarChar)
			{
				return true;
			}
			return false;
		}
	}

	internal bool IsVariableSizeType
	{
		get
		{
			if (IsStringType)
			{
				return true;
			}
			OdbcType odbcType = OdbcType;
			if (odbcType == OdbcType.Binary || odbcType == OdbcType.Image || odbcType == OdbcType.VarBinary)
			{
				return true;
			}
			return false;
		}
	}

	internal SQL_TYPE SqlType
	{
		get
		{
			if (_sqlType == SQL_TYPE.UNASSIGNED)
			{
				_sqlType = OdbcTypeConverter.GetTypeMap(OdbcType).SqlType;
			}
			return _sqlType;
		}
		set
		{
			_sqlType = value;
		}
	}

	internal SQL_C_TYPE SqlCType
	{
		get
		{
			if (_sqlCType == SQL_C_TYPE.UNASSIGNED)
			{
				_sqlCType = OdbcTypeConverter.GetTypeMap(OdbcType).NativeType;
			}
			return _sqlCType;
		}
		set
		{
			_sqlCType = value;
		}
	}

	internal OdbcColumn(string Name, OdbcType Type)
	{
		ColumnName = Name;
		OdbcType = Type;
		AllowDBNull = false;
		MaxLength = 0;
		Digits = 0;
		Value = null;
	}

	internal OdbcColumn(string Name, SQL_TYPE type)
	{
		ColumnName = Name;
		AllowDBNull = false;
		MaxLength = 0;
		Digits = 0;
		Value = null;
		UpdateTypes(type);
	}

	internal void UpdateTypes(SQL_TYPE sqlType)
	{
		SqlType = sqlType;
		OdbcTypeMap typeMap = OdbcTypeConverter.GetTypeMap(SqlType);
		OdbcType = typeMap.OdbcType;
		SqlCType = typeMap.NativeType;
	}
}
