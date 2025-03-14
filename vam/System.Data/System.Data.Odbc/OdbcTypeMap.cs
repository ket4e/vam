using System.Collections;

namespace System.Data.Odbc;

internal struct OdbcTypeMap
{
	public DbType DbType;

	public OdbcType OdbcType;

	public SQL_C_TYPE NativeType;

	public SQL_TYPE SqlType;

	private static Hashtable maps;

	public static Hashtable Maps => maps;

	public OdbcTypeMap(DbType dbType, OdbcType odbcType, SQL_C_TYPE nativeType, SQL_TYPE sqlType)
	{
		DbType = dbType;
		OdbcType = odbcType;
		SqlType = sqlType;
		NativeType = nativeType;
	}

	static OdbcTypeMap()
	{
		maps = new Hashtable();
		maps[OdbcType.BigInt] = new OdbcTypeMap(DbType.Int64, OdbcType.BigInt, SQL_C_TYPE.SBIGINT, SQL_TYPE.BIGINT);
		maps[OdbcType.Binary] = new OdbcTypeMap(DbType.Binary, OdbcType.Binary, SQL_C_TYPE.BINARY, SQL_TYPE.BINARY);
		maps[OdbcType.Bit] = new OdbcTypeMap(DbType.Boolean, OdbcType.Bit, SQL_C_TYPE.BIT, SQL_TYPE.BIT);
		maps[OdbcType.Char] = new OdbcTypeMap(DbType.String, OdbcType.Char, SQL_C_TYPE.CHAR, SQL_TYPE.CHAR);
		maps[OdbcType.Date] = new OdbcTypeMap(DbType.Date, OdbcType.Date, SQL_C_TYPE.DATE, SQL_TYPE.DATE);
		maps[OdbcType.DateTime] = new OdbcTypeMap(DbType.DateTime, OdbcType.DateTime, SQL_C_TYPE.TIMESTAMP, SQL_TYPE.TIMESTAMP);
		maps[OdbcType.Decimal] = new OdbcTypeMap(DbType.Decimal, OdbcType.Decimal, SQL_C_TYPE.NUMERIC, SQL_TYPE.NUMERIC);
		maps[OdbcType.Double] = new OdbcTypeMap(DbType.Double, OdbcType.Double, SQL_C_TYPE.DOUBLE, SQL_TYPE.DOUBLE);
		maps[OdbcType.Image] = new OdbcTypeMap(DbType.Binary, OdbcType.Image, SQL_C_TYPE.BINARY, SQL_TYPE.BINARY);
		maps[OdbcType.Int] = new OdbcTypeMap(DbType.Int32, OdbcType.Int, SQL_C_TYPE.LONG, SQL_TYPE.INTEGER);
		maps[OdbcType.NChar] = new OdbcTypeMap(DbType.String, OdbcType.NChar, SQL_C_TYPE.WCHAR, SQL_TYPE.WCHAR);
		maps[OdbcType.NText] = new OdbcTypeMap(DbType.String, OdbcType.NText, SQL_C_TYPE.WCHAR, SQL_TYPE.WLONGVARCHAR);
		maps[OdbcType.Numeric] = new OdbcTypeMap(DbType.Decimal, OdbcType.Numeric, SQL_C_TYPE.CHAR, SQL_TYPE.NUMERIC);
		maps[OdbcType.NVarChar] = new OdbcTypeMap(DbType.String, OdbcType.NVarChar, SQL_C_TYPE.WCHAR, SQL_TYPE.WVARCHAR);
		maps[OdbcType.Real] = new OdbcTypeMap(DbType.Single, OdbcType.Real, SQL_C_TYPE.FLOAT, SQL_TYPE.REAL);
		maps[OdbcType.SmallDateTime] = new OdbcTypeMap(DbType.DateTime, OdbcType.SmallDateTime, SQL_C_TYPE.TIMESTAMP, SQL_TYPE.TIMESTAMP);
		maps[OdbcType.SmallInt] = new OdbcTypeMap(DbType.Int16, OdbcType.SmallInt, SQL_C_TYPE.SHORT, SQL_TYPE.SMALLINT);
		maps[OdbcType.Text] = new OdbcTypeMap(DbType.String, OdbcType.Text, SQL_C_TYPE.CHAR, SQL_TYPE.LONGVARCHAR);
		maps[OdbcType.Time] = new OdbcTypeMap(DbType.DateTime, OdbcType.Time, SQL_C_TYPE.TIME, SQL_TYPE.TIME);
		maps[OdbcType.Timestamp] = new OdbcTypeMap(DbType.DateTime, OdbcType.Timestamp, SQL_C_TYPE.BINARY, SQL_TYPE.BINARY);
		maps[OdbcType.TinyInt] = new OdbcTypeMap(DbType.SByte, OdbcType.TinyInt, SQL_C_TYPE.UTINYINT, SQL_TYPE.TINYINT);
		maps[OdbcType.UniqueIdentifier] = new OdbcTypeMap(DbType.Guid, OdbcType.UniqueIdentifier, SQL_C_TYPE.GUID, SQL_TYPE.GUID);
		maps[OdbcType.VarBinary] = new OdbcTypeMap(DbType.Binary, OdbcType.VarBinary, SQL_C_TYPE.BINARY, SQL_TYPE.VARBINARY);
		maps[OdbcType.VarChar] = new OdbcTypeMap(DbType.String, OdbcType.VarChar, SQL_C_TYPE.CHAR, SQL_TYPE.VARCHAR);
	}
}
