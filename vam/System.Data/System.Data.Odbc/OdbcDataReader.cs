using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Text;

namespace System.Data.Odbc;

public sealed class OdbcDataReader : DbDataReader
{
	private OdbcCommand command;

	private bool open;

	private int currentRow;

	private OdbcColumn[] cols;

	private IntPtr hstmt;

	private int _recordsAffected = -1;

	private bool disposed;

	private DataTable _dataTableSchema;

	private CommandBehavior behavior;

	private CommandBehavior CommandBehavior
	{
		get
		{
			return behavior;
		}
		set
		{
			behavior = value;
		}
	}

	public override int Depth => 0;

	public override int FieldCount
	{
		get
		{
			if (IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			return cols.Length;
		}
	}

	public override bool IsClosed => !open;

	public override object this[string value]
	{
		get
		{
			int ordinal = GetOrdinal(value);
			return this[ordinal];
		}
	}

	public override object this[int i] => GetValue(i);

	public override int RecordsAffected => _recordsAffected;

	[System.MonoTODO]
	public override bool HasRows
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	private OdbcConnection Connection
	{
		get
		{
			if (command != null)
			{
				return command.Connection;
			}
			return null;
		}
	}

	internal OdbcDataReader(OdbcCommand command, CommandBehavior behavior)
	{
		this.command = command;
		CommandBehavior = behavior;
		open = true;
		currentRow = -1;
		hstmt = command.hStmt;
		short ColumnCount = 0;
		libodbc.SQLNumResultCols(hstmt, ref ColumnCount);
		cols = new OdbcColumn[ColumnCount];
		GetColumns();
	}

	internal OdbcDataReader(OdbcCommand command, CommandBehavior behavior, int recordAffected)
		: this(command, behavior)
	{
		_recordsAffected = recordAffected;
	}

	private int ColIndex(string colname)
	{
		int num = 0;
		OdbcColumn[] array = cols;
		foreach (OdbcColumn odbcColumn in array)
		{
			if (odbcColumn != null)
			{
				if (odbcColumn.ColumnName == colname)
				{
					return num;
				}
				if (string.Compare(odbcColumn.ColumnName, colname, ignoreCase: true) == 0)
				{
					return num;
				}
			}
			num++;
		}
		return -1;
	}

	private OdbcColumn GetColumn(int ordinal)
	{
		if (cols[ordinal] == null)
		{
			short num = 255;
			byte[] array = new byte[num];
			short NameLength = 0;
			uint ColumnSize = 0u;
			short DecimalDigits = 0;
			short Nullable = 0;
			short DataType = 0;
			OdbcReturn odbcReturn = libodbc.SQLDescribeCol(hstmt, Convert.ToUInt16(ordinal + 1), array, num, ref NameLength, ref DataType, ref ColumnSize, ref DecimalDigits, ref Nullable);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
			}
			string name = RemoveTrailingNullChar(Encoding.Unicode.GetString(array));
			OdbcColumn odbcColumn = new OdbcColumn(name, (SQL_TYPE)DataType);
			odbcColumn.AllowDBNull = Nullable != 0;
			odbcColumn.Digits = DecimalDigits;
			if (odbcColumn.IsVariableSizeType)
			{
				odbcColumn.MaxLength = (int)ColumnSize;
			}
			cols[ordinal] = odbcColumn;
		}
		return cols[ordinal];
	}

	private void GetColumns()
	{
		for (int i = 0; i < cols.Length; i++)
		{
			GetColumn(i);
		}
	}

	public override void Close()
	{
		open = false;
		currentRow = -1;
		command.FreeIfNotPrepared();
		if ((CommandBehavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
		{
			command.Connection.Close();
		}
	}

	public override bool GetBoolean(int i)
	{
		return (bool)GetValue(i);
	}

	public override byte GetByte(int i)
	{
		return Convert.ToByte(GetValue(i));
	}

	public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("Reader is not open.");
		}
		if (currentRow == -1)
		{
			throw new InvalidOperationException("No data available.");
		}
		OdbcReturn odbcReturn = OdbcReturn.Error;
		bool flag = false;
		int num = 0;
		int Len = 0;
		byte[] array = new byte[length + 1];
		if (buffer == null)
		{
			length = 0;
		}
		odbcReturn = libodbc.SQLGetData(hstmt, (ushort)(i + 1), SQL_C_TYPE.BINARY, array, length, ref Len);
		switch (odbcReturn)
		{
		case OdbcReturn.NoData:
			return 0L;
		default:
			throw Connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
		case OdbcReturn.Success:
		case OdbcReturn.SuccessWithInfo:
		{
			OdbcException ex = null;
			if (odbcReturn == OdbcReturn.SuccessWithInfo)
			{
				ex = Connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
			}
			if (buffer == null)
			{
				return Len;
			}
			if (odbcReturn == OdbcReturn.SuccessWithInfo)
			{
				switch (Len)
				{
				case -4:
					flag = true;
					break;
				case -1:
					flag = false;
					num = -1;
					break;
				default:
				{
					string sQLState = ex.Errors[0].SQLState;
					if (sQLState != "01004")
					{
						throw ex;
					}
					flag = true;
					break;
				}
				}
			}
			else
			{
				flag = Len != -1;
				num = Len;
			}
			if (flag)
			{
				if (Len == -4)
				{
					int j;
					for (j = 0; array[j] != 0; j++)
					{
						buffer[bufferIndex + j] = array[j];
					}
					num = j;
				}
				else
				{
					int num2 = Math.Min(Len, length);
					for (int k = 0; k < num2; k++)
					{
						buffer[bufferIndex + k] = array[k];
					}
					num = num2;
				}
			}
			return num;
		}
		}
	}

	[System.MonoTODO]
	public override char GetChar(int i)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("The reader is closed.");
		}
		if (currentRow == -1)
		{
			throw new InvalidOperationException("No data available.");
		}
		if (i < 0 || i >= FieldCount)
		{
			throw new IndexOutOfRangeException();
		}
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private new IDataReader GetData(int i)
	{
		throw new NotImplementedException();
	}

	public override string GetDataTypeName(int i)
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("The reader is closed.");
		}
		if (i < 0 || i >= FieldCount)
		{
			throw new IndexOutOfRangeException();
		}
		return GetColumnAttributeStr(i + 1, FieldIdentifier.TypeName);
	}

	public DateTime GetDate(int i)
	{
		return GetDateTime(i);
	}

	public override DateTime GetDateTime(int i)
	{
		return (DateTime)GetValue(i);
	}

	public override decimal GetDecimal(int i)
	{
		return (decimal)GetValue(i);
	}

	public override double GetDouble(int i)
	{
		return (double)GetValue(i);
	}

	public override Type GetFieldType(int i)
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("The reader is closed.");
		}
		return GetColumn(i).DataType;
	}

	public override float GetFloat(int i)
	{
		return (float)GetValue(i);
	}

	[System.MonoTODO]
	public override Guid GetGuid(int i)
	{
		throw new NotImplementedException();
	}

	public override short GetInt16(int i)
	{
		return (short)GetValue(i);
	}

	public override int GetInt32(int i)
	{
		return (int)GetValue(i);
	}

	public override long GetInt64(int i)
	{
		return (long)GetValue(i);
	}

	public override string GetName(int i)
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("The reader is closed.");
		}
		return GetColumn(i).ColumnName;
	}

	public override int GetOrdinal(string value)
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("The reader is closed.");
		}
		if (value == null)
		{
			throw new ArgumentNullException("fieldName");
		}
		int num = ColIndex(value);
		if (num == -1)
		{
			throw new IndexOutOfRangeException();
		}
		return num;
	}

	[System.MonoTODO]
	public override DataTable GetSchemaTable()
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("The reader is closed.");
		}
		if (_dataTableSchema != null)
		{
			return _dataTableSchema;
		}
		DataTable dataTable = null;
		if (cols.Length > 0)
		{
			dataTable = new DataTable();
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("ColumnOrdinal", typeof(int));
			dataTable.Columns.Add("ColumnSize", typeof(int));
			dataTable.Columns.Add("NumericPrecision", typeof(int));
			dataTable.Columns.Add("NumericScale", typeof(int));
			dataTable.Columns.Add("IsUnique", typeof(bool));
			dataTable.Columns.Add("IsKey", typeof(bool));
			DataColumn dataColumn = dataTable.Columns["IsKey"];
			dataColumn.AllowDBNull = true;
			dataTable.Columns.Add("BaseCatalogName", typeof(string));
			dataTable.Columns.Add("BaseColumnName", typeof(string));
			dataTable.Columns.Add("BaseSchemaName", typeof(string));
			dataTable.Columns.Add("BaseTableName", typeof(string));
			dataTable.Columns.Add("DataType", typeof(Type));
			dataTable.Columns.Add("AllowDBNull", typeof(bool));
			dataTable.Columns.Add("ProviderType", typeof(int));
			dataTable.Columns.Add("IsAliased", typeof(bool));
			dataTable.Columns.Add("IsExpression", typeof(bool));
			dataTable.Columns.Add("IsIdentity", typeof(bool));
			dataTable.Columns.Add("IsAutoIncrement", typeof(bool));
			dataTable.Columns.Add("IsRowVersion", typeof(bool));
			dataTable.Columns.Add("IsHidden", typeof(bool));
			dataTable.Columns.Add("IsLong", typeof(bool));
			dataTable.Columns.Add("IsReadOnly", typeof(bool));
			for (int i = 0; i < cols.Length; i++)
			{
				OdbcColumn column = GetColumn(i);
				DataRow dataRow = dataTable.NewRow();
				dataTable.Rows.Add(dataRow);
				dataRow["ColumnName"] = column.ColumnName;
				dataRow["ColumnOrdinal"] = i;
				dataRow["ColumnSize"] = column.MaxLength;
				dataRow["NumericPrecision"] = GetColumnAttribute(i + 1, FieldIdentifier.Precision);
				dataRow["NumericScale"] = GetColumnAttribute(i + 1, FieldIdentifier.Scale);
				dataRow["BaseTableName"] = GetColumnAttributeStr(i + 1, FieldIdentifier.TableName);
				dataRow["BaseSchemaName"] = GetColumnAttributeStr(i + 1, FieldIdentifier.SchemaName);
				dataRow["BaseCatalogName"] = GetColumnAttributeStr(i + 1, FieldIdentifier.CatelogName);
				dataRow["BaseColumnName"] = GetColumnAttributeStr(i + 1, FieldIdentifier.BaseColumnName);
				dataRow["DataType"] = column.DataType;
				dataRow["IsUnique"] = false;
				dataRow["IsKey"] = DBNull.Value;
				dataRow["AllowDBNull"] = GetColumnAttribute(i + 1, FieldIdentifier.Nullable) != 0;
				dataRow["ProviderType"] = (int)column.OdbcType;
				dataRow["IsAutoIncrement"] = GetColumnAttribute(i + 1, FieldIdentifier.AutoUniqueValue) == 1;
				dataRow["IsExpression"] = dataRow.IsNull("BaseTableName") || (string)dataRow["BaseTableName"] == string.Empty;
				dataRow["IsAliased"] = (string)dataRow["BaseColumnName"] != (string)dataRow["ColumnName"];
				dataRow["IsReadOnly"] = (bool)dataRow["IsExpression"] || GetColumnAttribute(i + 1, FieldIdentifier.Updatable) == 0;
				dataRow["IsIdentity"] = false;
				dataRow["IsRowVersion"] = false;
				dataRow["IsHidden"] = false;
				dataRow["IsLong"] = false;
			}
			DataRow[] array = dataTable.Select("BaseTableName <> ''", "BaseCatalogName, BaseSchemaName, BaseTableName ASC");
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string[] array2 = null;
			DataRow[] array3 = array;
			foreach (DataRow dataRow2 in array3)
			{
				string text4 = (string)dataRow2["BaseTableName"];
				string text5 = (string)dataRow2["BaseSchemaName"];
				string text6 = (string)dataRow2["BaseCatalogName"];
				if (text4 != text || text5 != text2 || text6 != text3)
				{
					array2 = GetPrimaryKeys(text6, text5, text4);
				}
				if (array2 != null && Array.BinarySearch(array2, (string)dataRow2["BaseColumnName"]) >= 0)
				{
					dataRow2["IsKey"] = true;
					dataRow2["IsUnique"] = true;
					dataRow2["AllowDBNull"] = false;
					GetColumn(ColIndex((string)dataRow2["ColumnName"])).AllowDBNull = false;
				}
				text = text4;
				text2 = text5;
				text3 = text6;
			}
			dataTable.AcceptChanges();
		}
		return _dataTableSchema = dataTable;
	}

	public override string GetString(int i)
	{
		object value = GetValue(i);
		if (value != null && value.GetType() != typeof(string))
		{
			return Convert.ToString(value);
		}
		return (string)GetValue(i);
	}

	[System.MonoTODO]
	public TimeSpan GetTime(int i)
	{
		throw new NotImplementedException();
	}

	public override object GetValue(int i)
	{
		if (IsClosed)
		{
			throw new InvalidOperationException("The reader is closed.");
		}
		if (currentRow == -1)
		{
			throw new InvalidOperationException("No data available.");
		}
		if (i > cols.Length - 1 || i < 0)
		{
			throw new IndexOutOfRangeException();
		}
		int Len = 0;
		OdbcColumn column = GetColumn(i);
		object obj = null;
		ushort columnNumber = Convert.ToUInt16(i + 1);
		if (column.Value == null)
		{
			OdbcReturn odbcReturn;
			switch (column.OdbcType)
			{
			case OdbcType.Bit:
			{
				short TargetPtr6 = 0;
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, ref TargetPtr6, 0, ref Len);
				if (Len != -1)
				{
					obj = ((TargetPtr6 != 0) ? "True" : "False");
				}
				break;
			}
			case OdbcType.Decimal:
			case OdbcType.Numeric:
			{
				int num = 50;
				byte[] array = new byte[num];
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, SQL_C_TYPE.CHAR, array, num, ref Len);
				if (Len != -1)
				{
					byte[] array2 = new byte[Len];
					for (int j = 0; j < Len; j++)
					{
						array2[j] = array[j];
					}
					obj = decimal.Parse(Encoding.Default.GetString(array2), CultureInfo.InvariantCulture);
				}
				break;
			}
			case OdbcType.TinyInt:
			{
				short TargetPtr8 = 0;
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, ref TargetPtr8, 0, ref Len);
				obj = Convert.ToByte(TargetPtr8);
				break;
			}
			case OdbcType.Int:
			{
				int TargetPtr4 = 0;
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, ref TargetPtr4, 0, ref Len);
				obj = TargetPtr4;
				break;
			}
			case OdbcType.SmallInt:
			{
				short TargetPtr3 = 0;
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, ref TargetPtr3, 0, ref Len);
				obj = TargetPtr3;
				break;
			}
			case OdbcType.BigInt:
			{
				long TargetPtr2 = 0L;
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, ref TargetPtr2, 0, ref Len);
				obj = TargetPtr2;
				break;
			}
			case OdbcType.NChar:
			{
				int num = 255;
				byte[] array = new byte[num];
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, SQL_C_TYPE.WCHAR, array, num, ref Len);
				if (Len != -1 && (odbcReturn != OdbcReturn.SuccessWithInfo || Len != -4))
				{
					obj = Encoding.Unicode.GetString(array, 0, Len);
				}
				break;
			}
			case OdbcType.NText:
			case OdbcType.NVarChar:
			{
				int num = ((column.MaxLength >= 127) ? 255 : (column.MaxLength * 2 + 1));
				byte[] array = new byte[num];
				StringBuilder stringBuilder2 = new StringBuilder();
				do
				{
					odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, array, num, ref Len);
					if (odbcReturn == OdbcReturn.Error)
					{
						break;
					}
					if (odbcReturn == OdbcReturn.Success && Len == -1)
					{
						odbcReturn = OdbcReturn.NoData;
					}
					if (odbcReturn != OdbcReturn.NoData && Len > 0)
					{
						string text = null;
						text = ((Len >= num) ? Encoding.Unicode.GetString(array, 0, num) : Encoding.Unicode.GetString(array, 0, Len));
						stringBuilder2.Append(RemoveTrailingNullChar(text));
					}
				}
				while (odbcReturn != OdbcReturn.NoData);
				obj = stringBuilder2.ToString();
				break;
			}
			case OdbcType.Text:
			case OdbcType.VarChar:
			{
				int num = ((column.MaxLength >= 255) ? 255 : (column.MaxLength + 1));
				byte[] array = new byte[num];
				StringBuilder stringBuilder = new StringBuilder();
				do
				{
					odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, array, num, ref Len);
					if (odbcReturn == OdbcReturn.Error)
					{
						break;
					}
					if (odbcReturn == OdbcReturn.Success && Len == -1)
					{
						odbcReturn = OdbcReturn.NoData;
					}
					if (odbcReturn != OdbcReturn.NoData && Len > 0)
					{
						if (Len < num)
						{
							stringBuilder.Append(Encoding.Default.GetString(array, 0, Len));
						}
						else
						{
							stringBuilder.Append(Encoding.Default.GetString(array, 0, num - 1));
						}
					}
				}
				while (odbcReturn != OdbcReturn.NoData);
				obj = stringBuilder.ToString();
				break;
			}
			case OdbcType.Real:
			{
				float TargetPtr5 = 0f;
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, ref TargetPtr5, 0, ref Len);
				obj = TargetPtr5;
				break;
			}
			case OdbcType.Double:
			{
				double TargetPtr = 0.0;
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, ref TargetPtr, 0, ref Len);
				obj = TargetPtr;
				break;
			}
			case OdbcType.DateTime:
			case OdbcType.Timestamp:
			case OdbcType.Date:
			case OdbcType.Time:
			{
				OdbcTimestamp TargetPtr7 = default(OdbcTimestamp);
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, column.SqlCType, ref TargetPtr7, 0, ref Len);
				if (Len == -1)
				{
					break;
				}
				if (column.OdbcType == OdbcType.Time)
				{
					obj = new TimeSpan(TargetPtr7.year, TargetPtr7.month, TargetPtr7.day);
					break;
				}
				obj = new DateTime(TargetPtr7.year, TargetPtr7.month, TargetPtr7.day, TargetPtr7.hour, TargetPtr7.minute, TargetPtr7.second);
				if (TargetPtr7.fraction != 0L)
				{
					obj = ((DateTime)obj).AddTicks((long)TargetPtr7.fraction / 100L);
				}
				break;
			}
			case OdbcType.Image:
			case OdbcType.VarBinary:
			{
				int num = ((column.MaxLength >= 255 || column.MaxLength <= 0) ? 255 : column.MaxLength);
				byte[] array = new byte[num];
				ArrayList arrayList = new ArrayList();
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, SQL_C_TYPE.BINARY, array, 0, ref Len);
				if (Len != -1)
				{
					do
					{
						odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, SQL_C_TYPE.BINARY, array, num, ref Len);
						if (odbcReturn != OdbcReturn.Error && odbcReturn != OdbcReturn.NoData && Len != -1)
						{
							if (Len < num)
							{
								byte[] array3 = new byte[Len];
								Array.Copy(array, 0, array3, 0, Len);
								arrayList.AddRange(array3);
							}
							else
							{
								arrayList.AddRange(array);
							}
							continue;
						}
						break;
					}
					while (odbcReturn != OdbcReturn.NoData);
				}
				obj = arrayList.ToArray(typeof(byte));
				break;
			}
			case OdbcType.Binary:
			{
				int num = column.MaxLength;
				byte[] array = new byte[num];
				long bytes = GetBytes(i, 0L, array, 0, num);
				odbcReturn = OdbcReturn.Success;
				obj = array;
				break;
			}
			default:
			{
				int num = 255;
				byte[] array = new byte[num];
				odbcReturn = libodbc.SQLGetData(hstmt, columnNumber, SQL_C_TYPE.CHAR, array, num, ref Len);
				if (Len != -1 && (odbcReturn != OdbcReturn.SuccessWithInfo || Len != -4))
				{
					obj = Encoding.Default.GetString(array, 0, Len);
				}
				break;
			}
			}
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo && odbcReturn != OdbcReturn.NoData)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
			}
			if (Len == -1)
			{
				column.Value = DBNull.Value;
			}
			else
			{
				column.Value = obj;
			}
		}
		return column.Value;
	}

	public override int GetValues(object[] values)
	{
		int num = 0;
		if (IsClosed)
		{
			throw new InvalidOperationException("The reader is closed.");
		}
		if (currentRow == -1)
		{
			throw new InvalidOperationException("No data available.");
		}
		for (int i = 0; i < values.Length; i++)
		{
			if (i < FieldCount)
			{
				values[i] = GetValue(i);
			}
			else
			{
				values[i] = null;
			}
		}
		if (values.Length < FieldCount)
		{
			return values.Length;
		}
		if (values.Length == FieldCount)
		{
			return FieldCount;
		}
		return FieldCount;
	}

	public override IEnumerator GetEnumerator()
	{
		return new DbEnumerator(this);
	}

	protected override void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				Close();
			}
			command = null;
			cols = null;
			_dataTableSchema = null;
			disposed = true;
		}
	}

	public override bool IsDBNull(int i)
	{
		return GetValue(i) is DBNull;
	}

	public override bool NextResult()
	{
		OdbcReturn odbcReturn = OdbcReturn.Success;
		odbcReturn = libodbc.SQLMoreResults(hstmt);
		if (odbcReturn == OdbcReturn.Success)
		{
			short ColumnCount = 0;
			libodbc.SQLNumResultCols(hstmt, ref ColumnCount);
			cols = new OdbcColumn[ColumnCount];
			_dataTableSchema = null;
			GetColumns();
		}
		return odbcReturn == OdbcReturn.Success;
	}

	private bool NextRow()
	{
		OdbcReturn odbcReturn = libodbc.SQLFetch(hstmt);
		if (odbcReturn != 0)
		{
			currentRow = -1;
		}
		else
		{
			currentRow++;
		}
		OdbcColumn[] array = cols;
		foreach (OdbcColumn odbcColumn in array)
		{
			if (odbcColumn != null)
			{
				odbcColumn.Value = null;
			}
		}
		return odbcReturn == OdbcReturn.Success;
	}

	private int GetColumnAttribute(int column, FieldIdentifier fieldId)
	{
		OdbcReturn odbcReturn = OdbcReturn.Error;
		byte[] array = new byte[255];
		short strLengthPtr = 0;
		int numericAttributePtr = 0;
		odbcReturn = libodbc.SQLColAttribute(hstmt, (short)column, fieldId, array, (short)array.Length, ref strLengthPtr, ref numericAttributePtr);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw Connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
		}
		return numericAttributePtr;
	}

	private string GetColumnAttributeStr(int column, FieldIdentifier fieldId)
	{
		OdbcReturn odbcReturn = OdbcReturn.Error;
		byte[] array = new byte[255];
		short strLengthPtr = 0;
		int numericAttributePtr = 0;
		odbcReturn = libodbc.SQLColAttribute(hstmt, (short)column, fieldId, array, (short)array.Length, ref strLengthPtr, ref numericAttributePtr);
		if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
		{
			throw Connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
		}
		string result = string.Empty;
		if (strLengthPtr > 0)
		{
			result = Encoding.Unicode.GetString(array, 0, strLengthPtr);
		}
		return result;
	}

	private string[] GetPrimaryKeys(string catalog, string schema, string table)
	{
		if (cols.Length <= 0)
		{
			return new string[0];
		}
		ArrayList arrayList = null;
		try
		{
			arrayList = GetPrimaryKeysBySQLPrimaryKey(catalog, schema, table);
		}
		catch (OdbcException)
		{
			try
			{
				arrayList = GetPrimaryKeysBySQLStatistics(catalog, schema, table);
			}
			catch (OdbcException)
			{
			}
		}
		if (arrayList == null)
		{
			return null;
		}
		arrayList.Sort();
		return (string[])arrayList.ToArray(typeof(string));
	}

	private ArrayList GetPrimaryKeysBySQLPrimaryKey(string catalog, string schema, string table)
	{
		ArrayList arrayList = new ArrayList();
		IntPtr OutputHandlePtr = IntPtr.Zero;
		try
		{
			OdbcReturn odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Stmt, command.Connection.hDbc, ref OutputHandlePtr);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Dbc, Connection.hDbc);
			}
			odbcReturn = libodbc.SQLPrimaryKeys(OutputHandlePtr, catalog, -3, schema, -3, table, -3);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
			}
			int indicator = 0;
			byte[] array = new byte[255];
			odbcReturn = libodbc.SQLBindCol(OutputHandlePtr, 4, SQL_C_TYPE.CHAR, array, array.Length, ref indicator);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
			}
			while (true)
			{
				odbcReturn = libodbc.SQLFetch(OutputHandlePtr);
				if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					break;
				}
				string @string = Encoding.Default.GetString(array, 0, indicator);
				arrayList.Add(@string);
			}
			return arrayList;
		}
		finally
		{
			if (OutputHandlePtr != IntPtr.Zero)
			{
				OdbcReturn odbcReturn = libodbc.SQLFreeStmt(OutputHandlePtr, libodbc.SQLFreeStmtOptions.Close);
				if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
				}
				odbcReturn = libodbc.SQLFreeHandle(3, OutputHandlePtr);
				if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
				}
			}
		}
	}

	private ArrayList GetPrimaryKeysBySQLStatistics(string catalog, string schema, string table)
	{
		ArrayList arrayList = new ArrayList();
		IntPtr OutputHandlePtr = IntPtr.Zero;
		try
		{
			OdbcReturn odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Stmt, command.Connection.hDbc, ref OutputHandlePtr);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Dbc, Connection.hDbc);
			}
			odbcReturn = libodbc.SQLStatistics(OutputHandlePtr, catalog, -3, schema, -3, table, -3, 0, 0);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
			}
			int indicator = 0;
			short value = 0;
			odbcReturn = libodbc.SQLBindCol(OutputHandlePtr, 4, SQL_C_TYPE.SHORT, ref value, 2, ref indicator);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
			}
			int indicator2 = 0;
			byte[] array = new byte[255];
			odbcReturn = libodbc.SQLBindCol(OutputHandlePtr, 9, SQL_C_TYPE.CHAR, array, array.Length, ref indicator2);
			if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
			}
			while (true)
			{
				odbcReturn = libodbc.SQLFetch(OutputHandlePtr);
				if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					break;
				}
				if (value == 1)
				{
					string @string = Encoding.Default.GetString(array, 0, indicator2);
					arrayList.Add(@string);
					break;
				}
			}
		}
		finally
		{
			if (OutputHandlePtr != IntPtr.Zero)
			{
				OdbcReturn odbcReturn = libodbc.SQLFreeStmt(OutputHandlePtr, libodbc.SQLFreeStmtOptions.Close);
				if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
				}
				odbcReturn = libodbc.SQLFreeHandle(3, OutputHandlePtr);
				if (odbcReturn != 0 && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw Connection.CreateOdbcException(OdbcHandleType.Stmt, OutputHandlePtr);
				}
			}
		}
		return arrayList;
	}

	public override bool Read()
	{
		return NextRow();
	}

	private static string RemoveTrailingNullChar(string value)
	{
		return value.TrimEnd(default(char));
	}
}
