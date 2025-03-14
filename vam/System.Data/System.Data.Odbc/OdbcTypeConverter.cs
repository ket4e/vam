namespace System.Data.Odbc;

internal class OdbcTypeConverter
{
	public static OdbcTypeMap GetTypeMap(OdbcType odbcType)
	{
		return (OdbcTypeMap)OdbcTypeMap.Maps[odbcType];
	}

	public static OdbcTypeMap InferFromValue(object value)
	{
		if (value.GetType().IsArray)
		{
			if (value.GetType().GetElementType() == typeof(byte))
			{
				return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Binary];
			}
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
		}
		switch (Type.GetTypeCode(value.GetType()))
		{
		case TypeCode.Empty:
		case TypeCode.Object:
		case TypeCode.DBNull:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NVarChar];
		case TypeCode.Boolean:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Bit];
		case TypeCode.Char:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Char];
		case TypeCode.SByte:
			throw new ArgumentException("infering OdbcType from SByte is not supported");
		case TypeCode.Byte:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.TinyInt];
		case TypeCode.Int16:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.SmallInt];
		case TypeCode.UInt16:
		case TypeCode.Int32:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Int];
		case TypeCode.UInt32:
		case TypeCode.Int64:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.BigInt];
		case TypeCode.UInt64:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric];
		case TypeCode.Single:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Real];
		case TypeCode.Double:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Double];
		case TypeCode.Decimal:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric];
		case TypeCode.DateTime:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
		case TypeCode.String:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NVarChar];
		default:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
		}
	}

	public static OdbcTypeMap GetTypeMap(SQL_TYPE sqlType)
	{
		switch (sqlType)
		{
		case SQL_TYPE.BINARY:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Binary];
		case SQL_TYPE.BIT:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Bit];
		case SQL_TYPE.CHAR:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Char];
		case SQL_TYPE.DATE:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Date];
		case SQL_TYPE.DECIMAL:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Decimal];
		case SQL_TYPE.DOUBLE:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Double];
		case SQL_TYPE.GUID:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.UniqueIdentifier];
		case SQL_TYPE.INTEGER:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Int];
		case SQL_TYPE.INTERVAL_YEAR:
		case SQL_TYPE.INTERVAL_MONTH:
		case SQL_TYPE.INTERVAL_DAY:
		case SQL_TYPE.INTERVAL_HOUR:
		case SQL_TYPE.INTERVAL_MINUTE:
		case SQL_TYPE.INTERVAL_SECOND:
		case SQL_TYPE.INTERVAL_YEAR_TO_MONTH:
		case SQL_TYPE.INTERVAL_DAY_TO_HOUR:
		case SQL_TYPE.INTERVAL_DAY_TO_MINUTE:
		case SQL_TYPE.INTERVAL_DAY_TO_SECOND:
		case SQL_TYPE.INTERVAL_HOUR_TO_MINUTE:
		case SQL_TYPE.INTERVAL_HOUR_TO_SECOND:
		case SQL_TYPE.INTERVAL_MINUTE_TO_SECOND:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
		case SQL_TYPE.LONGVARBINARY:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Image];
		case SQL_TYPE.LONGVARCHAR:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Text];
		case SQL_TYPE.NUMERIC:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric];
		case SQL_TYPE.REAL:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Real];
		case SQL_TYPE.SMALLINT:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.SmallInt];
		case SQL_TYPE.TIME:
		case SQL_TYPE.TYPE_TIME:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Time];
		case SQL_TYPE.TIMESTAMP:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
		case SQL_TYPE.TINYINT:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.TinyInt];
		case SQL_TYPE.TYPE_DATE:
		case SQL_TYPE.TYPE_TIMESTAMP:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime];
		case SQL_TYPE.VARBINARY:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarBinary];
		case SQL_TYPE.VARCHAR:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
		case SQL_TYPE.WCHAR:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NChar];
		case SQL_TYPE.WLONGVARCHAR:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NText];
		case SQL_TYPE.WVARCHAR:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NVarChar];
		case SQL_TYPE.UNASSIGNED:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
		default:
			return (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar];
		}
	}

	public static OdbcTypeMap GetTypeMap(DbType dbType)
	{
		return dbType switch
		{
			DbType.AnsiString => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar], 
			DbType.Binary => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Binary], 
			DbType.Byte => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.TinyInt], 
			DbType.Boolean => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Bit], 
			DbType.Currency => throw new NotSupportedException("Infering OdbcType from DbType.Currency is not supported"), 
			DbType.Date => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Date], 
			DbType.DateTime => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.DateTime], 
			DbType.Decimal => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric], 
			DbType.Double => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Double], 
			DbType.Guid => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.UniqueIdentifier], 
			DbType.Int16 => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.SmallInt], 
			DbType.Int32 => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Int], 
			DbType.Int64 => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.BigInt], 
			DbType.Object => throw new NotSupportedException("Infering OdbcType from DbType.Object is not supported"), 
			DbType.SByte => throw new NotSupportedException("Infering OdbcType from DbType.SByte is not supported"), 
			DbType.Single => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Real], 
			DbType.String => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NVarChar], 
			DbType.Time => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Time], 
			DbType.UInt16 => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Int], 
			DbType.UInt32 => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.BigInt], 
			DbType.UInt64 => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Numeric], 
			DbType.VarNumeric => throw new NotSupportedException("Infering OdbcType from DbType.VarNumeric is not supported"), 
			DbType.AnsiStringFixedLength => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.Char], 
			DbType.StringFixedLength => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.NChar], 
			_ => (OdbcTypeMap)OdbcTypeMap.Maps[OdbcType.VarChar], 
		};
	}
}
