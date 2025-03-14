using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace System.Data.OleDb;

public sealed class OleDbDataReader : DbDataReader, IDisposable
{
	private OleDbCommand command;

	private bool open;

	private ArrayList gdaResults;

	private int currentResult;

	private int currentRow;

	private bool disposed;

	public override int Depth => 0;

	public override int FieldCount
	{
		get
		{
			if (currentResult < 0 || currentResult >= gdaResults.Count)
			{
				return 0;
			}
			return libgda.gda_data_model_get_n_columns((IntPtr)gdaResults[currentResult]);
		}
	}

	public override bool IsClosed => !open;

	public override object this[string name]
	{
		get
		{
			if (currentResult == -1)
			{
				throw new InvalidOperationException();
			}
			int num = libgda.gda_data_model_get_column_position((IntPtr)gdaResults[currentResult], name);
			if (num == -1)
			{
				throw new IndexOutOfRangeException();
			}
			return this[num];
		}
	}

	public override object this[int index] => GetValue(index);

	public override int RecordsAffected
	{
		get
		{
			if (currentResult < 0 || currentResult >= gdaResults.Count)
			{
				return 0;
			}
			int num = libgda.gda_data_model_get_n_rows((IntPtr)gdaResults[currentResult]);
			if (num > 0 && FieldCount > 0)
			{
				return -1;
			}
			return (FieldCount <= 0) ? num : (-1);
		}
	}

	[System.MonoTODO]
	public override bool HasRows
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	[System.MonoTODO]
	public override int VisibleFieldCount
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal OleDbDataReader(OleDbCommand command, ArrayList results)
	{
		this.command = command;
		open = true;
		if (results != null)
		{
			gdaResults = results;
		}
		else
		{
			gdaResults = new ArrayList();
		}
		currentResult = -1;
		currentRow = -1;
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
	}

	public override void Close()
	{
		for (int i = 0; i < gdaResults.Count; i++)
		{
			IntPtr obj = (IntPtr)gdaResults[i];
			libgda.FreeObject(obj);
		}
		gdaResults.Clear();
		gdaResults = null;
		open = false;
		currentResult = -1;
		currentRow = -1;
	}

	public override bool GetBoolean(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.Boolean)
		{
			throw new InvalidCastException();
		}
		return libgda.gda_value_get_boolean(intPtr);
	}

	public override byte GetByte(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.Tinyint)
		{
			throw new InvalidCastException();
		}
		return libgda.gda_value_get_tinyint(intPtr);
	}

	[System.MonoTODO]
	public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
	{
		throw new NotImplementedException();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public override char GetChar(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.Tinyint)
		{
			throw new InvalidCastException();
		}
		return (char)libgda.gda_value_get_tinyint(intPtr);
	}

	[System.MonoTODO]
	public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public new OleDbDataReader GetData(int ordinal)
	{
		throw new NotImplementedException();
	}

	protected override DbDataReader GetDbDataReader(int ordinal)
	{
		return GetData(ordinal);
	}

	public override string GetDataTypeName(int index)
	{
		if (currentResult == -1)
		{
			return "unknown";
		}
		IntPtr intPtr = libgda.gda_data_model_describe_column((IntPtr)gdaResults[currentResult], index);
		if (intPtr == IntPtr.Zero)
		{
			return "unknown";
		}
		GdaValueType type = libgda.gda_field_attributes_get_gdatype(intPtr);
		libgda.gda_field_attributes_free(intPtr);
		return libgda.gda_type_to_string(type);
	}

	public override DateTime GetDateTime(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) == GdaValueType.Date)
		{
			GdaDate gdaDate = (GdaDate)Marshal.PtrToStructure(libgda.gda_value_get_date(intPtr), typeof(GdaDate));
			return new DateTime(gdaDate.year, gdaDate.month, gdaDate.day);
		}
		if (libgda.gda_value_get_type(intPtr) == GdaValueType.Time)
		{
			GdaTime gdaTime = (GdaTime)Marshal.PtrToStructure(libgda.gda_value_get_time(intPtr), typeof(GdaTime));
			return new DateTime(0, 0, 0, gdaTime.hour, gdaTime.minute, gdaTime.second, 0);
		}
		if (libgda.gda_value_get_type(intPtr) == GdaValueType.Timestamp)
		{
			GdaTimestamp gdaTimestamp = (GdaTimestamp)Marshal.PtrToStructure(libgda.gda_value_get_timestamp(intPtr), typeof(GdaTimestamp));
			return new DateTime(gdaTimestamp.year, gdaTimestamp.month, gdaTimestamp.day, gdaTimestamp.hour, gdaTimestamp.minute, gdaTimestamp.second, (int)gdaTimestamp.fraction);
		}
		throw new InvalidCastException();
	}

	[System.MonoTODO]
	public override decimal GetDecimal(int ordinal)
	{
		throw new NotImplementedException();
	}

	public override double GetDouble(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.Double)
		{
			throw new InvalidCastException();
		}
		return libgda.gda_value_get_double(intPtr);
	}

	public override Type GetFieldType(int index)
	{
		if (currentResult == -1)
		{
			throw new IndexOutOfRangeException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], index, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new IndexOutOfRangeException();
		}
		return libgda.gda_value_get_type(intPtr) switch
		{
			GdaValueType.Bigint => typeof(long), 
			GdaValueType.Boolean => typeof(bool), 
			GdaValueType.Date => typeof(DateTime), 
			GdaValueType.Double => typeof(double), 
			GdaValueType.Integer => typeof(int), 
			GdaValueType.Single => typeof(float), 
			GdaValueType.Smallint => typeof(byte), 
			GdaValueType.String => typeof(string), 
			GdaValueType.Time => typeof(DateTime), 
			GdaValueType.Timestamp => typeof(DateTime), 
			GdaValueType.Tinyint => typeof(byte), 
			_ => typeof(string), 
		};
	}

	public override float GetFloat(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.Single)
		{
			throw new InvalidCastException();
		}
		return libgda.gda_value_get_single(intPtr);
	}

	[System.MonoTODO]
	public override Guid GetGuid(int ordinal)
	{
		throw new NotImplementedException();
	}

	public override short GetInt16(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.Smallint)
		{
			throw new InvalidCastException();
		}
		return (short)libgda.gda_value_get_smallint(intPtr);
	}

	public override int GetInt32(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.Integer)
		{
			throw new InvalidCastException();
		}
		return libgda.gda_value_get_integer(intPtr);
	}

	public override long GetInt64(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.Bigint)
		{
			throw new InvalidCastException();
		}
		return libgda.gda_value_get_bigint(intPtr);
	}

	public override string GetName(int index)
	{
		if (currentResult == -1)
		{
			return null;
		}
		return libgda.gda_data_model_get_column_title((IntPtr)gdaResults[currentResult], index);
	}

	public override int GetOrdinal(string name)
	{
		if (currentResult == -1)
		{
			throw new IndexOutOfRangeException();
		}
		for (int i = 0; i < FieldCount; i++)
		{
			if (GetName(i) == name)
			{
				return i;
			}
		}
		throw new IndexOutOfRangeException();
	}

	public override DataTable GetSchemaTable()
	{
		DataTable dataTable = null;
		if (FieldCount > 0)
		{
			long num = 0L;
			if (currentResult == -1)
			{
				return null;
			}
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
			for (int i = 0; i < FieldCount; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				IntPtr intPtr = libgda.gda_data_model_describe_column((IntPtr)gdaResults[currentResult], i);
				if (intPtr == IntPtr.Zero)
				{
					return null;
				}
				GdaValueType gdaValueType = libgda.gda_field_attributes_get_gdatype(intPtr);
				num = libgda.gda_field_attributes_get_defined_size(intPtr);
				libgda.gda_field_attributes_free(intPtr);
				dataRow["ColumnName"] = GetName(i);
				dataRow["ColumnOrdinal"] = i + 1;
				dataRow["ColumnSize"] = (int)num;
				dataRow["NumericPrecision"] = 0;
				dataRow["NumericScale"] = 0;
				dataRow["IsUnique"] = false;
				dataRow["IsKey"] = DBNull.Value;
				dataRow["BaseCatalogName"] = string.Empty;
				dataRow["BaseColumnName"] = GetName(i);
				dataRow["BaseSchemaName"] = string.Empty;
				dataRow["BaseTableName"] = string.Empty;
				dataRow["DataType"] = GetFieldType(i);
				dataRow["AllowDBNull"] = false;
				dataRow["ProviderType"] = (int)gdaValueType;
				dataRow["IsAliased"] = false;
				dataRow["IsExpression"] = false;
				dataRow["IsIdentity"] = false;
				dataRow["IsAutoIncrement"] = false;
				dataRow["IsRowVersion"] = false;
				dataRow["IsHidden"] = false;
				dataRow["IsLong"] = false;
				dataRow["IsReadOnly"] = false;
				dataRow.AcceptChanges();
				dataTable.Rows.Add(dataRow);
			}
		}
		return dataTable;
	}

	public override string GetString(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new InvalidCastException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidCastException();
		}
		if (libgda.gda_value_get_type(intPtr) != GdaValueType.String)
		{
			throw new InvalidCastException();
		}
		return libgda.gda_value_get_string(intPtr);
	}

	[System.MonoTODO]
	public TimeSpan GetTimeSpan(int ordinal)
	{
		throw new NotImplementedException();
	}

	public override object GetValue(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new IndexOutOfRangeException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new IndexOutOfRangeException();
		}
		return libgda.gda_value_get_type(intPtr) switch
		{
			GdaValueType.Bigint => GetInt64(ordinal), 
			GdaValueType.Boolean => GetBoolean(ordinal), 
			GdaValueType.Date => GetDateTime(ordinal), 
			GdaValueType.Double => GetDouble(ordinal), 
			GdaValueType.Integer => GetInt32(ordinal), 
			GdaValueType.Single => GetFloat(ordinal), 
			GdaValueType.Smallint => GetByte(ordinal), 
			GdaValueType.String => GetString(ordinal), 
			GdaValueType.Time => GetDateTime(ordinal), 
			GdaValueType.Timestamp => GetDateTime(ordinal), 
			GdaValueType.Tinyint => GetByte(ordinal), 
			_ => libgda.gda_value_stringify(intPtr), 
		};
	}

	[System.MonoTODO]
	public override int GetValues(object[] values)
	{
		throw new NotImplementedException();
	}

	public override IEnumerator GetEnumerator()
	{
		return new DbEnumerator(this);
	}

	public override bool IsDBNull(int ordinal)
	{
		if (currentResult == -1)
		{
			throw new IndexOutOfRangeException();
		}
		IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)gdaResults[currentResult], ordinal, currentRow);
		if (intPtr == IntPtr.Zero)
		{
			throw new IndexOutOfRangeException();
		}
		return libgda.gda_value_is_null(intPtr);
	}

	public override bool NextResult()
	{
		int num = currentResult + 1;
		if (num >= 0 && num < gdaResults.Count)
		{
			currentResult++;
			return true;
		}
		return false;
	}

	public override bool Read()
	{
		if (currentResult < 0 || currentResult >= gdaResults.Count)
		{
			return false;
		}
		currentRow++;
		if (currentRow < libgda.gda_data_model_get_n_rows((IntPtr)gdaResults[currentResult]))
		{
			return true;
		}
		return false;
	}

	private new void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				command = null;
				GC.SuppressFinalize(this);
			}
			if (gdaResults != null)
			{
				gdaResults.Clear();
				gdaResults = null;
			}
			if (open)
			{
				Close();
			}
			disposed = true;
		}
	}
}
